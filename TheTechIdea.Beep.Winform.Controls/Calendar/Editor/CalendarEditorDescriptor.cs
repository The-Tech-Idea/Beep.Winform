using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Editor
{
    /// <summary>
    /// Describes a registered inline editor that can be hosted inside the
    /// <see cref="CalendarEditorLayer"/> for in-place editing of a
    /// <see cref="CalendarEvent"/>.
    ///
    /// W3 wires the infrastructure; concrete editor factories
    /// (<c>InlineEventTitleEditor</c>, <c>InlineEventDateRangeEditor</c>,
    /// <c>InlineAllDayToggleEditor</c>) are registered in W4.
    /// </summary>
    public sealed class CalendarEditorDescriptor
    {
        public CalendarEditorDescriptor(
            string id,
            string displayName,
            bool supportsInline,
            bool supportsDialog,
            Func<HostedEditor> factory)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Editor id must be non-empty.", nameof(id));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            Id = id;
            DisplayName = displayName ?? id;
            SupportsInline = supportsInline;
            SupportsDialog = supportsDialog;
            Factory = factory;
        }

        /// <summary>Stable identifier (e.g. "title", "daterange", "allday").</summary>
        public string Id { get; }

        /// <summary>Human-readable label for dialogs, smart-tags, logs.</summary>
        public string DisplayName { get; }

        /// <summary>Whether this editor can be hosted inside the inline layer.</summary>
        public bool SupportsInline { get; }

        /// <summary>Whether this editor can be hosted inside a modal dialog.</summary>
        public bool SupportsDialog { get; }

        /// <summary>Factory used by <see cref="CalendarEditorHost.BeginEdit"/> to create a new instance.</summary>
        public Func<HostedEditor> Factory { get; }
    }
}
