using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Helpers;
using TheTechIdea.Beep.Winform.Controls.Dates.HitHandlers;

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
        
        // Hit test helper and handler for BaseControl integration
        private BeepDateTimePickerHitTestHelper _hitHelper;
        private IDateTimePickerHitHandler _hitHandler;
        
        // Internal property to expose HitTestHelper to painters
        internal BeepDateTimePickerHitTestHelper HitTestHelper => _hitHelper;

        // Date/Time values
        private DateTime? _selectedDate;
    private DateTime? _rangeStartDate;
    private DateTime? _rangeEndDate;
    private TimeSpan? _rangeStartTime;
    private TimeSpan? _rangeEndTime;
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

        // NO DROPDOWN - Direct painting like BeepDatePickerView
        private DateTimePickerLayout _layout;
        private DateTimePickerHoverState _hoverState = new DateTimePickerHoverState();
        
        // Hit testing and interaction
        private bool _isMouseDown = false;
        private Point _lastMousePosition = Point.Empty;
        
        // Appearance
        private bool _useScaledFont = false;
        private bool _useThemeFont = true;
        private Font _textFont;

        #endregion

        #region Events

        public event EventHandler<DateTimePickerEventArgs> DateChanged;
        public event EventHandler<DateTimePickerEventArgs> TimeChanged;
        public event EventHandler<DateTimePickerEventArgs> RangeChanged;
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

        #endregion

        #region Constructor

        public BeepDateTimePicker() : base()
        {
            // Use BaseControl's Minimalist painter to render the outer container (subtle 1px border, card-like)
            // Our internal calendar UI is still painted by IDateTimePickerPainter inside DrawingRect.
            PainterKind = BaseControlPainterKind.Classic;
            // Prevent WinForms default black border from showing in addition to our painter border
            BorderStyle = System.Windows.Forms.BorderStyle.None;
            
            // Control configuration - CRITICAL: Make it focusable and handle clicks properly
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | 
                     ControlStyles.Selectable | ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
            UpdateStyles();
            DoubleBuffered = true;
            TabStop = true; // Allow tab navigation

            // Initialize font
            _textFont = new Font("Segoe UI", 9.75f);

            // Initialize hit test helper for BaseControl integration
            _hitHelper = new BeepDateTimePickerHitTestHelper(this);

            // Initialize painter - this will set default size
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
            
            // Initialize hit handler for this mode
            InitializeHitHandler();
            
            // Set size based on painter requirements
            if (_currentPainter != null)
            {
                var props = GetCurrentProperties();
                var preferredSize = _currentPainter.GetPreferredSize(props);
                var minimumSize = _currentPainter.GetMinimumSize(props);
                
                Size = preferredSize;
                MinimumSize = minimumSize;
            }
            
            UpdateLayout();
        }

        /// <summary>
        /// Initialize the appropriate hit handler based on current mode
        /// </summary>
        private void InitializeHitHandler()
        {
            switch (_mode)
            {
                case DatePickerMode.Single:
                    _hitHandler = new HitHandlers.SingleDateTimePickerHitHandler();
                    break;
                case DatePickerMode.SingleWithTime:
                    _hitHandler = new HitHandlers.SingleWithTimeDateTimePickerHitHandler();
                    break;
                case DatePickerMode.Range:
                    _hitHandler = new HitHandlers.RangeDateTimePickerHitHandler();
                    break;
                case DatePickerMode.RangeWithTime:
                    _hitHandler = new HitHandlers.RangeWithTimeDateTimePickerHitHandler();
                    break;
                case DatePickerMode.Multiple:
                    _hitHandler = new HitHandlers.MultipleDateTimePickerHitHandler();
                    break;
                case DatePickerMode.Appointment:
                    _hitHandler = new HitHandlers.AppointmentDateTimePickerHitHandler();
                    break;
                case DatePickerMode.Timeline:
                    _hitHandler = new HitHandlers.TimelineDateTimePickerHitHandler();
                    break;
                case DatePickerMode.Quarterly:
                    _hitHandler = new HitHandlers.QuarterlyDateTimePickerHitHandler();
                    break;
                case DatePickerMode.Compact:
                    _hitHandler = new HitHandlers.CompactDateTimePickerHitHandler();
                    break;
                case DatePickerMode.ModernCard:
                    _hitHandler = new HitHandlers.ModernCardDateTimePickerHitHandler();
                    break;
                case DatePickerMode.DualCalendar:
                    _hitHandler = new HitHandlers.DualCalendarDateTimePickerHitHandler();
                    break;
                case DatePickerMode.WeekView:
                    _hitHandler = new HitHandlers.WeekViewDateTimePickerHitHandler();
                    break;
                case DatePickerMode.MonthView:
                    _hitHandler = new HitHandlers.MonthViewDateTimePickerHitHandler();
                    break;
                case DatePickerMode.YearView:
                    _hitHandler = new HitHandlers.YearViewDateTimePickerHitHandler();
                    break;
                case DatePickerMode.SidebarEvent:
                    _hitHandler = new HitHandlers.SidebarEventDateTimePickerHitHandler();
                    break;
                case DatePickerMode.FlexibleRange:
                    _hitHandler = new HitHandlers.FlexibleRangeDateTimePickerHitHandler();
                    break;
                case DatePickerMode.FilteredRange:
                    _hitHandler = new HitHandlers.FilteredRangeDateTimePickerHitHandler();
                    break;
                case DatePickerMode.Header:
                    _hitHandler = new HitHandlers.HeaderDateTimePickerHitHandler();
                    break;
                default:
                    _hitHandler = new HitHandlers.SingleDateTimePickerHitHandler();
                    break;
            }
            
            // Sync initial state from control to handler
            _hitHandler?.SyncFromControl(this);
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
            
            // Re-initialize hit handler for new mode
            InitializeHitHandler();
            
            // Update size based on painter requirements
            if (_currentPainter != null)
            {
                var props = GetCurrentProperties();
                var preferredSize = _currentPainter.GetPreferredSize(props);
                var minimumSize = _currentPainter.GetMinimumSize(props);
                
                Size = preferredSize;
                MinimumSize = minimumSize;
            }
            
            UpdateLayout();
            Invalidate();
        }

        #endregion

        #region Helper Methods

        private void UpdateLayout()
        {
            if (_currentPainter == null) return;

            // Calculate layout using painter
            // Use the content area provided by BaseControl's outer painter (e.g., Minimalist)
            // to avoid double-padding and mismatched borders
            var bounds = DrawingRect;
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                // Fallback to full client area if DrawingRect isn't ready yet
                bounds = ClientRectangle;
            }
            _layout = _currentPainter.CalculateLayout(bounds, GetCurrentProperties());
        }

        public DateTimePickerProperties GetCurrentProperties()
        {
            return new DateTimePickerProperties
            {
                Format = _format,
                ShowTime = _showTime,
                ShowWeekNumbers = _showWeekNumbers ? DatePickerWeekNumbers.Left : DatePickerWeekNumbers.None,
                AllowClear = _allowClear,
                FirstDayOfWeek = _firstDayOfWeek,
                TimeInterval = TimeSpan.FromMinutes(_timeIntervalMinutes),
                TimeIntervalMinutes = _timeIntervalMinutes,
                TimeStartHour = TimeStartHour,
                MinTime = _minTime,
                MaxTime = _maxTime,
                MinDate = _minDate,
                MaxDate = _maxDate,
                ShowTodayButton = true,
                ShowTomorrowButton = true,
                ShowCustomQuickDates = _showQuickButtons
            };
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textFont?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
