using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Helpers;

using Models = TheTechIdea.Beep.Winform.Controls.TextFields.Models;

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Helpers
{
    /// <summary>
    /// Handles advanced drawing operations for BeepSimpleTextBox including
    /// image/text alignment similar to BeepButton and DevExpress-Style features
    /// </summary>
    public class TextBoxDrawingHelper
    {
        private readonly IBeepTextBox _textBox;
        private TextBoxPerformanceHelper _performance;
        
        public TextBoxDrawingHelper(IBeepTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        }
        
        public void SetPerformanceHelper(TextBoxPerformanceHelper performance)
        {
            _performance = performance;
        }
        
        /// <summary>
        /// Main drawing method that handles all rendering
        /// </summary>
        private Control OwnerControl => _textBox as Control;
        private int S(int v) => OwnerControl == null ? v : DpiScalingHelper.ScaleValue(v, OwnerControl);
        private int ContentInset => S(2);
        private int ImageTextGap => S(4);
        private int ImageTextGapV => S(2);
        private int ScrollX => (_textBox as BeepTextBox)?.HorizontalScrollOffset ?? 0;

        /// <summary>
        /// THE text rectangle: content rect inset, then adjusted for the image zone.
        /// Text, placeholder, selection, caret and search highlights must all use this one
        /// method - previously the text drew in an inset rect while caret/placeholder/selection
        /// used the un-inset one, so the caret sat 2px left of the first character.
        /// </summary>
        public Rectangle GetEffectiveTextRect(Rectangle textRect)
        {
            var r = textRect;
            r.Inflate(-ContentInset, -ContentInset);
            if (!HasImage()) return r;

            Size imageSize = GetImageSize();
            switch (_textBox.TextImageRelation)
            {
                case TextImageRelation.ImageBeforeText:
                    r.X += imageSize.Width + ImageTextGap;
                    r.Width = Math.Max(0, r.Width - imageSize.Width - ImageTextGap);
                    break;
                case TextImageRelation.TextBeforeImage:
                    r.Width = Math.Max(0, r.Width - imageSize.Width - ImageTextGap);
                    break;
                case TextImageRelation.ImageAboveText:
                    r.Y += imageSize.Height + ImageTextGapV;
                    r.Height = Math.Max(0, r.Height - imageSize.Height - ImageTextGapV);
                    break;
                case TextImageRelation.TextAboveImage:
                    r.Height = Math.Max(0, r.Height - imageSize.Height - ImageTextGapV);
                    break;
            }
            return r;
        }

        public void DrawAll(Graphics g, Rectangle clientRect, Rectangle textRect)
        {
            if (g == null) return;
            
            // Set high quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            
            // Draw line numbers if enabled
            if (ShouldDrawLineNumbers())
            {
                DrawLineNumbers(g, clientRect, textRect);
            }
            
            // Draw main content (text and image)
            DrawContent(g, textRect);
            
            // Draw placeholder if needed
            if (ShouldDrawPlaceholder())
            {
                DrawPlaceholder(g, textRect);
            }
            
            // Draw selection
            DrawSelection(g, textRect);
            
            // Draw caret
            DrawCaret(g, textRect);
        }
        
        /// <summary>
        /// Draws content with proper image and text alignment like BeepButton
        /// </summary>
        private void DrawContent(Graphics g, Rectangle contentRect)
        {
            // Always allow image to draw even when placeholder is showing
            bool showingPlaceholder = ShouldDrawPlaceholder();

            Size imageSize = GetImageSize();

            if (HasImage() && (!string.IsNullOrEmpty(GetActualText()) || showingPlaceholder))
            {
                Rectangle working = contentRect;
                working.Inflate(-ContentInset, -ContentInset);

                Rectangle imageRect = _textBox.TextImageRelation switch
                {
                    TextImageRelation.ImageBeforeText => AlignRectangle(
                        new Rectangle(working.Left, working.Top, imageSize.Width, working.Height),
                        imageSize, _textBox.ImageAlign),
                    TextImageRelation.TextBeforeImage => AlignRectangle(
                        new Rectangle(working.Right - imageSize.Width, working.Top, imageSize.Width, working.Height),
                        imageSize, _textBox.ImageAlign),
                    TextImageRelation.ImageAboveText => AlignRectangle(
                        new Rectangle(working.Left, working.Top, working.Width, imageSize.Height),
                        imageSize, _textBox.ImageAlign),
                    TextImageRelation.TextAboveImage => AlignRectangle(
                        new Rectangle(working.Left, working.Bottom - imageSize.Height, working.Width, imageSize.Height),
                        imageSize, _textBox.ImageAlign),
                    _ => AlignRectangle(working, imageSize, _textBox.ImageAlign),
                };
                DrawImage(g, imageRect);
            }

            if (!showingPlaceholder && !string.IsNullOrEmpty(GetActualText()))
            {
                DrawText(g, GetEffectiveTextRect(contentRect));
            }
        }

        /// <summary>
        /// Draw text with advanced formatting options
        /// </summary>
        private void DrawText(Graphics g, Rectangle textRect)
        {
            string displayText = GetActualText();
            if (string.IsNullOrEmpty(displayText)) return;
            
            Font font = _textBox.TextFont ?? BeepFontManager.GetFont("Segoe UI", 9f);
            Color textColor = GetTextColor();
            
            // Apply text formatting flags
            TextFormatFlags flags = GetTextFormatFlags();
            
            // Single-line text longer than the box scrolls left so the caret stays visible;
            // draw shifted inside a clip so nothing bleeds past the text zone.
            int scrollX = !_textBox.Multiline ? ScrollX : 0;
            if (scrollX > 0)
            {
                var state = g.Save();
                g.SetClip(textRect);
                var shifted = new Rectangle(textRect.X - scrollX, textRect.Y,
                                            textRect.Width + scrollX, textRect.Height);
                TextRenderer.DrawText(g, displayText, font, shifted, textColor, flags);
                g.Restore(state);
            }
            else
            {
                TextRenderer.DrawText(g, displayText, font, textRect, textColor, flags);
            }
        }
        
        /// <summary>
        /// Calculate layout similar to BeepButton's image/text positioning
        /// </summary>
        private void CalculateLayout(Rectangle contentRect, Size imageSize, Size textSize, 
            out Rectangle imageRect, out Rectangle textRect)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;
            
            bool hasImage = imageSize != Size.Empty && HasImage();
            bool hasText = !string.IsNullOrEmpty(GetDisplayText());
            
            // Adjust for padding
            contentRect.Inflate(-2, -2);
            
            if (hasImage && !hasText)
            {
                // Only image - center it
                imageRect = AlignRectangle(contentRect, imageSize, _textBox.ImageAlign);
            }
            else if (hasText && !hasImage)
            {
                // Only text - align according to TextAlignment
                textRect = AlignRectangle(contentRect, textSize, GetTextAlignment());
            }
            else if (hasImage && hasText)
            {
                // Both image and text - use TextImageRelation
                switch (_textBox.TextImageRelation)
                {
                    case TextImageRelation.Overlay:
                        imageRect = AlignRectangle(contentRect, imageSize, _textBox.ImageAlign);
                        textRect = AlignRectangle(contentRect, textSize, GetTextAlignment());
                        break;
                        
                    case TextImageRelation.ImageBeforeText:
                        imageRect = AlignRectangle(
                            new Rectangle(contentRect.Left, contentRect.Top, imageSize.Width, contentRect.Height),
                            imageSize, _textBox.ImageAlign);
                        textRect = AlignRectangle(
                            new Rectangle(contentRect.Left + imageSize.Width + 4, contentRect.Top, 
                                contentRect.Width - imageSize.Width - 4, contentRect.Height),
                            textSize, GetTextAlignment());
                        break;
                        
                    case TextImageRelation.TextBeforeImage:
                        textRect = AlignRectangle(
                            new Rectangle(contentRect.Left, contentRect.Top, textSize.Width, contentRect.Height),
                            textSize, GetTextAlignment());
                        imageRect = AlignRectangle(
                            new Rectangle(contentRect.Left + textSize.Width + 4, contentRect.Top,
                                contentRect.Width - textSize.Width - 4, contentRect.Height),
                            imageSize, _textBox.ImageAlign);
                        break;
                        
                    case TextImageRelation.ImageAboveText:
                        imageRect = AlignRectangle(
                            new Rectangle(contentRect.Left, contentRect.Top, contentRect.Width, imageSize.Height),
                            imageSize, _textBox.ImageAlign);
                        textRect = AlignRectangle(
                            new Rectangle(contentRect.Left, contentRect.Top + imageSize.Height + 2,
                                contentRect.Width, contentRect.Height - imageSize.Height - 2),
                            textSize, GetTextAlignment());
                        break;
                        
                    case TextImageRelation.TextAboveImage:
                        textRect = AlignRectangle(
                            new Rectangle(contentRect.Left, contentRect.Top, contentRect.Width, textSize.Height),
                            textSize, GetTextAlignment());
                        imageRect = AlignRectangle(
                            new Rectangle(contentRect.Left, contentRect.Top + textSize.Height + 2,
                                contentRect.Width, contentRect.Height - textSize.Height - 2),
                            imageSize, _textBox.ImageAlign);
                        break;
                }
            }
        }
        
        /// <summary>
        /// Align rectangle within container similar to BeepButton
        /// </summary>
        private Rectangle AlignRectangle(Rectangle container, Size size, ContentAlignment alignment)
        {
            int x = 0, y = 0;
            
            // Horizontal alignment
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    x = container.X;
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    x = container.X + (container.Width - size.Width) / 2;
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.BottomRight:
                    x = container.Right - size.Width;
                    break;
            }
            
            // Vertical alignment
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    y = container.Y;
                    break;
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    y = container.Y + (container.Height - size.Height) / 2;
                    break;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    y = container.Bottom - size.Height;
                    break;
            }
            
            return new Rectangle(new Point(x, y), size);
        }
        
        /// <summary>
        /// Draw the image using BeepImage
        /// </summary>
        private void DrawImage(Graphics g, Rectangle imageRect)
        {
            if (_textBox.BeepImage == null || !HasImage()) return;
            _textBox.BeepImage.BackColor= _textBox.BackColor;
            // Constrain to MaxImageSize
            Size maxSize = _textBox.MaxImageSize;
            if (imageRect.Width > maxSize.Width || imageRect.Height > maxSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)maxSize.Width / imageRect.Width,
                    (float)maxSize.Height / imageRect.Height);
                
                int newWidth = (int)(imageRect.Width * scaleFactor);
                int newHeight = (int)(imageRect.Height * scaleFactor);
                
                imageRect = new Rectangle(
                    imageRect.X + (imageRect.Width - newWidth) / 2,
                    imageRect.Y + (imageRect.Height - newHeight) / 2,
                    newWidth, newHeight);
            }
            
            _textBox.BeepImage.Size = imageRect.Size;
            _textBox.BeepImage.DrawImage(g, imageRect);
        }
        
        /// <summary>
        /// Draw placeholder text
        /// </summary>
        private void DrawPlaceholder(Graphics g, Rectangle textRect)
        {
            if (string.IsNullOrEmpty(_textBox.PlaceholderText)) return;
            
            Font font = _textBox.TextFont ?? BeepFontManager.GetFont("Segoe UI", 9f);
            Color placeholderColor = _textBox.PlaceholderTextColor;
            
            // Use actual text rectangle (account for image space and alignment area)
            Rectangle actualTextRect = GetEffectiveTextRect(textRect);
            
            TextFormatFlags flags = GetTextFormatFlags();
            
            TextRenderer.DrawText(g, _textBox.PlaceholderText, font, actualTextRect, placeholderColor, flags);
        }

        /// <summary>
        /// Draw text selection highlighting
        /// </summary>
        private void DrawSelection(Graphics g, Rectangle textRect)
        {
            var control = _textBox as Control;
            if (control == null || !control.Focused) return;
            
            int selStart = _textBox.SelectionStart;
            int selLength = _textBox.SelectionLength;
            
            if (selLength <= 0) return;
            
            string text = GetActualText();
            if (string.IsNullOrEmpty(text)) return;
            
            Font font = _textBox.TextFont ?? BeepFontManager.GetFont("Segoe UI", 9f);
            
            // Get the actual text rectangle considering image layout
            Rectangle actualTextRect = GetEffectiveTextRect(textRect);
            
            // Use the same TextFormatFlags as drawing to ensure consistent measurement
            TextFormatFlags measureFlags = GetTextFormatFlags();
            
            // Calculate selection rectangle
            string beforeSelection = text.Substring(0, Math.Min(selStart, text.Length));
            string selectedText = selLength > 0 && selStart < text.Length 
                ? text.Substring(selStart, Math.Min(selLength, text.Length - selStart))
                : "";
            
            if (string.IsNullOrEmpty(selectedText)) return;
            
            Size beforeSize = Size.Empty;
            if (!string.IsNullOrEmpty(beforeSelection))
            {
                beforeSize = TextRenderer.MeasureText(g, beforeSelection, font, actualTextRect.Size, measureFlags);
            }
            
            Size selectedSize = TextRenderer.MeasureText(g, selectedText, font, actualTextRect.Size, measureFlags);
            
            Rectangle selectionRect = new Rectangle(
                actualTextRect.X + beforeSize.Width - (!_textBox.Multiline ? ScrollX : 0),
                actualTextRect.Y,
                selectedSize.Width,
                Math.Max(selectedSize.Height, actualTextRect.Height));
            
            // Ensure selection rectangle doesn't exceed actual text area
            if (selectionRect.Right > actualTextRect.Right)
            {
                selectionRect.Width = actualTextRect.Right - selectionRect.X;
            }
            
            if (selectionRect.Width > 0 && selectionRect.Height > 0)
            {
                // Draw selection background
                using (var brush = new SolidBrush(_textBox.SelectionBackColor))
                {
                    g.FillRectangle(brush, selectionRect);
                }
                
                // The theme owns the selection pair - a wrong-looking combination is the
                // theme's bug, not something to brightness-guess around.
                Color selectedTextColor = ThemeManagement.BeepThemesManager.CurrentTheme.TextBoxSelectedForeColor;
                
                TextRenderer.DrawText(g, selectedText, font, selectionRect, selectedTextColor, GetTextFormatFlags());
            }
        }
        
        /// <summary>
        /// Draw the text cursor/caret
        /// </summary>
        private void DrawCaret(Graphics g, Rectangle textRect)
        {
            var control = _textBox as Control;
            if (control == null || !control.Focused) return;
            
            // Only draw caret if no selection is active
            if (_textBox.SelectionLength > 0) return;
            
            string text = GetActualText();
            // Paint at the CARET, not the selection anchor - the anchor stays where a
            // selection began and made the caret render at the wrong position after End/click.
            int caretPosition = (_textBox as BeepTextBox)?.VisibleCaretPosition ?? _textBox.SelectionStart;
            
            Font font = _textBox.TextFont ?? BeepFontManager.GetFont("Segoe UI", 9f);
            
            // Get the actual text rectangle considering image layout
            Rectangle actualTextRect = GetEffectiveTextRect(textRect);
            
            // Use the same TextFormatFlags as drawing to ensure consistent measurement
            TextFormatFlags measureFlags = GetTextFormatFlags();
            
            // Determine left offset based on alignment
            int baseX = actualTextRect.X;
            Size fullTextSize = Size.Empty;
            if (!string.IsNullOrEmpty(text))
            {
                fullTextSize = TextRenderer.MeasureText(g, text, font, actualTextRect.Size, measureFlags);
                if (_textBox.TextAlignment == HorizontalAlignment.Center)
                {
                    baseX = actualTextRect.X + Math.Max(0, (actualTextRect.Width - fullTextSize.Width) / 2);
                }
                else if (_textBox.TextAlignment == HorizontalAlignment.Right)
                {
                    baseX = actualTextRect.Right - fullTextSize.Width;
                }
            }
            
            // Calculate caret position within the actual text area
            int caretX = baseX;
            if (!string.IsNullOrEmpty(text) && caretPosition > 0)
            {
                string textBeforeCaret = text.Substring(0, Math.Min(caretPosition, text.Length));
                Size textSize = TextRenderer.MeasureText(g, textBeforeCaret, font, actualTextRect.Size, measureFlags);
                caretX = baseX + textSize.Width;
            }
            if (!_textBox.Multiline) caretX -= ScrollX;
            
            // Clamp caret within actualTextRect
            if (caretX < actualTextRect.X) caretX = actualTextRect.X;
            if (caretX > actualTextRect.Right) caretX = actualTextRect.Right;
            
            using (var pen = new Pen(control.ForeColor, 1))
            {
                g.DrawLine(pen, caretX, actualTextRect.Y + 2, caretX, actualTextRect.Bottom - 2);
            }
        }
        
        
        /// <summary>
        /// Draw line numbers for multiline textbox
        /// </summary>
        private void DrawLineNumbers(Graphics g, Rectangle clientRect, Rectangle textRect)
        {
            if (!_textBox.ShowLineNumbers || !_textBox.Multiline) return;
            
            // Get line number area
            Rectangle lineNumberRect = new Rectangle(
                clientRect.X, clientRect.Y,
                _textBox.LineNumberMarginWidth, clientRect.Height);
            
            // Fill line number background
            using (var brush = new SolidBrush(_textBox.LineNumberBackColor))
            {
                g.FillRectangle(brush, lineNumberRect);
            }
            
            // Draw line numbers
            var lines = _textBox.GetLines();
            Font lineFont = _textBox.LineNumberFont ?? _textBox.TextFont ?? BeepFontManager.GetFont("Consolas", 8f);
            
            for (int i = 0; i < lines.Count; i++)
            {
                string lineNumber = (i + 1).ToString();
                Rectangle lineRect = new Rectangle(
                    lineNumberRect.X + 2,
                    lineNumberRect.Y + i * GetLineHeight(g, lineFont),
                    lineNumberRect.Width - 4,
                    GetLineHeight(g, lineFont));
                
                TextRenderer.DrawText(g, lineNumber, lineFont, lineRect,
                    _textBox.LineNumberForeColor, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            }
            
            // Draw separator line
            using (var pen = new Pen(Color.FromArgb(100, _textBox.LineNumberForeColor)))
            {
                g.DrawLine(pen, lineNumberRect.Right - 1, lineNumberRect.Top,
                    lineNumberRect.Right - 1, lineNumberRect.Bottom);
            }
        }
        
        #region Helper Methods
        
        private bool ShouldDrawLineNumbers()
        {
            return _textBox.ShowLineNumbers && _textBox.Multiline;
        }
        
        private bool ShouldDrawPlaceholder()
        {
            // FIX: Use _textBox.Text directly, not GetActualText()
            // GetActualText() might filter out valid text
            string actualText = _textBox.Text;
            
            // Only show placeholder if truly no text
            return string.IsNullOrEmpty(actualText) && !string.IsNullOrEmpty(_textBox.PlaceholderText);
        }
        
        private bool HasImage()
        {
            var beepImg = _textBox.BeepImage;
            if (beepImg == null) return false;
            
            // Require: ImageVisible flag, control BeepImage.Visible, non-empty path, and actual image loaded
            bool imageVisibleFlags = _textBox.ImageVisible && beepImg.Visible;
            bool hasPath = !string.IsNullOrWhiteSpace(_textBox.ImagePath);
            // BeepImage.HasImage is a typed property - the old dynamic probe with a
            // swallowing catch was reflection for something statically available.
            return imageVisibleFlags && hasPath && beepImg.HasImage;
        }
        
        private Size GetImageSize()
        {
            if (!HasImage()) return Size.Empty;
            
            Size imageSize = _textBox.BeepImage.GetImageSize();
            // The clamp scales with DPI - a fixed 20px icon shrinks relative to text at 150%.
            Size maxSize = OwnerControl == null
                ? _textBox.MaxImageSize
                : DpiScalingHelper.ScaleSize(_textBox.MaxImageSize, OwnerControl);
            
            if (imageSize.Width > maxSize.Width || imageSize.Height > maxSize.Height)
            {
                float scaleFactor = Math.Min(
                    (float)maxSize.Width / imageSize.Width,
                    (float)maxSize.Height / imageSize.Height);
                
                return new Size(
                    (int)(imageSize.Width * scaleFactor),
                    (int)(imageSize.Height * scaleFactor));
            }
            
            return imageSize;
        }
        
        private Size GetTextSize(Graphics g)
        {
            string displayText = ShouldDrawPlaceholder() ? _textBox.PlaceholderText : GetActualText();
            if (string.IsNullOrEmpty(displayText)) return Size.Empty;
            
            Font font = _textBox.TextFont ?? BeepFontManager.GetFont("Segoe UI", 9f);
            SizeF sizeF = TextUtils.MeasureText(g, displayText, font);
            return new Size((int)sizeF.Width, (int)sizeF.Height);
        }
        
        private string GetDisplayText()
        {
            return ShouldDrawPlaceholder() ? _textBox.PlaceholderText : GetActualText();
        }
        
        /// <summary>
        /// Gets the actual text without placeholder logic
        /// </summary>
        private string GetActualText()
        {
            // While an effect animates, paint its frame text (typewriter partials, scramble
            // frames). Terminal paints its own full surface and FadeIn paints an alpha overlay -
            // both suppress the base text so it cannot show at full opacity underneath.
            if (_textBox is BeepTextBox fx && fx.HasActiveEffectVisual)
            {
                if (fx.TerminalModeEnabled || fx.EffectMode == Models.TextEffectMode.FadeIn)
                    return string.Empty;
                return fx.EffectFrameText ?? string.Empty;
            }

            string text = _textBox.Text;

            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            if (_textBox is BeepTextBox beepBox && beepBox.PasswordRevealed)
                return text;

            if (_textBox.UseSystemPasswordChar && !string.IsNullOrEmpty(text))
            {
                return new string('\u2022', text.Length);
            }
            else if (_textBox.PasswordChar != '\0' && !string.IsNullOrEmpty(text))
            {
                return new string(_textBox.PasswordChar, text.Length);
            }

            return text;
        }
        
        private ContentAlignment GetTextAlignment()
        {
            return _textBox.TextAlignment switch
            {
                HorizontalAlignment.Left => ContentAlignment.MiddleLeft,
                HorizontalAlignment.Center => ContentAlignment.MiddleCenter,
                HorizontalAlignment.Right => ContentAlignment.MiddleRight,
                _ => ContentAlignment.MiddleLeft
            };
        }
        
        private Color GetTextColor()
        {
            var control = _textBox as Control;
            if (control != null)
            {
                return control.Enabled ? control.ForeColor : GetDisabledTextColor();
            }
            return GetDefaultTextColor();
        }
        
        private static Color GetDisabledTextColor()
            => ThemeManagement.BeepThemesManager.CurrentTheme.DisabledForeColor;

        private static Color GetDefaultTextColor()
            => ThemeManagement.BeepThemesManager.CurrentTheme.TextBoxForeColor;
        
        private TextFormatFlags GetTextFormatFlags()
        {
            TextFormatFlags flags = TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.NoPadding;
            
            // RTL support
            if (_textBox is Control ctrl && ctrl.RightToLeft == RightToLeft.Yes)
            {
                flags |= TextFormatFlags.RightToLeft;
            }
            
            // Alignment
            switch (_textBox.TextAlignment)
            {
                case HorizontalAlignment.Left:
                    flags |= TextFormatFlags.Left;
                    break;
                case HorizontalAlignment.Center:
                    flags |= TextFormatFlags.HorizontalCenter;
                    break;
                case HorizontalAlignment.Right:
                    flags |= TextFormatFlags.Right;
                    break;
            }
            
            // Multiline behavior must be based on the interface, not a specific control type
            if (_textBox.Multiline)
            {
                // Enable multiline rendering with line breaks support
                flags |= TextFormatFlags.TextBoxControl;
                flags |= TextFormatFlags.Top; // top-align for multiline
                flags |= TextFormatFlags.ExpandTabs; // better tab handling
                
                // WordWrap is on the interface now - the old code probed it by reflection.
                if (_textBox.WordWrap)
                {
                    flags |= TextFormatFlags.WordBreak;
                }
                else
                {
                    // No WordBreak => do not wrap long lines (still honor explicit newlines)
                }
                
                // Remove single-line-only flags
                flags &= ~TextFormatFlags.SingleLine;
                flags &= ~TextFormatFlags.VerticalCenter;
                flags &= ~TextFormatFlags.EndEllipsis;
                flags &= ~TextFormatFlags.NoClipping; // allow clipping to rect
            }
            else
            {
                // Single line behavior
                flags |= TextFormatFlags.SingleLine;
                flags |= TextFormatFlags.VerticalCenter;
                flags |= TextFormatFlags.EndEllipsis;
                flags |= TextFormatFlags.NoClipping;
            }
            
            return flags;
        }
        
        private int GetLineHeight(Graphics g, Font font)
        {
            if (_performance != null)
                return (int)_performance.GetCachedLineHeight(g, font);
            
            SizeF sizeF = TextUtils.MeasureText(g, "Ag", font);
            return (int)sizeF.Height;
        }
        
        #endregion
    }
}