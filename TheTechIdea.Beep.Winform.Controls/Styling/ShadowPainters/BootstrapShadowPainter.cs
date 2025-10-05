using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Bootstrap shadow painter
    /// Uses Bootstrap's standard box-shadow
    /// </summary>
    public static class BootstrapShadowPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ShadowPainterHelpers.MaterialElevation elevation = ShadowPainterHelpers.MaterialElevation.Level1)
        {
            // Bootstrap uses subtle shadow
            ShadowPainterHelpers.PaintSoftShadow(g, bounds, radius, 0, 3, Color.Black, 0.18f, 4);
        }
    }
}
