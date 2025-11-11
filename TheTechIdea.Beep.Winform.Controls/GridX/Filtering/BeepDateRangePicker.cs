using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Filtering;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Filtering
{
    /// <summary>
    /// Modern date range picker control using BeepControls
    /// Provides start and end date selection with calendar popup
    /// </summary>
    public class BeepDateRangePicker : BaseControl
    {
        private BeepTextBox _startDateTextBox;
        private BeepTextBox _endDateTextBox;
        private BeepButton _startCalendarButton;
        private BeepButton _endCalendarButton;
        private BeepLabel _toLabel;

        private MonthCalendar _startCalendar;
        private MonthCalendar _endCalendar;
        private Form _calendarPopup;
        private bool _isStartCalendar;

        public event EventHandler DateRangeChanged;

        /// <summary>
        /// Gets or sets the start date
        /// </summary>
        public DateTime? StartDate
        {
            get
            {
                if (DateTime.TryParse(_startDateTextBox.Text, out DateTime dt))
                    return dt;
                return null;
            }
            set
            {
                _startDateTextBox.Text = value?.ToString("yyyy-MM-dd") ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the end date
        /// </summary>
        public DateTime? EndDate
        {
            get
            {
                if (DateTime.TryParse(_endDateTextBox.Text, out DateTime dt))
                    return dt;
                return null;
            }
            set
            {
                _endDateTextBox.Text = value?.ToString("yyyy-MM-dd") ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets whether both dates are set and start is before or equal to end
        /// </summary>
        public bool IsValidRange
        {
            get
            {
                var start = StartDate;
                var end = EndDate;
                if (!start.HasValue || !end.HasValue)
                    return false;
                return start.Value <= end.Value;
            }
        }

        public BeepDateRangePicker()
        {
            IsFrameless = true;
            ShowAllBorders = false;
            ShowShadow = false;

            InitializeControls();
            LayoutControls();
            HookEvents();
        }

        private void InitializeControls()
        {
            // Start date input
            _startDateTextBox = new BeepTextBox
            {
                Width = 120,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true,
                PlaceholderText = "Start date"
            };

            // Start calendar button
            _startCalendarButton = new BeepButton
            {
                Text = "📅",
                Width = 30,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };

            // "to" label
            _toLabel = new BeepLabel
            {
                Text = "to",
                Width = 30,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                IsChild = true,
                IsFrameless = true
            };

            // End date input
            _endDateTextBox = new BeepTextBox
            {
                Width = 120,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true,
                PlaceholderText = "End date"
            };

            // End calendar button
            _endCalendarButton = new BeepButton
            {
                Text = "📅",
                Width = 30,
                Height = 30,
                IsChild = true,
                IsFrameless = true,
                ShowAllBorders = true
            };

            Controls.Add(_startDateTextBox);
            Controls.Add(_startCalendarButton);
            Controls.Add(_toLabel);
            Controls.Add(_endDateTextBox);
            Controls.Add(_endCalendarButton);
        }

        private void LayoutControls()
        {
            int margin = 5;
            int x = margin;
            int y = margin;

            _startDateTextBox.Location = new Point(x, y);
            x += _startDateTextBox.Width + 2;

            _startCalendarButton.Location = new Point(x, y);
            x += _startCalendarButton.Width + margin;

            _toLabel.Location = new Point(x, y);
            x += _toLabel.Width + margin;

            _endDateTextBox.Location = new Point(x, y);
            x += _endDateTextBox.Width + 2;

            _endCalendarButton.Location = new Point(x, y);
            x += _endCalendarButton.Width + margin;

            this.Height = _startDateTextBox.Height + (margin * 2);
            this.Width = x;
        }

        private void HookEvents()
        {
            _startDateTextBox.TextChanged += (s, e) => OnDateRangeChanged();
            _endDateTextBox.TextChanged += (s, e) => OnDateRangeChanged();
            _startCalendarButton.Click += (s, e) => ShowCalendar(true);
            _endCalendarButton.Click += (s, e) => ShowCalendar(false);
        }

        private void ShowCalendar(bool isStartDate)
        {
            _isStartCalendar = isStartDate;

            // Create calendar
            var calendar = new MonthCalendar
            {
                MaxSelectionCount = 1
            };

            // Set initial date
            var currentDate = isStartDate ? StartDate : EndDate;
            if (currentDate.HasValue)
            {
                calendar.SetDate(currentDate.Value);
            }

            // Create popup form
            _calendarPopup = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                Size = calendar.Size,
                TopMost = true
            };

            _calendarPopup.Controls.Add(calendar);

            // Position popup below the button
            var button = isStartDate ? _startCalendarButton : _endCalendarButton;
            var screenPoint = button.PointToScreen(new Point(0, button.Height));
            _calendarPopup.Location = screenPoint;

            // Handle date selection
            calendar.DateSelected += (s, e) =>
            {
                if (_isStartCalendar)
                {
                    StartDate = e.Start;
                }
                else
                {
                    EndDate = e.Start;
                }
                _calendarPopup?.Close();
            };

            // Close on lost focus
            _calendarPopup.Deactivate += (s, e) => _calendarPopup?.Close();

            _calendarPopup.ShowDialog(this);
        }

        private void OnDateRangeChanged()
        {
            DateRangeChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Clears both dates
        /// </summary>
        public void Clear()
        {
            _startDateTextBox.Text = string.Empty;
            _endDateTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Sets a preset date range
        /// </summary>
        public void SetPresetRange(DateRangePreset preset)
        {
            var end = DateTime.Today;
            DateTime start;

            switch (preset)
            {
                case DateRangePreset.Today:
                    start = end;
                    break;
                case DateRangePreset.Yesterday:
                    start = end = end.AddDays(-1);
                    break;
                case DateRangePreset.Last7Days:
                    start = end.AddDays(-6);
                    break;
                case DateRangePreset.Last30Days:
                    start = end.AddDays(-29);
                    break;
                case DateRangePreset.ThisMonth:
                    start = new DateTime(end.Year, end.Month, 1);
                    break;
                case DateRangePreset.LastMonth:
                    start = new DateTime(end.Year, end.Month, 1).AddMonths(-1);
                    end = start.AddMonths(1).AddDays(-1);
                    break;
                case DateRangePreset.ThisYear:
                    start = new DateTime(end.Year, 1, 1);
                    break;
                case DateRangePreset.LastYear:
                    start = new DateTime(end.Year - 1, 1, 1);
                    end = new DateTime(end.Year - 1, 12, 31);
                    break;
                default:
                    return;
            }

            StartDate = start;
            EndDate = end;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _calendarPopup?.Dispose();
                _startDateTextBox?.Dispose();
                _endDateTextBox?.Dispose();
                _startCalendarButton?.Dispose();
                _endCalendarButton?.Dispose();
                _toLabel?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Preset date ranges for quick selection
    /// </summary>
    public enum DateRangePreset
    {
        Today,
        Yesterday,
        Last7Days,
        Last30Days,
        ThisMonth,
        LastMonth,
        ThisYear,
        LastYear
    }
}
