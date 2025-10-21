using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    public static class NeonBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (path == null) return path;

            Color cyanGlow = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Neon);
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Neon);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            BorderPainterHelpers.PaintSimpleBorder(g, path, cyanGlow, borderWidth);

            // Neon: Always show intense glow
            BorderPainterHelpers.PaintGlowBorder(g, path, cyanGlow, 6.0f, 1.0f);

            return path;
        }
    }
}
