using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Gradient modern path painter with glassmorphism
    /// Uses vibrant gradient fills from indigo to purple
    /// </summary>
    public static class GradientModernPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            Color color1 = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(99, 102, 241));
            Color color2 = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Secondary", Color.FromArgb(139, 92, 246));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Use multi-stop gradient for richer gradient (indigo to purple with intermediate stops)
                var stops = new[]
                {
                    (0.0f, color1),
                    (0.3f, Color.FromArgb(119, 97, 243)), // Intermediate stop
                    (0.7f, Color.FromArgb(129, 94, 245)), // Intermediate stop
                    (1.0f, color2)
                };

                // Apply state adjustments to stops
                var stateStops = stops.Select(s => (
                    s.Item1,
                    PathPainterHelpers.ApplyState(s.Item2, state)
                )).ToArray();

                BackgroundPainterHelpers.PaintMultiStopGradientBackground(g, path, stateStops, LinearGradientMode.Vertical, state, BackgroundPainterHelpers.StateIntensity.Normal);
            }
        }
    }
}

