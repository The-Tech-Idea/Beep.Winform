using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class HolographicBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors ? theme.BackgroundColor : Color.FromArgb(24, 17, 47);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            var bounds = path.GetBounds();
            using (var gradient = new LinearGradientBrush(bounds, Color.Magenta, Color.Cyan, LinearGradientMode.Horizontal))
            {
                gradient.InterpolationColors = new ColorBlend
                {
                    Colors = new[]
                    {
                        Color.FromArgb(40, 255, 0, 150),
                        Color.FromArgb(40, 255, 200, 0),
                        Color.FromArgb(40, 0, 255, 100),
                        Color.FromArgb(40, 100, 150, 255),
                        Color.FromArgb(40, 255, 0, 200)
                    },
                    Positions = new[] { 0f, 0.25f, 0.5f, 0.75f, 1f }
                };
                g.FillPath(gradient, path);
            }

            using (var shinePen = new Pen(Color.FromArgb(40, Color.White), 3))
            {
                g.DrawLine(shinePen, bounds.Left, bounds.Top + bounds.Height / 3f, bounds.Right, bounds.Top);
            }
        }
    }
}
