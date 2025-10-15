using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Discord style shadow painter
    /// Uses Discord's card shadow system
    /// </summary>
    public static class DiscordStyleShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            // Discord ControlStyle UX: Flat design with minimal state shadows
            if (!StyleShadows.HasShadow(style)) return path;

            // Only show shadow on interaction states (Discord's subtle depth)
            if (state == ControlState.Normal) return path;

            float shadowOpacity = 0.12f; // Minimal
            if (state == ControlState.Hovered)
                shadowOpacity = 0.18f; // Slightly more on hover
            else if (state == ControlState.Focused)
                shadowOpacity = 0.15f; // Moderate on focus
            else if (state == ControlState.Pressed)
                shadowOpacity = 0.08f; // Minimal on press

            // Use StyleShadows for consistent Discord shadows
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            return ShadowPainterHelpers.PaintSoftShadow(g, path, radius, offsetX, offsetY, shadowColor, shadowOpacity, blur / 5);
        }
    }
}
