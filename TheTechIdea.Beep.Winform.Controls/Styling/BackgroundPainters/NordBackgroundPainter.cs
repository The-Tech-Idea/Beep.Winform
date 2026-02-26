using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Nord theme background painter - arctic, north-bluish color palette
    /// Clean polar aesthetic with subtle frost glow
    /// </summary>
    public static class NordBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Nord polar night base
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0x2E, 0x34, 0x40); // nord0
            
            // Nord frost accent
            Color frost = Color.FromArgb(0x88, 0xC0, 0xD0); // nord8

            // Paint solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                // Frost glow from top (aurora effect)
                var topRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, bounds.Height / 3f);
                var frostBrush = PaintersFactory.GetLinearGradientBrush(
                    topRect,
                    Color.FromArgb(10, frost),
                    Color.FromArgb(0, frost),
                    LinearGradientMode.Vertical);
                g.FillRectangle(frostBrush, topRect);

                // Subtle frost accent line at top
                var pen = PaintersFactory.GetPen(Color.FromArgb(30, frost), 1f);
                g.DrawLine(pen, bounds.Left, bounds.Top + 0.5f, bounds.Right, bounds.Top + 0.5f);
            }
        }
    }
}
