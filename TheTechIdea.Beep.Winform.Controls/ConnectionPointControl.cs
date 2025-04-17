using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class ConnectionPointControl : Control
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Control OwnerControl { get; set; } // BeepPanel or child control
        public int PointIndex { get; set; } // Index in connection points (0: Top, 1: Bottom, 2: Left, 3: Right)
        private bool _isDragging;

        public event EventHandler<ConnectionPointDragEventArgs> DragStarted;
        public event EventHandler<ConnectionPointDragEventArgs> DragEnded;

        public ConnectionPointControl()
        {
            Size = new Size(8, 8); // Small size for connection point
            BackColor = Color.Transparent;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw a blue circle for visibility
            using (SolidBrush brush = new SolidBrush(Color.Blue))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                Capture = true;
                DragStarted?.Invoke(this, new ConnectionPointDragEventArgs(this, e.Location));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging)
            {
                // Notify parent of drag movement
                DragStarted?.Invoke(this, new ConnectionPointDragEventArgs(this, PointToScreen(e.Location)));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_isDragging)
            {
                _isDragging = false;
                Capture = false;
                DragEnded?.Invoke(this, new ConnectionPointDragEventArgs(this, PointToScreen(e.Location)));
            }
        }
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