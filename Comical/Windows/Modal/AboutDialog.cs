using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Comical
{
	public partial class AboutDialog : Form
	{
		public AboutDialog()
		{
			InitializeComponent();
			using (GraphicsPath path = new GraphicsPath(FillMode.Winding))
			{
				const int round = 60;
				path.AddArc(0, 0, round, round, 180, 90);
				path.AddLine(round, 0, Width - round, 0);
				path.AddArc(Width - round, 0, round, round, 270, 90);
				path.AddLine(Width, round, Width, Height - round);
				path.AddArc(Width - round, Height - round, round, round, 0, 90);
				path.AddLine(Width - round, Height, round, Height);
				path.AddArc(0, Height - round, round, round, 90, 90);
				path.AddLine(0, Height - round, 0, round);
				path.CloseFigure();
				this.Region = new Region(path);
			}
		}

		#region SplashScreen EventHandlers

		void SplashScreen_Click(object sender, System.EventArgs e) { Close(); }

		protected override void OnLoad(System.EventArgs e)
		{
			base.OnLoad(e);
			var ca = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).FirstOrDefault() as AssemblyCopyrightAttribute;
			lblVersionHeader.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
			lblCopyright.Text = ca != null ? ca.Copyright : "";
		}

		#endregion
	}
}
