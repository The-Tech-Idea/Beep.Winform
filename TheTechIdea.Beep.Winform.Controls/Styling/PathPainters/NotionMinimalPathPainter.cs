using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Notion minimal path painter.
    /// Uses a very light surface and a subtle dark border, similar to Notion blocks.
    /// </summary>
    public static class NotionMinimalPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            // Light block surface
            Color surface = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Surface",
                Color.FromArgb(250, 249, 246));

            // Dark text/border tone
            Color ink = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Foreground",
                Color.FromArgb(55, 53, 47));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Slightly darker on hover, pressed - use HSL for more natural results
                Color fill = state switch
                {
                    ControlState.Hovered => ColorAccessibilityHelper.DarkenColor(surface, 0.02f),
                    ControlState.Pressed => ColorAccessibilityHelper.DarkenColor(surface, 0.05f),
                    _ => surface
                };

                PathPainterHelpers.PaintSolidPath(g, path, fill, state);

                // Very subtle border, stronger on focus/selection
                Color borderColor = state switch
                {
                    ControlState.Focused => ink,
                    ControlState.Selected => ink,
                    _ => PathPainterHelpers.WithAlpha(ink, 80)
                };

                using (var pen = new Pen(borderColor, 1f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawPath(pen, path);
                }
            }
        }
    }
}

