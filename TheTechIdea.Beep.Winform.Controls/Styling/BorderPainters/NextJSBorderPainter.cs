using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Next.js App Router border painter - Contemporary web borders
    /// </summary>
    public static class NextJSBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border",
                Color.FromArgb(229, 231, 235)); // Next.js gray border
            
            float borderWidth = 1f;

            if (isFocused)
            {
                borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary",
                    Color.FromArgb(37, 99, 235)); // Next.js blue
                borderWidth = 2f;
            }
            else if (state == ControlState.Normal)
            {
                // Idle parity baseline for web-style controls.
                borderWidth = 1.2f;
                borderColor = BorderPainterHelpers.WithAlpha(borderColor, Math.Max((int)borderColor.A, 180));
            }

            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}
