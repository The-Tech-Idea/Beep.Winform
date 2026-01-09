using System;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Calendar event
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CalendarEvent
    {
        [Category("Data")]
        [Description("Unique identifier for the event")]
        public string Id { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Event title")]
        public string Title { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Event start time")]
        public DateTime StartTime { get; set; } = DateTime.Now;

        [Category("Data")]
        [Description("Event end time")]
        public DateTime EndTime { get; set; } = DateTime.Now.AddHours(1);

        [Category("Appearance")]
        [Description("Color for the event display")]
        public Color Color { get; set; } = Color.Blue;

        [Category("Behavior")]
        [Description("Whether this is an all-day event")]
        public bool IsAllDay { get; set; } = false;

        [Category("Data")]
        [Description("Event description or notes")]
        public string Description { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Event location")]
        public string Location { get; set; } = string.Empty;

        public override string ToString() => Title;
    }
}
