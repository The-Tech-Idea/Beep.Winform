using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Pill Rail background painter - Light solid background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class PillRailBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Pill Rail: Soft light background with simple feedback
            Color backgroundColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.PillRail);

            // PillRail-specific state handling - NO HELPER FUNCTIONS
            // Unique simple 4% hover for pill-shaped rail design
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // PillRail hover: Simple 4% lighter
                    int hR = Math.Min(255, backgroundColor.R + (int)(255 * 0.04f));
                    int hG = Math.Min(255, backgroundColor.G + (int)(255 * 0.04f));
                    int hB = Math.Min(255, backgroundColor.B + (int)(255 * 0.04f));
                    stateColor = Color.FromArgb(backgroundColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // PillRail pressed: 5% darker (simple press)
                    int pR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.05f));
                    int pG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.05f));
                    int pB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.05f));
                    stateColor = Color.FromArgb(backgroundColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // PillRail selected: 6% lighter (noticeable pill selection)
                    int sR = Math.Min(255, backgroundColor.R + (int)(255 * 0.06f));
                    int sG = Math.Min(255, backgroundColor.G + (int)(255 * 0.06f));
                    int sB = Math.Min(255, backgroundColor.B + (int)(255 * 0.06f));
                    stateColor = Color.FromArgb(backgroundColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // PillRail focused: 3% lighter (simple focus)
                    int fR = Math.Min(255, backgroundColor.R + (int)(255 * 0.03f));
                    int fG = Math.Min(255, backgroundColor.G + (int)(255 * 0.03f));
                    int fB = Math.Min(255, backgroundColor.B + (int)(255 * 0.03f));
                    stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // PillRail disabled: 105 alpha (soft translucency)
                    stateColor = Color.FromArgb(105, backgroundColor);
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
