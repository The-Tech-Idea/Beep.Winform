using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Telerik UI-style header painter with professional enterprise styling
    /// </summary>
    public class TelerikHeaderPainter : BaseHeaderPainter
    {
        /// <summary>
        /// Gets the navigation style
        /// </summary>
        public override navigationStyle Style => navigationStyle.Telerik;
        
        /// <summary>
        /// Gets the style name
        /// </summary>
        public override string StyleName => "Telerik";

        /// <summary>
        /// Calculate padding for Telerik headers
        /// </summary>
        public override int CalculateHeaderPadding() => 10;

        /// <summary>
        /// Paint all column headers
        /// </summary>
        public override void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || grid?.Layout == null || headerRect.IsEmpty) return;

            // Telerik uses gradient background for professional look
            Color topColor = Color.FromArgb(246, 246, 246);
            Color bottomColor = Color.FromArgb(232, 232, 232);

            using (var gradientBrush = new LinearGradientBrush(
                headerRect,
                topColor,
                bottomColor,
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(gradientBrush, headerRect);
            }

            // Draw top border
            Color topBorderColor = Color.FromArgb(255, 255, 255);
            using (var topBorderPen = new Pen(topBorderColor, 1))
            {
                g.DrawLine(topBorderPen,
                    headerRect.Left, headerRect.Top,
                    headerRect.Right, headerRect.Top);
            }

            // Draw bottom border
            Color bottomBorderColor = Color.FromArgb(213, 213, 213);
            using (var bottomBorderPen = new Pen(bottomBorderColor, 1))
            {
                g.DrawLine(bottomBorderPen,
                    headerRect.Left, headerRect.Bottom - 1,
                    headerRect.Right, headerRect.Bottom - 1);
            }

            // Calculate sticky regions for proper layering
            var stickyColumns = grid.Data.Columns.Where(c => c.Sticked && c.Visible).ToList();
            int stickyWidth = stickyColumns.Sum(c => Math.Max(20, c.Width));
            stickyWidth = Math.Min(stickyWidth, headerRect.Width);

            Rectangle stickyRegion = new Rectangle(headerRect.Left, headerRect.Top, stickyWidth, headerRect.Height);
            Rectangle scrollingRegion = new Rectangle(headerRect.Left + stickyWidth, headerRect.Top,
                Math.Max(0, headerRect.Width - stickyWidth), headerRect.Height);

            // Draw scrolling (non-sticky) headers first
            var state1 = g.Save();
            g.SetClip(scrollingRegion);
            for (int i = 0; i < grid.Data.Columns.Count; i++)
            {
                var col = grid.Data.Columns[i];
                if (!col.Visible || col.Sticked) continue;
                if (i < grid.Layout.HeaderCellRects.Length)
                {
                    var cellRect = grid.Layout.HeaderCellRects[i];
                    if (!cellRect.IsEmpty && cellRect.Width > 0 && cellRect.Height > 0)
                        PaintHeaderCell(g, cellRect, col, i, grid, theme);
                }
            }
            g.Restore(state1);

            // Draw sticky headers on top
            var state2 = g.Save();
            g.SetClip(stickyRegion);
            for (int i = 0; i < grid.Data.Columns.Count; i++)
            {
                var col = grid.Data.Columns[i];
                if (!col.Visible || !col.Sticked) continue;
                if (i < grid.Layout.HeaderCellRects.Length)
                {
                    var cellRect = grid.Layout.HeaderCellRects[i];
                    if (!cellRect.IsEmpty && cellRect.Width > 0 && cellRect.Height > 0)
                        PaintHeaderCell(g, cellRect, col, i, grid, theme);
                }
            }
            g.Restore(state2);
        }

        /// <summary>
        /// Paint a single header cell
        /// </summary>
        public override void PaintHeaderCell(Graphics g, Rectangle cellRect, BeepColumnConfig column,
            int columnIndex, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || column == null || cellRect.IsEmpty) return;

            bool isHovered = grid.Layout.HoveredHeaderColumnIndex == columnIndex;
            bool isSorted = column.IsSorted;

            // Telerik hover effect - lighter gradient
            if (isHovered)
            {
                Color hoverTop = Color.FromArgb(252, 252, 252);
                Color hoverBottom = Color.FromArgb(240, 240, 240);

                using (var hoverBrush = new LinearGradientBrush(
                    cellRect,
                    hoverTop,
                    hoverBottom,
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(hoverBrush, cellRect);
                }

                // Hover border
                Color hoverBorderColor = Color.FromArgb(184, 184, 184);
                using (var hoverBorderPen = new Pen(hoverBorderColor, 1))
                {
                    g.DrawRectangle(hoverBorderPen, 
                        cellRect.X, cellRect.Y, 
                        cellRect.Width - 1, cellRect.Height - 1);
                }
            }

            // Sorted column highlight with different gradient
            if (isSorted && !isHovered)
            {
                Color sortedTop = Color.FromArgb(238, 244, 252);
                Color sortedBottom = Color.FromArgb(220, 232, 246);

                using (var sortedBrush = new LinearGradientBrush(
                    cellRect,
                    sortedTop,
                    sortedBottom,
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(sortedBrush, cellRect);
                }
            }

            // Draw right separator
            Color separatorColor = Color.FromArgb(213, 213, 213);
            using (var separatorPen = new Pen(separatorColor, 1))
            {
                g.DrawLine(separatorPen,
                    cellRect.Right - 1, cellRect.Top + 4,
                    cellRect.Right - 1, cellRect.Bottom - 4);
            }

            // Draw inner highlight on left
            Color highlightColor = Color.FromArgb(255, 255, 255);
            using (var highlightPen = new Pen(highlightColor, 1))
            {
                g.DrawLine(highlightPen,
                    cellRect.Left, cellRect.Top + 1,
                    cellRect.Left, cellRect.Bottom - 1);
            }

            // Calculate layout areas
            int padding = CalculateHeaderPadding();
            int sortIconSize = column.IsSorted && column.ShowSortIcon ? 16 : 0;
            int filterIconSize = column.ShowFilterIcon && isHovered ? 18 : 0;

            int rightX = cellRect.Right - padding;

            // Filter icon area (rightmost)
            Rectangle filterIconRect = Rectangle.Empty;
            if (filterIconSize > 0)
            {
                filterIconRect = new Rectangle(
                    rightX - filterIconSize,
                    cellRect.Y + (cellRect.Height - filterIconSize) / 2,
                    filterIconSize,
                    filterIconSize);
                rightX -= filterIconSize + padding;
            }

            // Sort icon area
            Rectangle sortIconRect = Rectangle.Empty;
            if (sortIconSize > 0)
            {
                sortIconRect = new Rectangle(
                    rightX - sortIconSize,
                    cellRect.Y + (cellRect.Height - sortIconSize) / 2,
                    sortIconSize,
                    sortIconSize);
                rightX -= sortIconSize + padding;
            }

            // Text area
            var textRect = new Rectangle(
                cellRect.X + padding,
                cellRect.Y + padding,
                Math.Max(1, rightX - cellRect.X - padding),
                Math.Max(1, cellRect.Height - padding * 2)
            );

            // Draw header text
            Color textColor = Color.FromArgb(51, 51, 51);
            var font = new Font("Arial", 9f, FontStyle.Regular);
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            PaintHeaderText(g, textRect, text, font, column.HeaderTextAlignment, theme);

            // Draw sort indicator with Telerik arrow style
            if (sortIconSize > 0)
            {
                PaintTelerikSortIndicator(g, sortIconRect, column.SortDirection);
            }

            // Draw filter icon if needed
            if (filterIconSize > 0)
            {
                PaintTelerikFilterIcon(g, filterIconRect);
            }
        }

        private void PaintTelerikSortIndicator(Graphics g, Rectangle bounds, SortDirection direction)
        {
            Color arrowColor = Color.FromArgb(45, 45, 45);
            Color borderColor = Color.FromArgb(160, 160, 160);

            using (var arrowBrush = new SolidBrush(arrowColor))
            using (var borderPen = new Pen(borderColor, 1))
            {
                var centerX = bounds.X + bounds.Width / 2f;

                if (direction == SortDirection.Ascending)
                {
                    // Upward arrow with outline
                    PointF[] points = new PointF[]
                    {
                        new PointF(centerX, bounds.Y + 2),           // Top
                        new PointF(centerX - 5, bounds.Bottom - 2),  // Bottom left
                        new PointF(centerX + 5, bounds.Bottom - 2)   // Bottom right
                    };
                    g.FillPolygon(arrowBrush, points);
                    g.DrawPolygon(borderPen, points);
                }
                else
                {
                    // Downward arrow with outline
                    PointF[] points = new PointF[]
                    {
                        new PointF(centerX - 5, bounds.Y + 2),      // Top left
                        new PointF(centerX + 5, bounds.Y + 2),      // Top right
                        new PointF(centerX, bounds.Bottom - 2)      // Bottom
                    };
                    g.FillPolygon(arrowBrush, points);
                    g.DrawPolygon(borderPen, points);
                }
            }
        }

        private void PaintTelerikFilterIcon(Graphics g, Rectangle bounds)
        {
            Color filterColor = Color.FromArgb(100, 100, 100);
            Color filterHighlight = Color.FromArgb(150, 150, 150);

            using (var filterBrush = new LinearGradientBrush(
                bounds,
                filterHighlight,
                filterColor,
                LinearGradientMode.Vertical))
            using (var borderPen = new Pen(Color.FromArgb(80, 80, 80), 1))
            {
                var centerX = bounds.X + bounds.Width / 2f;

                // Funnel shape with professional look
                PointF[] funnelPoints = new PointF[]
                {
                    new PointF(bounds.X + 2, bounds.Y + 3),           // Top left
                    new PointF(bounds.Right - 2, bounds.Y + 3),       // Top right
                    new PointF(centerX + 3, bounds.Y + 8),            // Middle right
                    new PointF(centerX + 3, bounds.Bottom - 3),       // Bottom right
                    new PointF(centerX - 3, bounds.Bottom - 3),       // Bottom left
                    new PointF(centerX - 3, bounds.Y + 8),            // Middle left
                };

                g.FillPolygon(filterBrush, funnelPoints);
                g.DrawPolygon(borderPen, funnelPoints);
            }
        }
    }
}
