using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        public event EventHandler<CalendarEventArgs> EventSelected;
        public event EventHandler<CalendarEventArgs> EventDoubleClick;
        public event EventHandler<CalendarDateArgs> DateSelected;
        public event EventHandler<CalendarEventArgs> CreateEventRequested;
        public event EventHandler<CalendarConflictEventArgs> ConflictDetected;
        public event EventHandler<CalendarEventMutationEventArgs> EventMutating;
        public event EventHandler<CalendarEventMutationEventArgs> EventMutated;

        [Browsable(false)]
        public ICalendarEventEditor EventEditor { get; set; }

        [Browsable(false)]
        public bool CanUndo => _undoStack.Count > 0;

        [Browsable(false)]
        public bool CanRedo => _redoStack.Count > 0;
    }
}