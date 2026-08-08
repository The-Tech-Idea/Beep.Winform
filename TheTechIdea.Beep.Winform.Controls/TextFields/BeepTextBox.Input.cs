using System;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Keyboard handling for BeepTextBox
    /// </summary>
    public partial class BeepTextBox
    {
        #region "Key Handling"
        
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.Back:
                case Keys.Delete:
                    return true;
                case Keys.Enter:
                    RaiseSubmitChanges();
                    return _multiline && _acceptsReturn;
                case Keys.Tab:
                    RaiseSubmitChanges();
                    return _acceptsTab;
                case Keys.Up:
                case Keys.Down:
                    return _multiline;
                default:
                    return base.IsInputKey(keyData);
            }
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            // Handle search keyboard shortcuts first (works even in read-only mode)
            if (HandleSearchKeyDown(e))
            {
                return; // Search handled the key
            }
            
            if (_readOnly) return;
            
            // Start typing indicator
            if (!_isTyping && _enableTypingIndicator && _enableSmartFeatures)
            {
                _isTyping = true;
                _lastKeyPressTime = DateTime.Now;
                TypingStarted?.Invoke(this, EventArgs.Empty);
                Invalidate();
            }
            
            // Reset typing timer
            _typingTimer?.Stop();
            _typingTimer?.Start();
            
            // Handle keyboard shortcuts
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Z:
                        if (e.Shift)
                            _helper?.UndoRedo?.Redo();
                        else
                            _helper?.UndoRedo?.Undo();
                        e.Handled = true;
                        return;
                    case Keys.Y:
                        _helper?.UndoRedo?.Redo();
                        e.Handled = true;
                        return;
                    case Keys.A:
                        SelectAll();
                        e.Handled = true;
                        return;
                    case Keys.C:
                        Copy();
                        e.Handled = true;
                        return;
                    case Keys.V:
                        Paste();
                        e.Handled = true;
                        return;
                    case Keys.X:
                        Cut();
                        e.Handled = true;
                        return;
                }
            }
            
            // Handle navigation
            switch (e.KeyCode)
            {
                case Keys.Left:
                    _helper?.Caret?.MoveCaret(-1, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Right:
                    _helper?.Caret?.MoveCaret(1, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Up:
                    if (_multiline)
                    {
                        _helper?.Caret?.MoveCaretVertical(-1, e.Shift);
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    if (_multiline)
                    {
                        _helper?.Caret?.MoveCaretVertical(1, e.Shift);
                        e.Handled = true;
                    }
                    break;
                case Keys.Home:
                    HandleHomeKey(e.Control, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.End:
                    HandleEndKey(e.Control, e.Shift);
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
                    if (_multiline && _acceptsReturn)
                    {
                        _helper?.InsertText(Environment.NewLine);
                        UpdateLines();
                        ValidateCaretPosition();
                        ScrollToCaret();
                        Invalidate();
                    }
                    else
                    {
                        SearchTriggered?.Invoke(this, EventArgs.Empty);
                    }
                    e.Handled = true;
                    break;
                case Keys.Tab:
                    if (_acceptsTab)
                    {
                        _helper?.InsertText("\t");
                        UpdateLines();
                        ValidateCaretPosition();
                        ScrollToCaret();
                        Invalidate();
                        e.Handled = true;
                    }
                    break;
            }
        }
        
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            
            if (_readOnly)
            {
                e.Handled = true;
                return;
            }
            
            if (char.IsControl(e.KeyChar))
            {
                return;
            }
            
            if (_maxLength > 0 && _text.Length >= _maxLength && _helper?.Caret?.SelectionLength == 0)
            {
                e.Handled = true;
                return;
            }
            
            _helper?.InsertText(e.KeyChar.ToString());
            UpdateLines();
            ValidateCaretPosition();

            // Single-line needs this too - it is the horizontal caret-follow. The old guard
            // matched the era when single-line ScrollToCaret was an empty stub.
            ScrollToCaret();
            
            Invalidate();
            e.Handled = true;
        }
        
        #endregion
        
        #region "Key Handling Helpers"
        
        private void HandleHomeKey(bool control, bool shift)
        {
            if (_helper?.Caret == null) return;
            
            int newPosition;
            if (control)
            {
                newPosition = 0;
            }
            else if (_multiline)
            {
                var lines = GetLines();
                var currentLineInfo = GetLineInfoFromPosition(_helper.Caret.CaretPosition, lines);
                newPosition = GetPositionFromLineInfo(currentLineInfo.LineIndex, 0, lines);
            }
            else
            {
                newPosition = 0;
            }
            
            _helper.Caret.CaretPosition = newPosition;
            if (!shift)
            {
                _helper.Caret.ClearSelection();
            }
            else
            {
                UpdateSelection(newPosition);
            }
            ScrollToCaret();
        }
        
        private void HandleEndKey(bool control, bool shift)
        {
            if (_helper?.Caret == null) return;
            
            int newPosition;
            if (control)
            {
                newPosition = _text.Length;
            }
            else if (_multiline)
            {
                var lines = GetLines();
                var currentLineInfo = GetLineInfoFromPosition(_helper.Caret.CaretPosition, lines);
                newPosition = GetPositionFromLineInfo(currentLineInfo.LineIndex, lines[currentLineInfo.LineIndex].Length, lines);
            }
            else
            {
                newPosition = _text.Length;
            }
            
            _helper.Caret.CaretPosition = newPosition;
            if (!shift)
            {
                _helper.Caret.ClearSelection();
            }
            else
            {
                UpdateSelection(newPosition);
            }
            ScrollToCaret();
        }
        
        private void UpdateSelection(int newCaretPosition)
        {
            if (_helper?.Caret == null) return;
            
            if (_helper.Caret.SelectionLength == 0)
            {
                _helper.Caret.SelectionStart = _helper.Caret.CaretPosition;
            }
            
            int selStart = Math.Min(_helper.Caret.SelectionStart, newCaretPosition);
            int selEnd = Math.Max(_helper.Caret.SelectionStart, newCaretPosition);
            
            _helper.Caret.SelectionStart = selStart;
            _helper.Caret.SelectionLength = selEnd - selStart;
            _helper.Caret.CaretPosition = newCaretPosition;
        }
        
        private void HandleBackspace()
        {
            if (_helper?.Caret?.HasSelection == true)
            {
                string oldText = _text;
                int oldCaret = _helper.Caret.CaretPosition;
                _helper.Caret.RemoveSelectedText();
                _helper?.UndoRedo?.AddUndoAction("Delete", oldText, _text, oldCaret, _helper.Caret.CaretPosition);
            }
            else if (_helper?.Caret?.CaretPosition > 0)
            {
                _helper.DeleteText(_helper.Caret.CaretPosition - 1, 1);
            }
            
            UpdateLines();
            ValidateCaretPosition();
            Invalidate();
        }
        
        private void HandleDelete()
        {
            if (_helper?.Caret?.HasSelection == true)
            {
                string oldText = _text;
                int oldCaret = _helper.Caret.CaretPosition;
                _helper.Caret.RemoveSelectedText();
                _helper?.UndoRedo?.AddUndoAction("Delete", oldText, _text, oldCaret, _helper.Caret.CaretPosition);
            }
            else if (_helper?.Caret?.CaretPosition < _text.Length)
            {
                _helper.DeleteText(_helper.Caret.CaretPosition, 1);
            }
            
            UpdateLines();
            ValidateCaretPosition();
            Invalidate();
        }
        
        #endregion
        
        #region "Text Operations"
        
        public void Copy()
        {
            if (!string.IsNullOrEmpty(SelectedText))
            {
                try
                {
                    Clipboard.SetText(SelectedText);
                }
                catch (ExternalException ex)
                {
                    // Another process holds the clipboard - the user's copy did NOT happen.
                    BeepLog.Failure(this, "copy selection to clipboard", ex);
                }
            }
        }
        
        public void Cut()
        {
            if (!string.IsNullOrEmpty(SelectedText) && !_readOnly)
            {
                string oldText = _text;
                int oldCaret = _helper?.Caret?.CaretPosition ?? 0;
                Copy();
                _helper?.Caret?.RemoveSelectedText();
                _helper?.UndoRedo?.AddUndoAction("Cut", oldText, _text, oldCaret, _helper?.Caret?.CaretPosition ?? 0);
                UpdateLines();
                ValidateCaretPosition();
                Invalidate();
            }
        }
        
        public void Paste()
        {
            if (!_readOnly && Clipboard.ContainsText())
            {
                try
                {
                    string clipboardText = Clipboard.GetText();
                    
                    if (!_multiline && clipboardText.Contains('\n'))
                    {
                        clipboardText = clipboardText.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                    }
                    
                    if (_maxLength > 0)
                    {
                        int availableLength = _maxLength - _text.Length + (_helper?.Caret?.SelectionLength ?? 0);
                        if (clipboardText.Length > availableLength)
                        {
                            clipboardText = clipboardText.Substring(0, Math.Max(0, availableLength));
                        }
                    }
                    
                    _helper?.InsertText(clipboardText);
                    UpdateLines();
                    ValidateCaretPosition();
                    ScrollToCaret();
                    Invalidate();
                }
                catch (ExternalException ex)
                {
                    BeepLog.Failure(this, "paste from clipboard", ex);
                }
            }
        }
        
        public void SelectAll()
        {
            _helper?.Caret?.SelectAll();
            Invalidate();
        }
        
        public void Clear()
        {
            if (!_readOnly)
            {
                Text = "";
                if (_helper?.Caret != null)
                {
                    _helper.Caret.CaretPosition = 0;
                    _helper.Caret.SelectionStart = 0;
                    _helper.Caret.SelectionLength = 0;
                }
                UpdateLines();
                Invalidate();
            }
        }
        
        /// <summary>Current horizontal scroll of the single-line text, in pixels.</summary>
        internal int HorizontalScrollOffset => _helper?.Scrolling?.ScrollOffsetX ?? 0;

        /// <summary>Current vertical scroll of multiline text, in pixels.</summary>
        internal int VerticalScrollOffset => _helper?.Scrolling?.ScrollOffsetY ?? 0;

        /// <summary>The caret's actual position for painting - SelectionStart is the selection
        /// ANCHOR and goes stale after ClearSelection (the painted caret froze at it).</summary>
        internal int VisibleCaretPosition => _helper?.Caret?.CaretPosition ?? 0;

        public void ScrollToCaret()
        {
            var scrolling = _helper?.Scrolling;
            if (scrolling == null) return;

            if (_multiline)
            {
                int caretPos = _helper.Caret?.CaretPosition ?? 0;
                // Wrap-aware: the caret's VISUAL line from the cached layout; raw-line
                // fallback until the first paint builds one.
                int vline = _helper.Drawing?.GetCaretVisualLineFromCache(caretPos) ?? -1;
                if (vline >= 0) scrolling.ScrollLineIntoView(vline);
                else scrolling.ScrollToCaret(caretPos);
                return;
            }

            // Single-line horizontal caret-follow, alignment-independent: the offset is
            // "pixels hidden left of the view"; the drawing origin model anchors unfocused
            // overflow at the alignment's natural end.
            string text = _text ?? string.Empty;
            int caret = Math.Max(0, Math.Min(_helper.Caret?.CaretPosition ?? 0, text.Length));
            Font font = _textFont ?? Font;

            int caretX = caret == 0 ? 0 : (int)Math.Ceiling(TextUtils.MeasureText(text.Substring(0, caret), font).Width);
            int fullW = (int)Math.Ceiling(TextUtils.MeasureText(text, font).Width);
            int viewW = Math.Max(1, _textRect.Width - DpiScalingHelper.ScaleValue(6, this));

            int offset = scrolling.ScrollOffsetX;
            if (caretX - offset > viewW) offset = caretX - viewW;
            else if (caretX - offset < 0) offset = caretX;
            offset = Math.Max(0, Math.Min(offset, Math.Max(0, fullW - viewW)));

            if (offset != scrolling.ScrollOffsetX)
            {
                scrolling.ScrollOffsetX = offset;
                Invalidate();
            }
        }
        
        public void InsertText(string text)
        {
            if (!_readOnly && !string.IsNullOrEmpty(text))
            {
                if (!_multiline && text.Contains('\n'))
                {
                    text = text.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                }
                
                _helper?.InsertText(text);
                UpdateLines();
                ValidateCaretPosition();
                ScrollToCaret();
                Invalidate();
            }
        }
        
        public void AppendText(string text)
        {
            if (!_readOnly && !string.IsNullOrEmpty(text))
            {
                string newText = _text + text;
                
                if (_maxLength > 0 && newText.Length > _maxLength)
                {
                    newText = newText.Substring(0, _maxLength);
                }
                
                Text = newText;
                
                if (_helper?.Caret != null)
                {
                    _helper.Caret.CaretPosition = _text.Length;
                    _helper.Caret.SelectionStart = _text.Length;
                    _helper.Caret.SelectionLength = 0;
                }
                
                if (_multiline)
                {
                    ScrollToCaret();
                }
                
                Invalidate();
            }
        }
        
        #endregion
    }
}
