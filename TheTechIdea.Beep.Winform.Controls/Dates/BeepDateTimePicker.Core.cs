using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    /// <summary>
    /// BeepDateTimePicker - Core partial: fields, events, helpers, constructor
    /// </summary>
    public partial class BeepDateTimePicker
    {
        #region Fields

        // Mode and painter
        private DatePickerMode _mode = DatePickerMode.Single;
        private IDateTimePickerPainter _currentPainter;

        // Date/Time values
        private DateTime? _selectedDate;
        private DateTime? _rangeStartDate;
        private DateTime? _rangeEndDate;
        private TimeSpan? _selectedTime;
        private DateTime _displayMonth = DateTime.Today;
    private List<DateTime> _selectedDates = new List<DateTime>();
        
        // Constraints
        private DateTime _minDate = new DateTime(1900, 1, 1);
        private DateTime _maxDate = new DateTime(2100, 12, 31);

        // Mode and behavior
        private DatePickerFormat _format = DatePickerFormat.ShortDate;
        private bool _showTime = false;
        private bool _showQuickButtons = true;
        private bool _showWeekNumbers = false;
        private bool _allowClear = true;
        private DatePickerFirstDayOfWeek _firstDayOfWeek = DatePickerFirstDayOfWeek.Monday;

        // Time picker settings
        private int _timeIntervalMinutes = 30;
        private TimeSpan _minTime = TimeSpan.Zero;
        private TimeSpan _maxTime = new TimeSpan(23, 59, 59);

        // Dropdown and layout
        private Form _dropdownForm;
        private bool _isDropDownOpen = false;
        private DateTimePickerLayout _layout;
        private DateTimePickerHoverState _hoverState = new DateTimePickerHoverState();
        
        // Hit testing and interaction
        private bool _isMouseDown = false;
        private Point _lastMousePosition = Point.Empty;
        
        // Text display
        private Rectangle _textDisplayRect;
        private Rectangle _dropdownButtonRect;
        private Rectangle _clearButtonRect;
        
        // Quick selection options
        private bool _showToday = true;
        private bool _showTomorrow = true;
        private bool _showYesterday = true;
        private bool _showThisWeek = true;
        private bool _showThisMonth = true;

        // Appearance
        private bool _useScaledFont = false;
        private bool _useThemeFont = true;
        private Font _textFont;

        #endregion

        #region Events

        public event EventHandler<DateTimePickerEventArgs> DateChanged;
        public event EventHandler<DateTimePickerEventArgs> TimeChanged;
        public event EventHandler<DateTimePickerEventArgs> RangeChanged;
        public event EventHandler DropDownOpened;
        public event EventHandler DropDownClosed;
        public event EventHandler<DateTimePickerEventArgs> DateHovered;
        public event EventHandler<DateTimePickerEventArgs> QuickButtonClicked;
        public event EventHandler ClearClicked;

        protected virtual void OnDateChanged(DateTime? date)
        {
            DateChanged?.Invoke(this, new DateTimePickerEventArgs(date, null, null, DatePickerMode.Single));
        }

        protected virtual void OnTimeChanged(TimeSpan? time)
        {
            TimeChanged?.Invoke(this, new DateTimePickerEventArgs(_selectedDate, time, null, _mode));
        }

        protected virtual void OnRangeChanged(DateTime? start, DateTime? end)
        {
            RangeChanged?.Invoke(this, new DateTimePickerEventArgs(start, null, end, DatePickerMode.Range));
        }

        protected virtual void OnDropDownOpened()
        {
            DropDownOpened?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDropDownClosed()
        {
            DropDownClosed?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Constructor

        public BeepDateTimePicker() : base()
        {
            // Control configuration
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | 
                     ControlStyles.Selectable, true);
            UpdateStyles();
            DoubleBuffered = true;
            
            // Default size
            Size = new Size(250, 35);
            MinimumSize = new Size(150, 30);

            // Initialize font
            _textFont = new Font("Segoe UI", 9.75f);

            // Setup mouse events
            MouseDown += OnMouseDownHandler;
            MouseUp += OnMouseUpHandler;
            MouseMove += OnMouseMoveHandler;
            MouseClick += OnMouseClickHandler;
            MouseLeave += OnMouseLeaveHandler;
            
            // Keyboard events
            KeyDown += OnKeyDownHandler;
            KeyPress += OnKeyPressHandler;

            // Initialize painter
            InitializePainter();

            // Handle creation
            this.HandleCreated += (s, e) =>
            {
                UpdateLayout();
                Invalidate();
            };

            this.VisibleChanged += (s, e) =>
            {
                if (this.Visible)
                {
                    Invalidate();
                    Update();
                }
            };
        }

        #endregion

        #region Painter Management

        private void InitializePainter()
        {
            // Ensure theme exists (BaseControl sets _currentTheme in ctor)
            if (_currentTheme == null)
            {
                try
                {
                    _currentTheme = ThemeManagement.BeepThemesManager.GetDefaultTheme();
                }
                catch { /* fallback to null-safe painters */ }
            }

            // Create painter based on current mode
            _currentPainter = DateTimePickerPainterFactory.CreatePainter(_mode, this, _currentTheme);
            UpdateLayout();
        }

        /// <summary>
        /// Updates painter based on current Mode
        /// Visual styling is handled by _currentTheme (BeepTheme)
        /// </summary>
        private void UpdatePainter()
        {
            if (_currentTheme == null)
            {
                try
                {
                    _currentTheme = ThemeManagement.BeepThemesManager.GetDefaultTheme();
                }
                catch { /* fallback to null-safe painters */ }
            }

            // Create painter based on mode - theme provides visual styling
            _currentPainter = DateTimePickerPainterFactory.CreatePainter(_mode, this, _currentTheme);
            UpdateLayout();
            Invalidate();
        }

        #endregion

        #region Helper Methods

        private void UpdateLayout()
        {
            if (_currentPainter == null) return;

            // Calculate text display and button rectangles
            var clientRect = ClientRectangle;
            var buttonWidth = Math.Min(30, clientRect.Height);
            
            _dropdownButtonRect = new Rectangle(
                clientRect.Right - buttonWidth - 2,
                clientRect.Y + 2,
                buttonWidth,
                clientRect.Height - 4
            );

            if (_allowClear && _selectedDate.HasValue)
            {
                _clearButtonRect = new Rectangle(
                    _dropdownButtonRect.Left - buttonWidth - 2,
                    clientRect.Y + 2,
                    buttonWidth,
                    clientRect.Height - 4
                );
                
                _textDisplayRect = new Rectangle(
                    clientRect.X + 4,
                    clientRect.Y + 2,
                    clientRect.Width - (buttonWidth * 2) - 12,
                    clientRect.Height - 4
                );
            }
            else
            {
                _clearButtonRect = Rectangle.Empty;
                _textDisplayRect = new Rectangle(
                    clientRect.X + 4,
                    clientRect.Y + 2,
                    clientRect.Width - buttonWidth - 8,
                    clientRect.Height - 4
                );
            }
        }

        private string GetFormattedText()
        {
            if (!_selectedDate.HasValue)
                return string.Empty;

            string dateText = _format switch
            {
                DatePickerFormat.ShortDate => _selectedDate.Value.ToShortDateString(),
                DatePickerFormat.Long => _selectedDate.Value.ToLongDateString(),
                DatePickerFormat.Custom => _selectedDate.Value.ToString("MMM dd, yyyy"),
                _ => _selectedDate.Value.ToShortDateString()
            };

            if (_showTime && _selectedTime.HasValue)
            {
                dateText += " " + _selectedTime.Value.ToString(@"hh\:mm");
            }

            if (_mode == DatePickerMode.Range && _rangeEndDate.HasValue)
            {
                string endText = _format switch
                {
                    DatePickerFormat.ShortDate => _rangeEndDate.Value.ToShortDateString(),
                    DatePickerFormat.Long => _rangeEndDate.Value.ToLongDateString(),
                    DatePickerFormat.Custom => _rangeEndDate.Value.ToString("MMM dd, yyyy"),
                    _ => _rangeEndDate.Value.ToShortDateString()
                };
                dateText += " - " + endText;
            }

            return dateText;
        }

        private DateTimePickerProperties GetCurrentProperties()
        {
            return new DateTimePickerProperties
            {
                Format = _format,
                ShowTime = _showTime,
                ShowWeekNumbers = _showWeekNumbers ? DatePickerWeekNumbers.Left : DatePickerWeekNumbers.None,
                AllowClear = _allowClear,
                FirstDayOfWeek = _firstDayOfWeek,
                TimeInterval = TimeSpan.FromMinutes(_timeIntervalMinutes),
                MinDate = _minDate,
                MaxDate = _maxDate,
                ShowTodayButton = _showToday,
                ShowTomorrowButton = _showTomorrow,
                ShowCustomQuickDates = true
            };
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseDropDown();
                _textFont?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
