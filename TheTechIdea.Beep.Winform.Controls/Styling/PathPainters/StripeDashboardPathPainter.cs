using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Stripe dashboard path painter.
    /// Uses a neutral surface with a Stripe purple accent border, matching dashboard cards.
    /// </summary>
    public static class StripeDashboardPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            // Neutral card surface
            Color surface = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Surface",
                Color.FromArgb(249, 250, 252));

            // Stripe purple accent
            Color accent = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Primary",
                Color.FromArgb(99, 91, 255));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Fill with neutral surface; slightly darker when pressed
                Color fill = state == ControlState.Pressed
                    ? PathPainterHelpers.Darken(surface, 0.06f)
                    : surface;

                PathPainterHelpers.PaintSolidPath(g, path, fill, state);

                // Accent border for emphasis
                Color borderColor = state switch
                {
                    ControlState.Focused => accent,
                    ControlState.Selected => accent,
                    ControlState.Hovered => PathPainterHelpers.Lighten(accent, 0.08f),
                    _ => PathPainterHelpers.WithAlpha(accent, 170)
                };

                using (var pen = new Pen(borderColor, 1.5f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawPath(pen, path);
                }
            }
        }
    }
}

