using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using Comical.Core;

namespace Comical.Controls
{
	public class Previewer : ScrollableControl
	{
		public Previewer()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			ViewPane = new FocusablePictureBox();
			((System.ComponentModel.ISupportInitialize)ViewPane).BeginInit();
			SuspendLayout();
			ViewPane.Dock = DockStyle.Fill;
			ViewPane.Location = new Point(0, 0);
			ViewPane.MouseDown += OnViewPaneMouseDown;
			ViewPane.MouseMove += OnViewPaneMouseMove;
			ViewPane.MouseUp += OnViewPaneMouseUp;
			AutoScroll = true;
			Controls.Add(ViewPane);
			((System.ComponentModel.ISupportInitialize)ViewPane).EndInit();
			ResumeLayout(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ViewPane.Image?.Dispose();
				ViewPane.Dispose();
			}
			base.Dispose(disposing);
		}

		PreviewerStretchMode stretchMode;
		bool avoidResizeMessage = false;
		Cursor currentCursor = Cursors.Default;
		bool cursorOverride = false;
		Binary?[]? images;

		public PictureBox ViewPane { get; private set; }

		public Binary? Image => images != null && images.Length > 0 ? images[0] : null;

		public void SetImage(Binary? image) => SetImage(image, null, false);

		public void SetImage(Binary? leftImage, Binary? rightImage) => SetImage(leftImage, rightImage, true);

		void SetImage(Binary? primaryImage, Binary? secondaryImage, bool useSecondary)
		{
			if (useSecondary)
			{
				if (images != null && images.Length == 2 && images[0] == primaryImage && images[1] == secondaryImage) return;
				images = [primaryImage, secondaryImage];
			}
			else
			{
				if (images != null && images.Length == 1 && images[0] == primaryImage) return;
				images = [primaryImage];
			}
			ViewPane.Image?.Dispose();
			if (useSecondary)
			{
				Bitmap? image = null;
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
					ViewPane.Image = image;
					image = null;
				}
				finally { image?.Dispose(); }
			}
			else
			{
				ViewPane.Image = primaryImage?.ToImage();
			}
			UpdatePictureBoxSize();
		}

		public Size ImageSize => ViewPane.Image == null ? default : ViewPane.Image.Size;

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
				ViewPane.SizeMode = pbsm;
				ViewPane.Dock = ds;
				if (ds == DockStyle.None)
				{
					ViewPane.ClientSize = new Size(Math.Max(imageSize.Width, ClientSize.Width - AutoScrollMargin.Width - SystemInformation.VerticalScrollBarWidth),
						Math.Max(imageSize.Height, ClientSize.Height - AutoScrollMargin.Height - SystemInformation.HorizontalScrollBarHeight));
					ViewPane.Location = AutoScrollPosition;
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
				ViewPane.Cursor = currentCursor;
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
					ViewPane.Cursor = currentCursor;
				}
				else
				{
					cursorOverride = true;
					ViewPane.Cursor = value;
				}
			}
		}

		void OnViewPaneMouseDown(object? sender, MouseEventArgs e)
		{
			if (e.Button.HasFlag(MouseButtons.Left))
			{
				origin = ViewPane.PointToScreen(e.Location);
				if (ViewPane.Dock == DockStyle.None)
					SetCursor(true);
			}
		}

		void OnViewPaneMouseMove(object? sender, MouseEventArgs e)
		{
			if (e.Button.HasFlag(MouseButtons.Left) && (dragging || Math.Abs(origin.X - e.X) > SystemInformation.DragSize.Width / 2 || Math.Abs(origin.Y - e.Y) > SystemInformation.DragSize.Height / 2))
			{
				dragging = true;
				AutoScrollPosition = origin - (Size)ViewPane.PointToScreen(e.Location);
				ViewPane.Refresh();
			}
		}

		void OnViewPaneMouseUp(object? sender, MouseEventArgs e)
		{
			if (e.Button.HasFlag(MouseButtons.Left))
			{
				dragging = false;
				if (ViewPane.Dock == DockStyle.None)
					SetCursor(false);
			}
		}

		protected override void Select(bool directed, bool forward)
		{
			base.Select(directed, forward);
			ViewPane.Select();
		}

		public override ContextMenuStrip? ContextMenuStrip
		{
			get => base.ContextMenuStrip;
			set
			{
				base.ContextMenuStrip = value;
				ViewPane.ContextMenuStrip = value;
			}
		}
	}

	public enum PreviewerStretchMode
	{
		None,
		Uniform,
	}
}
