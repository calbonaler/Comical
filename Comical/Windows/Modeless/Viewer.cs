using System;
using Comical.Core;

namespace Comical
{
	public partial class Viewer : WeifenLuo.WinFormsUI.Docking.DockContent
	{
		public Viewer() => InitializeComponent();

		protected override string GetPersistString() => "Viewer";

		public Binary? Image
		{
			get => MainPreviewer.Image;
			set => MainPreviewer.SetImage(value);
		}

		void OnSizeMenuItemsCheckedChanged(object? sender, EventArgs e) => MainPreviewer.StretchMode = FitToWindowMenuItem.Checked ? Comical.Controls.PreviewerStretchMode.Uniform : Comical.Controls.PreviewerStretchMode.None;
	}
}
