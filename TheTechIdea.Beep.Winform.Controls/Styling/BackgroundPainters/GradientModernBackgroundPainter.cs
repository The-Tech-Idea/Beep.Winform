using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Gradient Modern background painter - Vertical gradient from primary to 30% darker
    /// Supports: Normal, Hovered, Pressed, Selected, Disabled, Focused states
    /// </summary>
    public static class GradientModernBackgroundPainter
    {
        public static void Paint(Graphics g, Rectangle bounds, GraphicsPath path, 
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Gradient Modern: Smooth vertical gradient with state-aware intensity
            Color primaryColor = useThemeColors ? theme.PrimaryColor : StyleColors.GetBackground(BeepControlStyle.GradientModern);

            // GradientModern-specific state handling - NO HELPER FUNCTIONS
            // Unique gradient intensity modulation for modern gradient design
            float gradientIntensity;
            switch (state)
            {
                case ControlState.Hovered:
                    // GradientModern hover: Intensify gradient 15%
                    gradientIntensity = 1.15f;
                    break;
                case ControlState.Pressed:
                    // GradientModern pressed: Reduce gradient 10% (flatten)
                    gradientIntensity = 0.90f;
                    break;
                case ControlState.Selected:
                    // GradientModern selected: Bold gradient 25% increase
                    gradientIntensity = 1.25f;
                    break;
                case ControlState.Focused:
                    // GradientModern focused: Subtle gradient 8% increase
                    gradientIntensity = 1.08f;
                    break;
                case ControlState.Disabled:
                    // GradientModern disabled: Dim gradient 40%
                    gradientIntensity = 0.60f;
                    break;
                default: // Normal
                    gradientIntensity = 1.0f;
                    break;
            }

            // Apply intensity to primary color
            int intensifiedR = Math.Min(255, Math.Max(0, (int)(primaryColor.R * gradientIntensity)));
            int intensifiedG = Math.Min(255, Math.Max(0, (int)(primaryColor.G * gradientIntensity)));
            int intensifiedB = Math.Min(255, Math.Max(0, (int)(primaryColor.B * gradientIntensity)));
            Color gradientStart = Color.FromArgb(primaryColor.A, intensifiedR, intensifiedG, intensifiedB);

            // Create gradient end color (30% darker than start for modern depth)
            int endR = Math.Max(0, (int)(gradientStart.R * 0.70f));
            int endG = Math.Max(0, (int)(gradientStart.G * 0.70f));
            int endB = Math.Max(0, (int)(gradientStart.B * 0.70f));
            Color gradientEnd = Color.FromArgb(primaryColor.A, endR, endG, endB);

            using (var brush = new LinearGradientBrush(bounds, gradientStart, gradientEnd, 90f))
            {
                if (path != null)
                    g.FillPath(brush, path);
                else
                    g.FillRectangle(brush, bounds);
            }
        }
    }
}
