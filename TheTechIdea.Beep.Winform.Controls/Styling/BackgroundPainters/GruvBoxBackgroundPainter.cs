using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class GruvBoxBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors ? theme.BackgroundColor : Color.FromArgb(0x3C, 0x38, 0x36);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            var bounds = Rectangle.Round(path.GetBounds());
            using var clip = new BackgroundPainterHelpers.ClipScope(g, path);
            using (var grainPen = new Pen(Color.FromArgb(15, 251, 184, 108), 1))
            {
                for (int y = bounds.Top; y < bounds.Bottom; y += 3)
                {
                    g.DrawLine(grainPen, bounds.Left, y, bounds.Right, y);
                }
            }

            var topRect = new Rectangle(bounds.Left, bounds.Top, bounds.Width, Math.Max(1, bounds.Height / 6));
            using (var glow = new LinearGradientBrush(topRect, Color.FromArgb(40, 251, 184, 108), Color.Transparent, LinearGradientMode.Vertical))
            {
                g.FillRectangle(glow, topRect);
            }
        }
    }
}
