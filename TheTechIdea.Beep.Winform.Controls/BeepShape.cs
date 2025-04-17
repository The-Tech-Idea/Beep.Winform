using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{


    public class BeepShape: BeepControl
    {
        [Browsable(true)]
        [Category("Shape")]
        public BeepShapeType ShapeType { get; set; } = BeepShapeType.Rectangle;

        [Browsable(true)]
        [Category("Shape")]
        public float RotationAngle { get; set; } = 0f;

        [Browsable(true)]
        [Category("Connection")]
        public BeepControl ConnectedStart { get; set; }

        [Browsable(true)]
        [Category("Connection")]
        public BeepControl ConnectedEnd { get; set; }

        private bool isDragging = false;
        private Point dragStart;

        public BeepShape()
        {
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.DoubleBuffered = true;
        }
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
          
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Matrix transform = new Matrix())
            {
                transform.RotateAt(RotationAngle, new PointF(Width / 2f, Height / 2f));
                g.Transform = transform;

                using (Pen pen = new Pen(BorderColor, BorderThickness))
                using (Brush brush = new SolidBrush(BackColor))
                {
                    switch (ShapeType)
                    {
                        case BeepShapeType.Line:
                            g.DrawLine(pen, GetStartPoint(), GetEndPoint());
                            break;
                        case BeepShapeType.Rectangle:
                            g.FillRectangle(brush, DrawingRect);
                            g.DrawRectangle(pen, DrawingRect);
                            break;
                        case BeepShapeType.Ellipse:
                            g.FillEllipse(brush, DrawingRect);
                            g.DrawEllipse(pen, DrawingRect);
                            break;
                        case BeepShapeType.Triangle:
                            Point[] triangle = new[]
                            {
                                new Point(Width / 2, 0),
                                new Point(0, Height),
                                new Point(Width, Height)
                            };
                            g.FillPolygon(brush, triangle);
                            g.DrawPolygon(pen, triangle);
                            break;
                        case BeepShapeType.Star:
                            DrawStar(g, pen, brush);
                            break;
                    }
                }
            }
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
         
        }

        private void DrawStar(Graphics g, Pen pen, Brush brush)
        {
            PointF[] points = new PointF[10];
            float cx = Width / 2f, cy = Height / 2f;
            float outerRadius = Math.Min(Width, Height) / 2f;
            float innerRadius = outerRadius * 0.5f;

            for (int i = 0; i < 10; i++)
            {
                float angle = i * 36f * (float)Math.PI / 180f;
                float radius = i % 2 == 0 ? outerRadius : innerRadius;
                points[i] = new PointF(
                    cx + radius * (float)Math.Cos(angle),
                    cy + radius * (float)Math.Sin(angle)
                );
            }
            g.FillPolygon(brush, points);
            g.DrawPolygon(pen, points);
        }

        private Point GetStartPoint()
        {
            return ConnectedStart?.PointToScreen(Point.Empty) ?? Point.Empty;
        }

        private Point GetEndPoint()
        {
            return ConnectedEnd?.PointToScreen(Point.Empty) ?? new Point(Width, Height);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStart = e.Location;
                Cursor = Cursors.SizeAll;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging)
            {
                int dx = e.X - dragStart.X;
                int dy = e.Y - dragStart.Y;
                Location = new Point(Location.X + dx, Location.Y + dy);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                Cursor = Cursors.Default;
            }
        }
    }
}
