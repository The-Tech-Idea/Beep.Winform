using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Shadow painter for standard single shadow styles
    /// Used by most styles that have simple drop shadows
    /// </summary>
    public static class StandardShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, BeepControlStyle style, GraphicsPath borderPath)
        {
            if (!StyleShadows.HasShadow(style))
                return path;

            int blur = StyleShadows.GetShadowBlur(style);
            int offsetY = StyleShadows.GetShadowOffsetY(style);
            int offsetX = StyleShadows.GetShadowOffsetX(style);
            Color shadowColor = StyleShadows.GetShadowColor(style);
            int radius = StyleBorders.GetRadius(style);

            // Use helper for standard shadow
            GraphicsPath remainingPath = ShadowPainterHelpers.PaintSoftShadow(
                g, path, radius, offsetX, offsetY, shadowColor, 0.16f, blur);

            return remainingPath;
        }
    }
}
