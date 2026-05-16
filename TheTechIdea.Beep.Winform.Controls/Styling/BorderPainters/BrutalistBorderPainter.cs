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
            outerColor = BorderPainterHelpers.EnsureVisibleBorderColor(outerColor, theme, state);

            var oldMode = g.SmoothingMode;
            var oldPixel = g.PixelOffsetMode;
            g.SmoothingMode = SmoothingMode.None;
            g.PixelOffsetMode = PixelOffsetMode.None;

            // PenAlignment.Inset is unreliable for GraphicsPath in GDI+.
            // Instead, manually inset the path by half the stroke width and
            // draw with Center alignment so the full stroke sits inside the boundary.
            int detectedRadius = (int)GraphicsExtensions.DetectRadiusFromRoundedRectPath(path);
            float halfStroke = outerWidth / 2f;
            using (var insetPath = path.CreateInsetPath(halfStroke, detectedRadius))
            {
                var drawTarget = (insetPath != null && insetPath.PointCount > 2) ? insetPath : path;
                using (var pen = new Pen(outerColor, outerWidth))
                {
                    pen.LineJoin = LineJoin.Miter; // Sharp corners for Brutalist style
                    pen.Alignment = PenAlignment.Center;
                    g.DrawPath(pen, drawTarget);
                }
            }

            g.SmoothingMode = oldMode;
            g.PixelOffsetMode = oldPixel;

            // Return inset path accounting for the thick border
            return BorderPainterHelpers.CreateStrokeInsetPath(path, outerWidth);
        }
    }
}
