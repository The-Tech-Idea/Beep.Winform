using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Microsoft Fluent 2 shadow painter
    /// Uses modern soft shadows
    /// </summary>
    public static class Fluent2ShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            // Fluent2 UX: Subtle elevation with state changes
            if (!StyleShadows.HasShadow(style)) return path;

            // Adjust shadow intensity based on state
            float shadowOpacity = 0.25f; // Base Fluent subtle shadow
            if (state == ControlState.Hovered)
                shadowOpacity = 0.35f; // More prominent on hover
            else if (state == ControlState.Focused)
                shadowOpacity = 0.30f; // Moderate increase on focus
            else if (state == ControlState.Pressed)
                shadowOpacity = 0.15f; // Reduced on press

            // Use StyleShadows for consistent Fluent shadows
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            return ShadowPainterHelpers.PaintSoftShadow(g, path, radius, offsetX, offsetY, shadowColor, shadowOpacity, blur / 2);
        }
    }
}
