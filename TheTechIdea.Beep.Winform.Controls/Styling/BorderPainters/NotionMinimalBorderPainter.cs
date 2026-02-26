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
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(227, 226, 224));
            Color primaryColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(55, 53, 47));
            Color borderColor = baseBorderColor;
            bool showRing = false;

            switch (state)
            {
                case ControlState.Hovered:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.05f);
                    break;
                case ControlState.Pressed:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.15f);
                    break;
                case ControlState.Selected:
                    borderColor = BorderPainterHelpers.BlendColors(baseBorderColor, primaryColor, 0.30f);
                    showRing = true;
                    break;
                case ControlState.Disabled:
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 30);
                    break;
            }

            if (isFocused)
            {
                showRing = true;
            }

            borderColor = BorderPainterHelpers.EnsureVisibleBorderColor(borderColor, theme, state);
            float borderWidth = StyleBorders.GetBorderWidth(style);
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (showRing)
            {
                Color subtleRing = BorderPainterHelpers.WithAlpha(primaryColor, 20);
                float ringWidth = StyleBorders.GetRingWidth(style);
                float ringOffset = StyleBorders.GetRingOffset(style);
                if (ringWidth > 0)
                {
                    BorderPainterHelpers.PaintRing(g, path, subtleRing, ringWidth, ringOffset);
                }
            }

            // Return the area inside the border
                // Return the area inside the border using shape-aware inset
                return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}

