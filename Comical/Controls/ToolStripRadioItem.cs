using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Comical.Controls;

[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.MenuStrip)]
public partial class ToolStripRadioMenuItem : ToolStripMenuItem
{
	protected override void OnCheckedChanged(EventArgs e)
	{
		base.OnCheckedChanged(e);
		if (CheckState == CheckState.Unchecked || Owner == null)
			return;
		foreach (var item in Owner.Items)
		{
			if (item is ToolStripRadioMenuItem it && it != this && it.Group == Group)
				it.CheckState = CheckState.Unchecked;
		}
	}

	protected override void OnClick(EventArgs e)
	{
		CheckState = CheckState.Indeterminate;
		base.OnClick(e);
	}

	public int Group { get; set; }
}
