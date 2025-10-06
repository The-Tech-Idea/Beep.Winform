using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
 
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.SpinnerButtonPainters
{
    /// <summary>
    /// Button painter for Fluent Design styles (Fluent2, Windows11Mica)
    /// Uses secondary color filled buttons
    /// Legacy painter - now supports state
    /// </summary>
    public static class FluentButtonPainter
    {
        public static void PaintButtons(Graphics g, Rectangle upRect, Rectangle downRect, bool isFocused, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState upState = ControlState.Normal,
            ControlState downState = ControlState.Normal)
        {
            Color buttonColor = GetColor(style, StyleColors.GetSecondary, "Secondary", theme, useThemeColors);
            Color arrowColor = GetColor(style, StyleColors.GetForeground, "Foreground", theme, useThemeColors);
            
            Color upButtonColor = SpinnerButtonPainterHelpers.ApplyState(buttonColor, upState);
            Color downButtonColor = SpinnerButtonPainterHelpers.ApplyState(buttonColor, downState);
            
            int radius = 4;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Up button
            using (var upPath = SpinnerButtonPainterHelpers.CreateRoundedRectangle(upRect, radius))
            using (var buttonBrush = new SolidBrush(upButtonColor))
            {
                g.FillPath(buttonBrush, upPath);
            }
            
            // Down button
            using (var downPath = SpinnerButtonPainterHelpers.CreateRoundedRectangle(downRect, radius))
            using (var buttonBrush = new SolidBrush(downButtonColor))
            {
                g.FillPath(buttonBrush, downPath);
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
            
            SpinnerButtonPainterHelpers.DrawArrow(g, upRect, ArrowDirection.Up, arrowColor);
            SpinnerButtonPainterHelpers.DrawArrow(g, downRect, ArrowDirection.Down, arrowColor);
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
