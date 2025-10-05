using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// iOS 15 button painter - Outlined buttons with system accent and 6px radius
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class iOS15ButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            SpinnerButtonPainterHelpers.ControlState upState = SpinnerButtonPainterHelpers.ControlState.Normal,
            SpinnerButtonPainterHelpers.ControlState downState = SpinnerButtonPainterHelpers.ControlState.Normal)
        {
            // iOS 15: Light outlined style with system blue accent
            Color accentColor = useThemeColors ? theme.AccentColor : Color.FromArgb(0, 122, 255);
            Color borderColor = Color.FromArgb(235, 235, 245);
            Color backgroundColor = Color.FromArgb(248, 248, 248);
            Color upBgColor = SpinnerButtonPainterHelpers.ApplyState(backgroundColor, upState);
            Color downBgColor = SpinnerButtonPainterHelpers.ApplyState(backgroundColor, downState);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Fill with subtle background
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

            // Draw border
            using (var pen = new Pen(borderColor, 0.5f))
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

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Up, accentColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Down, accentColor);
        }
    }
}
