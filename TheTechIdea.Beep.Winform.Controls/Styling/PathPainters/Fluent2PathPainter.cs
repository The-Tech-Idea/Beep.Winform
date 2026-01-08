using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Microsoft Fluent 2 path painter
    /// Uses Fluent blue with modern fills
    /// </summary>
    public static class Fluent2PathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            Color fillColor = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(0, 120, 212));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Fluent 2 buttons have subtle radial gradients for depth
                Color centerColor = PathPainterHelpers.ApplyState(fillColor, state);
                Color edgeColor = PathPainterHelpers.Darken(centerColor, 0.1f);
                
                // Use radial gradient for Fluent 2 button appearance
                BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, state, BackgroundPainterHelpers.StateIntensity.Normal);
            }
        }
    }
}

