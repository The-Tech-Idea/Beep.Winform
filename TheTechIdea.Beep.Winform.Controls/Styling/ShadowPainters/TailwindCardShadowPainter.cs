using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Tailwind Card shadow painter - Tailwind CSS shadow utilities
    /// shadow-sm, shadow, shadow-md, shadow-lg, shadow-xl, shadow-2xl
    /// Clean, modern card shadows
    /// </summary>
    public static class TailwindCardShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Tailwind uses neutral shadows - use darker theme color instead of pure black
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

            // Tailwind shadow levels
            int baseAlpha = (int)elevation switch
            {
                0 => 15,    // shadow-sm
                1 => 25,    // shadow
                2 => 35,    // shadow-md
                3 => 45,    // shadow-lg
                4 => 55,    // shadow-xl
                _ => 65     // shadow-2xl
            };

            // State-based adjustments
            int alpha = state switch
            {
                ControlState.Hovered => (int)(baseAlpha * 1.4f),  // shadow-lg on hover
                ControlState.Pressed => (int)(baseAlpha * 0.6f),  // shadow-sm on press
                ControlState.Focused => (int)(baseAlpha * 1.2f),  // shadow-md on focus
                ControlState.Disabled => (int)(baseAlpha * 0.3f),
                _ => baseAlpha
            };

            int spread = Math.Max(2, (int)elevation + 1);

            // Use enhanced dual-layer shadow for better depth (Tailwind modern style)
            // Tailwind shadow-lg and above benefit from dual-layer shadows
            if ((int)elevation >= 3)
            {
                return ShadowPainterHelpers.PaintDualLayerShadow(
                    g, path, radius,
                    (int)elevation,
                    shadowColor);
            }
            else
            {
                // Use clean drop shadow for lower elevations
                return ShadowPainterHelpers.PaintCleanDropShadow(
                    g, path, radius,
                    0, offsetY,
                    shadowColor, alpha,
                    spread);
            }
        }
    }
}
