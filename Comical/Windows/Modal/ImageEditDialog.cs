using System;
using System.Drawing;
using System.Windows.Forms;

namespace Comical
{
	public partial class ImageEditDialog : DialogBase
	{
		public ImageEditDialog()
		{
			InitializeComponent();
			picPreview.MouseWheel += picPreview_MouseWheel;
		}

		Point point1;
		Point point2;
		bool point2Move = true;
		Point shiftScale;

		Rectangle ImageBounds => new Rectangle(0, 0, Image.Width * (int)numMagnifyRatio.Value / 100, Image.Height * (int)numMagnifyRatio.Value / 100);

		Rectangle ClippedImageBounds => point1 == point2 ? ImageBounds : new Rectangle(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y), Math.Abs(point2.X - point1.X) + 1, Math.Abs(point2.Y - point1.Y) + 1);

		Rectangle UnmagnifiedClippedImageBounds
		{
			get
			{
				if (point1 == point2)
					return new Rectangle(Point.Empty, Image.Size);
				var bounds = new Rectangle(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y), Math.Abs(point2.X - point1.X) + 1, Math.Abs(point2.Y - point1.Y) + 1);
				return new Rectangle(bounds.X * 100 / (int)numMagnifyRatio.Value, bounds.Y * 100 / (int)numMagnifyRatio.Value, bounds.Width * 100 / (int)numMagnifyRatio.Value, bounds.Height * 100 / (int)numMagnifyRatio.Value);
			}
		}

		public Image Image { get; set; }

		static void DrawCross(Graphics g, Point center)
		{
			System.IO.MemoryStream ms = null;
			try
			{
				ms = new System.IO.MemoryStream(Properties.Resources.Cross);
				using (Cursor cursor = new Cursor(ms))
				{
					ms = null;
					cursor.Draw(g, new Rectangle(center.X - 15, center.Y - 15, 32, 32));
				}
			}
			finally
			{
				if (ms != null)
					ms.Dispose();
			}
		}

		Point GetVerifiedLocation(Point loc)
		{
			var sz = ImageBounds.Size;
			if (loc.X < 0)
				loc.X = 0;
			else if (loc.X > sz.Width - 1)
				loc.X = sz.Width - 1;
			if (loc.Y < 0)
				loc.Y = 0;
			else if (loc.Y > sz.Height - 1)
				loc.Y = sz.Height - 1;
			return loc;
		}

		void RecalculateRequested(object sender, EventArgs e)
		{
			var magSz = ImageBounds.Size;
			hsPreview.LargeChange = picPreview.ClientSize.Width;
			hsPreview.Maximum = magSz.Width;
			hsPreview.Visible = hsPreview.Maximum > hsPreview.LargeChange - 1;
			vsPreview.LargeChange = picPreview.ClientSize.Height;
			vsPreview.Maximum = magSz.Height;
			vsPreview.Visible = vsPreview.Maximum > vsPreview.LargeChange - 1;
			picPreview.Invalidate();
		}

		void vsPreview_Scroll(object sender, ScrollEventArgs e)
		{
			picPreview.Invalidate();
			picPreview.Focus();
		}

		void hsPreview_Scroll(object sender, ScrollEventArgs e)
		{
			picPreview.Invalidate();
			picPreview.Focus();
		}

		#region picPreview EventHandlers

		void picPreview_MouseDown(object sender, MouseEventArgs e) { point1 = point2 = e.Location + new Size(hsPreview.Value, vsPreview.Value); }

		void picPreview_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				point2 = GetVerifiedLocation(e.Location + new Size(hsPreview.Value, vsPreview.Value));
				picPreview.Invalidate();
			}
		}

		void picPreview_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				point2 = GetVerifiedLocation(e.Location + new Size(hsPreview.Value, vsPreview.Value));
				picPreview.Invalidate();
			}
		}

		void picPreview_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.TranslateTransform(-hsPreview.Value, -vsPreview.Value);
			e.Graphics.DrawImage(Image, ImageBounds);
			using (Region reg = new Region(ImageBounds))
			{
				reg.Xor(ClippedImageBounds);
				lblSize.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.ImageSizeStringRepresentation, ClippedImageBounds.Width, ClippedImageBounds.Height);
				e.Graphics.FillRegion(Brushes.Blue, reg);
			}
			DrawCross(e.Graphics, point2);
			DrawCross(e.Graphics, point1);
		}

		void picPreview_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu || e.KeyCode == Keys.ControlKey)
				return;
			else if (e.KeyCode == Keys.Enter)
			{
				point2Move = !point2Move;
				return;
			}
			var loc = point2Move ? point2 : point1;
			int dif = e.Control ? 10 : 1;
			if (e.KeyCode == Keys.Left)
				shiftScale.X = -1;
			else if (e.KeyCode == Keys.Right)
				shiftScale.X = +1;
			else if (e.KeyCode == Keys.Up)
				shiftScale.Y = -1;
			else if (e.KeyCode == Keys.Down)
				shiftScale.Y = +1;
			loc = GetVerifiedLocation(loc + new Size(shiftScale.X * dif, shiftScale.Y * dif));
			if (e.Shift)
				point1 = point2 = loc;
			else if (point2Move)
				point2 = loc;
			else
				point1 = loc;
			picPreview.Invalidate();
		}

		void picPreview_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
				shiftScale.X = 0;
			else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
				shiftScale.Y = 0;
		}

		void picPreview_MouseLeave(object sender, EventArgs e) { picPreview.Invalidate(); }

		void picPreview_MouseWheel(object sender, MouseEventArgs e)
		{
			var scPt = picPreview.PointToScreen(e.Location);
			if (numMagnifyRatio.ClientRectangle.Contains(numMagnifyRatio.PointToClient(scPt)))
				numMagnifyRatio.Value = RoundInteger((int)numMagnifyRatio.Value + e.Delta / SystemInformation.MouseWheelScrollDelta, 0, 100);
			else if (hsPreview.ClientRectangle.Contains(hsPreview.PointToClient(scPt)))
				hsPreview.Value = RoundInteger(hsPreview.Value - e.Delta, 0, hsPreview.Maximum - hsPreview.LargeChange + 1);
			else
				vsPreview.Value = RoundInteger(vsPreview.Value - e.Delta, 0, vsPreview.Maximum - vsPreview.LargeChange + 1);
			picPreview.Invalidate();
		}

		static int RoundInteger(int value, int min, int max)
		{
			if (value < min)
				return min;
			if (value > max)
				return max;
			return value;
		}

		#endregion

		void btnOK_Click(object sender, EventArgs e)
		{
			Image image = null;
			try
			{
				image = new Bitmap(ClippedImageBounds.Width, ClippedImageBounds.Height);
				using (Graphics g = Graphics.FromImage(image))
					g.DrawImage(Image, new Rectangle(Point.Empty, image.Size), UnmagnifiedClippedImageBounds, GraphicsUnit.Pixel);
				Image = image;
				image = null;
			}
			finally { image?.Dispose(); }
			DialogResult = DialogResult.OK;
			Close();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Properties.Resources.Cross))
				picPreview.Cursor = new Cursor(ms);
			RecalculateRequested(null, null);
			picPreview.Select();
		}
	}
}
