using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Dracula shadow painter - Purple-tinted subtle shadow
    /// Matches Dracula theme's purple accent aesthetic
    /// Subtle depth without overwhelming dark backgrounds
    /// </summary>
    public static class DraculaShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Dracula uses purple-tinted shadow
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // State-based subtle shadow
            int alpha = state switch
            {
                ControlState.Hovered => 50,
                ControlState.Pressed => 25,
                ControlState.Focused => 45,
                ControlState.Disabled => 15,
                _ => 35
            };

            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius, 0, offsetY, shadowColor, alpha, 2);
        }
    }
}
