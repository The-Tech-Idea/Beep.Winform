using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// elementary OS background painter - Pantheon desktop
    /// Very clean, minimal white backgrounds with subtle interactions
    /// elementary's signature refined, macOS-inspired aesthetic
    /// </summary>
    public static class ElementaryBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, 
            IBeepTheme theme, bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // elementary: clean white
            Color cleanWhite = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : StyleColors.GetBackground(BeepControlStyle.Elementary);
            Color subtleBlue = useThemeColors && theme != null 
                ? theme.AccentColor 
                : StyleColors.GetPrimary(BeepControlStyle.Elementary);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            // elementary uses very subtle state changes (refined, not bold) - use HSL for more natural results
            Color fillColor = state switch
            {
                ControlState.Hovered => ColorAccessibilityHelper.DarkenColor(cleanWhite, 0.03f),
                ControlState.Pressed => ColorAccessibilityHelper.DarkenColor(cleanWhite, 0.06f),
                ControlState.Selected => BackgroundPainterHelpers.BlendColors(cleanWhite, subtleBlue, 0.10f),
                ControlState.Focused => ColorAccessibilityHelper.DarkenColor(cleanWhite, 0.02f),
                ControlState.Disabled => BackgroundPainterHelpers.WithAlpha(cleanWhite, 100),
                _ => cleanWhite
            };

            var brush = PaintersFactory.GetSolidBrush(fillColor);
            g.FillPath(brush, path);
        }
    }
}
