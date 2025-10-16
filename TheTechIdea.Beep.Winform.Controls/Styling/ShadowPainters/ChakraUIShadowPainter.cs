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
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(style)) return path;

            // Calculate shadow properties based on state and elevation
            float shadowOpacity = 0.18f;
            int shadowLayers = 4;
            if (state == ControlState.Hovered) { shadowOpacity = 0.26f; shadowLayers = 5; }
            else if (state == ControlState.Focused) { shadowOpacity = 0.22f; shadowLayers = 5; }
            else if (state == ControlState.Pressed) { shadowOpacity = 0.14f; shadowLayers = 3; }
            if (elevation >= MaterialElevation.Level3) { shadowOpacity += 0.06f; shadowLayers += 1; }
            else if (elevation >= MaterialElevation.Level2) { shadowOpacity += 0.03f; shadowLayers += 1; }

            // Paint shadows
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintLayeredShadow(g, path, radius, shadowOpacity, shadowLayers);
            return remainingPath;
        }
    }
}
