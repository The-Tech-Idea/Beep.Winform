using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    public class BeepDatePicker : BeepControl
    {
        private DateTimePicker _innerDatePicker;
        private string _customDateFormat = "yyyy-MM-dd"; // Default date format

        public BeepDatePicker()
        {
            InitializeDatePicker();
        }

        private void InitializeDatePicker()
        {
            _innerDatePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = _customDateFormat,
                Dock = DockStyle.Fill
            };

            _innerDatePicker.ValueChanged += (s, e) => Text = _innerDatePicker.Value.ToString(_customDateFormat);
            Controls.Add(_innerDatePicker);
        }

        // Expose properties for the DateTimePicker
        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the selected date.")]
        public DateTime SelectedDate
        {
            get => _innerDatePicker.Value;
            set => _innerDatePicker.Value = value;
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the date format.")]
        public string DateFormat
        {
            get => _innerDatePicker.CustomFormat;
            set
            {
                _customDateFormat = value;
                _innerDatePicker.CustomFormat = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the minimum allowable date.")]
        public DateTime MinDate
        {
            get => _innerDatePicker.MinDate;
            set => _innerDatePicker.MinDate = value;
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the maximum allowable date.")]
        public DateTime MaxDate
        {
            get => _innerDatePicker.MaxDate;
            set => _innerDatePicker.MaxDate = value;
        }

        // Override Text property to sync with the selected date in the specified format
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The date text displayed in the control.")]
        public override string Text
        {
            get => _innerDatePicker.Value.ToString(_customDateFormat);
            set
            {
                if (DateTime.TryParse(value, out DateTime result))
                {
                    _innerDatePicker.Value = result;
                }
                else
                {
                    _innerDatePicker.Value = DateTime.Now; // Default to current date if invalid
                }
            }
        }

        // Apply theme styling to the DateTimePicker control
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            _innerDatePicker.CalendarForeColor = _currentTheme.PrimaryTextColor;
            _innerDatePicker.CalendarMonthBackground = _currentTheme.TextBoxBackColor;
            _innerDatePicker.CalendarTitleBackColor = _currentTheme.BackgroundColor;
            _innerDatePicker.CalendarTitleForeColor = _currentTheme.PrimaryTextColor;
            _innerDatePicker.BackColor = _currentTheme.TextBoxBackColor;
            _innerDatePicker.ForeColor = _currentTheme.PrimaryTextColor;
        }

        // Manage layout updates and position adjustments for inner DatePicker
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionDatePicker();
        }

        private void PositionDatePicker()
        {
            int padding = BorderThickness + 2; // Adjust padding to account for borders
            _innerDatePicker.Location = new Point(DrawingRect.Left + padding, DrawingRect.Top + padding);
            _innerDatePicker.Width = DrawingRect.Width - padding * 2;
            _innerDatePicker.Height = DrawingRect.Height - padding * 2;
        }
    }
}
