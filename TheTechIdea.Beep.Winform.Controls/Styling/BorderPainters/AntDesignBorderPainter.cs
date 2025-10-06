using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// AntDesign border painter - Ant blue 1px border
    /// Ant Design UX: Spec-compliant 2px focus border, subtle hover states
    /// </summary>
    public static class AntDesignBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get Ant Design colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(217, 217, 217));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(24, 144, 255));
            
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style); // 1px default

            // Ant Design UX: Spec-compliant state behaviors
            switch (state)
            {
                case ControlState.Hovered:
                    // Ant Design: Subtle primary tint (60 alpha as per spec)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 60);
                    break;
                    
                case ControlState.Pressed:
                    // Ant Design: Darker primary (pressed state)
                    borderColor = BorderPainterHelpers.Darken(primaryColor, 0.1f);
                    borderWidth = 2.0f; // 2px on press (Ant Design spec)
                    break;
                    
                case ControlState.Selected:
                    // Ant Design: Full primary border
                    borderColor = primaryColor;
                    borderWidth = 2.0f; // 2px on selection
                    break;
                    
                case ControlState.Disabled:
                    // Ant Design: Very light disabled
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 50);
                    break;
            }

            // Focus overrides state (Ant Design spec: 2px focus border)
            if (isFocused)
            {
                borderColor = primaryColor;
                borderWidth = 2.0f; // Ant Design spec: 2px focus border
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
        }
    }
}
