using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar
{
    public partial class BeepCalendar
    {
        private void ConfigureEventServiceTelemetry()
        {
            if (_eventService != null)
            {
                _eventService.QueryTimingSink = RecordQueryDuration;
            }
        }

        private void RecordQueryDuration(TimeSpan elapsed)
        {
            if (PerformanceMetrics.Enabled)
            {
                PerformanceMetrics.RecordQuery(elapsed);
            }
        }
    }
}