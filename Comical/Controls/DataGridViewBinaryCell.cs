using System;
using System.Drawing;
using System.Windows.Forms;
using Comical.Core;

namespace Comical.Controls
{
	public class DataGridViewBinaryColumn : DataGridViewColumn
	{
		public DataGridViewBinaryColumn() : base(new DataGridViewBinaryCell()) { }

		public override DataGridViewCell CellTemplate
		{
			get => base.CellTemplate;
			set
			{
				if (value is not null and not DataGridViewBinaryCell)
					throw new InvalidCastException($"{nameof(CellTemplate)}には{nameof(DataGridViewBinaryCell)}を指定してください。");
				base.CellTemplate = value;
			}
		}
	}

	public class DataGridViewBinaryCell : DataGridViewCell
	{
		public override object DefaultNewRowValue => null;

		public override Type EditType => null;

		public override Type ValueType => typeof(Binary);

		protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			// 必要な場合は画像を準備
			var showImage = (paintParts & DataGridViewPaintParts.ContentForeground) != 0 && formattedValue != null;
			using var image = !showImage ? null : ((Binary)formattedValue).ToImage();

			// セルの境界線（枠）を描画する
			if ((paintParts & DataGridViewPaintParts.Border) != 0)
				PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);

			// 境界線の内側に範囲を取得する
			var borderWidths = BorderWidths(advancedBorderStyle);
			var paddingRect = new Rectangle(
				cellBounds.Left + borderWidths.Left,
				cellBounds.Top + borderWidths.Top,
				cellBounds.Width - borderWidths.Right,
				cellBounds.Height - borderWidths.Bottom);

			// 背景色を決定する
			// 選択されている時とされていない時で色を変える
			var isSelected = (cellState & DataGridViewElementStates.Selected) != 0;
			var backColor = (paintParts & DataGridViewPaintParts.SelectionBackground) != 0 && isSelected ? cellStyle.SelectionBackColor : cellStyle.BackColor;
			// 背景を描画する
			if ((paintParts & DataGridViewPaintParts.Background) != 0)
			{
				using var brush = new SolidBrush(backColor);
				graphics.FillRectangle(brush, paddingRect);
			}

			// Paddingを差し引く
			var contentRect = paddingRect;
			if (DataGridView.RightToLeft == RightToLeft.Yes)
				contentRect.Offset(cellStyle.Padding.Right, cellStyle.Padding.Top);
			else
				contentRect.Offset(cellStyle.Padding.Left, cellStyle.Padding.Top);
			contentRect.Width -= cellStyle.Padding.Horizontal;
			contentRect.Height -= cellStyle.Padding.Vertical;

			// 画像を表示
			if (showImage)
			{
				var scaledSize = Utils.ScaleSize(image.Size, contentRect.Size);
				var sizeDiff = contentRect.Size - scaledSize;
				sizeDiff = new Size(sizeDiff.Width / 2, sizeDiff.Height / 2);
				graphics.DrawImage(image, new Rectangle(contentRect.Location + sizeDiff, scaledSize));
			}

			// フォーカスの枠を表示
			if ((paintParts & DataGridViewPaintParts.Focus) != 0 && DataGridView.CurrentCellAddress.X == ColumnIndex && DataGridView.CurrentCellAddress.Y == RowIndex && DataGridView.Focused)
				ControlPaint.DrawFocusRectangle(graphics, paddingRect, Color.Empty, backColor);

			// エラーアイコンの表示
			if ((paintParts & DataGridViewPaintParts.ErrorIcon) != 0 && DataGridView.ShowCellErrors)
				PaintErrorIcon(graphics, default, paddingRect, errorText);
		}
	}
}
