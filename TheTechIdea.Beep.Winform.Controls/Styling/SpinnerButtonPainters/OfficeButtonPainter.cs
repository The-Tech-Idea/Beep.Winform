using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Office button painter - Microsoft Office Ribbon UI Style
    /// 4px rounded corners, subtle gradients, professional appearance
    /// Compact 32px height buttons with clean styling
    /// </summary>
    public static class OfficeButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // Office: Professional, clean ribbon Style
            Color borderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.Office);
            Color primaryColor = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Office);
            Color arrowColor = useThemeColors ? theme.ForeColor : StyleColors.GetForeground(BeepControlStyle.Office);
            
            int radius = StyleBorders.GetRadius(BeepControlStyle.Office); // 4px subtle rounded
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Office); // 1.0f

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint Office buttons with subtle rounded corners
            using (var pen = new Pen(borderColor, borderWidth))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
            {
                g.DrawPath(pen, path1);
                g.DrawPath(pen, path2);
            }

            // Office state backgrounds - Subtle gradients
            PaintOfficeButtonBackground(g, upButtonRect, upState, primaryColor, radius);
            PaintOfficeButtonBackground(g, downButtonRect, downState, primaryColor, radius);

            // Draw arrows
            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }

        private static void PaintOfficeButtonBackground(Graphics g, Rectangle rect, 
            ControlState state, Color primaryColor, int radius)
        {
            Color topColor, bottomColor;

            // Office: Professional gradients for each state
            switch (state)
            {
                case ControlState.Hovered:
                    // Office hover: Light blue gradient
                    topColor = Color.FromArgb(30, 250, 251, 253);
                    bottomColor = Color.FromArgb(30, 245, 248, 252);
                    break;
                case ControlState.Pressed:
                    // Office pressed: Noticeable blue gradient
                    topColor = Color.FromArgb(80, primaryColor);
                    bottomColor = Color.FromArgb(100, primaryColor);
                    break;
                case ControlState.Selected:
                    // Office selected: Medium blue gradient
                    topColor = Color.FromArgb(50, primaryColor);
                    bottomColor = Color.FromArgb(70, primaryColor);
                    break;
                case ControlState.Disabled:
                    // Office disabled: Gray gradient
                    topColor = Color.FromArgb(100, 245, 245, 245);
                    bottomColor = Color.FromArgb(100, 235, 235, 235);
                    break;
                default:
                    // Normal: No background (transparent)
                    return;
            }

            // Paint subtle gradient
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(rect, radius))
            using (var brush = new LinearGradientBrush(
                new Point(rect.Left, rect.Top),
                new Point(rect.Left, rect.Bottom),
                topColor, bottomColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
