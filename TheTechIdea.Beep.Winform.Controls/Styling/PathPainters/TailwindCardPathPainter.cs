using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Tailwind card path painter.
    /// Uses a light surface with a blue accent border to resemble Tailwind UI cards.
    /// </summary>
    public static class TailwindCardPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            // Light card surface (slate-50 / gray-50 like)
            Color surface = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Surface",
                Color.FromArgb(248, 250, 252));

            // Tailwind blue-500 as accent
            Color accent = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Primary",
                Color.FromArgb(59, 130, 246));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Fill with light surface
                PathPainterHelpers.PaintSolidPath(g, path, surface, state);

                // Subtle border using accent color, stronger on hover/focus
                // Use HSL for more natural color manipulation
                Color borderColor = state switch
                {
                    ControlState.Hovered => ColorAccessibilityHelper.LightenColor(accent, 0.1f),
                    ControlState.Focused => accent,
                    ControlState.Selected => accent,
                    _ => PathPainterHelpers.WithAlpha(accent, 160)
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

