using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// KDE button painter - Breeze design system
    /// 4px rounded corners (modern)
    /// Clean flat backgrounds with blue glow on hover
    /// 1px borders, Breeze blue (#3DAEE9) accent
    /// </summary>
    public static class KdeButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // KDE Breeze colors
            Color borderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.Kde);
            Color breezeBlue = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Kde);
            Color lightBackground = useThemeColors ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Kde);
            
            int radius = StyleBorders.GetRadius(BeepControlStyle.Kde); // 4px
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Kde); // 1.0f

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint KDE Breeze backgrounds
            PaintKdeButtonBackground(g, upButtonRect, radius, upState, lightBackground, breezeBlue);
            PaintKdeButtonBackground(g, downButtonRect, radius, downState, lightBackground, breezeBlue);

            // Paint borders
            Color upBorderColor = upState == ControlState.Hovered || upState == ControlState.Selected ? breezeBlue : borderColor;
            Color downBorderColor = downState == ControlState.Hovered || downState == ControlState.Selected ? breezeBlue : borderColor;

            using (var pen1 = new Pen(upBorderColor, borderWidth))
            using (var pen2 = new Pen(downBorderColor, borderWidth))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
            {
                g.DrawPath(pen1, path1);
                g.DrawPath(pen2, path2);
            }

            // Draw arrows
            Color arrowColor = Color.FromArgb(50, 50, 50);
            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }

        private static void PaintKdeButtonBackground(Graphics g, Rectangle rect, int radius, 
            ControlState state, Color lightBackground, Color breezeBlue)
        {
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(rect, radius))
            {
                Color fillColor = GetKdeStateFill(state, lightBackground, breezeBlue);

                // KDE: Flat fill
                using (var brush = new SolidBrush(fillColor))
                {
                    g.FillPath(brush, path);
                }

                // KDE signature: Blue glow overlay on hover
                if (state == ControlState.Hovered)
                {
                    using (var glowBrush = new SolidBrush(Color.FromArgb(25, breezeBlue)))
                    {
                        g.FillPath(glowBrush, path);
                    }
                }
            }
        }

        private static Color GetKdeStateFill(ControlState state, Color lightBackground, Color breezeBlue)
        {
            return state switch
            {
                ControlState.Hovered => lightBackground,
                ControlState.Pressed => ColorUtils.Darken(lightBackground, 0.1f),
                ControlState.Selected => Color.FromArgb(30, breezeBlue),
                ControlState.Disabled => ColorUtils.Lighten(lightBackground, 0.1f),
                _ => lightBackground
            };
        }
    }
}
