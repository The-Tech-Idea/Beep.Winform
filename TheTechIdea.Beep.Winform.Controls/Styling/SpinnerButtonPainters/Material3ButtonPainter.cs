using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Material 3 button painter - Filled tonal buttons with 28px height
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class Material3ButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // Material 3: Filled tonal buttons with elevation
            Color buttonColor = useThemeColors ? theme.PrimaryColor : Color.FromArgb(103, 80, 164);
            Color upButtonColor = SpinnerButtonPainterHelpers.ApplyState(buttonColor, upState);
            Color downButtonColor = SpinnerButtonPainterHelpers.ApplyState(buttonColor, downState);
            Color textColor = Color.White;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint up button
            using (var brush = new SolidBrush(upButtonColor))
            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 4))
            {
                g.FillPath(brush, path1);
            }

            // Paint down button
            using (var brush = new SolidBrush(downButtonColor))
            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 4))
            {
                g.FillPath(brush, path2);
            }

            // Add subtle elevation effect
            if (isFocused)
            {
                Color elevationColor = SpinnerButtonPainterHelpers.WithAlpha(Color.White, 20);
                using (var brush = new SolidBrush(elevationColor))
                using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 4))
                using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 4))
                {
                    g.FillPath(brush, path1);
                    g.FillPath(brush, path2);
                }
            }

            // Apply state overlays
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

            // Draw arrows
            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, textColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, textColor);
        }
    }
}
