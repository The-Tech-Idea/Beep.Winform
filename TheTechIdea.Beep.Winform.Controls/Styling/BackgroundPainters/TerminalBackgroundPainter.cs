using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class TerminalBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors && theme != null ? theme.BackgroundColor : Color.Black;
            Color accent = useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(0x00,0xFF,0x00);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            var bounds = Rectangle.Round(path.GetBounds());
            using var clip = new BackgroundPainterHelpers.ClipScope(g, path);
            var previous = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            var scanPen = PaintersFactory.GetPen(Color.FromArgb(8,255,255,255),1f);
            for (int y = bounds.Top; y < bounds.Bottom; y +=2)
            {
                g.DrawLine(scanPen, bounds.Left, y, bounds.Right, y);
            }

            var gridPen = PaintersFactory.GetPen(Color.FromArgb(5, accent),1f);
            int size =20;
            for (int x = bounds.Left; x < bounds.Right; x += size)
            {
                g.DrawLine(gridPen, x, bounds.Top, x, bounds.Bottom);
            }
            for (int y = bounds.Top; y < bounds.Bottom; y += size)
            {
                g.DrawLine(gridPen, bounds.Left, y, bounds.Right, y);
            }

            g.SmoothingMode = previous;
        }
    }
}
