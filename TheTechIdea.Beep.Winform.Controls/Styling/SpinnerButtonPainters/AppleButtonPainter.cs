using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Button painter for Apple styles (iOS15, MacOSBigSur)
    /// Uses subtle outlined buttons
    /// Legacy painter - now supports state
    /// </summary>
    public static class AppleButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upRect, Rectangle downRect, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            SpinnerButtonPainterHelpers.ControlState upState = SpinnerButtonPainterHelpers.ControlState.Normal,
            SpinnerButtonPainterHelpers.ControlState downState = SpinnerButtonPainterHelpers.ControlState.Normal)
        {
            Color borderColor = GetColor(style, StyleColors.GetBorder, "Border", theme, useThemeColors);
            Color arrowColor = GetColor(style, StyleColors.GetForeground, "Foreground", theme, useThemeColors);
            
            Color upBorderColor = SpinnerButtonPainterHelpers.ApplyState(borderColor, upState);
            Color downBorderColor = SpinnerButtonPainterHelpers.ApplyState(borderColor, downState);
            
            int radius = 6;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Up button
            using (var upPath = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upRect, radius))
            using (var borderPen = new Pen(upBorderColor, 1f))
            {
                g.DrawPath(borderPen, upPath);
            }
            
            // Down button
            using (var downPath = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downRect, radius))
            using (var borderPen = new Pen(downBorderColor, 1f))
            {
                g.DrawPath(borderPen, downPath);
            }
            
            // State overlays
            Color upOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(upState);
            if (upOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(upOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upRect, radius))
                {
                    g.FillPath(brush, path);
                }
            }
            
            Color downOverlay = SpinnerButtonPainterHelpers.GetStateOverlay(downState);
            if (downOverlay != Color.Transparent)
            {
                using (var brush = new SolidBrush(downOverlay))
                using (var path = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downRect, radius))
                {
                    g.FillPath(brush, path);
                }
            }
            
            SpinnerButtonPainterHelpers.DrawArrow(g, upRect, SpinnerButtonPainterHelpers.ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downRect, SpinnerButtonPainterHelpers.ArrowDirection.Down, arrowColor);
        }
        
        private static Color GetColor(BeepControlStyle style, System.Func<BeepControlStyle, Color> styleColorFunc, string themeColorKey, IBeepTheme theme, bool useThemeColors)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return styleColorFunc(style);
        }
    }
}
