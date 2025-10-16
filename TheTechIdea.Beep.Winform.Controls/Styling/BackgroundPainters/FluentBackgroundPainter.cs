using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Fluent (legacy) background painter - Microsoft Fluent Design System
    /// Fluent UX: Acrylic materials, subtle transparency, depth through layering
    /// </summary>
    public static class FluentBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            // Fluent style: Semi-transparent with subtle lighting
            Color baseColor = useThemeColors ? theme.BackColor : StyleColors.GetBackground(BeepControlStyle.Fluent);

            // Fluent-specific state handling - subtle reveals
            Color stateColor;
            switch (state)
            {
                case ControlState.Hovered:
                    // Fluent reveal: Subtle white overlay (10% reveal)
                    stateColor = Color.FromArgb(
                        baseColor.A,
                        Math.Min(255, baseColor.R + 26),
                        Math.Min(255, baseColor.G + 26),
                        Math.Min(255, baseColor.B + 26)
                    );
                    break;

                case ControlState.Pressed:
                    // Fluent pressed: Slightly darker (8%)
                    int pR = Math.Max(0, baseColor.R - (int)(baseColor.R * 0.08f));
                    int pG = Math.Max(0, baseColor.G - (int)(baseColor.G * 0.08f));
                    int pB = Math.Max(0, baseColor.B - (int)(baseColor.B * 0.08f));
                    stateColor = Color.FromArgb(baseColor.A, pR, pG, pB);
                    break;

                case ControlState.Selected:
                    // Fluent selected: Accent color with transparency
                    Color accentColor = useThemeColors ? theme.AccentColor : Color.FromArgb(0, 120, 212);
                    stateColor = Color.FromArgb(
                        200, // Semi-transparent
                        accentColor.R,
                        accentColor.G,
                        accentColor.B
                    );
                    break;

                case ControlState.Disabled:
                    // Fluent disabled: Lower opacity
                    stateColor = Color.FromArgb(80, baseColor.R, baseColor.G, baseColor.B);
                    break;

                case ControlState.Focused:
                    // Fluent focused: Subtle reveal (12%)
                    int fR = Math.Min(255, baseColor.R + 31);
                    int fG = Math.Min(255, baseColor.G + 31);
                    int fB = Math.Min(255, baseColor.B + 31);
                    stateColor = Color.FromArgb(baseColor.A, fR, fG, fB);
                    break;

                default: // Normal
                    stateColor = baseColor;
                    break;
            }

            // Fluent style: Subtle acrylic-like gradient
            var bounds = path.GetBounds();
            using (var brush = new LinearGradientBrush(
                new PointF(bounds.Left, bounds.Top),
                new PointF(bounds.Right, bounds.Bottom),
                stateColor,
                Color.FromArgb(
                    stateColor.A,
                    Math.Max(0, stateColor.R - 8),
                    Math.Max(0, stateColor.G - 8),
                    Math.Max(0, stateColor.B - 8)
                )))
            {
                // Add acrylic noise effect
                ColorBlend blend = new ColorBlend();
                blend.Colors = new Color[]
                {
                    stateColor,
                    Color.FromArgb(stateColor.A, Math.Min(255, stateColor.R + 5), Math.Min(255, stateColor.G + 5), Math.Min(255, stateColor.B + 5)),
                    stateColor
                };
                blend.Positions = new float[] { 0.0f, 0.5f, 1.0f };
                brush.InterpolationColors = blend;

                g.FillPath(brush, path);
            }
        }
    }
}
