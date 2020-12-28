using System;
using System.Drawing;
using System.Windows.Forms;
using Comical.Core;

namespace Comical.Controls
{
	public partial class Previewer : ScrollableControl
	{
		public Previewer()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			picPreview = new FocusablePictureBox();
			((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
			SuspendLayout();
			picPreview.Dock = DockStyle.Fill;
			picPreview.Location = new Point(0, 0);
			picPreview.MouseDown += picPreview_MouseDown;
			picPreview.MouseMove += picPreview_MouseMove;
			picPreview.MouseUp += picPreview_MouseUp;
			AutoScroll = true;
			Controls.Add(picPreview);
			((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
			ResumeLayout(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && picPreview != null)
			{
				picPreview.Image?.Dispose();
				picPreview.Dispose();
				picPreview = null;
			}
			base.Dispose(disposing);
		}

		FocusablePictureBox picPreview;
		PreviewerStretchMode stretchMode;
		bool avoidResizeMessage = false;
		Cursor currentCursor = Cursors.Default;
		bool cursorOverride = false;
		Binary[] images;

		public PictureBox ViewPane => picPreview;

		public Binary Image => images != null && images.Length > 0 ? images[0] : null;

		public void SetImage(Binary image) => SetImage(image, null, false);

		public void SetImage(Binary leftImage, Binary rightImage) => SetImage(leftImage, rightImage, true);

		void SetImage(Binary primaryImage, Binary secondaryImage, bool useSecondary)
		{
			if (useSecondary)
			{
				if (images != null && images.Length == 2 && images[0] == primaryImage && images[1] == secondaryImage) return;
				images = new[] { primaryImage, secondaryImage };
			}
			else
			{
				if (images != null && images.Length == 1 && images[0] == primaryImage) return;
				images = new[] { primaryImage };
			}
			picPreview.Image?.Dispose();
			if (useSecondary)
			{
				Bitmap image = null;
				try
				{
					using (var left = primaryImage?.ToImage())
					using (var right = secondaryImage?.ToImage())
					{
						image = new Bitmap(Math.Max(left?.Width ?? 0, right?.Width ?? 0) * 2, Math.Max(left?.Height ?? 0, right?.Height ?? 0));
						using var g = Graphics.FromImage(image);
						if (left != null)
							g.DrawImage(left, new Point(image.Width / 2 - left.Width, 0));
						if (right != null)
							g.DrawImage(right, new Point(image.Width / 2, 0));
					}
					picPreview.Image = image;
					image = null;
				}
				finally { image?.Dispose(); }
			}
			else
			{
				picPreview.Image = primaryImage?.ToImage();
			}
			UpdatePictureBoxSize();
		}

		public Size ImageSize => picPreview.Image == null ? default : picPreview.Image.Size;

		public PreviewerStretchMode StretchMode
		{
			get => stretchMode;
			set
			{
				if (stretchMode != value)
				{
					stretchMode = value;
					UpdatePictureBoxSize();
				}
			}
		}

		void UpdatePictureBoxSize()
		{
			try
			{
				avoidResizeMessage = true;
				var imageSize = ImageSize;
				var pbsm = PictureBoxSizeMode.CenterImage;
				var ds = DockStyle.Fill;
				if (imageSize.Width > ClientSize.Width - AutoScrollMargin.Width || imageSize.Height > ClientSize.Height - AutoScrollMargin.Height)
				{
					if (stretchMode == PreviewerStretchMode.Uniform)
						pbsm = PictureBoxSizeMode.Zoom;
					else
						ds = DockStyle.None;
				}
				picPreview.SizeMode = pbsm;
				picPreview.Dock = ds;
				if (ds == DockStyle.None)
				{
					picPreview.ClientSize = new Size(Math.Max(imageSize.Width, ClientSize.Width - AutoScrollMargin.Width - SystemInformation.VerticalScrollBarWidth),
						Math.Max(imageSize.Height, ClientSize.Height - AutoScrollMargin.Height - SystemInformation.HorizontalScrollBarHeight));
					picPreview.Location = AutoScrollPosition;
					SetCursor(false);
				}
				else
					SetCursorInternal(Cursors.Default);
			}
			finally { avoidResizeMessage = false; }
		}

		protected override void OnResize(EventArgs e)
		{
			if (!avoidResizeMessage)
				UpdatePictureBoxSize();
			base.OnResize(e);
		}

		Point origin;
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
				picPreview.Cursor = currentCursor;
		}

		public override Cursor Cursor
		{
			get => base.Cursor;
			set
			{
				if (value == null)
				{
					cursorOverride = false;
					picPreview.Cursor = currentCursor;
				}
				else
				{
					cursorOverride = true;
					picPreview.Cursor = value;
				}
			}
		}

		void picPreview_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button.HasFlag(MouseButtons.Left))
			{
				origin = picPreview.Parent.PointToScreen(e.Location);
				if (picPreview.Dock == DockStyle.None)
					SetCursor(true);
			}
		}

		void picPreview_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button.HasFlag(MouseButtons.Left) && (dragging || Math.Abs(origin.X - e.X) > SystemInformation.DragSize.Width / 2 || Math.Abs(origin.Y - e.Y) > SystemInformation.DragSize.Height / 2))
			{
				dragging = true;
				AutoScrollPosition = origin - (Size)picPreview.PointToScreen(e.Location);
				picPreview.Refresh();
			}
		}

		void picPreview_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button.HasFlag(MouseButtons.Left))
			{
				dragging = false;
				if (picPreview.Dock == DockStyle.None)
					SetCursor(false);
			}
		}

		protected override void Select(bool directed, bool forward)
		{
			base.Select(directed, forward);
			picPreview.Select();
		}

		public override ContextMenuStrip ContextMenuStrip
		{
			get => base.ContextMenuStrip;
			set
			{
				base.ContextMenuStrip = value;
				picPreview.ContextMenuStrip = value;
			}
		}
	}

	public enum PreviewerStretchMode
	{
		None,
		Uniform,
	}
}
