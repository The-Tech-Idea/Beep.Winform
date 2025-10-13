using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public partial class BeepDateDropDown : BaseControl
    {
        // Core fields (shared across partials)
        internal DateTime _selectedDateTime = DateTime.MinValue;
        internal bool _isPopupOpen = false;
        internal TheTechIdea.Beep.Winform.Controls.BeepDatePickerView _calendarView;
        internal BeepPopupForm _popup;
        internal Font _textFont;
        internal int _buttonWidth => ScaleValue(20);
        internal int _padding => ScaleValue(4);
        internal bool _showDropDown = true;
        // Use a simple image path and StyledImagePainter for rendering instead of BeepImage instance
        internal string _calendarIconPath;
        // read-only flag and validation message
        internal bool _readOnly = false;
        internal string _validationErrorMessage = string.Empty;

        // Editing + input
        internal string _inputText = string.Empty;
        internal bool _isEditing = false;
        internal bool _allowEmpty = true;
        internal DateTime? _minDate = null;
        internal DateTime? _maxDate = null;

        // Events
        public event EventHandler SelectedDateTimeChanged;
        public event EventHandler DropDownOpened;
        public event EventHandler DropDownClosed;

        public BeepDateDropDown()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            _textFont = this.Font;

            // store icon path (do not create UI object here)
            try
            {
                _calendarIconPath = TheTechIdea.Beep.Icons.Svgs.fi_tr_calendar;
            }
            catch { _calendarIconPath = string.Empty; }
        }

        protected override Size DefaultSize => new Size(180, 30);

        protected override void InitLayout()
        {
            base.InitLayout();
            if (_selectedDateTime == DateTime.MinValue)
                _selectedDateTime = DateTime.Now;
            UpdateMinimumSize();
            ApplyTheme();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            // Map calendar-related theme properties to control appearance where available
            try
            {
                if (_currentTheme != null)
                {
                    // Background / foreground / border
                    if (_currentTheme.CalendarBackColor != Color.Empty) BackColor = _currentTheme.CalendarBackColor;
                    if (_currentTheme.CalendarForeColor != Color.Empty) ForeColor = _currentTheme.CalendarForeColor;
                    if (_currentTheme.CalendarBorderColor != Color.Empty) BorderColor = _currentTheme.CalendarBorderColor;

                    // Fonts
                    try { _textFont = BeepThemesManager.ToFont(_currentTheme.DateFont); } catch { _textFont = this.Font; }

                    // Invalidate previous caches for this icon to ensure color/asset changes are respected
                    if (!string.IsNullOrEmpty(_calendarIconPath))
                    {
                        try { StyledImagePainter.InvalidateCaches(_calendarIconPath); } catch { }

                        // Pre-render in background to avoid UI jank
                        Color iconColor = _currentTheme.CalendarTitleForColor != Color.Empty ? _currentTheme.CalendarTitleForColor : ForeColor;
                        var sizes = new int[] { 8, 12, 14, 16, 18, 20, 24, 28, 32 };
                        Task.Run(() =>
                        {
                            foreach (var s in sizes)
                            {
                                try { StyledImagePainter.PreRenderTintedToCache(_calendarIconPath, iconColor, 1f, new Size(s, s)); } catch { }
                            }
                        });
                    }
                }
            }
            catch { }

            Invalidate();
        }

        // Helper validation methods
        private void ShowValidationError()
        {
            if (!string.IsNullOrEmpty(_validationErrorMessage))
            {
                try { SetValidationError(_validationErrorMessage); } catch { ToolTipText = _validationErrorMessage; }
            }
        }
        private void ClearValidationError()
        {
            try { SetValidationError(string.Empty); } catch { ToolTipText = string.Empty; }
            ApplyTheme();
        }

        // Toggle calls core popup operations
        internal void TogglePopup()
        {
            if (_isPopupOpen) ClosePopup(); else ShowPopup();
        }

        internal void ClosePopup()
        {
            if (!_isPopupOpen) return;
            if (_calendarView != null)
            {
                _calendarView.OkClicked -= CalendarView_OkClicked;
                _calendarView.CancelClicked -= CalendarView_CancelClicked;
            }
            _popup?.CloseCascade();
            _popup = null;
            _calendarView = null;
            _isPopupOpen = false;
            DropDownClosed?.Invoke(this, EventArgs.Empty);
        }

        internal void ShowPopup()
        {
            DropDownOpened?.Invoke(this, EventArgs.Empty);

            _popup = new    BeepPopupForm
            {
                BorderThickness = 1,
                BorderRadius = this.BorderRadius,
                Theme = Theme
            };

            _calendarView = new TheTechIdea.Beep.Winform.Controls.BeepDatePickerView { Dock = DockStyle.Fill, Theme = Theme };
            if (_selectedDateTime != DateTime.MinValue) _calendarView.SelectedDateTime = _selectedDateTime;

            // event handlers are implemented in Events partial
            _calendarView.OkClicked += CalendarView_OkClicked;
            _calendarView.CancelClicked += CalendarView_CancelClicked;

            int desiredW = Math.Max(300, _calendarView.MinimumSize.Width + 16);
            int desiredH = Math.Max(360, _calendarView.MinimumSize.Height + 16);

            _popup.Size = new Size(desiredW, desiredH);
            _popup.Controls.Add(_calendarView);
            _popup.ShowPopup(this, BeepPopupFormPosition.Bottom, desiredW, desiredH);
            _isPopupOpen = true;
        }

        // Fire selected change
        private void OnSelectedDateTimeChanged()
        {
            SelectedDateTimeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
