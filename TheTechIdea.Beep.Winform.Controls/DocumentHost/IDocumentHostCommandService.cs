// IDocumentHostCommandService.cs
// First-class command-service interface for BeepDocumentHost (Sprint 16.1).
// Exposes all document operations as a mockable service contract — ideal for MVVM
// command wiring, unit testing, and dependency injection.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Abstraction over all document-management operations on a
    /// <see cref="BeepDocumentHost"/>.  Obtain a live instance via
    /// <see cref="BeepDocumentHost.CommandService"/> or create a
    /// <see cref="DocumentHostCommandService"/> wrapper around an existing host.
    /// </summary>
    public interface IDocumentHostCommandService
    {
        // ── Open / close ──────────────────────────────────────────────────────

        /// <summary>
        /// Opens a new document tab.  If <paramref name="documentId"/> is null an
        /// auto-generated GUID is used.  Raises <c>NewDocumentRequested</c> on the host.
        /// </summary>
        void NewDocument(string? documentId = null, string? title = null);

        /// <summary>Closes the document with the specified id.</summary>
        /// <returns><see langword="true"/> if the document was found and closed.</returns>
        bool CloseDocument(string documentId);

        /// <summary>Closes every currently open document.</summary>
        /// <returns><see langword="true"/> if at least one document was closed.</returns>
        bool CloseAllDocuments();

        // ── Activation ───────────────────────────────────────────────────────

        /// <summary>Brings the document with <paramref name="documentId"/> to the front.</summary>
        void ActivateDocument(string documentId);

        // ── Float / dock ─────────────────────────────────────────────────────

        /// <summary>Detaches the document into a floating window.</summary>
        void FloatDocument(string documentId);

        /// <summary>Re-docks a previously floated document.</summary>
        void DockBackDocument(string documentId);

        // ── Pin / unpin ───────────────────────────────────────────────────────

        /// <summary>Pins the document tab (icon-only, left-anchored).</summary>
        void PinDocument(string documentId);

        /// <summary>Unpins the document tab.</summary>
        void UnpinDocument(string documentId);

        // ── Split ─────────────────────────────────────────────────────────────

        /// <summary>Splits the view horizontally, placing <paramref name="documentId"/> in the new right pane.</summary>
        void SplitHorizontal(string documentId);

        /// <summary>Splits the view vertically, placing <paramref name="documentId"/> in the new bottom pane.</summary>
        void SplitVertical(string documentId);

        // ── Layout persistence ────────────────────────────────────────────────

        /// <summary>Serialises the current layout and writes it to <paramref name="filePath"/>.</summary>
        void SaveLayout(string filePath);

        /// <summary>Reads <paramref name="filePath"/> and asynchronously restores the layout.</summary>
        /// <returns><see langword="true"/> when the layout was restored successfully.</returns>
        Task<bool> RestoreLayoutAsync(string filePath);
    }
}
