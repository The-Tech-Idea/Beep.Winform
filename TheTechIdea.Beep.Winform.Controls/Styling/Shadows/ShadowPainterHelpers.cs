using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Helper utilities for painting shadows
    /// Provides common methods for soft shadows, material elevation, and special effects
    /// Supports advanced UX/UI shadow effects for modern interfaces
    /// </summary>
    public static class ShadowPainterHelpers
    {
        #region "Clean UX/UI Shadow Methods"

        /// <summary>
        /// Paints a clean single-layer drop shadow (GNOME/Apple/Ubuntu style)
        /// Best for: Linux desktop themes, Apple designs, minimal UIs
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Control path</param>
        /// <param name="radius">Corner radius</param>
        /// <param name="offsetX">Horizontal offset (usually 0)</param>
        /// <param name="offsetY">Vertical offset (2-6px typical)</param>
        /// <param name="shadowColor">Shadow color (use neutral black)</param>
        /// <param name="alpha">Shadow opacity (0-255, typical 30-60)</param>
        /// <param name="spread">Shadow expansion (0-4px typical)</param>
        public static GraphicsPath PaintCleanDropShadow(Graphics g, GraphicsPath bounds, int radius,
            int offsetX, int offsetY, Color shadowColor, int alpha, int spread = 2)
        {
            if (g == null || bounds == null) return bounds;
            if (alpha <= 0) return bounds;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            RectangleF boundsRect = bounds.GetBounds();
            Rectangle shadowBounds = Rectangle.Round(boundsRect);
            shadowBounds.Offset(offsetX, offsetY);
            shadowBounds.Inflate(spread, spread);

            using (var shadowPath = new GraphicsPath())
            {
                if (radius <= 0)
                {
                    shadowPath.AddRectangle(shadowBounds);
                }
                else
                {
                    int effectiveRadius = radius + spread;
                    int diameter = effectiveRadius * 2;

                    if (diameter > 0 && shadowBounds.Width > diameter && shadowBounds.Height > diameter)
                    {
                        Size cornerSize = new Size(diameter, diameter);
                        Rectangle arc = new Rectangle(shadowBounds.Location, cornerSize);

                        shadowPath.AddArc(arc, 180, 90);
                        arc.X = shadowBounds.Right - diameter;
                        shadowPath.AddArc(arc, 270, 90);
                        arc.Y = shadowBounds.Bottom - diameter;
                        shadowPath.AddArc(arc, 0, 90);
                        arc.X = shadowBounds.Left;
                        shadowPath.AddArc(arc, 90, 90);
                        shadowPath.CloseFigure();
                    }
                    else
                    {
                        shadowPath.AddRectangle(shadowBounds);
                    }
                }

                var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(alpha, shadowColor));
                g.FillPath(brush, shadowPath);
            }

            return bounds;
        }

        /// <summary>
        /// Paints a hard-edged offset shadow (Brutalist/Retro/Cartoon style)
        /// Best for: Brutalist design, retro UIs, cartoon styles
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Control path</param>
        /// <param name="offsetX">Horizontal offset (4-8px typical)</param>
        /// <param name="offsetY">Vertical offset (4-8px typical)</param>
        /// <param name="shadowColor">Shadow color (solid black typical)</param>
        public static GraphicsPath PaintHardOffsetShadow(Graphics g, GraphicsPath bounds,
            int offsetX, int offsetY, Color shadowColor)
        {
            if (g == null || bounds == null) return bounds;

            RectangleF boundsRect = bounds.GetBounds();

            // Disable anti-aliasing for crisp hard edges
            var prevMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.None;

            try
            {
                using (var brush = new SolidBrush(shadowColor))
                {
                    g.FillRectangle(brush,
                        boundsRect.X + offsetX,
                        boundsRect.Y + offsetY,
                        boundsRect.Width,
                        boundsRect.Height);
                }
            }
            finally
            {
                g.SmoothingMode = prevMode;
            }

            return bounds;
        }

        /// <summary>
        /// Paints a state-aware shadow that adjusts based on control state
        /// Best for: Any interactive control needing state feedback
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Control path</param>
        /// <param name="radius">Corner radius</param>
        /// <param name="state">Current control state</param>
        /// <param name="baseAlpha">Base shadow alpha (for Normal state)</param>
        /// <param name="baseOffsetY">Base Y offset (for Normal state)</param>
        /// <param name="shadowColor">Shadow color</param>
        public static GraphicsPath PaintStateAwareShadow(Graphics g, GraphicsPath bounds, int radius,
            ControlState state, int baseAlpha = 40, int baseOffsetY = 3, Color? shadowColor = null)
        {
            if (g == null || bounds == null) return bounds;

            Color color = shadowColor ?? Color.Black;

            // Adjust shadow based on state
            int alpha;
            int offsetY;
            int spread;

            switch (state)
            {
                case ControlState.Hovered:
                    alpha = (int)(baseAlpha * 1.4f);  // More prominent
                    offsetY = baseOffsetY + 1;        // Slightly more offset
                    spread = 3;
                    break;
                case ControlState.Pressed:
                    alpha = (int)(baseAlpha * 0.6f);  // Less prominent (appears closer)
                    offsetY = Math.Max(1, baseOffsetY - 1);
                    spread = 1;
                    break;
                case ControlState.Focused:
                    alpha = (int)(baseAlpha * 1.2f);  // Slightly more prominent
                    offsetY = baseOffsetY;
                    spread = 2;
                    break;
                case ControlState.Disabled:
                    alpha = (int)(baseAlpha * 0.4f);  // Very subtle
                    offsetY = Math.Max(1, baseOffsetY - 1);
                    spread = 1;
                    break;
                default: // Normal
                    alpha = baseAlpha;
                    offsetY = baseOffsetY;
                    spread = 2;
                    break;
            }

            return PaintCleanDropShadow(g, bounds, radius, 0, offsetY, color, alpha, spread);
        }

        /// <summary>
        /// Paints a very subtle shadow for minimal designs
        /// Best for: Minimal, Notion, Vercel, flat designs with subtle depth
        /// </summary>
        public static GraphicsPath PaintSubtleShadow(Graphics g, GraphicsPath bounds, int radius,
            int offsetY = 2, int alpha = 20)
        {
            return PaintCleanDropShadow(g, bounds, radius, 0, offsetY, Color.Black, alpha, 1);
        }

        /// <summary>
        /// Paints a two-layer shadow (key + ambient) for Material-style elevation
        /// Enhanced with proper Material Design 3 elevation specs
        /// Best for: Material Design, modern cards, elevated surfaces
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Control path</param>
        /// <param name="radius">Corner radius</param>
        /// <param name="elevation">Elevation level (1-5 typical, maps to Material dp levels)</param>
        /// <param name="shadowColor">Shadow base color (uses darker base color, not pure black)</param>
        public static GraphicsPath PaintDualLayerShadow(Graphics g, GraphicsPath bounds, int radius,
            int elevation = 2, Color? shadowColor = null)
        {
            if (g == null || bounds == null) return bounds;
            if (elevation <= 0) return bounds;

            // Use darker version of base color for more realistic shadows
            // If no color provided, use a dark gray instead of pure black
            Color baseShadowColor = shadowColor ?? Color.FromArgb(30, 30, 30);
            Color color = DarkenShadowColor(baseShadowColor);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Material Design 3 elevation shadow parameters
            // Ambient shadow (larger, softer, less opaque)
            int ambientAlpha = GetMaterialAmbientAlpha(elevation);
            int ambientOffsetY = GetMaterialAmbientOffset(elevation);
            int ambientSpread = GetMaterialAmbientSpread(elevation);
            int ambientBlur = GetMaterialAmbientBlur(elevation);
            
            PaintCleanDropShadow(g, bounds, radius, 0, ambientOffsetY, color, ambientAlpha, ambientSpread);

            // Key shadow (tighter, more defined, directional)
            int keyAlpha = GetMaterialKeyAlpha(elevation);
            int keyOffsetY = GetMaterialKeyOffset(elevation);
            int keyOffsetX = GetMaterialKeyOffsetX(elevation); // Slight horizontal offset for realism
            int keySpread = GetMaterialKeySpread(elevation);
            
            PaintCleanDropShadow(g, bounds, radius, keyOffsetX, keyOffsetY, color, keyAlpha, keySpread);

            return bounds;
        }

        /// <summary>
        /// Paint soft layered shadow for ultra-soft, realistic depth
        /// Best for: Subtle elevation, refined designs
        /// </summary>
        public static GraphicsPath PaintSoftLayeredShadow(Graphics g, GraphicsPath bounds, int radius,
            int offsetY = 4, float opacity = 0.3f, Color? shadowColor = null)
        {
            if (g == null || bounds == null) return bounds;

            Color baseColor = shadowColor ?? Color.FromArgb(30, 30, 30);
            Color color = DarkenShadowColor(baseColor);
            
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Use multiple soft layers for ultra-smooth shadow
            int layers = 6;
            for (int i = layers; i > 0; i--)
            {
                float layerOpacity = opacity * (i / (float)layers);
                int alpha = (int)(255 * layerOpacity);
                int layerOffsetY = offsetY + (layers - i);
                int layerSpread = i;

                PaintCleanDropShadow(g, bounds, radius, 0, layerOffsetY, color, alpha, layerSpread);
            }

            return bounds;
        }

        /// <summary>
        /// Darken shadow color for more realistic shadows (not pure black)
        /// </summary>
        private static Color DarkenShadowColor(Color baseColor)
        {
            // If it's already dark, use as-is; otherwise darken it
            if (baseColor.R < 50 && baseColor.G < 50 && baseColor.B < 50)
                return baseColor;
            
            // Create a darker, desaturated version
            return Color.FromArgb(
                Math.Max(0, baseColor.R - 40),
                Math.Max(0, baseColor.G - 40),
                Math.Max(0, baseColor.B - 40)
            );
        }

        /// <summary>
        /// Get Material Design ambient shadow alpha based on elevation
        /// </summary>
        private static int GetMaterialAmbientAlpha(int elevation)
        {
            // Material Design 3 elevation specs
            return elevation switch
            {
                1 => 25,  // 1dp
                2 => 30,  // 2dp
                3 => 35,  // 4dp
                4 => 40,  // 8dp
                5 => 45,  // 12dp
                _ => 30 + (elevation * 3)
            };
        }

        /// <summary>
        /// Get Material Design ambient shadow offset
        /// </summary>
        private static int GetMaterialAmbientOffset(int elevation)
        {
            return elevation switch
            {
                1 => 1,
                2 => 2,
                3 => 4,
                4 => 6,
                5 => 8,
                _ => elevation
            };
        }

        /// <summary>
        /// Get Material Design ambient shadow spread
        /// </summary>
        private static int GetMaterialAmbientSpread(int elevation)
        {
            return elevation switch
            {
                1 => 1,
                2 => 2,
                3 => 3,
                4 => 4,
                5 => 5,
                _ => elevation
            };
        }

        /// <summary>
        /// Get Material Design ambient shadow blur
        /// </summary>
        private static int GetMaterialAmbientBlur(int elevation)
        {
            return elevation switch
            {
                1 => 3,
                2 => 4,
                3 => 6,
                4 => 8,
                5 => 10,
                _ => elevation * 2
            };
        }

        /// <summary>
        /// Get Material Design key shadow alpha
        /// </summary>
        private static int GetMaterialKeyAlpha(int elevation)
        {
            return elevation switch
            {
                1 => 20,
                2 => 30,
                3 => 40,
                4 => 50,
                5 => 60,
                _ => 20 + (elevation * 8)
            };
        }

        /// <summary>
        /// Get Material Design key shadow Y offset
        /// </summary>
        private static int GetMaterialKeyOffset(int elevation)
        {
            return elevation switch
            {
                1 => 1,
                2 => 2,
                3 => 4,
                4 => 6,
                5 => 8,
                _ => elevation
            };
        }

        /// <summary>
        /// Get Material Design key shadow X offset (slight horizontal offset for realism)
        /// </summary>
        private static int GetMaterialKeyOffsetX(int elevation)
        {
            // Very slight horizontal offset (0-1px) for more realistic directional light
            return elevation >= 3 ? 1 : 0;
        }

        /// <summary>
        /// Get Material Design key shadow spread
        /// </summary>
        private static int GetMaterialKeySpread(int elevation)
        {
            return elevation switch
            {
                1 => 0,
                2 => 1,
                3 => 2,
                4 => 3,
                5 => 4,
                _ => elevation - 1
            };
        }

        #endregion

        #region "Core Shadow Methods"

        /// <summary>
        /// Paints a soft multi-layer shadow (improved opacity calculation)
        /// Note: For clean single-layer shadows, prefer PaintCleanDropShadow
        /// </summary>
        public static GraphicsPath PaintSoftShadow(Graphics g, GraphicsPath bounds, int radius, int offsetX, int offsetY,
            Color shadowColor, float opacity, int layers = 4)
        {
            if (opacity <= 0 || opacity > 1) return bounds;
            if (layers <= 0) layers = 1;
            if (layers > 8) layers = 8; // Cap layers for performance

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // Create offset shadow path
            using (Matrix offsetMatrix = new Matrix())
            {
                offsetMatrix.Translate(offsetX, offsetY);

                // Draw layers from OUTERMOST to INNERMOST
                // Outer layers are larger (more spread) and dimmer
                // Inner layers are smaller and slightly brighter, painting OVER outer layers
                // This creates a soft fade-out effect at the edges
                for (int i = 1; i <= layers; i++)
                {
                    // Outer layers (i=1) have more spread and are dimmer
                    // Inner layers (i=layers) have less spread and are brighter
                    int spread = layers - i;
                    
                    // Calculate alpha - use sqrt for smoother progression
                    // Base alpha from shadowColor (or 80 if not set), scaled by opacity
                    // Increased base alpha for better visibility
                    int baseAlpha = Math.Min(180, (int)(shadowColor.A * opacity));
                    float layerFade = (float)Math.Sqrt((float)i / layers); // sqrt for smoother progression
                    int layerAlpha = Math.Max(5, Math.Min(180, (int)(baseAlpha * layerFade)));

                    Color layerShadowColor = Color.FromArgb(layerAlpha, shadowColor.R, shadowColor.G, shadowColor.B);

                    using (GraphicsPath shadowPath = (GraphicsPath)bounds.Clone())
                    {
                        shadowPath.Transform(offsetMatrix);

                        // Apply spread - expand path for outer layers
                        if (spread > 0)
                        {
                            using (GraphicsPath expandedPath = shadowPath.CreateInsetPath(-spread))
                            {
                                if (expandedPath != null && expandedPath.PointCount > 0)
                                {
                                    var shadowBrush = PaintersFactory.GetSolidBrush(layerShadowColor);
                                    g.FillPath(shadowBrush, expandedPath);
                                }
                            }
                        }
                        else
                        {
                            var shadowBrush = PaintersFactory.GetSolidBrush(layerShadowColor);
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }
                }
            }

            return bounds;
        }

        /// <summary>
        /// Paints Material Design elevation shadow
        /// </summary>
        public static GraphicsPath PaintMaterialShadow(Graphics g, GraphicsPath bounds, int radius, MaterialElevation elevation)
        {
            if (elevation == MaterialElevation.Level0) return bounds;

            // Material Design elevation: two-layer shadow (ambient + key)
            int elevationValue = (int)elevation;
            Color baseShadowTheme = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ShadowColor ?? Color.Black;

            // Material Design shadow parameters based on elevation level
            // Alpha values increased for better visibility
            int ambientAlpha = Math.Min(70, 20 + elevationValue * 5);
            int keyAlpha = Math.Min(90, 30 + elevationValue * 6);
            
            int ambientOffsetY = Math.Max(1, elevationValue);
            int keyOffsetY = Math.Max(2, elevationValue + 1);
            
            int ambientSpread = Math.Max(2, elevationValue + 1);
            int keySpread = Math.Max(1, elevationValue);

            // Draw ambient shadow first (larger, softer)
            PaintCleanDropShadow(g, bounds, radius, 0, ambientOffsetY, baseShadowTheme, ambientAlpha, ambientSpread);

            // Draw key shadow on top (tighter, more defined)
            PaintCleanDropShadow(g, bounds, radius, 0, keyOffsetY, baseShadowTheme, keyAlpha, keySpread);

            // Return the area inside the shadow using shape-aware inset
            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints neumorphic embossed shadow (dual shadow for raised effect)
        /// </summary>
        public static GraphicsPath PaintNeumorphicShadow(Graphics g, GraphicsPath bounds, int radius, Color backgroundColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Light shadow (top-left)
            Color lightShadow = Lighten(backgroundColor, 0.15f);

            using (GraphicsPath lightPath = (GraphicsPath)bounds.Clone())
            using (Matrix lightMatrix = new Matrix())
            {
                lightMatrix.Translate(-4, -4);
                lightPath.Transform(lightMatrix);

                var lightBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(80, lightShadow));
                g.FillPath(lightBrush, lightPath);
            }

            // Dark shadow (bottom-right)
            Color darkShadow = Darken(backgroundColor, 0.15f);

            using (GraphicsPath darkPath = (GraphicsPath)bounds.Clone())
            using (Matrix darkMatrix = new Matrix())
            {
                darkMatrix.Translate(4, 4);
                darkPath.Transform(darkMatrix);

                var darkBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(80, darkShadow));
                g.FillPath(darkBrush, darkPath);
            }

            // Return the area inside the shadow using shape-aware inset
            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints a glow effect (for DarkGlow Style)
        /// </summary>
        public static GraphicsPath PaintGlow(Graphics g, GraphicsPath bounds, int radius, Color glowColor, float intensity)
        {
            if (intensity <= 0) return bounds;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            int glowSize = (int)(8 * intensity);
            for (int i = 0; i < glowSize; i++)
            {
                int alpha = (int)(30 * intensity * (1f - (float)i / glowSize));
                if (alpha <= 0) continue;

                // Create expanded glow path using negative inset (outset)
                using (GraphicsPath glowPath = bounds.CreateInsetPath(-i))
                {
                    var glowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb(alpha, glowColor));
                    g.FillPath(glowBrush, glowPath);
                }
            }

            return bounds;
        }

        /// <summary>
        /// Lightens a color by a percentage
        /// </summary>
        public static Color Lighten(Color color, float percent)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, color.R + (int)(255 * percent)),
                Math.Min(255, color.G + (int)(255 * percent)),
                Math.Min(255, color.B + (int)(255 * percent))
            );
        }

        /// <summary>
        /// Darkens a color by a percentage
        /// </summary>
        public static Color Darken(Color color, float percent)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, color.R - (int)(color.R * percent)),
                Math.Max(0, color.G - (int)(color.G * percent)),
                Math.Max(0, color.B - (int)(color.B * percent))
            );
        }
        #endregion
        #region "Modern UX/UI Shadow Effects"

        /// <summary>
        /// Paints inner shadow (inset shadow for pressed/recessed effect)
        /// Best for: Pressed button states, recessed panels, input fields
        /// </summary>
        public static GraphicsPath PaintInnerShadow(Graphics g, GraphicsPath bounds, int radius, int depth = 4, Color? shadowColor = null)
        {
            if (g == null || bounds == null || bounds.PointCount == 0) return bounds;

            Color shadow = shadowColor ?? BeepStyling.CurrentTheme?.ShadowColor ?? Color.FromArgb(100, 0, 0, 0);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            try
            {
                // Create gradient from edge to center using PathGradientBrush
                using (var pathBrush = new PathGradientBrush(bounds))
                {
                    pathBrush.CenterColor = Color.Transparent;
                    pathBrush.SurroundColors = new[] { shadow };
                    pathBrush.FocusScales = new PointF(0.85f, 0.85f);

                    g.FillPath(pathBrush, bounds);
                }
            }
            catch
            {
                // Fallback: simple darkened edge if PathGradientBrush fails
                using (var pen = new Pen(Color.FromArgb(shadow.A / 2, shadow), 1))
                {
                    g.DrawPath(pen, bounds);
                }
            }

            return bounds;
        }

        /// <summary>
        /// Paints modern card shadow (inspired by modern web design)
        /// </summary>
        public static GraphicsPath PaintCardShadow(Graphics g, GraphicsPath bounds, int radius, CardShadowStyle style = CardShadowStyle.Medium)
        {
            int offsetY, spread, alpha;

            // Use direct alpha values for cleaner, more predictable shadows
            // Increased alpha values for better visibility
            switch (style)
            {
                case CardShadowStyle.Small:
                    offsetY = 1; spread = 2; alpha = 40;
                    break;
                case CardShadowStyle.Medium:
                    offsetY = 2; spread = 3; alpha = 60;
                    break;
                case CardShadowStyle.Large:
                    offsetY = 4; spread = 5; alpha = 80;
                    break;
                case CardShadowStyle.XLarge:
                    offsetY = 6; spread = 8; alpha = 100;
                    break;
                default:
                    offsetY = 2; spread = 3; alpha = 60;
                    break;
            }

            // Use PaintCleanDropShadow for predictable, subtle shadow
            return PaintCleanDropShadow(g, bounds, radius, 0, offsetY, Color.Black, alpha, spread);
        }

        /// <summary>
        /// Paints colored shadow (for colored UI elements)
        /// </summary>
        public static GraphicsPath PaintColoredShadow(Graphics g, GraphicsPath bounds, int radius, Color baseColor, int offsetX = 0, int offsetY = 4, float intensity = 0.6f)
        {
            // Create a darker, saturated version of the base color for shadow
            Color shadowColor = Color.FromArgb(
                Math.Max(0, baseColor.R - 40),
                Math.Max(0, baseColor.G - 40),
                Math.Max(0, baseColor.B - 40)
            );

            return PaintSoftShadow(g, bounds, radius, offsetX, offsetY, shadowColor, intensity, 8);
        }

        /// <summary>
        /// Paints neon glow shadow (for accent/highlight effects)
        /// </summary>
        public static GraphicsPath PaintNeonGlow(Graphics g, GraphicsPath bounds, int radius, Color glowColor, float intensity = 1f, int glowRadius = 12)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = 0; i < glowRadius; i++)
            {
                float progress = 1f - ((float)i / glowRadius);
                // Adjusted base intensity for a smoother, less solid glow
                // Was 220 (too solid), now 100 (allows layering to create depth)
                // Use cubic falloff (Math.Pow(progress, 3)) to make outer edges fade faster, reducing perceived size
                int alpha = (int)(100 * intensity * Math.Pow(progress, 3)); 

                if (alpha <= 0) continue;
                alpha = Math.Min(255, alpha); // Clamp

                if (bounds is null) return new GraphicsPath();
                if (bounds.GetBounds().IsEmpty) return new GraphicsPath();
                
                // Use negative inset (outset)
                using (var glowPath = bounds.CreateInsetPath(-i))
                // Create NEW pen (not cached) so we can modify LineJoin property
                using (var pen = new Pen(Color.FromArgb(alpha, glowColor), 2f))
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, glowPath);
                }
            }

            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints floating shadow (for floating action buttons and elevated cards)
        /// </summary>
        public static GraphicsPath PaintFloatingShadow(Graphics g, GraphicsPath bounds, int radius, int elevation = 8)
        {
            Color baseShadowTheme = Beep.Winform.Controls.Styling.BeepStyling.CurrentTheme?.ShadowColor ?? Color.Black;
            
            // Floating shadows: two layers for depth - ambient (soft) + key (tighter)
            int offsetY = Math.Max(2, elevation / 2);
            
            // Ambient shadow (larger, softer, more spread)
            // Increased alpha for visibility
            int ambientAlpha = Math.Min(80, 20 + elevation * 2);
            int ambientSpread = Math.Max(3, elevation / 2);
            PaintCleanDropShadow(g, bounds, radius, 0, offsetY + 2, baseShadowTheme, ambientAlpha, ambientSpread);

            // Key shadow (tighter, more defined)
            // Increased alpha for visibility
            int keyAlpha = Math.Min(100, 30 + elevation * 2);
            int keySpread = Math.Max(2, elevation / 3);
            PaintCleanDropShadow(g, bounds, radius, 0, offsetY, baseShadowTheme, keyAlpha, keySpread);

            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints drop shadow with custom blur
        /// </summary>
        public static GraphicsPath PaintDropShadow(Graphics g, GraphicsPath bounds, int radius, int offsetX, int offsetY, int blurRadius, Color shadowColor, float opacity = 0.5f)
        {
            return PaintSoftShadow(g, bounds, radius, offsetX, offsetY, shadowColor, opacity, blurRadius);
        }

        /// <summary>
        /// Paints long shadow (flat design Style)
        /// </summary>
        public static GraphicsPath PaintLongShadow(Graphics g, GraphicsPath bounds, int radius, float angle = 45f, int length = 20, Color? shadowColor = null)
        {
            Color shadow = shadowColor ?? Color.FromArgb(60, 0, 0, 0);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float angleRad = angle * (float)Math.PI / 180f;
            float dx = (float)Math.Cos(angleRad);
            float dy = (float)Math.Sin(angleRad);

            // Draw shadow in steps
            for (int i = 1; i <= length; i++)
            {
                float progress = 1f - ((float)i / length);
                int alpha = (int)(shadow.A * progress);

                if (alpha <= 5) continue;

                Color stepColor = Color.FromArgb(alpha, shadow.R, shadow.G, shadow.B);

                using (var shadowPath = (GraphicsPath)bounds.Clone())
                using (var matrix = new Matrix())
                {
                    matrix.Translate(dx * i, dy * i);
                    shadowPath.Transform(matrix);

                    var brush = PaintersFactory.GetSolidBrush(stepColor);
                    g.FillPath(brush, shadowPath);
                }
            }

            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints perspective shadow (for 3D tilt effect)
        /// </summary>
        public static GraphicsPath PaintPerspectiveShadow(Graphics g, GraphicsPath bounds, int radius, PerspectiveDirection direction = PerspectiveDirection.BottomRight, float intensity = 0.5f)
        {
            var boundsRect = bounds.GetBounds();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create perspective points based on direction
            PointF[] srcPoints = new PointF[]
            {
                new PointF(boundsRect.Left, boundsRect.Top),
                new PointF(boundsRect.Right, boundsRect.Top),
                new PointF(boundsRect.Left, boundsRect.Bottom),
                new PointF(boundsRect.Right, boundsRect.Bottom)
            };

            PointF[] destPoints = (PointF[])srcPoints.Clone();
            float offset = 15f * intensity;

            switch (direction)
            {
                case PerspectiveDirection.BottomRight:
                    destPoints[1].X += offset;
                    destPoints[3].X += offset;
                    destPoints[3].Y += offset;
                    break;
                case PerspectiveDirection.BottomLeft:
                    destPoints[0].X -= offset;
                    destPoints[2].X -= offset;
                    destPoints[2].Y += offset;
                    break;
                case PerspectiveDirection.TopRight:
                    destPoints[1].X += offset;
                    destPoints[1].Y -= offset;
                    break;
                case PerspectiveDirection.TopLeft:
                    destPoints[0].X -= offset;
                    destPoints[0].Y -= offset;
                    break;
            }

            // Draw transformed shadow
            var shadowBrush = PaintersFactory.GetSolidBrush(Color.FromArgb((int)(80 * intensity), 0, 0, 0));
            using (var shadowPath = (GraphicsPath)bounds.Clone())
            using (var matrix = new Matrix(boundsRect, destPoints))
            {
                shadowPath.Transform(matrix);
                g.FillPath(shadowBrush, shadowPath);
            }

            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints double shadow (for layered depth effect)
        /// </summary>
        public static GraphicsPath PaintDoubleShadow(Graphics g, GraphicsPath bounds, int radius, Color color1, Color color2, int offset1X = 2, int offset1Y = 2, int offset2X = 4, int offset2Y = 4)
        {
            // First shadow layer
            PaintSoftShadow(g, bounds, radius, offset1X, offset1Y, color1, 0.4f, 4);

            // Second shadow layer
            PaintSoftShadow(g, bounds, radius, offset2X, offset2Y, color2, 0.3f, 6);

            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints ambient shadow (soft all-around shadow)
        /// </summary>
        public static GraphicsPath PaintAmbientShadow(Graphics g, GraphicsPath bounds, int radius, int spread = 4, float opacity = 0.3f)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = 0; i < spread; i++)
            {
                float progress = 1f - ((float)i / spread);
                int alpha = (int)(50 * opacity * progress);

                if (alpha <= 0) continue;

                using (var spreadPath = bounds.CreateInsetPath(-i))
                {
                    var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(alpha, 0, 0, 0));
                    g.FillPath(brush, spreadPath);
                }
            }

            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints glassmorphism shadow (subtle blur with frost effect)
        /// </summary>
        public static GraphicsPath PaintGlassShadow(Graphics g, GraphicsPath bounds, int radius, Color tintColor, float opacity = 0.2f)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Outer glow
            for (int i = 0; i < 6; i++)
            {
                int alpha = (int)(30 * opacity * (1f - (float)i / 6));
                if (alpha <= 0) continue;

                using (var glowPath = bounds.CreateInsetPath(-i))
                {
                    var brush = PaintersFactory.GetSolidBrush(Color.FromArgb(alpha, tintColor));
                    g.FillPath(brush, glowPath);
                }
            }

            // Inner highlight
            using (var insetPath = bounds.CreateInsetPath(2))
            {
                var brush = PaintersFactory.GetSolidBrush(Color.FromArgb((int)(40 * opacity), Color.White));
                g.FillPath(brush, insetPath);
            }

            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints reflection shadow (for mirrored/reflected elements)
        /// </summary>
        public static void PaintReflectionShadow(Graphics g, GraphicsPath bounds, int offsetY = 5, float fadeStart = 0.5f, float fadeEnd = 0.1f)
        {
            var boundsRect = bounds.GetBounds();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Flip the shape vertically
            using (var reflectionPath = (GraphicsPath)bounds.Clone())
            using (var matrix = new Matrix())
            {
                matrix.Translate(0, boundsRect.Bottom + offsetY);
                matrix.Scale(1, -1, MatrixOrder.Append);
                matrix.Translate(0, boundsRect.Bottom + offsetY, MatrixOrder.Append);
                reflectionPath.Transform(matrix);

                // Create gradient for fade effect
                var reflectionBounds = reflectionPath.GetBounds();
                var gradientBrush = PaintersFactory.GetLinearGradientBrush(reflectionBounds, Color.FromArgb((int)(255 * fadeStart), Color.Black), Color.FromArgb((int)(255 * fadeEnd), Color.Black), LinearGradientMode.Vertical);
                g.FillPath(gradientBrush, reflectionPath);
            }
        }

        /// <summary>
        /// Paints border glow shadow (outline glow effect)
        /// </summary>
        public static GraphicsPath PaintBorderGlow(Graphics g, GraphicsPath bounds, int radius, Color glowColor, int glowWidth = 3, float intensity = 0.8f)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (bounds == null || bounds.PointCount == 0)
            {
                throw new ArgumentException("Invalid GraphicsPath object.");
            }
            for (int i = 0; i < glowWidth; i++)
            {
                float progress = 1f - ((float)i / glowWidth);
                int alpha = (int)(180 * intensity * progress);

                if (alpha <= 0) continue;

                // Create NEW pen (not cached) so we can modify LineJoin property
                using (var pen = new Pen(Color.FromArgb(alpha, glowColor), (glowWidth - i) * 2))
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, bounds);
                }
            }

            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints shape-specific shadows for different controls
        /// </summary>
        public static GraphicsPath PaintShapeSpecificShadow(Graphics g, GraphicsPath bounds, int radius, ShapeShadowStyle style)
        {
            switch (style)
            {
                case ShapeShadowStyle.Circle:
                    return PaintRadialShadow(g, bounds, radius);
                case ShapeShadowStyle.Tab:
                    return PaintTabShadow(g, bounds, radius);
                case ShapeShadowStyle.Button:
                    return PaintButtonShadow(g, bounds, radius);
                case ShapeShadowStyle.Panel:
                    return PaintPanelShadow(g, bounds, radius);
                case ShapeShadowStyle.Dropdown:
                    return PaintDropdownShadow(g, bounds, radius);
                default:
                    return PaintCardShadow(g, bounds, radius);
            }
        }

        private static GraphicsPath PaintRadialShadow(Graphics g, GraphicsPath bounds, int radius)
        {
            var boundsRect = bounds.GetBounds();
            var center = new PointF(boundsRect.X + boundsRect.Width / 2, boundsRect.Y + boundsRect.Height / 2);

            using (var shadowPath = new GraphicsPath())
            {
                float maxRadius = Math.Max(boundsRect.Width, boundsRect.Height) / 2;
                shadowPath.AddEllipse(center.X - maxRadius - 8, center.Y - maxRadius - 8, (maxRadius + 8) * 2, (maxRadius + 8) * 2);

                using (var brush = new PathGradientBrush(shadowPath))
                {
                    brush.CenterPoint = center;
                    brush.CenterColor = Color.FromArgb(60, 0, 0, 0);
                    brush.SurroundColors = new[] { Color.Transparent };

                    g.FillPath(brush, shadowPath);
                }
            }

            return bounds.CreateInsetPath(radius);
        }

        private static GraphicsPath PaintTabShadow(Graphics g, GraphicsPath bounds, int radius)
        {
            // Tabs only have bottom shadow
            return PaintSoftShadow(g, bounds, radius, 0, 2, Color.Black, 0.15f, 4);
        }

        private static GraphicsPath PaintButtonShadow(Graphics g, GraphicsPath bounds, int radius)
        {
            // Buttons have tight, close shadows
            return PaintSoftShadow(g, bounds, radius, 0, 2, Color.Black, 0.25f, 3);
        }

        private static GraphicsPath PaintPanelShadow(Graphics g, GraphicsPath bounds, int radius)
        {
            // Panels have larger, softer shadows
            return PaintSoftShadow(g, bounds, radius, 0, 4, Color.Black, 0.2f, 12);
        }

        private static GraphicsPath PaintDropdownShadow(Graphics g, GraphicsPath bounds, int radius)
        {
            // Dropdowns have all-around shadows
            return PaintAmbientShadow(g, bounds, radius, 8, 0.25f);
        }

        #endregion

        #region "Shadow Combinations & Presets"

        /// <summary>
        /// Applies modern UI shadow preset
        /// </summary>
        public static GraphicsPath ApplyModernShadowPreset(Graphics g, GraphicsPath bounds, int radius, ModernShadowPreset preset)
        {
            switch (preset)
            {
                case ModernShadowPreset.Flat:
                    // No shadow
                    return bounds;

                case ModernShadowPreset.Subtle:
                    return PaintCardShadow(g, bounds, radius, CardShadowStyle.Small);

                case ModernShadowPreset.Elevated:
                    return PaintCardShadow(g, bounds, radius, CardShadowStyle.Medium);

                case ModernShadowPreset.Floating:
                    return PaintFloatingShadow(g, bounds, radius, 12);

                case ModernShadowPreset.Dramatic:
                    return PaintCardShadow(g, bounds, radius, CardShadowStyle.XLarge);

                case ModernShadowPreset.Neumorphic:
                    return PaintNeumorphicShadow(g, bounds, radius, SystemColors.Control);

                case ModernShadowPreset.Glowing:
                    return PaintNeonGlow(g, bounds, radius, Color.FromArgb(100, 150, 255), 0.8f, 16);

                case ModernShadowPreset.Material:
                    return PaintMaterialShadow(g, bounds, radius, MaterialElevation.Level4);

                default:
                    return PaintCardShadow(g, bounds, radius, CardShadowStyle.Medium);
            }
        }

        #endregion

        #region "Utility Methods"

        /// <summary>
        /// Calculates optimal shadow parameters based on control size
        /// </summary>
        public static (int offsetX, int offsetY, int blur, float opacity) CalculateAdaptiveShadow(GraphicsPath bounds, ShadowIntensity intensity = ShadowIntensity.Medium)
        {
            var boundsRect = bounds.GetBounds();
            float size = Math.Min(boundsRect.Width, boundsRect.Height);

            float scale = size / 100f; // Base on 100px reference

            switch (intensity)
            {
                case ShadowIntensity.Light:
                    return (0, (int)(2 * scale), (int)(4 * scale), 0.15f);
                case ShadowIntensity.Medium:
                    return (0, (int)(4 * scale), (int)(8 * scale), 0.25f);
                case ShadowIntensity.Strong:
                    return (0, (int)(8 * scale), (int)(16 * scale), 0.35f);
                default:
                    return (0, (int)(4 * scale), (int)(8 * scale), 0.25f);
            }
        }

        /// <summary>
        /// Blends two colors
        /// </summary>
        public static Color BlendColors(Color color1, Color color2, float ratio)
        {
            return Color.FromArgb(
                (int)(color1.A * (1 - ratio) + color2.A * ratio),
                (int)(color1.R * (1 - ratio) + color2.R * ratio),
                (int)(color1.G * (1 - ratio) + color2.G * ratio),
                (int)(color1.B * (1 - ratio) + color2.B * ratio)
            );
        }

        #endregion

        #region "Enumerations"

        public enum CardShadowStyle
        {
            Small,
            Medium,
            Large,
            XLarge
        }

        public enum PerspectiveDirection
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public enum ShapeShadowStyle
        {
            Circle,
            Tab,
            Button,
            Panel,
            Dropdown,
            Card
        }

        public enum ModernShadowPreset
        {
            Flat,
            Subtle,
            Elevated,
            Floating,
            Dramatic,
            Neumorphic,
            Glowing,
            Material
        }

        public enum ShadowIntensity
        {
            Light,
            Medium,
            Strong
        }

        #endregion
    }
}
