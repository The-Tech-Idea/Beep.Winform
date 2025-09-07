using System.ComponentModel;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Managers;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Date Picker View")]
    [Description("A modern calendar view control optimized for date selection with clean design.")]
    public class BeepDatePickerView : BaseControl
    {
        #region Fields
        private DateTime _currentMonth;
        private DateTime? _selectedDateTime;
        private Rectangle _headerRect;
        private Rectangle _daysHeaderRect;
        private Rectangle _datesGridRect;
        private Rectangle _selectedDateDisplayRect;
        private Rectangle _buttonsRect;
        private BeepImage _leftArrow;
        private BeepImage _rightArrow;
        private BeepButton _clearButton;
        private BeepButton _doneButton;
        
        // Add time spinner controls
        private BeepButton _timeUpButton;
        private BeepButton _timeDownButton;
        private Rectangle _timeSpinnerRect;
        private bool _mouseOverTimeArea = false;
        
        // Modern layout constants
        private const int CellSize = 36;
        private const int HeaderHeight = 50;
        private const int SelectedDateDisplayHeight = 40;
        private const int ButtonHeight = 40;
        private const int ModernPadding = 12;
        private const int TimeSpinnerWidth = 20;
        #endregion

        #region Properties
        #region Appearance Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the month/year header")]
        public Font HeaderFont { get; set; } = new Font("Segoe UI", 14, FontStyle.Bold);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the days of week header")]
        public Font DaysHeaderFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Regular);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the date numbers")]
        public Font DateFont { get; set; } = new Font("Segoe UI", 12, FontStyle.Regular);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for the selected date display")]
        public Font SelectedDateFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Bold);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for buttons")]
        public Font ButtonFont { get; set; } = new Font("Segoe UI", 11, FontStyle.Bold);
        #endregion

        [Browsable(true)]
        [Category("Data")]
        [Description("The selected date and time. Null if no date is selected.")]
        public DateTime? SelectedDateTime
        {
            get => _selectedDateTime;
            set
            {
                _selectedDateTime = value;
                if (_selectedDateTime.HasValue)
                {
                    _currentMonth = new DateTime(_selectedDateTime.Value.Year, _selectedDateTime.Value.Month, 1);
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show time in selected date display")]
        public bool ShowTime { get; set; } = true;
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
        public BeepDatePickerView()
        {
            InitializeModernCalendar();
        }

        private void InitializeModernCalendar()
        {
            DoubleBuffered = true;
            _currentMonth = DateTime.Today;
            if (!_selectedDateTime.HasValue)
            {
                _selectedDateTime = DateTime.Today;
            }

            // Initialize navigation arrows
            _leftArrow = new BeepImage
            {
                ImagePath = "angle-small-left.svg",
                ApplyThemeOnImage = true,
                Size = new Size(24, 24),
                Theme = Theme
            };
            _rightArrow = new BeepImage
            {
                ImagePath = "angle-small-right.svg",
                ApplyThemeOnImage = true,
                Size = new Size(24, 24),
                Theme = Theme
            };

            // Initialize time spinner buttons (triangle up/down)
            _timeUpButton = new BeepButton
            {
                Text = "▲",
                Theme = Theme,
                IsChild = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Color.FromArgb(66, 133, 244),
                ForeColor = Color.White,
                Size = new Size(20, 15),
                Visible = false
            };
            _timeUpButton.Click += TimeUpButton_Click;

            _timeDownButton = new BeepButton
            {
                Text = "▼",
                Theme = Theme,
                IsChild = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Color.FromArgb(66, 133, 244),
                ForeColor = Color.White,
                Size = new Size(20, 15),
                Visible = false
            };
            _timeDownButton.Click += TimeDownButton_Click;

            Controls.Add(_timeUpButton);
            Controls.Add(_timeDownButton);

            // Initialize Clear and Done buttons
            _clearButton = new BeepButton
            {
                Text = "Clear",
                Theme = Theme,
                IsChild = false, // Change to false so buttons render independently
                TextAlign = ContentAlignment.MiddleCenter,
                Font = ButtonFont,
                BackColor = Color.FromArgb(45, 45, 45), // Dark background like in image
                ForeColor = Color.White,
                Visible = true,
                Enabled = true
            };
            _clearButton.Click += ClearButton_Click;

            _doneButton = new BeepButton
            {
                Text = "Done",
                Theme = Theme,
                IsChild = false, // Change to false so buttons render independently
                TextAlign = ContentAlignment.MiddleCenter,
                Font = ButtonFont,
                BackColor = Color.FromArgb(66, 133, 244), // Blue background like in image
                ForeColor = Color.White,
                Visible = true,
                Enabled = true
            };
            _doneButton.Click += DoneButton_Click;

            Controls.Add(_clearButton);
            Controls.Add(_doneButton);

            // Calculate proper minimum size that includes ALL components
            int minimumRequiredHeight = CalculateMinimumRequiredHeight();
            int minimumRequiredWidth = CalculateMinimumRequiredWidth();

            // Set default size and minimum size to match modern calendar design
            Size = new Size(Math.Max(320, minimumRequiredWidth), Math.Max(420, minimumRequiredHeight));
            MinimumSize = new Size(minimumRequiredWidth, minimumRequiredHeight);
            
            UpdateModernLayout();
        }

     

        private int CalculateMinimumRequiredHeight()
        {
            // Calculate total height needed for all components
            int totalHeight = 0;
            
            totalHeight += ModernPadding; // Top padding (12)
            totalHeight += HeaderHeight; // Month/year header (50)
            totalHeight += ModernPadding; // Spacing after header (12)
            totalHeight += CellSize; // Days header (36)
            totalHeight += CellSize * 6; // Calendar grid (216)
            totalHeight += ModernPadding; // Spacing after grid (12)
            totalHeight += SelectedDateDisplayHeight; // Selected date display (40)
            totalHeight += ModernPadding; // Spacing after selected date (12)
            totalHeight += ButtonHeight; // Buttons area (40)
            totalHeight += ModernPadding; // Bottom padding (12)
            
            return totalHeight; // Total: 442px
        }

        private int CalculateMinimumRequiredWidth()
        {
            // Calculate minimum width needed
            int minimumCellWidth = 32; // Minimum cell width for readability
            int minimumCalendarWidth = minimumCellWidth * 7; // 7 days = 224px
            int totalMinimumWidth = minimumCalendarWidth + (ModernPadding * 2); // 224 + 24 = 248px
            
            return Math.Max(280, totalMinimumWidth); // Return 280px minimum
        }
        #endregion

        #region Layout and Drawing
        private void UpdateModernLayout()
        {
            if (Width <= 0 || Height <= 0)
                return;

            // Ensure we have proper drawing rectangle
            UpdateDrawingRect();
            
            int availableWidth = Math.Max(200, DrawingRect.Width - (ModernPadding * 2));
            int currentY = DrawingRect.Top + ModernPadding;

            // Ensure the control height is adequate
            int requiredHeight = CalculateMinimumRequiredHeight();
            if (Height < requiredHeight)
            {
                Height = requiredHeight;
                return; // Will trigger another layout update
            }

            // Header rectangle (month/year display with arrows)
            _headerRect = new Rectangle(
                DrawingRect.Left + ModernPadding,
                currentY,
                availableWidth,
                HeaderHeight
            );
            currentY += HeaderHeight + ModernPadding;

            // Days header rectangle (Mo Tu We Th Fr Sa Su)
            _daysHeaderRect = new Rectangle(
                DrawingRect.Left + ModernPadding,
                currentY,
                availableWidth,
                CellSize
            );
            currentY += CellSize;

            // Dates grid rectangle (calendar dates - 6 rows max)
            _datesGridRect = new Rectangle(
                DrawingRect.Left + ModernPadding,
                currentY,
                availableWidth,
                CellSize * 6
            );
            currentY += CellSize * 6 + ModernPadding;

            // Selected date display rectangle (the blue rounded box)
            _selectedDateDisplayRect = new Rectangle(
                DrawingRect.Left + ModernPadding,
                currentY,
                availableWidth,
                SelectedDateDisplayHeight
            );
            currentY += SelectedDateDisplayHeight + ModernPadding;

            // Buttons rectangle (Clear and Done buttons)
            _buttonsRect = new Rectangle(
                DrawingRect.Left + ModernPadding,
                currentY,
                availableWidth,
                ButtonHeight
            );

            // TIME SPINNER AREA
            _timeSpinnerRect = new Rectangle(
                DrawingRect.Right - TimeSpinnerWidth - ModernPadding,
                DrawingRect.Top + ModernPadding + HeaderHeight + ModernPadding + CellSize + ModernPadding,
                TimeSpinnerWidth,
                CellSize * 6 + SelectedDateDisplayHeight + ButtonHeight + ModernPadding * 3
            );

            // Position buttons with proper size validation
            int buttonSpacing = 8;
            int buttonWidth = Math.Max(80, (availableWidth - buttonSpacing) / 2);

            if (_clearButton != null)
            {
                _clearButton.Location = new Point(_buttonsRect.X, _buttonsRect.Y);
                _clearButton.Size = new Size(buttonWidth, ButtonHeight);
                _clearButton.Visible = true;
                _clearButton.BringToFront();
            }

            if (_doneButton != null)
            {
                _doneButton.Location = new Point(_buttonsRect.X + buttonWidth + buttonSpacing, _buttonsRect.Y);
                _doneButton.Size = new Size(buttonWidth, ButtonHeight);
                _doneButton.Visible = true;
                _doneButton.BringToFront();
            }

            // POSITION TIME SPINNER BUTTONS
            Point timeBaseLocation = new Point(_timeSpinnerRect.X, _timeSpinnerRect.Y);
            Size timeButtonSize = new Size(_timeSpinnerRect.Width, (CellSize * 6 + SelectedDateDisplayHeight + ButtonHeight) / 6);

            if (_timeUpButton == null)
            {
                _timeUpButton = new BeepButton
                {
                    Theme = Theme,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = ButtonFont,
                    BackColor = Color.FromArgb(66, 133, 244), // Blue background like in image
                    ForeColor = Color.White,
                    Text = "▲",
                    Visible = true,
                    Enabled = true
                };
                _timeUpButton.Click += TimeUpButton_Click;
                Controls.Add(_timeUpButton);
            }
            _timeUpButton.Location = timeBaseLocation;
            _timeUpButton.Size = timeButtonSize;

            if (_timeDownButton == null)
            {
                _timeDownButton = new BeepButton
                {
                    Theme = Theme,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = ButtonFont,
                    BackColor = Color.FromArgb(45, 45, 45), // Dark background like in image
                    ForeColor = Color.White,
                    Text = "▼",
                    Visible = true,
                    Enabled = true
                };
                _timeDownButton.Click += TimeDownButton_Click;
                Controls.Add(_timeDownButton);
            }
            _timeDownButton.Location = new Point(timeBaseLocation.X, timeBaseLocation.Y + timeButtonSize.Height);
            _timeDownButton.Size = timeButtonSize;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            // Custom background rendering if needed
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            // Ensure minimum size is maintained
            int requiredHeight = CalculateMinimumRequiredHeight();
            int requiredWidth = CalculateMinimumRequiredWidth();
            
            bool sizeChanged = false;
            
            if (Width < requiredWidth)
            {
                Width = requiredWidth;
                sizeChanged = true;
            }
            
            if (Height < requiredHeight)
            {
                Height = requiredHeight;
                sizeChanged = true;
            }
            
            if (!sizeChanged)
            {
                UpdateModernLayout();
            }
            
            // Force buttons to be visible and properly positioned as a fallback
            if (_clearButton != null && _doneButton != null)
            {
                int safeButtonY = Math.Max(10, Height - ButtonHeight - 15);
                int safeButtonWidth = Math.Max(80, (Width - 40) / 2);
                
                _clearButton.Location = new Point(10, safeButtonY);
                _clearButton.Size = new Size(safeButtonWidth, ButtonHeight);
                _clearButton.Visible = true;
                _clearButton.BringToFront();
                
                _doneButton.Location = new Point(20 + safeButtonWidth, safeButtonY);
                _doneButton.Size = new Size(safeButtonWidth, ButtonHeight);
                _doneButton.Visible = true;
                _doneButton.BringToFront();
            }

            // Update time spinner layout
            if (_timeUpButton != null && _timeDownButton != null)
            {
                _timeUpButton.Size = _timeDownButton.Size = new Size(TimeSpinnerWidth, (CellSize * 6 + SelectedDateDisplayHeight + ButtonHeight) / 6);
                _timeUpButton.Location = new Point(Width - TimeSpinnerWidth - ModernPadding, ModernPadding + HeaderHeight + ModernPadding + CellSize + ModernPadding);
                _timeDownButton.Location = new Point(Width - TimeSpinnerWidth - ModernPadding, _timeUpButton.Bottom);
            }
        }

        protected override void DrawContent(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            base.DrawContent(g);
            
            // Draw header (month and year with navigation)
            DrawModernHeader(g);

            // Draw days of the week header
            DrawModernDaysHeader(g);

            // Draw calendar dates grid
            DrawModernDatesGrid(g);

            // Draw selected date display
            DrawModernSelectedDateDisplay(g);

            // Draw time spinner area if mouse is over
            if (_mouseOverTimeArea)
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(10, 66, 133, 244)))
                {
                    g.FillRectangle(brush, _timeSpinnerRect);
                }
            }
        }

        private void DrawModernHeader(Graphics g)
        {
            // Draw month/year text
            string monthYear = _currentMonth.ToString("MMMM yyyy");
            SizeF textSize = g.MeasureString(monthYear, HeaderFont);
            PointF textPos = new PointF(
                _headerRect.Left + (_headerRect.Width - textSize.Width) / 2,
                _headerRect.Top + (_headerRect.Height - textSize.Height) / 2
            );
            
            using (Brush brush = new SolidBrush(_currentTheme?.CalendarTitleForColor ?? ForeColor))
            {
                g.DrawString(monthYear, HeaderFont, brush, textPos);
            }

            // Position and draw navigation arrows
            int arrowVerticalCenter = _headerRect.Top + (_headerRect.Height - _leftArrow.Height) / 2;
            
            _leftArrow.Location = new Point(_headerRect.Left + 12, arrowVerticalCenter);
            _leftArrow.DrawImage(g, new Rectangle(_leftArrow.Location, _leftArrow.Size));

            _rightArrow.Location = new Point(_headerRect.Right - _rightArrow.Width - 12, arrowVerticalCenter);
            _rightArrow.DrawImage(g, new Rectangle(_rightArrow.Location, _rightArrow.Size));
        }

        private void DrawModernDaysHeader(Graphics g)
        {
            string[] days = { "Mo", "Tu", "We", "Th", "Fr", "Sa", "Su" };
            int cellWidth = _daysHeaderRect.Width / 7;
            
            using (Brush brush = new SolidBrush(_currentTheme?.CalendarDaysHeaderForColor ?? Color.Gray))
            {
                for (int i = 0; i < 7; i++)
                {
                    Rectangle dayRect = new Rectangle(
                        _daysHeaderRect.X + i * cellWidth, 
                        _daysHeaderRect.Y,
                        cellWidth, 
                        _daysHeaderRect.Height
                    );
                    
                    g.DrawString(days[i], DaysHeaderFont, brush, dayRect,
                                 new StringFormat
                                 {
                                     Alignment = StringAlignment.Center,
                                     LineAlignment = StringAlignment.Center
                                 });
                }
            }
        }

        private void DrawModernDatesGrid(Graphics g)
        {
            DateTime firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            int dayOfWeek = ((int)firstDayOfMonth.DayOfWeek + 6) % 7; // Monday start (0 = Monday)
            int daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
            
            int cellWidth = _datesGridRect.Width / 7;
            int cellHeight = CellSize;

            // Draw previous month dates (dimmed)
            if (dayOfWeek > 0)
            {
                DateTime prevMonth = _currentMonth.AddMonths(-1);
                int prevMonthDays = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
                
                using (SolidBrush dimBrush = new SolidBrush(Color.FromArgb(100, _currentTheme?.CalendarForeColor ?? ForeColor)))
                {
                    for (int i = dayOfWeek - 1; i >= 0; i--)
                    {
                        int day = prevMonthDays - i;
                        Rectangle cellRect = new Rectangle(
                            _datesGridRect.X + (dayOfWeek - 1 - i) * cellWidth,
                            _datesGridRect.Y,
                            cellWidth,
                            cellHeight
                        );
                        
                        g.DrawString(day.ToString(), DateFont, dimBrush, cellRect,
                                     new StringFormat
                                     {
                                         Alignment = StringAlignment.Center,
                                         LineAlignment = StringAlignment.Center
                                     });
                    }
                }
            }

            // Draw current month dates
            int row = 0, col = dayOfWeek;
            for (int day = 1; day <= daysInMonth; day++)
            {
                Rectangle cellRect = new Rectangle(
                    _datesGridRect.X + col * cellWidth,
                    _datesGridRect.Y + row * cellHeight,
                    cellWidth,
                    cellHeight
                );
                
                DateTime currentDate = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
                bool isSelected = _selectedDateTime.HasValue && _selectedDateTime.Value.Date == currentDate.Date;
                bool isToday = currentDate.Date == DateTime.Today;

                // Draw selection background - modern rounded style like in the image
                if (isSelected)
                {
                    Rectangle selectionRect = Rectangle.Inflate(cellRect, -6, -6);
                    using (GraphicsPath path = GetRoundedRectPath(selectionRect, 8))
                    using (SolidBrush brush = new SolidBrush(_currentTheme?.CalendarSelectedDateBackColor ?? Color.FromArgb(66, 133, 244)))
                    {
                        g.FillPath(brush, path);
                    }
                }
                else if (isToday)
                {
                    Rectangle todayRect = Rectangle.Inflate(cellRect, -6, -6);
                    using (GraphicsPath path = GetRoundedRectPath(todayRect, 8))
                    using (Pen pen = new Pen(_currentTheme?.CalendarTodayForeColor ?? Color.FromArgb(66, 133, 244), 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                // Draw date number
                Color textColor = isSelected ? 
                    (_currentTheme?.CalendarSelectedDateForColor ?? Color.White) :
                    (_currentTheme?.CalendarForeColor ?? ForeColor);
                    
                using (SolidBrush textBrush = new SolidBrush(textColor))
                {
                    g.DrawString(day.ToString(), DateFont, textBrush, cellRect,
                                 new StringFormat
                                 {
                                     Alignment = StringAlignment.Center,
                                     LineAlignment = StringAlignment.Center
                                 });
                }

                col++;
                if (col > 6)
                {
                    col = 0;
                    row++;
                }
            }

            // Draw next month dates (dimmed) to fill remaining cells
            int remainingCells = 42 - (dayOfWeek + daysInMonth);
            if (remainingCells > 0 && row < 6)
            {
                using (SolidBrush dimBrush = new SolidBrush(Color.FromArgb(100, _currentTheme?.CalendarForeColor ?? ForeColor)))
                {
                    int nextMonthDay = 1;
                    while (col <= 6 && row < 6)
                    {
                        Rectangle cellRect = new Rectangle(
                            _datesGridRect.X + col * cellWidth,
                            _datesGridRect.Y + row * cellHeight,
                            cellWidth,
                            cellHeight
                        );
                        
                        g.DrawString(nextMonthDay.ToString(), DateFont, dimBrush, cellRect,
                                     new StringFormat
                                     {
                                         Alignment = StringAlignment.Center,
                                         LineAlignment = StringAlignment.Center
                                     });
                        
                        nextMonthDay++;
                        col++;
                        if (col > 6)
                        {
                            col = 0;
                            row++;
                        }
                    }
                }
            }
        }

        private void DrawModernSelectedDateDisplay(Graphics g)
        {
            if (_selectedDateTime.HasValue)
            {
                // Draw blue rounded background like in the image
                using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(66, 133, 244)))
                {
                    using (GraphicsPath path = GetRoundedRectPath(_selectedDateDisplayRect, 8))
                    {
                        g.FillPath(bgBrush, path);
                    }
                }

                // Split the display into date and time parts when ShowTime is true
                if (ShowTime)
                {
                    // Calculate areas for date and time
                    int timeWidth = 60; // Width for time display (HH:mm)
                    int dateWidth = _selectedDateDisplayRect.Width - timeWidth - TimeSpinnerWidth - 10;
                    
                    // Date portion
                    Rectangle dateRect = new Rectangle(
                        _selectedDateDisplayRect.X + 5,
                        _selectedDateDisplayRect.Y,
                        dateWidth,
                        _selectedDateDisplayRect.Height
                    );

                    // Time portion
                    Rectangle timeRect = new Rectangle(
                        dateRect.Right + 5,
                        _selectedDateDisplayRect.Y,
                        timeWidth,
                        _selectedDateDisplayRect.Height
                    );

                    // Time spinner area
                    _timeSpinnerRect = new Rectangle(
                        timeRect.Right + 5,
                        _selectedDateDisplayRect.Y + 5,
                        TimeSpinnerWidth,
                        _selectedDateDisplayRect.Height - 10
                    );

                    // Draw date text
                    string dateText = _selectedDateTime.Value.ToString("dd.MM.yyyy");
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        g.DrawString(dateText, SelectedDateFont, textBrush, dateRect,
                                     new StringFormat
                                     {
                                         Alignment = StringAlignment.Center,
                                         LineAlignment = StringAlignment.Center
                                     });
                    }

                    // Draw time text
                    string timeText = _selectedDateTime.Value.ToString("HH:mm");
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        g.DrawString(timeText, SelectedDateFont, textBrush, timeRect,
                                     new StringFormat
                                     {
                                         Alignment = StringAlignment.Center,
                                         LineAlignment = StringAlignment.Center
                                     });
                    }

                    // Show/hide time spinner buttons based on mouse hover
                    if (_mouseOverTimeArea)
                    {
                        PositionTimeSpinnerButtons();
                        _timeUpButton.Visible = true;
                        _timeDownButton.Visible = true;
                        _timeUpButton.BringToFront();
                        _timeDownButton.BringToFront();
                    }
                    else
                    {
                        _timeUpButton.Visible = false;
                        _timeDownButton.Visible = false;
                    }
                }
                else
                {
                    // Show only date when ShowTime is false
                    string selectedText = _selectedDateTime.Value.ToString("dd.MM.yyyy");
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        g.DrawString(selectedText, SelectedDateFont, textBrush, _selectedDateDisplayRect,
                                     new StringFormat
                                     {
                                         Alignment = StringAlignment.Center,
                                         LineAlignment = StringAlignment.Center
                                     });
                    }
                    
                    _timeUpButton.Visible = false;
                    _timeDownButton.Visible = false;
                }
            }
            else
            {
                // Gray placeholder when no date selected
                using (SolidBrush textBrush = new SolidBrush(Color.Gray))
                {
                    g.DrawString("Select a date", SelectedDateFont, textBrush, _selectedDateDisplayRect,
                                 new StringFormat
                                 {
                                     Alignment = StringAlignment.Center,
                                     LineAlignment = StringAlignment.Center
                                 });
                }
                
                _timeUpButton.Visible = false;
                _timeDownButton.Visible = false;
            }
        }

        private void PositionTimeSpinnerButtons()
        {
            int buttonHeight = _timeSpinnerRect.Height / 2;
            
            // Position up button (top half of spinner area)
            _timeUpButton.Location = new Point(
                _timeSpinnerRect.X,
                _timeSpinnerRect.Y
            );
            _timeUpButton.Size = new Size(_timeSpinnerRect.Width, buttonHeight);

            // Position down button (bottom half of spinner area)
            _timeDownButton.Location = new Point(
                _timeSpinnerRect.X,
                _timeSpinnerRect.Y + buttonHeight
            );
            _timeDownButton.Size = new Size(_timeSpinnerRect.Width, buttonHeight);
        }
        #endregion

        #region Mouse Interaction
        private void ClearButton_Click(object sender, EventArgs e)
        {
            SelectedDateTime = null;
            OnCancelled();
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            OnDateTimeSelected();
        }

        private void TimeUpButton_Click(object sender, EventArgs e)
        {
            if (_selectedDateTime.HasValue)
            {
                _selectedDateTime = _selectedDateTime.Value.AddMinutes(1);
                Invalidate();
            }
        }

        private void TimeDownButton_Click(object sender, EventArgs e)
        {
            if (_selectedDateTime.HasValue)
            {
                _selectedDateTime = _selectedDateTime.Value.AddMinutes(-1);
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
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
                else if (_datesGridRect.Contains(e.Location))
                {
                    HandleDateSelection(e.Location);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Check if mouse is over time spinner area
            _mouseOverTimeArea = _timeSpinnerRect.Contains(e.Location);

            // Hand cursor over time spinner up/down buttons
            if (_timeUpButton.Bounds.Contains(e.Location) || _timeDownButton.Bounds.Contains(e.Location))
            {
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        private void HandleDateSelection(Point location)
        {
            int cellWidth = _datesGridRect.Width / 7;
            int cellHeight = CellSize;
            
            int col = (location.X - _datesGridRect.X) / cellWidth;
            int row = (location.Y - _datesGridRect.Y) / cellHeight;
            
            if (col >= 0 && col < 7 && row >= 0 && row < 6)
            {
                DateTime firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
                int dayOfWeek = ((int)firstDayOfMonth.DayOfWeek + 6) % 7; // Monday start
                
                int dayIndex = row * 7 + col;
                int day = dayIndex - dayOfWeek + 1;
                
                if (day > 0 && day <= DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month))
                {
                    DateTime selectedDate = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
                    
                    // Preserve time if previously selected
                    if (_selectedDateTime.HasValue && ShowTime)
                    {
                        selectedDate = selectedDate.Date.Add(_selectedDateTime.Value.TimeOfDay);
                    }
                    else if (ShowTime)
                    {
                        selectedDate = selectedDate.Date.AddHours(9); // Default to 9:00 AM
                    }
                    
                    _selectedDateTime = selectedDate;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Size Enforcement
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // Calculate minimum required dimensions
            int minHeight = ModernPadding + HeaderHeight + ModernPadding + CellSize + (CellSize * 6) + 
                           ModernPadding + SelectedDateDisplayHeight + ModernPadding + ButtonHeight + ModernPadding;
            int minWidth = (32 * 7) + (ModernPadding * 2); // Minimum for 7 day cells + padding
            
            // Enforce minimum size
            if (width < Math.Max(280, minWidth))
                width = Math.Max(280, minWidth);
            
            if (height < Math.Max(420, minHeight))
                height = Math.Max(420, minHeight);
                
            base.SetBoundsCore(x, y, width, height, specified);
        }
        #endregion

        #region Theme Support
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
                    HeaderFont = FontListHelper.CreateFontFromTypography(_currentTheme.CalendarTitleFont);
                if (_currentTheme.DaysHeaderFont != null)
                    DaysHeaderFont = FontListHelper.CreateFontFromTypography(_currentTheme.DaysHeaderFont);
                if (_currentTheme.DateFont != null)
                    DateFont = FontListHelper.CreateFontFromTypography(_currentTheme.DateFont);
                if (_currentTheme.CalendarSelectedFont != null)
                    SelectedDateFont = FontListHelper.CreateFontFromTypography(_currentTheme.CalendarSelectedFont);
                if (_currentTheme.FooterFont != null)
                    ButtonFont = FontListHelper.CreateFontFromTypography(_currentTheme.FooterFont);
            }

            // Apply theme to buttons
            if (_clearButton != null)
            {
                _clearButton.Theme = Theme;
                _clearButton.Font = ButtonFont;
            }

            if (_doneButton != null)
            {
                _doneButton.Theme = Theme;
                _doneButton.Font = ButtonFont;
            }

            // Update layout and redraw
            UpdateModernLayout();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _leftArrow?.Dispose();
                _rightArrow?.Dispose();
                _clearButton?.Dispose();
                _doneButton?.Dispose();
                _timeUpButton?.Dispose();
                _timeDownButton?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}