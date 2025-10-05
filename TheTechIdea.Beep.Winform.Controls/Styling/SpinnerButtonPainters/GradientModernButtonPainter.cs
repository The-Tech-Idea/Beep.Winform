using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Gradient Modern button painter - Vertical gradient effect with 6px radius
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class GradientModernButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            SpinnerButtonPainterHelpers.ControlState upState = SpinnerButtonPainterHelpers.ControlState.Normal,
            SpinnerButtonPainterHelpers.ControlState downState = SpinnerButtonPainterHelpers.ControlState.Normal)
        {
            // Gradient Modern: Sleek vertical gradient
            Color primaryColor = useThemeColors ? theme.PrimaryColor : Color.FromArgb(103, 80, 164);
            Color upPrimaryColor = SpinnerButtonPainterHelpers.ApplyState(primaryColor, upState);
            Color downPrimaryColor = SpinnerButtonPainterHelpers.ApplyState(primaryColor, downState);
            Color upSecondaryColor = SpinnerButtonPainterHelpers.Darken(upPrimaryColor, 0.3f);
            Color downSecondaryColor = SpinnerButtonPainterHelpers.Darken(downPrimaryColor, 0.3f);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 6))
            using (var brush1 = new LinearGradientBrush(upButtonRect, upPrimaryColor, upSecondaryColor, 90f))
            {
                g.FillPath(brush1, path1);
            }

            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 6))
            using (var brush2 = new LinearGradientBrush(downButtonRect, downPrimaryColor, downSecondaryColor, 90f))
            {
                g.FillPath(brush2, path2);
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

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Up, Color.White);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Down, Color.White);
        }
    }
}
