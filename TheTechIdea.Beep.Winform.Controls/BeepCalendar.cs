using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Calendar")]
    [Description("A button control that displays a custom calendar popup for date and time selection.")]
    public class BeepCalendar : BeepButton
    {
        #region Fields
        private DateTime? _selectedDateTime;
        private BeepPopupForm _calendarPopup;
        private BeepCalendarView _calendarView;
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
                UpdateButtonText();
                OnSelectedDateTimeChanged();
                Invalidate();
            }
        }

        [Browsable(false)]
        public  bool PopupMode
        {
            get => base.PopupMode;
            set => base.PopupMode = false; // Disable popup mode as we handle it manually
        }
        #endregion

        #region Events
        public event EventHandler<DateTime?> SelectedDateTimeChanged;

        protected virtual void OnSelectedDateTimeChanged()
        {
            SelectedDateTimeChanged?.Invoke(this, SelectedDateTime);
        }
        #endregion

        #region Constructor
        public BeepCalendar()
        {
            InitializeCalendar();
        }

        private void InitializeCalendar()
        {
            // Set default button properties
            Size = new Size(240, 30); // Match React example width
            TextAlign = ContentAlignment.MiddleLeft;
            ImageAlign = ContentAlignment.MiddleLeft;
            TextImageRelation = TextImageRelation.ImageBeforeText;
            MaxImageSize = new Size(16, 16); // Small icon size
            ImagePath = "calendar.svg"; // Assume an embedded SVG resource
            ApplyThemeOnImage = true;
            UpdateButtonText();

            // Prevent default popup behavior
            base.PopupMode = false;
        }
        #endregion

        #region Methods
        private void UpdateButtonText()
        {
            Text = _selectedDateTime.HasValue
                ? _selectedDateTime.Value.ToString("MMMM dd, yyyy h:mm tt")
                : "Pick a date and time";
        }

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
                Size = new Size(300, 300), // Adjust size to fit BeepCalendarView
                Theme = Theme // Inherit theme from BeepButton
            };

            // Create custom calendar control
            _calendarView = new BeepCalendarView
            {
                Dock = DockStyle.Fill,
                Theme = Theme
            };

            if (_selectedDateTime.HasValue)
            {
                _calendarView.SelectedDateTime = _selectedDateTime.Value;
            }

            _calendarView.DateTimeSelected += CalendarView_DateTimeSelected;
            _calendarPopup.Controls.Add(_calendarView);

            // Show popup below the button
            _calendarPopup.ShowPopup(this, BeepPopupFormPosition.Bottom);
        }

        private void CalendarView_DateTimeSelected(object sender, DateTime? dateTime)
        {
            SelectedDateTime = dateTime;
            _calendarPopup?.CloseCascade();
        }
        #endregion

        #region Overrides
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                ShowCalendarPopup();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _calendarPopup?.Dispose();
                _calendarView?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}