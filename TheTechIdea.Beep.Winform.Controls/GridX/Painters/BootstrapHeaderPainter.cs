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
    /// Bootstrap-inspired header painter with clean lines and utility-first approach
    /// Matches the Bootstrap navigation style
    /// </summary>
    public class BootstrapHeaderPainter : BaseHeaderPainter
    {
        /// <summary>
        /// Gets the navigation style
        /// </summary>
        public override navigationStyle Style => navigationStyle.Bootstrap;
        
        /// <summary>
        /// Gets the style name
        /// </summary>
        public override string StyleName => "Bootstrap";

        /// <summary>
        /// Calculate padding for Bootstrap headers
        /// </summary>
        public override int CalculateHeaderPadding() => 8;

        /// <summary>
        /// Paint all column headers
        /// </summary>
        public override void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || grid?.Layout == null || headerRect.IsEmpty) return;

            // Bootstrap flat background
            var bgColor = theme?.GridHeaderBackColor ?? Color.FromArgb(248, 249, 250);
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, headerRect);
            }

            // Top and bottom borders (Bootstrap border style)
            var borderColor = theme?.GridLineColor ?? Color.FromArgb(222, 226, 230);
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

            // Bootstrap hover state (subtle)
            if (isHovered)
            {
                var hoverColor = Color.FromArgb(233, 236, 239);
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, cellRect);
                }
            }

            // Calculate icon areas
            int sortIconSize = column.IsSorted && column.ShowSortIcon ? 14 : 0;
            int filterIconSize = column.ShowFilterIcon && isHovered ? 16 : 0;
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

            // Bootstrap typography
            var font = (theme?.GridHeaderFont != null ? BeepThemesManager.ToFont(theme.GridHeaderFont) : null) ?? new Font("Segoe UI", 9f, FontStyle.Bold);
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            var textColor = theme?.GridHeaderForeColor ?? Color.FromArgb(73, 80, 87);
            PaintHeaderText(g, textRect, text, font, column.HeaderTextAlignment, theme);

            // Draw Bootstrap sort indicator
            if (sortIconSize > 0)
            {
                PaintBootstrapSortIndicator(g, sortIconRect, column.SortDirection, theme);
            }

            // Draw Bootstrap filter icon
            if (filterIconSize > 0)
            {
                PaintBootstrapFilterIcon(g, filterIconRect, column.IsFiltered, theme);
            }

            // Right border
            var borderColor = theme?.GridLineColor ?? Color.FromArgb(222, 226, 230);
            using (var pen = new Pen(borderColor))
            {
                g.DrawLine(pen, cellRect.Right - 1, cellRect.Y, cellRect.Right - 1, cellRect.Bottom);
            }
        }

        private void PaintBootstrapSortIndicator(Graphics g, Rectangle iconRect, SortDirection sortDirection, IBeepTheme? theme)
        {
            if (sortDirection == SortDirection.None || iconRect.IsEmpty) return;

            // Bootstrap primary color
            var color = theme?.AccentColor ?? Color.FromArgb(13, 110, 253);
            using (var brush = new SolidBrush(color))
            {
                var centerX = iconRect.X + iconRect.Width / 2;
                var centerY = iconRect.Y + iconRect.Height / 2;

                Point[] triangle;
                if (sortDirection == SortDirection.Ascending)
                {
                    // Up triangle
                    triangle = new[]
                    {
                        new Point(centerX, centerY - 4),
                        new Point(centerX - 4, centerY + 2),
                        new Point(centerX + 4, centerY + 2)
                    };
                }
                else if (sortDirection == SortDirection.Descending)
                {
                    // Down triangle
                    triangle = new[]
                    {
                        new Point(centerX, centerY + 4),
                        new Point(centerX - 4, centerY - 2),
                        new Point(centerX + 4, centerY - 2)
                    };
                }
                else
                {
                    return;
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPolygon(brush, triangle);
                g.SmoothingMode = SmoothingMode.Default;
            }
        }

        private void PaintBootstrapFilterIcon(Graphics g, Rectangle iconRect, bool isActive, IBeepTheme? theme)
        {
            if (iconRect.IsEmpty) return;

            var color = isActive
                ? (theme?.AccentColor ?? Color.FromArgb(13, 110, 253))
                : Color.FromArgb(108, 117, 125);

            using (var pen = new Pen(color, 1.5f))
            {
                // Simple funnel icon
                var topY = iconRect.Y + 2;
                var bottomY = iconRect.Bottom - 2;
                var leftX = iconRect.X + 2;
                var rightX = iconRect.Right - 2;
                var midX = iconRect.X + iconRect.Width / 2;
                var midY = iconRect.Y + iconRect.Height / 2;

                Point[] funnel = new[]
                {
                    new Point(leftX, topY),
                    new Point(rightX, topY),
                    new Point(midX + 2, midY),
                    new Point(midX + 2, bottomY),
                    new Point(midX - 2, bottomY),
                    new Point(midX - 2, midY),
                    new Point(leftX, topY)
                };

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawLines(pen, funnel);
                g.SmoothingMode = SmoothingMode.Default;
            }

            // Active badge
            if (isActive)
            {
                var badgeColor = Color.FromArgb(220, 53, 69); // Bootstrap danger color
                using (var brush = new SolidBrush(badgeColor))
                {
                    g.FillEllipse(brush, iconRect.Right - 5, iconRect.Y, 4, 4);
                }
            }
        }
    }
}
