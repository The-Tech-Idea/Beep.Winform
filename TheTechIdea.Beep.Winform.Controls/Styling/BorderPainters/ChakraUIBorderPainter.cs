using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// ChakraUI border painter - Chakra teal 1px border
    /// Chakra UI UX: Distinctive teal states with smooth transitions
    /// </summary>
    public static class ChakraUIBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get Chakra UI colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(226, 232, 240));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(49, 151, 149));
            
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);
            bool showRing = false;

            // Chakra UI UX: Distinctive teal states with smooth transitions
            switch (state)
            {
                case ControlState.Hovered:
                    // Chakra: Subtle teal tint
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.25f);
                    showRing = true; // Preview ring on hover
                    break;
                    
                case ControlState.Pressed:
                    // Chakra: Full teal with slight darkening
                    borderColor = BorderPainterHelpers.Darken(primaryColor, 0.1f);
                    showRing = true;
                    break;
                    
                case ControlState.Selected:
                    // Chakra: Full teal border + ring
                    borderColor = primaryColor;
                    showRing = true;
                    break;
                    
                case ControlState.Disabled:
                    // Chakra: Light disabled
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 55);
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                showRing = true;
            }

            // Paint border
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // Chakra UI: Add focus ring (distinctive teal ring)
            if (showRing)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 60); // Chakra focus ring opacity
                float ringWidth = StyleBorders.GetRingWidth(style); // 2.0f for Chakra
                float ringOffset = StyleBorders.GetRingOffset(style); // 1.5f for Chakra
                BorderPainterHelpers.PaintRing(g, path, focusRing, ringWidth, ringOffset);
            }

                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
    }
}
