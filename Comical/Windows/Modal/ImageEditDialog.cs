using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Comical.Core;

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
		bool leftMouseDown = false;
		Binary? image;
		Image? internalImage;

		Rectangle ImageBounds => internalImage != null ? new(default, internalImage.Size * (int)numMagnifyRatio.Value / 100) : default;

		Rectangle ClippedImageBounds => point1 == point2 ? ImageBounds : new Rectangle(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y), Math.Abs(point2.X - point1.X) + 1, Math.Abs(point2.Y - point1.Y) + 1);

		Rectangle UnmagnifiedClippedImageBounds
		{
			get
			{
				Debug.Assert(internalImage != null);
				if (point1 == point2)
					return new Rectangle(Point.Empty, internalImage.Size);
				var bounds = new Rectangle(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y), Math.Abs(point2.X - point1.X) + 1, Math.Abs(point2.Y - point1.Y) + 1);
				return new Rectangle(bounds.X * 100 / (int)numMagnifyRatio.Value, bounds.Y * 100 / (int)numMagnifyRatio.Value, bounds.Width * 100 / (int)numMagnifyRatio.Value, bounds.Height * 100 / (int)numMagnifyRatio.Value);
			}
		}

		[MaybeNull]
		public Binary Image
		{
			get => image;
			set
			{
				if (image == value) return;
				image = value;
				internalImage?.Dispose();
				internalImage = image.ToImage();
			}
		}

		static void DrawCross(Graphics g, Point center)
		{
			using var ms = new System.IO.MemoryStream(Properties.Resources.Cross);
			using var cursor = new Cursor(ms);
			cursor.Draw(g, new Rectangle(center - (Size)cursor.HotSpot, cursor.Size));
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

		void RecalculateRequested(object? sender, EventArgs e)
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

		void vsPreview_Scroll(object? sender, ScrollEventArgs e)
		{
			picPreview.Invalidate();
			picPreview.Focus();
		}

		void hsPreview_Scroll(object? sender, ScrollEventArgs e)
		{
			picPreview.Invalidate();
			picPreview.Focus();
		}

		#region picPreview EventHandlers

		void picPreview_MouseDown(object? sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				point1 = point2 = GetVerifiedLocation(e.Location + new Size(hsPreview.Value, vsPreview.Value));
				leftMouseDown = true;
			}
		}

		void picPreview_MouseMove(object? sender, MouseEventArgs e)
		{
			if (leftMouseDown && e.Button == MouseButtons.Left)
			{
				point2 = GetVerifiedLocation(e.Location + new Size(hsPreview.Value, vsPreview.Value));
				picPreview.Invalidate();
			}
		}

		void picPreview_MouseUp(object? sender, MouseEventArgs e)
		{
			if (leftMouseDown && e.Button == MouseButtons.Left)
			{
				point2 = GetVerifiedLocation(e.Location + new Size(hsPreview.Value, vsPreview.Value));
				picPreview.Invalidate();
				leftMouseDown = false;
			}
		}

		void picPreview_Paint(object? sender, PaintEventArgs e)
		{
			Debug.Assert(internalImage != null);
			e.Graphics.TranslateTransform(-hsPreview.Value, -vsPreview.Value);
			e.Graphics.DrawImage(internalImage, ImageBounds);
			using (var reg = new Region(ImageBounds))
			{
				reg.Xor(ClippedImageBounds);
				lblSize.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.ImageSizeStringRepresentation, ClippedImageBounds.Width, ClippedImageBounds.Height);
				e.Graphics.FillRegion(Brushes.Blue, reg);
			}
			DrawCross(e.Graphics, point2);
			DrawCross(e.Graphics, point1);
		}

		void picPreview_KeyDown(object? sender, KeyEventArgs e)
		{
			if (e.KeyCode is Keys.ShiftKey or Keys.Menu or Keys.ControlKey)
				return;
			else if (e.KeyCode == Keys.Enter)
			{
				point2Move = !point2Move;
				return;
			}
			var loc = point2Move ? point2 : point1;
			var dif = e.Control ? 10 : 1;
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

		void picPreview_KeyUp(object? sender, KeyEventArgs e)
		{
			if (e.KeyCode is Keys.Left or Keys.Right)
				shiftScale.X = 0;
			else if (e.KeyCode is Keys.Up or Keys.Down)
				shiftScale.Y = 0;
		}

		void picPreview_MouseLeave(object? sender, EventArgs e) => picPreview.Invalidate();

		void picPreview_MouseWheel(object? sender, MouseEventArgs e)
		{
			if ((ModifierKeys & Keys.Control) != 0)
				numMagnifyRatio.Value = RoundInteger((int)numMagnifyRatio.Value + SystemInformation.MouseWheelScrollLines * e.Delta / SystemInformation.MouseWheelScrollDelta, 1, 100);
			else if ((ModifierKeys & Keys.Shift) != 0)
				hsPreview.Value = RoundInteger(hsPreview.Value - 10 * SystemInformation.MouseWheelScrollLines * e.Delta / SystemInformation.MouseWheelScrollDelta, 0, hsPreview.Maximum - hsPreview.LargeChange + 1);
			else
				vsPreview.Value = RoundInteger(vsPreview.Value - 10 * SystemInformation.MouseWheelScrollLines * e.Delta / SystemInformation.MouseWheelScrollDelta, 0, vsPreview.Maximum - vsPreview.LargeChange + 1);
			picPreview.Invalidate();
		}

		static int RoundInteger(int value, int min, int max) => value < min ? min : value > max ? max : value;

		#endregion

		void btnOK_Click(object? sender, EventArgs e)
		{
			Debug.Assert(internalImage != null);
			using (var image = new Bitmap(ClippedImageBounds.Width, ClippedImageBounds.Height))
			{
				using (var g = Graphics.FromImage(image))
					g.DrawImage(internalImage, new Rectangle(Point.Empty, image.Size), UnmagnifiedClippedImageBounds, GraphicsUnit.Pixel);
				this.image = image.ToBinary(ImageFormat.Bmp);
			}
			DialogResult = DialogResult.OK;
			Close();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			hsPreview.Height = SystemInformation.HorizontalScrollBarHeight;
			vsPreview.Width = SystemInformation.VerticalScrollBarWidth;
			using (var ms = new System.IO.MemoryStream(Properties.Resources.Cross))
				picPreview.Cursor = new Cursor(ms);
			RecalculateRequested(null, EventArgs.Empty);
			picPreview.Select();
		}

		protected override void SetVisibleCore(bool value)
		{
			if (!Visible && value && image == null)
				throw new InvalidOperationException($"表示前に{nameof(Image)}にnull以外の値を設定してください。");
			base.SetVisibleCore(value);
		}
	}
}
