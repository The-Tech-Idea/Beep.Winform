namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// What a <see cref="BeepTabMode"/> allows. This is the one place the mode contract is defined.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The mode used to be re-decided at 20 call sites across 11 files, every one of them spelled
    /// <c>TabMode == BeepTabMode.Navigation</c> or its negation. Nothing named what any individual
    /// check was actually protecting, so the contract could only be reconstructed by reading all
    /// twenty. Each capability below replaces a group of those guards with a name that says what it
    /// governs.
    /// </para>
    /// <para>
    /// <b>Documents and Workspace are currently identical.</b> Every one of those twenty checks
    /// tested Navigation against not-Navigation, so the enum declares three modes and the control
    /// implements two. That is recorded rather than silently collapsed: which behaviours ought to
    /// differ — preview tabs, pinning, MRU, split groups — is a product decision, not something to
    /// infer from the absence of code. Until it is decided, this type documents the truth instead of
    /// spreading the ambiguity back across the call sites.
    /// </para>
    /// </remarks>
    public readonly struct BeepTabModeCapabilities
    {
        private readonly BeepTabMode _mode;

        private BeepTabModeCapabilities(BeepTabMode mode) => _mode = mode;

        public static BeepTabModeCapabilities For(BeepTabMode mode) => new BeepTabModeCapabilities(mode);

        /// <summary>The mode these capabilities were resolved from.</summary>
        public BeepTabMode Mode => _mode;

        /// <summary>
        /// Document-like behaviour: tabs represent open documents rather than fixed destinations.
        /// Everything below is currently equivalent to this; they are separate members because they
        /// guard genuinely different features and will not necessarily move together.
        /// </summary>
        public bool IsDocumentLike => _mode != BeepTabMode.Navigation;

        /// <summary>Tabs can be pinned, and pinned tabs resist closing and reordering.</summary>
        public bool SupportsPinning => IsDocumentLike;

        /// <summary>Ctrl+Tab walks most-recently-used order rather than positional order.</summary>
        public bool SupportsMruOrdering => IsDocumentLike;

        /// <summary>Closed tabs are remembered and can be reopened (Ctrl+Shift+T).</summary>
        public bool SupportsClosedTabHistory => IsDocumentLike;

        /// <summary>At most one preview tab is open, and opening another replaces it.</summary>
        public bool SupportsPreviewTabs => IsDocumentLike;

        /// <summary>Closing a dirty tab raises a cancellable event so the host can prompt to save.</summary>
        public bool SupportsDirtyCloseGuard => IsDocumentLike;

        /// <summary>Tabs can be dragged to reorder.</summary>
        public bool SupportsDragReorder => IsDocumentLike;

        /// <summary>The header offers a per-tab context menu (close others, pin, …).</summary>
        public bool SupportsTabContextMenu => IsDocumentLike;
    }
}
