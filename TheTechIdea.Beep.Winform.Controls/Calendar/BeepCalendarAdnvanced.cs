using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Calendar Advanced")]
    [Description("Refactored calendar control using helper classes and view renderers.")]
    public class BeepCalendarAdnvanced : BaseControl
    {
        private readonly CalendarState _state = new CalendarState();
        private readonly CalendarRects _rects = new CalendarRects();
        private CalendarLayoutManager _layout;
        private CalendarEventService _eventService;
        private CalendarRenderer _renderer;

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

        public BeepCalendarAdnvanced()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);

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
            UpdateLayout();
        }

        [Browsable(true)]
        [Category("Calendar")] public DateTime CurrentDate { get => _state.CurrentDate; set { _state.CurrentDate = value; _state.SelectedDate = value; Invalidate(); } }
        [Browsable(true)]
        [Category("Calendar")] public CalendarViewMode ViewMode { get => _state.ViewMode; set { _state.ViewMode = value; UpdateLayout(); Invalidate(); } }
        [Browsable(true)]
        [Category("Calendar")] public bool ShowSidebar { get => _state.ShowSidebar; set { _state.ShowSidebar = value; UpdateLayout(); Invalidate(); } }

        [Browsable(true)]
        [Category("Calendar")] public List<CalendarEvent> Events
        {
            get => _events;
            set { _events = value ?? new(); _eventService = new CalendarEventService(_events); Invalidate(); }
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
            UpdateLayout();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateLayout();
        }

        protected override void DrawContent(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get the actual content rectangle from BaseControl, like BeepComboBox does
            Rectangle contentRect = GetContentRectForDrawing();

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

        // Helper method like BeepComboBox uses
        private Rectangle GetContentRectForDrawing()
        {
            if (EnableMaterialStyle && _materialHelper != null)
            {
                var r = _materialHelper.GetContentRect();
                if (r.Width > 0 && r.Height > 0) return r;
            }
            return DrawingRect;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

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
            if (_state.SelectedEvent != null)
                EventDoubleClick?.Invoke(this, new CalendarEventArgs(_state.SelectedEvent));
        }

        private void InitializeControls()
        {
            // Use ASCII arrows to avoid font glyph issues showing '?'
            _prevButton = MakeButton("<", new Size(30,30), (s,e)=> NavigatePrevious());
            _nextButton = MakeButton(">", new Size(30,30), (s,e)=> NavigateNext());
            _todayButton = MakeButton("Today", new Size(70,30), (s,e)=> NavigateToday());

            _monthViewButton = MakeButton("Month", new Size(70,30), (s,e)=> ViewMode = CalendarViewMode.Month);
            _weekViewButton  = MakeButton("Week",  new Size(70,30), (s,e)=> ViewMode = CalendarViewMode.Week);
            _dayViewButton   = MakeButton("Day",   new Size(70,30), (s,e)=> ViewMode = CalendarViewMode.Day);
            _listViewButton  = MakeButton("List",  new Size(70,30), (s,e)=> ViewMode = CalendarViewMode.List);

            _createEventButton = MakeButton("+ Create Event", new Size(120,30), (s,e)=> OnCreateEventRequested(_state.SelectedDate));

            Controls.AddRange(new Control[] { _prevButton, _nextButton, _todayButton,
                _monthViewButton, _weekViewButton, _dayViewButton, _listViewButton, _createEventButton });
        }

        private BeepButton MakeButton(string text, Size size, EventHandler handler)
        {
            var b = new BeepButton { Text = text, Size = size, IsChild = true, Theme = Theme, Anchor = AnchorStyles.Top | AnchorStyles.Left };
            b.Click += handler;
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
    }
}
