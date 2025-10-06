using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for Material Design styles (Material3, MaterialYou)
    /// Original Material Design UX: Bold focus thickening for accessibility
    /// </summary>
    public static class MaterialBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Material UX: Focus border thickening for accessibility
            if (StyleBorders.IsFilled(style))
                return;

            Color borderColor = GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            Color primaryColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            float borderWidth = StyleBorders.GetBorderWidth(style);

            // Original Material Design UX: Bold, clear state transitions
            switch (state)
            {
                case ControlState.Hovered:
                    // Material: Hover tint with transparency
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 70);
                    break;
                    
                case ControlState.Pressed:
                    // Material: Bold press state with dramatic thickening
                    borderColor = primaryColor;
                    borderWidth = Math.Max(borderWidth * 2.5f, 3.0f); // 2.5x or minimum 3px
                    break;
                    
                case ControlState.Selected:
                    // Material: Full primary color with standard thickening
                    borderColor = primaryColor;
                    borderWidth = Math.Max(borderWidth * 1.5f, 2.0f); // 1.5x or minimum 2px
                    break;
                    
                case ControlState.Disabled:
                    // Material: Low-contrast disabled state
                    borderColor = BorderPainterHelpers.WithAlpha(borderColor, 40);
                    borderWidth *= 0.8f; // Slightly thinner for disabled
                    break;
            }

            // Material: Focus border thickening (Material Design guideline) - overrides state
            if (isFocused)
            {
                borderWidth = Math.Max(borderWidth * 2.0f, 2.0f); // At least 2px on focus
                borderColor = primaryColor; // Full focus color
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
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
