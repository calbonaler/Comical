using System;
using System.ComponentModel;
using Comical.Controls;
using Comical.Core;

namespace Comical;

public partial class Viewer : WeifenLuo.WinFormsUI.Docking.DockContent
{
	public Viewer() => InitializeComponent();

	protected override string GetPersistString() => "Viewer";

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Binary? Image
	{
		get;
		set
		{
			if (field == value) return;
			field = value;
			MainPreviewer.Image?.Dispose();
			MainPreviewer.Image = field?.ToImage();
		}
	}

	void OnSizeMenuItemsCheckedChanged(object? sender, EventArgs e)
		=> MainPreviewer.StretchMode = FitToWindowMenuItem.Checked ? PreviewerStretchMode.Uniform : PreviewerStretchMode.None;
}
