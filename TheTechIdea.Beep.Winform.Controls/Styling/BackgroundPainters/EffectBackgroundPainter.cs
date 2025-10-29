using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Effect background painter - Dynamic visual effects Style
    /// Effect UX: Gradients, glows, animated appearance (static snapshot)
    /// </summary>
    public static class EffectBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Effect Style: Rich gradients with vibrant color shifts
            Color baseColor = useThemeColors && theme != null ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Effect);

            // Effect-specific state handling - dynamic color shifts
            Color stateColor;
            Color gradientEndColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Effect hover: Vibrant color boost (20%)
                    stateColor = Color.FromArgb(
                        baseColor.A,
                        Math.Min(255, baseColor.R + 51),
                        Math.Min(255, baseColor.G + 51),
                        Math.Min(255, baseColor.B + 51)
                    );
                    gradientEndColor = Color.FromArgb(
                        baseColor.A,
                        Math.Min(255, baseColor.R + 77),
                        Math.Min(255, baseColor.G + 64),
                        Math.Min(255, baseColor.B + 90) // Blue shift
                    );
                    break;

                case ControlState.Pressed:
                    // Effect pressed: Darker with color shift (15%)
                    int pR = Math.Max(0, baseColor.R - (int)(baseColor.R * 0.15f));
                    int pG = Math.Max(0, baseColor.G - (int)(baseColor.G * 0.15f));
                    int pB = Math.Max(0, baseColor.B - (int)(baseColor.B * 0.10f)); // Less blue reduction
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    gradientEndColor = Color.FromArgb(baseColor.A, Math.Max(0, pR - 10), Math.Max(0, pG - 10), Math.Min(255, pB + 20));
                    break;

                case ControlState.Selected:
                    // Effect selected: Vivid accent gradient
                    Color accentColor = useThemeColors && theme != null ? theme.AccentColor : Color.FromArgb(100, 150, 255);
                    stateColor = accentColor;
                    gradientEndColor = Color.FromArgb(
                        accentColor.A,
                        Math.Min(255, accentColor.R + 50),
                        Math.Max(0, accentColor.G - 30),
                        accentColor.B
                    );
                    break;

                case ControlState.Disabled:
                    // Effect disabled: Desaturated gray with subtle gradient
                    int gray = (baseColor.R + baseColor.G + baseColor.B) / 3;
                    stateColor = Color.FromArgb(baseColor.A, gray, gray, gray);
                    gradientEndColor = Color.FromArgb(baseColor.A, Math.Max(0, gray - 15), Math.Max(0, gray - 15), Math.Max(0, gray - 15));
                    break;

                case ControlState.Focused:
                    // Effect focused: Energetic gradient (25%)
                    int fR = Math.Min(255, baseColor.R + 64);
                    int fG = Math.Min(255, baseColor.G + 64);
                    int fB = Math.Min(255, baseColor.B + 80); // Extra blue
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    gradientEndColor = Color.FromArgb(baseColor.A, Math.Min(255, fR + 20), Math.Max(0, fG - 10), Math.Min(255, fB + 40));
                    break;

                default: // Normal
                    stateColor = baseColor;
                    gradientEndColor = Color.FromArgb(
                        baseColor.A,
                        Math.Min(255, baseColor.R + 30),
                        Math.Max(0, baseColor.G - 10),
                        Math.Min(255, baseColor.B + 40)
                    );
                    break;
            }

            // Effect Style: Rich diagonal gradient with multiple color stops
            var bounds = path.GetBounds();
            var brush = PaintersFactory.GetLinearGradientBrush(bounds, stateColor, gradientEndColor, LinearGradientMode.ForwardDiagonal);

            // Add multi-stop gradient for complex effect
            ColorBlend blend = new ColorBlend();
            blend.Colors = new Color[]
            {
                stateColor,
                Color.FromArgb(
                    stateColor.A,
                    (stateColor.R + gradientEndColor.R) / 2,
                    (stateColor.G + gradientEndColor.G) / 2,
                    (stateColor.B + gradientEndColor.B) / 2
                ),
                gradientEndColor,
                Color.FromArgb(
                    gradientEndColor.A,
                    Math.Max(0, gradientEndColor.R - 20),
                    Math.Max(0, gradientEndColor.G - 10),
                    Math.Min(255, gradientEndColor.B + 20)
                )
            };
            blend.Positions = new float[] { 0.0f, 0.4f, 0.7f, 1.0f };
            try { brush.InterpolationColors = blend; } catch { }

            g.FillPath(brush, path);

            // Optional: Add subtle glow effect for Selected/Focused states
            if (state == ControlState.Selected || state == ControlState.Focused)
            {
                var glowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(20, stateColor));
                g.FillPath(glowBrush, path);
            }
        }
    }
}
