using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Apple background painter - Clean, subtle gradients with premium feel
    /// Apple UX: Minimal, refined aesthetics with smooth state transitions
    /// </summary>
    public static class AppleBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Apple Style: Clean, subtle backgrounds with premium feel
            Color baseColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Apple);

            // Apple-specific state handling - subtle and elegant
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Apple hover: Subtle lightening (8%)
                    int hR = Math.Min(255, baseColor.R + (int)(255 * 0.08f));
                    int hG = Math.Min(255, baseColor.G + (int)(255 * 0.08f));
                    int hB = Math.Min(255, baseColor.B + (int)(255 * 0.08f));
                    stateColor = Color.FromArgb(baseColor.A, hR, hG, hB);
                    break;

                case ControlState.Pressed:
                    // Apple pressed: Slightly darker (12%)
                    int pR = Math.Max(0, baseColor.R - (int)(baseColor.R * 0.12f));
                    int pG = Math.Max(0, baseColor.G - (int)(baseColor.G * 0.12f));
                    int pB = Math.Max(0, baseColor.B - (int)(baseColor.B * 0.12f));
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    break;

                case ControlState.Selected:
                    // Apple selected: Accent color tint
                    Color accentColor = useThemeColors ? theme.AccentColor : Color.FromArgb(0, 122, 255);
                    stateColor = Color.FromArgb(
                        baseColor.A,
                        (int)(baseColor.R * 0.85f + accentColor.R * 0.15f),
                        (int)(baseColor.G * 0.85f + accentColor.G * 0.15f),
                        (int)(baseColor.B * 0.85f + accentColor.B * 0.15f)
                    );
                    break;

                case ControlState.Disabled:
                    // Apple disabled: Desaturate and lighten
                    int gray = (baseColor.R + baseColor.G + baseColor.B) / 3;
                    stateColor = Color.FromArgb(100, gray, gray, gray);
                    break;

                case ControlState.Focused:
                    // Apple focused: Slightly brighter (10%)
                    int fR = Math.Min(255, baseColor.R + (int)(255 * 0.10f));
                    int fG = Math.Min(255, baseColor.G + (int)(255 * 0.10f));
                    int fB = Math.Min(255, baseColor.B + (int)(255 * 0.10f));
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    break;

                default: // Normal
                    stateColor = baseColor;
                    break;
            }

            // Apple Style: Subtle vertical gradient for premium feel
            var bounds = path.GetBounds();
            using (var brush = new LinearGradientBrush(
                new PointF(bounds.Left, bounds.Top),
                new PointF(bounds.Left, bounds.Bottom),
                stateColor,
                Color.FromArgb(
                    stateColor.A,
                    Math.Max(0, stateColor.R - 5),
                    Math.Max(0, stateColor.G - 5),
                    Math.Max(0, stateColor.B - 5)
                )))
            {
                g.FillPath(brush, path);
            }
        }
    }
}
