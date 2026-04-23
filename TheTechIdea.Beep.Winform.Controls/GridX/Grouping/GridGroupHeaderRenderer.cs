using System;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Grouping
{
    /// <summary>
    /// Paints group header rows inside the grid body.
    /// Called from <see cref="Helpers.GridRenderHelper"/> when grouping is active.
    /// </summary>
    public sealed class GridGroupHeaderRenderer
    {
        private readonly BeepGridPro _grid;

        public GridGroupHeaderRenderer(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }

        /// <summary>
        /// Paint a group header band at the specified Y offset.
        /// </summary>
        public void PaintHeader(Graphics g, Rectangle bounds, GridGroup group, bool isHovered, bool isPressed)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            var theme = _grid.Theme != null ? BeepThemesManager.GetTheme(_grid.Theme) : BeepThemesManager.GetDefaultTheme();
            var backColor = theme?.GridHeaderBackColor ?? SystemColors.Control;
            var foreColor = theme?.GridHeaderForeColor ?? SystemColors.ControlText;
            var accentColor = theme?.AccentColor ?? Color.DeepSkyBlue;
            var lineColor = theme?.GridLineColor ?? SystemColors.ControlDark;

            // Background
            using var bgBrush = new SolidBrush(backColor);
            g.FillRectangle(bgBrush, bounds);

            // Indent based on nesting level
            int indent = group.Level * 16;
            int x = bounds.X + indent + 8;
            int cy = bounds.Y + bounds.Height / 2;

            // Expand/collapse chevron
            PaintChevron(g, x, cy, group.IsCollapsed, foreColor);
            x += 20;

            // Group label
            var label = $"{group.Label} ({group.RowIndices.Count})";
            TextRenderer.DrawText(g, label, _grid.Font, new Rectangle(x, bounds.Y, bounds.Right - x - 4, bounds.Height),
                foreColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // Bottom separator
            using var linePen = new Pen(lineColor, 1);
            g.DrawLine(linePen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);

            // Hover / pressed feedback
            if (isPressed)
            {
                using var pressBrush = new SolidBrush(Color.FromArgb(40, accentColor));
                g.FillRectangle(pressBrush, bounds);
            }
            else if (isHovered)
            {
                using var hoverBrush = new SolidBrush(Color.FromArgb(20, accentColor));
                g.FillRectangle(hoverBrush, bounds);
            }
        }

        /// <summary>
        /// Hit-test whether a point falls inside the expand/collapse chevron of a group header.
        /// </summary>
        public bool HitTestChevron(Point p, Rectangle headerBounds, GridGroup group)
        {
            int indent = group.Level * 16;
            int cx = headerBounds.X + indent + 8 + 6; // center of chevron
            int cy = headerBounds.Y + headerBounds.Height / 2;
            var chevronRect = new Rectangle(cx - 8, cy - 8, 16, 16);
            return chevronRect.Contains(p);
        }

        /// <summary>
        /// Recommended height for a group header row.
        /// </summary>
        public int GetHeaderHeight()
        {
            return Math.Max(22, _grid.RowHeight);
        }

        private static void PaintChevron(Graphics g, int x, int y, bool collapsed, Color color)
        {
            // Simple triangle chevron
            Point[] points;
            if (collapsed)
            {
                points = new[]
                {
                    new Point(x, y - 4),
                    new Point(x + 6, y),
                    new Point(x, y + 4)
                };
            }
            else
            {
                points = new[]
                {
                    new Point(x - 3, y - 3),
                    new Point(x + 3, y - 3),
                    new Point(x, y + 3)
                };
            }

            using var brush = new SolidBrush(color);
            g.FillPolygon(brush, points);
        }
    }
}
