using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Metro button painter - Windows Modern UI flat, sharp-edged buttons
    /// 0px radius (sharp edges), 2px borders, flat colors
    /// Metro signature: Bold, flat design with no curves
    /// </summary>
    public static class MetroButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // Metro: Sharp edges (0px radius), flat colors, 2px borders
            Color borderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.Metro);
            Color primaryColor = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Metro);
            Color arrowColor = useThemeColors ? theme.ForeColor : StyleColors.GetForeground(BeepControlStyle.Metro);
            
            int radius = StyleBorders.GetRadius(BeepControlStyle.Metro); // 0px - sharp edges!
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Metro); // 2.0f

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint flat button backgrounds with sharp edges
            using (var pen = new Pen(borderColor, borderWidth))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
            {
                g.DrawPath(pen, path1);
                g.DrawPath(pen, path2);
            }

            // Metro state overlays - Bold flat colors
            Color upOverlay = GetMetroStateOverlay(upState, primaryColor);
            if (upOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(upOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
                {
                    g.FillPath(brush, path);
                }
            }

            Color downOverlay = GetMetroStateOverlay(downState, primaryColor);
            if (downOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(downOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
                {
                    g.FillPath(brush, path);
                }
            }

            // Draw arrows
            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }

        private static Color GetMetroStateOverlay(ControlState state, Color primaryColor)
        {
            // Metro: Bold, flat state colors (no subtle transitions)
            return state switch
            {
                ControlState.Hovered => Color.FromArgb(30, primaryColor), // 30 alpha overlay
                ControlState.Pressed => Color.FromArgb(60, primaryColor), // 60 alpha overlay (bold)
                ControlState.Selected => Color.FromArgb(50, primaryColor), // 50 alpha overlay
                ControlState.Disabled => Color.FromArgb(120, 240, 240, 240), // Light gray
                _ => Color.Transparent
            };
        }
    }
}
