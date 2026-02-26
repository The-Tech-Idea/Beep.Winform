using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Holographic background painter - rainbow iridescent effect
    /// Dark purple base with multi-color gradient overlay and shine line
    /// </summary>
    public static class HolographicBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Holographic: dark purple base
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(24, 17, 47);

            // Solid background with subtle state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Rainbow iridescent gradient overlay
            var gradient = PaintersFactory.GetLinearGradientBrush(
                bounds, Color.Magenta, Color.Cyan, LinearGradientMode.Horizontal);
            
            try
            {
                gradient.InterpolationColors = new ColorBlend
                {
                    Colors = new[]
                    {
                        Color.FromArgb(35, 255, 0, 150),   // Pink
                        Color.FromArgb(35, 255, 200, 0),   // Yellow
                        Color.FromArgb(35, 0, 255, 100),   // Green
                        Color.FromArgb(35, 100, 150, 255), // Blue
                        Color.FromArgb(35, 255, 0, 200)    // Magenta
                    },
                    Positions = new[] { 0f, 0.25f, 0.5f, 0.75f, 1f }
                };
            }
            catch { /* Fallback to linear gradient */ }
            
            g.FillPath(gradient, path);

            using (var clip = new BackgroundPainterHelpers.ClipScope(g, path))
            {
                // Holographic shine line
                var shinePen = PaintersFactory.GetPen(Color.FromArgb(35, Color.White), 2f);
                g.DrawLine(shinePen,
                    bounds.Left, bounds.Top + bounds.Height / 3f,
                    bounds.Right, bounds.Top);
            }
        }
    }
}
