// BeepCommandEntry.cs
// A single command entry stored in BeepCommandRegistry.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Describes a single executable command registered in <see cref="BeepCommandRegistry"/>.
    /// </summary>
    public sealed class BeepCommandEntry
    {
        /// <summary>Unique string identifier, e.g. <c>"document.close"</c>.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Human-readable display name shown in the command palette.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Category grouping, e.g. "Documents", "Navigation", "View".</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Optional path or name of an icon glyph or image resource.</summary>
        public string? IconPath { get; set; }

        /// <summary>Human-readable keyboard shortcut description, e.g. <c>"Ctrl+W"</c>.</summary>
        public string? Shortcut { get; set; }

        /// <summary>Action executed when the command is invoked.</summary>
        public Action Execute { get; set; } = static () => { };

        /// <summary>
        /// Optional predicate.  When non-null the command is only invocable when this
        /// returns <see langword="true"/>.
        /// </summary>
        public Func<bool>? CanExecute { get; set; }

        /// <summary>Number of times this command has been executed (used for MRU ranking).</summary>
        public int UsageCount { get; set; }

        /// <summary>UTC timestamp of the last execution, or null if never run.</summary>
        public DateTime? LastUsed { get; set; }
    }
}
