using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    internal class ControlInputHelper
    {
        private readonly BaseControl _owner;
        private readonly ControlEffectHelper _effects;
        private readonly ControlHitTestHelper _hitTest;
        // Re-entrancy guard to prevent click recursion via SendMouseEvent -> ReceiveMouseEvent -> OnClick
        private bool _isDispatchingClick;

        public ControlInputHelper(BaseControl owner, ControlEffectHelper effects, ControlHitTestHelper hitTest)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _effects = effects ?? throw new ArgumentNullException(nameof(effects));
            _hitTest = hitTest ?? throw new ArgumentNullException(nameof(hitTest));
        }

        #region Key Events
        public event EventHandler TabKeyPressed;
        public event EventHandler ShiftTabKeyPressed;
        public event EventHandler EnterKeyPressed;
        public event EventHandler EscapeKeyPressed;
        public event EventHandler LeftArrowKeyPressed;
        public event EventHandler RightArrowKeyPressed;
        public event EventHandler UpArrowKeyPressed;
        public event EventHandler DownArrowKeyPressed;
        public event EventHandler PageUpKeyPressed;
        public event EventHandler PageDownKeyPressed;
        public event EventHandler HomeKeyPressed;
        public event EventHandler EndKeyPressed;
        public event EventHandler<KeyEventArgs> DialogKeyDetected;
        #endregion

        #region Mouse Event Handlers
        public void OnMouseEnter()
        {
            Point location = _owner.PointToClient(Cursor.Position);
            _hitTest.HandleMouseEnter(location);
            _owner.Invalidate();
        }

        public void OnMouseLeave()
        {
            _hitTest.HandleMouseLeave();
            _owner.Invalidate();
        }

        public void OnMouseMove(Point location)
        {
            _hitTest.HandleMouseMove(location);
        }

        public void OnMouseDown(MouseEventArgs e)
        {
            if (_effects.EnableRippleEffect && e.Button == MouseButtons.Left)
            {
                _effects.StartMaterialRipple(e.Location);
            }

            if (e.Button == MouseButtons.Left)
            {
                _hitTest.HandleMouseDown(e.Location, e);
            }

            _owner.Invalidate();
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            _hitTest.HandleMouseUp(e.Location, e);
            _owner.Invalidate();
        }

        public void OnMouseHover()
        {
            Point location = _owner.PointToClient(Cursor.Position);
            _hitTest.HandleMouseHover(location);
            // Show tooltip if configured
            if (!string.IsNullOrEmpty(_owner.ToolTipText))
            {
                _owner.ShowToolTip(_owner.ToolTipText);
            }
        }

        public void OnClick()
        {
            if (_owner.IsDisposed) return;

            // Prevent recursion when clicks are re-routed through ReceiveMouseEvent
            if (_isDispatchingClick) return;

            try
            {
                _isDispatchingClick = true;
                Point location = _owner.PointToClient(Cursor.Position);
                _hitTest.HandleClick(location);
            }
            catch (ObjectDisposedException)
            {
                // Handle gracefully if control is disposed during click
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnClick: {ex.Message}");
            }
            finally
            {
                _isDispatchingClick = false;
            }
        }

        public void OnGotFocus()
        {
            Point location = _owner.PointToClient(Cursor.Position);
            _hitTest.HandleGotFocus(location);
            _owner.Invalidate();
        }

        public void OnLostFocus()
        {
            _hitTest.HandleLostFocus();
            _owner.Invalidate();
        }
        #endregion

        #region Key Processing
        public bool ProcessDialogKey(Keys keyData)
        {
            var keyCode = (keyData & Keys.KeyCode);
            DialogKeyDetected?.Invoke(_owner, new KeyEventArgs(keyData));

            bool shiftPressed = (keyData & Keys.Shift) == Keys.Shift;

            switch (keyCode)
            {
                case Keys.Tab:
                    if (shiftPressed)
                    {
                        ShiftTabKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    }
                    else
                    {
                        TabKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    }
                    return false; // Allow normal tab navigation

                case Keys.Enter:
                    EnterKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    return true; // Handled

                case Keys.Escape:
                    EscapeKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    return true; // Handled

                case Keys.Left:
                    LeftArrowKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    return false;

                case Keys.Right:
                    RightArrowKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    return false;

                case Keys.Up:
                    UpArrowKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    return false;

                case Keys.Down:
                    DownArrowKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    return false;

                case Keys.PageUp:
                    PageUpKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    return false;

                case Keys.PageDown:
                    PageDownKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    return false;

                case Keys.Home:
                    HomeKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    return false;

                case Keys.End:
                    EndKeyPressed?.Invoke(_owner, EventArgs.Empty);
                    return false;

                default:
                    return false; // Let base class handle
            }
        }
        #endregion

        #region IBeepUIComponent Mouse Event Interface
        public void ReceiveMouseEvent(HitTestEventArgs eventArgs)
        {
            Point location = eventArgs.Location;
            switch (eventArgs.MouseEvent)
            {
                case MouseEventType.Click:
                    OnClick();
                    break;
                case MouseEventType.DoubleClick:
                    // Handle double click if needed
                    break;
                case MouseEventType.MouseDown:
                    OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, location.X, location.Y, 0));
                    break;
                case MouseEventType.MouseUp:
                    OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, location.X, location.Y, 0));
                    break;
                case MouseEventType.MouseMove:
                    OnMouseMove(location);
                    break;
                case MouseEventType.MouseEnter:
                    OnMouseEnter();
                    break;
                case MouseEventType.MouseLeave:
                    OnMouseLeave();
                    break;
                case MouseEventType.MouseHover:
                    OnMouseHover();
                    break;
                case MouseEventType.MouseWheel:
                    // Handle mouse wheel if needed
                    break;
                case MouseEventType.None:
                default:
                    break;
            }
        }
        #endregion

        #region Utility Methods
        public void StartRippleEffect(Point center)
        {
            _effects.StartRippleEffect(center);
        }

        public void ShowWithAnimation(DisplayAnimationType animationType, Control parentControl = null)
        {
            _effects.ShowWithAnimation(animationType, parentControl);
        }

        public void ShowWithDropdownAnimation(Control parentControl = null)
        {
            _effects.ShowWithDropdownAnimation(parentControl);
        }

        public void StopAnimation()
        {
            _effects.StopAnimation();
        }
        #endregion
    }
}
