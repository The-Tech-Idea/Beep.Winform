using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Stripe Dashboard shadow painter - Stripe's professional shadow system
    /// Premium, polished shadows for financial/enterprise UIs
    /// More prominent than typical web shadows for premium feel
    /// </summary>
    public static class StripeDashboardShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Stripe uses neutral shadow - use darker theme color instead of pure black
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

            // Stripe elevation levels (more prominent than typical)
            int baseAlpha = (int)elevation switch
            {
                0 => 25,    // Subtle
                1 => 35,    // Standard
                2 => 45,    // Elevated (default)
                3 => 55,    // Prominent
                4 => 65,    // Modal/overlay
                _ => 75     // Maximum
            };

            // State-based Stripe adjustments
            int alpha = state switch
            {
                ControlState.Hovered => (int)(baseAlpha * 1.4f),
                ControlState.Pressed => (int)(baseAlpha * 0.7f),
                ControlState.Focused => (int)(baseAlpha * 1.25f),
                ControlState.Disabled => (int)(baseAlpha * 0.4f),
                _ => baseAlpha
            };

            int spread = Math.Max(3, (int)elevation + 2);

            // Use enhanced dual-layer shadow for better depth (Stripe premium style)
            // Stripe dashboard cards benefit from dual-layer shadows for premium feel
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
