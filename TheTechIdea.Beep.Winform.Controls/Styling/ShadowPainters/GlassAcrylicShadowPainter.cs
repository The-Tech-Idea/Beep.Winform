using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Glass Acrylic shadow painter - Windows acrylic blur shadow
    /// Very subtle shadow for acrylic/blur backgrounds
    /// Minimal to not compete with blur effect
    /// </summary>
    public static class GlassAcrylicShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Glass Acrylic: Very subtle shadow - use soft layered for glass effects
            int alpha = state switch
            {
                ControlState.Hovered => 25,
                ControlState.Pressed => 12,
                ControlState.Focused => 22,
                ControlState.Disabled => 8,
                _ => 15
            };

            // Get shadow color
            Color shadowColor = StyleShadows.GetShadowColor(style);
            if (useThemeColors && theme?.ShadowColor != null && theme.ShadowColor != Color.Empty)
            {
                shadowColor = theme.ShadowColor;
            }
            else
            {
                shadowColor = Color.FromArgb(30, 30, 30);
            }

            // Use soft layered shadow for glass effects (needs soft shadows)
            return ShadowPainterHelpers.PaintSoftLayeredShadow(
                g, path, radius,
                2, alpha / 255.0f, shadowColor);
        }
    }
}
