using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns
{
    //[ToolboxBitmap(typeof(BeepDataGridViewButtonColumn), "DataGridViewButtonColumn.bmp")]
    //[ToolboxItem(true)]
  
    public class BeepDataGridViewButtonColumn : DataGridViewColumn
    {
        public BeepDataGridViewButtonColumn()
            : base(new DataGridViewCustomButtonCell())
        {
        }

        public override object Clone()
        {
            BeepDataGridViewButtonColumn column = (BeepDataGridViewButtonColumn)base.Clone();
            return column;
        }

        // Allow custom button click handler.
        public event EventHandler<DataGridViewCellEventArgs> ButtonClick;

        internal void RaiseCellClickEvent(DataGridViewCellEventArgs e)
        {
            ButtonClick?.Invoke(this, e);
        }
    }
    public class DataGridViewCustomButtonCell : DataGridViewButtonCell
    {
        public string ButtonText { get; set; }
        public Image ButtonIcon { get; set; }

        public DataGridViewCustomButtonCell()
        {
            ButtonText = "Click Me"; // Default text
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, null, null, errorText, cellStyle, advancedBorderStyle, (paintParts & ~DataGridViewPaintParts.ContentForeground));

            // Draw the button text or icon
            if (ButtonIcon != null)
            {
                graphics.DrawImage(ButtonIcon, cellBounds.X + 2, cellBounds.Y + 2, 16, 16);
            }

            if (!string.IsNullOrEmpty(ButtonText))
            {
                TextRenderer.DrawText(graphics, ButtonText, cellStyle.Font, cellBounds, cellStyle.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        public override object Clone()
        {
            DataGridViewCustomButtonCell clone = (DataGridViewCustomButtonCell)base.Clone();
            clone.ButtonText = this.ButtonText;
            clone.ButtonIcon = this.ButtonIcon;
            return clone;
        }
    }
}
