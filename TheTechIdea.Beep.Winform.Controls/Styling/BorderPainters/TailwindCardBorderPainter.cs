using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// TailwindCard border painter - 1px border + ring effect on focus
    /// </summary>
    public static class TailwindCardBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            BorderPainterHelpers.ControlState state = BorderPainterHelpers.ControlState.Normal)
        {
            // Paint border first
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(229, 231, 235));
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, 1f, state);

            // Paint ring effect on focus
            if (isFocused)
            {
                Color ringColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(59, 130, 246));
                Color translucentRing = BorderPainterHelpers.WithAlpha(ringColor, 60);
                BorderPainterHelpers.PaintRing(g, path, translucentRing, 3f, 2f);
            }
        }
    }
}

