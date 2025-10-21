using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    public static class ElementaryButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal, ControlState downState = ControlState.Normal)
        {
            Color borderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.Elementary);
            Color cleanWhite = useThemeColors ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Elementary);
            
            int radius = StyleBorders.GetRadius(BeepControlStyle.Elementary);
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Elementary);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var pen = new Pen(borderColor, borderWidth))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
            using (var brush = new SolidBrush(cleanWhite))
            {
                g.FillPath(brush, path1);
                g.FillPath(brush, path2);
                g.DrawPath(pen, path1);
                g.DrawPath(pen, path2);
            }

            Color arrowColor = Color.FromArgb(80, 80, 80);
            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }
    }
}
