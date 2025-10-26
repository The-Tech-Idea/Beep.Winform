using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class CyberpunkBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors ? theme.BackgroundColor : Color.FromArgb(0x08, 0x0B, 0x14);
            Color accent = useThemeColors ? theme.AccentColor : Color.FromArgb(0x00, 0xFF, 0xFF);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            var bounds = Rectangle.Round(path.GetBounds());
            using var clip = new BackgroundPainterHelpers.ClipScope(g, path);
            using (var scanPen = new Pen(Color.FromArgb(20, accent), 1))
            {
                for (int y = bounds.Top; y < bounds.Bottom; y += 4)
                {
                    g.DrawLine(scanPen, bounds.Left, y, bounds.Right, y);
                }
            }

            int seed = unchecked(bounds.GetHashCode() * 17);
            var rand = new Random(seed);
            using (var glitchBrush = new SolidBrush(Color.FromArgb(80, accent)))
            {
                int glitchCount = Math.Max(1, (bounds.Width * bounds.Height) / 4000);
                for (int i = 0; i < glitchCount; i++)
                {
                    if (rand.NextDouble() < 0.25)
                    {
                        int width = rand.Next(40, 120);
                        int height = rand.Next(1, 4);
                        int x = rand.Next(bounds.Left, Math.Max(bounds.Left, bounds.Right - width));
                        int y = rand.Next(bounds.Top, bounds.Bottom);
                        g.FillRectangle(glitchBrush, x, y, width, height);
                    }
                }
            }
        }
    }
}
