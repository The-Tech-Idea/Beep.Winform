using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class TokyoBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(0x1A,0x1B,0x27);
            Color accent = useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(0x7A,0xA2,0xF7);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            var bounds = path.GetBounds();
            var glowRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, Math.Min(100f, bounds.Height));
            var glow = PaintersFactory.GetLinearGradientBrush(glowRect, Color.FromArgb(30, accent), Color.Transparent, LinearGradientMode.Vertical);
            g.FillRectangle(glow, glowRect);

            var neonPen = PaintersFactory.GetPen(Color.FromArgb(80, accent),2f);
            g.DrawLine(neonPen, bounds.Left, bounds.Top +0.5f, bounds.Right, bounds.Top +0.5f);

            var rectBounds = Rectangle.Round(bounds);
            using var clip = new BackgroundPainterHelpers.ClipScope(g, path);
            var scanPen = PaintersFactory.GetPen(Color.FromArgb(10, Color.White),1f);
            for (int y = rectBounds.Top; y < rectBounds.Bottom; y +=4)
            {
                g.DrawLine(scanPen, rectBounds.Left, y, rectBounds.Right, y);
            }
        }
    }
}
