using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.ConnectionManagement
{
    public enum LineRouteType
    {
        Straight,
        Curved,
        AngleBased
    }
    public class ConnectionLine
    {
        public ConnectionPointControl StartPoint { get; set; }
        public ConnectionPointControl EndPoint { get; set; }
        public Color LineColor { get; set; } = Color.Black;
        public float LineThickness { get; set; } = 1.5f;
        public DashStyle LineStyle { get; set; } = DashStyle.Solid;

        // Line type - straight by default, but can be curved or angled

        // Add to the ConnectionLine class
        private ContextMenuStrip _contextMenu;

        public LineRouteType RouteType { get; set; } = LineRouteType.Straight;

        // Control points for curved lines
        public float CurveIntensity { get; set; } = 30f; // Controls curve intensity

        // Add to ConnectionLine class
        public event EventHandler<TextChangedEventArgs> TextChanged;

        // Modify the Text property
        private string _text = "";
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    string oldText = _text;
                    _text = value ?? "";

                    // Raise the event
                    TextChanged?.Invoke(this, new TextChangedEventArgs(oldText, _text));

                    // Invalidate parent if text changes
                    if (StartPoint?.Parent != null)
                    {
                        Control parent = GetTopParent(StartPoint.Parent);
                        parent?.Invalidate();
                    }
                }
            }
        }

        public Font TextFont { get; set; } = new Font("Arial", 8);
        public Color TextColor { get; set; } = Color.Black;
        public Color TextBackgroundColor { get; set; } = Color.White;
        public bool ShowTextBackground { get; set; } = true;
        public int TextPadding { get; set; } = 3;

        // Optional offset for text positioning
        public Point TextOffset { get; set; } = new Point(0, -10);

        public ConnectionLine(ConnectionPointControl start, ConnectionPointControl end)
        {
            StartPoint = start;
            EndPoint = end;
        }

        public void Draw(Graphics g)
        {
            if (StartPoint == null || EndPoint == null) return;

            using (Pen pen = new Pen(LineColor, LineThickness))
            {
                pen.DashStyle = LineStyle;

                // Get the center points of each connection point control
                Point startPt = StartPoint.Parent.PointToScreen(
                    new Point(StartPoint.Left + StartPoint.Width / 2, StartPoint.Top + StartPoint.Height / 2));
                Point endPt = EndPoint.Parent.PointToScreen(
                    new Point(EndPoint.Left + EndPoint.Width / 2, EndPoint.Top + EndPoint.Height / 2));

                // Convert to coordinate system of the graphics object's control
                Control parent = GetTopParent(StartPoint.Parent);

                startPt = parent.PointToClient(startPt);
                endPt = parent.PointToClient(endPt);

                // Draw based on route type
                switch (RouteType)
                {
                    case LineRouteType.Curved:
                        DrawCurvedLine(g, pen, startPt, endPt);
                        break;

                    case LineRouteType.AngleBased:
                        DrawAngledLine(g, pen, startPt, endPt);
                        break;

                    case LineRouteType.Straight:
                    default:
                        g.DrawLine(pen, startPt, endPt);

                        // Draw text if provided
                        if (!string.IsNullOrEmpty(Text))
                        {
                            DrawTextOnLine(g, startPt, endPt);
                        }
                        break;
                }
            }
        }

        private void DrawCurvedLine(Graphics g, Pen pen, Point start, Point end)
        {
            // Calculate control points for the Bezier curve
            Point mid = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);

            // Determine direction for the curve based on the relative positions
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;
            float angle = (float)Math.Atan2(dy, dx);

            // Calculate perpendicular offset
            float offsetX = (float)Math.Sin(angle) * CurveIntensity;
            float offsetY = (float)-Math.Cos(angle) * CurveIntensity;

            Point controlPoint = new Point(
                mid.X + (int)offsetX,
                mid.Y + (int)offsetY
            );

            // Draw the curve using a quadratic Bezier
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddBezier(
                    start,
                    new Point(start.X + (int)(dx / 3), start.Y + (int)(dy / 3)),
                    new Point(end.X - (int)(dx / 3), end.Y - (int)(dy / 3)),
                    end
                );

                g.DrawPath(pen, path);

                // Draw text if provided
                if (!string.IsNullOrEmpty(Text))
                {
                    // For curved lines, place the text at the control point
                    Point textPoint = controlPoint;
                    DrawTextAtPoint(g, textPoint);
                }
            }
        }

        private void DrawAngledLine(Graphics g, Pen pen, Point start, Point end)
        {
            // Check connection point positions to determine the best angled route
            ConnectionPointPosition startPos = GetPointRelativePosition(StartPoint);
            ConnectionPointPosition endPos = GetPointRelativePosition(EndPoint);

            // Calculate midpoints for the angled segments
            Point mid;

            // Determine if we should route horizontally first, then vertically
            bool horizontalFirst = (startPos == ConnectionPointPosition.Left ||
                                     startPos == ConnectionPointPosition.Right ||
                                     Math.Abs(start.X - end.X) > Math.Abs(start.Y - end.Y));

            if (horizontalFirst)
            {
                // Horizontal then vertical
                mid = new Point(end.X, start.Y);
            }
            else
            {
                // Vertical then horizontal
                mid = new Point(start.X, end.Y);
            }

            // Draw the two segments
            Point[] points = { start, mid, end };
            g.DrawLines(pen, points);

            // Draw text if provided
            if (!string.IsNullOrEmpty(Text))
            {
                // For angled lines, place text at the corner/mid point
                DrawTextAtPoint(g, mid);
            }
        }

        private ConnectionPointPosition GetPointRelativePosition(ConnectionPointControl point)
        {
            // If the point has a defined position index, use it
            if (point.PointIndex >= 0 && point.PointIndex < 4)
            {
                return (ConnectionPointPosition)point.PointIndex;
            }

            // Otherwise infer from relative position to its parent
            Control parent = point.OwnerControl ?? point.Parent;
            if (parent == null) return ConnectionPointPosition.Top;

            int x = point.Left + point.Width / 2;
            int y = point.Top + point.Height / 2;

            // Calculate distances to each edge
            int toTop = y;
            int toBottom = parent.Height - y;
            int toLeft = x;
            int toRight = parent.Width - x;

            // Find the minimum distance
            int min = Math.Min(Math.Min(toTop, toBottom), Math.Min(toLeft, toRight));

            if (min == toTop) return ConnectionPointPosition.Top;
            if (min == toBottom) return ConnectionPointPosition.Bottom;
            if (min == toLeft) return ConnectionPointPosition.Left;
            return ConnectionPointPosition.Right;
        }

        private void DrawTextOnLine(Graphics g, Point startPoint, Point endPoint)
        {
            // Calculate the midpoint of the line
            Point midpoint = new Point(
                (startPoint.X + endPoint.X) / 2,
                (startPoint.Y + endPoint.Y) / 2
            );

            // Apply any offset to the midpoint
            midpoint.Offset(TextOffset);

            DrawTextAtPoint(g, midpoint);
        }

        private void DrawTextAtPoint(Graphics g, Point position)
        {
            // Apply any offset to the position
            position.Offset(TextOffset);

            // Measure the text
            SizeF textSize = g.MeasureString(Text, TextFont);

            // Calculate the rectangle for the text
            RectangleF textRect = new RectangleF(
                position.X - (textSize.Width / 2),
                position.Y - (textSize.Height / 2),
                textSize.Width,
                textSize.Height
            );

            // Add padding to the rectangle
            textRect.Inflate(TextPadding, TextPadding);

            // Draw background if enabled
            if (ShowTextBackground)
            {
                using (SolidBrush backgroundBrush = new SolidBrush(TextBackgroundColor))
                {
                    // Draw a rounded rectangle for better appearance
                    using (GraphicsPath path = CreateRoundedRectangle(textRect, 4))
                    {
                        g.FillPath(backgroundBrush, path);

                        // Add a border to the background
                        using (Pen borderPen = new Pen(Color.FromArgb(200, Color.Gray), 1))
                        {
                            g.DrawPath(borderPen, path);
                        }
                    }
                }
            }

            // Draw the text
            using (SolidBrush textBrush = new SolidBrush(TextColor))
            {
                // Remove the padding for the text drawing
                textRect.Inflate(-TextPadding, -TextPadding);
                g.DrawString(Text, TextFont, textBrush, textRect);
            }
        }

        private GraphicsPath CreateRoundedRectangle(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();

            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.X + rect.Width - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.X + rect.Width - radius * 2, rect.Y + rect.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();

            return path;
        }

        private Control GetTopParent(Control control)
        {
            Control parent = control;
            while (!(parent is Form) && parent.Parent != null)
            {
                parent = parent.Parent;
            }
            return parent;
        }

        /// <summary>
        /// Checks if this connection line intersects with another connection line
        /// </summary>
        public bool IntersectsWith(ConnectionLine otherLine)
        {
            if (StartPoint == null || EndPoint == null ||
                otherLine.StartPoint == null || otherLine.EndPoint == null)
                return false;

            // Get the center points of each connection point control for this line
            Point startPt = StartPoint.Parent.PointToScreen(
                new Point(StartPoint.Left + StartPoint.Width / 2, StartPoint.Top + StartPoint.Height / 2));
            Point endPt = EndPoint.Parent.PointToScreen(
                new Point(EndPoint.Left + EndPoint.Width / 2, EndPoint.Top + EndPoint.Height / 2));

            // Get the center points of each connection point control for the other line
            Point otherStartPt = otherLine.StartPoint.Parent.PointToScreen(
                new Point(otherLine.StartPoint.Left + otherLine.StartPoint.Width / 2,
                          otherLine.StartPoint.Top + otherLine.StartPoint.Height / 2));
            Point otherEndPt = otherLine.EndPoint.Parent.PointToScreen(
                new Point(otherLine.EndPoint.Left + otherLine.EndPoint.Width / 2,
                          otherLine.EndPoint.Top + otherLine.EndPoint.Height / 2));

            // Convert to same coordinate system
            Control parent = GetTopParent(StartPoint.Parent);
            startPt = parent.PointToClient(startPt);
            endPt = parent.PointToClient(endPt);
            otherStartPt = parent.PointToClient(otherStartPt);
            otherEndPt = parent.PointToClient(otherEndPt);

            // Check for line segment intersection using the algorithm
            return DoLineSegmentsIntersect(
                startPt.X, startPt.Y, endPt.X, endPt.Y,
                otherStartPt.X, otherStartPt.Y, otherEndPt.X, otherEndPt.Y);
        }

        /// <summary>
        /// Checks if two line segments intersect
        /// </summary>
        private bool DoLineSegmentsIntersect(
            float x1, float y1, float x2, float y2,
            float x3, float y3, float x4, float y4)
        {
            // Calculate the direction vectors
            float dx1 = x2 - x1;
            float dy1 = y2 - y1;
            float dx2 = x4 - x3;
            float dy2 = y4 - y3;

            // Calculate the determinant
            float determinant = (dx1 * dy2) - (dy1 * dx2);

            // If determinant is near zero, lines are parallel
            if (Math.Abs(determinant) < 0.0001f)
                return false;

            // Calculate parameters for both lines
            float s = ((x1 - x3) * dy2 - (y1 - y3) * dx2) / determinant;
            float t = ((x1 - x3) * dy1 - (y1 - y3) * dx1) / determinant;

            // Check if the intersection point is within both line segments
            return (s >= 0 && s <= 1 && t >= 0 && t <= 1);
        }

        // Method to set up and enable the context menu
        public void EnableInteractiveLabelEditing(Control parentControl)
        {
            // Create context menu if not already created
            if (_contextMenu == null)
            {
                _contextMenu = new ContextMenuStrip();

                // Edit Label menu item
                var editItem = new ToolStripMenuItem("Edit Label");
                editItem.Click += (s, e) => ShowLabelEditDialog(parentControl);
                _contextMenu.Items.Add(editItem);

                // Font options submenu
                var fontItem = new ToolStripMenuItem("Text Font");

                var boldItem = new ToolStripMenuItem("Bold") { CheckOnClick = true };
                boldItem.Click += (s, e) =>
                {
                    SetFontStyle(boldItem.Checked ? FontStyle.Bold : FontStyle.Regular);
                    parentControl.Invalidate();
                };

                var italicItem = new ToolStripMenuItem("Italic") { CheckOnClick = true };
                italicItem.Click += (s, e) =>
                {
                    SetFontStyle(italicItem.Checked ? FontStyle.Italic : FontStyle.Regular);
                    parentControl.Invalidate();
                };

                // Font size submenu
                var fontSizeItem = new ToolStripMenuItem("Size");
                foreach (var size in new[] { 8, 9, 10, 11, 12 })
                {
                    var sizeItem = new ToolStripMenuItem($"{size}pt");
                    sizeItem.Click += (s, e) =>
                    {
                        TextFont = new Font(TextFont.FontFamily, size, TextFont.Style);
                        parentControl.Invalidate();
                    };
                    fontSizeItem.DropDownItems.Add(sizeItem);
                }

                fontItem.DropDownItems.AddRange(new ToolStripItem[] { boldItem, italicItem, fontSizeItem });
                _contextMenu.Items.Add(fontItem);

                // Color options
                var colorItem = new ToolStripMenuItem("Text Color");

                // Common colors
                foreach (var colorInfo in new[] {
            new { Name = "Black", Color = Color.Black },
            new { Name = "Blue", Color = Color.Blue },
            new { Name = "Red", Color = Color.Red },
            new { Name = "Green", Color = Color.DarkGreen },
            new { Name = "Custom...", Color = Color.Empty }
        })
                {
                    var colorMenuItem = new ToolStripMenuItem(colorInfo.Name);
                    colorMenuItem.Click += (s, e) =>
                    {
                        if (colorInfo.Color == Color.Empty)
                        {
                            // Show color picker dialog for custom color
                            using (ColorDialog colorDialog = new ColorDialog())
                            {
                                colorDialog.Color = TextColor;
                                if (colorDialog.ShowDialog(parentControl) == DialogResult.OK)
                                {
                                    TextColor = colorDialog.Color;
                                    parentControl.Invalidate();
                                }
                            }
                        }
                        else
                        {
                            TextColor = colorInfo.Color;
                            parentControl.Invalidate();
                        }
                    };
                    colorItem.DropDownItems.Add(colorMenuItem);
                }
                _contextMenu.Items.Add(colorItem);

                // Background options
                var bgItem = new ToolStripMenuItem("Background");
                var showBgItem = new ToolStripMenuItem("Show Background") { CheckOnClick = true, Checked = ShowTextBackground };
                showBgItem.Click += (s, e) =>
                {
                    ShowTextBackground = showBgItem.Checked;
                    parentControl.Invalidate();
                };
                bgItem.DropDownItems.Add(showBgItem);

                var bgColorItem = new ToolStripMenuItem("Background Color...");
                bgColorItem.Click += (s, e) =>
                {
                    using (ColorDialog colorDialog = new ColorDialog())
                    {
                        colorDialog.Color = TextBackgroundColor;
                        if (colorDialog.ShowDialog(parentControl) == DialogResult.OK)
                        {
                            TextBackgroundColor = colorDialog.Color;
                            parentControl.Invalidate();
                        }
                    }
                };
                bgItem.DropDownItems.Add(bgColorItem);
                _contextMenu.Items.Add(bgItem);
            }

            // Add mouse click handler to parent control to show the context menu
            parentControl.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    // Convert click point to parent coordinates
                    Point clickPoint = e.Location;

                    // Check if the click is on this connection line
                    if (IsPointOnLine(clickPoint))
                    {
                        _contextMenu.Show(parentControl, clickPoint);
                    }
                }
            };
        }

        // Method to check if a point is near the connection line
        public bool IsPointOnLine(Point point)
        {
            if (StartPoint == null || EndPoint == null) return false;

            // Get connection points in parent coordinates
            Control parent = GetTopParent(StartPoint.Parent);
            Point startPt = parent.PointToClient(StartPoint.Parent.PointToScreen(
                new Point(StartPoint.Left + StartPoint.Width / 2, StartPoint.Top + StartPoint.Height / 2)));
            Point endPt = parent.PointToClient(EndPoint.Parent.PointToScreen(
                new Point(EndPoint.Left + EndPoint.Width / 2, EndPoint.Top + EndPoint.Height / 2)));

            // Calculate distance from point to line
            double distance = DistanceFromPointToLine(point, startPt, endPt);

            // Consider the point "on the line" if it's within a certain threshold
            return distance <= 5.0; // 5 pixels threshold
        }

        // Calculate distance from a point to a line segment
        private double DistanceFromPointToLine(Point point, Point lineStart, Point lineEnd)
        {
            double lineLength = Math.Sqrt(Math.Pow(lineEnd.X - lineStart.X, 2) + Math.Pow(lineEnd.Y - lineStart.Y, 2));

            // If line is a point, return distance to that point
            if (lineLength == 0) return Math.Sqrt(Math.Pow(point.X - lineStart.X, 2) + Math.Pow(point.Y - lineStart.Y, 2));

            // Calculate the projection of the point onto the line
            double t = ((point.X - lineStart.X) * (lineEnd.X - lineStart.X) +
                      (point.Y - lineStart.Y) * (lineEnd.Y - lineStart.Y)) / (lineLength * lineLength);

            // Clamp t to the line segment
            t = Math.Max(0, Math.Min(1, t));

            // Calculate the nearest point on the line
            double nearestX = lineStart.X + t * (lineEnd.X - lineStart.X);
            double nearestY = lineStart.Y + t * (lineEnd.Y - lineStart.Y);

            // Return the distance to the nearest point
            return Math.Sqrt(Math.Pow(point.X - nearestX, 2) + Math.Pow(point.Y - nearestY, 2));
        }

        // Helper method to set font style
        private void SetFontStyle(FontStyle style)
        {
            TextFont = new Font(TextFont.FontFamily, TextFont.Size, style);
        }

        // Show dialog to edit the label text
        private void ShowLabelEditDialog(Control parentControl)
        {
            using (var dialog = new Form())
            {
                dialog.Text = "Edit Connection Label";
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.Size = new Size(300, 150);
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                var label = new Label { Text = "Label:", Left = 10, Top = 15, AutoSize = true };
                var textBox = new TextBox { Text = Text, Left = 10, Top = 40, Width = 260 };
                var okButton = new Button { Text = "OK", Left = 110, Top = 70, DialogResult = DialogResult.OK };
                var cancelButton = new Button { Text = "Cancel", Left = 200, Top = 70, DialogResult = DialogResult.Cancel };

                dialog.Controls.AddRange(new Control[] { label, textBox, okButton, cancelButton });
                dialog.AcceptButton = okButton;
                dialog.CancelButton = cancelButton;

                if (dialog.ShowDialog(parentControl) == DialogResult.OK)
                {
                    Text = textBox.Text;
                }
            }
        }

        // Add this event args class
        public class TextChangedEventArgs : EventArgs
        {
            public string OldText { get; }
            public string NewText { get; }

            public TextChangedEventArgs(string oldText, string newText)
            {
                OldText = oldText;
                NewText = newText;
            }
        }
    }
}
