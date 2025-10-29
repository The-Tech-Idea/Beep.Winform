using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class OneDarkBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(0x28, 0x2C, 0x34);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            var bounds = Rectangle.Round(path.GetBounds());
            using var clip = new BackgroundPainterHelpers.ClipScope(g, path);
            var dotBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(25, 255, 255, 255));
            for (int y = bounds.Top; y < bounds.Bottom; y += 40)
            {
                for (int x = bounds.Left; x < bounds.Right; x += 40)
                {
                    g.FillRectangle(dotBrush, x, y, 1, 1);
                }
            }
        }
    }
}
