using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// StripeDashboard border painter - Subtle 1px border
    /// Stripe UX: Prominent state changes with rings + accent bars
    /// </summary>
    public static class StripeDashboardBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get Stripe colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(225, 225, 225));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(99, 91, 255));
            Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(99, 91, 255));
            
            Color borderColor = baseBorderColor;
            bool showRing = false;
            bool showAccentBar = false;

            // Stripe UX: Prominent state changes
            switch (state)
            {
                case ControlState.Hovered:
                    // Stripe: Subtle primary tint + ring preview
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.2f);
                    showRing = true;
                    break;
                    
                case ControlState.Pressed:
                    // Stripe: Primary border + prominent ring + accent bar
                    borderColor = primaryColor;
                    showRing = true;
                    showAccentBar = true;
                    break;
                    
                case ControlState.Selected:
                    // Stripe: Primary border + ring + accent bar
                    borderColor = primaryColor;
                    showRing = true;
                    showAccentBar = true;
                    break;
                    
                case ControlState.Disabled:
                    // Stripe: Light disabled (50 alpha)
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 50);
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                showRing = true;
                showAccentBar = true;
            }

            // Paint border
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, StyleBorders.GetBorderWidth(style), state); // 1.5f for Stripe

            // Stripe: Add focus ring
            if (showRing)
            {
                Color translucentRing = BorderPainterHelpers.WithAlpha(primaryColor, 30); // Subtle Stripe ring
                BorderPainterHelpers.PaintRing(g, path, translucentRing, 2.0f, 1.0f);
            }
            
            // Stripe: Add accent bar
            if (showAccentBar)
            {
                var bounds = path.GetBounds();
                int accentBarWidth = StyleBorders.GetAccentBarWidth(style); // 4px for StripeDashboard
                if (accentBarWidth > 0)
                {
                    BorderPainterHelpers.PaintAccentBar(g, Rectangle.Round(bounds), accentColor, accentBarWidth);
                }
            }
        }
    }
}

