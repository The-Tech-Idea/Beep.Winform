using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Date Picker")]
    public class BeepDatePicker : BeepControl
    {
        private TextBox _textBox;
        private Button _calendarButton;
        private MonthCalendar _monthCalendar;
        private Form _popupForm;
        private string _customDateFormat;
        private int buttonWidth = 25;
        private bool isPopupOpening = false; // Flag to track the popup state
        private System.Windows.Forms.Timer popupDelayTimer;       // Timer to add a small delay


        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the selected date as a string.")]
        public string SelectedDate
        {
            get => _textBox.Text;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _textBox.Text = string.Empty;
                }
                else if (DateTime.TryParse(value, out DateTime result))
                {
                    _textBox.Text = result.ToString(_customDateFormat);
                    _monthCalendar.SetDate(result);
                }
                else
                {
                    throw new FormatException("Invalid date format.");
                }
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
                if (_monthCalendar != null)
                {
                    //_monthCalendar.CustomFormat = value;
                    UpdateTextBoxFromCalendar();
                }
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the minimum allowable date.")]
        public DateTime MinDate
        {
            get => _monthCalendar?.MinDate ?? DateTimePicker.MinimumDateTime;
            set
            {
                if (_monthCalendar != null)
                    _monthCalendar.MinDate = value;
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the maximum allowable date.")]
        public DateTime MaxDate
        {
            get => _monthCalendar?.MaxDate ?? DateTimePicker.MaximumDateTime;
            set
            {
                if (_monthCalendar != null)
                    _monthCalendar.MaxDate = value;
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
            // Initialize the delay timer
            popupDelayTimer = new System.Windows.Forms.Timer
            {
                Interval = 200 // Adjust the delay as needed (in milliseconds)
            };
            popupDelayTimer.Tick += (s, e) =>
            {
                isPopupOpening = false; // Reset the flag after the delay
                popupDelayTimer.Stop();
            };

        }
        protected override void InitLayout()
        {
            base.InitLayout();
        
            AdjustSizeToTextBox(); // Ensure initial size matches TextBox
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
          //  _calendarButton.Click += CalendarButton_Click;

            // Initialize MonthCalendar within a popup form
            _popupForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.Manual,
                BackColor = Color.White,
                Size = new Size(200, 160)
            };
            // Initialize MonthCalendar for selecting dates
            _monthCalendar = new MonthCalendar
            {
                Visible = true,
                MaxSelectionCount = 1
            };
            _monthCalendar.DateSelected += MonthCalendar_DateSelected;
           // _monthCalendar.LostFocus += (s, e) => _monthCalendar.Visible = false;
            _popupForm.Controls.Add(_monthCalendar);
            _monthCalendar.Dock = DockStyle.Fill;
            _popupForm.Deactivate += (s, e) => _popupForm.Hide();
          //  _popupForm.Leave += (s, e) => _popupForm.Hide();
            // Add TextBox and Button
            Controls.Add(_textBox);
            Controls.Add(_calendarButton);
            AdjustLayout();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_textBox.Text) && DateTime.TryParse(_textBox.Text, out DateTime result))
            {
                _monthCalendar.SetDate(result);
            }
        }

        private void UpdateTextBoxFromCalendar()
        {
            _textBox.Text = _monthCalendar.SelectionStart.ToString(_customDateFormat);
        }
        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_textBox.Text) && !DateTime.TryParse(_textBox.Text, out _))
            {
                e.Cancel = true; // Prevent focus change
                MessageBox.Show("Invalid date format.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateTextBoxFromCalendar(); // Reset to last valid value
            }
        }
        private void MonthCalendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            // Update TextBox when a date is selected
            _textBox.Text = _monthCalendar.SelectionStart.ToString(_customDateFormat);
            _popupForm.Hide();
        }

        private bool _isToggling = false;

        private void CalendarButton_Click(object sender, EventArgs e)
        {
            if (_isToggling) return;

            _isToggling = true;

            if (_popupForm.Visible)
            {
                _popupForm.Hide();
            }
            else
            {
                PositionPopupForm();
                _popupForm.Show();
                _popupForm.BringToFront();
            }

            Task.Delay(200).ContinueWith(_ => _isToggling = false);
        }

        private void PositionPopupForm()
        {
            var screenLocation = PointToScreen(new Point(0, Height));
            _popupForm.Location = new Point(screenLocation.X, screenLocation.Y);
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

                _monthCalendar.TitleBackColor = _currentTheme.BackgroundColor;
                _monthCalendar.TitleForeColor = _currentTheme.PrimaryTextColor;
                _monthCalendar.TrailingForeColor = _currentTheme.TextBoxForeColor;
                _monthCalendar.BackColor = _currentTheme.TextBoxBackColor;
                _monthCalendar.ForeColor = _currentTheme.PrimaryTextColor;
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
            // Use the TextBox's preferred height for consistent alignment
            int adjustedHeight = _textBox?.PreferredHeight ?? base.Height;
            base.SetBoundsCore(x, y, width, adjustedHeight, specified);
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
           // _datePicker.Location = new Point(-Width, -Height);
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
