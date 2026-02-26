using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Styling.BorderPainters
{
    /// <summary>
    /// Helper methods for individual border painters
    /// Provides common border painting utilities
    /// </summary>
    public static class BorderPainterHelpers
    {
        /// <summary>
        /// Paint a simple border with the given color and width
        /// </summary>
        public static void PaintSimpleBorder(Graphics g, GraphicsPath path, Color borderColor, float borderWidth, ControlState state = ControlState.Normal)
        {
            Color stateAdjustedColor = ApplyState(borderColor, state);

            if (borderWidth > 0 && stateAdjustedColor.A > 0)
            {
                float effectiveWidth = Math.Max(1f, borderWidth);
                var pen = PaintersFactory.GetPen(stateAdjustedColor, effectiveWidth);
                pen.LineJoin = LineJoin.Round;
                pen.Alignment = PenAlignment.Inset;

                var savedPixelMode = g.PixelOffsetMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                // PixelOffsetMode.Half shifts all rendering left/up by 0.5px.
                // For a 1px border at x=0, this makes pixel column 0 receive only 50% coverage —
                // making thin left/top borders appear nearly invisible.
                // PixelOffsetMode.None aligns strokes exactly to pixel columns so x=0 → full coverage.
                g.PixelOffsetMode = PixelOffsetMode.None;
                g.DrawPath(pen, path);
                g.PixelOffsetMode = savedPixelMode;
            }
        }

        /// <summary>
        /// Paint a border with a glow effect
        /// </summary>
        public static void PaintGlowBorder(Graphics g, GraphicsPath path, Color glowColor, float glowWidth, float glowIntensity = 1.0f)
        {
            if (glowWidth <= 0 || glowColor.A == 0) return;

            // Draw multiple layers to simulate glow
            // We draw from widest (faintest) to narrowest (brightest accumulation)
            int layers = 8;
            float step = glowWidth / layers;
            
            // Base alpha for each layer. 
            // If we have 8 layers overlapping at the center, total alpha will be high.
            // We want the edge to be very faint.
            int baseAlpha = (int)(40 * glowIntensity); 

            for (int i = 0; i < layers; i++)
            {
                // Width decreases from glowWidth to near 0
                float w = glowWidth - (i * step);
                if (w <= 0.5f) continue;

                int alpha = baseAlpha;
                
                using (var p = new Pen(Color.FromArgb(alpha, glowColor), w))
                {
                    p.LineJoin = LineJoin.Round;
                    p.StartCap = LineCap.Round;
                    p.EndCap = LineCap.Round;
                    g.DrawPath(p, path);
                }
            }
        }

        /// <summary>
        /// Paint an accent bar (typically on the left side for focus indication)
        /// </summary>
        public static void PaintAccentBar(Graphics g, Rectangle bounds, Color accentColor, int barWidth = 4)
        {
            if (barWidth <= 0 || accentColor.A == 0) return;

            var barRect = new Rectangle(bounds.X, bounds.Y, barWidth, bounds.Height);
            var brush = PaintersFactory.GetSolidBrush(accentColor);
            g.FillRectangle(brush, barRect);
        }

        /// <summary>
        /// Paint a ring effect (Tailwind-Style)
        /// </summary>
        public static void PaintRing(Graphics g, GraphicsPath path, Color ringColor, float ringWidth = 3f, float ringOffset = 2f)
        {
            if (ringWidth <= 0 || ringColor.A == 0) return;

            using (var ringPath = (GraphicsPath)path.Clone())
            {
                // Get path center for proper scaling around center, not origin
                RectangleF pathBounds = path.GetBounds();
                float centerX = pathBounds.X + pathBounds.Width / 2;
                float centerY = pathBounds.Y + pathBounds.Height / 2;

                using (var wideMatrix = new Matrix())
                {
                    // Translate to origin, scale, translate back - this scales around center
                    wideMatrix.Translate(-centerX, -centerY, MatrixOrder.Append);
                    wideMatrix.Scale(1 + (ringOffset / Math.Max(pathBounds.Width, 50f)), 
                                     1 + (ringOffset / Math.Max(pathBounds.Height, 50f)), MatrixOrder.Append);
                    wideMatrix.Translate(centerX, centerY, MatrixOrder.Append);
                    ringPath.Transform(wideMatrix);
                }

                var ringPen = PaintersFactory.GetPen(ringColor, ringWidth);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(ringPen, ringPath);
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
        /// Create a color with a specific alpha value, but return Color.Empty if original color is Color.Empty
        /// This is useful when falling back to Color.Empty for theme-missing colors so the result remains empty instead of a gray/black overlay.
        /// </summary>
        public static Color WithAlphaIfNotEmpty(Color color, int alpha)
        {
            if (color == Color.Empty) return Color.Empty;
            return WithAlpha(color, alpha);
        }

        /// <summary>
        /// Blend two colors together by a specified amount
        /// </summary>
        /// <param name="baseColor">The base color</param>
        /// <param name="blendColor">The color to blend with</param>
        /// <param name="blendAmount">Amount of blend color (0.0 = all base, 1.0 = all blend)</param>
        public static Color BlendColors(Color baseColor, Color blendColor, float blendAmount)
        {
            blendAmount = Math.Max(0f, Math.Min(1f, blendAmount));
            int r = (int)(baseColor.R * (1 - blendAmount) + blendColor.R * blendAmount);
            int g = (int)(baseColor.G * (1 - blendAmount) + blendColor.G * blendAmount);
            int b = (int)(baseColor.B * (1 - blendAmount) + blendColor.B * blendAmount);
            int a = (int)(baseColor.A * (1 - blendAmount) + blendColor.A * blendAmount);
            return Color.FromArgb(a, r, g, b);
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
        /// Get color from Style or theme
        /// </summary>
        public static Color GetColorFromStyleOrTheme(IBeepTheme theme, bool useThemeColors, string themeColorKey, Color fallbackColor)
        {
            Color resolvedColor = fallbackColor;
            if (useThemeColors && theme != null)
            {
                var themeColor = BeepStyling.GetThemeColor(theme, themeColorKey);
                if (themeColor != Color.Empty)
                    resolvedColor = themeColor;
            }

            // Border visibility guard: ensure normal-state edge color stays readable.
            if (string.Equals(themeColorKey, "Border", System.StringComparison.OrdinalIgnoreCase))
            {
                resolvedColor = EnsureVisibleBorderColor(resolvedColor, theme, ControlState.Normal);
            }
            return resolvedColor;
        }

        /// <summary>
        /// Normalizes border visibility so idle borders remain readable against the background.
        /// Focus/selected states still get stronger emphasis, but normal state should never disappear.
        /// </summary>
        public static Color EnsureVisibleBorderColor(Color borderColor, IBeepTheme theme, ControlState state = ControlState.Normal)
        {
            if (borderColor == Color.Empty)
                return borderColor;

            int minAlpha = state switch
            {
                ControlState.Disabled => 70,
                ControlState.Focused => 200,
                ControlState.Selected => 180,
                ControlState.Pressed => 160,
                ControlState.Hovered => 140,
                _ => 120
            };

            int targetAlpha = Math.Max(minAlpha, borderColor.A);
            Color candidate = Color.FromArgb(255, borderColor.R, borderColor.G, borderColor.B);

            if (theme != null)
            {
                Color back = theme.BackColor;
                float targetContrast = state switch
                {
                    ControlState.Focused => 1.8f,
                    ControlState.Selected => 1.7f,
                    _ => 1.6f
                };

                float contrast = ColorAccessibilityHelper.CalculateContrastRatio(candidate, back);
                bool backgroundIsDark = ColorAccessibilityHelper.CalculateRelativeLuminance(back) < 0.5;

                int guard = 0;
                while (contrast < targetContrast && guard < 8)
                {
                    candidate = backgroundIsDark ? Lighten(candidate, 0.12f) : Darken(candidate, 0.12f);
                    contrast = ColorAccessibilityHelper.CalculateContrastRatio(candidate, back);
                    guard++;
                }
            }

            return Color.FromArgb(targetAlpha, candidate.R, candidate.G, candidate.B);
        }

        /// <summary>
        /// Creates a standardized inner path for centered border strokes.
        /// Uses half the stroke width as the default inset so the visible border
        /// remains aligned and consistent across style painters.
        /// </summary>
        public static GraphicsPath CreateStrokeInsetPath(GraphicsPath path, float strokeWidth, float extraInset = 0f)
        {
            if (path == null)
                return null;

            float safeWidth = Math.Max(0f, strokeWidth);
            float safeExtra = Math.Max(0f, extraInset);
            float inset = safeWidth + safeExtra;

            if (inset <= 0f)
                return path;

            int detectedRadius = (int)GraphicsExtensions.DetectRadiusFromRoundedRectPath(path);
            return path.CreateInsetPath(inset, detectedRadius);
        }

        #region Enhanced Focus Indicators

        /// <summary>
        /// Focus ring style options
        /// </summary>
        public enum FocusRingStyle
        {
            Outline,    // Simple outline border
            Glow,       // Glowing effect around control
            Inset       // Inset focus ring
        }

        /// <summary>
        /// Paint a modern focus ring with multiple style options
        /// Ensures 2px minimum width for accessibility compliance
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="path">Control path</param>
        /// <param name="focusColor">Focus ring color (typically primary color)</param>
        /// <param name="width">Focus ring width (minimum 2px for accessibility)</param>
        /// <param name="style">Focus ring style</param>
        public static void PaintFocusRing(Graphics g, GraphicsPath path, 
            Color focusColor, float width = 2f, FocusRingStyle style = FocusRingStyle.Outline)
        {
            if (g == null || path == null || width <= 0) return;

            // Ensure minimum 2px width for accessibility
            float effectiveWidth = Math.Max(2f, width);

            g.SmoothingMode = SmoothingMode.AntiAlias;

            switch (style)
            {
                case FocusRingStyle.Outline:
                    PaintOutlineFocusRing(g, path, focusColor, effectiveWidth);
                    break;
                case FocusRingStyle.Glow:
                    PaintGlowFocusRing(g, path, focusColor, effectiveWidth);
                    break;
                case FocusRingStyle.Inset:
                    PaintInsetFocusRing(g, path, focusColor, effectiveWidth);
                    break;
            }
        }

        /// <summary>
        /// Paint outline-style focus ring (standard)
        /// </summary>
        private static void PaintOutlineFocusRing(Graphics g, GraphicsPath path, Color focusColor, float width)
        {
            var pen = PaintersFactory.GetPen(focusColor, width);
            pen.LineJoin = LineJoin.Round;
            g.DrawPath(pen, path);
        }

        /// <summary>
        /// Paint glow-style focus ring (modern, prominent)
        /// </summary>
        private static void PaintGlowFocusRing(Graphics g, GraphicsPath path, Color focusColor, float width)
        {
            // Draw multiple layers for glow effect
            int layers = 4;
            for (int i = layers; i > 0; i--)
            {
                float layerWidth = width + (i * 1.5f);
                int alpha = (int)(255 * (1.0f - (float)(layers - i) / layers));
                alpha = Math.Max(30, alpha); // Minimum visibility

                using (var pen = new Pen(Color.FromArgb(alpha, focusColor), layerWidth))
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, path);
                }
            }
        }

        /// <summary>
        /// Paint inset-style focus ring (subtle, inside border)
        /// </summary>
        private static void PaintInsetFocusRing(Graphics g, GraphicsPath path, Color focusColor, float width)
        {
            // Create inset path using extension method
            var insetPath = path.CreateInsetPath(width);
            if (insetPath != null && insetPath.PointCount > 0)
            {
                try
                {
                    var pen = PaintersFactory.GetPen(focusColor, width);
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, insetPath);
                }
                finally
                {
                    insetPath?.Dispose();
                }
            }
        }

        #endregion
    }
}
