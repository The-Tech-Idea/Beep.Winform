using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Material You background painter - Solid with 8% tonal primary highlight
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class MaterialYouBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Material You: Dynamic color with personalized primary tinting
            Color baseColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.MaterialYou);
            Color primaryColor = useThemeColors ? theme.PrimaryColor : StyleColors.GetPrimary(BeepControlStyle.MaterialYou);

            // MaterialYou-specific state handling - NO HELPER FUNCTIONS
            // Unique dynamic color tinting with primary color for Material You
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // MaterialYou hover: Blend 10% primary color (personalized feedback)
                    int hR = (int)(baseColor.R * 0.90f + primaryColor.R * 0.10f);
                    int hG = (int)(baseColor.G * 0.90f + primaryColor.G * 0.10f);
                    int hB = (int)(baseColor.B * 0.90f + primaryColor.B * 0.10f);
                    stateColor = Color.FromArgb(baseColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // MaterialYou pressed: 8% darker
                    int pR = Math.Max(0, baseColor.R - (int)(baseColor.R * 0.08f));
                    int pG = Math.Max(0, baseColor.G - (int)(baseColor.G * 0.08f));
                    int pB = Math.Max(0, baseColor.B - (int)(baseColor.B * 0.08f));
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // MaterialYou selected: Blend 15% primary color (bold personalization)
                    int sR = (int)(baseColor.R * 0.85f + primaryColor.R * 0.15f);
                    int sG = (int)(baseColor.G * 0.85f + primaryColor.G * 0.15f);
                    int sB = (int)(baseColor.B * 0.85f + primaryColor.B * 0.15f);
                    stateColor = Color.FromArgb(baseColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // MaterialYou focused: Blend 7% primary color
                    int fR = (int)(baseColor.R * 0.93f + primaryColor.R * 0.07f);
                    int fG = (int)(baseColor.G * 0.93f + primaryColor.G * 0.07f);
                    int fB = (int)(baseColor.B * 0.93f + primaryColor.B * 0.07f);
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // MaterialYou disabled: 90 alpha
                    stateColor = Color.FromArgb(90, baseColor);
                    break;
                default: // Normal
                    stateColor = baseColor;
                    break;
            }

            using (var brush = new SolidBrush(stateColor))
            {
                g.FillPath(brush, path);
            }

            // Add tonal primary highlight (8% alpha) - consistent across states
            Color tonalColor = Color.FromArgb(20, primaryColor);
            using (var brush = new SolidBrush(tonalColor))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
