using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Comical.Core;

[Serializable]
[ComVisible(false)]
[DebuggerDisplay("Count = {Count}")]
public class SynchronizedObservableCollection<T> : IDisposable, IList<T>, IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged, INotifyCollectionItemPropertyChanged where T : INotifyPropertyChanged
{
	public SynchronizedObservableCollection() => _items = [];
	public SynchronizedObservableCollection(IEnumerable<T> collection) => _items = [.. collection];

	readonly List<T> _items;
	[NonSerialized] ReaderWriterLockSlim? _itemsLock = new();
	readonly SimpleMonitor _monitor = new();

	public bool IsDisposed => _itemsLock == null;

	public int Count { get { using (LockForRead()) return _items.Count; } }

	bool ICollection<T>.IsReadOnly => false;

	public T this[int index]
	{
		get { using (LockForRead()) return _items[index]; }
		set
		{
			T oldValue;
			using (LockForWrite())
			{
				CheckReentrancy();
				oldValue = _items[index];
				_items[index] = value;
			}
			oldValue.PropertyChanged -= OnItemPropertyChanged;
			value.PropertyChanged += OnItemPropertyChanged;
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue, index));
		}
	}

	void OnItemInserted(int index, T item)
	{
		item.PropertyChanged += OnItemPropertyChanged;
		OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
		OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
	}

	void OnItemsInserted(int index, T[] items)
	{
		foreach (var item in items)
			item.PropertyChanged += OnItemPropertyChanged;
		if (items.Length > 0)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, index));
		}
	}

	public void Add(T item)
	{
		int index;
		using (LockForWrite())
		{
			CheckReentrancy();
			index = _items.Count;
			_items.Insert(index, item);
		}
		OnItemInserted(index, item);
	}

	public void AddRange(IEnumerable<T> items)
	{
		int index;
		T[] addedItems;
		using (LockForWrite())
		{
			CheckReentrancy();
			index = _items.Count;
			_items.InsertRange(index, items);
			addedItems = new T[_items.Count - index];
			_items.CopyTo(index, addedItems, 0, addedItems.Length);
		}
		OnItemsInserted(index, addedItems);
	}

	protected IDisposable BlockReentrancy()
	{
		_monitor.Enter();
		return _monitor;
	}

	protected void CheckReentrancy()
	{
		if (_monitor.IsBusy && CollectionChanged != null && CollectionChanged.GetInvocationList().Length > 1)
			throw new InvalidOperationException(nameof(SynchronizedObservableCollection<T>) + " の再入は許可されていません。");
	}

	public void Clear()
	{
		using (LockForWrite())
		{
			CheckReentrancy();
			foreach (var item in _items)
				item.PropertyChanged -= OnItemPropertyChanged;
			_items.Clear();
			_items.TrimExcess();
		}
		OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
		OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
	}

	public bool Contains(T item) { using (LockForRead()) return _items.Contains(item); }

	public void CopyTo(T[] array, int arrayIndex) { using (LockForRead()) _items.CopyTo(array, arrayIndex); }

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing && _itemsLock != null)
		{
			_itemsLock.Dispose();
			_itemsLock = null;
		}
	}

	public IEnumerator<T> GetEnumerator() { using (LockForRead()) return _items.ToList().GetEnumerator(); }

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public int IndexOf(T item) { using (LockForRead()) return _items.IndexOf(item); }

	public void Insert(int index, T item)
	{
		using (LockForWrite())
		{
			CheckReentrancy();
			_items.Insert(index, item);
		}
		OnItemInserted(index, item);
	}

	[SuppressMessage("Performance", "CA1859", Justification = "内部実装を隠蔽し変更容易性を保つため")]
	IDisposable LockForRead()
	{
		ObjectDisposedException.ThrowIf(_itemsLock == null, this);
		_itemsLock.EnterReadLock();
		return new DelegateDisposable(_itemsLock.ExitReadLock);
	}

	[SuppressMessage("Performance", "CA1859", Justification = "内部実装を隠蔽し変更容易性を保つため")]
	IDisposable LockForWrite()
	{
		ObjectDisposedException.ThrowIf(_itemsLock == null, this);
		_itemsLock.EnterWriteLock();
		return new DelegateDisposable(_itemsLock.ExitWriteLock);
	}

	public void Move(int oldIndex, int newIndex) => MoveRange(oldIndex, 1, newIndex);

	public void MoveRange(int oldIndex, int count, int newIndex)
	{
		var movedItems = new T[count];
		using (LockForWrite())
		{
			CheckReentrancy();
			_items.CopyTo(oldIndex, movedItems, 0, count);
			_items.RemoveRange(oldIndex, count);
			_items.InsertRange(newIndex, movedItems);
		}
		OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, movedItems, newIndex, oldIndex));
	}

	protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e) { using (BlockReentrancy()) CollectionChanged?.Invoke(this, e); }

	protected virtual void OnCollectionItemPropertyChanged(CollectionItemPropertyChangedEventArgs e) => CollectionItemPropertyChanged?.Invoke(this, e);

	void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e) => OnCollectionItemPropertyChanged(new CollectionItemPropertyChangedEventArgs(sender, e.PropertyName));

	protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

	void OnItemRemoved(int index, T item)
	{
		item.PropertyChanged -= OnItemPropertyChanged;
		OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
		OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
		OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
	}

	public bool Remove(T item)
	{
		int index;
		using (LockForWrite())
		{
			if ((index = _items.IndexOf(item)) < 0)
				return false;
			CheckReentrancy();
			_items.RemoveAt(index);
		}
		OnItemRemoved(index, item);
		return true;
	}

	public void RemoveAt(int index)
	{
		T item;
		using (LockForWrite())
		{
			item = _items[index];
			CheckReentrancy();
			_items.RemoveAt(index);
		}
		OnItemRemoved(index, item);
	}

	public event NotifyCollectionChangedEventHandler? CollectionChanged;
	public event EventHandler<CollectionItemPropertyChangedEventArgs>? CollectionItemPropertyChanged;
	protected event PropertyChangedEventHandler? PropertyChanged;

	event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
	{
		add => PropertyChanged += value;
		remove => PropertyChanged -= value;
	}

	[Serializable]
	sealed class SimpleMonitor : IDisposable
	{
		int _busyCount;

		public bool IsBusy => _busyCount > 0;

		public void Enter() => ++_busyCount;

		public void Dispose() => --_busyCount;
	}
}
