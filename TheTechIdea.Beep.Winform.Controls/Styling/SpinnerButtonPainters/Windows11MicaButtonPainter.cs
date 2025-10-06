using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Windows 11 Mica button painter - Subtle gradient with mica material effect
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class Windows11MicaButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            // Windows 11 Mica: Subtle gradient material effect
            Color accentColor = useThemeColors ? theme.AccentColor : Color.FromArgb(0, 120, 212);
            Color upTopColor = SpinnerButtonPainterHelpers.ApplyState(accentColor, upState);
            Color downTopColor = SpinnerButtonPainterHelpers.ApplyState(accentColor, downState);
            Color upBottomColor = SpinnerButtonPainterHelpers.Darken(upTopColor, 0.05f);
            Color downBottomColor = SpinnerButtonPainterHelpers.Darken(downTopColor, 0.05f);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 4))
            using (var brush1 = new LinearGradientBrush(upButtonRect, upTopColor, upBottomColor, 90f))
            {
                g.FillPath(brush1, path1);
            }

            using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 4))
            using (var brush2 = new LinearGradientBrush(downButtonRect, downTopColor, downBottomColor, 90f))
            {
                g.FillPath(brush2, path2);
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

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, ArrowDirection.Up, Color.White);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, ArrowDirection.Down, Color.White);
        }
    }
}
