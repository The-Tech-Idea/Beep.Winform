using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Apple shadow painter - Apple Design Language
    /// Very subtle, refined shadows that feel natural
    /// Clean and unobtrusive - shadows should be "felt, not seen"
    /// </summary>
    public static class AppleShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Apple shadow color - very subtle, slightly warm-tinted (Apple shadows are often warm)
            Color shadowColor = StyleShadows.GetShadowColor(style);
            if (useThemeColors && theme?.ShadowColor != null && theme.ShadowColor != Color.Empty)
            {
                shadowColor = theme.ShadowColor;
            }
            else
            {
                // Apple shadows are often slightly warm-tinted, not pure black
                shadowColor = Color.FromArgb(40, 35, 35);
            }
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Apple shadows are extremely subtle - "felt, not seen"
            // Increased opacity for better visibility while maintaining subtlety
            float opacity = state switch
            {
                ControlState.Hovered => 0.25f,    // Slightly more visible
                ControlState.Pressed => 0.1f,    // Very subtle when pressed
                ControlState.Focused => 0.2f,    // Moderate
                ControlState.Selected => 0.3f,   // Slightly more for selection
                ControlState.Disabled => 0.05f,    // Almost invisible
                _ => 0.15f                        // Default - very subtle
            };

            // Use soft shadow with max layers for Apple's refined, diffused look
            return ShadowPainterHelpers.PaintSoftShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, opacity,
                8);
        }
    }
}
