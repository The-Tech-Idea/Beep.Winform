using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
 

namespace TheTechIdea.Beep.Winform.Controls.GridX.Helpers
{
    /// <summary>
    /// Helper class responsible for rendering the footer area at the bottom of BeepGridPro.
    /// The footer can display summary information, aggregates, or custom content.
    /// </summary>
    public sealed class GridFooterPainterHelper
    {
        private readonly BeepGridPro _grid;

        // Configuration properties
        public bool ShowGridLines { get; set; } = true;
        public DashStyle GridLineStyle { get; set; } = DashStyle.Solid;
        public bool ShowFooter { get; set; } = false;
        public int FooterHeight { get; set; } = 30;

        private IBeepTheme? Theme => _grid?._currentTheme;

        public GridFooterPainterHelper(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        /// <summary>
        /// Main entry point to draw the footer area.
        /// Only renders if ShowFooter is enabled.
        /// </summary>
        public void DrawFooter(Graphics g)
        {
            if (!ShowFooter || g == null || _grid?.Layout == null) return;

            Rectangle footerRect = CalculateFooterRect();
            if (footerRect.Height <= 0 || footerRect.Width <= 0) return;

            // Fill background
            using (var brush = new SolidBrush(Theme?.GridHeaderBackColor ?? SystemColors.Control))
            {
                g.FillRectangle(brush, footerRect);
            }

            // Top border
            if (ShowGridLines)
            {
                using (var pen = new Pen(Theme?.GridLineColor ?? SystemColors.ControlDark))
                {
                    pen.DashStyle = GridLineStyle;
                    g.DrawLine(pen, footerRect.Left, footerRect.Top, footerRect.Right, footerRect.Top);
                }
            }

            // Draw footer content (summary, aggregates, etc.)
            DrawFooterContent(g, footerRect);
        }

        private Rectangle CalculateFooterRect()
        {
            if (_grid?.Layout == null) return Rectangle.Empty;

            // Footer appears between rows area and navigator area
            var navRect = _grid.Layout.NavigatorRect;
            var rowsRect = _grid.Layout.RowsRect;

            if (navRect.IsEmpty || rowsRect.IsEmpty) return Rectangle.Empty;

            return new Rectangle(
                rowsRect.Left,
                rowsRect.Bottom,
                rowsRect.Width,
                FooterHeight
            );
        }

        private void DrawFooterContent(Graphics g, Rectangle footerRect)
        {
            // Calculate summary information
            int totalRows = _grid.Rows?.Count ?? 0;
            int selectedRows = _grid.Rows?.Count(r => r.IsSelected) ?? 0;
            int visibleRows = _grid.Data?.Rows?.Count(r => r.IsVisible) ?? 0;

            string summaryText = BuildSummaryText(totalRows, selectedRows, visibleRows);

            // Draw summary text
            var font = BeepThemesManager.ToFont(_grid._currentTheme.GridCellFont) ?? SystemFonts.DefaultFont;
            var textColor = Theme?.GridHeaderForeColor ?? SystemColors.ControlText;

            var textRect = new Rectangle(
                footerRect.X + 10,
                footerRect.Y,
                footerRect.Width - 20,
                footerRect.Height
            );

            TextRenderer.DrawText(g, summaryText, font, textRect, textColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

            // Optional: Draw column-specific aggregates if configured
            DrawColumnAggregates(g, footerRect);
        }

        private string BuildSummaryText(int totalRows, int selectedRows, int visibleRows)
        {
            if (totalRows == 0)
            {
                return "No records";
            }

            string summary = $"{totalRows} total";

            if (selectedRows > 0)
            {
                summary += $" • {selectedRows} selected";
            }

            if (visibleRows != totalRows)
            {
                summary += $" • {visibleRows} visible";
            }

            return summary;
        }

        private void DrawColumnAggregates(Graphics g, Rectangle footerRect)
        {
            // This method can be extended to show column-specific aggregates
            // For example: SUM, AVG, COUNT, MIN, MAX for numeric columns
            // Future implementation can align aggregate values with their column positions

            // Example placeholder for future aggregate support:
            // foreach (var column in _grid.Data.Columns.Where(c => c.Visible && c.ShowAggregate))
            // {
            //     var cellRect = GetColumnFooterRect(column, footerRect);
            //     DrawAggregateValue(g, column, cellRect);
            // }
        }

        /// <summary>
        /// Helper method to test if a point is within the footer area.
        /// </summary>
        public bool ContainsPoint(Point pt)
        {
            if (!ShowFooter) return false;
            Rectangle footerRect = CalculateFooterRect();
            return footerRect.Contains(pt);
        }

        /// <summary>
        /// Get the current footer rectangle bounds.
        /// </summary>
        public Rectangle GetBounds()
        {
            return ShowFooter ? CalculateFooterRect() : Rectangle.Empty;
        }

        /// <summary>
        /// Calculate the total height needed for footer (used in layout calculations).
        /// </summary>
        public int GetRequiredHeight()
        {
            return ShowFooter ? FooterHeight : 0;
        }
    }
}
