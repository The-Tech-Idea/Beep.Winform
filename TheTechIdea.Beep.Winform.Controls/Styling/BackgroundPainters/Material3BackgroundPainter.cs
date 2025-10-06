using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material 3 background painter - Solid with 10% white elevation highlight
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class Material3BackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors, 
            ControlState state = ControlState.Normal)
        {
            // Material 3: Solid background with elevation-based lighting
            Color baseColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Material3);
            
            // Material3-specific state handling - NO HELPER FUNCTIONS
            // Unique elevation-based lighting percentages for Material3
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Material3 elevation lighting: 15% lighter on hover
                    int hR = Math.Min(255, baseColor.R + (int)(255 * 0.15f));
                    int hG = Math.Min(255, baseColor.G + (int)(255 * 0.15f));
                    int hB = Math.Min(255, baseColor.B + (int)(255 * 0.15f));
                    stateColor = Color.FromArgb(baseColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // Material3 pressed: 12% darker
                    int pR = Math.Max(0, baseColor.R - (int)(baseColor.R * 0.12f));
                    int pG = Math.Max(0, baseColor.G - (int)(baseColor.G * 0.12f));
                    int pB = Math.Max(0, baseColor.B - (int)(baseColor.B * 0.12f));
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // Material3 selected: 20% lighter (bold elevation)
                    int sR = Math.Min(255, baseColor.R + (int)(255 * 0.20f));
                    int sG = Math.Min(255, baseColor.G + (int)(255 * 0.20f));
                    int sB = Math.Min(255, baseColor.B + (int)(255 * 0.20f));
                    stateColor = Color.FromArgb(baseColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // Material3 focused: 8% lighter
                    int fR = Math.Min(255, baseColor.R + (int)(255 * 0.08f));
                    int fG = Math.Min(255, baseColor.G + (int)(255 * 0.08f));
                    int fB = Math.Min(255, baseColor.B + (int)(255 * 0.08f));
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // Material3 disabled: 80 alpha
                    stateColor = Color.FromArgb(80, baseColor);
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

            // Add elevation highlight (10% white overlay) - consistent across states
            Color elevationColor = Color.FromArgb(25, Color.White);
            using (var brush = new SolidBrush(elevationColor))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }
        }
    }
}
