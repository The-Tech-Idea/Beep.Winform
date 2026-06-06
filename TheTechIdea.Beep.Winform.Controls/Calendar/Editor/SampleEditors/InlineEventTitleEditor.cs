using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Editor.SampleEditors
{
    /// <summary>
    /// W4 sample editor — a single <see cref="BeepTextBox"/> that edits
    /// <see cref="CalendarEvent.Title"/> in place.
    ///
    /// Behavior:
    /// <list type="bullet">
    ///   <item><b>Enter</b> raises <see cref="HostedEditor.RequestCommit"/> so
    ///         <see cref="CalendarEditorHost.EndEdit"/>(true) fires
    ///         <see cref="CalendarEditorHost.EditCommitted"/>.</item>
    ///   <item><b>Esc</b> raises <see cref="HostedEditor.RequestCancel"/> so
    ///         <see cref="CalendarEditorHost.EndEdit"/>(false) fires
    ///         <see cref="CalendarEditorHost.EditCancelled"/>.</item>
    ///   <item>On <see cref="HostedEditor.Loading"/> the textbox receives
    ///         <c>evt.Title</c> and selects all so the user can type to
    ///         replace.</item>
    ///   <item>On <see cref="HostedEditor.Saving"/> the textbox text is
    ///         written back to <c>evt.Title</c>.</item>
    /// </list>
    /// </summary>
    public static class InlineEventTitleEditor
    {
        public const string Id = "title";
        public const string DisplayName = "Title";

        public static HostedEditor Create()
        {
            var textBox = new BeepTextBox
            {
                Site = null,
                ShowAllBorders = true,
                TabStop = true
            };
            var hosted = new HostedEditor(GetDescriptor(), textBox);
            CalendarEvent? current = null;

            hosted.Loading += (s, evt) =>
            {
                current = evt;
                textBox.Text = evt?.Title ?? string.Empty;
                textBox.SelectAll();
            };
            hosted.Saving += (s, evt) =>
            {
                if (current != null) current.Title = textBox.Text ?? string.Empty;
            };
            textBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    hosted.RequestCommit();
                }
                else if (e.KeyCode == Keys.Escape)
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
