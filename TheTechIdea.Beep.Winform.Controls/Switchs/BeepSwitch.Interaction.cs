using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepSwitch
    {
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (!Enabled) return;
            
            if (_dragToToggleEnabled && _metrics.ThumbCurrentRect.Contains(e.Location))
            {
                _dragging = true;
                _dragStartX = e.X;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (!Enabled)
            {
                Cursor = Cursors.Default;
                return;
            }

            if (!_dragging)
            {
                Cursor = Cursors.Hand;
            }
            
            int totalDistance = _metrics.ThumbOnRect.X - _metrics.ThumbOffRect.X;
            if (totalDistance == 0) return;
            
            int currentDistance = e.X - _dragStartX;
            float progress = (float)currentDistance / totalDistance;
            
            if (_checked)
            {
                _animProgress = 1f + progress;
            }
            else
            {
                _animProgress = progress;
            }
            
            _animProgress = Math.Max(0f, Math.Min(1f, _animProgress));
            UpdateMetrics();
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            
            if (!_dragging) return;
            
            _dragging = false;
            
            bool newState = _animProgress > 0.5f;
            if (newState != _checked)
            {
                Checked = newState;
            }
            else
            {
                AnimateToggle(_checked);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Default;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Space || 
                (keyData & Keys.KeyCode) == Keys.Enter ||
                (keyData & Keys.KeyCode) == Keys.Left ||
                (keyData & Keys.KeyCode) == Keys.Right)
            {
                return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (!Enabled) return;

            switch (e.KeyCode)
            {
                case Keys.Space:
                case Keys.Enter:
                    Checked = !Checked;
                    e.Handled = true;
                    break;
                case Keys.Right:
                case Keys.Up:
                    if (!Checked)
                    {
                        Checked = true;
                        e.Handled = true;
                    }
                    break;
                case Keys.Left:
                case Keys.Down:
                    if (Checked)
                    {
                        Checked = false;
                        e.Handled = true;
                    }
                    break;
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }
    }
}

