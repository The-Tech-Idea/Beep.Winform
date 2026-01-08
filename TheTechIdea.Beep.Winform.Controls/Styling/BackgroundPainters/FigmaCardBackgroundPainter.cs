using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Figma Card background painter - design tool aesthetic
    /// Clean white background with clear hover feedback for designers
    /// </summary>
    public static class FigmaCardBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Figma Card: clean white
            Color backgroundColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.FigmaCard);

            // Ensure accessibility compliance for text containers
            if (useThemeColors && theme != null && theme.ForeColor != Color.Empty)
            {
                Color accessibleColor = ColorAccessibilityHelper.EnsureContrastRatio(
                    backgroundColor, theme.ForeColor, 
                    ColorAccessibilityHelper.WCAG_AA_Normal);
                if (accessibleColor != backgroundColor)
                {
                    backgroundColor = accessibleColor;
                }
            }

            // Strong state feedback for designer-friendly interaction
            BackgroundPainterHelpers.PaintSolidBackground(g, path, backgroundColor, state,
                BackgroundPainterHelpers.StateIntensity.Strong);
        }
    }
}
