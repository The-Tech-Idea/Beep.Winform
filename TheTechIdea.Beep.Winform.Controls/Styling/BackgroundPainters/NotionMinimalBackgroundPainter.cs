using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
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

            // Subtle state handling for Notion's refined aesthetic
            BackgroundPainterHelpers.PaintSolidBackground(g, path, backgroundColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);
        }
    }
}
