using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// macOS Big Sur background painter - Vertical gradient (5% lighter at top)
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class MacOSBigSurBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // macOS Big Sur: Refined, subtle vibrancy with vertical gradient
            Color baseColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.MacOSBigSur);

            // MacOSBigSur-specific state handling - NO HELPER FUNCTIONS
            // Unique refined, subtle vibrancy for macOS (Apple's refined aesthetics)
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // macOS hover: Very subtle 3% lighter (refined feedback)
                    int hR = Math.Min(255, baseColor.R + (int)(255 * 0.03f));
                    int hG = Math.Min(255, baseColor.G + (int)(255 * 0.03f));
                    int hB = Math.Min(255, baseColor.B + (int)(255 * 0.03f));
                    stateColor = Color.FromArgb(baseColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // macOS pressed: 6% darker (gentle press)
                    int pR = Math.Max(0, baseColor.R - (int)(baseColor.R * 0.06f));
                    int pG = Math.Max(0, baseColor.G - (int)(baseColor.G * 0.06f));
                    int pB = Math.Max(0, baseColor.B - (int)(baseColor.B * 0.06f));
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // macOS selected: 5% lighter (subtle selection)
                    int sR = Math.Min(255, baseColor.R + (int)(255 * 0.05f));
                    int sG = Math.Min(255, baseColor.G + (int)(255 * 0.05f));
                    int sB = Math.Min(255, baseColor.B + (int)(255 * 0.05f));
                    stateColor = Color.FromArgb(baseColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // macOS focused: 2% lighter (barely noticeable, very refined)
                    int fR = Math.Min(255, baseColor.R + (int)(255 * 0.02f));
                    int fG = Math.Min(255, baseColor.G + (int)(255 * 0.02f));
                    int fB = Math.Min(255, baseColor.B + (int)(255 * 0.02f));
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // macOS disabled: 120 alpha (translucent)
                    stateColor = Color.FromArgb(120, baseColor);
                    break;
                default: // Normal
                    stateColor = baseColor;
                    break;
            }

            // Create macOS Big Sur gradient effect (5% lighter at top)
            int topR = Math.Min(255, stateColor.R + (int)(255 * 0.05f));
            int topG = Math.Min(255, stateColor.G + (int)(255 * 0.05f));
            int topB = Math.Min(255, stateColor.B + (int)(255 * 0.05f));
            Color topColor = Color.FromArgb(stateColor.A, topR, topG, topB);
            Color bottomColor = stateColor;

            RectangleF bounds = path.GetBounds();
            if (bounds.IsEmpty) return;
            var brush = PaintersFactory.GetLinearGradientBrush(bounds, topColor, bottomColor, LinearGradientMode.Vertical);
            g.FillPath(brush, path);

            // Add very subtle white overlay on hover (macOS vibrancy)
            if (state == ControlState.Hovered)
            {
                var vibrancyBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(10, Color.White));
                g.FillPath(vibrancyBrush, path);
            }
            else if (state == ControlState.Pressed)
            {
                var pressBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(15, Color.Black));
                g.FillPath(pressBrush, path);
            }
        }
    }
}
