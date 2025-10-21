using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    public static class GamingButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal, ControlState downState = ControlState.Normal)
        {
            Color darkBg = useThemeColors ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Gaming);
            Color neonGreen = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Gaming);
            
            int radius = StyleBorders.GetRadius(BeepControlStyle.Gaming); // 0px - angular
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Gaming);

            g.SmoothingMode = SmoothingMode.None; // Angular gaming aesthetic

            using (var pen = new Pen(neonGreen, borderWidth))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
            using (var brush = new SolidBrush(darkBg))
            {
                g.FillPath(brush, path1);
                g.FillPath(brush, path2);
                g.DrawPath(pen, path1);
                g.DrawPath(pen, path2);
            }

            Color arrowColor = neonGreen;
            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }
    }
}
