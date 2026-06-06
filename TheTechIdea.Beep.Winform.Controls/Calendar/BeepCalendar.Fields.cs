using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Editor;
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

        // Inline editor layer (W3). _editorLayer is the transparent child Panel
        // that hosts real Beep controls (BeepTextBox, BeepDateTimePicker, etc.)
        // for in-place event editing. _editorHost owns the active editors and
        // is the public surface for BeginEdit / EndEdit.
        private CalendarEditorLayer _editorLayer;
        private CalendarEditorHost _editorHost;

        // Cell component cache (W8). _componentCache holds developer-supplied
        // IBeepUIComponent instances, one per cell key. The developer registers
        // a single factory via CellComponentFactory and the calendar's
        // DrawCellComponent uses
        // the cache to fetch the right component for paint. Click-to-edit
        // routes through ActivateCellComponent which hosts the actual Control
        // inside _editorLayer.
        private readonly CalendarCellComponentCache _componentCache = new CalendarCellComponentCache();

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
        // W2-Redo-7 GAP 3 - the painters hardcode 4px (Month, Week4) or 6px
        // (timed views + Week5/6) in their ResolveResizeEdge calls. The
        // shared ResizeHandleHitSizePx const was never wired in. Removed
        // to avoid suggesting a single value that doesn't reflect the
        // per-view handle sizes the painters actually use.
        private bool _pointerDown;
        private bool _dragInProgress;
        private Point _pointerDownLocation;
        private CalendarInteractionHitTestResult _activeInteractionHit;

        // W2-Redo-13 GAP A - W4 inline editor before-state snapshot.
        // When the W4 sample editors (title / date-range / all-day toggle)
        // commit, their Saving handler modifies the CalendarEvent in-place.
        // Recording undo history and raising EventMutated requires the
        // pre-edit state. This field is captured in the EditStarted handler
        // and consumed in the EditCommitted handler.
        private CalendarEvent _editingBeforeSnapshot;

        private bool _controlsInitialized;
        private bool IsDesignModeSafe => LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode || (Site?.DesignMode ?? false);

        // Exposed paddings for title & grid
        [Browsable(true)] [Category("Layout")] public int HeaderLeftPadding { get; set; } = 160; // min
        [Browsable(true)] [Category("Layout")] public int HeaderRightPadding { get; set; } = CalendarLayoutMetrics.HeaderRightPadding;
        [Browsable(true)] [Category("Layout")] public int GridLeftGutter { get; set; } = 0;
    }
}
