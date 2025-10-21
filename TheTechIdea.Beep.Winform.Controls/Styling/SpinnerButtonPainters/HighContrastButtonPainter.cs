using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// HighContrast button painter - WCAG AAA accessibility compliance
    /// Pure black 2px borders, 0px radius (sharp for clarity), large touch targets
    /// Yellow selection (#FFFF00) for maximum contrast
    /// 48px item height for accessibility
    /// </summary>
    public static class HighContrastButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // HighContrast: Pure colors for maximum contrast (WCAG AAA)
            Color blackBorder = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.HighContrast);
            Color yellowSelection = useThemeColors ? theme.AccentColor : StyleColors.GetSelection(BeepControlStyle.HighContrast);
            Color blackArrow = Color.Black; // Always black for maximum contrast
            
            int radius = StyleBorders.GetRadius(BeepControlStyle.HighContrast); // 0px - sharp for clarity
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.HighContrast); // 2.0f

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint HighContrast 2px black borders with sharp edges
            using (var pen = new Pen(blackBorder, borderWidth))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
            {
                g.DrawPath(pen, path1);
                g.DrawPath(pen, path2);
            }

            // HighContrast state backgrounds - WCAG AAA compliant colors
            Color upFill = GetHighContrastStateFill(upState, yellowSelection);
            if (upFill != Color.Transparent)
            {
                using (var brush = new SolidBrush(upFill))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
                {
                    g.FillPath(brush, path);
                }
            }

            Color downFill = GetHighContrastStateFill(downState, yellowSelection);
            if (downFill != Color.Transparent)
            {
                using (var brush = new SolidBrush(downFill))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
                {
                    g.FillPath(brush, path);
                }
            }

            // Draw black arrows for maximum contrast
            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, blackArrow);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, blackArrow);
        }

        private static Color GetHighContrastStateFill(ControlState state, Color yellowSelection)
        {
            // HighContrast: WCAG AAA compliant fills
            return state switch
            {
                ControlState.Hovered => Color.FromArgb(250, 250, 250), // Very light gray
                ControlState.Pressed => Color.FromArgb(240, 240, 240), // Light gray
                ControlState.Selected => yellowSelection, // Yellow (#FFFF00) for max contrast
                ControlState.Disabled => Color.FromArgb(200, 200, 200), // Medium gray
                _ => Color.Transparent // Normal: transparent (white background)
            };
        }
    }
}
