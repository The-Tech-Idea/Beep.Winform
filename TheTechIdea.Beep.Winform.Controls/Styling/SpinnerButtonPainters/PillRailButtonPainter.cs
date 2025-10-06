using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Pill Rail button painter - Pill-shaped soft buttons with 12px radius
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class PillRailButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // Pill Rail: Soft rounded pill-shaped style
            Color backgroundColor = Color.FromArgb(243, 244, 246);
            Color upBgColor = SpinnerButtonPainterHelpers.ApplyState(backgroundColor, upState);
            Color downBgColor = SpinnerButtonPainterHelpers.ApplyState(backgroundColor, downState);
            Color borderColor = Color.FromArgb(229, 231, 235);
            Color arrowColor = Color.FromArgb(75, 85, 99);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var brush = new SolidBrush(upBgColor))
            using (var pen = new Pen(borderColor, 1f))
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 12))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            using (var brush = new SolidBrush(downBgColor))
            using (var pen = new Pen(borderColor, 1f))
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 12))
            {
                g.FillPath(brush, path);
                g.DrawPath(pen, path);
            }

            // State overlays
            Color upOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(upState);
            if (upOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(upOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 12))
                {
                    g.FillPath(brush, path);
                }
            }

            Color downOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(downState);
            if (downOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(downOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 12))
                {
                    g.FillPath(brush, path);
                }
            }

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }
    }
}
