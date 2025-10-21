using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Metro border painter - Windows Modern UI flat, sharp-edged borders
    /// 2px borders, no radius (sharp edges), bold color changes
    /// Metro signature: Flat design with no curves
    /// </summary>
    public static class MetroBorderPainter
    {
        public static GraphicsPath Paint(Graphics g, GraphicsPath path, bool isFocused,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            Color baseBorderColor = useThemeColors ? theme.BorderColor : StyleColors.GetBorder(BeepControlStyle.Metro);
            Color primaryColor = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Metro);
            Color borderColor = baseBorderColor;

            // Metro: Bold, flat color changes for different states
            switch (state)
            {
                case ControlState.Hovered:
                    // Metro hover: Use darker primary color (bold change)
                    borderColor = BorderPainterHelpers.WithAlpha(primaryColor, 150);
                    break;
                case ControlState.Pressed:
                    // Metro pressed: Full primary color (bold accent)
                    borderColor = primaryColor;
                    break;
                case ControlState.Selected:
                    // Metro selected: Full primary color (bold accent)
                    borderColor = primaryColor;
                    break;
                case ControlState.Disabled:
                    // Metro disabled: Lighter border
                    borderColor = BorderPainterHelpers.WithAlpha(baseBorderColor, 80);
                    break;
            }

            // Metro focused: Simple 2px accent border (no ring - flat design)
            if (isFocused)
            {
                borderColor = primaryColor;
            }

            // Paint Metro 2px flat border (sharp edges - 0px radius from StyleBorders)
            float borderWidth = StyleBorders.GetBorderWidth(BeepControlStyle.Metro); // 2.0f
            BorderPainterHelpers.PaintSimpleBorder(g, path, borderColor, borderWidth, state);

            // Return the area inside the border
            return path.CreateInsetPath(borderWidth);
        }
    }
}
