// BeepDocumentHostOptions.cs
// Configuration options for BeepDocumentHost (Sprint 16.3).
// Used with BeepDocumentHost.Configure() and the DI registration helpers.
// ─────────────────────────────────────────────────────────────────────────────────────────

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Strongly-typed options applied to a <see cref="BeepDocumentHost"/> during
    /// initial configuration.
    /// <para>
    /// Obtain a populated instance through the fluent
    /// <see cref="BeepDocumentHost.Configure(System.Action{BeepDocumentHostOptions})"/>
    /// extension or the dependency-injection helper
    /// <c>services.AddBeepDocumentHost(options =&gt; { … })</c>.
    /// </para>
    /// </summary>
    public sealed class BeepDocumentHostOptions
    {
        // ── Appearance ────────────────────────────────────────────────────────

        /// <summary>Default visual style applied to every tab strip.  Defaults to <c>DocumentTabStyle.VSCode</c>.</summary>
        public DocumentTabStyle DefaultTabStyle { get; set; } = DocumentTabStyle.VSCode;

        /// <summary>Position of the tab strip relative to the document content area.</summary>
        public TabStripPosition TabPosition { get; set; } = TabStripPosition.Top;

        // ── Behaviour ─────────────────────────────────────────────────────────

        /// <summary>Maximum number of split groups that the user can create.  Range: 1–4.</summary>
        public int MaxGroups { get; set; } = 2;

        /// <summary>When <see langword="true"/> Ctrl+W, Ctrl+Tab and other host-level keyboard shortcuts are active.</summary>
        public bool KeyboardShortcutsEnabled { get; set; } = true;

        /// <summary>Controls when the close button is shown on a tab.</summary>
        public TabCloseMode CloseMode { get; set; } = TabCloseMode.OnHover;

        /// <summary>Show the (+) new-document button in the tab strip.</summary>
        public bool ShowAddButton { get; set; } = true;

        /// <summary>
        /// When <see langword="true"/> the layout is saved to <see cref="SessionFile"/>
        /// on application exit and restored on startup.
        /// </summary>
        public bool AutoSaveLayout { get; set; } = false;

        /// <summary>File path used when <see cref="AutoSaveLayout"/> is <see langword="true"/>.</summary>
        public string? SessionFile { get; set; }

        // ─────────────────────────────────────────────────────────────────────
        // Apply
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Writes all option values to <paramref name="host"/>.
        /// Called by <see cref="BeepDocumentHostExtensions.Configure"/> and
        /// <see cref="BeepDocumentHostExtensions.ApplyOptions"/>.
        /// </summary>
        public void ApplyTo(BeepDocumentHost host)
        {
            if (host == null) throw new System.ArgumentNullException(nameof(host));

            host.TabStyle                   = DefaultTabStyle;
            host.TabPosition                = TabPosition;
            host.MaxGroups                  = MaxGroups;
            host.KeyboardShortcutsEnabled   = KeyboardShortcutsEnabled;
            host.CloseMode                  = CloseMode;
            host.ShowAddButton              = ShowAddButton;
            host.AutoSaveLayout             = AutoSaveLayout;

            if (SessionFile != null)
                host.SessionFile = SessionFile;
        }
    }
}
