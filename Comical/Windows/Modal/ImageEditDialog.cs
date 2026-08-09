using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using Comical.Core;
using Comical.Properties;

namespace Comical;

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
	bool leftMouseDown;
	Binary? image;
	Image? internalImage;

	(Rectangle ClipBounds, Size ScaledSize) ImageClipBoundsAndScaledSize
	{
		get
		{
			Debug.Assert(internalImage != null);
			var clipRect =
				point1 == point2 ? new Rectangle(default, internalImage.Size) :
				new Rectangle(
					Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y),
					Math.Abs(point2.X - point1.X) + 1, Math.Abs(point2.Y - point1.Y) + 1
				);
			return (clipRect, clipRect.Size * (int)MagnifyRatioNumericUpDown.Value / 100);
		}
	}

	[MaybeNull]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
		var state = g.Save();
		try
		{
			using var ms = new MemoryStream(Resources.Cross);
			using var cursor = new Cursor(ms);
			var points = (stackalloc Point[] { center });
			g.TransformPoints(CoordinateSpace.Page, CoordinateSpace.World, points);
			g.ResetTransform();
			cursor.Draw(g, new Rectangle(points[0] - (Size)cursor.HotSpot, cursor.Size));
		}
		finally { g.Restore(state); }
	}

	Point ConstrainPointInImage(Point loc)
	{
		Debug.Assert(internalImage != null);
		return new(Math.Clamp(loc.X, 0, internalImage.Width - 1), Math.Clamp(loc.Y, 0, internalImage.Height - 1));
	}

	Matrix3x2 Transform =>
		Matrix3x2.CreateScale((float)MagnifyRatioNumericUpDown.Value / 100)
		* Matrix3x2.CreateTranslation(-PreviewBox.ScrollBars.Position.X, -PreviewBox.ScrollBars.Position.Y);

	Point ClientPointToImage(Point point)
	{
		Matrix3x2.Invert(Transform, out var inverseTransform);
		var result = Vector2.Transform(new Vector2(point.X, point.Y), inverseTransform);
		return ConstrainPointInImage(new Point((int)result.X, (int)result.Y));
	}

	void OnRecalculateRequested(object? sender, EventArgs e)
	{
		Debug.Assert(internalImage != null);
		PreviewBox.ScrollBars.ContentSize = internalImage.Size * (int)MagnifyRatioNumericUpDown.Value / 100;
		PreviewBox.Invalidate();
	}

	#region PreviewBox EventHandlers

	void OnPreviewBoxScrollChanged(object sender, EventArgs e) => PreviewBox.Invalidate();

	void OnPreviewBoxMouseDown(object? sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			point1 = point2 = ClientPointToImage(e.Location);
			leftMouseDown = true;
		}
	}

	void OnPreviewBoxMouseMove(object? sender, MouseEventArgs e)
	{
		if (leftMouseDown && e.Button == MouseButtons.Left)
		{
			point2 = ClientPointToImage(e.Location);
			PreviewBox.Invalidate();
		}
	}

	void OnPreviewBoxMouseUp(object? sender, MouseEventArgs e)
	{
		if (leftMouseDown && e.Button == MouseButtons.Left)
		{
			point2 = ClientPointToImage(e.Location);
			PreviewBox.Invalidate();
			leftMouseDown = false;
		}
	}

	void OnPreviewBoxPaint(object? sender, PaintEventArgs e)
	{
		Debug.Assert(internalImage != null);
		e.Graphics.TransformElements = Transform;
		e.Graphics.DrawImage(internalImage, default(Point));
		using (var reg = new Region(new Rectangle(default, internalImage.Size)))
		{
			var (clipRect, scaledSize) = ImageClipBoundsAndScaledSize;
			reg.Xor(clipRect);
#pragma warning disable CA1863 // リソースに対してCompositeFormatは使用できない
			SizeLabel.Text = string.Format(CultureInfo.CurrentCulture, Resources.ImageSizeStringRepresentation, scaledSize.Width, scaledSize.Height);
#pragma warning restore CA1863
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
		loc = ConstrainPointInImage(loc + new Size(shiftScale.X * dif, shiftScale.Y * dif));
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
			MagnifyRatioNumericUpDown.Value = Math.Clamp((int)MagnifyRatioNumericUpDown.Value + SystemInformation.MouseWheelScrollLines * e.Delta / SystemInformation.MouseWheelScrollDelta, 1, 100);
	}

	#endregion

	void OnOKButtonClick(object? sender, EventArgs e)
	{
		Debug.Assert(internalImage != null);
		var (clipRect, scaledSize) = ImageClipBoundsAndScaledSize;
		using (var image = new Bitmap(scaledSize.Width, scaledSize.Height))
		{
			using (var g = Graphics.FromImage(image))
				g.DrawImage(internalImage, new Rectangle(default, scaledSize), clipRect, GraphicsUnit.Pixel);
			this.image = image.ToBinary(ImageFormat.Bmp);
		}
		DialogResult = DialogResult.OK;
		Close();
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		using (var ms = new MemoryStream(Resources.Cross))
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
