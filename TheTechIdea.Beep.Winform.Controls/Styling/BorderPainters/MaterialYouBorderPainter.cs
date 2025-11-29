using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// MaterialYou border painter - Dynamic color system with 1px border
    /// Filled variants have no border
    /// Material You UX: Adaptive, dynamic colors that respond to user interaction
    /// </summary>
    public static class MaterialYouBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(121, 116, 126));
            Color dynamicPrimary = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(103, 80, 164));
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.WithAlpha(dynamicPrimary, 70);
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.WithAlpha(dynamicPrimary, 120);
                    borderWidth *= 1.3f;
                    break;
                case ControlState.Selected:
                    borderColor = dynamicPrimary;
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 50);
                    break;
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (isFocused)
            {
                Color focusAccent = BorderPainterHelpers.WithAlpha(dynamicPrimary, 90);
                BorderPainterHelpers.PaintRing(g, path, focusAccent, StyleBorders.GetRingWidth(style), StyleBorders.GetRingOffset(style));
            }

            // Return the area inside the border
            // Return the area inside the border using shape-aware inset
            return path.CreateInsetPath(borderWidth);
        }
    }
}

