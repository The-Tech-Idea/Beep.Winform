using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Notion Minimal background painter - productivity app aesthetic
    /// Clean background with extremely refined, barely noticeable changes
    /// </summary>
    public static class NotionMinimalBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Notion Minimal: off-white
            Color backgroundColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.NotionMinimal);

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

            // Subtle state handling for Notion's refined aesthetic
            BackgroundPainterHelpers.PaintSolidBackground(g, path, backgroundColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);
        }
    }
}
