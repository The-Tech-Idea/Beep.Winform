using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Arc Linux background painter - Arc GTK theme
    /// Dark gray background with green accent glow
    /// Modern, flat dark theme aesthetic
    /// </summary>
    public static class ArcLinuxBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Arc: dark charcoal gray
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0x2F, 0x2E, 0x2E);
            Color accent = useThemeColors && theme != null 
                ? theme.AccentColor 
                : Color.FromArgb(0x8A, 0xCB, 0x49); // Arc green

            // Solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                // Arc signature: green accent glow from top
                float glowHeight = bounds.Height / 5f;
                if (glowHeight > 2)
                {
                    var topRect = new RectangleF(bounds.Left, bounds.Top, bounds.Width, glowHeight);
                    var grad = PaintersFactory.GetLinearGradientBrush(
                        topRect,
                        Color.FromArgb(25, accent),
                        Color.Transparent,
                        LinearGradientMode.Vertical);
                    g.FillRectangle(grad, topRect);
                }
            }
        }
    }
}
