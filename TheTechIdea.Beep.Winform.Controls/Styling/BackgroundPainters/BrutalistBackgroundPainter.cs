using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Brutalist background painter - raw, honest, geometric design
    /// Flat solid colors with sharp edges - no decoration, no gradients
    /// Anti-aliasing disabled for maximum geometric precision
    /// </summary>
    public static class BrutalistBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Brutalist: raw concrete-like gray
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0xF2, 0xF2, 0xF2);

            // Disable anti-aliasing for sharp, brutalist geometric edges
            using (var scope = new BackgroundPainterHelpers.SmoothingScope(g, SmoothingMode.None))
            {
                // Strong state changes for bold brutalist feedback
                BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                    BackgroundPainterHelpers.StateIntensity.Strong);
            }
        }
    }
}
