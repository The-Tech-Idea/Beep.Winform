using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Search
{
    public interface IRibbonSearchTelemetry
    {
        void OnSearchExecuted(RibbonSearchTelemetryEvent payload);
    }

    public sealed class RibbonSearchTelemetryEvent
    {
        public string Query { get; set; } = string.Empty;
        public RibbonSearchMode Mode { get; set; } = RibbonSearchMode.Off;
        public int ResultCount { get; set; }
        public bool ProviderUsed { get; set; }
        public bool ProviderFailed { get; set; }
        public bool UsedLocalFallback { get; set; }
        public long DurationMs { get; set; }
        public DateTime ExecutedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
