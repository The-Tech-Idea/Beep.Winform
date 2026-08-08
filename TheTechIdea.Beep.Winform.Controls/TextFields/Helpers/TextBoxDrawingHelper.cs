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
        private int ScrollY => (_textBox as BeepTextBox)?.VerticalScrollOffset ?? 0;

        /// <summary>
        /// Left origin the text is drawn from. No overflow: the alignment decides. Overflow:
        /// while focused the caret-follow offset governs (Home shows the head even when
        /// right-aligned); unfocused, the alignment's natural anchor governs (right-aligned
        /// long text shows its tail, centred shows its middle).
        /// </summary>
        /// <summary>
        /// Alignment with RTL applied: RightToLeft flips Left and Right (a native RTL
        /// textbox right-aligns its text; the flag alone only changes reading order).
        /// </summary>
        private HorizontalAlignment EffectiveAlignment
        {
            get
            {
                var a = _textBox.TextAlignment;
                if (_textBox is Control c && c.RightToLeft == RightToLeft.Yes)
                {
                    if (a == HorizontalAlignment.Left) return HorizontalAlignment.Right;
                    if (a == HorizontalAlignment.Right) return HorizontalAlignment.Left;
                }
                return a;
            }
        }

        private int GetTextOriginX(Rectangle rect, int fullWidth)
        {
            if (fullWidth <= rect.Width)
            {
                return EffectiveAlignment switch
                {
                    HorizontalAlignment.Center => rect.X + (rect.Width - fullWidth) / 2,
                    HorizontalAlignment.Right => rect.Right - fullWidth,
                    _ => rect.X,
                };
            }

            bool focused = (_textBox as Control)?.Focused == true;
            int hidden = focused
                ? ScrollX
                : EffectiveAlignment switch
                {
                    HorizontalAlignment.Center => (fullWidth - rect.Width) / 2,
                    HorizontalAlignment.Right => fullWidth - rect.Width,
                    _ => ScrollX,
                };
            return rect.X - hidden;
        }

        /// <summary>
        /// THE text rectangle: content rect inset, then adjusted for the image zone.
        /// Text, placeholder, selection, caret and search highlights must all use this one
        /// method - previously the text drew in an inset rect while caret/placeholder/selection
        /// used the un-inset one, so the caret sat 2px left of the first character.
        /// </summary>
        /// <summary>
        /// One visual line of the multiline layout: its text, where it starts in the full
        /// string, which raw (newline-delimited) line it belongs to, and whether it is that
        /// raw line's first segment (the gutter numbers those).
        /// </summary>
        public struct VisualLine
        {
            public string Text;
            public int StartIndex;
            public int RawLine;
            public bool IsRawStart;
        }

        private List<VisualLine> _layout;
        private string _layoutText;
        private int _layoutWidth = -1;
        private string _layoutFontKey;
        private bool _layoutWrap;

        private static readonly Size Unbounded = new Size(int.MaxValue, int.MaxValue);
        private const TextFormatFlags MeasureFlags = TextFormatFlags.NoPadding;

        private int MeasureWidth(Graphics g, Font font, string text) =>
            string.IsNullOrEmpty(text) ? 0 : TextRenderer.MeasureText(g, text, font, Unbounded, MeasureFlags).Width;

        /// <summary>
        /// The wrapped layout the multiline pipeline draws from. Text, caret, selection,
        /// line numbers and the scroll range all consume THIS - one wrap authority, so
        /// what is measured is exactly what is painted. Cached per (text, width, font, wrap).
        /// </summary>
        public IReadOnlyList<VisualLine> GetVisualLines(Graphics g, Rectangle textRect)
        {
            string text = GetActualText() ?? string.Empty;
            Font font = _textBox.TextFont ?? BeepFontManager.GetFont("Segoe UI", 9f);
            string fontKey = font.Name + "|" + font.Size + "|" + (int)font.Style;
            bool wrap = _textBox.WordWrap;
            if (_layout != null && _layoutText == text && _layoutWidth == textRect.Width
                && _layoutFontKey == fontKey && _layoutWrap == wrap)
                return _layout;

            var lines = new List<VisualLine>();
            int pos = 0, raw = 0;
            while (pos <= text.Length)
            {
                int nl = text.IndexOfAny(new[] { '\r', '\n' }, pos);
                string rawLine = nl < 0 ? text.Substring(pos) : text.Substring(pos, nl - pos);
                AppendWrapped(g, font, rawLine, pos, raw, wrap ? Math.Max(8, textRect.Width) : int.MaxValue, lines);
                if (nl < 0) break;
                pos = nl + (text[nl] == '\r' && nl + 1 < text.Length && text[nl + 1] == '\n' ? 2 : 1);
                raw++;
                if (pos == text.Length)
                {
                    // Trailing newline: the empty final line is real (the caret can sit on it).
                    lines.Add(new VisualLine { Text = string.Empty, StartIndex = pos, RawLine = raw, IsRawStart = true });
                    break;
                }
            }
            if (lines.Count == 0)
                lines.Add(new VisualLine { Text = string.Empty, StartIndex = 0, RawLine = 0, IsRawStart = true });

            _layout = lines; _layoutText = text; _layoutWidth = textRect.Width;
            _layoutFontKey = fontKey; _layoutWrap = wrap;
            return _layout;
        }

        private void AppendWrapped(Graphics g, Font font, string rawLine, int startIndex, int rawIdx, int maxWidth, List<VisualLine> into)
        {
            if (rawLine.Length == 0 || maxWidth == int.MaxValue || MeasureWidth(g, font, rawLine) <= maxWidth)
            {
                into.Add(new VisualLine { Text = rawLine, StartIndex = startIndex, RawLine = rawIdx, IsRawStart = true });
                return;
            }

            int lineStart = 0;
            bool first = true;
            while (lineStart < rawLine.Length)
            {
                int fit = FitChars(g, font, rawLine, lineStart, maxWidth);
                int cut = lineStart + Math.Max(1, fit);
                if (cut < rawLine.Length)
                {
                    // Prefer breaking after the last space that still fits.
                    int space = rawLine.LastIndexOf(' ', cut - 1, cut - lineStart);
                    if (space > lineStart) cut = space + 1;
                }
                into.Add(new VisualLine
                {
                    Text = rawLine.Substring(lineStart, cut - lineStart),
                    StartIndex = startIndex + lineStart,
                    RawLine = rawIdx,
                    IsRawStart = first,
                });
                first = false;
                lineStart = cut;
            }
        }

        private int FitChars(Graphics g, Font font, string text, int start, int maxWidth)
        {
            int lo = 1, hi = text.Length - start, best = 1;
            while (lo <= hi)
            {
                int mid = lo + (hi - lo) / 2;
                if (MeasureWidth(g, font, text.Substring(start, mid)) <= maxWidth) { best = mid; lo = mid + 1; }
                else hi = mid - 1;
            }
            return best;
        }

        private static int FindVisualLine(IReadOnlyList<VisualLine> lines, int charIndex)
        {
            int idx = 0;
            for (int i = 0; i < lines.Count; i++)
                if (lines[i].StartIndex <= charIndex) idx = i;
                else break;
            return idx;
        }

        /// <summary>Left origin of one visual line under the multiline alignment.</summary>
        private int LineOriginX(Graphics g, Font font, string lineText, Rectangle rect)
        {
            return EffectiveAlignment switch
            {
                HorizontalAlignment.Center => rect.X + Math.Max(0, (rect.Width - MeasureWidth(g, font, lineText)) / 2),
                HorizontalAlignment.Right => Math.Max(rect.X, rect.Right - MeasureWidth(g, font, lineText)),
                _ => rect.X,
            };
        }

        /// <summary>
        /// Caret index for a click point, from the SAME layout the text paints with -
        /// clicks in wrapped text used single-line math and landed on the wrong line.
        /// </summary>
        public int GetCaretIndexFromPoint(Graphics g, Rectangle textRect, Point p)
        {
            var rect = GetEffectiveTextRect(textRect);
            Font font = _textBox.TextFont ?? BeepFontManager.GetFont("Segoe UI", 9f);
            string text = GetActualText() ?? string.Empty;

            if (_textBox.Multiline)
            {
                var lines = GetVisualLines(g, rect);
                int lh = Math.Max(1, GetLineHeight(g, font));
                int row = (p.Y - rect.Y + ScrollY) / lh;
                row = Math.Max(0, Math.Min(row, lines.Count - 1));
                var vl = lines[row];
                int originX = LineOriginX(g, font, vl.Text, rect);
                return vl.StartIndex + NearestCharIndex(g, font, vl.Text, p.X - originX);
            }

            int fullW = MeasureWidth(g, font, text);
            int origin = GetTextOriginX(rect, fullW);
            return NearestCharIndex(g, font, text, p.X - origin);
        }

        private int NearestCharIndex(Graphics g, Font font, string line, int relX)
        {
            if (relX <= 0 || line.Length == 0) return 0;
            int best = 0, bestDelta = int.MaxValue;
            for (int i = 0; i <= line.Length; i++)
            {
                int w = i == 0 ? 0 : MeasureWidth(g, font, line.Substring(0, i));
                int d = Math.Abs(w - relX);
                if (d < bestDelta) { bestDelta = d; best = i; }
                if (w > relX + bestDelta) break;
            }
            return best;
        }

        /// <summary>
        /// Pixel position (x, line top y) of a character index, from the layout - the IME
        /// composition underline and any caret-anchored adornment share the text's geometry.
        /// </summary>
        public Point GetCaretPixelPosition(Graphics g, Rectangle textRect, int charIndex)
        {
            var rect = GetEffectiveTextRect(textRect);
            Font font = _textBox.TextFont ?? BeepFontManager.GetFont("Segoe UI", 9f);
            string text = GetActualText() ?? string.Empty;
            charIndex = Math.Max(0, Math.Min(charIndex, text.Length));

            if (_textBox.Multiline)
            {
                var lines = GetVisualLines(g, rect);
                int lh = GetLineHeight(g, font);
                int idx = FindVisualLine(lines, charIndex);
                var vl = lines[idx];
                int local = Math.Max(0, Math.Min(charIndex - vl.StartIndex, vl.Text.Length));
                int x = LineOriginX(g, font, vl.Text, rect)
                      + (local == 0 ? 0 : MeasureWidth(g, font, vl.Text.Substring(0, local)));
                return new Point(x, rect.Y + idx * lh - ScrollY);
            }

            int fullW = MeasureWidth(g, font, text);
            int originX = GetTextOriginX(rect, fullW);
            int cx = originX + (charIndex == 0 ? 0 : MeasureWidth(g, font, text.Substring(0, charIndex)));
            return new Point(cx, rect.Y);
        }

        /// <summary>Visual line count for the scroll range - the coordinator pushes this to the scrolling helper.</summary>
        public int GetVisualLineCount(Graphics g, Rectangle textRect) => GetVisualLines(g, textRect).Count;

        /// <summary>Line height in pixels for scroll metrics.</summary>
        public int GetLineHeightPx(Graphics g) =>
            GetLineHeight(g, _textBox.TextFont ?? BeepFontManager.GetFont("Segoe UI", 9f));

        /// <summary>
        /// Caret's visual line from the CACHED layout (no Graphics at caret-move time).
        /// Returns -1 when no layout has been built yet - callers fall back to raw lines.
        /// </summary>
        public int GetCaretVisualLineFromCache(int caretPosition)
        {
            var layout = _layout;
            if (layout == null || layout.Count == 0) return -1;
            return FindVisualLine(layout, Math.Max(0, caretPosition));
        }

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
            
            if (_textBox.Multiline)
            {
                // Per-visual-line drawing from the ONE layout authority - what the caret,
                // selection, gutter and scroll range measure is exactly what paints.
                var lines = GetVisualLines(g, textRect);
                int lh = GetLineHeight(g, font);
                int scrollY = ScrollY;
                var lineFlags = MeasureFlags | TextFormatFlags.PreserveGraphicsClipping;
                var state = g.Save();
                g.SetClip(textRect);
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Text.Length == 0) continue;
                    int y = textRect.Y + i * lh - scrollY;
                    if (y + lh < textRect.Top || y > textRect.Bottom) continue;
                    int x = LineOriginX(g, font, lines[i].Text, textRect);
                    TextRenderer.DrawText(g, lines[i].Text, font,
                        new Rectangle(x, y, textRect.Width, lh), textColor, lineFlags);
                }
                g.Restore(state);
                return;
            }

            // Single line: one origin model for every alignment. Overflowing text draws
            // left-aligned from the computed origin inside a clip (alignment decides the
            // natural anchor; the caret-follow offset governs while focused).
            var plainFlags = flags & ~TextFormatFlags.HorizontalCenter & ~TextFormatFlags.Right;
            int fullW = TextRenderer.MeasureText(g, displayText, font,
                new Size(int.MaxValue, int.MaxValue), plainFlags).Width;
            if (fullW > textRect.Width)
            {
                var state = g.Save();
                g.SetClip(textRect);
                int originX = GetTextOriginX(textRect, fullW);
                var shifted = new Rectangle(originX, textRect.Y, fullW + 2, textRect.Height);
                TextRenderer.DrawText(g, displayText, font, shifted, textColor,
                                      plainFlags | TextFormatFlags.Left);
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
            if (!HasImage()) return;
            // User directive: JUST PAINT the icon - no ApplyTheme on the image, no tint,
            // no fill. The SVG renders with its own artwork colours through the plain
            // StyledImagePainter path (which rasterizes at the requested size).
            Styling.ImagePainters.StyledImagePainter.Paint(g, imageRect, _textBox.ImagePath);
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

            if (_textBox.Multiline)
            {
                DrawMultilineSelection(g, actualTextRect, font, text, selStart, selLength);
                return;
            }
            
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
            
            int selFullW = TextRenderer.MeasureText(g, text, font,
                new Size(int.MaxValue, int.MaxValue), measureFlags).Width;
            int selOriginX = _textBox.Multiline ? actualTextRect.X : GetTextOriginX(actualTextRect, selFullW);
            Rectangle selectionRect = new Rectangle(
                selOriginX + beforeSize.Width,
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
        /// Multiline selection: one fill + ink per visual line the range touches. The old
        /// code measured the whole prefix as a single line, so multi-row selections drew
        /// as one misplaced band.
        /// </summary>
        private void DrawMultilineSelection(Graphics g, Rectangle rect, Font font, string text, int selStart, int selLength)
        {
            int selEnd = Math.Min(selStart + selLength, text.Length);
            var lines = GetVisualLines(g, rect);
            int lh = GetLineHeight(g, font);
            Color selInk = ThemeManagement.BeepThemesManager.CurrentTheme.TextBoxSelectedForeColor;
            var lineFlags = MeasureFlags | TextFormatFlags.PreserveGraphicsClipping;

            var state = g.Save();
            g.SetClip(rect);
            using var back = new SolidBrush(_textBox.SelectionBackColor);
            for (int i = 0; i < lines.Count; i++)
            {
                var vl = lines[i];
                int lineEnd = vl.StartIndex + vl.Text.Length;
                int s0 = Math.Max(selStart, vl.StartIndex);
                int e0 = Math.Min(selEnd, lineEnd);
                bool coversNewline = selEnd > lineEnd && selStart <= lineEnd;
                if (s0 >= e0 && !coversNewline) continue;

                int y = rect.Y + i * lh - ScrollY;
                if (y + lh < rect.Top || y > rect.Bottom) continue;

                int originX = LineOriginX(g, font, vl.Text, rect);
                int x1 = originX + (s0 > vl.StartIndex ? MeasureWidth(g, font, vl.Text.Substring(0, s0 - vl.StartIndex)) : 0);
                int x2 = originX + (e0 > vl.StartIndex ? MeasureWidth(g, font, vl.Text.Substring(0, e0 - vl.StartIndex)) : 0);
                if (coversNewline) x2 += 4; // show that the line break is inside the selection
                if (x2 <= x1) x2 = x1 + 4;

                g.FillRectangle(back, new Rectangle(x1, y, x2 - x1, lh));
                if (e0 > s0)
                {
                    string part = vl.Text.Substring(s0 - vl.StartIndex, e0 - s0);
                    TextRenderer.DrawText(g, part, font, new Rectangle(x1, y, x2 - x1, lh), selInk, lineFlags);
                }
            }
            g.Restore(state);
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

            if (_textBox.Multiline)
            {
                // Real (line, column) placement from the layout - the old math measured the
                // whole prefix as ONE line, so the caret drifted right instead of down.
                var mlLines = GetVisualLines(g, actualTextRect);
                int mlLh = GetLineHeight(g, font);
                int mlIdx = FindVisualLine(mlLines, caretPosition);
                var mlLine = mlLines[mlIdx];
                int mlLocal = Math.Max(0, Math.Min(caretPosition - mlLine.StartIndex, mlLine.Text.Length));
                int mlX = LineOriginX(g, font, mlLine.Text, actualTextRect)
                        + (mlLocal == 0 ? 0 : MeasureWidth(g, font, mlLine.Text.Substring(0, mlLocal)));
                int mlY = actualTextRect.Y + mlIdx * mlLh - ScrollY;
                if (mlY + mlLh < actualTextRect.Top || mlY > actualTextRect.Bottom) return; // off-view
                mlX = Math.Max(actualTextRect.X, Math.Min(mlX, actualTextRect.Right - 1));
                using (var mlPen = new Pen(control.ForeColor, 1))
                {
                    g.DrawLine(mlPen, mlX, mlY + 1, mlX, mlY + mlLh - 1);
                }
                return;
            }
            
            // Use the same TextFormatFlags as drawing to ensure consistent measurement
            TextFormatFlags measureFlags = GetTextFormatFlags();
            
            // One origin model with DrawText - alignment when it fits, scroll when it overflows.
            int baseX = actualTextRect.X;
            if (!string.IsNullOrEmpty(text))
            {
                int fullW = TextRenderer.MeasureText(g, text, font,
                    new Size(int.MaxValue, int.MaxValue), measureFlags).Width;
                baseX = GetTextOriginX(actualTextRect, fullW);
            }
            
            // Calculate caret position within the actual text area
            int caretX = baseX;
            if (!string.IsNullOrEmpty(text) && caretPosition > 0)
            {
                string textBeforeCaret = text.Substring(0, Math.Min(caretPosition, text.Length));
                Size textSize = TextRenderer.MeasureText(g, textBeforeCaret, font, actualTextRect.Size, measureFlags);
                caretX = baseX + textSize.Width;
            }
            
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

            Rectangle lineNumberRect = new Rectangle(
                clientRect.X, clientRect.Y,
                _textBox.LineNumberMarginWidth, clientRect.Height);

            using (var brush = new SolidBrush(_textBox.LineNumberBackColor))
            {
                g.FillRectangle(brush, lineNumberRect);
            }

            // Rows come from the SAME visual layout the text draws from, so numbers stay
            // beside their lines when wrapping and scrolling. Wrapped continuation rows
            // carry no number - only a raw line's first segment does.
            Font textFont = _textBox.TextFont ?? BeepFontManager.GetFont("Segoe UI", 9f);
            Font lineFont = _textBox.LineNumberFont ?? textFont;
            var lines = GetVisualLines(g, GetEffectiveTextRect(textRect));
            int lh = GetLineHeight(g, textFont);

            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].IsRawStart) continue;
                int y = lineNumberRect.Y + i * lh - ScrollY;
                if (y + lh < lineNumberRect.Top || y > lineNumberRect.Bottom) continue;
                Rectangle lineRect = new Rectangle(lineNumberRect.X + 2, y, lineNumberRect.Width - 4, lh);
                TextRenderer.DrawText(g, (lines[i].RawLine + 1).ToString(), lineFont, lineRect,
                    _textBox.LineNumberForeColor, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            }

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
            
            // Alignment (RTL flips Left/Right)
            switch (EffectiveAlignment)
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