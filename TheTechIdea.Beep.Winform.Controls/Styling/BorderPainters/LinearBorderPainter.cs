using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Linear style border painter - Ultra-clean, minimal borders
    /// </summary>
    public static class LinearBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Linear style: very subtle borders, almost invisible
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border",
                Color.FromArgb(235, 235, 245)); // Linear subtle border

            // NOTE: Previously 0.5f but GDI+ PaintSimpleBorder enforces Math.Max(1f, borderWidth),
            // so 0.5f was always rendered as 1px anyway. Using 1f explicitly avoids the mismatch
            // between intended width and actual rendered width.
            float borderWidth = 1.5f;

            if (isFocused)
            {
                borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary",
                    Color.FromArgb(99, 102, 241)); // Linear indigo
                borderWidth = 2f;
            }
            else if (state == ControlState.Hovered)
            {
                borderWidth = 1.8f;
                borderColor = BorderPainterHelpers.WithAlpha(borderColor, Math.Max((int)borderColor.A, 200));
            }

            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}
