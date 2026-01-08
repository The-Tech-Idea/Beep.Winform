using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Elementary OS shadow painter - Pantheon design system
    /// Refined, subtle shadows for a polished desktop experience
    /// Similar to GNOME but with slightly more definition
    /// </summary>
    public static class ElementaryShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Elementary uses clean neutral shadows - use darker theme color instead of pure black
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

            // State-based alpha - Elementary has refined interactive feedback
            int alpha = state switch
            {
                ControlState.Hovered => 55,    // More visible on hover
                ControlState.Pressed => 30,    // Reduced when pressed
                ControlState.Focused => 50,    // Moderate focus
                ControlState.Disabled => 15,   // Minimal
                _ => 40                        // Default - slightly more than GNOME
            };

            int spread = blur / 3;

            // Use clean single-layer drop shadow (Pantheon style)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                spread > 0 ? spread : 2);
        }
    }
}
