using System;
using Comical.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace Comical
{
	public partial class GoToIndexView : DockContent
	{
		public GoToIndexView() { InitializeComponent(); }

		protected override string GetPersistString() { return "GoToIndex"; }

		public event EventHandler Go
		{
			add { btnOK.Click += value; }
			remove { btnOK.Click -= value; }
		}

		public int Index
		{
			get { return (int)numIndex.Value; }
			set { numIndex.Value = value; }
		}

		ImageReferenceCollection cic;

		public void SetImages(ImageReferenceCollection collection)
		{
			if (cic != collection)
			{
				if (cic != null)
					cic.CollectionChanged -= ComicImageCollection_CollectionChanged;
				cic = collection;
				if (cic != null)
				{
					cic.CollectionChanged += ComicImageCollection_CollectionChanged;
					ComicImageCollection_CollectionChanged(cic, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
				}
			}
		}

		void ComicImageCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			lblIndex.Enabled = numIndex.Enabled = btnOK.Enabled = cic != null && cic.Count > 0;
			if (cic != null && cic.Count > 0)
				numIndex.Maximum = cic.Count - 1;
		}
	}
}
