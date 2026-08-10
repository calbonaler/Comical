using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Comical.Core;
using Comical.Properties;

namespace Comical.Controls;

public class ImageClipper : ScrollBaredControl, INotifyPropertyChanged
{
	Image? _image;
	Point ClipPoint0 { get; set { field = value; OnPropertyChanged(nameof(ClippedImageSize)); } }
	Point ClipPoint1 { get; set { field = value; OnPropertyChanged(nameof(ClippedImageSize)); } }
	bool _clipPoint1Moving;
	Size _clipPointVelocity;

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_image?.Dispose();
			_image = null;
		}
		base.Dispose(disposing);
	}

	public void SetImage(Binary? value)
	{
		_image?.Dispose();
		_image = value?.ToImage();
		if (_image is null)
			CancelEdit();
		else
			UpdateImageRelatedInfo(true);
	}
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int MagnifyRatio
	{
		get;
		set
		{
			if (field == value) return;
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
			SetProperty(ref field, value);
			UpdateImageRelatedInfo(false);
		}
	} = 100;
	public Size ClippedImageSize => ImageClipBoundsAndScaledSize.ScaledSize;
	public bool IsEditing
	{
		get;
		private set
		{
			if (field == value) return;
			SetProperty(ref field, value);
		}
	}

	Matrix3x2 ImageToClientTransform => Matrix3x2.CreateScale((float)MagnifyRatio / 100) * Matrix3x2.CreateTranslation(-ScrollBars.Position.X, -ScrollBars.Position.Y);
	(Rectangle ClipBounds, Size ScaledSize) ImageClipBoundsAndScaledSize
	{
		get
		{
			if (_image is null) return default;
			var clipRect = ClipPoint0 == ClipPoint1
				? new Rectangle(default, _image.Size)
				: new Rectangle(
					Math.Min(ClipPoint0.X, ClipPoint1.X),
					Math.Min(ClipPoint0.Y, ClipPoint1.Y),
					Math.Abs(ClipPoint1.X - ClipPoint0.X) + 1,
					Math.Abs(ClipPoint1.Y - ClipPoint0.Y) + 1
				);
			return (clipRect, clipRect.Size * MagnifyRatio / 100);
		}
	}
	Point ConstrainPointInImage(Point loc)
		=> _image is not null ? new(Math.Clamp(loc.X, 0, _image.Width - 1), Math.Clamp(loc.Y, 0, _image.Height - 1)) : default;
	Point ClientPointToImage(Point point)
	{
		Matrix3x2.Invert(ImageToClientTransform, out var inverseTransform);
		var result = Vector2.Transform(new Vector2(point.X, point.Y), inverseTransform);
		return ConstrainPointInImage(new Point((int)result.X, (int)result.Y));
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
	void UpdateImageRelatedInfo(bool initEditSessionData)
	{
		if (initEditSessionData)
		{
			ClipPoint0 = ClipPoint1 = default;
			_clipPoint1Moving = true;
			_clipPointVelocity = default;
		}
		ScrollBars.ContentSize = IsEditing && _image is not null ? _image.Size * MagnifyRatio / 100 : default;
		Invalidate();
		OnPropertyChanged(nameof(ClippedImageSize));
	}
	void BeginOrEndEdit(bool begin)
	{
		if (begin)
		{
			using var ms = new MemoryStream(Resources.Cross);
			Cursor = new Cursor(ms);
		}
		else
		{
			Cursor.Dispose();
			Cursor = null;
		}
		MagnifyRatio = 100;
		IsEditing = begin;
		UpdateImageRelatedInfo(true);
	}
	public void BeginEdit() => BeginOrEndEdit(true);
	public Binary CommitEdit(ImageFormat format)
	{
		[DoesNotReturn]
		static void Throw() => throw new InvalidOperationException("Cannot commit empty image editing.");
		if (_image is null) Throw();
		var (clipRect, scaledSize) = ImageClipBoundsAndScaledSize;
		Binary result;
		using (var image = new Bitmap(scaledSize.Width, scaledSize.Height))
		{
			using (var g = Graphics.FromImage(image))
				g.DrawImage(_image, new Rectangle(default, scaledSize), clipRect, GraphicsUnit.Pixel);
			result = image.ToBinary(format);
		}
		BeginOrEndEdit(false);
		return result;
	}
	public void CancelEdit() => BeginOrEndEdit(false);

	protected override void OnPaint(PaintEventArgs e)
	{
		base.OnPaint(e);
		if (_image is null) return;
		if (IsEditing)
		{
			e.Graphics.TransformElements = ImageToClientTransform;
			e.Graphics.DrawImage(_image, new Rectangle(default, _image.Size));
			using (var reg = new Region(new Rectangle(default, _image.Size)))
			{
				reg.Xor(ImageClipBoundsAndScaledSize.ClipBounds);
				e.Graphics.FillRegion(Brushes.Blue, reg);
			}
			DrawCross(e.Graphics, ClipPoint0);
			DrawCross(e.Graphics, ClipPoint1);
		}
		else
		{
			var scaledSize = Utils.ScaleSize(_image.Size, ClientSize);
			e.Graphics.DrawImage(_image, new Rectangle((Point)((ClientSize - scaledSize) / 2), scaledSize));
		}
	}
	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);
		Invalidate();
	}
	protected override void OnScrollChanged(EventArgs e)
	{
		base.OnScrollChanged(e);
		Invalidate();
	}
	protected override void OnMouseWheel(MouseEventArgs e)
	{
		base.OnMouseWheel(e);
		if (IsEditing && (ModifierKeys & Keys.Control) != 0)
			MagnifyRatio = Math.Clamp(MagnifyRatio + SystemInformation.MouseWheelScrollLines * e.Delta / SystemInformation.MouseWheelScrollDelta, 1, 100);
	}
	protected override void OnMouseDown(MouseEventArgs e)
	{
		base.OnMouseDown(e);
		if (IsEditing && e.Button == MouseButtons.Left)
		{
			Select();
			ClipPoint0 = ClipPoint1 = ClientPointToImage(e.Location);
		}
	}
	protected override void OnMouseMove(MouseEventArgs e)
	{
		base.OnMouseMove(e);
		OnMouseMoveOrUp(e);
	}
	protected override void OnMouseUp(MouseEventArgs e)
	{
		base.OnMouseUp(e);
		OnMouseMoveOrUp(e);
	}
	void OnMouseMoveOrUp(MouseEventArgs e)
	{
		if (IsEditing && e.Button == MouseButtons.Left)
		{
			ClipPoint1 = ClientPointToImage(e.Location);
			Invalidate();
		}
	}
	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!IsEditing || e.KeyCode is Keys.ShiftKey or Keys.Menu or Keys.ControlKey)
			return;
		else if (e.KeyCode == Keys.Enter)
		{
			_clipPoint1Moving = !_clipPoint1Moving;
			return;
		}
		if (e.KeyCode == Keys.Left)
			_clipPointVelocity.Width = -1;
		else if (e.KeyCode == Keys.Right)
			_clipPointVelocity.Width = +1;
		else if (e.KeyCode == Keys.Up)
			_clipPointVelocity.Height = -1;
		else if (e.KeyCode == Keys.Down)
			_clipPointVelocity.Height = +1;
		var loc = _clipPoint1Moving ? ClipPoint1 : ClipPoint0;
		var newLoc = ConstrainPointInImage(loc + _clipPointVelocity * (e.Control ? 10 : 1));
		if (e.Shift)
			ClipPoint0 = ClipPoint1 = newLoc;
		else if (_clipPoint1Moving)
			ClipPoint1 = newLoc;
		else
			ClipPoint0 = newLoc;
		Invalidate();
	}
	protected override void OnKeyUp(KeyEventArgs e)
	{
		base.OnKeyUp(e);
		if (!IsEditing) return;
		if (e.KeyCode is Keys.Left or Keys.Right)
			_clipPointVelocity.Width = 0;
		else if (e.KeyCode is Keys.Up or Keys.Down)
			_clipPointVelocity.Height = 0;
	}

	void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
	{
		if (EqualityComparer<T>.Default.Equals(storage, value))
			return;
		storage = value;
		OnPropertyChanged(propertyName);
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}
