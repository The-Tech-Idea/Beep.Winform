// IDocumentViewModel.cs
// Interface that MVVM view-models must implement to participate in the
// BeepDocumentHost.DocumentSource binding pipeline (Feature 6.4).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Marker interface for documents that participate in MVVM-style data binding
    /// with <see cref="BeepDocumentHost"/>.
    ///
    /// Implement this on your own view-model class and expose it through
    /// <see cref="BeepDocumentHost.DocumentSource"/> together with a
    /// <see cref="BeepDocumentHost.DocumentContentTemplate"/> factory.
    ///
    /// The host automatically reflects Id, Title, IsModified, IsPinned, CanClose,
    /// IconPath and BadgeText changes from the view-model to the matching tab in
    /// real time, as long as the view-model also implements
    /// <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public interface IDocumentViewModel : INotifyPropertyChanged
    {
        // ── Identity ─────────────────────────────────────────────────────────

        /// <summary>
        /// Globally-unique identifier for this document.
        /// Must be stable for the lifetime of the view-model.
        /// </summary>
        string Id { get; }

        /// <summary>Text shown in the tab header.</summary>
        string Title { get; }

        // ── Visual state ─────────────────────────────────────────────────────

        /// <summary>
        /// When true the tab shows a "●" dirty indicator.
        /// </summary>
        bool IsModified { get; }

        /// <summary>
        /// When true the tab is pinned (icon-only, left-anchored; cannot be closed).
        /// </summary>
        bool IsPinned { get; }

        /// <summary>
        /// When false the close (×) button is hidden.
        /// </summary>
        bool CanClose { get; }

        /// <summary>
        /// Optional path to an icon image drawn into the tab.
        /// The same path convention as <c>BeepButton.ImagePath</c>.
        /// Null means no icon.
        /// </summary>
        string? IconPath { get; }

        /// <summary>
        /// Optional tooltip text.  Falls back to <see cref="Title"/> when null.
        /// </summary>
        string? TooltipText { get; }

        /// <summary>
        /// Optional badge text shown as a coloured pill on the tab
        /// (e.g. "3" for unread messages, "!" for error).
        /// Null / empty → no badge.
        /// </summary>
        string? BadgeText { get; }
    }
}
