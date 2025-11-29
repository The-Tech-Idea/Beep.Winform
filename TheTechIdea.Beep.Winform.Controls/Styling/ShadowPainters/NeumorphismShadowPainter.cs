using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Neumorphism shadow painter - Soft UI dual shadow effect
    /// Light shadow top-left, dark shadow bottom-right
    /// Creates embossed/raised appearance
    /// </summary>
    public static class NeumorphismShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.UsesDualShadows(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Get background color for neumorphic calculation
            Color backgroundColor = theme?.BackColor ?? SystemColors.Control;

            // State-based intensity adjustment
            float intensity = state switch
            {
                ControlState.Hovered => 1.2f,   // More pronounced
                ControlState.Pressed => 0.6f,   // Pressed in (inverted effect)
                ControlState.Focused => 1.1f,   // Slightly more
                ControlState.Disabled => 0.4f,  // Much reduced
                _ => 1.0f                       // Default
            };

            // For pressed state, we could invert the shadows (inner shadow effect)
            if (state == ControlState.Pressed)
            {
                // Pressed: use inner shadow for "pressed in" effect
                return ShadowPainterHelpers.PaintInnerShadow(
                    g, path, radius,
                    4, Color.FromArgb(60, 0, 0, 0));
            }

            // Use neumorphic shadow helper with intensity-adjusted background
            if (intensity != 1.0f)
            {
                // Adjust background brightness based on intensity
                int adjust = (int)((intensity - 1.0f) * 20);
                backgroundColor = Color.FromArgb(
                    Math.Max(0, Math.Min(255, backgroundColor.R + adjust)),
                    Math.Max(0, Math.Min(255, backgroundColor.G + adjust)),
                    Math.Max(0, Math.Min(255, backgroundColor.B + adjust)));
            }

            return ShadowPainterHelpers.PaintNeumorphicShadow(
                g, path, radius, backgroundColor);
        }
    }
}
