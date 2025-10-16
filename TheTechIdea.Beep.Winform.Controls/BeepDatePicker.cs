using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
 
using TheTechIdea.Beep.Vis.Modules;
 
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Date Picker")]
    [Description("A comprehensive date and time picker control with format masking, calendar popup, and business application features.")]
    public class BeepDatePicker : BaseControl
    {
        #region Fields
        // Visual/layout fields (no child TextBox/Button)
        private string _customDateFormat;
        private DateFormatStyle _dateFormatStyle = DateFormatStyle.ShortDateTime;
        private CultureInfo _culture = CultureInfo.CurrentCulture;
        private DateTime _selectedDateTime = DateTime.Now;
        private BeepPopupForm _calendarPopup;
        private BeepDatePickerView _calendarView;
        private string _inputText = string.Empty;
        private bool _isEditing = false;
        private bool _isPopupOpen = false;
        private Font _textFont;
        private BeepImage calendarIcon;
        // Constants - framework handles DPI scaling
        private int _buttonWidth => 24;
        private int _padding => 3;

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
        private bool _autoHeight = true;
        private bool _autoSize = false;
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
                    _inputText = _selectedDateTime == DateTime.MinValue ? string.Empty : FormatDate(_selectedDateTime);
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
            get
            {
                if (_isEditing) return _inputText;
                return _selectedDateTime != DateTime.MinValue ? FormatDate(_selectedDateTime) : string.Empty;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    if (_allowEmpty)
                    {
                        _inputText = string.Empty;
                        _selectedDateTime = DateTime.MinValue;
                    }
                }
                else if (DateTime.TryParse(value, _culture, DateTimeStyles.None, out DateTime result))
                {
                    if (IsDateValid(result))
                    {
                        _selectedDateTime = result;
                        _inputText = FormatDate(result);
                    }
                    else
                    {
                        _inputText = value;
                        ShowValidationError();
                        return;
                    }
                }
                else
                {
                    _inputText = value;
                    _selectedDateTime = DateTime.MinValue;
                }
                UpdateMinimumSize();
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
                _inputText = SelectedDate;
                UpdateMinimumSize(); // Recalculate size when format changes
                
                // Apply size compensation if Material Design is enabled
                if (PainterKind == BaseControlPainterKind.Material && DatePickerAutoSizeForMaterial)
                {
                    ApplyMaterialSizeCompensation();
                }
                
                if (_autoSize)
                {
                    Size pref = GetPreferredSize(Size.Empty);
                    Width = pref.Width;
                }
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
                    _inputText = SelectedDate;
                    UpdateMinimumSize(); // Recalculate size when custom format changes
                    
                    // Apply size compensation if Material Design is enabled
                    if (PainterKind == BaseControlPainterKind.Material && DatePickerAutoSizeForMaterial)
                    {
                        ApplyMaterialSizeCompensation();
                    }
                    
                    if (_autoSize)
                    {
                        Size pref = GetPreferredSize(Size.Empty);
                        Width = pref.Width;
                    }
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
                _culture = value ?? CultureInfo.CurrentCulture;
                _inputText = SelectedDate;
                UpdateMinimumSize(); // Recalculate size when culture changes
                
                // Apply size compensation if Material Design is enabled
                if (PainterKind == BaseControlPainterKind.Material && DatePickerAutoSizeForMaterial)
                {
                    ApplyMaterialSizeCompensation();
                }
                
                if (_autoSize)
                {
                    Size pref = GetPreferredSize(Size.Empty);
                    Width = pref.Width;
                }
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
            set { _minDate = value; ValidateCurrentDate(); }
        }

        [Browsable(true)]
        [Category("Business")]
        [Description("Maximum allowed date for selection.")]
        public DateTime? MaxDate
        {
            get => _maxDate;
            set { _maxDate = value; ValidateCurrentDate(); }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether empty/null dates are allowed.")]
        [DefaultValue(true)]
        public bool AllowEmpty { get => _allowEmpty; set => _allowEmpty = value; }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether to show the calendar dropdown button.")]
        [DefaultValue(true)]
        public bool ShowDropDown { get => _showDropDown; set { _showDropDown = value; UpdateMinimumSize(); Invalidate(); } }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Makes the date picker read-only (display only).")]
        [DefaultValue(false)]
        public bool ReadOnly { get => _readOnly; set { _readOnly = value; } }

        [Browsable(true)]
        [Category("Business")]
        [Description("Context description for this date picker (e.g., 'Due Date', 'Start Date').")]
        public string DateContext { get => _dateContext; set => _dateContext = value ?? "Date Selection"; }

        [Browsable(true)]
        [Category("Business")]
        [Description("Automatically adjust selected dates to business days (skip weekends).")]
        [DefaultValue(false)]
        public bool AutoAdjustToBusinessDays { get => _autoAdjustToBusinessDays; set => _autoAdjustToBusinessDays = value; }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Highlight today's date in the calendar.")]
        [DefaultValue(true)]
        public bool HighlightToday { get => _highlightToday; set => _highlightToday = value; }

        [Browsable(true)]
        [Category("Business")]
        [Description("Days of the week considered as weekends.")]
        public string[] Weekends { get => _weekends; set => _weekends = value ?? new[] { "Saturday", "Sunday" }; }

        [Browsable(true)]
        [Category("Business")]
        [Description("List of dates to exclude from selection.")]
        public List<DateTime> ExcludedDates { get => _excludedDates; set => _excludedDates = value ?? new List<DateTime>(); }

        [Browsable(true)]
        [Category("Business")]
        [Description("List of dates to highlight in the calendar.")]
        public List<DateTime> HighlightedDates { get => _highlightedDates; set => _highlightedDates = value ?? new List<DateTime>(); }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show validation error icon when date is invalid.")]
        [DefaultValue(true)]
        public bool ShowValidationIcon { get => _showValidationIcon; set => _showValidationIcon = value; }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Automatically submit date when selected from calendar.")]
        [DefaultValue(false)]
        public bool AutoSubmit { get => _autoSubmit; set => _autoSubmit = value; }

        [Browsable(true)]
        [Category("Business")]
        [Description("Current validation error message (read-only).")]
        public string ValidationErrorMessage { get => _validationErrorMessage; private set => _validationErrorMessage = value; }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Automatically calculates height based on font and padding. Set to false to allow manual resizing.")]
        [DefaultValue(true)]
        public bool AutoHeight { get => _autoHeight; set { _autoHeight = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Automatically sizes control (width only) to fit formatted date text. Height governed by AutoHeight.")]
        [DefaultValue(false)]
        public bool AutoSize { get => _autoSize; set { _autoSize = value; if (value) Width = GetPreferredSize(Size.Empty).Width; Invalidate(); } }

        // Material convenience bindings
        [Browsable(true)]
        [Category("Material Design")]
        public string DatePickerLabel { get => LabelText; set => LabelText = value; }
        [Browsable(true)]
        [Category("Material Design")]
        public string DatePickerHelperText { get => HelperText; set => HelperText = value; }
        [Browsable(true)]
        [Category("Material Design")]
        public string DatePickerErrorText { get => ErrorText; set => ErrorText = value; }
        [Browsable(true)]
        [Category("Material Design")]
        public bool DatePickerHasError { get => HasError; set => HasError = value; }
        [Browsable(true)]
        [Category("Material Design")]
        [DefaultValue(true)]
        public bool DatePickerAutoSizeForMaterial { get; set; } = true;

        // Events
        public event EventHandler SelectedDateTimeChanged;
        public event EventHandler<DateChangedEventArgs> DateChanged;
        public event EventHandler<DateSubmittedEventArgs> DateSubmitted;
        public event EventHandler<DateValidationEventArgs> DateValidationFailed;
        public event EventHandler CalendarOpened;
        public event EventHandler CalendarClosed;

        // Bubble up calendar OK/Cancel with payload
        public event EventHandler<DateTimeDialogResultEventArgs> CalendarOkClicked;
        public event EventHandler<DateTimeDialogResultEventArgs> CalendarCancelClicked;
        #endregion

        #region Constructor
        public BeepDatePicker()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
          
            _textFont = this.Font;

          
            calendarIcon = new BeepImage
            {
                IsChild = true,
                ImagePath = TheTechIdea.Beep.Icons.Svgs.fi_tr_calendar,
            };
        }

        protected override Size DefaultSize => new Size(200, 36);

        protected override void InitLayout()
        {
            base.InitLayout();
            _customDateFormat = $"{_culture.DateTimeFormat.ShortDatePattern} HH:mm";
            BoundProperty = "SelectedDateTime";
            // Ensure default date is visible immediately
            if (_selectedDateTime == DateTime.MinValue)
                _selectedDateTime = DateTime.Now;
            UpdateMinimumSize();
            ApplyTheme();
        }
        #endregion

        #region Business Methods
        public void ConfigureForDueDate()
        {
            DateFormatStyle = DateFormatStyle.ShortDateTime;
            DateContext = "Due Date";
            MinDate = DateTime.Today;
            AutoAdjustToBusinessDays = true;
            ShowValidationIcon = true;
            AllowEmpty = true;
        }
        public void ConfigureForCreationDate()
        {
            DateFormatStyle = DateFormatStyle.ShortDate;
            DateContext = "Creation Date";
            MaxDate = DateTime.Today;
            ReadOnly = true;
            ShowDropDown = false;
            AllowEmpty = false;
        }
        public void ConfigureForEventScheduling()
        {
            DateFormatStyle = DateFormatStyle.FullDateTime;
            DateContext = "Event Date";
            MinDate = DateTime.Today;
            AutoAdjustToBusinessDays = false;
            ShowValidationIcon = true;
            AutoSubmit = true;
        }
        public void ConfigureForBirthDate()
        {
            DateFormatStyle = DateFormatStyle.ShortDate;
            DateContext = "Birth Date";
            MaxDate = DateTime.Today;
            MinDate = DateTime.Today.AddYears(-120);
            AllowEmpty = false;
            ShowValidationIcon = true;
        }
        public void ConfigureForAppointment()
        {
            DateFormatStyle = DateFormatStyle.ShortDateTime;
            DateContext = "Appointment Date";
            MinDate = DateTime.Today;
            MaxDate = DateTime.Today.AddMonths(6);
            AutoAdjustToBusinessDays = true;
            AutoSubmit = true;
        }
        public void AddBusinessHolidays(int year)
        {
            foreach (var d in GetBusinessHolidays(year)) if (!_excludedDates.Contains(d)) _excludedDates.Add(d);
        }
        public void SetBusinessDateRange(DateTime startDate, DateTime endDate)
        {
            MinDate = startDate; MaxDate = endDate;
            if (_selectedDateTime < startDate || _selectedDateTime > endDate) SelectedDateTime = startDate;
        }
        public DateTime GetNextBusinessDay(DateTime date)
        {
            var next = date.AddDays(1);
            while (IsWeekend(next) || _excludedDates.Contains(next.Date)) next = next.AddDays(1);
            return next;
        }
        public bool IsDateValid(DateTime date)
        {
            if (_minDate.HasValue && date.Date < _minDate.Value.Date) { _validationErrorMessage = $"Date must be on or after {_minDate.Value:d}"; return false; }
            if (_maxDate.HasValue && date.Date > _maxDate.Value.Date) { _validationErrorMessage = $"Date must be on or before {_maxDate.Value:d}"; return false; }
            if (_excludedDates.Contains(date.Date)) { _validationErrorMessage = "This date is not available for selection"; return false; }
            if (_autoAdjustToBusinessDays && IsWeekend(date)) { _validationErrorMessage = "Please select a business day (Monday-Friday)"; return false; }
            _validationErrorMessage = ""; return true;
        }
        private bool IsWeekend(DateTime date) => _weekends.Contains(date.DayOfWeek.ToString());
        private List<DateTime> GetBusinessHolidays(int year) => new() { new DateTime(year, 1, 1), new DateTime(year, 7, 4), new DateTime(year, 12, 25) };
        private void ValidateCurrentDate()
        {
            if (_selectedDateTime != DateTime.MinValue && !IsDateValid(_selectedDateTime))
                DateValidationFailed?.Invoke(this, new DateValidationEventArgs(_selectedDateTime, _validationErrorMessage));
        }
        private void ShowValidationError()
        {
            if (_showValidationIcon && !string.IsNullOrEmpty(_validationErrorMessage))
            {
                ToolTipText = _validationErrorMessage;
                BackColor = Color.FromArgb(255, 245, 245);
            }
        }
        private void ClearValidationError() { ToolTipText = ""; ApplyTheme(); }
        #endregion

        #region Material sizing and overrides
        public override void ApplyMaterialSizeCompensation()
        {
            if (PainterKind != BaseControlPainterKind.Material || !DatePickerAutoSizeForMaterial) return;
            
            // Use TextRenderer to measure without creating a Graphics
            Size contentSize;
            // Use the same sample text logic as UpdateMinimumSize for consistency
            string sample = GetSampleTextForFormat(GetCurrentFormat());
            if (string.IsNullOrEmpty(sample)) 
                sample = GetPlaceholderText();

            var measured = System.Windows.Forms.TextRenderer.MeasureText(sample + "  ", _textFont);

            // Include dropdown button space in the calculation
            int buttonSpace = _showDropDown ? _buttonWidth + (_padding * 2) : 0;

            contentSize = new Size(measured.Width + buttonSpace, measured.Height);
            
            AdjustSizeForMaterial(contentSize, true);
        }
        
        protected override int GetMaterialMinimumHeight()
        {
            return MaterialVariant switch
            {
                MaterialTextFieldVariant.Standard => 40,
                _ => 56
            };
        }
        
        public override Size GetMaterialIconSpace()
        {
            var baseIcons = base.GetMaterialIconSpace();
            if (!_showDropDown) return baseIcons;
            int trailing = _buttonWidth + (_padding * 2);
            return new Size(baseIcons.Width + trailing, baseIcons.Height);
        }
        #endregion

        #region Events and input
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateMinimumSize();
            // Enforce minimum
            if (Width < MinimumSize.Width) Width = MinimumSize.Width;
            if (Height < MinimumSize.Height) Height = MinimumSize.Height;
            Invalidate();
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = this.Font;
            if (PainterKind == BaseControlPainterKind.Material && DatePickerAutoSizeForMaterial) ApplyMaterialSizeCompensation();
            UpdateMinimumSize();
            Invalidate();
        }
        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            UpdateMinimumSize();
            Invalidate();
        }
        protected override void OnMaterialPropertyChanged()
        {
            base.OnMaterialPropertyChanged();
            UpdateMinimumSize();
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_readOnly) return;
            var contentRect = GetContentRectForDrawing();
            var buttonRect = GetButtonRectFromContent(contentRect);
            if (_showDropDown && buttonRect.Contains(e.Location))
            {
                TogglePopup();
            }
            else
            {
                _isEditing = true;
                _inputText = SelectedDate;
                Invalidate();
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (_readOnly) return;
            // Common shortcuts for pickers
            if (e.KeyCode == Keys.F4 || (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Down))
            {
                TogglePopup();
                e.Handled = true;
            }
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (_readOnly) return;
            if (!_isEditing) _isEditing = true;
            if (e.KeyChar == (char)Keys.Escape) { _isEditing = false; _inputText = SelectedDate; Invalidate(); return; }
            if (e.KeyChar == (char)Keys.Return)
            {
                if (string.IsNullOrWhiteSpace(_inputText))
                {
                    if (_allowEmpty) SelectedDateTime = DateTime.MinValue;
                    _isEditing = false; Invalidate(); return;
                }
                if (DateTime.TryParse(_inputText, _culture, DateTimeStyles.None, out var result) && IsDateValid(result))
                {
                    SelectedDateTime = result;
                    _isEditing = false;
                }
                else
                {
                    ShowValidationError();
                }
                Invalidate();
                return;
            }
            // Accept typical date/time characters
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar) || "/- :".Contains(e.KeyChar))
            {
                _inputText += e.KeyChar;
                Invalidate();
            }
            else
            {
                e.Handled = true;
            }
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _isEditing = false;
        }
        #endregion

        #region Rendering
        // Use material helper content rect if available; fallback to DrawingRect
        private Rectangle GetContentRectForDrawing()
        {
            //if (PainterKind == BaseControlPainterKind.Material )
            //{
            //    var r = GetContentRect();
            //    if (r.Width > 0 && r.Height > 0) return r;
            //}
            UpdateDrawingRect();
            return DrawingRect;
        }

        private Rectangle GetButtonRectFromContent(Rectangle contentRect)
        {
            int x = contentRect.Right - _buttonWidth - _padding;
            int y = contentRect.Y + _padding;
            int h = Math.Max(0, contentRect.Height - (2 * _padding));
            return new Rectangle(x, y, _buttonWidth, h);
        }

        private Rectangle GetTextRect(Rectangle workingRect)
        {
            int rightPadding = _showDropDown ? (_buttonWidth + (_padding * 3)) : _padding;
            return new Rectangle(workingRect.X + _padding, workingRect.Y, Math.Max(0, workingRect.Width - rightPadding - _padding), workingRect.Height);
        }

        private Rectangle GetButtonRect() => GetButtonRect(new Rectangle(0, 0, Width, Height));
        private Rectangle GetButtonRect(Rectangle hostRect)
        {
            int x = hostRect.Right - _buttonWidth - _padding - (ShowAllBorders || BorderThickness > 0 ? BorderThickness : 0);
            int y = hostRect.Y + _padding + (ShowAllBorders || BorderThickness > 0 ? BorderThickness : 0);
            int h = Math.Max(0, hostRect.Height - (2 * _padding) - (ShowAllBorders || BorderThickness > 0 ? BorderThickness * 2 : 0));
            return new Rectangle(x, y, _buttonWidth, h);
        }

        // Remove full override of Draw and render content in DrawContent instead, like other controls
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            if (_currentTheme == null) return;

            Rectangle contentRect = GetContentRectForDrawing();

            // Text
            string textToDraw = _isEditing ? _inputText : (_selectedDateTime != DateTime.MinValue ? FormatDate(_selectedDateTime) : string.Empty);
            if (!string.IsNullOrEmpty(textToDraw))
            {
                Rectangle textRect = GetTextRect(contentRect);
                TextRenderer.DrawText(g, textToDraw, _textFont, textRect,
                    _currentTheme.ComboBoxForeColor != Color.Empty ? _currentTheme.ComboBoxForeColor : ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
            else
            {
                string placeholder = GetPlaceholderText();
                Rectangle placeholderRect = GetTextRect(contentRect);
                Color placeholderColor = _currentTheme.TextBoxPlaceholderColor != Color.Empty ? _currentTheme.TextBoxPlaceholderColor : Color.FromArgb(150, ForeColor);
                TextRenderer.DrawText(g, placeholder, _textFont, placeholderRect, placeholderColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            // Dropdown divider and arrow (no extra background fill to mimic BeepComboBox/BeepButton style)
            if (_showDropDown)
            {
                Rectangle buttonRect = GetButtonRectFromContent(contentRect);
                int dividerX = buttonRect.Left - _padding;
                using (Pen dividerPen = new Pen(Color.FromArgb(60, _currentTheme.ComboBoxBorderColor != Color.Empty ? _currentTheme.ComboBoxBorderColor : BorderColor), 1))
                {
                    g.DrawLine(dividerPen, new Point(dividerX, contentRect.Y + _padding), new Point(dividerX, contentRect.Bottom - _padding));
                }
                DrawCalendarIcon(g, buttonRect);
            }
        }

        private void DrawCalendarIcon(Graphics g, Rectangle buttonRect)
        {
            // Keep a small inner padding for the icon
            int pad = Math.Max(1, _padding);
            var iconRect = new Rectangle(buttonRect.X + pad, buttonRect.Y + pad, Math.Max(8, buttonRect.Width - (2 * pad)), Math.Max(8, buttonRect.Height - (2 * pad)));
            string imagePath = calendarIcon?.ImagePath;
            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    Color iconColor = _currentTheme?.CalendarTitleForColor != Color.Empty ? _currentTheme.CalendarTitleForColor : ForeColor;
                    int cornerRadius = Math.Max(0, BorderRadius);
                    StyledImagePainter.PaintWithTint(g, iconRect, imagePath, iconColor, 1f, cornerRadius);
                    return;
                }
                catch { /* fallback */ }
            }

            if (calendarIcon == null)
            {
                DrawDropdownArrow(g, buttonRect);
                return;
            }

            try
            {
                // Render the BeepImage into the provided rectangle
                calendarIcon.Draw(g, iconRect);
            }
            catch
            {
                // Fallback to arrow if icon draw fails
                DrawDropdownArrow(g, buttonRect);
            }
        }

        private Color ModifyLightness(Color c, float factor)
        {
            int r = Math.Clamp((int)(c.R * factor), 0, 255);
            int g = Math.Clamp((int)(c.G * factor), 0, 255);
            int b = Math.Clamp((int)(c.B * factor), 0, 255);
            return Color.FromArgb(c.A, r, g, b);
        }

        private void DrawDropdownArrow(Graphics g, Rectangle workingRect)
        {
            // Framework handles DPI scaling
            int arrowVisualSize = Math.Min(12, Math.Min(_buttonWidth - (_padding * 2), workingRect.Height - (_padding * 2)));
            int arrowX = workingRect.Left + (workingRect.Width - arrowVisualSize) / 2;
            int arrowY = workingRect.Top + (workingRect.Height - arrowVisualSize) / 2;
            Rectangle arrowRect = new Rectangle(arrowX, arrowY, arrowVisualSize, arrowVisualSize);
            Point[] arrowPoints = new Point[]
            {
                new Point(arrowRect.Left + arrowVisualSize / 4, arrowRect.Top + arrowVisualSize / 3),
                new Point(arrowRect.Left + arrowVisualSize / 2, arrowRect.Top + (2 * arrowVisualSize) / 3),
                new Point(arrowRect.Left + (3 * arrowVisualSize) / 4, arrowRect.Top + arrowVisualSize / 3)
            };
            using (SolidBrush arrowBrush = new SolidBrush(ForeColor))
            {
                g.FillPolygon(arrowBrush, arrowPoints);
            }
        }
        #endregion

        #region Popup Calendar
        private void TogglePopup()
        {
            if (_isPopupOpen) ClosePopup(); else ShowCalendarPopup();
        }
        private void ClosePopup()
        {
            if (!_isPopupOpen) return;

            // Detach handlers to avoid multiple subscriptions after reopen
            if (_calendarView != null)
            {
                _calendarView.OkClicked -= CalendarView_OkClicked;
                _calendarView.CancelClicked -= CalendarView_CancelClicked;
            }

            _calendarPopup?.CloseCascade();
            _calendarPopup = null;
            _calendarView = null;
            _isPopupOpen = false;
            CalendarClosed?.Invoke(this, EventArgs.Empty);
        }
        private void ShowCalendarPopup()
        {
            CalendarOpened?.Invoke(this, EventArgs.Empty);
            _calendarPopup = new BeepPopupForm
            {
               
                Theme = Theme
            };

            _calendarView = new BeepDatePickerView { Dock = DockStyle.Fill, Theme = Theme };
            _calendarView.Context = _dateContext;
            if (_selectedDateTime != DateTime.MinValue) _calendarView.SelectedDateTime = _selectedDateTime;

            _calendarView.OkClicked += CalendarView_OkClicked;
            _calendarView.CancelClicked += CalendarView_CancelClicked;

            var min = _calendarView.MinimumSize;
            int pad = 8;
            int desiredW = Math.Max(350, min.Width + pad * 2);
            int desiredH = Math.Max(400, min.Height + pad * 2);

            _calendarPopup.Size = new Size(desiredW, desiredH);
            _calendarPopup.Controls.Add(_calendarView);
            _calendarPopup.ShowPopup(this, BeepPopupFormPosition.Bottom, desiredW, desiredH);
            _isPopupOpen = true;
        }

        private void CalendarView_OkClicked(object sender, DateTimeDialogResultEventArgs e)
        {
            if (e.SelectedDateTime.HasValue)
            {
                if (IsDateValid(e.SelectedDateTime.Value))
                {
                    SelectedDateTime = e.SelectedDateTime.Value;
                    ClearValidationError();
                }
                else
                {
                    ShowValidationError();
                    MessageBox.Show(_validationErrorMessage, "Date Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            CalendarOkClicked?.Invoke(this, e);
            ClosePopup();
        }

        private void CalendarView_CancelClicked(object sender, DateTimeDialogResultEventArgs e)
        {
            CalendarCancelClicked?.Invoke(this, e);
            ClosePopup();
        }
        #endregion

        #region Format helpers
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
        private string GetPlaceholderText()
        {
            string format = GetCurrentFormat();
            return Regex.Replace(format, @"[dMyHhmsf]+", m => m.Value[0] switch
            {
                'd' => "DD",
                'M' => "MM",
                'y' => "YYYY",
                'H' or 'h' => "HH",
                'm' => "MM",
                's' => "SS",
                'f' => "FF",
                _ => m.Value
            });
        }
        private string FormatDate(DateTime date) => date.ToString(GetCurrentFormat(), _culture);
        #endregion

        #region Theme/Value
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            // propagate theme to the calendar icon
            if (calendarIcon != null)
            {
                calendarIcon.Theme = Theme;
                calendarIcon.ApplyThemeOnImage = true;
            }

            // Map calendar-related theme properties to control appearance where available
            if (_currentTheme != null)
            {
                if (_currentTheme.CalendarBackColor != Color.Empty) BackColor = _currentTheme.CalendarBackColor;
                if (_currentTheme.CalendarForeColor != Color.Empty) ForeColor = _currentTheme.CalendarForeColor;
                if (_currentTheme.CalendarBorderColor != Color.Empty) BorderColor = _currentTheme.CalendarBorderColor;

                try { _textFont = BeepThemesManager.ToFont(_currentTheme.DateFont); } catch { _textFont = this.Font; }

                // Invalidate and pre-render calendar icon tinted variants in background
                try
                {
                    if (!string.IsNullOrEmpty(calendarIcon?.ImagePath))
                    {
                        var iconPath = calendarIcon.ImagePath;
                        StyledImagePainter.InvalidateCaches(iconPath);
                        Color iconColor = _currentTheme.CalendarTitleForColor != Color.Empty ? _currentTheme.CalendarTitleForColor : ForeColor;
                        var sizes = new int[] { 8, 12, 14, 16, 18, 20, 24, 28, 32 };
                        Task.Run(() =>
                        {
                            foreach (var s in sizes)
                            {
                                try { StyledImagePainter.PreRenderTintedToCache(iconPath, iconColor, 1f, new Size(s, s)); } catch { }
                            }
                        });
                    }
                }
                catch { }
            }

            _textFont = BeepThemesManager.ToFont(_currentTheme.DateFont);
            Invalidate();
        }

        public override void SetValue(object value)
        {
            if (value is DateTime dt) SelectedDateTime = dt;
            else if (value is string str && DateTime.TryParse(str, out DateTime parsed)) SelectedDateTime = parsed;
            else SelectedDate = value?.ToString();
        }
        public override object GetValue() => _selectedDateTime == DateTime.MinValue ? null : _selectedDateTime;
        public void Reset() => SelectedDateTime = DateTime.Now;
        public void Clear() { if (_allowEmpty) { SelectedDateTime = DateTime.MinValue; _inputText = ""; ClearValidationError(); } }
        #endregion

        #region Minimum Size Calculation
        private void UpdateMinimumSize()
        {
            try
            {
                // Get the actual format that will be displayed
                string currentFormat = GetCurrentFormat();
                
                // Create a representative sample text that shows the maximum width needed
                string sample = GetSampleTextForFormat(currentFormat);
                
                Size textSize;
                using (var g = CreateGraphics())
                {
                    // Measure the actual text that will be displayed, adding some padding
                    textSize = TextRenderer.MeasureText(g, sample + "  ", _textFont, 
                        new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine);
                }

                // Calculate minimum dimensions based on content
                int textPrefH = Math.Max(_textFont.Height + 6, 16);
                int buttonWidth = _showDropDown ? Math.Max(_buttonWidth, Math.Max(16, textPrefH)) : 0;
                
                // Add padding for comfortable text display
                int horizontalPadding = (_padding * 4); // Extra padding for comfortable reading
                int baseContentW = textSize.Width + buttonWidth + horizontalPadding;
                int baseContentH = textPrefH;

                // Ensure reasonable minimums
                Size baseContentMin = new Size(Math.Max(120, baseContentW), Math.Max(20, baseContentH));

                // Apply Material Design calculations if enabled
                Size effectiveMin = PainterKind == BaseControlPainterKind.Material
                    ? GetEffectiveMaterialMinimum(baseContentMin)
                    : new Size(
                        baseContentMin.Width + (BorderThickness + 2) * 2,
                        baseContentMin.Height + (BorderThickness + 2) * 2);

                // Safety clamps
                effectiveMin.Width = Math.Max(effectiveMin.Width, 120);
                effectiveMin.Height = Math.Max(effectiveMin.Height, 24);

                this.MinimumSize = effectiveMin;

                // Auto-adjust height if enabled
                if (_autoHeight || Height < effectiveMin.Height) 
                    Height = effectiveMin.Height;
            }
            catch 
            { 
                this.MinimumSize = new Size(150, 28); 
            }
        }

        /// <summary>
        /// Gets a sample text that represents the maximum width needed for the current format
        /// </summary>
        /// <param name="format">The date format string</param>
        /// <returns>Sample text for width calculation</returns>
        private string GetSampleTextForFormat(string format)
        {
            // Create a sample date that will likely be the widest possible
            DateTime sampleDate = new DateTime(2023, 12, 28, 23, 59, 59); // Wide date with long month/day names
            
            try
            {
                // First try with the sample date to get realistic width
                string formattedSample = sampleDate.ToString(format, _culture);
                
                // For certain formats, we want to ensure we account for the widest possible text
                switch (_dateFormatStyle)
                {
                    case DateFormatStyle.LongDate:
                        // Use a date with the longest month and day names
                        var longSample = new DateTime(2023, 9, 28); // September + Wednesday
                        return longSample.ToString(format, _culture);
                        
                    case DateFormatStyle.FullDateTime:
                        // Use the longest possible full date time
                        var fullSample = new DateTime(2023, 12, 28, 23, 59, 59);
                        return fullSample.ToString(format, _culture);
                        
                    case DateFormatStyle.DayMonthYear:
                        // Use longest month name
                        var dayMonthSample = new DateTime(2023, 9, 28); // September
                        return dayMonthSample.ToString(format, _culture);
                        
                    case DateFormatStyle.MonthDay:
                        var monthDaySample = new DateTime(2023, 9, 28);
                        return monthDaySample.ToString(format, _culture);
                        
                    case DateFormatStyle.DayOfWeek:
                        var dayOfWeekSample = new DateTime(2023, 9, 27); // Wednesday (typically longest)
                        return dayOfWeekSample.ToString(format, _culture);
                        
                    default:
                        return formattedSample;
                }
            }
            catch
            {
                // Fallback to a reasonable placeholder if formatting fails
                return GetPlaceholderText();
            }
        }
        #endregion
    }

    #region Event Args
    public class DateChangedEventArgs : EventArgs
    {
        public DateTime OldDate { get; }
        public DateTime NewDate { get; }
        public DateChangedEventArgs(DateTime oldDate, DateTime newDate) { OldDate = oldDate; NewDate = newDate; }
    }
    public class DateSubmittedEventArgs : EventArgs
    {
        public DateTime SelectedDate { get; }
        public string Context { get; }
        public DateSubmittedEventArgs(DateTime selectedDate, string context) { SelectedDate = selectedDate; Context = context; }
    }
    public class DateValidationEventArgs : EventArgs
    {
        public DateTime InvalidDate { get; }
        public string ErrorMessage { get; }
        public DateValidationEventArgs(DateTime invalidDate, string errorMessage) { InvalidDate = invalidDate; ErrorMessage = errorMessage; }
    }

    public class DateTimeDialogResultEventArgs : EventArgs
    {
        public DateTime? SelectedDateTime { get; }
        public string Context { get; }
        public bool IsConfirmed { get; }

        public DateTimeDialogResultEventArgs(DateTime? selectedDateTime, string context, bool isConfirmed)
        {
            SelectedDateTime = selectedDateTime;
            Context = context;
            IsConfirmed = isConfirmed;
        }
    }
    #endregion
}
