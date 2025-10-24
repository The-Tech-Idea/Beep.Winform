using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Notion minimal shadow painter - no shadow (placeholder for consistency)
    /// Notion Style emphasizes clean, flat design without shadows
    /// </summary>
    public static class NotionMinimalShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level0,
            ControlState state = ControlState.Normal)
        {
            // Notion Minimal UX: Very subtle shadows only on interaction
            if (!StyleShadows.HasShadow(style)) return path;

            // Only show shadow on interaction states (Notion's subtle depth)
            if (state == ControlState.Normal) return path;

            float shadowOpacity = 0.06f; // Very subtle
            if (state == ControlState.Hovered)
                shadowOpacity = 0.10f; // Slightly more on hover
            else if (state == ControlState.Focused)
                shadowOpacity = 0.08f; // Moderate on focus
            else if (state == ControlState.Pressed)
                shadowOpacity = 0.04f; // Minimal on press

            // Use StyleShadows for consistent Notion shadows
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            GraphicsPath remainingPath = ShadowPainterHelpers.PaintSoftShadow(g, path, radius, offsetX, offsetY, shadowColor, shadowOpacity, blur / 6);

            return remainingPath;
        }
    }
}
