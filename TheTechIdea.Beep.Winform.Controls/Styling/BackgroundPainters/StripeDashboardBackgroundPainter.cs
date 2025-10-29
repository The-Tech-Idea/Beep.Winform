using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Stripe Dashboard background painter - Solid with3% lighter top gradient
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class StripeDashboardBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Stripe Dashboard: Professional, refined, subtle feedback
            Color baseColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.StripeDashboard);

            // StripeDashboard-specific state handling - NO HELPER FUNCTIONS
            // Unique professional, refined, very subtle business-focused feedback
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Stripe hover: Very professional subtle4% lighter
                    int hR = Math.Min(255, baseColor.R + (int)(255 *0.04f));
                    int hG = Math.Min(255, baseColor.G + (int)(255 *0.04f));
                    int hB = Math.Min(255, baseColor.B + (int)(255 *0.04f));
                    stateColor = Color.FromArgb(baseColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // Stripe pressed: Gentle6% darker (refined press)
                    int pR = Math.Max(0, baseColor.R - (int)(baseColor.R *0.06f));
                    int pG = Math.Max(0, baseColor.G - (int)(baseColor.G *0.06f));
                    int pB = Math.Max(0, baseColor.B - (int)(baseColor.B *0.06f));
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // Stripe selected: Noticeable but refined7% lighter
                    int sR = Math.Min(255, baseColor.R + (int)(255 *0.07f));
                    int sG = Math.Min(255, baseColor.G + (int)(255 *0.07f));
                    int sB = Math.Min(255, baseColor.B + (int)(255 *0.07f));
                    stateColor = Color.FromArgb(baseColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // Stripe focused: Barely noticeable3% lighter (focus ring)
                    int fR = Math.Min(255, baseColor.R + (int)(255 *0.03f));
                    int fG = Math.Min(255, baseColor.G + (int)(255 *0.03f));
                    int fB = Math.Min(255, baseColor.B + (int)(255 *0.03f));
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // Stripe disabled: Professional110 alpha
                    stateColor = Color.FromArgb(110, baseColor);
                    break;
                default: // Normal
                    stateColor = baseColor;
                    break;
            }

            // Create Stripe Dashboard gradient (3% lighter at top for professional polish)
            int topR = Math.Min(255, stateColor.R + (int)(255 *0.03f));
            int topG = Math.Min(255, stateColor.G + (int)(255 *0.03f));
            int topB = Math.Min(255, stateColor.B + (int)(255 *0.03f));
            Color topColor = Color.FromArgb(stateColor.A, topR, topG, topB);

            RectangleF bounds = path.GetBounds();
            var brush = PaintersFactory.GetLinearGradientBrush(bounds, topColor, stateColor, LinearGradientMode.Vertical);
            g.FillPath(brush, path);
        }
    }
}
