using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// GradientModern border painter - 1px border matching gradient theme
    /// Gradient Modern UX: Subtle gradient tints with rings
    /// </summary>
    public static class GradientModernBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(148, 163, 184));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(99, 102, 241));
            Color borderColor = baseBorderColor;
            bool showRing = false;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 50);
                    showRing = true;
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 80);
                    showRing = true;
                    break;
                case ControlState.Selected:
                    borderColor = primaryColor;
                    showRing = true;
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 40);
                    break;
            }

            if (isFocused)
            {
                showRing = true;
            }

            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (showRing)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 60);
                BorderPainterHelpers.PaintRing(g, path, focusRing, StyleBorders.GetRingWidth(style), StyleBorders.GetRingOffset(style));
            }

            // Return the area inside the border
                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
    }
}

