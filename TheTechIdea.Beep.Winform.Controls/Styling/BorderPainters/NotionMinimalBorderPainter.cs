using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// NotionMinimal border painter - Very subtle 1px border
    /// Notion UX: Extremely subtle state changes, zen-like minimalism
    /// </summary>
    public static class NotionMinimalBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get Notion colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(227, 226, 224));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(55, 53, 47));
            
            Color borderColor = baseBorderColor;
            bool showRing = false;

            // Notion UX: Extremely subtle state changes (zen-like minimalism)
            switch (state)
            {
                case ControlState.Hovered:
                    // Notion: Very subtle darkening (5% blend for zen-like hover)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.05f);
                    break;
                    
                case ControlState.Pressed:
                    // Notion: Subtle darkening (15% blend, still very subtle)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.15f);
                    break;
                    
                case ControlState.Selected:
                    // Notion: Notion gray border (30% blend for subtle selection)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.30f);
                    showRing = true;
                    break;
                    
                case ControlState.Disabled:
                    // Notion: Extremely light disabled (30 alpha for zen disabled)
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 30);
                    break;
            }

            // Focus overrides state
            if (isFocused)
            {
                showRing = true;
            }

            // Paint border
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, StyleBorders.GetBorderWidth(style), state);

            // Notion: Add very subtle focus ring
            if (showRing)
            {
                Color subtleRing = BorderPainterHelpers.WithAlpha(primaryColor, 20); // Very subtle Notion ring
                float ringWidth = StyleBorders.GetRingWidth(style);
                float ringOffset = StyleBorders.GetRingOffset(style);
                if (ringWidth > 0)
                {
                    BorderPainterHelpers.PaintRing(g, path, subtleRing, ringWidth, ringOffset);
                }
            }
        }
    }
}

