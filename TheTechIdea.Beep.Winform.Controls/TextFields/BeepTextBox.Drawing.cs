using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Drawing methods for BeepTextBox
    /// </summary>
    public partial class BeepTextBox
    {
        #region "Drawing"
        
        private void DrawFocusAnimation(Graphics g)
        {
            if (_focusAnimationProgress <= 0) return;
            
            var focusRect = ClientRectangle;
            focusRect.Inflate(-1, -1);
            
            int alpha = (int)(255 * _focusAnimationProgress);
            using (var pen = new Pen(Color.FromArgb(alpha, _focusBorderColor), _borderWidth))
            {
                if (BorderRadius > 0)
                {
                    using (var path = GraphicsExtensions.GetRoundedRectPath(focusRect, BorderRadius))
                    {
                        g.DrawPath(pen, path);
                    }
                }
                else
                {
                    g.DrawRectangle(pen, focusRect);
                }
            }
        }
        
        /// <summary>
        /// Draws character count indicator with DPI-aware cached font.
        /// Per Microsoft: Don't create Font in OnPaint - use cached fonts.
        /// </summary>
        private void DrawCharacterCount(Graphics g)
        {
            string countText = $"{_text.Length}/{_maxLength}";
            Font font = GetCharacterCountFont(); // DPI-aware cached font
            
            var textSize = TextUtils.MeasureText(g, countText, font);
            var location = new PointF(Width - textSize.Width - 5, Height - textSize.Height - 2);
            
            Color textColor = _text.Length > _maxLength * 0.9 ? Color.Red : Color.Gray;
            using (var brush = new SolidBrush(textColor))
            {
                g.DrawString(countText, font, brush, location);
            }
        }
        
        private void DrawTypingIndicator(Graphics g)
        {
            var indicatorRect = new Rectangle(Width - 12, Height - 12, 8, 8);
            using (var brush = new SolidBrush(Color.Green))
            {
                g.FillEllipse(brush, indicatorRect);
            }
        }
        
        #endregion
        
        #region "Draw Override"
        
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            if (graphics == null || rectangle.Width <= 0 || rectangle.Height <= 0)
                return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle textRect = rectangle;
            if (PainterKind == BaseControlPainterKind.Material)
            {
                var padding = GetMaterialStylePadding();
                var effects = GetMaterialEffectsSpace();
                textRect = new Rectangle(
                    rectangle.X + padding.Left + effects.Width / 2,
                    rectangle.Y + padding.Top + effects.Height / 2,
                    Math.Max(0, rectangle.Width - padding.Horizontal - effects.Width),
                    Math.Max(0, rectangle.Height - padding.Vertical - effects.Height)
                );
            }

            int borderOffset = Math.Max(0, _borderWidth);
            textRect.Inflate(-(Padding.Horizontal / 2 + borderOffset), -(Padding.Vertical / 2 + borderOffset));
            
            if (_showLineNumbers && _multiline)
            {
                textRect.X += _lineNumberMarginWidth;
                textRect.Width = Math.Max(1, textRect.Width - _lineNumberMarginWidth);
            }

            if (_showCharacterCount && _maxLength > 0)
            {
                // Use cached DPI-aware font instead of creating new Font
                Font font = GetCharacterCountFont();
                try
                {
                    var charCountHeight = (int)Math.Ceiling(TextUtils.MeasureText(graphics, "00", font).Height);
                    textRect.Height = Math.Max(1, textRect.Height - charCountHeight);
                }
                catch
                {
                    // Ignore
                }
            }

            _helper?.DrawAll(graphics, rectangle, textRect);
        }
        
        #endregion
    }
}
