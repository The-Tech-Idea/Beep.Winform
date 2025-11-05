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
    /// Minimal header painter - ultra-clean, borderless design
    /// Maximum whitespace, subtle typography, zen-like simplicity
    /// </summary>
    public class MinimalHeaderPainter : BaseHeaderPainter
    {
        /// <summary>
        /// Gets the navigation style
        /// </summary>
        public override navigationStyle Style => navigationStyle.Minimal;
        
        /// <summary>
        /// Gets the style name
        /// </summary>
        public override string StyleName => "Minimal";

        /// <summary>
        /// Calculate padding for Minimal headers
        /// </summary>
        public override int CalculateHeaderPadding() => 16;

        /// <summary>
        /// Paint all column headers
        /// </summary>
        public override void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || grid?.Layout == null || headerRect.IsEmpty) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Minimal white/light background
            var bgColor = theme?.GridHeaderBackColor ?? Color.White;
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, headerRect);
            }

            // Ultra-subtle bottom border
            var borderColor = Color.FromArgb(15, 0, 0, 0);
            using (var pen = new Pen(borderColor, 1))
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

            // No sticky separator in minimal design
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

            // Extremely subtle hover effect
            if (isHovered)
            {
                var hoverColor = Color.FromArgb(8, 0, 0, 0);
                using (var brush = new SolidBrush(hoverColor))
                {
                    g.FillRectangle(brush, cellRect);
                }
            }

            // Calculate icon areas
            int sortIconSize = column.IsSorted && column.ShowSortIcon ? 12 : 0;
            int filterIconSize = column.ShowFilterIcon && isHovered ? 14 : 0;
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
                rightX -= filterIconSize + padding / 2;
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
                rightX -= sortIconSize + padding / 2;
            }

            // Text area
            var textRect = new Rectangle(
                cellRect.X + padding,
                cellRect.Y + padding,
                Math.Max(1, rightX - cellRect.X - padding),
                Math.Max(1, cellRect.Height - padding * 2)
            );

            // Minimal typography (light, elegant)
            var font = (theme?.GridHeaderFont != null ? BeepThemesManager.ToFont(theme.GridHeaderFont) : null) 
                ?? new Font("Segoe UI", 9f, FontStyle.Regular);
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            var textColor = theme?.GridHeaderForeColor ?? Color.FromArgb(100, 100, 100);
            
            // Use TextUtils for clean text rendering
            PaintHeaderText(g, textRect, text, font, column.HeaderTextAlignment, theme);

            // Draw minimal sort indicator (very subtle)
            if (sortIconSize > 0)
            {
                PaintMinimalSortIndicator(g, sortIconRect, column.SortDirection, theme);
            }

            // Draw minimal filter icon
            if (filterIconSize > 0)
            {
                PaintMinimalFilterIcon(g, filterIconRect, column.IsFiltered, theme);
            }

            // No borders in minimal design

            g.SmoothingMode = SmoothingMode.Default;
        }

        private void PaintMinimalSortIndicator(Graphics g, Rectangle iconRect, SortDirection sortDirection, IBeepTheme? theme)
        {
            if (sortDirection == SortDirection.None || iconRect.IsEmpty) return;

            var color = Color.FromArgb(120, 120, 120);
            using (var pen = new Pen(color, 1f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                var centerX = iconRect.X + iconRect.Width / 2;
                var centerY = iconRect.Y + iconRect.Height / 2;

                if (sortDirection == SortDirection.Ascending)
                {
                    // Simple line pointing up
                    g.DrawLine(pen, centerX, centerY + 4, centerX, centerY - 4);
                    g.DrawLine(pen, centerX, centerY - 4, centerX - 3, centerY - 1);
                    g.DrawLine(pen, centerX, centerY - 4, centerX + 3, centerY - 1);
                }
                else
                {
                    // Simple line pointing down
                    g.DrawLine(pen, centerX, centerY - 4, centerX, centerY + 4);
                    g.DrawLine(pen, centerX, centerY + 4, centerX - 3, centerY + 1);
                    g.DrawLine(pen, centerX, centerY + 4, centerX + 3, centerY + 1);
                }
            }
        }

        private void PaintMinimalFilterIcon(Graphics g, Rectangle iconRect, bool isActive, IBeepTheme? theme)
        {
            if (iconRect.IsEmpty) return;

            var color = isActive 
                ? Color.FromArgb(100, 100, 100) 
                : Color.FromArgb(180, 180, 180);

            using (var pen = new Pen(color, 1f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Simple circle outline
                var centerX = iconRect.X + iconRect.Width / 2;
                var centerY = iconRect.Y + iconRect.Height / 2;
                var radius = Math.Min(iconRect.Width, iconRect.Height) / 3;

                g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
            }

            // Tiny dot if active
            if (isActive)
            {
                using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
                {
                    var centerX = iconRect.X + iconRect.Width / 2;
                    var centerY = iconRect.Y + iconRect.Height / 2;
                    g.FillEllipse(brush, centerX - 2, centerY - 2, 4, 4);
                }
            }
        }
    }
}
