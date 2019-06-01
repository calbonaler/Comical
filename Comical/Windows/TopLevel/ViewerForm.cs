using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Comical.Core;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Comical
{
	[CLSCompliant(false)]
	public partial class ViewerForm : Microsoft.WindowsAPICodePack.Shell.GlassForm
	{
		public ViewerForm()
		{
			InitializeComponent();
			prevMain.ContextMenuStrip = conBookmarks;
			prevMain.ViewPane.MouseMove += picPreview_MouseMove;
			prevMain.ViewPane.MouseUp += picPreview_MouseUp;
			prevMain.ViewPane.Paint += picPreview_Paint;
			prevMain.ViewPane.KeyDown += picPreview_KeyDown;
			if (AeroGlassCompositionEnabled)
				ExtendFrame(new Padding(-1));
		}

		protected override CreateParams CreateParams
		{
			get
			{
				var cparams = base.CreateParams;
				cparams.Style = cparams.Style & ~0x000F0000 | 0x00020000;
				return cparams;
			}
		}

		public ViewerForm(string fileName) : this()
		{
			_comic = new Comic(); // Construct with read-only mode.
			_openingFileName = fileName;
		}

		Comic _comic;
		string _openingFileName = "";
		Spread[] _spreads;
		int _current;

		int CurrentPage
		{
			get => _current;
			set
			{
				value = value % _spreads.Length;
				if (value < 0)
					value += _spreads.Length;
				if (_current != value)
				{
					_current = value;
					ViewCurrentPage();
				}
			}
		}

		void Open(string fileName)
		{
			Activate();
			prevMain.Select();
			using (var dialog = new TaskDialog())
			{
				dialog.Cancelable = false;
				TaskDialogButton btnCancel = new TaskDialogButton(nameof(btnCancel), Properties.Resources.Cancel);
				dialog.Controls.Add(btnCancel);
				dialog.Caption = Application.ProductName;
				dialog.Icon = TaskDialogStandardIcon.None;
				dialog.InstructionText = Properties.Resources.OpeningFile;
				dialog.OwnerWindowHandle = Handle;
				dialog.ProgressBar = new TaskDialogProgressBar(0, 100, 0);
				dialog.Opened += async (s, ev) =>
				{
					btnCancel.Enabled = false;
					await _comic.OpenAsync(fileName, new Progress<int>(x => this.InvokeIfNeeded(() => dialog.ProgressBar.Value = x)));
					dialog.Close(TaskDialogResult.Ok);
				};
				dialog.StartupLocation = TaskDialogStartupLocation.CenterOwner;
				dialog.Show();
			}
			conBookmarks.Items.AddRange(_comic.Bookmarks.Select(b => new ToolStripMenuItem(b.Name, null, (s, ev) => CurrentPage = Array.FindIndex(_spreads, x => x.Left == _comic.Images[b.Target] || x.Right == _comic.Images[b.Target]))).ToArray());
			_spreads = _comic.ConstructSpreads().ToArray();
			_openingFileName = "";
			ViewCurrentPage();
		}

		void ViewCurrentPage()
		{
			if (prevMain.Image != null)
			{
				prevMain.Image.Dispose();
				prevMain.Image = null;
			}
			if (_spreads[CurrentPage].IsFillSpread)
			{
				prevMain.Image = _spreads[CurrentPage].Left.CreateImage();
				return;
			}
			using (var left = _spreads[CurrentPage].Left?.CreateImage())
			using (var right = _spreads[CurrentPage].Right?.CreateImage())
			{
				prevMain.Image = new Bitmap(Math.Max(left?.Width ?? 0, right?.Width ?? 0) * 2, Math.Max(left?.Height ?? 0, right?.Height ?? 0));
				using (var g = Graphics.FromImage(prevMain.Image))
				{
					if (left != null)
						g.DrawImage(left, new Point(prevMain.Image.Width / 2 - left.Width, 0));
					if (right != null)
						g.DrawImage(right, new Point(prevMain.Image.Width / 2, 0));
				}
			}
		}

		void ViewPrevious() => CurrentPage--;

		void ViewNext() => CurrentPage++;

		FocusMode _focusMode = FocusMode.None;
		const int CloseHeight = 20;
		SolidBrush _closeBrush = new SolidBrush(Color.FromArgb(64, 255, 0, 0));

		#region picPreview EventHandlers

		void picPreview_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.None && (_comic.PageTurningDirection == PageTurningDirection.ToLeft ? e.X >= Math.Min(prevMain.ViewPane.ClientSize.Width, prevMain.ClientSize.Width) - prevMain.AutoScrollPosition.X - Properties.Resources.Next.Width : e.X <= Properties.Resources.Next.Width - prevMain.AutoScrollPosition.X))
			{
				_focusMode = FocusMode.Next;
				prevMain.Cursor = Cursors.Default;
			}
			else if (e.Button == MouseButtons.None && (_comic.PageTurningDirection == PageTurningDirection.ToLeft ? e.X <= Properties.Resources.Previous.Width - prevMain.AutoScrollPosition.X : e.X >= Math.Min(prevMain.ViewPane.ClientSize.Width, prevMain.ClientSize.Width) - prevMain.AutoScrollPosition.X - Properties.Resources.Previous.Width))
			{
				_focusMode = FocusMode.Previous;
				prevMain.Cursor = Cursors.Default;
			}
			else if (e.Button == MouseButtons.None && e.Y >= prevMain.ClientSize.Height - CloseHeight - prevMain.AutoScrollPosition.Y)
			{
				_focusMode = FocusMode.Close;
				prevMain.Cursor = Cursors.Default;
			}
			else
			{
				_focusMode = FocusMode.None;
				prevMain.Cursor = null;
			}
			prevMain.Invalidate();
		}

		void picPreview_MouseUp(object sender, MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) != 0)
			{
				if (_focusMode == FocusMode.Previous)
					ViewPrevious();
				else if (_focusMode == FocusMode.Next)
					ViewNext();
				else if (_focusMode == FocusMode.Close)
					Close();
			}
			if (e.Button == MouseButtons.XButton2)
				ViewPrevious();
			else if (e.Button == MouseButtons.XButton1)
				ViewNext();
			else if (e.Button == MouseButtons.Middle)
				prevMain.StretchMode = prevMain.StretchMode == Comical.Controls.PreviewerStretchMode.Uniform ? Comical.Controls.PreviewerStretchMode.None : Comical.Controls.PreviewerStretchMode.Uniform;
		}

		void picPreview_Paint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			if (_focusMode == FocusMode.Close)
				g.FillRectangle(_closeBrush, -prevMain.AutoScrollPosition.X, prevMain.ClientSize.Height - CloseHeight - prevMain.AutoScrollPosition.Y, prevMain.ViewPane.ClientSize.Width, CloseHeight);
			else if (_focusMode != FocusMode.None)
			{
				var img = (Bitmap)Properties.Resources.ResourceManager.GetObject(_focusMode.ToString(), Properties.Resources.Culture);
				var y = (prevMain.ClientSize.Height - img.Height) / 2 - prevMain.AutoScrollPosition.Y;
				if (_comic.PageTurningDirection == PageTurningDirection.ToLeft ^ _focusMode == FocusMode.Next)
					g.DrawImage(img, -prevMain.AutoScrollPosition.X, y);
				else
					g.DrawImage(img, Math.Min(prevMain.ClientSize.Width, prevMain.ViewPane.ClientSize.Width) - img.Width - prevMain.AutoScrollPosition.X, y);
			}
		}

		void picPreview_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
			else if (e.KeyCode == (_comic.PageTurningDirection == PageTurningDirection.ToLeft ? Keys.Left : Keys.Right))
				ViewPrevious();
			else if (e.KeyCode == (_comic.PageTurningDirection == PageTurningDirection.ToLeft ? Keys.Right : Keys.Left))
				ViewNext();
		}

		#endregion

		protected override void OnClosing(CancelEventArgs e)
		{
			if (e != null && _comic.IsBusy)
				e.Cancel = true;
			base.OnClosing(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			DesktopBounds = Screen.FromControl(this).Bounds;
			Open(_openingFileName);
			base.OnLoad(e);
		}

		enum FocusMode
		{
			None,
			Close,
			Previous,
			Next,
		}
	}
}


