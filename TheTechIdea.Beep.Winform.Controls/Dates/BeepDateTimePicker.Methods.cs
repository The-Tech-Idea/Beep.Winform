using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    /// <summary>
    /// BeepDateTimePicker - Methods partial: Public methods for date/time operations and dropdown management
    /// </summary>
    public partial class BeepDateTimePicker
    {
        #region Dropdown Management

        public void OpenDropDown()
        {
            if (_isDropDownOpen || _currentPainter == null) return;

            // Create dropdown form
            _dropdownForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.Manual,
                TopMost = true
            };

            // Get preferred size from painter
            var dropdownSize = _currentPainter.GetPreferredDropDownSize(GetCurrentProperties());
            _dropdownForm.Size = dropdownSize;

            // Position below the control
            var screenPoint = PointToScreen(new Point(0, Height));
            var screen = Screen.FromControl(this);

            // Adjust if it would go off screen
            if (screenPoint.Y + dropdownSize.Height > screen.WorkingArea.Bottom)
            {
                screenPoint.Y = PointToScreen(new Point(0, 0)).Y - dropdownSize.Height;
            }

            if (screenPoint.X + dropdownSize.Width > screen.WorkingArea.Right)
            {
                screenPoint.X = screen.WorkingArea.Right - dropdownSize.Width;
            }

            _dropdownForm.Location = screenPoint;

            // Create panel for calendar
            var panel = new DropdownPanel(this)
            {
                Dock = DockStyle.Fill,
                BackColor = UseThemeColors ? _currentTheme?.BackgroundColor ?? Color.White : Color.White
            };

            _dropdownForm.Controls.Add(panel);

            // Hook up events
            _dropdownForm.Deactivate += OnDropdownFormDeactivate;
            _dropdownForm.FormClosed += OnDropdownFormClosed;

            // Show the form
            _dropdownForm.Show(this);
            _isDropDownOpen = true;
            
            OnDropDownOpened();
            Invalidate();
        }

        public void CloseDropDown()
        {
            if (!_isDropDownOpen || _dropdownForm == null) return;

            _dropdownForm.Close();
            _dropdownForm = null;
            _isDropDownOpen = false;
            
            OnDropDownClosed();
            Invalidate();
        }

        #endregion

        #region Date Selection Methods

        public void SetDate(DateTime date)
        {
            if (date < _minDate || date > _maxDate) return;

            _selectedDate = date;
            _displayMonth = new DateTime(date.Year, date.Month, 1);
            OnDateChanged(date);
            Invalidate();
        }

        public void SetTime(TimeSpan time)
        {
            if (time < _minTime || time > _maxTime) return;

            _selectedTime = time;
            OnTimeChanged(time);
            Invalidate();
        }

        public void SetRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            if (startDate < _minDate || endDate > _maxDate) return;

            _rangeStartDate = startDate;
            _rangeEndDate = endDate;
            _displayMonth = new DateTime(startDate.Year, startDate.Month, 1);
            OnRangeChanged(startDate, endDate);
            Invalidate();
        }

        public void ClearSelection()
        {
            _selectedDate = null;
            _selectedTime = null;
            _rangeStartDate = null;
            _rangeEndDate = null;
            _selectedDates?.Clear();
            
            OnDateChanged(null);
            ClearClicked?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }

        public void SetToday()
        {
            var today = DateTime.Today;
            if (today >= _minDate && today <= _maxDate)
            {
                SetDate(today);
            }
        }

        public void NavigateDate(int days)
        {
            if (!_selectedDate.HasValue) return;

            var newDate = _selectedDate.Value.AddDays(days);
            if (newDate >= _minDate && newDate <= _maxDate)
            {
                SetDate(newDate);
            }
        }

        #endregion

        #region Month Navigation

        public void NavigateToPreviousMonth()
        {
            _displayMonth = _displayMonth.AddMonths(-1);
            if (_displayMonth < _minDate)
            {
                _displayMonth = new DateTime(_minDate.Year, _minDate.Month, 1);
            }
            Invalidate();
        }

        public void NavigateToNextMonth()
        {
            _displayMonth = _displayMonth.AddMonths(1);
            if (_displayMonth > _maxDate)
            {
                _displayMonth = new DateTime(_maxDate.Year, _maxDate.Month, 1);
            }
            Invalidate();
        }

        public void NavigateToMonth(int year, int month)
        {
            if (month < 1 || month > 12) return;

            var newMonth = new DateTime(year, month, 1);
            if (newMonth >= _minDate && newMonth <= _maxDate)
            {
                _displayMonth = newMonth;
                Invalidate();
            }
        }

        #endregion

        #region Quick Selection Methods

        public void SelectToday()
        {
            SetToday();
            QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(DateTime.Today, null, null, _mode));
        }

        public void SelectTomorrow()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            if (tomorrow <= _maxDate)
            {
                SetDate(tomorrow);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(tomorrow, null, null, _mode));
            }
        }

        public void SelectYesterday()
        {
            var yesterday = DateTime.Today.AddDays(-1);
            if (yesterday >= _minDate)
            {
                SetDate(yesterday);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(yesterday, null, null, _mode));
            }
        }

        public void SelectThisWeek()
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);

            if (_mode == DatePickerMode.Range)
            {
                SetRange(startOfWeek, endOfWeek);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(startOfWeek, null, endOfWeek, _mode));
            }
            else
            {
                SetDate(startOfWeek);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(startOfWeek, null, null, _mode));
            }
        }

        public void SelectThisMonth()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            if (_mode == DatePickerMode.Range)
            {
                SetRange(startOfMonth, endOfMonth);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(startOfMonth, null, endOfMonth, _mode));
            }
            else
            {
                SetDate(startOfMonth);
                QuickButtonClicked?.Invoke(this, new DateTimePickerEventArgs(startOfMonth, null, null, _mode));
            }
        }

        #endregion

        #region Helper Methods

        public bool IsDateInRange(DateTime date)
        {
            return date >= _minDate && date <= _maxDate;
        }

        public bool IsTimeInRange(TimeSpan time)
        {
            return time >= _minTime && time <= _maxTime;
        }

        public bool IsDateDisabled(DateTime date)
        {
            return !IsDateInRange(date);
        }

        #endregion

        #region Dropdown Panel

        private void ToggleMultipleDate(DateTime date)
        {
            if (date < _minDate || date > _maxDate) return;
            date = date.Date;

            int idx = _selectedDates.FindIndex(d => d == date);
            if (idx >= 0)
            {
                _selectedDates.RemoveAt(idx);
            }
            else
            {
                _selectedDates.Add(date);
                _selectedDates.Sort();
            }
            Invalidate();
        }

        private bool IsMultipleDateSelected(DateTime date)
        {
            return _selectedDates?.Any(d => d.Date == date.Date) == true;
        }

        private class DropdownPanel : Panel
        {
            private readonly BeepDateTimePicker _owner;

            public DropdownPanel(BeepDateTimePicker owner)
            {
                _owner = owner;
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
                DoubleBuffered = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                if (_owner._currentPainter == null) return;

                var g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Paint the calendar dropdown
                _owner._currentPainter.PaintCalendar(
                    g,
                    ClientRectangle,
                    _owner.GetCurrentProperties(),
                    _owner._displayMonth,
                    _owner._hoverState
                );
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                
                // Perform hit test
                var layout = _owner._currentPainter?.CalculateLayout(ClientRectangle, _owner.GetCurrentProperties());
                if (layout != null)
                {
                    var hitResult = _owner._currentPainter.HitTest(e.Location, layout, _owner._displayMonth);
                    if (hitResult != null)
                    {
                        _owner._hoverState.HoverArea = hitResult.HitArea;
                        _owner._hoverState.HoveredDate = hitResult.Date;
                        _owner._hoverState.HoveredTime = hitResult.Time;
                        _owner._hoverState.HoverBounds = hitResult.HitBounds;
                        Invalidate();
                    }
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                _owner._hoverState.ClearHover();
                Invalidate();
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnMouseClick(e);

                var layout = _owner._currentPainter?.CalculateLayout(ClientRectangle, _owner.GetCurrentProperties());
                if (layout != null)
                {
                    var hitResult = _owner._currentPainter.HitTest(e.Location, layout, _owner._displayMonth);
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
                        if (hitResult.Date.HasValue && _owner.IsDateInRange(hitResult.Date.Value))
                        {
                            if (_owner._mode == DatePickerMode.Multiple)
                            {
                                _owner.ToggleMultipleDate(hitResult.Date.Value);
                                // Keep dropdown open for multiple selection
                                Invalidate();
                            }
                            else
                            {
                                _owner.SetDate(hitResult.Date.Value);
                                if (!_owner._showTime)
                                {
                                    _owner.CloseDropDown();
                                }
                            }
                        }
                        break;

                    case DateTimePickerHitArea.PreviousButton:
                        _owner.NavigateToPreviousMonth();
                        Invalidate();
                        break;

                    case DateTimePickerHitArea.NextButton:
                        _owner.NavigateToNextMonth();
                        Invalidate();
                        break;

                    case DateTimePickerHitArea.TimeSlot:
                        if (hitResult.Time.HasValue && _owner.IsTimeInRange(hitResult.Time.Value))
                        {
                            _owner.SetTime(hitResult.Time.Value);
                        }
                        break;

                    case DateTimePickerHitArea.QuickButton:
                        HandleQuickButton(hitResult.QuickButtonText);
                        break;

                    case DateTimePickerHitArea.ApplyButton:
                        _owner.CloseDropDown();
                        break;

                    case DateTimePickerHitArea.CancelButton:
                        _owner.CloseDropDown();
                        break;
                }
            }

            private void HandleQuickButton(string buttonText)
            {
                if (string.IsNullOrEmpty(buttonText)) return;

                switch (buttonText.ToLower())
                {
                    case "today":
                        _owner.SelectToday();
                        break;
                    case "tomorrow":
                        _owner.SelectTomorrow();
                        break;
                    case "yesterday":
                        _owner.SelectYesterday();
                        break;
                    case "this week":
                        _owner.SelectThisWeek();
                        break;
                    case "this month":
                        _owner.SelectThisMonth();
                        break;
                }

                if (!_owner._showTime)
                {
                    _owner.CloseDropDown();
                }
                else
                {
                    Invalidate();
                }
            }
        }

        #endregion
    }
}
