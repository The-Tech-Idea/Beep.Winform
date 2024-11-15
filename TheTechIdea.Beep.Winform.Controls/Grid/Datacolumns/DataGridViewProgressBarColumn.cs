using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns
{
    public class DataGridViewProgressBarCell : DataGridViewTextBoxCell
    {
        public DataGridViewProgressBarCell()
        {
            // Default value type for the cell is integer
            this.ValueType = typeof(int);
        }

        // Override the Paint method to draw the progress bar
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                                      DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
                                      DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {
            // Get the owning column to access the Minimum, Maximum, and Step properties
            BeepDataGridViewProgressBarColumn owningColumn = this.OwningColumn as BeepDataGridViewProgressBarColumn;

            // Use the minimum, maximum, and step values defined in the column
            int minimum = owningColumn?.Minimum ?? 0;
            int maximum = owningColumn?.Maximum ?? 100;
            int step = owningColumn?.Step ?? 1;

            // If the value is null, set it to the minimum value
            int progressVal = value != null && int.TryParse(value.ToString(), out int result) ? result : minimum;

            // Ensure value is within the bounds of minimum and maximum
            progressVal = Math.Max(minimum, Math.Min(maximum, progressVal));

            // Calculate the percentage for the progress bar fill
            float percentage = (float)(progressVal - minimum) / (maximum - minimum);

            // Base paint to handle cell borders, etc.
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, null, null, errorText, cellStyle, advancedBorderStyle, (paintParts & ~DataGridViewPaintParts.ContentForeground));

            // Determine progress bar color and fill
            Color progressBarColor = owningColumn.ProgressBarColor;
            using (Brush progressBarBrush = new SolidBrush(progressBarColor))
            {
                // Draw progress bar
                int barWidth = (int)(percentage * (cellBounds.Width - 4));
                Rectangle progressBarRect = new Rectangle(cellBounds.X + 2, cellBounds.Y + 2, barWidth, cellBounds.Height - 4);
                graphics.FillRectangle(progressBarBrush, progressBarRect);
            }

            // Draw percentage text
            string text = $"{progressVal}%";
            SizeF textSize = graphics.MeasureString(text, cellStyle.Font);
            using (Brush textBrush = new SolidBrush(cellStyle.ForeColor))
            {
                float textX = cellBounds.X + (cellBounds.Width - textSize.Width) / 2;
                float textY = cellBounds.Y + (cellBounds.Height - textSize.Height) / 2;
                graphics.DrawString(text, cellStyle.Font, textBrush, textX, textY);
            }
        }
    }

    [ToolboxItem(true)]
    public class BeepDataGridViewProgressBarColumn : DataGridViewColumn
    {
        public BeepDataGridViewProgressBarColumn()
        {
            // Set the cell template to be a DataGridViewProgressBarCell
            this.CellTemplate = new DataGridViewProgressBarCell();
        }

        // Property to customize progress bar color
        public Color ProgressBarColor { get; set; } = Color.LightGreen; // Default color is light green

        // Minimum value for the progress bar
        private int _minimum = 0;
        public int Minimum
        {
            get { return _minimum; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Minimum), "Minimum value must be 0 or greater.");
                _minimum = value;
            }
        }

        // Maximum value for the progress bar
        private int _maximum = 100;
        public int Maximum
        {
            get { return _maximum; }
            set
            {
                if (value <= _minimum)
                    throw new ArgumentOutOfRangeException(nameof(Maximum), "Maximum must be greater than Minimum.");
                _maximum = value;
            }
        }

        // Step value for the progress bar
        private int _step = 1;
        public int Step
        {
            get { return _step; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Step), "Step must be greater than 0.");
                _step = value;
            }
        }

        // Clone method to ensure properties are copied when column is duplicated
        public override object Clone()
        {
            var clone = (BeepDataGridViewProgressBarColumn)base.Clone();
            clone.ProgressBarColor = this.ProgressBarColor;
            clone.Minimum = this.Minimum;
            clone.Maximum = this.Maximum;
            clone.Step = this.Step;
            return clone;
        }
    }
}
