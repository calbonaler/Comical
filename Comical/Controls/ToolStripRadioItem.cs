using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Comical.Controls
{
	[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.MenuStrip)]
	public class ToolStripRadioMenuItem : ToolStripMenuItem
	{
		static class NativeMethods
		{
			[DllImport("gdi32.dll")]
			public static extern uint GetPixel(IntPtr hdc, int x, int y);

			public static Color GetPixelColor(IntPtr hdc, int xPos, int yPos)
			{
				uint rgb = GetPixel(hdc, xPos, yPos);
				return Color.FromArgb((int)(rgb & 0xFF), (int)((rgb >> 8) & 0xFF), (int)((rgb >> 16) & 0xFF));
			}
		}

		public ToolStripRadioMenuItem() : base() { Initialize(); }

		void Initialize() { CheckOnClick = true; }

		protected override void OnCheckedChanged(EventArgs e)
		{
			base.OnCheckedChanged(e);
			if (!Checked || Owner == null)
				return;
			foreach (ToolStripItem item in Owner.Items)
			{
				ToolStripRadioMenuItem it = item as ToolStripRadioMenuItem;
				if (it != null && it != this && it.Group == Group)
					it.Checked = false;
			}
		}

		protected override void OnClick(EventArgs e)
		{
			if (Checked)
				return;
			base.OnClick(e);
		}

		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (e == null || Image != null || !Checked)
				return;
			Rectangle rcBase = new Rectangle(ContentRectangle.X + 2, ContentRectangle.Y + 1, 18, ContentRectangle.Height - 2);
			Color rgb = NativeMethods.GetPixelColor(e.Graphics.GetHdc(), rcBase.X, rcBase.Y);
			e.Graphics.ReleaseHdc();
			using (var brush = new SolidBrush(rgb))
				e.Graphics.FillRectangle(brush, rcBase);
			Size radioSize = new Size(7, 7);
			e.Graphics.FillEllipse(Brushes.Black, new Rectangle((rcBase.Width - radioSize.Width) / 2 + rcBase.X, (rcBase.Height - radioSize.Height) / 2 + rcBase.Y,
				radioSize.Width, radioSize.Height));
		}

		public int Group { get; set; }
	}
}
