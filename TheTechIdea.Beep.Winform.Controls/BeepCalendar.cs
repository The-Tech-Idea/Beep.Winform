using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Calendar")]
    [Description("A comprehensive calendar control with event management, multiple views, and scheduling capabilities.")]
    public class BeepCalendar : BaseControl
    {
        #region Fields
        private DateTime _currentDate = DateTime.Today;
        private CalendarViewMode _viewMode = CalendarViewMode.Month;
        private List<CalendarEvent> _events = new List<CalendarEvent>();
        private List<EventCategory> _categories = new List<EventCategory>();
        
        // Layout rectangles
        private Rectangle _headerRect;
        private Rectangle _viewSelectorRect;
        private Rectangle _navigationRect;
        private Rectangle _calendarGridRect;
        private Rectangle _sidebarRect;
        
        // Controls
        private BeepButton _prevButton;
        private BeepButton _nextButton;
        private BeepButton _todayButton;
        private BeepButton _monthViewButton;
        private BeepButton _weekViewButton;
        private BeepButton _dayViewButton;
        private BeepButton _listViewButton;
        private BeepButton _createEventButton;
        
        // Event handling
        private CalendarEvent _selectedEvent;
        private DateTime _selectedDate;
        private bool _showSidebar = true;
        
        // Constants
        private const int HeaderHeight = 60;
        private const int ViewSelectorHeight = 40;
        private const int SidebarWidth = 300;
        private const int MinCellHeight = 120;
        private const int DayHeaderHeight = 30;
        private const int TimeSlotHeight = 60;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Calendar")]
        [Description("Current date displayed in the calendar")]
        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                _selectedDate = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Calendar")]
        [Description("Current view mode of the calendar")]
        public CalendarViewMode ViewMode
        {
            get => _viewMode;
            set
            {
                _viewMode = value;
                UpdateLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Calendar")]
        [Description("Show sidebar with event details and mini calendar")]
        public bool ShowSidebar
        {
            get => _showSidebar;
            set
            {
                _showSidebar = value;
                UpdateLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Calendar")]
        [Description("List of events to display in the calendar")]
        public List<CalendarEvent> Events
        {
            get => _events;
            set
            {
                _events = value ?? new List<CalendarEvent>();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Calendar")]
        [Description("List of event categories with colors")]
        public List<EventCategory> Categories
        {
            get => _categories;
            set
            {
                _categories = value ?? new List<EventCategory>();
                InitializeDefaultCategories();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font for calendar headers")]
        public Font HeaderFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font for calendar day numbers")]
        public Font DayFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font for event text")]
        public Font EventFont { get; set; } = new Font("Segoe UI", 9, FontStyle.Regular);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font for time labels")]
        public Font TimeFont { get; set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        public Font DaysHeaderFont { get; private set; } = new Font("Segoe UI", 10, FontStyle.Regular);
        #endregion

        #region Events
        public event EventHandler<CalendarEventArgs> EventSelected;
        public event EventHandler<CalendarEventArgs> EventDoubleClick;
        public event EventHandler<CalendarDateArgs> DateSelected;
        public event EventHandler<CalendarEventArgs> CreateEventRequested;
        public event EventHandler<CalendarEventArgs> EditEventRequested;
        public event EventHandler<CalendarEventArgs> DeleteEventRequested;

        protected virtual void OnEventSelected(CalendarEvent calendarEvent)
        {
            _selectedEvent = calendarEvent;
            EventSelected?.Invoke(this, new CalendarEventArgs(calendarEvent));
            Invalidate();
        }

        protected virtual void OnDateSelected(DateTime date)
        {
            _selectedDate = date;
            DateSelected?.Invoke(this, new CalendarDateArgs(date));
            Invalidate();
        }

        protected virtual void OnCreateEventRequested(DateTime date)
        {
            CreateEventRequested?.Invoke(this, new CalendarEventArgs(new CalendarEvent
            {
                StartTime = date,
                EndTime = date.AddHours(1),
                Title = "New Event"
            }));
        }
        #endregion

        #region Constructor
        public BeepCalendar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            
            InitializeControls();
            InitializeDefaultCategories();
            Size = new Size(800, 600);
            UpdateLayout();
        }

        private void InitializeControls()
        {
            // Navigation buttons
            _prevButton = new BeepButton
            {
                Text = "◀",
                Size = new Size(30, 30),
                IsChild = true,
                Theme = Theme
            };
            _prevButton.Click += (s, e) => NavigatePrevious();

            _nextButton = new BeepButton
            {
                Text = "▶",
                Size = new Size(30, 30),
                IsChild = true,
                Theme = Theme
            };
            _nextButton.Click += (s, e) => NavigateNext();

            _todayButton = new BeepButton
            {
                Text = "Today",
                Size = new Size(60, 30),
                IsChild = true,
                Theme = Theme
            };
            _todayButton.Click += (s, e) => NavigateToday();

            // View mode buttons
            _monthViewButton = new BeepButton { Text = "Month", Size = new Size(60, 30), IsChild = true, Theme = Theme };
            _weekViewButton = new BeepButton { Text = "Week", Size = new Size(60, 30), IsChild = true, Theme = Theme };
            _dayViewButton = new BeepButton { Text = "Day", Size = new Size(60, 30), IsChild = true, Theme = Theme };
            _listViewButton = new BeepButton { Text = "List", Size = new Size(60, 30), IsChild = true, Theme = Theme };

            _monthViewButton.Click += (s, e) => ViewMode = CalendarViewMode.Month;
            _weekViewButton.Click += (s, e) => ViewMode = CalendarViewMode.Week;
            _dayViewButton.Click += (s, e) => ViewMode = CalendarViewMode.Day;
            _listViewButton.Click += (s, e) => ViewMode = CalendarViewMode.List;

            // Create event button
            _createEventButton = new BeepButton
            {
                Text = "+ Create Event",
                Size = new Size(100, 30),
                IsChild = true,
                Theme = Theme
            };
            _createEventButton.Click += (s, e) => OnCreateEventRequested(_selectedDate);

            // Add all controls
            Controls.AddRange(new Control[]
            {
                _prevButton, _nextButton, _todayButton,
                _monthViewButton, _weekViewButton, _dayViewButton, _listViewButton,
                _createEventButton
            });
        }

        private void InitializeDefaultCategories()
        {
            if (_categories == null || _categories.Count == 0)
            {
                _categories = new List<EventCategory>
                {
                    new EventCategory { Id = 1, Name = "Work", Color = Color.FromArgb(66, 133, 244) },
                    new EventCategory { Id = 2, Name = "Personal", Color = Color.FromArgb(52, 168, 83) },
                    new EventCategory { Id = 3, Name = "Meeting", Color = Color.FromArgb(251, 188, 5) },
                    new EventCategory { Id = 4, Name = "Holiday", Color = Color.FromArgb(234, 67, 53) },
                    new EventCategory { Id = 5, Name = "Birthday", Color = Color.FromArgb(156, 39, 176) }
                };
            }
        }
        #endregion

        #region Layout and Drawing
        private void UpdateLayout()
        {
            if (Width <= 0 || Height <= 0) return;

            // Header area
            _headerRect = new Rectangle(0, 0, Width, HeaderHeight);

            // View selector area
            _viewSelectorRect = new Rectangle(0, HeaderHeight, Width, ViewSelectorHeight);

            // Sidebar area (if enabled)
            int sidebarWidth = _showSidebar ? SidebarWidth : 0;
            _sidebarRect = new Rectangle(Width - sidebarWidth, HeaderHeight + ViewSelectorHeight, 
                                       sidebarWidth, Height - HeaderHeight - ViewSelectorHeight);

            // Main calendar area
            _calendarGridRect = new Rectangle(0, HeaderHeight + ViewSelectorHeight,
                                            Width - sidebarWidth, Height - HeaderHeight - ViewSelectorHeight);

            PositionControls();
        }

        private void PositionControls()
        {
            // Navigation controls
            _prevButton.Location = new Point(10, (_headerRect.Height - 30) / 2);
            _nextButton.Location = new Point(50, (_headerRect.Height - 30) / 2);
            _todayButton.Location = new Point(90, (_headerRect.Height - 30) / 2);

            // View mode buttons
            int viewButtonsStartX = _viewSelectorRect.Right - 260;
            _monthViewButton.Location = new Point(viewButtonsStartX, _viewSelectorRect.Y + 5);
            _weekViewButton.Location = new Point(viewButtonsStartX + 65, _viewSelectorRect.Y + 5);
            _dayViewButton.Location = new Point(viewButtonsStartX + 130, _viewSelectorRect.Y + 5);
            _listViewButton.Location = new Point(viewButtonsStartX + 195, _viewSelectorRect.Y + 5);

            // Create event button
            _createEventButton.Location = new Point(10, _viewSelectorRect.Y + 5);

            // Update button states
            UpdateViewButtonStates();
        }

        private void UpdateViewButtonStates()
        {
            Color selectedColor = _currentTheme?.CalendarSelectedDateBackColor ?? Color.FromArgb(66, 133, 244);
            Color normalColor = _currentTheme?.CalendarBackColor ?? Color.LightGray;

            _monthViewButton.BackColor = _viewMode == CalendarViewMode.Month ? selectedColor : normalColor;
            _weekViewButton.BackColor = _viewMode == CalendarViewMode.Week ? selectedColor : normalColor;
            _dayViewButton.BackColor = _viewMode == CalendarViewMode.Day ? selectedColor : normalColor;
            _listViewButton.BackColor = _viewMode == CalendarViewMode.List ? selectedColor : normalColor;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        protected override void DrawContent(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Draw header
            DrawHeader(g);

            // Draw view selector
            DrawViewSelector(g);

            // Draw main calendar based on view mode
            switch (_viewMode)
            {
                case CalendarViewMode.Month:
                    DrawMonthView(g);
                    break;
                case CalendarViewMode.Week:
                    DrawWeekView(g);
                    break;
                case CalendarViewMode.Day:
                    DrawDayView(g);
                    break;
                case CalendarViewMode.List:
                    DrawListView(g);
                    break;
            }

            // Draw sidebar
            if (_showSidebar)
            {
                DrawSidebar(g);
            }
        }

        private void DrawHeader(Graphics g)
        {
            // Background
            using (var brush = new SolidBrush(_currentTheme?.CalendarBackColor ?? BackColor))
            {
                g.FillRectangle(brush, _headerRect);
            }

            // Header text
            string headerText = GetHeaderText();
            using (var brush = new SolidBrush(_currentTheme?.CalendarTitleForColor ?? ForeColor))
            {
                var textRect = new Rectangle(_headerRect.X + 160, _headerRect.Y, _headerRect.Width - 320, _headerRect.Height);
                g.DrawString(headerText, HeaderFont, brush, textRect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawViewSelector(Graphics g)
        {
            // Background
            using (var brush = new SolidBrush(_currentTheme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250)))
            {
                g.FillRectangle(brush, _viewSelectorRect);
            }

            // Border
            using (var pen = new Pen(_currentTheme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224)))
            {
                g.DrawLine(pen, _viewSelectorRect.X, _viewSelectorRect.Bottom - 1, 
                          _viewSelectorRect.Right, _viewSelectorRect.Bottom - 1);
            }
        }
        #endregion

        #region View Drawing Methods
        private void DrawMonthView(Graphics g)
        {
            var firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
            
            // Calculate cell dimensions
            int cellWidth = _calendarGridRect.Width / 7;
            int cellHeight = (_calendarGridRect.Height - DayHeaderHeight) / 6;

            // Draw day headers
            DrawDayHeaders(g, cellWidth);

            // Draw calendar grid
            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var cellDate = firstDayOfCalendar.AddDays(week * 7 + day);
                    var cellRect = new Rectangle(
                        _calendarGridRect.X + day * cellWidth,
                        _calendarGridRect.Y + DayHeaderHeight + week * cellHeight,
                        cellWidth, cellHeight);

                    DrawMonthCell(g, cellDate, cellRect);
                }
            }
        }

        private void DrawWeekView(Graphics g)
        {
            var startOfWeek = _currentDate.AddDays(-(int)_currentDate.DayOfWeek);
            int cellWidth = _calendarGridRect.Width / 7;
            
            // Draw day headers with dates
            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = new Rectangle(
                    _calendarGridRect.X + day * cellWidth,
                    _calendarGridRect.Y,
                    cellWidth, DayHeaderHeight);

                DrawWeekDayHeader(g, dayDate, headerRect);
            }

            // Draw time slots
            DrawTimeSlots(g, startOfWeek, cellWidth);
        }

        private void DrawDayView(Graphics g)
        {
            // Single day header
            var headerRect = new Rectangle(_calendarGridRect.X, _calendarGridRect.Y, _calendarGridRect.Width, DayHeaderHeight);
            DrawDayHeader(g, _currentDate, headerRect);

            // Time slots for the day
            DrawTimeSlots(g, _currentDate, _calendarGridRect.Width);
        }

        private void DrawListView(Graphics g)
        {
            // Get events for current month
            var monthEvents = GetEventsForMonth(_currentDate);
            
            // Draw event list
            int yPos = _calendarGridRect.Y + 10;
            foreach (var evt in monthEvents.OrderBy(e => e.StartTime))
            {
                DrawListViewEvent(g, evt, new Rectangle(_calendarGridRect.X + 10, yPos, _calendarGridRect.Width - 20, 60));
                yPos += 70;
            }
        }

        private void DrawSidebar(Graphics g)
        {
            // Background using theme color
            using (var brush = new SolidBrush(_currentTheme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250)))
            {
                g.FillRectangle(brush, _sidebarRect);
            }

            // Border using theme color
            using (var pen = new Pen(_currentTheme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224)))
            {
                g.DrawLine(pen, _sidebarRect.X, _sidebarRect.Y, _sidebarRect.X, _sidebarRect.Bottom);
            }

            // Mini calendar
            DrawMiniCalendar(g);

            // Selected event details
            if (_selectedEvent != null)
            {
                DrawEventDetails(g);
            }
        }
        #endregion

        #region Helper Methods
        private string GetHeaderText()
        {
            return _viewMode switch
            {
                CalendarViewMode.Month => _currentDate.ToString("MMMM yyyy"),
                CalendarViewMode.Week => $"Week of {_currentDate.AddDays(-(int)_currentDate.DayOfWeek):MMMM dd, yyyy}",
                CalendarViewMode.Day => _currentDate.ToString("dddd, MMMM dd, yyyy"),
                CalendarViewMode.List => _currentDate.ToString("MMMM yyyy") + " Events",
                _ => _currentDate.ToString("MMMM yyyy")
            };
        }

        private void NavigatePrevious()
        {
            _currentDate = _viewMode switch
            {
                CalendarViewMode.Month => _currentDate.AddMonths(-1),
                CalendarViewMode.Week => _currentDate.AddDays(-7),
                CalendarViewMode.Day => _currentDate.AddDays(-1),
                CalendarViewMode.List => _currentDate.AddMonths(-1),
                _ => _currentDate.AddMonths(-1)
            };
            Invalidate();
        }

        private void NavigateNext()
        {
            _currentDate = _viewMode switch
            {
                CalendarViewMode.Month => _currentDate.AddMonths(1),
                CalendarViewMode.Week => _currentDate.AddDays(7),
                CalendarViewMode.Day => _currentDate.AddDays(1),
                CalendarViewMode.List => _currentDate.AddMonths(1),
                _ => _currentDate.AddMonths(1)
            };
            Invalidate();
        }

        private void NavigateToday()
        {
            _currentDate = DateTime.Today;
            _selectedDate = DateTime.Today;
            Invalidate();
        }

        private List<CalendarEvent> GetEventsForDate(DateTime date)
        {
            return _events.Where(e => e.StartTime.Date == date.Date).ToList();
        }

        private List<CalendarEvent> GetEventsForMonth(DateTime date)
        {
            var firstDay = new DateTime(date.Year, date.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);
            return _events.Where(e => e.StartTime.Date >= firstDay && e.StartTime.Date <= lastDay).ToList();
        }
        #endregion

        #region Drawing Helper Methods
        private void DrawMonthCell(Graphics g, DateTime cellDate, Rectangle cellRect)
        {
            bool isCurrentMonth = cellDate.Month == _currentDate.Month;
            bool isToday = cellDate.Date == DateTime.Today;
            bool isSelected = cellDate.Date == _selectedDate.Date;

            // Cell background using theme colors
            Color bgColor = isSelected ? (_currentTheme?.CalendarSelectedDateBackColor ?? Color.FromArgb(66, 133, 244)) :
                           isToday ? (_currentTheme?.CalendarHoverBackColor ?? Color.FromArgb(230, 240, 255)) :
                           isCurrentMonth ? (_currentTheme?.CalendarBackColor ?? Color.White) : 
                           Color.FromArgb(248, 249, 250);

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, cellRect);
            }

            // Cell border using theme color
            using (var pen = new Pen(_currentTheme?.CalendarBorderColor ?? Color.FromArgb(218, 220, 224)))
            {
                g.DrawRectangle(pen, cellRect);
            }

            // Day number using theme colors
            Color textColor = isSelected ? (_currentTheme?.CalendarSelectedDateForColor ?? Color.White) : 
                             isToday ? (_currentTheme?.CalendarTodayForeColor ?? Color.FromArgb(244, 67, 54)) :
                             isCurrentMonth ? (_currentTheme?.CalendarForeColor ?? Color.Black) : Color.Gray;
            
            using (var brush = new SolidBrush(textColor))
            {
                var dayRect = new Rectangle(cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, 20);
                g.DrawString(cellDate.Day.ToString(), DayFont, brush, dayRect);
            }

            // Events
            var events = GetEventsForDate(cellDate);
            int eventY = cellRect.Y + 25;
            foreach (var evt in events.Take(3)) // Show max 3 events
            {
                DrawEventBar(g, evt, new Rectangle(cellRect.X + 2, eventY, cellRect.Width - 4, 16));
                eventY += 18;
            }

            if (events.Count > 3)
            {
                using (var brush = new SolidBrush(_currentTheme?.CalendarForeColor ?? Color.Gray))
                {
                    g.DrawString($"+{events.Count - 3} more", new Font(EventFont.FontFamily, 8), 
                               brush, new PointF(cellRect.X + 2, eventY));
                }
            }
        }

        private void DrawEventBar(Graphics g, CalendarEvent evt, Rectangle rect)
        {
            var category = _categories.FirstOrDefault(c => c.Id == evt.CategoryId);
            Color eventColor = category?.Color ?? Color.Gray;

            using (var brush = new SolidBrush(eventColor))
            {
                using (var path = GetRoundedRectPath(rect, 3))
                {
                    g.FillPath(brush, path);
                }
            }

            using (var brush = new SolidBrush(Color.White))
            {
                g.DrawString(evt.Title, EventFont, brush, rect, 
                           new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawDayHeaders(Graphics g, int cellWidth)
        {
            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            
            for (int i = 0; i < 7; i++)
            {
                var headerRect = new Rectangle(
                    _calendarGridRect.X + i * cellWidth,
                    _calendarGridRect.Y,
                    cellWidth, DayHeaderHeight);

                // Use theme colors for day headers
                using (var brush = new SolidBrush(_currentTheme?.CalendarBackColor ?? Color.FromArgb(248, 249, 250)))
                {
                    g.FillRectangle(brush, headerRect);
                }

                using (var brush = new SolidBrush(_currentTheme?.CalendarDaysHeaderForColor ?? Color.FromArgb(95, 99, 104)))
                {
                    g.DrawString(dayNames[i], DaysHeaderFont, brush, headerRect,
                               new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }
            }
        }

        private void DrawTimeSlots(Graphics g, DateTime startDate, int cellWidth)
        {
            int timeSlotCount = 24; // 24 hours
            int slotHeight = Math.Max(TimeSlotHeight, (_calendarGridRect.Height - DayHeaderHeight) / timeSlotCount);

            for (int hour = 0; hour < timeSlotCount; hour++)
            {
                int yPos = _calendarGridRect.Y + DayHeaderHeight + hour * slotHeight;
                
                // Time label
                var timeRect = new Rectangle(_calendarGridRect.X, yPos, 60, slotHeight);
                using (var brush = new SolidBrush(Color.Gray))
                {
                    g.DrawString($"{hour:00}:00", TimeFont, brush, timeRect,
                               new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }

                // Time slot grid
                using (var pen = new Pen(Color.FromArgb(218, 220, 224)))
                {
                    g.DrawLine(pen, _calendarGridRect.X + 60, yPos, _calendarGridRect.Right, yPos);
                }

                // Draw events for this time slot
                if (_viewMode == CalendarViewMode.Week)
                {
                    for (int day = 0; day < 7; day++)
                    {
                        var dayDate = startDate.AddDays(day);
                        var dayEvents = GetEventsForDate(dayDate)
                            .Where(e => e.StartTime.Hour == hour)
                            .ToList();

                        foreach (var evt in dayEvents)
                        {
                            var eventRect = new Rectangle(
                                _calendarGridRect.X + 60 + day * cellWidth,
                                yPos,
                                cellWidth,
                                (int)(evt.Duration.TotalHours * slotHeight));

                            DrawEventBlock(g, evt, eventRect);
                        }
                    }
                }
                else if (_viewMode == CalendarViewMode.Day)
                {
                    var dayEvents = GetEventsForDate(startDate)
                        .Where(e => e.StartTime.Hour == hour)
                        .ToList();

                    foreach (var evt in dayEvents)
                    {
                        var eventRect = new Rectangle(
                            _calendarGridRect.X + 80,
                            yPos,
                            _calendarGridRect.Width - 100,
                            (int)(evt.Duration.TotalHours * slotHeight));

                        DrawEventBlock(g, evt, eventRect);
                    }
                }
            }
        }

        private void DrawEventBlock(Graphics g, CalendarEvent evt, Rectangle rect)
        {
            var category = _categories.FirstOrDefault(c => c.Id == evt.CategoryId);
            Color eventColor = category?.Color ?? Color.Gray;

            using (var brush = new SolidBrush(Color.FromArgb(200, eventColor)))
            {
                using (var path = GetRoundedRectPath(rect, 5))
                {
                    g.FillPath(brush, path);
                }
            }

            using (var pen = new Pen(eventColor, 2))
            {
                using (var path = GetRoundedRectPath(rect, 5))
                {
                    g.DrawPath(pen, path);
                }
            }

            using (var brush = new SolidBrush(Color.White))
            {
                var textRect = Rectangle.Inflate(rect, -5, -2);
                g.DrawString($"{evt.Title}\n{evt.StartTime:HH:mm} - {evt.EndTime:HH:mm}", 
                           EventFont, brush, textRect);
            }
        }

        private void DrawMiniCalendar(Graphics g)
        {
            // Implementation for mini calendar in sidebar
            var miniRect = new Rectangle(_sidebarRect.X + 10, _sidebarRect.Y + 10, _sidebarRect.Width - 20, 200);
            
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, miniRect);
            }

            using (var pen = new Pen(Color.FromArgb(218, 220, 224)))
            {
                g.DrawRectangle(pen, miniRect);
            }

            // Mini calendar title
            using (var brush = new SolidBrush(Color.Black))
            {
                g.DrawString(_currentDate.ToString("MMMM yyyy"), new Font(DayFont.FontFamily, 10, FontStyle.Bold),
                           brush, new Rectangle(miniRect.X + 5, miniRect.Y + 5, miniRect.Width - 10, 20),
                           new StringFormat { Alignment = StringAlignment.Center });
            }
        }

        private void DrawEventDetails(Graphics g)
        {
            var detailsRect = new Rectangle(_sidebarRect.X + 10, _sidebarRect.Y + 230, _sidebarRect.Width - 20, 200);
            
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, detailsRect);
            }

            using (var pen = new Pen(Color.FromArgb(218, 220, 224)))
            {
                g.DrawRectangle(pen, detailsRect);
            }

            // Event details
            var category = _categories.FirstOrDefault(c => c.Id == _selectedEvent.CategoryId);
            string details = $"Title: {_selectedEvent.Title}\n" +
                           $"Date: {_selectedEvent.StartTime:MMM dd, yyyy}\n" +
                           $"Time: {_selectedEvent.StartTime:HH:mm} - {_selectedEvent.EndTime:HH:mm}\n" +
                           $"Category: {category?.Name ?? "None"}\n" +
                           $"Description: {_selectedEvent.Description}";

            using (var brush = new SolidBrush(Color.Black))
            {
                g.DrawString(details, EventFont, brush, Rectangle.Inflate(detailsRect, -10, -10));
            }
        }

        private void DrawListViewEvent(Graphics g, CalendarEvent evt, Rectangle rect)
        {
            var category = _categories.FirstOrDefault(c => c.Id == evt.CategoryId);
            Color eventColor = category?.Color ?? Color.Gray;

            // Event background
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, rect);
            }

            // Color bar
            using (var brush = new SolidBrush(eventColor))
            {
                g.FillRectangle(brush, new Rectangle(rect.X, rect.Y, 4, rect.Height));
            }

            // Event text
            string eventText = $"{evt.Title}\n{evt.StartTime:MMM dd, yyyy HH:mm} - {evt.EndTime:HH:mm}";
            if (!string.IsNullOrEmpty(evt.Description))
            {
                eventText += $"\n{evt.Description}";
            }

            using (var brush = new SolidBrush(Color.Black))
            {
                g.DrawString(eventText, EventFont, brush, new Rectangle(rect.X + 10, rect.Y + 5, rect.Width - 15, rect.Height - 10));
            }

            // Border
            using (var pen = new Pen(Color.FromArgb(218, 220, 224)))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        private void DrawWeekDayHeader(Graphics g, DateTime date, Rectangle rect)
        {
            bool isToday = date.Date == DateTime.Today;
            
            using (var brush = new SolidBrush(isToday ? Color.FromArgb(66, 133, 244) : Color.FromArgb(248, 249, 250)))
            {
                g.FillRectangle(brush, rect);
            }

            using (var brush = new SolidBrush(isToday ? Color.White : Color.Black))
            {
                string headerText = $"{date:ddd}\n{date:dd}";
                g.DrawString(headerText, DayFont, brush, rect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawDayHeader(Graphics g, DateTime date, Rectangle rect)
        {
            using (var brush = new SolidBrush(Color.FromArgb(248, 249, 250)))
            {
                g.FillRectangle(brush, rect);
            }

            using (var brush = new SolidBrush(Color.Black))
            {
                g.DrawString(date.ToString("dddd, MMMM dd"), HeaderFont, brush, rect,
                           new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }
        #endregion

        #region Mouse Handling
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (_viewMode == CalendarViewMode.Month)
            {
                HandleMonthViewClick(e.Location);
            }
            else if (_viewMode == CalendarViewMode.Week || _viewMode == CalendarViewMode.Day)
            {
                HandleTimeSlotClick(e.Location);
            }
            else if (_viewMode == CalendarViewMode.List)
            {
                HandleListViewClick(e.Location);
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            
            if (_selectedEvent != null)
            {
                EventDoubleClick?.Invoke(this, new CalendarEventArgs(_selectedEvent));
            }
        }

        private void HandleMonthViewClick(Point location)
        {
            var firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
            
            int cellWidth = _calendarGridRect.Width / 7;
            int cellHeight = (_calendarGridRect.Height - DayHeaderHeight) / 6;

            if (location.Y >= _calendarGridRect.Y + DayHeaderHeight)
            {
                int col = (location.X - _calendarGridRect.X) / cellWidth;
                int row = (location.Y - _calendarGridRect.Y - DayHeaderHeight) / cellHeight;

                if (col >= 0 && col < 7 && row >= 0 && row < 6)
                {
                    var clickedDate = firstDayOfCalendar.AddDays(row * 7 + col);
                    OnDateSelected(clickedDate);

                    // Check if clicked on an event
                    var events = GetEventsForDate(clickedDate);
                    if (events.Any())
                    {
                        int eventY = 25;
                        int relativeY = location.Y - (_calendarGridRect.Y + DayHeaderHeight + row * cellHeight);
                        
                        foreach (var evt in events.Take(3))
                        {
                            if (relativeY >= eventY && relativeY <= eventY + 16)
                            {
                                OnEventSelected(evt);
                                break;
                            }
                            eventY += 18;
                        }
                    }
                }
            }
        }

        private void HandleTimeSlotClick(Point location)
        {
            // Implementation for time slot clicks in week/day view
            if (location.X > _calendarGridRect.X + 60 && location.Y > _calendarGridRect.Y + DayHeaderHeight)
            {
                int slotHeight = Math.Max(TimeSlotHeight, (_calendarGridRect.Height - DayHeaderHeight) / 24);
                int hour = (location.Y - _calendarGridRect.Y - DayHeaderHeight) / slotHeight;
                
                DateTime clickedDateTime;
                if (_viewMode == CalendarViewMode.Week)
                {
                    var startOfWeek = _currentDate.AddDays(-(int)_currentDate.DayOfWeek);
                    int cellWidth = (_calendarGridRect.Width - 60) / 7;
                    int day = (location.X - _calendarGridRect.X - 60) / cellWidth;
                    clickedDateTime = startOfWeek.AddDays(day).AddHours(hour);
                }
                else
                {
                    clickedDateTime = _currentDate.Date.AddHours(hour);
                }

                OnDateSelected(clickedDateTime);
            }
        }

        private void HandleListViewClick(Point location)
        {
            // Implementation for list view clicks
            var monthEvents = GetEventsForMonth(_currentDate).OrderBy(e => e.StartTime).ToList();
            
            int yPos = _calendarGridRect.Y + 10;
            foreach (var evt in monthEvents)
            {
                var eventRect = new Rectangle(_calendarGridRect.X + 10, yPos, _calendarGridRect.Width - 20, 60);
                if (eventRect.Contains(location))
                {
                    OnEventSelected(evt);
                    break;
                }
                yPos += 70;
            }
        }
        #endregion

        #region Public Methods
        public void AddEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent != null)
            {
                _events.Add(calendarEvent);
                Invalidate();
            }
        }

        public void RemoveEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent != null && _events.Contains(calendarEvent))
            {
                _events.Remove(calendarEvent);
                if (_selectedEvent == calendarEvent)
                {
                    _selectedEvent = null;
                }
                Invalidate();
            }
        }

        public void UpdateEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent != null)
            {
                var existingEvent = _events.FirstOrDefault(e => e.Id == calendarEvent.Id);
                if (existingEvent != null)
                {
                    int index = _events.IndexOf(existingEvent);
                    _events[index] = calendarEvent;
                    Invalidate();
                }
            }
        }

        public List<CalendarEvent> GetEventsForDateRange(DateTime startDate, DateTime endDate)
        {
            return _events.Where(e => e.StartTime.Date >= startDate.Date && e.StartTime.Date <= endDate.Date).ToList();
        }
        #endregion

        #region Theme Support
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null) return;

            // Apply calendar-specific theme properties
            BackColor = _currentTheme.CalendarBackColor;
            ForeColor = _currentTheme.CalendarForeColor;

            // Apply fonts if they exist in the theme and UseThemeFont is enabled
            if (UseThemeFont)
            {
                if (_currentTheme.CalendarTitleFont != null)
                    HeaderFont = FontListHelper.CreateFontFromTypography(_currentTheme.CalendarTitleFont);
                if (_currentTheme.DaysHeaderFont != null)
                    DaysHeaderFont = FontListHelper.CreateFontFromTypography(_currentTheme.DaysHeaderFont);
                if (_currentTheme.DateFont != null)
                    DayFont = FontListHelper.CreateFontFromTypography(_currentTheme.DateFont);
                if (_currentTheme.CalendarSelectedFont != null)
                    EventFont = FontListHelper.CreateFontFromTypography(_currentTheme.CalendarSelectedFont);
                if (_currentTheme.CalendarUnSelectedFont != null)
                    TimeFont = FontListHelper.CreateFontFromTypography(_currentTheme.CalendarUnSelectedFont);
            }

            // Apply theme to child controls
            foreach (Control control in Controls)
            {
                if (control is BeepButton button)
                {
                    button.Theme = Theme;
                   // button.ApplyTheme();
                }
            }

            // Update view button states with theme colors
            UpdateViewButtonStates();
            UpdateLayout();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _events?.Clear();
                _categories?.Clear();
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    #region Supporting Classes
    public enum CalendarViewMode
    {
        Month,
        Week,
        Day,
        List
    }

    public class CalendarEvent
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int CategoryId { get; set; }
        public string Location { get; set; } = "";
        public bool IsAllDay { get; set; }
        public string Organizer { get; set; } = "";
        public List<string> Tags { get; set; } = new List<string>();

        public TimeSpan Duration => EndTime - StartTime;
    }

    public class EventCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public Color Color { get; set; }
        public string Description { get; set; } = "";
    }

    public class CalendarEventArgs : EventArgs
    {
        public CalendarEvent Event { get; }

        public CalendarEventArgs(CalendarEvent calendarEvent)
        {
            Event = calendarEvent;
        }
    }

    public class CalendarDateArgs : EventArgs
    {
        public DateTime Date { get; }

        public CalendarDateArgs(DateTime date)
        {
            Date = date;
        }
    }
    #endregion
}
