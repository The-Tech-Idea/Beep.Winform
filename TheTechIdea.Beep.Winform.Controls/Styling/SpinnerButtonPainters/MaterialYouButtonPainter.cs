using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Material You button painter - Dynamic color tonal buttons with 8px radius
    /// </summary>
    public static class MaterialYouButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upButtonRect, Rectangle downButtonRect,
            bool isFocused, BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            SpinnerButtonPainterHelpers.ControlState upState = SpinnerButtonPainterHelpers.ControlState.Normal,
            SpinnerButtonPainterHelpers.ControlState downState = SpinnerButtonPainterHelpers.ControlState.Normal)
        {
            // Material You: Dynamic color system with tonal surface
            Color primaryColor = useThemeColors ? theme.PrimaryColor : Color.FromArgb(103, 80, 164);
            Color baseButtonColor = SpinnerButtonPainterHelpers.Lighten(primaryColor, 0.12f);
            Color upColor = SpinnerButtonPainterHelpers.ApplyState(baseButtonColor, upState);
            Color downColor = SpinnerButtonPainterHelpers.ApplyState(baseButtonColor, downState);
            Color textColor = Color.White;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var brush = new SolidBrush(upColor))
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 8))
            {
                g.FillPath(brush, path);
            }

            using (var brush = new SolidBrush(downColor))
            using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 8))
            {
                g.FillPath(brush, path);
            }

            // Add dynamic color overlay on focus
            if (isFocused)
            {
                Color overlayColor = SpinnerButtonPainterHelpers.WithAlpha(primaryColor, 30);
                using (var brush = new SolidBrush(overlayColor))
                using (var path1 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 8))
                using (var path2 = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 8))
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
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upButtonRect, 8))
                {
                    g.FillPath(brush, path);
                }
            }

            Color downOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(downState);
            if (downOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(downOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downButtonRect, 8))
                {
                    g.FillPath(brush, path);
                }
            }

            SpinnerButtonPainterHelpers.DrawArrow(g, upButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Up, textColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downButtonRect, SpinnerButtonPainterHelpers.ArrowDirection.Down, textColor);
        }
    }
}
