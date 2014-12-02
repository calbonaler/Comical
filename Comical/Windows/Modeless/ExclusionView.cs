using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Comical.Core;
using CommonLibrary;

namespace Comical
{
	public partial class ExclusionView : WeifenLuo.WinFormsUI.Docking.DockContent
	{
		public ExclusionView()
		{
			InitializeComponent();
			dgvTrashBox.Font = new System.Drawing.Font("Meiryo", 9);
		}

		ImageReferenceCollection _images;
		static readonly Size trashThumSize = new Size(96, 96);

		public event EventHandler ImageReferenceSelected
		{
			add { dgvTrashBox.SelectionChanged += value; }
			remove { dgvTrashBox.SelectionChanged -= value; }
		}

		public IEnumerable<int> SelectedIndicies { get { return dgvTrashBox.SelectedRows.Cast<DataGridViewRow>().Select(row => row.Index); } }

		public void SetImages(ImageReferenceCollection images)
		{
			if (this._images != images)
			{
				if (this._images != null)
					this._images.CollectionChanged -= Images_CollectionChanged;
				this._images = images;
				if (images != null)
				{
					images.CollectionChanged += Images_CollectionChanged;
					Images_CollectionChanged(images, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		void Images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { dgvTrashBox.RowCount = _images.ExcludedItems.Count; }

		protected override string GetPersistString() { return "TrashBox"; }

		private void dgvTrashBox_CellValueNeeded(object sender, System.Windows.Forms.DataGridViewCellValueEventArgs e)
		{
			if (dgvTrashBox.Columns[e.ColumnIndex].Name == "clmImage")
			{
				if (e.RowIndex >= 0 && e.RowIndex < _images.ExcludedItems.Count)
					e.Value = _images.ExcludedItems[e.RowIndex].GetImage(trashThumSize);
			}
		}

		private void dgvTrashBox_QueryActualDestination(object sender, Controls.QueryActualDestinationEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
			if (e.Source.Name == "dgvImages")
				e.ActualDestination = dgvTrashBox.RowCount;
			else
				e.ActualDestination = -1;
		}

		private void dgvTrashBox_RowMoving(object sender, Controls.RowMovingEventArgs e)
		{
			if (e.Source.Name == "dgvImages")
			{
				e.Cancel = true;
				foreach (var item in e.SourceRows)
					_images.ExcludeAt(item.Index);
			}
		}
	}
}
