using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Minimal shadow painter - no shadow (placeholder for consistency)
    /// Minimal design doesn't use shadows
    /// </summary>
    public static class MinimalShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            // Minimal UX: Very subtle shadows only on interaction
                        if (!StyleShadows.HasShadow(style)) return path;

            // Only show shadow on interaction states
                        if (state == ControlState.Normal) return path;

            float shadowOpacity = 0.08f; // Very subtle
            if (state == ControlState.Hovered)
                shadowOpacity = 0.12f; // Slightly more on hover
            else if (state == ControlState.Focused)
                shadowOpacity = 0.10f; // Moderate on focus
            else if (state == ControlState.Pressed)
                shadowOpacity = 0.05f; // Minimal on press

            // Use StyleShadows for consistent minimal shadows
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            GraphicsPath remainingPath = ShadowPainterHelpers.PaintSoftShadow(g, path, radius, offsetX, offsetY, shadowColor, shadowOpacity, blur / 4);

            return remainingPath;
        }
    }
}
