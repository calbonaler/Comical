using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Comical.Core;
using CPDialogs = Microsoft.WindowsAPICodePack.Dialogs;

namespace Comical;

public partial class ViewerForm : Microsoft.WindowsAPICodePack.Shell.GlassForm
{
	public ViewerForm(string fileName)
	{
		InitializeComponent();
		MainPreviewer.ContextMenuStrip = BookmarksContextMenu;
		MainPreviewer.MouseMove += OnMainPreviewerViewPaneMouseMove;
		MainPreviewer.MouseUp += OnMainPreviewerViewPaneMouseUp;
		MainPreviewer.Paint += OnMainPreviewerViewPanePaint;
		MainPreviewer.KeyDown += OnMainPreviewerViewPaneKeyDown;
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
		if (_currentSpreadIndex == value)
			return;
		MainPreviewer.Image?.Dispose();
		MainPreviewer.Image = null;
		_currentSpreadIndex = value;
		Debug.Assert(_spreads != null);
		var spread = _spreads[_currentSpreadIndex];
		if (spread.Fill is { } fill)
			MainPreviewer.Image = fill.Data.ToImage();
		else
		{
			using var left = spread.Left?.Data?.ToImage();
			using var right = spread.Right?.Data?.ToImage();
			var image = new Bitmap(Math.Max(left?.Width ?? 0, right?.Width ?? 0) * 2, Math.Max(left?.Height ?? 0, right?.Height ?? 0));
			try
			{
				using var g = Graphics.FromImage(image);
				if (left != null)
					g.DrawImage(left, new Point(image.Width / 2 - left.Width, 0));
				if (right != null)
					g.DrawImage(right, new Point(image.Width / 2, 0));
			}
			catch
			{
				image?.Dispose();
				throw;
			}
			MainPreviewer.Image = image;
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
		MainPreviewer.Select();
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
		_spreads = [.. _comic.ConstructSpreads()];
		BookmarksContextMenu.Items.AddRange([.. _comic.Bookmarks.Select(b => new ToolStripMenuItem(b.Name, null, (s, ev) => SetCurrentSpread(Array.FindIndex(_spreads, x => x.Left == _comic.Images[b.Target] || x.Right == _comic.Images[b.Target]))))]);
		_openingFileName = "";
		SetCurrentSpread(0);
	}

	void ViewPrevious() => MoveCurrentSpread(-1);

	void ViewNext() => MoveCurrentSpread(1);

	FocusMode _focusMode = FocusMode.None;
	const int CloseHeight = 20;
	readonly SolidBrush _closeBrush = new(Color.FromArgb(64, 255, 0, 0));

	#region MainPreviewer ViewPane EventHandlers

	void OnMainPreviewerViewPaneMouseMove(object? sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.None && (_comic.BindingSide == BindingSide.Left ? e.X >= MainPreviewer.ClientSize.Width - Properties.Resources.Next.Width * DeviceDpi / 96 : e.X <= Properties.Resources.Next.Width * DeviceDpi / 96))
		{
			_focusMode = FocusMode.Next;
			MainPreviewer.Cursor = Cursors.Default;
		}
		else if (e.Button == MouseButtons.None && (_comic.BindingSide == BindingSide.Left ? e.X <= Properties.Resources.Previous.Width : e.X >= MainPreviewer.ClientSize.Width - Properties.Resources.Previous.Width * DeviceDpi / 96))
		{
			_focusMode = FocusMode.Previous;
			MainPreviewer.Cursor = Cursors.Default;
		}
		else if (e.Button == MouseButtons.None && e.Y >= MainPreviewer.ClientSize.Height - CloseHeight * DeviceDpi / 96)
		{
			_focusMode = FocusMode.Close;
			MainPreviewer.Cursor = Cursors.Default;
		}
		else
		{
			_focusMode = FocusMode.None;
			MainPreviewer.Cursor = null;
		}
		MainPreviewer.Invalidate();
	}

	void OnMainPreviewerViewPaneMouseUp(object? sender, MouseEventArgs e)
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
			MainPreviewer.StretchMode = MainPreviewer.StretchMode == Comical.Controls.PreviewerStretchMode.Uniform ? Comical.Controls.PreviewerStretchMode.None : Comical.Controls.PreviewerStretchMode.Uniform;
	}

	void OnMainPreviewerViewPanePaint(object? sender, PaintEventArgs e)
	{
		var g = e.Graphics;
		if (_focusMode == FocusMode.Close)
			g.FillRectangle(_closeBrush, 0, MainPreviewer.ClientSize.Height - CloseHeight * DeviceDpi / 96, MainPreviewer.ClientSize.Width, CloseHeight * DeviceDpi / 96);
		else if (_focusMode != FocusMode.None)
		{
			var img = (Bitmap?)Properties.Resources.ResourceManager.GetObject(_focusMode.ToString(), Properties.Resources.Culture);
			Debug.Assert(img != null);
			var y = (MainPreviewer.ClientSize.Height - img.Height * DeviceDpi / 96) / 2;
			if (_comic.BindingSide == BindingSide.Left ^ _focusMode == FocusMode.Next)
				g.DrawImage(img, 0, y);
			else
				g.DrawImage(img, MainPreviewer.ClientSize.Width - img.Width * DeviceDpi / 96, y);
		}
	}

	void OnMainPreviewerViewPaneKeyDown(object? sender, KeyEventArgs e)
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
