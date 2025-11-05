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
    /// Tailwind CSS-inspired header painter with utility-first design
    /// Clean, minimal, with subtle borders and modern typography
    /// </summary>
    public class TailwindHeaderPainter : BaseHeaderPainter
    {
        /// <summary>
        /// Gets the navigation style
        /// </summary>
        public override navigationStyle Style => navigationStyle.Tailwind;
        
        /// <summary>
        /// Gets the style name
        /// </summary>
        public override string StyleName => "Tailwind";

        /// <summary>
        /// Calculate padding for Tailwind headers
        /// </summary>
        public override int CalculateHeaderPadding() => 10;

        /// <summary>
        /// Paint all column headers
        /// </summary>
        public override void PaintHeaders(Graphics g, Rectangle headerRect, BeepGridPro grid, IBeepTheme? theme)
        {
            if (g == null || grid?.Layout == null || headerRect.IsEmpty) return;

            // Tailwind neutral background
            var bgColor = theme?.GridHeaderBackColor ?? Color.FromArgb(249, 250, 251);
            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, headerRect);
            }

            // Thin bottom border (Tailwind border-b)
            var borderColor = theme?.GridLineColor ?? Color.FromArgb(229, 231, 235);
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

            // Sticky separator
            if (stickyWidth > 0)
            {
                using (var pen = new Pen(borderColor, 1))
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

            // Tailwind hover state (bg-gray-100)
            if (isHovered)
            {
                var hoverColor = Color.FromArgb(243, 244, 246);
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

            // Tailwind typography (text-sm font-medium text-gray-700)
            var font = (theme?.GridHeaderFont != null ? BeepThemesManager.ToFont(theme.GridHeaderFont) : null) 
                ?? new Font("Inter", 9f, FontStyle.Regular);
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            var textColor = theme?.GridHeaderForeColor ?? Color.FromArgb(55, 65, 81);
            PaintHeaderText(g, textRect, text, font, column.HeaderTextAlignment, theme);

            // Draw sort indicator
            if (sortIconSize > 0)
            {
                PaintSortIndicator(g, sortIconRect, column.SortDirection, theme);
            }

            // Draw filter icon
            if (filterIconSize > 0)
            {
                PaintFilterIcon(g, filterIconRect, column.IsFiltered, theme);
            }

            // Right border (Tailwind border-r)
            var borderColor = theme?.GridLineColor ?? Color.FromArgb(229, 231, 235);
            using (var pen = new Pen(borderColor))
            {
                g.DrawLine(pen, cellRect.Right - 1, cellRect.Y, cellRect.Right - 1, cellRect.Bottom);
            }
        }
    }
}
