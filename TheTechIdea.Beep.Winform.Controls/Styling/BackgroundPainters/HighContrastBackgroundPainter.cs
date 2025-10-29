using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// HighContrast background painter - WCAG AAA accessibility compliance
    /// Pure white background for maximum contrast with black text/borders
    /// NO gradients - flat colors for clarity and readability
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class HighContrastBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // HighContrast: Pure white (#FFFFFF) or pure black for maximum contrast
            Color backgroundColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.HighContrast);
            Color selectionColor = useThemeColors ? theme.AccentColor : StyleColors.GetSelection(BeepControlStyle.HighContrast);

            // HighContrast-specific state handling - WCAG AAA compliant
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // HighContrast hover: Very light gray for subtle indication
                    stateColor = Color.FromArgb(250,250,250);
                    break;
                case ControlState.Pressed:
                    // HighContrast pressed: Light gray for clear feedback
                    stateColor = Color.FromArgb(240,240,240);
                    break;
                case ControlState.Selected:
                    // HighContrast selected: Yellow (#FFFF00) for WCAG AAA visibility
                    stateColor = selectionColor; // Yellow for maximum contrast
                    break;
                case ControlState.Focused:
                    // HighContrast focused: Pure white (rely on focus ring)
                    stateColor = backgroundColor;
                    break;
                case ControlState.Disabled:
                    // HighContrast disabled: Medium gray (still readable)
                    stateColor = Color.FromArgb(200,200,200);
                    break;
                default: // Normal
                    // HighContrast normal: Pure white (#FFFFFF)
                    stateColor = backgroundColor;
                    break;
            }

            // Fill with FLAT color (NO gradients - accessibility requirement)
            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);
        }
    }
}
