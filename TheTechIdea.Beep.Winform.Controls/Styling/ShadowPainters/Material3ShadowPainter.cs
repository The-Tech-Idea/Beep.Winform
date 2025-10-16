using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Material Design 3 shadow painter
    /// Uses Material elevation levels for consistent shadows
    /// </summary>
    public static class Material3ShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            // Material3 UX: Elevation-based shadows with state awareness
            if (!StyleShadows.HasShadow(style)) return path;

            // Adjust elevation based on state
            var actualElevation = elevation;
            switch (state)
            {
                case ControlState.Hovered:
                    actualElevation = (MaterialElevation)Math.Min(5, (int)elevation + 1);
                    break;
                case ControlState.Pressed:
                    actualElevation = (MaterialElevation)Math.Max(0, (int)elevation - 1);
                    break;
                case ControlState.Focused:
                    actualElevation = (MaterialElevation)Math.Min(5, (int)elevation + 1);
                    break;
                case ControlState.Disabled:
                    actualElevation = MaterialElevation.Level0; // No elevation when disabled
                    break;
                default: // Normal
                    break;
            }

            // Paint shadows
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintSoftShadow(g, path, radius, 0, 0, StyleShadows.GetShadowColor(style), 0.6f, StyleShadows.GetShadowBlur(style) / 2);
            return remainingPath;
            // Return the area inside the shadow using shape-aware inset
           // return path.CreateInsetPath(radius);
        }
    }
}
