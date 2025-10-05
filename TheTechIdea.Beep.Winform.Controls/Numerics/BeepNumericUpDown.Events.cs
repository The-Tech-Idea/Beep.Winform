using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Numerics
{
    /// <summary>
    /// Partial class for BeepNumericUpDown event handling
    /// </summary>
    public partial class BeepNumericUpDown
    {
        #region Mouse Events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            UpdateButtonHoverState(e.Location);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_upButtonHovered || _downButtonHovered)
            {
                _upButtonHovered = false;
                _downButtonHovered = false;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && _showSpinButtons)
            {
                if (_upButtonRect.Contains(e.Location))
                {
                    _upButtonPressed = true;
                    StartRepeatTimer(true);
                    Invalidate();
                }
                else if (_downButtonRect.Contains(e.Location))
                {
                    _downButtonPressed = true;
                    StartRepeatTimer(false);
                    Invalidate();
                }
                else
                {
                    // Click in text area - start editing
                    StartEditing();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_upButtonPressed || _downButtonPressed)
            {
                _upButtonPressed = false;
                _downButtonPressed = false;
                StopRepeatTimer();
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            HandleHitAreaClick(e.Location);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (!_showSpinButtons || (!_upButtonRect.Contains(e.Location) && !_downButtonRect.Contains(e.Location)))
            {
                StartEditing();
            }
        }
        #endregion

        #region Keyboard Events
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!_interceptArrowKeys && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Up:
                    IncrementValueInternal();
                    e.Handled = true;
                    break;

                case Keys.Down:
                    DecrementValueInternal();
                    e.Handled = true;
                    break;

                case Keys.PageUp:
                    IncrementValueInternal(_incrementValue * 10);
                    e.Handled = true;
                    break;

                case Keys.PageDown:
                    DecrementValueInternal(_incrementValue * 10);
                    e.Handled = true;
                    break;

                case Keys.Home:
                    Value = _minimumValue;
                    e.Handled = true;
                    break;

                case Keys.End:
                    Value = _maximumValue;
                    e.Handled = true;
                    break;

                case Keys.Enter:
                    if (_isEditing)
                    {
                        EndEditing(true);
                        e.Handled = true;
                    }
                    break;

                case Keys.Escape:
                    if (_isEditing)
                    {
                        EndEditing(false);
                        e.Handled = true;
                    }
                    break;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (_interceptArrowKeys)
            {
                switch (keyData)
                {
                    case Keys.Up:
                    case Keys.Down:
                        return true;
                }
            }
            return base.ProcessDialogKey(keyData);
        }
        #endregion

        #region Focus Events
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (_selectAllOnFocus && !_isEditing)
            {
                StartEditing();
            }
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (_isEditing)
            {
                EndEditing(true);
            }
            Invalidate();
        }
        #endregion

        #region Timer Events
        private void StartRepeatTimer(bool increment)
        {
            if (_repeatTimer == null)
            {
                _repeatTimer = new System.Windows.Forms.Timer();
                _repeatTimer.Tick += RepeatTimer_Tick;
            }

            _repeatCount = 0;
            _repeatTimer.Interval = INITIAL_DELAY;
            _repeatTimer.Tag = increment; // Store direction
            _repeatTimer.Start();

            // Immediate action
            if (increment)
                IncrementValueInternal();
            else
                DecrementValueInternal();
        }

        private void StopRepeatTimer()
        {
            if (_repeatTimer != null)
            {
                _repeatTimer.Stop();
                _repeatCount = 0;
            }
        }

        private void RepeatTimer_Tick(object sender, EventArgs e)
        {
            _repeatCount++;

            // Speed up after several repeats
            if (_repeatCount > 5 && _repeatTimer.Interval > REPEAT_DELAY)
            {
                _repeatTimer.Interval = REPEAT_DELAY;
            }

            bool increment = (bool)_repeatTimer.Tag;
            if (increment)
                IncrementValueInternal();
            else
                DecrementValueInternal();
        }
        #endregion
    }
}
