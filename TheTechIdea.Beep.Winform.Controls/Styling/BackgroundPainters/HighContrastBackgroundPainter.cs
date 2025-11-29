using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// High Contrast background painter - WCAG AAA accessibility compliance
    /// Pure flat colors for maximum contrast and readability
    /// NO gradients - accessibility requirement
    /// </summary>
    public static class HighContrastBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // High Contrast: pure white/black for maximum contrast
            Color backgroundColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.HighContrast);
            Color selectionColor = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetSelection(BeepControlStyle.HighContrast);

            // WCAG AAA compliant state colors - explicit for accessibility
            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(250, 250, 250),
                ControlState.Pressed => Color.FromArgb(240, 240, 240),
                ControlState.Selected => selectionColor,
                ControlState.Focused => backgroundColor,
                ControlState.Disabled => Color.FromArgb(200, 200, 200),
                _ => backgroundColor
            };

            // FLAT color only - no gradients for accessibility
            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);
        }
    }
}
