using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Date Picker")]
    public class BeepDatePicker : BeepControl
    {
        private TextBox _textBox;
        private Button _calendarButton;
        private DateTimePicker _datePicker;
        int buttonWidth = 15;
        private string _customDateFormat; // To store a custom date format if needed
                                          // Expose properties for customization
        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the selected date.")]
        public DateTime SelectedDate
        {
            get => _datePicker.Value;
            set
            {
                _datePicker.Value = value;
                _textBox.Text = value.ToString(_customDateFormat);
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the date format.")]
        public string DateFormat
        {
            get => _customDateFormat;
            set
            {
                _customDateFormat = value;
                _datePicker.CustomFormat = value;
                _textBox.Text = _datePicker.Value.ToString(value);
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the minimum allowable date.")]
        public DateTime MinDate
        {
            get => _datePicker.MinDate;
            set => _datePicker.MinDate = value;
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the maximum allowable date.")]
        public DateTime MaxDate
        {
            get => _datePicker.MaxDate;
            set => _datePicker.MaxDate = value;
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The date text displayed in the control.")]
        public override string Text
        {
            get => _textBox.Text;
            set
            {
                if (DateTime.TryParse(value, out DateTime result))
                {
                    _textBox.Text = result.ToString(_customDateFormat);
                    _datePicker.Value = result;
                }
                else
                {
                    _textBox.Text = DateTime.Now.ToString(_customDateFormat);
                }
            }
        }
        public BeepDatePicker()
        {
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 80;
                Height = 30;
            }

            _customDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern; // Default to system format
            InitializeComponents();
            AdjustSizeToTextBox(); // Ensure initial size matches TextBox
        }
        protected override void InitLayout()
        {
            base.InitLayout();
          
        }
       
        private void InitializeComponents()
        {
            // Initialize TextBox for manual input
            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                Text = DateTime.Now.ToString(_customDateFormat)
            };
            _textBox.TextChanged += TextBox_TextChanged;
            _textBox.Validating += TextBox_Validating;

            // Initialize Calendar Button
            _calendarButton = new Button
            {
                Dock = DockStyle.Right,
                Text = "📅", // Unicode calendar icon
                Width = Height, // Square button
                FlatStyle = FlatStyle.Flat
            };
            _calendarButton.Click += CalendarButton_Click;

            // Initialize DatePicker for selecting dates
            _datePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = _customDateFormat,
                Visible = false // Initially hidden
            };
            _datePicker.ValueChanged += DatePicker_ValueChanged;
            Controls.Add(_datePicker);

            // Add TextBox and Button
            Controls.Add(_textBox);
            Controls.Add(_calendarButton);
            AdjustLayout();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            // Update the DatePicker value when the TextBox text changes
            if (DateTime.TryParse(_textBox.Text, out DateTime result))
            {
                _datePicker.Value = result;
            }
        }

        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            // Validate the date input
            if (!DateTime.TryParse(_textBox.Text, out _))
            {
                e.Cancel = true; // Cancel focus change
                MessageBox.Show("Invalid date format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBox.Text = _datePicker.Value.ToString(_customDateFormat); // Reset to last valid value
            }
        }

        private void CalendarButton_Click(object sender, EventArgs e)
        {
            // Show the DatePicker dropdown
            _datePicker.Visible = !_datePicker.Visible;
            if (_datePicker.Visible)
            {
                _datePicker.BringToFront();
                _datePicker.Focus();
            }
        }

        private void DatePicker_ValueChanged(object sender, EventArgs e)
        {
            // Update TextBox when DatePicker value changes
            _textBox.Text = _datePicker.Value.ToString(_customDateFormat);
        }

    

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_textBox != null && _calendarButton != null)
            {
                _textBox.BackColor = _currentTheme.TextBoxBackColor;
                _textBox.ForeColor = _currentTheme.TextBoxForeColor;

                _calendarButton.BackColor = _currentTheme.TextBoxBackColor;
                _calendarButton.ForeColor = _currentTheme.TextBoxForeColor;

                _datePicker.CalendarForeColor = _currentTheme.PrimaryTextColor;
                _datePicker.CalendarMonthBackground = _currentTheme.TextBoxBackColor;
                _datePicker.CalendarTitleBackColor = _currentTheme.BackgroundColor;
                _datePicker.CalendarTitleForeColor = _currentTheme.PrimaryTextColor;
            }
           
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
   
            if (_textBox != null && _calendarButton!=null) {

                // Ensure the height is fixed
                Height = _textBox.PreferredHeight;
                AdjustLayout();
            }
           
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // Override height to match TextBox height
            base.SetBoundsCore(x, y, width, _textBox.PreferredHeight, specified);
        }

        private void AdjustLayout()
        {
            if (DrawingRect == Rectangle.Empty)
                UpdateDrawingRect();

            int padding = BorderThickness + 3; // Adjust padding for borders
           // int buttonWidth = Height - padding * 2; // Square button
            int textBoxWidth = DrawingRect.Width - buttonWidth - padding * 3; // TextBox width
            int centerY = DrawingRect.Top + (DrawingRect.Height - _textBox.Height) / 2;

            // Set TextBox bounds
            _textBox.Location = new Point(DrawingRect.Left + padding, centerY);
            _textBox.Width = textBoxWidth;
            _textBox.Height = _textBox.PreferredHeight;

            // Set Calendar Button bounds
            _calendarButton.Location = new Point(_textBox.Right + padding, centerY);
            _calendarButton.Width = buttonWidth;
            _calendarButton.Height = _textBox.PreferredHeight;

            // Hide DatePicker off the screen
            _datePicker.Location = new Point(-Width, -Height);
        }
        private void AdjustSizeToTextBox()
        {
            if(_textBox != null)
            {
                Height = _textBox.PreferredHeight; // Set the control's height based on the TextBox height
            }
          
            OnResize(EventArgs.Empty); // Trigger layout adjustment
        }
    }
}
