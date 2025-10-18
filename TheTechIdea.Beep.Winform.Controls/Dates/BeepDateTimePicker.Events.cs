using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;
using TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    /// <summary>
    /// BeepDateTimePicker - Events partial: Mouse, keyboard, and dropdown event handlers
    /// Uses IDateTimePickerHitHandler for mode-specific interaction logic
    /// </summary>
    public partial class BeepDateTimePicker
    {
        #region Mouse Event Handlers

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_hitHandler == null || _currentPainter == null) return;

            // Use hit handler for mode-specific hit testing
            var layout = _currentPainter.CalculateLayout(DrawingRect, GetCurrentProperties());
            if (layout != null)
            {
                var hitResult = _hitHandler.HitTest(e.Location, layout, _displayMonth, GetCurrentProperties());
                
                // Update hover state through handler
                _hitHandler.UpdateHoverState(hitResult, _hoverState);
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            
            if (_hoverState.HoverArea != DateTimePickerHitArea.None)
            {
                _hoverState.ClearHover();
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _isMouseDown = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isMouseDown = false;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (_hitHandler == null || _currentPainter == null) return;

            // Use hit handler for mode-specific click handling
            var layout = _currentPainter.CalculateLayout(DrawingRect, GetCurrentProperties());
            if (layout != null)
            {
                var hitResult = _hitHandler.HitTest(e.Location, layout, _displayMonth, GetCurrentProperties());
                
                // Let handler process the click
                bool shouldClose = _hitHandler.HandleClick(hitResult, this);
                
                // Sync handler state back to control
                _hitHandler.SyncToControl(this);
                
                // Refresh UI
                Invalidate();
                
                // If this is in a dropdown and selection is complete, close it
                if (shouldClose && this.Parent is ToolStripDropDown dropdown)
                {
                    dropdown.Close();
                }
            }
        }

        #endregion

        #region Keyboard Event Handlers

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
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

                case Keys.Down:
                    if (_selectedDate.HasValue)
                    {
                        NavigateDate(7);
                        e.Handled = true;
                    }
                    break;

                case Keys.PageUp:
                    NavigateToPreviousMonth();
                    e.Handled = true;
                    break;

                case Keys.PageDown:
                    NavigateToNextMonth();
                    e.Handled = true;
                    break;

                case Keys.Home:
                    SelectToday();
                    e.Handled = true;
                    break;
            }
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
    }
}
