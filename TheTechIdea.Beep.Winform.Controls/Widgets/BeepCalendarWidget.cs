using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Painters.Calendar;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum CalendarWidgetStyle
    {
        DateGrid,         // Calendar grid layout
        TimeSlots,        // Available time slot picker
        EventCard,        // Event display card
        CalendarView,     // Full calendar month view
        ScheduleCard,     // Schedule/appointment display
        DatePicker,       // Date selection interface
        TimelineView,     // Timeline-based calendar
        WeekView,         // Weekly calendar display
        EventList,        // List of upcoming events
        AvailabilityGrid  // Availability/booking grid
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Calendar Widget")]
    [Category("Beep Widgets")]
    [Description("Calendar widget for scheduling, time management, event display, and date selection.")]
    public class BeepCalendarWidget : BaseControl
    {
        #region Fields
        private CalendarWidgetStyle _style = CalendarWidgetStyle.CalendarView;
        private IWidgetPainter _painter;
        private string _title = "Calendar";
        private string _subtitle = "Schedule";
        private DateTime _selectedDate = DateTime.Now;
        private DateTime _displayMonth = DateTime.Now;
        private List<CalendarEvent> _events = new List<CalendarEvent>();
        private List<TimeSlot> _timeSlots = new List<TimeSlot>();
        private Color _accentColor = Color.FromArgb(33, 150, 243); // Blue
        private Color _eventColor = Color.FromArgb(76, 175, 80); // Green
        private Color _selectedColor = Color.FromArgb(255, 193, 7); // Amber
        private Color _todayColor = Color.FromArgb(244, 67, 54); // Red
        private Color _calendarBorderColor = Color.FromArgb(200, 200, 200);
        private Color _surfaceColor = Color.White;
        private Color _selectedDateBackColor = Color.FromArgb(33, 150, 243);
        private Color _selectedDateForeColor = Color.White;
        private Color _hoverBackColor = Color.FromArgb(245, 245, 245);
        private Color _hoverForeColor = Color.Black;
        private Color _titleForeColor = Color.Black;
        private Color _daysHeaderForeColor = Color.FromArgb(100, 100, 100);
        private Color _confirmedEventColor = Color.FromArgb(76, 175, 80);
        private Color _pendingEventColor = Color.FromArgb(255, 193, 7);
        private Color _conflictEventColor = Color.FromArgb(244, 67, 54);
        private bool _showWeekends = true;
        private bool _showToday = true;
        private bool _showEvents = true;
        private bool _allowMultiSelect = false;
        private CalendarViewMode _viewMode = CalendarViewMode.Month;
        private int _workingHoursStart = 9;
        private int _workingHoursEnd = 17;
        private DayOfWeek _firstDayOfWeek = DayOfWeek.Sunday;
        private List<DateTime> _selectedDates = new List<DateTime>();

        // Events
        public event EventHandler<BeepEventDataArgs> DateSelected;
        public event EventHandler<BeepEventDataArgs> EventClicked;
        public event EventHandler<BeepEventDataArgs> TimeSlotClicked;
        public event EventHandler<BeepEventDataArgs> MonthChanged;
        public event EventHandler<BeepEventDataArgs> ViewModeChanged;
        #endregion

        #region Constructor
        public BeepCalendarWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(350, 300);
            ApplyThemeToChilds = false;
            InitializeSampleData();
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializeSampleData()
        {
            _events.AddRange(new[]
            {
                new CalendarEvent 
                { 
                    Title = "Team Meeting", 
                    StartTime = DateTime.Today.AddHours(10), 
                    EndTime = DateTime.Today.AddHours(11),
                    Description = "Weekly team standup",
                    Color = Color.FromArgb(76, 175, 80),
                    Type = "Meeting"
                },
                new CalendarEvent 
                { 
                    Title = "Project Review", 
                    StartTime = DateTime.Today.AddDays(1).AddHours(14), 
                    EndTime = DateTime.Today.AddDays(1).AddHours(15),
                    Description = "Quarterly project review",
                    Color = Color.FromArgb(33, 150, 243),
                    Type = "Review"
                },
                new CalendarEvent 
                { 
                    Title = "Client Call", 
                    StartTime = DateTime.Today.AddDays(2).AddHours(16), 
                    EndTime = DateTime.Today.AddDays(2).AddHours(17),
                    Description = "Client consultation call",
                    Color = Color.FromArgb(255, 193, 7),
                    Type = "Call"
                }
            });

            _timeSlots.AddRange(new[]
            {
                new TimeSlot { StartTime = TimeSpan.FromHours(9), EndTime = TimeSpan.FromHours(10), IsAvailable = true, Label = "9:00 AM" },
                new TimeSlot { StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(11), IsAvailable = false, Label = "10:00 AM" },
                new TimeSlot { StartTime = TimeSpan.FromHours(11), EndTime = TimeSpan.FromHours(12), IsAvailable = true, Label = "11:00 AM" },
                new TimeSlot { StartTime = TimeSpan.FromHours(14), EndTime = TimeSpan.FromHours(15), IsAvailable = true, Label = "2:00 PM" },
                new TimeSlot { StartTime = TimeSpan.FromHours(15), EndTime = TimeSpan.FromHours(16), IsAvailable = false, Label = "3:00 PM" },
                new TimeSlot { StartTime = TimeSpan.FromHours(16), EndTime = TimeSpan.FromHours(17), IsAvailable = true, Label = "4:00 PM" }
            });

            _selectedDates.Add(_selectedDate);
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case CalendarWidgetStyle.DateGrid:
                    _painter = new DateGridPainter();
                    break;
                case CalendarWidgetStyle.TimeSlots:
                    _painter = new TimeSlotsPainter();
                    break;
                case CalendarWidgetStyle.EventCard:
                    _painter = new EventCardPainter();
                    break;
                case CalendarWidgetStyle.CalendarView:
                    _painter = new CalendarViewPainter();
                    break;
                case CalendarWidgetStyle.ScheduleCard:
                    _painter = new ScheduleCardPainter();
                    break;
                case CalendarWidgetStyle.DatePicker:
                    _painter = new DatePickerPainter();
                    break;
                case CalendarWidgetStyle.TimelineView:
                    _painter = new TimelineViewPainter();
                    break;
                case CalendarWidgetStyle.WeekView:
                    _painter = new WeekViewPainter();
                    break;
                case CalendarWidgetStyle.EventList:
                    _painter = new EventListPainter();
                    break;
                case CalendarWidgetStyle.AvailabilityGrid:
                    _painter = new AvailabilityGridPainter();
                    break;
                default:
                    _painter = new CalendarViewPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Calendar")]
        [Description("Visual style of the calendar widget.")]
        public CalendarWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Title text for the calendar widget.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Subtitle text for the calendar widget.")]
        public string Subtitle
        {
            get => _subtitle;
            set { _subtitle = value; Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Currently selected date.")]
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set { _selectedDate = value; Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Currently displayed month/year.")]
        public DateTime DisplayMonth
        {
            get => _displayMonth;
            set { _displayMonth = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the calendar widget.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for calendar events.")]
        public Color EventColor
        {
            get => _eventColor;
            set { _eventColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for selected dates.")]
        public Color SelectedColor
        {
            get => _selectedColor;
            set { _selectedColor = value; Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Color for today's date.")]
        public Color TodayColor
        {
            get => _todayColor;
            set { _todayColor = value; Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Whether to show weekends.")]
        public bool ShowWeekends
        {
            get => _showWeekends;
            set { _showWeekends = value; Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Whether to highlight today's date.")]
        public bool ShowToday
        {
            get => _showToday;
            set { _showToday = value; Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Whether to show events on the calendar.")]
        public bool ShowEvents
        {
            get => _showEvents;
            set { _showEvents = value; Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Whether to allow multiple date selection.")]
        public bool AllowMultiSelect
        {
            get => _allowMultiSelect;
            set { _allowMultiSelect = value; Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Calendar view mode (Month, Week, Day).")]
        public CalendarViewMode ViewMode
        {
            get => _viewMode;
            set { _viewMode = value; Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Working hours start time (24-hour format).")]
        public int WorkingHoursStart
        {
            get => _workingHoursStart;
            set { _workingHoursStart = Math.Max(0, Math.Min(23, value)); Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Working hours end time (24-hour format).")]
        public int WorkingHoursEnd
        {
            get => _workingHoursEnd;
            set { _workingHoursEnd = Math.Max(0, Math.Min(23, value)); Invalidate(); }
        }

        [Category("Calendar")]
        [Description("First day of the week.")]
        public DayOfWeek FirstDayOfWeek
        {
            get => _firstDayOfWeek;
            set { _firstDayOfWeek = value; Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Collection of calendar events.")]
        public List<CalendarEvent> Events
        {
            get => _events;
            set { _events = value ?? new List<CalendarEvent>(); Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Collection of available time slots.")]
        public List<TimeSlot> TimeSlots
        {
            get => _timeSlots;
            set { _timeSlots = value ?? new List<TimeSlot>(); Invalidate(); }
        }

        [Category("Calendar")]
        [Description("Collection of selected dates (for multi-select mode).")]
        public List<DateTime> SelectedDates
        {
            get => _selectedDates;
            set { _selectedDates = value ?? new List<DateTime>(); Invalidate(); }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            var ctx = new WidgetContext
            {
                DrawingRect = DrawingRect,
                Title = _title,
                Value = _subtitle,
                AccentColor = _accentColor,
                ShowIcon = true,
                IsInteractive = true,
                CornerRadius = BorderRadius,
                CustomData = new Dictionary<string, object>
                {
                    ["Events"] = _events,
                    ["TimeSlots"] = _timeSlots,
                    ["SelectedDate"] = _selectedDate,
                    ["DisplayMonth"] = _displayMonth,
                    ["EventColor"] = _eventColor,
                    ["SelectedColor"] = _selectedColor,
                    ["TodayColor"] = _todayColor,
                    ["ShowWeekends"] = _showWeekends,
                    ["ShowToday"] = _showToday,
                    ["ShowEvents"] = _showEvents,
                    ["AllowMultiSelect"] = _allowMultiSelect,
                    ["ViewMode"] = _viewMode,
                    ["WorkingHoursStart"] = _workingHoursStart,
                    ["WorkingHoursEnd"] = _workingHoursEnd,
                    ["FirstDayOfWeek"] = _firstDayOfWeek,
                    ["SelectedDates"] = _selectedDates
                }
            };

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);
            _painter?.DrawContent(g, ctx);
            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => { });
        }

        private void RefreshHitAreas(WidgetContext ctx)
        {
            ClearHitList();

            if (!ctx.ContentRect.IsEmpty)
            {
                AddHitArea("Calendar", ctx.ContentRect, null, () =>
                {
                    DateSelected?.Invoke(this, new BeepEventDataArgs("DateSelected", this));
                });
            }

            if (!ctx.HeaderRect.IsEmpty)
            {
                AddHitArea("Header", ctx.HeaderRect, null, () =>
                {
                    MonthChanged?.Invoke(this, new BeepEventDataArgs("MonthChanged", this));
                });
            }

            // Add hit areas for events
            if (_showEvents && _events.Count > 0)
            {
                for (int i = 0; i < _events.Count && i < 10; i++) // Limit to 10 visible events
                {
                    int eventIndex = i; // Capture for closure
                    AddHitArea($"Event{i}", new Rectangle(), null, () =>
                    {
                        EventClicked?.Invoke(this, new BeepEventDataArgs("EventClicked", this) { EventData = _events[eventIndex] });
                    });
                }
            }

            // Add hit areas for time slots
            if (_style == CalendarWidgetStyle.TimeSlots && _timeSlots.Count > 0)
            {
                for (int i = 0; i < _timeSlots.Count; i++)
                {
                    int slotIndex = i; // Capture for closure
                    AddHitArea($"TimeSlot{i}", new Rectangle(), null, () =>
                    {
                        TimeSlotClicked?.Invoke(this, new BeepEventDataArgs("TimeSlotClicked", this) { EventData = _timeSlots[slotIndex] });
                    });
                }
            }
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply calendar-specific theme colors
            BackColor = _currentTheme.CalendarBackColor;
            ForeColor = _currentTheme.CalendarForeColor;
            
            // Update calendar structure colors
            _calendarBorderColor = _currentTheme.CalendarBorderColor;
            _todayColor = _currentTheme.CalendarTodayForeColor;
            _surfaceColor = _currentTheme.SurfaceColor;
            
            // Update date selection colors
            _selectedDateBackColor = _currentTheme.CalendarSelectedDateBackColor;
            _selectedDateForeColor = _currentTheme.CalendarSelectedDateForColor;
            _hoverBackColor = _currentTheme.CalendarHoverBackColor;
            _hoverForeColor = _currentTheme.CalendarHoverForeColor;
            
            // Update header colors
            _titleForeColor = _currentTheme.CalendarTitleForColor;
            _daysHeaderForeColor = _currentTheme.CalendarDaysHeaderForColor;
            
            // Update event colors
            _eventColor = _currentTheme.AccentColor;
            _confirmedEventColor = _currentTheme.SuccessColor;
            _pendingEventColor = _currentTheme.WarningColor;
            _conflictEventColor = _currentTheme.ErrorColor;
            
            InitializePainter();
            Invalidate();
        }
    }

    /// <summary>
    /// Calendar view mode enumeration
    /// </summary>
    public enum CalendarViewMode
    {
        Month,        // Monthly view
        Week,         // Weekly view
        Day,          // Daily view
        Year,         // Yearly view
        Agenda        // Agenda/list view
    }

    /// <summary>
    /// Calendar event data structure
    /// </summary>
    public class CalendarEvent
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now.AddHours(1);
        public Color Color { get; set; } = Color.Blue;
        public string Type { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsAllDay { get; set; } = false;
        public bool IsRecurring { get; set; } = false;
        public string RecurrencePattern { get; set; } = string.Empty;
        public List<string> Attendees { get; set; } = new List<string>();
        public object Tag { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Time slot data structure
    /// </summary>
    public class TimeSlot
    {
        public string Label { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
        public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
        public bool IsAvailable { get; set; } = true;
        public bool IsBooked { get; set; } = false;
        public bool IsWorkingHours { get; set; } = true;
        public string BookedBy { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public Color Color { get; set; } = Color.LightBlue;
        public object Tag { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}