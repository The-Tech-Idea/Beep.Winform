using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class BrutalistBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(0xF2, 0xF2, 0xF2);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            var bounds = Rectangle.Round(path.GetBounds());
            using var clip = new BackgroundPainterHelpers.ClipScope(g, path);
            var previousSmoothing = g.SmoothingMode;
            var previousCompositing = g.CompositingMode;
            
            g.SmoothingMode = SmoothingMode.None;
            g.CompositingMode = CompositingMode.SourceOver; // Ensure proper alpha blending
            
            var pen = PaintersFactory.GetPen(Color.FromArgb(30, 0, 0, 0), 1);
            for (int x = bounds.Left; x < bounds.Right; x += 40)
            {
                g.DrawLine(pen, x, bounds.Top, x, bounds.Bottom);
            }
            
            g.SmoothingMode = previousSmoothing;
            g.CompositingMode = previousCompositing;
        }
    }
}
