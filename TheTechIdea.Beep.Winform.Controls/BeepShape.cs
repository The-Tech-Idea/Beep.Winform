using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepShape))]
    [Category("Beep Controls")]
    [Description("Advanced shapes with connection support and interactive features")]
    [DisplayName("Beep Shape")]
    public class BeepShape : BeepControl
    {
        #region Fields
        private bool isDragging = false;
        private Point dragStart;
        private bool isResizing = false;
        private ResizeHandle selectedHandle = ResizeHandle.None;
        private const int HandleSize = 8;
        private GraphicsPath cachedPath;
        private bool pathNeedsUpdate = true;
        private Point[] connectionPoints;
        private string _shapeText = "";
        #endregion

        #region Enhanced Properties
        [Browsable(true)]
        [Category("Shape")]
        [Description("Type of shape to draw")]
        public BeepShapeType ShapeType { get; set; } = BeepShapeType.Rectangle;

        [Browsable(true)]
        [Category("Shape")]
        [Description("Rotation angle in degrees")]
        public float RotationAngle { get; set; } = 0f;

        [Browsable(true)]
        [Category("Shape")]
        [Description("Number of sides for polygon shapes")]
        public int PolygonSides { get; set; } = 6;

        [Browsable(true)]
        [Category("Shape")]
        [Description("Number of points for star shapes")]
        public int StarPoints { get; set; } = 5;

        [Browsable(true)]
        [Category("Shape")]
        [Description("Inner radius ratio for star shapes (0.0 to 1.0)")]
        public float StarInnerRatio { get; set; } = 0.5f;

        [Browsable(true)]
        [Category("Shape")]
        [Description("Corner radius for rounded shapes")]
        public int CornerRadius { get; set; } = 10;

        [Browsable(true)]
        [Category("Shape")]
        [Description("Arrow head size for line shapes")]
        public int ArrowHeadSize { get; set; } = 12;

        [Browsable(true)]
        [Category("Shape")]
        [Description("Show arrow head on line end")]
        public bool ShowArrowHead { get; set; } = false;

        [Browsable(true)]
        [Category("Shape")]
        [Description("Text to display inside the shape")]
        public string ShapeText
        {
            get => _shapeText;
            set { _shapeText = value; Invalidate(); }
        }

        [Browsable(true)]
        [Category("Connection")]
        [Description("Control connected to the start of this shape")]
        public BeepControl ConnectedStart { get; set; }

        [Browsable(true)]
        [Category("Connection")]
        [Description("Control connected to the end of this shape")]
        public BeepControl ConnectedEnd { get; set; }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow shape to be dragged")]
        public bool AllowDrag { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow shape to be resized")]
        public bool AllowResize { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show resize handles when selected")]
        public bool ShowResizeHandles { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show connection points when selected")]
        public bool ShowConnectionPoints { get; set; } = true;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Fill style for the shape")]
        public ShapeFillStyle FillStyle { get; set; } = ShapeFillStyle.Solid;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Line style for the shape outline")]
        public DashStyle LineStyle { get; set; } = DashStyle.Solid;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Gradient start color")]
        public Color GradientStartColor { get; set; } = Color.White;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Gradient end color")]
        public Color GradientEndColor { get; set; } = Color.LightGray;
        #endregion

        #region Constructor
        public BeepShape()
        {
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.UserPaint |
                         ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            Size = new Size(100, 100);
            UpdateConnectionPoints();
            SetupHitAreas();
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // Update cached path if needed
            if (pathNeedsUpdate)
            {
                UpdateCachedPath();
                pathNeedsUpdate = false;
            }

            // Apply rotation transform if needed
            var state = g.Save();
            if (RotationAngle != 0)
            {
                var centerPoint = new PointF(Width / 2f, Height / 2f);
                g.TranslateTransform(centerPoint.X, centerPoint.Y);
                g.RotateTransform(RotationAngle);
                g.TranslateTransform(-centerPoint.X, -centerPoint.Y);
            }

            // Draw the shape
            DrawShapeContent(g);

            // Draw text if provided
            if (!string.IsNullOrEmpty(ShapeText))
            {
                DrawShapeText(g);
            }

            g.Restore(state);

            // Draw connection points if selected (outside rotation)
            if (IsSelected && ShowConnectionPoints)
            {
                DrawConnectionPoints(g);
            }

            // Draw resize handles if enabled and selected (outside rotation)
            if (ShowResizeHandles && AllowResize && IsSelected)
            {
                DrawResizeHandles(g);
            }

            // Draw connections to other shapes
            DrawConnections(g);
        }

        private void DrawShapeContent(Graphics g)
        {
            using (var pen = new Pen(BorderColor, BorderThickness))
            using (var brush = CreateFillBrush())
            {
                pen.DashStyle = LineStyle;
                pen.LineJoin = LineJoin.Round;

                switch (ShapeType)
                {
                    case BeepShapeType.Line:
                        DrawLine(g, pen);
                        break;
                    case BeepShapeType.Rectangle:
                        DrawRectangle(g, pen, brush);
                        break;
                    case BeepShapeType.Ellipse:
                        DrawEllipse(g, pen, brush);
                        break;
                    case BeepShapeType.Triangle:
                        DrawTriangle(g, pen, brush);
                        break;
                    case BeepShapeType.Star:
                        DrawStar(g, pen, brush);
                        break;
                    // Add new shapes
                    case BeepShapeType.Diamond:
                        DrawDiamond(g, pen, brush);
                        break;
                    case BeepShapeType.Pentagon:
                        DrawPolygon(g, pen, brush, 5);
                        break;
                    case BeepShapeType.Hexagon:
                        DrawPolygon(g, pen, brush, 6);
                        break;
                    case BeepShapeType.Octagon:
                        DrawPolygon(g, pen, brush, 8);
                        break;
                }
            }
        }

        private void DrawLine(Graphics g, Pen pen)
        {
            var start = GetStartPoint();
            var end = GetEndPoint();
            g.DrawLine(pen, start, end);

            if (ShowArrowHead)
            {
                DrawArrowHead(g, pen, start, end);
            }
        }

        private void DrawRectangle(Graphics g, Pen pen, Brush brush)
        {
            if (CornerRadius > 0)
            {
                using (var path = GetRoundedRectPath(DrawingRect, CornerRadius))
                {
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }
            }
            else
            {
                g.FillRectangle(brush, DrawingRect);
                g.DrawRectangle(pen, DrawingRect);
            }
        }

        private void DrawEllipse(Graphics g, Pen pen, Brush brush)
        {
            g.FillEllipse(brush, DrawingRect);
            g.DrawEllipse(pen, DrawingRect);
        }

        private void DrawTriangle(Graphics g, Pen pen, Brush brush)
        {
            var points = new[]
            {
                new Point(Width / 2, 0),
                new Point(0, Height),
                new Point(Width, Height)
            };
            g.FillPolygon(brush, points);
            g.DrawPolygon(pen, points);
        }

        private void DrawDiamond(Graphics g, Pen pen, Brush brush)
        {
            var points = new[]
            {
                new Point(Width / 2, 0),           // Top
                new Point(Width, Height / 2),     // Right
                new Point(Width / 2, Height),     // Bottom
                new Point(0, Height / 2)          // Left
            };
            g.FillPolygon(brush, points);
            g.DrawPolygon(pen, points);
        }

        private void DrawPolygon(Graphics g, Pen pen, Brush brush, int sides)
        {
            var points = GeneratePolygonPoints(sides);
            g.FillPolygon(brush, points);
            g.DrawPolygon(pen, points);
        }

        private void DrawStar(Graphics g, Pen pen, Brush brush)
        {
            var points = GenerateStarPoints();
            g.FillPolygon(brush, points);
            g.DrawPolygon(pen, points);
        }

        private void DrawShapeText(Graphics g)
        {
            if (string.IsNullOrEmpty(ShapeText)) return;

            using (var brush = new SolidBrush(ForeColor))
            using (var format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                var textRect = new RectangleF(0, 0, Width, Height);
                g.DrawString(ShapeText, Font, brush, textRect, format);
            }
        }

        private void DrawArrowHead(Graphics g, Pen pen, Point start, Point end)
        {
            var angle = Math.Atan2(end.Y - start.Y, end.X - start.X);
            var arrowAngle1 = angle + Math.PI / 6;
            var arrowAngle2 = angle - Math.PI / 6;

            var arrowPoint1 = new Point(
                (int)(end.X - ArrowHeadSize * Math.Cos(arrowAngle1)),
                (int)(end.Y - ArrowHeadSize * Math.Sin(arrowAngle1)));

            var arrowPoint2 = new Point(
                (int)(end.X - ArrowHeadSize * Math.Cos(arrowAngle2)),
                (int)(end.Y - ArrowHeadSize * Math.Sin(arrowAngle2)));

            g.DrawLine(pen, end, arrowPoint1);
            g.DrawLine(pen, end, arrowPoint2);
        }

        private void DrawConnectionPoints(Graphics g)
        {
            if (connectionPoints == null) return;

            using (var brush = new SolidBrush(Color.Blue))
            using (var pen = new Pen(Color.White, 2))
            {
                foreach (var point in connectionPoints)
                {
                    var rect = new Rectangle(point.X - 3, point.Y - 3, 6, 6);
                    g.FillEllipse(brush, rect);
                    g.DrawEllipse(pen, rect);
                }
            }
        }

        private void DrawResizeHandles(Graphics g)
        {
            using (var brush = new SolidBrush(Color.White))
            using (var pen = new Pen(Color.Black, 1))
            {
                var handles = GetResizeHandles();
                foreach (var handle in handles)
                {
                    g.FillRectangle(brush, handle);
                    g.DrawRectangle(pen, handle);
                }
            }
        }

        private void DrawConnections(Graphics g)
        {
            if (ConnectedStart != null)
            {
                using (var pen = new Pen(Color.Gray, 2) { DashStyle = DashStyle.Dash })
                {
                    var start = ConnectedStart.Location;
                    var end = this.Location;
                    g.DrawLine(pen, start, end);
                }
            }
        }
        #endregion

        #region Helper Methods
        private Brush CreateFillBrush()
        {
            switch (FillStyle)
            {
                case ShapeFillStyle.Solid:
                    return new SolidBrush(BackColor);

                case ShapeFillStyle.Gradient:
                    return new LinearGradientBrush(
                        DrawingRect,
                        GradientStartColor,
                        GradientEndColor,
                        LinearGradientMode.Vertical);

                case ShapeFillStyle.Hatch:
                    return new HatchBrush(HatchStyle.Cross, BorderColor, BackColor);

                case ShapeFillStyle.None:
                    return new SolidBrush(Color.Transparent);

                default:
                    return new SolidBrush(BackColor);
            }
        }

        private PointF[] GeneratePolygonPoints(int sides)
        {
            var points = new PointF[sides];
            var centerX = Width / 2f;
            var centerY = Height / 2f;
            var radius = Math.Min(Width, Height) / 2f;

            for (int i = 0; i < sides; i++)
            {
                var angle = i * 2 * Math.PI / sides - Math.PI / 2; // Start from top
                points[i] = new PointF(
                    (float)(centerX + radius * Math.Cos(angle)),
                    (float)(centerY + radius * Math.Sin(angle)));
            }

            return points;
        }

        private PointF[] GenerateStarPoints()
        {
            var points = new PointF[StarPoints * 2];
            var centerX = Width / 2f;
            var centerY = Height / 2f;
            var outerRadius = Math.Min(Width, Height) / 2f;
            var innerRadius = outerRadius * StarInnerRatio;

            for (int i = 0; i < StarPoints * 2; i++)
            {
                var angle = i * Math.PI / StarPoints - Math.PI / 2;
                var radius = i % 2 == 0 ? outerRadius : innerRadius;

                points[i] = new PointF(
                    (float)(centerX + radius * Math.Cos(angle)),
                    (float)(centerY + radius * Math.Sin(angle)));
            }

            return points;
        }

        private void UpdateCachedPath()
        {
            cachedPath?.Dispose();
            cachedPath = new GraphicsPath();

            switch (ShapeType)
            {
                case BeepShapeType.Rectangle:
                    if (CornerRadius > 0)
                        cachedPath = GetRoundedRectPath(DrawingRect, CornerRadius);
                    else
                        cachedPath.AddRectangle(DrawingRect);
                    break;

                case BeepShapeType.Ellipse:
                    cachedPath.AddEllipse(DrawingRect);
                    break;

                case BeepShapeType.Triangle:
                    var trianglePoints = new[]
                    {
                        new Point(Width / 2, 0),
                        new Point(0, Height),
                        new Point(Width, Height)
                    };
                    cachedPath.AddPolygon(trianglePoints);
                    break;

                case BeepShapeType.Star:
                    cachedPath.AddPolygon(GenerateStarPoints());
                    break;

                case BeepShapeType.Diamond:
                    var diamondPoints = new[]
                    {
                        new Point(Width / 2, 0),
                        new Point(Width, Height / 2),
                        new Point(Width / 2, Height),
                        new Point(0, Height / 2)
                    };
                    cachedPath.AddPolygon(diamondPoints);
                    break;
            }
        }

        private void UpdateConnectionPoints()
        {
            connectionPoints = new[]
            {
                new Point(Width / 2, 0),          // Top
                new Point(Width, Height / 2),    // Right
                new Point(Width / 2, Height),    // Bottom
                new Point(0, Height / 2)         // Left
            };
        }

        private void SetupHitAreas()
        {
            ClearHitList();

            // Add hit area for the main shape
            AddHitArea("Shape", DrawingRect, this, () => OnShapeClicked());

            // Add hit areas for resize handles if enabled
            if (AllowResize && ShowResizeHandles && IsSelected)
            {
                SetupResizeHandleHitAreas();
            }

            // Add hit areas for connection points if enabled
            if (ShowConnectionPoints && IsSelected && connectionPoints != null)
            {
                SetupConnectionPointHitAreas();
            }
        }

        private void SetupResizeHandleHitAreas()
        {
            var handles = GetResizeHandles();
            var handleNames = new[] { "TopLeft", "TopRight", "BottomLeft", "BottomRight" };

            for (int i = 0; i < Math.Min(handles.Length, handleNames.Length); i++)
            {
                int handleIndex = i;
                AddHitArea(handleNames[i], handles[i], this, () => OnResizeHandleClicked(handleIndex));
            }
        }

        private void SetupConnectionPointHitAreas()
        {
            for (int i = 0; i < connectionPoints.Length; i++)
            {
                var point = connectionPoints[i];
                var rect = new Rectangle(point.X - 5, point.Y - 5, 10, 10);
                int pointIndex = i;
                AddHitArea($"ConnectionPoint_{i}", rect, this, () => OnConnectionPointClicked(pointIndex));
            }
        }

        private Rectangle[] GetResizeHandles()
        {
            return new[]
            {
                new Rectangle(0, 0, HandleSize, HandleSize),
                new Rectangle(Width - HandleSize, 0, HandleSize, HandleSize),
                new Rectangle(0, Height - HandleSize, HandleSize, HandleSize),
                new Rectangle(Width - HandleSize, Height - HandleSize, HandleSize, HandleSize)
            };
        }

        private Point GetStartPoint()
        {
            if (ConnectedStart != null)
            {
                var startCenter = new Point(
                    ConnectedStart.Left + ConnectedStart.Width / 2,
                    ConnectedStart.Top + ConnectedStart.Height / 2);
                return PointToClient(Parent?.PointToScreen(startCenter) ?? startCenter);
            }
            return new Point(0, Height / 2);
        }

        private Point GetEndPoint()
        {
            if (ConnectedEnd != null)
            {
                var endCenter = new Point(
                    ConnectedEnd.Left + ConnectedEnd.Width / 2,
                    ConnectedEnd.Top + ConnectedEnd.Height / 2);
                return PointToClient(Parent?.PointToScreen(endCenter) ?? endCenter);
            }
            return new Point(Width, Height / 2);
        }
        #endregion

        #region Event Handlers
        private void OnShapeClicked()
        {
            IsSelected = !IsSelected;
            SetupHitAreas(); // Update hit areas based on selection state
            Invalidate();
        }

        private void OnResizeHandleClicked(int handleIndex)
        {
            selectedHandle = (ResizeHandle)handleIndex;
            isResizing = true;
        }

        private void OnConnectionPointClicked(int pointIndex)
        {
            // Handle connection point interactions
            // This could trigger a connection mode or show a context menu
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && AllowDrag && !isResizing)
            {
                isDragging = true;
                dragStart = e.Location;
                Cursor = Cursors.SizeAll;
                this.BringToFront();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isDragging && AllowDrag)
            {
                int dx = e.X - dragStart.X;
                int dy = e.Y - dragStart.Y;
                Location = new Point(Location.X + dx, Location.Y + dy);
            }
            else if (isResizing && AllowResize)
            {
                HandleResize(e.Location);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                isResizing = false;
                selectedHandle = ResizeHandle.None;
                Cursor = Cursors.Default;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            pathNeedsUpdate = true;
            UpdateConnectionPoints();
            SetupHitAreas();
            Invalidate();
        }

        private void HandleResize(Point location)
        {
            switch (selectedHandle)
            {
                case ResizeHandle.BottomRight:
                    Size = new Size(Math.Max(20, location.X), Math.Max(20, location.Y));
                    break;
                    // Add other resize handle logic as needed
            }
        }
        #endregion

        #region Public Methods
        public void ConnectTo(BeepControl target, bool asStart = true)
        {
            if (asStart)
                ConnectedStart = target;
            else
                ConnectedEnd = target;

            Invalidate();
        }

        public void Disconnect(bool fromStart = true)
        {
            if (fromStart)
                ConnectedStart = null;
            else
                ConnectedEnd = null;

            Invalidate();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme != null)
            {
                BorderColor = _currentTheme.BorderColor;
                GradientStartColor = _currentTheme.GradientStartColor;
                GradientEndColor = _currentTheme.GradientEndColor;
                Invalidate();
            }
        }
        #endregion

        #region Cleanup
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                cachedPath?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }

   
}