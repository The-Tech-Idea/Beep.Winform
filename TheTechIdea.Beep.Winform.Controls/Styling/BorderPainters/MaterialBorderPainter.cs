using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for Material Design styles (Material3, MaterialYou)
    /// Original Material Design UX: Bold focus thickening for accessibility
    /// </summary>
    public static class MaterialBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (StyleBorders.IsFilled(style))
                return path;

            Color borderColor = GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            Color primaryColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            float borderWidth = StyleBorders.GetBorderWidth(style);

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 70);
                    break;
                case ControlState.Pressed:
                    borderColor = primaryColor;
                    borderWidth = Math.Max(borderWidth * 2.5f, 3.0f);
                    break;
                case ControlState.Selected:
                    borderColor = primaryColor;
                    borderWidth = Math.Max(borderWidth * 1.5f, 2.0f);
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(borderColor, 40);
                    borderWidth *= 0.8f;
                    break;
            }

            if (isFocused)
            {
                borderWidth = Math.Max(borderWidth * 2.0f, 2.0f);
                borderColor = primaryColor;
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // Return the area inside the border
                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
        
        private static Color GetColor(BeepControlStyle style, System.Func<BeepControlStyle, Color> styleColorFunc, string themeColorKey, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }
    }
}
