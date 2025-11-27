using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// iOS 15 clean minimal path painter.
    /// Uses system accent blue with a soft translucent gradient to mimic iOS filled controls.
    /// </summary>
    public static class iOS15PathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            // iOS accent blue
            Color accent = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Primary",
                Color.FromArgb(0, 122, 255));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Translucent, slightly lighter towards the top
                Color top = PathPainterHelpers.WithAlpha(
                    PathPainterHelpers.Lighten(accent, 0.15f),
                    180);
                Color bottom = PathPainterHelpers.WithAlpha(
                    PathPainterHelpers.Darken(accent, 0.05f),
                    200);

                PathPainterHelpers.PaintGradientPath(g, path, top, bottom, LinearGradientMode.Vertical, state);
            }
        }
    }
}

