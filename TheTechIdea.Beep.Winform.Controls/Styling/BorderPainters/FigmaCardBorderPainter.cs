using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// FigmaCard border painter - Figma blue 1px border
    /// Figma UX: Subtle blue tints with prominent focus rings
    /// </summary>
    public static class FigmaCardBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(227, 227, 227));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(24, 160, 251));
            Color borderColor = baseBorderColor;
            bool showRing = false;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 40);
                    showRing = true;
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 70);
                    showRing = true;
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
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (showRing)
            {
                Color focusRing = BorderPainterHelpers.WithAlpha(primaryColor, 70);
                BorderPainterHelpers.PaintRing(g, path, focusRing, 2.0f, 1.0f);
            }

            // Return the area inside the border
                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
    }
}

