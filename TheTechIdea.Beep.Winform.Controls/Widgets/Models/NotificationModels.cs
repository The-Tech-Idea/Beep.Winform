using System;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Widgets;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Models
{
    /// <summary>
    /// Notification message for notification widgets
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NotificationMessage
    {
        [Category("Data")]
        [Description("Unique identifier for the notification")]
        public string Id { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Title of the notification")]
        public string Title { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Message content")]
        public string Message { get; set; } = string.Empty;

        [Category("Data")]
        [Description("Type of notification")]
        public NotificationType Type { get; set; } = NotificationType.Info;

        [Category("Data")]
        [Description("Timestamp when the notification was created")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Category("Behavior")]
        [Description("Whether the notification has been read")]
        public bool IsRead { get; set; } = false;

        [Category("Data")]
        [Description("Optional action text")]
        public string ActionText { get; set; } = string.Empty;

        [Category("Appearance")]
        [Description("Path to icon image")]
        public string IconPath { get; set; } = string.Empty;

        public override string ToString() => Title;
    }
}
