using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Comical
{
	public partial class DialogBase : Form
	{
		public DialogBase() => InitializeComponent();

		[Localizable(true)]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public string Description
		{
			get => lblDescription.Text;
			set => lblDescription.Text = value;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (e == null)
				return;
			if (Application.RenderWithVisualStyles)
			{
				e.Graphics.FillRectangle(Brushes.White, 0, 0, ClientSize.Width, ClientSize.Height - 44);
				using var pen = new Pen(Color.FromArgb(223, 223, 223));
				using var sb = new SolidBrush(Color.FromArgb(240, 240, 240));
				e.Graphics.DrawLine(pen, 0, ClientSize.Height - 44, ClientSize.Width, ClientSize.Height - 44);
				e.Graphics.FillRectangle(sb, 0, ClientSize.Height - 43, ClientSize.Width, 43);
			}
		}
	}
}
