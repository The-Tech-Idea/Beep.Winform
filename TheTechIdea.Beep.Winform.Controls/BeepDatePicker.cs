using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

using TheTechIdea.Beep.Winform.Controls.Models;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Date Picker")]
    [Description("A control that allows users to enter a date with format masking, supporting multiple cultures.")]
    public class BeepDatePicker : BeepControl
    {
        #region Properties
        private TextBox _textBox;
        private string _customDateFormat;
        private DateFormatStyle _dateFormatStyle = DateFormatStyle.ShortDate;
        private CultureInfo _culture = CultureInfo.CurrentCulture;
        private int _padding = 2;

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
                else if (DateTime.TryParse(value, _culture, DateTimeStyles.None, out DateTime result))
                {
                    _textBox.Text = FormatDate(result);
                }
                else
                {
                    _textBox.Text = string.Empty;
                }
                ApplyMask();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Date Settings")]
        [Description("Sets the predefined date format style.")]
        [DefaultValue(DateFormatStyle.ShortDate)]
        public DateFormatStyle DateFormatStyle
        {
            get => _dateFormatStyle;
            set
            {
                _dateFormatStyle = value;
                ApplyMask();
                UpdateTextBoxFromValue();
                _textBox.PlaceholderText = GetPlaceholderText();
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
                    _textBox.PlaceholderText = GetPlaceholderText();
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

            Size = new Size(120, 25);
            _customDateFormat = _culture.DateTimeFormat.ShortDatePattern;
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
                Text = DateTime.Now.ToString(GetCurrentFormat()),
                PlaceholderText = GetPlaceholderText()
            };
            _textBox.TextChanged += TextBox_TextChanged;
            _textBox.KeyPress += TextBox_KeyPress;
            _textBox.Validating += TextBox_Validating;
            
            Controls.Add(_textBox);

            ApplyMask();
            AdjustLayout();
        }
        #endregion

        #region Event Handlers
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
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
            if (!string.IsNullOrWhiteSpace(_textBox.Text) && !DateTime.TryParse(_textBox.Text, _culture, DateTimeStyles.None, out _))
            {
                e.Cancel = true;
                MessageBox.Show($"Invalid date format. Expected: {GetCurrentFormat()}", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _textBox.Text = DateTime.Today.ToString(GetCurrentFormat());
            }
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
            UpdateDrawingRect();
            GetHeight();

            _textBox.Location = new Point(DrawingRect.Left + _padding, DrawingRect.Top + _padding);
            _textBox.Size = new Size(DrawingRect.Width - (_padding * 2), DrawingRect.Height - (_padding * 2));
        }

        private void GetHeight()
        {
            _padding = BorderThickness + 2;
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

                Rectangle textRect = new Rectangle(
                    rectangle.X + _padding,
                    rectangle.Y + _padding,
                    rectangle.Width - (_padding * 2),
                    rectangle.Height - (_padding * 2));

                string textToDraw = _textBox.Text;
                if (!string.IsNullOrEmpty(textToDraw))
                {
                    TextRenderer.DrawText(
                        graphics,
                        textToDraw,
                        _textBox.Font,
                        textRect,
                        _currentTheme.TextBoxForeColor,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }

                graphics.Clip = originalClip;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BeepDatePicker.Draw: {ex.Message}");
            }
        }
        #endregion

        #region Format and Masking
        private string GetCurrentFormat()
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
                _ => _culture.DateTimeFormat.ShortDatePattern
            };
        }

        private string GetCurrentMask()
        {
            return _dateFormatStyle switch
            {
                DateFormatStyle.ShortDate => ConvertFormatToMask(_culture.DateTimeFormat.ShortDatePattern),
                DateFormatStyle.LongDate => _culture.DateTimeFormat.LongDatePattern, // Free-form
                DateFormatStyle.YearMonth => "MMMM yyyy",                            // Free-form
                DateFormatStyle.Custom => ConvertFormatToMask(_customDateFormat),
                DateFormatStyle.FullDateTime => ConvertFormatToMask($"{_culture.DateTimeFormat.LongDatePattern} HH:mm:ss"),
                DateFormatStyle.ShortDateTime => ConvertFormatToMask($"{_culture.DateTimeFormat.ShortDatePattern} HH:mm"),
                DateFormatStyle.DayMonthYear => "00 MMMM yyyy",
                DateFormatStyle.ISODate => "0000-00-00",
                DateFormatStyle.ISODateTime => "0000-00-00 00:00:00",
                DateFormatStyle.TimeOnly => "00:00:00",
                DateFormatStyle.ShortTime => "00:00",
                DateFormatStyle.MonthDay => "MMMM 00",
                DateFormatStyle.DayOfWeek => "dddd",                                 // Free-form
                DateFormatStyle.RFC1123 => "ddd, 00 MMM yyyy 00:00:00 GMT",
                DateFormatStyle.UniversalSortable => "0000-00-00 00:00:00Z",
                _ => ConvertFormatToMask(_culture.DateTimeFormat.ShortDatePattern)
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
            if (!string.IsNullOrWhiteSpace(_textBox.Text) && DateTime.TryParse(_textBox.Text, _culture, DateTimeStyles.None, out DateTime result))
            {
                _textBox.Text = FormatDate(result);
            }
            else
            {
                _textBox.Text = DateTime.Today.ToString(GetCurrentFormat());
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
            Invalidate();
        }

        public override void SetValue(object value)
        {
            SelectedDate = value?.ToString();
        }

        public override object GetValue()
        {
            return SelectedDate;
        }

        public void Reset()
        {
            SelectedDate = null;
        }
        #endregion

        #region Disposal
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textBox?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}