using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Comical.Controls
{
	public partial class Previewer : UserControl
	{
		public Previewer() { InitializeComponent(); }

		string desc;
		PreviewerStretchMode stretchMode;
		bool avoidResizeMessage = false;
		Cursor currentCursor = Cursors.Default;
		bool cursorOverride = false;

		public PictureBox ViewPane { get { return picPreview; } }

		public Image Image
		{
			get { return picPreview.Image; }
			set
			{
				if (picPreview.Image != value)
					LoadImage(value);
			}
		}

		public string Description
		{
			get { return desc; }
			set
			{
				desc = value;
				Invalidate();
			}
		}

		public PreviewerStretchMode StretchMode
		{
			get { return stretchMode; }
			set
			{
				if (stretchMode != value)
					ChangeStretchMode(value);
			}
		}

		public void ChangeStretchMode(PreviewerStretchMode mode)
		{
			avoidResizeMessage = true;
			Image img = picPreview.Image;
			stretchMode = mode;
			PictureBoxSizeMode pbsm = PictureBoxSizeMode.CenterImage;
			DockStyle ds = DockStyle.Fill;
			if (img != null && (img.Width > ClientSize.Width - AutoScrollMargin.Width || img.Height > ClientSize.Height - AutoScrollMargin.Height))
			{
				if (mode == PreviewerStretchMode.Uniform)
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
			avoidResizeMessage = false;
		}

		public void LoadImage(Image image)
		{
			picPreview.Image = image;
			ChangeStretchMode(StretchMode);
		}

		public void LoadImageAsync(Uri url)
		{
			if (url == null)
				throw new ArgumentNullException("url");
			picPreview.LoadAsync(url.AbsoluteUri);
		}

		protected override void OnResize(EventArgs e)
		{
			if (!avoidResizeMessage)
				ChangeStretchMode(stretchMode);
			base.OnResize(e);
		}

		void picPreview_Paint(object sender, PaintEventArgs e)
		{
			if (picPreview.Image == null)
			{
				using (SolidBrush sb = new SolidBrush(ForeColor))
				using (StringFormat sf = new StringFormat())
				{
					sf.Alignment = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Center;
					e.Graphics.DrawString(Description, Font, sb, ClientRectangle, sf);
				}
			}
		}

		void picPreview_LoadCompleted(object sender, AsyncCompletedEventArgs e) { ChangeStretchMode(StretchMode); }

		Point origin;
		bool dragging = false;

		void SetCursor(bool grisp)
		{
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream(grisp ? Properties.Resources.GrispingHand : Properties.Resources.FreeHand))
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
			get { return base.Cursor; }
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
				Point sc = picPreview.PointToScreen(e.Location);
				int x = origin.X - sc.X;
				int y = origin.Y - sc.Y;
				AutoScrollPosition = new Point(x, y);
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
	}

	public enum PreviewerStretchMode
	{
		None,
		Uniform,
	}
}
