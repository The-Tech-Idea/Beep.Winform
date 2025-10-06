using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Tailwind Card button painter - Ring effect on focus with 4px radius
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class TailwindCardButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // Tailwind: Utility-first design with ring effect
            Color borderColor = Color.FromArgb(229, 231, 235);
            Color ringColor = Color.FromArgb(59, 130, 246);
            Color arrowColor = Color.FromArgb(75, 85, 99);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Ring effect on focus
            if (isFocused)
            {
                var upRing = upButtonRect;
                upRing.Inflate(2, 2);
                var downRing = downButtonRect;
                downRing.Inflate(2, 2);

                using (var pen = new Pen(SpinnerButtonPainterHelpers.WithAlpha(ringColor, 40), 3f))
                using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upRing, 6))
                using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downRing, 6))
                {
                    g.DrawPath(pen, path1);
                    g.DrawPath(pen, path2);
                }
            }

            using (var pen = new Pen(borderColor, 1f))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 4))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 4))
            {
                g.DrawPath(pen, path1);
                g.DrawPath(pen, path2);
            }

            // State overlays
            Color upOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(upState);
            if (upOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(upOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            Color downOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(downState);
            if (downOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(downOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 4))
                {
                    g.FillPath(brush, path);
                }
            }

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, arrowColor);
        }
    }
}
