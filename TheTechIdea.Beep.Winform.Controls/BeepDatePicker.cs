using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Date Picker")]
    [Description("A control that allows users to enter a date with format masking and a dropdown calendar, supporting multiple cultures.")]
    public class BeepDatePicker : BeepControl
    {
        #region Fields
        private TextBox _textBox;
        private BeepButton _dropdownButton;
        private string _customDateFormat;
        private DateFormatStyle _dateFormatStyle = DateFormatStyle.ShortDateTime; // Updated to include time
        private CultureInfo _culture = CultureInfo.CurrentCulture;
        private int _padding = 2;
        private DateTime _selectedDateTime = DateTime.Now; // Updated to include time
        private BeepPopupForm _calendarPopup;
        private BeepCalendarView _calendarView;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets or gets the selected date and time as a DateTime object.")]
        public DateTime SelectedDateTime
        {
            get => _selectedDateTime;
            set
            {
                _selectedDateTime = value;
                UpdateTextBoxFromValue();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the selected date and time as a string.")]
        public string SelectedDate
        {
            get => _textBox?.Text ?? string.Empty;
            set
            {
                if (_textBox == null) return;
                if (string.IsNullOrWhiteSpace(value))
                {
                    _textBox.Text = string.Empty;
                    _selectedDateTime = DateTime.MinValue;
                }
                else if (DateTime.TryParse(value, _culture, DateTimeStyles.None, out DateTime result))
                {
                    _selectedDateTime = result;
                    _textBox.Text = FormatDate(result);
                }
                else
                {
                    _textBox.Text = string.Empty;
                    _selectedDateTime = DateTime.MinValue;
                }
                ApplyMask();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the predefined date format style.")]
        [DefaultValue(DateFormatStyle.ShortDateTime)]
        public DateFormatStyle DateFormatStyle
        {
            get => _dateFormatStyle;
            set
            {
                _dateFormatStyle = value;
                ApplyMask();
                UpdateTextBoxFromValue();
                if (_textBox != null) _textBox.PlaceholderText = GetPlaceholderText();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets a custom date format string, used when DateFormatStyle is Custom.")]
        [DefaultValue("d")]
        public string DateFormat
        {
            get => _customDateFormat;
            set
            {
                _customDateFormat = string.IsNullOrEmpty(value) ? _culture.DateTimeFormat.ShortDatePattern : value;
                if (_dateFormatStyle == DateFormatStyle.Custom)
                {
                    ApplyMask();
                    UpdateTextBoxFromValue();
                    if (_textBox != null) _textBox.PlaceholderText = GetPlaceholderText();
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the culture for date formatting and parsing. Defaults to current culture.")]
        public CultureInfo Culture
        {
            get => _culture;
            set
            {
                if (_textBox == null) return;
                _culture = value ?? CultureInfo.CurrentCulture;
                ApplyMask();
                UpdateTextBoxFromValue();
                _textBox.PlaceholderText = GetPlaceholderText();
                Invalidate();
            }
        }
        #endregion

        #region Constructor
        public BeepDatePicker()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            BorderRadius = 3;
        }

        protected override Size DefaultSize => new Size(200, 36); // Adjusted width to accommodate time

        protected override void InitLayout()
        {
            base.InitLayout();

            _customDateFormat = $"{_culture.DateTimeFormat.ShortDatePattern} HH:mm";
            BoundProperty = "SelectedDate";

            InitializeComponents();
            ApplyTheme();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Text = FormatDate(_selectedDateTime),
                PlaceholderText = GetPlaceholderText()
            };
            _textBox.TextChanged += TextBox_TextChanged;
            _textBox.KeyPress += TextBox_KeyPress;
            _textBox.Validating += TextBox_Validating;

            _dropdownButton = new BeepButton
            {
                Text = "▼",
                HideText = true,
                ShowAllBorders = false,
                IsShadowAffectedByTheme = false,
                IsChild = true,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.Overlay,
                TextAlign = ContentAlignment.MiddleCenter,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.dropdown-select.svg"
            };
            _dropdownButton.Click += DropdownButton_Click;

            Controls.Add(_textBox);
            Controls.Add(_dropdownButton);

            ApplyMask();
            AdjustLayout();
        }
        #endregion

        #region Event Handlers
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (DateTime.TryParse(_textBox.Text, _culture, DateTimeStyles.None, out DateTime result))
            {
                _selectedDateTime = result;
            }
            Invalidate();
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            string mask = GetCurrentMask();
            string currentText = _textBox.Text;
            int cursorPos = _textBox.SelectionStart;

            if (!char.IsControl(e.KeyChar) && cursorPos < mask.Length)
            {
                char maskChar = mask[cursorPos];
                if (!IsValidInput(e.KeyChar, maskChar))
                {
                    e.Handled = true;
                }
            }
        }

        private void TextBox_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_textBox.Text))
            {
                _selectedDateTime = DateTime.MinValue;
                return;
            }

            if (!DateTime.TryParse(_textBox.Text, _culture, DateTimeStyles.None, out DateTime result))
            {
                e.Cancel = true;
                MessageBox.Show($"Invalid date format. Expected: {GetCurrentFormat()}", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBox.Text = FormatDate(_selectedDateTime);
            }
            else
            {
                _selectedDateTime = result;
            }
        }

        private void DropdownButton_Click(object sender, EventArgs e)
        {
            ShowCalendarPopup();
        }
        #endregion

        #region Layout and Rendering
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLayout();
            Invalidate();
        }

        private void AdjustLayout()
        {
            if (_textBox == null || _dropdownButton == null) return;
            UpdateDrawingRect();
            GetHeight();

            int buttonWidth = DrawingRect.Height - (_padding * 2);
            _textBox.Location = new Point(DrawingRect.Left + _padding, DrawingRect.Top + _padding);
            _textBox.Size = new Size(DrawingRect.Width - buttonWidth - (_padding * 3), DrawingRect.Height - (_padding * 2));

            _dropdownButton.Location = new Point(_textBox.Right + _padding, DrawingRect.Top + _padding);
            _dropdownButton.Size = new Size(buttonWidth, DrawingRect.Height - (_padding * 2));
            _dropdownButton.MaxImageSize = new Size(buttonWidth - 4, buttonWidth - 4);
        }

        private void GetHeight()
        {
            _padding = BorderThickness + 2;
            if (_textBox == null) return;
            Height = _textBox.PreferredHeight + (_padding * 2);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            try
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                Region originalClip = graphics.Clip;
                graphics.SetClip(rectangle);

                using (SolidBrush backgroundBrush = new SolidBrush(_currentTheme.TextBoxBackColor))
                {
                    graphics.FillRectangle(backgroundBrush, rectangle);
                }

                if (BorderThickness > 0)
                {
                    using (Pen borderPen = new Pen(_currentTheme.BorderColor, BorderThickness))
                    {
                        Rectangle borderRect = rectangle;
                        borderRect.Inflate(-BorderThickness / 2, -BorderThickness / 2);
                        graphics.DrawRectangle(borderPen, borderRect);
                    }
                }

                graphics.Clip = originalClip;
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Error in BeepDatePicker.Draw: {ex.Message}");
            }
        }
        #endregion

        #region Popup Calendar
        private void ShowCalendarPopup()
        {
            if (_calendarPopup != null && _calendarPopup.Visible)
            {
                _calendarPopup.CloseCascade();
                return;
            }

            // Create popup form
            _calendarPopup = new BeepPopupForm
            {
                BorderThickness = 1,
                BorderRadius = this.BorderRadius,
                Size = new Size(300, 300), // Adjust size to fit BeepCalendarView
                Theme = Theme // Inherit theme from BeepControl
                 
            };

            // Create custom calendar control
            _calendarView = new BeepCalendarView
            {
                Dock = DockStyle.Fill,
                Theme = Theme
            };

            if (_selectedDateTime != DateTime.MinValue)
            {
                _calendarView.SelectedDateTime = _selectedDateTime;
            }

            _calendarView.DateTimeSelected += CalendarView_DateTimeSelected;
            _calendarPopup.Controls.Add(_calendarView);

            // Show popup below the control
            _calendarPopup.ShowPopup(this, BeepPopupFormPosition.Bottom);
        }

        private void CalendarView_DateTimeSelected(object sender, DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                _selectedDateTime = dateTime.Value;
                _textBox.Text = FormatDate(_selectedDateTime);
            }
            _calendarPopup?.CloseCascade();
        }
        #endregion

        #region Format and Masking
        public string GetCurrentFormat()
        {
            return _dateFormatStyle switch
            {
                DateFormatStyle.ShortDate => _culture.DateTimeFormat.ShortDatePattern,
                DateFormatStyle.LongDate => _culture.DateTimeFormat.LongDatePattern,
                DateFormatStyle.YearMonth => "MMMM yyyy",
                DateFormatStyle.Custom => _customDateFormat,
                DateFormatStyle.FullDateTime => $"{_culture.DateTimeFormat.LongDatePattern} {_culture.DateTimeFormat.LongTimePattern}",
                DateFormatStyle.ShortDateTime => $"{_culture.DateTimeFormat.ShortDatePattern} HH:mm",
                DateFormatStyle.DayMonthYear => "dd MMMM yyyy",
                DateFormatStyle.ISODate => "yyyy-MM-dd",
                DateFormatStyle.ISODateTime => "yyyy-MM-dd HH:mm:ss",
                DateFormatStyle.TimeOnly => "HH:mm:ss",
                DateFormatStyle.ShortTime => "HH:mm",
                DateFormatStyle.MonthDay => "MMMM dd",
                DateFormatStyle.DayOfWeek => "dddd",
                DateFormatStyle.RFC1123 => "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                DateFormatStyle.UniversalSortable => "yyyy-MM-dd HH:mm:ss'Z'",
                _ => $"{_culture.DateTimeFormat.ShortDatePattern} HH:mm"
            };
        }

        private string GetCurrentMask()
        {
            return _dateFormatStyle switch
            {
                DateFormatStyle.ShortDate => ConvertFormatToMask(_culture.DateTimeFormat.ShortDatePattern),
                DateFormatStyle.LongDate => _culture.DateTimeFormat.LongDatePattern,
                DateFormatStyle.YearMonth => "MMMM yyyy",
                DateFormatStyle.Custom => ConvertFormatToMask(_customDateFormat),
                DateFormatStyle.FullDateTime => ConvertFormatToMask($"{_culture.DateTimeFormat.LongDatePattern} HH:mm:ss"),
                DateFormatStyle.ShortDateTime => ConvertFormatToMask($"{_culture.DateTimeFormat.ShortDatePattern} HH:mm"),
                DateFormatStyle.DayMonthYear => "00 MMMM yyyy",
                DateFormatStyle.ISODate => "0000-00-00",
                DateFormatStyle.ISODateTime => "0000-00-00 00:00:00",
                DateFormatStyle.TimeOnly => "00:00:00",
                DateFormatStyle.ShortTime => "00:00",
                DateFormatStyle.MonthDay => "MMMM 00",
                DateFormatStyle.DayOfWeek => "dddd",
                DateFormatStyle.RFC1123 => "ddd, 00 MMM yyyy 00:00:00 GMT",
                DateFormatStyle.UniversalSortable => "0000-00-00 00:00:00Z",
                _ => ConvertFormatToMask($"{_culture.DateTimeFormat.ShortDatePattern} HH:mm")
            };
        }

        private string ConvertFormatToMask(string format)
        {
            return Regex.Replace(format,
                @"[dMyHhmsf]+",
                match => new string(match.Value[0] switch
                {
                    'd' => '0',
                    'M' => '0',
                    'y' => '0',
                    'H' => '0',
                    'h' => '0',
                    'm' => '0',
                    's' => '0',
                    'f' => '0',
                    _ => match.Value[0]
                }, match.Length));
        }

        private string GetPlaceholderText()
        {
            string format = GetCurrentFormat();
            return Regex.Replace(format, @"[dMyHhmsf]+", match => match.Value[0] switch
            {
                'd' => "DD",
                'M' => "MM",
                'y' => "YYYY",
                'H' or 'h' => "HH",
                'm' => "MM",
                's' => "SS",
                'f' => "FF",
                _ => match.Value
            });
        }

        private bool IsValidInput(char input, char maskChar)
        {
            return maskChar switch
            {
                '0' => char.IsDigit(input),
                'd' or 'M' or 'y' or 'H' or 'h' or 'm' or 's' => char.IsLetterOrDigit(input),
                _ => true // Allow separators like /, -, :, space
            };
        }

        private void ApplyMask()
        {
            if (_textBox == null) return;
            string mask = GetCurrentMask();
            if (!Regex.IsMatch(mask, @"^[dMyHmsf]+$")) // If mask contains digits
            {
                _textBox.MaxLength = mask.Length;
            }
            else
            {
                _textBox.MaxLength = 50; // Arbitrary max for free-form text
            }
        }

        private void UpdateTextBoxFromValue()
        {
            if (_textBox == null) return;
            if (_selectedDateTime != DateTime.MinValue)
            {
                _textBox.Text = FormatDate(_selectedDateTime);
            }
            else
            {
                _textBox.Text = string.Empty;
            }
        }

        private string FormatDate(DateTime date)
        {
            return date.ToString(GetCurrentFormat(), _culture);
        }
        #endregion

        #region Theme and Value Management
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_textBox != null)
            {
                _textBox.BackColor = _currentTheme.TextBoxBackColor;
                _textBox.ForeColor = _currentTheme.TextBoxForeColor;
            }
            if (_dropdownButton != null)
            {
                _dropdownButton.Theme = Theme;
            }
            if (_calendarView != null)
            {
                _calendarView.Theme = Theme;
            }
            Invalidate();
        }

        public override void SetValue(object value)
        {
            if (value is DateTime dt)
            {
                SelectedDateTime = dt;
            }
            else
            {
                SelectedDate = value?.ToString();
            }
        }

        public override object GetValue()
        {
            return _selectedDateTime == DateTime.MinValue ? null : _selectedDateTime;
        }

        public void Reset()
        {
            SelectedDateTime = DateTime.Now;
        }
        #endregion

        #region Disposal
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textBox?.Dispose();
                _dropdownButton?.Dispose();
                _calendarPopup?.Dispose();
                _calendarView?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}