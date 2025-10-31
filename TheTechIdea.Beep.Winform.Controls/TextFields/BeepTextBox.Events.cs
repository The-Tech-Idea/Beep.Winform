using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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
            _delayedUpdateTimer.Stop();
            
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
                    _animationTimer.Stop();
                }
                
                Invalidate();
            }
        }
        
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            _typingTimer.Stop();
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
                _animationTimer.Start();
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
            
            if (_enableFocusAnimation && _enableSmartFeatures)
            {
                _isFocusAnimating = true;
                _animationTimer.Start();
            }
            else
            {
                Invalidate();
            }
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!_readOnly)
            {
                Focus();
                _helper?.HandleMouseClick(e, _textRect);
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
        #endregion
        
        #region "Paint Override"
        
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            base.OnPaint(e);
        }
        
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            if (Focused && _enableFocusAnimation && _focusAnimationProgress > 0)
            {
                DrawFocusAnimation(g);
            }

            if (_showCharacterCount && _maxLength > 0)
            {
                DrawCharacterCount(g);
            }

            if (_isTyping && _enableTypingIndicator && _enableSmartFeatures)
            {
                DrawTypingIndicator(g);
            }

            _helper?.DrawAll(g, ClientRectangle, _textRect);
        }
        
        #endregion
        
        #region "Internal Event Handlers"
        
        private void OnTextChangedInternal(string oldText, string newText)
        {
            Modified = oldText != newText;
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
        
        #endregion
    }
}
