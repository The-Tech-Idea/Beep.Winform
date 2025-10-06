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
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get Minimal colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(224, 224, 224));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(64, 64, 64));
            
            Color borderColor = baseBorderColor;
            bool showRing = false;

            // Minimal UX: Very subtle state changes, focus on clarity
            switch (state)
            {
                case ControlState.Hovered:
                    // Minimal: Better contrast on hover (60 alpha for subtle feedback)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 60);
                    break;
                    
                case ControlState.Pressed:
                    // Minimal: Darker border (80 alpha for clear press state)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 80);
                    break;
                    
                case ControlState.Selected:
                    // Minimal: Full primary color for clear selection
                    borderColor = primaryColor;
                    showRing = true;
                    break;
                    
                case ControlState.Disabled:
                    // Minimal: Very subtle disabled (40 alpha for minimalism)
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 40);
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                showRing = true;
            }

            // Paint border
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, StyleBorders.GetBorderWidth(style), state);

            // Minimal: Add focus ring for better accessibility
            if (showRing)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 80); // Better contrast focus ring
                BorderPainterHelpers.PaintRing(g, path, focusRing, StyleBorders.GetRingWidth(style), StyleBorders.GetRingOffset(style));
            }
        }
    }
}
