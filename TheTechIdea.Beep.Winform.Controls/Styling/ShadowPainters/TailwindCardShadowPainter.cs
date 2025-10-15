using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Tailwind card shadow painter
    /// Uses Tailwind's ring-shadow system with subtle depth
    /// </summary>
    public static class TailwindCardShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            // Tailwind Card UX: Shadow variants with ring-shadow system
            if (!StyleShadows.HasShadow(style)) return path;

            // Tailwind shadow variants based on state
            float shadowOpacity = 0.15f; // Base Tailwind shadow
            int shadowLayers = 4;

            if (state == ControlState.Hovered)
            {
                shadowOpacity = 0.25f; // Tailwind shadow-lg equivalent
                shadowLayers = 6;
            }
            else if (state == ControlState.Focused)
            {
                shadowOpacity = 0.20f; // Tailwind shadow-md equivalent
                shadowLayers = 5;
            }
            else if (state == ControlState.Pressed)
            {
                shadowOpacity = 0.10f; // Reduced on press
                shadowLayers = 3;
            }

            // Tailwind elevation levels
            if (elevation >= MaterialElevation.Level3)
            {
                shadowOpacity += 0.10f; // shadow-xl
                shadowLayers += 2;
            }
            else if (elevation >= MaterialElevation.Level2)
            {
                shadowOpacity += 0.05f; // shadow-lg
                shadowLayers += 1;
            }

            // Use StyleShadows for consistent Tailwind shadows
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            return ShadowPainterHelpers.PaintSoftShadow(g, path, radius, offsetX, offsetY, shadowColor, shadowOpacity, shadowLayers);
        }
    }
}
