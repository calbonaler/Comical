using System;
using System.Drawing;
using System.Windows.Forms;

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

		readonly FocusablePictureBox picPreview;
		PreviewerStretchMode stretchMode;
		bool avoidResizeMessage = false;
		Cursor currentCursor = Cursors.Default;
		bool cursorOverride = false;

		public PictureBox ViewPane => picPreview;

		public Image Image
		{
			get => picPreview.Image;
			set
			{
				if (picPreview.Image != value)
				{
					picPreview.Image = value;
					UpdatePictureBoxSize();
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
					UpdatePictureBoxSize();
				}
			}
		}

		void UpdatePictureBoxSize()
		{
			try
			{
				avoidResizeMessage = true;
				var img = picPreview.Image;
				var pbsm = PictureBoxSizeMode.CenterImage;
				var ds = DockStyle.Fill;
				if (img != null && (img.Width > ClientSize.Width - AutoScrollMargin.Width || img.Height > ClientSize.Height - AutoScrollMargin.Height))
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
					picPreview.ClientSize = new Size(Math.Max(img.Width, ClientSize.Width - AutoScrollMargin.Width - SystemInformation.VerticalScrollBarWidth),
						Math.Max(img.Height, ClientSize.Height - AutoScrollMargin.Height - SystemInformation.HorizontalScrollBarHeight));
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
