using System;
using Comical.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace Comical
{
	public partial class ViewModeSettingsView : DockContent
	{
		public ViewModeSettingsView() { InitializeComponent(); }

		protected override string GetPersistString() => "ViewModeSettings";

		public int SelectionStart
		{
			get { return (int)numStartIndex.Value; }
			set { numStartIndex.Value = value; }
		}

		public int SelectionLength
		{
			get { return (int)numSelectionCount.Value; }
			set
			{
				if (value < numSelectionCount.Minimum)
					value = (int)numSelectionCount.Minimum;
				else if (value > numSelectionCount.Maximum)
					value = (int)numSelectionCount.Maximum;
				numSelectionCount.Value = value;
			}
		}

		public bool StartInLeft => radLeftStart.Checked;

		public void InvertViewMode()
		{
			foreach (var image in cic)
			{
				if (image.ViewMode == ImageViewMode.Left)
					image.ViewMode = ImageViewMode.Right;
				else if (image.ViewMode == ImageViewMode.Right)
					image.ViewMode = ImageViewMode.Left;
			}
		}

		ImageReferenceCollection cic;

		public void SetImages(ImageReferenceCollection collection)
		{
			if (cic == collection)
				return;
			if (cic != null)
				cic.CollectionChanged -= ComicImageCollection_CollectionChanged;
			cic = collection;
			if (cic != null)
			{
				cic.CollectionChanged += ComicImageCollection_CollectionChanged;
				ComicImageCollection_CollectionChanged(cic, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
			}
		}

		void ComicImageCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			lblStartIndex.Enabled = lblSelectionCount.Enabled = numStartIndex.Enabled = numSelectionCount.Enabled = btnSet.Enabled = cic != null && cic.Count > 0;
			if (cic != null && cic.Count > 0)
			{
				numStartIndex.Maximum = cic.Count - 1;
				numSelectionCount.Maximum = cic.Count - numStartIndex.Value;
			}
		}

		void numStartIndex_ValueChanged(object sender, EventArgs e) { numSelectionCount.Maximum = numStartIndex.Maximum - numStartIndex.Value + 1; }

		void btnSet_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < SelectionLength; i++)
			{
				if (i % 2 == (StartInLeft ? 0 : 1))
					cic[i + SelectionStart].ViewMode = ImageViewMode.Left;
				else
					cic[i + SelectionStart].ViewMode = ImageViewMode.Right;
			}
		}
	}
}
