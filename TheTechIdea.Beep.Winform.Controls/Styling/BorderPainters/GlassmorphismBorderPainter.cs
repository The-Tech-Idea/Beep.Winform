using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

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
            if (borderWidth <= 0f) return path;
            Color outer = BorderPainterHelpers.WithAlpha(Color.White, 50);
            if (isFocused)
            {
                outer = BorderPainterHelpers.WithAlpha(Color.White, 80);
            }
            outer = BorderPainterHelpers.EnsureVisibleBorderColor(outer, theme, state);

            // Match GlassFormPainter: single border with proper alignment
            BorderPainterHelpers.PaintSimpleBorder(g, path, outer, borderWidth, state);

            // Return content area inset by half border width
            return BorderPainterHelpers.CreateStrokeInsetPath(path, borderWidth);
        }
    }
}
