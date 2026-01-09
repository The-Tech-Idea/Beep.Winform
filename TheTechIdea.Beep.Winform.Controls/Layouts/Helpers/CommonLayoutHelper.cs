using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Common helper methods for layout operations.
    /// Provides reusable methods for creating table cells and detail rows with theme-aware styling.
    /// </summary>
    public static class CommonLayoutHelper
    {
        /// <summary>
        /// Adds a centered label to a TableLayoutPanel cell with theme-aware styling.
        /// </summary>
        /// <param name="table">The TableLayoutPanel to add the label to.</param>
        /// <param name="text">The text to display in the label.</param>
        /// <param name="column">The column index.</param>
        /// <param name="row">The row index.</param>
        /// <param name="options">Optional layout configuration for theming. If null, uses default styling.</param>
        public static void AddTableCellLabel(TableLayoutPanel table, string text, int column, int row, LayoutOptions options = null)
        {
            var label = options != null 
                ? BaseLayoutHelper.CreateStyledLabel(text, options, ContentAlignment.MiddleCenter)
                : new Label
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
            
            table.Controls.Add(label, column, row);
        }

        /// <summary>
        /// Adds a detail row (label-value pair) to a TableLayoutPanel with theme-aware styling.
        /// </summary>
        /// <param name="table">The TableLayoutPanel to add the row to.</param>
        /// <param name="rowIndex">The row index where the detail row will be added.</param>
        /// <param name="labelText">The label text (typically displayed on the right).</param>
        /// <param name="valueText">The value text (typically displayed on the left).</param>
        /// <param name="options">Optional layout configuration for theming. If null, uses default styling.</param>
        public static void AddDetailRow(TableLayoutPanel table, int rowIndex, string labelText, string valueText, LayoutOptions options = null)
        {
            var label = options != null
                ? BaseLayoutHelper.CreateStyledLabel(labelText, options, ContentAlignment.MiddleRight)
                : new Label
                {
                    Text = labelText,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight
                };

            var valueLabel = options != null
                ? BaseLayoutHelper.CreateStyledLabel(valueText, options, ContentAlignment.MiddleLeft)
                : new Label
                {
                    Text = valueText,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft
                };

            table.Controls.Add(label, 0, rowIndex);
            table.Controls.Add(valueLabel, 1, rowIndex);
        }
    }
}
