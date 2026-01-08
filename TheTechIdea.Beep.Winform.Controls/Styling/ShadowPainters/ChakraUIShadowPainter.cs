using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Chakra UI shadow painter - Chakra design system shadows
    /// sm, base, md, lg, xl, 2xl, dark-lg, outline, inner
    /// Modern component library shadows
    /// </summary>
    public static class ChakraUIShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Chakra uses neutral shadows - use darker theme color instead of pure black
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

            // Chakra shadow tokens
            int baseAlpha = (int)elevation switch
            {
                0 => 20,    // sm
                1 => 30,    // base
                2 => 40,    // md
                3 => 50,    // lg
                4 => 60,    // xl
                _ => 70     // 2xl
            };

            // State-based Chakra adjustments
            int alpha = state switch
            {
                ControlState.Hovered => (int)(baseAlpha * 1.35f),
                ControlState.Pressed => (int)(baseAlpha * 0.65f),
                ControlState.Focused => (int)(baseAlpha * 1.2f),
                ControlState.Disabled => (int)(baseAlpha * 0.35f),
                _ => baseAlpha
            };

            int spread = Math.Max(2, (int)elevation + 1);

            // Use enhanced dual-layer shadow for better depth (Chakra modern style)
            // Chakra UI benefits from dual-layer shadows for elevation levels
            if ((int)elevation >= 2)
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
