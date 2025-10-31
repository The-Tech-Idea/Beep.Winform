using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Modern shadow painter - medium blur elevation.
    /// </summary>
    public static class ModernShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(style))
                return path;

            // Match ModernFormPainter shadow: alpha ~25, blur 10, offsetY 4
            var shadowColor = Color.FromArgb(25, 0, 0, 0);
            int offsetX = 0;
            int offsetY = 4;
            int blur = 10;

            return ShadowPainterHelpers.PaintDropShadow(
                g, path, radius,
                offsetX,
                offsetY,
                blur,
                shadowColor,
                1.0f);
        }
    }
}
