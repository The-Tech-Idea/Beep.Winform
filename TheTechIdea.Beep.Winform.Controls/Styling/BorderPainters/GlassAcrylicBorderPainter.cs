using System.Drawing;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// GlassAcrylic border painter - Frosted glass with subtle translucent border
    /// </summary>
    public static class GlassAcrylicBorderPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            BorderPainterHelpers.ControlState state = BorderPainterHelpers.ControlState.Normal)
        {
            Color borderColor = isFocused
                ? BorderPainterHelpers.WithAlpha(255, 255, 255, 80)
                : BorderPainterHelpers.WithAlpha(255, 255, 255, 60);

            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, 1f, state);
        }
    }
}

