using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepSwitch - Mouse and keyboard interaction
    /// </summary>
    public partial class BeepSwitch
    {
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            // BaseControl hit areas will handle the click
            // No need to duplicate logic here
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (!Enabled) return;
            
            // Check if clicking on thumb for drag support
            if (_dragToToggleEnabled && _metrics.ThumbCurrentRect.Contains(e.Location))
            {
                _dragging = true;
                _dragStartX = e.X;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (!Enabled || !_dragging) return;
            
            // Calculate drag progress
            int totalDistance = _metrics.ThumbOnRect.X - _metrics.ThumbOffRect.X;
            if (totalDistance == 0) return;
            
            int currentDistance = e.X - _dragStartX;
            float progress = (float)currentDistance / totalDistance;
            
            // Adjust based on current state
            if (_checked)
            {
                _animProgress = 1f + progress;
            }
            else
            {
                _animProgress = progress;
            }
            
            _animProgress = System.Math.Max(0f, System.Math.Min(1f, _animProgress));
            UpdateMetrics();
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            
            if (!_dragging) return;
            
            _dragging = false;
            
            // Snap to nearest state
            bool newState = _animProgress > 0.5f;
            if (newState != _checked)
            {
                Checked = newState;
            }
            else
            {
                // Reset to current state with animation
                AnimateToggle(_checked);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (!Enabled) return base.ProcessDialogKey(keyData);
            
            // Space or Enter toggles the switch
            if (keyData == Keys.Space || keyData == Keys.Enter)
            {
                Checked = !Checked;
                return true;
            }
            
            return base.ProcessDialogKey(keyData);
        }

        protected override void OnEnabledChanged(System.EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(System.EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }
    }
}

