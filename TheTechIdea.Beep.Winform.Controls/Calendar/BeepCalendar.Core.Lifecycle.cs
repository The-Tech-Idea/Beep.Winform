using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Editor.SampleEditors;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            HandleClientAreaChanged();
        }

        /// <summary>
        /// Fires on Dock / splitter changes where the client area changes
        /// without OnResize. Mirrors OnResize's layout + editor sync.
        /// </summary>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            HandleClientAreaChanged();
        }

        private void HandleClientAreaChanged()
        {
            if (!IsDesignModeSafe)
            {
                RequestLayoutUpdate();
            }
            SyncEditorLayerBounds();

            // W2-Redo-5 GAP 4: W8 cell-component Controls are anchored to
            // the cell rect at activation time. After a resize the cells
            // have moved, so the hosted Control would render at a stale
            // location. Drop every active W8 host — the user re-clicks to
            // re-activate at the new cell rect.
            DeactivateAllCellComponents();

            // W2-Redo-5 GAP 5: W4 sample editors are anchored to the
            // editor layer's top-left (Y is invariant) and only their
            // width depends on the client width, so update the width in
            // place so an open title editor still spans the new layout.
            if (_editorHost != null)
            {
                int newWidth = ClientRectangle.Width - 2 * 8;
                if (newWidth < 0) newWidth = 0;
                var active = _editorHost.ActiveEditors;
                for (int i = 0; i < active.Count; i++)
                {
                    var c = active[i].Control;
                    if (c != null) c.Width = newWidth;
                }
            }
        }

        /// <summary>
        /// W3 - keeps the editor layer sized to the calendar's client area.
        /// W4 will replace this with per-editor binding (sidebar rect, header
        /// text bounds, or event-bar rect) driven by the active descriptor id.
        /// </summary>
        private void SyncEditorLayerBounds()
        {
            if (_editorLayer == null) return;
            _editorLayer.Bounds = ClientRectangle;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!IsDesignModeSafe)
            {
                RequestLayoutUpdate();
            }
            SyncEditorLayerBounds();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (IsDesignModeSafe) return;
            Focus();
            _keyboardFocusVisible = false;

            // W2-Redo-6 GAP 1 - if the click landed on a toolbar button,
            // execute its Action and return. OnMouseDown already updated
            // _toolbarHoveredIndex (and short-circuited interaction
            // setup) for the same click, so by the time OnMouseClick
            // fires we just need to dispatch the action.
            HitTestToolbar(e.Location);
            if (IsClickOnToolbarButton(e.Location))
            {
                ExecuteToolbarClick(e.Location);
                return;
            }

            // Hit-test through the per-view painter. The legacy
            // CalendarRenderer / CalendarRenderContext pipeline is gone.
            var hit = ResolveInteractionTarget(e.Location);
            if (hit == null || !hit.HasTarget) return;

            // The interaction target result drives the existing
            // BeepCalendar interaction pipeline via the public hit-test
            // method consumers expect. We do not need to manually call
            // UpdateViewButtonStates here — the painted toolbar's
            // active-state is recomputed in PaintToolbar via IsViewActive.
        }

        // W4 + W8 - double-click opens an inline editor for the double-clicked
        // cell. W8 takes precedence: if the developer registered an
        // IBeepUIComponent factory for the hit cell kind, that component is
        // hosted in the editor layer at the cell's bounds. Otherwise we fall
        // back to the W4 sample editor (InlineEventTitleEditor for events).
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (IsDesignModeSafe) return;

            var hit = ResolveInteractionTarget(e.Location);
            if (hit == null || !hit.HasTarget) return;

            if (hit.TargetKind == CalendarInteractionTargetKind.SidebarDetailCard
                && _state.SelectedEvent != null && _componentCache.GetFactory() != null)
            {
                var sidebarCtx = new CalendarCellContext(
                    CalendarCellKind.SidebarDetailCard,
                    _state.SelectedEvent,
                    _state.CurrentDate,
                    _state.ViewMode, 0, 0);
                if (ActivateCellComponent("sidebar:detail", sidebarCtx, hit.Bounds))
                {
                    return;
                }
            }

            if (hit.TargetKind == CalendarInteractionTargetKind.EventBlock && hit.Event != null)
            {
                var ctx = new CalendarCellContext(
                    CalendarCellKind.EventBlock,
                    hit.Event,
                    hit.Event.StartTime.Date,
                    _state.ViewMode,
                    0,
                    0);
                string cellKey = $"evt:{hit.Event.Id}";

                // W8 path: developer-supplied IBeepUIComponent.
                if (_componentCache.GetFactory() != null)
                {
                    if (ActivateCellComponent(cellKey, ctx, hit.Bounds))
                    {
                        return;
                    }
                }

                // W4 fallback: sample editor. Commit any prior edit first.
                EndEdit(true);
                try
                {
                    _ = BeginEdit(hit.Event, InlineEventTitleEditor.Id);
                }
                catch
                {
                    // editor not registered / host not initialized → swallow
                }
                return;
            }

            if (hit.TargetKind == CalendarInteractionTargetKind.DateCell)
            {
                // W8 + W4 fallback for time-slot hits. A double-click on a
                // time slot in a timed view (Week / WorkWeek / Day / Week1 /
                // Week2 / Week3 / Week7) returns a DateCell hit with a
                // time-of-day != midnight. We try the developer's W8
                // CellComponentFactory first, then fall back to a W4
                // create-event at the clicked time-of-day.
                if (hit.Date.HasValue && hit.Date.Value.TimeOfDay != TimeSpan.Zero)
                {
                    int hour = hit.Date.Value.Hour;
                    var slotCtx = new CalendarCellContext(
                        CalendarCellKind.TimeSlot,
                        null,
                        hit.Date.Value,
                        _state.ViewMode,
                        hour,
                        0);
                    string slotCellKey = $"slot:{hit.Date.Value:yyyy-MM-dd}:{hour}";
                    if (_componentCache.GetFactory() != null)
                    {
                        if (ActivateCellComponent(slotCellKey, slotCtx, hit.Bounds))
                        {
                            return;
                        }
                    }

                    // W4 fallback: focus the time-bearing date and create a
                    // new event starting at the clicked time-of-day.
                    _state.SelectedDate = hit.Date.Value.Date;
                    _state.CurrentDate = hit.Date.Value.Date;
                    _focusedDate = hit.Date.Value;
                    _state.FocusedDate = hit.Date.Value;
                    OnCreateEventRequested(hit.Date.Value);
                    return;
                }

                var dateForKey = hit.Date ?? _state.CurrentDate.Date;
                var ctx = new CalendarCellContext(
                    CalendarCellKind.DateCell,
                    null,
                    dateForKey,
                    _state.ViewMode,
                    0,
                    0);
                string cellKey = GetDateCellKey(dateForKey, _state.ViewMode);

                if (_componentCache.GetFactory() != null)
                {
                    if (ActivateCellComponent(cellKey, ctx, hit.Bounds))
                    {
                        return;
                    }
                }

                // W2-Redo-5 GAP 7: W4 fallback for an empty date-cell with
                // no factory. Always focus the hit date and create a new
                // event at it. Return immediately so the shotgun
                // "open title editor on the currently selected event"
                // fallback below does NOT fire for an unrelated selected
                // event — that was the bug (clicking Jan 5 opened the
                // title editor for a Jan 20 event if one was selected).
                if (hit.Date.HasValue)
                {
                    _state.SelectedDate = hit.Date.Value.Date;
                    _state.CurrentDate = hit.Date.Value.Date;
                    _focusedDate = hit.Date.Value.Date;
                    _state.FocusedDate = hit.Date.Value.Date;
                    CreateEventAtFocusedDate();
                    return;
                }
            }

            // W4 fallback for OTHER hit types (header cell, sidebar,
            // toolbar, time gutter, etc. — anything that is not an event
            // block and not a date cell): open the title editor on the
            // currently selected event if any. This preserves the
            // pre-W8 shotgun behavior for non-cell surfaces.
            if (_state?.SelectedEvent != null)
            {
                try
                {
                    _ = BeginEdit(_state.SelectedEvent, InlineEventTitleEditor.Id);
                }
                catch
                {
                    // editor not registered / host not initialized → swallow
                }
            }
        }
    }
}