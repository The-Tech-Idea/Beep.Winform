using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.ComponentModel;

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
        private BeepImage _leftArrow;
        private BeepImage _rightArrow;
        private BeepScrollList _timeList;
        private const int CellSize = 30;
        private const int TimeSlotHeight = 30;
        private const int HeaderHeight = 40;
        private const int FooterHeight = 30;
        #endregion

        #region Properties
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

        protected virtual void OnDateTimeSelected()
        {
            DateTimeSelected?.Invoke(this, SelectedDateTime);
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
                ImagePath = "left-arrow.svg",
                ApplyThemeOnImage = true,
                Size = new Size(16, 16),
                Theme = Theme
            };
            _rightArrow = new BeepImage
            {
                ImagePath = "right-arrow.svg",
                ApplyThemeOnImage = true,
                Size = new Size(16, 16),
                Theme = Theme
            };

            // Initialize time list
            _timeList = new BeepScrollList
            {
                ItemSize = TimeSlotHeight,
                Orientation = ScrollOrientation.VerticalScroll,
                Theme = Theme,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsFrameless = true,
                IsRounded=false,
                IsRoundedAffectedByTheme = false,
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

            Controls.Add(_timeList);

            // Set default size
            Size = new Size(300, 300);
            UpdateLayout();
        }
        #endregion

        #region Layout and Drawing
        private void UpdateLayout()
        {
            int timeListWidth = 80;
            _headerRect = new Rectangle(0, 0, Width, HeaderHeight);
            _daysHeaderRect = new Rectangle(0, HeaderHeight, Width - timeListWidth, CellSize);
            _datesGridRect = new Rectangle(0, HeaderHeight + CellSize, Width - timeListWidth, CellSize * 6);
            _timeListRect = new Rectangle(Width - timeListWidth, HeaderHeight, timeListWidth, Height - HeaderHeight - FooterHeight);
            _footerRect = new Rectangle(0, Height - FooterHeight, Width, FooterHeight);

            _timeList.Location = new Point(_timeListRect.X, _timeListRect.Y);
            _timeList.Size = new Size(_timeListRect.Width, _timeListRect.Height);
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
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

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
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, 50, 50)))
            {
                g.FillRectangle(brush, _headerRect);
            }

            string monthYear = _currentMonth.ToString("MMMM yyyy");
            SizeF textSize = g.MeasureString(monthYear, Font);
            PointF textPos = new PointF((_headerRect.Width - textSize.Width) / 2, (_headerRect.Height - textSize.Height) / 2);
            g.DrawString(monthYear, Font, Brushes.White, textPos);

            // Draw navigation arrows
            _leftArrow.Location = new Point(10, (_headerRect.Height - _leftArrow.Height) / 2);
            _rightArrow.Location = new Point(_headerRect.Width - _rightArrow.Width - 10, (_headerRect.Height - _rightArrow.Height) / 2);
            _leftArrow.DrawImage(g, new Rectangle(_leftArrow.Location, _leftArrow.Size));
            _rightArrow.DrawImage(g, new Rectangle(_rightArrow.Location, _rightArrow.Size));
        }

        private void DrawDaysHeader(Graphics g)
        {
            string[] days = { "M", "T", "W", "T", "F", "S", "S" };
            for (int i = 0; i < 7; i++)
            {
                Rectangle dayRect = new Rectangle(_daysHeaderRect.X + i * CellSize, _daysHeaderRect.Y, CellSize, CellSize);
                g.DrawString(days[i], Font, Brushes.Gray, dayRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        private void DrawDatesGrid(Graphics g)
        {
            DateTime firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            int dayOfWeek = ((int)firstDayOfMonth.DayOfWeek + 6) % 7; // Adjust for Monday start
            int daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);

            int row = 0, col = dayOfWeek;
            for (int day = 1; day <= daysInMonth; day++)
            {
                Rectangle cellRect = new Rectangle(_datesGridRect.X + col * CellSize, _datesGridRect.Y + row * CellSize, CellSize, CellSize);
                DateTime currentDate = new DateTime(_currentMonth.Year, _currentMonth.Month, day);

                bool isSelected = _selectedDateTime.HasValue && _selectedDateTime.Value.Date == currentDate.Date;
                bool isToday = currentDate.Date == DateTime.Today;

                if (isSelected)
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 182, 193)))
                    {
                        g.FillRectangle(brush, cellRect);
                    }
                }
                else if (isToday)
                {
                    using (Pen pen = new Pen(Color.Gray, 1))
                    {
                        g.DrawRectangle(pen, cellRect);
                    }
                }

                g.DrawString(day.ToString(), Font, Brushes.Black, cellRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

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
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(50, 50, 50)))
            {
                g.FillRectangle(brush, _footerRect);
            }

            string todayText = "TODAY";
            g.DrawString(todayText, Font, Brushes.White, new Rectangle(_footerRect.X + 5, _footerRect.Y, 50, _footerRect.Height), new StringFormat { LineAlignment = StringAlignment.Center });

            string selectedText = _selectedDateTime.HasValue ? _selectedDateTime.Value.ToString("MMMM d, yyyy h:mm tt") : "Select a date";
            g.DrawString(selectedText, Font, Brushes.White, new Rectangle(_footerRect.X + 60, _footerRect.Y, _footerRect.Width - 65, _footerRect.Height), new StringFormat { LineAlignment = StringAlignment.Center });
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
                    int col = (e.X - _datesGridRect.X) / CellSize;
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
                        OnDateTimeSelected();
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
                    OnDateTimeSelected();
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
                    OnDateTimeSelected();
                }
                Invalidate();
            }
        }
        #endregion

        #region Overrides
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            _leftArrow.Theme = Theme;
            _rightArrow.Theme = Theme;
            _timeList.Theme = Theme;
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _leftArrow?.Dispose();
                _rightArrow?.Dispose();
                _timeList?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}