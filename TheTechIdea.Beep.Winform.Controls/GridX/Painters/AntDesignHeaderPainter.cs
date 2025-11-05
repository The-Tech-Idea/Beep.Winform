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
    /// Ant Design-style header painter with clean borders and subtle shadows
    /// </summary>
    public class AntDesignHeaderPainter : BaseHeaderPainter
    {
        /// <summary>
        /// Gets the navigation style
        /// </summary>
        public override navigationStyle Style => navigationStyle.AntDesign;
        
        /// <summary>
        /// Gets the style name
        /// </summary>
        public override string StyleName => "AntDesign";

        /// <summary>
        /// Calculate padding for Ant Design headers
        /// </summary>
        public override int CalculateHeaderPadding() => 12;

        /// <summary>
        /// Paint all column headers
        /// </summary>
        public override void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || grid?.Layout == null || headerRect.IsEmpty) return;

            // Ant Design uses #FAFAFA background
            Color bgColor = Color.FromArgb(250, 250, 250);
            
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, headerRect);
            }

            // Draw subtle shadow at bottom using gradient
            var shadowRect = new Rectangle(
                headerRect.X,
                headerRect.Bottom - 3,
                headerRect.Width,
                3
            );

            using (var shadowBrush = new LinearGradientBrush(
                shadowRect,
                Color.FromArgb(30, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(shadowBrush, shadowRect);
            }

            // Draw bottom border
            Color borderColor = Color.FromArgb(240, 240, 240); // #F0F0F0
            using (var borderPen = new Pen(borderColor, 1))
            {
                g.DrawLine(borderPen,
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

            // Ant Design hover effect - subtle background change
            if (isHovered)
            {
                Color hoverColor = Color.FromArgb(245, 245, 245); // Slightly darker
                using (var hoverBrush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(hoverBrush, cellRect);
                }
            }

            // Highlight sorted column with blue tint
            if (isSorted)
            {
                Color sortedBgColor = Color.FromArgb(15, 24, 144, 255); // Very subtle blue tint
                using (var sortedBrush = new SolidBrush(sortedBgColor))
                {
                    g.FillRectangle(sortedBrush, cellRect);
                }
            }

            // Draw right border between columns
            Color borderColor = Color.FromArgb(240, 240, 240); // #F0F0F0
            using (var borderPen = new Pen(borderColor, 1))
            {
                g.DrawLine(borderPen,
                    cellRect.Right - 1, cellRect.Top,
                    cellRect.Right - 1, cellRect.Bottom);
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
            Color textColor = Color.FromArgb(0, 0, 0, 85); // rgba(0, 0, 0, 0.85) - Ant Design text color
            var font = new Font("Segoe UI", 9f, FontStyle.Regular);
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            PaintHeaderText(g, textRect, text, font, column.HeaderTextAlignment, theme);

            // Draw sort indicator with Ant Design dual carets
            if (sortIconSize > 0)
            {
                PaintAntDesignSortIndicator(g, sortIconRect, column.SortDirection);
            }

            // Draw filter icon if needed
            if (filterIconSize > 0)
            {
                PaintFilterIcon(g, filterIconRect, column.IsFiltered, theme);
            }
        }

        private void PaintAntDesignSortIndicator(Graphics g, Rectangle bounds, SortDirection direction)
        {
            // Ant Design uses #1890FF for primary color
            Color activeColor = Color.FromArgb(24, 144, 255);
            Color inactiveColor = Color.FromArgb(191, 191, 191); // #BFBFBF

            using (var activeBrush = new SolidBrush(activeColor))
            using (var inactiveBrush = new SolidBrush(inactiveColor))
            {
                var centerX = bounds.X + bounds.Width / 2f;

                // Draw up caret
                PointF[] upPoints = new PointF[]
                {
                    new PointF(centerX, bounds.Y + 3),           // Top
                    new PointF(centerX - 4, bounds.Y + 7),       // Bottom left
                    new PointF(centerX + 4, bounds.Y + 7)        // Bottom right
                };
                g.FillPolygon(
                    direction == SortDirection.Ascending ? activeBrush : inactiveBrush,
                    upPoints
                );

                // Draw down caret
                PointF[] downPoints = new PointF[]
                {
                    new PointF(centerX - 4, bounds.Bottom - 7),  // Top left
                    new PointF(centerX + 4, bounds.Bottom - 7),  // Top right
                    new PointF(centerX, bounds.Bottom - 3)       // Bottom
                };
                g.FillPolygon(
                    direction == SortDirection.Descending ? activeBrush : inactiveBrush,
                    downPoints
                );
            }
        }
    }
}
