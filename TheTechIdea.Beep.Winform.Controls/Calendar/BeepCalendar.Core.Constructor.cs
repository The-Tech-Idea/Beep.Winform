using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        public BeepCalendar():base()
        {
            ShowAllBorders = true;
            TabStop = true;
            GridLeftGutter = 0;

            try
            {
                InitializeToolbar();
                _controlsInitialized = true;
            }
            catch { /* designer safety */ }

            InitializeDefaultCategories();
            Size = new Size(800, 600);
            _eventService = new CalendarEventService(_events);
            ConfigureEventServiceTelemetry();
            _layout = new CalendarLayoutManager(this, _state, _rects);
            _conflictPolicy = new CalendarConflictPolicy(_conflictPolicyMode);
            EventEditor = new CalendarEventEditor();

            // Initialize per-view painter
            _viewPainter = ViewPainterFactory.GetPainter(_state.ViewMode);
            ApplyThemeTypography();

            if (!IsDesignModeSafe)
            {
                UpdateLayout();
            }
        }
    }
}
