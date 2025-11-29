using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Glassmorphism background painter - frosted glass UI trend
    /// Semi-transparent background with blur simulation and top highlight
    /// </summary>
    public static class GlassmorphismBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Glassmorphism: semi-transparent dark base
            Color baseColor = useThemeColors && theme != null
                ? BackgroundPainterHelpers.WithAlpha(theme.BackgroundColor, 200)
                : Color.FromArgb(200, 30, 40, 60);

            // Use the frosted glass helper
            BackgroundPainterHelpers.PaintFrostedGlassBackground(g, path, baseColor, 
                state == ControlState.Disabled ? 120 : 200, state);
        }
    }
}
