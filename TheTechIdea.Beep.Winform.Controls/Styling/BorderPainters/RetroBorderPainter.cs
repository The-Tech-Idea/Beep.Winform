using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Retro border painter - Win95-style beveled frame.
    /// </summary>
    public static class RetroBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Rectangle rect = Rectangle.Round(path.GetBounds());

            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            Color light = Color.FromArgb(255, 255, 255);
            Color dark = Color.FromArgb(128, 128, 128);

            var penLight = PaintersFactory.GetPen(light, 2f);
            var penDark = PaintersFactory.GetPen(dark, 2f);

            g.DrawLine(penLight, rect.Left, rect.Top, rect.Right - 1, rect.Top);
            g.DrawLine(penLight, rect.Left, rect.Top, rect.Left, rect.Bottom - 1);

            g.DrawLine(penDark, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            g.DrawLine(penDark, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);

            var inner = Rectangle.Inflate(rect, -3, -3);
            var penInner = PaintersFactory.GetPen(Color.FromArgb(180, 160, 160, 160), 1f);
            g.DrawRectangle(penInner, inner);

            g.SmoothingMode = oldMode;
            return path.CreateInsetPath(4f);
        }
    }
}
