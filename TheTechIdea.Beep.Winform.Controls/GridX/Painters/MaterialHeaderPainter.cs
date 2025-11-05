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
    /// Material Design header painter with gradients, elevation shadows, and rounded corners
    /// Matches the Material navigation style
    /// </summary>
    public class MaterialHeaderPainter : BaseHeaderPainter
    {
        /// <summary>
        /// Gets the navigation style
        /// </summary>
        public override navigationStyle Style => navigationStyle.Material;
        
        /// <summary>
        /// Gets the style name
        /// </summary>
        public override string StyleName => "Material";

        /// <summary>
        /// Calculate padding for Material headers
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

            // Material gradient background
            var startColor = theme?.GridHeaderBackColor ?? Color.FromArgb(240, 240, 245);
            DrawGradientBackground(g, headerRect, startColor, LinearGradientMode.Vertical);

            // Subtle elevation shadow
            DrawElevationShadow(g, headerRect, 2);

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

            // Draw sticky headers with slight elevation
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
                    {
                        // Add elevation for sticky columns
                        DrawElevationShadow(g, cellRect, 3);
                        PaintHeaderCell(g, cellRect, col, i, grid, theme);
                    }
                }
            }
            g.Restore(state2);

            // Vertical divider with shadow
            if (stickyWidth > 0)
            {
                using (var shadowPen = new Pen(Color.FromArgb(40, 0, 0, 0), 3))
                {
                    g.DrawLine(shadowPen, headerRect.Left + stickyWidth + 1, headerRect.Top,
                        headerRect.Left + stickyWidth + 1, headerRect.Bottom);
                }
                using (var pen = new Pen(Color.FromArgb(60, 0, 0, 0)))
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

            // Material hover effect with ripple-like gradient
            if (isHovered)
            {
                var accentColor = theme?.AccentColor ?? Color.DeepSkyBlue;
                var hoverColor = Color.FromArgb(30, accentColor);
                var hoverRect = new Rectangle(cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, cellRect.Height - 4);
                
                using (var hoverBrush = new LinearGradientBrush(
                    hoverRect,
                    Color.FromArgb(50, hoverColor),
                    Color.FromArgb(10, hoverColor),
                    LinearGradientMode.Vertical))
                {
                    using (var path = CreateRoundedRectangle(hoverRect, 4))
                    {
                        g.FillPath(hoverBrush, path);
                    }
                }
            }

            // Calculate icon areas
            int sortIconSize = column.IsSorted && column.ShowSortIcon ? 18 : 0;
            int filterIconSize = column.ShowFilterIcon && isHovered ? 20 : 0;
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

            // Draw text with Material typography
            var font = (theme?.GridHeaderFont != null ? BeepThemesManager.ToFont(theme.GridHeaderFont) : null) ?? new Font("Roboto", 10f, FontStyle.Bold);
            string text = column.ColumnCaption ?? column.ColumnName ?? string.Empty;
            var textColor = theme?.GridHeaderForeColor ?? Color.FromArgb(60, 60, 67);
            PaintHeaderText(g, textRect, text, font, column.HeaderTextAlignment, theme);

            // Draw Material sort indicator (animated-style)
            if (sortIconSize > 0)
            {
                PaintMaterialSortIndicator(g, sortIconRect, column.SortDirection, theme);
            }

            // Draw Material filter icon
            if (filterIconSize > 0)
            {
                PaintMaterialFilterIcon(g, filterIconRect, column.IsFiltered, theme);
            }

            // Subtle divider
            using (var pen = new Pen(Color.FromArgb(30, 0, 0, 0)))
            {
                g.DrawLine(pen, cellRect.Right - 1, cellRect.Y + 8, cellRect.Right - 1, cellRect.Bottom - 8);
            }

            g.SmoothingMode = SmoothingMode.Default;
        }

        private void PaintMaterialSortIndicator(Graphics g, Rectangle iconRect, SortDirection sortDirection, IBeepTheme? theme)
        {
            if (sortDirection == SortDirection.None || iconRect.IsEmpty) return;

            var accentColor = theme?.AccentColor ?? Color.DeepSkyBlue;
            using (var brush = new SolidBrush(accentColor))
            using (var pen = new Pen(accentColor, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.ArrowAnchor;
                pen.CustomEndCap = new AdjustableArrowCap(4, 5);

                var centerX = iconRect.X + iconRect.Width / 2;
                var centerY = iconRect.Y + iconRect.Height / 2;

                if (sortDirection == SortDirection.Ascending)
                {
                    g.DrawLine(pen, centerX, centerY + 5, centerX, centerY - 5);
                }
                else if (sortDirection == SortDirection.Descending)
                {
                    g.DrawLine(pen, centerX, centerY - 5, centerX, centerY + 5);
                }
            }
        }

        private void PaintMaterialFilterIcon(Graphics g, Rectangle iconRect, bool isActive, IBeepTheme? theme)
        {
            if (iconRect.IsEmpty) return;

            var color = isActive 
                ? (theme?.AccentColor ?? Color.DeepSkyBlue) 
                : Color.FromArgb(120, 120, 128);

            using (var pen = new Pen(color, 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                // Funnel shape
                var topY = iconRect.Y + 3;
                var bottomY = iconRect.Bottom - 3;
                var leftX = iconRect.X + 2;
                var rightX = iconRect.Right - 2;
                var midY = iconRect.Y + iconRect.Height / 2;

                g.DrawLine(pen, leftX, topY, rightX, topY);
                g.DrawLine(pen, leftX, topY, iconRect.X + iconRect.Width / 2 - 2, midY);
                g.DrawLine(pen, rightX, topY, iconRect.X + iconRect.Width / 2 + 2, midY);
                g.DrawLine(pen, iconRect.X + iconRect.Width / 2 - 2, midY, 
                    iconRect.X + iconRect.Width / 2 - 2, bottomY);
                g.DrawLine(pen, iconRect.X + iconRect.Width / 2 + 2, midY, 
                    iconRect.X + iconRect.Width / 2 + 2, bottomY);
            }

            // Active indicator
            if (isActive)
            {
                using (var brush = new SolidBrush(theme?.AccentColor ?? Color.DeepSkyBlue))
                {
                    g.FillEllipse(brush, iconRect.Right - 6, iconRect.Y, 4, 4);
                }
            }
        }
    }
}
