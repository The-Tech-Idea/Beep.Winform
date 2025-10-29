using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class NordBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors && theme != null ? theme.BackgroundColor : Color.FromArgb(0x2E,0x34,0x40);
            Color frost = Color.FromArgb(0x88,0xC0,0xD0);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);

            var bounds = path.GetBounds();
            var topRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height /3f);
            var frostBrush = PaintersFactory.GetLinearGradientBrush(topRect, Color.FromArgb(10, frost), Color.FromArgb(0, frost), LinearGradientMode.Vertical);
            g.FillRectangle(frostBrush, topRect);

            var pen = PaintersFactory.GetPen(Color.FromArgb(40, frost),1f);
            g.DrawLine(pen, bounds.Left, bounds.Top +0.5f, bounds.Right, bounds.Top +0.5f);
        }
    }
}
