using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Brutalist border painter - thick slab outline with inner accent line.
    /// </summary>
    public static class BrutalistBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float outerWidth = StyleBorders.GetBorderWidth(style);
            Color outerColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(40, 40, 40));
            Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Accent", Color.FromArgb(80, 80, 80));

            if (isFocused || state == ControlState.Selected)
            {
                accentColor = BorderPainterHelpers.Lighten(accentColor, 0.2f);
            }

            // Brutalist: no anti aliasing for crisp slab edges.
            var oldMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;
            var pen = PaintersFactory.GetPen(outerColor, outerWidth);
            pen.Alignment = PenAlignment.Inset;
            g.DrawPath(pen, path);
            g.SmoothingMode = oldMode;

            using (var innerPath = path.CreateInsetPath(outerWidth + 3f))
            {
                BorderPainterHelpers.PaintSimpleBorder(g, innerPath, accentColor, 2f, state);
            }

            return path.CreateInsetPath(outerWidth + 5f);
        }
    }
}
