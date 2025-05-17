
using TheTechIdea.Beep.Winform.Controls.Models;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Calendar View")]
    [Description("A custom calendar view control for selecting date and time.")]
    public class BeepCalendarView : BeepControl
    {
        #region Fields
        private DateTime _currentMonth;
        private DateTime? _selectedDateTime;
        private BindingList<SimpleItem> _timeSlots;
        private string _selectedTime;
        private Rectangle _headerRect;
        private Rectangle _daysHeaderRect;
        private Rectangle _datesGridRect;
        private Rectangle _timeListRect;
        private Rectangle _footerRect;
        private Rectangle _buttonsRect;
        private BeepImage _leftArrow;
        private BeepImage _rightArrow;
        private BeepScrollList _timeList;
        private BeepButton _okButton;
        private BeepButton _cancelButton;
        private const int CellSize = 30;
        private const int TimeSlotHeight = 30;
        private const int HeaderHeight = 40;
        private const int FooterHeight = 30;
        private const int ButtonHeight = 30;
        #endregion

        #region Properties
        #region Appearance Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the month/year header")]
        public Font HeaderFont { get; set; } = new Font("Arial", 10, FontStyle.Bold);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the days of week header (M T W T F S S)")]
        public Font DaysHeaderFont { get; set; } = new Font("Arial", 8);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the date numbers")]
        public Font DateFont { get; set; } = new Font("Arial", 9);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the selected date display in the footer")]
        public Font SelectedDateFont { get; set; } = new Font("Arial", 8);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the 'TODAY' button in the footer")]
        public Font FooterButtonFont { get; set; } = new Font("Arial", 8, FontStyle.Bold);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the time list items")]
        public Font TimeListFont { get; set; } = new Font("Arial", 8);
        #endregion
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The selected date and time. Null if no date is selected.")]
        public DateTime? SelectedDateTime
        {
            get => _selectedDateTime;
            set
            {
                _selectedDateTime = value;
                if (_selectedDateTime.HasValue)
                {
                    _currentMonth = _selectedDateTime.Value;
                    _selectedTime = _selectedDateTime.Value.ToString("h:mm tt");
                    if (_timeList != null)
                    {
                        _timeList.SelectedItem = _timeSlots.FirstOrDefault(item => item.Text == _selectedTime);
                    }
                }
                Invalidate();
            }
        }
        #endregion

        #region Events
        public event EventHandler<DateTime?> DateTimeSelected;
        public event EventHandler Cancelled;

        protected virtual void OnDateTimeSelected()
        {
            DateTimeSelected?.Invoke(this, SelectedDateTime);
        }

        protected virtual void OnCancelled()
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Constructor
        public BeepCalendarView()
        {
            InitializeCalendar();
        }

        private void InitializeCalendar()
        {
            DoubleBuffered = true;
            _currentMonth = DateTime.Today;
            _timeSlots = GenerateTimeSlots();
            _selectedTime = "9:00 PM"; // Default time

            // Initialize navigation arrows
            _leftArrow = new BeepImage
            {
                ImagePath = "angle-small-left.svg",
                ApplyThemeOnImage = true,
                Size = new Size(20, 20),
                Theme = Theme
            };
            _rightArrow = new BeepImage
            {
                ImagePath = "angle-small-right.svg",
                ApplyThemeOnImage = true,
                Size = new Size(20, 20),
                Theme = Theme
            };

            // Initialize time list
            _timeList = new BeepScrollList
            {
                ItemHeight = TimeSlotHeight,
                Orientation = ScrollOrientation.VerticalScroll,
                Theme = Theme,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsFrameless = true,
                IsRounded = false,
                IsRoundedAffectedByTheme = false
            };
            _timeList.ItemSelected += TimeList_ItemSelected;

            // Ensure time slots are populated
            if (_timeSlots != null && _timeSlots.Count > 0)
            {
                _timeList.ListItems = _timeSlots; // Directly assign _timeSlots
                _timeList.SelectedItem = _timeSlots.FirstOrDefault(item => item.Text == _selectedTime);
            }
            else
            {
                // Fallback in case _timeSlots is empty
                _timeList.ListItems = new BindingList<SimpleItem>();
            }

            // Initialize OK and Cancel buttons
            _okButton = new BeepButton
            {
                Text = "OK",
                Theme = Theme,
             
                IsChild = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _okButton.Click += OkButton_Click;

            _cancelButton = new BeepButton
            {
                Text = "Cancel",
                Theme = Theme,
              
                IsChild = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _cancelButton.Click += CancelButton_Click;

            Controls.Add(_timeList);
            Controls.Add(_okButton);
            Controls.Add(_cancelButton);

            // Set default size
            Size = new Size(300, 300 + ButtonHeight); // Increased height to accommodate buttons
            UpdateLayout();
        }
        #endregion

        #region Layout and Drawing
        private void UpdateLayout()
        {
            int padding = 2;
            int timeListWidth = 80;

            // Account for rounded corners and border thickness
            int cornerPadding = BorderRadius + BorderThickness;
            int innerWidth = DrawingRect.Width - 2 * cornerPadding;
            int innerHeight = DrawingRect.Height - 2 * cornerPadding;

            // Adjust regions to fit within the rounded shape
            _headerRect = new Rectangle(
                DrawingRect.Left + cornerPadding,
                DrawingRect.Top + padding,
                innerWidth,
                HeaderHeight
            );

            _daysHeaderRect = new Rectangle(
                DrawingRect.Left + cornerPadding,
                DrawingRect.Top + HeaderHeight + padding,
                innerWidth - timeListWidth,
                CellSize
            );

            _datesGridRect = new Rectangle(
                DrawingRect.Left + cornerPadding,
                DrawingRect.Top + HeaderHeight + CellSize + padding,
                innerWidth - timeListWidth,
                CellSize * 6
            );

            _timeListRect = new Rectangle(
               _datesGridRect.Right,
                DrawingRect.Top + HeaderHeight + padding,
                timeListWidth,
                innerHeight - HeaderHeight - FooterHeight - ButtonHeight - 2 * padding
            );

            _footerRect = new Rectangle(
                DrawingRect.Left + cornerPadding,
                DrawingRect.Top + innerHeight - FooterHeight - ButtonHeight,
                innerWidth,
                FooterHeight
            );

            _buttonsRect = new Rectangle(
                DrawingRect.Left + cornerPadding,
                DrawingRect.Top + innerHeight - ButtonHeight,
                innerWidth,
                ButtonHeight
            );

            _timeList.Location = new Point(_timeListRect.X, _timeListRect.Y);
            _timeList.Size = new Size(_timeListRect.Width, _timeListRect.Height);
            // Change this line:
            // _timeList.UpdateButtonItems();
            // To:
            _timeList.UpdateVirtualItems();
            // Position OK and Cancel buttons side by side
            int buttonWidth = (_buttonsRect.Width - 3 * padding) / 2;
            _okButton.Location = new Point(_buttonsRect.X, _buttonsRect.Y);
            _okButton.Size = new Size(buttonWidth, ButtonHeight);

            _cancelButton.Location = new Point(_buttonsRect.X + buttonWidth + padding, _buttonsRect.Y);
            _cancelButton.Size = new Size(buttonWidth, ButtonHeight);
            int y = DrawingRect.Top + cornerPadding + padding;

            _headerRect = new Rectangle(DrawingRect.Left + cornerPadding, y, innerWidth, HeaderHeight);
            y += HeaderHeight + padding;

            _daysHeaderRect = new Rectangle(DrawingRect.Left + cornerPadding, y, innerWidth - timeListWidth, CellSize);
            y += CellSize;

            _datesGridRect = new Rectangle(DrawingRect.Left + cornerPadding, y, innerWidth - timeListWidth, CellSize * 6);

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
         

            // Clip drawing to the rounded shape of the control
            //using (GraphicsPath clipPath = GetRoundedRectPath(DrawingRect, BorderRadius))
            //{
            //    Region originalClip = g.Clip;
            //    g.SetClip(clipPath, CombineMode.Intersect);

               

           //     g.Clip = originalClip;
          //  }
        }
        protected override void DrawContent(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            base.DrawContent(g);
            // Draw header (month and year)
            DrawHeader(g);

            // Draw days of the week
            DrawDaysHeader(g);

            // Draw dates grid
            DrawDatesGrid(g);

            // Draw footer (selected date/time)
            DrawFooter(g);
        }
        private void DrawHeader(Graphics g)
        {
            // Removed background drawing; only draw the text and arrows
            string monthYear = _currentMonth.ToString("MMMM yyyy");
            SizeF textSize = g.MeasureString(monthYear, Font);
            PointF textPos = new PointF(_headerRect.Left + (_headerRect.Width - textSize.Width) / 2, _headerRect.Top + (_headerRect.Height - textSize.Height) / 2);
            // Draw String with Brush color _currenttheme.TitleForColor
            Brush brush = new SolidBrush(_currentTheme.CalendarForeColor); // Use a solid color for the text
                g.DrawString(monthYear, Font, brush, textPos); // Use a solid color for the text

            // Draw navigation arrows
            _leftArrow.Location = new Point(_headerRect.Left + 10, _headerRect.Top + (_headerRect.Height - _leftArrow.Height) / 2);
            
            _leftArrow.DrawImage(g, new Rectangle(_leftArrow.Location, _leftArrow.Size));
            
            _rightArrow.Location = new Point(
    _headerRect.Right - _rightArrow.Width - 10,
    _headerRect.Top + (_headerRect.Height - _rightArrow.Height) / 2
);
            _rightArrow.DrawImage(g, new Rectangle(_rightArrow.Location, _rightArrow.Size));

        }

        private void DrawDaysHeader(Graphics g)
        {
            Brush brush = new SolidBrush(_currentTheme.CalendarForeColor);
            string[] days = { "M", "T", "W", "T", "F", "S", "S" };

            // Calculate cell width consistently with what's used in DrawDatesGrid
            const int minCellWidth = 36;
            int cellWidth = Math.Max(CellSize, minCellWidth);

            for (int i = 0; i < 7; i++)
            {
                Rectangle dayRect = new Rectangle(_daysHeaderRect.X + i * cellWidth, _daysHeaderRect.Y, cellWidth, CellSize);
                g.DrawString(days[i], DaysHeaderFont, brush, dayRect,
                            new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawDatesGrid(Graphics g)
        {
            DateTime firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            int dayOfWeek = ((int)firstDayOfMonth.DayOfWeek + 6) % 7; // Adjust for Monday start
            int daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);

            // Use the same cell width calculation as in DrawDaysHeader
            const int minCellWidth = 36; // Make sure cell width can handle "31" comfortably
            int cellWidth = Math.Max(CellSize, minCellWidth);
            int cellHeight = CellSize; // Keep the height the same

            int row = 0, col = dayOfWeek;
            for (int day = 1; day <= daysInMonth; day++)
            {
                // Use cellWidth to match the header's calculation
                Rectangle cellRect = new Rectangle(
                    _datesGridRect.X + col * cellWidth,
                    _datesGridRect.Y + row * cellHeight,
                    cellWidth,
                    cellHeight);

                DateTime currentDate = new DateTime(_currentMonth.Year, _currentMonth.Month, day);

                bool isSelected = _selectedDateTime.HasValue && _selectedDateTime.Value.Date == currentDate.Date;
                bool isToday = currentDate.Date == DateTime.Today;

                if (isSelected)
                {
                    using (GraphicsPath path = GetRoundedRectPath(cellRect, BorderRadius / 4))
                    using (SolidBrush brush = new SolidBrush(_currentTheme.CalendarTodayForeColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else if (isToday)
                {
                    using (GraphicsPath path = GetRoundedRectPath(cellRect, BorderRadius / 4))
                    using (Pen pen = new Pen(_currentTheme.ButtonBackColor, 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                // Format day number with consistent styling
                SolidBrush textBrush = new SolidBrush(_currentTheme.CalendarForeColor);

                // Always use StringFormat for alignment consistency
                // Use DateFont for the date numbers
                g.DrawString(
                    day.ToString("00"),
                    DateFont,
                    textBrush,
                    cellRect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }
                );

                col++;
                if (col > 6)
                {
                    col = 0;
                    row++;
                }
            }
        }

        private void DrawFooter(Graphics g)
        {
            SolidBrush forcolor = new SolidBrush(_currentTheme.CalendarForeColor);
            SolidBrush selforcolor = new SolidBrush(_currentTheme.CalendarTodayForeColor);

            // Use FooterButtonFont for the TODAY button
            string todayText = "TODAY";
            g.DrawString(todayText, FooterButtonFont, forcolor,
                        new Rectangle(_footerRect.X + 5, _footerRect.Y, 50, _footerRect.Height),
                        new StringFormat { LineAlignment = StringAlignment.Center });

            // Use SelectedDateFont for the selected date display
            string selectedText = _selectedDateTime.HasValue ?
                _selectedDateTime.Value.ToString("MMMM d, yyyy h:mm tt") :
                "Select a date";
            g.DrawString(selectedText, SelectedDateFont, selforcolor,
                        new Rectangle(_footerRect.X + 60, _footerRect.Y, _footerRect.Width - 65, _footerRect.Height),
                        new StringFormat { LineAlignment = StringAlignment.Center });
        }

        private BindingList<SimpleItem> GenerateTimeSlots()
        {
            BindingList<SimpleItem> slots = new BindingList<SimpleItem>();
            for (int hour = 0; hour < 24; hour++)
            {
                string time = $"{(hour % 12 == 0 ? 12 : hour % 12)}:00 {(hour < 12 ? "AM" : "PM")}";
                slots.Add(new SimpleItem { Text = time });
            }
            return slots;
        }
        #endregion

        #region Mouse Interaction
        private void OkButton_Click(object sender, EventArgs e)
        {
            OnDateTimeSelected();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancelled();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                // Check for navigation arrow clicks
                if (_leftArrow.Bounds.Contains(e.Location))
                {
                    _currentMonth = _currentMonth.AddMonths(-1);
                    Invalidate();
                }
                else if (_rightArrow.Bounds.Contains(e.Location))
                {
                    _currentMonth = _currentMonth.AddMonths(1);
                    Invalidate();
                }
                // Check for date selection
                else if (_datesGridRect.Contains(e.Location))
                {
                    // Use the same cell width calculation as in drawing methods
                    const int minCellWidth = 36;
                    int cellWidth = Math.Max(CellSize, minCellWidth);

                    // Calculate column and row using the consistent cellWidth
                    int col = (e.X - _datesGridRect.X) / cellWidth;
                    int row = (e.Y - _datesGridRect.Y) / CellSize;
                    int cellIndex = row * 7 + col;

                    DateTime firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
                    int dayOfWeek = ((int)firstDayOfMonth.DayOfWeek + 6) % 7; // Adjust for Monday start
                    int day = cellIndex - dayOfWeek + 1;

                    if (day > 0 && day <= DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month))
                    {
                        DateTime selectedDate = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
                        if (_selectedDateTime.HasValue)
                        {
                            selectedDate = selectedDate.Date.Add(_selectedDateTime.Value.TimeOfDay);
                        }
                        else
                        {
                            DateTime parsedTime = DateTime.Parse(_selectedTime);
                            selectedDate = selectedDate.Date.Add(parsedTime.TimeOfDay);
                        }
                        _selectedDateTime = selectedDate;
                        // Do not call OnDateTimeSelected here; wait for OK button
                        Invalidate();
                    }
                }
                // Check for "TODAY" click in footer
                else if (_footerRect.Contains(e.Location) && e.X < _footerRect.X + 50)
                {
                    DateTime today = DateTime.Today;
                    if (_selectedDateTime.HasValue)
                    {
                        today = today.Add(_selectedDateTime.Value.TimeOfDay);
                    }
                    else
                    {
                        DateTime parsedTime = DateTime.Parse(_selectedTime);
                        today = today.Add(parsedTime.TimeOfDay);
                    }
                    _selectedDateTime = today;
                    _currentMonth = today;
                    // Do not call OnDateTimeSelected here; wait for OK button
                    Invalidate();
                }
            }
        }

        private void TimeList_ItemSelected(object sender, SimpleItem selectedItem)
        {
            if (selectedItem != null)
            {
                _selectedTime = selectedItem.Text;
                if (_selectedDateTime.HasValue)
                {
                    DateTime parsedTime = DateTime.Parse(_selectedTime);
                    _selectedDateTime = _selectedDateTime.Value.Date.Add(parsedTime.TimeOfDay);
                    // Do not call OnDateTimeSelected here; wait for OK button
                }
                Invalidate();
            }
        }
        #endregion

        #region Overrides
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null)
                return;

            // Apply calendar-specific theme properties
            BackColor = _currentTheme.CalendarBackColor;
            ForeColor = _currentTheme.CalendarForeColor;
            BorderColor = _currentTheme.CalendarBorderColor;

            // Apply fonts if they exist in the theme
            if (UseThemeFont)
            {
                if (_currentTheme.CalendarTitleFont != null)
                    HeaderFont = _currentTheme.CalendarTitleFont;

                // Use appropriate theme fonts for other elements if they exist
                if (_currentTheme.CalendarSelectedFont != null)
                    SelectedDateFont = _currentTheme.CalendarSelectedFont;

                if (_currentTheme.CalendarUnSelectedFont != null)
                    DateFont = _currentTheme.CalendarUnSelectedFont;
                DaysHeaderFont = _currentTheme.CalendarUnSelectedFont;
                FooterButtonFont = _currentTheme.CalendarUnSelectedFont;
                TimeListFont = _currentTheme.CalendarUnSelectedFont;
            }

            // Apply the TimeListFont to the time list items
            if (_timeList != null)
            {
                _timeList.Theme = Theme;
                _timeList.BackColor = _currentTheme.CalendarBackColor;
                _timeList.ForeColor = _currentTheme.CalendarForeColor;
                // If BeepScrollList has a Font property:
                _timeList.TextFont = TimeListFont;
            }

            // Apply theme to buttons with appropriate font
            if (_okButton != null)
            {
                _okButton.Theme = Theme;
                _okButton.IsRounded = IsRounded;
                _okButton.BorderRadius = BorderRadius;
                _okButton.BorderThickness = BorderThickness;
                _okButton.IsRoundedAffectedByTheme = IsRoundedAffectedByTheme;
                _okButton.BorderColor = _currentTheme.CalendarBorderColor;
                _okButton.TextFont = FooterButtonFont;
            }

            if (_cancelButton != null)
            {
                _cancelButton.Theme = Theme;
                _cancelButton.IsRounded = IsRounded;
                _cancelButton.BorderRadius = BorderRadius;
                _cancelButton.BorderThickness = BorderThickness;
                _cancelButton.IsRoundedAffectedByTheme = IsRoundedAffectedByTheme;
                _cancelButton.BorderColor = _currentTheme.CalendarBorderColor;
                _cancelButton.TextFont = FooterButtonFont;
            }

            // Update layout to apply the new theme
            UpdateLayout();

            // Force redraw
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _leftArrow?.Dispose();
                _rightArrow?.Dispose();
                _timeList?.Dispose();
                _okButton?.Dispose();
                _cancelButton?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}