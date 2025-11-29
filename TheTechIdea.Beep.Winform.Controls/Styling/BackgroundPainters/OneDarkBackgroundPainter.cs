using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// One Dark theme background painter - Atom editor's iconic dark theme
    /// Clean dark background with subtle dot grid pattern
    /// </summary>
    public static class OneDarkBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // One Dark's signature dark gray background
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0x28, 0x2C, 0x34);

            // Paint solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = Rectangle.Round(path.GetBounds());
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Subtle dot grid pattern (editor grid feel)
            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            using (var smoothScope = new BackgroundPainterHelpers.SmoothingScope(g, SmoothingMode.None))
            {
                var dotBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(20, 255, 255, 255));
                for (int y = bounds.Top; y < bounds.Bottom; y += 40)
                {
                    for (int x = bounds.Left; x < bounds.Right; x += 40)
                    {
                        g.FillRectangle(dotBrush, x, y, 1, 1);
                    }
                }
            }
        }
    }
}
