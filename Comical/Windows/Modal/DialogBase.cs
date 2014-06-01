using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Comical
{
	public partial class DialogBase : Form
	{
		public DialogBase()
		{
			InitializeComponent();
		}

		[Localizable(true)]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public string Description
		{
			get { return lblDescription.Text; }
			set { lblDescription.Text = value; }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (e != null)
			{
				if (Application.RenderWithVisualStyles)
					e.Graphics.FillRectangle(Brushes.White, 0, 0, ClientSize.Width, ClientSize.Height - 44);
				if (Application.RenderWithVisualStyles)
				{
					using (Pen pen = new Pen(Color.FromArgb(223, 223, 223)))
						e.Graphics.DrawLine(pen, 0, ClientSize.Height - 44, ClientSize.Width, ClientSize.Height - 44);
					using (SolidBrush sb = new SolidBrush(Color.FromArgb(240, 240, 240)))
						e.Graphics.FillRectangle(sb, 0, ClientSize.Height - 43, ClientSize.Width, 43);
				}
			}
		}
	}
}
