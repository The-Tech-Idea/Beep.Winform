// DocumentHostCommandService.cs
// Concrete IDocumentHostCommandService that delegates every operation to a live
// BeepDocumentHost instance (Sprint 16.1).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.IO;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Default implementation of <see cref="IDocumentHostCommandService"/>.
    /// Wraps a <see cref="BeepDocumentHost"/> instance; all operations are
    /// dispatched to the UI thread via <c>BeginInvoke</c> when necessary.
    /// </summary>
    public sealed class DocumentHostCommandService : IDocumentHostCommandService
    {
        private readonly BeepDocumentHost _host;

        /// <summary>Creates a command service for the given <paramref name="host"/>.</summary>
        /// <exception cref="ArgumentNullException"/>
        public DocumentHostCommandService(BeepDocumentHost host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
        }

        // ── Open / close ──────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void NewDocument(string? documentId = null, string? title = null)
        {
            var id = documentId ?? Guid.NewGuid().ToString();
            _host.AddDocument(id, title ?? "New Document", iconPath: null, activate: true);
        }

        /// <inheritdoc/>
        public bool CloseDocument(string documentId)
            => _host.CloseDocument(documentId);

        /// <inheritdoc/>
        public bool CloseAllDocuments()
            => _host.CloseAllDocuments();

        // ── Activation ───────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void ActivateDocument(string documentId)
            => _host.SetActiveDocument(documentId);

        // ── Float / dock ─────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void FloatDocument(string documentId)
            => _host.FloatDocument(documentId);

        /// <inheritdoc/>
        public void DockBackDocument(string documentId)
            => _host.DockBackDocument(documentId);

        // ── Pin / unpin ───────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void PinDocument(string documentId)
            => _host.PinDocument(documentId, true);

        /// <inheritdoc/>
        public void UnpinDocument(string documentId)
            => _host.PinDocument(documentId, false);

        // ── Split ─────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public void SplitHorizontal(string documentId)
            => _host.SplitDocumentHorizontal(documentId);

        /// <inheritdoc/>
        public void SplitVertical(string documentId)
            => _host.SplitDocumentVertical(documentId);

        // ── Layout persistence ────────────────────────────────────────────────

        /// <inheritdoc/>
        public void SaveLayout(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath);
            File.WriteAllText(filePath, _host.SaveLayout());
        }

        /// <inheritdoc/>
        public async Task<bool> RestoreLayoutAsync(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath);
            if (!File.Exists(filePath)) return false;
            string json = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
            // Marshal back to the UI thread
            var tcs = new TaskCompletionSource<bool>();
            _host.BeginInvoke(new Action(() =>
            {
                try { tcs.SetResult(_host.RestoreLayout(json)); }
                catch (Exception ex) { tcs.SetException(ex); }
            }));
            return await tcs.Task.ConfigureAwait(false);
        }
    }
}
