using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Fluent Acrylic inspired painter: soft elevation, translucent fill, subtle highlight.
    /// Paints only the outer container; inner content is drawn by inheriting controls using DrawingRect.
    /// </summary>
    internal sealed class FluentAcrylicBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _outerRect;
        private Rectangle _drawingRect;

        // Layout constants
        private const int CARD_PADDING = 6;     // space to edges
        private const int CONTENT_PADDING = 10; // inner padding for content
        private const int RADIUS = 12;          // rounded corners
        private const int SHADOW_OFFSET = 4;    // soft shadow offset

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

            // Reserve space for shadow
            _outerRect = new Rectangle(
                CARD_PADDING,
                CARD_PADDING,
                Math.Max(0, owner.Width - (CARD_PADDING * 2) - SHADOW_OFFSET),
                Math.Max(0, owner.Height - (CARD_PADDING * 2) - SHADOW_OFFSET));

            // Inner content area
            _drawingRect = Rectangle.Inflate(_outerRect, -CONTENT_PADDING, -CONTENT_PADDING);
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
                // Shadow (soft)
                DrawSoftShadow(g, _outerRect, RADIUS, owner.ShadowColor == Color.Black ? Color.FromArgb(40, 0, 0, 0) : owner.ShadowColor);

                // Acrylic-like background (translucent)
                Color baseFill = owner.BackColor != Color.Empty && owner.BackColor.A > 0
                    ? Color.FromArgb(180, owner.BackColor)
                    : Color.FromArgb(160, theme?.BackColor ?? Color.White);

                using (var path = CreateRoundedPath(_outerRect, RADIUS))
                using (var fill = new SolidBrush(baseFill))
                {
                    g.FillPath(fill, path);
                }

                // Subtle top highlight
                Rectangle highlight = new Rectangle(_outerRect.X + 2, _outerRect.Y + 2, _outerRect.Width - 4, Math.Max(6, _outerRect.Height / 10));
                using (var lg = new LinearGradientBrush(highlight, Color.FromArgb(60, Color.White), Color.FromArgb(0, Color.White), LinearGradientMode.Vertical))
                using (var clip = CreateRoundedPath(_outerRect, RADIUS))
                {
                    Region oldClip = g.Clip;
                    try
                    {
                        g.SetClip(clip);
                        g.FillRectangle(lg, highlight);
                    }
                    finally
                    {
                        g.Clip = oldClip;
                    }
                }

                // Border
                Color border = owner.BorderColor != Color.Empty
                    ? owner.BorderColor
                    : (theme?.BorderColor ?? Color.FromArgb(210, 210, 210));

                // Focus/hover accents
                if (owner.IsFocused)
                    border = theme?.BorderColor != Color.Empty ? theme.BorderColor : Color.RoyalBlue;
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
            {
                register("Acrylic_Main", _outerRect, owner.TriggerClick);
            }
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            if (owner == null) return Size.Empty;
            int minW = 180; // sensible min for a card/control
            int minH = 60;
            int requiredH = (CARD_PADDING * 2) + (CONTENT_PADDING * 2) + SHADOW_OFFSET + 20;
            return new Size(
                Math.Max(minW, proposedSize.Width),
                Math.Max(Math.Max(minH, requiredH), proposedSize.Height));
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

        private static void DrawSoftShadow(Graphics g, Rectangle rect, int radius, Color shadow)
        {
            // Simple soft shadow using multiple alpha strokes (no bitmap blur)
            int layers = 4;
            for (int i = layers; i >= 1; i--)
            {
                int inflate = i * 2;
                Rectangle r = Rectangle.Inflate(new Rectangle(rect.X + 4, rect.Y + 4, rect.Width, rect.Height), inflate, inflate);
                int alpha = Math.Max(4, (int)(12f / i * (shadow.A / 255f) * 20));
                using (var pen = new Pen(Color.FromArgb(alpha, shadow), 1))
                using (var path = CreateRoundedPath(r, Math.Max(1, radius + i)))
                {
                    g.DrawPath(pen, path);
                }
            }
        }
    }
}
