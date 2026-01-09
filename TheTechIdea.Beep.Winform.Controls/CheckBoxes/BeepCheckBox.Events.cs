using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.CheckBoxes
{
    public partial class BeepCheckBox<T>
    {
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            // Force a layout update when the text changes.
        }

        #region Keyboard Event Handlers
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                if (Enabled)
                {
                    // Toggle state when Spacebar or Enter is pressed
                    _state = _state == CheckBoxState.Checked ? CheckBoxState.Unchecked : CheckBoxState.Checked;
                    OnStateChanged();
                    e.Handled = true;
                }
            }
        }

        // Provide visual feedback when the control receives focus
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate(); // Redraw to show focus indication
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate(); // Redraw to remove focus indication
        }

        // Make the control focusable and ensure proper tab navigation
        protected bool CanSelect => true;

        // Optional: Handle arrow keys or other navigation if desired
        protected override bool IsInputKey(Keys keyData)
        {
            // Allow arrow keys and other navigation keys to be processed
            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
            {
                return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!Focused)
            {
                Focus();
            }
            base.OnMouseClick(e);

            // Make the entire control area clickable - this is standard checkbox behavior
            // Users expect to be able to click anywhere on the control (text or checkbox) to toggle
            if (Enabled)
            {
                // Toggle between checked and unchecked states only
                // (Don't cycle through indeterminate state unless specifically needed)
                _state = _state == CheckBoxState.Checked ? CheckBoxState.Unchecked : CheckBoxState.Checked;

                // Update the current value based on the new state
                _currentValue = _state == CheckBoxState.Checked ? _checkedValue : _uncheckedValue;

                // Fire the StateChanged event
                StateChanged?.Invoke(this, EventArgs.Empty);

                // Redraw the control
                Invalidate();
            }
        }
        #endregion
    }
}
