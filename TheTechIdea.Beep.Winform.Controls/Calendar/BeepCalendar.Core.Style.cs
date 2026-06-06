using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Gets or sets the visual style for the calendar. The per-view
        /// <see cref="ICalendarViewPainter"/> reads this via
        /// <see cref="ViewPaintArgs.ControlStyle"/>; Material3 and Minimal
        /// are produced by switching on this value inside the painter body
        /// (there is no separate <c>ICalendarStylePainter</c> hierarchy).
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style for the calendar")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle CalendarStyle
        {
            get => _calendarStyle;
            set
            {
                if (_calendarStyle != value)
                {
                    _calendarStyle = value;
                    RequestRedraw();
                }
            }
        }
    }
}