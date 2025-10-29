using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class UbuntuBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color topColor = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(0xE9,0x54,0x20);
            Color bottomColor = useThemeColors && theme != null ? BackgroundPainterHelpers.Darken(theme.BackgroundColor,0.25f) : Color.FromArgb(0x7F,0x2A,0x81);

            Color fillTop = BackgroundPainterHelpers.ApplyState(topColor, state);
            Color fillBottom = BackgroundPainterHelpers.ApplyState(bottomColor, state);

            var bounds = path.GetBounds();
            var gradient = PaintersFactory.GetLinearGradientBrush(bounds, fillTop, fillBottom, LinearGradientMode.Vertical);
            g.FillPath(gradient, path);

            var accentBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(150,233,84,32));
            var accentRect = new RectangleF(bounds.Left, bounds.Top, Math.Min(4f, bounds.Width), bounds.Height);
            g.FillRectangle(accentBrush, accentRect);
        }
    }
}
