using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace Comical.Controls;

public class SuffixNumericUpDown : NumericUpDown
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public string Suffix
	{
		get;
		set
		{
			field = value;
			UpdateEditText();
		}
	} = string.Empty;

	public override void DownButton()
	{
		if (UserEdit) ParseEditText();
		base.DownButton();
	}
	public override void UpButton()
	{
		if (UserEdit) ParseEditText();
		base.UpButton();
	}
	protected override void ValidateEditText()
	{
		ParseEditText();
		UpdateEditText();
	}
	protected override void UpdateEditText()
	{
		if (UserEdit) ParseEditText();
		Text = Value.ToString($"{(ThousandsSeparator ? "N" : "F")}{DecimalPlaces}", CultureInfo.CurrentCulture) + Suffix;
	}
	protected new void ParseEditText()
	{
		var text = Text.AsSpan();
		if (decimal.TryParse(text[text.EndsWith(Suffix, StringComparison.Ordinal) ? ..^Suffix.Length : ..],
			CultureInfo.CurrentCulture, out var value))
			Value = Math.Clamp(value, Minimum, Maximum);
		UserEdit = false;
	}
}
