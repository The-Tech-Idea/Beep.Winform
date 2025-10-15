using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.Styling.ShadowPainters
{
    /// <summary>
    /// Helper utilities for painting shadows
    /// Provides common methods for soft shadows, material elevation, and special effects
    /// </summary>
    public static class ShadowPainterHelpers
    {
      

      

        /// <summary>
        /// Creates a rounded rectangle path for shadow
        /// </summary>
        public static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

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
        /// Paints a glow effect (for DarkGlow style)
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
    }
}
