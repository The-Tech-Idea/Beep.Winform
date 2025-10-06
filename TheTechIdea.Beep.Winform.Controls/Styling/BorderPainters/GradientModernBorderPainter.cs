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
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get Gradient Modern colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(148, 163, 184));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(99, 102, 241));
            
            Color borderColor = baseBorderColor;
            bool showRing = false;

            // Gradient Modern UX: Subtle gradient tints
            switch (state)
            {
                case ControlState.Hovered:
                    // Gradient: Subtle hover tint (50 alpha)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 50);
                    showRing = true; // Preview ring
                    break;
                    
                case ControlState.Pressed:
                    // Gradient: Stronger tint (80 alpha) + ring
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 80);
                    showRing = true;
                    break;
                    
                case ControlState.Selected:
                    // Gradient: Full primary + ring
                    borderColor = primaryColor;
                    showRing = true;
                    break;
                    
                case ControlState.Disabled:
                    // Gradient: Subdued disabled (40 alpha)
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

            // Gradient Modern: Add focus ring
            if (showRing)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 60); // Subtle focus ring
                BorderPainterHelpers.PaintRing(g, path, focusRing, StyleBorders.GetRingWidth(style), StyleBorders.GetRingOffset(style));
            }
        }
    }
}

