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

            // Apple shadow color - very subtle neutral
            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // Apple shadows are extremely subtle - "felt, not seen"
            int alpha = state switch
            {
                ControlState.Hovered => 35,    // Slightly more visible
                ControlState.Pressed => 15,    // Very subtle when pressed
                ControlState.Focused => 30,    // Moderate
                ControlState.Selected => 40,   // Slightly more for selection
                ControlState.Disabled => 8,    // Almost invisible
                _ => 25                        // Default - very subtle
            };

            // Very minimal spread for Apple's refined look
            int spread = 1;

            // Use clean single-layer shadow (Apple refinement)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                spread);
        }
    }
}
