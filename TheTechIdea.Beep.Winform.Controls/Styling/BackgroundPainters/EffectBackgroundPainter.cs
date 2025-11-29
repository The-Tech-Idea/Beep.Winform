using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BackgroundPainters
{
    /// <summary>
    /// Effect background painter - dynamic visual effects style
    /// Rich gradients with vibrant color shifts and multi-stop blends
    /// </summary>
    public static class EffectBackgroundPainter
    {
        public static void Paint(Graphics g, GraphicsPath path,
            BeepControlStyle style, IBeepTheme theme, bool useThemeColors,
            ControlState state = ControlState.Normal)
        {
            if (g == null || path == null) return;

            // Effect: base color from theme or style
            Color baseColor = useThemeColors && theme != null 
                ? theme.BackColor 
                : StyleColors.GetBackground(BeepControlStyle.Effect);

            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0) return;

            // Effect-specific dynamic color shifts based on state
            Color stateColor, gradientEndColor;
            GetEffectColors(baseColor, state, theme, useThemeColors, out stateColor, out gradientEndColor);

            // Rich diagonal gradient with multiple color stops
            var brush = PaintersFactory.GetLinearGradientBrush(
                bounds, stateColor, gradientEndColor, LinearGradientMode.ForwardDiagonal);

            // Multi-stop gradient for complex effect
            try
            {
                var blend = new ColorBlend
                {
                    Colors = new Color[]
                    {
                        stateColor,
                        BackgroundPainterHelpers.BlendColors(stateColor, gradientEndColor, 0.5f),
                        gradientEndColor,
                        Color.FromArgb(
                            gradientEndColor.A,
                            Math.Max(0, gradientEndColor.R - 20),
                            Math.Max(0, gradientEndColor.G - 10),
                            Math.Min(255, gradientEndColor.B + 20))
                    },
                    Positions = new float[] { 0.0f, 0.4f, 0.7f, 1.0f }
                };
                brush.InterpolationColors = blend;
            }
            catch { /* Fallback to linear gradient */ }

            g.FillPath(brush, path);

            // Subtle glow overlay for Selected/Focused states
            if (state == ControlState.Selected || state == ControlState.Focused)
            {
                var glowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(20, stateColor));
                g.FillPath(glowBrush, path);
            }
        }

        private static void GetEffectColors(Color baseColor, ControlState state, 
            IBeepTheme theme, bool useThemeColors, out Color stateColor, out Color gradientEndColor)
        {
            switch (state)
            {
                case ControlState.Hovered:
                    stateColor = BackgroundPainterHelpers.Lighten(baseColor, 0.20f);
                    gradientEndColor = Color.FromArgb(baseColor.A,
                        Math.Min(255, baseColor.R + 77),
                        Math.Min(255, baseColor.G + 64),
                        Math.Min(255, baseColor.B + 90)); // Blue shift
                    break;

                case ControlState.Pressed:
                    stateColor = BackgroundPainterHelpers.Darken(baseColor, 0.15f);
                    gradientEndColor = Color.FromArgb(baseColor.A,
                        Math.Max(0, stateColor.R - 10),
                        Math.Max(0, stateColor.G - 10),
                        Math.Min(255, stateColor.B + 20));
                    break;

                case ControlState.Selected:
                    Color accentColor = useThemeColors && theme != null 
                        ? theme.AccentColor 
                        : Color.FromArgb(100, 150, 255);
                    stateColor = accentColor;
                    gradientEndColor = Color.FromArgb(accentColor.A,
                        Math.Min(255, accentColor.R + 50),
                        Math.Max(0, accentColor.G - 30),
                        accentColor.B);
                    break;

                case ControlState.Disabled:
                    int gray = (baseColor.R + baseColor.G + baseColor.B) / 3;
                    stateColor = Color.FromArgb(baseColor.A, gray, gray, gray);
                    gradientEndColor = BackgroundPainterHelpers.Darken(stateColor, 0.06f);
                    break;

                case ControlState.Focused:
                    stateColor = BackgroundPainterHelpers.Lighten(baseColor, 0.25f);
                    gradientEndColor = Color.FromArgb(baseColor.A,
                        Math.Min(255, stateColor.R + 20),
                        Math.Max(0, stateColor.G - 10),
                        Math.Min(255, stateColor.B + 40));
                    break;

                default: // Normal
                    stateColor = baseColor;
                    gradientEndColor = Color.FromArgb(baseColor.A,
                        Math.Min(255, baseColor.R + 30),
                        Math.Max(0, baseColor.G - 10),
                        Math.Min(255, baseColor.B + 40));
                    break;
            }
        }
    }
}
