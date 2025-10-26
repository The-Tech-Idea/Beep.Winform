using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    public static class RetroBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return;

            Color baseColor = useThemeColors ? theme.BackgroundColor : Color.FromArgb(0xD8, 0xD8, 0xD8);
            Color fillColor = BackgroundPainterHelpers.ApplyState(baseColor, state);

            using (var brush = new SolidBrush(fillColor))
            {
                g.FillPath(brush, path);
            }

            var bounds = Rectangle.Round(path.GetBounds());
            using var clip = new BackgroundPainterHelpers.ClipScope(g, path);
            var previous = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;
            using (var scanPen = new Pen(Color.FromArgb(25, 0, 0, 0), 1))
            {
                for (int y = bounds.Top; y < bounds.Bottom; y += 3)
                {
                    g.DrawLine(scanPen, bounds.Left, y, bounds.Right, y);
                }
            }
            using (var hatchBrush = new HatchBrush(HatchStyle.Percent50, Color.FromArgb(12, 0, 0, 0), Color.Transparent))
            {
                g.FillRectangle(hatchBrush, bounds);
            }
            g.SmoothingMode = previous;
        }
    }
}
