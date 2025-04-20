using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.ConnectionManagement
{
    public class ConnectionManager
    {
        private Control _parentControl;
        private List<ConnectionLine> _connections = new List<ConnectionLine>();

        // Fields for drag line drawing
        private ConnectionPointControl _activeDragPoint = null;
        private Point _currentDragPoint = Point.Empty;
        private bool _isDragging = false;

        // Properties for customizing the drag line appearance
        public Color DragLineColor { get; set; } = Color.DarkGray;
        public float DragLineThickness { get; set; } = 1.5f;
        public DashStyle DragLineStyle { get; set; } = DashStyle.Dash;

        public ConnectionManager(Control parentControl)
        {
            _parentControl = parentControl;

            // Subscribe to the parent's paint event to draw connections
            _parentControl.Paint += ParentControl_Paint;
        }

        /// <summary>
        /// Initializes connection points in the specified container and attaches necessary event handlers
        /// </summary>
        /// <param name="container">The container control that contains connection points</param>
        public void InitializeConnectionPoints(Control container)
        {
            foreach (Control control in container.Controls)
            {
                // Process this control if it's a connection point
                if (control is ConnectionPointControl connectionPoint)
                {
                    // Set the connection manager reference in the connection point
                    connectionPoint.ConnectionManager = this;

                    // Subscribe to drag events
                    connectionPoint.DragStarted += ConnectionPoint_DragStarted;
                    connectionPoint.DragMoved += ConnectionPoint_DragMoved;
                    connectionPoint.DragEnded += ConnectionPoint_DragEnded;
                    connectionPoint.DragCancelled += ConnectionPoint_DragCancelled;
                }

                // Recursively process child controls if they might contain connection points
                if (control.Controls.Count > 0)
                {
                    InitializeConnectionPoints(control);
                }
            }
        }

        /// <summary>
        /// Cleans up event handlers for connection points in the specified container
        /// </summary>
        /// <param name="container">The container control that contains connection points</param>
        public void CleanupConnectionPoints(Control container)
        {
            foreach (Control control in container.Controls)
            {
                // Process this control if it's a connection point
                if (control is ConnectionPointControl connectionPoint)
                {
                    // Unsubscribe from drag events
                    connectionPoint.DragStarted -= ConnectionPoint_DragStarted;
                    connectionPoint.DragMoved -= ConnectionPoint_DragMoved;
                    connectionPoint.DragEnded -= ConnectionPoint_DragEnded;
                    connectionPoint.DragCancelled -= ConnectionPoint_DragCancelled;
                }

                // Recursively process child controls if they might contain connection points
                if (control.Controls.Count > 0)
                {
                    CleanupConnectionPoints(control);
                }
            }
        }

        public void AddConnection(ConnectionPointControl start, ConnectionPointControl end)
        {
            ConnectionLine line = new ConnectionLine(start, end);
            _connections.Add(line);

            // Store the connection in the start point's ConnectedControls dictionary
            start.ConnectedControls[end.Id] = end.OwnerControl?.Name ?? "Unknown";
            // Also store in the end point's dictionary for bidirectional awareness
            end.ConnectedControls[start.Id] = start.OwnerControl?.Name ?? "Unknown";

            // Update the state of both connection points
            start.ConnectionPointState = ConnectionPointState.Connected;
            end.ConnectionPointState = ConnectionPointState.Connected;

            // Redraw the parent control
            _parentControl.Invalidate();
        }

        public void RemoveConnection(ConnectionPointControl start, ConnectionPointControl end)
        {
            var connectionToRemove = _connections.FirstOrDefault(c =>
                (c.StartPoint == start && c.EndPoint == end) ||
                (c.StartPoint == end && c.EndPoint == start));

            if (connectionToRemove != null)
            {
                _connections.Remove(connectionToRemove);

                // Remove from the connected controls dictionaries
                start.ConnectedControls.Remove(end.Id);
                end.ConnectedControls.Remove(start.Id);

                // Update state if there are no more connections
                if (start.ConnectedControls.Count == 0)
                    start.ConnectionPointState = ConnectionPointState.None;

                if (end.ConnectedControls.Count == 0)
                    end.ConnectionPointState = ConnectionPointState.None;

                // Redraw the parent control
                _parentControl.Invalidate();
            }
        }

        // Event handlers for connection point drag operations
        private void ConnectionPoint_DragStarted(object sender, ConnectionPointDragEventArgs e)
        {
            _activeDragPoint = e.PointControl;
            _currentDragPoint = e.Location;
            _isDragging = true;
            _parentControl.Invalidate(); // Trigger a repaint
        }

        private void ConnectionPoint_DragMoved(object sender, ConnectionPointDragEventArgs e)
        {
            _currentDragPoint = e.Location;
            _parentControl.Invalidate(); // Trigger a repaint
        }

        private void ConnectionPoint_DragEnded(object sender, ConnectionPointDragEventArgs e)
        {
            _activeDragPoint = null;
            _isDragging = false;
            _parentControl.Invalidate(); // Trigger a repaint
        }

        private void ConnectionPoint_DragCancelled(object sender, ConnectionPointDragEventArgs e)
        {
            _activeDragPoint = null;
            _isDragging = false;
            _parentControl.Invalidate(); // Trigger a repaint
        }

        private void ParentControl_Paint(object sender, PaintEventArgs e)
        {
            // Draw all established connections
            foreach (var connection in _connections)
            {
                connection.Draw(e.Graphics);
            }

            // Draw the drag line if currently dragging
            if (_isDragging && _activeDragPoint != null)
            {
                DrawDragLine(e.Graphics);
            }
        }

        private void DrawDragLine(Graphics g)
        {
            // Get the center of the active connection point
            Point startPoint = _activeDragPoint.Parent.PointToScreen(
                new Point(_activeDragPoint.Left + _activeDragPoint.Width / 2,
                          _activeDragPoint.Top + _activeDragPoint.Height / 2));

            // Convert to the coordinate system of the parent control
            startPoint = _parentControl.PointToClient(startPoint);
            Point endPoint = _parentControl.PointToClient(_currentDragPoint);

            // Draw the line
            using (Pen pen = new Pen(DragLineColor, DragLineThickness))
            {
                pen.DashStyle = DragLineStyle;
                g.DrawLine(pen, startPoint, endPoint);
            }
        }
        // Add these methods to the ConnectionManager class

        /// <summary>
        /// Sets text on the connection between two connection points
        /// </summary>
        /// <param name="start">Start connection point</param>
        /// <param name="end">End connection point</param>
        /// <param name="text">The text to display on the connection</param>
        public void SetConnectionText(ConnectionPointControl start, ConnectionPointControl end, string text)
        {
            var connection = FindConnection(start, end);
            if (connection != null)
            {
                connection.Text = text;
                _parentControl.Invalidate();
            }
        }

        /// <summary>
        /// Configures the text appearance for a specific connection
        /// </summary>
        /// <param name="start">Start connection point</param>
        /// <param name="end">End connection point</param>
        /// <param name="font">The font to use for the text</param>
        /// <param name="textColor">The color of the text</param>
        /// <param name="backgroundColor">The background color behind the text</param>
        /// <param name="showBackground">Whether to show a background behind the text</param>
        public void ConfigureConnectionText(ConnectionPointControl start, ConnectionPointControl end,
            Font font = null, Color? textColor = null, Color? backgroundColor = null, bool? showBackground = null)
        {
            var connection = FindConnection(start, end);
            if (connection != null)
            {
                if (font != null) connection.TextFont = font;
                if (textColor.HasValue) connection.TextColor = textColor.Value;
                if (backgroundColor.HasValue) connection.TextBackgroundColor = backgroundColor.Value;
                if (showBackground.HasValue) connection.ShowTextBackground = showBackground.Value;

                _parentControl.Invalidate();
            }
        }

        /// <summary>
        /// Sets a custom offset for positioning the text relative to the middle of the connection line
        /// </summary>
        /// <param name="start">Start connection point</param>
        /// <param name="end">End connection point</param>
        /// <param name="offset">The offset point from the line's middle</param>
        public void SetConnectionTextOffset(ConnectionPointControl start, ConnectionPointControl end, Point offset)
        {
            var connection = FindConnection(start, end);
            if (connection != null)
            {
                connection.TextOffset = offset;
                _parentControl.Invalidate();
            }
        }

        /// <summary>
        /// Gets the text currently displayed on the connection between two points
        /// </summary>
        /// <param name="start">Start connection point</param>
        /// <param name="end">End connection point</param>
        /// <returns>The connection text or empty string if not found</returns>
        public string GetConnectionText(ConnectionPointControl start, ConnectionPointControl end)
        {
            var connection = FindConnection(start, end);
            return connection?.Text ?? string.Empty;
        }

        /// <summary>
        /// Finds a connection between two connection points
        /// </summary>
        /// <param name="start">Start connection point</param>
        /// <param name="end">End connection point</param>
        /// <returns>The ConnectionLine object or null if not found</returns>
        private ConnectionLine FindConnection(ConnectionPointControl start, ConnectionPointControl end)
        {
            return _connections.FirstOrDefault(c =>
                (c.StartPoint == start && c.EndPoint == end) ||
                (c.StartPoint == end && c.EndPoint == start));
        }

       // Add these methods to the ConnectionManager class

        /// <summary>
        /// Checks all connections for intersections and optimizes the routing
        /// </summary>
        public void OptimizeConnectionRouting()
        {
            // Check each pair of connections for intersection
            for (int i = 0; i < _connections.Count; i++)
            {
                for (int j = i + 1; j < _connections.Count; j++)
                {
                    var line1 = _connections[i];
                    var line2 = _connections[j];

                    // If lines intersect, change the routing type of one of them
                    if (line1.IntersectsWith(line2))
                    {
                        // By default, make the second line use a different route type
                        if (line2.RouteType == LineRouteType.Straight)
                        {
                            // Make shorter lines curved, longer lines angled
                            double line1Length = GetLineLength(line1);
                            double line2Length = GetLineLength(line2);

                            if (line2Length < line1Length)
                            {
                                line2.RouteType = LineRouteType.Curved;
                            }
                            else
                            {
                                line2.RouteType = LineRouteType.AngleBased;
                            }
                        }
                    }
                }
            }

            // Redraw with optimized routing
            _parentControl.Invalidate();
        }
        // Add to the ConnectionManager class
        public void EnableInteractiveLabelEditing()
        {
            // Enable interactive editing for all existing connections
            foreach (var connection in _connections)
            {
                connection.EnableInteractiveLabelEditing(_parentControl);
            }
        }
        /// <summary>
        /// Gets the approximate length of a connection line
        /// </summary>
        private double GetLineLength(ConnectionLine line)
        {
            // Get the center points of connection points
            Point startPt = line.StartPoint.Parent.PointToScreen(
                new Point(line.StartPoint.Left + line.StartPoint.Width / 2,
                          line.StartPoint.Top + line.StartPoint.Height / 2));
            Point endPt = line.EndPoint.Parent.PointToScreen(
                new Point(line.EndPoint.Left + line.EndPoint.Width / 2,
                          line.EndPoint.Top + line.EndPoint.Height / 2));

            // Convert to parent coordinates
            startPt = _parentControl.PointToClient(startPt);
            endPt = _parentControl.PointToClient(endPt);

            // Calculate distance
            return Math.Sqrt(
                Math.Pow(endPt.X - startPt.X, 2) +
                Math.Pow(endPt.Y - startPt.Y, 2)
            );
        }

        // Modify the AddConnection method to check for route optimization after adding a connection
        public void AddConnection(ConnectionPointControl start, ConnectionPointControl end, string text = null)
        {
            ConnectionLine line = new ConnectionLine(start, end);

            // Set initial text if provided
            if (!string.IsNullOrEmpty(text))
            {
                line.Text = text;
            }

            // Enable interactive label editing for this connection
            line.EnableInteractiveLabelEditing(_parentControl);

            _connections.Add(line);

            // Store the connection in dictionaries
            start.ConnectedControls[end.Id] = end.OwnerControl?.Name ?? "Unknown";
            end.ConnectedControls[start.Id] = start.OwnerControl?.Name ?? "Unknown";

            // Update the state of connection points
            start.ConnectionPointState = ConnectionPointState.Connected;
            end.ConnectionPointState = ConnectionPointState.Connected;

            // Check for intersections and optimize routing
            OptimizeConnectionRouting();

            // Redraw the parent control
            _parentControl.Invalidate();
        }

        // Clean up resources when this manager is no longer needed
        public void Dispose()
        {
            // Unsubscribe from paint event
            if (_parentControl != null)
            {
                _parentControl.Paint -= ParentControl_Paint;
                CleanupConnectionPoints(_parentControl);
            }
        }
    }
}
