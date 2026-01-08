using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Caching;
using TheTechIdea.Beep.Winform.Controls.Styling.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Styling.PathPainters
{
    /// <summary>
    /// Helper utilities for painting filled graphics paths
    /// Provides common methods for solid fills, gradients, and effects
    /// </summary>
    public static class PathPainterHelpers
    {
        // Static path cache for performance optimization
        private static readonly PathCache _pathCache = new PathCache(100);

        /// <summary>
        /// Creates a chat bubble-style path (speech balloon shape)
        /// </summary>
        public static GraphicsPath CreateChatBubblePath(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            // Create rounded rectangle for the main bubble
            int tailHeight = Math.Min(bounds.Height / 4, 15);
            int tailWidth = Math.Min(bounds.Width / 4, 20);

            Rectangle bubbleBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height - tailHeight);

            // Add rounded rectangle
            path = PathPainterHelpers.CreateRoundedRectangle(bubbleBounds, radius);

            // Add triangular tail pointing down
            Point[] tailPoints = new Point[]
            {
                new Point(bounds.X + bounds.Width / 3, bounds.Bottom - tailHeight),
                new Point(bounds.X + bounds.Width / 3 + tailWidth / 2, bounds.Bottom),
                new Point(bounds.X + bounds.Width / 3 + tailWidth, bounds.Bottom - tailHeight)
            };

            path.AddPolygon(tailPoints);
            path.CloseFigure();

            return path;
        }
        /// <summary>
        /// Creates a rounded rectangle path
        /// Uses PathCache for performance optimization when bounds and radius are reused
        /// </summary>
        public static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            // Use path cache for performance (returns a clone, caller must dispose)
            return _pathCache.GetRoundedRectanglePath(bounds, radius);
        }

        /// <summary>
        /// Creates a neumorphism-style path with soft, extruded appearance
        /// </summary>
        public static GraphicsPath CreateNeumorphismPath(Rectangle bounds, int radius)
        {
            // Neumorphism uses slightly larger radius for soft shadows
            return CreateRoundedRectangle(bounds, radius + 2);
        }

        /// <summary>
        /// Creates a gaming-style path with sharp, angular edges
        /// </summary>
        public static GraphicsPath CreateGamingPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            
            // Create angular, aggressive shape
            int cutSize = Math.Min(bounds.Width, bounds.Height) / 8;
            
            // Start from top-left
            path.AddLine(bounds.Left + cutSize, bounds.Top, bounds.Right - cutSize, bounds.Top);
            path.AddLine(bounds.Right - cutSize, bounds.Top, bounds.Right, bounds.Top + cutSize);
            path.AddLine(bounds.Right, bounds.Top + cutSize, bounds.Right, bounds.Bottom - cutSize);
            path.AddLine(bounds.Right, bounds.Bottom - cutSize, bounds.Right - cutSize, bounds.Bottom);
            path.AddLine(bounds.Right - cutSize, bounds.Bottom, bounds.Left + cutSize, bounds.Bottom);
            path.AddLine(bounds.Left + cutSize, bounds.Bottom, bounds.Left, bounds.Bottom - cutSize);
            path.AddLine(bounds.Left, bounds.Bottom - cutSize, bounds.Left, bounds.Top + cutSize);
            path.AddLine(bounds.Left, bounds.Top + cutSize, bounds.Left + cutSize, bounds.Top);
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a retro-style path with pixelated, 80s aesthetic
        /// </summary>
        public static GraphicsPath CreateRetroPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            
            // Create a slightly irregular shape reminiscent of retro design
            int irregularity = Math.Min(bounds.Width, bounds.Height) / 20;
            
            path.AddLine(bounds.Left + irregularity, bounds.Top, bounds.Right - irregularity, bounds.Top);
            path.AddLine(bounds.Right - irregularity, bounds.Top, bounds.Right, bounds.Top + irregularity);
            path.AddLine(bounds.Right, bounds.Top + irregularity, bounds.Right, bounds.Bottom - irregularity);
            path.AddLine(bounds.Right, bounds.Bottom - irregularity, bounds.Right - irregularity, bounds.Bottom);
            path.AddLine(bounds.Right - irregularity, bounds.Bottom, bounds.Left + irregularity, bounds.Bottom);
            path.AddLine(bounds.Left + irregularity, bounds.Bottom, bounds.Left, bounds.Bottom - irregularity);
            path.AddLine(bounds.Left, bounds.Bottom - irregularity, bounds.Left, bounds.Top + irregularity);
            path.AddLine(bounds.Left, bounds.Top + irregularity, bounds.Left + irregularity, bounds.Top);
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a cyberpunk-style path with neon glow effect simulation
        /// </summary>
        public static GraphicsPath CreateCyberpunkPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            
            // Sharp edges with slight bevels
            int bevel = Math.Min(bounds.Width, bounds.Height) / 16;
            
            path.AddLine(bounds.Left + bevel, bounds.Top, bounds.Right - bevel, bounds.Top);
            path.AddLine(bounds.Right - bevel, bounds.Top, bounds.Right, bounds.Top + bevel);
            path.AddLine(bounds.Right, bounds.Top + bevel, bounds.Right, bounds.Bottom - bevel);
            path.AddLine(bounds.Right, bounds.Bottom - bevel, bounds.Right - bevel, bounds.Bottom);
            path.AddLine(bounds.Right - bevel, bounds.Bottom, bounds.Left + bevel, bounds.Bottom);
            path.AddLine(bounds.Left + bevel, bounds.Bottom, bounds.Left, bounds.Bottom - bevel);
            path.AddLine(bounds.Left, bounds.Bottom - bevel, bounds.Left, bounds.Top + bevel);
            path.AddLine(bounds.Left, bounds.Top + bevel, bounds.Left + bevel, bounds.Top);
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a pill-shaped path (rounded rectangle with high radius)
        /// </summary>
        public static GraphicsPath CreatePillPath(Rectangle bounds)
        {
            int radius = Math.Min(bounds.Width, bounds.Height) / 2;
            return CreateRoundedRectangle(bounds, radius);
        }

        /// <summary>
        /// Creates a hexagonal path
        /// </summary>
        public static GraphicsPath CreateHexagonPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            
            int width = bounds.Width;
            int height = bounds.Height;
            int x = bounds.X;
            int y = bounds.Y;
            
            Point[] points = new Point[]
            {
                new Point(x + width / 4, y),
                new Point(x + 3 * width / 4, y),
                new Point(x + width, y + height / 2),
                new Point(x + 3 * width / 4, y + height),
                new Point(x + width / 4, y + height),
                new Point(x, y + height / 2)
            };
            
            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Creates a star-shaped path
        /// </summary>
        public static GraphicsPath CreateStarPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            
            int centerX = bounds.X + bounds.Width / 2;
            int centerY = bounds.Y + bounds.Height / 2;
            int outerRadius = Math.Min(bounds.Width, bounds.Height) / 2;
            int innerRadius = outerRadius / 2;
            
            Point[] points = new Point[10];
            
            for (int i = 0; i < 10; i++)
            {
                double angle = i * Math.PI / 5;
                int radius = (i % 2 == 0) ? outerRadius : innerRadius;
                points[i] = new Point(
                    centerX + (int)(radius * Math.Sin(angle)),
                    centerY - (int)(radius * Math.Cos(angle))
                );
            }
            
            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Creates a trapezoid path
        /// </summary>
        public static GraphicsPath CreateTrapezoidPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            
            int width = bounds.Width;
            int height = bounds.Height;
            int x = bounds.X;
            int y = bounds.Y;
            int inset = width / 4;
            
            Point[] points = new Point[]
            {
                new Point(x + inset, y),
                new Point(x + width - inset, y),
                new Point(x + width, y + height),
                new Point(x, y + height)
            };
            
            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Creates a diamond/rhombus path
        /// </summary>
        public static GraphicsPath CreateDiamondPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            
            int centerX = bounds.X + bounds.Width / 2;
            int centerY = bounds.Y + bounds.Height / 2;
            
            Point[] points = new Point[]
            {
                new Point(centerX, bounds.Y),
                new Point(bounds.Right, centerY),
                new Point(centerX, bounds.Bottom),
                new Point(bounds.Left, centerY)
            };
            
            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Creates a notched rectangle path (like macOS dock items)
        /// </summary>
        public static GraphicsPath CreateNotchedPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();
            
            int notchWidth = Math.Min(bounds.Width / 4, 20);
            int notchHeight = Math.Min(bounds.Height / 4, 10);
            int notchCenterX = bounds.X + bounds.Width / 2;
            
            // Top side with notch
            path.AddLine(bounds.Left, bounds.Top, notchCenterX - notchWidth / 2, bounds.Top);
            path.AddLine(notchCenterX - notchWidth / 2, bounds.Top, notchCenterX, bounds.Top + notchHeight);
            path.AddLine(notchCenterX, bounds.Top + notchHeight, notchCenterX + notchWidth / 2, bounds.Top);
            path.AddLine(notchCenterX + notchWidth / 2, bounds.Top, bounds.Right, bounds.Top);
            
            // Right side
            path.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
            
            // Bottom side
            path.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
            
            // Left side
            path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top);
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Paints a solid filled path with state support
        /// </summary>
        public static void PaintSolidPath(Graphics g, GraphicsPath path, Color fillColor, ControlState state = ControlState.Normal)
        {
            Color adjustedColor = ApplyState(fillColor, state);
            var fillBrush = PaintersFactory.GetSolidBrush(adjustedColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(fillBrush, path);
        }

        /// <summary>
        /// Paints a gradient filled path (custom direction)
        /// </summary>
        public static void PaintGradientPath(Graphics g, GraphicsPath path, Color color1, Color color2, LinearGradientMode mode, ControlState state = ControlState.Normal)
        {
            Color adjustedColor1 = ApplyState(color1, state);
            Color adjustedColor2 = ApplyState(color2, state);

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <=0 || bounds.Height <=0) return;

            var gradientBrush = PaintersFactory.GetLinearGradientBrush(bounds, adjustedColor1, adjustedColor2, mode);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(gradientBrush, path);
        }

        /// <summary>
        /// Paints a gradient filled path (vertical)
        /// </summary>
        public static void PaintGradientPath(Graphics g, GraphicsPath path, Color color1, Color color2, ControlState state = ControlState.Normal)
        {
            Color adjustedColor1 = ApplyState(color1, state);
            Color adjustedColor2 = ApplyState(color2, state);

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <=0 || bounds.Height <=0) return;

            var gradientBrush = PaintersFactory.GetLinearGradientBrush(bounds, adjustedColor1, adjustedColor2, LinearGradientMode.Vertical);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(gradientBrush, path);
        }

        /// <summary>
        /// Paints a gradient filled path (custom angle)
        /// </summary>
        public static void PaintGradientPath(Graphics g, GraphicsPath path, Color color1, Color color2, float angle, ControlState state = ControlState.Normal)
        {
            Color adjustedColor1 = ApplyState(color1, state);
            Color adjustedColor2 = ApplyState(color2, state);

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <=0 || bounds.Height <=0) return;

            var gradientBrush = PaintersFactory.GetLinearGradientBrush(bounds, adjustedColor1, adjustedColor2, angle);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPath(gradientBrush, path);
        }

        /// <summary>
        /// Paints a radial gradient filled path
        /// </summary>
        public static void PaintRadialGradientPath(Graphics g, GraphicsPath path, Color centerColor, Color edgeColor, ControlState state = ControlState.Normal)
        {
            Color adjustedCenter = ApplyState(centerColor, state);
            Color adjustedEdge = ApplyState(edgeColor, state);

            RectangleF bounds = path.GetBounds();
            if (bounds.Width <=0 || bounds.Height <=0) return;

            using (var gradientBrush = new PathGradientBrush(path))
            {
                gradientBrush.CenterColor = adjustedCenter;
                gradientBrush.SurroundColors = new Color[] { adjustedEdge };
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(gradientBrush, path);
            }
        }

        /// <summary>
        /// Applies state-based color modifications
        /// Uses HSL-based color manipulation for more natural results
        /// </summary>
        public static Color ApplyState(Color baseColor, ControlState state)
        {
            switch (state)
            {
                case ControlState.Hovered:
                    return ColorAccessibilityHelper.LightenColor(baseColor, 0.1f);
                case ControlState.Pressed:
                    return ColorAccessibilityHelper.DarkenColor(baseColor, 0.15f);
                case ControlState.Selected:
                    return ColorAccessibilityHelper.LightenColor(baseColor, 0.15f);
                case ControlState.Disabled:
                    return WithAlpha(baseColor, 100);
                case ControlState.Focused:
                    return ColorAccessibilityHelper.LightenColor(baseColor, 0.05f);
                default:
                    return baseColor;
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

        /// <summary>
        /// Creates a new color with specified alpha
        /// </summary>
        public static Color WithAlpha(Color color, int alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        /// <summary>
        /// Creates a new color with specified alpha unless the base color is Color.Empty; in that case, returns Color.Empty.
        /// </summary>
        public static Color WithAlphaIfNotEmpty(Color color, int alpha)
        {
            if (color == Color.Empty) return Color.Empty;
            return WithAlpha(color, alpha);
        }

        /// <summary>
        /// Creates a new color with specified RGB and alpha
        /// </summary>
        public static Color WithAlpha(int r, int g, int b, int alpha)
        {
            return Color.FromArgb(alpha, r, g, b);
        }

        /// <summary>
        /// Gets a color from Style or theme
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

