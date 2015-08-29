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
				if (!FileHeader.Load(fileName).IsProperPassword(""))
				{
					using (PasswordDialog dialog = new PasswordDialog())
					{
						dialog.Creating = false;
						if (dialog.ShowDialog(this) == DialogResult.OK)
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
					dialog.Cancelable = false;
					dialog.Controls.Add(new TaskDialogButton("btnCancel", Properties.Resources.Cancel));
					dialog.Caption = Application.ProductName;
					dialog.Icon = TaskDialogStandardIcon.None;
					dialog.InstructionText = Properties.Resources.OpeningFile;
					dialog.OwnerWindowHandle = Handle;
					dialog.ProgressBar = new TaskDialogProgressBar(0, 100, 0);
					dialog.Opened += async (s, ev) =>
					{
						((TaskDialogButtonBase)dialog.Controls["btnCancel"]).Enabled = false;
						try
						{
							await icd.OpenAsync(fileName, pass, new Progress<int>(value => dialog.ProgressBar.Value = value));
							dialog.Close(TaskDialogResult.Ok);
						}
						catch (OperationCanceledException)
						{
							dialog.Close(TaskDialogResult.Close);
						}
						catch (WrongPasswordException ex)
						{
							if (TaskDialog.Show(ex.Message, null, Application.ProductName, TaskDialogStandardButtons.Retry | TaskDialogStandardButtons.Cancel, TaskDialogStandardIcon.Error, ownerWindowHandle: Handle) == TaskDialogResult.Retry)
								dialog.Close(TaskDialogResult.Retry);
							else
								dialog.Close(TaskDialogResult.Close);
						}
					};
					dialog.StartupLocation = TaskDialogStartupLocation.CenterOwner;
					result = dialog.Show();
				}
			} while (result == TaskDialogResult.Retry);
			if (result == TaskDialogResult.Close)
			{
				Close();
				return;
			}
			conBookmarks.Items.AddRange(icd.Bookmarks.Select(b => new ToolStripMenuItem(b.Name, null, (sen, eve) =>
			{
				current = spreads.FindIndex(sp => sp.Left == b.Target || sp.Right == b.Target);
				ViewCurrentPage();
			})).ToArray());
			spreads = new List<Spread>(icd.ConstructSpreads(false));
			openingFileName = "";
			ViewCurrentPage();
		}

		void ViewCurrentPage()
		{
			if (prevMain.Image != null)
			{
				prevMain.Image.Dispose();
				prevMain.Image = null;
			}
			if (spreads[current].Left == null)
			{
				prevMain.Image = icd.Images[(int)spreads[current].Right].GetImage();
				return;
			}
			if (spreads[current].Right == null)
			{
				prevMain.Image = icd.Images[(int)spreads[current].Left].GetImage();
				return;
			}
			using (var left = icd.Images[(int)spreads[current].Left].GetImage())
			using (var right = icd.Images[(int)spreads[current].Right].GetImage())
			{
				prevMain.Image = new Bitmap(left.Width + right.Width, Math.Max(left.Height, right.Height));
				using (Graphics g = Graphics.FromImage(prevMain.Image))
				{
					g.DrawImage(left, new Point(0, 0));
					g.DrawImage(right, new Point(left.Width, 0));
				}
			}
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
			if (e.Button == MouseButtons.None && (icd.PageTurningDirection == PageTurningDirection.ToLeft ? e.X >= Math.Min(prevMain.ViewPane.ClientSize.Width, prevMain.ClientSize.Width) - prevMain.AutoScrollPosition.X - Properties.Resources.Next.Width : e.X <= Properties.Resources.Next.Width - prevMain.AutoScrollPosition.X))
			{
				focusMode = FocusMode.Next;
				prevMain.Cursor = Cursors.Default;
			}
			else if (e.Button == MouseButtons.None && (icd.PageTurningDirection == PageTurningDirection.ToLeft ? e.X <= Properties.Resources.Previous.Width - prevMain.AutoScrollPosition.X : e.X >= Math.Min(prevMain.ViewPane.ClientSize.Width, prevMain.ClientSize.Width) - prevMain.AutoScrollPosition.X - Properties.Resources.Previous.Width))
			{
				focusMode = FocusMode.Previous;
				prevMain.Cursor = Cursors.Default;
			}
			else if (e.Button == MouseButtons.None && e.Y >= prevMain.ClientSize.Height - closeHeight - prevMain.AutoScrollPosition.Y)
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

		void picPreview_MouseUp(object sender, MouseEventArgs e)
		{
			if ((e.Button & MouseButtons.Left) != 0)
			{
				if (focusMode == FocusMode.Previous)
					ViewPrevious();
				else if (focusMode == FocusMode.Next)
					ViewNext();
				else if (focusMode == FocusMode.Close)
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
			Graphics g = e.Graphics;
			if (focusMode == FocusMode.Close)
				g.FillRectangle(closeBrush, -prevMain.AutoScrollPosition.X, prevMain.ClientSize.Height - closeHeight - prevMain.AutoScrollPosition.Y, prevMain.ViewPane.ClientSize.Width, closeHeight);
			else if (focusMode != FocusMode.None)
			{
				var img = (Bitmap)Properties.Resources.ResourceManager.GetObject(focusMode.ToString(), Properties.Resources.Culture);
				var y = (prevMain.ClientSize.Height - img.Height) / 2 - prevMain.AutoScrollPosition.Y;
				if (icd.PageTurningDirection == PageTurningDirection.ToLeft ^ focusMode == FocusMode.Next)
					g.DrawImage(img, -prevMain.AutoScrollPosition.X, y);
				else
					g.DrawImage(img, Math.Min(prevMain.ClientSize.Width, prevMain.ViewPane.ClientSize.Width) - img.Width - prevMain.AutoScrollPosition.X, y);
			}
		}

		void picPreview_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
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


