using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Fluent2 border painter - accent bar on left when focused + border (sizes from StyleBorders)
    /// Fluent2 UX: Accent bars indicate interaction intensity
    /// </summary>
    public static class Fluent2BorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get base colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(96, 94, 92));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(0, 120, 212));
            Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 120, 212));
            
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);
            int accentBarWidth = StyleBorders.GetAccentBarWidth(style);
            bool showAccentBar = false;
            int accentAlpha = 180;

            // Fluent2 UX: State-based border and accent bar changes
            switch (state)
            {
                case ControlState.Hovered:
                    // Fluent2: Hover shows subtle accent bar + primary border hint
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.3f);
                    showAccentBar = true;
                    accentAlpha = 180; // Semi-transparent accent bar
                    break;
                    
                case ControlState.Pressed:
                    // Fluent2: Pressed shows darker primary border + thicker accent bar
                    borderColor = BorderPainterHelpers.Darken(primaryColor, 0.15f);
                    showAccentBar = true;
                    accentBarWidth = (int)(accentBarWidth * 1.5f); // 1.5x thicker accent bar
                    accentAlpha = 255; // Full opacity
                    break;
                    
                case ControlState.Selected:
                    // Fluent2: Selected shows full primary border + full accent bar
                    borderColor = primaryColor;
                    showAccentBar = true;
                    accentAlpha = 255; // Full opacity
                    break;
                    
                case ControlState.Disabled:
                    // Fluent2: Disabled state is subtle with no accent bar
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 80);
                    showAccentBar = false;
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                borderColor = primaryColor;
                showAccentBar = true;
                accentAlpha = 255; // Full opacity on focus
            }

            // Paint border
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // Paint accent bar if needed
            if (showAccentBar)
            {
                var bounds = path.GetBounds();
                Color finalAccentColor = BorderPainterHelpers.WithAlpha(accentColor, accentAlpha);
                BorderPainterHelpers.PaintAccentBar(g, Rectangle.Round(bounds), finalAccentColor, accentBarWidth);
            }
        }
    }
}

