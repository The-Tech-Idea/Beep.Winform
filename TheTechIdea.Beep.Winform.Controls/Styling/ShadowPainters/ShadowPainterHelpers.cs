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
        /// Control state enum for consistent state handling
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
        /// Material elevation levels (0-5)
        /// </summary>
        public enum MaterialElevation
        {
            Level0 = 0,  // No shadow
            Level1 = 1,  // 1dp elevation
            Level2 = 2,  // 2dp elevation
            Level3 = 3,  // 4dp elevation
            Level4 = 4,  // 8dp elevation
            Level5 = 5   // 16dp elevation
        }

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
        public static void PaintSoftShadow(Graphics g, Rectangle bounds, int radius, int offsetX, int offsetY, 
            Color shadowColor, float opacity, int layers = 6)
        {
            if (opacity <= 0 || opacity > 1) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            Rectangle shadowRect = new Rectangle(
                bounds.X + offsetX,
                bounds.Y + offsetY,
                bounds.Width,
                bounds.Height
            );

            for (int i = 1; i <= layers; i++)
            {
                float layerOpacityFactor = (float)(layers - i + 1) / layers;
                float finalOpacity = opacity * layerOpacityFactor * 0.6f;
                int layerAlpha = Math.Max(5, (int)(255 * finalOpacity));

                Color layerShadowColor = Color.FromArgb(layerAlpha, shadowColor);

                int spread = i - 1;
                Rectangle layerRect = new Rectangle(
                    shadowRect.X - spread,
                    shadowRect.Y - spread,
                    shadowRect.Width + (spread * 2),
                    shadowRect.Height + (spread * 2)
                );

                using (SolidBrush shadowBrush = new SolidBrush(layerShadowColor))
                {
                    if (radius > 0)
                    {
                        int shadowRadius = Math.Max(0, radius + spread);
                        using (GraphicsPath shadowPath = CreateRoundedRectangle(layerRect, shadowRadius))
                        {
                            g.FillPath(shadowBrush, shadowPath);
                        }
                    }
                    else
                    {
                        g.FillRectangle(shadowBrush, layerRect);
                    }
                }
            }
        }

        /// <summary>
        /// Paints Material Design elevation shadow
        /// </summary>
        public static void PaintMaterialShadow(Graphics g, Rectangle bounds, int radius, MaterialElevation elevation)
        {
            if (elevation == MaterialElevation.Level0) return;

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
        }

        /// <summary>
        /// Paints neumorphic embossed shadow (dual shadow for raised effect)
        /// </summary>
        public static void PaintNeumorphicShadow(Graphics g, Rectangle bounds, int radius, Color backgroundColor)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Light shadow (top-left)
            Color lightShadow = Lighten(backgroundColor, 0.15f);
            Rectangle lightRect = new Rectangle(bounds.X - 4, bounds.Y - 4, bounds.Width, bounds.Height);
            
            using (SolidBrush lightBrush = new SolidBrush(Color.FromArgb(80, lightShadow)))
            {
                if (radius > 0)
                {
                    using (GraphicsPath lightPath = CreateRoundedRectangle(lightRect, radius))
                    {
                        g.FillPath(lightBrush, lightPath);
                    }
                }
                else
                {
                    g.FillRectangle(lightBrush, lightRect);
                }
            }

            // Dark shadow (bottom-right)
            Color darkShadow = Darken(backgroundColor, 0.15f);
            Rectangle darkRect = new Rectangle(bounds.X + 4, bounds.Y + 4, bounds.Width, bounds.Height);
            
            using (SolidBrush darkBrush = new SolidBrush(Color.FromArgb(80, darkShadow)))
            {
                if (radius > 0)
                {
                    using (GraphicsPath darkPath = CreateRoundedRectangle(darkRect, radius))
                    {
                        g.FillPath(darkBrush, darkPath);
                    }
                }
                else
                {
                    g.FillRectangle(darkBrush, darkRect);
                }
            }
        }

        /// <summary>
        /// Paints a glow effect (for DarkGlow style)
        /// </summary>
        public static void PaintGlow(Graphics g, Rectangle bounds, int radius, Color glowColor, float intensity)
        {
            if (intensity <= 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            int glowSize = (int)(8 * intensity);
            for (int i = 0; i < glowSize; i++)
            {
                int alpha = (int)(30 * intensity * (1f - (float)i / glowSize));
                if (alpha <= 0) continue;

                Rectangle glowRect = new Rectangle(
                    bounds.X - i,
                    bounds.Y - i,
                    bounds.Width + (i * 2),
                    bounds.Height + (i * 2)
                );

                using (SolidBrush glowBrush = new SolidBrush(Color.FromArgb(alpha, glowColor)))
                {
                    if (radius > 0)
                    {
                        using (GraphicsPath glowPath = CreateRoundedRectangle(glowRect, radius + i))
                        {
                            g.FillPath(glowBrush, glowPath);
                        }
                    }
                    else
                    {
                        g.FillRectangle(glowBrush, glowRect);
                    }
                }
            }
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
