using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Icons;

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Helpers
{
    /// <summary>
    /// Enhanced drawing helper for BeepMaterialTextField with Material Design rendering
    /// Handles all drawing operations including Material Design elements and BeepSimpleTextBox features
    /// </summary>
    public class MaterialTextFieldDrawingHelper : IDisposable
    {
        #region Private Fields
        private readonly BeepMaterialTextField _textField;
        private readonly BeepMaterialTextFieldHelper _materialHelper;
        
        // Cached brushes and pens for performance
        private SolidBrush _textBrush;
        private SolidBrush _placeholderBrush;
        private SolidBrush _labelBrush;
        private SolidBrush _helperTextBrush;
        private SolidBrush _selectionBrush;
        private Pen _borderPen;
        private Pen _focusBorderPen;
        private Pen _caretPen;
        
        // Material Design constants
        private const float FLOATING_LABEL_SCALE = 0.75f;
        private const int OUTLINED_BORDER_RADIUS = 4;
        private const int FILLED_BORDER_RADIUS = 4;
        
        #endregion

        #region Constructor
        public MaterialTextFieldDrawingHelper(BeepMaterialTextField textField, BeepMaterialTextFieldHelper materialHelper)
        {
            _textField = textField ?? throw new ArgumentNullException(nameof(textField));
            _materialHelper = materialHelper ?? throw new ArgumentNullException(nameof(materialHelper));
            
            InitializeBrushesAndPens();
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Main drawing method that handles all Material Design rendering
        /// </summary>
        public void DrawAll(Graphics g, Rectangle drawingRect)
        {
            if (g == null) return;
            
            // Set high quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            
            // Draw Material Design field background and border
            DrawFieldBackground(g, drawingRect);
            
            // Draw ripple effect if active
            DrawRippleEffect(g, drawingRect);
            
            // Draw prefix/suffix text
            DrawPrefixSuffixText(g, drawingRect);
            
            // Draw icons - both leading and trailing
            DrawIcons(g, drawingRect);
            
            // Draw floating label
            DrawFloatingLabel(g, drawingRect);
            
            // Draw text content
            DrawTextContent(g, drawingRect);
            
            // Draw placeholder if needed
            DrawPlaceholder(g, drawingRect);
            
            // Draw selection
            DrawSelection(g, drawingRect);
            
            // Draw caret if focused
            if (_textField.IsFocused && !_textField.ReadOnly)
            {
                DrawCaret(g, drawingRect);
            }
            
            // Draw helper text
            DrawHelperText(g, drawingRect);
            
            // Draw character counter if enabled
            if (_textField.ShowCharacterCounter)
            {
                DrawCharacterCounter(g, drawingRect);
            }
            
            // Draw line numbers if enabled
            if (_textField.ShowLineNumbers && _textField.Multiline)
            {
                DrawLineNumbers(g, drawingRect);
            }
        }

        #endregion

        #region Material Design Drawing Methods

        /// <summary>
        /// Draw Material Design field background and border
        /// </summary>
        private void DrawFieldBackground(Graphics g, Rectangle drawingRect)
        {
            // Use the actual input rectangle for border/fill so supporting text sits outside the border
            var inputRectRel = GetInputRectangle();
            var inputRect = new Rectangle(
                drawingRect.X + inputRectRel.X,
                drawingRect.Y + inputRectRel.Y,
                inputRectRel.Width,
                inputRectRel.Height);

            Color borderColor = GetEffectiveBorderColor();
            int borderWidth = _textField.IsFocused ? 2 : 1;

            // Determine border radius
            int borderRadius = _textField.CurvedBorderRadius;
            if (_textField.SearchBoxStyle)
            {
                borderRadius = Math.Max(inputRect.Height / 2, 20);
            }

            switch (_textField.Variant)
            {
                case MaterialTextFieldVariant.Filled:
                    DrawFilledBackground(g, inputRect, borderColor, borderWidth, borderRadius);
                    break;

                case MaterialTextFieldVariant.Outlined:
                    DrawOutlinedBackground(g, inputRect, borderColor, borderWidth, borderRadius);
                    break;

                case MaterialTextFieldVariant.Standard:
                default:
                    DrawStandardBackground(g, inputRect, borderColor, borderWidth, borderRadius);
                    break;
            }
        }

        /// <summary>
        /// Draw standard background with curved styling
        /// </summary>
        private void DrawStandardBackground(Graphics g, Rectangle drawingRect, Color borderColor, int borderWidth, int borderRadius)
        {
            // Background fill if enabled
            if (_textField.ShowFill)
            {
                using (var fillBrush = new SolidBrush(_textField.FillColor))
                {
                    var bgRect = new RectangleF(drawingRect.X, drawingRect.Y, drawingRect.Width, drawingRect.Height);
                    if (borderRadius > 0)
                    {
                        using (var path = CreateRoundedRectanglePath(bgRect, borderRadius))
                        {
                            g.FillPath(fillBrush, path);
                        }
                    }
                    else
                    {
                        g.FillRectangle(fillBrush, bgRect);
                    }
                }
            }

            // Always draw the main border first
            using (var borderPen = new Pen(borderColor, borderWidth))
            {
                var rect = new RectangleF(
                    drawingRect.X + borderWidth / 2f,
                    drawingRect.Y + borderWidth / 2f,
                    drawingRect.Width - borderWidth,
                    drawingRect.Height - borderWidth);

                if (borderRadius > 0)
                {
                    using (var path = CreateRoundedRectanglePath(rect, borderRadius))
                    {
                        g.DrawPath(borderPen, path);
                    }
                }
                else
                {
                    g.DrawRectangle(borderPen, Rectangle.Round(rect));
                }
            }

            // Draw focus overlay (not replacement) when focused
            if (_materialHelper.FocusAnimationProgress > 0 && _textField.IsFocused)
            {
                using (var focusPen = new Pen(_textField.PrimaryColor, Math.Max(1, borderWidth)))
                {
                    var focusRect = new RectangleF(
                        drawingRect.X + borderWidth / 2f,
                        drawingRect.Y + borderWidth / 2f,
                        drawingRect.Width - borderWidth,
                        drawingRect.Height - borderWidth);

                    // Set opacity based on animation progress
                    var focusColor = Color.FromArgb(
                        (int)(255 * _materialHelper.FocusAnimationProgress),
                        _textField.PrimaryColor);
                    focusPen.Color = focusColor;

                    if (borderRadius > 0)
                    {
                        using (var path = CreateRoundedRectanglePath(focusRect, borderRadius))
                        {
                            g.DrawPath(focusPen, path);
                        }
                    }
                    else
                    {
                        g.DrawRectangle(focusPen, Rectangle.Round(focusRect));
                    }
                }
            }
        }

        /// <summary>
        /// Draw filled variant background with curves
        /// </summary>
        private void DrawFilledBackground(Graphics g, Rectangle drawingRect, Color borderColor, int borderWidth, int borderRadius)
        {
            // Background fill
            using (var fillBrush = new SolidBrush(Color.FromArgb(12, _textField.SurfaceColor.R, _textField.SurfaceColor.G, _textField.SurfaceColor.B)))
            {
                var bgRect = new RectangleF(drawingRect.X, drawingRect.Y + 8, drawingRect.Width, drawingRect.Height - 8);
                
                if (borderRadius > 0)
                {
                    // Only round top corners for filled variant
                    using (var path = CreateRoundedRectanglePath(bgRect, borderRadius, borderRadius, 0, 0))
                    {
                        g.FillPath(fillBrush, path);
                    }
                }
                else
                {
                    g.FillRectangle(fillBrush, bgRect);
                }
            }

            // Bottom border
            using (var borderPen = new Pen(borderColor, borderWidth))
            {
                float y = drawingRect.Bottom - borderWidth / 2f;
                g.DrawLine(borderPen, drawingRect.Left, y, drawingRect.Right, y);
            }

            // Animated focus underline
            if (_materialHelper.FocusAnimationProgress > 0)
            {
                using (var focusPen = new Pen(_textField.PrimaryColor, 2))
                {
                    float centerX = drawingRect.Left + drawingRect.Width / 2f;
                    float underlineWidth = drawingRect.Width * _materialHelper.FocusAnimationProgress;
                    float startX = centerX - underlineWidth / 2f;
                    float endX = centerX + underlineWidth / 2f;
                    float y = drawingRect.Bottom - 1;
                    g.DrawLine(focusPen, startX, y, endX, y);
                }
            }
        }

        /// <summary>
        /// Draw outlined variant background with curves
        /// </summary>
        private void DrawOutlinedBackground(Graphics g, Rectangle drawingRect, Color borderColor, int borderWidth, int borderRadius)
        {
            // Compute base border rect
            var rect = new RectangleF(
                drawingRect.X + borderWidth / 2f,
                drawingRect.Y + borderWidth / 2f,
                drawingRect.Width - borderWidth,
                drawingRect.Height - borderWidth);

            // Determine notch when label is floating and we have text
            bool showNotch = _materialHelper.LabelAnimationProgress > 0 && !string.IsNullOrEmpty(_textField.LabelText) && !_textField.SearchBoxStyle;

            // If notch: measure floating label width and compute gap
            float notchStart = 0f, notchEnd = 0f;
            if (showNotch)
            {
                using (var gtmp = _textField.CreateGraphics())
                {
                    var labelSize = TextRenderer.MeasureText(gtmp, _textField.IsRequired ? _textField.LabelText + " *" : _textField.LabelText,
                        _textField.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                    float scale = 1f - _materialHelper.LabelAnimationProgress * (1f - 0.75f);
                    float notchWidth = (labelSize.Width * scale) + 12f; // 12px padding total
                    var labelRect = _materialHelper.GetLabelRectangle();

                    // Translate to drawing space
                    float labelCenterX = drawingRect.X + labelRect.X + (notchWidth / 2f);
                    notchStart = Math.Max(rect.Left + borderRadius, labelCenterX - (notchWidth / 2f));
                    notchEnd = Math.Min(rect.Right - borderRadius, labelCenterX + (notchWidth / 2f));
                }
            }

            using (var pen = new Pen(borderColor, borderWidth))
            {
                using (var path = new GraphicsPath())
                {
                    // Build path with optional notch on top edge
                    float r = Math.Max(0, borderRadius);
                    float d = r * 2f;

                    // Top-left corner arc
                    if (r > 0) path.AddArc(rect.Left, rect.Top, d, d, 180, 90); else path.AddLine(rect.Left, rect.Top, rect.Left, rect.Top);

                    // Top edge: from after TL arc to before TR arc, with optional notch gap
                    float topStart = rect.Left + r;
                    float topEnd = rect.Right - r;

                    if (showNotch && notchEnd > notchStart && notchEnd - notchStart > 4f)
                    {
                        path.AddLine(topStart, rect.Top, notchStart, rect.Top);
                        // Skip the notch gap, move pen to notchEnd
                        path.StartFigure();
                        path.AddLine(notchEnd, rect.Top, topEnd, rect.Top);
                    }
                    else
                    {
                        path.AddLine(topStart, rect.Top, topEnd, rect.Top);
                    }

                    // Top-right corner arc
                    if (r > 0) path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90); else path.AddLine(rect.Right, rect.Top, rect.Right, rect.Top);

                    // Right edge
                    path.AddLine(rect.Right, rect.Top + r, rect.Right, rect.Bottom - r);

                    // Bottom-right arc
                    if (r > 0) path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90); else path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);

                    // Bottom edge
                    path.AddLine(rect.Right - r, rect.Bottom, rect.Left + r, rect.Bottom);

                    // Bottom-left arc
                    if (r > 0) path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90); else path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Bottom);

                    // Left edge
                    path.AddLine(rect.Left, rect.Bottom - r, rect.Left, rect.Top + r);

                    // Draw
                    g.DrawPath(pen, path);
                }
            }

            // Focus overlay
            if (_materialHelper.FocusAnimationProgress > 0 && _textField.IsFocused)
            {
                using (var focusPen = new Pen(Color.FromArgb((int)(255 * _materialHelper.FocusAnimationProgress), _textField.PrimaryColor), borderWidth + 1))
                {
                    g.DrawRectangle(focusPen, Rectangle.Round(rect)); // overlay simple rect for now
                }
            }
        }

        /// <summary>
        /// Draw floating label with animation
        /// </summary>
        private void DrawFloatingLabel(Graphics g, Rectangle drawingRect)
        {
            if (string.IsNullOrEmpty(_textField.LabelText)) return;

            Color labelColor = GetEffectiveLabelColor();
            
            float scale = 1f - _materialHelper.LabelAnimationProgress * (1f - FLOATING_LABEL_SCALE);
            PointF labelPosition = GetAnimatedLabelPosition(drawingRect);

            var state = g.Save();
            g.TranslateTransform(labelPosition.X, labelPosition.Y);
            g.ScaleTransform(scale, scale);

            using (var labelBrush = new SolidBrush(labelColor))
            {
                string displayText = _textField.IsRequired ? $"{_textField.LabelText} *" : _textField.LabelText;
                g.DrawString(displayText, _textField.Font, labelBrush, PointF.Empty);
            }

            g.Restore(state);
        }

        /// <summary>
        /// Draw text content using TextRenderer with BeepSimpleTextBox flags and adjusted text rect
        /// </summary>
        private void DrawTextContent(Graphics g, Rectangle drawingRect)
        {
            if (string.IsNullOrEmpty(_textField.DisplayText)) return;

            var textRect = OffsetRect(_materialHelper.GetAdjustedTextRectangle(), drawingRect);
            var flags = GetTextFormatFlags();
            TextRenderer.DrawText(g, _textField.DisplayText, _textField.Font, textRect, _textField.ForeColor, flags);
        }

        /// <summary>
        /// Draw placeholder text using TextRenderer and adjusted text rect
        /// </summary>
        private void DrawPlaceholder(Graphics g, Rectangle drawingRect)
        {
            if (string.IsNullOrEmpty(_textField.EffectivePlaceholderText)) return;

            var textRect = OffsetRect(_materialHelper.GetAdjustedTextRectangle(), drawingRect);
            var flags = GetTextFormatFlags();
            TextRenderer.DrawText(g, _textField.EffectivePlaceholderText, _textField.Font, textRect, _textField.PlaceholderTextColor, flags);
        }

        /// <summary>
        /// Draw helper text using TextRenderer
        /// </summary>
        private void DrawHelperText(Graphics g, Rectangle drawingRect)
        {
            string textToDraw = _textField.EffectiveHelperText;
            if (string.IsNullOrEmpty(textToDraw)) return;

            var helperRect = OffsetRect(_materialHelper.GetHelperTextRectangle(), drawingRect);
            using (var helperFont = new Font(_textField.Font.FontFamily, Math.Max(8f, _textField.Font.Size - 1f)))
            {
                TextRenderer.DrawText(g, textToDraw, helperFont, helperRect, _textField.EffectiveHelperTextColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
        }

        /// <summary>
        /// Draw ripple effect
        /// </summary>
        private void DrawRippleEffect(Graphics g, Rectangle drawingRect)
        {
            // Implementation would depend on ripple animation state in MaterialHelper
        }

        /// <summary>
        /// Draw icons (leading and trailing) - Enhanced for dual icon support with DrawingRect
        /// </summary>
        private void DrawIcons(Graphics g, Rectangle drawingRect)
        {
            // Draw leading icon if specified (either SVG path or image path)
            if (!string.IsNullOrEmpty(_textField.LeadingIconPath) || !string.IsNullOrEmpty(_textField.LeadingImagePath))
            {
                DrawLeadingIcon(g, drawingRect);
            }

            // Draw trailing icon or clear button
            if (!string.IsNullOrEmpty(_textField.TrailingIconPath) || 
                !string.IsNullOrEmpty(_textField.TrailingImagePath) ||
                (_textField.ShowClearButton && _textField.HasContent))
            {
                DrawTrailingIcon(g, drawingRect);
            }
        }

        /// <summary>
        /// Draw leading icon using SVG path or image path
        /// </summary>
        private void DrawLeadingIcon(Graphics g, Rectangle drawingRect)
        {
            var iconRect = GetLeadingIconRectangle();
            if (iconRect.IsEmpty) return;

            // Adjust icon rect to be relative to drawingRect
            iconRect.Offset(drawingRect.X, drawingRect.Y);

            string iconPath = !string.IsNullOrEmpty(_textField.LeadingIconPath) 
                ? _textField.LeadingIconPath 
                : _textField.LeadingImagePath;

            if (string.IsNullOrEmpty(iconPath)) return;

            using (var leadingIcon = new BeepImage
            {
                ImagePath = iconPath,
                Size = iconRect.Size,
                ApplyThemeOnImage = _textField.ApplyThemeToImage,
                Theme = _textField.Theme,
                IsChild = true
            })
            {
                leadingIcon.Draw(g, iconRect);
            }
        }

        /// <summary>
        /// Draw trailing icon or clear button - Enhanced for dual icon support
        /// </summary>
        private void DrawTrailingIcon(Graphics g, Rectangle drawingRect)
        {
            // Determine which icon to show
            string iconPath = "";
            if (_textField.ShowClearButton && _textField.HasContent)
            {
                iconPath = Svgs.Close;
            }
            else if (!string.IsNullOrEmpty(_textField.TrailingIconPath))
            {
                iconPath = _textField.TrailingIconPath;
            }
            else if (!string.IsNullOrEmpty(_textField.TrailingImagePath))
            {
                iconPath = _textField.TrailingImagePath;
            }

            if (string.IsNullOrEmpty(iconPath)) return;

            var iconRect = GetTrailingIconRectangle();
            if (iconRect.IsEmpty) return;

            // Adjust icon rect to be relative to drawingRect
            iconRect.Offset(drawingRect.X, drawingRect.Y);
            
            using (var trailingIcon = new BeepImage
            {
                ImagePath = iconPath,
                Size = iconRect.Size,
                ApplyThemeOnImage = _textField.ApplyThemeToImage,
                Theme = _textField.Theme,
                IsChild = true
            })
            {
                trailingIcon.Draw(g, iconRect);
            }
        }

        /// <summary>
        /// Draw text selection centered vertically inside actual text rect
        /// </summary>
        private void DrawSelection(Graphics g, Rectangle drawingRect)
        {
            if (_textField.SelectionLength <= 0) return;

            var textRect = OffsetRect(_materialHelper.GetAdjustedTextRectangle(), drawingRect);
            using (var selectionBrush = new SolidBrush(_textField.SelectionBackColor))
            {
                int measuredHeight = TextRenderer.MeasureText(g, "Ag", _textField.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                int y = textRect.Y + Math.Max(0, (textRect.Height - measuredHeight) / 2);
                var selectionRect = new Rectangle(textRect.X, y, Math.Max(1, textRect.Width / 2), measuredHeight);
                g.FillRectangle(selectionBrush, selectionRect);
            }
        }

        /// <summary>
        /// Draw text caret vertically centered in the actual text rect
        /// </summary>
        private void DrawCaret(Graphics g, Rectangle drawingRect)
        {
            var textRect = OffsetRect(_materialHelper.GetAdjustedTextRectangle(), drawingRect);

            string display = _textField.DisplayText ?? string.Empty;
            Size fullSize = TextRenderer.MeasureText(g, display, _textField.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);

            int baseX = textRect.X;
            if (_textField.TextAlignment == HorizontalAlignment.Center && fullSize.Width < textRect.Width)
            {
                baseX = textRect.X + (textRect.Width - fullSize.Width) / 2;
            }
            else if (_textField.TextAlignment == HorizontalAlignment.Right)
            {
                baseX = textRect.Right - fullSize.Width;
            }

            int caretX = Math.Max(textRect.X, Math.Min(textRect.Right, baseX + fullSize.Width));

            int measuredHeight = TextRenderer.MeasureText(g, "Ag", _textField.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
            int y = textRect.Y + Math.Max(0, (textRect.Height - measuredHeight) / 2);

            using (var caretPen = new Pen(_textField.PrimaryColor, 1))
            {
                g.DrawLine(caretPen, caretX, y, caretX, y + measuredHeight);
            }
        }

        /// <summary>
        /// Draw line numbers for multiline text
        /// </summary>
        private void DrawLineNumbers(Graphics g, Rectangle drawingRect)
        {
            if (!_textField.ShowLineNumbers || !_textField.Multiline) return;

            var lineNumberRect = GetLineNumberRectangle(drawingRect);
            using (var bgBrush = new SolidBrush(_textField.LineNumberBackColor))
            {
                g.FillRectangle(bgBrush, lineNumberRect);
            }

            var lines = _materialHelper.GetLines();
            using (var numberBrush = new SolidBrush(_textField.LineNumberForeColor))
            using (var numberFont = _textField.LineNumberFont ?? _textField.Font)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string lineNumber = (i + 1).ToString();
                    var lineRect = new Rectangle(
                        lineNumberRect.X + 4,
                        lineNumberRect.Y + i * _textField.Font.Height,
                        lineNumberRect.Width - 8,
                        _textField.Font.Height);

                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Far,
                        LineAlignment = StringAlignment.Center
                    };

                    g.DrawString(lineNumber, numberFont, numberBrush, lineRect, format);
                }
            }
        }

        /// <summary>
        /// Draw prefix and suffix text
        /// </summary>
        private void DrawPrefixSuffixText(Graphics g, Rectangle drawingRect)
        {
            if (!string.IsNullOrEmpty(_textField.PrefixText))
            {
                var prefixRect = OffsetRect(_materialHelper.GetPrefixTextRectangle(), drawingRect);
                TextRenderer.DrawText(g, _textField.PrefixText, _textField.Font, prefixRect, _textField.ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }

            if (!string.IsNullOrEmpty(_textField.SuffixText))
            {
                var suffixRect = OffsetRect(_materialHelper.GetSuffixTextRectangle(), drawingRect);
                TextRenderer.DrawText(g, _textField.SuffixText, _textField.Font, suffixRect, _textField.ForeColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
        }

        /// <summary>
        /// Draw character counter
        /// </summary>
        private void DrawCharacterCounter(Graphics g, Rectangle drawingRect)
        {
            if (!_textField.ShowCharacterCounter) return;
            string counterText = _textField.GetCharacterCountText();
            if (string.IsNullOrEmpty(counterText)) return;

            var counterRect = OffsetRect(_materialHelper.GetCharacterCounterRectangle(), drawingRect);
            Color counterColor = _textField.IsCharacterLimitExceeded ? _textField.ErrorColor : _textField.HelperTextColor;
            using (var counterFont = new Font(_textField.Font.FontFamily, Math.Max(8f, _textField.Font.Size - 1f)))
            {
                TextRenderer.DrawText(g, counterText, counterFont, counterRect, counterColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
        }

        #endregion

        #region Helper Methods

        private void InitializeBrushesAndPens()
        {
            // Initialize cached drawing objects for performance
            UpdateDrawingObjects();
        }

        private void UpdateDrawingObjects()
        {
            _textBrush?.Dispose();
            _placeholderBrush?.Dispose();
            _labelBrush?.Dispose();
            _helperTextBrush?.Dispose();
            _selectionBrush?.Dispose();
            _borderPen?.Dispose();
            _focusBorderPen?.Dispose();
            _caretPen?.Dispose();

            _textBrush = new SolidBrush(_textField.ForeColor);
            _placeholderBrush = new SolidBrush(_textField.PlaceholderTextColor);
            _labelBrush = new SolidBrush(_textField.LabelColor);
            _helperTextBrush = new SolidBrush(_textField.HelperTextColor);
            _selectionBrush = new SolidBrush(_textField.SelectionBackColor);
            _borderPen = new Pen(_textField.OutlineColor);
            _focusBorderPen = new Pen(_textField.PrimaryColor, 2);
            _caretPen = new Pen(_textField.PrimaryColor);
        }

        private Color GetEffectiveBorderColor()
        {
            if (_textField.HasError)
                return _textField.ErrorColor;
            else if (_textField.IsFocused)
                return _textField.PrimaryColor;
            else
                return _textField.OutlineColor;
        }

        private Color GetEffectiveLabelColor()
        {
            if (_textField.HasError)
                return _textField.ErrorColor;
            else if (_textField.IsFocused)
                return _textField.PrimaryColor;
            else
                return _textField.LabelColor;
        }

        private Rectangle GetInputRectangle()
        {
            var inputRect = _materialHelper?.GetInputRectangle() ?? Rectangle.Empty;
            if (inputRect.IsEmpty)
            {
                var drawingRect = _textField.DrawingRect;
                return new Rectangle(drawingRect.X, drawingRect.Y, drawingRect.Width, Math.Max(32, drawingRect.Height));
            }
            return inputRect;
        }

        private Rectangle GetLeadingIconRectangle()
        {
            return _materialHelper?.GetLeadingIconRectangle() ?? Rectangle.Empty;
        }

        private Rectangle GetTrailingIconRectangle()
        {
            return _materialHelper?.GetTrailingIconRectangle() ?? Rectangle.Empty;
        }

        private Rectangle GetLineNumberRectangle(Rectangle drawingRect)
        {
            return new Rectangle(drawingRect.X, drawingRect.Y, _textField.LineNumberMarginWidth, drawingRect.Height);
        }

        private PointF GetAnimatedLabelPosition(Rectangle drawingRect)
        {
            var labelRect = _materialHelper.GetLabelRectangle();
            var textRect = _materialHelper.GetTextRectangle();
            
            if (_materialHelper.LabelAnimationProgress > 0)
            {
                // Floating position
                return new PointF(
                    drawingRect.X + labelRect.X,
                    drawingRect.Y + (labelRect.Y * _materialHelper.LabelAnimationProgress + textRect.Y * (1f - _materialHelper.LabelAnimationProgress)));
            }
            else
            {
                // Resting position
                return new PointF(drawingRect.X + textRect.X, drawingRect.Y + textRect.Y + 4);
            }
        }

        private TextFormatFlags GetTextFormatFlags()
        {
            var flags = TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.NoPadding | TextFormatFlags.NoClipping;

            switch (_textField.TextAlignment)
            {
                case HorizontalAlignment.Left: flags |= TextFormatFlags.Left; break;
                case HorizontalAlignment.Center: flags |= TextFormatFlags.HorizontalCenter; break;
                case HorizontalAlignment.Right: flags |= TextFormatFlags.Right; break;
            }

            if (_textField.Multiline)
            {
                flags |= TextFormatFlags.WordBreak;
                flags &= ~TextFormatFlags.SingleLine;
                flags |= TextFormatFlags.Top;
            }
            else
            {
                flags |= TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            }

            if (_textField.RightToLeft == RightToLeft.Yes)
            {
                flags |= TextFormatFlags.RightToLeft;
            }

            return flags;
        }

        private GraphicsPath CreateRoundedRectanglePath(RectangleF rect, float radius)
        {
            return CreateRoundedRectanglePath(rect, radius, radius, radius, radius);
        }

        private GraphicsPath CreateRoundedRectanglePath(RectangleF rect, float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            var path = new GraphicsPath();
            
            // Ensure radius doesn't exceed rectangle dimensions
            float maxRadius = Math.Min(rect.Width, rect.Height) / 2f;
            topLeft = Math.Min(topLeft, maxRadius);
            topRight = Math.Min(topRight, maxRadius);
            bottomRight = Math.Min(bottomRight, maxRadius);
            bottomLeft = Math.Min(bottomLeft, maxRadius);

            if (topLeft > 0)
                path.AddArc(rect.X, rect.Y, topLeft * 2, topLeft * 2, 180, 90);
            else
                path.AddLine(rect.X, rect.Y, rect.X, rect.Y);

            if (topRight > 0)
                path.AddArc(rect.Right - topRight * 2, rect.Y, topRight * 2, topRight * 2, 270, 90);
            else
                path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);

            if (bottomRight > 0)
                path.AddArc(rect.Right - bottomRight * 2, rect.Bottom - bottomRight * 2, bottomRight * 2, bottomRight * 2, 0, 90);
            else
                path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);

            if (bottomLeft > 0)
                path.AddArc(rect.X, rect.Bottom - bottomLeft * 2, bottomLeft * 2, bottomLeft * 2, 90, 90);
            else
                path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);

            path.CloseFigure();
            return path;
        }

        private Rectangle OffsetRect(Rectangle rect, Rectangle drawingRect)
        {
            rect.Offset(drawingRect.X, drawingRect.Y);
            return rect;
        }

        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            _textBrush?.Dispose();
            _placeholderBrush?.Dispose();
            _labelBrush?.Dispose();
            _helperTextBrush?.Dispose();
            _selectionBrush?.Dispose();
            _borderPen?.Dispose();
            _focusBorderPen?.Dispose();
            _caretPen?.Dispose();
        }
        #endregion
    }
}