using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Modern border painter - clean anti-aliased outline.
    /// </summary>
    public static class ModernBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Modern style - clean, sophisticated borders with indigo accent
            float width = StyleBorders.GetBorderWidth(style);
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(200, 200, 200));  // #C8C8C8

            if (isFocused)
            {
                // Modern focus: use indigo accent color from theme
                Color activeBorderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "ActiveBorder", Color.FromArgb(99, 102, 241));  // Indigo
                borderColor = activeBorderColor;
                
                // Optional: add a subtle focus ring
                Color focusRingColor = BorderPainterHelpers.WithAlpha(activeBorderColor, 100);
                BorderPainterHelpers.PaintRing(g, path, focusRingColor, 2.0f, 1.0f);
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, width, state);
            return path.CreateInsetPath(width);
        }
    }
}
