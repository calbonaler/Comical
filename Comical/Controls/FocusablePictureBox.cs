using System;
using System.Windows.Forms;

namespace Comical.Controls
{
	public class FocusablePictureBox : PictureBox
	{
		public FocusablePictureBox()
		{
			SetStyle(ControlStyles.Selectable, true);
			TabStop = true;
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Always)]
		public new int TabIndex
		{
			get { return base.TabIndex; }
			set { base.TabIndex = value; }
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Always)]
		public new bool TabStop
		{
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if ((keyData & Keys.Up) != 0 || (keyData & Keys.Down) != 0 || (keyData & Keys.Left) != 0 || (keyData & Keys.Right) != 0)
				return true;
			return base.IsInputKey(keyData);
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			Focus();
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Always)]
		public new event KeyEventHandler KeyDown
		{
			add { base.KeyDown += value; }
			remove { base.KeyDown -= value; }
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Always)]
		public new event KeyEventHandler KeyUp
		{
			add { base.KeyUp += value; }
			remove { base.KeyUp -= value; }
		}

		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Always)]
		public new event KeyPressEventHandler KeyPress
		{
			add { base.KeyPress += value; }
			remove { base.KeyPress -= value; }
		}
	}
}
