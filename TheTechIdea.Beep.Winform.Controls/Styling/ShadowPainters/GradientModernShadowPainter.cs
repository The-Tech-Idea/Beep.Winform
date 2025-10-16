using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Gradient modern shadow painter
    /// Uses modern soft shadow for gradient backgrounds
    /// </summary>
    public static class GradientModernShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            // Gradient Modern UX: Deep shadows with enhanced state depth
            if (!StyleShadows.HasShadow(style)) return path;

            // Get base shadow values from StyleShadows
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);
            Color shadowColor = StyleShadows.GetShadowColor(style);

            // Gradient modern shadow depth based on state
            float shadowOpacity = 0.3f; // Base deep shadow

            switch (state)
            {
                case ControlState.Hovered:
                    shadowOpacity = 0.42f; // Deeper on hover
                    blur += 4; // More blur on hover
                    break;
                case ControlState.Focused:
                    shadowOpacity = 0.36f; // Moderate increase on focus
                    blur += 2; // Slightly more blur on focus
                    break;
                case ControlState.Pressed:
                    shadowOpacity = 0.24f; // Reduced on press
                    blur -= 2; // Less blur when pressed
                    offsetY = Math.Max(1, offsetY - 2); // Closer shadow
                    break;
                case ControlState.Disabled:
                    shadowOpacity = 0.15f; // Much reduced when disabled
                    blur -= 4; // Less blur when disabled
                    break;
                default: // Normal
                    break;
            }

            // Gradient modern elevation levels
            if (elevation >= MaterialElevation.Level4)
            {
                shadowOpacity += 0.12f; // Very deep
                blur += 4;
            }
            else if (elevation >= MaterialElevation.Level3)
            {
                shadowOpacity += 0.08f; // Deeper
                blur += 2;
            }

            // Paint shadows
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintSoftShadow(g, path, radius, offsetX, offsetY, shadowColor, shadowOpacity, blur);

            return remainingPath;
        }
    }
}
