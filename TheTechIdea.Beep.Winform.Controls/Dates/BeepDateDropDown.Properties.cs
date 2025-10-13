using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    public partial class BeepDateDropDown
    {
        [Browsable(true)]
        [Category("Date")]
        [Description("Selected date/time or null if not set (MinValue means empty).")]
        public DateTime SelectedDateTime
        {
            get => _selectedDateTime;
            set
            {
                if (_selectedDateTime != value)
                {
                    var old = _selectedDateTime;
                    _selectedDateTime = value;
                    Invalidate();
                    SelectedDateTimeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether drop down button is shown.")]
        public bool ShowDropDown { get => _showDropDown; set { _showDropDown = value; Invalidate(); } }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow empty date values.")]
        public bool AllowEmpty { get => _allowEmpty; set => _allowEmpty = value; }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Minimum allowed date for selection.")]
        public DateTime? MinDate { get => _minDate; set => _minDate = value; }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Maximum allowed date for selection.")]
        public DateTime? MaxDate { get => _maxDate; set => _maxDate = value; }
    }
}
