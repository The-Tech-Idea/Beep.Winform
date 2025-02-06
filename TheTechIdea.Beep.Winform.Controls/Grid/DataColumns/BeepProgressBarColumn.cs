using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepProgressBarColumn : DataGridViewColumn
    {
        public BeepProgressBarColumn() : base(new BeepProgressBarCell())
        {
            this.CellTemplate = new BeepProgressBarCell();
        }
    }
    public class BeepProgressBarCell : DataGridViewCell
    {
        private BeepProgressBar beepProgressBar;

        public BeepProgressBarCell()
        {
            beepProgressBar = new BeepProgressBar
            {
                Size = new Size(100, 20), // Default size, adjusted dynamically
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                VisualMode = ProgressBarDisplayMode.Percentage,
            };
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, formattedValue, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            int progressValue = 0;
            if (value != null && int.TryParse(value.ToString(), out progressValue))
            {
                beepProgressBar.Value = progressValue;
            }
            else
            {
                beepProgressBar.Value = 0;
            }

            beepProgressBar.Bounds = cellBounds;
            beepProgressBar.DrawToBitmap(new Bitmap(cellBounds.Width, cellBounds.Height), cellBounds);
            graphics.DrawImage(beepProgressBar.BackgroundImage, cellBounds.Location);
        }

        protected override object GetValue(int rowIndex)
        {
            return beepProgressBar.Value;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value != null && int.TryParse(value.ToString(), out int newValue))
            {
                beepProgressBar.Value = newValue;
                return true; // Successfully updated the value
            }
            return false; // Indicate that setting the value was unsuccessful
        }
    }
}
