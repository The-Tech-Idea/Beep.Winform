using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Tailwind Card background painter - Solid with 5% darker bottom gradient
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class TailwindCardBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Tailwind Card: Utility-first card with shadow elevation feedback
            Color baseColor = StyleColors.GetBackground(BeepControlStyle.TailwindCard);

            // TailwindCard-specific state handling - NO HELPER FUNCTIONS
            // Unique shadow elevation + brightness for Tailwind utility-first design
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Tailwind hover: 7% lighter (elevation lift)
                    int hR = Math.Min(255, baseColor.R + (int)(255 * 0.07f));
                    int hG = Math.Min(255, baseColor.G + (int)(255 * 0.07f));
                    int hB = Math.Min(255, baseColor.B + (int)(255 * 0.07f));
                    stateColor = Color.FromArgb(baseColor.A, hR, hG, hB);
                    break;
                case ControlState.Pressed:
                    // Tailwind pressed: 5% darker (shadow depth)
                    int pR = Math.Max(0, baseColor.R - (int)(baseColor.R * 0.05f));
                    int pG = Math.Max(0, baseColor.G - (int)(baseColor.G * 0.05f));
                    int pB = Math.Max(0, baseColor.B - (int)(baseColor.B * 0.05f));
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    break;
                case ControlState.Selected:
                    // Tailwind selected: 10% lighter (clear selection)
                    int sR = Math.Min(255, baseColor.R + (int)(255 * 0.10f));
                    int sG = Math.Min(255, baseColor.G + (int)(255 * 0.10f));
                    int sB = Math.Min(255, baseColor.B + (int)(255 * 0.10f));
                    stateColor = Color.FromArgb(baseColor.A, sR, sG, sB);
                    break;
                case ControlState.Focused:
                    // Tailwind focused: 5% lighter (focus ring)
                    int fR = Math.Min(255, baseColor.R + (int)(255 * 0.05f));
                    int fG = Math.Min(255, baseColor.G + (int)(255 * 0.05f));
                    int fB = Math.Min(255, baseColor.B + (int)(255 * 0.05f));
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    break;
                case ControlState.Disabled:
                    // Tailwind disabled: 100 alpha (muted)
                    stateColor = Color.FromArgb(100, baseColor);
                    break;
                default: // Normal
                    stateColor = baseColor;
                    break;
            }

            // Create Tailwind Card gradient (5% darker at bottom for depth)
            int bottomR = Math.Max(0, stateColor.R - (int)(stateColor.R * 0.05f));
            int bottomG = Math.Max(0, stateColor.G - (int)(stateColor.G * 0.05f));
            int bottomB = Math.Max(0, stateColor.B - (int)(stateColor.B * 0.05f));
            Color bottomColor = Color.FromArgb(stateColor.A, bottomR, bottomG, bottomB);

            RectangleF bounds = path.GetBounds();
            using (var brush = new LinearGradientBrush(bounds, stateColor, bottomColor, 90f))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
