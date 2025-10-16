using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Ant Design shadow painter
    /// Uses Ant Design's elevation shadow system
    /// </summary>
    public static class AntDesignShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            // Ant Design UX: Proper shadow hierarchy with elevation levels
            if (!StyleShadows.HasShadow(style)) return path;

            // Ant Design shadow hierarchy based on elevation and state
            float shadowOpacity = 0.16f; // Base Ant Design shadow
            int shadowLayers = 4;

            if (state == ControlState.Hovered)
            {
                shadowOpacity = 0.24f; // Increased on hover
                shadowLayers = 5;
            }
            else if (state == ControlState.Focused)
            {
                shadowOpacity = 0.20f; // Moderate increase on focus
                shadowLayers = 5;
            }
            else if (state == ControlState.Pressed)
            {
                shadowOpacity = 0.12f; // Reduced on press
                shadowLayers = 3;
            }

            // Adjust based on elevation level
            if (elevation >= MaterialElevation.Level3)
            {
                shadowOpacity += 0.08f;
                shadowLayers += 2;
            }

            // Paint shadows
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintLayeredShadow(g, path, radius, shadowOpacity, shadowLayers);

            return remainingPath;
        }
    }
}
