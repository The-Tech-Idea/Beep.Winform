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
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(209, 209, 214));
            Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 122, 255));
            Color borderColor = baseBorderColor;
            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, accentColor, 0.15f);
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, accentColor, 0.30f);
                    break;
                case ControlState.Selected:
                    borderColor = BorderPainterHelpers.WithAlpha(accentColor, 180);
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 60);
                    break;
            }

            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (isFocused)
            {
                Color translucentRing = BorderPainterHelpers.WithAlpha(accentColor, 40);
                float ringWidth = StyleBorders.GetRingWidth(style);
                float ringOffset = StyleBorders.GetRingOffset(style);
                if (ringWidth > 0)
                {
                    BorderPainterHelpers.PaintRing(g, path, translucentRing, ringWidth, ringOffset);
                }
            }

            // Return the area inside the border
                // Return the area inside the border using shape-aware inset
                return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}

