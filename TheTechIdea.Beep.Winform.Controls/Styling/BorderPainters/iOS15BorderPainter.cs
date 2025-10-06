using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// iOS15 border painter - Subtle border with system colors (width from StyleBorders)
    /// iOS UX: Very subtle, refined tints without dramatic changes
    /// </summary>
    public static class iOS15BorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get iOS system colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(209, 209, 214));
            Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 122, 255));
            
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);

            // iOS UX: Very subtle, refined state tints
            switch (state)
            {
                case ControlState.Hovered:
                    // iOS: Very subtle tint (15% blend for refinement)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, accentColor, 0.15f);
                    break;
                    
                case ControlState.Pressed:
                    // iOS: Slightly stronger tint (30% blend, still subtle)
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, accentColor, 0.30f);
                    break;
                    
                case ControlState.Selected:
                    // iOS: Full accent color with subtle transparency
                    borderColor = BorderPainterHelpers.WithAlpha(accentColor, 180);
                    break;
                    
                case ControlState.Disabled:
                    // iOS: Lighter, more transparent disabled
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 60);
                    break;
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // iOS15: Add focus ring effect (subtle blue ring around focused elements)
            if (isFocused)
            {
                Color translucentRing = BorderPainterHelpers.WithAlpha(accentColor, 40); // Very subtle
                float ringWidth = StyleBorders.GetRingWidth(style);
                float ringOffset = StyleBorders.GetRingOffset(style);
                if (ringWidth > 0)
                {
                    BorderPainterHelpers.PaintRing(g, path, translucentRing, ringWidth, ringOffset);
                }
            }
        }
    }
}

