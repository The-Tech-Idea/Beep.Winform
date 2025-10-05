using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Fluent2 border painter - 4px accent bar on left when focused + 1px border
    /// </summary>
    public static class Fluent2BorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            BorderPainterHelpers.ControlState state = BorderPainterHelpers.ControlState.Normal)
        {
            // Paint border first
            Color borderColor = isFocused
                ? BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Primary", Color.FromArgb(0, 120, 212))
                : BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(96, 94, 92));

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, 1f, state);

            // Paint accent bar on focus
            if (isFocused)
            {
                var bounds = path.GetBounds();
                Color accentColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "AccentColor", Color.FromArgb(0, 120, 212));
                BorderPainterHelpers.PaintAccentBar(g, Rectangle.Round(bounds), accentColor, 4);
            }
        }
    }
}

