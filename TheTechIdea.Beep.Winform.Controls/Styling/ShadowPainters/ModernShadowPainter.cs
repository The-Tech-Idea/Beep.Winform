using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Modern shadow painter - creates clean, professional shadows following contemporary UI/UX best practices.
    /// Inspired by Apple HIG, Google Material Design, and Tailwind CSS shadow scales.
    /// Uses subtle, clean shadows for natural depth perception.
    /// </summary>
    public static class ModernShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // State-based shadow intensity for interactive feedback
            float intensityMultiplier = state switch
            {
                ControlState.Hovered => 1.2f,   // Slightly elevated on hover
                ControlState.Pressed => 0.5f,  // Pressed in (less shadow)
                ControlState.Focused => 1.1f,  // Elevated on focus
                ControlState.Selected => 1.0f, // Same as normal when selected
                ControlState.Disabled => 0.2f, // Very subtle when disabled
                _ => 1.0f                      // Normal state
            };

            // Modern shadow: Clean, subtle single-layer shadow
            // Apple/Tailwind style - not too dark, not too spread
            
            // Use clean drop shadow for professional look
            int offsetY = (int)(2 * intensityMultiplier);
            int alpha = (int)(25 * intensityMultiplier); // Very subtle alpha (25 out of 255)
            int spread = 2;

            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                Color.Black, alpha,
                spread);
        }
    }
}
