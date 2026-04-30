using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Selects the command target scope used by vNext command routing.
    /// </summary>
    public enum DocumentCommandTargetScope
    {
        ActiveDocument = 0,
        ActiveGroup = 1,
        Host = 2,
        Global = 3
    }

    /// <summary>
    /// Operation categories used for layout transaction and telemetry semantics.
    /// </summary>
    public enum DocumentHostOperationType
    {
        Unknown = 0,
        Dock = 1,
        Split = 2,
        Float = 3,
        Close = 4,
        RestoreLayout = 5,
        AutoHide = 6,
        Command = 7
    }

    /// <summary>
    /// Context object used when evaluating or executing routed commands.
    /// </summary>
    public sealed class DocumentCommandContext
    {
        public string? ActiveDocumentId { get; init; }
        public string? ActiveGroupId { get; init; }
        public bool IsFloatingDocument { get; init; }
        public IReadOnlyDictionary<string, object?> Metadata { get; init; } =
            new Dictionary<string, object?>(StringComparer.Ordinal);
    }

    /// <summary>
    /// Minimal telemetry event DTO for host-level instrumentation.
    /// </summary>
    public sealed class DocumentHostTelemetryEvent
    {
        public string CorrelationId { get; init; } = string.Empty;
        public DocumentHostOperationType OperationType { get; init; } = DocumentHostOperationType.Unknown;
        public string EventName { get; init; } = string.Empty;
        public string? DocumentId { get; init; }
        public bool Success { get; init; }
        public double DurationMs { get; init; }
        public string? Error { get; init; }
    }
}
