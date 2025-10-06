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
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state =    ControlState.Normal)
        {
            // Discord UX: No borders normally, subtle focus glow
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(66, 70, 77));

            // Discord: Very subtle border normally, no change on hover
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, StyleBorders.GetBorderWidth(style), state);

            // Discord: Add subtle focus glow (Discord blurple)
            if (isFocused)
            {
                Color glowColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(88, 101, 242));
                Color focusGlow = BorderPainterHelpers.WithAlpha(glowColor, 30); // Very subtle glow
                BorderPainterHelpers.PaintGlowBorder(g, path, focusGlow, StyleBorders.GetGlowWidth(style));
            }
        }
    }
}
