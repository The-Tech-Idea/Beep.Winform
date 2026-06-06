using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Editor.SampleEditors
{
    /// <summary>
    /// W4 sample editor — a single <see cref="BeepCheckBoxBool"/> that toggles
    /// <see cref="CalendarEvent.IsAllDay"/> in place.
    ///
    /// Behavior:
    /// <list type="bullet">
    ///   <item><b>Check / uncheck</b> raises
    ///         <see cref="HostedEditor.RequestCommit"/> immediately so the
    ///         user does not need to press Enter.</item>
    ///   <item><b>Esc</b> raises <see cref="HostedEditor.RequestCancel"/>.</item>
    ///   <item>On <see cref="HostedEditor.Loading"/> the checkbox receives
    ///         <c>evt.IsAllDay</c>.</item>
    ///   <item>On <see cref="HostedEditor.Saving"/> the checkbox value is
    ///         written back to <c>evt.IsAllDay</c>.</item>
    /// </list>
    /// </summary>
    public static class InlineAllDayToggleEditor
    {
        public const string Id = "allday";
        public const string DisplayName = "All Day";

        public static HostedEditor Create()
        {
            var checkBox = new BeepCheckBoxBool
            {
                Site = null,
                Text = "All Day",
                TabStop = true
            };
            var hosted = new HostedEditor(GetDescriptor(), checkBox);
            CalendarEvent? current = null;
            // W2-Redo-8 GAP 1 - the previous version fired
            // CheckedChanged → RequestCommit on the very first Loading
            // (when Loading sets checkBox.Checked = evt.IsAllDay). For
            // all-day events (IsAllDay=true) the editor opened and
            // immediately closed itself. The _isLoading guard suppresses
            // the commit request while we're seeding the checkbox from
            // the event's current value.
            bool _isLoading = false;

            hosted.Loading += (s, evt) =>
            {
                current = evt;
                _isLoading = true;
                try { checkBox.Checked = evt?.IsAllDay ?? false; }
                finally { _isLoading = false; }
            };
            hosted.Saving += (s, evt) =>
            {
                if (current != null) current.IsAllDay = checkBox.Checked;
            };
            checkBox.CheckedChanged += (s, e) =>
            {
                if (_isLoading) return;
                if (current != null) hosted.RequestCommit();
            };
            checkBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    hosted.RequestCancel();
                }
            };

            return hosted;
        }

        public static CalendarEditorDescriptor GetDescriptor()
        {
            return new CalendarEditorDescriptor(
                id: Id,
                displayName: DisplayName,
                supportsInline: true,
                supportsDialog: true,
                factory: Create);
        }
    }
}
