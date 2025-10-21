using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    public static class CinnamonButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal, ControlState downState = ControlState.Normal)
        {
            Color borderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.Cinnamon);
            Color mintGreen = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Cinnamon);
            Color lightGray = useThemeColors ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Cinnamon);
            
            int radius = StyleBorders.GetRadius(BeepControlStyle.Cinnamon);
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Cinnamon);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var pen = new Pen(borderColor, borderWidth))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
            {
                Color upFill = upState == ControlState.Hovered ? ColorUtils.Lighten(lightGray, 0.08f) : lightGray;
                Color downFill = downState == ControlState.Hovered ? ColorUtils.Lighten(lightGray, 0.08f) : lightGray;

                using (var brush1 = new SolidBrush(upFill))
                using (var brush2 = new SolidBrush(downFill))
                {
                    g.FillPath(brush1, path1);
                    g.FillPath(brush2, path2);
                }

                g.DrawPath(pen, path1);
                g.DrawPath(pen, path2);
            }

            Color arrowColor = Color.FromArgb(60, 60, 60);
            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }
    }
}
