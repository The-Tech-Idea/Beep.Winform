using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Fluent 2 background painter - Solid clean background
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class Fluent2BackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Fluent 2: Clean solid background with acrylic-inspired opacity changes
            Color baseColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Fluent2);

            // Fluent2-specific state handling - NO HELPER FUNCTIONS
            // Unique acrylic-inspired opacity modulation for Fluent Design
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Fluent2 hover: Increase opacity/brightness 10%
                    int hR = Math.Min(255, baseColor.R + (int)(255 * 0.10f));
                    int hG = Math.Min(255, baseColor.G + (int)(255 * 0.10f));
                    int hB = Math.Min(255, baseColor.B + (int)(255 * 0.10f));
                    stateColor = Color.FromArgb(baseColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // Fluent2 pressed: Decrease opacity/brightness 8%
                    int pR = Math.Max(0, baseColor.R - (int)(baseColor.R * 0.08f));
                    int pG = Math.Max(0, baseColor.G - (int)(baseColor.G * 0.08f));
                    int pB = Math.Max(0, baseColor.B - (int)(baseColor.B * 0.08f));
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // Fluent2 selected: Increase opacity/brightness 14%
                    int sR = Math.Min(255, baseColor.R + (int)(255 * 0.14f));
                    int sG = Math.Min(255, baseColor.G + (int)(255 * 0.14f));
                    int sB = Math.Min(255, baseColor.B + (int)(255 * 0.14f));
                    stateColor = Color.FromArgb(baseColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // Fluent2 focused: Subtle 6% brightness increase
                    int fR = Math.Min(255, baseColor.R + (int)(255 * 0.06f));
                    int fG = Math.Min(255, baseColor.G + (int)(255 * 0.06f));
                    int fB = Math.Min(255, baseColor.B + (int)(255 * 0.06f));
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // Fluent2 disabled: 70 alpha (more transparent)
                    stateColor = Color.FromArgb(70, baseColor);
                    break;
                default: // Normal
                    stateColor = baseColor;
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
