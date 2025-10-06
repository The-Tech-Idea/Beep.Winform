using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// FigmaCard border painter - Figma blue 1px border
    /// Figma UX: Subtle blue tints with prominent focus rings
    /// </summary>
    public static class FigmaCardBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get Figma colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(227, 227, 227));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(24, 160, 251));
            
            Color borderColor = baseBorderColor;
            bool showRing = false;

            // Figma UX: Subtle blue tints
            switch (state)
            {
                case ControlState.Hovered:
                    // Figma: Subtle hover tint (40 alpha)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 40);
                    showRing = true; // Preview ring
                    break;
                    
                case ControlState.Pressed:
                    // Figma: Stronger tint (70 alpha) + ring
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 70);
                    showRing = true;
                    break;
                    
                case ControlState.Selected:
                    // Figma: Full blue + prominent ring
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 140);
                    showRing = true;
                    break;
                    
                case ControlState.Disabled:
                    // Figma: Light disabled (45 alpha)
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 45);
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                showRing = true;
            }

            // Paint border
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, StyleBorders.GetBorderWidth(style), state);

            // Figma: Add focus ring (Figma blue)
            if (showRing)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 70); // Figma focus ring
                BorderPainterHelpers.PaintRing(g, path, focusRing, 2.0f, 1.0f);
            }
        }
    }
}

