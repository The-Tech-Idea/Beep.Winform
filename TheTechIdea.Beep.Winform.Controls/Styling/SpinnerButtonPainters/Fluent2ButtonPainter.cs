using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Fluent 2 button painter - Microsoft Fluent Design filled buttons with 4px radius
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class Fluent2ButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // Fluent 2: Modern Microsoft design
            Color buttonColor = useThemeColors ? theme.PrimaryColor : Color.FromArgb(0, 120, 212);
            Color upColor = SpinnerButtonPainterHelpers.ApplyState(buttonColor, upState);
            Color downColor = SpinnerButtonPainterHelpers.ApplyState(buttonColor, downState);
            Color textColor = Color.White;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var brush = new SolidBrush(upColor))
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 4))
            {
                g.FillPath(brush, path);
            }

            using (var brush = new SolidBrush(downColor))
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 4))
            {
                g.FillPath(brush, path);
            }

            // Add subtle hover effect
            if (isFocused)
            {
                Color hoverColor = SpinnerButtonPainterHelpers.Lighten(buttonColor, 0.1f);
                using (var brush = new SolidBrush(SpinnerButtonPainterHelpers.WithAlpha(hoverColor, 40)))
                using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 4))
                using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 4))
                {
                    g.FillPath(brush, path1);
                    g.FillPath(brush, path2);
                }
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

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, textColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, textColor);
        }
    }
}
