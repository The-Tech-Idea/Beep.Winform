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
            Color outer = BorderPainterHelpers.WithAlpha(Color.White, 50);
            Color inner = BorderPainterHelpers.WithAlpha(Color.White, isFocused ? 80 : 60);

            // Match GlassFormPainter: single border with proper alignment
            var pen = PaintersFactory.GetPen(outer, borderWidth);
            
             
                g.DrawPath(pen, path);
            

            // Return content area inset by half border width
            return path.CreateInsetPath(borderWidth / 2f);
        }
    }
}
