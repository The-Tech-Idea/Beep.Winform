using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Cinnamon shadow painter - Linux Mint desktop environment
    /// Comfortable, familiar shadows for a traditional desktop feel
    /// Slightly warmer and more prominent than GNOME
    /// </summary>
    public static class CinnamonShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Cinnamon uses neutral shadows - use darker theme color instead of pure black
            Color shadowColor = StyleShadows.GetShadowColor(style);
            if (useThemeColors && theme?.ShadowColor != null && theme.ShadowColor != Color.Empty)
            {
                shadowColor = theme.ShadowColor;
            }
            else
            {
                // Use darker gray for more realistic shadows
                shadowColor = Color.FromArgb(30, 30, 30);
            }
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int blur = StyleShadows.GetShadowBlur(style);

            // State-based alpha - Cinnamon has comfortable interactive feedback
            int alpha = state switch
            {
                ControlState.Hovered => 60,    // More prominent on hover
                ControlState.Pressed => 35,    // Reduced when pressed
                ControlState.Focused => 55,    // Moderate focus
                ControlState.Disabled => 20,   // Subtle
                _ => 45                        // Default - slightly more than Elementary
            };

            int spread = blur / 3;

            // Use clean single-layer drop shadow (Linux Mint style)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                spread > 0 ? spread : 3);
        }
    }
}
