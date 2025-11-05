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
    /// Fluent Design header painter with depth, acrylic effects, and smooth transitions
    /// Matches Microsoft Fluent Design System
    /// </summary>
    public class FluentHeaderPainter : BaseHeaderPainter
    {
        /// <summary>
        /// Gets the navigation style
        /// </summary>
        public override navigationStyle Style => navigationStyle.Fluent;
        
        /// <summary>
        /// Gets the style name
        /// </summary>
        public override string StyleName => "Fluent";

        /// <summary>
        /// Calculate padding for Fluent headers
        /// </summary>
        public override int CalculateHeaderPadding() => 12;

        /// <summary>
        /// Paint all column headers
        /// </summary>
        public override void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || grid?.Layout == null || headerRect.IsEmpty) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Fluent acrylic background with subtle gradient
            var baseColor = theme?.GridHeaderBackColor ?? Color.FromArgb(243, 243, 243);
            using (var path = new GraphicsPath())
            {
                path.AddRectangle(headerRect);
                using (var brush = new LinearGradientBrush(
                    headerRect,
                    baseColor,
                    ColorUtils.Lighten(baseColor, 0.02f),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }
            }

            // Subtle reveal border at bottom
            var revealColor = theme?.AccentColor ?? Color.FromArgb(0, 120, 212);
            using (var pen = new Pen(Color.FromArgb(20, revealColor), 2))
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

            // Draw sticky headers with acrylic effect
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

            // Sticky separator with reveal highlight
            if (stickyWidth > 0)
            {
                using (var pen = new Pen(Color.FromArgb(30, revealColor), 2))
                {
                    g.DrawLine(pen, headerRect.Left + stickyWidth, headerRect.Top,
                        headerRect.Left + stickyWidth, headerRect.Bottom);
                }
            }

            g.SmoothingMode = SmoothingMode.Default;
        }

        /// <summary>
        /// Paint a single header cell
        /// </summary>
        public override void PaintHeaderCell(Graphics g, Rectangle cellRect, BeepColumnConfig column,
            int columnIndex, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || column == null || cellRect.IsEmpty) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            bool isHovered = grid.Layout.HoveredHeaderColumnIndex == columnIndex;
            int padding = CalculateHeaderPadding();

            // Fluent reveal hover effect
            if (isHovered)
            {
                var accentColor = theme?.AccentColor ?? Color.FromArgb(0, 120, 212);
                var hoverRect = new Rectangle(cellRect.X + 1, cellRect.Y + 1, cellRect.Width - 2, cellRect.Height - 2);
                
                using (var brush = new LinearGradientBrush(
                    hoverRect,
                    Color.FromArgb(25, accentColor),
                    Color.FromArgb(10, accentColor),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, hoverRect);
                }
            }

            // Calculate icon areas
            int sortIconSize = column.IsSorted && column.ShowSortIcon ? 16 : 0;
            int filterIconSize = column.ShowFilterIcon && isHovered ? 18 : 0;
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

            // Fluent typography (Segoe UI)
            var font = (theme?.GridHeaderFont != null ? BeepThemesManager.ToFont(theme.GridHeaderFont) : null) 
                ?? new Font("Segoe UI", 9.5f, FontStyle.Regular);
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            PaintHeaderText(g, textRect, text, font, column.HeaderTextAlignment, theme);

            // Draw Fluent sort indicator
            if (sortIconSize > 0)
            {
                PaintFluentSortIndicator(g, sortIconRect, column.SortDirection, theme);
            }

            // Draw Fluent filter icon
            if (filterIconSize > 0)
            {
                PaintFilterIcon(g, filterIconRect, column.IsFiltered, theme);
            }

            // Subtle divider
            using (var pen = new Pen(Color.FromArgb(20, 0, 0, 0)))
            {
                g.DrawLine(pen, cellRect.Right - 1, cellRect.Y + 6, cellRect.Right - 1, cellRect.Bottom - 6);
            }

            g.SmoothingMode = SmoothingMode.Default;
        }

        private void PaintFluentSortIndicator(Graphics g, Rectangle iconRect, SortDirection sortDirection, IBeepTheme? theme)
        {
            if (sortDirection == SortDirection.None || iconRect.IsEmpty) return;

            var accentColor = theme?.AccentColor ?? Color.FromArgb(0, 120, 212);
            using (var brush = new SolidBrush(accentColor))
            {
                var centerX = iconRect.X + iconRect.Width / 2;
                var centerY = iconRect.Y + iconRect.Height / 2;

                Point[] chevron;
                if (sortDirection == SortDirection.Ascending)
                {
                    // Chevron up
                    chevron = new[]
                    {
                        new Point(centerX, centerY - 3),
                        new Point(centerX - 5, centerY + 2),
                        new Point(centerX - 3, centerY + 2),
                        new Point(centerX, centerY - 1),
                        new Point(centerX + 3, centerY + 2),
                        new Point(centerX + 5, centerY + 2)
                    };
                }
                else
                {
                    // Chevron down
                    chevron = new[]
                    {
                        new Point(centerX, centerY + 3),
                        new Point(centerX - 5, centerY - 2),
                        new Point(centerX - 3, centerY - 2),
                        new Point(centerX, centerY + 1),
                        new Point(centerX + 3, centerY - 2),
                        new Point(centerX + 5, centerY - 2)
                    };
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPolygon(brush, chevron);
                g.SmoothingMode = SmoothingMode.Default;
            }
        }
    }
}
