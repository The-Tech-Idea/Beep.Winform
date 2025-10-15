using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Windows 11 Mica shadow painter
    /// Uses subtle mica-style shadows
    /// </summary>
    public static class Windows11MicaShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            // Windows 11 Mica UX: Mica effects don't need traditional shadows
            // Mica gets its depth from the background blur effect, not shadows
            if (!StyleShadows.HasShadow(style))
            {
                // Explicitly no shadow for Mica - intentionally empty
                return path;
            }

            // If for some reason HasShadow returns true, use very minimal shadow
            // But this should not happen for Mica styles
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);

            return ShadowPainterHelpers.PaintSoftShadow(g, path, radius, offsetX, offsetY, shadowColor, 0.08f, blur / 4);
        }
    }
}
