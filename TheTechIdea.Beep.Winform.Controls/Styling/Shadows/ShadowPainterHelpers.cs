using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Helper utilities for painting shadows
    /// Provides common methods for soft shadows, material elevation, and special effects
    /// Supports advanced UX/UI shadow effects for modern interfaces
    /// </summary>
    public static class ShadowPainterHelpers
    {
      

      

     
        /// <summary>
        /// Paints a soft multi-layer shadow
        /// </summary>
        public static GraphicsPath PaintSoftShadow(Graphics g, GraphicsPath bounds, int radius, int offsetX, int offsetY, 
            Color shadowColor, float opacity, int layers = 6)
        {
            if (opacity <= 0 || opacity > 1) return bounds;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // Create offset shadow path
            using (Matrix offsetMatrix = new Matrix())
            {
                offsetMatrix.Translate(offsetX, offsetY);
                
                for (int i = 1; i <= layers; i++)
                {
                    float layerOpacityFactor = (float)(layers - i + 1) / layers;
                    float finalOpacity = opacity * layerOpacityFactor * 0.6f;
                    int layerAlpha = Math.Max(5, (int)(255 * finalOpacity));

                    Color layerShadowColor = Color.FromArgb(layerAlpha, shadowColor);

                    int spread = i - 1;
                    
                    using (GraphicsPath shadowPath = (GraphicsPath)bounds.Clone())
                    {
                        shadowPath.Transform(offsetMatrix);
                        
                        // Apply spread if needed
                        if (spread > 0)
                        {
                            using (GraphicsPath expandedPath = shadowPath.CreateInsetPath(-spread))
                            {
                                using (SolidBrush shadowBrush = new SolidBrush(layerShadowColor))
                                {
                                    g.FillPath(shadowBrush, expandedPath);
                                }
                            }
                        }
                        else
                        {
                            using (SolidBrush shadowBrush = new SolidBrush(layerShadowColor))
                            {
                                g.FillPath(shadowBrush, shadowPath);
                            }
                        }
                    }
                }
            }
            
            // Return the area inside the shadow using shape-aware inset
            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints Material Design elevation shadow
        /// </summary>
        public static GraphicsPath PaintMaterialShadow(Graphics g, GraphicsPath bounds, int radius, MaterialElevation elevation)
        {
            if (elevation == MaterialElevation.Level0) return bounds;

            // Material shadows use two layers: key light (top) and ambient light (bottom)
            int elevationValue = (int)elevation;
            
            // Key light shadow (directional, smaller)
            int keyOffsetY = elevationValue * 2;
            int keyBlur = elevationValue * 2;
            Color keyShadowColor = Color.FromArgb(40, 0, 0, 0);
            
            // Ambient light shadow (larger, softer)
            int ambientOffsetY = elevationValue;
            int ambientBlur = elevationValue * 4;
            Color ambientShadowColor = Color.FromArgb(30, 0, 0, 0);

            // Draw ambient shadow first (larger)
            PaintSoftShadow(g, bounds, radius, 0, ambientOffsetY, ambientShadowColor, 0.3f, ambientBlur);
            
            // Draw key shadow on top (smaller, more defined)
            PaintSoftShadow(g, bounds, radius, 0, keyOffsetY, keyShadowColor, 0.4f, keyBlur);
            
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
                
                using (SolidBrush lightBrush = new SolidBrush(Color.FromArgb(80, lightShadow)))
                {
                    g.FillPath(lightBrush, lightPath);
                }
            }

            // Dark shadow (bottom-right)
            Color darkShadow = Darken(backgroundColor, 0.15f);
            
            using (GraphicsPath darkPath = (GraphicsPath)bounds.Clone())
            using (Matrix darkMatrix = new Matrix())
            {
                darkMatrix.Translate(4, 4);
                darkPath.Transform(darkMatrix);
                
                using (SolidBrush darkBrush = new SolidBrush(Color.FromArgb(80, darkShadow)))
                {
                    g.FillPath(darkBrush, darkPath);
                }
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
                using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(alpha, glowColor)))
                {
                    g.FillPath(glowBrush, glowPath);
                }
            }
            
            // Return the area inside the glow using shape-aware inset
            return bounds.CreateInsetPath(radius);
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

        #region "Modern UX/UI Shadow Effects"

        /// <summary>
        /// Paints inner shadow (inset shadow for pressed/recessed effect)
        /// </summary>
        public static GraphicsPath PaintInnerShadow(Graphics g, GraphicsPath bounds, int radius, int depth = 4, Color? shadowColor = null)
        {
            Color shadow = shadowColor ?? Color.FromArgb(100, 0, 0, 0);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create inset path
            using (var insetPath = bounds.CreateInsetPath(depth))
            {
                // Create gradient from edge to center
                using (var pathBrush = new PathGradientBrush(bounds))
                {
                    pathBrush.CenterColor = Color.Transparent;
                    pathBrush.SurroundColors = new[] { shadow };
                    pathBrush.FocusScales = new PointF(0.8f, 0.8f);

                    g.FillPath(pathBrush, bounds);
                }
            }

            return bounds.CreateInsetPath(radius);
        }

        /// <summary>
        /// Paints modern card shadow (inspired by modern web design)
        /// </summary>
        public static GraphicsPath PaintCardShadow(Graphics g, GraphicsPath bounds, int radius, CardShadowStyle style = CardShadowStyle.Medium)
        {
            int offsetY, blur;
            float opacity;

            switch (style)
            {
                case CardShadowStyle.Small:
                    offsetY = 2; blur = 4; opacity = 0.15f;
                    break;
                case CardShadowStyle.Medium:
                    offsetY = 4; blur = 8; opacity = 0.2f;
                    break;
                case CardShadowStyle.Large:
                    offsetY = 8; blur = 16; opacity = 0.25f;
                    break;
                case CardShadowStyle.XLarge:
                    offsetY = 16; blur = 24; opacity = 0.3f;
                    break;
                default:
                    offsetY = 4; blur = 8; opacity = 0.2f;
                    break;
            }

            return PaintSoftShadow(g, bounds, radius, 0, offsetY, Color.Black, opacity, blur / 2);
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
                int alpha = (int)(120 * intensity * progress * progress); // Quadratic falloff

                if (alpha <= 0) continue;
                if (bounds is null) return new GraphicsPath();
                if (bounds.GetBounds().IsEmpty ) return new GraphicsPath();
                using (var glowPath = bounds.CreateInsetPath(-i))
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
            // Floating shadows have both soft blur and offset
            int blur = elevation * 2;
            int offsetY = elevation;

            // Draw large soft shadow
            PaintSoftShadow(g, bounds, radius, 0, offsetY, Color.FromArgb(40, 0, 0, 0), 0.4f, blur);

            // Draw tighter core shadow
            PaintSoftShadow(g, bounds, radius, 0, offsetY / 2, Color.FromArgb(60, 0, 0, 0), 0.3f, blur / 2);

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

                    using (var brush = new SolidBrush(stepColor))
                    {
                        g.FillPath(brush, shadowPath);
                    }
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
            using (var shadowBrush = new SolidBrush(Color.FromArgb((int)(80 * intensity), 0, 0, 0)))
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
                using (var brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                {
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
                using (var brush = new SolidBrush(Color.FromArgb(alpha, tintColor)))
                {
                    g.FillPath(brush, glowPath);
                }
            }

            // Inner highlight
            using (var insetPath = bounds.CreateInsetPath(2))
            using (var brush = new SolidBrush(Color.FromArgb((int)(40 * opacity), Color.White)))
            {
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
                using (var gradientBrush = new LinearGradientBrush(
                    new PointF(reflectionBounds.Left, reflectionBounds.Top),
                    new PointF(reflectionBounds.Left, reflectionBounds.Bottom),
                    Color.FromArgb((int)(255 * fadeStart), Color.Black),
                    Color.FromArgb((int)(255 * fadeEnd), Color.Black)))
                {
                    g.FillPath(gradientBrush, reflectionPath);
                }
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
