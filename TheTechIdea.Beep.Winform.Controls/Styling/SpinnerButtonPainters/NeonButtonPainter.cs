using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    public static class NeonButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal, ControlState downState = ControlState.Normal)
        {
            Color veryDark = useThemeColors ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Neon);
            Color cyanGlow = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Neon);
            
            int radius = StyleBorders.GetRadius(BeepControlStyle.Neon);
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Neon);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var pen = new Pen(cyanGlow, borderWidth))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
            using (var brush = new SolidBrush(veryDark))
            {
                g.FillPath(brush, path1);
                g.FillPath(brush, path2);
                g.DrawPath(pen, path1);
                g.DrawPath(pen, path2);
            }

            Color arrowColor = cyanGlow;
            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }
    }
}
