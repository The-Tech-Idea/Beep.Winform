using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// GNOME shadow painter - Adwaita/libadwaita design system
    /// Clean, subtle drop shadow with state-aware feedback
    /// Professional, welcoming appearance matching GTK4/GNOME HIG
    /// </summary>
    public static class GnomeShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // GNOME/Adwaita uses clean neutral shadows
            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int spread = StyleShadows.GetShadowSpread(style);

            // State-based alpha - Adwaita has subtle interactive feedback
            int alpha = state switch
            {
                ControlState.Hovered => 50,    // Slightly more prominent
                ControlState.Pressed => 25,    // Subtle when pressed
                ControlState.Focused => 45,    // Moderate focus
                ControlState.Disabled => 15,   // Very subtle
                _ => 35                        // Default subtle shadow
            };

            // Use clean single-layer drop shadow (authentic GNOME style)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                spread > 0 ? spread : 2);
        }
    }
}
