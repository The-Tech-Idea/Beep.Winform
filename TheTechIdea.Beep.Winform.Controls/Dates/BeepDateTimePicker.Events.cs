using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    /// <summary>
    /// BeepDateTimePicker - Events partial: Mouse, keyboard, and dropdown event handlers
    /// </summary>
    public partial class BeepDateTimePicker
    {
        #region Mouse Event Handlers

        private void OnMouseDownHandler(object sender, MouseEventArgs e)
        {
            _isMouseDown = true;
            _lastMousePosition = e.Location;

            // Check dropdown button
            if (_dropdownButtonRect.Contains(e.Location))
            {
                _hoverState.IsPressed = true;
                _hoverState.PressedArea = DateTimePickerHitArea.DropdownButton;
                Invalidate();
                return;
            }

            // Check clear button
            if (_allowClear && _selectedDate.HasValue && _clearButtonRect.Contains(e.Location))
            {
                _hoverState.IsPressed = true;
                _hoverState.PressedArea = DateTimePickerHitArea.ClearButton;
                Invalidate();
                return;
            }
        }

        private void OnMouseUpHandler(object sender, MouseEventArgs e)
        {
            if (!_isMouseDown) return;

            var pressedArea = _hoverState.PressedArea;
            _isMouseDown = false;
            _hoverState.ClearPress();

            // Handle dropdown button click
            if (pressedArea == DateTimePickerHitArea.DropdownButton && _dropdownButtonRect.Contains(e.Location))
            {
                if (_isDropDownOpen)
                    CloseDropDown();
                else
                    OpenDropDown();
                
                Invalidate();
                return;
            }

            // Handle clear button click
            if (pressedArea == DateTimePickerHitArea.ClearButton && _clearButtonRect.Contains(e.Location))
            {
                ClearSelection();
                Invalidate();
                return;
            }
        }

        private void OnMouseMoveHandler(object sender, MouseEventArgs e)
        {
            var oldHoverArea = _hoverState.HoverArea;
            _hoverState.ClearHover();

            // Check dropdown button hover
            if (_dropdownButtonRect.Contains(e.Location))
            {
                _hoverState.HoverArea = DateTimePickerHitArea.DropdownButton;
                _hoverState.HoverBounds = _dropdownButtonRect;
            }
            // Check clear button hover
            else if (_allowClear && _selectedDate.HasValue && _clearButtonRect.Contains(e.Location))
            {
                _hoverState.HoverArea = DateTimePickerHitArea.ClearButton;
                _hoverState.HoverBounds = _clearButtonRect;
            }

            // Invalidate if hover state changed
            if (oldHoverArea != _hoverState.HoverArea)
            {
                Invalidate();
            }

            _lastMousePosition = e.Location;
        }

        private void OnMouseClickHandler(object sender, MouseEventArgs e)
        {
            // Click handling is done in MouseUp
        }

        private void OnMouseLeaveHandler(object sender, EventArgs e)
        {
            if (_hoverState.HoverArea != DateTimePickerHitArea.None)
            {
                _hoverState.ClearHover();
                Invalidate();
            }
        }

        #endregion

        #region Keyboard Event Handlers

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Space:
                case Keys.Enter:
                    if (!_isDropDownOpen)
                    {
                        OpenDropDown();
                        e.Handled = true;
                    }
                    break;

                case Keys.Escape:
                    if (_isDropDownOpen)
                    {
                        CloseDropDown();
                        e.Handled = true;
                    }
                    break;

                case Keys.Delete:
                case Keys.Back:
                    if (_allowClear && _selectedDate.HasValue)
                    {
                        ClearSelection();
                        e.Handled = true;
                    }
                    break;

                case Keys.Left:
                    if (_selectedDate.HasValue)
                    {
                        NavigateDate(-1);
                        e.Handled = true;
                    }
                    break;

                case Keys.Right:
                    if (_selectedDate.HasValue)
                    {
                        NavigateDate(1);
                        e.Handled = true;
                    }
                    break;

                case Keys.Up:
                    if (_selectedDate.HasValue)
                    {
                        NavigateDate(-7);
                        e.Handled = true;
                    }
                    break;

                case Keys.PageUp:
                    if (_selectedDate.HasValue)
                    {
                        NavigateDate(-30);
                        e.Handled = true;
                    }
                    break;

                case Keys.PageDown:
                    if (_selectedDate.HasValue)
                    {
                        NavigateDate(30);
                        e.Handled = true;
                    }
                    break;

                case Keys.Home:
                    SetToday();
                    e.Handled = true;
                    break;
            }
        }

        private void OnKeyPressHandler(object sender, KeyPressEventArgs e)
        {
            // Handle character input if needed for custom date entry
        }

        #endregion

        #region Dropdown Event Handlers

        private void OnDropdownFormClosed(object sender, FormClosedEventArgs e)
        {
            _isDropDownOpen = false;
            _dropdownForm = null;
            OnDropDownClosed();
            Invalidate();
            Focus();
        }

        private void OnDropdownFormDeactivate(object sender, EventArgs e)
        {
            CloseDropDown();
        }

        #endregion

        #region Focus Event Handlers

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

        #endregion

        #region Resize Event Handlers

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
            Invalidate();
        }

        #endregion
    }
}
