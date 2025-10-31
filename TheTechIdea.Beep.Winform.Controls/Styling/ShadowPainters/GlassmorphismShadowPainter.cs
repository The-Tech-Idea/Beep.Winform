using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Glassmorphism shadow painter - soft ambient shadow.
    /// </summary>
    public static class GlassmorphismShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(style))
                return path;

            // Match GlassFormPainter: subtle shadow for glass effect
            return ShadowPainterHelpers.PaintDropShadow(
                g, path, radius,
                0, 2, 8, // offsetX, offsetY, blur
                Color.FromArgb(30, 0, 0, 0), // subtle dark shadow
                0.15f);
        }
    }
}
