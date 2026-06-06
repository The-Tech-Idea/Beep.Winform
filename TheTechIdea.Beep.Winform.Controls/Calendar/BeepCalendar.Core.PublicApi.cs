using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Calendar.CellRender;
using TheTechIdea.Beep.Winform.Controls.Calendar.Editor;
using TheTechIdea.Beep.Winform.Controls.Calendar.Editor.SampleEditors;
using TheTechIdea.Beep.Winform.Controls.Calendar.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.Colors;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        [Browsable(true)]
        [Category("Calendar")]
        public DateTime CurrentDate
        {
            get => _state.CurrentDate;
            set
            {
                DateTime newDate = value.Date;
                if (_state.CurrentDate == newDate) return;
                _state.CurrentDate = newDate;
                _state.SelectedDate = newDate;
                // W2-Redo-9 GAP 1 - keep _focusedDate in sync with the new
                // current date so keyboard navigation (OnKeyDown) and
                // hit-test (which falls back to _focusedDate when no cell
                // is hit) reflect the new date. Without this, after a
                // programmatic CurrentDate assignment the user could
                // press the right arrow and the focus would jump to
                // (newDate + 1 day) instead of the expected (previous
                // focus + 1 day).
                _focusedDate = newDate;
                _state.FocusedDate = _focusedDate;
                // W2-Redo-9 GAP 1b - W8 cell components (DateCell / TimeSlot
                // kinds) are keyed by the date. When CurrentDate changes
                // those keys no longer match any visible cell but the
                // hosted Control is still visible at its old bounds.
                // Deactivate the W8 layer for the same reason
                // NavigatePrevious/Next/Today do.
                DeactivateAllCellComponents();
                RequestLayoutAndRedraw();
            }
        }
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
                // GAP A - W8 cell components are hosted at the active cell's
                // bounds. When the view changes, those bounds shift (or the
                // cell no longer exists in the new view), so the hosted
                // Control would be left visible at a stale position. Clear
                // the W8 layer and end any active W4 sample editor session
                // (view-switch is a hard context change).
                EndEdit(commit: true);
                RequestLayoutAndRedraw();
            }
        }
        [Browsable(true)]
        [Category("Calendar")] public bool ShowSidebar { get => _state.ShowSidebar; set { _state.ShowSidebar = value; DeactivateAllCellComponents(); RequestLayoutAndRedraw(); } }
        [Browsable(true)]
        [Category("Calendar")]
        [DefaultValue(CalendarDensityMode.Comfortable)]
        public CalendarDensityMode DensityMode
        {
            get => _densityMode;
            set { _densityMode = value; DeactivateAllCellComponents(); RequestLayoutAndRedraw(); }
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
                DeactivateAllCellComponents();
                _componentCache.DisposeAll();
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

        [Browsable(false)]
        public CalendarEvent SelectedEvent
        {
            get => _state.SelectedEvent;
            set
            {
                if (_state.SelectedEvent == value) return;
                _state.SelectedEvent = value;
                if (value != null)
                {
                    _state.SelectedDate = value.StartTime.Date;
                    _focusedDate = value.StartTime.Date;
                    _state.FocusedDate = _focusedDate;
                }
                EventSelected?.Invoke(this, new CalendarEventArgs(value));
                DeactivateAllCellComponents();
                _componentCache.DisposeAll();
                RequestRedraw();
            }
        }

        bool ICalendarCellHost.UpdateEvent(CalendarEvent evt) => TryUpdateEvent(evt);
        bool ICalendarCellHost.RemoveEvent(CalendarEvent evt) { RemoveEvent(evt); return true; }
        void ICalendarCellHost.InvalidateCell() => RequestRedraw();

        [Category("Calendar")] public List<EventCategory> Categories
        {
            get => _categories;
            set { _categories = value ?? new(); InitializeDefaultCategories(); DeactivateAllCellComponents(); _componentCache.DisposeAll(); RequestRedraw(); }
        }

        [Browsable(true)]
        [Category("Calendar")]
        public List<CalendarResource> Resources { get; set; } = new();

        [Browsable(false)]
        public CalendarPerformanceMetrics PerformanceMetrics { get; } = new();

        // ── W4 - inline editor API ─────────────────────────────────────────
        // Hosts the W4 sample editors (title / date-range / all-day) inside
        // the editor layer. BeginEdit opens the editor for the given event;
        // EndEdit commits (true) or discards (false) any pending change.
        // Internal callers (e.g. OnMouseDoubleClick) can use the editorId
        // overload to pick a specific editor.

        /// <summary>
        /// Opens the inline editor identified by <paramref name="editorId"/>
        /// (default "title") for the supplied <paramref name="event"/>.
        /// </summary>
        /// <returns>The hosted editor instance, or null if the host is not yet initialized.</returns>
        public HostedEditor? BeginEdit(CalendarEvent @event, string editorId = InlineEventTitleEditor.Id)
        {
            if (_editorHost == null) return null;
            if (@event == null) throw new System.ArgumentNullException(nameof(@event));
            var bounds = ComputeEditorBounds(editorId, @event);
            return _editorHost.BeginEdit(editorId, @event, bounds);
        }

        /// <summary>
        /// Commits (or, when <paramref name="commit"/> is false, discards) the
        /// active inline editor session.
        /// </summary>
        public void EndEdit(bool commit = true)
        {
            _editorHost?.EndEdit(commit);
            // W8 - deactivate any active IBeepUIComponent cell host so the
            // painter's Draw() takes over again on the next paint cycle.
            DeactivateAllCellComponents();
        }

        /// <summary>
        /// W8 - hide and remove every Control currently hosted in the
        /// editor layer for a cell component. The cached components are
        /// kept (not disposed) and re-shown by future
        /// <see cref="ActivateCellComponent"/> calls.
        /// </summary>
        public void DeactivateAllCellComponents()
        {
            if (_editorLayer == null || _componentCache == null) return;
            for (int i = _editorLayer.Controls.Count - 1; i >= 0; i--)
            {
                if (_editorLayer.Controls[i] is Control c && c.Tag is string)
                {
                    c.Visible = false;
                    _editorLayer.Controls.Remove(c);
                }
            }
            if (_editorLayer.Controls.Count == 0)
                _editorLayer.Visible = false;
        }

        /// <summary>
        /// Returns true when a W8 cell component is hosted in the editor layer.
        /// Used by pointer handlers to suppress hover/drag when the editor layer
        /// intercepts mouse events.
        /// </summary>
        public bool HasActiveCellComponent()
        {
            if (_editorLayer == null) return false;
            for (int i = 0; i < _editorLayer.Controls.Count; i++)
            {
                if (_editorLayer.Controls[i] is Control c && c.Tag is string)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// True when at least one inline editor is currently active.
        /// </summary>
        [Browsable(false)]
        public bool IsEditing => _editorHost != null && _editorHost.ActiveEditors.Count > 0;

        /// <summary>
        /// W4 - default inline editor bounds. The three sample editors
        /// stack vertically inside the editor layer with sensible defaults;
        /// the day-cell-relative position is computed in W5 from the
        /// per-event hit-test result.
        /// </summary>
        private Rectangle ComputeEditorBounds(string editorId, CalendarEvent _)
        {
            int w = ClientRectangle.Width;
            const int barH = 28;
            const int pad = 8;
            const int gap = 6;
            int y = pad;
            switch (editorId)
            {
                case InlineEventTitleEditor.Id:
                    return new Rectangle(pad, y, w - 2 * pad, barH);
                case InlineEventDateRangeEditor.Id:
                    y += barH + gap;
                    return new Rectangle(pad, y, w - 2 * pad, barH);
                case InlineAllDayToggleEditor.Id:
                    y += 2 * (barH + gap);
                    return new Rectangle(pad, y, 200, barH);
                default:
                    return new Rectangle(pad, y, w - 2 * pad, barH);
            }
        }

        // ── W8 - developer-supplied IBeepUIComponent cell render API ─────────
        // Developers can supply a Func<CalendarCellContext, IBeepUIComponent>
        // for each of the three cell kinds (event block / date cell / time
        // slot). The calendar caches the resulting IBeepUIComponent per cell
        // key (event id, date string, or (date, hour) tuple) and calls the
        // component's Draw(g, rect) when the painter renders that cell.
        // When the user clicks a cell, the calendar hosts the actual Control
        // inside _editorLayer (or removes it) so the user can edit the data.

        /// <summary>
        /// Resolves the suffixed date-cell key that matches what the active
        /// per-view painter uses in <see cref="CalendarPainterHelpers.TryDrawCellComponent"/>.
        /// Without the suffix, the double-click handler would create a
        /// duplicate cache entry that the painter never renders.
        /// </summary>
        private static string GetDateCellKey(DateTime date, CalendarViewMode viewMode)
        {
            string d = date.ToString("yyyy-MM-dd");
            return viewMode switch
            {
                CalendarViewMode.Month or CalendarViewMode.Week4 => $"date:{d}:cell",
                CalendarViewMode.Week5 => $"date:{d}:week5-header",
                CalendarViewMode.Week6 => $"date:{d}:week6-header",
                CalendarViewMode.Timeline => $"date:{d}:timeline-header",
                CalendarViewMode.Agenda => $"date:{d}:agenda-header",
                // Week1 / Week2 / Week3 / Week7 / Week / WorkWeek / Day
                _ => $"date:{d}:header"
            };
        }

        [Browsable(false)]
        public Func<CalendarCellContext, IBeepUIComponent> CellComponentFactory
        {
            get => _componentCache.GetFactory();
            set
            {
                DeactivateAllCellComponents();
                _componentCache.DisposeAll();
                _componentCache.SetFactory(value);
                Invalidate();
            }
        }

        /// <summary>
        /// Number of IBeepUIComponent instances currently cached. Test hook.
        /// </summary>
        [Browsable(false)]
        public int CachedCellComponentCount => _componentCache.Count;

        /// <summary>
        /// Remove all cached cell components. Components are NOT disposed;
        /// the next paint will re-create them via the registered factories.
        /// </summary>
        public void ClearCellComponentCache() => _componentCache.Clear();

        /// <summary>
        /// Get (or create) the IBeepUIComponent for the cell at
        /// <paramref name="cellKey"/>. Returns null when no factory is
        /// registered for the cell's <see cref="CalendarCellContext.Kind"/>.
        /// </summary>
        public IBeepUIComponent GetCellComponent(string cellKey, CalendarCellContext ctx)
        {
            if (_componentCache == null) return null;
            ctx.Host = this;
            return _componentCache.GetOrCreate(cellKey, ctx);
        }

        /// <summary>
        /// Painter entry point: get the cached component for the cell at
        /// <paramref name="cellKey"/> and call its
        /// <c>Draw(graphics, rectangle)</c>. When no component is registered
        /// for the cell kind, the call is a no-op and the painter falls back
        /// to its default rendering.
        /// </summary>
        public void DrawCellComponent(Graphics g, Rectangle cellRect, string cellKey, CalendarCellContext ctx)
        {
            if (g == null || cellRect.Width <= 0 || cellRect.Height <= 0 || string.IsNullOrEmpty(cellKey))
            {
                return;
            }
            var comp = _componentCache.GetOrCreate(cellKey, ctx);
            if (comp == null) return;
            try
            {
                if (comp is Control asControl)
                {
                    if (asControl.Parent == null)
                    {
                        asControl.Bounds = cellRect;
                        if (!asControl.Visible) asControl.Visible = true;
                    }
                    if (asControl.Parent != null)
                    {
                        return;
                    }
                }
                comp.Draw(g, cellRect);
            }
            catch
            {
                // drawing failures are non-fatal; the calendar keeps painting
            }
        }

        /// <summary>
        /// Activate the cached component for the cell at <paramref name="cellKey"/>
        /// and host its underlying <see cref="Control"/> inside the editor
        /// layer at <paramref name="bounds"/>. The component is the same
        /// instance the painter uses for <c>Draw</c>; calling
        /// <see cref="DeactivateCellComponent"/> hides it again.
        /// </summary>
        /// <returns>True when a component was found and activated; false otherwise.</returns>
        public bool ActivateCellComponent(string cellKey, CalendarCellContext ctx, Rectangle bounds)
        {
            if (string.IsNullOrEmpty(cellKey) || _editorLayer == null) return false;
            var comp = _componentCache.GetOrCreate(cellKey, ctx);
            if (comp == null) return false;
            if (comp is not Control control) return false;

            control.Bounds = bounds;
            control.Tag = cellKey;
            if (!control.Visible) control.Visible = true;
            if (!_editorLayer.Controls.Contains(control))
            {
                _editorLayer.Controls.Add(control);
            }
            control.BringToFront();
            _editorLayer.Visible = true;
            Focus();
            return true;
        }

        /// <summary>
        /// Hide (and remove from the editor layer) the Control previously
        /// activated by <see cref="ActivateCellComponent"/>. The cached
        /// component itself is NOT disposed; it will be re-shown on the next
        /// activation of the same cell key.
        /// </summary>
        public bool DeactivateCellComponent(string cellKey)
        {
            if (string.IsNullOrEmpty(cellKey) || _editorLayer == null) return false;
            if (!_componentCache.Contains(cellKey)) return false;
            // The cache key is opaque to the calendar; the caller must know
            // which Control to remove. Walk the editor layer looking for a
            // Control whose Tag matches the key.
            for (int i = _editorLayer.Controls.Count - 1; i >= 0; i--)
            {
                if (_editorLayer.Controls[i] is Control c
                    && c.Tag is string tag
                    && string.Equals(tag, cellKey, StringComparison.OrdinalIgnoreCase))
                {
                    c.Visible = false;
                    _editorLayer.Controls.Remove(c);
                    if (_editorLayer.Controls.Count == 0)
                        _editorLayer.Visible = false;
                    return true;
                }
            }
            return false;
        }

    }
}