using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public sealed class CalendarEventEditor : ICalendarEventEditor
    {
        public CalendarEventEditorMode Mode { get; set; } = CalendarEventEditorMode.QuickEdit;

        public bool TryEdit(CalendarEventEditorRequest request, out CalendarEvent editedEvent)
        {
            editedEvent = null;

            if (request?.ProposedEvent == null)
            {
                return false;
            }

            using var dialog = new BeepCustomDialog();
            dialog.Title = request.Mode == CalendarEventEditorMode.DialogEdit ? "Edit Calendar Event" : "Quick Edit Calendar Event";
            dialog.ClientSize = request.Mode == CalendarEventEditorMode.DialogEdit ? new Size(720, 620) : new Size(560, 420);

            var content = new CalendarEventEditorContent();
            content.LoadFromRequest(request);
            dialog.SetCustomControl(content);

            var owner = Application.OpenForms.Cast<Form>().FirstOrDefault(form => form.Visible && !form.IsDisposed);
            var result = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return true;
            }

            editedEvent = content.BuildEditedEvent(request.ProposedEvent);
            return editedEvent != null;
        }
    }

    internal sealed class CalendarEventEditorContent : UserControl
    {
        private readonly Label _conflictLabel;
        private readonly TextBox _titleBox;
        private readonly DateTimePicker _startPicker;
        private readonly DateTimePicker _endPicker;
        private readonly CheckBox _allDayCheckBox;
        private readonly TextBox _locationBox;
        private readonly ComboBox _statusBox;
        private readonly TextBox _organizerBox;
        private readonly TextBox _descriptionBox;
        private readonly Panel _advancedPanel;
        private CalendarEventEditorMode _mode;

        public CalendarEventEditorContent()
        {
            BackColor = SystemColors.Window;
            Dock = DockStyle.Fill;
            Padding = new Padding(12);
            Font = SystemFonts.MessageBoxFont;

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                AutoSize = true,
                AutoScroll = true,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            _conflictLabel = new Label
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ForeColor = Color.Firebrick,
                Margin = new Padding(0, 0, 0, 8),
                Text = string.Empty
            };

            var corePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                AutoSize = true,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            corePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120f));
            corePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            _titleBox = new TextBox { Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 8) };
            _locationBox = new TextBox { Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 8) };
            _statusBox = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 0, 0, 8) };
            _organizerBox = new TextBox { Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 8) };
            _descriptionBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Height = 120,
                Margin = new Padding(0, 0, 0, 8)
            };

            _startPicker = CreateDateTimePicker();
            _endPicker = CreateDateTimePicker();
            _allDayCheckBox = new CheckBox
            {
                Text = "All day",
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 8)
            };
            _allDayCheckBox.CheckedChanged += (_, _) => UpdateDatePickerFormats();

            AddField(corePanel, "Title", _titleBox, 0);
            AddField(corePanel, "Start", _startPicker, 1);
            AddField(corePanel, "End", _endPicker, 2);
            AddField(corePanel, string.Empty, _allDayCheckBox, 3, false);
            AddField(corePanel, "Location", _locationBox, 4);
            AddField(corePanel, "Status", _statusBox, 5);

            _advancedPanel = new Panel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            var advancedLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                AutoSize = true,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            advancedLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120f));
            advancedLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            AddField(advancedLayout, "Organizer", _organizerBox, 0);
            AddField(advancedLayout, "Description", _descriptionBox, 1, true);
            _advancedPanel.Controls.Add(advancedLayout);

            root.Controls.Add(_conflictLabel, 0, 0);
            root.Controls.Add(corePanel, 0, 1);
            root.Controls.Add(_advancedPanel, 0, 2);

            Controls.Add(root);

            _statusBox.DataSource = Enum.GetValues(typeof(CalendarEventStatus));
            SetMode(CalendarEventEditorMode.QuickEdit);
        }

        public void LoadFromRequest(CalendarEventEditorRequest request)
        {
            _mode = request.Mode;
            SetMode(_mode);

            var source = request.ProposedEvent ?? new CalendarEvent();
            _titleBox.Text = source.Title ?? string.Empty;
            _locationBox.Text = source.Location ?? string.Empty;
            _organizerBox.Text = source.Organizer ?? string.Empty;
            _descriptionBox.Text = source.Description ?? string.Empty;
            _allDayCheckBox.Checked = source.IsAllDay;
            _startPicker.Value = NormalizePickerValue(source.StartTime);
            _endPicker.Value = NormalizePickerValue(source.EndTime);
            _statusBox.SelectedItem = source.Status;
            if (_statusBox.SelectedItem == null && _statusBox.Items.Count > 0)
            {
                _statusBox.SelectedIndex = 0;
            }

            UpdateDatePickerFormats();
            UpdateConflictSummary(request.Conflicts);
        }

        public CalendarEvent BuildEditedEvent(CalendarEvent source)
        {
            if (source == null)
            {
                return null;
            }

            var edited = CloneEvent(source);
            edited.Title = _titleBox.Text?.Trim() ?? string.Empty;
            edited.Location = _locationBox.Text?.Trim() ?? string.Empty;
            edited.IsAllDay = _allDayCheckBox.Checked;
            edited.Status = _statusBox.SelectedItem is CalendarEventStatus status ? status : edited.Status;

            if (_mode == CalendarEventEditorMode.DialogEdit)
            {
                edited.Organizer = _organizerBox.Text?.Trim() ?? string.Empty;
                edited.Description = _descriptionBox.Text ?? string.Empty;
            }

            edited.StartTime = _allDayCheckBox.Checked
                ? _startPicker.Value.Date
                : _startPicker.Value;
            edited.EndTime = _allDayCheckBox.Checked
                ? _endPicker.Value.Date.AddDays(1)
                : _endPicker.Value;

            if (edited.EndTime <= edited.StartTime)
            {
                edited.EndTime = edited.StartTime.AddHours(1);
            }

            return edited;
        }

        private void SetMode(CalendarEventEditorMode mode)
        {
            _mode = mode;
            _advancedPanel.Visible = mode == CalendarEventEditorMode.DialogEdit;
            UpdateDatePickerFormats();
        }

        private void UpdateConflictSummary(IReadOnlyList<CalendarEvent> conflicts)
        {
            if (conflicts == null || conflicts.Count == 0)
            {
                _conflictLabel.Text = string.Empty;
                _conflictLabel.Visible = false;
                return;
            }

            _conflictLabel.Visible = true;
            _conflictLabel.Text = $"{conflicts.Count} conflict{(conflicts.Count == 1 ? string.Empty : "s")} detected for the proposed time.";
        }

        private void UpdateDatePickerFormats()
        {
            string format = _allDayCheckBox.Checked ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm";
            _startPicker.CustomFormat = format;
            _endPicker.CustomFormat = format;
            _startPicker.ShowUpDown = !_allDayCheckBox.Checked;
            _endPicker.ShowUpDown = !_allDayCheckBox.Checked;
        }

        private static DateTimePicker CreateDateTimePicker()
        {
            return new DateTimePicker
            {
                Dock = DockStyle.Top,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm",
                ShowUpDown = true,
                Margin = new Padding(0, 0, 0, 8)
            };
        }

        private static void AddField(TableLayoutPanel layout, string labelText, Control control, int rowIndex, bool stretch = false)
        {
            if (layout.RowCount <= rowIndex)
            {
                layout.RowCount = rowIndex + 1;
            }

            while (layout.RowStyles.Count <= rowIndex)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            var label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 8, 8),
                Visible = !string.IsNullOrWhiteSpace(labelText)
            };

            control.Margin = new Padding(0, 0, 0, 8);
            if (stretch)
            {
                control.Dock = DockStyle.Fill;
                control.MinimumSize = new Size(0, 110);
            }

            layout.Controls.Add(label, 0, rowIndex);
            layout.Controls.Add(control, 1, rowIndex);
        }

        private static DateTime NormalizePickerValue(DateTime value)
        {
            if (value.Year < 1900)
            {
                return DateTime.Now;
            }

            return value;
        }

        private static CalendarEvent CloneEvent(CalendarEvent source)
        {
            return new CalendarEvent
            {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                StartTime = source.StartTime,
                EndTime = source.EndTime,
                CategoryId = source.CategoryId,
                Location = source.Location,
                IsAllDay = source.IsAllDay,
                Organizer = source.Organizer,
                Tags = source.Tags != null ? new List<string>(source.Tags) : new List<string>(),
                Status = source.Status,
                TimeZoneId = source.TimeZoneId,
                SeriesId = source.SeriesId,
                ParentEventId = source.ParentEventId,
                ResourceId = source.ResourceId,
                ResourceIds = source.ResourceIds != null ? new List<string>(source.ResourceIds) : new List<string>(),
                RecurrenceRule = source.RecurrenceRule,
                RecurrenceFrequency = source.RecurrenceFrequency,
                RecurrenceInterval = source.RecurrenceInterval,
                RecurrenceCount = source.RecurrenceCount,
                RecurrenceUntilUtc = source.RecurrenceUntilUtc,
                RecurrenceExceptions = source.RecurrenceExceptions != null ? new List<DateTime>(source.RecurrenceExceptions) : new List<DateTime>(),
                ReminderMinutesBeforeStart = source.ReminderMinutesBeforeStart,
                Metadata = source.Metadata != null ? new Dictionary<string, string>(source.Metadata, StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            };
        }
    }
}
