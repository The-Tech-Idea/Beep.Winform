using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// TailwindCard border painter -1px border + ring effect on focus
    /// Tailwind UX: Prominent rings with utility-first state behaviors
    /// </summary>
    public static class TailwindCardBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(229,231,235));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(59,130,246));
            Color borderColor = baseBorderColor;
            bool showRing = false;
            int ringAlpha =100;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.Darken(baseBorderColor,0.1f);
                    showRing = true;
                    ringAlpha =60;
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.Darken(baseBorderColor,0.2f);
                    showRing = true;
                    ringAlpha =140;
                    break;
                case ControlState.Selected:
                    borderColor = primaryColor;
                    showRing = true;
                    ringAlpha =100;
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor,70);
                    showRing = false;
                    break;
            }

            if (isFocused)
            {
                showRing = true;
                ringAlpha =100;
            }

            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);
            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (showRing)
            {
                Color translucentRing = BorderPainterHelpers.WithAlpha(primaryColor, ringAlpha);
                float ringWidth = StyleBorders.GetRingWidth(style);
                float ringOffset = StyleBorders.GetRingOffset(style);
                BorderPainterHelpers.PaintRing(g, path, translucentRing, ringWidth, ringOffset);
            }

            // Return the area inside the border
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}

