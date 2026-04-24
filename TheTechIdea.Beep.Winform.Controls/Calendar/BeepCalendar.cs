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
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

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
        private DateTime _focusedDate = DateTime.Today;
        private bool _keyboardFocusVisible;
        private CalendarDensityMode _densityMode = CalendarDensityMode.Comfortable;
        private CalendarToolbarLabelMode _toolbarLabelMode = CalendarToolbarLabelMode.Full;

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
        [Browsable(true)] [Category("Layout")] public int HeaderRightPadding { get; set; } = CalendarLayoutMetrics.HeaderRightPadding;
        [Browsable(true)] [Category("Layout")] public int GridLeftGutter { get; set; } = CalendarLayoutMetrics.TimeColumnWidth / 5;
        
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
            TabStop = true;
            GridLeftGutter = CalendarLayoutMetrics.TimeColumnWidth / 5;

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
            ApplyThemeTypography();
            
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
        [DefaultValue(CalendarDensityMode.Comfortable)]
        public CalendarDensityMode DensityMode
        {
            get => _densityMode;
            set
            {
                if (_densityMode != value)
                {
                    _densityMode = value;
                    UpdateLayout();
                    Invalidate();
                }
            }
        }

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

        [Browsable(true)] [Category("Appearance")] public Font HeaderFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 16f, FontWeight.Bold, FontStyle.Bold);
        [Browsable(true)] [Category("Appearance")] public Font DayFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 12f, FontWeight.Regular, FontStyle.Regular);
        [Browsable(true)] [Category("Appearance")] public Font EventFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 9f, FontWeight.Regular, FontStyle.Regular);
        [Browsable(true)] [Category("Appearance")] public Font TimeFont { get; set; } = BeepThemesManager.ToFont("Segoe UI", 10f, FontWeight.Regular, FontStyle.Regular);
        public Font DaysHeaderFont { get; private set; } = BeepThemesManager.ToFont("Segoe UI", 10f, FontWeight.Medium, FontStyle.Regular);

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
            // Design-time placeholder rendering
            if (IsDesignModeSafe)
            {
                PaintDesignTimePlaceholder(g);
                return;
            }
            
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get the actual content rectangle
            Rectangle contentRect = bounds.IsEmpty ? GetContentRectForDrawing() : bounds;

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
            var headerTextBounds = GetHeaderTextBounds();
            _stylePainter.PaintHeader(g, _rects.HeaderRect, string.Empty, painterCtx);
            DrawPainterHeaderText(g, painterCtx, GetHeaderText(), headerTextBounds);

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
            
            var headerTextBounds = GetHeaderTextBounds();
            int headerLeft = Math.Max(0, headerTextBounds.Left - _rects.HeaderRect.X);
            int headerRight = Math.Max(0, _rects.HeaderRect.Right - headerTextBounds.Right);

            var ctx = new CalendarRenderContext(this, _currentTheme,
                HeaderFont, DayFont, EventFont, TimeFont, DaysHeaderFont,
                _state, _rects, _eventService, _categories,
                headerLeft, headerRight, GetDensityScale());

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
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int eventHeight = ScaleMetric(16);
            int eventSpacing = ScaleMetric(2);
            int eventStartOffset = ScaleMetric(30);

            int cellWidth = grid.Width / 7;
            int cellHeight = (grid.Height - dayHeaderHeight) / 6;

            // Day headers
            string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            for (int i = 0; i < 7; i++)
            {
                var headerRect = new Rectangle(grid.X + i * cellWidth, grid.Y, cellWidth, dayHeaderHeight);
                bool isToday = (int)DateTime.Today.DayOfWeek == i;
                _stylePainter.PaintDayHeader(g, headerRect, dayNames[i], isToday, ctx);
            }

            // Day cells
            var eventsByDate = new Dictionary<DateTime, List<CalendarEvent>>();
            for (int week = 0; week < 6; week++)
            {
                for (int day = 0; day < 7; day++)
                {
                    var cellDate = firstDayOfCalendar.AddDays(week * 7 + day);
                    var cellRect = new Rectangle(
                        grid.X + day * cellWidth, 
                        grid.Y + dayHeaderHeight + week * cellHeight, 
                        cellWidth, 
                        cellHeight
                    );

                    var dateKey = cellDate.Date;
                    if (!eventsByDate.TryGetValue(dateKey, out var dayEvents))
                    {
                        dayEvents = _eventService.GetEventsForDate(cellDate);
                        eventsByDate[dateKey] = dayEvents;
                    }

                    var state = new DayCellState
                    {
                        IsCurrentMonth = cellDate.Month == _state.CurrentDate.Month,
                        IsToday = cellDate.Date == DateTime.Today,
                        IsSelected = cellDate.Date == _state.SelectedDate.Date,
                        IsHovered = _hoveredDate.HasValue && cellDate.Date == _hoveredDate.Value.Date,
                        IsFocused = _keyboardFocusVisible && cellDate.Date == _focusedDate.Date,
                        IsWeekend = cellDate.DayOfWeek == DayOfWeek.Saturday || cellDate.DayOfWeek == DayOfWeek.Sunday,
                        EventCount = dayEvents.Count
                    };

                    _stylePainter.PaintDayCell(g, cellRect, cellDate, state, ctx);

                    // Draw events in cell
                    int eventY = cellRect.Y + eventStartOffset;
                    foreach (var evt in dayEvents.Take(3))
                    {
                        var eventRect = new Rectangle(cellRect.X + 2, eventY, cellRect.Width - 4, eventHeight);
                        bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                        bool isHovered = _hoveredEvent?.Id == evt.Id;
                        _stylePainter.PaintEventBar(g, eventRect, evt, isSelected, isHovered, ctx);
                        eventY += eventHeight + eventSpacing;
                    }

                    if (dayEvents.Count > 3)
                    {
                        using (var brush = new SolidBrush(ctx.ForegroundColor))
                        {
                            g.DrawString($"+{dayEvents.Count - 3} more", EventFont,
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
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
            int eventInsetX = ScaleMetric(CalendarLayoutMetrics.EventInsetX);
            int eventInsetY = ScaleMetric(CalendarLayoutMetrics.EventInsetY);
            int cellWidth = (grid.Width - timeColumnWidth) / 7;

            // Day headers
            for (int day = 0; day < 7; day++)
            {
                var dayDate = startOfWeek.AddDays(day);
                var headerRect = new Rectangle(
                    grid.X + timeColumnWidth + day * cellWidth, 
                    grid.Y, 
                    cellWidth, 
                    dayHeaderHeight
                );
                _stylePainter.PaintWeekDayHeader(g, headerRect, dayDate, dayDate.Date == DateTime.Today, ctx);
            }

            // Time slots
            int slotHeight = Math.Max(ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight), (grid.Height - dayHeaderHeight) / 24);
            int currentHour = DateTime.Now.Hour;
            var eventsByDay = Enumerable.Range(0, 7)
                .Select(index => startOfWeek.AddDays(index).Date)
                .ToDictionary(day => day, day => _eventService.GetEventsForDate(day));

            for (int hour = 0; hour < 24; hour++)
            {
                int yPos = grid.Y + dayHeaderHeight + hour * slotHeight;
                
                // Time label
                var timeLabelRect = new Rectangle(grid.X, yPos, timeColumnWidth, slotHeight);
                _stylePainter.PaintTimeLabel(g, timeLabelRect, hour, ctx);

                // Time slot for each day
                for (int day = 0; day < 7; day++)
                {
                    var slotRect = new Rectangle(
                        grid.X + timeColumnWidth + day * cellWidth, 
                        yPos, 
                        cellWidth, 
                        slotHeight
                    );
                    _stylePainter.PaintTimeSlot(g, slotRect, hour, hour == currentHour && startOfWeek.AddDays(day).Date == DateTime.Today, ctx);

                    // Draw events
                    var dayDate = startOfWeek.AddDays(day).Date;
                    var dayEvents = eventsByDay[dayDate]
                        .Where(e => e.StartTime.Hour == hour)
                        .ToList();

                    foreach (var evt in dayEvents)
                    {
                        var eventRect = new Rectangle(
                            slotRect.X + eventInsetX,
                            yPos + eventInsetY,
                            Math.Max(20, slotRect.Width - (eventInsetX * 2)),
                            Math.Max(CalendarLayoutMetrics.MinEventHitHeight, (int)(evt.Duration.TotalHours * slotHeight) - (eventInsetY * 2))
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
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            int timeColumnWidth = ScaleMetric(CalendarLayoutMetrics.TimeColumnWidth);
            int eventInsetX = ScaleMetric(CalendarLayoutMetrics.EventInsetX);
            int eventInsetY = ScaleMetric(CalendarLayoutMetrics.EventInsetY);
            
            // Day header
            var headerRect = new Rectangle(grid.X, grid.Y, grid.Width, dayHeaderHeight);
            _stylePainter.PaintWeekDayHeader(g, headerRect, _state.CurrentDate, _state.CurrentDate.Date == DateTime.Today, ctx);

            // Time slots
            int slotHeight = Math.Max(ScaleMetric(CalendarLayoutMetrics.TimeSlotHeight), (grid.Height - dayHeaderHeight) / 24);
            int currentHour = DateTime.Now.Hour;
            var eventsByHour = _eventService.GetEventsForDate(_state.CurrentDate)
                .GroupBy(e => e.StartTime.Hour)
                .ToDictionary(group => group.Key, group => group.ToList());

            for (int hour = 0; hour < 24; hour++)
            {
                int yPos = grid.Y + dayHeaderHeight + hour * slotHeight;
                
                // Time label
                var timeLabelRect = new Rectangle(grid.X, yPos, timeColumnWidth, slotHeight);
                _stylePainter.PaintTimeLabel(g, timeLabelRect, hour, ctx);

                // Time slot
                var slotRect = new Rectangle(
                    grid.X + timeColumnWidth, 
                    yPos, 
                    grid.Width - timeColumnWidth, 
                    slotHeight
                );
                _stylePainter.PaintTimeSlot(g, slotRect, hour, hour == currentHour && _state.CurrentDate.Date == DateTime.Today, ctx);

                // Draw events
                if (!eventsByHour.TryGetValue(hour, out var dayEvents))
                {
                    continue;
                }

                foreach (var evt in dayEvents)
                {
                    var eventRect = new Rectangle(
                        slotRect.X + eventInsetX,
                        yPos + eventInsetY,
                        Math.Max(20, slotRect.Width - (eventInsetX * 2)),
                        Math.Max(CalendarLayoutMetrics.MinEventHitHeight, (int)(evt.Duration.TotalHours * slotHeight) - (eventInsetY * 2))
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
            int rowHeight = ScaleMetric(CalendarLayoutMetrics.ListRowHeight);
            int rowSpacing = ScaleMetric(CalendarLayoutMetrics.ListRowSpacing);
            int padding = ScaleMetric(CalendarLayoutMetrics.SidebarPadding);

            int yPos = grid.Y + padding;
            foreach (var evt in monthEvents)
            {
                if (yPos + rowHeight > grid.Bottom) break;
                
                var eventRect = new Rectangle(grid.X + padding, yPos, grid.Width - (padding * 2), rowHeight);
                bool isSelected = _state.SelectedEvent?.Id == evt.Id;
                bool isHovered = _hoveredEvent?.Id == evt.Id;
                _stylePainter.PaintListViewEvent(g, eventRect, evt, isSelected, isHovered, ctx);
                yPos += rowHeight + rowSpacing;
            }
        }
        
        private void DrawSidebarWithPainter(Graphics g, CalendarPainterContext ctx)
        {
            var rect = _rects.SidebarRect;
            int padding = ScaleMetric(CalendarLayoutMetrics.SidebarPadding);
            int cardHeight = ScaleMetric(CalendarLayoutMetrics.SidebarCardHeight);
            int cardGap = ScaleMetric(CalendarLayoutMetrics.SidebarCardGap);
            _stylePainter.PaintSidebar(g, rect, ctx);

            // Mini calendar
            var miniRect = new Rectangle(rect.X + padding, rect.Y + padding, Math.Max(10, rect.Width - (padding * 2)), cardHeight);
            if (miniRect.Width > 20)
            {
                _stylePainter.PaintMiniCalendar(g, miniRect, _state.CurrentDate, _state.SelectedDate, ctx);
            }

            // Event details / empty state
            var detailsRect = new Rectangle(
                rect.X + padding,
                rect.Y + padding + cardHeight + cardGap,
                Math.Max(10, rect.Width - (padding * 2)),
                cardHeight);
            if (detailsRect.Width > 20)
            {
                if (_state.SelectedEvent != null)
                {
                    _stylePainter.PaintEventDetails(g, detailsRect, _state.SelectedEvent, ctx);
                }
                else
                {
                    using (var backBrush = new SolidBrush(ctx.BackgroundColor))
                    using (var borderPen = new Pen(ctx.BorderColor))
                    using (var titleBrush = new SolidBrush(ctx.ForegroundColor))
                    using (var bodyBrush = new SolidBrush(Color.FromArgb(170, ctx.ForegroundColor)))
                    {
                        g.FillRectangle(backBrush, detailsRect);
                        g.DrawRectangle(borderPen, detailsRect);
                        g.DrawString("No event selected", DayFont, titleBrush, new Rectangle(detailsRect.X + 10, detailsRect.Y + 14, detailsRect.Width - 20, 24));
                        g.DrawString("Select a date/event or use + Create Event.", EventFont, bodyBrush, new Rectangle(detailsRect.X + 10, detailsRect.Y + 40, detailsRect.Width - 20, 40));
                    }
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
            Focus();
            _keyboardFocusVisible = false;

            Rectangle contentRect = GetContentRectForDrawing();
            
            var headerTextBounds = GetHeaderTextBounds();
            int headerLeft = Math.Max(0, headerTextBounds.Left - _rects.HeaderRect.X);
            int headerRight = Math.Max(0, _rects.HeaderRect.Right - headerTextBounds.Right);

            var ctx = new CalendarRenderContext(this, _currentTheme,
                HeaderFont, DayFont, EventFont, TimeFont, DaysHeaderFont,
                _state, _rects, _eventService, _categories,
                headerLeft, headerRight, GetDensityScale());
            _renderer.HandleClick(e.Location, ctx);
            _focusedDate = _state.SelectedDate.Date;

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
                int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
                if (e.Location.Y >= grid.Y + dayHeaderHeight)
                {
                    int cellWidth = grid.Width / 7;
                    int cellHeight = (grid.Height - dayHeaderHeight) / 6;
                    int col = (e.Location.X - grid.X) / cellWidth;
                    int row = (e.Location.Y - grid.Y - dayHeaderHeight) / cellHeight;

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
                var previousHoveredDate = _hoveredDate;
                _hoveredDate = newHoveredDate;
                if (_state.ViewMode == CalendarViewMode.Month)
                {
                    InvalidateDateCell(previousHoveredDate);
                    InvalidateDateCell(_hoveredDate);
                }
                else
                {
                    needsRedraw = true;
                }
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
                var previousHoveredDate = _hoveredDate;
                _hoveredDate = null;
                _hoveredEvent = null;
                if (_state.ViewMode == CalendarViewMode.Month)
                {
                    InvalidateDateCell(previousHoveredDate);
                }
                else
                {
                    Invalidate();
                }
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _keyboardFocusVisible = true;
            _focusedDate = _state.SelectedDate.Date;
            InvalidateDateCell(_focusedDate);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (_keyboardFocusVisible)
            {
                _keyboardFocusVisible = false;
                InvalidateDateCell(_focusedDate);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (IsDesignModeSafe)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            var previousFocusDate = _focusedDate.Date;
            bool handled = true;

            switch (keyData)
            {
                case Keys.Left:
                    MoveFocusedDate(-1);
                    break;
                case Keys.Right:
                    MoveFocusedDate(1);
                    break;
                case Keys.Up:
                    MoveFocusedDate(-7);
                    break;
                case Keys.Down:
                    MoveFocusedDate(7);
                    break;
                case Keys.Home:
                    _focusedDate = _focusedDate.AddDays(-(int)_focusedDate.DayOfWeek);
                    break;
                case Keys.End:
                    _focusedDate = _focusedDate.AddDays(6 - (int)_focusedDate.DayOfWeek);
                    break;
                case Keys.PageUp:
                    NavigatePrevious();
                    _focusedDate = _state.CurrentDate.Date;
                    break;
                case Keys.PageDown:
                    NavigateNext();
                    _focusedDate = _state.CurrentDate.Date;
                    break;
                case Keys.Enter:
                    _state.SelectedDate = _focusedDate.Date;
                    _state.CurrentDate = _focusedDate.Date;
                    _state.SelectedEvent = _eventService.GetEventsForDate(_focusedDate.Date).FirstOrDefault();
                    DateSelected?.Invoke(this, new CalendarDateArgs(_state.SelectedDate));
                    if (_state.SelectedEvent != null)
                    {
                        EventSelected?.Invoke(this, new CalendarEventArgs(_state.SelectedEvent));
                    }
                    break;
                default:
                    handled = false;
                    break;
            }

            if (!handled)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            _keyboardFocusVisible = true;
            _state.SelectedDate = _focusedDate.Date;
            _state.CurrentDate = _focusedDate.Date;

            if (_state.ViewMode == CalendarViewMode.Month)
            {
                InvalidateDateCell(previousFocusDate);
                InvalidateDateCell(_focusedDate.Date);
            }
            else
            {
                Invalidate();
            }

            return true;
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

            Rectangle contentRect = GetContentRectForDrawing();
            ApplyResponsiveButtonLabels(contentRect.Width);

            int tallestBtn = new[]
            {
                _createEventButton?.Height ?? 30,
                _monthViewButton?.Height ?? 30,
                _weekViewButton?.Height ?? 30,
                _dayViewButton?.Height ?? 30,
                _listViewButton?.Height ?? 30
            }.DefaultIfEmpty(30).Max();

            // Use the content rect from BaseControl instead of ClientRectangle
            _layout.UpdateLayout(
                contentRect,
                tallestBtn,
                ScaleMetric(CalendarLayoutMetrics.SidebarWidth),
                ScaleMetric(GridLeftGutter));

            if (!_controlsInitialized || _prevButton == null || _nextButton == null || _todayButton == null || _createEventButton == null ||
                _monthViewButton == null || _weekViewButton == null || _dayViewButton == null || _listViewButton == null)
            {
                return;
            }

            // CRITICAL FIX: Position buttons relative to DrawingRect, not absolute coordinates
            // This ensures buttons stay within the proper content area defined by BaseControl
            int spacing = ScaleMetric(CalendarLayoutMetrics.ControlSpacing); 
            int margin = ScaleMetric(CalendarLayoutMetrics.OuterMargin);

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

            Color selectedColor = _currentTheme?.CalendarSelectedDateBackColor ?? _currentTheme?.PrimaryColor ?? _currentTheme?.AccentColor ?? Color.FromArgb(66, 133, 244);
            Color normalColor = _currentTheme?.CalendarBackColor ?? _currentTheme?.SurfaceColor ?? _currentTheme?.BackColor ?? BackColor;
            Color selectedForeColor = _currentTheme?.CalendarSelectedDateForColor ?? _currentTheme?.OnPrimaryColor ?? Color.White;
            Color normalForeColor = _currentTheme?.CalendarForeColor ?? _currentTheme?.ForeColor ?? ForeColor;

            _monthViewButton.BackColor = _state.ViewMode == CalendarViewMode.Month ? selectedColor : normalColor;
            _monthViewButton.ForeColor = _state.ViewMode == CalendarViewMode.Month ? selectedForeColor : normalForeColor;
            _weekViewButton.BackColor  = _state.ViewMode == CalendarViewMode.Week  ? selectedColor : normalColor;
            _weekViewButton.ForeColor  = _state.ViewMode == CalendarViewMode.Week  ? selectedForeColor : normalForeColor;
            _dayViewButton.BackColor   = _state.ViewMode == CalendarViewMode.Day   ? selectedColor : normalColor;
            _dayViewButton.ForeColor   = _state.ViewMode == CalendarViewMode.Day   ? selectedForeColor : normalForeColor;
            _listViewButton.BackColor  = _state.ViewMode == CalendarViewMode.List  ? selectedColor : normalColor;
            _listViewButton.ForeColor  = _state.ViewMode == CalendarViewMode.List  ? selectedForeColor : normalForeColor;
        }

        private void ApplyResponsiveButtonLabels(int availableWidth)
        {
            if (_todayButton == null || _monthViewButton == null || _weekViewButton == null ||
                _dayViewButton == null || _listViewButton == null || _createEventButton == null)
            {
                return;
            }

            CalendarToolbarLabelMode requestedMode = availableWidth < 720
                ? CalendarToolbarLabelMode.Compact
                : availableWidth < 980
                    ? CalendarToolbarLabelMode.Medium
                    : CalendarToolbarLabelMode.Full;

            if (_toolbarLabelMode == requestedMode)
            {
                return;
            }

            _toolbarLabelMode = requestedMode;
            switch (requestedMode)
            {
                case CalendarToolbarLabelMode.Compact:
                    _todayButton.Text = "T";
                    _monthViewButton.Text = "M";
                    _weekViewButton.Text = "W";
                    _dayViewButton.Text = "D";
                    _listViewButton.Text = "L";
                    _createEventButton.Text = "+";
                    break;

                case CalendarToolbarLabelMode.Medium:
                    _todayButton.Text = "Today";
                    _monthViewButton.Text = "Mon";
                    _weekViewButton.Text = "Week";
                    _dayViewButton.Text = "Day";
                    _listViewButton.Text = "List";
                    _createEventButton.Text = "+ Event";
                    break;

                default:
                    _todayButton.Text = "Today";
                    _monthViewButton.Text = "Month";
                    _weekViewButton.Text = "Week";
                    _dayViewButton.Text = "Day";
                    _listViewButton.Text = "List";
                    _createEventButton.Text = "+ Create Event";
                    break;
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;
            BackColor = _currentTheme.CalendarBackColor;
            ForeColor = _currentTheme.CalendarForeColor;
            
         
            
            if (UseThemeFont)
            {
                ApplyThemeTypography();
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

        private void ApplyThemeTypography()
        {
            HeaderFont = ResolveFont(_currentTheme?.CalendarTitleFont, HeaderFont);
            DaysHeaderFont = ResolveFont(_currentTheme?.DaysHeaderFont, DaysHeaderFont);
            DayFont = ResolveFont(_currentTheme?.DateFont, DayFont);
            EventFont = ResolveFont(_currentTheme?.CalendarSelectedFont, EventFont);
            TimeFont = ResolveFont(_currentTheme?.CalendarUnSelectedFont, TimeFont);
        }

        private static Font ResolveFont(TypographyStyle style, Font fallback)
        {
            return style != null ? BeepThemesManager.ToFont(style) : fallback;
        }

        private void DrawPainterHeaderText(Graphics g, CalendarPainterContext ctx, string headerText, Rectangle textRect)
        {
            if (textRect.Width <= 0 || textRect.Height <= 0) return;

            using (var brush = new SolidBrush(ctx.ForegroundColor))
            using (var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            })
            {
                g.DrawString(headerText, HeaderFont, brush, textRect, sf);
            }
        }

        private float GetDensityScale()
        {
            return _densityMode == CalendarDensityMode.Compact ? 0.85f : 1.0f;
        }

        private int ScaleMetric(int baseValue)
        {
            float dpiScale = BeepThemesManager.DpiScaleX > 0f ? BeepThemesManager.DpiScaleX : 1f;
            return Math.Max(1, (int)Math.Round(baseValue * GetDensityScale() * dpiScale));
        }

        private void MoveFocusedDate(int deltaDays)
        {
            _focusedDate = _focusedDate.AddDays(deltaDays);
        }

        private void InvalidateDateCell(DateTime? date)
        {
            if (!date.HasValue || _state.ViewMode != CalendarViewMode.Month)
            {
                return;
            }

            var cellRect = TryGetMonthCellRect(date.Value.Date);
            if (cellRect.HasValue)
            {
                Invalidate(cellRect.Value);
            }
        }

        private Rectangle? TryGetMonthCellRect(DateTime date)
        {
            var grid = _rects.CalendarGridRect;
            int dayHeaderHeight = ScaleMetric(CalendarLayoutMetrics.DayHeaderHeight);
            if (grid.Width <= 0 || grid.Height <= dayHeaderHeight)
            {
                return null;
            }

            var firstDayOfMonth = new DateTime(_state.CurrentDate.Year, _state.CurrentDate.Month, 1);
            var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
            int offset = (date.Date - firstDayOfCalendar.Date).Days;
            if (offset < 0 || offset > 41)
            {
                return null;
            }

            int col = offset % 7;
            int row = offset / 7;
            int cellWidth = grid.Width / 7;
            int cellHeight = (grid.Height - dayHeaderHeight) / 6;

            return new Rectangle(
                grid.X + (col * cellWidth),
                grid.Y + dayHeaderHeight + (row * cellHeight),
                cellWidth,
                cellHeight);
        }

        private Rectangle GetHeaderTextBounds()
        {
            var headerRect = _rects.HeaderRect;
            if (headerRect.Width <= 0 || headerRect.Height <= 0)
            {
                return Rectangle.Empty;
            }

            int navRightAnchor = Math.Max(_todayButton?.Right ?? headerRect.X, _nextButton?.Right ?? headerRect.X);
            int leftFromNav = navRightAnchor + CalendarLayoutMetrics.HeaderTextSpacingFromNav;
            int leftFromPadding = headerRect.X + Math.Max(0, HeaderLeftPadding);
            int leftFromGrid = Math.Max(headerRect.X, _rects.CalendarGridRect.Left);
            int availableLeft = Math.Max(Math.Max(leftFromNav, leftFromPadding), leftFromGrid);

            int rightFromPadding = headerRect.Right - Math.Max(0, HeaderRightPadding);
            int rightFromGrid = _rects.CalendarGridRect.Width > 0
                ? Math.Min(headerRect.Right, _rects.CalendarGridRect.Right)
                : headerRect.Right;
            int availableRight = Math.Min(rightFromPadding, rightFromGrid);

            if (availableRight <= availableLeft)
            {
                availableLeft = headerRect.X + Math.Max(0, HeaderLeftPadding);
                availableRight = headerRect.Right - Math.Max(0, HeaderRightPadding);
                if (availableRight <= availableLeft)
                {
                    availableLeft = headerRect.X;
                    availableRight = headerRect.Right;
                }
            }

            return new Rectangle(
                availableLeft,
                headerRect.Y,
                Math.Max(1, availableRight - availableLeft),
                headerRect.Height);
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

    public enum CalendarDensityMode
    {
        Compact,
        Comfortable
    }

    internal enum CalendarToolbarLabelMode
    {
        Full,
        Medium,
        Compact
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
