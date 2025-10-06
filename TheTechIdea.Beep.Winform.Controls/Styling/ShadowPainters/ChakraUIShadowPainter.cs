using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Chakra UI shadow painter
    /// Uses Chakra UI's shadow tokens
    /// </summary>
    public static class ChakraUIShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            // Chakra UI UX: Shadow tokens with elevation levels
            if (!StyleShadows.HasShadow(style)) return;

            // Chakra shadow tokens based on elevation and state
            float shadowOpacity = 0.18f; // Base Chakra shadow
            int shadowLayers = 4;

            if (state == ControlState.Hovered)
            {
                shadowOpacity = 0.26f; // Increased on hover (Chakra lg token)
                shadowLayers = 5;
            }
            else if (state == ControlState.Focused)
            {
                shadowOpacity = 0.22f; // Moderate increase on focus
                shadowLayers = 5;
            }
            else if (state == ControlState.Pressed)
            {
                shadowOpacity = 0.14f; // Reduced on press
                shadowLayers = 3;
            }

            // Chakra elevation levels
            if (elevation >= MaterialElevation.Level3)
            {
                shadowOpacity += 0.06f;
                shadowLayers += 1;
            }
            else if (elevation >= MaterialElevation.Level2)
            {
                shadowOpacity += 0.03f;
                shadowLayers += 1;
            }

            // Use StyleShadows for consistent Chakra shadows
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, offsetX, offsetY, shadowColor, shadowOpacity, shadowLayers);
        }
    }
}
