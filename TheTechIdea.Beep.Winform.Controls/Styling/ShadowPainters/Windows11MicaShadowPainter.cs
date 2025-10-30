using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Windows 11 Mica shadow painter
    /// Uses subtle mica-Style shadows
    /// </summary>
    public static class Windows11MicaShadowPainter
    {
       public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            MaterialElevation elevation = MaterialElevation.Level1,
            ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(style)) return path;

            // Paint minimal shadow for Mica Style
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintSoftShadow(g, path, radius, 0, 0, StyleShadows.GetShadowColor(style), 0.08f, StyleShadows.GetShadowBlur(style) / 4);
            return remainingPath;
        }
    }
}
