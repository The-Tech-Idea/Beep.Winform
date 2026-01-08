using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Ant Design shadow painter - Ant Design elevation system
    /// Professional enterprise UI shadows
    /// boxShadowBase, boxShadowSecondary, boxShadowTertiary
    /// </summary>
    public static class AntDesignShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Ant Design neutral shadow - use darker theme color instead of pure black
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

            // Ant Design elevation hierarchy
            int baseAlpha = (int)elevation switch
            {
                0 => 15,    // Minimal
                1 => 25,    // boxShadowBase
                2 => 35,    // boxShadowSecondary  
                3 => 45,    // boxShadowTertiary
                _ => 55     // Elevated
            };

            // State-based Ant Design adjustments
            int alpha = state switch
            {
                ControlState.Hovered => (int)(baseAlpha * 1.4f),
                ControlState.Pressed => (int)(baseAlpha * 0.6f),
                ControlState.Focused => (int)(baseAlpha * 1.25f),
                ControlState.Disabled => (int)(baseAlpha * 0.4f),
                _ => baseAlpha
            };

            int spread = Math.Max(2, (int)elevation + 1);

            // Use enhanced dual-layer shadow for better depth (Ant Design professional)
            // Ant Design benefits from Material-style dual-layer shadows
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
