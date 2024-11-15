using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDatePicker : BeepControl
    {
        private DateTimePicker dateTimePickerControl;

        public BeepDatePicker()
        {
            InitializeDateTimePicker();
        }

        private void InitializeDateTimePicker()
        {
            dateTimePickerControl = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd" // Default format, can be changed
            };

            dateTimePickerControl.ValueChanged += (s, e) => Text = dateTimePickerControl.Value.ToString("yyyy-MM-dd");
            Controls.Add(dateTimePickerControl);
        }

        // Properties to expose DateTimePicker settings
        [Browsable(true)]
        [Category("Date Settings")]
        public DateTime SelectedDate
        {
            get => dateTimePickerControl.Value;
            set => dateTimePickerControl.Value = value;
        }

        [Browsable(true)]
        [Category("Date Settings")]
        public string DateFormat
        {
            get => dateTimePickerControl.CustomFormat;
            set => dateTimePickerControl.CustomFormat = value;
        }

        [Browsable(true)]
        [Category("Date Settings")]
        public DateTime MinDate
        {
            get => dateTimePickerControl.MinDate;
            set => dateTimePickerControl.MinDate = value;
        }

        [Browsable(true)]
        [Category("Date Settings")]
        public DateTime MaxDate
        {
            get => dateTimePickerControl.MaxDate;
            set => dateTimePickerControl.MaxDate = value;
        }

        // Override Text property to sync with the selected date
        [Browsable(true)]
        [Category("Appearance")]
        public override string Text
        {
            get => dateTimePickerControl.Value.ToString(DateFormat);
            set
            {
                if (DateTime.TryParse(value, out DateTime result))
                {
                    dateTimePickerControl.Value = result;
                }
            }
        }

        // Apply theme customization for DateTimePicker
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            dateTimePickerControl.CalendarForeColor = _currentTheme.PrimaryTextColor;
            dateTimePickerControl.CalendarMonthBackground = _currentTheme.TextBoxBackColor;
            dateTimePickerControl.CalendarTitleBackColor = _currentTheme.BackgroundColor;
            dateTimePickerControl.CalendarTitleForeColor = _currentTheme.PrimaryTextColor;
        }
    }
}
