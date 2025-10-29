using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Chakra UI background painter - Solid white background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class ChakraUIBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Chakra UI: Modern clean with balanced feedback
            Color backgroundColor = StyleColors.GetBackground(BeepControlStyle.ChakraUI);

            Color stateColor = state switch
            {
                ControlState.Hovered => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.05f)), Math.Min(255, backgroundColor.G + (int)(255 *0.05f)), Math.Min(255, backgroundColor.B + (int)(255 *0.05f))),
                ControlState.Pressed => Color.FromArgb(backgroundColor.A, Math.Max(0, backgroundColor.R - (int)(backgroundColor.R *0.06f)), Math.Max(0, backgroundColor.G - (int)(backgroundColor.G *0.06f)), Math.Max(0, backgroundColor.B - (int)(backgroundColor.B *0.06f))),
                ControlState.Selected => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.08f)), Math.Min(255, backgroundColor.G + (int)(255 *0.08f)), Math.Min(255, backgroundColor.B + (int)(255 *0.08f))),
                ControlState.Focused => Color.FromArgb(backgroundColor.A, Math.Min(255, backgroundColor.R + (int)(255 *0.035f)), Math.Min(255, backgroundColor.G + (int)(255 *0.035f)), Math.Min(255, backgroundColor.B + (int)(255 *0.035f))),
                ControlState.Disabled => Color.FromArgb(100, backgroundColor),
                _ => backgroundColor,
            };

            var brush = PaintersFactory.GetSolidBrush(stateColor);
            g.FillPath(brush, path);
        }
    }
}
