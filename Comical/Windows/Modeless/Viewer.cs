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
			get => preMain.Image;
			set => preMain.SetImage(value);
		}

		void itmFitToWindow_CheckedChanged(object? sender, EventArgs e) => preMain.StretchMode = itmFitToWindow.Checked ? Comical.Controls.PreviewerStretchMode.Uniform : Comical.Controls.PreviewerStretchMode.None;
	}
}
