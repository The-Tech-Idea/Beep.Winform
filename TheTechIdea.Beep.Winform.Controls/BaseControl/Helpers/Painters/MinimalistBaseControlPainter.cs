using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Minimalist painter: clean web-style card/control. Very subtle border, no heavy effects.
    /// Only paints the outer container; inheriting controls render their content inside DrawingRect.
    /// </summary>
    internal sealed class MinimalistBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _outerRect;
        private Rectangle _drawingRect;

        private const int RADIUS = 8;
        private const int PADDING = 8;

        public Rectangle DrawingRect => _drawingRect;
        public Rectangle BorderRect => _outerRect;
        public Rectangle ContentRect => _drawingRect;

        public void UpdateLayout(Base.BaseControl owner)
        {
            if (owner == null || owner.Width <= 0 || owner.Height <= 0)
            {
                _outerRect = Rectangle.Empty;
                _drawingRect = Rectangle.Empty;
                return;
            }

            _outerRect = new Rectangle(1, 1, Math.Max(0, owner.Width - 2), Math.Max(0, owner.Height - 2));
            _drawingRect = Rectangle.Inflate(_outerRect, -PADDING, -PADDING);
            if (_drawingRect.Width <= 0 || _drawingRect.Height <= 0)
                _drawingRect = Rectangle.Empty;
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null || _outerRect.IsEmpty) return;
            var theme = owner._currentTheme;

            var oldSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            try
            {
                // Background
                Color bg = owner.BackColor != Color.Empty && owner.BackColor != SystemColors.Control
                    ? owner.BackColor
                    : (theme?.BackColor ?? Color.White);

                using (var path = CreateRoundedPath(_outerRect, RADIUS))
                using (var brush = new SolidBrush(bg))
                {
                    g.FillPath(brush, path);
                }

                // Border
                Color border = owner.BorderColor != Color.Empty ? owner.BorderColor : (theme?.BorderColor ?? Color.FromArgb(210, 210, 210));
                if (owner.IsFocused)
                    border = theme?.FocusIndicatorColor != Color.Empty ? theme.FocusIndicatorColor : ControlPaint.Dark(border);
                else if (owner.IsHovered)
                    border = theme?.HoverLinkColor != Color.Empty ? theme.HoverLinkColor : ControlPaint.Light(border);

                using (var path = CreateRoundedPath(_outerRect, RADIUS))
                using (var pen = new Pen(border, 1))
                {
                    g.DrawPath(pen, path);
                }
            }
            finally
            {
                g.SmoothingMode = oldSmoothing;
            }
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null || register == null) return;
            if (!_outerRect.IsEmpty)
                register("Minimalist_Main", _outerRect, owner.TriggerClick);
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            int minW = 120, minH = 36;
            return new Size(Math.Max(minW, proposedSize.Width), Math.Max(minH, proposedSize.Height));
        }

        private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
