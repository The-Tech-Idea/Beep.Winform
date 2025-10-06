using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Notion Minimal button painter - Very light borders with 3px radius
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class NotionMinimalButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // Notion: Very light and minimal
            Color borderColor = Color.FromArgb(235, 235, 235);
            Color arrowColor = Color.FromArgb(55, 53, 47);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var pen = new Pen(borderColor, 1f))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 3))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 3))
            {
                g.DrawPath(pen, path1);
                g.DrawPath(pen, path2);
            }

            // State overlays
            Color upOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(upState);
            if (upOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(upOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 3))
                {
                    g.FillPath(brush, path);
                }
            }

            Color downOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(downState);
            if (downOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(downOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 3))
                {
                    g.FillPath(brush, path);
                }
            }

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }
    }
}
