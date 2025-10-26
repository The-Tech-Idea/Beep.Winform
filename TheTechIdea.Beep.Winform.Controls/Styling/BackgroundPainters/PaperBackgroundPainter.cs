using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class PaperBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors ? theme.BackgroundColor : Color.FromArgb(0xFA, 0xFA, 0xF8);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            var bounds = Rectangle.Round(path.GetBounds());
            using var clip = new BackgroundPainterHelpers.ClipScope(g, path);
            int seed = unchecked(bounds.GetHashCode() * 31);
            var rand = new Random(seed);
            using (var noiseBrush = new SolidBrush(Color.FromArgb(8, 200, 200, 200)))
            {
                int noiseCount = Math.Max(20, (bounds.Width * bounds.Height) / 1500);
                for (int i = 0; i < noiseCount; i++)
                {
                    int x = rand.Next(bounds.Left, bounds.Right);
                    int y = rand.Next(bounds.Top, bounds.Bottom);
                    g.FillRectangle(noiseBrush, x, y, 1, 1);
                }
            }

            using (var pen = new Pen(Color.FromArgb(40, Color.White), 1))
            {
                g.DrawLine(pen, bounds.Left, bounds.Top + 0.5f, bounds.Right, bounds.Top + 0.5f);
            }
        }
    }
}
