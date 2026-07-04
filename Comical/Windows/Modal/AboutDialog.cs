using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Comical.Properties;

namespace Comical;

public partial class AboutDialog : Form
{
	public AboutDialog()
	{
		InitializeComponent();
		using var path = new GraphicsPath(FillMode.Winding);
		var round = 60 * DeviceDpi / 96;
		path.AddArc(0, 0, round, round, 180, 90);
		path.AddLine(round, 0, Width - round, 0);
		path.AddArc(Width - round, 0, round, round, 270, 90);
		path.AddLine(Width, round, Width, Height - round);
		path.AddArc(Width - round, Height - round, round, round, 0, 90);
		path.AddLine(Width - round, Height, round, Height);
		path.AddArc(0, Height - round, round, round, 90, 90);
		path.AddLine(0, Height - round, 0, round);
		path.CloseFigure();
		Region = new Region(path);
	}

	#region SplashScreen EventHandlers

	void OnClick(object? sender, System.EventArgs e) => Close();

	protected override void OnLoad(System.EventArgs e)
	{
		base.OnLoad(e);
		var version = Assembly.GetExecutingAssembly().GetName().Version;
		Debug.Assert(version != null);
		lblVersionHeader.Text = version.ToString(3);
#pragma warning disable CA1863 // リソースに対してCompositeFormatは使用できない
		CopyrightLabel.Text = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).FirstOrDefault() is AssemblyCopyrightAttribute ca
			? string.Format(CultureInfo.CurrentCulture, Resources.AboutDialog_CopyrightFormat, ca.Copyright)
			: string.Empty;
#pragma warning restore CA1863
		LibrariesTextBox.Select(0, 0);
	}

	#endregion
}
