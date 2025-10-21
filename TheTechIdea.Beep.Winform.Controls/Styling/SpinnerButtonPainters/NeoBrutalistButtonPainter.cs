using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// NeoBrutalist button painter - THICK BLACK BORDERS, sharp edges, bold colors
    /// 0px radius (sharp!), 4px thick black borders, bold yellow/magenta fills
    /// Neo-Brutalism signature: Raw, aggressive, brutalist aesthetic
    /// </summary>
    public static class NeoBrutalistButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // NeoBrutalist: THICK black borders, sharp edges, bold colors
            Color blackBorder = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.NeoBrutalist);
            Color backgroundColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.NeoBrutalist);
            Color secondaryColor = useThemeColors ? theme.SecondaryColor : StyleColors.GetSecondary(BeepControlStyle.NeoBrutalist);
            Color arrowColor = Color.Black; // Always black arrows for contrast
            
            int radius = StyleBorders.GetRadius(BeepControlStyle.NeoBrutalist); // 0px - SHARP!
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.NeoBrutalist); // 4.0f THICK!

            g.SmoothingMode = SmoothingMode.None; // NO anti-aliasing for raw brutalist look!

            // Paint THICK black borders with sharp edges
            using (var pen = new Pen(blackBorder, borderWidth))
            {
                pen.LineJoin = LineJoin.Miter; // Sharp corners!
                
                using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
                using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
                {
                    g.DrawPath(pen, path1);
                    g.DrawPath(pen, path2);
                }
            }

            // NeoBrutalist bold state backgrounds - FLAT colors (no gradients!)
            Color upFill = GetNeoBrutalistStateFill(upState, backgroundColor, secondaryColor);
            if (upFill != Color.Transparent)
            {
                using (var brush = new SolidBrush(upFill))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, radius))
                {
                    g.FillPath(brush, path);
                }
            }

            Color downFill = GetNeoBrutalistStateFill(downState, backgroundColor, secondaryColor);
            if (downFill != Color.Transparent)
            {
                using (var brush = new SolidBrush(downFill))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, radius))
                {
                    g.FillPath(brush, path);
                }
            }

            // Draw BOLD black arrows
            g.SmoothingMode = SmoothingMode.AntiAlias; // Re-enable for arrows
            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }

        private static Color GetNeoBrutalistStateFill(ControlState state, Color yellow, Color magenta)
        {
            // NeoBrutalist: BOLD flat color fills (no subtlety!)
            return state switch
            {
                ControlState.Hovered => magenta, // SWAP to magenta!
                ControlState.Pressed => Color.FromArgb(Math.Max(0, magenta.R - 40), Math.Max(0, magenta.G - 40), Math.Max(0, magenta.B - 40)), // Darker magenta
                ControlState.Selected => magenta, // Bold magenta
                ControlState.Disabled => Color.FromArgb(200, 200, 200), // Gray
                _ => yellow // Normal: Bold yellow
            };
        }
    }
}
