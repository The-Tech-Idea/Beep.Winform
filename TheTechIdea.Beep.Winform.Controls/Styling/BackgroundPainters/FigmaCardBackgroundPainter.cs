using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Figma Card background painter - Solid white background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class FigmaCardBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Figma Card: Designer-friendly with clear hover feedback
            Color backgroundColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.FigmaCard);

            // FigmaCard-specific state handling - NO HELPER FUNCTIONS
            // Unique designer-friendly clear brightness feedback for Figma design tool
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Figma hover: Clear 10% lighter (designer-friendly feedback)
                    int hR = Math.Min(255, backgroundColor.R + (int)(255 * 0.10f));
                    int hG = Math.Min(255, backgroundColor.G + (int)(255 * 0.10f));
                    int hB = Math.Min(255, backgroundColor.B + (int)(255 * 0.10f));
                    stateColor = Color.FromArgb(backgroundColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // Figma pressed: 8% darker (press feedback)
                    int pR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.08f));
                    int pG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.08f));
                    int pB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.08f));
                    stateColor = Color.FromArgb(backgroundColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // Figma selected: Bold 15% lighter (clear selection for designers)
                    int sR = Math.Min(255, backgroundColor.R + (int)(255 * 0.15f));
                    int sG = Math.Min(255, backgroundColor.G + (int)(255 * 0.15f));
                    int sB = Math.Min(255, backgroundColor.B + (int)(255 * 0.15f));
                    stateColor = Color.FromArgb(backgroundColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // Figma focused: Clear 7% lighter (focus feedback)
                    int fR = Math.Min(255, backgroundColor.R + (int)(255 * 0.07f));
                    int fG = Math.Min(255, backgroundColor.G + (int)(255 * 0.07f));
                    int fB = Math.Min(255, backgroundColor.B + (int)(255 * 0.07f));
                    stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // Figma disabled: 100 alpha (muted)
                    stateColor = Color.FromArgb(100, backgroundColor);
                    break;
                default: // Normal
                    stateColor = backgroundColor;
                    break;
            }

            using (var brush = new SolidBrush(stateColor))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }
        }
    }
}
