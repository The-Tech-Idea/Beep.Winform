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
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(229, 231, 235));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", theme?.SubLabelForColor ?? Color.FromArgb(75, 85, 99));
            Color borderColor = baseBorderColor;
            bool showRing = false;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 40);
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 70);
                    break;
                case ControlState.Selected:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 140);
                    showRing = true;
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 45);
                    break;
            }

            if (isFocused)
            {
                showRing = true;
            }

            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (showRing)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 50);
                float ringWidth = StyleBorders.GetRingWidth(style);
                float ringOffset = StyleBorders.GetRingOffset(style);
                if (ringWidth > 0)
                {
                    BorderPainterHelpers.PaintRing(g, path, focusRing, ringWidth, ringOffset);
                }
            }

            // Return the area inside the border
                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
    }
}
             
             
