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
            _renderer = new CalendarRenderer();
            _conflictPolicy = new CalendarConflictPolicy(_conflictPolicyMode);
            EventEditor = new CalendarEventEditor();

            // Initialize style painter
            _stylePainter = CalendarPainterFactory.GetPainter(_calendarStyle);
            ApplyThemeTypography();

            if (!IsDesignModeSafe)
            {
                UpdateLayout();
            }
        }
    }
}
