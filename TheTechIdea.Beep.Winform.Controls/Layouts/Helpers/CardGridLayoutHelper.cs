using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Helper for creating card-based grid layouts.
    /// Creates a responsive grid of BeepCard controls with theme-aware styling.
    /// </summary>
    public static class CardGridLayoutHelper
    {
        /// <summary>
        /// Builds a card grid layout with default options.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="rows">Number of rows in the grid. Must be greater than 0.</param>
        /// <param name="columns">Number of columns in the grid. Must be greater than 0.</param>
        /// <returns>The TableLayoutPanel containing the card grid.</returns>
        public static Control Build(Control parent, int rows, int columns)
        {
            return Build(parent, rows, columns, LayoutOptions.Default);
        }

        /// <summary>
        /// Builds a card grid layout with theme-aware styling.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="rows">Number of rows in the grid. Must be greater than 0.</param>
        /// <param name="columns">Number of columns in the grid. Must be greater than 0.</param>
        /// <param name="options">Layout configuration options for theming and styling.</param>
        /// <returns>The TableLayoutPanel containing the card grid.</returns>
        public static Control Build(Control parent, int rows, int columns, LayoutOptions options)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (rows <= 0)
                throw new ArgumentException("Rows must be greater than 0", nameof(rows));
            if (columns <= 0)
                throw new ArgumentException("Columns must be greater than 0", nameof(columns));

            options = options ?? LayoutOptions.Default;

            var table = new TableLayoutPanel 
            { 
                RowCount = rows, 
                ColumnCount = columns, 
                Dock = DockStyle.Fill,
                BackColor = BaseLayoutHelper.GetBackgroundColor(options)
            };

            for (int c = 0; c < columns; c++) 
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));
            
            for (int r = 0; r < rows; r++) 
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    var card = BaseLayoutHelper.CreateBeepCard(options);
                    card.Dock = DockStyle.Fill;
                    card.Margin = new Padding(options.Spacing);
                    
                    // Add a sample label to the card
                    var label = BaseLayoutHelper.CreateStyledLabel($"Card {r * columns + c + 1}", options, ContentAlignment.MiddleCenter);
                    label.Dock = DockStyle.Fill;
                    card.Controls.Add(label);

                    table.Controls.Add(card, c, r);
                }
            }

            parent.Controls.Add(table);
            return table;
        }
    }
}
