using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Brutalist shadow painter - hard offset rectangle shadow.
    /// </summary>
    public static class BrutalistShadowPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, int radius,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (!StyleShadows.HasShadow(style))
                return path;

            Rectangle shadowRect = Rectangle.Round(path.GetBounds());
            shadowRect.Offset(6, 6);

            var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(80, 0, 0, 0));
            g.FillRectangle(brush, shadowRect);

            return path.CreateInsetPath(0f);
        }
    }
}
