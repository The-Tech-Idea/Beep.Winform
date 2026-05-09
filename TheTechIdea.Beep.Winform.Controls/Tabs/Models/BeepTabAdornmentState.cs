namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Visual kind of badge rendered on a tab header.
    /// </summary>
    public enum BeepTabBadgeKind
    {
        /// <summary>No badge.</summary>
        None,
        /// <summary>Numeric count badge (e.g. "3").</summary>
        Count,
        /// <summary>Small filled dot – no text.</summary>
        Dot,
        /// <summary>Neutral status indicator.</summary>
        Status,
        /// <summary>Red error indicator.</summary>
        Error,
        /// <summary>Amber warning indicator.</summary>
        Warning,
        /// <summary>Green success indicator.</summary>
        Success,
        /// <summary>Blue info indicator.</summary>
        Info
    }

    /// <summary>
    /// Snapshot of the adornment inputs for one tab item, consumed by
    /// <see cref="BeepTabAdornmentLayoutHelper"/> and passed to painters.
    /// All properties are init-only so the snapshot is effectively immutable after creation.
    /// </summary>
    public sealed class BeepTabAdornmentState
    {
        public static BeepTabAdornmentState Empty { get; } = new BeepTabAdornmentState();

        /// <summary>Text shown inside the badge glyph.</summary>
        public string BadgeText { get; init; } = string.Empty;

        /// <summary>Visual kind of the badge.</summary>
        public BeepTabBadgeKind BadgeKind { get; init; } = BeepTabBadgeKind.None;

        /// <summary>True when the tab content has unsaved changes.</summary>
        public bool IsDirty { get; init; }

        /// <summary>True while an async operation is in progress for this tab.</summary>
        public bool IsBusy { get; init; }

        /// <summary>True when the tab item carries a non-empty icon path.</summary>
        public bool HasIcon { get; init; }

        /// <summary>True when the tab item carries non-empty secondary text.</summary>
        public bool HasSubText { get; init; }

        // ── Derived helpers ───────────────────────────────────────────────────

        /// <summary>Whether any badge element should be rendered.</summary>
        public bool HasBadge => BadgeKind != BeepTabBadgeKind.None;

        /// <summary>Whether any active adornment beyond icon/text should be rendered.</summary>
        public bool HasActiveAdornment => HasBadge || IsDirty || IsBusy;

        /// <summary>Whether any content beyond the title text should be rendered.</summary>
        public bool HasRichContent => HasIcon || HasSubText || HasActiveAdornment;
    }
}
