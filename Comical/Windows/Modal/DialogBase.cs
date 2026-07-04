using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Comical;

public partial class DialogBase : Form
{
	public DialogBase() => InitializeComponent();

	[Localizable(true)]
	[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
	public string Description
	{
		get => DescriptionLabel.Text;
		set => DescriptionLabel.Text = value;
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (e == null)
			return;
		if (Application.RenderWithVisualStyles)
		{
			var bottomAreaHeight = 43 * DeviceDpi / 96;
			e.Graphics.FillRectangle(Brushes.White, 0, 0, ClientSize.Width, ClientSize.Height - bottomAreaHeight - 1);
			using var pen = new Pen(Color.FromArgb(223, 223, 223));
			using var sb = new SolidBrush(Color.FromArgb(240, 240, 240));
			e.Graphics.DrawLine(pen, 0, ClientSize.Height - bottomAreaHeight - 1, ClientSize.Width, ClientSize.Height - bottomAreaHeight - 1);
			e.Graphics.FillRectangle(sb, 0, ClientSize.Height - bottomAreaHeight, ClientSize.Width, bottomAreaHeight);
		}
	}
}
