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
    /// jQuery DataTables-style header painter with stripe hover effects and subtle styling
    /// </summary>
    public class DataTablesHeaderPainter : BaseHeaderPainter
    {
        /// <summary>
        /// Gets the navigation style
        /// </summary>
        public override navigationStyle Style => navigationStyle.DataTables;
        
        /// <summary>
        /// Gets the style name
        /// </summary>
        public override string StyleName => "DataTables";

        /// <summary>
        /// Calculate padding for DataTables headers
        /// </summary>
        public override int CalculateHeaderPadding() => 8;

        /// <summary>
        /// Paint all column headers
        /// </summary>
        public override void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || grid?.Layout == null || headerRect.IsEmpty) return;

            // DataTables uses light gray background with subtle border
            Color bgColor = Color.FromArgb(249, 249, 249); // Very light gray
            Color borderColor = Color.FromArgb(221, 221, 221); // #DDDDDD

            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, headerRect);
            }

            // Draw bottom border
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

            // DataTables hover effect - subtle stripe pattern
            if (isHovered)
            {
                Color hoverColor = Color.FromArgb(245, 245, 245); // Slightly darker gray
                using (var hoverBrush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(hoverBrush, cellRect);
                }
            }

            // Draw right border between columns
            Color borderColor = Color.FromArgb(221, 221, 221);
            using (var borderPen = new Pen(borderColor, 1))
            {
                g.DrawLine(borderPen,
                    cellRect.Right - 1, cellRect.Top,
                    cellRect.Right - 1, cellRect.Bottom);
            }

            // Calculate text area with padding for icons
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
            Color textColor = Color.FromArgb(51, 51, 51); // #333333 - dark gray
            var font = new Font("Segoe UI", 9f, FontStyle.Bold);
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            PaintHeaderText(g, textRect, text, font, column.HeaderTextAlignment, theme);

            // Draw sort indicator if needed
            if (sortIconSize > 0)
            {
                PaintSortIndicator(g, sortIconRect, column.SortDirection, theme);
            }

            // Draw filter icon if needed
            if (filterIconSize > 0)
            {
                PaintFilterIcon(g, filterIconRect, column.IsFiltered, theme);
            }
        }
    }
}
