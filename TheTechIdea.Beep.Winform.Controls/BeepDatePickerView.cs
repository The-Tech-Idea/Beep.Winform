using System.ComponentModel;
using System.Drawing.Drawing2D;
 
using TheTechIdea.Beep.Vis.Modules;
 
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

        // Hit/paint rects (no child controls)
        private Rectangle _prevMonthRect;
        private Rectangle _nextMonthRect;
        private Rectangle _clearRect;
        private Rectangle _doneRect;

        // Time spinner (drawn + hit areas)
        private Rectangle _timeSpinnerRect;
        private Rectangle _timeUpRect;
        private Rectangle _timeDownRect;
        private bool _mouseOverTimeArea = false;

        // Modern layout constants (base values; actual layout is responsive)
        private const int BaseCellSize = 36;
        private const int HeaderHeight = 50;
        private const int SelectedDateDisplayHeight = 40;
        private const int ButtonBarHeight = 32;
        private const int ModernPadding = 12;
        private const int TimeSpinnerWidth = 20;
        private const int MinCell = 28;
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
        [Description("Font used for footer link buttons")]
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

        // Allow parent to pass a context (caption/purpose)
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Context/label passed back with OK/Cancel events (e.g., 'Due Date').")]
        public string Context { get; set; } = "Date Selection";
        #endregion

        #region Events
        public event EventHandler<DateTime?> DateTimeSelected;
        public event EventHandler Cancelled;
        // New explicit events for OK/Cancel with payload
        public event EventHandler<DateTimeDialogResultEventArgs> OkClicked;
        public event EventHandler<DateTimeDialogResultEventArgs> CancelClicked;

        protected virtual void OnDateTimeSelected()
        {
            DateTimeSelected?.Invoke(this, SelectedDateTime);
        }

        protected virtual void OnCancelled()
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnOkClicked()
        {
            OkClicked?.Invoke(this, new DateTimeDialogResultEventArgs(SelectedDateTime, Context, true));
            // maintain backward compatibility
            OnDateTimeSelected();
        }

        protected virtual void OnCancelClicked()
        {
            CancelClicked?.Invoke(this, new DateTimeDialogResultEventArgs(SelectedDateTime, Context, false));
            // maintain backward compatibility
            OnCancelled();
        }
        #endregion

        #region Constructor
        public BeepDatePickerView()
        {
            MaterialUseVariantPadding = true;
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
            // Layout with minimum cell size
            int cell = Math.Max(MinCell, BaseCellSize);
            int totalHeight = 0;
            totalHeight += ModernPadding;               // Top padding
            totalHeight += HeaderHeight;                // Month/year header
            totalHeight += ModernPadding;               // Spacing
            totalHeight += cell;                        // Days header
            totalHeight += cell * 6;                    // Calendar grid (6 rows)
            totalHeight += ModernPadding;               // Spacing
            totalHeight += SelectedDateDisplayHeight;   // Selected date display
            totalHeight += ModernPadding;               // Spacing
            totalHeight += ButtonBarHeight;             // Footer buttons area
            totalHeight += ModernPadding;               // Bottom padding
            return totalHeight;
        }

        private int CalculateMinimumRequiredWidth()
        {
            int minimumCalendarWidth = Math.Max(MinCell, 32) * 7; // 7 day columns
            int totalMinimumWidth = minimumCalendarWidth + (ModernPadding * 2);
            return Math.Max(280, totalMinimumWidth);
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
            int cellW = Math.Max(MinCell, Math.Min(BaseCellSize, availableWidth / 7));
            int gridWidth = cellW * 7; // keep exact multiple of 7
            int leftInset = DrawingRect.Left + ModernPadding + (availableWidth - gridWidth) / 2;
            int currentY = DrawingRect.Top + ModernPadding;

            // Ensure the control height is adequate
            int requiredHeight = CalculateMinimumRequiredHeight();
            if (Height < requiredHeight)
            {
                Height = requiredHeight;
                // continue; layout will use current Height anyway
            }

            // Header rectangle (month/year display with arrows)
            _headerRect = new Rectangle(
                leftInset,
                currentY,
                gridWidth,
                HeaderHeight
            );
            currentY += HeaderHeight + ModernPadding;

            // Days header rectangle (Mo Tu We Th Fr Sa Su)
            _daysHeaderRect = new Rectangle(
                leftInset,
                currentY,
                gridWidth,
                cellW
            );
            currentY += cellW;

            // Dates grid rectangle (calendar dates - 6 rows max)
            int gridHeight = cellW * 6;
            _datesGridRect = new Rectangle(
                leftInset,
                currentY,
                gridWidth,
                gridHeight
            );
            currentY += gridHeight + ModernPadding;

            // Selected date display rectangle (the blue rounded box)
            _selectedDateDisplayRect = new Rectangle(
                leftInset,
                currentY,
                gridWidth,
                SelectedDateDisplayHeight
            );
            currentY += SelectedDateDisplayHeight + ModernPadding;

            // Footer buttons line (Clear and Done link-like)
            _buttonsRect = new Rectangle(
                leftInset,
                currentY,
                gridWidth,
                ButtonBarHeight
            );

            // Compute arrow hit rects within header (squares)
            int arrowSize = Math.Min(24, HeaderHeight - 16);
            _prevMonthRect = new Rectangle(_headerRect.Left + 8, _headerRect.Top + (HeaderHeight - arrowSize) / 2, arrowSize, arrowSize);
            _nextMonthRect = new Rectangle(_headerRect.Right - arrowSize - 8, _headerRect.Top + (HeaderHeight - arrowSize) / 2, arrowSize, arrowSize);

            // Footer hit rects: left/right aligned text areas
            int linkPadding = 6;
            Size clearSize = TextRenderer.MeasureText("Clear", ButtonFont);
            Size doneSize = TextRenderer.MeasureText("Done", ButtonFont);
            _clearRect = new Rectangle(_buttonsRect.Left + linkPadding, _buttonsRect.Top, clearSize.Width + 8, _buttonsRect.Height);
            _doneRect = new Rectangle(_buttonsRect.Right - doneSize.Width - 8 - linkPadding, _buttonsRect.Top, doneSize.Width + 8, _buttonsRect.Height);

            // TIME SPINNER AREA aligned to the right of selected date display
            if (ShowTime)
            {
                int spinnerH = Math.Max(SelectedDateDisplayHeight - 10, 24);
                _timeSpinnerRect = new Rectangle(
                    _selectedDateDisplayRect.Right - TimeSpinnerWidth - 6,
                    _selectedDateDisplayRect.Top + (_selectedDateDisplayRect.Height - spinnerH) / 2,
                    TimeSpinnerWidth,
                    spinnerH
                );
                int half = _timeSpinnerRect.Height / 2;
                _timeUpRect = new Rectangle(_timeSpinnerRect.Left, _timeSpinnerRect.Top, _timeSpinnerRect.Width, half);
                _timeDownRect = new Rectangle(_timeSpinnerRect.Left, _timeSpinnerRect.Top + half, _timeSpinnerRect.Width, _timeSpinnerRect.Height - half);
            }
            else
            {
                _timeSpinnerRect = Rectangle.Empty;
                _timeUpRect = Rectangle.Empty;
                _timeDownRect = Rectangle.Empty;
            }

            // Refresh hit areas using BaseControl helpers
            ClearHitList();
            AddHitArea("PrevMonth", _prevMonthRect, null, () => { _currentMonth = _currentMonth.AddMonths(-1); Invalidate(); });
            AddHitArea("NextMonth", _nextMonthRect, null, () => { _currentMonth = _currentMonth.AddMonths(1); Invalidate(); });
            AddHitArea("Clear", _clearRect, null, () => { SelectedDateTime = null; OnCancelClicked(); Invalidate(); });
            AddHitArea("Done", _doneRect, null, () => { OnOkClicked(); });
            if (ShowTime && !_timeUpRect.IsEmpty)
            {
                AddHitArea("TimeUp", _timeUpRect, null, () => { if (_selectedDateTime.HasValue) { _selectedDateTime = _selectedDateTime.Value.AddMinutes(1); Invalidate(); } });
                AddHitArea("TimeDown", _timeDownRect, null, () => { if (_selectedDateTime.HasValue) { _selectedDateTime = _selectedDateTime.Value.AddMinutes(-1); Invalidate(); } });
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Ensure minimum size is maintained
            int requiredHeight = CalculateMinimumRequiredHeight();
            int requiredWidth = CalculateMinimumRequiredWidth();

            bool sizeChanged = false;
            if (Width < requiredWidth) { Width = requiredWidth; sizeChanged = true; }
            if (Height < requiredHeight) { Height = requiredHeight; sizeChanged = true; }

            UpdateModernLayout();
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

            // Footer link-like buttons
            DrawFooterLinks(g);

            // Draw time spinner hover
            if (_mouseOverTimeArea && ShowTime && !_timeSpinnerRect.IsEmpty)
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(16, 66, 133, 244)))
                {
                    g.FillRectangle(brush, _timeSpinnerRect);
                }
            }
        }

        private (bool hovered, bool pressed) GetHitState(Rectangle rect)
        {
            try
            {
                var ht = _hitTest?.HitTestControl;
                if (ht == null) return (false, false);
                // Prefer exact rect match
                if (ht.TargetRect == rect)
                    return (ht.IsHovered, ht.IsPressed);
                return (false, false);
            }
            catch { return (false, false); }
        }

        private void DrawHoverBackground(Graphics g, Rectangle rect, bool pressed, Color baseColor)
        {
            int alpha = pressed ? 48 : 24; // stronger when pressed
            using var brush = new SolidBrush(Color.FromArgb(alpha, baseColor));
            using var path = GetRoundedRectPath(rect, Math.Min(rect.Width, rect.Height) / 3);
            g.FillPath(brush, path);
        }

        private void DrawModernHeader(Graphics g)
        {
            // Month/year centered
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

            var navColor = _currentTheme?.CalendarForeColor ?? ForeColor;
            // Hover/press backgrounds for chevrons
            var prevState = GetHitState(_prevMonthRect);
            if (prevState.hovered || prevState.pressed)
            {
                DrawHoverBackground(g, _prevMonthRect, prevState.pressed, navColor);
            }
            var nextState = GetHitState(_nextMonthRect);
            if (nextState.hovered || nextState.pressed)
            {
                DrawHoverBackground(g, _nextMonthRect, nextState.pressed, navColor);
            }

            // Draw chevrons on top
            DrawChevron(g, _prevMonthRect, true, navColor);
            DrawChevron(g, _nextMonthRect, false, navColor);
        }

        private void DrawChevron(Graphics g, Rectangle rect, bool left, Color color)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            int w = rect.Width; int h = rect.Height;
            Point p1, p2, p3;
            if (left)
            {
                p1 = new Point(rect.Left + (int)(w * 0.65), rect.Top + (int)(h * 0.2));
                p2 = new Point(rect.Left + (int)(w * 0.35), rect.Top + h / 2);
                p3 = new Point(rect.Left + (int)(w * 0.65), rect.Bottom - (int)(h * 0.2));
            }
            else
            {
                p1 = new Point(rect.Left + (int)(w * 0.35), rect.Top + (int)(h * 0.2));
                p2 = new Point(rect.Left + (int)(w * 0.65), rect.Top + h / 2);
                p3 = new Point(rect.Left + (int)(w * 0.35), rect.Bottom - (int)(h * 0.2));
            }
            using var pen = new Pen(color, 2f) { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLines(pen, new[] { p1, p2, p3 });
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
                                 new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }
            }
        }

        private void DrawModernDatesGrid(Graphics g)
        {
            DateTime firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            int dayOfWeek = ((int)firstDayOfMonth.DayOfWeek + 6) % 7; // Monday start (0 = Monday)
            int daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);

            int cellWidth = _datesGridRect.Width / 7;
            int cellHeight = _datesGridRect.Height / 6;

            // Previous month leading days
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
                                     new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                }
            }

            // Current month dates
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

                Color textColor = isSelected ? (_currentTheme?.CalendarSelectedDateForColor ?? Color.White) : (_currentTheme?.CalendarForeColor ?? ForeColor);
                using (SolidBrush textBrush = new SolidBrush(textColor))
                {
                    g.DrawString(day.ToString(), DateFont, textBrush, cellRect,
                                 new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }

                col++;
                if (col > 6) { col = 0; row++; }
            }

            // Trailing next month days to fill grid
            int usedCells = dayOfWeek + daysInMonth;
            int remainingCells = 42 - usedCells;
            if (remainingCells > 0)
            {
                using (SolidBrush dimBrush = new SolidBrush(Color.FromArgb(100, _currentTheme?.CalendarForeColor ?? ForeColor)))
                {
                    int nextMonthDay = 1;
                    while (row < 6)
                    {
                        while (col <= 6)
                        {
                            Rectangle cellRect = new Rectangle(
                                _datesGridRect.X + col * cellWidth,
                                _datesGridRect.Y + row * cellHeight,
                                cellWidth,
                                cellHeight
                            );
                            g.DrawString(nextMonthDay.ToString(), DateFont, dimBrush, cellRect,
                                         new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                            nextMonthDay++; col++;
                        }
                        col = 0; row++;
                    }
                }
            }
        }

        private void DrawModernSelectedDateDisplay(Graphics g)
        {
            if (_selectedDateTime.HasValue)
            {
                // Blue rounded background
                using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(66, 133, 244)))
                using (GraphicsPath path = GetRoundedRectPath(_selectedDateDisplayRect, 8))
                {
                    g.FillPath(bgBrush, path);
                }

                if (ShowTime)
                {
                    // Date on the left, time near right, spinner on the far right
                    int timeWidth = 72; // HH:mm
                    int spinnerGap = 6;
                    int dateWidth = Math.Max(60, _selectedDateDisplayRect.Width - timeWidth - TimeSpinnerWidth - spinnerGap - 12);

                    Rectangle dateRect = new Rectangle(
                        _selectedDateDisplayRect.X + 6,
                        _selectedDateDisplayRect.Y,
                        dateWidth,
                        _selectedDateDisplayRect.Height
                    );

                    Rectangle timeRect = new Rectangle(
                        dateRect.Right + 6,
                        _selectedDateDisplayRect.Y,
                        timeWidth,
                        _selectedDateDisplayRect.Height
                    );

                    // Spinner already computed in layout
                    // Draw texts
                    string dateText = _selectedDateTime.Value.ToString("dd.MM.yyyy");
                    string timeText = _selectedDateTime.Value.ToString("HH:mm");
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        g.DrawString(dateText, SelectedDateFont, textBrush, dateRect,
                                     new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                        g.DrawString(timeText, SelectedDateFont, textBrush, timeRect,
                                     new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }

                    // Draw spinner chevrons
                    DrawUpDownGlyphs(g, _timeUpRect, _timeDownRect, Color.White);
                }
                else
                {
                    string selectedText = _selectedDateTime.Value.ToString("dd.MM.yyyy");
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        g.DrawString(selectedText, SelectedDateFont, textBrush, _selectedDateDisplayRect,
                                     new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }
                }
            }
            else
            {
                using (SolidBrush textBrush = new SolidBrush(Color.Gray))
                {
                    g.DrawString("Select a date", SelectedDateFont, textBrush, _selectedDateDisplayRect,
                                 new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }
            }
        }

        private void DrawUpDownGlyphs(Graphics g, Rectangle upRect, Rectangle downRect, Color color)
        {
            // Adjust color on hover/press
            var upState = GetHitState(upRect);
            var downState = GetHitState(downRect);
            Color upColor = upState.hovered || upState.pressed ? ControlPaint.Light(color) : color;
            Color downColor = downState.hovered || downState.pressed ? ControlPaint.Light(color) : color;

            if (!upRect.IsEmpty)
            {
                using var pen = new Pen(upColor, upState.pressed ? 2.5f : 2f) { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round };
                Point u1 = new Point(upRect.Left + upRect.Width / 2, upRect.Top + upRect.Height / 3);
                Point u2 = new Point(upRect.Left + upRect.Width / 3, upRect.Bottom - upRect.Height / 3);
                Point u3 = new Point(upRect.Right - upRect.Width / 3, upRect.Bottom - upRect.Height / 3);
                g.DrawPolygon(pen, new[] { u1, u2, u3 });
            }
            if (!downRect.IsEmpty)
            {
                using var pen2 = new Pen(downColor, downState.pressed ? 2.5f : 2f) { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round };
                Point d1 = new Point(downRect.Left + downRect.Width / 3, downRect.Top + downRect.Height / 3);
                Point d2 = new Point(downRect.Right - downRect.Width / 3, downRect.Top + downRect.Height / 3);
                Point d3 = new Point(downRect.Left + downRect.Width / 2, downRect.Bottom - downRect.Height / 3);
                g.DrawPolygon(pen2, new[] { d1, d2, d3 });
            }
        }

        private void DrawFooterLinks(Graphics g)
        {
            var linkColor = _currentTheme?.CalendarSelectedDateBackColor;
            if (linkColor == null || linkColor.Value == Color.Empty) linkColor = Color.FromArgb(66, 133, 244);

            // Compute states
            var clearState = GetHitState(_clearRect);
            var doneState = GetHitState(_doneRect);

            // Optional subtle background for links
            if (clearState.hovered || clearState.pressed)
            {
                var bg = Color.FromArgb(clearState.pressed ? 36 : 20, linkColor.Value);
                using var brush = new SolidBrush(bg);
                g.FillRectangle(brush, _clearRect);
            }
            if (doneState.hovered || doneState.pressed)
            {
                var bg = Color.FromArgb(doneState.pressed ? 36 : 20, linkColor.Value);
                using var brush = new SolidBrush(bg);
                g.FillRectangle(brush, _doneRect);
            }

            // Text color: brighter when hovered/pressed
            Color clearText = clearState.hovered || clearState.pressed ? ControlPaint.Light(linkColor.Value) : linkColor.Value;
            Color doneText = doneState.hovered || doneState.pressed ? ControlPaint.Light(linkColor.Value) : linkColor.Value;

            TextRenderer.DrawText(g, "Clear", ButtonFont, _clearRect, clearText, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            TextRenderer.DrawText(g, "Done", ButtonFont, _doneRect, doneText, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);

            // Underline when hovered
            if (clearState.hovered)
            {
                var size = TextRenderer.MeasureText("Clear", ButtonFont);
                int y = _clearRect.Bottom - 6;
                using var pen = new Pen(clearText, 1);
                g.DrawLine(pen, _clearRect.Left, y, _clearRect.Left + size.Width, y);
            }
            if (doneState.hovered)
            {
                var size = TextRenderer.MeasureText("Done", ButtonFont);
                int y = _doneRect.Bottom - 6;
                using var pen = new Pen(doneText, 1);
                g.DrawLine(pen, _doneRect.Right - size.Width, y, _doneRect.Right, y);
            }
        }
        #endregion

        #region Mouse Interaction
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Use base helper to capture pressed state for hit areas
            _hitTest?.HandleMouseDown(e.Location, e);

            if (e.Button == MouseButtons.Left)
            {
                // Guarantee action for key rectangles even if helper misses
                if (!_doneRect.IsEmpty && _doneRect.Contains(e.Location)) { OnOkClicked(); return; }
                if (!_clearRect.IsEmpty && _clearRect.Contains(e.Location)) { SelectedDateTime = null; OnCancelClicked(); Invalidate(); return; }
                if (!_prevMonthRect.IsEmpty && _prevMonthRect.Contains(e.Location)) { _currentMonth = _currentMonth.AddMonths(-1); Invalidate(); return; }
                if (!_nextMonthRect.IsEmpty && _nextMonthRect.Contains(e.Location)) { _currentMonth = _currentMonth.AddMonths(1); Invalidate(); return; }

                // Direct handling for time spinner clicks to guarantee detection
                if (ShowTime)
                {
                    if (!_timeUpRect.IsEmpty && _timeUpRect.Contains(e.Location))
                    {
                        if (_selectedDateTime.HasValue)
                        {
                            _selectedDateTime = _selectedDateTime.Value.AddMinutes(1);
                            Invalidate();
                        }
                        return;
                    }
                    if (!_timeDownRect.IsEmpty && _timeDownRect.Contains(e.Location))
                    {
                        if (_selectedDateTime.HasValue)
                        {
                            _selectedDateTime = _selectedDateTime.Value.AddMinutes(-1);
                            Invalidate();
                        }
                        return;
                    }
                }

                if (_datesGridRect.Contains(e.Location))
                {
                    HandleDateSelection(e.Location);
                    return;
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Release state and trigger click evaluation via helper
            _hitTest?.HandleMouseUp(e.Location, e);
            _hitTest?.HandleClick(e.Location);
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Keep helper in sync for hover and press visuals
            _hitTest?.HandleMouseMove(e.Location);
            base.OnMouseMove(e);
            _mouseOverTimeArea = ShowTime && (!_timeSpinnerRect.IsEmpty && _timeSpinnerRect.Contains(e.Location));
            Cursor = (_mouseOverTimeArea) ? Cursors.Hand : Cursors.Default;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hitTest?.HandleMouseLeave();
            base.OnMouseLeave(e);
            Invalidate();
        }

        private void HandleDateSelection(Point location)
        {
            int cellWidth = _datesGridRect.Width / 7;
            int cellHeight = _datesGridRect.Height / 6;

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
            int minHeight = CalculateMinimumRequiredHeight();
            int minWidth = CalculateMinimumRequiredWidth();

            if (width < minWidth) width = minWidth;
            if (height < minHeight) height = minHeight;

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

            UpdateModernLayout();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #endregion
    }

    // New EventArgs that returns the selected date/time and whether it was OK or Cancel
   
}