using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class CartoonBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors ? theme.BackgroundColor : Color.FromArgb(255, 223, 127);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            var bounds = Rectangle.Round(path.GetBounds());
            using var clip = new BackgroundPainterHelpers.ClipScope(g, path);
            using (var dotBrush = new SolidBrush(Color.FromArgb(25, 0, 0, 0)))
            {
                for (int y = bounds.Top; y < bounds.Bottom; y += 8)
                {
                    for (int x = bounds.Left; x < bounds.Right; x += 8)
                    {
                        g.FillRectangle(dotBrush, x, y, 1, 1);
                    }
                }
            }
        }
    }
}
