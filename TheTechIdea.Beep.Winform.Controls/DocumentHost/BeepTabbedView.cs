// BeepTabbedView.cs
// Default IBeepDocumentManagerView implementation — wraps a BeepDocumentHost
// and provides the familiar docked / tabbed document experience.
// ─────────────────────────────────────────────────────────────────────────────────────────
// Usage:
//   // Designer (component tray):
//   beepDocumentManager1.View = beepTabbedView1;
//   beepTabbedView1.Host = beepDocumentHost1;
//
//   // Code-behind:
//   var view = manager.ChangeView<BeepTabbedView>();
//   view.Host = beepDocumentHost1;
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Default <see cref="IBeepDocumentManagerView"/> that delegates all document
    /// operations to an associated <see cref="BeepDocumentHost"/> control.
    /// </summary>
    /// <remarks>
    /// Drag this component from the toolbox onto the form tray, set its
    /// <see cref="Host"/> property to the <see cref="BeepDocumentHost"/> on the form,
    /// then assign it to <see cref="BeepDocumentManager.View"/>.
    /// </remarks>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepTabbedView), "Resources.BeepTabbedView.bmp")]
    [DesignerCategory("Component")]
    [DefaultProperty(nameof(Host))]
    [Description("Default IBeepDocumentManagerView — delegates to a BeepDocumentHost.")]
    public sealed class BeepTabbedView : Component, IBeepDocumentManagerView
    {
        // ── Private state ─────────────────────────────────────────────────────

        private BeepDocumentHost?     _host;
        private BeepDocumentManager?  _manager;
        private bool                  _disposed;
        private MenuStrip?            _windowMenuOwner;
        private string                _windowMenuText = "&Window";

        // ── Constructors ──────────────────────────────────────────────────────

        public BeepTabbedView() { }

        /// <summary>Designer-friendly overload that adds this component to a container.</summary>
        public BeepTabbedView(IContainer container) { container?.Add(this); }

        // ══════════════════════════════════════════════════════════════════════
        // Properties
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// The <see cref="BeepDocumentHost"/> control that physically renders
        /// the document tabs.  May be set before or after assigning this view to
        /// <see cref="BeepDocumentManager.View"/>.
        /// </summary>
        [Category("Document Manager")]
        [Description("The BeepDocumentHost control that renders document tabs.")]
        [DefaultValue(null)]
        public BeepDocumentHost? Host
        {
            get => _host;
            set
            {
                if (_host == value) return;
                var manager = _manager;
                DetachWindowMenu();
                UnwireHost();
                manager?.OnAttachedViewHostChanging(this);
                _host = value;
                if (manager != null)
                {
                    WireHost();
                    ReattachWindowMenu();
                    manager.OnAttachedViewHostChanged(this);
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // IBeepDocumentManagerView — Lifecycle
        // ══════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        public void Attach(BeepDocumentManager manager)
        {
            _manager = manager;
            WireHost();
        }

        /// <inheritdoc/>
        public void Detach()
        {
            DetachWindowMenu();
            UnwireHost();
            _manager = null;
        }

        // ══════════════════════════════════════════════════════════════════════
        // IBeepDocumentManagerView — Document operations
        // ══════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        public BeepDocumentPanel? AddDocument(DocumentDescriptor desc)
        {
            if (desc == null) return null;
            if (_host == null) return null;

            _host.OpenOrActivate(desc, new DocumentOpenOptions { Activate = true });
            return _host.GetPanel(desc.Id);
        }

        /// <inheritdoc/>
        public BeepDocumentPanel? AddDocument(string title, string iconPath, bool activate)
        {
            if (_host == null) return null;

            var descriptor = DocumentDescriptor.Create(
                Guid.NewGuid().ToString(),
                title,
                iconPath);
            _host.OpenDocument(
                descriptor,
                new DocumentOpenOptions { Activate = activate });
            return _host.GetPanel(descriptor.Id);
        }

        /// <inheritdoc/>
        public bool RemoveDocument(string id, bool force = false)
        {
            if (_host == null || string.IsNullOrEmpty(id)) return false;
            if (!force)
                return _host.CloseDocument(id);

            var panel = _host.GetPanel(id);
            if (panel == null)
                return false;

            var previousCanClose = panel.CanClose;
            try
            {
                panel.CanClose = true;
                return _host.CloseDocument(id);
            }
            finally
            {
                if (_host.GetPanel(id) != null)
                    panel.CanClose = previousCanClose;
            }
        }

        /// <inheritdoc/>
        public bool DetachDocumentForTransfer(string id)
        {
            if (_host == null || string.IsNullOrEmpty(id))
                return false;

            return RemoveDocument(id, force: true);
        }

        /// <inheritdoc/>
        public void ActivateDocument(string id)
        {
            if (_host == null || string.IsNullOrEmpty(id)) return;
            _host.CommandService.ActivateDocument(id);
        }

        /// <inheritdoc/>
        public void BeginBatchAddDocuments() => _host?.BeginBatchAddDocuments();

        /// <inheritdoc/>
        public void EndBatchAddDocuments() => _host?.EndBatchAddDocuments();

        /// <inheritdoc/>
        public int DocumentCount => _host?.DocumentCount ?? 0;

        /// <inheritdoc/>
        public DocumentEventArgs? ActiveDocument
        {
            get
            {
                if (_host?.ActiveDocumentId is not string documentId || string.IsNullOrEmpty(documentId))
                    return null;

                var panel = _host.ActivePanel;
                var title = panel != null && !string.IsNullOrEmpty(panel.DocumentTitle)
                    ? panel.DocumentTitle
                    : documentId;

                return new DocumentEventArgs(documentId, title);
            }
        }

        /// <inheritdoc/>
        public bool CloseAllDocuments() => _host?.CloseAllDocuments() ?? false;

        /// <inheritdoc/>
        public BeepDocumentPanel? GetPanel(string documentId)
            => _host?.GetPanel(documentId);

        // ══════════════════════════════════════════════════════════════════════
        // IBeepDocumentManagerView — Layout persistence
        // ══════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        public string SaveLayout()
            => _host?.SaveLayout() ?? string.Empty;

        /// <inheritdoc/>
        public void SaveLayoutToFile(string? filePath)
            => _host?.SaveLayoutToFile(filePath);

        /// <inheritdoc/>
        public bool TryRestoreLayout(string json)
        {
            if (_host == null || string.IsNullOrEmpty(json)) return false;
            try
            {
                _host.TryRestoreLayout(json, out var report);
                return report?.IsSuccess ?? false;
            }
            catch { return false; }
        }

        // ══════════════════════════════════════════════════════════════════════
        // IBeepDocumentManagerView — Settings push
        // ══════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        public void PushPolicy(BeepDocumentPolicy policy)
        {
            if (_host == null || policy == null) return;
            _host.AllowFloat    = policy.AllowFloat;
            _host.AllowPin      = policy.AllowPin;
            _host.AllowSplit    = policy.AllowSplit;
            _host.MaxSplitDepth = policy.MaxSplitDepth;
        }

        /// <inheritdoc/>
        public void PushTheme(string themeName)
        {
            if (_host != null && !string.IsNullOrEmpty(themeName))
                _host.ThemeName = themeName;
        }

        /// <inheritdoc/>
        public void PushPersistence(bool autoSave, string sessionFile)
        {
            if (_host == null) return;
            _host.AutoSaveLayout = autoSave;
            _host.SessionFile = sessionFile ?? string.Empty;
        }

        /// <inheritdoc/>
        public void AttachWindowMenu(MenuStrip menu, string menuText)
        {
            if (_host == null || menu == null) return;
            _windowMenuOwner = menu;
            _windowMenuText  = string.IsNullOrEmpty(menuText) ? "&Window" : menuText;
            _host.AttachWindowMenu(menu, menuText);
        }

        /// <inheritdoc/>
        public void DetachWindowMenu()
        {
            if (_host == null || _windowMenuOwner == null) return;

            _host.DetachWindowMenu(_windowMenuOwner, _windowMenuText);
            _windowMenuOwner = null;
        }

        // ══════════════════════════════════════════════════════════════════════
        // IBeepDocumentManagerView — Events
        // ══════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        public event EventHandler<DocumentAddedEventArgs>? DocumentAdded;

        /// <inheritdoc/>
        public event EventHandler<DocumentEventArgs>?      DocumentRemoved;

        /// <inheritdoc/>
        public event EventHandler<DocumentEventArgs>?      ActiveDocumentChanged;

        /// <inheritdoc/>
        public event EventHandler<TabClosingEventArgs>?    DocumentClosing;

        // ══════════════════════════════════════════════════════════════════════
        // Private helpers — host event bridge
        // ══════════════════════════════════════════════════════════════════════

        private void WireHost()
        {
            if (_host == null) return;
            _host.DocumentAdded         += OnHostDocumentAdded;
            _host.DocumentClosed        += OnHostDocumentClosed;
            _host.ActiveDocumentChanged += OnHostActiveDocumentChanged;
            _host.DocumentClosing       += OnHostDocumentClosing;
        }

        private void UnwireHost()
        {
            if (_host == null) return;
            try { _host.DocumentAdded         -= OnHostDocumentAdded;         } catch { }
            try { _host.DocumentClosed        -= OnHostDocumentClosed;        } catch { }
            try { _host.ActiveDocumentChanged -= OnHostActiveDocumentChanged; } catch { }
            try { _host.DocumentClosing       -= OnHostDocumentClosing;       } catch { }
        }

        private void OnHostDocumentAdded(object? s, DocumentAddedEventArgs e)
            => DocumentAdded?.Invoke(this, e);

        private void OnHostDocumentClosed(object? s, DocumentEventArgs e)
            => DocumentRemoved?.Invoke(this, e);

        private void OnHostActiveDocumentChanged(object? s, DocumentEventArgs e)
            => ActiveDocumentChanged?.Invoke(this, e);

        private void OnHostDocumentClosing(object? s, TabClosingEventArgs e)
            => DocumentClosing?.Invoke(this, e);

        private void ReattachWindowMenu()
        {
            if (_host == null || _windowMenuOwner == null) return;
            _host.AttachWindowMenu(_windowMenuOwner, _windowMenuText);
        }

        // ══════════════════════════════════════════════════════════════════════
        // Dispose
        // ══════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                DetachWindowMenu();
                UnwireHost();
                _host    = null;
                _manager = null;
            }
            base.Dispose(disposing);
        }
    }
}
