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
			PreviewBox.MouseWheel += OnPreviewBoxMouseWheel;
		}

		Point point1;
		Point point2;
		bool point2Move = true;
		Point shiftScale;
		bool leftMouseDown = false;
		Binary? image;
		Image? internalImage;

		Rectangle ImageBounds => internalImage != null ? new(default, internalImage.Size * (int)MagnifyRatioNumericUpDown.Value / 100) : default;

		Rectangle ClippedImageBounds => point1 == point2 ? ImageBounds : new Rectangle(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y), Math.Abs(point2.X - point1.X) + 1, Math.Abs(point2.Y - point1.Y) + 1);

		Rectangle UnmagnifiedClippedImageBounds
		{
			get
			{
				Debug.Assert(internalImage != null);
				if (point1 == point2)
					return new Rectangle(Point.Empty, internalImage.Size);
				var bounds = new Rectangle(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y), Math.Abs(point2.X - point1.X) + 1, Math.Abs(point2.Y - point1.Y) + 1);
				return new Rectangle(bounds.X * 100 / (int)MagnifyRatioNumericUpDown.Value, bounds.Y * 100 / (int)MagnifyRatioNumericUpDown.Value, bounds.Width * 100 / (int)MagnifyRatioNumericUpDown.Value, bounds.Height * 100 / (int)MagnifyRatioNumericUpDown.Value);
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

		void OnRecalculateRequested(object? sender, EventArgs e)
		{
			var magSz = ImageBounds.Size;
			PreviewHScrollBar.LargeChange = PreviewBox.ClientSize.Width;
			PreviewHScrollBar.Maximum = magSz.Width;
			PreviewHScrollBar.Visible = PreviewHScrollBar.Maximum > PreviewHScrollBar.LargeChange - 1;
			PreviewVScrollBar.LargeChange = PreviewBox.ClientSize.Height;
			PreviewVScrollBar.Maximum = magSz.Height;
			PreviewVScrollBar.Visible = PreviewVScrollBar.Maximum > PreviewVScrollBar.LargeChange - 1;
			PreviewBox.Invalidate();
		}

		void OnPreviewScrollBarsScroll(object? sender, ScrollEventArgs e)
		{
			PreviewBox.Invalidate();
			PreviewBox.Focus();
		}

		#region PreviewBox EventHandlers

		void OnPreviewBoxMouseDown(object? sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				point1 = point2 = GetVerifiedLocation(e.Location + new Size(PreviewHScrollBar.Value, PreviewVScrollBar.Value));
				leftMouseDown = true;
			}
		}

		void OnPreviewBoxMouseMove(object? sender, MouseEventArgs e)
		{
			if (leftMouseDown && e.Button == MouseButtons.Left)
			{
				point2 = GetVerifiedLocation(e.Location + new Size(PreviewHScrollBar.Value, PreviewVScrollBar.Value));
				PreviewBox.Invalidate();
			}
		}

		void OnPreviewBoxMouseUp(object? sender, MouseEventArgs e)
		{
			if (leftMouseDown && e.Button == MouseButtons.Left)
			{
				point2 = GetVerifiedLocation(e.Location + new Size(PreviewHScrollBar.Value, PreviewVScrollBar.Value));
				PreviewBox.Invalidate();
				leftMouseDown = false;
			}
		}

		void OnPreviewBoxPaint(object? sender, PaintEventArgs e)
		{
			Debug.Assert(internalImage != null);
			e.Graphics.TranslateTransform(-PreviewHScrollBar.Value, -PreviewVScrollBar.Value);
			e.Graphics.DrawImage(internalImage, ImageBounds);
			using (var reg = new Region(ImageBounds))
			{
				reg.Xor(ClippedImageBounds);
				SizeLabel.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture, Properties.Resources.ImageSizeStringRepresentation, ClippedImageBounds.Width, ClippedImageBounds.Height);
				e.Graphics.FillRegion(Brushes.Blue, reg);
			}
			DrawCross(e.Graphics, point2);
			DrawCross(e.Graphics, point1);
		}

		void OnPreviewBoxKeyDown(object? sender, KeyEventArgs e)
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
			PreviewBox.Invalidate();
		}

		void OnPreviewBoxKeyUp(object? sender, KeyEventArgs e)
		{
			if (e.KeyCode is Keys.Left or Keys.Right)
				shiftScale.X = 0;
			else if (e.KeyCode is Keys.Up or Keys.Down)
				shiftScale.Y = 0;
		}

		void OnPreviewBoxMouseLeave(object? sender, EventArgs e) => PreviewBox.Invalidate();

		void OnPreviewBoxMouseWheel(object? sender, MouseEventArgs e)
		{
			if ((ModifierKeys & Keys.Control) != 0)
				MagnifyRatioNumericUpDown.Value = RoundInteger((int)MagnifyRatioNumericUpDown.Value + SystemInformation.MouseWheelScrollLines * e.Delta / SystemInformation.MouseWheelScrollDelta, 1, 100);
			else if ((ModifierKeys & Keys.Shift) != 0)
				PreviewHScrollBar.Value = RoundInteger(PreviewHScrollBar.Value - 10 * SystemInformation.MouseWheelScrollLines * e.Delta / SystemInformation.MouseWheelScrollDelta, 0, PreviewHScrollBar.Maximum - PreviewHScrollBar.LargeChange + 1);
			else
				PreviewVScrollBar.Value = RoundInteger(PreviewVScrollBar.Value - 10 * SystemInformation.MouseWheelScrollLines * e.Delta / SystemInformation.MouseWheelScrollDelta, 0, PreviewVScrollBar.Maximum - PreviewVScrollBar.LargeChange + 1);
			PreviewBox.Invalidate();
		}

		static int RoundInteger(int value, int min, int max) => value < min ? min : value > max ? max : value;

		#endregion

		void OnOKButtonClick(object? sender, EventArgs e)
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
			PreviewHScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
			PreviewVScrollBar.Width = SystemInformation.VerticalScrollBarWidth;
			using (var ms = new System.IO.MemoryStream(Properties.Resources.Cross))
				PreviewBox.Cursor = new Cursor(ms);
			OnRecalculateRequested(null, EventArgs.Empty);
			PreviewBox.Select();
		}

		protected override void SetVisibleCore(bool value)
		{
			if (!Visible && value && image == null)
				throw new InvalidOperationException($"表示前に{nameof(Image)}にnull以外の値を設定してください。");
			base.SetVisibleCore(value);
		}
	}
}
