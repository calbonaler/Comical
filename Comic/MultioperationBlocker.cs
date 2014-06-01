using System;
using System.ComponentModel;

namespace Comical.Core
{
	internal class MultioperationBlocker : INotifyPropertyChanged
	{
		bool busy = false;

		public bool IsBusy
		{
			get { return busy; }
			set
			{
				if (busy != value)
				{
					busy = value;
					if (PropertyChanged != null)
						PropertyChanged(this, new PropertyChangedEventArgs("IsBusy"));
				}
			}
		}

		IDisposable leaver = null;

		public IDisposable Enter()
		{
			if (leaver != null)
				throw new InvalidOperationException(Properties.Resources.MultiAsyncOperationIsNotSupported);
			return new BlockerInstance(this);
		}

		class BlockerInstance : IDisposable
		{
			public BlockerInstance(MultioperationBlocker blocker)
			{
				this.blocker = blocker;
				blocker.IsBusy = true;
				blocker.leaver = this;
			}

			MultioperationBlocker blocker;

			public void Dispose()
			{
				if (blocker != null)
				{
					blocker.IsBusy = false;
					blocker.leaver = null;
					blocker = null;
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
