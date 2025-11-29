using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Glass background painter - translucent glass/mica effect
    /// Semi-transparent light background with state awareness
    /// </summary>
    public static class GlassBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme, 
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Glass: semi-transparent light gray
            Color glassColor = useThemeColors && theme != null 
                ? BackgroundPainterHelpers.WithAlpha(theme.BackgroundColor, 220)
                : Color.FromArgb(220, 245, 245, 245);

            // Use frosted glass helper for consistent glass effect
            BackgroundPainterHelpers.PaintFrostedGlassBackground(g, path, glassColor,220,state);
        }
    }
}
