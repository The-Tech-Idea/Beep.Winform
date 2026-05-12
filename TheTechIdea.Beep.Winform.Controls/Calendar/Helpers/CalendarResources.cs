using System.Collections.Generic;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    public sealed class CalendarResource
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Color Color { get; set; } = Color.Empty;
        public bool IsVisible { get; set; } = true;
        public int SortOrder { get; set; }

        public override string ToString() => string.IsNullOrWhiteSpace(Name) ? Id : Name;
    }

    public static class CalendarResourceHelper
    {
        public static IReadOnlyList<string> GetEventResourceIds(CalendarEvent calendarEvent)
        {
            if (calendarEvent == null)
            {
                return new List<string>();
            }

            var resourceIds = new List<string>();
            if (!string.IsNullOrWhiteSpace(calendarEvent.ResourceId))
            {
                resourceIds.Add(calendarEvent.ResourceId);
            }

            if (calendarEvent.ResourceIds != null)
            {
                foreach (var id in calendarEvent.ResourceIds)
                {
                    if (!string.IsNullOrWhiteSpace(id) && !resourceIds.Contains(id))
                    {
                        resourceIds.Add(id);
                    }
                }
            }

            return resourceIds;
        }
    }
}