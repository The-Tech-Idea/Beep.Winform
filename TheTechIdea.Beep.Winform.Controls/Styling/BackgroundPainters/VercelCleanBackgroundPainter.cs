using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Vercel Clean background painter - Pure white solid
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class VercelCleanBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Vercel Clean: Pure clean white with subtle feedback
            Color backgroundColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.VercelClean);

            // VercelClean-specific state handling - NO HELPER FUNCTIONS
            // Unique clean 3% hover for Vercel's pristine design
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Vercel hover: Clean 3% lighter
                    int hR = Math.Min(255, backgroundColor.R + (int)(255 * 0.03f));
                    int hG = Math.Min(255, backgroundColor.G + (int)(255 * 0.03f));
                    int hB = Math.Min(255, backgroundColor.B + (int)(255 * 0.03f));
                    stateColor = Color.FromArgb(backgroundColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // Vercel pressed: 4% darker (clean press)
                    int pR = Math.Max(0, backgroundColor.R - (int)(backgroundColor.R * 0.04f));
                    int pG = Math.Max(0, backgroundColor.G - (int)(backgroundColor.G * 0.04f));
                    int pB = Math.Max(0, backgroundColor.B - (int)(backgroundColor.B * 0.04f));
                    stateColor = Color.FromArgb(backgroundColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // Vercel selected: 5% lighter (clear selection)
                    int sR = Math.Min(255, backgroundColor.R + (int)(255 * 0.05f));
                    int sG = Math.Min(255, backgroundColor.G + (int)(255 * 0.05f));
                    int sB = Math.Min(255, backgroundColor.B + (int)(255 * 0.05f));
                    stateColor = Color.FromArgb(backgroundColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // Vercel focused: 2% lighter (clean focus)
                    int fR = Math.Min(255, backgroundColor.R + (int)(255 * 0.02f));
                    int fG = Math.Min(255, backgroundColor.G + (int)(255 * 0.02f));
                    int fB = Math.Min(255, backgroundColor.B + (int)(255 * 0.02f));
                    stateColor = Color.FromArgb(backgroundColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // Vercel disabled: 110 alpha (clean translucency)
                    stateColor = Color.FromArgb(110, backgroundColor);
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
