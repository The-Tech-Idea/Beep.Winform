using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Minimal background painter - Simple solid background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class MinimalBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Minimal: Ultra-subtle changes for true minimalism
            Color backgroundColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Minimal);

            // Minimal-specific state handling - NO HELPER FUNCTIONS
            // Unique ultra-subtle 2% changes for true minimal design
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Minimal hover: Only 2% lighter (ultra-subtle)
                    int hR = Math.Min(255, backgroundColor.R + (int)(255 * 0.02f));
                    int hG = Math.Min(255, backgroundColor.G + (int)(255 * 0.02f));
                    int hB = Math.Min(255, backgroundColor.B + (int)(255 * 0.02f));
                    stateColor = Color.FromArgb(backgroundColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // Minimal pressed: 3% darker (very gentle)
                    int pR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.03f));
                    int pG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.03f));
                    int pB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.03f));
                    stateColor = Color.FromArgb(backgroundColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // Minimal selected: 4% lighter (barely noticeable)
                    int sR = Math.Min(255, backgroundColor.R + (int)(255 * 0.04f));
                    int sG = Math.Min(255, backgroundColor.G + (int)(255 * 0.04f));
                    int sB = Math.Min(255, backgroundColor.B + (int)(255 * 0.04f));
                    stateColor = Color.FromArgb(backgroundColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // Minimal focused: 1.5% lighter (extremely subtle)
                    int fR = Math.Min(255, backgroundColor.R + (int)(255 * 0.015f));
                    int fG = Math.Min(255, backgroundColor.G + (int)(255 * 0.015f));
                    int fB = Math.Min(255, backgroundColor.B + (int)(255 * 0.015f));
                    stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // Minimal disabled: 120 alpha (minimal translucency)
                    stateColor = Color.FromArgb(120, backgroundColor);
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
