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
            float width = StyleBorders.GetBorderWidth(style);
            Color borderColor = BorderPainterHelpers.GetColorFromStyleOrTheme(theme, useThemeColors, "Border", Color.FromArgb(210, 210, 220));

            if (isFocused)
            {
                borderColor = BorderPainterHelpers.Lighten(borderColor, 0.2f);
            }

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, width, state);
            return path.CreateInsetPath(width);
        }
    }
}
