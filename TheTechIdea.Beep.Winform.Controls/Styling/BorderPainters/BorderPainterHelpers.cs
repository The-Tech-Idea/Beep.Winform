using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Helper methods for individual border painters
    /// Provides common border painting utilities
    /// </summary>
    public static class BorderPainterHelpers
    {
        /// <summary>
        /// Control state for border painting
        /// </summary>
        public enum ControlState
        {
            Normal,
            Hovered,
            Pressed,
            Selected,
            Disabled,
            Focused
        }

        /// <summary>
        /// Paint a simple border with the given color and width
        /// </summary>
        public static void PaintSimpleBorder(Graphics g, GraphicsPath path, Color borderColor, float borderWidth, ControlState state = ControlState.Normal)
        {
            Color stateAdjustedColor = ApplyState(borderColor, state);
            
            if (borderWidth > 0 && stateAdjustedColor.A > 0)
            {
                using (var pen = new Pen(stateAdjustedColor, borderWidth))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawPath(pen, path);
                }
            }
        }

        /// <summary>
        /// Paint a border with a glow effect
        /// </summary>
        public static void PaintGlowBorder(Graphics g, GraphicsPath path, Color glowColor, float glowWidth, float glowIntensity = 1.0f)
        {
            if (glowWidth <= 0 || glowColor.A == 0) return;

            int glowAlpha = (int)(glowColor.A * glowIntensity);
            Color adjustedGlow = Color.FromArgb(Math.Max(0, Math.Min(255, glowAlpha)), glowColor);

            using (var glowPen = new Pen(adjustedGlow, glowWidth))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(glowPen, path);
            }
        }

        /// <summary>
        /// Paint an accent bar (typically on the left side for focus indication)
        /// </summary>
        public static void PaintAccentBar(Graphics g, Rectangle bounds, Color accentColor, int barWidth = 4)
        {
            if (barWidth <= 0 || accentColor.A == 0) return;

            var barRect = new Rectangle(bounds.X, bounds.Y, barWidth, bounds.Height);
            using (var brush = new SolidBrush(accentColor))
            {
                g.FillRectangle(brush, barRect);
            }
        }

        /// <summary>
        /// Paint a ring effect (Tailwind-style)
        /// </summary>
        public static void PaintRing(Graphics g, GraphicsPath path, Color ringColor, float ringWidth = 3f, float ringOffset = 2f)
        {
            if (ringWidth <= 0 || ringColor.A == 0) return;

            using (var ringPath = (GraphicsPath)path.Clone())
            {
                // Widen the path to create the ring effect
                using (var wideMatrix = new Matrix())
                {
                    wideMatrix.Scale(1 + (ringOffset / 100f), 1 + (ringOffset / 100f));
                    ringPath.Transform(wideMatrix);
                }

                using (var ringPen = new Pen(ringColor, ringWidth))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawPath(ringPen, ringPath);
                }
            }
        }

        /// <summary>
        /// Apply state modifications to a color
        /// </summary>
        public static Color ApplyState(Color baseColor, ControlState state)
        {
            return state switch
            {
                ControlState.Hovered => Lighten(baseColor, 0.1f),
                ControlState.Pressed => Darken(baseColor, 0.15f),
                ControlState.Selected => Lighten(baseColor, 0.12f),
                ControlState.Disabled => WithAlpha(baseColor, 80),
                ControlState.Focused => Lighten(baseColor, 0.05f),
                _ => baseColor
            };
        }

        /// <summary>
        /// Lighten a color by a percentage
        /// </summary>
        public static Color Lighten(Color color, float percent)
        {
            int r = Math.Min(255, color.R + (int)((255 - color.R) * percent));
            int g = Math.Min(255, color.G + (int)((255 - color.G) * percent));
            int b = Math.Min(255, color.B + (int)((255 - color.B) * percent));
            return Color.FromArgb(color.A, r, g, b);
        }

        /// <summary>
        /// Darken a color by a percentage
        /// </summary>
        public static Color Darken(Color color, float percent)
        {
            int r = Math.Max(0, color.R - (int)(color.R * percent));
            int g = Math.Max(0, color.G - (int)(color.G * percent));
            int b = Math.Max(0, color.B - (int)(color.B * percent));
            return Color.FromArgb(color.A, r, g, b);
        }

        /// <summary>
        /// Create a color with a specific alpha value
        /// </summary>
        public static Color WithAlpha(Color color, int alpha)
        {
            return Color.FromArgb(Math.Max(0, Math.Min(255, alpha)), color.R, color.G, color.B);
        }

        /// <summary>
        /// Create a color from RGB with alpha
        /// </summary>
        public static Color WithAlpha(int r, int g, int b, int alpha)
        {
            return Color.FromArgb(
                Math.Max(0, Math.Min(255, alpha)),
                Math.Max(0, Math.Min(255, r)),
                Math.Max(0, Math.Min(255, g)),
                Math.Max(0, Math.Min(255, b))
            );
        }

        /// <summary>
        /// Get color from style or theme
        /// </summary>
        public static Color GetColorFromStyleOrTheme(IBeepTheme theme, bool useThemeColors, string themeColorKey, Color fallbackColor)
        {
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(themeColorKey);
                if (themeColor != Color.Empty)
                    return themeColor;
            }
            return fallbackColor;
        }
    }
}
