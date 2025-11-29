using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// DiscordStyle border painter - Discord blurple 1px border
    /// </summary>
    public static class DiscordStyleBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(66, 70, 77));
            float borderWidth = StyleBorders.GetBorderWidth(style);
            if (borderWidth <= 0f) return path;

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            if (isFocused)
            {
                Color glowColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(88, 101, 242));
                Color focusGlow = BorderPainterHelpers.WithAlpha(glowColor, 30);
                BorderPainterHelpers.PaintGlowBorder(g, path, focusGlow, StyleBorders.GetGlowWidth(style));
            }

            // Return the area inside the border (shrink path by borderWidth)
                // Return the area inside the border using shape-aware inset
                return path.CreateInsetPath(borderWidth);
        }
    }
}
