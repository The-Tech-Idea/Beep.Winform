using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Data")]
    [Description("A grid control that displays data in a pivot table format.")]
    [DisplayName("Beep Pivot Grid")]
    public class BeepPivotGrid : BeepSimpleGrid
    {
        public int RowHeaderWidth { get; set; } = 100;
        private bool _showrowheaderborders=true;
        private string _columnsHeaderText =string.Empty;
        public string ColumnsHeaderText
        {
            get { return _columnsHeaderText; }
            set { _columnsHeaderText = value; }
        }
        private string _rowsHeaderText = string.Empty;
        public string RowsHeaderText
        {
            get { return _rowsHeaderText; }
            set { _rowsHeaderText = value; }
        }
        public bool ShowRowHeaderBorders
        {
            get { return _showrowheaderborders; }
            set { _showrowheaderborders = value; }
        }

        public Rectangle topLeftRect { get; private set; }

        public BeepPivotGrid()
        {
            XOffset = RowHeaderWidth;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Call base to retain BeepSimpleGrid's painting logic

            int yOffset = ColumnHeight + headerPanelHeight + 2;

            // Define top-left header rectangle area
            topLeftRect = new Rectangle(XOffset - RowHeaderWidth, headerPanelHeight, RowHeaderWidth, _rowHeight);

            // Draw each row header
            foreach (var row in Rows)
            {
                // Define the rectangle for each row header
                var headerRect = new Rectangle(XOffset - RowHeaderWidth, yOffset, RowHeaderWidth, _rowHeight);

                // Create and configure a BeepLabel for the row header
                var headerLabel = new BeepLabel
                {
                    Text = row.RowName ?? "Row",
                    Location = new Point(headerRect.X, headerRect.Y),
                    Size = headerRect.Size,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Theme = Theme,
                    BackColor = _currentTheme.HeaderBackColor,
                    ForeColor = _currentTheme.PrimaryTextColor
                };

                // Apply the theme to the header label
                headerLabel.ApplyTheme(_currentTheme);

                // Draw the header label within the defined header rectangle
                headerLabel.DrawToGraphics(e.Graphics, headerRect);

                // Optionally, draw a border around each row header
                if (ShowRowHeaderBorders)
                {
                    using (var pen = new Pen(_currentTheme.BorderColor, 1))
                    {
                        e.Graphics.DrawRectangle(pen, headerRect);
                    }
                }

                yOffset += _rowHeight; // Move to the next row's position
            }

            // Draw the pivot header, including the column and row header labels
            DrawHeader(e);
        }

        private void DrawHeader(PaintEventArgs e)
        {
            int yOffset = headerPanelHeight;
            int lineYPosition = ColumnHeight / 2; // Position for the horizontal dividing line

            // Draw a thin horizontal line between the headers
            using (var pen = new Pen(_currentTheme.BorderColor, 1))
            {
                e.Graphics.DrawLine(pen, DrawingRect.Left, lineYPosition + yOffset, RowHeaderWidth, lineYPosition + yOffset);
            }

            // Define rectangle areas for the column and row headers
            var columnHeaderRect = new Rectangle(0, yOffset, RowHeaderWidth, lineYPosition);
            var rowHeaderRect = new Rectangle(0, lineYPosition + yOffset, RowHeaderWidth, lineYPosition);

            // Draw Column Header Label
            if (!string.IsNullOrEmpty(ColumnsHeaderText))
            {
                var columnHeaderLabel = new BeepLabel
                {
                    Text = ColumnsHeaderText,
                    Location = columnHeaderRect.Location,
                    Size = columnHeaderRect.Size,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Theme = Theme,
                    BackColor = _currentTheme.HeaderBackColor,
                    ForeColor = _currentTheme.PrimaryTextColor
                };
                columnHeaderLabel.ApplyTheme(_currentTheme);
                columnHeaderLabel.DrawToGraphics(e.Graphics, columnHeaderRect);
            }

            // Draw Row Header Label
            if (!string.IsNullOrEmpty(RowsHeaderText))
            {
                var rowHeaderLabel = new BeepLabel
                {
                    Text = RowsHeaderText,
                    Location = rowHeaderRect.Location,
                    Size = rowHeaderRect.Size,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Theme = Theme,
                    BackColor = _currentTheme.HeaderBackColor,
                    ForeColor = _currentTheme.PrimaryTextColor
                };
                rowHeaderLabel.ApplyTheme(_currentTheme);
                rowHeaderLabel.DrawToGraphics(e.Graphics, rowHeaderRect);
            }
        }

    }


}
