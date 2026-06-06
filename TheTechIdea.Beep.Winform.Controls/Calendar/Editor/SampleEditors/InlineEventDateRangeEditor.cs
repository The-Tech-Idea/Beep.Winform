using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Dates;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Editor.SampleEditors
{
    /// <summary>
    /// W4 sample editor — two <see cref="BeepDateTimePicker"/> instances side
    /// by side that edit <see cref="CalendarEvent.StartTime"/> and
    /// <see cref="CalendarEvent.EndTime"/>.
    ///
    /// The wrapped control is a tiny <see cref="Panel"/> that hosts the two
    /// pickers (and a dash separator). This keeps
    /// <see cref="CalendarEditorHost"/> single-child-per-descriptor simple
    /// while still giving the user two cooperating inputs.
    ///
    /// Behavior:
    /// <list type="bullet">
    ///   <item><b>Focus leave</b> on either picker raises
    ///         <see cref="HostedEditor.RequestCommit"/> (only when
    ///         <c>start &lt;= end</c> to avoid invalid ranges).</item>
    ///   <item><b>Esc</b> on the panel raises
    ///         <see cref="HostedEditor.RequestCancel"/>.</item>
    ///   <item>On <see cref="HostedEditor.Loading"/> both pickers receive
    ///         the current event start / end.</item>
    ///   <item>On <see cref="HostedEditor.Saving"/> the picker values are
    ///         written back to the event.</item>
    /// </list>
    /// </summary>
    public static class InlineEventDateRangeEditor
    {
        public const string Id = "daterange";
        public const string DisplayName = "Date Range";

        private const int SeparatorWidth = 16;

        public static HostedEditor Create()
        {
            var startPicker = new BeepDateTimePicker
            {
                Site = null,
                ShowAllBorders = true,
                TabStop = true
            };
            var endPicker = new BeepDateTimePicker
            {
                Site = null,
                ShowAllBorders = true,
                TabStop = true
            };
            var separator = new Label
            {
                Text = "→",
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Site = null
            };
            var panel = new Panel
            {
                Site = null,
                BackColor = Color.Transparent,
                TabStop = false
            };
            panel.Controls.Add(startPicker);
            panel.Controls.Add(separator);
            panel.Controls.Add(endPicker);

            var hosted = new HostedEditor(GetDescriptor(), panel);
            CalendarEvent? current = null;

            void LayoutChildren()
            {
                int w = panel.ClientSize.Width;
                int h = panel.ClientSize.Height;
                if (w <= 0 || h <= 0) return;
                int pickerW = (w - SeparatorWidth) / 2;
                startPicker.SetBounds(0, 0, pickerW, h);
                separator.SetBounds(pickerW, 0, SeparatorWidth, h);
                endPicker.SetBounds(pickerW + SeparatorWidth, 0, w - pickerW - SeparatorWidth, h);
            }
            panel.Resize += (s, e) => LayoutChildren();

            hosted.Loading += (s, evt) =>
            {
                current = evt;
                if (current != null)
                {
                    startPicker.SelectedDate = current.StartTime;
                    endPicker.SelectedDate = current.EndTime;
                }
            };
            hosted.Saving += (s, evt) =>
            {
                if (current != null)
                {
                    if (startPicker.SelectedDate.HasValue) current.StartTime = startPicker.SelectedDate.Value;
                    if (endPicker.SelectedDate.HasValue) current.EndTime = endPicker.SelectedDate.Value;
                }
            };

            void CommitIfValid()
            {
                if (current == null) return;
                if (startPicker.SelectedDate.HasValue && endPicker.SelectedDate.HasValue
                    && startPicker.SelectedDate.Value <= endPicker.SelectedDate.Value)
                {
                    hosted.RequestCommit();
                }
            }

            startPicker.Leave += (s, e) => CommitIfValid();
            endPicker.Leave += (s, e) => CommitIfValid();

            panel.KeyDown += (s, e) =>
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
