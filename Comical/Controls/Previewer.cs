using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace Comical.Controls
{
	public partial class Previewer : ScrollBaredControl
	{
		Image? image;
		PreviewerStretchMode stretchMode;
		Cursor currentCursor = Cursors.Default;
		bool cursorOverride = false;

		public Image? Image
		{
			get => image;
			set
			{
				if (image != value)
				{
					image = value;
					UpdateAutoScroll();
				}
			}
		}

		public PreviewerStretchMode StretchMode
		{
			get => stretchMode;
			set
			{
				if (stretchMode != value)
				{
					stretchMode = value;
					UpdateAutoScroll();
				}
			}
		}

		void UpdateAutoScroll()
		{
			ScrollBars.ContentSize = StretchMode != PreviewerStretchMode.Uniform && Image != null ? Image.Size : default;
			if (ScrollBars.IsOverflow)
				SetCursor(false);
			else
				SetCursorInternal(Cursors.Default);
			Invalidate();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateAutoScroll();
		}

		Point origin;
		Point originalAutoScrollPosition;
		bool dragging = false;

		void SetCursor(bool grisp)
		{
			using var ms = new System.IO.MemoryStream(grisp ? Properties.Resources.GrispingHand : Properties.Resources.FreeHand);
			SetCursorInternal(new Cursor(ms));
		}

		void SetCursorInternal(Cursor cursor)
		{
			currentCursor = cursor;
			if (!cursorOverride)
				base.Cursor = currentCursor;
		}

		[AllowNull]
		public override Cursor Cursor
		{
			get => base.Cursor;
			set
			{
				if (value == null)
				{
					cursorOverride = false;
					base.Cursor = currentCursor;
				}
				else
				{
					cursorOverride = true;
					base.Cursor = value;
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button.HasFlag(MouseButtons.Left))
			{
				origin = e.Location;
				originalAutoScrollPosition = Point.Empty - (Size)ScrollBars.Position;
				if (ScrollBars.IsOverflow)
					SetCursor(true);
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button.HasFlag(MouseButtons.Left))
			{
				var diff = origin - (Size)e.Location;
				if (!dragging || Math.Abs(diff.X) > SystemInformation.DragSize.Width / 2 || Math.Abs(diff.Y) > SystemInformation.DragSize.Height / 2)
				{
					dragging = true;
					ScrollBars.Position = diff - (Size)originalAutoScrollPosition;
					Invalidate();
				}
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (e.Button.HasFlag(MouseButtons.Left))
			{
				dragging = false;
				if (ScrollBars.IsOverflow)
					SetCursor(false);
			}
			base.OnMouseUp(e);
		}

		protected override void OnScrollChanged(EventArgs e)
		{
			base.OnScrollChanged(e);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Image == null)
				return;
			var scaledSize = Utils.ScaleSize(Image.Size, ClientSize);
			Size displaySize;
			Rectangle srcRect;
			if (StretchMode == PreviewerStretchMode.Uniform || scaledSize == Image.Size)
			{
				displaySize = scaledSize;
				srcRect = new(default, Image.Size);
			}
			else
			{
				displaySize = new Size(Math.Min(Image.Width, ClientSize.Width), Math.Min(Image.Height, ClientSize.Height));
				srcRect = new(ScrollBars.Position, displaySize);
			}
			e.Graphics.DrawImage(Image,
				new Rectangle((Point)((ClientSize - displaySize) / 2), displaySize),
				srcRect, GraphicsUnit.Pixel);
			base.OnPaint(e);
		}
	}

	public enum PreviewerStretchMode
	{
		None,
		Uniform,
	}
}
