using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Metro background painter - Windows 8 Modern UI flat tile design
    /// Bold, flat, no gradients - pure geometric simplicity
    /// </summary>
    public static class MetroBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Metro: crisp light gray background
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackgroundColor 
                : Color.FromArgb(0xF8, 0xF8, 0xFA);

            // Flat design: pure solid fill with strong state changes
            using (var scope = new BackgroundPainterHelpers.SmoothingScope(g, SmoothingMode.None))
            {
                BackgroundPainterHelpers.PaintSolidBackground(g, path, baseColor, state,
                    BackgroundPainterHelpers.StateIntensity.Strong);
            }
        }
    }
}
