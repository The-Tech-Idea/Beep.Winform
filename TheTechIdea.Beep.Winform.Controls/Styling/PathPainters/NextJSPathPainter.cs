using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Next.js App Router path painter - Modern gradient fills
    /// </summary>
    public static class NextJSPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style,
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            Color baseColor = PathPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary",
                Color.FromArgb(37, 99, 235)); // Next.js blue

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Next.js uses subtle gradients - use HSL for more natural results
                Color lighter = ColorAccessibilityHelper.LightenColor(baseColor, 0.1f);
                PathPainterHelpers.PaintGradientPath(g, path, lighter, baseColor, LinearGradientMode.Vertical, state);
            }
        }
    }
}
