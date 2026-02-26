using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Neo-Brutalist background painter - modern brutalism with bold colors
    /// Bright, bold fills with characteristic thick dark outline hint
    /// Aggressive simplicity - flat colors, no gradients
    /// </summary>
    public static class NeoBrutalistBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Neo-Brutalist: bright, bold colors (often yellow, magenta, or white)
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0xF0, 0xF0, 0xF0);

            // Disable anti-aliasing for sharp geometric edges
            using (var scope = new BackgroundPainterHelpers.SmoothingScope(g, SmoothingMode.None))
            {
                // Strong state changes for bold neo-brutalist feedback
                BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                    BackgroundPainterHelpers.StateIntensity.Strong);
            }

            if (BackgroundPainterHelpers.ShouldPaintDecorativeEdgeStroke(style))
            {
                // Neo-Brutalist signature: subtle inner outline hint
                var pen = PaintersFactory.GetPen(Color.FromArgb(20, 0, 0, 0), 1f);
                g.DrawPath(pen, path);
            }
        }
    }
}
