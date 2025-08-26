using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Extension methods for Graphics class to support Material Design drawing operations
    /// </summary>
    public static class GraphicsExtensions
    {
        /// <summary>
        /// Fills a rounded rectangle with specified corner radii
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="brush">Brush to fill with</param>
        /// <param name="rect">Rectangle bounds</param>
        /// <param name="topLeftRadius">Top-left corner radius</param>
        /// <param name="topRightRadius">Top-right corner radius</param>
        /// <param name="bottomLeftRadius">Bottom-left corner radius</param>
        /// <param name="bottomRightRadius">Bottom-right corner radius</param>
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, RectangleF rect, 
            float topLeftRadius, float topRightRadius, float bottomLeftRadius, float bottomRightRadius)
        {
            using (var path = CreateRoundedRectanglePath(rect, topLeftRadius, topRightRadius, bottomLeftRadius, bottomRightRadius))
            {
                graphics.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Draws a rounded rectangle with specified corner radii
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="pen">Pen to draw with</param>
        /// <param name="rect">Rectangle bounds</param>
        /// <param name="topLeftRadius">Top-left corner radius</param>
        /// <param name="topRightRadius">Top-right corner radius</param>
        /// <param name="bottomLeftRadius">Bottom-left corner radius</param>
        /// <param name="bottomRightRadius">Bottom-right corner radius</param>
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, RectangleF rect, 
            float topLeftRadius, float topRightRadius, float bottomLeftRadius, float bottomRightRadius)
        {
            using (var path = CreateRoundedRectanglePath(rect, topLeftRadius, topRightRadius, bottomLeftRadius, bottomRightRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Creates a GraphicsPath for a rounded rectangle with specified corner radii
        /// </summary>
        /// <param name="rect">Rectangle bounds</param>
        /// <param name="topLeftRadius">Top-left corner radius</param>
        /// <param name="topRightRadius">Top-right corner radius</param>
        /// <param name="bottomLeftRadius">Bottom-left corner radius</param>
        /// <param name="bottomRightRadius">Bottom-right corner radius</param>
        /// <returns>GraphicsPath for the rounded rectangle</returns>
        public static GraphicsPath CreateRoundedRectanglePath(RectangleF rect, 
            float topLeftRadius, float topRightRadius, float bottomLeftRadius, float bottomRightRadius)
        {
            var path = new GraphicsPath();

            // Ensure radii don't exceed rectangle dimensions
            float maxRadius = Math.Min(rect.Width, rect.Height) / 2f;
            topLeftRadius = Math.Min(topLeftRadius, maxRadius);
            topRightRadius = Math.Min(topRightRadius, maxRadius);
            bottomLeftRadius = Math.Min(bottomLeftRadius, maxRadius);
            bottomRightRadius = Math.Min(bottomRightRadius, maxRadius);

            // Top-left corner
            if (topLeftRadius > 0)
            {
                path.AddArc(rect.X, rect.Y, topLeftRadius * 2, topLeftRadius * 2, 180, 90);
            }
            else
            {
                path.AddLine(rect.X, rect.Y, rect.X, rect.Y);
            }

            // Top edge
            path.AddLine(rect.X + topLeftRadius, rect.Y, rect.Right - topRightRadius, rect.Y);

            // Top-right corner
            if (topRightRadius > 0)
            {
                path.AddArc(rect.Right - topRightRadius * 2, rect.Y, topRightRadius * 2, topRightRadius * 2, 270, 90);
            }

            // Right edge
            path.AddLine(rect.Right, rect.Y + topRightRadius, rect.Right, rect.Bottom - bottomRightRadius);

            // Bottom-right corner
            if (bottomRightRadius > 0)
            {
                path.AddArc(rect.Right - bottomRightRadius * 2, rect.Bottom - bottomRightRadius * 2, 
                    bottomRightRadius * 2, bottomRightRadius * 2, 0, 90);
            }

            // Bottom edge
            path.AddLine(rect.Right - bottomRightRadius, rect.Bottom, rect.X + bottomLeftRadius, rect.Bottom);

            // Bottom-left corner
            if (bottomLeftRadius > 0)
            {
                path.AddArc(rect.X, rect.Bottom - bottomLeftRadius * 2, bottomLeftRadius * 2, bottomLeftRadius * 2, 90, 90);
            }

            // Left edge
            path.AddLine(rect.X, rect.Bottom - bottomLeftRadius, rect.X, rect.Y + topLeftRadius);

            path.CloseFigure();
            return path;
        }

        #region "Drawing gradiants"
        ///// <summary>
        ///// Draws modern gradient backgrounds with various professional styles
        ///// </summary>
        //public static void DrawModernGradient(Graphics g, Color baseColor,Rectangle DrawingRect,bool isrounded, ModernGradientType modernGradientType,bool UseGlassmorphism)
        //{
        //    Rectangle drawRect = DrawingRect;
        //    if (DrawingRect.Width == 0 || DrawingRect.Height == 0) return;
        //    switch (modernGradientType)
        //    {
        //        case ModernGradientType.Subtle:
        //            DrawSubtleGradient(g, drawRect, baseColor);
        //            break;

        //        case ModernGradientType.Linear:
        //            DrawLinearGradient(g, drawRect, baseColor,);
        //            break;

        //        case ModernGradientType.Radial:
        //            DrawRadialGradient(g, drawRect, baseColor);
        //            break;

        //        case ModernGradientType.Conic:
        //            DrawConicGradient(g, drawRect, baseColor);
        //            break;

        //        case ModernGradientType.Mesh:
        //            DrawMeshGradient(g, drawRect, baseColor);
        //            break;
        //    }

        //    // Apply glassmorphism if enabled
        //    if (UseGlassmorphism)
        //    {
        //        ApplyGlassmorphism(g, drawRect);
        //    }
        //}

        /// <summary>
        /// Draws a subtle, professional gradient
        /// </summary>
        public static void DrawSubtleGradient(Graphics g, Rectangle rect, Color baseColor,int GradientAngle,bool IsRounded,int BorderRadius)
        {
            // Create a very subtle gradient for modern, professional look
            Color color1 = baseColor;
            Color color2;

            // Calculate subtle color variations based on brightness
            float brightness = baseColor.GetBrightness();
            const float subtleFactor = 0.05f; // Increased from 0.03f for more visibility

            if (brightness > 0.5f)
            {
                // Light color -> create subtle shadow effect
                color2 = Color.FromArgb(
                    Math.Max(0, baseColor.R - (int)(255 * subtleFactor)),
                    Math.Max(0, baseColor.G - (int)(255 * subtleFactor)),
                    Math.Max(0, baseColor.B - (int)(255 * subtleFactor)));
            }
            else
            {
                // Dark color -> create subtle highlight effect
                color2 = Color.FromArgb(
                    Math.Min(255, baseColor.R + (int)(255 * subtleFactor)),
                    Math.Min(255, baseColor.G + (int)(255 * subtleFactor)),
                    Math.Min(255, baseColor.B + (int)(255 * subtleFactor)));
            }

            // Create gradient with custom angle
            float angleRadians = (float)(GradientAngle * Math.PI / 180f);

            using (LinearGradientBrush gradientBrush = CreateAngledGradientBrush(rect, color1, color2, angleRadians))
            {
                // Add sophisticated color blend for smooth transitions
                ColorBlend blend = new ColorBlend();
                blend.Colors = new Color[] { color1, BlendColors(color1, color2, 0.5f), color2 };
                blend.Positions = new float[] { 0.0f, 0.3f, 1.0f };
                gradientBrush.InterpolationColors = blend;

                FillShape(g, gradientBrush, rect,IsRounded,BorderRadius);
            }
        }

        /// <summary>
        /// Draws a linear gradient with custom angle support
        /// </summary>
        public static void DrawLinearGradient(Graphics g, Rectangle rect, Color baseColor,Color GradientStartColor, Color GradientEndColor, PointF RadialCenter, bool IsRounded, int BorderRadius,int GradientAngle, List<GradientStop> GradientStops )
        {
            Color startColor = GradientStartColor;
            Color endColor = GradientEndColor;

            // Use base color if gradient colors aren't set
            if (startColor == Color.Gray && endColor == Color.Gray)
            {
                startColor = baseColor;
                endColor = ModifyColorBrightness(baseColor, 0.8f);
            }

            float angleRadians = (float)(GradientAngle * Math.PI / 180f);

            using (LinearGradientBrush gradientBrush = CreateAngledGradientBrush(rect, startColor, endColor, angleRadians))
            {
                // Add multi-stop gradient if gradient stops are defined
                if (GradientStops.Count > 0)
                {
                    ApplyGradientStops(gradientBrush,GradientStops);
                }

                FillShape(g, gradientBrush, rect, IsRounded, BorderRadius);
            }
        }
        public static void ApplyGradientStops(LinearGradientBrush brush, List<GradientStop> GradientStops)
        {
            if (GradientStops.Count < 2) return;

            // Sort gradient stops by position
            var sortedStops = GradientStops.OrderBy(s => s.Position).ToList();

            ColorBlend blend = new ColorBlend();
            blend.Colors = sortedStops.Select(s => s.Color).ToArray();
            blend.Positions = sortedStops.Select(s => s.Position).ToArray();

            brush.InterpolationColors = blend;
        }
        public static LinearGradientBrush CreateAngledGradientBrush(Rectangle rect, Color color1, Color color2, float angleRadians)
        {
            // Calculate gradient direction based on angle
            float cos = (float)Math.Cos(angleRadians); // Fixed: Cast to float
            float sin = (float)Math.Sin(angleRadians); // Fixed: Cast to float

            PointF start = new PointF(
                rect.X + rect.Width * (0.5f - cos * 0.5f),
                rect.Y + rect.Height * (0.5f - sin * 0.5f));

            PointF end = new PointF(
                rect.X + rect.Width * (0.5f + cos * 0.5f),
                rect.Y + rect.Height * (0.5f + sin * 0.5f));

            return new LinearGradientBrush(start, end, color1, color2);
        }
        /// <summary>
        /// Draws a radial gradient from center outward
        /// </summary>
        public static void DrawRadialGradient(Graphics g, Rectangle rect, Color baseColor,Color GradientStartColor,Color GradientEndColor, PointF RadialCenter,bool IsRounded, int BorderRadius)
        {
            Color centerColor = GradientStartColor != Color.Gray ? GradientStartColor : baseColor;
            Color edgeColor = GradientEndColor != Color.Gray ? GradientEndColor : ModifyColorBrightness(baseColor, 0.7f);

            // Calculate center point
            PointF center = new PointF(
                rect.X + rect.Width * RadialCenter.X,
                rect.Y + rect.Height * RadialCenter.Y);

            // Calculate radius
            float radius = Math.Max(rect.Width, rect.Height) * 0.7f;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(center.X - radius, center.Y - radius, radius * 2, radius * 2);

                using (PathGradientBrush gradientBrush = new PathGradientBrush(path))
                {
                    gradientBrush.CenterColor = centerColor;
                    gradientBrush.SurroundColors = new Color[] { edgeColor };
                    gradientBrush.CenterPoint = center;

                    FillShape(g, gradientBrush, rect,IsRounded,BorderRadius);
                }
            }
        }

        /// <summary>
        /// Draws a conic (angular) gradient - simulated with multiple linear gradients
        /// </summary>
        public static void DrawConicGradient(Graphics g, Rectangle rect, Color baseColor,int GradientAngle)
        {
            // Simulate conic gradient using multiple segments
            PointF center = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
            int segments = 36; // 10-degree segments

            for (int i = 0; i < segments; i++)
            {
                float startAngle = (i * 360f / segments) + GradientAngle;
                float endAngle = ((i + 1) * 360f / segments) + GradientAngle;

                // Calculate color for this segment
                float hue = (startAngle % 360f) / 360f;
                Color segmentColor = ColorFromHSV(hue, 0.5f, baseColor.GetBrightness());

                using (SolidBrush segmentBrush = new SolidBrush(Color.FromArgb(100, segmentColor)))
                {
                    g.FillPie(segmentBrush, rect, startAngle, 360f / segments);
                }
            }
        }

        /// <summary>
        /// Draws a mesh gradient with multiple control points
        /// </summary>
        public static void DrawMeshGradient(Graphics g, Rectangle rect, Color baseColor)
        {
            // Create a 3x3 mesh gradient effect
            int gridSize = 3;
            Color[,] colorGrid = new Color[gridSize, gridSize];

            // Initialize color grid with variations of base color
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    float brightness = 0.7f + (0.3f * ((x + y) / (float)(gridSize * 2)));
                    colorGrid[x, y] = ModifyColorBrightness(baseColor, brightness);
                }
            }

            // Draw gradient patches
            float cellWidth = rect.Width / (float)(gridSize - 1);
            float cellHeight = rect.Height / (float)(gridSize - 1);

            for (int x = 0; x < gridSize - 1; x++)
            {
                for (int y = 0; y < gridSize - 1; y++)
                {
                    RectangleF cellRect = new RectangleF(
                        rect.X + x * cellWidth,
                        rect.Y + y * cellHeight,
                        cellWidth * 1.5f, // Overlap cells
                        cellHeight * 1.5f);

                    using (LinearGradientBrush cellBrush = new LinearGradientBrush(
                        cellRect, colorGrid[x, y], colorGrid[x + 1, y + 1], LinearGradientMode.ForwardDiagonal))
                    {
                        g.FillRectangle(cellBrush, cellRect);
                    }
                }
            }
        }

        /// <summary>
        /// Applies glassmorphism effect over the background
        /// </summary>
        public static void ApplyGlassmorphism(Graphics g, Rectangle rect, int BorderRadius, bool IsRounded, float GlassmorphismOpacity)
        {
            // Create translucent overlay with noise pattern
            using (SolidBrush glassBrush = new SolidBrush(Color.FromArgb(
                (int)(255 * GlassmorphismOpacity), Color.White)))
            {
                // Apply subtle noise pattern for glass effect
                Random random = new Random(42); // Fixed seed for consistent pattern

                for (int i = 0; i < rect.Width * rect.Height / 1000; i++)
                {
                    int x = random.Next(rect.X, rect.X + rect.Width);
                    int y = random.Next(rect.Y, rect.Y + rect.Height);

                    using (SolidBrush noiseBrush = new SolidBrush(Color.FromArgb(
                        random.Next(5, 15), Color.White)))
                    {
                        g.FillRectangle(noiseBrush, x, y, 1, 1);
                    }
                }

                // Apply overall glass overlay
                FillShape(g, glassBrush, rect,IsRounded, BorderRadius);
            }
        }

        #region "Helper Methods"



        /// <summary>
        /// Fills a shape (rounded or rectangular) with the specified brush
        /// </summary>
        public static void FillShape(Graphics g, Brush brush, Rectangle rect,bool IsRounded,int BorderRadius)
        {
            if (IsRounded)
            {
                using (GraphicsPath path = GetRoundedRectPath(rect, BorderRadius))
                {
                    g.FillPath(brush, path);
                }
            }
            else
            {
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Modifies color brightness
        /// </summary>
        public static Color ModifyColorBrightness(Color color, float brightness)
        {
            return Color.FromArgb(
                color.A,
                (int)(color.R * brightness),
                (int)(color.G * brightness),
                (int)(color.B * brightness));
        }

        /// <summary>
        /// Blends two colors by the specified amount
        /// </summary>
        public static Color BlendColors(Color color1, Color color2, float amount)
        {
            return Color.FromArgb(
                (int)(color1.R + (color2.R - color1.R) * amount),
                (int)(color1.G + (color2.G - color1.G) * amount),
                (int)(color1.B + (color2.B - color1.B) * amount));
        }

        /// <summary>
        /// Creates a color from HSV values
        /// </summary>
        public static Color ColorFromHSV(float hue, float saturation, double brightness)
        {
            hue = hue * 360f;
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            double v = brightness;
            double p = brightness * (1 - saturation);
            double q = brightness * (1 - f * saturation);
            double t = brightness * (1 - (1 - f) * saturation);

            switch (hi)
            {
                case 0: return Color.FromArgb(255, (int)(v * 255), (int)(t * 255), (int)(p * 255));
                case 1: return Color.FromArgb(255, (int)(q * 255), (int)(v * 255), (int)(p * 255));
                case 2: return Color.FromArgb(255, (int)(p * 255), (int)(v * 255), (int)(t * 255));
                case 3: return Color.FromArgb(255, (int)(p * 255), (int)(q * 255), (int)(v * 255));
                case 4: return Color.FromArgb(255, (int)(t * 255), (int)(p * 255), (int)(v * 255));
                default: return Color.FromArgb(255, (int)(v * 255), (int)(p * 255), (int)(q * 255));
            }
        }

        
       

        public static GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            // Ensure the radius is valid relative to the rectangle's dimensions
            int diameter = Math.Min(Math.Min(radius * 2, rect.Width), rect.Height);

            if (diameter > 0)
            {
                // Add arcs and lines for rounded rectangle
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
            }
            else
            {
                // Fallback to a regular rectangle if diameter is zero
                path.AddRectangle(rect);
            }

            return path;
            //   return GetEllipticalRoundedRectPath(rect, radius, radius);
        }
        /// <summary>
        /// Creates a GraphicsPath for a rectangle with elliptical corners
        /// allowing different horizontal and vertical radii. This can be used
        /// for more “modern” or “material” style corners.
        /// </summary>
        /// <param name="rect">The overall bounding rectangle.</param>
        /// <param name="radiusX">Horizontal radius for corners.</param>
        /// <param name="radiusY">Vertical radius for corners.</param>
        /// <returns>A GraphicsPath defining the elliptical-corner rectangle.</returns>
        public static GraphicsPath GetEllipticalRoundedRectPath(Rectangle rect, int radiusX, int radiusY)
        {
            GraphicsPath path = new GraphicsPath();

            // Cap the radii so we don't exceed rect dimensions
            int diameterX = Math.Min(radiusX * 2, rect.Width);
            int diameterY = Math.Min(radiusY * 2, rect.Height);

            // If either diameter is 0, fallback to a plain rectangle
            if (diameterX <= 0 || diameterY <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // top-left corner
            path.AddArc(rect.X, rect.Y, diameterX, diameterY, 180, 90);

            // top-right corner
            path.AddArc(rect.Right - diameterX, rect.Y, diameterX, diameterY, 270, 90);

            // bottom-right corner
            path.AddArc(rect.Right - diameterX, rect.Bottom - diameterY, diameterX, diameterY, 0, 90);

            // bottom-left corner
            path.AddArc(rect.X, rect.Bottom - diameterY, diameterX, diameterY, 90, 90);

            path.CloseFigure();
            return path;
        }
        #endregion
        #endregion "Drawing gradiants"

    }
}