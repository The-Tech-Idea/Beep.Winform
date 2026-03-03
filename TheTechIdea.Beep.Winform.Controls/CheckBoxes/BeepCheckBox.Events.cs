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
            if (AutoSize)
            {
                Size = GetPreferredSize(Size.Empty);
            }
            RequestVisualRefresh(includeText: true);
        }

        #region Keyboard Event Handlers
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                if (Enabled)
                {
                    // Keyboard toggle keeps parity with mouse behavior.
                    _keyboardFocusVisible = true;
                    State = State == CheckBoxState.Checked ? CheckBoxState.Unchecked : CheckBoxState.Checked;
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                _keyboardFocusVisible = true;
                RequestVisualRefresh(includeText: false);
            }
        }

        // Provide visual feedback when the control receives focus
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            RequestVisualRefresh(includeText: false);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _keyboardFocusVisible = false;
            RequestVisualRefresh(includeText: false);
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
            _keyboardFocusVisible = false;
            if (!Focused)
            {
                Focus();
            }
            base.OnMouseClick(e);

            // Make the entire control area clickable - this is standard checkbox behavior
            // Users expect to be able to click anywhere on the control (text or checkbox) to toggle
            if (Enabled)
            {
                // Keep two-state click behavior, but route through State property
                // so mappings, submit notifications, and painters stay in sync.
                State = State == CheckBoxState.Checked ? CheckBoxState.Unchecked : CheckBoxState.Checked;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            RequestVisualRefresh(includeText: false);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            RequestVisualRefresh(includeText: false);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            ClearGraphicsCaches();
            base.OnHandleDestroyed(e);
        }
        #endregion
    }
}
