using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Dracula border painter - purple glow silhouette with thin outline.
    /// </summary>
    public static class DraculaBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color primary = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(189, 147, 249));
            Color outline = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(120, 80, 120));

            BorderPainterHelpers.PaintGlowBorder(g, path, BorderPainterHelpers.WithAlpha(primary, 60), 10f);
            BorderPainterHelpers.PaintGlowBorder(g, path, BorderPainterHelpers.WithAlpha(primary, 90), 6f);

            if (isFocused || state == ControlState.Selected)
            {
                BorderPainterHelpers.PaintGlowBorder(g, path, BorderPainterHelpers.WithAlpha(primary, 140), 4f, 1.0f);
            }

            float bw = StyleBorders.GetBorderWidth(style);
            if (bw <= 0f) return path;
            BorderPainterHelpers.PaintSimpleBorder(g, path, outline, bw, state);
            return path.CreateInsetPath(bw + 2f);
        }
    }
}
