using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Calendar.Rendering;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        /// <summary>
        /// Gets or sets the visual style for the calendar
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
                    _stylePainter = CalendarPainterFactory.GetPainter(value);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to use the new painter system
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use the new style painter system for rendering")]
        [DefaultValue(true)]
        public bool UsePainterSystem
        {
            get => _usePainterSystem;
            set
            {
                if (_usePainterSystem != value)
                {
                    _usePainterSystem = value;
                    Invalidate();
                }
            }
        }
    }
}