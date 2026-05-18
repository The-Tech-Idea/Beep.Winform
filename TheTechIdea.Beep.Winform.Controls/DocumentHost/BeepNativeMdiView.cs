// BeepNativeMdiView.cs
// IBeepDocumentManagerView implementation using standard WinForms native MDI.
// ─────────────────────────────────────────────────────────────────────────────────────────
// Usage:
//   var view = manager.ChangeView<BeepNativeMdiView>();
//   view.ParentForm = this;          // the form that owns the MDI client area
//
// Each AddDocument() call creates a new child Form with MdiParent set.
// Subscribe to DocumentFormCreated to customise the MDI child (set size, icon, etc.)
// or to add a content Control to it.
//
// Layout helpers (Cascade / Tile / ArrangeIcons) wrap Form.LayoutMdi(...).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    // ═══════════════════════════════════════════════════════════════════════════
    // Event args for MDI child form creation
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Event args raised by <see cref="BeepNativeMdiView.DocumentFormCreated"/> when
    /// a new MDI child <see cref="Form"/> has been constructed.
    /// </summary>
    public sealed class MdiDocumentEventArgs : EventArgs
    {
        /// <summary>The new MDI child form; its <c>MdiParent</c> is already set.</summary>
        public Form Form { get; }

        /// <summary>Document identifier used to track the window.</summary>
        public string Id { get; }

        /// <summary>Document title (also the initial <see cref="Form.Text"/>).</summary>
        public string Title { get; }

        internal MdiDocumentEventArgs(Form form, string id, string title)
        {
            Form  = form;
            Id    = id;
            Title = title;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // BeepNativeMdiView
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// <see cref="IBeepDocumentManagerView"/> that renders documents as standard
    /// WinForms MDI child windows inside an <c>IsMdiContainer</c> parent form.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Drop this component onto the form tray or call
    /// <c>manager.ChangeView&lt;BeepNativeMdiView&gt;()</c>, then set
    /// <see cref="ParentForm"/> to the form that should host the MDI client area.
    /// When <see cref="ParentForm"/> is assigned the view automatically sets
    /// <c>Form.IsMdiContainer = true</c>.
    /// </para>
    /// <para>
    /// Because MDI child windows are full Forms, <see cref="IBeepDocumentManagerView.AddDocument"/>
    /// returns <see langword="null"/>.  Subscribe to <see cref="DocumentFormCreated"/> to
    /// obtain the child <see cref="Form"/> and add content Controls to it.
    /// </para>
    /// <para>
    /// Use <see cref="Cascade"/>, <see cref="TileHorizontal"/>, <see cref="TileVertical"/>,
    /// and <see cref="ArrangeIcons"/> to control MDI child arrangement.
    /// </para>
    /// </remarks>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepNativeMdiView), "Resources.BeepNativeMdiView.bmp")]
    [DesignerCategory("Component")]
    [DefaultProperty(nameof(ParentForm))]
    [DefaultEvent(nameof(DocumentFormCreated))]
    [Description("IBeepDocumentManagerView implementation using standard WinForms native MDI.")]
    public sealed class BeepNativeMdiView : Component, IBeepDocumentManagerView
    {
        // ── Private state ─────────────────────────────────────────────────────

        private Form?                          _parentForm;
        private BeepDocumentManager?           _manager;
        private bool                           _disposed;
        private bool                           _autoSave;
        private string                         _sessionFile   = string.Empty;
        private int                            _untitledCount = 0;

        // tracks MDI child forms by document ID
        private readonly Dictionary<string, Form> _forms =
            new(StringComparer.OrdinalIgnoreCase);

        // menu items added to the Window menu (removed on detach)
        private MenuStrip?  _windowMenu;
        private string      _windowMenuText = "&Window";
        private ToolStripMenuItem? _windowMenuItem;

        // ── Constructors ──────────────────────────────────────────────────────

        public BeepNativeMdiView() { }

        /// <summary>Designer-friendly overload that adds this component to a container.</summary>
        public BeepNativeMdiView(IContainer container) { container?.Add(this); }

        // ══════════════════════════════════════════════════════════════════════
        // Properties
        // ══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// The <see cref="Form"/> that owns the MDI client area.  Setting this
        /// automatically sets <see cref="Form.IsMdiContainer"/> to <see langword="true"/>.
        /// </summary>
        [Category("Document Manager")]
        [Description("The parent MDI form.  IsMdiContainer is set automatically.")]
        [DefaultValue(null)]
        public Form? ParentForm
        {
            get => _parentForm;
            set
            {
                if (_parentForm == value) return;
                _parentForm = value;
                if (_parentForm != null && !_parentForm.IsMdiContainer)
                    _parentForm.IsMdiContainer = true;
            }
        }

        // ── MDI layout helpers ────────────────────────────────────────────────

        /// <summary>Cascades all MDI child windows.</summary>
        public void Cascade()        => _parentForm?.LayoutMdi(MdiLayout.Cascade);

        /// <summary>Tiles all MDI child windows horizontally.</summary>
        public void TileHorizontal() => _parentForm?.LayoutMdi(MdiLayout.TileHorizontal);

        /// <summary>Tiles all MDI child windows vertically.</summary>
        public void TileVertical()   => _parentForm?.LayoutMdi(MdiLayout.TileVertical);

        /// <summary>Arranges minimised MDI child icons.</summary>
        public void ArrangeIcons()   => _parentForm?.LayoutMdi(MdiLayout.ArrangeIcons);

        // ══════════════════════════════════════════════════════════════════════
        // IBeepDocumentManagerView — Lifecycle
        // ══════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        public void Attach(BeepDocumentManager manager)
        {
            _manager = manager;
        }

        /// <inheritdoc/>
        public void Detach()
        {
            DetachWindowMenu();
            _manager = null;
        }

        // ══════════════════════════════════════════════════════════════════════
        // IBeepDocumentManagerView — Document operations
        // ══════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        public BeepDocumentPanel? AddDocument(DocumentDescriptor desc)
        {
            if (desc == null) return null;
            return AddDocumentCore(desc.Id,
                                   desc.Title ?? string.Empty,
                                   desc.IconPath ?? string.Empty,
                                   true);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Creates a new MDI child <see cref="Form"/> and raises
        /// <see cref="DocumentFormCreated"/> so callers can add content.
        /// Always returns <see langword="null"/> — content is managed by the child Form.
        /// </remarks>
        public BeepDocumentPanel? AddDocument(string title, string iconPath, bool activate)
            => AddDocumentCore(GenerateDocumentId(title), title, iconPath, activate);

        private BeepDocumentPanel? AddDocumentCore(string id, string title, string iconPath, bool activate)
        {
            if (_parentForm == null) return null;
            if (string.IsNullOrWhiteSpace(id))
                id = GenerateDocumentId(title);

            if (_forms.TryGetValue(id, out var existing))
            {
                existing.Text = string.IsNullOrEmpty(title) ? existing.Text : title;
                if (activate) existing.Activate();
                return null;
            }

            var child = new Form
            {
                Text       = string.IsNullOrEmpty(title) ? GenerateUntitledTitle() : title,
                MdiParent  = _parentForm,
                WindowState = FormWindowState.Normal,
                StartPosition = FormStartPosition.WindowsDefaultBounds
            };

            // Apply icon from path if supplied (best-effort — file might not exist)
            if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
            {
                try { child.Icon = new System.Drawing.Icon(iconPath); }
                catch { /* non-fatal */ }
            }

            _forms[id] = child;

            child.FormClosed += (s, e) =>
            {
                _forms.Remove(id);
                RemoveWindowMenuItem(id, (Form?)s);
                DocumentRemoved?.Invoke(this,
                    new DocumentEventArgs(id, child.Text));
            };

            child.Activated += (s, e) =>
            {
                ActiveDocumentChanged?.Invoke(this,
                    new DocumentEventArgs(id, child.Text));
            };

            child.Show();
            if (activate) child.Activate();

            // Notify subscribers so they can add content controls
            DocumentFormCreated?.Invoke(this, new MdiDocumentEventArgs(child, id, child.Text));

            // Raise DocumentAdded (Panel is null — MDI uses Forms, not BeepDocumentPanels)
            DocumentAdded?.Invoke(this,
                new DocumentAddedEventArgs(new DocumentDescriptor { Id = id, Title = child.Text }, null));

            // Refresh window menu
            AddWindowMenuItem(id, child);

            return null; // MDI mode: content lives in the child Form, not a panel
        }

        /// <inheritdoc/>
        public bool RemoveDocument(string id)
        {
            if (_forms.TryGetValue(id, out var form))
            {
                form.Close();
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public void ActivateDocument(string id)
        {
            if (_forms.TryGetValue(id, out var form))
                form.Activate();
        }

        /// <inheritdoc/>
        public void BeginBatchAddDocuments() { /* No-op in MDI mode */ }

        /// <inheritdoc/>
        public void EndBatchAddDocuments()   { /* No-op in MDI mode */ }

        /// <inheritdoc/>
        public bool CloseAllDocuments()
        {
            // snapshot to avoid modifying collection during iteration
            var snapshot = new List<Form>(_forms.Values);
            foreach (var form in snapshot)
                try { form.Close(); } catch { }
            return true;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Native MDI mode does not use <see cref="BeepDocumentPanel"/> instances —
        /// documents are hosted in child <see cref="Form"/>s. Always returns
        /// <see langword="null"/>.
        /// </remarks>
        public BeepDocumentPanel? GetPanel(string documentId) => null;

        // ══════════════════════════════════════════════════════════════════════
        // IBeepDocumentManagerView — Layout persistence
        // ══════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        /// <remarks>Returns a JSON array describing each MDI child's bounds and title.</remarks>
        public string SaveLayout()
        {
            var entries = new List<MdiChildLayout>();
            foreach (var kvp in _forms)
            {
                var f = kvp.Value;
                entries.Add(new MdiChildLayout
                {
                    Id    = kvp.Key,
                    Title = f.Text,
                    X     = f.Location.X,
                    Y     = f.Location.Y,
                    W     = f.Width,
                    H     = f.Height,
                    State = f.WindowState.ToString()
                });
            }
            return JsonSerializer.Serialize(entries);
        }

        /// <inheritdoc/>
        public void SaveLayoutToFile(string? filePath)
        {
            var path = filePath ?? _sessionFile;
            if (string.IsNullOrEmpty(path)) return;
            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(path, SaveLayout(), Encoding.UTF8);
            }
            catch { /* non-fatal */ }
        }

        /// <inheritdoc/>
        /// <remarks>Restores MDI child positions; new child Forms must already exist.</remarks>
        public bool TryRestoreLayout(string json)
        {
            if (string.IsNullOrEmpty(json)) return false;
            try
            {
                var entries = JsonSerializer.Deserialize<List<MdiChildLayout>>(json);
                if (entries == null) return false;

                foreach (var entry in entries)
                {
                    if (!_forms.TryGetValue(entry.Id, out var form)) continue;
                    if (Enum.TryParse<FormWindowState>(entry.State, out var ws))
                        form.WindowState = ws;
                    if (ws == FormWindowState.Normal)
                        form.SetBounds(entry.X, entry.Y, entry.W, entry.H);
                }
                return true;
            }
            catch { return false; }
        }

        // ══════════════════════════════════════════════════════════════════════
        // IBeepDocumentManagerView — Settings push
        // ══════════════════════════════════════════════════════════════════════

        /// <inheritdoc/>
        /// <remarks>
        /// Float / pin / split policies are not applicable in native MDI mode;
        /// this method is a no-op.
        /// </remarks>
        public void PushPolicy(BeepDocumentPolicy policy) { /* N/A in MDI mode */ }

        /// <inheritdoc/>
        /// <remarks>Theme is not applied to MDI chrome in this release.</remarks>
        public void PushTheme(string themeName)           { /* future: tint MDI client area */ }

        /// <inheritdoc/>
        public void PushPersistence(bool autoSave, string sessionFile)
        {
            _autoSave    = autoSave;
            _sessionFile = sessionFile ?? string.Empty;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Creates (or replaces) a top-level <see cref="ToolStripMenuItem"/> in
        /// <paramref name="menu"/>.  Open documents are listed as sub-items and clicking
        /// one activates the corresponding MDI child.
        /// </remarks>
        public void AttachWindowMenu(MenuStrip menu, string menuText)
        {
            if (menu == null) return;
            DetachWindowMenu();

            _windowMenu     = menu;
            _windowMenuText = string.IsNullOrEmpty(menuText) ? "&Window" : menuText;
            _windowMenuItem = new ToolStripMenuItem(_windowMenuText);

            menu.Items.Add(_windowMenuItem);

            // Cascade / Tile / Arrange layout commands
            _windowMenuItem.DropDownItems.Add("Cascade",        null, (s, e) => Cascade());
            _windowMenuItem.DropDownItems.Add("Tile Horizontal",null, (s, e) => TileHorizontal());
            _windowMenuItem.DropDownItems.Add("Tile Vertical",  null, (s, e) => TileVertical());
            _windowMenuItem.DropDownItems.Add("Arrange Icons",  null, (s, e) => ArrangeIcons());
            _windowMenuItem.DropDownItems.Add(new ToolStripSeparator());

            // Re-add existing child entries
            foreach (var kvp in _forms)
                AddWindowMenuItemCore(kvp.Key, kvp.Value);
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

        // ── Extra MDI-specific events ─────────────────────────────────────────

        /// <summary>
        /// Fires immediately after a new MDI child <see cref="Form"/> is created.
        /// Use this event to add content <see cref="Control"/>s to the child Form.
        /// </summary>
        [Category("Document Manager")]
        [Description("Fires after a new MDI child Form is created. Add content controls here.")]
        public event EventHandler<MdiDocumentEventArgs>? DocumentFormCreated;

        // ══════════════════════════════════════════════════════════════════════
        // Private helpers
        // ══════════════════════════════════════════════════════════════════════

        private string GenerateDocumentId(string title)
        {
            var safe = string.IsNullOrWhiteSpace(title)
                ? "doc"
                : title.ToLowerInvariant().Replace(' ', '_');
            return $"{safe}_{Guid.NewGuid():N}";
        }

        private string GenerateUntitledTitle() => $"Document {++_untitledCount}";

        private void AddWindowMenuItem(string id, Form form)
        {
            if (_windowMenuItem == null) return;
            AddWindowMenuItemCore(id, form);
        }

        private void AddWindowMenuItemCore(string id, Form form)
        {
            if (_windowMenuItem == null) return;
            var item = new ToolStripMenuItem(form.Text) { Tag = id };
            item.Click += (s, e) => ActivateDocument(id);
            form.TextChanged += (s, e) =>
            {
                if (s is Form f) item.Text = f.Text;
            };
            _windowMenuItem.DropDownItems.Add(item);
        }

        private void RemoveWindowMenuItem(string id, Form? form)
        {
            if (_windowMenuItem == null) return;
            for (int i = _windowMenuItem.DropDownItems.Count - 1; i >= 0; i--)
            {
                if (_windowMenuItem.DropDownItems[i] is ToolStripMenuItem mi
                    && mi.Tag is string tagId
                    && tagId == id)
                {
                    _windowMenuItem.DropDownItems.RemoveAt(i);
                    break;
                }
            }
        }

        private void DetachWindowMenu()
        {
            if (_windowMenu == null || _windowMenuItem == null) return;
            try { _windowMenu.Items.Remove(_windowMenuItem); } catch { }
            _windowMenuItem.Dispose();
            _windowMenuItem = null;
            _windowMenu     = null;
        }

        // ══════════════════════════════════════════════════════════════════════
        // Layout persistence DTO
        // ══════════════════════════════════════════════════════════════════════

        private sealed class MdiChildLayout
        {
            public string Id    { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public int    X     { get; set; }
            public int    Y     { get; set; }
            public int    W     { get; set; }
            public int    H     { get; set; }
            public string State { get; set; } = "Normal";
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
                // Close open child forms
                var snapshot = new List<Form>(_forms.Values);
                foreach (var f in snapshot)
                    try { f.Close(); } catch { }
                _forms.Clear();
                _parentForm = null;
                _manager    = null;
            }
            base.Dispose(disposing);
        }
    }
}
