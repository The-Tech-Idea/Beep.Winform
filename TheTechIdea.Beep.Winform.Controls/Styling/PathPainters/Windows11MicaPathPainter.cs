using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Windows 11 Mica path painter.
    /// Uses subtle vertical gradients to hint at mica material surfaces.
    /// </summary>
    public static class Windows11MicaPathPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, int radius, BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            // Prefer surface/background for mica, fall back to accent blue
            Color surface = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Surface",
                Color.FromArgb(32, 32, 32));
            Color accent = PathPainterHelpers.GetColorFromStyleOrTheme(
                theme,
                useThemeColors,
                "Primary",
                Color.FromArgb(0, 120, 212));

            using (var path = PathPainterHelpers.CreateRoundedRectangle(bounds, radius))
            {
                // Subtle mica-like gradient: slightly tinted by accent, lighter at top, darker at bottom
                Color top = PathPainterHelpers.Lighten(Color.FromArgb(220, surface), 0.06f);
                Color bottom = PathPainterHelpers.Darken(Color.FromArgb(220, surface), 0.06f);

                // Light accent tint
                top = Color.FromArgb(top.A, 
                    (top.R + accent.R) / 2,
                    (top.G + accent.G) / 2,
                    (top.B + accent.B) / 2);

                PathPainterHelpers.PaintGradientPath(g, path, top, bottom, LinearGradientMode.Vertical, state);
            }
        }
    }
}

