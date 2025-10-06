using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
 

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Helpers
{
    /// <summary>
    /// Material Design-specific helper for BeepMaterialTextField
    /// Handles animations, layout, and Material Design rendering
    /// </summary>
    public class BeepMaterialTextFieldHelper : IDisposable
    {
        #region Private Fields
        private readonly BeepMaterialTextField _textField;
        private readonly Timer _rippleTimer;

        // Layout rectangles
        private Rectangle _inputRect;
        private Rectangle _textRect;          // adjusted input text rect (excludes prefix/suffix widths)
        private Rectangle _labelRect;
        private Rectangle _helperTextRect;
        private Rectangle _leadingIconRect;
        private Rectangle _trailingIconRect;
        private Rectangle _prefixTextRect;    // inside-text prefix area
        private Rectangle _suffixTextRect;    // inside-text suffix area

        // Animation values
        private float _labelAnimationProgress = 0f;
        private float _focusAnimationProgress = 0f;
        private bool _labelShouldFloat = false;
        private bool _focusShouldShow = false;

        // Ripple effect
        private PointF _rippleCenter = PointF.Empty;
        private float _rippleRadius = 0f;
        private float _rippleOpacity = 0f;
        private bool _rippleActive = false;

        // Text handling
        private int _caretPosition = 0;
        private int _selectionStart = 0;
        private int _selectionLength = 0;
        private readonly List<string> _lines = new List<string>();

        // Constants
        private const float FLOATING_LABEL_SCALE = 0.75f;
        private const int ICON_SIZE = 24;
        private const int PADDING = 16;
        private const int ICON_PADDING = 12;
        #endregion

        #region Constructor
        public BeepMaterialTextFieldHelper(BeepMaterialTextField textField)
        {
            _textField = textField ?? throw new ArgumentNullException(nameof(textField));

            _rippleTimer = new Timer { Interval = 16 }; // ~60 FPS
            _rippleTimer.Tick += RippleTimer_Tick;

            UpdateLayout();
            UpdateLines();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Compute all layout rectangles (icons, text, prefix/suffix, label, helper)
        /// </summary>
        public void UpdateLayout()
        {
            var drawingRect = _textField.DrawingRect;
            if (drawingRect.Width < 50 || drawingRect.Height < 24) return;

            // Font metrics
            int baseHeight;
            using (var g = _textField.CreateGraphics())
            {
                baseHeight = TextRenderer.MeasureText(g, "Ag", _textField.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
            }
            int spacing = Math.Max(4, baseHeight / 4);

            int helperTextHeight;
            using (var g = _textField.CreateGraphics())
            using (var helperFont = new Font(_textField.Font.FontFamily, Math.Max(8f, _textField.Font.Size - 1f)))
            {
                helperTextHeight = TextRenderer.MeasureText(g, "Ag", helperFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
            }

            // Input container area (exclude helper area)
            int inputHeight = drawingRect.Height;
            bool hasHelper = !string.IsNullOrEmpty(_textField.EffectiveHelperText);
            bool needReserve = _textField.ReserveSupportingSpace == SupportingSpaceMode.Always
                               || (_textField.ReserveSupportingSpace == SupportingSpaceMode.WhenHasTextOrCounter
                                   && (hasHelper || _textField.ShowCharacterCounter))
                               || (_textField.ReserveSupportingSpace == SupportingSpaceMode.Off && hasHelper);
            if (needReserve)
            {
                inputHeight = Math.Max(24, inputHeight - (helperTextHeight + spacing));
            }
            _inputRect = new Rectangle(0, 0, drawingRect.Width, inputHeight);

            // Determine available text zone between icons
            int zoneLeft = PADDING;
            int zoneRight = drawingRect.Width - PADDING;

            // Leading icon
            _leadingIconRect = Rectangle.Empty;
            if (!string.IsNullOrEmpty(_textField.LeadingIconPath) || !string.IsNullOrEmpty(_textField.LeadingImagePath))
            {
                int iconSize = Math.Min(_textField.IconSize > 0 ? _textField.IconSize : ICON_SIZE,
                                        Math.Max(16, Math.Min(_inputRect.Height - 8, _inputRect.Width / 10)));
                _leadingIconRect = new Rectangle(zoneLeft, Math.Max(0, (_inputRect.Height - iconSize) / 2), iconSize, iconSize);
                zoneLeft += iconSize + (_textField.IconPadding > 0 ? _textField.IconPadding : ICON_PADDING);
            }

            // Trailing icon / clear
            _trailingIconRect = Rectangle.Empty;
            bool hasTrailing = !string.IsNullOrEmpty(_textField.TrailingIconPath) ||
                               !string.IsNullOrEmpty(_textField.TrailingImagePath) ||
                               (_textField.ShowClearButton && _textField.HasContent);
            if (hasTrailing)
            {
                int iconSize = Math.Min(_textField.IconSize > 0 ? _textField.IconSize : ICON_SIZE,
                                        Math.Max(16, Math.Min(_inputRect.Height - 8, _inputRect.Width / 10)));
                _trailingIconRect = new Rectangle(Math.Max(zoneLeft, zoneRight - iconSize), Math.Max(0, (_inputRect.Height - iconSize) / 2), iconSize, iconSize);
                zoneRight = _trailingIconRect.Left - (_textField.IconPadding > 0 ? _textField.IconPadding : ICON_PADDING);
            }

            // Base text rect (tall so TextRenderer can center vertically)
            int textTop = 2;
            int textHeight = Math.Max(10, _inputRect.Height - 4);
            _textRect = new Rectangle(zoneLeft, textTop, Math.Max(10, zoneRight - zoneLeft), textHeight);

            // Prefix/Suffix inside the text zone
            _prefixTextRect = Rectangle.Empty;
            _suffixTextRect = Rectangle.Empty;
            using (var g = _textField.CreateGraphics())
            {
                if (!string.IsNullOrEmpty(_textField.PrefixText))
                {
                    var sz = TextRenderer.MeasureText(g, _textField.PrefixText, _textField.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                    _prefixTextRect = new Rectangle(_textRect.X, _textRect.Y, sz.Width, _textRect.Height);
                    _textRect = new Rectangle(_textRect.X + sz.Width + 4, _textRect.Y, Math.Max(10, _textRect.Width - sz.Width - 4), _textRect.Height);
                }
                if (!string.IsNullOrEmpty(_textField.SuffixText))
                {
                    var sz = TextRenderer.MeasureText(g, _textField.SuffixText, _textField.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                    int sx = Math.Max(_textRect.X, zoneRight - sz.Width);
                    _suffixTextRect = new Rectangle(sx, _textRect.Y, sz.Width, _textRect.Height);
                    int reduce = Math.Max(0, _textRect.Right - (_suffixTextRect.Left - 4));
                    if (reduce > 0)
                    {
                        _textRect = new Rectangle(_textRect.X, _textRect.Y, Math.Max(10, _textRect.Width - reduce), _textRect.Height);
                    }
                }
            }

            // Label rect
            int labelHeight = Math.Max(12, (int)Math.Ceiling(baseHeight * FLOATING_LABEL_SCALE));
            if (_textField.SearchBoxStyle || _textField.CurvedBorderRadius > 10)
            {
                _labelRect = new Rectangle(_textRect.X, Math.Max(0, (_inputRect.Height - labelHeight) / 2), Math.Max(10, _textRect.Width), labelHeight);
            }
            else
            {
                int labelY = _textField.Variant == MaterialTextFieldVariant.Outlined ? -labelHeight / 2 : spacing;
                _labelRect = new Rectangle(_textRect.X, labelY, Math.Max(10, _textRect.Width), labelHeight);
            }

            // Helper text rect
            if (needReserve)
            {
                // +2 to ensure it is visually below the bottom border line even when border is 2px
                int belowBorderOffset = spacing + 2;
                _helperTextRect = new Rectangle(
                    PADDING,
                    _inputRect.Bottom + belowBorderOffset,
                    Math.Max(10, drawingRect.Width - PADDING * 2),
                    helperTextHeight);
            }
            else
            {
                _helperTextRect = Rectangle.Empty;
            }
        }

        public void StartLabelAnimation(bool shouldFloat)
        {
            if (_labelShouldFloat == shouldFloat) return;
            _labelShouldFloat = shouldFloat;
            if (_textField.EnableAnimations && _textField.AnimationTimer != null) _textField.AnimationTimer.Start();
            else { _labelAnimationProgress = shouldFloat ? 1f : 0f; _textField.Invalidate(); }
        }

        public void StartFocusAnimation(bool isFocused)
        {
            if (_focusShouldShow == isFocused) return;
            _focusShouldShow = isFocused;
            if (_textField.EnableAnimations && _textField.AnimationTimer != null) _textField.AnimationTimer.Start();
            else { _focusAnimationProgress = isFocused ? 1f : 0f; _textField.Invalidate(); }
        }

        public void StartRippleAnimation(Point clickPosition)
        {
            if (!_textField.EnableRippleEffect) return;
            _rippleCenter = clickPosition; _rippleRadius = 0f; _rippleOpacity = 0.3f; _rippleActive = true; _rippleTimer.Start();
        }

        public void UpdateAnimations()
        {
            bool needsUpdate = false;
            float labelSpeed = (1000f / Math.Max(1, _textField.LabelAnimationDuration)) / 60f;
            float focusSpeed = (1000f / Math.Max(1, _textField.FocusAnimationDuration)) / 60f;

            float targetLabel = _labelShouldFloat ? 1f : 0f;
            if (Math.Abs(_labelAnimationProgress - targetLabel) > 0.01f)
            {
                _labelAnimationProgress += Math.Sign(targetLabel - _labelAnimationProgress) * labelSpeed;
                _labelAnimationProgress = Math.Clamp(_labelAnimationProgress, 0f, 1f);
                needsUpdate = true;
            }

            float targetFocus = _focusShouldShow ? 1f : 0f;
            if (Math.Abs(_focusAnimationProgress - targetFocus) > 0.01f)
            {
                _focusAnimationProgress += Math.Sign(targetFocus - _focusAnimationProgress) * focusSpeed;
                _focusAnimationProgress = Math.Clamp(_focusAnimationProgress, 0f, 1f);
                needsUpdate = true;
            }

            if (needsUpdate) _textField.Invalidate(); else if (!_rippleActive) _textField.AnimationTimer?.Stop();
        }

        public void ApplyVariant(MaterialTextFieldVariant variant)
        {
            UpdateLayout();
            _textField.Invalidate();
        }

        public Rectangle GetCharacterCounterRectangle()
        {
            if (!_textField.ShowCharacterCounter) return Rectangle.Empty;
            var drawingRect = _textField.DrawingRect;
            int counterHeight;
            using (var g = _textField.CreateGraphics())
            {
                counterHeight = TextRenderer.MeasureText(g, "Ag", _textField.TextFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
            }
            int padding = Math.Max(2, (int)Math.Round(_textField.TextFont.Size / 3));
            return new Rectangle(drawingRect.Right - 60, drawingRect.Bottom - counterHeight - padding, 60, counterHeight);
        }

        public Rectangle GetPrefixTextRectangle() => _prefixTextRect;
        public Rectangle GetSuffixTextRectangle() => _suffixTextRect;
        public Rectangle GetAdjustedTextRectangle() => _textRect;

        public float LabelAnimationProgress => _labelAnimationProgress;
        public float FocusAnimationProgress => _focusAnimationProgress;

        public void UpdateIcons()
        {
            bool showClear = _textField.ShowClearButton && _textField.HasContent;
            if (_textField.BeepImage != null && !string.IsNullOrEmpty(_textField.LeadingIconPath))
            {
                _textField.BeepImage.ImagePath = _textField.LeadingIconPath;
                _textField.BeepImage.Visible = true;
            }
            else if (_textField.BeepImage != null)
            {
                _textField.BeepImage.Visible = false;
            }
            UpdateLayout();
            _textField.Invalidate();
        }

        public void HandleMouseClick(MouseEventArgs e)
        {
            StartRippleAnimation(e.Location);
            var drawingRect = _textField.DrawingRect;
            var local = new Point(e.X - drawingRect.X, e.Y - drawingRect.Y);

            // Leading icon click
            if (_textField.LeadingIconClickable && (!string.IsNullOrEmpty(_textField.LeadingIconPath) || !string.IsNullOrEmpty(_textField.LeadingImagePath)) && _leadingIconRect.Contains(local))
            { _textField.RaiseLeadingIconClicked(); return; }

            // Trailing icon / clear
            bool hasTrailingAction = !string.IsNullOrEmpty(_textField.TrailingIconPath) || !string.IsNullOrEmpty(_textField.TrailingImagePath) || (_textField.ShowClearButton && _textField.HasContent);
            if (_textField.TrailingIconClickable && hasTrailingAction && _trailingIconRect.Contains(local))
            {
                if (_textField.ShowClearButton && _textField.HasContent)
                { _textField.Text = string.Empty; _textField.RaiseClearButtonClicked(); }
                else { _textField.RaiseTrailingIconClicked(); }
                return;
            }

            // Caret positioning (simple: end of text)
            if (_textRect.Contains(local)) PositionCaretFromPoint(local);
        }

        public void HandleKeyDown(KeyEventArgs e)
        {
            // Placeholder for extended logic; keep layout fresh
            UpdateLayout();
            _textField.Invalidate();
        }

        public void InsertText(string text)
        {
            if (!string.IsNullOrEmpty(text)) _textField.Text += text;
        }

        public string GetSelectedText() => string.Empty; // Placeholder for full selection support
        public void RemoveSelectedText() { UpdateLayout(); _textField.Invalidate(); }
        public void SelectAll() { UpdateLayout(); _textField.Invalidate(); }
        public void Undo() { UpdateLayout(); _textField.Invalidate(); }
        public void Redo() { UpdateLayout(); _textField.Invalidate(); }

        public void ApplyTheme(IBeepTheme theme)
        {
            if (theme == null) return;
            _textField.PrimaryColor = theme.PrimaryColor;
            _textField.ErrorColor = theme.ErrorColor;
            _textField.SurfaceColor = theme.SurfaceColor;
            _textField.OutlineColor = theme.BorderColor;
            _textField.BackColor = theme.SurfaceColor;
            _textField.ForeColor = theme.PrimaryTextColor;
            _textField.Invalidate();
        }
        #endregion

        #region Selection/Caret Helpers
        public int GetSelectionStart() => _selectionStart;
        public void SetSelectionStart(int value) { _selectionStart = Math.Max(0, Math.Min(value, _textField.Text.Length)); _caretPosition = _selectionStart; _textField.Invalidate(); }
        public int GetSelectionLength() => _selectionLength;
        public void SetSelectionLength(int value) { _selectionLength = Math.Max(0, Math.Min(value, _textField.Text.Length - _selectionStart)); _textField.Invalidate(); }
        #endregion

        #region Layout Getters
        public Rectangle GetTextRectangle() => _textRect;
        public Rectangle GetLabelRectangle() => _labelRect;
        public Rectangle GetHelperTextRectangle() => _helperTextRect;
        public Rectangle GetInputRectangle() => _inputRect;
        public Rectangle GetLeadingIconRectangle() => _leadingIconRect;
        public Rectangle GetTrailingIconRectangle() => _trailingIconRect;
        public void ScrollToCaret() { }
        public void EnsureCaretVisible() { ScrollToCaret(); }
        public int GetCurrentLineNumber() => 1;
        public List<string> GetLines() => _lines.ToList();
        #endregion

        #region Private Helpers
        private void UpdateLines()
        {
            _lines.Clear();
            if (string.IsNullOrEmpty(_textField.Text)) { _lines.Add(""); return; }
            if (_textField.Multiline)
            {
                string[] split = _textField.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                _lines.AddRange(split);
            }
            else
            {
                _lines.Add(_textField.Text.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " "));
            }
            if (_lines.Count == 0) _lines.Add("");
        }

        private void PositionCaretFromPoint(Point point)
        {
            _caretPosition = _textField.Text.Length;
            _selectionStart = _caretPosition;
            _selectionLength = 0;
            _textField.Invalidate();
        }

        private void RippleTimer_Tick(object sender, EventArgs e)
        {
            if (!_rippleActive) return;
            float maxRadius = Math.Max(_textField.Width, _textField.Height);
            _rippleRadius += maxRadius * 0.05f;
            _rippleOpacity = Math.Max(0f, 0.3f - _rippleRadius / maxRadius * 0.3f);
            if (_rippleRadius >= maxRadius || _rippleOpacity <= 0f)
            { _rippleActive = false; _rippleTimer.Stop(); _rippleRadius = 0f; _rippleOpacity = 0f; }
            _textField.Invalidate();
        }
        #endregion

        #region IDisposable
        public void Dispose() { _rippleTimer?.Dispose(); }
        #endregion
    }
}