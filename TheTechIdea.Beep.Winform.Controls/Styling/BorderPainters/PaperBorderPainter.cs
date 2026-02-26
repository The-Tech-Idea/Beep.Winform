using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Paper border painter - light card outline for Material paper surfaces.
    /// </summary>
    public static class PaperBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float width = StyleBorders.GetBorderWidth(style);
            if (width <= 0f) return path;
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(220, 220, 220));
            float effectiveWidth = width;

            if (!isFocused && state == ControlState.Normal)
            {
                // Idle parity baseline: keep paper subtle but clearly readable.
                effectiveWidth = Math.Max(effectiveWidth, 1.15f);
                borderColor = BorderPainterHelpers.WithAlpha(borderColor, Math.Max((int)borderColor.A, 170));
            }

            if (isFocused)
            {
                borderColor = BorderPainterHelpers.Lighten(borderColor, 0.1f);
            }
            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, effectiveWidth, state);
            return BorderPainterHelpers.CreateStrokeInsetPath(path, effectiveWidth);
        }
    }
}
