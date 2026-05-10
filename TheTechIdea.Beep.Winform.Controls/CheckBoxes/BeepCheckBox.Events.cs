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
                if (Enabled && !ReadOnly)
                {
                    _keyboardFocusVisible = true;
                    if (AutoCheck)
                    {
                        ToggleState();
                    }
                    else
                    {
                        RequestVisualRefresh(includeText: false);
                    }
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

            bool canToggleFromMouse = MouseHitMode switch
            {
                CheckBoxMouseHitMode.CheckBoxGlyph => !_lastCheckBoxRect.IsEmpty && _lastCheckBoxRect.Contains(e.Location),
                _ => ClientRectangle.Contains(e.Location)
            };

            if (e.Button == MouseButtons.Left && Enabled && !ReadOnly && AutoCheck && canToggleFromMouse)
            {
                ToggleState();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool showHandCursor = Enabled && !ReadOnly && MouseHitMode switch
            {
                CheckBoxMouseHitMode.CheckBoxGlyph => !_lastCheckBoxRect.IsEmpty && _lastCheckBoxRect.Contains(e.Location),
                _ => ClientRectangle.Contains(e.Location)
            };

            Cursor = showHandCursor ? Cursors.Hand : Cursors.Default;
            RequestVisualRefresh(includeText: false);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Default;
            RequestVisualRefresh(includeText: false);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && Enabled && !ReadOnly)
            {
                IsPressed = true;
                RequestVisualRefresh(includeText: false);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (IsPressed)
            {
                IsPressed = false;
                RequestVisualRefresh(includeText: false);
            }
        }

        protected override bool ProcessMnemonic(char charCode)
        {
            if (CanSelect && IsMnemonic(charCode, Text))
            {
                if (Enabled && !ReadOnly && AutoCheck)
                {
                    _keyboardFocusVisible = true;
                    Focus();
                    ToggleState();
                    return true;
                }
            }
            return base.ProcessMnemonic(charCode);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            ClearGraphicsCaches();
            base.OnHandleDestroyed(e);
        }
        #endregion
    }
}
