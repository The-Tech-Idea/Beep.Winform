using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Standard shadow painter - Default fallback shadow
    /// Used when no specific style shadow painter is available
    /// Clean, neutral, state-aware
    /// </summary>
    public static class StandardShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Standard shadow - use darker theme color instead of pure black
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

            // Standard: Good default shadow
            // Increased alpha for visibility
            float opacity = state switch
            {
                ControlState.Hovered => 0.4f,
                ControlState.Pressed => 0.2f,
                ControlState.Focused => 0.35f,
                ControlState.Disabled => 0.1f,
                _ => 0.25f
            };

            // Use soft shadow for better quality
            return ShadowPainterHelpers.PaintSoftShadow(
                g, path, radius, 0, offsetY, shadowColor, opacity, 4);
        }
    }
}
