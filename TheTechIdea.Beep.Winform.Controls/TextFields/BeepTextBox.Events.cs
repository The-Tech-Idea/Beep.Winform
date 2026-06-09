using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Event handlers for BeepTextBox
    /// </summary>
    public partial class BeepTextBox
    {
        #region "Timer Event Handlers"
        
        private void DelayedUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) return;
            _delayedUpdateTimer?.Stop();

            if (_needsTextUpdate)
            {
                UpdateLines();
                _helper?.InvalidateAllCaches();
                _needsTextUpdate = false;
            }

            if (_needsLayoutUpdate)
            {
                UpdateDrawingRect();
                _helper?.HandleResize();
                _needsLayoutUpdate = false;
            }

            Invalidate();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) return;
            if (_isFocusAnimating && _enableFocusAnimation)
            {
                if (Focused)
                {
                    _focusAnimationProgress = Math.Min(1.0f, _focusAnimationProgress + 0.1f);
                }
                else
                {
                    _focusAnimationProgress = Math.Max(0.0f, _focusAnimationProgress - 0.1f);
                }
                
                if (_focusAnimationProgress <= 0.0f || _focusAnimationProgress >= 1.0f)
                {
                    _isFocusAnimating = false;
                    _animationTimer?.Stop();
                }
                
                Invalidate();
            }
        }
        
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) return;
            if (IsDisposed) return;
            _typingTimer?.Stop();
            _isTyping = false;
            TypingStopped?.Invoke(this, EventArgs.Empty);

            if (_enableTypingIndicator)
            {
                Invalidate();
            }
        }
        
        #endregion
        
        #region "Event Overrides"
        
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _helper?.HandleFocusGained();
            
            if (_enableFocusAnimation && _enableSmartFeatures)
            {
                _isFocusAnimating = true;
                _animationTimer?.Start();
            }
            else
            {
                Invalidate();
            }
        }
        
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _helper?.HandleFocusLost();

            if (PasswordRevealed && AutoReMaskDelayMs > 0 && !IsDisposed)
            {
                _revealTimer?.Stop();
                _revealTimer?.Start();
            }
            
            if (_enableFocusAnimation && _enableSmartFeatures)
            {
                _isFocusAnimating = true;
                _animationTimer?.Start();
            }
            else
            {
                Invalidate();
            }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (e.Button == MouseButtons.Right)
            {
                HideAutoCompletePopup();
                ShowTextBoxContextMenu(e.Location);
                return;
            }
            
            if (!_readOnly)
            {
                Focus();
                _helper?.HandleMouseClick(e, _textRect);
                
                if (e.Clicks >= 3)
                {
                    SelectLineAtCurrentPosition();
                }
                else if (e.Clicks >= 2)
                {
                    SelectWordAtCurrentPosition();
                }
            }
        }
        
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_multiline)
            {
                _helper?.Scrolling?.HandleMouseWheel(e);
            }
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            // Only start timer if control has a valid handle and is not being disposed
            // This prevents "Error creating window handle" when control is added before handle is created
            if (!IsHandleCreated || IsDisposed || Disposing)
                return;
                
            _needsLayoutUpdate = true;
            _delayedUpdateTimer?.Stop();
            _delayedUpdateTimer?.Start();

            // Use cached min height computed via TextRenderer rather than CreateGraphics
            if (!_multiline && !_isInitializing && !IsDisposed && IsHandleCreated && _cachedMinHeightPx > 0)
            {
                try
                {
                    if (MinimumSize.Height < _cachedMinHeightPx)
                    {
                        MinimumSize = new Size(MinimumSize.Width, _cachedMinHeightPx);
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            
            // FIX: Recalculate drawing rectangles now that handle exists
            // This ensures _textRect is properly calculated for text rendering
            UpdateDrawingRect();
            UpdateLines();
            _helper?.InvalidateAllCaches();
            UpdateTrailingIcon();
            UpdateRequiredIndicator();
            
            // Force immediate paint to display initial text
            if (!string.IsNullOrEmpty(_text))
            {
                Invalidate();
                Update(); // Force immediate paint, don't wait for message queue
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible && !string.IsNullOrEmpty(_text))
            {
                Invalidate();
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            UpdateDrawingRect();
            _helper?.InvalidateAllCaches();
            Invalidate();
        }

        #endregion
        
        #region "Paint Override"
        
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        //    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
        //    base.OnPaint(e);
        //}
        
        protected override void DrawContent(Graphics g)
        {
            if (g == null) return;
            base.DrawContent(g);

            if (HasError)
            {
                using (var brush = new SolidBrush(Color.FromArgb(24, ErrorColor)))
                    g.FillRectangle(brush, ClientRectangle);
                using (var pen = new Pen(ErrorColor, Math.Max(2, _borderWidth)))
                    CreateAndDrawBorderPath(g, pen);
            }
            else if (_readOnly)
            {
                using (var brush = new SolidBrush(Color.FromArgb(64, DisabledBackColor)))
                    g.FillRectangle(brush, ClientRectangle);
            }
            else if (IsHovered && !Focused)
            {
                using (var brush = new SolidBrush(Color.FromArgb(32, HoverBackColor)))
                    g.FillRectangle(brush, ClientRectangle);
            }

            if (Focused && _enableFocusAnimation && _focusAnimationProgress > 0 && !HasError && !_readOnly)
            {
                DrawFocusAnimation(g);
            }

            if (_showCharacterCount && _maxLength > 0)
            {
                DrawCharacterCount(g);
            }

            if (_isTyping && _enableTypingIndicator && _enableSmartFeatures && !_readOnly)
            {
                DrawTypingIndicator(g);
            }

            if (_isComposing && !string.IsNullOrEmpty(_compositionText))
            {
                DrawCompositionUnderline(g);
            }

            _helper?.DrawAll(g, DrawingRect, _textRect);
        }

        private void DrawCompositionUnderline(Graphics g)
        {
            if (_helper?.Caret == null) return;
            if (string.IsNullOrEmpty(_compositionText)) return;
            if (_textRect.Width <= 0 || _textRect.Height <= 0) return;

            Font font = TextFont ?? Font ?? SystemFonts.MessageBoxFont;
            int caretPos = _compositionStart;
            string textBefore = _text?.Substring(0, Math.Min(caretPos, _text?.Length ?? 0)) ?? string.Empty;
            var beforeSize = TextRenderer.MeasureText(g, textBefore, font);
            var compSize = TextRenderer.MeasureText(g, _compositionText, font);

            int x = _textRect.Left + beforeSize.Width;
            int y = _textRect.Bottom - 3;
            int width = Math.Min(compSize.Width, Math.Max(0, _textRect.Right - x));

            using (var pen = new Pen(ForeColor, 2))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawLine(pen, x, y, x + width, y);
            }
        }

        private void CreateAndDrawBorderPath(Graphics g, Pen pen)
        {
            if (BorderRadius > 0)
            {
                var rect = ClientRectangle;
                rect.Inflate(-1, -1);
                using (var path = GraphicsExtensions.GetRoundedRectPath(rect, BorderRadius))
                {
                    g.DrawPath(pen, path);
                }
            }
            else
            {
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
        }
        
        #endregion
        
        #region "Internal Event Handlers"
        
        private void OnTextChangedInternal(string oldText, string newText)
        {
            Modified = oldText != newText;
            TextChanged?.Invoke(this, EventArgs.Empty);
            NotifyTextChangedUIA();
        }
        
        #region "Context Menu"
        
        private void HideAutoCompletePopup()
        {
            _helper?.AutoComplete?.CloseAutoComplete();
        }
        
        private void ShowTextBoxContextMenu(Point location)
        {
            var items = new List<SimpleItem>
            {
                CreateMenuItemWithShortcut("Undo", "Ctrl+Z", SvgsUIcons.Common.Undo, "undo"),
                CreateMenuItemWithShortcut("Redo", "Ctrl+Y", SvgsUIcons.Common.Redo, "redo"),
                CreateMenuSeparator(),
                CreateMenuItemWithShortcut("Cut", "Ctrl+X", SvgsUIcons.Common.Edit, "cut"),
                CreateMenuItemWithShortcut("Copy", "Ctrl+C", SvgsUIcons.Common.Copy, "copy"),
                CreateMenuItemWithShortcut("Paste", "Ctrl+V", SvgsUIcons.Common.Paste, "paste"),
                CreateMenuItemWithShortcut("Delete", "Del", SvgsUIcons.Common.Delete, "delete"),
                CreateMenuSeparator(),
                CreateMenuItemWithShortcut("Select All", "Ctrl+A", null, "selectall")
            };
            
            int selLen = _helper?.Caret?.SelectionLength ?? 0;
            bool hasText = !string.IsNullOrEmpty(_text);
            bool canUndo = _helper?.UndoRedo?.HasUndo == true;
            bool canRedo = _helper?.UndoRedo?.HasRedo == true;
            
            foreach (var item in items)
            {
                var itemTag = item.Tag as string;
                if (itemTag == "separator") continue;
                item.IsEnabled = itemTag switch
                {
                    "undo" => canUndo,
                    "redo" => canRedo,
                    "cut" => !_readOnly && selLen > 0,
                    "copy" => selLen > 0,
                    "paste" => !_readOnly && Clipboard.ContainsText(),
                    "delete" => !_readOnly && selLen > 0,
                    "selectall" => hasText,
                    _ => true
                };
            }
            
            var selected = ShowContextMenu(items, PointToScreen(location));
            if (selected?.Tag is string selectedTag)
            {
                switch (selectedTag)
                {
                    case "undo": _helper?.UndoRedo?.Undo(); break;
                    case "redo": _helper?.UndoRedo?.Redo(); break;
                    case "cut": Cut(); break;
                    case "copy": Copy(); break;
                    case "paste": Paste(); break;
                    case "delete": DeleteSelection(); break;
                    case "selectall": SelectAll(); break;
                }
            }
        }
        
        private void DeleteSelection()
        {
            if (_readOnly) return;
            if ((_helper?.Caret?.HasSelection) == true)
            {
                HandleDelete();
            }
        }

        private void SelectWordAtCurrentPosition()
        {
            var caret = _helper?.Caret;
            if (caret == null || string.IsNullOrEmpty(_text)) return;

            int pos = caret.CaretPosition;
            if (pos < 0 || pos > _text.Length) return;

            int start = pos;
            while (start > 0 && !char.IsWhiteSpace(_text[start - 1]))
                start--;

            int end = pos;
            while (end < _text.Length && !char.IsWhiteSpace(_text[end]))
                end++;

            caret.SelectionStart = start;
            caret.SelectionLength = end - start;
            caret.CaretPosition = end;
        }

        private void SelectLineAtCurrentPosition()
        {
            var caret = _helper?.Caret;
            if (caret == null || string.IsNullOrEmpty(_text)) return;

            int pos = caret.CaretPosition;
            int lineStart = 0;
            int i = 0;

            for (int lineIdx = 0; lineIdx < _lines.Count && i < _text.Length; lineIdx++)
            {
                int lineLen = _lines[lineIdx].Length;
                int lineEnd = lineStart + lineLen;
                if (pos >= lineStart && pos <= lineEnd)
                {
                    caret.SelectionStart = lineStart;
                    caret.SelectionLength = lineLen;
                    caret.CaretPosition = lineEnd;
                    return;
                }

                i = lineStart + lineLen;
                if (i >= _text.Length) break;

                if (_text[i] == '\r')
                {
                    i++;
                    if (i < _text.Length && _text[i] == '\n') i++;
                }
                else if (_text[i] == '\n')
                {
                    i++;
                }
                lineStart = i;
            }
        }
        
        #endregion
        
        #endregion
    }
}
