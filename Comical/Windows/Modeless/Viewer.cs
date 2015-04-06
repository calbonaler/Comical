using System;
using System.Drawing;

namespace Comical
{
	public partial class Viewer : WeifenLuo.WinFormsUI.Docking.DockContent
	{
		public Viewer() { InitializeComponent(); }

		protected override string GetPersistString() { return "Viewer"; }

		public string Description
		{
			get { return preMain.Description; }
			set { preMain.Description = value; }
		}

		public Image Image
		{
			get { return preMain.Image; }
			set { preMain.Image = value; }
		}

		public void SetImageAsync(string url)
		{
			Uri uri;
			if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
				SetImageAsync(uri);
		}

		public void SetImageAsync(Uri url) { preMain.LoadImageAsync(url); }

		void itmFitToWindow_CheckedChanged(object sender, EventArgs e) { preMain.StretchMode = itmFitToWindow.Checked ? Comical.Controls.PreviewerStretchMode.Uniform : Comical.Controls.PreviewerStretchMode.None; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (this.DockPanel == null)
				this.ExtendFrameIntoClientArea(true);
		}
	}
}
