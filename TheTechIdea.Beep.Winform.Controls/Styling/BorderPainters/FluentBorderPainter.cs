using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Border painter for Fluent Design (Fluent2, Windows11Mica)
    /// Includes accent bar support
    /// Fluent UX: Accent bars + rings for layered interaction feedback
    /// </summary>
    public static class FluentBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get base colors
            Color baseBorderColor = GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            Color primaryColor = GetColor(style, StyleColors.GetPrimary, "Primary", theme, useThemeColors);
            
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);
            bool showAccentBar = false;
            int accentBarWidth = StyleBorders.GetAccentBarWidth(style);

            // Fluent UX: Layered state feedback with accent bars
            switch (state)
            {
                case ControlState.Hovered:
                    // Fluent: Hover shows subtle primary tint + accent bar
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 60);
                    showAccentBar = true;
                    break;
                    
                case ControlState.Pressed:
                    // Fluent: Pressed shows full primary border + prominent accent bar
                    borderColor = primaryColor;
                    showAccentBar = true;
                    accentBarWidth = (int)(accentBarWidth * 1.3f); // Slightly thicker
                    break;
                    
                case ControlState.Selected:
                    // Fluent: Selected shows full primary border + full accent bar
                    borderColor = primaryColor;
                    showAccentBar = true;
                    break;
                    
                case ControlState.Disabled:
                    // Fluent: Disabled is muted with no accent bar
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 70);
                    showAccentBar = false;
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                showAccentBar = true;
                // Keep state border color, add accent bar + ring
            }

            // Paint border if not filled
            if (!StyleBorders.IsFilled(style))
            {
                BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
            }

            // Fluent: Draw accent bar on interaction
            if (showAccentBar)
            {
                var bounds = path.GetBounds();
                BorderPainterHelpers.PaintAccentBar(g, Rectangle.Round(bounds), primaryColor, accentBarWidth);
            }

            // Fluent: Add focus rings for better accessibility
            if (isFocused)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 70); // Fluent focus ring
                BorderPainterHelpers.PaintRing(g, path, focusRing, StyleBorders.GetRingWidth(style), StyleBorders.GetRingOffset(style));
            }
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
