using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Metro background painter - Windows Modern UI flat tile design
    /// Flat gray background with bold accent colors, no gradients
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class MetroBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Metro: Flat design with bold color changes (no gradients, no subtle transitions)
            Color backgroundColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Metro);

            // Metro-specific state handling - Bold, flat color changes
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Metro hover: Darken by 10% (noticeable flat change)
                    int hR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.10f));
                    int hG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.10f));
                    int hB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.10f));
                    stateColor = Color.FromArgb(backgroundColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // Metro pressed: Darken by 20% (bold flat change)
                    int pR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.20f));
                    int pG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.20f));
                    int pB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.20f));
                    stateColor = Color.FromArgb(backgroundColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // Metro selected: Use primary color (bold accent)
                    stateColor = useThemeColors ? theme.AccentColor : StyleColors.GetPrimary(BeepControlStyle.Metro);
                    break;
                case ControlState.Focused:
                    // Metro focused: Lighten by 5% (subtle indication)
                    int fR = Math.Min(255, backgroundColor.R + (int)(255 * 0.05f));
                    int fG = Math.Min(255, backgroundColor.G + (int)(255 * 0.05f));
                    int fB = Math.Min(255, backgroundColor.B + (int)(255 * 0.05f));
                    stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // Metro disabled: 100 alpha (translucent flat)
                    stateColor = Color.FromArgb(100, backgroundColor);
                    break;
                default: // Normal
                    stateColor = backgroundColor;
                    break;
            }

            // Fill with flat color (no gradients - Metro signature)
            using (var brush = new SolidBrush(stateColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
