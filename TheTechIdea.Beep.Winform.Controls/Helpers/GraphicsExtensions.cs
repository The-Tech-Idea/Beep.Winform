using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Extension methods for Graphics class to support Material Design drawing operations
    /// </summary>
    public static class GraphicsExtensions
    {
        public static GraphicsPath CreateRoundedRectanglePath(RectangleF     rect, CornerRadius radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(new RectangleF(rect.X, rect.Y, Math.Max(1, rect.Width), Math.Max(1, rect.Height)));
                return path;
            }
            int maxRadius = (int)(Math.Min(rect.Width, rect.Height) / 2);
            int tl = Math.Max(0, Math.Min(radius.TopLeft, maxRadius));
            int tr = Math.Max(0, Math.Min(radius.TopRight, maxRadius));
            int br = Math.Max(0, Math.Min(radius.BottomRight, maxRadius));
            int bl = Math.Max(0, Math.Min(radius.BottomLeft, maxRadius));
            if (tl == 0 && tr == 0 && br == 0 && bl == 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            if (tl > 0) path.AddArc(rect.X, rect.Y, tl * 2, tl * 2, 180, 90); else path.AddLine(rect.X, rect.Y, rect.X, rect.Y);
            if (tr > 0) path.AddArc(rect.Right - tr * 2, rect.Y, tr * 2, tr * 2, 270, 90); else path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);
            if (br > 0) path.AddArc(rect.Right - br * 2, rect.Bottom - br * 2, br * 2, br * 2, 0, 90); else path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);
            if (bl > 0) path.AddArc(rect.X, rect.Bottom - bl * 2, bl * 2, bl * 2, 90, 90); else path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);
            path.CloseFigure();
            return path;
        }

        public static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, CornerRadius radius)
        {
            var path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(new Rectangle(rect.X, rect.Y, Math.Max(1, rect.Width), Math.Max(1, rect.Height)));
                return path;
            }
            int maxRadius = (int)(Math.Min(rect.Width, rect.Height) / 2);
            int tl = Math.Max(0, Math.Min(radius.TopLeft, maxRadius));
            int tr = Math.Max(0, Math.Min(radius.TopRight, maxRadius));
            int br = Math.Max(0, Math.Min(radius.BottomRight, maxRadius));
            int bl = Math.Max(0, Math.Min(radius.BottomLeft, maxRadius));
            if (tl == 0 && tr == 0 && br == 0 && bl == 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            if (tl > 0) path.AddArc(rect.X, rect.Y, tl * 2, tl * 2, 180, 90); else path.AddLine(rect.X, rect.Y, rect.X, rect.Y);
            if (tr > 0) path.AddArc(rect.Right - tr * 2, rect.Y, tr * 2, tr * 2, 270, 90); else path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);
            if (br > 0) path.AddArc(rect.Right - br * 2, rect.Bottom - br * 2, br * 2, br * 2, 0, 90); else path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);
            if (bl > 0) path.AddArc(rect.X, rect.Bottom - bl * 2, bl * 2, bl * 2, 90, 90); else path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);
            path.CloseFigure();
            return path;
        }

        public static GraphicsPath CreateInsetPath(GraphicsPath originalPath, int inset)
        {
           
            return originalPath.CreateInsetPath((float)inset);
        }
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
        /// for more �modern� or �material� Style corners.
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
        /// Creates a hexagonal path (for Gaming Style)
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

        #region "Advanced Shape Creation"

        /// <summary>
        /// Creates a perfect circle path
        /// </summary>
        public static GraphicsPath CreateCircle(float centerX, float centerY, float radius)
        {
            var path = new GraphicsPath();
            path.AddEllipse(centerX - radius, centerY - radius, radius * 2, radius * 2);
            return path;
        }

        /// <summary>
        /// Creates an equilateral triangle path
        /// </summary>
        public static GraphicsPath CreateTriangle(float centerX, float centerY, float size, float rotation = 0f)
        {
            var path = new GraphicsPath();
            var points = new PointF[3];
            
            // Calculate triangle points (equilateral)
            float angleStep = 360f / 3f;
            float startAngle = -90f + rotation; // Start from top
            
            for (int i = 0; i < 3; i++)
            {
                float angle = (startAngle + angleStep * i) * (float)Math.PI / 180f;
                points[i] = new PointF(
                    centerX + size * (float)Math.Cos(angle),
                    centerY + size * (float)Math.Sin(angle)
                );
            }
            
            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Creates a star path with specified points
        /// </summary>
        public static GraphicsPath CreateStar(float centerX, float centerY, float outerRadius, float innerRadius, int points = 5, float rotation = 0f)
        {
            var path = new GraphicsPath();
            var starPoints = new PointF[points * 2];
            
            float angleStep = 360f / points;
            float startAngle = -90f + rotation;
            
            for (int i = 0; i < points * 2; i++)
            {
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;
                float angle = (startAngle + angleStep * (i / 2f)) * (float)Math.PI / 180f;
                
                starPoints[i] = new PointF(
                    centerX + radius * (float)Math.Cos(angle),
                    centerY + radius * (float)Math.Sin(angle)
                );
            }
            
            path.AddPolygon(starPoints);
            return path;
        }

        /// <summary>
        /// Creates a pentagon path
        /// </summary>
        public static GraphicsPath CreatePentagon(float centerX, float centerY, float size, float rotation = 0f)
        {
            return CreateRegularPolygon(centerX, centerY, size, 5, rotation);
        }

        /// <summary>
        /// Creates a hexagon path
        /// </summary>
        public static GraphicsPath CreateHexagon(float centerX, float centerY, float size, float rotation = 0f)
        {
            return CreateRegularPolygon(centerX, centerY, size, 6, rotation);
        }

        /// <summary>
        /// Creates an octagon path
        /// </summary>
        public static GraphicsPath CreateOctagon(float centerX, float centerY, float size, float rotation = 0f)
        {
            return CreateRegularPolygon(centerX, centerY, size, 8, rotation);
        }

        /// <summary>
        /// Creates a regular polygon with any number of sides
        /// </summary>
        public static GraphicsPath CreateRegularPolygon(float centerX, float centerY, float size, int sides, float rotation = 0f)
        {
            var path = new GraphicsPath();
            var points = new PointF[sides];
            
            float angleStep = 360f / sides;
            float startAngle = -90f + rotation;
            
            for (int i = 0; i < sides; i++)
            {
                float angle = (startAngle + angleStep * i) * (float)Math.PI / 180f;
                points[i] = new PointF(
                    centerX + size * (float)Math.Cos(angle),
                    centerY + size * (float)Math.Sin(angle)
                );
            }
            
            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Creates a diamond/rhombus path
        /// </summary>
        public static GraphicsPath CreateDiamond(float centerX, float centerY, float width, float height)
        {
            var path = new GraphicsPath();
            var points = new PointF[]
            {
                new PointF(centerX, centerY - height / 2),          // Top
                new PointF(centerX + width / 2, centerY),           // Right
                new PointF(centerX, centerY + height / 2),          // Bottom
                new PointF(centerX - width / 2, centerY)            // Left
            };
            
            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Creates an arrow path pointing in specified direction
        /// </summary>
        public static GraphicsPath CreateArrow(float x, float y, float width, float height, ArrowDirection direction = ArrowDirection.Right)
        {
            var path = new GraphicsPath();
            PointF[] points;
            
            switch (direction)
            {
                case ArrowDirection.Up:
                    points = new PointF[]
                    {
                        new PointF(x + width / 2, y),                    // Tip
                        new PointF(x + width, y + height * 0.6f),        // Right outer
                        new PointF(x + width * 0.7f, y + height * 0.6f), // Right inner
                        new PointF(x + width * 0.7f, y + height),        // Right bottom
                        new PointF(x + width * 0.3f, y + height),        // Left bottom
                        new PointF(x + width * 0.3f, y + height * 0.6f), // Left inner
                        new PointF(x, y + height * 0.6f)                 // Left outer
                    };
                    break;
                    
                case ArrowDirection.Down:
                    points = new PointF[]
                    {
                        new PointF(x + width / 2, y + height),           // Tip
                        new PointF(x, y + height * 0.4f),                // Left outer
                        new PointF(x + width * 0.3f, y + height * 0.4f), // Left inner
                        new PointF(x + width * 0.3f, y),                 // Left top
                        new PointF(x + width * 0.7f, y),                 // Right top
                        new PointF(x + width * 0.7f, y + height * 0.4f), // Right inner
                        new PointF(x + width, y + height * 0.4f)         // Right outer
                    };
                    break;
                    
                case ArrowDirection.Left:
                    points = new PointF[]
                    {
                        new PointF(x, y + height / 2),                   // Tip
                        new PointF(x + width * 0.6f, y),                 // Top outer
                        new PointF(x + width * 0.6f, y + height * 0.3f), // Top inner
                        new PointF(x + width, y + height * 0.3f),        // Top right
                        new PointF(x + width, y + height * 0.7f),        // Bottom right
                        new PointF(x + width * 0.6f, y + height * 0.7f), // Bottom inner
                        new PointF(x + width * 0.6f, y + height)         // Bottom outer
                    };
                    break;
                    
                case ArrowDirection.Right:
                default:
                    points = new PointF[]
                    {
                        new PointF(x + width, y + height / 2),           // Tip
                        new PointF(x + width * 0.4f, y + height),        // Bottom outer
                        new PointF(x + width * 0.4f, y + height * 0.7f), // Bottom inner
                        new PointF(x, y + height * 0.7f),                // Bottom left
                        new PointF(x, y + height * 0.3f),                // Top left
                        new PointF(x + width * 0.4f, y + height * 0.3f), // Top inner
                        new PointF(x + width * 0.4f, y)                  // Top outer
                    };
                    break;
            }
            
            path.AddPolygon(points);
            return path;
        }

        /// <summary>
        /// Creates a chevron path (V-shape arrow)
        /// </summary>
        public static GraphicsPath CreateChevron(float x, float y, float width, float height, ArrowDirection direction = ArrowDirection.Right, float thickness = 0.3f)
        {
            var path = new GraphicsPath();
            
            switch (direction)
            {
                case ArrowDirection.Up:
                    path.AddLine(x, y + height, x + width / 2, y);
                    path.AddLine(x + width / 2, y, x + width, y + height);
                    break;
                    
                case ArrowDirection.Down:
                    path.AddLine(x, y, x + width / 2, y + height);
                    path.AddLine(x + width / 2, y + height, x + width, y);
                    break;
                    
                case ArrowDirection.Left:
                    path.AddLine(x + width, y, x, y + height / 2);
                    path.AddLine(x, y + height / 2, x + width, y + height);
                    break;
                    
                case ArrowDirection.Right:
                    path.AddLine(x, y, x + width, y + height / 2);
                    path.AddLine(x + width, y + height / 2, x, y + height);
                    break;
            }
            
            return path;
        }

        /// <summary>
        /// Creates a pill/capsule shape
        /// </summary>
        public static GraphicsPath CreatePill(float x, float y, float width, float height)
        {
            var path = new GraphicsPath();
            float radius = Math.Min(width, height) / 2f;
            
            if (width > height)
            {
                // Horizontal pill
                path.AddArc(x, y, height, height, 90, 180);
                path.AddLine(x + radius, y + height, x + width - radius, y + height);
                path.AddArc(x + width - height, y, height, height, 270, 180);
                path.AddLine(x + width - radius, y, x + radius, y);
            }
            else
            {
                // Vertical pill
                path.AddArc(x, y, width, width, 180, 180);
                path.AddLine(x + width, y + radius, x + width, y + height - radius);
                path.AddArc(x, y + height - width, width, width, 0, 180);
                path.AddLine(x, y + height - radius, x, y + radius);
            }
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a speech bubble path
        /// </summary>
        public static GraphicsPath CreateSpeechBubble(float x, float y, float width, float height, float cornerRadius = 10f, float tailSize = 15f, SpeechBubbleTailPosition tailPosition = SpeechBubbleTailPosition.BottomLeft)
        {
            var path = new GraphicsPath();
            
            // Main rounded rectangle
            var mainRect = new RectangleF(x, y, width, height);
            
            // Add rounded rectangle
            path.AddArc(x, y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            path.AddArc(x + width - cornerRadius * 2, y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            path.AddArc(x + width - cornerRadius * 2, y + height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            
            // Add tail based on position
            switch (tailPosition)
            {
                case SpeechBubbleTailPosition.BottomLeft:
                    path.AddLine(x + width - cornerRadius, y + height, x + tailSize * 2, y + height);
                    path.AddLine(x + tailSize * 2, y + height, x + tailSize, y + height + tailSize);
                    path.AddLine(x + tailSize, y + height + tailSize, x + tailSize, y + height);
                    path.AddLine(x + tailSize, y + height, x + cornerRadius, y + height);
                    break;
                    
                case SpeechBubbleTailPosition.BottomRight:
                    path.AddLine(x + width - cornerRadius, y + height, x + width - tailSize, y + height);
                    path.AddLine(x + width - tailSize, y + height, x + width - tailSize, y + height + tailSize);
                    path.AddLine(x + width - tailSize, y + height + tailSize, x + width - tailSize * 2, y + height);
                    path.AddLine(x + width - tailSize * 2, y + height, x + cornerRadius, y + height);
                    break;
                    
                default:
                    path.AddLine(x + width - cornerRadius, y + height, x + cornerRadius, y + height);
                    break;
            }
            
            path.AddArc(x, y + height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            path.CloseFigure();
            
            return path;
        }

        /// <summary>
        /// Creates a cross/plus path
        /// </summary>
        public static GraphicsPath CreateCross(float centerX, float centerY, float size, float thickness = 0.3f)
        {
            var path = new GraphicsPath();
            float halfSize = size / 2f;
            float halfThickness = (size * thickness) / 2f;
            
            // Horizontal bar
            path.AddRectangle(new RectangleF(centerX - halfSize, centerY - halfThickness, size, size * thickness));
            
            // Vertical bar
            path.AddRectangle(new RectangleF(centerX - halfThickness, centerY - halfSize, size * thickness, size));
            
            return path;
        }

        /// <summary>
        /// Creates a heart shape path
        /// </summary>
        public static GraphicsPath CreateHeart(float centerX, float centerY, float size)
        {
            var path = new GraphicsPath();
            float width = size;
            float height = size * 0.9f;
            
            // Top left arc
            path.AddArc(centerX - width / 2, centerY - height / 3, width / 2, height / 2, 135, 225);
            
            // Top right arc
            path.AddArc(centerX, centerY - height / 3, width / 2, height / 2, 270, 225);
            
            // Bottom point
            path.AddLine(centerX + width / 2, centerY + height / 6, centerX, centerY + height / 2);
            path.AddLine(centerX, centerY + height / 2, centerX - width / 2, centerY + height / 6);
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a cloud shape path
        /// </summary>
        public static GraphicsPath CreateCloud(float x, float y, float width, float height)
        {
            var path = new GraphicsPath();
            
            // Cloud is composed of multiple circles
            float r1 = height / 3f;
            float r2 = height / 2.5f;
            float r3 = height / 3.5f;
            float r4 = height / 4f;
            
            path.AddEllipse(x, y + height / 3, r1 * 2, r1 * 2);
            path.AddEllipse(x + width / 4 - r2, y, r2 * 2, r2 * 2);
            path.AddEllipse(x + width / 2 - r2, y + height / 6, r2 * 2, r2 * 2);
            path.AddEllipse(x + width - r3 * 2, y + height / 4, r3 * 2, r3 * 2);
            path.AddEllipse(x + width - r4 * 2.5f, y + height / 2, r4 * 2, r4 * 2);
            
            return path;
        }

        /// <summary>
        /// Creates a gear/cog shape path
        /// </summary>
        public static GraphicsPath CreateGear(float centerX, float centerY, float outerRadius, float innerRadius, int teeth = 12, float toothDepth = 0.3f)
        {
            var path = new GraphicsPath();
            var points = new List<PointF>();
            
            float angleStep = 360f / teeth;
            float toothWidth = angleStep / 3f;
            
            for (int i = 0; i < teeth; i++)
            {
                float baseAngle = i * angleStep;
                
                // Outer tooth
                float angle1 = (baseAngle - toothWidth / 2) * (float)Math.PI / 180f;
                float angle2 = (baseAngle + toothWidth / 2) * (float)Math.PI / 180f;
                
                points.Add(new PointF(
                    centerX + outerRadius * (float)Math.Cos(angle1),
                    centerY + outerRadius * (float)Math.Sin(angle1)
                ));
                
                points.Add(new PointF(
                    centerX + outerRadius * (float)Math.Cos(angle2),
                    centerY + outerRadius * (float)Math.Sin(angle2)
                ));
                
                // Inner valley
                float angle3 = ((baseAngle + angleStep / 2) - toothWidth / 2) * (float)Math.PI / 180f;
                float angle4 = ((baseAngle + angleStep / 2) + toothWidth / 2) * (float)Math.PI / 180f;
                float valleyRadius = outerRadius * (1 - toothDepth);
                
                points.Add(new PointF(
                    centerX + valleyRadius * (float)Math.Cos(angle3),
                    centerY + valleyRadius * (float)Math.Sin(angle3)
                ));
                
                points.Add(new PointF(
                    centerX + valleyRadius * (float)Math.Cos(angle4),
                    centerY + valleyRadius * (float)Math.Sin(angle4)
                ));
            }
            
            path.AddPolygon(points.ToArray());
            
            // Add center hole
            path.AddEllipse(centerX - innerRadius, centerY - innerRadius, innerRadius * 2, innerRadius * 2);
            
            return path;
        }

        #endregion

        #region "Control-Specific Shapes"

        /// <summary>
        /// Creates a tab shape (for tab controls)
        /// </summary>
        public static GraphicsPath CreateTab(float x, float y, float width, float height, float cornerRadius = 8f, TabPosition position = TabPosition.Top)
        {
            var path = new GraphicsPath();
            
            switch (position)
            {
                case TabPosition.Top:
                    path.AddArc(x, y, cornerRadius * 2, cornerRadius * 2, 180, 90);
                    path.AddLine(x + cornerRadius, y, x + width - cornerRadius, y);
                    path.AddArc(x + width - cornerRadius * 2, y, cornerRadius * 2, cornerRadius * 2, 270, 90);
                    path.AddLine(x + width, y + cornerRadius, x + width, y + height);
                    path.AddLine(x + width, y + height, x, y + height);
                    path.AddLine(x, y + height, x, y + cornerRadius);
                    break;
                    
                case TabPosition.Bottom:
                    path.AddLine(x, y, x + width, y);
                    path.AddLine(x + width, y, x + width, y + height - cornerRadius);
                    path.AddArc(x + width - cornerRadius * 2, y + height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
                    path.AddLine(x + width - cornerRadius, y + height, x + cornerRadius, y + height);
                    path.AddArc(x, y + height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
                    path.AddLine(x, y + height - cornerRadius, x, y);
                    break;
                    
                case TabPosition.Left:
                    path.AddArc(x, y, cornerRadius * 2, cornerRadius * 2, 180, 90);
                    path.AddLine(x + cornerRadius, y, x + width, y);
                    path.AddLine(x + width, y, x + width, y + height);
                    path.AddLine(x + width, y + height, x + cornerRadius, y + height);
                    path.AddArc(x, y + height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
                    path.AddLine(x, y + height - cornerRadius, x, y + cornerRadius);
                    break;
                    
                case TabPosition.Right:
                    path.AddLine(x, y, x + width - cornerRadius, y);
                    path.AddArc(x + width - cornerRadius * 2, y, cornerRadius * 2, cornerRadius * 2, 270, 90);
                    path.AddLine(x + width, y + cornerRadius, x + width, y + height - cornerRadius);
                    path.AddArc(x + width - cornerRadius * 2, y + height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
                    path.AddLine(x + width - cornerRadius, y + height, x, y + height);
                    path.AddLine(x, y + height, x, y);
                    break;
            }
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a tag/label shape
        /// </summary>
        public static GraphicsPath CreateTag(float x, float y, float width, float height, float cornerRadius = 5f)
        {
            var path = new GraphicsPath();
            
            // Left side with rounded corners
            path.AddArc(x, y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            path.AddLine(x + cornerRadius, y, x + width - height / 3, y);
            
            // Right side arrow point
            path.AddLine(x + width - height / 3, y, x + width, y + height / 2);
            path.AddLine(x + width, y + height / 2, x + width - height / 3, y + height);
            
            path.AddLine(x + width - height / 3, y + height, x + cornerRadius, y + height);
            path.AddArc(x, y + height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            path.AddLine(x, y + height - cornerRadius, x, y + cornerRadius);
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a breadcrumb item shape
        /// </summary>
        public static GraphicsPath CreateBreadcrumb(float x, float y, float width, float height, float arrowSize = 10f)
        {
            var path = new GraphicsPath();
            
            path.AddLine(x, y, x + width - arrowSize, y);
            path.AddLine(x + width - arrowSize, y, x + width, y + height / 2);
            path.AddLine(x + width, y + height / 2, x + width - arrowSize, y + height);
            path.AddLine(x + width - arrowSize, y + height, x, y + height);
            
            if (x > arrowSize)
            {
                path.AddLine(x, y + height, x + arrowSize, y + height / 2);
                path.AddLine(x + arrowSize, y + height / 2, x, y);
            }
            else
            {
                path.AddLine(x, y + height, x, y);
            }
            
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Creates a notification badge circle
        /// </summary>
        public static GraphicsPath CreateBadge(float x, float y, float size)
        {
            var path = new GraphicsPath();
            path.AddEllipse(x, y, size, size);
            return path;
        }

        /// <summary>
        /// Creates a toggle switch track
        /// </summary>
        public static GraphicsPath CreateToggleTrack(float x, float y, float width, float height)
        {
            return CreatePill(x, y, width, height);
        }

        /// <summary>
        /// Creates a slider track
        /// </summary>
        public static GraphicsPath CreateSliderTrack(float x, float y, float width, float height, float cornerRadius = 2f)
        {
            return CreateRoundedRectanglePath(new RectangleF(x, y, width, height), cornerRadius);
        }

        /// <summary>
        /// Creates a slider thumb (circular or rectangular)
        /// </summary>
        public static GraphicsPath CreateSliderThumb(float centerX, float centerY, float size, bool circular = true)
        {
            if (circular)
            {
                return CreateCircle(centerX, centerY, size / 2);
            }
            else
            {
                return CreateRoundedRectanglePath(centerX - size / 2, centerY - size / 2, size, size, size / 4);
            }
        }

        /// <summary>
        /// Creates a dropdown arrow (small triangle)
        /// </summary>
        public static GraphicsPath CreateDropdownArrow(float x, float y, float size, bool pointDown = true)
        {
            var path = new GraphicsPath();
            
            if (pointDown)
            {
                path.AddPolygon(new PointF[]
                {
                    new PointF(x, y),
                    new PointF(x + size, y),
                    new PointF(x + size / 2, y + size * 0.6f)
                });
            }
            else
            {
                path.AddPolygon(new PointF[]
                {
                    new PointF(x, y + size * 0.6f),
                    new PointF(x + size, y + size * 0.6f),
                    new PointF(x + size / 2, y)
                });
            }
            
            return path;
        }

        #endregion

        #region "Path Manipulation Utilities"

        /// <summary>
        /// Merges multiple GraphicsPath objects into one
        /// </summary>
        public static GraphicsPath MergePaths(params GraphicsPath[] paths)
        {
            var merged = new GraphicsPath();
            foreach (var path in paths)
            {
                if (path != null)
                {
                    merged.AddPath(path, false);
                }
            }
            return merged;
        }

        /// <summary>
        /// Creates a border path (outline) from an existing path
        /// </summary>
        public static GraphicsPath CreateBorderPath(GraphicsPath sourcePath, float borderWidth)
        {
            var borderPath = (GraphicsPath)sourcePath.Clone();
            using (var pen = new Pen(Color.Black, borderWidth))
            {
                borderPath.Widen(pen);
            }
            return borderPath;
        }

        /// <summary>
        /// Rotates a GraphicsPath around a center point
        /// </summary>
        public static GraphicsPath RotatePath(GraphicsPath path, float centerX, float centerY, float degrees)
        {
            var rotated = (GraphicsPath)path.Clone();
            using (var matrix = new Matrix())
            {
                matrix.RotateAt(degrees, new PointF(centerX, centerY));
                rotated.Transform(matrix);
            }
            return rotated;
        }

        /// <summary>
        /// Scales a GraphicsPath from a center point
        /// </summary>
        public static GraphicsPath ScalePath(GraphicsPath path, float centerX, float centerY, float scaleX, float scaleY)
        {
            var scaled = (GraphicsPath)path.Clone();
            using (var matrix = new Matrix())
            {
                matrix.Translate(centerX, centerY);
                matrix.Scale(scaleX, scaleY);
                matrix.Translate(-centerX, -centerY);
                scaled.Transform(matrix);
            }
            return scaled;
        }

        /// <summary>
        /// Translates (moves) a GraphicsPath
        /// </summary>
        public static GraphicsPath TranslatePath(GraphicsPath path, float dx, float dy)
        {
            var translated = (GraphicsPath)path.Clone();
            using (var matrix = new Matrix())
            {
                matrix.Translate(dx, dy);
                translated.Transform(matrix);
            }
            return translated;
        }

        /// <summary>
        /// Calculates the centroid (center point) of a GraphicsPath
        /// </summary>
        public static PointF GetCentroid(this GraphicsPath path)
        {
            var points = path.PathPoints;
            if (points.Length == 0) return PointF.Empty;
            
            float cx = 0, cy = 0;
            foreach (var pt in points)
            {
                cx += pt.X;
                cy += pt.Y;
            }
            
            return new PointF(cx / points.Length, cy / points.Length);
        }

        /// <summary>
        /// Gets the area of a GraphicsPath
        /// </summary>
        public static float GetArea(this GraphicsPath path)
        {
            var bounds = path.GetBounds();
            return bounds.Width * bounds.Height;
        }

        /// <summary>
        /// Checks if a point is inside the GraphicsPath
        /// </summary>
        public static bool ContainsPoint(this GraphicsPath path, PointF point)
        {
            return path.IsVisible(point);
        }

        /// <summary>
        /// Creates a shadow path offset from original
        /// </summary>
        public static GraphicsPath CreateShadowPath(GraphicsPath path, float offsetX, float offsetY)
        {
            return TranslatePath(path, offsetX, offsetY);
        }

        #endregion

        #region "Gradient and Effect Helpers"

        /// <summary>
        /// Creates a linear gradient brush from a GraphicsPath bounds
        /// </summary>
        public static LinearGradientBrush CreateLinearGradientFromPath(GraphicsPath path, Color color1, Color color2, float angle = 90f)
        {
            var bounds = path.GetBounds();
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                bounds = new RectangleF(0, 0, 1, 1); // Fallback
            }
            
            return new LinearGradientBrush(bounds, color1, color2, angle);
        }

        /// <summary>
        /// Creates a radial gradient brush from a GraphicsPath
        /// </summary>
        public static PathGradientBrush CreateRadialGradientFromPath(GraphicsPath path, Color centerColor, Color edgeColor)
        {
            var brush = new PathGradientBrush(path);
            brush.CenterColor = centerColor;
            brush.SurroundColors = new Color[] { edgeColor };
            return brush;
        }

        /// <summary>
        /// Applies a glow effect to a GraphicsPath
        /// </summary>
        public static void DrawGlow(Graphics g, GraphicsPath path, Color glowColor, int glowSize = 10)
        {
            for (int i = glowSize; i > 0; i--)
            {
                int alpha = (int)(255 * (1.0f - (float)i / glowSize) * 0.3f);
                using (var pen = new Pen(Color.FromArgb(alpha, glowColor), i * 2))
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, path);
                }
            }
        }

        #endregion

        #region "Enumerations"

        public enum ArrowDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        public enum SpeechBubbleTailPosition
        {
            BottomLeft,
            BottomRight,
            TopLeft,
            TopRight
        }

        public enum TabPosition
        {
            Top,
            Bottom,
            Left,
            Right
        }

        #endregion

    }
}