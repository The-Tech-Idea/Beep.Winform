using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Rendering
{
    public static class BeepRibbonRenderer
    {
        public static void DrawToolStripSurface(Graphics graphics, Rectangle bounds, RibbonTheme theme)
        {
            if (graphics == null || bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            using var back = new SolidBrush(theme.GroupBack);
            graphics.FillRectangle(back, bounds);
            DrawElevationLines(graphics, bounds, theme.ElevationColor, theme.ElevationLevel);
            using var border = new Pen(theme.GroupBorder);
            graphics.DrawRectangle(border, new Rectangle(bounds.Location, new Size(bounds.Width - 1, bounds.Height - 1)));
        }

        public static void DrawInteractiveItem(Graphics graphics, Rectangle bounds, RibbonTheme theme, bool hovered, bool pressed, bool enabled = true, bool selected = false)
        {
            if (graphics == null || bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            Color fill = Color.Transparent;
            Color borderColor = theme.FocusBorder;
            if (!enabled)
            {
                fill = theme.DisabledBack;
                borderColor = theme.DisabledBorder;
            }
            else if (pressed || selected)
            {
                fill = theme.SelectionBack;
            }
            else if (hovered)
            {
                fill = theme.HoverBack;
            }

            if (fill != Color.Transparent)
            {
                using var brush = new SolidBrush(fill);
                int radius = Math.Max(0, theme.CornerRadius);
                if (radius > 1)
                {
                    using var path = CreateRoundedRect(bounds, radius);
                    graphics.FillPath(brush, path);
                }
                else
                {
                    graphics.FillRectangle(brush, bounds);
                }
            }

            if (enabled)
            {
                int elevationLevel = 0;
                if (pressed || selected)
                {
                    elevationLevel = Math.Max(theme.ElevationStrongLevel, theme.ElevationLevel);
                }
                else if (hovered)
                {
                    elevationLevel = Math.Max(0, theme.ElevationLevel - 1);
                }

                if (elevationLevel > 0)
                {
                    var elevationBounds = new Rectangle(bounds.X + 1, bounds.Y + 1, Math.Max(1, bounds.Width - 2), Math.Max(1, bounds.Height - 2));
                    DrawElevationLines(graphics, elevationBounds, theme.ElevationColor, elevationLevel);
                }
            }

            if (!enabled)
            {
                using var disabledBorder = new Pen(theme.DisabledBorder);
                graphics.DrawRectangle(disabledBorder, new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1));
                return;
            }

            if (hovered || pressed || selected)
            {
                float thickness = Math.Clamp(theme.FocusBorderThickness, 1f, 3f);
                using var border = new Pen(borderColor, thickness);
                var drawBounds = new RectangleF(bounds.X, bounds.Y, Math.Max(1, bounds.Width - 1), Math.Max(1, bounds.Height - 1));
                graphics.DrawRectangle(border, drawBounds.X, drawBounds.Y, drawBounds.Width, drawBounds.Height);
            }
        }

        private static void DrawElevationLines(Graphics graphics, Rectangle bounds, Color elevationColor, int level)
        {
            int depth = Math.Clamp(level, 0, 6);
            if (depth <= 0 || elevationColor.A <= 0)
            {
                return;
            }

            for (int i = 0; i < depth; i++)
            {
                int lineY = bounds.Bottom - 1 - i;
                if (lineY <= bounds.Top)
                {
                    break;
                }

                int alpha = Math.Max(4, elevationColor.A - (i * Math.Max(1, elevationColor.A / (depth + 1))));
                using var pen = new Pen(Color.FromArgb(alpha, elevationColor.R, elevationColor.G, elevationColor.B));
                graphics.DrawLine(pen, bounds.Left + 1, lineY, bounds.Right - 2, lineY);
            }
        }

        public static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
        {
            int r = Math.Max(0, radius);
            int d = Math.Max(1, r * 2);
            var path = new GraphicsPath();
            if (r <= 0 || rect.Width < d || rect.Height < d)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
