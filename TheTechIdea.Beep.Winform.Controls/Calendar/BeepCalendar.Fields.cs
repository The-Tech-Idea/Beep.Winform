using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private readonly CalendarState _state = new CalendarState();
        private readonly CalendarRects _rects = new CalendarRects();
        private CalendarLayoutManager _layout;
        private CalendarEventService _eventService;
        private CalendarSurfaceModel _surfaceModel;

        // Per-view painter (one per CalendarViewMode, swapped by ViewPainterFactory)
        private ICalendarViewPainter _viewPainter;
        private BeepControlStyle _calendarStyle = BeepControlStyle.Material3;

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
        private ICalendarConflictPolicy _conflictPolicy;
        private CalendarConflictPolicyMode _conflictPolicyMode = CalendarConflictPolicyMode.AllowOverlap;
        private int _interactionSnapIntervalMinutes = 15;
        private readonly Stack<CalendarMutationRecord> _undoStack = new();
        private readonly Stack<CalendarMutationRecord> _redoStack = new();
        private bool _suspendHistory;

        // Toolbar uses painted buttons (BeepCalendar.Toolbar.cs) — no child controls needed

        private const int InteractionDragThresholdPx = 4;
        private const int ResizeHandleHitSizePx = 6;
        private bool _pointerDown;
        private bool _dragInProgress;
        private Point _pointerDownLocation;
        private CalendarInteractionHitTestResult _activeInteractionHit;

        private bool _controlsInitialized;
        private bool IsDesignModeSafe => LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode || (Site?.DesignMode ?? false);

        // Exposed paddings for title & grid
        [Browsable(true)] [Category("Layout")] public int HeaderLeftPadding { get; set; } = 160; // min
        [Browsable(true)] [Category("Layout")] public int HeaderRightPadding { get; set; } = CalendarLayoutMetrics.HeaderRightPadding;
        [Browsable(true)] [Category("Layout")] public int GridLeftGutter { get; set; } = 0;
    }
}
