using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
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
			prevMain.ViewPane.ContextMenuStrip = conBookmarks;
			prevMain.ViewPane.MouseMove += picPreview_MouseMove;
			prevMain.ViewPane.MouseUp += picPreview_MouseUp;
			prevMain.ViewPane.Paint += picPreview_Paint;
			prevMain.ViewPane.KeyDown += picPreview_KeyDown;
			if (AeroGlassCompositionEnabled)
				ExtendFrame(new Padding(-1));
		}

		public ViewerForm(string fileName) : this()
		{
			icd = new Comic(); // Construct with read-only mode.
			openingFileName = fileName;
		}

		Comic icd;
		string openingFileName = "";
		List<Spread> spreads;
		int current;

		void Open(string fileName)
		{
			string pass = "";
			TaskDialogResult result;
			do
			{
				if (!FileHeader.Create(fileName).IsProperPassword(""))
				{
					using (PasswordDialog dialog = new PasswordDialog())
					{
						dialog.Creating = false;
						if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
							pass = dialog.Password;
						else
						{
							result = TaskDialogResult.Close;
							break;
						}
					}
				}
				Activate();
				prevMain.ViewPane.Select();
				using (TaskDialog dialog = new TaskDialog())
				{
					CancellationTokenSource cts = new CancellationTokenSource();
					dialog.Cancelable = true;
					dialog.StandardButtons = TaskDialogStandardButtons.Cancel;
					dialog.Caption = Application.ProductName;
					dialog.Icon = TaskDialogStandardIcon.None;
					dialog.InstructionText = Properties.Resources.OpeningFile;
					dialog.OwnerWindowHandle = this.Handle;
					dialog.ProgressBar = new TaskDialogProgressBar(0, 100, 0);
					dialog.Opened += async (s, ev) =>
					{
						try
						{
							await icd.OpenAsync(fileName, pass, true, cts.Token, new Progress<int>(value => dialog.ProgressBar.Value = value));
							dialog.Close(TaskDialogResult.Ok);
						}
						catch (OperationCanceledException)
						{
							dialog.Close(TaskDialogResult.Close);
						}
						catch (WrongPasswordException ex)
						{
							using (TaskDialog td = new TaskDialog())
							{
								td.Caption = Application.ProductName;
								td.Icon = TaskDialogStandardIcon.Error;
								td.InstructionText = ex.Message;
								td.OwnerWindowHandle = this.Handle;
								td.StandardButtons = TaskDialogStandardButtons.Retry | TaskDialogStandardButtons.Cancel;
								td.StartupLocation = TaskDialogStartupLocation.CenterOwner;
								if (td.Show() == TaskDialogResult.Retry)
									dialog.Close(TaskDialogResult.Retry);
								else
									dialog.Close(TaskDialogResult.Close);
							}
						}
					};
					dialog.Closing += (s, ev) =>
					{
						if (ev.TaskDialogResult == TaskDialogResult.Cancel)
						{
							cts.Cancel();
							ev.Cancel = true;
						}
					};
					dialog.StartupLocation = TaskDialogStartupLocation.CenterOwner;
					result = dialog.Show();
				}
			} while (result == TaskDialogResult.Retry);
			if (result == TaskDialogResult.Ok)
			{
				conBookmarks.Items.AddRange(icd.Bookmarks.Select(b => new ToolStripMenuItem(b.Name, null, (sen, eve) =>
				{
					current = spreads.FindIndex(sp => sp.Left == b.TargetImage || sp.Right == b.TargetImage);
					ViewCurrentPage();
				})).ToArray());
				spreads = new List<Spread>(icd.ConstructSpreads(!Properties.Settings.Default.UsePageView));
				openingFileName = "";
				ViewCurrentPage();
			}
			else if (result == TaskDialogResult.Close)
				Close();
		}

		void ViewCurrentPage()
		{
			Image im;
			if (spreads[current].Left != null)
			{
				im = spreads[current].Left.GetImage();
				if (spreads[current].Right != null)
				{
					Image other = spreads[current].Right.GetImage();
					Bitmap bmp = new Bitmap(im.Width + other.Width, Math.Max(im.Height, other.Height));
					try
					{
						using (Graphics g = Graphics.FromImage(bmp))
						{
							g.DrawImage(im, new Point(0, 0));
							g.DrawImage(other, new Point(im.Width, 0));
						}
						im = bmp;
						bmp = null;
					}
					finally
					{
						if (bmp != null)
							bmp.Dispose();
					}
				}
			}
			else
				im = spreads[current].Right.GetImage();
			prevMain.Image = im;
		}

		void ViewPrevious()
		{
			current = (spreads.Count + current - 1) % spreads.Count;
			ViewCurrentPage();
		}

		void ViewNext()
		{
			current = (current + 1) % spreads.Count;
			ViewCurrentPage();
		}

		FocusMode focusMode = FocusMode.None;
		const int closeHeight = 20;
		SolidBrush closeBrush = new SolidBrush(Color.FromArgb(64, 255, 0, 0));

		#region picPreview EventHandlers

		void picPreview_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.None)
			{
				if (icd.PageTurningDirection == PageTurningDirection.ToLeft ? e.X >= Math.Min(prevMain.ViewPane.ClientSize.Width, prevMain.ClientSize.Width) - prevMain.AutoScrollPosition.X - Properties.Resources.Next.Width : e.X <= Properties.Resources.Next.Width - prevMain.AutoScrollPosition.X)
				{
					focusMode = FocusMode.Next;
					prevMain.Cursor = Cursors.Default;
				}
				else if (icd.PageTurningDirection == PageTurningDirection.ToLeft ? e.X <= Properties.Resources.Previous.Width - prevMain.AutoScrollPosition.X : e.X >= Math.Min(prevMain.ViewPane.ClientSize.Width, prevMain.ClientSize.Width) - prevMain.AutoScrollPosition.X - Properties.Resources.Previous.Width)
				{
					focusMode = FocusMode.Previous;
					prevMain.Cursor = Cursors.Default;
				}
				else if (e.Y >= prevMain.ClientSize.Height - closeHeight - prevMain.AutoScrollPosition.Y)
				{
					focusMode = FocusMode.Close;
					prevMain.Cursor = Cursors.Default;
				}
				else
				{
					focusMode = FocusMode.None;
					prevMain.Cursor = null;
				}
				prevMain.Invalidate();
			}
		}

		void picPreview_MouseUp(object sender, MouseEventArgs e)
		{
			if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
			{
				if (focusMode == FocusMode.Previous)
					ViewPrevious();
				else if (focusMode == FocusMode.Next)
					ViewNext();
				else if (focusMode == FocusMode.Close)
					this.Close();
			}
			if (e.Button == System.Windows.Forms.MouseButtons.XButton2)
				ViewPrevious();
			else if (e.Button == System.Windows.Forms.MouseButtons.XButton1)
				ViewNext();
			else if (e.Button == System.Windows.Forms.MouseButtons.Middle)
				prevMain.StretchMode = prevMain.StretchMode == Comical.Controls.PreviewerStretchMode.Uniform ? Comical.Controls.PreviewerStretchMode.None : Comical.Controls.PreviewerStretchMode.Uniform;
		}

		void picPreview_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if (focusMode == FocusMode.Next)
			{
				var img = Properties.Resources.Next;
				var y = (prevMain.ClientSize.Height - img.Height) / 2 - prevMain.AutoScrollPosition.Y;
				if (icd.PageTurningDirection == PageTurningDirection.ToLeft)
					g.DrawImage(img, Math.Min(prevMain.ClientSize.Width, prevMain.ViewPane.ClientSize.Width) - img.Width - prevMain.AutoScrollPosition.X, y);
				else
					g.DrawImage(img, -prevMain.AutoScrollPosition.X, y);
			}
			else if (focusMode == FocusMode.Previous)
			{
				var img = Properties.Resources.Previous;
				var y = (prevMain.ClientSize.Height - img.Height) / 2 - prevMain.AutoScrollPosition.Y;
				if (icd.PageTurningDirection == PageTurningDirection.ToLeft)
					g.DrawImage(img, -prevMain.AutoScrollPosition.X, y);
				else
					g.DrawImage(img, Math.Min(prevMain.ClientSize.Width, prevMain.ViewPane.ClientSize.Width) - img.Width - prevMain.AutoScrollPosition.X, y);
			}
			else if (focusMode == FocusMode.Close)
				g.FillRectangle(closeBrush, -prevMain.AutoScrollPosition.X, prevMain.ClientSize.Height - closeHeight - prevMain.AutoScrollPosition.Y, prevMain.ViewPane.ClientSize.Width, closeHeight);
		}

		void picPreview_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				this.Close();
			else if (e.KeyCode == (icd.PageTurningDirection == PageTurningDirection.ToLeft ? Keys.Left : Keys.Right))
				ViewPrevious();
			else if (e.KeyCode == (icd.PageTurningDirection == PageTurningDirection.ToLeft ? Keys.Right : Keys.Left))
				ViewNext();
		}

		#endregion

		protected override void OnClosing(CancelEventArgs e)
		{
			if (e != null && icd.IsBusy)
				e.Cancel = true;
			base.OnClosing(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			Open(openingFileName);
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


