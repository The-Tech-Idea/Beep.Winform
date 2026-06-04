using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        [Browsable(true)]
        [Category("Calendar")] public DateTime CurrentDate { get => _state.CurrentDate; set { _state.CurrentDate = value; _state.SelectedDate = value; Invalidate(); } }
        [Browsable(true)]
        [Category("Calendar")]
        public CalendarViewMode ViewMode
        {
            get => _state.ViewMode;
            set
            {
                if (_state.ViewMode == value) return;
                _state.ViewMode = value;
                _viewPainter = ViewPainterFactory.GetPainter(value);
                RequestLayoutAndRedraw();
            }
        }
        [Browsable(true)]
        [Category("Calendar")] public bool ShowSidebar { get => _state.ShowSidebar; set { _state.ShowSidebar = value; RequestLayoutAndRedraw(); } }
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
                    RequestLayoutAndRedraw();
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
                ConfigureEventServiceTelemetry();
                RequestRedraw();
            }
        }

        [Browsable(true)]
        [Category("Calendar")]
        [DefaultValue(CalendarConflictPolicyMode.AllowOverlap)]
        public CalendarConflictPolicyMode ConflictPolicyMode
        {
            get => _conflictPolicyMode;
            set
            {
                if (_conflictPolicyMode == value)
                {
                    return;
                }

                _conflictPolicyMode = value;
                _conflictPolicy = new CalendarConflictPolicy(value);
            }
        }

        [Browsable(true)]
        [Category("Calendar")]
        [DefaultValue(15)]
        public int InteractionSnapIntervalMinutes
        {
            get => _interactionSnapIntervalMinutes;
            set => _interactionSnapIntervalMinutes = Math.Max(1, value);
        }

        [Browsable(true)]
        [Category("Calendar")] public List<EventCategory> Categories
        {
            get => _categories;
            set { _categories = value ?? new(); InitializeDefaultCategories(); RequestRedraw(); }
        }

        [Browsable(true)]
        [Category("Calendar")]
        public List<CalendarResource> Resources { get; set; } = new();

        [Browsable(false)]
        public CalendarPerformanceMetrics PerformanceMetrics { get; } = new();

    }
}