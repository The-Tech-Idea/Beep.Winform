using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.ConnectionManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum ConnectionPointShapeType
    {
        Square,
        Circle,
        Triangle,
        Diamond,
        Hexagon,
        Octagon,
        Custom
    }
    public enum ConnectionPointState
    {
        None,
        Connected,
        Hovered
    }
    public enum ConnectionPointAction
    {
        None,
        Dragging,
        Dropped
    }
    public enum ConnectionPointPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }
    [ToolboxItem(false)]
    public class ConnectionPointControl : Control
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        // Reference to the connection manager
        private ConnectionManager _connectionManager;

        public ConnectionManager ConnectionManager
        {
            get => _connectionManager;
            set => _connectionManager = value;
        }

        // Add a property to get the center point of this connection
        public Point CenterPoint => new Point(Width / 2, Height / 2);
        public Control OwnerControl { get; set; } // BeepPanel or child control
        public int PointIndex { get; set; } // RowIndex in connection points (0: Top, 1: Bottom, 2: Left, 3: Right)
        private bool _isDragging;

        public event EventHandler<ConnectionPointDragEventArgs> DragStarted;
        public event EventHandler<ConnectionPointDragEventArgs> DragEnded;

        public event EventHandler<ConnectionPointDragEventArgs> DragMoved;
        public event EventHandler<ConnectionPointDragEventArgs> DragCancelled;

        public ConnectionPointShapeType ConnectionPointType { get; set; } = ConnectionPointShapeType.Square; // Default shape
        public ConnectionPointState ConnectionPointState { get; set; } = ConnectionPointState.None; // Default state
        public ConnectionPointAction ConnectionPointAction { get; set; } = ConnectionPointAction.None; // Default action
        public Dictionary<string,string> ConnectedControls { get; set; } = new Dictionary<string, string>(); // Dictionary to hold connected controls <key, name> pairs
        public ConnectionPointControl()
        {
            Size = new Size(8, 8); // Small size for connection point
           
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(BackColor);

            // Define colors based on state
            Color fillColor;
            Color borderColor = Color.Black;

            switch (ConnectionPointState)
            {
                case ConnectionPointState.Connected:
                    fillColor = Color.Green;
                    break;
                case ConnectionPointState.Hovered:
                    fillColor = Color.LightBlue;
                    break;
                default:
                    fillColor = Color.Gray;
                    break;
            }

            // Create a pen for the border
            using (Pen borderPen = new Pen(borderColor, 1))
            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            {
                GraphicsPath path = CreateShapePath();

                // Fill and draw the shape
                g.FillPath(fillBrush, path);
                g.DrawPath(borderPen, path);

                // Set the control's region to the shape for proper hit testing
                Region = new Region(path);

                path.Dispose();
            }
        }

        private GraphicsPath CreateShapePath()
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);

            switch (ConnectionPointType)
            {
                case ConnectionPointShapeType.Circle:
                    path.AddEllipse(bounds);
                    break;

                case ConnectionPointShapeType.Triangle:
                    Point[] trianglePoints = {
                new Point(bounds.Width / 2, 0),
                new Point(bounds.Width, bounds.Height),
                new Point(0, bounds.Height)
            };
                    path.AddPolygon(trianglePoints);
                    break;

                case ConnectionPointShapeType.Diamond:
                    Point[] diamondPoints = {
                new Point(bounds.Width / 2, 0),
                new Point(bounds.Width, bounds.Height / 2),
                new Point(bounds.Width / 2, bounds.Height),
                new Point(0, bounds.Height / 2)
            };
                    path.AddPolygon(diamondPoints);
                    break;

                case ConnectionPointShapeType.Hexagon:
                    int sixth = bounds.Width / 6;
                    Point[] hexagonPoints = {
                new Point(sixth, 0),
                new Point(bounds.Width - sixth, 0),
                new Point(bounds.Width, bounds.Height / 2),
                new Point(bounds.Width - sixth, bounds.Height),
                new Point(sixth, bounds.Height),
                new Point(0, bounds.Height / 2)
            };
                    path.AddPolygon(hexagonPoints);
                    break;

                case ConnectionPointShapeType.Octagon:
                    int quarter = bounds.Width / 4;
                    Point[] octagonPoints = {
                new Point(quarter, 0),
                new Point(bounds.Width - quarter, 0),
                new Point(bounds.Width, quarter),
                new Point(bounds.Width, bounds.Height - quarter),
                new Point(bounds.Width - quarter, bounds.Height),
                new Point(quarter, bounds.Height),
                new Point(0, bounds.Height - quarter),
                new Point(0, quarter)
            };
                    path.AddPolygon(octagonPoints);
                    break;

                case ConnectionPointShapeType.Custom:
                    // Add custom shape logic here if needed
                    // For now, default to a square
                    path.AddRectangle(bounds);
                    break;

                case ConnectionPointShapeType.Square:
                default:
                    path.AddRectangle(bounds);
                    break;
            }

            return path;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                ConnectionPointAction = ConnectionPointAction.Dragging;
                Capture = true;
                // Invoke DragStarted with the correct point location
                DragStarted?.Invoke(this, new ConnectionPointDragEventArgs(this, PointToScreen(e.Location)));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Update hovered state when not dragging
            if (!_isDragging)
            {
                ConnectionPointState = ConnectionPointState.Hovered;
                Invalidate(); // Trigger repaint with hover state
            }
            else if (_isDragging)
            {
                // Invoke DragMoved rather than DragStarted during movement
                DragMoved?.Invoke(this, new ConnectionPointDragEventArgs(this, PointToScreen(e.Location)));
            }
        }


        // Add to the end of your OnMouseUp method
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isDragging)
            {
                _isDragging = false;
                ConnectionPointAction = ConnectionPointAction.Dropped;
                Capture = false;

                // Find if we've dropped on another ConnectionPointControl
                Point screenPoint = PointToScreen(e.Location);
                foreach (Control control in Parent.Controls)
                {
                    if (control is ConnectionPointControl targetPoint && targetPoint != this)
                    {
                        Point targetScreenPoint = targetPoint.PointToScreen(Point.Empty);
                        Rectangle targetRect = new Rectangle(targetScreenPoint, targetPoint.Size);

                        if (targetRect.Contains(screenPoint))
                        {
                            // We've found a target connection point
                            if (_connectionManager != null)
                            {
                                _connectionManager.AddConnection(this, targetPoint);
                            }
                            break;
                        }
                    }
                }

                DragEnded?.Invoke(this, new ConnectionPointDragEventArgs(this, PointToScreen(e.Location)));
            }
        }

        // Adding MouseLeave handler to reset hover state
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (ConnectionPointState == ConnectionPointState.Hovered)
            {
                ConnectionPointState = ConnectionPointState.None;
                Invalidate(); // Trigger repaint to remove hover effect
            }

            // Cancel drag if mouse leaves the control area while dragging
            if (_isDragging)
            {
                _isDragging = false;
                ConnectionPointAction = ConnectionPointAction.None;
                Capture = false;
                DragCancelled?.Invoke(this, new ConnectionPointDragEventArgs(this, Point.Empty));
            }
        }
        // Add these members to your ConnectionPointControl class

     


    }

    public class ConnectionPointDragEventArgs : EventArgs
    {
        public ConnectionPointControl PointControl { get; }
        public Point Location { get; }

        public ConnectionPointDragEventArgs(ConnectionPointControl pointControl, Point location)
        {
            PointControl = pointControl;
            Location = location;
        }
    }
}