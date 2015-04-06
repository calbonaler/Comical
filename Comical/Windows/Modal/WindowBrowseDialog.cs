using System;
using System.Windows.Forms;

namespace Comical
{
	public partial class WindowBrowseDialog : DialogBase
	{
		public WindowBrowseDialog()
		{
			InitializeComponent();
			WindowFilter = (window) => true;
			lvWindows.SetWindow("Explorer");
		}

		#region PublicProperties

		public Func<Window, bool> WindowFilter { get; set; }

		public Window SelectedWindow { get; set; }

		#endregion

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			foreach (var window in Window.TopLevels(WindowFilter))
			{
				var icon = window.GetIcon(true);
				try
				{
					if (icon != null)
						ilIcons.Images.Add(window.Handle.ToString(), icon);
				}
				catch (InvalidOperationException) { }
				var item = lvWindows.Items.Add(string.IsNullOrEmpty(window.Text) ? window.ClassName : window.Text, window.Handle.ToString());
				item.Tag = window;
			}
		}

		void btnOK_Click(object sender, EventArgs e)
		{
			if (lvWindows.SelectedItems.Count > 0)
			{
				SelectedWindow = lvWindows.SelectedItems[0].Tag as Window;
				DialogResult = System.Windows.Forms.DialogResult.OK;
			}
		}
	}
}
