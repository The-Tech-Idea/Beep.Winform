using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Date Picker")]
    [Description("A comprehensive date and time picker control with format masking, calendar popup, and business application features.")]
    public class BeepDatePicker : BaseControl
    {
        #region Fields
        private TextBox _textBox;
        private BeepButton _dropdownButton;
        private string _customDateFormat;
        private DateFormatStyle _dateFormatStyle = DateFormatStyle.ShortDateTime;
        private CultureInfo _culture = CultureInfo.CurrentCulture;
        private int _padding = 2;
        private DateTime _selectedDateTime = DateTime.Now;
        private BeepPopupForm _calendarPopup;
        private BeepDatePickerView _calendarView;

        // Business application features
        private DateTime? _minDate = null;
        private DateTime? _maxDate = null;
        private bool _allowEmpty = true;
        private bool _showDropDown = true;
        private bool _readOnly = false;
        private string _validationErrorMessage = "";
        private bool _autoAdjustToBusinessDays = false;
        private bool _highlightToday = true;
        private string _dateContext = "Date Selection";
        private string[] _weekends = { "Saturday", "Sunday" };
        private List<DateTime> _excludedDates = new List<DateTime>();
        private List<DateTime> _highlightedDates = new List<DateTime>();
        private bool _showValidationIcon = true;
        private bool _autoSubmit = false;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets or gets the selected date and time as a DateTime object.")]
        public DateTime SelectedDateTime
        {
            get => _selectedDateTime;
            set
            {
                if (_selectedDateTime != value)
                {
                    DateTime oldValue = _selectedDateTime;
                    _selectedDateTime = value;
                    UpdateTextBoxFromValue();
                    Invalidate();
                    
                    // Fire events
                    SelectedDateTimeChanged?.Invoke(this, EventArgs.Empty);
                    DateChanged?.Invoke(this, new DateChangedEventArgs(oldValue, value));
                    
                    // Auto submit if enabled
                    if (_autoSubmit && value != DateTime.MinValue)
                    {
                        DateSubmitted?.Invoke(this, new DateSubmittedEventArgs(value, _dateContext));
                    }
                }
            }
        }

        /// <summary>
        /// Alias for SelectedDateTime to maintain compatibility with existing code (like ProductsView)
        /// </summary>
        [Browsable(false)]
        public DateTime Value
        {
            get => SelectedDateTime;
            set => SelectedDateTime = value;
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the selected date and time as a string.")]
        public string SelectedDate
        {
            get => _textBox?.Text ?? string.Empty;
            set
            {
                if (_textBox == null) return;
                if (string.IsNullOrWhiteSpace(value))
                {
                    if (_allowEmpty)
                    {
                        _textBox.Text = string.Empty;
                        _selectedDateTime = DateTime.MinValue;
                    }
                }
                else if (DateTime.TryParse(value, _culture, DateTimeStyles.None, out DateTime result))
                {
                    if (IsDateValid(result))
                    {
                        _selectedDateTime = result;
                        _textBox.Text = FormatDate(result);
                    }
                    else
                    {
                        ShowValidationError();
                        return;
                    }
                }
                else
                {
                    _textBox.Text = string.Empty;
                    _selectedDateTime = DateTime.MinValue;
                }
                ApplyMask();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the predefined date format style.")]
        [DefaultValue(DateFormatStyle.ShortDateTime)]
        public DateFormatStyle DateFormatStyle
        {
            get => _dateFormatStyle;
            set
            {
                _dateFormatStyle = value;
                ApplyMask();
                UpdateTextBoxFromValue();
                if (_textBox != null) _textBox.PlaceholderText = GetPlaceholderText();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets a custom date format string, used when DateFormatStyle is Custom.")]
        [DefaultValue("d")]
        public string DateFormat
        {
            get => _customDateFormat;
            set
            {
                _customDateFormat = string.IsNullOrEmpty(value) ? _culture.DateTimeFormat.ShortDatePattern : value;
                if (_dateFormatStyle == DateFormatStyle.Custom)
                {
                    ApplyMask();
                    UpdateTextBoxFromValue();
                    if (_textBox != null) _textBox.PlaceholderText = GetPlaceholderText();
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the culture for date formatting and parsing. Defaults to current culture.")]
        public CultureInfo Culture
        {
            get => _culture;
            set
            {
                if (_textBox == null) return;
                _culture = value ?? CultureInfo.CurrentCulture;
                ApplyMask();
                UpdateTextBoxFromValue();
                _textBox.PlaceholderText = GetPlaceholderText();
                Invalidate();
            }
        }

        // Business Features
        [Browsable(true)]
        [Category("Business")]
        [Description("Minimum allowed date for selection.")]
        public DateTime? MinDate
        {
            get => _minDate;
            set
            {
                _minDate = value;
                if (_calendarView != null)
                {
                    // Pass to calendar view when available
                }
                ValidateCurrentDate();
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Maximum allowed date for selection.")]
        public DateTime? MaxDate
        {
            get => _maxDate;
            set
            {
                _maxDate = value;
                if (_calendarView != null)
                {
                    // Pass to calendar view when available
                }
                ValidateCurrentDate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether empty/null dates are allowed.")]
        [DefaultValue(true)]
        public bool AllowEmpty
        {
            get => _allowEmpty;
            set => _allowEmpty = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether to show the calendar dropdown button.")]
        [DefaultValue(true)]
        public bool ShowDropDown
        {
            get => _showDropDown;
            set
            {
                _showDropDown = value;
                if (_dropdownButton != null)
                {
                    _dropdownButton.Visible = value;
                    AdjustLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Makes the date picker read-only (display only).")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                _readOnly = value;
                if (_textBox != null)
                {
                    _textBox.ReadOnly = value;
                }
                if (_dropdownButton != null)
                {
                    _dropdownButton.Enabled = !value;
                }
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Context description for this date picker (e.g., 'Due Date', 'Start Date').")]
        public string DateContext
        {
            get => _dateContext;
            set => _dateContext = value ?? "Date Selection";
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Automatically adjust selected dates to business days (skip weekends).")]
        [DefaultValue(false)]
        public bool AutoAdjustToBusinessDays
        {
            get => _autoAdjustToBusinessDays;
            set => _autoAdjustToBusinessDays = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Highlight today's date in the calendar.")]
        [DefaultValue(true)]
        public bool HighlightToday
        {
            get => _highlightToday;
            set => _highlightToday = value;
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Days of the week considered as weekends.")]
        public string[] Weekends
        {
            get => _weekends;
            set => _weekends = value ?? new[] { "Saturday", "Sunday" };
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("List of dates to exclude from selection.")]
        public List<DateTime> ExcludedDates
        {
            get => _excludedDates;
            set => _excludedDates = value ?? new List<DateTime>();
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("List of dates to highlight in the calendar.")]
        public List<DateTime> HighlightedDates
        {
            get => _highlightedDates;
            set => _highlightedDates = value ?? new List<DateTime>();
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show validation error icon when date is invalid.")]
        [DefaultValue(true)]
        public bool ShowValidationIcon
        {
            get => _showValidationIcon;
            set => _showValidationIcon = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Automatically submit date when selected from calendar.")]
        [DefaultValue(false)]
        public bool AutoSubmit
        {
            get => _autoSubmit;
            set => _autoSubmit = value;
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Current validation error message (read-only).")]
        public string ValidationErrorMessage
        {
            get => _validationErrorMessage;
            private set => _validationErrorMessage = value;
        }

        // Events
        public event EventHandler SelectedDateTimeChanged;
        public event EventHandler<DateChangedEventArgs> DateChanged;
        public event EventHandler<DateSubmittedEventArgs> DateSubmitted;
        public event EventHandler<DateValidationEventArgs> DateValidationFailed;
        public event EventHandler CalendarOpened;
        public event EventHandler CalendarClosed;
        #endregion

        #region Constructor
        public BeepDatePicker()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            BorderRadius = 3;
        }

        protected override Size DefaultSize => new Size(200, 36);

        protected override void InitLayout()
        {
            base.InitLayout();

            _customDateFormat = $"{_culture.DateTimeFormat.ShortDatePattern} HH:mm";
            BoundProperty = "SelectedDateTime";

            InitializeComponents();
            ApplyTheme();
        }
        #endregion

        #region Business Methods
        /// <summary>
        /// Configure for task due date scenario
        /// </summary>
        public void ConfigureForDueDate()
        {
            DateFormatStyle = DateFormatStyle.ShortDateTime;
            DateContext = "Due Date";
            MinDate = DateTime.Today;
            AutoAdjustToBusinessDays = true;
            ShowValidationIcon = true;
            AllowEmpty = true;
        }

        /// <summary>
        /// Configure for product creation date
        /// </summary>
        public void ConfigureForCreationDate()
        {
            DateFormatStyle = DateFormatStyle.ShortDate;
            DateContext = "Creation Date";
            MaxDate = DateTime.Today;
            ReadOnly = true;
            ShowDropDown = false;
            AllowEmpty = false;
        }

        /// <summary>
        /// Configure for event scheduling
        /// </summary>
        public void ConfigureForEventScheduling()
        {
            DateFormatStyle = DateFormatStyle.FullDateTime;
            DateContext = "Event Date";
            MinDate = DateTime.Today;
            AutoAdjustToBusinessDays = false;
            ShowValidationIcon = true;
            AutoSubmit = true;
        }

        /// <summary>
        /// Configure for birth date entry
        /// </summary>
        public void ConfigureForBirthDate()
        {
            DateFormatStyle = DateFormatStyle.ShortDate;
            DateContext = "Birth Date";
            MaxDate = DateTime.Today;
            MinDate = DateTime.Today.AddYears(-120);
            AllowEmpty = false;
            ShowValidationIcon = true;
        }

        /// <summary>
        /// Configure for appointment booking
        /// </summary>
        public void ConfigureForAppointment()
        {
            DateFormatStyle = DateFormatStyle.ShortDateTime;
            DateContext = "Appointment Date";
            MinDate = DateTime.Today;
            MaxDate = DateTime.Today.AddMonths(6);
            AutoAdjustToBusinessDays = true;
            AutoSubmit = true;
        }

        /// <summary>
        /// Add business holidays to excluded dates
        /// </summary>
        public void AddBusinessHolidays(int year)
        {
            var holidays = GetBusinessHolidays(year);
            foreach (var holiday in holidays)
            {
                if (!_excludedDates.Contains(holiday))
                {
                    _excludedDates.Add(holiday);
                }
            }
        }

        /// <summary>
        /// Set date range for business operations
        /// </summary>
        public void SetBusinessDateRange(DateTime startDate, DateTime endDate)
        {
            MinDate = startDate;
            MaxDate = endDate;
            
            if (_selectedDateTime < startDate || _selectedDateTime > endDate)
            {
                SelectedDateTime = startDate;
            }
        }

        /// <summary>
        /// Get next business day from given date
        /// </summary>
        public DateTime GetNextBusinessDay(DateTime date)
        {
            var nextDay = date.AddDays(1);
            while (IsWeekend(nextDay) || _excludedDates.Contains(nextDay.Date))
            {
                nextDay = nextDay.AddDays(1);
            }
            return nextDay;
        }

        /// <summary>
        /// Validate if a date is valid for business rules
        /// </summary>
        public bool IsDateValid(DateTime date)
        {
            // Check min/max range
            if (_minDate.HasValue && date.Date < _minDate.Value.Date)
            {
                _validationErrorMessage = $"Date must be on or after {_minDate.Value:d}";
                return false;
            }

            if (_maxDate.HasValue && date.Date > _maxDate.Value.Date)
            {
                _validationErrorMessage = $"Date must be on or before {_maxDate.Value:d}";
                return false;
            }

            // Check excluded dates
            if (_excludedDates.Contains(date.Date))
            {
                _validationErrorMessage = "This date is not available for selection";
                return false;
            }

            // Check business days if required
            if (_autoAdjustToBusinessDays && IsWeekend(date))
            {
                _validationErrorMessage = "Please select a business day (Monday-Friday)";
                return false;
            }

            _validationErrorMessage = "";
            return true;
        }

        private bool IsWeekend(DateTime date)
        {
            var dayName = date.DayOfWeek.ToString();
            return _weekends.Contains(dayName);
        }

        private List<DateTime> GetBusinessHolidays(int year)
        {
            // Common US business holidays - can be customized for different regions
            return new List<DateTime>
            {
                new DateTime(year, 1, 1),   // New Year's Day
                new DateTime(year, 7, 4),   // Independence Day
                new DateTime(year, 12, 25), // Christmas Day
                // Add more holidays as needed
            };
        }

        private void ValidateCurrentDate()
        {
            if (_selectedDateTime != DateTime.MinValue && !IsDateValid(_selectedDateTime))
            {
                DateValidationFailed?.Invoke(this, new DateValidationEventArgs(_selectedDateTime, _validationErrorMessage));
            }
        }

        private void ShowValidationError()
        {
            if (_showValidationIcon && !string.IsNullOrEmpty(_validationErrorMessage))
            {
                this.ToolTipText = _validationErrorMessage;
                this.BackColor = Color.FromArgb(255, 245, 245); // Light red background
            }
        }

        private void ClearValidationError()
        {
            this.ToolTipText = "";
            ApplyTheme(); // Reset background color
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Text = FormatDate(_selectedDateTime),
                PlaceholderText = GetPlaceholderText(),
                ReadOnly = _readOnly
            };
            _textBox.TextChanged += TextBox_TextChanged;
            _textBox.KeyPress += TextBox_KeyPress;
            _textBox.Validating += TextBox_Validating;
            _textBox.Enter += TextBox_Enter;
            _textBox.Leave += TextBox_Leave;

            _dropdownButton = new BeepButton
            {
                Text = "▼",
                HideText = true,
                ShowAllBorders = false,
                IsShadowAffectedByTheme = false,
                IsChild = true,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.Overlay,
                TextAlign = ContentAlignment.MiddleCenter,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.dropdown-select.svg",
                Visible = _showDropDown,
                Enabled = !_readOnly
            };
            _dropdownButton.Click += DropdownButton_Click;

            Controls.Add(_textBox);
            Controls.Add(_dropdownButton);

            ApplyMask();
            AdjustLayout();
        }
        #endregion

        #region Event Handlers
        private void TextBox_Enter(object sender, EventArgs e)
        {
            ClearValidationError();
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            if (_autoAdjustToBusinessDays && _selectedDateTime != DateTime.MinValue && IsWeekend(_selectedDateTime))
            {
                var nextBusinessDay = GetNextBusinessDay(_selectedDateTime.AddDays(-1));
                SelectedDateTime = new DateTime(nextBusinessDay.Year, nextBusinessDay.Month, nextBusinessDay.Day,
                    _selectedDateTime.Hour, _selectedDateTime.Minute, _selectedDateTime.Second);
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (DateTime.TryParse(_textBox.Text, _culture, DateTimeStyles.None, out DateTime result))
            {
                if (IsDateValid(result))
                {
                    _selectedDateTime = result;
                    ClearValidationError();
                }
                else
                {
                    ShowValidationError();
                }
            }
            Invalidate();
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            string mask = GetCurrentMask();
            string currentText = _textBox.Text;
            int cursorPos = _textBox.SelectionStart;

            if (!char.IsControl(e.KeyChar) && cursorPos < mask.Length)
            {
                char maskChar = mask[cursorPos];
                if (!IsValidInput(e.KeyChar, maskChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_textBox.Text))
            {
                if (_allowEmpty)
                {
                    _selectedDateTime = DateTime.MinValue;
                    ClearValidationError();
                    return;
                }
                else
                {
                    e.Cancel = true;
                    _validationErrorMessage = "Date is required";
                    ShowValidationError();
                    return;
                }
            }

            if (!DateTime.TryParse(_textBox.Text, _culture, DateTimeStyles.None, out DateTime result))
            {
                e.Cancel = true;
                _validationErrorMessage = $"Invalid date format. Expected: {GetCurrentFormat()}";
                ShowValidationError();
                MessageBox.Show(_validationErrorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBox.Text = FormatDate(_selectedDateTime);
            }
            else if (!IsDateValid(result))
            {
                e.Cancel = true;
                ShowValidationError();
                MessageBox.Show(_validationErrorMessage, "Date Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBox.Text = FormatDate(_selectedDateTime);
            }
            else
            {
                _selectedDateTime = result;
                ClearValidationError();
            }
        }

        private void DropdownButton_Click(object sender, EventArgs e)
        {
            if (_readOnly) return;
            ShowCalendarPopup();
        }
        #endregion

        #region Layout and Rendering
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLayout();
            Invalidate();
        }

        private void AdjustLayout()
        {
            if (_textBox == null || _dropdownButton == null) return;
            UpdateDrawingRect();
            GetHeight();

            int buttonWidth = _showDropDown ? (DrawingRect.Height - (_padding * 2)) : 0;
            _textBox.Location = new Point(DrawingRect.Left + _padding, DrawingRect.Top + _padding);
            _textBox.Size = new Size(DrawingRect.Width - buttonWidth - (_padding * (_showDropDown ? 3 : 2)), 
                                   DrawingRect.Height - (_padding * 2));

            if (_showDropDown)
            {
                _dropdownButton.Location = new Point(_textBox.Right + _padding, DrawingRect.Top + _padding);
                _dropdownButton.Size = new Size(buttonWidth, DrawingRect.Height - (_padding * 2));
                _dropdownButton.MaxImageSize = new Size(buttonWidth - 4, buttonWidth - 4);
            }
        }

        private void GetHeight()
        {
            _padding = BorderThickness + 2;
            if (_textBox == null) return;
            Height = _textBox.PreferredHeight + (_padding * 2);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            try
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                Region originalClip = graphics.Clip;
                graphics.SetClip(rectangle);

                // Background color based on validation state
                Color backgroundColor = !string.IsNullOrEmpty(_validationErrorMessage) 
                    ? Color.FromArgb(255, 245, 245) 
                    : _currentTheme.TextBoxBackColor;

                using (SolidBrush backgroundBrush = new SolidBrush(backgroundColor))
                {
                    graphics.FillRectangle(backgroundBrush, rectangle);
                }

                if (BorderThickness > 0)
                {
                    Color borderColor = !string.IsNullOrEmpty(_validationErrorMessage)
                        ? Color.FromArgb(220, 53, 69)
                        : _currentTheme.BorderColor;

                    using (Pen borderPen = new Pen(borderColor, BorderThickness))
                    {
                        Rectangle borderRect = rectangle;
                        borderRect.Inflate(-BorderThickness / 2, -BorderThickness / 2);
                        graphics.DrawRectangle(borderPen, borderRect);
                    }
                }

                graphics.Clip = originalClip;
            }
            catch (Exception ex)
            {
                // Log error if logging is available
            }
        }
        #endregion

        #region Popup Calendar
        private void ShowCalendarPopup()
        {
            if (_calendarPopup != null && _calendarPopup.Visible)
            {
                _calendarPopup.CloseCascade();
                return;
            }

            CalendarOpened?.Invoke(this, EventArgs.Empty);

            // Create popup form
            _calendarPopup = new BeepPopupForm
            {
                BorderThickness = 1,
                BorderRadius = this.BorderRadius,
                Size = new Size(350, 400),
                Theme = Theme
            };

            // Create custom calendar control
            _calendarView = new BeepDatePickerView
            {
                Dock = DockStyle.Fill,
                Theme = Theme
            };

            if (_selectedDateTime != DateTime.MinValue)
            {
                _calendarView.SelectedDateTime = _selectedDateTime;
            }

            _calendarView.DateTimeSelected += CalendarView_DateTimeSelected;
            _calendarView.Cancelled += CalendarView_Cancelled;
            _calendarPopup.Controls.Add(_calendarView);

            // Show popup below the control
            _calendarPopup.ShowPopup(this, BeepPopupFormPosition.Bottom);
        }

        private void CalendarView_DateTimeSelected(object sender, DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                if (IsDateValid(dateTime.Value))
                {
                    SelectedDateTime = dateTime.Value;
                    _textBox.Text = FormatDate(_selectedDateTime);
                    ClearValidationError();
                }
                else
                {
                    ShowValidationError();
                    MessageBox.Show(_validationErrorMessage, "Date Selection Error", 
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Don't close calendar if date is invalid
                }
            }
            _calendarPopup?.CloseCascade();
            CalendarClosed?.Invoke(this, EventArgs.Empty);
        }

        private void CalendarView_Cancelled(object sender, EventArgs e)
        {
            _calendarPopup?.CloseCascade();
            CalendarClosed?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Format and Masking
        public string GetCurrentFormat()
        {
            return _dateFormatStyle switch
            {
                DateFormatStyle.ShortDate => _culture.DateTimeFormat.ShortDatePattern,
                DateFormatStyle.LongDate => _culture.DateTimeFormat.LongDatePattern,
                DateFormatStyle.YearMonth => "MMMM yyyy",
                DateFormatStyle.Custom => _customDateFormat,
                DateFormatStyle.FullDateTime => $"{_culture.DateTimeFormat.LongDatePattern} {_culture.DateTimeFormat.LongTimePattern}",
                DateFormatStyle.ShortDateTime => $"{_culture.DateTimeFormat.ShortDatePattern} HH:mm",
                DateFormatStyle.DayMonthYear => "dd MMMM yyyy",
                DateFormatStyle.ISODate => "yyyy-MM-dd",
                DateFormatStyle.ISODateTime => "yyyy-MM-dd HH:mm:ss",
                DateFormatStyle.TimeOnly => "HH:mm:ss",
                DateFormatStyle.ShortTime => "HH:mm",
                DateFormatStyle.MonthDay => "MMMM dd",
                DateFormatStyle.DayOfWeek => "dddd",
                DateFormatStyle.RFC1123 => "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                DateFormatStyle.UniversalSortable => "yyyy-MM-dd HH:mm:ss'Z'",
                _ => $"{_culture.DateTimeFormat.ShortDatePattern} HH:mm"
            };
        }

        private string GetCurrentMask()
        {
            return _dateFormatStyle switch
            {
                DateFormatStyle.ShortDate => ConvertFormatToMask(_culture.DateTimeFormat.ShortDatePattern),
                DateFormatStyle.LongDate => _culture.DateTimeFormat.LongDatePattern,
                DateFormatStyle.YearMonth => "MMMM yyyy",
                DateFormatStyle.Custom => ConvertFormatToMask(_customDateFormat),
                DateFormatStyle.FullDateTime => ConvertFormatToMask($"{_culture.DateTimeFormat.LongDatePattern} HH:mm:ss"),
                DateFormatStyle.ShortDateTime => ConvertFormatToMask($"{_culture.DateTimeFormat.ShortDatePattern} HH:mm"),
                DateFormatStyle.DayMonthYear => "00 MMMM yyyy",
                DateFormatStyle.ISODate => "0000-00-00",
                DateFormatStyle.ISODateTime => "0000-00-00 00:00:00",
                DateFormatStyle.TimeOnly => "00:00:00",
                DateFormatStyle.ShortTime => "00:00",
                DateFormatStyle.MonthDay => "MMMM 00",
                DateFormatStyle.DayOfWeek => "dddd",
                DateFormatStyle.RFC1123 => "ddd, 00 MMM yyyy 00:00:00 GMT",
                DateFormatStyle.UniversalSortable => "0000-00-00 00:00:00Z",
                _ => ConvertFormatToMask($"{_culture.DateTimeFormat.ShortDatePattern} HH:mm")
            };
        }

        private string ConvertFormatToMask(string format)
        {
            return Regex.Replace(format,
                @"[dMyHhmsf]+",
                match => new string(match.Value[0] switch
                {
                    'd' => '0',
                    'M' => '0',
                    'y' => '0',
                    'H' => '0',
                    'h' => '0',
                    'm' => '0',
                    's' => '0',
                    'f' => '0',
                    _ => match.Value[0]
                }, match.Length));
        }

        private string GetPlaceholderText()
        {
            string format = GetCurrentFormat();
            return Regex.Replace(format, @"[dMyHhmsf]+", match => match.Value[0] switch
            {
                'd' => "DD",
                'M' => "MM",
                'y' => "YYYY",
                'H' or 'h' => "HH",
                'm' => "MM",
                's' => "SS",
                'f' => "FF",
                _ => match.Value
            });
        }

        private bool IsValidInput(char input, char maskChar)
        {
            return maskChar switch
            {
                '0' => char.IsDigit(input),
                'd' or 'M' or 'y' or 'H' or 'h' or 'm' or 's' => char.IsLetterOrDigit(input),
                _ => true // Allow separators like /, -, :, space
            };
        }

        private void ApplyMask()
        {
            if (_textBox == null) return;
            string mask = GetCurrentMask();
            if (!Regex.IsMatch(mask, @"^[dMyHmsf]+$")) // If mask contains digits
            {
                _textBox.MaxLength = mask.Length;
            }
            else
            {
                _textBox.MaxLength = 50; // Arbitrary max for free-form text
            }
        }

        private void UpdateTextBoxFromValue()
        {
            if (_textBox == null) return;
            if (_selectedDateTime != DateTime.MinValue)
            {
                _textBox.Text = FormatDate(_selectedDateTime);
            }
            else
            {
                _textBox.Text = string.Empty;
            }
        }

        private string FormatDate(DateTime date)
        {
            return date.ToString(GetCurrentFormat(), _culture);
        }
        #endregion

        #region Theme and Value Management
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_textBox != null)
            {
                _textBox.BackColor = _currentTheme.TextBoxBackColor;
                _textBox.ForeColor = _currentTheme.TextBoxForeColor;
            }
            if (_dropdownButton != null)
            {
                _dropdownButton.Theme = Theme;
            }
            if (_calendarView != null)
            {
                _calendarView.Theme = Theme;
            }
            Invalidate();
        }

        public override void SetValue(object value)
        {
            if (value is DateTime dt)
            {
                SelectedDateTime = dt;
            }
            else if (value is string str && DateTime.TryParse(str, out DateTime parsed))
            {
                SelectedDateTime = parsed;
            }
            else
            {
                SelectedDate = value?.ToString();
            }
        }

        public override object GetValue()
        {
            return _selectedDateTime == DateTime.MinValue ? null : _selectedDateTime;
        }

        public void Reset()
        {
            SelectedDateTime = DateTime.Now;
        }

        /// <summary>
        /// Clear the selected date
        /// </summary>
        public void Clear()
        {
            if (_allowEmpty)
            {
                SelectedDateTime = DateTime.MinValue;
                _textBox.Text = "";
                ClearValidationError();
            }
        }
        #endregion

        #region Disposal
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textBox?.Dispose();
                _dropdownButton?.Dispose();
                _calendarPopup?.Dispose();
                _calendarView?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    #region Event Args
    public class DateChangedEventArgs : EventArgs
    {
        public DateTime OldDate { get; }
        public DateTime NewDate { get; }

        public DateChangedEventArgs(DateTime oldDate, DateTime newDate)
        {
            OldDate = oldDate;
            NewDate = newDate;
        }
    }

    public class DateSubmittedEventArgs : EventArgs
    {
        public DateTime SelectedDate { get; }
        public string Context { get; }

        public DateSubmittedEventArgs(DateTime selectedDate, string context)
        {
            SelectedDate = selectedDate;
            Context = context;
        }
    }

    public class DateValidationEventArgs : EventArgs
    {
        public DateTime InvalidDate { get; }
        public string ErrorMessage { get; }

        public DateValidationEventArgs(DateTime invalidDate, string errorMessage)
        {
            InvalidDate = invalidDate;
            ErrorMessage = errorMessage;
        }
    }
    #endregion
}