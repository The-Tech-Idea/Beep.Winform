using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
 
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
 
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Calendar")]
    [Description("A comprehensive calendar control with event management, multiple views, and scheduling capabilities.")]
    public class BeepCalendar : BaseControl
    {
        private readonly CalendarState _state = new CalendarState();
        private readonly CalendarRects _rects = new CalendarRects();
        private CalendarLayoutManager _layout;
        private CalendarEventService _eventService;
        private CalendarRenderer _renderer;
        
        // Painter system
        private ICalendarStylePainter _stylePainter;
        private BeepControlStyle _calendarStyle = BeepControlStyle.Material3;
        private bool _usePainterSystem = true;
        
        // Hover tracking
        private DateTime? _hoveredDate;
        private CalendarEvent _hoveredEvent;

        // Data
        private List<CalendarEvent> _events = new();
        private List<EventCategory> _categories = new();

        // Controls
        private BeepButton _prevButton;
        private BeepButton _nextButton;
        private BeepButton _todayButton;
        private BeepButton _monthViewButton;
        private BeepButton _weekViewButton;
        private BeepButton _dayViewButton;
        private BeepButton _listViewButton;
        private BeepButton _createEventButton;

        private bool _controlsInitialized;
        private bool IsDesignModeSafe => LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode || (Site?.DesignMode ?? false);

        // Exposed paddings for title & grid
        [Browsable(true)] [Category("Layout")] public int HeaderLeftPadding { get; set; } = 160; // min
        [Browsable(true)] [Category("Layout")] public int HeaderRightPadding { get; set; } = 20;
        [Browsable(true)] [Category("Layout")] public int GridLeftGutter { get; set; } = 12;
        
        /// <summary>
        /// Gets or sets the visual style for the calendar
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style for the calendar")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle CalendarStyle
        {
            get => _calendarStyle;
            set
            {
                if (_calendarStyle != value)
                {
                    _calendarStyle = value;
                    _stylePainter = CalendarPainterFactory.GetPainter(value);
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether to use the new painter system
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use the new style painter system for rendering")]
        [DefaultValue(true)]
        public bool UsePainterSystem
        {
            get => _usePainterSystem;
            set
            {
                if (_usePainterSystem != value)
                {
                    _usePainterSystem = value;
                    Invalidate();
                }
            }
        }

        public BeepCalendar():base()
        {
            ShowAllBorders = true;

            try
            {
                InitializeControls();
                _controlsInitialized = true;
            }
            catch { /* designer safety */ }

            InitializeDefaultCategories();
            Size = new Size(800, 600);
            _eventService = new CalendarEventService(_events);
            _layout = new CalendarLayoutManager(this, _state, _rects);
            _renderer = new CalendarRenderer();
            
            // Initialize style painter
            _stylePainter = CalendarPainterFactory.GetPainter(_calendarStyle);
            
            if (!IsDesignModeSafe)
            {
                UpdateLayout();
            }
        }

        [Browsable(true)]
        [Category("Calendar")] public DateTime CurrentDate { get => _state.CurrentDate; set { _state.CurrentDate = value; _state.SelectedDate = value; Invalidate(); } }
        [Browsable(true)]
        [Category("Calendar")] public CalendarViewMode ViewMode { get => _state.ViewMode; set { _state.ViewMode = value; UpdateLayout(); Invalidate(); } }
        [Browsable(true)]
        [Category("Calendar")] public bool ShowSidebar { get => _state.ShowSidebar; set { _state.ShowSidebar = value; UpdateLayout(); Invalidate(); } }

        [Browsable(true)]
        [Category("Calendar")] 
        public List<CalendarEvent> Events
        {
            get => _events;
            set 
            { 
                _events = value ?? new(); 
                _eventService = new CalendarEventService(_events);
                Invalidate(); 
            }
        }

        [Browsable(true)]
        [Category("Calendar")] public List<EventCategory> Categories
        {
            get => _categories;
            set { _categories = value ?? new(); InitializeDefaultCategories(); Invalidate(); }
        }

        [Browsable(true)] [Category("Appearance")] public Font HeaderFont { get; set; } = new Font("Segoe UI", 16, FontStyle.Bold);
        [Browsable(true)] [Category("Appearance")] public Font DayFont { get; set; } = new Font("Segoe UI", 12);
        [Browsable(true)] [Category("Appearance")] public Font EventFont { get; set; } = new Font("Segoe UI", 9);
        [Browsable(true)] [Category("Appearance")] public Font TimeFont { get; set; } = new Font("Segoe UI", 10);
        public Font DaysHeaderFont { get; private set; } = new Font("Segoe UI", 10);

        public event EventHandler<CalendarEventArgs> EventSelected;
        public event EventHandler<CalendarEventArgs> EventDoubleClick;
        public event EventHandler<CalendarDateArgs> DateSelected;
        public event EventHandler<CalendarEventArgs> CreateEventRequested;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!IsDesignModeSafe)
            {
                UpdateLayout();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!IsDesignModeSafe)
            {
                UpdateLayout();
            }
        }

        protected override void DrawContent(Graphics g)
        {
            // Design-time placeholder rendering
            if (IsDesignModeSafe)
            {
                PaintDesignTimePlaceholder(g);
                return;
            }
            
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get the actual content rectangle from BaseControl, like BeepComboBox does
            Rectangle contentRect = GetContentRectForDrawing();

            if (_usePainterSystem && _stylePainter != null)
            {
                // Use new painter system
                DrawWithPainter(g, contentRect);
            }
            else
            {
                // Use legacy renderer
                DrawWithLegacyRenderer(g, contentRect);
            }
        }
        
        /// <summary>
        /// Draw using the new style painter system
        /// </summary>
        private void DrawWithPainter(Graphics g, Rectangle contentRect)
        {
            // Begin paint cycle for event caching
            _eventService?.BeginPaintCycle();
            
            // Create painter context with StyleColors
            var painterCtx = CalendarPainterFactory.CreateContext(
                _calendarStyle,
                _currentTheme,
                UseThemeColors,
                _state,
                _rects,
                _eventService,
                _categories,
                HeaderFont,
                DayFont,
                EventFont,
                TimeFont,
                DaysHeaderFont
            );

            // Paint background
            _stylePainter.PaintBackground(g, contentRect, painterCtx);

            // Paint header
            int leftAnchor = Math.Max(_todayButton?.Right ?? 0, _nextButton?.Right ?? 0);
            int headerLeft = Math.Max(leftAnchor + 20, HeaderLeftPadding);
            var headerTextRect = new Rectangle(
                headerLeft, 
                _rects.HeaderRect.Y, 
                _rects.HeaderRect.Width - headerLeft - HeaderRightPadding, 
                _rects.HeaderRect.Height
            );
            _stylePainter.PaintHeader(g, _rects.HeaderRect, GetHeaderText(), painterCtx);

            // Paint view selector
            _stylePainter.PaintViewSelector(g, _rects.ViewSelectorRect, painterCtx);

            // Paint main calendar based on view mode
            switch (_state.ViewMode)
            {
                case CalendarViewMode.Month:
                    DrawMonthViewWithPainter(g, painterCtx);
                    break;
                case CalendarViewMode.Week:
                    DrawWeekViewWithPainter(g, painterCtx);
                    break;
                case CalendarViewMode.Day:
                    DrawDayViewWithPainter(g, painterCtx);
                    break;
                case CalendarViewMode.List:
                    DrawListViewWithPainter(g, painterCtx);
                    break;
            }

            // Paint sidebar
            if (_state.ShowSidebar && _rects.SidebarRect.Width > 0)
            {
                DrawSidebarWithPainter(g, painterCtx);
            }
        }
        
        /// <summary>
        /// Draw using the legacy renderer system
        /// </summary>
        private void DrawWithLegacyRenderer(Graphics g, Rectangle contentRect)
        {
            // Begin paint cycle for event caching
            _eventService?.BeginPaintCycle();
            
            // Dynamic header left margin from right-most of nav buttons
            int leftAnchor = Math.Max(_todayButton?.Right ?? 0, _nextButton?.Right ?? 0);
            int headerLeft = Math.Max(leftAnchor + 20, HeaderLeftPadding);
            int headerRight = HeaderRightPadding;

            var ctx = new CalendarRenderContext(this, _currentTheme,
                HeaderFont, DayFont, EventFont, TimeFont, DaysHeaderFont,
                _state, _rects, _eventService, _categories,
                headerLeft, headerRight);

            _renderer.Draw(g, ctx);
        }
        
        private string GetHeaderText()
        {
            var d = _state.CurrentDate;
            return _state.ViewMode switch
            {
                CalendarViewMode.Month => d.ToString("MMMM yyyy"),
                CalendarViewMode.Week => $"Week of {d.AddDays(-(int)d.DayOfWeek):MMMM dd, yyyy}",
                CalendarViewMode.Day => d.ToString("dddd, MMMM dd, yyyy"),
                CalendarViewMode.List => d.ToString("MMMM yyyy") + " Events",
                _ => d.ToString("MMMM yyyy")
            };
        }
        
        #region Painter-based View Drawing
        
        private void DrawMonthViewWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var grid = _rects.CalendarGridRect;
            var firstDayOfMonth = new DateTime(_state.CurrentDate.Year, _state.CurrentDate.Month, 1);
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);

            int cellWidth = grid.Width / 7;
            int cellHeight = (grid.Height - CalendarLayoutMetrics.DayHeaderHeight) / 6;

            // Day headers
            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            for (int i = 0; i < 7; i++)
            {
                var headerRect = new Rectangle(grid.X + i * cellWidth, grid.Y, cellWidth, CalendarLayoutMetrics.DayHeaderHeight);
                bool isToday = (int)DateTime.Today.DayOfWeek == i;
                _stylePainter.PaintDayHeader(g, headerRect, dayNames[i], isToday, ctx);
            }

            // Day cells
            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var cellDate = firstDayOfCalendar.AddDays(week * 7 + day);
                    var cellRect = new Rectangle(
                        grid.X + day * cellWidth, 
                        grid.Y + CalendarLayoutMetrics.DayHeaderHeight + week * cellHeight, 
                        cellWidth, 
                        cellHeight
                    );

                    var state = new DayCellState
                    {
                        IsCurrentMonth = cellDate.Month == _state.CurrentDate.Month,
                        IsToday = cellDate.Date == DateTime.Today,
                        IsSelected = cellDate.Date == _state.SelectedDate.Date,
                        IsHovered = _hoveredDate.HasValue && cellDate.Date == _hoveredDate.Value.Date,
                        IsWeekend = cellDate.DayOfWeek == DayOfWeek.Saturday || cellDate.DayOfWeek == DayOfWeek.Sunday,
                        EventCount = _eventService.GetEventsForDate(cellDate).Count
                    };

                    _stylePainter.PaintDayCell(g, cellRect, cellDate, state, ctx);

                    // Draw events in cell
                    var events = _eventService.GetEventsForDate(cellDate);
                    int eventY = cellRect.Y + 30;
                    foreach (var evt in events.Take(3))
                    {
                        var eventRect = new Rectangle(cellRect.X + 2, eventY, cellRect.Width - 4, 16);
                        bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                        bool isHovered = _hoveredEvent?.Id == evt.Id;
                        _stylePainter.PaintEventBar(g, eventRect, evt, isSelected, isHovered, ctx);
                        eventY += 18;
                    }

                    if (events.Count > 3)
                    {
                        using (var brush = new SolidBrush(ctx.ForegroundColor))
                        {
                            g.DrawString($"+{events.Count - 3} more", new Font(EventFont.FontFamily, 8), 
                                brush, new PointF(cellRect.X + 2, eventY));
                        }
                    }
                }
            }
        }
        
        private void DrawWeekViewWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var grid = _rects.CalendarGridRect;
            var startOfWeek = _state.CurrentDate.AddDays(-(int)_state.CurrentDate.DayOfWeek);
            int cellWidth = (grid.Width - CalendarLayoutMetrics.TimeColumnWidth) / 7;

            // Day headers
            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = new Rectangle(
                    grid.X + CalendarLayoutMetrics.TimeColumnWidth + day * cellWidth, 
                    grid.Y, 
                    cellWidth, 
                    CalendarLayoutMetrics.DayHeaderHeight
                );
                _stylePainter.PaintWeekDayHeader(g, headerRect, dayDate, dayDate.Date == DateTime.Today, ctx);
            }

            // Time slots
            int slotHeight = Math.Max(CalendarLayoutMetrics.TimeSlotHeight, (grid.Height - CalendarLayoutMetrics.DayHeaderHeight) / 24);
            int currentHour = DateTime.Now.Hour;

            for (int hour = 0; hour < 24; hour++)
            {
                int yPos = grid.Y + CalendarLayoutMetrics.DayHeaderHeight + hour * slotHeight;
                
                // Time label
                var timeLabelRect = new Rectangle(grid.X, yPos, CalendarLayoutMetrics.TimeColumnWidth, slotHeight);
                _stylePainter.PaintTimeLabel(g, timeLabelRect, hour, ctx);

                // Time slot for each day
                for (int day = 0; day < 7; day++)
                {
                    var slotRect = new Rectangle(
                        grid.X + CalendarLayoutMetrics.TimeColumnWidth + day * cellWidth, 
                        yPos, 
                        cellWidth, 
                        slotHeight
                    );
                    _stylePainter.PaintTimeSlot(g, slotRect, hour, hour == currentHour && startOfWeek.AddDays(day).Date == DateTime.Today, ctx);

                    // Draw events
                    var dayDate = startOfWeek.AddDays(day);
                    var dayEvents = _eventService.GetEventsForDate(dayDate)
                        .Where(e => e.StartTime.Hour == hour)
                        .ToList();

                    foreach (var evt in dayEvents)
                    {
                        var eventRect = new Rectangle(
                            slotRect.X + 2, 
                            yPos, 
                            slotRect.Width - 4, 
                            (int)(evt.Duration.TotalHours * slotHeight)
                        );
                        bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                        bool isHovered = _hoveredEvent?.Id == evt.Id;
                        _stylePainter.PaintEventBlock(g, eventRect, evt, isSelected, isHovered, ctx);
                    }
                }
            }
        }
        
        private void DrawDayViewWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var grid = _rects.CalendarGridRect;
            
            // Day header
            var headerRect = new Rectangle(grid.X, grid.Y, grid.Width, CalendarLayoutMetrics.DayHeaderHeight);
            _stylePainter.PaintWeekDayHeader(g, headerRect, _state.CurrentDate, _state.CurrentDate.Date == DateTime.Today, ctx);

            // Time slots
            int slotHeight = Math.Max(CalendarLayoutMetrics.TimeSlotHeight, (grid.Height - CalendarLayoutMetrics.DayHeaderHeight) / 24);
            int currentHour = DateTime.Now.Hour;

            for (int hour = 0; hour < 24; hour++)
            {
                int yPos = grid.Y + CalendarLayoutMetrics.DayHeaderHeight + hour * slotHeight;
                
                // Time label
                var timeLabelRect = new Rectangle(grid.X, yPos, CalendarLayoutMetrics.TimeColumnWidth, slotHeight);
                _stylePainter.PaintTimeLabel(g, timeLabelRect, hour, ctx);

                // Time slot
                var slotRect = new Rectangle(
                    grid.X + CalendarLayoutMetrics.TimeColumnWidth, 
                    yPos, 
                    grid.Width - CalendarLayoutMetrics.TimeColumnWidth, 
                    slotHeight
                );
                _stylePainter.PaintTimeSlot(g, slotRect, hour, hour == currentHour && _state.CurrentDate.Date == DateTime.Today, ctx);

                // Draw events
                var dayEvents = _eventService.GetEventsForDate(_state.CurrentDate)
                    .Where(e => e.StartTime.Hour == hour)
                    .ToList();

                foreach (var evt in dayEvents)
                {
                    var eventRect = new Rectangle(
                        slotRect.X + 10, 
                        yPos, 
                        slotRect.Width - 20, 
                        (int)(evt.Duration.TotalHours * slotHeight)
                    );
                    bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                    bool isHovered = _hoveredEvent?.Id == evt.Id;
                    _stylePainter.PaintEventBlock(g, eventRect, evt, isSelected, isHovered, ctx);
                }
            }
        }
        
        private void DrawListViewWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var grid = _rects.CalendarGridRect;
            var monthEvents = _eventService.GetEventsForMonth(_state.CurrentDate)
                .OrderBy(e => e.StartTime)
                .ToList();

            int yPos = grid.Y + 10;
            foreach (var evt in monthEvents)
            {
                if (yPos + 70 > grid.Bottom) break;
                
                var eventRect = new Rectangle(grid.X + 10, yPos, grid.Width - 20, 60);
                bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                bool isHovered = _hoveredEvent?.Id == evt.Id;
                _stylePainter.PaintListViewEvent(g, eventRect, evt, isSelected, isHovered, ctx);
                yPos += 70;
            }
        }
        
        private void DrawSidebarWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var rect = _rects.SidebarRect;
            _stylePainter.PaintSidebar(g, rect, ctx);

            // Mini calendar
            var miniRect = new Rectangle(rect.X + 10, rect.Y + 10, Math.Max(10, rect.Width - 20), 200);
            if (miniRect.Width > 20)
            {
                _stylePainter.PaintMiniCalendar(g, miniRect, _state.CurrentDate, _state.SelectedDate, ctx);
            }

            // Event details
            if (_state.SelectedEvent != null)
            {
                var detailsRect = new Rectangle(rect.X + 10, rect.Y + 230, Math.Max(10, rect.Width - 20), 200);
                if (detailsRect.Width > 20)
                {
                    _stylePainter.PaintEventDetails(g, detailsRect, _state.SelectedEvent, ctx);
                }
            }
        }
        
        #endregion
        
        /// <summary>
        /// Simple placeholder rendering for Visual Studio Designer
        /// </summary>
        private void PaintDesignTimePlaceholder(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(248, 249, 250)), ClientRectangle);
            using (var pen = new Pen(Color.FromArgb(218, 220, 224), 1))
            {
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
            using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
            {
                g.DrawString("BeepCalendar (Design Mode)", font, brush, 10, 10);
            }
            using (var font = new Font("Segoe UI", 10))
            using (var brush = new SolidBrush(Color.FromArgb(120, 120, 120)))
            {
                g.DrawString($"View: {_state.ViewMode}", font, brush, 10, 35);
                g.DrawString($"Date: {_state.CurrentDate:MMMM yyyy}", font, brush, 10, 55);
                g.DrawString($"Events: {_events?.Count ?? 0}", font, brush, 10, 75);
            }
            
            // Draw a simple calendar grid preview
            int previewTop = 100;
            int cellWidth = Math.Min(40, (Width - 40) / 7);
            int cellHeight = 30;
            string[] days = { "S", "M", "T", "W", "T", "F", "S" };
            
            using (var font = new Font("Segoe UI", 9))
            using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
            using (var linePen = new Pen(Color.FromArgb(200, 200, 200)))
            {
                for (int i = 0; i < 7 && (20 + i * cellWidth + cellWidth) < Width; i++)
                {
                    var rect = new Rectangle(20 + i * cellWidth, previewTop, cellWidth, cellHeight);
                    g.DrawRectangle(linePen, rect);
                    g.DrawString(days[i], font, brush, rect, 
                        new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }
            }
        }

        // Helper method like BeepComboBox uses
        private Rectangle GetContentRectForDrawing()
        {
            if (PainterKind == BaseControlPainterKind.Material)
            {
                var r = GetContentRect();
                if (r.Width > 0 && r.Height > 0) return r;
            }
            return DrawingRect;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (IsDesignModeSafe) return;

            Rectangle contentRect = GetContentRectForDrawing();
            
            int leftAnchor = Math.Max(_todayButton?.Right ?? 0, _nextButton?.Right ?? 0);
            int headerLeft = Math.Max(leftAnchor + 20, HeaderLeftPadding);
            int headerRight = HeaderRightPadding;

            var ctx = new CalendarRenderContext(this, _currentTheme,
                HeaderFont, DayFont, EventFont, TimeFont, DaysHeaderFont,
                _state, _rects, _eventService, _categories,
                headerLeft, headerRight);
            _renderer.HandleClick(e.Location, ctx);

            DateSelected?.Invoke(this, new CalendarDateArgs(_state.SelectedDate));
            if (_state.SelectedEvent != null)
                EventSelected?.Invoke(this, new CalendarEventArgs(_state.SelectedEvent));
            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (IsDesignModeSafe) return;
            
            if (_state.SelectedEvent != null)
                EventDoubleClick?.Invoke(this, new CalendarEventArgs(_state.SelectedEvent));
        }
        
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsDesignModeSafe) return;
            
            // Track hovered date for hover effects
            DateTime? newHoveredDate = null;
            CalendarEvent newHoveredEvent = null;
            
            if (_state.ViewMode == CalendarViewMode.Month)
            {
                var grid = _rects.CalendarGridRect;
                if (e.Location.Y >= grid.Y + CalendarLayoutMetrics.DayHeaderHeight)
                {
                    int cellWidth = grid.Width / 7;
                    int cellHeight = (grid.Height - CalendarLayoutMetrics.DayHeaderHeight) / 6;
                    int col = (e.Location.X - grid.X) / cellWidth;
                    int row = (e.Location.Y - grid.Y - CalendarLayoutMetrics.DayHeaderHeight) / cellHeight;

                    if (col >= 0 && col < 7 && row >= 0 && row < 6)
                    {
                        var firstDayOfMonth = new DateTime(_state.CurrentDate.Year, _state.CurrentDate.Month, 1);
                        var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
                        newHoveredDate = firstDayOfCalendar.AddDays(row * 7 + col);
                    }
                }
            }
            
            bool needsRedraw = false;
            if (_hoveredDate != newHoveredDate)
            {
                _hoveredDate = newHoveredDate;
                needsRedraw = true;
            }
            if (_hoveredEvent != newHoveredEvent)
            {
                _hoveredEvent = newHoveredEvent;
                needsRedraw = true;
            }
            
            if (needsRedraw)
            {
                Invalidate();
            }
        }
        
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (IsDesignModeSafe) return;
            
            if (_hoveredDate.HasValue || _hoveredEvent != null)
            {
                _hoveredDate = null;
                _hoveredEvent = null;
                Invalidate();
            }
        }

        private void InitializeControls()
        {
            // Create buttons with proper Material Design and auto-sizing based on content
            _prevButton = MakeButton("<", (s,e)=> NavigatePrevious());
            _nextButton = MakeButton(">", (s,e)=> NavigateNext());
            _todayButton = MakeButton("Today", (s,e)=> NavigateToday());

            _monthViewButton = MakeButton("Month", (s,e)=> ViewMode = CalendarViewMode.Month);
            _weekViewButton  = MakeButton("Week",  (s,e)=> ViewMode = CalendarViewMode.Week);
            _dayViewButton   = MakeButton("Day",   (s,e)=> ViewMode = CalendarViewMode.Day);
            _listViewButton  = MakeButton("List",  (s,e)=> ViewMode = CalendarViewMode.List);

            _createEventButton = MakeButton("+ Create Event", (s,e)=> OnCreateEventRequested(_state.SelectedDate));

            Controls.AddRange(new Control[] { _prevButton, _nextButton, _todayButton,
                _monthViewButton, _weekViewButton, _dayViewButton, _listViewButton, _createEventButton });
        }

        private BeepButton MakeButton(string text, EventHandler handler)
        {
            var b = new BeepButton 
            { 
                Text = text, 
                IsChild = true, 
                Theme = Theme, 
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                //EnableMaterialStyle = true,
               // MaterialVariant = MaterialTextFieldVariant.Outlined,
                // MaterialBorderRadius = 4,
                AutoSizeContent = true,  // Enable content-based auto-sizing
                ButtonAutoSizeForMaterial = true,  // Enable Material Design auto-sizing
                ButtonPreventAutoExpansion = false,  // Allow proper expansion for content
                MaterialPreserveContentArea = false  // Use full Material Design sizing
            };
            b.Click += handler;
            
            // Force Material Design size compensation after button creation
            b.HandleCreated += (s, e) => {
                var button = s as BeepButton;
              
            };
            
            return b;
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

        private void UpdateLayout()
        {
            if (Width <= 0 || Height <= 0) return;

            _layout ??= new CalendarLayoutManager(this, _state, _rects);

            int tallestBtn = new[]
            {
                _createEventButton?.Height ?? 30,
                _monthViewButton?.Height ?? 30,
                _weekViewButton?.Height ?? 30,
                _dayViewButton?.Height ?? 30,
                _listViewButton?.Height ?? 30
            }.DefaultIfEmpty(30).Max();

            // Use the content rect from BaseControl instead of ClientRectangle
            Rectangle contentRect = GetContentRectForDrawing();
            _layout.UpdateLayout(contentRect, tallestBtn, CalendarLayoutMetrics.SidebarWidth, GridLeftGutter);

            if (!_controlsInitialized || _prevButton == null || _nextButton == null || _todayButton == null || _createEventButton == null ||
                _monthViewButton == null || _weekViewButton == null || _dayViewButton == null || _listViewButton == null)
            {
                return;
            }

            // CRITICAL FIX: Position buttons relative to DrawingRect, not absolute coordinates
            // This ensures buttons stay within the proper content area defined by BaseControl
            int spacing = 8; 
            int margin = 10;

            // All button positioning should be offset by the content rectangle
            int baseX = contentRect.X;
            int baseY = contentRect.Y;

            // Navigation buttons - positioned relative to content rect
            int navY = baseY + (_rects.HeaderRect.Height - _prevButton.Height) / 2;
            _prevButton.Location = new Point(baseX + margin, navY);
            _nextButton.Location = new Point(_prevButton.Right + spacing, navY);
            _todayButton.Location = new Point(_nextButton.Right + spacing, navY);

            // Create event button - positioned relative to content rect
            int createY = baseY + _rects.ViewSelectorRect.Y - contentRect.Y + (_rects.ViewSelectorRect.Height - _createEventButton.Height) / 2;
            _createEventButton.Location = new Point(baseX + margin, createY);

            // View selector buttons - positioned relative to content rect
            int totalViewButtonsWidth = _monthViewButton.Width + _weekViewButton.Width + _dayViewButton.Width + _listViewButton.Width + spacing * 3;
            int startX = Math.Max(baseX + margin, contentRect.Right - totalViewButtonsWidth - margin);
            int viewY = baseY + _rects.ViewSelectorRect.Y - contentRect.Y + (_rects.ViewSelectorRect.Height - _monthViewButton.Height) / 2;
            
            _monthViewButton.Location = new Point(startX, viewY);
            _weekViewButton.Location  = new Point(_monthViewButton.Right + spacing, viewY);
            _dayViewButton.Location   = new Point(_weekViewButton.Right + spacing, viewY);
            _listViewButton.Location  = new Point(_dayViewButton.Right + spacing, viewY);

            UpdateViewButtonStates();
        }

        private void UpdateViewButtonStates()
        {
            if (_monthViewButton == null) return;

            Color selectedColor = _currentTheme?.CalendarSelectedDateBackColor ?? Color.FromArgb(66, 133, 244);
            Color normalColor = _currentTheme?.CalendarBackColor ?? Color.LightGray;

            _monthViewButton.BackColor = _state.ViewMode == CalendarViewMode.Month ? selectedColor : normalColor;
            _weekViewButton.BackColor  = _state.ViewMode == CalendarViewMode.Week  ? selectedColor : normalColor;
            _dayViewButton.BackColor   = _state.ViewMode == CalendarViewMode.Day   ? selectedColor : normalColor;
            _listViewButton.BackColor  = _state.ViewMode == CalendarViewMode.List  ? selectedColor : normalColor;
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;
            BackColor = _currentTheme.CalendarBackColor;
            ForeColor = _currentTheme.CalendarForeColor;
            
         
            
            if (UseThemeFont)
            {
                if (_currentTheme.CalendarTitleFont != null) HeaderFont = FontListHelper.CreateFontFromTypography(_currentTheme.CalendarTitleFont);
                if (_currentTheme.DaysHeaderFont != null) DaysHeaderFont = FontListHelper.CreateFontFromTypography(_currentTheme.DaysHeaderFont);
                if (_currentTheme.DateFont != null) DayFont = FontListHelper.CreateFontFromTypography(_currentTheme.DateFont);
                if (_currentTheme.CalendarSelectedFont != null) EventFont = FontListHelper.CreateFontFromTypography(_currentTheme.CalendarSelectedFont);
                if (_currentTheme.CalendarUnSelectedFont != null) TimeFont = FontListHelper.CreateFontFromTypography(_currentTheme.CalendarUnSelectedFont);
            }
            foreach (Control control in Controls)
            {
                if (control is BeepButton button)
                {
                    button.Theme = Theme;
                }
            }
            UpdateViewButtonStates();
            UpdateLayout();
            Invalidate();
        }

        private void NavigatePrevious()
        {
            _state.CurrentDate = _state.ViewMode switch
            {
                CalendarViewMode.Month => _state.CurrentDate.AddMonths(-1),
                CalendarViewMode.Week => _state.CurrentDate.AddDays(-7),
                CalendarViewMode.Day => _state.CurrentDate.AddDays(-1),
                CalendarViewMode.List => _state.CurrentDate.AddMonths(-1),
                _ => _state.CurrentDate.AddMonths(-1)
            };
            Invalidate();
            UpdateViewButtonStates();
        }

        private void NavigateNext()
        {
            _state.CurrentDate = _state.ViewMode switch
            {
                CalendarViewMode.Month => _state.CurrentDate.AddMonths(1),
                CalendarViewMode.Week => _state.CurrentDate.AddDays(7),
                CalendarViewMode.Day => _state.CurrentDate.AddDays(1),
                CalendarViewMode.List => _state.CurrentDate.AddMonths(1),
                _ => _state.CurrentDate.AddMonths(1)
            };
            Invalidate();
            UpdateViewButtonStates();
        }

        private void NavigateToday()
        {
            _state.CurrentDate = DateTime.Today;
            _state.SelectedDate = DateTime.Today;
            Invalidate();
            UpdateViewButtonStates();
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
        
        #region Public Methods
        
        /// <summary>
        /// Adds an event to the calendar
        /// </summary>
        public void AddEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent != null)
            {
                _events.Add(calendarEvent);
                _eventService?.InvalidateCache();
                Invalidate();
            }
        }

        /// <summary>
        /// Removes an event from the calendar
        /// </summary>
        public void RemoveEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent != null && _events.Contains(calendarEvent))
            {
                _events.Remove(calendarEvent);
                if (_state.SelectedEvent == calendarEvent)
                {
                    _state.SelectedEvent = null;
                }
                _eventService?.InvalidateCache();
                Invalidate();
            }
        }

        /// <summary>
        /// Updates an existing event
        /// </summary>
        public void UpdateEvent(CalendarEvent calendarEvent)
        {
            if (calendarEvent != null)
            {
                var existingEvent = _events.FirstOrDefault(e => e.Id == calendarEvent.Id);
                if (existingEvent != null)
                {
                    int index = _events.IndexOf(existingEvent);
                    _events[index] = calendarEvent;
                    _eventService?.InvalidateCache();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets events for a date range
        /// </summary>
        public List<CalendarEvent> GetEventsForDateRange(DateTime startDate, DateTime endDate)
        {
            return _events.Where(e => e.StartTime.Date >= startDate.Date && e.StartTime.Date <= endDate.Date).ToList();
        }
        
        #endregion
    }

    #region Supporting Classes
    
    /// <summary>
    /// Calendar view modes
    /// </summary>
    public enum CalendarViewMode
    {
        Month,
        Week,
        Day,
        List
    }

    /// <summary>
    /// Represents a calendar event
    /// </summary>
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

    /// <summary>
    /// Represents an event category with color
    /// </summary>
    public class EventCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public Color Color { get; set; }
        public string Description { get; set; } = "";
    }

    /// <summary>
    /// Event args for calendar event selection
    /// </summary>
    public class CalendarEventArgs : EventArgs
    {
        public CalendarEvent Event { get; }

        public CalendarEventArgs(CalendarEvent calendarEvent)
        {
            Event = calendarEvent;
        }
    }

    /// <summary>
    /// Event args for calendar date selection
    /// </summary>
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
