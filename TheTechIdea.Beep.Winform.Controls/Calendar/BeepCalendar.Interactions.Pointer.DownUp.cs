using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (IsDesignModeSafe)
            {
                return;
            }

            Focus();
            _keyboardFocusVisible = false;

            // W2-Redo-6 GAP 1 - update the toolbar's hover state from the
            // current mouse location. HitTestToolbar() sets
            // _toolbarHoveredIndex; PaintToolbar reads it to draw the hover
            // background. Previously this was dead code — HitTestToolbar
            // was never called, so the toolbar buttons never highlighted
            // and the entire toolbar was non-functional.
            HitTestToolbar(e.Location);
            bool onToolbarButton = IsClickOnToolbarButton(e.Location);
            if (onToolbarButton)
            {
                // The click landed on a toolbar button. Don't start an
                // interaction; OnMouseClick will execute the button's
                // Action (NavigatePrevious, GoToToday, Undo, etc.). We
                // also don't capture or set _pointerDown because the
                // toolbar buttons are not draggable.
                return;
            }

            _pointerDown = e.Button == MouseButtons.Left;
            _dragInProgress = false;
            _pointerDownLocation = e.Location;
            _activeInteractionHit = ResolveInteractionTarget(e.Location);

            // W2-Redo-6 GAP 5 - W4 sample editor close on deselect. When a
            // W4 title/date-range/all-day editor is open for Event A and
            // the user clicks a different event, an empty cell, or any
            // non-self-event surface, the W4 editor should commit and
            // close (matching the user's apparent intent to interact with
            // something else). The W4 editor can only stay open when the
            // user clicks inside its own bounds — and that case is handled
            // by the W4 editor's own Leave/Commit flow when the textbox
            // loses focus.
            if (IsClickOutsideW4Editor(e.Location))
            {
                EndEdit(commit: true);
            }

            // W2-Redo-5 GAP 3 - W8 cell components are visible at the
            // active cell's bounds. When the user clicks elsewhere on the
            // calendar's surface (a different cell, an event, or empty
            // area), the W8 host should close. In WinForms, clicks on a
            // hosted Control are captured by the Control and do NOT
            // reach BeepCalendar.OnMouseDown, so reaching this point
            // means the click is outside the editor. The
            // IsClickInsideActiveEditor guard handles edge cases (e.g.
            // clicks on the editor layer's background that don't get
            // captured by a hosted Control).
            if (!IsClickInsideActiveEditor(e.Location))
            {
                DeactivateAllCellComponents();
            }

            if (_activeInteractionHit?.Date.HasValue == true)
            {
                // W2-Redo-11 BUG B - preserve the time-of-day in
                // _focusedDate. The previous assignment routed through
                // _state.SelectedDate, which is always a midnight Date.
                // In OnMouseDoubleClick (BeepCalendar.Core.Lifecycle.cs)
                // the time-of-day is preserved via
                // _focusedDate = hit.Date.Value;. Without this fix a
                // user clicking a 14:30 timed-view cell and then
                // pressing Right would jump to the next *midnight*,
                // not the next 14:30 — silently shifting the focused
                // time on every click.
                _state.SelectedDate = _activeInteractionHit.Date.Value.Date;
                _state.CurrentDate = _activeInteractionHit.Date.Value.Date;
                _focusedDate = _activeInteractionHit.Date.Value;
                _state.FocusedDate = _focusedDate;
                DateSelected?.Invoke(this, new CalendarDateArgs(_state.SelectedDate));
            }

            if (_activeInteractionHit?.Event != null)
            {
                _state.SelectedEvent = _activeInteractionHit.Event;
                // W2-Redo-11 BUG B (cont.) - preserve the event's
                // StartTime time-of-day in _focusedDate so keyboard
                // navigation continues from the clicked event's actual
                // timestamp, not from midnight of the same day.
                _state.SelectedDate = _activeInteractionHit.Event.StartTime.Date;
                _state.CurrentDate = _activeInteractionHit.Event.StartTime.Date;
                _focusedDate = _activeInteractionHit.Event.StartTime;
                _state.FocusedDate = _focusedDate;
                EventSelected?.Invoke(this, new CalendarEventArgs(_state.SelectedEvent));
            }

            _state.IsPointerDown = _pointerDown;
            _state.PointerDownLocation = _pointerDown ? (Point?)_pointerDownLocation : null;
            _state.InteractionMode = _activeInteractionHit?.RequestedMode ?? CalendarInteractionMode.None;
            _state.InteractionTargetKind = _activeInteractionHit?.TargetKind ?? CalendarInteractionTargetKind.None;
            _state.InteractionEventId = _activeInteractionHit?.Event?.Id;

            // W2-Redo-6 GAP 2 - capture the mouse for the duration of the
            // drag so the calendar keeps receiving MouseMove / MouseUp
            // events even when the cursor leaves the control. Without
            // this, dragging an event off the calendar (or resizing
            // beyond the bottom edge) drops the drag mid-gesture.
            if (_pointerDown)
            {
                Capture = true;
            }

            InteractionStarted?.Invoke(this, new CalendarInteractionEventArgs(
                _state.InteractionMode,
                _state.InteractionTargetKind,
                _state.SelectedDate,
                _state.SelectedEvent,
                e.Location,
                Point.Empty,
                isCommit: false,
                modifierKeys: ModifierKeys,
                isCopyOperation: IsCopyModifierDown()));

            Invalidate();
        }

        /// <summary>
        /// GAP C helper - true when <paramref name="location"/> falls inside
        /// any active W8 hosted Control's bounds. Used to avoid deactivating
        /// the W8 layer when the user clicks inside the editor they're
        /// actively interacting with.
        /// </summary>
        private bool IsClickInsideActiveEditor(Point location)
        {
            if (_editorLayer == null) return false;
            for (int i = 0; i < _editorLayer.Controls.Count; i++)
            {
                if (_editorLayer.Controls[i] is Control c && c.Tag is string && c.Visible && c.Bounds.Contains(location))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// W2-Redo-6 GAP 5 helper - true when no W4 sample editor is
        /// active OR the click is outside the W4 editor's bounds. Used in
        /// <see cref="OnMouseDown"/> to commit and close the W4 editor
        /// when the user clicks elsewhere on the calendar. Clicks inside
        /// the W4 editor's bounds are handled by the editor's own
        /// Leave/Commit flow (textbox focus change, Enter, Esc).
        /// </summary>
        private bool IsClickOutsideW4Editor(Point location)
        {
            if (_editorHost == null) return false;
            var active = _editorHost.ActiveEditors;
            if (active == null || active.Count == 0) return false;
            for (int i = 0; i < active.Count; i++)
            {
                var ctrl = active[i].Control;
                if (ctrl != null && ctrl.Visible && ctrl.Bounds.Contains(location))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// W2-Redo-6 GAP 1 helper - true when <paramref name="location"/>
        /// lands on a real (non-spacer, non-null-Action) toolbar button.
        /// Used by <see cref="OnMouseDown"/> to short-circuit interaction
        /// setup when the click is a toolbar button click, and by
        /// <see cref="BeepCalendar.Core.Lifecycle.OnMouseClick"/> to know
        /// when to invoke <c>ExecuteToolbarClick</c>.
        /// </summary>
        private bool IsClickOnToolbarButton(Point location)
        {
            if (_toolbarButtons == null || _toolbarHoveredIndex < 0) return false;
            if (_toolbarHoveredIndex >= _toolbarButtons.Count) return false;
            var btn = _toolbarButtons[_toolbarHoveredIndex];
            if (btn.Action == null) return false;
            if (string.IsNullOrEmpty(btn.Key) || btn.Key.StartsWith("spacer")) return false;
            return btn.Bounds.Contains(location);
        }

        /// <summary>
        /// W2-Redo-6 GAP 4 - forward a left-button mouse-down on the
        /// editor layer's empty area to <see cref="OnMouseDown"/>. Clicks
        /// that land on a hosted W8 Control or a W4 sample editor's
        /// wrapped Control are captured by the child and never reach
        /// here, so we only need to forward "empty background" clicks.
        /// The <see cref="IsClickInsideActiveEditor"/> + W4 hit-test
        /// guards are belt-and-braces in case a child doesn't mark the
        /// event as handled. MouseEventArgs coordinates are in the
        /// editor layer's coordinate system; because the layer is sized
        /// to the calendar's <c>ClientRectangle</c> at (0, 0) the
        /// location is identical in calendar coordinates.
        /// </summary>
        private void EditorLayer_MouseDownForward(object sender, MouseEventArgs e)
        {
            if (IsDesignModeSafe) return;
            if (e.Button != MouseButtons.Left) return;
            // Don't steal clicks from a W8 hosted Control.
            if (IsClickInsideActiveEditor(e.Location)) return;
            // Don't steal clicks from a W4 sample editor's wrapped
            // Control (textbox, date pickers, etc.).
            if (_editorHost?.HitTest(e.Location) != null) return;
            // Empty area of the editor layer — forward as a calendar click.
            OnMouseDown(e);
        }

    }
}