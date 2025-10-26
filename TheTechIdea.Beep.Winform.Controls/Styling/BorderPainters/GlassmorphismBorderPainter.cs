using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Glassmorphism border painter - translucent double stroke.
    /// </summary>
    public static class GlassmorphismBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            float borderWidth = StyleBorders.GetBorderWidth(style);
            Color outer = BorderPainterHelpers.WithAlpha(Color.White, 50);
            Color inner = BorderPainterHelpers.WithAlpha(Color.White, isFocused ? 80 : 60);

            BorderPainterHelpers.PaintSimpleBorder(g, path, outer, borderWidth, state);

            using (var inset = path.CreateInsetPath(borderWidth + 1f))
            {
                BorderPainterHelpers.PaintSimpleBorder(g, inset, inner, 1f, state);
            }

            return path.CreateInsetPath(borderWidth + 2f);
        }
    }
}
