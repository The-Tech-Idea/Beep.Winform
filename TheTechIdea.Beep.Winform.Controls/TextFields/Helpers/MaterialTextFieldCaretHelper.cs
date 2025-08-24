using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Helpers
{
    /// <summary>
    /// Handles caret positioning, text selection, and cursor management for BeepMaterialTextField
    /// Following the helper pattern used by BeepSimpleTextBox
    /// </summary>
    public class MaterialTextFieldCaretHelper
    {
        private readonly BeepMaterialTextField _textField;
        private readonly BeepMaterialTextFieldHelper _materialHelper;
        
        private int _caretPosition = 0;
        private int _selectionStart = 0;
        private int _selectionLength = 0;
        private Timer _caretBlinkTimer;
        private bool _caretVisible = true;

        public MaterialTextFieldCaretHelper(BeepMaterialTextField textField, BeepMaterialTextFieldHelper materialHelper)
        {
            _textField = textField ?? throw new ArgumentNullException(nameof(textField));
            _materialHelper = materialHelper ?? throw new ArgumentNullException(nameof(materialHelper));
            
            InitializeCaretTimer();
        }

        #region Properties

        /// <summary>
        /// Current caret position in the text
        /// </summary>
        public int CaretPosition
        {
            get => _caretPosition;
            set
            {
                _caretPosition = Math.Max(0, Math.Min(value, _textField.Text.Length));
                _textField.Invalidate();
            }
        }

        /// <summary>
        /// Start position of text selection
        /// </summary>
        public int SelectionStart
        {
            get => _selectionStart;
            set
            {
                _selectionStart = Math.Max(0, Math.Min(value, _textField.Text.Length));
                _textField.Invalidate();
            }
        }

        /// <summary>
        /// Length of text selection
        /// </summary>
        public int SelectionLength
        {
            get => _selectionLength;
            set
            {
                _selectionLength = Math.Max(0, Math.Min(value, _textField.Text.Length - _selectionStart));
                _textField.Invalidate();
            }
        }

        /// <summary>
        /// Get the selected text
        /// </summary>
        public string SelectedText
        {
            get
            {
                if (_selectionLength <= 0 || string.IsNullOrEmpty(_textField.Text))
                    return string.Empty;
                
                int start = Math.Min(_selectionStart, _textField.Text.Length);
                int length = Math.Min(_selectionLength, _textField.Text.Length - start);
                
                if (length <= 0)
                    return string.Empty;
                
                return _textField.Text.Substring(start, length);
            }
        }

        /// <summary>
        /// Check if there is a text selection
        /// </summary>
        public bool HasSelection => _selectionLength > 0;

        /// <summary>
        /// Check if caret is currently visible
        /// </summary>
        public bool IsCaretVisible => _caretVisible;

        #endregion

        #region Caret Management

        /// <summary>
        /// Initialize the caret blink timer
        /// </summary>
        private void InitializeCaretTimer()
        {
            _caretBlinkTimer = new Timer { Interval = 500 }; // Standard blink rate
            _caretBlinkTimer.Tick += CaretBlinkTimer_Tick;
        }

        /// <summary>
        /// Start caret blinking
        /// </summary>
        public void StartCaretBlink()
        {
            if (_textField.IsFocused && !_textField.ReadOnly)
            {
                _caretVisible = true;
                _caretBlinkTimer.Start();
                _textField.Invalidate();
            }
        }

        /// <summary>
        /// Stop caret blinking
        /// </summary>
        public void StopCaretBlink()
        {
            _caretBlinkTimer.Stop();
            _caretVisible = false;
            _textField.Invalidate();
        }

        /// <summary>
        /// Handle caret blink timer tick
        /// </summary>
        private void CaretBlinkTimer_Tick(object sender, EventArgs e)
        {
            if (_textField.IsFocused && !_textField.ReadOnly)
            {
                _caretVisible = !_caretVisible;
                _textField.Invalidate();
            }
            else
            {
                StopCaretBlink();
            }
        }

        #endregion

        #region Text Selection

        /// <summary>
        /// Select all text
        /// </summary>
        public void SelectAll()
        {
            if (!string.IsNullOrEmpty(_textField.Text))
            {
                _selectionStart = 0;
                _selectionLength = _textField.Text.Length;
                _caretPosition = _textField.Text.Length;
                _textField.Invalidate();
            }
        }

        /// <summary>
        /// Clear text selection
        /// </summary>
        public void ClearSelection()
        {
            _selectionStart = _caretPosition;
            _selectionLength = 0;
            _textField.Invalidate();
        }

        /// <summary>
        /// Remove selected text
        /// </summary>
        public void RemoveSelectedText()
        {
            if (HasSelection && !_textField.ReadOnly)
            {
                string text = _textField.Text;
                string beforeSelection = text.Substring(0, _selectionStart);
                string afterSelection = text.Substring(_selectionStart + _selectionLength);
                
                _textField.Text = beforeSelection + afterSelection;
                _caretPosition = _selectionStart;
                ClearSelection();
            }
        }

        /// <summary>
        /// Set selection range
        /// </summary>
        public void SetSelection(int start, int length)
        {
            SelectionStart = start;
            SelectionLength = length;
            CaretPosition = start + length;
        }

        #endregion

        #region Caret Movement

        /// <summary>
        /// Move caret by specified offset
        /// </summary>
        public void MoveCaret(int offset, bool extendSelection = false)
        {
            int newPosition = _caretPosition + offset;
            newPosition = Math.Max(0, Math.Min(newPosition, _textField.Text.Length));
            
            if (extendSelection)
            {
                if (_selectionLength == 0)
                {
                    _selectionStart = _caretPosition;
                }
                
                if (newPosition >= _selectionStart)
                {
                    _selectionLength = newPosition - _selectionStart;
                }
                else
                {
                    _selectionLength = _selectionStart - newPosition;
                    _selectionStart = newPosition;
                }
            }
            else
            {
                ClearSelection();
            }
            
            _caretPosition = newPosition;
            EnsureCaretVisible();
            _textField.Invalidate();
        }

        /// <summary>
        /// Move caret vertically (for multiline)
        /// </summary>
        public void MoveCaretVertical(int lineOffset, bool extendSelection = false)
        {
            if (!_textField.Multiline) return;
            
            // Basic implementation - will be enhanced with proper line handling
            MoveCaret(lineOffset > 0 ? 10 : -10, extendSelection);
        }

        /// <summary>
        /// Ensure caret is visible by scrolling if necessary
        /// </summary>
        public void EnsureCaretVisible()
        {
            // Basic implementation - will be enhanced with actual scrolling
            _materialHelper?.UpdateLayout();
        }

        #endregion

        #region Position Calculation

        /// <summary>
        /// Get caret rectangle for drawing
        /// </summary>
        public Rectangle GetCaretRectangle()
        {
            // Basic implementation - will be enhanced with actual position calculation
            var textRect = _materialHelper?.GetTextRectangle() ?? _textField.ClientRectangle;
            
            // For now, just place at the end of visible text
            using (var g = _textField.CreateGraphics())
            {
                var textSize = g.MeasureString(_textField.Text, _textField.Font);
                return new Rectangle(
                    textRect.X + (int)textSize.Width,
                    textRect.Y,
                    1, // Caret width
                    Math.Min((int)textSize.Height, textRect.Height)
                );
            }
        }

        /// <summary>
        /// Get position from point (for mouse clicks)
        /// </summary>
        public int GetPositionFromPoint(Point point)
        {
            // Basic implementation - will be enhanced with actual position calculation
            // For now, just return end of text
            return _textField.Text.Length;
        }

        /// <summary>
        /// Get point from position (for drawing)
        /// </summary>
        public Point GetPointFromPosition(int position)
        {
            // Basic implementation - will be enhanced with actual point calculation
            var textRect = _materialHelper?.GetTextRectangle() ?? _textField.ClientRectangle;
            return new Point(textRect.X, textRect.Y);
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            _caretBlinkTimer?.Stop();
            _caretBlinkTimer?.Dispose();
        }

        #endregion
    }
}