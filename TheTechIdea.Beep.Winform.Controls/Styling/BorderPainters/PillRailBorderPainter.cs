using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// PillRail border painter - Soft 1px border for pill-shaped controls
    /// Pill Rail UX: Soft, rounded minimalism with gentle state transitions
    /// </summary>
    public static class PillRailBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Get Pill Rail colors
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(229, 231, 235));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(107, 114, 128));

            Color borderColor = baseBorderColor;
            bool showRing = false;

            // Pill Rail UX: Soft, rounded minimalism with gentle transitions
            switch (state)
            {
                case ControlState.Hovered:
                    // Pill Rail: Very subtle hover tint (40 alpha for soft feedback)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 40);
                    break;

                case ControlState.Pressed:
                    // Pill Rail: Slightly stronger tint (70 alpha for soft press)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 70);
                    break;

                case ControlState.Selected:
                    // Pill Rail: Full primary with soft transparency (140 alpha)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 140);
                    showRing = true;
                    break;

                case ControlState.Disabled:
                    // Pill Rail: Soft disabled (45 alpha for gentle disabled)
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

            // Pill Rail: Add subtle focus ring
            if (showRing)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 50); // Subtle focus ring
                float ringWidth = StyleBorders.GetRingWidth(style);
                float ringOffset = StyleBorders.GetRingOffset(style);
                if (ringWidth > 0)
                {
                    BorderPainterHelpers.PaintRing(g, path, focusRing, ringWidth, ringOffset);
                }
            }
        }
    }
}
             
             
