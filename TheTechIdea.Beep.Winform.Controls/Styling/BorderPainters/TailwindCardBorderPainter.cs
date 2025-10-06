using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// TailwindCard border painter - 1px border + ring effect on focus
    /// Tailwind UX: Prominent rings with utility-first state behaviors
    /// </summary>
    public static class TailwindCardBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get Tailwind colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(229, 231, 235));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(59, 130, 246));
            
            Color borderColor = baseBorderColor;
            bool showRing = false;
            int ringAlpha = 100;

            // Tailwind UX: Prominent rings with utility-first state behaviors
            switch (state)
            {
                case ControlState.Hovered:
                    // Tailwind: Subtle border darkening + ring preview
                    borderColor = BorderPainterHelpers.Darken(baseBorderColor, 0.1f);
                    showRing = true;
                    ringAlpha = 60; // Subtle ring on hover
                    break;
                    
                case ControlState.Pressed:
                    // Tailwind: Darker border + prominent ring
                    borderColor = BorderPainterHelpers.Darken(baseBorderColor, 0.2f);
                    showRing = true;
                    ringAlpha = 140; // Extra prominent ring
                    break;
                    
                case ControlState.Selected:
                    // Tailwind: Primary border + full ring
                    borderColor = primaryColor;
                    showRing = true;
                    ringAlpha = 100;
                    break;
                    
                case ControlState.Disabled:
                    // Tailwind: Lighter disabled (gray-200)
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 70);
                    showRing = false;
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                showRing = true;
                ringAlpha = 100; // Prominent focus ring
            }

            // Paint border
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, StyleBorders.GetBorderWidth(style), state);

            // Paint ring effect (Tailwind UX: more prominent focus ring)
            if (showRing)
            {
                Color translucentRing = BorderPainterHelpers.WithAlpha(primaryColor, ringAlpha);
                float ringWidth = StyleBorders.GetRingWidth(style); // 3.0f for Tailwind
                float ringOffset = StyleBorders.GetRingOffset(style); // 2.0f for Tailwind
                BorderPainterHelpers.PaintRing(g, path, translucentRing, ringWidth, ringOffset);
            }
        }
    }
}

