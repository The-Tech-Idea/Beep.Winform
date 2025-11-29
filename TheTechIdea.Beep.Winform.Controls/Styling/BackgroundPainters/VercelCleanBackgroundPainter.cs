using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Vercel Clean background painter - developer-focused minimal design
    /// Pure clean white with subtle feedback
    /// </summary>
    public static class VercelCleanBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Vercel Clean: pure white
            Color backgroundColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.VercelClean);

            // Subtle state handling for Vercel's clean aesthetic
            BackgroundPainterHelpers.PaintSolidBackground(g, path, backgroundColor, state,
                BackgroundPainterHelpers.StateIntensity.Subtle);
        }
    }
}
