using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class ChatBubbleBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors ? theme.BackgroundColor : Color.FromArgb(0xE6, 0xF7, 0xFF);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            var bounds = Rectangle.Round(path.GetBounds());
            using var clip = new BackgroundPainterHelpers.ClipScope(g, path);
            using (var pen = new Pen(Color.FromArgb(12, 0, 0, 0), 1))
            {
                for (int offset = -bounds.Height; offset < bounds.Width; offset += 24)
                {
                    g.DrawLine(pen, bounds.Left + offset, bounds.Top, bounds.Left + offset + bounds.Height, bounds.Bottom);
                }
            }
        }
    }
}
