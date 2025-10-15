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
        /// Scales all points in a GraphicsPath toward its centroid by 'inset' pixels.
        /// Works for convex shapes, may distort concave/complex shapes.
        /// </summary>
        public static GraphicsPath ScalePathTowardCentroid(this GraphicsPath path, float inset)
        {
            var points = path.PathPoints;
            if (points.Length == 0) return path;

            // Calculate centroid
            float cx = 0, cy = 0;
            foreach (var pt in points)
            {
                cx += pt.X;
                cy += pt.Y;
            }
            cx /= points.Length;
            cy /= points.Length;

            // Scale each point toward centroid
            PointF[] newPoints = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                float dx = points[i].X - cx;
                float dy = points[i].Y - cy;
                float length = (float)Math.Sqrt(dx * dx + dy * dy);
                float scale = (length > 0) ? (length - inset) / length : 1f;
                newPoints[i] = new PointF(cx + dx * scale, cy + dy * scale);
            }

            var newPath = new GraphicsPath();
            newPath.AddPolygon(newPoints);
            return newPath;
        }

        /// <summary>
        /// Attempts to detect the basic shape type of a GraphicsPath (rectangle, ellipse, polygon, custom).
        /// </summary>
        public static string DetectShapeType(this GraphicsPath path)
        {
            var points = path.PathPoints;
            if (points.Length == 4)
                return "Rectangle";
            if (points.Length > 4 && path.PathTypes.All(t => t == 0))
                return "Polygon";
            var bounds = path.GetBounds();
            if (points.Length > 8 && Math.Abs(bounds.Width - bounds.Height) < 2)
                return "Ellipse";
            return "Custom";
        }

        /// <summary>
        /// Creates an inset path for any shape type using multiple strategies.
        /// This is the main entry point for creating inset paths.
        /// </summary>
        public static GraphicsPath CreateInsetPath(this GraphicsPath originalPath, float inset, int radius = 0)
        {
            if (originalPath == null || inset <= 0)
                return originalPath;

            var bounds = originalPath.GetBounds();
            var pointCount = originalPath.PointCount;

            // Strategy 1: Rectangle (4 points)
            if (pointCount == 4)
            {
                RectangleF innerRect = RectangleF.Inflate(bounds, -inset, -inset);
                var innerPath = new GraphicsPath();
                innerPath.AddRectangle(innerRect);
                return innerPath;
            }

            // Strategy 2: Rounded Rectangle (typically 8+ points with arcs)
            if (pointCount >= 8 && HasArcs(originalPath))
            {
                // Calculate new radius (reduced by inset)
                int newRadius = Math.Max(0, radius - (int)inset);
                Rectangle innerRect = Rectangle.Round(RectangleF.Inflate(bounds, -inset, -inset));
                return CreateRoundedRectanglePath(innerRect, newRadius);
            }

            // Strategy 3: Ellipse/Circle
            if (IsEllipse(originalPath, bounds))
            {
                RectangleF innerRect = RectangleF.Inflate(bounds, -inset, -inset);
                var innerPath = new GraphicsPath();
                innerPath.AddEllipse(innerRect);
                return innerPath;
            }

            // Strategy 4: Polygon with straight edges
            if (IsPolygon(originalPath))
            {
                return CreateInsetPolygon(originalPath, inset);
            }

            // Strategy 5: Fallback - Scale toward centroid (works for most convex shapes)
            return ScalePathTowardCentroid(originalPath, inset);
        }

        /// <summary>
        /// Creates an inset polygon by moving each edge inward perpendicular to the edge.
        /// Works well for convex polygons.
        /// </summary>
        public static GraphicsPath CreateInsetPolygon(this GraphicsPath originalPath, float inset)
        {
            var points = originalPath.PathPoints;
            if (points.Length < 3) return originalPath;

            List<PointF> insetPoints = new List<PointF>();

            for (int i = 0; i < points.Length; i++)
            {
                int prevIndex = (i - 1 + points.Length) % points.Length;
                int nextIndex = (i + 1) % points.Length;

                PointF prev = points[prevIndex];
                PointF current = points[i];
                PointF next = points[nextIndex];

                // Calculate edge normals
                PointF edge1Normal = GetPerpendicular(prev, current);
                PointF edge2Normal = GetPerpendicular(current, next);

                // Average the normals for this vertex
                PointF avgNormal = new PointF(
                    (edge1Normal.X + edge2Normal.X) / 2,
                    (edge1Normal.Y + edge2Normal.Y) / 2);

                // Normalize
                float length = (float)Math.Sqrt(avgNormal.X * avgNormal.X + avgNormal.Y * avgNormal.Y);
                if (length > 0)
                {
                    avgNormal.X /= length;
                    avgNormal.Y /= length;
                }

                // Move point inward
                insetPoints.Add(new PointF(
                    current.X + avgNormal.X * inset,
                    current.Y + avgNormal.Y * inset));
            }

            var insetPath = new GraphicsPath();
            if (insetPoints.Count > 0)
            {
                insetPath.AddPolygon(insetPoints.ToArray());
            }
            return insetPath;
        }

        /// <summary>
        /// Gets the perpendicular (inward-pointing normal) for an edge.
        /// </summary>
        private static PointF GetPerpendicular(PointF p1, PointF p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            if (length == 0) return new PointF(0, 0);

            // Perpendicular vector (rotated 90 degrees clockwise for inward normal)
            return new PointF(-dy / length, dx / length);
        }

        /// <summary>
        /// Checks if a path contains arc segments (indicates rounded rectangle or curved shape).
        /// </summary>
        private static bool HasArcs(GraphicsPath path)
        {
            var types = path.PathTypes;
            foreach (var type in types)
            {
                // PathPointType.Bezier (3) indicates curve segments
                if ((type & 3) == 3) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a path is likely an ellipse by comparing point distribution.
        /// </summary>
        private static bool IsEllipse(GraphicsPath path, RectangleF bounds)
        {
            var points = path.PathPoints;
            if (points.Length < 8) return false;

            // Check if points are evenly distributed around the perimeter
            float centerX = bounds.X + bounds.Width / 2;
            float centerY = bounds.Y + bounds.Height / 2;

            int quadrantCounts = 0;
            bool[] quadrants = new bool[4];

            foreach (var pt in points)
            {
                int quadrant = GetQuadrant(pt.X - centerX, pt.Y - centerY);
                if (!quadrants[quadrant])
                {
                    quadrants[quadrant] = true;
                    quadrantCounts++;
                }
            }

            // If points are in all 4 quadrants, likely an ellipse
            return quadrantCounts == 4;
        }

        /// <summary>
        /// Gets the quadrant of a point relative to center (0=top-right, 1=top-left, 2=bottom-left, 3=bottom-right).
        /// </summary>
        private static int GetQuadrant(float x, float y)
        {
            if (x >= 0 && y <= 0) return 0; // Top-right
            if (x < 0 && y <= 0) return 1;  // Top-left
            if (x < 0 && y > 0) return 2;   // Bottom-left
            return 3;                        // Bottom-right
        }

        /// <summary>
        /// Checks if a path is a polygon (all straight line segments).
        /// </summary>
        private static bool IsPolygon(GraphicsPath path)
        {
            var types = path.PathTypes;
            foreach (var type in types)
            {
                // PathPointType.Line (1) indicates straight line segments
                // If we find curves (3) or other types, it's not a simple polygon
                if ((type & 3) == 3) return false;
            }
            return true;
        }

        /// <summary>
        /// Creates an inset path using widening technique (experimental).
        /// This uses GraphicsPath.Widen to create an outline, then attempts to extract the inner region.
        /// Note: This is a best-effort approach and may not work perfectly for all shapes.
        /// </summary>
        public static GraphicsPath CreateInsetPathByWidening(this GraphicsPath originalPath, float inset)
        {
            if (originalPath == null || inset <= 0)
                return originalPath;

            try
            {
                // Clone the original path
                var workingPath = (GraphicsPath)originalPath.Clone();

                // Create a pen with width equal to inset * 2 (to get inset on both sides)
                using (Pen pen = new Pen(Color.Black, inset * 2))
                {
                    // Widen creates an outline path
                    workingPath.Widen(pen);

                    // The widened path now represents the border area
                    // We need to extract the inner portion
                    // This is a simplified approach - for complex shapes, this may not work perfectly
                    var bounds = workingPath.GetBounds();
                    RectangleF innerRect = RectangleF.Inflate(bounds, -inset, -inset);

                    var innerPath = new GraphicsPath();
                    innerPath.AddRectangle(innerRect);
                    return innerPath;
                }
            }
            catch
            {
                // If widening fails, fall back to centroid scaling
                return ScalePathTowardCentroid(originalPath, inset);
            }
        }

        // NOTE: Path-first overloads to avoid Rectangle/RectangleF usage in renderer code
        // Renderers should use these overloads to keep code "rectangle-free" by passing floats or paths.

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
        /// Fills a rounded rectangle with uniform corner radius
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="brush">Brush to fill with</param>
        /// <param name="rect">Rectangle bounds</param>
        /// <param name="radius">Corner radius for all corners</param>
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rect, float radius)
        {
            FillRoundedRectangle(graphics, brush, rect, radius, radius, radius, radius);
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
        /// Draws a rounded rectangle with uniform corner radius
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="pen">Pen to draw with</param>
        /// <param name="rect">Rectangle bounds</param>
        /// <param name="radius">Corner radius for all corners</param>
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle rect, float radius)
        {
            DrawRoundedRectangle(graphics, pen, rect, radius, radius, radius, radius);
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

        // Rectangle-free overload for creating rounded rectangles
        public static GraphicsPath CreateRoundedRectanglePath(float x, float y, float width, float height, float radius)
        {
            return CreateRoundedRectanglePath(new RectangleF(x, y, width, height), radius, radius, radius, radius);
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
        /// for more �modern� or �material� style corners.
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

        #region "GraphicsPath-based Caption Button Drawing"
        
        /// <summary>
        /// Creates a circular GraphicsPath from a rectangle bounds
        /// </summary>
        public static GraphicsPath CreateCirclePath(RectangleF bounds)
        {
            var path = new GraphicsPath();
            path.AddEllipse(bounds);
            return path;
        }

        /// <summary>
        /// Creates a circular GraphicsPath from center point and radius
        /// </summary>
        public static GraphicsPath CreateCirclePath(PointF center, float radius)
        {
            var path = new GraphicsPath();
            path.AddEllipse(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            return path;
        }

        /// <summary>
        /// Creates a rectangular GraphicsPath
        /// </summary>
        public static GraphicsPath CreateRectanglePath(RectangleF bounds)
        {
            var path = new GraphicsPath();
            path.AddRectangle(bounds);
            return path;
        }

        // Rectangle-free overload for creating rectangle paths
        public static GraphicsPath CreateRectanglePath(float x, float y, float width, float height)
        {
            var path = new GraphicsPath();
            path.AddLine(x, y, x + width, y);
            path.AddLine(x + width, y, x + width, y + height);
            path.AddLine(x + width, y + height, x, y + height);
            path.AddLine(x, y + height, x, y);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a rounded rectangle GraphicsPath with uniform radius
        /// </summary>
        public static GraphicsPath CreateRoundedRectanglePath(RectangleF bounds, float radius)
        {
            return CreateRoundedRectanglePath(bounds, radius, radius, radius, radius);
        }

        /// <summary>
        /// Fills a circular path
        /// </summary>
        public static void FillCircle(this Graphics graphics, Brush brush, RectangleF bounds)
        {
            using (var path = CreateCirclePath(bounds))
            {
                graphics.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Fills a circular path from center and radius
        /// </summary>
        public static void FillCircle(this Graphics graphics, Brush brush, PointF center, float radius)
        {
            using (var path = CreateCirclePath(center, radius))
            {
                graphics.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Draws a circular outline
        /// </summary>
        public static void DrawCircle(this Graphics graphics, Pen pen, RectangleF bounds)
        {
            using (var path = CreateCirclePath(bounds))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Draws a circular outline from center and radius
        /// </summary>
        public static void DrawCircle(this Graphics graphics, Pen pen, PointF center, float radius)
        {
            using (var path = CreateCirclePath(center, radius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Fills a rectangular path
        /// </summary>
        public static void FillRectanglePath(this Graphics graphics, Brush brush, RectangleF bounds)
        {
            using (var path = CreateRectanglePath(bounds))
            {
                graphics.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Draws a rectangular outline path
        /// </summary>
        public static void DrawRectanglePath(this Graphics graphics, Pen pen, RectangleF bounds)
        {
            using (var path = CreateRectanglePath(bounds))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Fills a rounded rectangle path
        /// </summary>
        public static void FillRoundedRectanglePath(this Graphics graphics, Brush brush, RectangleF bounds, float radius)
        {
            using (var path = CreateRoundedRectanglePath(bounds, radius))
            {
                graphics.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Draws a rounded rectangle outline path
        /// </summary>
        public static void DrawRoundedRectanglePath(this Graphics graphics, Pen pen, RectangleF bounds, float radius)
        {
            using (var path = CreateRoundedRectanglePath(bounds, radius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Creates a hexagonal path (for Gaming style)
        /// </summary>
        public static GraphicsPath CreateHexagonPath(RectangleF bounds, float cutSize)
        {
            var path = new GraphicsPath();
            var points = new PointF[]
            {
                new PointF(bounds.Left + cutSize, bounds.Top),
                new PointF(bounds.Right - cutSize, bounds.Top),
                new PointF(bounds.Right, bounds.Top + cutSize),
                new PointF(bounds.Right, bounds.Bottom - cutSize),
                new PointF(bounds.Right - cutSize, bounds.Bottom),
                new PointF(bounds.Left + cutSize, bounds.Bottom),
                new PointF(bounds.Left, bounds.Bottom - cutSize),
                new PointF(bounds.Left, bounds.Top + cutSize)
            };
            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Fills a hexagonal path
        /// </summary>
        public static void FillHexagon(this Graphics graphics, Brush brush, RectangleF bounds, float cutSize)
        {
            using (var path = CreateHexagonPath(bounds, cutSize))
            {
                graphics.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Draws a hexagonal outline
        /// </summary>
        public static void DrawHexagon(this Graphics graphics, Pen pen, RectangleF bounds, float cutSize)
        {
            using (var path = CreateHexagonPath(bounds, cutSize))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Creates a line path for minimize button
        /// </summary>
        public static GraphicsPath CreateMinimizeLine(RectangleF bounds, float inset, float yPosition = 0.58f)
        {
            var path = new GraphicsPath();
            float y = bounds.Y + bounds.Height * yPosition;
            path.AddLine(bounds.Left + inset, y, bounds.Right - inset, y);
            return path;
        }

        /// <summary>
        /// Draws a minimize line
        /// </summary>
        public static void DrawMinimizeLine(this Graphics graphics, Pen pen, RectangleF bounds, float inset, float yPosition = 0.58f)
        {
            using (var path = CreateMinimizeLine(bounds, inset, yPosition))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Creates a maximize rectangle path
        /// </summary>
        public static GraphicsPath CreateMaximizeRect(RectangleF bounds, float inset)
        {
            var path = new GraphicsPath();
            var rect = new RectangleF(
                bounds.Left + inset,
                bounds.Top + inset,
                bounds.Width - inset * 2,
                bounds.Height - inset * 2);
            path.AddRectangle(rect);
            return path;
        }

        /// <summary>
        /// Draws a maximize rectangle
        /// </summary>
        public static void DrawMaximizeRect(this Graphics graphics, Pen pen, RectangleF bounds, float inset)
        {
            using (var path = CreateMaximizeRect(bounds, inset))
            {
                graphics.DrawPath(pen, path);
            }
        }

        // Restore (overlapped squares) glyph
        public static GraphicsPath CreateRestoreRect(RectangleF bounds, float inset, float offset = 3f)
        {
            var path = new GraphicsPath();
            var r1 = new RectangleF(bounds.Left + inset, bounds.Top + inset, bounds.Width - inset * 2, bounds.Height - inset * 2);
            var r2 = new RectangleF(r1.Left + offset, r1.Top + offset, r1.Width, r1.Height);
            path.AddRectangle(r1);
            path.AddRectangle(r2);
            return path;
        }

        public static void DrawRestoreRect(this Graphics graphics, Pen pen, RectangleF bounds, float inset, float offset = 3f)
        {
            using (var path = CreateRestoreRect(bounds, inset, offset))
            {
                graphics.DrawPath(pen, path);
            }
        }

        // Path-first glyph overloads
        public static void DrawMinimizeLine(this Graphics graphics, Pen pen, GraphicsPath boundsPath, float inset, float yPosition = 0.58f)
        {
            var b = boundsPath.GetBounds();
            graphics.DrawMinimizeLine(pen, b, inset, yPosition);
        }

        public static void DrawMaximizeRect(this Graphics graphics, Pen pen, GraphicsPath boundsPath, float inset)
        {
            var b = boundsPath.GetBounds();
            graphics.DrawMaximizeRect(pen, b, inset);
        }

        public static void DrawRestoreRect(this Graphics graphics, Pen pen, GraphicsPath boundsPath, float inset, float offset = 3f)
        {
            var b = boundsPath.GetBounds();
            graphics.DrawRestoreRect(pen, b, inset, offset);
        }

        public static void DrawCloseX(this Graphics graphics, Pen pen, GraphicsPath boundsPath, float inset)
        {
            var b = boundsPath.GetBounds();
            graphics.DrawCloseX(pen, b, inset);
        }

        /// <summary>
        /// Creates a close X path
        /// </summary>
        public static GraphicsPath CreateCloseX(RectangleF bounds, float inset)
        {
            var path = new GraphicsPath();
            // Diagonal line 1: top-left to bottom-right
            path.AddLine(bounds.Left + inset, bounds.Top + inset, bounds.Right - inset, bounds.Bottom - inset);
            // Start new figure for second line
            path.StartFigure();
            // Diagonal line 2: top-right to bottom-left
            path.AddLine(bounds.Right - inset, bounds.Top + inset, bounds.Left + inset, bounds.Bottom - inset);
            return path;
        }

        /// <summary>
        /// Draws a close X
        /// </summary>
        public static void DrawCloseX(this Graphics graphics, Pen pen, RectangleF bounds, float inset)
        {
            using (var path = CreateCloseX(bounds, inset))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Creates a union of multiple rectangular paths
        /// </summary>
        public static GraphicsPath CreateUnionPath(params RectangleF[] rectangles)
        {
            var path = new GraphicsPath();
            foreach (var rect in rectangles)
            {
                path.AddRectangle(rect);
            }
            return path;
        }

        // Union path for GraphicsPath inputs (no Rectangle exposure in renderer code)
        public static GraphicsPath CreateUnionPath(params GraphicsPath[] paths)
        {
            var union = new GraphicsPath();
            foreach (var p in paths)
            {
                if (p is null) continue;
                union.AddPath(p, false);
            }
            return union;
        }

        /// <summary>
        /// Converts a Rectangle to GraphicsPath
        /// </summary>
        public static GraphicsPath ToGraphicsPath(this Rectangle rect)
        {
            var path = new GraphicsPath();
            path.AddRectangle(rect);
            return path;
        }

        /// <summary>
        /// Converts a RectangleF to GraphicsPath
        /// </summary>
        public static GraphicsPath ToGraphicsPath(this RectangleF rect)
        {
            var path = new GraphicsPath();
            path.AddRectangle(rect);
            return path;
        }
        /// <summary>
        /// Create rounded rectangle path
        /// </summary>
        private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
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

            // Top left
            path.AddArc(arc, 180, 90);
            // Top right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            // Bottom right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            // Bottom left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
        /// <summary>
        /// Gets bounds from GraphicsPath as Rectangle
        /// </summary>
        public static Rectangle GetBoundsRect(this GraphicsPath path)
        {
            return Rectangle.Round(path.GetBounds());
        }

        // Rectangle-free overloads for rectangle fills/draws
        public static void FillRectanglePath(this Graphics graphics, Brush brush, float x, float y, float width, float height)
        {
            using (var path = new GraphicsPath())
            {
                path.AddLine(x, y, x + width, y);
                path.AddLine(x + width, y, x + width, y + height);
                path.AddLine(x + width, y + height, x, y + height);
                path.AddLine(x, y + height, x, y);
                path.CloseFigure();
                graphics.FillPath(brush, path);
            }
        }

        public static void DrawRectanglePath(this Graphics graphics, Pen pen, float x, float y, float width, float height)
        {
            using (var path = new GraphicsPath())
            {
                path.AddLine(x, y, x + width, y);
                path.AddLine(x + width, y, x + width, y + height);
                path.AddLine(x + width, y + height, x, y + height);
                path.AddLine(x, y + height, x, y);
                path.CloseFigure();
                graphics.DrawPath(pen, path);
            }
        }

        public static void FillRoundedRectanglePath(this Graphics graphics, Brush brush, float x, float y, float width, float height, float radius)
        {
            using (var path = CreateRoundedRectanglePath(new RectangleF(x, y, width, height), radius, radius, radius, radius))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void DrawRoundedRectanglePath(this Graphics graphics, Pen pen, float x, float y, float width, float height, float radius)
        {
            using (var path = CreateRoundedRectanglePath(new RectangleF(x, y, width, height), radius, radius, radius, radius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Inflates a GraphicsPath bounds by creating a new path
        /// </summary>
        public static GraphicsPath InflatePath(this GraphicsPath originalPath, float x, float y)
        {
            var bounds = originalPath.GetBounds();
            bounds.Inflate(x, y);
            var path = new GraphicsPath();
            path.AddRectangle(bounds);
            return path;
        }

        #endregion

    }
}