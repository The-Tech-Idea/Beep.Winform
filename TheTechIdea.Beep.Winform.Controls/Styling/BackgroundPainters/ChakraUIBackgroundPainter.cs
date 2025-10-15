using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

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

            // ChakraUI-specific state handling - NO HELPER FUNCTIONS
            // Unique modern 5% hover for Chakra's balanced design
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // ChakraUI hover: Modern 5% lighter
                    int hR = Math.Min(255, backgroundColor.R + (int)(255 * 0.05f));
                    int hG = Math.Min(255, backgroundColor.G + (int)(255 * 0.05f));
                    int hB = Math.Min(255, backgroundColor.B + (int)(255 * 0.05f));
                    stateColor = Color.FromArgb(backgroundColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // ChakraUI pressed: 6% darker (balanced press)
                    int pR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.06f));
                    int pG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.06f));
                    int pB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.06f));
                    stateColor = Color.FromArgb(backgroundColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // ChakraUI selected: 8% lighter (modern selection)
                    int sR = Math.Min(255, backgroundColor.R + (int)(255 * 0.08f));
                    int sG = Math.Min(255, backgroundColor.G + (int)(255 * 0.08f));
                    int sB = Math.Min(255, backgroundColor.B + (int)(255 * 0.08f));
                    stateColor = Color.FromArgb(backgroundColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // ChakraUI focused: 3.5% lighter (balanced focus)
                    int fR = Math.Min(255, backgroundColor.R + (int)(255 * 0.035f));
                    int fG = Math.Min(255, backgroundColor.G + (int)(255 * 0.035f));
                    int fB = Math.Min(255, backgroundColor.B + (int)(255 * 0.035f));
                    stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // ChakraUI disabled: 100 alpha (modern translucency)
                    stateColor = Color.FromArgb(100, backgroundColor);
                    break;
                default: // Normal
                    stateColor = backgroundColor;
                    break;
            }

            using (var brush = new SolidBrush(stateColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
