using System;
using Comical.Controls;
using Comical.Core;

namespace Comical;

public partial class Viewer : WeifenLuo.WinFormsUI.Docking.DockContent
{
	public Viewer() => InitializeComponent();

	Binary? _image;

	protected override string GetPersistString() => "Viewer";

	public Binary? Image
	{
		get => _image;
		set
		{
			if (_image == value) return;
			_image = value;
			MainPreviewer.Image?.Dispose();
			MainPreviewer.Image = _image?.ToImage();
		}
	}

	void OnSizeMenuItemsCheckedChanged(object? sender, EventArgs e)
		=> MainPreviewer.StretchMode = FitToWindowMenuItem.Checked ? PreviewerStretchMode.Uniform : PreviewerStretchMode.None;
}
