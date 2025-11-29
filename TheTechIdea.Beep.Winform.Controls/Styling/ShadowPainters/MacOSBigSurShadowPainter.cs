using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// macOS Big Sur shadow painter
    /// Soft, vibrant shadows with subtle state feedback
    /// Matches macOS layered window appearance
    /// </summary>
    public static class MacOSBigSurShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return path;
            if (!StyleShadows.HasShadow(style)) return path;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // macOS Big Sur shadow color - soft neutral
            Color shadowColor = Color.Black;
            int offsetY = StyleShadows.GetShadowOffsetY(style);

            // macOS shadow intensity - soft and vibrant
            int alpha = state switch
            {
                ControlState.Hovered => 45,    // More vibrant on hover
                ControlState.Pressed => 20,    // Reduced on press
                ControlState.Focused => 40,    // Moderate focus
                ControlState.Selected => 50,   // Slightly more for selection
                ControlState.Disabled => 10,   // Minimal
                _ => 30                        // Default - soft
            };

            // macOS uses slightly larger spread for soft appearance
            int spread = 2;

            // Use clean single-layer shadow (macOS softness)
            return ShadowPainterHelpers.PaintCleanDropShadow(
                g, path, radius,
                0, offsetY,
                shadowColor, alpha,
                spread);
        }
    }
}
