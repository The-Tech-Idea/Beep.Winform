// BeepTabClosedRecord.cs
// Snapshot of a tab page that was closed; used by BeepTabs closed-tab history.

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Models
{
    /// <summary>
    /// Immutable snapshot captured when a tab is closed.
    /// The original <see cref="Page"/> object is kept alive so it can be
    /// re-inserted without recreating content.
    /// </summary>
    public sealed class BeepTabClosedRecord
    {
        /// <summary>The tab page whose content can be re-added.</summary>
        public BeepTabPage Page { get; }

        /// <summary>Display title at the time of closing.</summary>
        public string  Title      { get; }

        /// <summary>Zero-based position the tab occupied when it was closed.</summary>
        public int     LastIndex  { get; }

        /// <summary>Whether the tab was pinned when it was closed.</summary>
        public bool    WasPinned  { get; }

        internal BeepTabClosedRecord(BeepTabPage page, string title, int lastIndex, bool wasPinned)
        {
            Page      = page;
            Title     = title;
            LastIndex = lastIndex;
            WasPinned = wasPinned;
        }
    }

    /// <summary>
    /// Event arguments raised by <see cref="BeepTabs.TabReopenRequested"/> when
    /// the user asks to reopen a previously closed tab.
    /// </summary>
    public sealed class BeepTabReopenEventArgs : System.EventArgs
    {
        /// <summary>The record of the tab being reopened.</summary>
        public BeepTabClosedRecord Record { get; }

        internal BeepTabReopenEventArgs(BeepTabClosedRecord record)
        {
            Record = record;
        }
    }

    /// <summary>
    /// Cancellable event arguments raised by <see cref="BeepTabs.TabCloseRequested"/>
    /// before a dirty (unsaved) tab is removed. Set <see cref="System.ComponentModel.CancelEventArgs.Cancel"/>
    /// to <see langword="true"/> to abort the close.
    /// </summary>
    public sealed class BeepTabCloseRequestedEventArgs : System.ComponentModel.CancelEventArgs
    {
        /// <summary>The tab page that is about to be closed.</summary>
        public BeepTabPage Page { get; }

        /// <summary>Rich metadata for the tab, including dirty/pinned/preview state.</summary>
        public BeepTabItem Metadata { get; }

        internal BeepTabCloseRequestedEventArgs(BeepTabPage page, BeepTabItem metadata)
        {
            Page     = page;
            Metadata = metadata;
        }
    }
}
