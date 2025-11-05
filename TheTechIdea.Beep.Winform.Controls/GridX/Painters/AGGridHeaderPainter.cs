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
    /// AG-Grid inspired header painter - professional enterprise grid style
    /// Clean, data-focused with excellent readability
    /// </summary>
    public class AGGridHeaderPainter : BaseHeaderPainter
    {
        /// <summary>
        /// Gets the navigation style
        /// </summary>
        public override navigationStyle Style => navigationStyle.AGGrid;
        
        /// <summary>
        /// Gets the style name
        /// </summary>
        public override string StyleName => "AGGrid";

        /// <summary>
        /// Calculate padding for AG-Grid headers
        /// </summary>
        public override int CalculateHeaderPadding() => 8;

        /// <summary>
        /// Paint all column headers
        /// </summary>
        public override void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || grid?.Layout == null || headerRect.IsEmpty) return;

            // AG-Grid neutral header background
            var bgColor = theme?.GridHeaderBackColor ?? Color.FromArgb(248, 248, 248);
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, headerRect);
            }

            // Top and bottom borders
            var borderColor = theme?.GridLineColor ?? Color.FromArgb(221, 221, 221);
            using (var pen = new Pen(borderColor))
            {
                g.DrawLine(pen, headerRect.Left, headerRect.Top, headerRect.Right, headerRect.Top);
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
                using (var pen = new Pen(borderColor, 2))
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

            // AG-Grid hover effect (slightly darker)
            if (isHovered)
            {
                var hoverColor = Color.FromArgb(238, 238, 238);
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, cellRect);
                }
            }

            // Calculate icon areas
            int sortIconSize = column.IsSorted && column.ShowSortIcon ? 12 : 0;
            int filterIconSize = column.ShowFilterIcon ? 14 : 0; // Always show if enabled
            int rightX = cellRect.Right - padding;

            // Filter icon area (always visible if column has filter)
            Rectangle filterIconRect = Rectangle.Empty;
            if (filterIconSize > 0)
            {
                filterIconRect = new Rectangle(
                    rightX - filterIconSize,
                    cellRect.Y + (cellRect.Height - filterIconSize) / 2,
                    filterIconSize,
                    filterIconSize);
                rightX -= filterIconSize + 4;
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

            // AG-Grid typography (clear, readable)
            var font = (theme?.GridHeaderFont != null ? BeepThemesManager.ToFont(theme.GridHeaderFont) : null) 
                ?? new Font("Arial", 9f, FontStyle.Bold);
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            var textColor = theme?.GridHeaderForeColor ?? Color.FromArgb(51, 51, 51);
            PaintHeaderText(g, textRect, text, font, column.HeaderTextAlignment, theme);

            // Draw AG-Grid sort indicator (small triangle)
            if (sortIconSize > 0)
            {
                PaintAGGridSortIndicator(g, sortIconRect, column.SortDirection, theme);
            }

            // Draw AG-Grid filter icon (three horizontal lines)
            if (filterIconSize > 0)
            {
                PaintAGGridFilterIcon(g, filterIconRect, column.IsFiltered, theme);
            }

            // Right border
            var borderColor = theme?.GridLineColor ?? Color.FromArgb(221, 221, 221);
            using (var pen = new Pen(borderColor))
            {
                g.DrawLine(pen, cellRect.Right - 1, cellRect.Y, cellRect.Right - 1, cellRect.Bottom);
            }
        }

        private void PaintAGGridSortIndicator(Graphics g, Rectangle iconRect, SortDirection sortDirection, IBeepTheme? theme)
        {
            if (sortDirection == SortDirection.None || iconRect.IsEmpty) return;

            var color = theme?.AccentColor ?? Color.FromArgb(33, 150, 243);
            using (var brush = new SolidBrush(color))
            {
                var centerX = iconRect.X + iconRect.Width / 2;
                var centerY = iconRect.Y + iconRect.Height / 2;

                Point[] triangle;
                if (sortDirection == SortDirection.Ascending)
                {
                    // Small up triangle
                    triangle = new[]
                    {
                        new Point(centerX, centerY - 3),
                        new Point(centerX - 3, centerY + 2),
                        new Point(centerX + 3, centerY + 2)
                    };
                }
                else
                {
                    // Small down triangle
                    triangle = new[]
                    {
                        new Point(centerX, centerY + 3),
                        new Point(centerX - 3, centerY - 2),
                        new Point(centerX + 3, centerY - 2)
                    };
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPolygon(brush, triangle);
                g.SmoothingMode = SmoothingMode.Default;
            }
        }

        private void PaintAGGridFilterIcon(Graphics g, Rectangle iconRect, bool isActive, IBeepTheme? theme)
        {
            if (iconRect.IsEmpty) return;

            var color = isActive
                ? (theme?.AccentColor ?? Color.FromArgb(33, 150, 243))
                : Color.FromArgb(150, 150, 150);

            using (var pen = new Pen(color, 1.5f))
            {
                // Three horizontal lines (menu icon style)
                int lineY1 = iconRect.Y + 3;
                int lineY2 = iconRect.Y + iconRect.Height / 2;
                int lineY3 = iconRect.Bottom - 3;
                int lineLeft = iconRect.X + 2;
                int lineRight = iconRect.Right - 2;

                g.DrawLine(pen, lineLeft, lineY1, lineRight, lineY1);
                g.DrawLine(pen, lineLeft, lineY2, lineRight, lineY2);
                g.DrawLine(pen, lineLeft, lineY3, lineRight, lineY3);
            }

            // Active indicator dot
            if (isActive)
            {
                using (var brush = new SolidBrush(theme?.AccentColor ?? Color.FromArgb(33, 150, 243)))
                {
                    g.FillEllipse(brush, iconRect.Right - 4, iconRect.Y, 3, 3);
                }
            }
        }
    }
}
