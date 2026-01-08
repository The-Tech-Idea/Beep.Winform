using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// iOS 15 shadow painter
    /// iOS uses very minimal shadows, focusing on blur effects instead
    /// Extremely subtle - iOS prefers flat design with blur backgrounds
    /// </summary>
    public static class iOS15ShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            
            // iOS 15 uses minimal shadows - rely mostly on blur effects
            // Return early for disabled state (no shadow at all)
            if (state == ControlState.Disabled) return path;

            // Check if shadow is enabled for this style
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // iOS shadow - extremely subtle, slightly tinted (iOS shadows are often slightly warm)
            Color shadowColor = StyleShadows.GetShadowColor(style);
            if (useThemeColors && theme?.ShadowColor != null && theme.ShadowColor != Color.Empty)
            {
                shadowColor = theme.ShadowColor;
            }
            else
            {
                // iOS shadows are often slightly warm-tinted, not pure black
                shadowColor = Color.FromArgb(35, 30, 30);
            }

            // iOS uses very minimal shadow alpha
            int alpha = state switch
            {
                ControlState.Hovered => 25,    // Slight increase
                ControlState.Pressed => 10,    // Almost none when pressed
                ControlState.Focused => 20,    // Subtle focus
                ControlState.Selected => 30,   // Slightly more for selection
                _ => 15                        // Default - barely visible
            };

            // Minimal offset for iOS flat aesthetic
            int offsetY = 1;

            // Use soft layered shadow for premium quality (iOS values smooth, subtle shadows)
            return ShadowPainterHelpers.PaintSoftLayeredShadow(
                g, path, radius,
                offsetY, alpha / 255.0f, shadowColor);
        }
    }
}
