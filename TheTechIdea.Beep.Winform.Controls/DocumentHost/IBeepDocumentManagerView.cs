// IBeepDocumentManagerView.cs
// View abstraction layer for BeepDocumentManager (Phase 03 — View Modes).
// ─────────────────────────────────────────────────────────────────────────────────────────
// Implementations:
//   BeepTabbedView    — default; delegates to a BeepDocumentHost (tabbed docking).
//   BeepNativeMdiView — standard WinForms MDI (IsMdiContainer / MdiParent).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Contract between <see cref="BeepDocumentManager"/> and a concrete rendering
    /// strategy for documents (tabbed-docking, native MDI, etc.).
    /// </summary>
    /// <remarks>
    /// The manager calls <see cref="Attach"/> immediately after this view is assigned,
    /// and calls <see cref="Detach"/> before replacing or disposing it.
    /// All document operations are routed through this interface when
    /// <see cref="BeepDocumentManager.View"/> is non-null.
    /// </remarks>
    public interface IBeepDocumentManagerView : IDisposable
    {
        // ── Lifecycle ─────────────────────────────────────────────────────────

        /// <summary>
        /// Called by <see cref="BeepDocumentManager"/> immediately after this view is
        /// assigned to the manager.  Implementations should store the reference and
        /// perform one-time wiring here.
        /// </summary>
        void Attach(BeepDocumentManager manager);

        /// <summary>
        /// Called by <see cref="BeepDocumentManager"/> before this view is replaced or
        /// disposed.  Implementations should unhook events and release the manager reference.
        /// </summary>
        void Detach();

        // ── Document operations ───────────────────────────────────────────────

        /// <summary>
        /// Opens a document described by <paramref name="desc"/> and returns the panel
        /// that hosts its content, or <see langword="null"/> if the view manages content
        /// in its own way (e.g. native MDI).
        /// </summary>
        BeepDocumentPanel? AddDocument(DocumentDescriptor desc);

        /// <summary>Opens a document by title / icon path.</summary>
        /// <param name="title">Text shown on the tab or title-bar.</param>
        /// <param name="iconPath">Beep image path for the tab icon; empty = no icon.</param>
        /// <param name="activate">
        /// <see langword="true"/> (default) to bring the new document to the foreground.
        /// </param>
        /// <returns>
        /// The <see cref="BeepDocumentPanel"/> created, or <see langword="null"/> when the
        /// view does not use panels (e.g. <see cref="BeepNativeMdiView"/>).
        /// </returns>
        BeepDocumentPanel? AddDocument(string title, string iconPath, bool activate);

        /// <summary>Closes the document with the given <paramref name="id"/>.</summary>
        /// <param name="force">
        /// <see langword="true"/> to bypass per-document close restrictions when the
        /// concrete view supports it.
        /// </param>
        /// <returns><see langword="true"/> if the document was found and closed.</returns>
        bool RemoveDocument(string id, bool force = false);

        /// <summary>
        /// Detaches a document as part of an internal manager view transfer without
        /// treating the operation as a user-initiated close.
        /// </summary>
        /// <returns><see langword="true"/> if the document was found and detached.</returns>
        bool DetachDocumentForTransfer(string id);

        /// <summary>Brings the document with the given <paramref name="id"/> to the foreground.</summary>
        void ActivateDocument(string id);

        /// <summary>
        /// Suspends layout while a batch of documents is being added; call
        /// <see cref="EndBatchAddDocuments"/> to flush.
        /// </summary>
        void BeginBatchAddDocuments();

        /// <summary>Ends the batch and performs a single layout pass.</summary>
        void EndBatchAddDocuments();

        /// <summary>Number of open documents currently tracked by this view.</summary>
        int DocumentCount { get; }

        /// <summary>The currently active document, or <see langword="null"/> when none is active.</summary>
        DocumentEventArgs? ActiveDocument { get; }

        /// <summary>Closes all open documents that are allowed to close.</summary>
        /// <returns><see langword="true"/> if at least one document was closed.</returns>
        bool CloseAllDocuments();

        /// <summary>
        /// Returns the <see cref="BeepDocumentPanel"/> for the given document id,
        /// or <see langword="null"/> if the view does not use panels (e.g. native MDI)
        /// or the id is not found.
        /// </summary>
        BeepDocumentPanel? GetPanel(string documentId);

        // ── Layout persistence ────────────────────────────────────────────────

        /// <summary>Returns a JSON snapshot of the current layout.</summary>
        string SaveLayout();

        /// <summary>
        /// Saves the layout JSON to <paramref name="filePath"/>;
        /// <see langword="null"/> uses the view's default session path.
        /// </summary>
        void SaveLayoutToFile(string? filePath);

        /// <summary>
        /// Attempts to restore a previously saved layout JSON.
        /// </summary>
        /// <returns><see langword="true"/> if the restore completed without failures.</returns>
        bool TryRestoreLayout(string json);

        // ── Settings push ─────────────────────────────────────────────────────

        /// <summary>Applies document float / pin / split policy.</summary>
        void PushPolicy(BeepDocumentPolicy policy);

        /// <summary>Propagates the Beep theme name to all rendered surfaces.</summary>
        void PushTheme(string themeName);

        /// <summary>Applies layout persistence settings.</summary>
        /// <param name="autoSave">Whether to auto-save on host form close.</param>
        /// <param name="sessionFile">Expanded file path for session JSON.</param>
        void PushPersistence(bool autoSave, string sessionFile);

        /// <summary>
        /// Attaches a Window-list sub-menu to <paramref name="menu"/> so open documents
        /// appear in the menu and can be activated from it.
        /// </summary>
        void AttachWindowMenu(MenuStrip menu, string menuText);

        /// <summary>Removes any Window-menu wiring previously attached by this view.</summary>
        void DetachWindowMenu();

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Fires after a document is added.</summary>
        event EventHandler<DocumentAddedEventArgs>? DocumentAdded;

        /// <summary>Fires after a document is removed / closed.</summary>
        event EventHandler<DocumentEventArgs>?      DocumentRemoved;

        /// <summary>Fires when the active (foreground) document changes.</summary>
        event EventHandler<DocumentEventArgs>?      ActiveDocumentChanged;

        /// <summary>
        /// Fires before a document is closed.  Set
        /// <see cref="TabClosingEventArgs.Cancel"/> to <see langword="true"/>
        /// to prevent the close.
        /// </summary>
        event EventHandler<TabClosingEventArgs>?    DocumentClosing;
    }
}
