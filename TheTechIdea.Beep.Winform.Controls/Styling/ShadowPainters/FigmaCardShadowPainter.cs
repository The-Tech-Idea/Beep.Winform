using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Figma card shadow painter
    /// Uses Figma's design system shadow
    /// </summary>
    public static class FigmaCardShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal,
            MaterialElevation elevation = MaterialElevation.Level1)
        {
            if (!StyleShadows.HasShadow(style))
                return path;

            // Calculate shadow properties based on state
            float opacity = state switch
            {
                ControlState.Hovered => 0.20f,    // More visible on hover
                ControlState.Focused => 0.18f,  // Slightly more visible when focused
                ControlState.Pressed => 0.12f,  // Less visible when pressed
                ControlState.Disabled => 0.08f, // Very subtle when disabled
                _ => 0.16f  // Normal state
            };

            // Paint shadows
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintSoftShadow(g, path, radius, 0, 0, StyleShadows.GetShadowColor(style), opacity, StyleShadows.GetShadowBlur(style));

            return remainingPath;
        }
    }
}
