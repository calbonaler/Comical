using System;
using System.Drawing;

namespace Comical
{
	public partial class Viewer : WeifenLuo.WinFormsUI.Docking.DockContent
	{
		public Viewer() => InitializeComponent();

		protected override string GetPersistString() => "Viewer";

		public Image Image
		{
			get { return preMain.Image; }
			set { preMain.Image = value; }
		}

		void itmFitToWindow_CheckedChanged(object sender, EventArgs e) => preMain.StretchMode = itmFitToWindow.Checked ? Comical.Controls.PreviewerStretchMode.Uniform : Comical.Controls.PreviewerStretchMode.None;
	}
}
