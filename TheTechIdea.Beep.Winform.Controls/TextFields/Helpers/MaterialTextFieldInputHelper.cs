using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls.TextFields.Helpers
{
    /// <summary>
    /// Handles all keyboard and mouse input for BeepMaterialTextField
    /// Following the helper pattern used by BeepSimpleTextBox
    /// </summary>
    public class MaterialTextFieldInputHelper
    {
        private readonly BeepMaterialTextField _textField;
        private readonly BeepMaterialTextFieldHelper _materialHelper;

        public MaterialTextFieldInputHelper(BeepMaterialTextField textField, BeepMaterialTextFieldHelper materialHelper)
        {
            _textField = textField ?? throw new ArgumentNullException(nameof(textField));
            _materialHelper = materialHelper ?? throw new ArgumentNullException(nameof(materialHelper));
        }

        #region Keyboard Input Handling

        /// <summary>
        /// Determine if a key should be processed as input
        /// </summary>
        public bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.Back:
                case Keys.Delete:
                case Keys.Enter:
                    return true;
                case Keys.Tab:
                    return _textField.AcceptsTab;
                case Keys.Up:
                case Keys.Down:
                    return _textField.Multiline;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Handle key down events
        /// </summary>
        public void HandleKeyDown(KeyEventArgs e)
        {
            if (_textField.ReadOnly) return;
            
            // Handle Ctrl combinations
            if (e.Control)
            {
                HandleControlKeyDown(e);
                return;
            }
            
            // Handle navigation and text operations
            HandleNavigationKeys(e);
        }

        /// <summary>
        /// Handle Ctrl key combinations
        /// </summary>
        private void HandleControlKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    _textField.SelectAll();
                    e.Handled = true;
                    break;
                case Keys.C:
                    _textField.Copy();
                    e.Handled = true;
                    break;
                case Keys.V:
                    _textField.Paste();
                    e.Handled = true;
                    break;
                case Keys.X:
                    _textField.Cut();
                    e.Handled = true;
                    break;
                case Keys.Z:
                    _textField.Undo();
                    e.Handled = true;
                    break;
                case Keys.Y:
                    _textField.Redo();
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// Handle navigation keys
        /// </summary>
        private void HandleNavigationKeys(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    MoveCaret(-1, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Right:
                    MoveCaret(1, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Home:
                    MoveToLineStart(e.Control, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.End:
                    MoveToLineEnd(e.Control, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Back:
                    HandleBackspace();
                    e.Handled = true;
                    break;
                case Keys.Delete:
                    HandleDelete();
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    HandleEnter();
                    e.Handled = true;
                    break;
                case Keys.Tab:
                    if (_textField.AcceptsTab)
                    {
                        InsertText("\t");
                        e.Handled = true;
                    }
                    break;
                case Keys.Up:
                    if (_textField.Multiline)
                    {
                        MoveCaretVertical(-1, e.Shift);
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    if (_textField.Multiline)
                    {
                        MoveCaretVertical(1, e.Shift);
                        e.Handled = true;
                    }
                    break;
            }
        }

        /// <summary>
        /// Handle character input
        /// </summary>
        public void HandleKeyPress(KeyPressEventArgs e)
        {
            if (_textField.ReadOnly)
            {
                e.Handled = true;
                return;
            }
            
            if (char.IsControl(e.KeyChar))
            {
                return; // Already handled in OnKeyDown
            }
            
            InsertText(e.KeyChar.ToString());
            e.Handled = true;
        }

        #endregion

        #region Mouse Input Handling

        /// <summary>
        /// Handle mouse click for caret positioning
        /// </summary>
        public void HandleMouseClick(MouseEventArgs e)
        {
            if (!_textField.ReadOnly)
            {
                _textField.Focus();
                // Position caret based on click location
                PositionCaretFromMouse(e.Location);
            }
        }

        /// <summary>
        /// Position caret based on mouse click location
        /// </summary>
        private void PositionCaretFromMouse(Point location)
        {
            var caretHelper = GetCaretHelper();
            if (caretHelper != null)
            {
                // Get position from the caret helper
                int position = caretHelper.GetPositionFromPoint(location);
                caretHelper.CaretPosition = position;
                caretHelper.ClearSelection();
            }
            
            // Update layout and invalidate
            _materialHelper?.UpdateLayout();
            _textField.Invalidate();
        }

        #endregion

        #region Text Manipulation

        /// <summary>
        /// Insert text at current caret position
        /// </summary>
        public void InsertText(string text)
        {
            if (!string.IsNullOrEmpty(text) && !_textField.ReadOnly)
            {
                // Get current caret position from caret helper
                var caretHelper = GetCaretHelper();
                if (caretHelper != null)
                {
                    int caretPos = caretHelper.CaretPosition;
                    string currentText = _textField.Text ?? "";
                    
                    // Insert text at caret position
                    string newText = currentText.Insert(caretPos, text);
                    _textField.Text = newText;
                    
                    // Update caret position
                    caretHelper.CaretPosition = caretPos + text.Length;
                }
                else
                {
                    // Fallback: append text
                    _textField.Text += text;
                }
            }
        }

        /// <summary>
        /// Handle backspace key
        /// </summary>
        private void HandleBackspace()
        {
            var caretHelper = GetCaretHelper();
            if (caretHelper?.HasSelection == true)
            {
                // Remove selected text
                caretHelper.RemoveSelectedText();
            }
            else if (caretHelper != null && caretHelper.CaretPosition > 0)
            {
                // Remove character before caret
                string currentText = _textField.Text ?? "";
                int caretPos = caretHelper.CaretPosition;
                
                if (caretPos > 0 && caretPos <= currentText.Length)
                {
                    string newText = currentText.Remove(caretPos - 1, 1);
                    _textField.Text = newText;
                    caretHelper.CaretPosition = caretPos - 1;
                }
            }
        }

        /// <summary>
        /// Handle delete key
        /// </summary>
        private void HandleDelete()
        {
            var caretHelper = GetCaretHelper();
            if (caretHelper?.HasSelection == true)
            {
                // Remove selected text
                caretHelper.RemoveSelectedText();
            }
            else if (caretHelper != null)
            {
                // Remove character after caret
                string currentText = _textField.Text ?? "";
                int caretPos = caretHelper.CaretPosition;
                
                if (caretPos < currentText.Length)
                {
                    string newText = currentText.Remove(caretPos, 1);
                    _textField.Text = newText;
                    // Caret position stays the same
                }
            }
        }

        /// <summary>
        /// Handle enter key
        /// </summary>
        private void HandleEnter()
        {
            if (_textField.Multiline && _textField.AcceptsReturn)
            {
                InsertText(Environment.NewLine);
            }
        }

        /// <summary>
        /// Get caret helper from the text field
        /// </summary>
        private MaterialTextFieldCaretHelper GetCaretHelper()
        {
            // Access the caret helper through reflection or a public property
            // For now, we'll use a simple approach
            return _textField.GetType()
                .GetField("_caretHelper", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                .GetValue(_textField) as MaterialTextFieldCaretHelper;
        }

        #endregion

        #region Caret Movement

        /// <summary>
        /// Move caret horizontally
        /// </summary>
        private void MoveCaret(int direction, bool extend)
        {
            var caretHelper = GetCaretHelper();
            if (caretHelper != null)
            {
                caretHelper.MoveCaret(direction, extend);
            }
        }

        /// <summary>
        /// Move caret vertically
        /// </summary>
        private void MoveCaretVertical(int direction, bool extend)
        {
            var caretHelper = GetCaretHelper();
            if (caretHelper != null)
            {
                caretHelper.MoveCaretVertical(direction, extend);
            }
        }

        /// <summary>
        /// Move to start of line
        /// </summary>
        private void MoveToLineStart(bool document, bool extend)
        {
            var caretHelper = GetCaretHelper();
            if (caretHelper != null)
            {
                if (document)
                {
                    // Move to start of document
                    if (extend)
                    {
                        caretHelper.SetSelection(0, caretHelper.CaretPosition);
                    }
                    else
                    {
                        caretHelper.CaretPosition = 0;
                        caretHelper.ClearSelection();
                    }
                }
                else
                {
                    // Move to start of current line (basic implementation)
                    // For now, just move to start of document
                    MoveToLineStart(true, extend);
                }
            }
        }

        /// <summary>
        /// Move to end of line
        /// </summary>
        private void MoveToLineEnd(bool document, bool extend)
        {
            var caretHelper = GetCaretHelper();
            if (caretHelper != null)
            {
                int endPosition = document ? _textField.Text.Length : _textField.Text.Length; // Basic implementation
                
                if (extend)
                {
                    caretHelper.SetSelection(caretHelper.CaretPosition, endPosition - caretHelper.CaretPosition);
                }
                else
                {
                    caretHelper.CaretPosition = endPosition;
                    caretHelper.ClearSelection();
                }
            }
        }

        #endregion
    }
}