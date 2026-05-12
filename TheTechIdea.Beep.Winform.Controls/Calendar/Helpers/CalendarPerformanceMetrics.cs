using System;

namespace TheTechIdea.Beep.Winform.Controls.Calendar.Helpers
{
    public sealed class CalendarPerformanceMetrics
    {
        public bool Enabled { get; set; } = true;
        public double QueryWarningThresholdMs { get; set; } = 8d;
        public double PaintWarningThresholdMs { get; set; } = 16d;
        public double LastQueryDurationMs { get; private set; }
        public double LastPaintDurationMs { get; private set; }
        public double MaxQueryDurationMs { get; private set; }
        public double MaxPaintDurationMs { get; private set; }
        public int QuerySamples { get; private set; }
        public int PaintSamples { get; private set; }

        public bool IsQueryOverThreshold => LastQueryDurationMs > QueryWarningThresholdMs;
        public bool IsPaintOverThreshold => LastPaintDurationMs > PaintWarningThresholdMs;

        internal void RecordQuery(TimeSpan elapsed)
        {
            LastQueryDurationMs = elapsed.TotalMilliseconds;
            if (LastQueryDurationMs > MaxQueryDurationMs)
            {
                MaxQueryDurationMs = LastQueryDurationMs;
            }

            QuerySamples++;
        }

        internal void RecordPaint(TimeSpan elapsed)
        {
            LastPaintDurationMs = elapsed.TotalMilliseconds;
            if (LastPaintDurationMs > MaxPaintDurationMs)
            {
                MaxPaintDurationMs = LastPaintDurationMs;
            }

            PaintSamples++;
        }
    }
}