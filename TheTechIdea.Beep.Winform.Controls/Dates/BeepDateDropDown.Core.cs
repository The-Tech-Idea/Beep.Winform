using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public partial class BeepDateDropDown 
    {
        // Core fields (shared across partials)
        internal DateTime? _selectedDateTime = null;
        internal DateTime? _startDate = null;  // For range mode
        internal DateTime? _endDate = null;    // For range mode
        internal bool _isPopupOpen = false;
        internal BeepDateTimePicker _calendarView;
        internal BeepPopupForm _popup;
        internal int _buttonWidth => ScaleValue(24);
        internal int _dropdownPadding => ScaleValue(4);
        internal bool _showDropDown = true;
        
        // SINGLE SOURCE OF TRUTH - Return Type
        internal Models.ReturnDateTimeType _returnType = Models.ReturnDateTimeType.Date;
        
        // Icon path for calendar button
        internal string _calendarIconPath;
        
        // Editing and validation
        internal bool _allowInlineEdit = true;
        internal bool _allowEmpty = true;
        internal DateTime? _minDate = null;
        internal DateTime? _maxDate = null;
        internal string _dateSeparator = " - ";  // For range mode
        
        // Events specific to date selection
        public new event EventHandler<Models.DateTimePickerEventArgs> SelectedDateTimeChanged;
        public event EventHandler<DateRangeEventArgs> DateRangeChanged;
        public event EventHandler DropDownOpened;
        public event EventHandler DropDownClosed;

        public BeepDateDropDown() : base()
        {
            // Store icon path (do not create UI object here)
            try
            {
                _calendarIconPath = TheTechIdea.Beep.Icons.Svgs.fi_tr_calendar;
            }
            catch { _calendarIconPath = string.Empty; }
            
            // Configure text box for date input
            InitializeDateTextBox();
            
            // Override TextChanged to handle date parsing
            base.TextChanged += BeepDateDropDown_TextChanged;
        }

        private void InitializeDateTextBox()
        {
            // Set default date masking
            MaskFormat = TextBoxMaskFormat.Date;
            DateFormat = "MM/dd/yyyy";
            PlaceholderText = "MM/DD/YYYY";
            
            // Configure validation
            ReadOnly = false;
            
            // Set default size
            MinimumSize = new Size(150, 30);
            
            // Configure image for calendar icon
            if (!string.IsNullOrEmpty(_calendarIconPath))
            {
                ImagePath = _calendarIconPath;
                ImageVisible = true;
                ApplyThemeOnImage = true;
                MaxImageSize = new Size(18, 18);
                TextImageRelation = TextImageRelation.TextBeforeImage;
                ImageAlign = ContentAlignment.MiddleRight;
            }
        }

        protected override Size DefaultSize => new Size(200, 34);

        protected override void InitLayout()
        {
            base.InitLayout();
            UpdateMaskForMode();
            ApplyTheme();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Map calendar-related theme properties to control appearance
            try
            {
                if (_currentTheme != null)
                {
                    // Background / foreground / border
                    if (_currentTheme.CalendarBackColor != Color.Empty) BackColor = _currentTheme.CalendarBackColor;
                    if (_currentTheme.CalendarForeColor != Color.Empty) ForeColor = _currentTheme.CalendarForeColor;
                    if (_currentTheme.CalendarBorderColor != Color.Empty) BorderColor = _currentTheme.CalendarBorderColor;

                    // Fonts
                    try 
                    { 
                        TextFont = BeepThemesManager.ToFont(_currentTheme.DateFont); 
                    } 
                    catch 
                    { 
                        // Use default font
                    }

                    // Placeholder color
                    if (_currentTheme.TextBoxPlaceholderColor != Color.Empty)
                    {
                        PlaceholderTextColor = _currentTheme.TextBoxPlaceholderColor;
                    }

                    // Invalidate icon cache to ensure theme colors are applied
                    if (!string.IsNullOrEmpty(_calendarIconPath))
                    {
                        try { StyledImagePainter.InvalidateCaches(_calendarIconPath); } catch { }

                        // Pre-render in background to avoid UI jank
                        Color iconColor = _currentTheme.CalendarTitleForColor != Color.Empty 
                            ? _currentTheme.CalendarTitleForColor 
                            : ForeColor;
                        
                        var sizes = new int[] { 16, 18, 20, 22, 24 };
                        Task.Run(() =>
                        {
                            foreach (var s in sizes)
                            {
                                try 
                                { 
                                    StyledImagePainter.PreRenderTintedToCache(_calendarIconPath, iconColor, 1f, new Size(s, s)); 
                                } 
                                catch { }
                            }
                        });
                    }
                }
            }
            catch { }

            Invalidate();
        }

        // Toggle popup operations
        internal void TogglePopup()
        {
            if (_isPopupOpen) ClosePopup(); else ShowPopup();
        }

        internal void ClosePopup()
        {
            if (!_isPopupOpen) return;
            if (_calendarView != null)
            {
                // Unsubscribe events from internal picker
                _calendarView.DateChanged -= CalendarView_DateChanged_Enhanced;
            }
            _popup?.CloseCascade();
            _popup = null;
            _calendarView = null;
            _isPopupOpen = false;
            DropDownClosed?.Invoke(this, EventArgs.Empty);
        }

        internal void ShowPopup()
        {
            if (!_showDropDown) return;
            
            DropDownOpened?.Invoke(this, EventArgs.Empty);

            _popup = new BeepPopupForm
            {
              
                Theme = Theme
            };

            // Create the internal BeepDateTimePicker and configure it
            _calendarView = new BeepDateTimePicker 
            { 
                Dock = DockStyle.Fill, 
                Theme = Theme 
            };
            
            // Apply mode and constraints
            try { _calendarView.Mode = _mode; } catch { }
            if (_minDate.HasValue) { try { _calendarView.MinDate = _minDate.Value; } catch { } }
            if (_maxDate.HasValue) { try { _calendarView.MaxDate = _maxDate.Value; } catch { } }
            
            // Sync dropdown state TO calendar using DateTimePickerProperties
            SyncToCalendar();
            
            // Push initial selection (legacy support)
            if (_selectedDateTime.HasValue && _selectedDateTime.Value != DateTime.MinValue)
            {
                try { _calendarView.SelectedDate = _selectedDateTime.Value; } catch { }
            }

            // Subscribe to events - use enhanced handler with sync
            _calendarView.DateChanged += CalendarView_DateChanged_Enhanced;

            int desiredW = Math.Max(300, _calendarView.MinimumSize.Width + 16);
            int desiredH = Math.Max(360, _calendarView.MinimumSize.Height + 16);

            _popup.Size = new Size(desiredW, desiredH);
            _popup.Controls.Add(_calendarView);
            _popup.ShowPopup(this, BeepPopupFormPosition.Bottom, desiredW, desiredH);
            _isPopupOpen = true;
        }

        // Fire selected change event
        protected virtual void OnSelectedDateTimeChanged(Models.DateTimePickerEventArgs args)
        {
            SelectedDateTimeChanged?.Invoke(this, args);
        }
        
        // Fire date range change event
        protected virtual void OnDateRangeChanged(DateRangeEventArgs args)
        {
            DateRangeChanged?.Invoke(this, args);
        }
        
        // Update mask pattern based on current mode
        private void UpdateMaskForMode()
        {
            switch (_mode)
            {
                case Models.DatePickerMode.Single:
                    MaskFormat = TextBoxMaskFormat.Date;
                    PlaceholderText = DateFormat.Replace("M", "M").Replace("d", "D").Replace("y", "Y");
                    break;
                    
                case Models.DatePickerMode.Range:
                    MaskFormat = TextBoxMaskFormat.Custom;
                    string dateMask = "00/00/0000";  // Default MM/dd/yyyy mask
                    CustomMask = $"{dateMask}{_dateSeparator}{dateMask}";
                    PlaceholderText = $"MM/DD/YYYY{_dateSeparator}MM/DD/YYYY";
                    break;
                    
                case Models.DatePickerMode.SingleWithTime:
                    MaskFormat = TextBoxMaskFormat.DateTime;
                    PlaceholderText = "MM/DD/YYYY HH:mm";
                    break;
                    
                case Models.DatePickerMode.RangeWithTime:
                    MaskFormat = TextBoxMaskFormat.Custom;
                    string dateTimeMask = "00/00/0000 00:00";
                    CustomMask = $"{dateTimeMask}{_dateSeparator}{dateTimeMask}";
                    PlaceholderText = $"MM/DD/YYYY HH:mm{_dateSeparator}MM/DD/YYYY HH:mm";
                    break;
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClosePopup();
                
                // Unsubscribe from base TextChanged
                base.TextChanged -= BeepDateDropDown_TextChanged;
            }
            base.Dispose(disposing);
        }
    }
    
    // Event args for date range selection
    public class DateRangeEventArgs : EventArgs
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        public DateRangeEventArgs(DateTime? start, DateTime? end)
        {
            StartDate = start;
            EndDate = end;
        }
    }
}
