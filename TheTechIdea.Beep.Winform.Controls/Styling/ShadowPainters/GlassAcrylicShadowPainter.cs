using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Glass acrylic shadow painter
    /// Uses soft, diffused shadow for frosted glass effect
    /// </summary>
    public static class GlassAcrylicShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level2,
            ControlState state = ControlState.Normal)
        {
            // Glass Acrylic UX: Glass effects don't need traditional shadows
            // Glass gets its depth from the background blur effect, not shadows
            if (!StyleShadows.HasShadow(style))
            {
                // Explicitly no shadow for glass styles - intentionally empty
                return path;
            }

            // If for some reason HasShadow returns true, use very minimal shadow
            // But this should not happen for glass styles
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            return ShadowPainterHelpers.PaintSoftShadow(g, path, radius, offsetX, offsetY, shadowColor, 0.08f, blur / 6);
        }
    }
}
