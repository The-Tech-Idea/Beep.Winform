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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_currentPainter == null) return;

            // Perform hit test on the painted calendar
            var layout = _currentPainter.CalculateLayout(DrawingRect, GetCurrentProperties());
            if (layout != null)
            {
                var hitResult = _currentPainter.HitTest(e.Location, layout, _displayMonth);
                if (hitResult != null)
                {
                    _hoverState.HoverArea = hitResult.HitArea;
                    _hoverState.HoveredDate = hitResult.Date;
                    _hoverState.HoveredTime = hitResult.Time;
                    _hoverState.HoverBounds = hitResult.HitBounds;
                    Invalidate();
                }
                else
                {
                    if (_hoverState.HoverArea != DateTimePickerHitArea.None)
                    {
                        _hoverState.ClearHover();
                        Invalidate();
                    }
                }
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

            if (_currentPainter == null) return;

            // Perform hit test and handle click
            var layout = _currentPainter.CalculateLayout(DrawingRect, GetCurrentProperties());
            if (layout != null)
            {
                var hitResult = _currentPainter.HitTest(e.Location, layout, _displayMonth);
                if (hitResult != null)
                {
                    HandleClick(hitResult);
                }
            }
        }

        private void HandleClick(DateTimePickerHitTestResult hitResult)
        {
            switch (hitResult.HitArea)
            {
                case DateTimePickerHitArea.DayCell:
                    if (hitResult.Date.HasValue && IsDateInRange(hitResult.Date.Value))
                    {
                        if (_mode == DatePickerMode.Multiple)
                        {
                            ToggleMultipleDate(hitResult.Date.Value);
                            Invalidate();
                        }
                        else
                        {
                            SetDate(hitResult.Date.Value);
                        }
                    }
                    break;

                case DateTimePickerHitArea.PreviousButton:
                    NavigateToPreviousMonth();
                    Invalidate();
                    break;

                case DateTimePickerHitArea.NextButton:
                    NavigateToNextMonth();
                    Invalidate();
                    break;

                case DateTimePickerHitArea.TimeSlot:
                    if (hitResult.Time.HasValue && IsTimeInRange(hitResult.Time.Value))
                    {
                        SetTime(hitResult.Time.Value);
                    }
                    break;

                case DateTimePickerHitArea.QuickButton:
                    HandleQuickButton(hitResult.QuickButtonText);
                    break;

                case DateTimePickerHitArea.ClearButton:
                    if (_allowClear)
                    {
                        ClearSelection();
                    }
                    break;
            }
        }

        private void HandleQuickButton(string buttonText)
        {
            if (string.IsNullOrEmpty(buttonText)) return;

            switch (buttonText.ToLower())
            {
                case "today":
                    SelectToday();
                    break;
                case "tomorrow":
                    SelectTomorrow();
                    break;
                case "yesterday":
                    SelectYesterday();
                    break;
                case "this week":
                    SelectThisWeek();
                    break;
                case "this month":
                    SelectThisMonth();
                    break;
            }

            Invalidate();
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
