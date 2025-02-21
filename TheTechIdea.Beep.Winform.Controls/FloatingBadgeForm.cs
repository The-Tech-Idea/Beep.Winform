using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum BadgeShape
    {
        Circle,
        RoundedRectangle,
        Rectangle
    }

    public partial class FloatingBadgeForm : Form
    {
        public string BadgeText { get; set; } = "";
        public Color BadgeBackColor { get; set; } = Color.Red;
        public Color BadgeForeColor { get; set; } = Color.White;
        public Font BadgeFont { get; set; } = new Font("Arial", 10, FontStyle.Bold);
        public BadgeShape BadgeShape { get; set; } = BadgeShape.Circle;
        public int BorderRadius { get; set; } = 10; // Only for rounded rectangles

        private Size _lastSize = Size.Empty;

        public FloatingBadgeForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.Magenta;
            TransparencyKey = Color.Magenta;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // **🔹 STEP 1: Measure text size before drawing**
            SizeF textSize = e.Graphics.MeasureString(BadgeText, BadgeFont);
            int padding = 10;
            int badgeWidth = Math.Max((int)textSize.Width + padding, 20);
            int badgeHeight = Math.Max((int)textSize.Height + padding, 20);

            // Keep circle proportional
            if (BadgeShape == BadgeShape.Circle)
            {
                int diameter = Math.Max(badgeWidth, badgeHeight);
                badgeWidth = badgeHeight = diameter;
            }

            // **🔹 STEP 2: Resize badge if necessary**
            if (_lastSize.Width != badgeWidth || _lastSize.Height != badgeHeight)
            {
                this.Size = new Size(badgeWidth, badgeHeight);
                _lastSize = this.Size;
                Invalidate(); // Repaint with new size
                return; // Stop drawing on this frame, will repaint on next
            }

            // **🔹 STEP 3: Draw the badge background**
            using (GraphicsPath path = new GraphicsPath())
            {
                switch (BadgeShape)
                {
                    case BadgeShape.Circle:
                        path.AddEllipse(new Rectangle(0, 0, Width, Height));
                        this.Region = new Region(path);
                        break;

                    case BadgeShape.RoundedRectangle:
                        Rectangle rrRect = new Rectangle(0, 0, Width, Height);
                        AddRoundedRectangle(path, rrRect, BorderRadius);
                        this.Region = new Region(path);
                        break;

                    case BadgeShape.Rectangle:
                    default:
                        path.AddRectangle(new Rectangle(0, 0, Width, Height));
                        this.Region = new Region(path);
                        break;
                }

                using (SolidBrush brush = new SolidBrush(BadgeBackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }

            // **🔹 STEP 4: Center the text properly**
            float textX = (Width - textSize.Width) / 2;
            float textY = (Height - textSize.Height) / 2;

            using (SolidBrush textBrush = new SolidBrush(BadgeForeColor))
            {
                e.Graphics.DrawString(BadgeText, BadgeFont, textBrush, textX, textY);
            }
        }

        private void AddRoundedRectangle(GraphicsPath path, Rectangle rect, int radius)
        {
            int diameter = radius * 2;
            if (diameter > rect.Width) diameter = rect.Width;
            if (diameter > rect.Height) diameter = rect.Height;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
        }
    }
}
