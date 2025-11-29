using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Brutalist border painter - thick, bold black outline for high contrast geometric look.
    /// </summary>
    public static class BrutalistBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Brutalist: Respect configured border width for style.
            float configuredWidth = StyleBorders.GetBorderWidth(style);
            if (configuredWidth <= 0f)
            {
                // No border configured for this style - return unchanged path
                return path;
            }
            // Use configured width for Brutalist, allow focus to increase it
            float outerWidth = configuredWidth;
            Color outerColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.Black);

            if (isFocused || state == ControlState.Selected)
            {
                // Make border even thicker when focused
                outerWidth = Math.Max(outerWidth, 4f);
                outerColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Accent", Color.FromArgb(40, 40, 40));
            }

            // Brutalist: Sharp edges, no anti-aliasing for crisp geometric look
            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            // Create NEW pen (not cached) so we can modify Alignment property
            using (var pen = new Pen(outerColor, outerWidth))
            {
                pen.Alignment = PenAlignment.Inset;
                g.DrawPath(pen, path);
            }

            g.SmoothingMode = oldMode;

            // Return inset path accounting for the thick border
            return path.CreateInsetPath(outerWidth);
        }
    }
}
