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

            // Respect configured border width - if zero do not draw any border
            float configuredWidth = StyleBorders.GetBorderWidth(style);
            if (configuredWidth <= 0f) return path;

            // Use the configured width (retain previous minimum if needed by style heuristics, but user should control this)
            float width = configuredWidth;
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(210, 210, 220));

            if (isFocused)
            {
                borderColor = BorderPainterHelpers.Lighten(borderColor, 0.2f);
            }
            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, width, state);

            // Return inner content area inset by half the border width
            return BorderPainterHelpers.CreateStrokeInsetPath(path, width);
        }
    }
}
