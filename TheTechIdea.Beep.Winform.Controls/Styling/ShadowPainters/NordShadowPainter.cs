using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Nord shadow painter - Cool arctic-blue tinted shadow
    /// Matches Nord theme's cool color palette
    /// Subtle elevation with blue undertone
    /// </summary>
    public static class NordShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Nord uses cool blue-tinted shadow
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // State-based subtle shadow
            int alpha = state switch
            {
                ControlState.Hovered => 45,
                ControlState.Pressed => 22,
                ControlState.Focused => 40,
                ControlState.Disabled => 12,
                _ => 30
            };

            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius, 0, offsetY, shadowColor, alpha, 2);
        }
    }
}
