using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Comical.Core;
using CPDialogs = Microsoft.WindowsAPICodePack.Dialogs;

namespace Comical
{
	public partial class ViewerForm : Microsoft.WindowsAPICodePack.Shell.GlassForm
	{
		public ViewerForm(string fileName)
		{
			InitializeComponent();
			prevMain.ContextMenuStrip = conBookmarks;
			prevMain.ViewPane.MouseMove += picPreview_MouseMove;
			prevMain.ViewPane.MouseUp += picPreview_MouseUp;
			prevMain.ViewPane.Paint += picPreview_Paint;
			prevMain.ViewPane.KeyDown += picPreview_KeyDown;
			_comic = new Comic();
			_openingFileName = fileName;
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

		readonly Comic _comic;
		string _openingFileName = "";
		Spread[]? _spreads;
		int _currentSpreadIndex = -1;

		void SetCurrentSpread(int value)
		{
			if (_currentSpreadIndex != value)
			{
				_currentSpreadIndex = value;
				Debug.Assert(_spreads != null);
				var spread = _spreads[_currentSpreadIndex];
				if (spread.Fill is { } fill)
					prevMain.SetImage(fill.Data);
				else
					prevMain.SetImage(spread.Left?.Data, spread.Right?.Data);
			}
		}

		void MoveCurrentSpread(int offset)
		{
			Debug.Assert(_spreads != null);
			var value = (_currentSpreadIndex + offset) % _spreads.Length;
			if (value < 0)
				value += _spreads.Length;
			SetCurrentSpread(value);
		}

		void Open(string fileName)
		{
			Activate();
			prevMain.Select();
			using (var dialog = new CPDialogs.TaskDialog())
			{
				dialog.Cancelable = false;
				CPDialogs.TaskDialogButton btnCancel = new CPDialogs.TaskDialogButton(nameof(btnCancel), Properties.Resources.Cancel);
				dialog.Controls.Add(btnCancel);
				dialog.Caption = Application.ProductName;
				dialog.Icon = CPDialogs.TaskDialogStandardIcon.None;
				dialog.InstructionText = Properties.Resources.OpeningFile;
				dialog.OwnerWindowHandle = Handle;
				dialog.ProgressBar = new CPDialogs.TaskDialogProgressBar(0, 100, 0);
				dialog.Opened += async (s, ev) =>
				{
					btnCancel.Enabled = false;
					await _comic.OpenAsync(fileName, new Progress<int>(x => this.InvokeIfNeeded(() => dialog.ProgressBar.Value = x)));
					dialog.Close(CPDialogs.TaskDialogResult.Ok);
				};
				dialog.StartupLocation = CPDialogs.TaskDialogStartupLocation.CenterOwner;
				dialog.Show();
			}
			_spreads = _comic.ConstructSpreads().ToArray();
			conBookmarks.Items.AddRange(_comic.Bookmarks.Select(b => new ToolStripMenuItem(b.Name, null, (s, ev) => SetCurrentSpread(Array.FindIndex(_spreads, x => x.Left == _comic.Images[b.Target] || x.Right == _comic.Images[b.Target])))).ToArray());
			_openingFileName = "";
			SetCurrentSpread(0);
		}

		void ViewPrevious() => MoveCurrentSpread(-1);

		void ViewNext() => MoveCurrentSpread(1);

		FocusMode _focusMode = FocusMode.None;
		const int CloseHeight = 20;
		readonly SolidBrush _closeBrush = new(Color.FromArgb(64, 255, 0, 0));

		#region picPreview EventHandlers

		void picPreview_MouseMove(object? sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.None && (_comic.BindingSide == BindingSide.Left ? e.X >= Math.Min(prevMain.ViewPane.ClientSize.Width, prevMain.ClientSize.Width) - prevMain.AutoScrollPosition.X - Properties.Resources.Next.Width : e.X <= Properties.Resources.Next.Width - prevMain.AutoScrollPosition.X))
			{
				_focusMode = FocusMode.Next;
				prevMain.Cursor = Cursors.Default;
			}
			else if (e.Button == MouseButtons.None && (_comic.BindingSide == BindingSide.Left ? e.X <= Properties.Resources.Previous.Width - prevMain.AutoScrollPosition.X : e.X >= Math.Min(prevMain.ViewPane.ClientSize.Width, prevMain.ClientSize.Width) - prevMain.AutoScrollPosition.X - Properties.Resources.Previous.Width))
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

		void picPreview_MouseUp(object? sender, MouseEventArgs e)
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

		void picPreview_Paint(object? sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			if (_focusMode == FocusMode.Close)
				g.FillRectangle(_closeBrush, -prevMain.AutoScrollPosition.X, prevMain.ClientSize.Height - CloseHeight - prevMain.AutoScrollPosition.Y, prevMain.ViewPane.ClientSize.Width, CloseHeight);
			else if (_focusMode != FocusMode.None)
			{
				var img = (Bitmap?)Properties.Resources.ResourceManager.GetObject(_focusMode.ToString(), Properties.Resources.Culture);
				Debug.Assert(img != null);
				var y = (prevMain.ClientSize.Height - img.Height) / 2 - prevMain.AutoScrollPosition.Y;
				if (_comic.BindingSide == BindingSide.Left ^ _focusMode == FocusMode.Next)
					g.DrawImage(img, -prevMain.AutoScrollPosition.X, y);
				else
					g.DrawImage(img, Math.Min(prevMain.ClientSize.Width, prevMain.ViewPane.ClientSize.Width) - img.Width - prevMain.AutoScrollPosition.X, y);
			}
		}

		void picPreview_KeyDown(object? sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
			else if (e.KeyCode == (_comic.BindingSide == BindingSide.Left ? Keys.Left : Keys.Right))
				ViewPrevious();
			else if (e.KeyCode == (_comic.BindingSide == BindingSide.Left ? Keys.Right : Keys.Left))
				ViewNext();
		}

		#endregion

		protected override void OnClosing(CancelEventArgs e)
		{
			if (_comic.IsBusy)
				e.Cancel = true;
			base.OnClosing(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			DesktopBounds = Screen.FromControl(this).Bounds;
			Open(_openingFileName);
			base.OnLoad(e);
		}

		// Prevent base class from filling background
		protected override void OnPaint(PaintEventArgs e) { }

		enum FocusMode
		{
			None,
			Close,
			Previous,
			Next,
		}
	}
}


