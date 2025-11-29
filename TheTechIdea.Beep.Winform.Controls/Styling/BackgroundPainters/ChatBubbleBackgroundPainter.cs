using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Chat Bubble background painter - messaging app aesthetic
    /// Light blue background with subtle diagonal stripe pattern
    /// </summary>
    public static class ChatBubbleBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Chat Bubble: light blue (messaging app style)
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0xE6, 0xF7, 0xFF);

            // Solid background with normal state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Normal);

            var bounds = Rectangle.Round(path.GetBounds());
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Subtle diagonal stripe pattern (chat bubble texture)
            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            using (var scope = new BackgroundPainterHelpers.SmoothingScope(g, SmoothingMode.None))
            {
                var pen = PaintersFactory.GetPen(Color.FromArgb(10, 0, 0, 0), 1);
                for (int offset = -bounds.Height; offset < bounds.Width; offset += 24)
                {
                    g.DrawLine(pen, 
                        bounds.Left + offset, bounds.Top, 
                        bounds.Left + offset + bounds.Height, bounds.Bottom);
                }
            }
        }
    }
}
