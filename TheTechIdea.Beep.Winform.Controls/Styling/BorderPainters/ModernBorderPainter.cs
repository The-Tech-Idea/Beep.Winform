using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Modern border painter - clean anti-aliased outline.
    /// </summary>
    public static class ModernBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (path == null || path.PointCount == 0)
            {
                throw new ArgumentException("GraphicsPath is invalid or empty.");
            }

            if (g == null)
            {
                throw new ArgumentException("Graphics object is null.");
            }

            // Ensure border width is at least 2
            float width = Math.Max(2f, StyleBorders.GetBorderWidth(style));
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(210, 210, 220));

            if (isFocused)
            {
                borderColor = BorderPainterHelpers.Lighten(borderColor, 0.2f);
            }

            var pen = PaintersFactory.GetPen(borderColor, width);

            pen.LineJoin = LineJoin.Round; // smooth corners for wider borders

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawPath(pen, path);

            // Return inner content area inset by half the border width
            return path.CreateInsetPath(width / 2f);
        }
    }
}
