namespace TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models
{
    public sealed class BeepFormsViewState
    {
        public bool IsDirty { get; set; }
        public bool IsQueryMode { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public string CoordinationText { get; set; } = string.Empty;
        public string SavepointText { get; set; } = string.Empty;
        public string AlertText { get; set; } = string.Empty;
        public string CurrentMessage { get; set; } = string.Empty;
        public BeepFormsMessageSeverity CoordinationSeverity { get; set; }
        public BeepFormsMessageSeverity SavepointSeverity { get; set; }
        public BeepFormsMessageSeverity AlertSeverity { get; set; }
        public BeepFormsMessageSeverity MessageSeverity { get; set; }
        public string? ActiveBlockName { get; set; }

        // Phase 7D — bootstrap progress surfaced to status strip
        public BootstrapState BootstrapState { get; set; } = BootstrapState.Idle;
    }
}