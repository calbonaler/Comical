using System;
using System.Drawing;
using System.Windows.Forms;

namespace Comical
{
	public partial class ImageEditingDialog : DialogBase
	{
		public ImageEditingDialog()
		{
			InitializeComponent();
			picPreview.MouseWheel += picPreview_MouseWheel;
		}
		
		[Flags]
		enum EditingMode
		{
			None = 0,
			TopLeft = 1,
			BottomRight = 2,
		}

		[Flags]
		enum KeyMode
		{
			None = 0,
			Left = 1,
			Right = 2,
			Up = 4,
			Down = 8,
			Horizontal = 3,
			Vertical = 12,
		}

		struct DrawableRectangle
		{
			public int Left { get; set; }

			public int Top { get; set; }

			public int Right { get; set; }

			public int Bottom { get; set; }

			public Point TopLeft
			{
				get { return new Point(Left, Top); }
				set
				{
					Left = value.X;
					Top = value.Y;
				}
			}

			public Point BottomRight
			{
				get { return new Point(Right, Bottom); }
				set
				{
					Right = value.X;
					Bottom = value.Y;
				}
			}

			public Point GetPoint(EditingMode mode) { return mode == EditingMode.TopLeft ? TopLeft : BottomRight; }

			public void SetPoint(EditingMode mode, Point pt)
			{
				if (mode.HasFlag(EditingMode.TopLeft))
					TopLeft = pt;
				if (mode.HasFlag(EditingMode.BottomRight))
					BottomRight = pt;
			}

			public Rectangle ToRectangle(Rectangle def)
			{
				if (TopLeft != BottomRight)
				{
					int l = Math.Min(Left, Right);
					int t = Math.Min(Top, Bottom);
					return new Rectangle(l, t, Math.Abs(Right - Left) + 1, Math.Abs(Bottom - Top) + 1);
				}
				return def;
			}
		}

		Image original;
		DrawableRectangle rectangle = new DrawableRectangle();
		EditingMode mode = EditingMode.BottomRight;
		KeyMode key = KeyMode.None;

		Rectangle ImageBounds { get { return rectangle.ToRectangle(new Rectangle(Point.Empty, GetMagnifiedSize((int)numMagnifyRatio.Value))); } }

		Size GetMagnifiedSize(int ratio) { return new Size(original.Width * ratio / 100, original.Height * ratio / 100); }

		static Rectangle UnmagnifyRectangle(Rectangle src, int magnify) { return new Rectangle(src.X * 100 / magnify, src.Y * 100 / magnify, src.Width * 100 / magnify, src.Height * 100 / magnify); }

		public Image Image
		{
			get { return original; }
			set { original = value; }
		}

		static void DrawCross(Graphics g, Point center)
		{
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Properties.Resources.Cross))
			using (Cursor cursor = new Cursor(ms))
				cursor.Draw(g, new Rectangle(center.X - 15, center.Y - 15, 32, 32));
		}

		Point GetVerifiedLocation(Point loc)
		{
			var sz = GetMagnifiedSize((int)numMagnifyRatio.Value);
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
			var magSz = GetMagnifiedSize((int)numMagnifyRatio.Value);
			hsPreview.LargeChange = picPreview.ClientSize.Width;
			hsPreview.Maximum = magSz.Width;
			vsPreview.LargeChange = picPreview.ClientSize.Height;
			vsPreview.Maximum = magSz.Height;
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

		void picPreview_MouseDown(object sender, MouseEventArgs e) { rectangle.TopLeft = rectangle.BottomRight = new Point(e.X + hsPreview.Value, e.Y + vsPreview.Value); }

		void picPreview_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				rectangle.BottomRight = GetVerifiedLocation(new Point(e.X + hsPreview.Value, e.Y + vsPreview.Value));
			picPreview.Invalidate();
		}

		void picPreview_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				rectangle.BottomRight = GetVerifiedLocation(new Point(e.X + hsPreview.Value, e.Y + vsPreview.Value));
				picPreview.Invalidate();
			}
		}

		void picPreview_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.TranslateTransform(-hsPreview.Value, -vsPreview.Value);
			var rect = new Rectangle(Point.Empty, GetMagnifiedSize((int)numMagnifyRatio.Value));
			e.Graphics.DrawImage(original, rect);
			using (Region reg = new Region(rect))
			{
				reg.Xor(ImageBounds);
				lblSize.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.ImageSizeStringRepresentation, ImageBounds.Width, ImageBounds.Height);
				e.Graphics.FillRegion(Brushes.Blue, reg);
			}
			DrawCross(e.Graphics, rectangle.BottomRight);
			DrawCross(e.Graphics, rectangle.TopLeft);
		}

		void picPreview_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu || e.KeyCode == Keys.ControlKey)
				return;
			else if (e.KeyCode == Keys.Enter)
			{
				if (mode == EditingMode.TopLeft)
					mode = EditingMode.BottomRight;
				else
					mode = EditingMode.TopLeft;
				return;
			}
			Point loc = rectangle.GetPoint(mode);
			int dif = e.Control ? 10 : 1;
			if (e.KeyCode == Keys.Left)
				key |= KeyMode.Left;
			else if (e.KeyCode == Keys.Right)
				key |= KeyMode.Right;
			else if (e.KeyCode == Keys.Up)
				key |= KeyMode.Up;
			else if (e.KeyCode == Keys.Down)
				key |= KeyMode.Down;
			loc.Offset(
				(key & KeyMode.Horizontal) != 0 ? (key.HasFlag(KeyMode.Left) ? -dif : dif) : 0,
				(key & KeyMode.Vertical) != 0 ? (key.HasFlag(KeyMode.Up) ? -dif : dif) : 0);
			loc = GetVerifiedLocation(loc);
			rectangle.SetPoint(e.Shift ? EditingMode.TopLeft | EditingMode.BottomRight : mode, loc);
			picPreview.Invalidate();
		}

		void picPreview_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Left)
				key &= ~KeyMode.Left;
			else if (e.KeyCode == Keys.Right)
				key &= ~KeyMode.Right;
			else if (e.KeyCode == Keys.Up)
				key &= ~KeyMode.Up;
			else if (e.KeyCode == Keys.Down)
				key &= ~KeyMode.Down;
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
				image = new Bitmap(ImageBounds.Width, ImageBounds.Height);
				using (Graphics g = Graphics.FromImage(image))
					g.DrawImage(original, new Rectangle(0, 0, image.Width, image.Height), UnmagnifyRectangle(ImageBounds, (int)numMagnifyRatio.Value), GraphicsUnit.Pixel);
				original = image;
				image = null;
			}
			finally
			{
				if (image != null)
					image.Dispose();
			}
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
