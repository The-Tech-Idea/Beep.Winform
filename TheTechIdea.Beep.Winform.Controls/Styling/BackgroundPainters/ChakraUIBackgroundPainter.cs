using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Chakra UI background painter - modern React design system
    /// Clean solid backgrounds with balanced state feedback
    /// </summary>
    public static class ChakraUIBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Chakra UI: clean white
            Color backgroundColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.ChakraUI);

            // Chakra UI uses normal balanced state feedback
            BackgroundPainterHelpers.PaintSolidBackground(g, path, backgroundColor, state,
                BackgroundPainterHelpers.StateIntensity.Normal);
        }
    }
}
