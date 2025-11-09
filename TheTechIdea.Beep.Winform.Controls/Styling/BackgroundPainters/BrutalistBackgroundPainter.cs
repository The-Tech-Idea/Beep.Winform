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

            // Brutalist: Bold, flat, solid colors - no gradients, no patterns
            Color baseColor = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(0xF2, 0xF2, 0xF2);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            // Disable anti-aliasing for sharp, geometric edges
            var previousSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            g.SmoothingMode = previousSmoothing;
        }
    }
}
