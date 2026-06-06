using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Calendar.Editor;
using TheTechIdea.Beep.Winform.Controls.Calendar.Editor.SampleEditors;
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

            // W3 - editor layer infrastructure. The layer is added to Controls
            // unconditionally (designers see it as a child but the Site=null +
            // [DesignerSerializationVisibility(Hidden)] on the layer type keep
            // it out of the host form's *.Designer.cs).
            _editorLayer = new CalendarEditorLayer();
            _editorHost = new CalendarEditorHost();
            _editorHost.Initialize(this, _editorLayer);
            try
            {
                ((System.ComponentModel.ISupportInitialize)_editorLayer).BeginInit();
                Controls.Add(_editorLayer);
                ((System.ComponentModel.ISupportInitialize)_editorLayer).EndInit();
            }
            catch { /* designer safety */ }

            // W2-Redo-6 GAP 4 - forward empty-area mouse-downs on the
            // editor layer to the calendar's OnMouseDown. Without this,
            // a click that lands on the editor layer's transparent
            // background (anywhere outside a hosted W8/W4 child) is
            // captured by the layer and never reaches the calendar. The
            // W2-Redo-5 GAP 3 W8 deactivation check in OnMouseDown would
            // never fire for these clicks, leaving stale W8 hosts visible.
            // We forward the click to OnMouseDown(e) so the full
            // interaction pipeline (deactivate W8, close W4, start
            // drag-create, hit-test painter) runs as if the user clicked
            // the calendar surface directly. The IsClickInsideActiveEditor
            // + CalendarEditorHost.HitTest guards inside the handler
            // ensure we don't steal clicks from real W8/W4 children.
            _editorLayer.MouseDown += EditorLayer_MouseDownForward;

            // W4 - register the sample editor factories. AddEditor is idempotent
            // (re-registering the same id replaces the descriptor), so this is
            // safe to call from the constructor each time.
            _editorHost.AddEditor(InlineEventTitleEditor.GetDescriptor());
            _editorHost.AddEditor(InlineEventDateRangeEditor.GetDescriptor());
            _editorHost.AddEditor(InlineAllDayToggleEditor.GetDescriptor());

            // W2-Redo-13 GAP A - W4 inline editors modify the CalendarEvent
            // in-place through their Saving handlers (e.g. the title editor
            // does current.Title = textBox.Text). The calendar must be
            // notified after commit so it can (a) invalidate the event-
            // service cache, (b) record undo history, (c) raise
            // EventMutated, and (d) repaint. Pre-W2-Redo-13 these three
            // steps were skipped on W4 edit commit — the event was silently
            // modified in _events with no undo, no notification, and
            // potentially stale cached query results.
            _editorHost.EditStarted += (s, hosted) =>
            {
                _editingBeforeSnapshot = hosted.Event != null ? CloneEvent(hosted.Event) : null;
            };
            _editorHost.EditCommitted += (s, hosted) =>
            {
                _eventService?.InvalidateCache();
                _componentCache?.DisposeAll();
                if (_editingBeforeSnapshot != null && hosted.Event != null)
                {
                    RecordMutationHistory(CalendarEventMutationKind.Update,
                        _editingBeforeSnapshot, hosted.Event);
                    RaiseMutated(CalendarEventMutationKind.Update,
                        _editingBeforeSnapshot, hosted.Event, hosted.Event, false,
                        GetConflicts(hosted.Event, _editingBeforeSnapshot));
                    _editingBeforeSnapshot = null;
                }
                RequestRedraw();
            };
            _editorHost.EditCancelled += (s, hosted) =>
            {
                _editingBeforeSnapshot = null;
                RequestRedraw();
            };

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
