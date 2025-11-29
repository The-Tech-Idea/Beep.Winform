using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;


namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Minimal border painter - Simple 1px border, always visible
    /// Minimal UX: Very subtle state changes, focus on clarity over visual flourish
    /// </summary>
    public static class MinimalBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(224, 224, 224));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(64, 64, 64));
            Color borderColor = baseBorderColor;
            bool showRing = false;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 60);
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 80);
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
            if (borderWidth <= 0f)
            {
                // no border configured for this style
                return path;
            }
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (showRing)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 80);
                BorderPainterHelpers.PaintRing(g, path, focusRing, StyleBorders.GetRingWidth(style), StyleBorders.GetRingOffset(style));
            }

            // Return the area inside the border using shape-aware inset by half width
            return path.CreateInsetPath(borderWidth / 2f);
        }
    }
}
