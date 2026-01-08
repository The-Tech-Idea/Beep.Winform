using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Glass acrylic frosted glass path painter
    /// Uses semi-transparent fills with frosted glass effect
    /// </summary>
    public static class GlassAcrylicPathPainter
    {
        // Converted to use PathPainterHelpers and PaintersFactory for brushes
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            // Semi-transparent white fill for frosted glass effect
            Color fillColor = PathPainterHelpers.WithAlpha(255, 255, 255, 180);

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Glass acrylic benefits from radial gradients for better glass effect
                Color centerColor = PathPainterHelpers.WithAlpha(255, 255, 255, 200);
                Color edgeColor = PathPainterHelpers.WithAlpha(255, 255, 255, 160);
                
                // Use radial gradient to enhance glass effect (radial gradients enhance glass appearance)
                BackgroundPainterHelpers.PaintRadialGradientBackground(g, path, centerColor, edgeColor, state, BackgroundPainterHelpers.StateIntensity.Subtle);
            }
        }
    }
}

