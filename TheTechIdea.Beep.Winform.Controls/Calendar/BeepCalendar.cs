using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    [DesignerCategory("")]
    [ToolboxBitmap(typeof(BeepCalendar), "BeepCalendar.ico")]
    [Description("A comprehensive calendar control with event management, multiple views, and scheduling capabilities.")]
    public partial class BeepCalendar : BaseControl, ICalendarCellHost
    {
        protected override Size DefaultSize => BeepLayoutMetrics.Calendar;
        protected override bool AllowBaseControlClear => false;
        protected override bool IsContainerControl => true;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (_editorLayer == null || !_editorLayer.Visible)
            {
                base.OnPaintBackground(e);
                return;
            }

            var clipRegion = new Region(e.ClipRectangle);
            for (int i = 0; i < _editorLayer.Controls.Count; i++)
            {
                if (_editorLayer.Controls[i] is Control c && c.Visible)
                    clipRegion.Exclude(c.Bounds);
            }
            var prev = e.Graphics.Clip;
            e.Graphics.Clip = clipRegion;
            try { base.OnPaintBackground(e); }
            finally
            {
                e.Graphics.Clip = prev;
                clipRegion.Dispose();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_editorLayer == null || !_editorLayer.Visible)
            {
                DrawContent(e.Graphics);
                return;
            }

            var clipRegion = new Region(e.ClipRectangle);
            for (int i = 0; i < _editorLayer.Controls.Count; i++)
            {
                if (_editorLayer.Controls[i] is Control c && c.Visible)
                    clipRegion.Exclude(c.Bounds);
            }
            var prev = e.Graphics.Clip;
            e.Graphics.Clip = clipRegion;
            try { DrawContent(e.Graphics); }
            finally
            {
                e.Graphics.Clip = prev;
                clipRegion.Dispose();
            }
        }

        // W4 - Esc cancels the active inline editor first, before falling
        // through to the calendar's other Esc handling (active interaction
        // cancellation is handled in the BeepCalendar.Interactions.* partials).
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape && IsEditing)
            {
                EndEdit(commit: false);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // W2-Redo-7 GAP 4 - release all calendar-owned resources when the
        // control is disposed. Without this, the cell-component cache, the
        // editor pool (which keeps up to 16 Controls per type cached), the
        // hosted W8 / W4 Controls inside the editor layer, the editor
        // layer's MouseDown subscription, and the undo / redo stacks all
        // leak when the calendar is destroyed in long-lived scenarios
        // (e.g. tabbed MDI hosts, dashboard refreshes). EndEdit(false) is
        // safe to call multiple times; _editorHost is null-checked.
        //
        // W2-Redo-8 GAP 2 - W7 added _componentCache.Clear() which removed
        // the cache entries but DID NOT dispose the IBeepUIComponent
        // instances they referenced (Controls held in the editor layer at
        // activation time would never be released). DisposeAll() walks the
        // cache and disposes every IDisposable component before clearing.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try { EndEdit(commit: false); } catch { /* swallow on dispose */ }
                DeactivateAllCellComponents();
                if (_componentCache != null)
                {
                    _componentCache.DisposeAll();
                }
                if (_editorHost != null)
                {
                    try { _editorHost.Pool?.DisposeAll(); } catch { }
                }
                if (_editorLayer != null)
                {
                    try { _editorLayer.MouseDown -= EditorLayer_MouseDownForward; } catch { }
                    if (_editorLayer.Parent != null)
                    {
                        try { _editorLayer.Parent.Controls.Remove(_editorLayer); } catch { }
                    }
                    try { _editorLayer.Dispose(); } catch { }
                    _editorLayer = null;
                }
                _undoStack.Clear();
                _redoStack.Clear();
            }
            base.Dispose(disposing);
        }

        // W2-Redo-7 GAP 5 - basic keyboard navigation. The previous code had
        // _focusedDate / _keyboardFocusVisible fields but no handler ever
        // wrote to them, so arrow keys did nothing. The view painter owns
        // NavigatePrevious/Next; we use them for Left/Right (day) and
        // Up/Down (week) and fall back to NavigateNextPeriod/NavigatePreviousPeriod
        // for the rest. Enter opens the focused-date create flow so the
        // user can create an event with the keyboard alone.
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (IsDesignModeSafe) return;
            if (IsEditing) return; // let the W4 editor consume the key

            // If a W8 cell component is hosted in the editor layer, drop it
            // so the calendar can navigate with arrow keys. Without this,
            // keyboard navigation is blocked when a W8 Control has stolen
            // focus or would render at stale coordinates after the date shift.
            DeactivateAllCellComponents();

            _keyboardFocusVisible = true;

            DateTime? newFocus = null;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    newFocus = _focusedDate.AddDays(-1);
                    break;
                case Keys.Right:
                    newFocus = _focusedDate.AddDays(1);
                    break;
                case Keys.Up:
                    newFocus = _focusedDate.AddDays(-7);
                    break;
                case Keys.Down:
                    newFocus = _focusedDate.AddDays(7);
                    break;
                case Keys.PageUp:
                    newFocus = _viewPainter != null
                        ? _viewPainter.NavigatePrevious(_focusedDate)
                        : _focusedDate.AddMonths(-1);
                    break;
                case Keys.PageDown:
                    newFocus = _viewPainter != null
                        ? _viewPainter.NavigateNext(_focusedDate)
                        : _focusedDate.AddMonths(1);
                    break;
                case Keys.Home:
                    int dow = (int)_focusedDate.DayOfWeek;
                    newFocus = _focusedDate.AddDays(-dow);
                    break;
                case Keys.End:
                    dow = (int)_focusedDate.DayOfWeek;
                    newFocus = _focusedDate.AddDays(6 - dow);
                    break;
                case Keys.Enter:
                    OnCreateEventRequested(_focusedDate);
                    e.Handled = true;
                    return;
                default:
                    return;
            }

            if (newFocus.HasValue)
            {
                _focusedDate = newFocus.Value;
                _state.FocusedDate = _focusedDate;
                _state.SelectedDate = _focusedDate.Date;
                _state.CurrentDate = _focusedDate.Date;
                DateSelected?.Invoke(this, new CalendarDateArgs(_state.SelectedDate));
                RequestLayoutAndRedraw();
                e.Handled = true;
            }
        }

        // W2-Redo-7 GAP 6 - mouse wheel navigates by one period. Holding
        // Ctrl while scrolling swaps to the next/previous view mode (mirrors
        // Ctrl+Left/Right in most calendar apps). Note: WinForms'
        // MouseEventArgs does not expose a Handled flag, so we suppress the
        // base behavior by simply not forwarding the event to the scrollable
        // container.
        //
        // W2-Redo-8 GAP 4 - Focus() before navigating so the next arrow-key
        // press (handled by OnKeyDown) actually reaches the calendar.
        // Without this, focus stays on whatever was focused when the user
        // started scrolling (often the form's chrome or another control),
        // and arrow keys would not work until the user clicked the calendar
        // first.
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (IsDesignModeSafe) return;
            if (!Focused) Focus();
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                int delta = e.Delta > 0 ? -1 : 1;
                var views = new List<KeyValuePair<CalendarViewMode, ICalendarViewPainter>>(
                    ViewPainterFactory.GetRegisteredViews());
                if (views.Count == 0) return;
                int idx = -1;
                for (int i = 0; i < views.Count; i++)
                {
                    if (views[i].Value?.Key == _viewPainter?.Key) { idx = i; break; }
                }
                if (idx < 0) return;
                int next = ((idx + delta) % views.Count + views.Count) % views.Count;
                ViewMode = views[next].Key;
                return;
            }
            if (e.Delta > 0)
            {
                NavigatePreviousPeriod();
            }
            else if (e.Delta < 0)
            {
                NavigateNextPeriod();
            }
        }
    }
}
