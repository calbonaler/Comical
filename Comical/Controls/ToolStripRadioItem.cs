using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Comical.Controls
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.MenuStrip)]
	public partial class ToolStripRadioMenuItem : ToolStripMenuItem
	{
		static partial class NativeMethods
		{
			[LibraryImport("gdi32.dll")]
			private static partial uint GetPixel(IntPtr hdc, int x, int y);

			public static Color GetPixelColor(IDeviceContext dc, int xPos, int yPos)
			{
				try
				{
					var hdc = dc.GetHdc();
					var rgb = GetPixel(hdc, xPos, yPos);
					return Color.FromArgb((int)(rgb & 0xFF), (int)((rgb >> 8) & 0xFF), (int)((rgb >> 16) & 0xFF));
				}
				finally { dc.ReleaseHdc(); }
			}
		}

		public ToolStripRadioMenuItem() => CheckOnClick = true;

		protected override void OnCheckedChanged(EventArgs e)
		{
			base.OnCheckedChanged(e);
			if (!Checked || Owner == null)
				return;
			foreach (var item in Owner.Items)
			{
				if (item is ToolStripRadioMenuItem it && it != this && it.Group == Group)
					it.Checked = false;
			}
		}

		protected override void OnClick(EventArgs e)
		{
			if (Checked)
				return;
			base.OnClick(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (e == null || Image != null || !Checked)
				return;
			var rcBase = new Rectangle(ContentRectangle.X + 2, ContentRectangle.Y + 1, 18, ContentRectangle.Height - 2);
			using (var brush = new SolidBrush(NativeMethods.GetPixelColor(e.Graphics, rcBase.X, rcBase.Y)))
				e.Graphics.FillRectangle(brush, rcBase);
			var radioSize = new Size(7, 7);
			e.Graphics.FillEllipse(Brushes.Black, new Rectangle((rcBase.Width - radioSize.Width) / 2 + rcBase.X, (rcBase.Height - radioSize.Height) / 2 + rcBase.Y,
				radioSize.Width, radioSize.Height));
		}

		public int Group { get; set; }
	}
}
