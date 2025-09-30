using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Shortcut card painter: provides outer card styling (border, background) 
    /// while leaving inner content (DrawingRect) for inheriting controls to handle.
    /// Based on the keyboard shortcut card design - only handles the outer card appearance.
    /// </summary>
    internal sealed class ShortcutCardBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _cardRect;
        private Rectangle _drawingRect;

        // Layout constants matching the design
        private const int CARD_PADDING = 6;
        private const int CONTENT_PADDING = 16;
        private const int BORDER_RADIUS = 8;
        private const int BORDER_WIDTH = 2;

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

            // Main card rectangle
            _cardRect = new Rectangle(
                CARD_PADDING,
                CARD_PADDING,
                owner.Width - (CARD_PADDING * 2),
                owner.Height - (CARD_PADDING * 2)
            );

            if (_cardRect.Width <= 0 || _cardRect.Height <= 0) 
            {
                _drawingRect = Rectangle.Empty;
                return;
            }

            // DrawingRect for inheriting controls (inside card with content padding)
            _drawingRect = new Rectangle(
                _cardRect.X + CONTENT_PADDING,
                _cardRect.Y + CONTENT_PADDING,
                _cardRect.Width - (CONTENT_PADDING * 2),
                _cardRect.Height - (CONTENT_PADDING * 2)
            );

            // Ensure DrawingRect is valid
            if (_drawingRect.Width <= 0 || _drawingRect.Height <= 0)
            {
                _drawingRect = Rectangle.Empty;
            }
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
                // Only draw card background and border - inheriting controls handle all inner content
                DrawCardBackground(g, owner);
            }
            finally
            {
                // Restore graphics state
                g.SmoothingMode = oldSmoothingMode;
                g.InterpolationMode = oldInterpolationMode;
            }
        }

        private void DrawCardBackground(Graphics g, Base.BaseControl owner)
        {
            // Get colors from theme
            var theme = owner._currentTheme;
            
            // Determine background color - use owner's BackColor if set, otherwise theme
            Color backgroundColor = owner.BackColor != Color.Transparent && owner.BackColor != SystemColors.Control 
                ? owner.BackColor 
                : (theme?.BackColor ?? Color.FromArgb(173, 216, 230)); // Light blue/cyan fallback

            // Determine border color - use owner's BorderColor if set, otherwise theme
            Color borderColor = owner.BorderColor != Color.Black && owner.BorderColor != Color.Empty
                ? owner.BorderColor
                : (theme?.BorderColor ?? Color.FromArgb(70, 130, 180)); // Steel blue fallback

            // Apply state-based modifications
            if (!owner.Enabled)
            {
                backgroundColor = Color.FromArgb(180, backgroundColor);
                borderColor = Color.FromArgb(180, borderColor);
            }
            else if (owner.IsPressed)
            {
                backgroundColor = Color.FromArgb(220, backgroundColor.R, backgroundColor.G, backgroundColor.B);
            }
            else if (owner.IsHovered)
            {
                backgroundColor = Color.FromArgb(255, Math.Min(255, backgroundColor.R + 10), 
                    Math.Min(255, backgroundColor.G + 10), Math.Min(255, backgroundColor.B + 10));
            }

            using (var backgroundBrush = new SolidBrush(backgroundColor))
            using (var borderPen = new Pen(borderColor, BORDER_WIDTH))
            using (var cardPath = CreateRoundedPath(_cardRect, BORDER_RADIUS))
            {
                g.FillPath(backgroundBrush, cardPath);
                g.DrawPath(borderPen, cardPath);
            }
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null || register == null) return;

            // Register entire card as clickable
            if (!_cardRect.IsEmpty)
            {
                register("ShortcutCard_Main", _cardRect, () => 
                {
                    // Trigger the Click event
                    owner.TriggerClick();
                });
            }
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            if (owner == null) return Size.Empty;

            // Minimum size to accommodate the card design
            int minWidth = 120;
            int minHeight = 80;

            return new Size(
                Math.Max(minWidth, proposedSize.Width),
                Math.Max(minHeight, proposedSize.Height)
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
    }
}