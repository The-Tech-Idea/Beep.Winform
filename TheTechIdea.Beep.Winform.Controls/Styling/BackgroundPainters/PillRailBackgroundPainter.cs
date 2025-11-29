using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Pill Rail background painter - iOS-style pill selector
    /// Light solid background with simple, clean state feedback
    /// </summary>
    public static class PillRailBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Pill Rail: soft light gray
            Color backgroundColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.PillRail);

            // Simple solid fill with normal state handling
            BackgroundPainterHelpers.PaintSolidBackground(g, path, backgroundColor, state,
                BackgroundPainterHelpers.StateIntensity.Normal);
        }
    }
}
