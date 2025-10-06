using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// MaterialYou border painter - Dynamic color system with 1px border
    /// Filled variants have no border
    /// Material You UX: Adaptive, dynamic colors that respond to user interaction
    /// </summary>
    public static class MaterialYouBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Material You: Get dynamic color palette
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(121, 116, 126));
            Color dynamicPrimary = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(103, 80, 164));
            
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);

            // Material You UX: Dynamic state-based color adaptation
            switch (state)
            {
                case ControlState.Hovered:
                    // Material You: Soft, adaptive hover with transparency
                    borderColor = BorderPainterHelpers.WithAlpha(dynamicPrimary, 70);
                    break;
                    
                case ControlState.Pressed:
                    // Material You: Vibrant press state with full saturation
                    borderColor = BorderPainterHelpers.WithAlpha(dynamicPrimary, 120);
                    borderWidth *= 1.3f; // Slight thickness increase
                    break;
                    
                case ControlState.Selected:
                    // Material You: Full dynamic primary color
                    borderColor = dynamicPrimary;
                    break;
                    
                case ControlState.Disabled:
                    // Material You: Very low contrast disabled (harmonious)
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 50);
                    break;
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // Material You: Dynamic focus ring with accent colors
            if (isFocused)
            {
                Color focusAccent = BorderPainterHelpers.WithAlpha(dynamicPrimary, 90);
                BorderPainterHelpers.PaintRing(g, path, focusAccent, StyleBorders.GetRingWidth(style), StyleBorders.GetRingOffset(style));
            }
        }
    }
}

