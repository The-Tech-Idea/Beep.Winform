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
            // Brutalist: Very thick, bold borders (3-4px) in solid black or dark color
            float outerWidth = Math.Max(StyleBorders.GetBorderWidth(style), 3f); // Minimum 3px for bold look
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

            var pen = PaintersFactory.GetPen(outerColor, outerWidth);
            pen.Alignment = PenAlignment.Inset;
            g.DrawPath(pen, path);

            g.SmoothingMode = oldMode;

            // Return inset path accounting for the thick border
            return path.CreateInsetPath(outerWidth);
        }
    }
}
