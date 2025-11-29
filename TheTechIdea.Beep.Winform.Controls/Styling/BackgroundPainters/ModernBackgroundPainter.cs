using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Modern background painter - contemporary clean design
    /// Light background with subtle gradient overlay for refined depth
    /// </summary>
    public static class ModernBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, BeepControlStyle style, IBeepTheme theme,
            bool useThemeColors, ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Modern: crisp off-white (Apple-inspired)
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0xF5, 0xF5, 0xF7);

            // Use subtle gradient background method
            BackgroundPainterHelpers.PaintSubtleGradientBackground(g, path, baseColor, 
                0.05f, state, BackgroundPainterHelpers.StateIntensity.Normal);
        }
    }
}
