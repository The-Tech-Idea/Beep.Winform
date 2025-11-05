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
    /// Compact header painter - dense layout for maximum data visibility
    /// Minimal padding, small fonts, efficient space usage
    /// </summary>
    public class CompactHeaderPainter : BaseHeaderPainter
    {
        /// <summary>
        /// Gets the navigation style
        /// </summary>
        public override navigationStyle Style => navigationStyle.Compact;
        
        /// <summary>
        /// Gets the style name
        /// </summary>
        public override string StyleName => "Compact";

        /// <summary>
        /// Calculate padding for Compact headers (minimal)
        /// </summary>
        public override int CalculateHeaderPadding() => 3;

        /// <summary>
        /// Paint all column headers
        /// </summary>
        public override void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || grid?.Layout == null || headerRect.IsEmpty) return;

            // Compact flat background
            var bgColor = theme?.GridHeaderBackColor ?? Color.FromArgb(245, 245, 245);
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, headerRect);
            }

            // Thin border
            var borderColor = theme?.GridLineColor ?? Color.FromArgb(200, 200, 200);
            using (var pen = new Pen(borderColor))
            {
                g.DrawLine(pen, headerRect.Left, headerRect.Bottom - 1, headerRect.Right, headerRect.Bottom - 1);
            }

            // Calculate sticky regions
            var stickyColumns = grid.Data.Columns.Where(c => c.Sticked && c.Visible).ToList();
            int stickyWidth = stickyColumns.Sum(c => Math.Max(20, c.Width));
            stickyWidth = Math.Min(stickyWidth, headerRect.Width);

            Rectangle stickyRegion = new Rectangle(headerRect.Left, headerRect.Top, stickyWidth, headerRect.Height);
            Rectangle scrollingRegion = new Rectangle(headerRect.Left + stickyWidth, headerRect.Top,
                Math.Max(0, headerRect.Width - stickyWidth), headerRect.Height);

            // Draw scrolling headers
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

            // Draw sticky headers
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

            // Sticky separator
            if (stickyWidth > 0)
            {
                using (var pen = new Pen(borderColor))
                {
                    g.DrawLine(pen, headerRect.Left + stickyWidth, headerRect.Top,
                        headerRect.Left + stickyWidth, headerRect.Bottom);
                }
            }
        }

        /// <summary>
        /// Paint a single header cell
        /// </summary>
        public override void PaintHeaderCell(Graphics g, Rectangle cellRect, BeepColumnConfig column,
            int columnIndex, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || column == null || cellRect.IsEmpty) return;

            bool isHovered = grid.Layout.HoveredHeaderColumnIndex == columnIndex;
            int padding = CalculateHeaderPadding();

            // Minimal hover effect
            if (isHovered)
            {
                var hoverColor = Color.FromArgb(235, 235, 235);
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, cellRect);
                }
            }

            // Calculate icon areas (smaller for compact)
            int sortIconSize = column.IsSorted && column.ShowSortIcon ? 10 : 0;
            int filterIconSize = column.ShowFilterIcon && isHovered ? 12 : 0;
            int rightX = cellRect.Right - padding;

            // Filter icon area
            Rectangle filterIconRect = Rectangle.Empty;
            if (filterIconSize > 0)
            {
                filterIconRect = new Rectangle(
                    rightX - filterIconSize,
                    cellRect.Y + (cellRect.Height - filterIconSize) / 2,
                    filterIconSize,
                    filterIconSize);
                rightX -= filterIconSize + 2;
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
                rightX -= sortIconSize + 2;
            }

            // Text area
            var textRect = new Rectangle(
                cellRect.X + padding,
                cellRect.Y + padding,
                Math.Max(1, rightX - cellRect.X - padding),
                Math.Max(1, cellRect.Height - padding * 2)
            );

            // Compact typography (small, efficient)
            var font = (theme?.GridHeaderFont != null ? BeepThemesManager.ToFont(theme.GridHeaderFont) : null) 
                ?? new Font("Arial", 8f, FontStyle.Regular);
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            
            // Use TextUtils for efficient text rendering
            var textColor = theme?.GridHeaderForeColor ?? Color.FromArgb(60, 60, 60);
            PaintHeaderText(g, textRect, text, font, column.HeaderTextAlignment, theme);

            // Draw compact sort indicator
            if (sortIconSize > 0)
            {
                PaintSortIndicator(g, sortIconRect, column.SortDirection, theme);
            }

            // Draw compact filter icon
            if (filterIconSize > 0)
            {
                PaintFilterIcon(g, filterIconRect, column.IsFiltered, theme);
            }

            // Thin right border
            var borderColor = theme?.GridLineColor ?? Color.FromArgb(200, 200, 200);
            using (var pen = new Pen(borderColor))
            {
                g.DrawLine(pen, cellRect.Right - 1, cellRect.Y, cellRect.Right - 1, cellRect.Bottom);
            }
        }
    }
}
