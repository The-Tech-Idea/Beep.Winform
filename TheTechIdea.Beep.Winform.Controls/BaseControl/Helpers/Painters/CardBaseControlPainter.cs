using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters
{
    /// <summary>
    /// Card painter: lays out card content and renders label/helper text.
    /// Border, background and shadow are rendered by BaseControl's painter to avoid duplication.
    /// </summary>
    internal sealed class CardBaseControlPainter : IBaseControlPainter
    {
        private Rectangle _cardRect;
        private Rectangle _drawingRect;

        // Layout constants matching the design
        private const int CARD_PADDING = 8;
        private const int CONTENT_PADDING = 16;
        private const int BORDER_RADIUS = 12;
        private const int SHADOW_OFFSET = 2; // kept for compatibility; not used for layout anymore

        // Reserved label/helper space computed during layout
        private int _reserveTop;
        private int _reserveBottom;

        public Rectangle DrawingRect => _drawingRect;
        public Rectangle BorderRect => _cardRect;
        public Rectangle ContentRect => _drawingRect;

        public void UpdateLayout(Base.BaseControl owner)
        {
            _reserveTop = 0;
            _reserveBottom = 0;

            if (owner == null || owner.Width <= 0 || owner.Height <= 0)
            {
                _cardRect = Rectangle.Empty;
                _drawingRect = Rectangle.Empty;
                return;
            }

            // Pre-measure label/helper to reserve space above/below card area
            try
            {
                using var g = owner.CreateGraphics();
                if (!string.IsNullOrEmpty(owner.LabelText))
                {
                    float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    // Slight extra padding so label doesn't touch the card border
                    _reserveTop = h + 4;
                }

                string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(supporting))
                {
                    float supSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    _reserveBottom = h + 6;
                }
            }
            catch { /* best-effort */ }

            // Main card rectangle with padding and reserved top/bottom label areas
            // Do not subtract shadow offset: BaseControl handles any shadow rendering.
            _cardRect = new Rectangle(
                CARD_PADDING,
                CARD_PADDING + _reserveTop,
                owner.Width - (CARD_PADDING * 2),
                owner.Height - (CARD_PADDING * 2) - _reserveTop - _reserveBottom
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
                // Do NOT draw card shadow/background/border here. BaseControl's painter handles that.
                // Only render label/helper text positioning around the content area.
                DrawLabelAndHelper(g, owner);
            }
            finally
            {
                // Restore graphics state
                g.SmoothingMode = oldSmoothingMode;
                g.InterpolationMode = oldInterpolationMode;
            }
        }

     

        private void DrawLabelAndHelper(Graphics g, Base.BaseControl owner)
        {
            // Label (caption above card border)
            if (!string.IsNullOrEmpty(owner.LabelText))
            {
                float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                int labelHeight = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                var labelRect = new Rectangle(_cardRect.Left + 6, Math.Max(0, _cardRect.Top - labelHeight - 2), Math.Max(10, _cardRect.Width - 12), labelHeight);
                Color labelColor = string.IsNullOrEmpty(owner.ErrorText) ? owner.ForeColor : owner.ErrorColor;
                TextRenderer.DrawText(g, owner.LabelText, lf, labelRect, labelColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            // Helper/Error text (below card border)
            string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
            if (!string.IsNullOrEmpty(supporting))
            {
                float supSize = Math.Max(8f, owner.Font.Size - 1f);
                using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                int supportHeight = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                var supportRect = new Rectangle(_cardRect.Left + 6, _cardRect.Bottom + 2, Math.Max(10, _cardRect.Width - 12), supportHeight);
                Color supportColor = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorColor : owner.ForeColor;
                TextRenderer.DrawText(g, supporting, sf, supportRect, supportColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
        }

        private static Color Blend(Color baseColor, Color overlay, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            byte r = (byte)(baseColor.R + (overlay.R - baseColor.R) * amount);
            byte g = (byte)(baseColor.G + (overlay.G - baseColor.G) * amount);
            byte b = (byte)(baseColor.B + (overlay.B - baseColor.B) * amount);
            return Color.FromArgb(r, g, b);
        }

        public void UpdateHitAreas(Base.BaseControl owner, Action<string, Rectangle, Action> register)
        {
            if (owner == null || register == null) return;

            // Register entire card as clickable
            if (!_cardRect.IsEmpty)
            {
                register("Card_Main", _cardRect, () => 
                {
                    // Trigger the Click event
                    owner.TriggerClick();
                });
            }
        }

        public Size GetPreferredSize(Base.BaseControl owner, Size proposedSize)
        {
            if (owner == null) return Size.Empty;

            // Minimum size to accommodate the card design + potential label/helper text
            int minWidth = 150;
            int minHeight = 100;

            // Add extra height to account for label/helper if provided
            int extraTop = 0;
            int extraBottom = 0;
            try
            {
                using var g = owner.CreateGraphics();
                if (!string.IsNullOrEmpty(owner.LabelText))
                {
                    float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var lf = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    extraTop = h + 4;
                }
                string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(supporting))
                {
                    float supSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var sf = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    int h = TextRenderer.MeasureText(g, "Ag", sf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    extraBottom = h + 6;
                }
            }
            catch { }

            int finalMinHeight = minHeight + extraTop + extraBottom;

            return new Size(
                Math.Max(minWidth, proposedSize.Width),
                Math.Max(finalMinHeight, proposedSize.Height)
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
