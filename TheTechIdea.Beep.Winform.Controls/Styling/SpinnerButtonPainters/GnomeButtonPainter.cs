using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Gnome button painter - Adwaita design system
    /// 6px rounded corners (friendly)
    /// 1px borders with subtle gradients
    /// Blue accent for selection, warm gray for normal state
    /// </summary>
    public static class GnomeButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // Gnome Adwaita colors
            Color borderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.Gnome);
            Color blueAccent = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Gnome);
            Color warmGray = useThemeColors ? theme.BackgroundColor : StyleColors.GetBackground(BeepControlStyle.Gnome);
            
            int radius = StyleBorders.GetRadius(BeepControlStyle.Gnome); // 6px - friendly rounded
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Gnome); // 1.0f

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint borders
            using (var pen = new Pen(borderColor, borderWidth))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
            {
                g.DrawPath(pen, path1);
                g.DrawPath(pen, path2);
            }

            // Paint Gnome Adwaita backgrounds with subtle gradients
            PaintGnomeButtonBackground(g, upButtonRect, radius, upState, warmGray, blueAccent);
            PaintGnomeButtonBackground(g, downButtonRect, radius, downState, warmGray, blueAccent);

            // Draw arrows
            Color arrowColor = Color.FromArgb(60, 60, 60); // Dark gray for contrast
            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }

        private static void PaintGnomeButtonBackground(Graphics g, Rectangle rect, int radius, 
            ControlState state, Color warmGray, Color blueAccent)
        {
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(rect, radius))
            {
                Color fillColor = GetGnomeStateFill(state, warmGray, blueAccent);

                // Gnome: Subtle vertical gradient (Adwaita signature)
                if (state == ControlState.Normal || state == ControlState.Hovered)
                {
                    Color topColor = ColorUtils.Lighten(fillColor, 0.03f);
                    Color bottomColor = ColorUtils.Darken(fillColor, 0.03f);

                    using (var brush = new LinearGradientBrush(rect, topColor, bottomColor, LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(fillColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        private static Color GetGnomeStateFill(ControlState state, Color warmGray, Color blueAccent)
        {
            return state switch
            {
                ControlState.Hovered => ColorUtils.Lighten(warmGray, 0.08f),
                ControlState.Pressed => ColorUtils.Darken(warmGray, 0.12f),
                ControlState.Selected => Color.FromArgb(40, blueAccent), // Translucent blue
                ControlState.Disabled => ColorUtils.Lighten(warmGray, 0.15f),
                _ => warmGray
            };
        }
    }
}
