using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Editor
{
    /// <summary>
    /// Owns the active <see cref="HostedEditor"/> instances and the registry
    /// of <see cref="CalendarEditorDescriptor"/>s available to the calendar.
    /// Sits on top of <see cref="CalendarEditorLayer"/> and is responsible
    /// for begin / end / hit-test lifecycle of inline editors.
    ///
    /// W3 provides the infrastructure; W4 wires up the sample editors
    /// (<c>InlineEventTitleEditor</c>, <c>InlineEventDateRangeEditor</c>,
    /// <c>InlineAllDayToggleEditor</c>) and exposes public
    /// <c>BeepCalendar.BeginEdit / EndEdit</c> API.
    /// </summary>
    public sealed class CalendarEditorHost
    {
        private readonly Dictionary<string, CalendarEditorDescriptor> _registry = new(StringComparer.Ordinal);
        private readonly List<HostedEditor> _active = new();
        private BeepCalendar _owner;
        private CalendarEditorLayer _layer;
        private readonly CalendarEditorPool _pool = new();

        public event EventHandler<HostedEditor> EditStarted;
        public event EventHandler<HostedEditor> EditCommitted;
        public event EventHandler<HostedEditor> EditCancelled;

        public BeepCalendar Owner => _owner;
        public CalendarEditorLayer Layer => _layer;
        public IReadOnlyList<HostedEditor> ActiveEditors => _active;
        public CalendarEditorPool Pool => _pool;

        /// <summary>Called by <c>BeepCalendar</c> constructor after the layer has been created.</summary>
        public void Initialize(BeepCalendar owner, CalendarEditorLayer layer)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _layer = layer ?? throw new ArgumentNullException(nameof(layer));
        }

        public void AddEditor(CalendarEditorDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            _registry[descriptor.Id] = descriptor;
        }

        public bool RemoveEditor(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return _registry.Remove(id);
        }

        public bool TryGetDescriptor(string id, out CalendarEditorDescriptor descriptor)
        {
            return _registry.TryGetValue(id, out descriptor);
        }

        public HostedEditor BeginEdit(string id, CalendarEvent evt, Rectangle bounds)
        {
            if (!_registry.TryGetValue(id, out var descriptor))
                throw new InvalidOperationException($"No editor registered for id '{id}'.");
            if (_layer == null)
                throw new InvalidOperationException("CalendarEditorHost has not been initialized.");

            // End any existing edit first (commit previous value).
            if (_active.Count > 0)
                EndEdit(commit: true);

            var hosted = descriptor.Factory();
            if (hosted == null || hosted.Control == null)
                throw new InvalidOperationException($"Editor factory for '{id}' returned a null HostedEditor.");

            _layer.SuspendLayout();
            try
            {
                _layer.Controls.Add(hosted.Control);
                hosted.BeginEdit(evt, bounds);
                _layer.ResumeLayout(performLayout: true);
            }
            catch
            {
                _layer.ResumeLayout(performLayout: false);
                throw;
            }

            // Subscribe to the editor's commit / cancel requests so that Enter,
            // Esc, focus leave, and check toggle all funnel through EndEdit.
            hosted.CommitRequested += OnHostedCommitRequested;
            hosted.CancelRequested += OnHostedCancelRequested;

            _active.Add(hosted);
            _layer.Visible = true;
            hosted.Control.Focus();

            EditStarted?.Invoke(this, hosted);
            return hosted;
        }

        public void EndEdit(bool commit)
        {
            if (_active.Count == 0) return;
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                var hosted = _active[i];
                hosted.CommitRequested -= OnHostedCommitRequested;
                hosted.CancelRequested -= OnHostedCancelRequested;
                hosted.EndEdit(commit);
                _layer.Controls.Remove(hosted.Control);
                _pool.Release(hosted.Control);

                if (commit) EditCommitted?.Invoke(this, hosted);
                else EditCancelled?.Invoke(this, hosted);
            }
            _active.Clear();
            _layer.Visible = false;
        }

        private void OnHostedCommitRequested(object sender, EventArgs e) => EndEdit(commit: true);
        private void OnHostedCancelRequested(object sender, EventArgs e) => EndEdit(commit: false);

        public HostedEditor HitTest(Point p)
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                var hosted = _active[i];
                if (hosted.Control != null && hosted.Control.Bounds.Contains(p))
                    return hosted;
            }
            return null;
        }
    }
}
