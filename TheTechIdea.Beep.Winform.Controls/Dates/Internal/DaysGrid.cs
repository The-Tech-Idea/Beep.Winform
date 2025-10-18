using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Dates.Models;
using TheTechIdea.Beep.Winform.Controls.Dates.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Dates.Internal
{
    [ToolboxItem(false)]
    public partial class DaysGrid : BaseControl
    {
        // this control represents the grid of days in the date picker
        // it handles the display and selection of days
        // it raises events when a date is selected
        // use basecontrol properties and events as needed
        // use basecontrol inputhandler as needed
        // use basecontrol styling as needed
        // use beepstyle and theming as needed for days grid specific styling
        // use property  public BeepControlStyle BorderPainter {get;set;} for border painter specific to days grid if needed
        // use Painters to paint the days grid if needed use BeepStyling class 
        // use helpers as needed
        // you need  2 helpers inputhelper and layouthelper for days grid
        // all days are interactive hove effects and click effects
        // again use base control input handling and and hittest handling styling as needed
        public event EventHandler<DateTimePickerEventArgs> DateSelected;

        // Visible month to render (first day of month)
        private DateTime _displayMonth = new(DateTime.Now.Year, DateTime.Now.Month, 1);
        [Browsable(true)]
        [Category("Behavior")]
        public DateTime DisplayMonth
        {
            get => _displayMonth;
            set
            {
                var first = new DateTime(value.Year, value.Month, 1);
                if (_displayMonth == first) return;
                _displayMonth = first;
                RebuildLayout();
                Invalidate();
            }
        }

        // Date Constraints
        private int startday;
        public int StartDay
        {
            get { return startday; }
            set
            {
                if (value >= 0 && value <= 6)
                {
                    startday = value;
                }
                else
                {
                    startday = 0; // Default to Sunday if invalid
                }
                RebuildLayout();
                Invalidate();
            }
        }
        private int _endday;
        public int EndDay
        {
            get { return _endday; }
            set
            {
                if (value >= 0 && value <= 6)
                {
                    _endday = value;
                }
                else
                {
                    _endday = 6; // Default to Saturday if invalid
                }
                RebuildLayout();
                Invalidate();
            }
        }

        private DateTime _mindate;
        public DateTime? MinDate
        {
            get { return _mindate; }
            set
            {
                if (value.HasValue)
                {
                    _mindate = value.Value;
                }
                else
                {
                    _mindate = DateTime.MinValue;
                }
                Invalidate();
            }
        }
        private DateTime _maxdate = DateTime.MaxValue;
        public DateTime? MaxDate
        {
            get { return _maxdate; }
            set
            {
                if (value.HasValue)
                {
                    _maxdate = value.Value;
                }
                else
                {
                    _maxdate = DateTime.MaxValue;
                }
                Invalidate();
            }
        }
        private DateTime _selectedDate = DateTime.Now;
        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (value.HasValue)
                {
                    _selectedDate = value.Value;
                }
                else
                {
                    _selectedDate = DateTime.Now;
                }
                Invalidate();
            }
        }
        private DateTime _rangestartdate = DateTime.Now;
        public DateTime? RangeStartDate
        {
            get { return _rangestartdate; }
            set
            {
                if (value.HasValue)
                {
                    _rangestartdate = value.Value;
                }
                else
                {
                    _rangestartdate = DateTime.Now;
                }
                Invalidate();
            }
        }
        private DateTime _rangeenddate = DateTime.Now;
        public DateTime? RangeEndDate
        {
            get { return _rangeenddate; }
            set
            {
                if (value.HasValue)
                {
                    _rangeenddate = value.Value;
                }
                else
                {
                    _rangeenddate = DateTime.Now;
                }
                Invalidate();
            }
        }

        #region Day Grid Settings 

        private List<Rectangle> _dayCellRects;
        private Rectangle[,] _dayCellMatrix;
        public List<Rectangle> DayCellRects
        {
            get => _dayCellRects;
            set
            {
                _dayCellRects = value;
                if (value == null)
                {
                    _dayCellMatrix = null;
                }
                else
                {
                    int rows = 6;
                    int cols = 7;
                    _dayCellMatrix = new Rectangle[rows, cols];
                    for (int i = 0; i < value.Count; i++)
                    {
                        int row = i / cols;
                        int col = i % cols;
                        if (row < rows && col < cols)
                        {
                            _dayCellMatrix[row, col] = value[i];
                        }
                    }
                }
            }
        }

        public Rectangle[,] DayCellMatrix
        {
            get => _dayCellMatrix;
            set
            {
                _dayCellMatrix = value;
                if (value == null)
                {
                    _dayCellRects = null;
                }
                else
                {
                    int rows = value.GetLength(0);
                    int cols = value.GetLength(1);
                    var list = new List<Rectangle>(rows * cols);
                    for (int row = 0; row < rows; row++)
                    {
                        for (int col = 0; col < cols; col++)
                        {
                            list.Add(value[row, col]);
                        }
                    }
                    _dayCellRects = list;
                }
            }
        }

        public Rectangle[,] GetDayCellMatrixOrDefault(int defaultRows = 6, int defaultCols = 7)
        {
            if (_dayCellMatrix != null)
            {
                return _dayCellMatrix;
            }

            if (defaultRows <= 0)
            {
                defaultRows = 6;
            }

            if (defaultCols <= 0)
            {
                defaultCols = 7;
            }

            if (_dayCellRects != null && _dayCellRects.Count > 0)
            {
                var matrix = new Rectangle[defaultRows, defaultCols];
                int limit = Math.Min(_dayCellRects.Count, defaultRows * defaultCols);
                for (int i = 0; i < limit; i++)
                {
                    matrix[i / defaultCols, i % defaultCols] = _dayCellRects[i];
                }

                DayCellMatrix = matrix;
                return _dayCellMatrix;
            }

            DayCellMatrix = new Rectangle[defaultRows, defaultCols];
            return _dayCellMatrix;
        }

        public Rectangle GetDayCellRect(int row, int col, int defaultRows = 6, int defaultCols = 7)
        {
            var matrix = GetDayCellMatrixOrDefault(defaultRows, defaultCols);
            if (matrix == null)
            {
                return Rectangle.Empty;
            }

            if (row < 0 || row >= matrix.GetLength(0) || col < 0 || col >= matrix.GetLength(1))
            {
                return Rectangle.Empty;
            }

            return matrix[row, col];
        }
        #region Theme Support
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null)
                return;

            // Apply calendar-specific theme properties to the control
            BackColor = _currentTheme.CalendarBackColor;
            ForeColor = _currentTheme.CalendarForeColor;
              
          

            // Apply calendar-specific theme properties
            BackColor = _currentTheme.CalendarBackColor;
            ForeColor = _currentTheme.CalendarForeColor;
            BorderColor = _currentTheme.CalendarBorderColor;
            CalendarTitleForColor = _currentTheme.CalendarTitleForColor;
            CalendarDaysHeaderForColor = _currentTheme.CalendarDaysHeaderForColor;
            CalendarSelectedDateBackColor = _currentTheme.CalendarSelectedDateBackColor;
            CalendarSelectedDateForColor = _currentTheme.CalendarSelectedDateForColor;
            CalendarBackColor = _currentTheme.CalendarBackColor;
            CalendarForeColor = _currentTheme.CalendarForeColor;
            CalendarTodayForeColor = _currentTheme.CalendarTodayForeColor;
            CalendarBorderColor = _currentTheme.CalendarBorderColor;
            CalendarHoverBackColor = _currentTheme.CalendarHoverBackColor;
            CalendarHoverForeColor = _currentTheme.CalendarHoverForeColor;
            CalendarFooterColor = _currentTheme.CalendarFooterColor;
            // Apply fonts if they exist in the theme
            if (UseThemeFont)
            {
                if (_currentTheme.DaysHeaderFont != null)
                    TodayFont = FontListHelper.CreateFontFromTypography(_currentTheme.DaysHeaderFont);
                if (_currentTheme.DateFont != null)
                    DayFont = FontListHelper.CreateFontFromTypography(_currentTheme.DateFont);
                if (_currentTheme.CalendarSelectedFont != null)
                    SelectedDayFont = FontListHelper.CreateFontFromTypography(_currentTheme.CalendarSelectedFont);

            }

          
            Invalidate();
        }
       
        #endregion

        // Keep only name->index for hover/click; compute dates on the fly
        private readonly Dictionary<string, int> _nameToIndex = new();
        // Cache the first visible date for this grid
        private DateTime _gridStartDate;

        #endregion
        // Visual Settings
        public DatePickerHighlightCurrentDate HighlightToday { get; set; } = DatePickerHighlightCurrentDate.Both;
        public DatePickerHighlightSelectedDate HighlightSelected { get; set; } = DatePickerHighlightSelectedDate.Both;
        public Font TodayFont { get; private set; }
        public Font DayFont { get; private set; }
        public Font SelectedDayFont { get; private set; }
        public Color CalendarTitleForColor { get; private set; }
        public Color CalendarDaysHeaderForColor { get; private set; }
        public Color CalendarSelectedDateBackColor { get; private set; }
        public Color CalendarSelectedDateForColor { get; private set; }
        public Color CalendarBackColor { get; private set; }
        public Color CalendarForeColor { get; private set; }
        public Color CalendarTodayForeColor { get; private set; }
        public Color CalendarBorderColor { get; private set; }
        public Color CalendarHoverBackColor { get; private set; }
        public Color CalendarHoverForeColor { get; private set; }
        public Color CalendarFooterColor { get; private set; }

        public DaysGrid()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            StartDay = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            EndDay = (StartDay + 6) % 7;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RebuildLayout();
            Invalidate();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            RebuildLayout();
            Invalidate();
        }

        private Rectangle GetContentArea()
        {
            // Prefer painter-provided content rect when using Material painter
            var rect = GetMaterialContentRectangle();
            if (!rect.IsEmpty && rect.Width > 0 && rect.Height > 0)
                return rect;

            // Fallbacks
            if (this.DrawingRect.Width > 0 && this.DrawingRect.Height > 0)
                return this.DrawingRect;

            return ClientRectangle;
        }

        private void RebuildLayout()
        {
            UpdateDrawingRect();
            var area = GetContentArea();
            _nameToIndex.Clear();
            ClearHitList();

            if (area.Width <= 0 || area.Height <= 0)
            {
                DayCellRects = new List<Rectangle>();
                return;
            }

            const int rows = 6;
            const int cols = 7;
            var inner = Rectangle.Inflate(area, -4, -4);

            int cellWidth = Math.Max(1, inner.Width / cols);
            int cellHeight = Math.Max(1, inner.Height / rows);

            // Calculate first date to show and cache
            var firstOfMonth = new DateTime(DisplayMonth.Year, DisplayMonth.Month, 1);
            int firstDayIndex = ((int)firstOfMonth.DayOfWeek - StartDay + 7) % 7;
            _gridStartDate = firstOfMonth.AddDays(-firstDayIndex);

            var rects = new List<Rectangle>(rows * cols);
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int idx = r * cols + c;
                    var cellRect = new Rectangle(inner.X + c * cellWidth, inner.Y + r * cellHeight, cellWidth, cellHeight);
                    var drawRect = Rectangle.Inflate(cellRect, -2, -2);

                    rects.Add(drawRect);

                    var day = _gridStartDate.AddDays(idx);
                    string name = $"day_{day:yyyyMMdd}";
                    _nameToIndex[name] = idx;
                    AddHitArea(name, drawRect, this, () => SelectDate(day));
                }
            }

            DayCellRects = rects;
        }

        private void SelectDate(DateTime day)
        {
            if (day < _mindate || day > _maxdate)
                return;

            _selectedDate = day;
            DateSelected?.Invoke(this, new DateTimePickerEventArgs(day, null, null, DatePickerMode.DualCalendar));
            Invalidate();
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            var area = GetContentArea();
            if (area.Width <= 0 || area.Height <= 0 || DayCellRects == null || DayCellRects.Count == 0)
                return;

            g.SmoothingMode = EnableHighQualityRendering ? SmoothingMode.AntiAlias : SmoothingMode.None;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Color textColor = ForeColor;
            Color dimText = ControlPaint.Light(textColor, 0.6f);
         
         
            Color todayBack = Color.FromArgb(32, FocusBackColor);
        
            // Use calendar colors/fonts set by ApplyTheme
            Color normalText = CalendarForeColor.IsEmpty ? ForeColor : CalendarForeColor;
            Color outMonthText = ControlPaint.Light(normalText, 0.5f);
            Color disabledText = ControlPaint.LightLight(normalText);
            Color selectedText = CalendarSelectedDateForColor.IsEmpty ? normalText : CalendarSelectedDateForColor;

            Color hoverBack = CalendarHoverBackColor.IsEmpty ? HoverBackColor : CalendarHoverBackColor;
            Color hoverText = CalendarHoverForeColor.IsEmpty ? normalText : CalendarHoverForeColor;
            Color selectedBack = CalendarSelectedDateBackColor.IsEmpty ? SelectedBackColor : CalendarSelectedDateBackColor;
            Color todayAccent = CalendarTodayForeColor.IsEmpty ? FocusBorderColor : CalendarTodayForeColor;
            Color borderColor = CalendarBorderColor.IsEmpty ? BorderColor : CalendarBorderColor;
            Color rangeBack = Color.FromArgb(24, selectedBack);

            Font fontDay = DayFont ?? Font;
            Font fontSelected = SelectedDayFont ?? DayFont ?? Font;
            Font fontToday = TodayFont ?? DayFont ?? Font;

            using var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };

            for (int i = 0; i < DayCellRects.Count; i++)
            {
                var rect = DayCellRects[i];
                if (rect.Width <= 0 || rect.Height <= 0)
                    continue;

                var day = _gridStartDate.AddDays(i);
                bool isCurrent = day.Month == DisplayMonth.Month;

                bool isDisabled = day < _mindate || day > _maxdate;
                bool isSelected = _selectedDate.Date == day.Date;
                bool isToday = DateTime.Today == day.Date;
              
                bool inRange = IsInRange(day);

                bool isHovered = false;
                if (HitTestControl != null && HitTestControl.Name != null)
                {
                    if (_nameToIndex.TryGetValue(HitTestControl.Name, out int hitIndex))
                    {
                        isHovered = hitIndex == i && HitTestControl.IsHovered;
                    }
                }

                // Fills
                if (inRange)
                {
                    using var rb = new SolidBrush(rangeBack);
                    g.FillRectangle(rb, rect);
                }

                if (isSelected && HighlightSelected == DatePickerHighlightSelectedDate.Both)
                {
                 
                    using var sb = new SolidBrush(selectedBack);
                    g.FillRectangle(sb, rect);
                }

                if (isToday && HighlightToday == DatePickerHighlightCurrentDate.Both)
                {
                    using var tb = new SolidBrush(todayBack);
                  
                    g.FillRectangle(tb, rect);
                }

                if (isHovered && !isSelected)
                {
                    using var hb = new SolidBrush(hoverBack);
                    g.FillRectangle(hb, rect);
                }

                // Text
                string dayText = day.Day.ToString(CultureInfo.InvariantCulture);
              
                Color useText = isSelected ? selectedText : (isDisabled ? disabledText : (isCurrent ? normalText : outMonthText));
                if (isHovered && !isSelected && !isDisabled)
                {
                    useText = hoverText;
                }
                Font useFont = isSelected ? fontSelected : (isToday ? fontToday : fontDay);

                var textRect = Rectangle.Inflate(rect, -6, -4);
                using var tbBrush = new SolidBrush(useText);
                g.DrawString(dayText, Font, tbBrush, textRect, sf);
                g.DrawString(dayText, useFont, tbBrush, textRect, sf);

                // Borders
                if (isSelected && HighlightSelected == DatePickerHighlightSelectedDate.Both)
                {
                    using var pen = new Pen(SelectedBorderColor, 2f);
                  
                    g.DrawRectangle(pen, Rectangle.Inflate(rect, -1, -1));
                }
                if (isToday && HighlightToday == DatePickerHighlightCurrentDate.Both)
                {
                    using var pen = new Pen(FocusBorderColor, 2f) { DashStyle = DashStyle.Dash };
                  
                    g.DrawRectangle(pen, Rectangle.Inflate(rect, -2, -2));
                }
            }
        }

        private bool IsInRange(DateTime date)
        {
            if (RangeStartDate == null || RangeEndDate == null)
                return false;
            var s = _rangestartdate.Date;
            var e = _rangeenddate.Date;
            if (s > e) (s, e) = (e, s);
            return date.Date >= s && date.Date <= e;
        }
    }
}
