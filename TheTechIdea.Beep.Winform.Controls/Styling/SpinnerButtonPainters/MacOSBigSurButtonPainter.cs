using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// macOS Big Sur button painter - System Style with subtle borders and 6px radius
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class MacOSBigSurButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // macOS Big Sur: System control Style
            Color accentColor = useThemeColors ? theme.AccentColor : Color.FromArgb(0, 122, 255);
            Color borderColor = Color.FromArgb(220, 220, 220);
            Color backgroundColor = Color.FromArgb(250, 250, 250);
            Color upBgColor = SpinnerButtonPainterHelpers.ApplyState(backgroundColor, upState);
            Color downBgColor = SpinnerButtonPainterHelpers.ApplyState(backgroundColor, downState);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fill with system background
            using (var brush = new SolidBrush(upBgColor))
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 6))
            {
                g.FillPath(brush, path);
            }

            using (var brush = new SolidBrush(downBgColor))
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 6))
            {
                g.FillPath(brush, path);
            }

            // Draw subtle border
            using (var pen = new Pen(borderColor, 0.75f))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 6))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 6))
            {
                g.DrawPath(pen, path1);
                g.DrawPath(pen, path2);
            }

            // State overlays
            Color upOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(upState);
            if (upOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(upOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 6))
                {
                    g.FillPath(brush, path);
                }
            }

            Color downOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(downState);
            if (downOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(downOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 6))
                {
                    g.FillPath(brush, path);
                }
            }

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, accentColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, accentColor);
        }
    }
}
