using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Reading card painter: only draws outer card styling (shadow, background, border)
    /// and exposes an inner DrawingRect for inheriting controls to render content.
    /// No text, icons, or inner content are drawn here.
    /// </summary>
    internal sealed class ReadingCardBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _cardRect;
        private Rectangle _drawingRect;

        // Layout constants
        private const int CARD_PADDING = 8;
        private const int CONTENT_PADDING = 20;
        private const int BORDER_RADIUS = 12;
        private const int SHADOW_OFFSET = 3;

        public Rectangle DrawingRect => _drawingRect;
        public Rectangle BorderRect => _cardRect;
        public Rectangle ContentRect => _drawingRect;

        public void UpdateLayout(Base.BaseControl owner)
        {
            if (owner == null || owner.Width <= 0 || owner.Height <= 0)
            {
                _cardRect = Rectangle.Empty;
                _drawingRect = Rectangle.Empty;
                return;
            }

            // Main card rectangle (reserve space for drop shadow)
            _cardRect = new Rectangle(
                CARD_PADDING,
                CARD_PADDING,
                owner.Width - (CARD_PADDING * 2) - SHADOW_OFFSET,
                owner.Height - (CARD_PADDING * 2) - SHADOW_OFFSET
            );

            if (_cardRect.Width <= 0 || _cardRect.Height <= 0)
            {
                _drawingRect = Rectangle.Empty;
                return;
            }

            // Expose inner content area
            _drawingRect = new Rectangle(
                _cardRect.X + CONTENT_PADDING,
                _cardRect.Y + CONTENT_PADDING,
                _cardRect.Width - (CONTENT_PADDING * 2),
                _cardRect.Height - (CONTENT_PADDING * 2)
            );

            if (_drawingRect.Width <= 0 || _drawingRect.Height <= 0)
                _drawingRect = Rectangle.Empty;
        }

        public void Paint(Graphics g, Base.BaseControl owner)
        {
            if (g == null || owner == null || _cardRect.IsEmpty) return;

            // Enable high-quality rendering
            var oldSmoothingMode = g.SmoothingMode;
            var oldInterpolationMode = g.InterpolationMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            try
            {
                DrawCardShadow(g, owner);
                DrawCardBackground(g, owner);
            }
            finally
            {
                g.SmoothingMode = oldSmoothingMode;
                g.InterpolationMode = oldInterpolationMode;
            }
        }

        private void DrawCardShadow(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            Rectangle shadowRect = new Rectangle(
                _cardRect.X + SHADOW_OFFSET,
                _cardRect.Y + SHADOW_OFFSET,
                _cardRect.Width,
                _cardRect.Height
            );

            Color shadowColor = theme?.ShadowColor ?? Color.FromArgb(15, Color.Black);
            using (var shadowBrush = new SolidBrush(shadowColor))
            using (var shadowPath = CreateRoundedPath(shadowRect, BORDER_RADIUS))
            {
                g.FillPath(shadowBrush, shadowPath);
            }
        }

        private void DrawCardBackground(Graphics g, Base.BaseControl owner)
        {
            var theme = owner._currentTheme;
            Color background = owner.BackColor != Color.Transparent && owner.BackColor != SystemColors.Control
                ? owner.BackColor
                : (theme?.CardBackColor ?? Color.White);
            Color border = owner.BorderColor != Color.Empty && owner.BorderColor != Color.Black
                ? owner.BorderColor
                : (theme?.BorderColor ?? Color.FromArgb(220, 220, 220));

            // State adjustments
            if (!owner.Enabled)
            {
                background = Color.FromArgb(200, background);
                border = Color.FromArgb(180, border);
            }
            else if (owner.IsPressed)
            {
                background = Color.FromArgb(230, background);
            }
            else if (owner.IsHovered)
            {
                // very subtle hover tint
                background = Blend(background, theme?.MenuItemHoverBackColor ?? Color.White, 0.06f);
            }

            using (var bg = new SolidBrush(background))
            using (var pen = new Pen(border, 1))
            using (var path = CreateRoundedPath(_cardRect, BORDER_RADIUS))
            {
                g.FillPath(bg, path);
                g.DrawPath(pen, path);
            }
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null || register == null) return;

            if (!_cardRect.IsEmpty)
            {
                register("ReadingCard_Main", _cardRect, () => owner.TriggerClick());
            }
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            if (owner == null) return Size.Empty;

            int minWidth = 240;
            int minHeight = 120;
            int requiredHeight = (CARD_PADDING * 2) + (CONTENT_PADDING * 2) + SHADOW_OFFSET;

            return new Size(
                Math.Max(minWidth, proposedSize.Width),
                Math.Max(Math.Max(minHeight, requiredHeight), proposedSize.Height)
            );
        }

        private GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static Color Blend(Color baseColor, Color overlay, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            byte r = (byte)(baseColor.R + (overlay.R - baseColor.R) * amount);
            byte g = (byte)(baseColor.G + (overlay.G - baseColor.G) * amount);
            byte b = (byte)(baseColor.B + (overlay.B - baseColor.B) * amount);
            return Color.FromArgb(r, g, b);
        }
    }
}