using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
 
 
 

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum TimeFormatStyle
    {
        TwelveHour,      // 12:30 PM
        TwentyFourHour,  // 12:30
        TwelveHourWithSeconds,  // 12:30:45 PM
        TwentyFourHourWithSeconds, // 12:30:45
        HoursOnly,       // 12 PM
        Custom           // User-defined format
    }

    /// <summary>
    /// A modern time picker control with multiple formats, validation, and business application features
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep Time Picker")]
    [Category("Beep Controls")]
    [Description("Modern time picker control with multiple formats, validation, and business features")]
    public class BeepTimePicker : BeepControl
    {
        #region Fields
        private TextBox _textBox;
        private BeepButton _dropdownButton;
        private BeepButton _incrementButton;
        private BeepButton _decrementButton;
        private string _customTimeFormat = "HH:mm";
        private TimeFormatStyle _timeFormatStyle = TimeFormatStyle.TwentyFourHour;
        private CultureInfo _culture = CultureInfo.CurrentCulture;
        private int _padding = 2;
        private TimeSpan _selectedTime = DateTime.Now.TimeOfDay;
        private BeepPopupForm _timePopup;
        private BeepPanel _timePickerPanel;

        // Business application features
        private TimeSpan? _minTime = null;
        private TimeSpan? _maxTime = null;
        private bool _allowEmpty = true;
        private bool _showDropDown = true;
        private bool _showSpinButtons = false;
        private bool _readOnly = false;
        private string _validationErrorMessage = "";
        private bool _autoAdjustToBusinessHours = false;
        private string _timeContext = "Time Selection";
        private TimeSpan _businessStartTime = new TimeSpan(9, 0, 0);  // 9:00 AM
        private TimeSpan _businessEndTime = new TimeSpan(17, 0, 0);   // 5:00 PM
        private int _minuteInterval = 1;
        private bool _snapToInterval = false;
        private bool _showValidationIcon = true;
        private bool _autoSubmit = false;
        private bool _use24HourFormat = true;
        private bool _allowSecondsEdit = false;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Time Settings")]
        [Description("Gets or sets the selected time as a TimeSpan object")]
        public TimeSpan SelectedTime
        {
            get => _selectedTime;
            set
            {
                if (_selectedTime != value)
                {
                    TimeSpan oldValue = _selectedTime;
                    _selectedTime = value;
                    
                    // Snap to interval if enabled
                    if (_snapToInterval && _minuteInterval > 1)
                    {
                        _selectedTime = SnapTimeToInterval(_selectedTime);
                    }
                    
                    UpdateTextBoxFromValue();
                    Invalidate();
                    
                    // Fire events
                    SelectedTimeChanged?.Invoke(this, EventArgs.Empty);
                    TimeChanged?.Invoke(this, new TimeChangedEventArgs(oldValue, value));
                    
                    // Auto submit if enabled
                    if (_autoSubmit)
                    {
                        TimeSubmitted?.Invoke(this, new TimeSubmittedEventArgs(value, _timeContext));
                    }
                }
            }
        }

        /// <summary>
        /// Alias for SelectedTime to maintain compatibility
        /// </summary>
        [Browsable(false)]
        public TimeSpan Value
        {
            get => SelectedTime;
            set => SelectedTime = value;
        }

        [Browsable(true)]
        [Category("Time Settings")]
        [Description("Gets or sets the selected time as a string")]
        public string SelectedTimeString
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
                        _selectedTime = TimeSpan.Zero;
                    }
                }
                else if (TryParseTime(value, out TimeSpan result))
                {
                    if (IsTimeValid(result))
                    {
                        _selectedTime = result;
                        _textBox.Text = FormatTime(result);
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
                    _selectedTime = TimeSpan.Zero;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Time Settings")]
        [Description("Sets the predefined time format Style")]
        [DefaultValue(TimeFormatStyle.TwentyFourHour)]
        public TimeFormatStyle TimeFormatStyle
        {
            get => _timeFormatStyle;
            set
            {
                _timeFormatStyle = value;
                UpdateTextBoxFromValue();
                if (_textBox != null) _textBox.PlaceholderText = GetPlaceholderText();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Time Settings")]
        [Description("Custom time format string, used when TimeFormatStyle is Custom")]
        [DefaultValue("HH:mm")]
        public string TimeFormat
        {
            get => _customTimeFormat;
            set
            {
                _customTimeFormat = string.IsNullOrEmpty(value) ? "HH:mm" : value;
                if (_timeFormatStyle == TimeFormatStyle.Custom)
                {
                    UpdateTextBoxFromValue();
                    if (_textBox != null) _textBox.PlaceholderText = GetPlaceholderText();
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Time Settings")]
        [Description("Culture for time formatting and parsing")]
        public CultureInfo Culture
        {
            get => _culture;
            set
            {
                _culture = value ?? CultureInfo.CurrentCulture;
                UpdateTextBoxFromValue();
                if (_textBox != null) _textBox.PlaceholderText = GetPlaceholderText();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Minimum allowed time for selection")]
        public TimeSpan? MinTime
        {
            get => _minTime;
            set
            {
                _minTime = value;
                ValidateCurrentTime();
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Maximum allowed time for selection")]
        public TimeSpan? MaxTime
        {
            get => _maxTime;
            set
            {
                _maxTime = value;
                ValidateCurrentTime();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether empty/null times are allowed")]
        [DefaultValue(true)]
        public bool AllowEmpty
        {
            get => _allowEmpty;
            set => _allowEmpty = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether to show the time picker dropdown button")]
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
        [Category("Appearance")]
        [Description("Whether to show spin up/down buttons")]
        [DefaultValue(false)]
        public bool ShowSpinButtons
        {
            get => _showSpinButtons;
            set
            {
                _showSpinButtons = value;
                if (_incrementButton != null && _decrementButton != null)
                {
                    _incrementButton.Visible = value;
                    _decrementButton.Visible = value;
                    AdjustLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Makes the time picker read-only")]
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
                if (_incrementButton != null && _decrementButton != null)
                {
                    _incrementButton.Enabled = !value;
                    _decrementButton.Enabled = !value;
                }
            }
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Context description for this time picker")]
        public string TimeContext
        {
            get => _timeContext;
            set => _timeContext = value ?? "Time Selection";
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Automatically adjust times to business hours")]
        [DefaultValue(false)]
        public bool AutoAdjustToBusinessHours
        {
            get => _autoAdjustToBusinessHours;
            set => _autoAdjustToBusinessHours = value;
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Business day start time")]
        public TimeSpan BusinessStartTime
        {
            get => _businessStartTime;
            set => _businessStartTime = value;
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Business day end time")]
        public TimeSpan BusinessEndTime
        {
            get => _businessEndTime;
            set => _businessEndTime = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Minute interval for time selection")]
        [DefaultValue(1)]
        public int MinuteInterval
        {
            get => _minuteInterval;
            set
            {
                _minuteInterval = Math.Max(1, Math.Min(60, value));
                if (_snapToInterval)
                {
                    SelectedTime = SnapTimeToInterval(_selectedTime);
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Snap time selection to minute intervals")]
        [DefaultValue(false)]
        public bool SnapToInterval
        {
            get => _snapToInterval;
            set
            {
                _snapToInterval = value;
                if (value)
                {
                    SelectedTime = SnapTimeToInterval(_selectedTime);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show validation error icon when time is invalid")]
        [DefaultValue(true)]
        public bool ShowValidationIcon
        {
            get => _showValidationIcon;
            set => _showValidationIcon = value;
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Automatically submit time when selected")]
        [DefaultValue(false)]
        public bool AutoSubmit
        {
            get => _autoSubmit;
            set => _autoSubmit = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use 24-hour format display")]
        [DefaultValue(true)]
        public bool Use24HourFormat
        {
            get => _use24HourFormat;
            set
            {
                _use24HourFormat = value;
                UpdateTextBoxFromValue();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow editing of seconds")]
        [DefaultValue(false)]
        public bool AllowSecondsEdit
        {
            get => _allowSecondsEdit;
            set => _allowSecondsEdit = value;
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Current validation error message (read-only)")]
        public string ValidationErrorMessage
        {
            get => _validationErrorMessage;
            private set => _validationErrorMessage = value;
        }

        // Events
        public event EventHandler SelectedTimeChanged;
        public event EventHandler<TimeChangedEventArgs> TimeChanged;
        public event EventHandler<TimeSubmittedEventArgs> TimeSubmitted;
        public event EventHandler<TimeValidationEventArgs> TimeValidationFailed;
        public event EventHandler TimePickerOpened;
        public event EventHandler TimePickerClosed;
        #endregion

        #region Constructor
        public BeepTimePicker()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            BorderRadius = 3;
        }

        protected override Size DefaultSize => new Size(150, 36);

        protected override void InitLayout()
        {
            base.InitLayout();
            BoundProperty = "SelectedTime";
            InitializeComponents();
            ApplyTheme();
        }
        #endregion

        #region Business Methods
        /// <summary>
        /// Configure for meeting times
        /// </summary>
        public void ConfigureForMeetingTime()
        {
            TimeFormatStyle = TimeFormatStyle.TwelveHour;
            TimeContext = "Meeting Time";
            AutoAdjustToBusinessHours = true;
            MinuteInterval = 15;
            SnapToInterval = true;
            ShowValidationIcon = true;
        }

        /// <summary>
        /// Configure for appointment scheduling
        /// </summary>
        public void ConfigureForAppointment()
        {
            TimeFormatStyle = TimeFormatStyle.TwelveHour;
            TimeContext = "Appointment Time";
            BusinessStartTime = new TimeSpan(8, 0, 0);
            BusinessEndTime = new TimeSpan(18, 0, 0);
            AutoAdjustToBusinessHours = true;
            MinuteInterval = 30;
            SnapToInterval = true;
            AutoSubmit = true;
        }

        /// <summary>
        /// Configure for shift scheduling
        /// </summary>
        public void ConfigureForShiftTime()
        {
            TimeFormatStyle = TimeFormatStyle.TwentyFourHour;
            TimeContext = "Shift Time";
            MinuteInterval = 15;
            SnapToInterval = true;
            AllowEmpty = false;
            ShowValidationIcon = true;
        }

        /// <summary>
        /// Configure for deadline times
        /// </summary>
        public void ConfigureForDeadline()
        {
            TimeFormatStyle = TimeFormatStyle.TwelveHour;
            TimeContext = "Deadline";
            AutoSubmit = true;
            ShowValidationIcon = true;
        }

        /// <summary>
        /// Validate if a time is valid for business rules
        /// </summary>
        public bool IsTimeValid(TimeSpan time)
        {
            // Check min/max range
            if (_minTime.HasValue && time < _minTime.Value)
            {
                _validationErrorMessage = $"Time must be after {FormatTime(_minTime.Value)}";
                return false;
            }

            if (_maxTime.HasValue && time > _maxTime.Value)
            {
                _validationErrorMessage = $"Time must be before {FormatTime(_maxTime.Value)}";
                return false;
            }

            // Check business hours if required
            if (_autoAdjustToBusinessHours && (time < _businessStartTime || time > _businessEndTime))
            {
                _validationErrorMessage = $"Please select a time between {FormatTime(_businessStartTime)} and {FormatTime(_businessEndTime)}";
                return false;
            }

            _validationErrorMessage = "";
            return true;
        }

        /// <summary>
        /// Snap time to the configured interval
        /// </summary>
        public TimeSpan SnapTimeToInterval(TimeSpan time)
        {
            if (_minuteInterval <= 1) return time;
            
            int totalMinutes = (int)time.TotalMinutes;
            int snappedMinutes = (totalMinutes / _minuteInterval) * _minuteInterval;
            
            return TimeSpan.FromMinutes(snappedMinutes);
        }

        /// <summary>
        /// Get next valid business time
        /// </summary>
        public TimeSpan GetNextBusinessTime(TimeSpan time)
        {
            if (time < _businessStartTime)
                return _businessStartTime;
            
            if (time > _businessEndTime)
                return _businessStartTime; // Next day start
            
            return time;
        }

        private void ValidateCurrentTime()
        {
            if (_selectedTime != TimeSpan.Zero && !IsTimeValid(_selectedTime))
            {
                TimeValidationFailed?.Invoke(this, new TimeValidationEventArgs(_selectedTime, _validationErrorMessage));
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
                Text = FormatTime(_selectedTime),
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
                Text = "🕒",
                HideText = true,
                ShowAllBorders = false,
                IsShadowAffectedByTheme = false,
                IsChild = true,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.Overlay,
                TextAlign = ContentAlignment.MiddleCenter,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.clock.svg",
                Visible = _showDropDown,
                Enabled = !_readOnly
            };
            _dropdownButton.Click += DropdownButton_Click;

            _incrementButton = new BeepButton
            {
                Text = "▲",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ShowAllBorders = false,
                IsShadowAffectedByTheme = false,
                IsChild = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = _showSpinButtons,
                Enabled = !_readOnly
            };
            _incrementButton.Click += IncrementButton_Click;

            _decrementButton = new BeepButton
            {
                Text = "▼",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ShowAllBorders = false,
                IsShadowAffectedByTheme = false,
                IsChild = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = _showSpinButtons,
                Enabled = !_readOnly
            };
            _decrementButton.Click += DecrementButton_Click;

            Controls.Add(_textBox);
            Controls.Add(_dropdownButton);
            Controls.Add(_incrementButton);
            Controls.Add(_decrementButton);

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
            if (_autoAdjustToBusinessHours && _selectedTime != TimeSpan.Zero)
            {
                var adjustedTime = GetNextBusinessTime(_selectedTime);
                if (adjustedTime != _selectedTime)
                {
                    SelectedTime = adjustedTime;
                }
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (TryParseTime(_textBox.Text, out TimeSpan result))
            {
                if (IsTimeValid(result))
                {
                    _selectedTime = result;
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
            // Allow control keys, digits, and time separators
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && 
                e.KeyChar != ':' && e.KeyChar != ' ' && e.KeyChar != 'A' && 
                e.KeyChar != 'M' && e.KeyChar != 'P' && e.KeyChar != 'a' && 
                e.KeyChar != 'm' && e.KeyChar != 'p')
            {
                e.Handled = true;
            }
        }

        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_textBox.Text))
            {
                if (_allowEmpty)
                {
                    _selectedTime = TimeSpan.Zero;
                    ClearValidationError();
                    return;
                }
                else
                {
                    e.Cancel = true;
                    _validationErrorMessage = "Time is required";
                    ShowValidationError();
                    return;
                }
            }

            if (!TryParseTime(_textBox.Text, out TimeSpan result))
            {
                e.Cancel = true;
                _validationErrorMessage = $"Invalid time format. Expected: {GetPlaceholderText()}";
                ShowValidationError();
                MessageBox.Show(_validationErrorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBox.Text = FormatTime(_selectedTime);
            }
            else if (!IsTimeValid(result))
            {
                e.Cancel = true;
                ShowValidationError();
                MessageBox.Show(_validationErrorMessage, "Time Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBox.Text = FormatTime(_selectedTime);
            }
            else
            {
                _selectedTime = result;
                ClearValidationError();
            }
        }

        private void DropdownButton_Click(object sender, EventArgs e)
        {
            if (_readOnly) return;
            ShowTimePickerPopup();
        }

        private void IncrementButton_Click(object sender, EventArgs e)
        {
            if (_readOnly) return;
            var newTime = _selectedTime.Add(TimeSpan.FromMinutes(_minuteInterval));
            if (newTime.TotalDays >= 1)
                newTime = new TimeSpan(23, 59, 0);
            
            if (IsTimeValid(newTime))
                SelectedTime = newTime;
        }

        private void DecrementButton_Click(object sender, EventArgs e)
        {
            if (_readOnly) return;
            var newTime = _selectedTime.Subtract(TimeSpan.FromMinutes(_minuteInterval));
            if (newTime < TimeSpan.Zero)
                newTime = TimeSpan.Zero;
            
            if (IsTimeValid(newTime))
                SelectedTime = newTime;
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
            int spinButtonWidth = _showSpinButtons ? 16 : 0;
            int totalButtonWidth = buttonWidth + spinButtonWidth;

            _textBox.Location = new Point(DrawingRect.Left + _padding, DrawingRect.Top + _padding);
            _textBox.Size = new Size(DrawingRect.Width - totalButtonWidth - (_padding * (totalButtonWidth > 0 ? 3 : 2)), 
                                   DrawingRect.Height - (_padding * 2));

            int currentX = _textBox.Right + _padding;

            if (_showSpinButtons)
            {
                int spinButtonHeight = (DrawingRect.Height - (_padding * 2)) / 2;
                _incrementButton.Location = new Point(currentX, DrawingRect.Top + _padding);
                _incrementButton.Size = new Size(spinButtonWidth, spinButtonHeight);
                
                _decrementButton.Location = new Point(currentX, DrawingRect.Top + _padding + spinButtonHeight);
                _decrementButton.Size = new Size(spinButtonWidth, spinButtonHeight);
                
                currentX += spinButtonWidth + _padding;
            }

            if (_showDropDown)
            {
                _dropdownButton.Location = new Point(currentX, DrawingRect.Top + _padding);
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

        /// <summary>
        /// DrawContent override - called by BaseControl
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            Paint(g, DrawingRect);
        }

        /// <summary>
        /// Draw override - called by BeepGridPro and containers
        /// </summary>
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            Paint(graphics, rectangle);
        }

        /// <summary>
        /// Main paint function - centralized painting logic
        /// Called from both DrawContent and Draw
        /// </summary>
        private void Paint(Graphics g, Rectangle bounds)
        {
            try
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Background color based on validation state
                Color backgroundColor = !string.IsNullOrEmpty(_validationErrorMessage) 
                    ? Color.FromArgb(255, 245, 245) 
                    : _currentTheme.TextBoxBackColor;

                using (SolidBrush backgroundBrush = new SolidBrush(backgroundColor))
                {
                    g.FillRectangle(backgroundBrush, bounds);
                }

                if (BorderThickness > 0)
                {
                    Color borderColor = !string.IsNullOrEmpty(_validationErrorMessage)
                        ? Color.FromArgb(220, 53, 69)
                        : _currentTheme.BorderColor;

                    using (Pen borderPen = new Pen(borderColor, BorderThickness))
                    {
                        Rectangle borderRect = bounds;
                        borderRect.Inflate(-BorderThickness / 2, -BorderThickness / 2);
                        g.DrawRectangle(borderPen, borderRect);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error if logging is available
            }
        }
        #endregion

        #region Time Picker Popup
        private void ShowTimePickerPopup()
        {
            if (_timePopup != null && _timePopup.Visible)
            {
                _timePopup.CloseCascade();
                return;
            }

            TimePickerOpened?.Invoke(this, EventArgs.Empty);

            _timePopup = new BeepPopupForm
            {
               
                Size = new Size(250, 300),
                Theme = Theme
            };

            _timePickerPanel = CreateTimePickerPanel();
            _timePopup.Controls.Add(_timePickerPanel);

            _timePopup.ShowPopup(this, BeepPopupFormPosition.Bottom);
        }

        private BeepPanel CreateTimePickerPanel()
        {
            var panel = new BeepPanel
            {
                Dock = DockStyle.Fill,
                ShowTitle = false,
                Theme = Theme
            };

            // Create time selection lists
            var hoursListBox = new ListBox
            {
                Location = new Point(10, 10),
                Size = new Size(60, 200),
                Font = new Font("Segoe UI", 11)
            };

            var minutesListBox = new ListBox
            {
                Location = new Point(80, 10),
                Size = new Size(60, 200),
                Font = new Font("Segoe UI", 11)
            };

            var ampmListBox = new ListBox
            {
                Location = new Point(150, 10),
                Size = new Size(60, 200),
                Font = new Font("Segoe UI", 11),
                Visible = !_use24HourFormat
            };

            // Populate hours
            if (_use24HourFormat)
            {
                for (int i = 0; i < 24; i++)
                {
                    hoursListBox.Items.Add(i.ToString("00"));
                }
            }
            else
            {
                for (int i = 1; i <= 12; i++)
                {
                    hoursListBox.Items.Add(i.ToString());
                }
            }

            // Populate minutes
            for (int i = 0; i < 60; i += _minuteInterval)
            {
                minutesListBox.Items.Add(i.ToString("00"));
            }

            // Populate AM/PM
            if (!_use24HourFormat)
            {
                ampmListBox.Items.Add("AM");
                ampmListBox.Items.Add("PM");
            }

            // Set current selection
            try
            {
                if (_use24HourFormat)
                {
                    hoursListBox.SelectedIndex = _selectedTime.Hours;
                }
                else
                {
                    int hour12 = _selectedTime.Hours == 0 ? 12 : (_selectedTime.Hours > 12 ? _selectedTime.Hours - 12 : _selectedTime.Hours);
                    hoursListBox.SelectedIndex = hour12 - 1;
                    ampmListBox.SelectedIndex = _selectedTime.Hours >= 12 ? 1 : 0;
                }
                
                minutesListBox.SelectedIndex = _selectedTime.Minutes / _minuteInterval;
            }
            catch { }

            // Handle selection changes
            EventHandler selectionChanged = (s, e) => {
                try
                {
                    int hours = 0;
                    int minutes = 0;

                    if (_use24HourFormat)
                    {
                        hours = hoursListBox.SelectedIndex;
                    }
                    else
                    {
                        int hour12 = hoursListBox.SelectedIndex + 1;
                        bool isPM = ampmListBox.SelectedIndex == 1;
                        hours = hour12 == 12 ? (isPM ? 12 : 0) : (isPM ? hour12 + 12 : hour12);
                    }

                    minutes = minutesListBox.SelectedIndex * _minuteInterval;
                    
                    var newTime = new TimeSpan(hours, minutes, 0);
                    if (IsTimeValid(newTime))
                    {
                        SelectedTime = newTime;
                    }
                }
                catch { }
            };

            hoursListBox.SelectedIndexChanged += selectionChanged;
            minutesListBox.SelectedIndexChanged += selectionChanged;
            if (!_use24HourFormat)
                ampmListBox.SelectedIndexChanged += selectionChanged;

            // OK/Cancel buttons
            var okButton = new BeepButton
            {
                Text = "OK",
                Location = new Point(10, 220),
                Size = new Size(60, 30),
                Theme = Theme
            };
            okButton.Click += (s, e) => {
                _timePopup?.CloseCascade();
                TimePickerClosed?.Invoke(this, EventArgs.Empty);
            };

            var cancelButton = new BeepButton
            {
                Text = "Cancel",
                Location = new Point(80, 220),
                Size = new Size(60, 30),
                Theme = Theme
            };
            cancelButton.Click += (s, e) => {
                _timePopup?.CloseCascade();
                TimePickerClosed?.Invoke(this, EventArgs.Empty);
            };

            panel.Controls.AddRange(new Control[] 
            { 
                hoursListBox, minutesListBox, ampmListBox, okButton, cancelButton 
            });

            return panel;
        }
        #endregion

        #region Format and Parsing
        public string GetCurrentFormat()
        {
            return _timeFormatStyle switch
            {
                TimeFormatStyle.TwelveHour => "h:mm tt",
                TimeFormatStyle.TwentyFourHour => "HH:mm",
                TimeFormatStyle.TwelveHourWithSeconds => "h:mm:ss tt",
                TimeFormatStyle.TwentyFourHourWithSeconds => "HH:mm:ss",
                TimeFormatStyle.HoursOnly => _use24HourFormat ? "HH" : "h tt",
                TimeFormatStyle.Custom => _customTimeFormat,
                _ => "HH:mm"
            };
        }

        private string GetPlaceholderText()
        {
            return _timeFormatStyle switch
            {
                TimeFormatStyle.TwelveHour => "12:30 PM",
                TimeFormatStyle.TwentyFourHour => "12:30",
                TimeFormatStyle.TwelveHourWithSeconds => "12:30:45 PM",
                TimeFormatStyle.TwentyFourHourWithSeconds => "12:30:45",
                TimeFormatStyle.HoursOnly => _use24HourFormat ? "12" : "12 PM",
                TimeFormatStyle.Custom => "Custom Format",
                _ => "12:30"
            };
        }

        private void UpdateTextBoxFromValue()
        {
            if (_textBox == null) return;
            if (_selectedTime != TimeSpan.Zero)
            {
                _textBox.Text = FormatTime(_selectedTime);
            }
            else
            {
                _textBox.Text = string.Empty;
            }
        }

        private string FormatTime(TimeSpan time)
        {
            var dateTime = DateTime.Today.Add(time);
            return dateTime.ToString(GetCurrentFormat(), _culture);
        }

        private bool TryParseTime(string timeString, out TimeSpan result)
        {
            result = TimeSpan.Zero;
            
            if (string.IsNullOrWhiteSpace(timeString))
                return false;

            // Try parsing as DateTime first
            if (DateTime.TryParse(timeString, _culture, DateTimeStyles.None, out DateTime dateTime))
            {
                result = dateTime.TimeOfDay;
                return true;
            }

            // Try parsing as TimeSpan
            if (TimeSpan.TryParse(timeString, _culture, out result))
            {
                return true;
            }

            return false;
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
            if (_incrementButton != null && _decrementButton != null)
            {
                _incrementButton.Theme = Theme;
                _decrementButton.Theme = Theme;
            }
            if (_timePickerPanel != null)
            {
                _timePickerPanel.Theme = Theme;
            }
            Invalidate();
        }

        public override void SetValue(object value)
        {
            if (value is TimeSpan ts)
            {
                SelectedTime = ts;
            }
            else if (value is DateTime dt)
            {
                SelectedTime = dt.TimeOfDay;
            }
            else if (value is string str && TryParseTime(str, out TimeSpan parsed))
            {
                SelectedTime = parsed;
            }
            else
            {
                SelectedTimeString = value?.ToString();
            }
        }

        public override object GetValue()
        {
            return _selectedTime == TimeSpan.Zero ? null : _selectedTime;
        }

        public void Reset()
        {
            SelectedTime = DateTime.Now.TimeOfDay;
        }

        public void Clear()
        {
            if (_allowEmpty)
            {
                SelectedTime = TimeSpan.Zero;
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
                _incrementButton?.Dispose();
                _decrementButton?.Dispose();
                _timePopup?.Dispose();
                _timePickerPanel?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    #region Event Args
    public class TimeChangedEventArgs : EventArgs
    {
        public TimeSpan OldTime { get; }
        public TimeSpan NewTime { get; }

        public TimeChangedEventArgs(TimeSpan oldTime, TimeSpan newTime)
        {
            OldTime = oldTime;
            NewTime = newTime;
        }
    }

    public class TimeSubmittedEventArgs : EventArgs
    {
        public TimeSpan SelectedTime { get; }
        public string Context { get; }

        public TimeSubmittedEventArgs(TimeSpan selectedTime, string context)
        {
            SelectedTime = selectedTime;
            Context = context;
        }
    }

    public class TimeValidationEventArgs : EventArgs
    {
        public TimeSpan InvalidTime { get; }
        public string ErrorMessage { get; }

        public TimeValidationEventArgs(TimeSpan invalidTime, string errorMessage)
        {
            InvalidTime = invalidTime;
            ErrorMessage = errorMessage;
        }
    }
    #endregion
}
