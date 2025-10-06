using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Material3 border painter - Outlined style with border from StyleBorders
    /// Filled variants have no border
    /// Material3 UX: Bold, clear state changes with elevation-like border thickness
    /// </summary>
    public static class Material3BorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Material3: Get base colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(121, 116, 126));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(103, 80, 164));
            
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);

            // Material3 UX: State-based border modifications (bold, clear changes)
            switch (state)
            {
                case ControlState.Hovered:
                    // Material3: Subtle primary tint on hover
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.3f);
                    break;
                    
                case ControlState.Pressed:
                    // Material3: Darker primary blend when pressed (tactile feedback)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.6f);
                    borderWidth *= 1.5f; // Thicker for "elevation" feel
                    break;
                    
                case ControlState.Selected:
                    // Material3: Full primary color for selected state
                    borderColor = primaryColor;
                    borderWidth = Math.Max(borderWidth, 2.0f);
                    break;
                    
                case ControlState.Disabled:
                    // Material3: Low contrast disabled state
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 60); // 40% opacity
                    break;
            }

            // Material3: Focused state overrides (2px accent border)
            if (isFocused)
            {
                borderColor = primaryColor;
                borderWidth = 2.0f;
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);
        }
    }
}

