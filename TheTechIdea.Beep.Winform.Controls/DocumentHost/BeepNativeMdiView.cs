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
        private string?                        _activeDocumentId;

        // tracks MDI child forms by document ID
        private readonly Dictionary<string, Form> _forms =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, DocumentDescriptor> _descriptors =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _suppressedClosingIds =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _forceClosingIds =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Form, TrackedFormHandlers> _trackedFormHandlers =
            new();
        private readonly Dictionary<Form, EventHandler> _windowMenuTextChangedHandlers =
            new();

        // menu items added to the Window menu (removed on detach)
        private MenuStrip?  _windowMenu;
        private string      _windowMenuText = "&Window";
        private ToolStripMenuItem? _windowMenuItem;

        private sealed class TrackedFormHandlers
        {
            public FormClosingEventHandler FormClosingHandler { get; init; } = default!;
            public FormClosedEventHandler FormClosedHandler { get; init; } = default!;
            public EventHandler ActivatedHandler { get; init; } = default!;
            public EventHandler TextChangedHandler { get; init; } = default!;
        }

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
            return AddDocumentCore(CloneDescriptor(desc), true);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Creates a new MDI child <see cref="Form"/> and raises
        /// <see cref="DocumentFormCreated"/> so callers can add content.
        /// Always returns <see langword="null"/> — content is managed by the child Form.
        /// </remarks>
        public BeepDocumentPanel? AddDocument(string title, string iconPath, bool activate)
            => AddDocumentCore(
                DocumentDescriptor.Create(GenerateDocumentId(title), title, iconPath),
                activate);

        internal bool RegisterExternalDocumentForm(
            Form form,
            DocumentDescriptor descriptor,
            bool activate)
        {
            if (_parentForm == null || form == null || descriptor == null)
                return false;

            NormalizeDescriptor(descriptor, form.Text);

            if (_forms.TryGetValue(descriptor.Id, out var existing))
            {
                if (!ReferenceEquals(existing, form))
                    return false;

                UpdateTrackedDocument(form, descriptor);
                if (activate)
                    form.Activate();
                return true;
            }

            if (!ReferenceEquals(form.MdiParent, _parentForm))
                form.MdiParent = _parentForm;

            UpdateTrackedDocument(form, descriptor);
            TrackDocumentForm(form, descriptor.Id);

            if (!form.Visible)
                form.Show();
            if (activate)
                form.Activate();

            DocumentAdded?.Invoke(this,
                new DocumentAddedEventArgs(CloneDescriptor(_descriptors[descriptor.Id]), null));
            AddWindowMenuItem(descriptor.Id, form);
            return true;
        }

        internal bool DetachDocumentForm(string id, Form form)
        {
            if (string.IsNullOrEmpty(id) || form == null)
                return false;

            if (!_forms.TryGetValue(id, out var tracked) || !ReferenceEquals(tracked, form))
                return false;

            try { form.Hide(); } catch { }
            UntrackDocumentForm(id, form);
            return true;
        }

        internal bool TryGetDocumentForm(string id, out Form? form)
        {
            if (string.IsNullOrEmpty(id))
            {
                form = null;
                return false;
            }

            if (_forms.TryGetValue(id, out var tracked) && tracked != null && !tracked.IsDisposed)
            {
                form = tracked;
                return true;
            }

            form = null;
            return false;
        }

        private BeepDocumentPanel? AddDocumentCore(DocumentDescriptor descriptor, bool activate)
        {
            if (_parentForm == null) return null;
            NormalizeDescriptor(descriptor, null);

            if (_forms.TryGetValue(descriptor.Id, out var existing))
            {
                UpdateTrackedDocument(existing, descriptor);
                if (activate)
                    existing.Activate();
                return null;
            }

            var child = new Form
            {
                WindowState = FormWindowState.Normal,
                StartPosition = FormStartPosition.WindowsDefaultBounds
            };

            UpdateTrackedDocument(child, descriptor);
            TrackDocumentForm(child, descriptor.Id);

            // Notify subscribers so they can add content controls
            DocumentFormCreated?.Invoke(this,
                new MdiDocumentEventArgs(child, descriptor.Id, child.Text));

            child.Show();
            if (activate)
                child.Activate();

            // Raise DocumentAdded (Panel is null — MDI uses Forms, not BeepDocumentPanels)
            DocumentAdded?.Invoke(this,
                new DocumentAddedEventArgs(CloneDescriptor(_descriptors[descriptor.Id]), null));

            // Refresh window menu
            AddWindowMenuItem(descriptor.Id, child);

            return null; // MDI mode: content lives in the child Form, not a panel
        }

        /// <inheritdoc/>
        public bool RemoveDocument(string id, bool force = false)
        {
            if (string.IsNullOrEmpty(id) || !_forms.TryGetValue(id, out var form))
                return false;

            if (!force && !CanCloseDocument(id))
                return false;

            _suppressedClosingIds.Add(id);
            if (force)
                _forceClosingIds.Add(id);

            try
            {
                form.Close();
            }
            finally
            {
                if (_forms.ContainsKey(id))
                {
                    _suppressedClosingIds.Remove(id);
                    _forceClosingIds.Remove(id);
                }
            }

            return !_forms.ContainsKey(id);
        }

        /// <inheritdoc/>
        public bool DetachDocumentForTransfer(string id)
        {
            if (string.IsNullOrEmpty(id) || !_forms.TryGetValue(id, out var form))
                return false;

            try { form.Hide(); } catch { }
            UntrackDocumentForm(id, form);
            return true;
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
        public int DocumentCount => _forms.Count;

        /// <inheritdoc/>
        public DocumentEventArgs? ActiveDocument
        {
            get
            {
                if (string.IsNullOrEmpty(_activeDocumentId))
                    return null;

                if (_descriptors.TryGetValue(_activeDocumentId, out var descriptor))
                    return new DocumentEventArgs(_activeDocumentId, descriptor.Title);

                if (_forms.TryGetValue(_activeDocumentId, out var form))
                    return new DocumentEventArgs(_activeDocumentId, form.Text);

                return null;
            }
        }

        /// <inheritdoc/>
        public bool CloseAllDocuments()
        {
            var ids = new List<string>();
            foreach (var id in _forms.Keys)
            {
                if (CanCloseDocument(id))
                    ids.Add(id);
            }

            if (ids.Count == 0)
                return false;

            var closedAny = false;
            foreach (var id in ids)
            {
                if (RemoveDocument(id))
                    closedAny = true;
            }

            return closedAny;
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

            foreach (ToolStripItem item in menu.Items)
            {
                if (item is ToolStripMenuItem menuItem
                    && string.Equals(menuItem.Text, _windowMenuText, StringComparison.OrdinalIgnoreCase))
                {
                    _windowMenuItem = menuItem;
                    break;
                }
            }

            if (_windowMenuItem == null)
            {
                _windowMenuItem = new ToolStripMenuItem(_windowMenuText);
                menu.Items.Add(_windowMenuItem);
            }

            _windowMenuItem.DropDownItems.Clear();

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

        private bool CanCloseDocument(string id)
        {
            if (_forceClosingIds.Contains(id))
                return true;

            return !_descriptors.TryGetValue(id, out var descriptor) || descriptor.CanClose;
        }

        private void NormalizeDescriptor(DocumentDescriptor descriptor, string? fallbackTitle)
        {
            if (string.IsNullOrWhiteSpace(descriptor.Id))
                descriptor.Id = GenerateDocumentId(descriptor.Title ?? fallbackTitle ?? string.Empty);

            if (string.IsNullOrWhiteSpace(descriptor.Title))
                descriptor.Title = !string.IsNullOrWhiteSpace(fallbackTitle)
                    ? fallbackTitle
                    : GenerateUntitledTitle();

            if (string.IsNullOrWhiteSpace(descriptor.PersistenceKey))
                descriptor.PersistenceKey = Guid.NewGuid().ToString();
        }

        private void UpdateTrackedDocument(Form form, DocumentDescriptor descriptor)
        {
            NormalizeDescriptor(descriptor, form.Text);

            _descriptors[descriptor.Id] = CloneDescriptor(descriptor);
            ApplyDescriptorToForm(form, _descriptors[descriptor.Id]);
            _descriptors[descriptor.Id].Title = form.Text;
        }

        private void ApplyDescriptorToForm(Form form, DocumentDescriptor descriptor)
        {
            if (_parentForm != null && !ReferenceEquals(form.MdiParent, _parentForm))
                form.MdiParent = _parentForm;

            if (!string.IsNullOrWhiteSpace(descriptor.Title))
                form.Text = descriptor.Title;

            if (!string.IsNullOrEmpty(descriptor.IconPath) && File.Exists(descriptor.IconPath))
            {
                try { form.Icon = new System.Drawing.Icon(descriptor.IconPath); }
                catch { /* non-fatal */ }
            }
        }

        private void TrackDocumentForm(Form form, string id)
        {
            _forms[id] = form;

            DetachTrackedFormHandlers(form);

            var handlers = new TrackedFormHandlers
            {
                FormClosingHandler = (s, e) => OnDocumentFormClosing(id, e, (Form?)s ?? form),
                FormClosedHandler = (s, e) => OnDocumentFormClosed(id, (Form?)s ?? form),
                ActivatedHandler = (s, e) =>
                {
                    var title = _descriptors.TryGetValue(id, out var descriptor)
                        ? descriptor.Title
                        : form.Text;
                    _activeDocumentId = id;
                    ActiveDocumentChanged?.Invoke(this, new DocumentEventArgs(id, title));
                },
                TextChangedHandler = (s, e) =>
                {
                    if (_descriptors.TryGetValue(id, out var descriptor))
                        descriptor.Title = form.Text;

                    if (string.Equals(_activeDocumentId, id, StringComparison.OrdinalIgnoreCase))
                        ActiveDocumentChanged?.Invoke(this, new DocumentEventArgs(id, form.Text));
                }
            };

            _trackedFormHandlers[form] = handlers;
            form.FormClosing += handlers.FormClosingHandler;
            form.FormClosed += handlers.FormClosedHandler;
            form.Activated += handlers.ActivatedHandler;
            form.TextChanged += handlers.TextChangedHandler;
        }

        private void DetachTrackedFormHandlers(Form form)
        {
            if (!_trackedFormHandlers.TryGetValue(form, out var handlers))
                return;

            try { form.FormClosing -= handlers.FormClosingHandler; } catch { }
            try { form.FormClosed -= handlers.FormClosedHandler; } catch { }
            try { form.Activated -= handlers.ActivatedHandler; } catch { }
            try { form.TextChanged -= handlers.TextChangedHandler; } catch { }
            _trackedFormHandlers.Remove(form);
        }

        private void UntrackDocumentForm(string id, Form form)
        {
            _forms.Remove(id);
            _descriptors.Remove(id);
            _suppressedClosingIds.Remove(id);
            _forceClosingIds.Remove(id);
            DetachTrackedFormHandlers(form);

            if (string.Equals(_activeDocumentId, id, StringComparison.OrdinalIgnoreCase))
            {
                _activeDocumentId = null;
                UpdateActiveDocumentFromParentForm();
            }

            RemoveWindowMenuItem(id, form);
        }

        private void OnDocumentFormClosing(string id, FormClosingEventArgs e, Form form)
        {
            if (e.Cancel)
                return;

            var force = _forceClosingIds.Contains(id);
            if (!_suppressedClosingIds.Contains(id))
            {
                var closingArgs = CreateClosingEventArgs(id, form);
                DocumentClosing?.Invoke(this, closingArgs);
                if (closingArgs.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (!force && !CanCloseDocument(id))
                e.Cancel = true;
        }

        private void OnDocumentFormClosed(string id, Form form)
        {
            var title = _descriptors.TryGetValue(id, out var descriptor)
                ? descriptor.Title
                : form.Text;

            UntrackDocumentForm(id, form);
            DocumentRemoved?.Invoke(this, new DocumentEventArgs(id, title));
        }

        private void UpdateActiveDocumentFromParentForm()
        {
            var activeChild = _parentForm?.ActiveMdiChild;
            if (activeChild == null)
                return;

            foreach (var pair in _forms)
            {
                if (ReferenceEquals(pair.Value, activeChild))
                {
                    _activeDocumentId = pair.Key;
                    return;
                }
            }
        }

        private TabClosingEventArgs CreateClosingEventArgs(string id, Form form)
        {
            var descriptor = _descriptors.TryGetValue(id, out var tracked)
                ? CloneDescriptor(tracked)
                : DocumentDescriptor.Create(id, form.Text);

            var tab = new BeepDocumentTab(id, descriptor.Title)
            {
                IconPath = descriptor.IconPath,
                IsModified = descriptor.IsModified,
                CanClose = descriptor.CanClose,
                IsPinned = descriptor.IsPinned,
                AccentColor = descriptor.AccentColor,
                TooltipText = descriptor.TooltipText,
                Tag = descriptor.Tag,
                BadgeText = descriptor.BadgeText,
                BadgeColor = descriptor.BadgeColor,
                TabColor = descriptor.TabColor,
                DocumentCategory = descriptor.Category
            };

            return new TabClosingEventArgs(-1, tab);
        }

        private static DocumentDescriptor CloneDescriptor(DocumentDescriptor source)
        {
            var clone = new DocumentDescriptor
            {
                Id = source.Id,
                PersistenceKey = source.PersistenceKey,
                PreviousGroupId = source.PreviousGroupId,
                Title = source.Title,
                IconPath = source.IconPath,
                IsModified = source.IsModified,
                IsPinned = source.IsPinned,
                CanClose = source.CanClose,
                Category = source.Category,
                TooltipText = source.TooltipText,
                Tag = source.Tag,
                BadgeText = source.BadgeText,
                BadgeColor = source.BadgeColor,
                TabColor = source.TabColor,
                AccentColor = source.AccentColor,
                InitialContent = source.InitialContent
            };

            foreach (var pair in source.CustomData)
                clone.CustomData[pair.Key] = pair.Value;

            return clone;
        }

        private void AddWindowMenuItem(string id, Form form)
        {
            if (_windowMenuItem == null) return;
            AddWindowMenuItemCore(id, form);
        }

        private void AddWindowMenuItemCore(string id, Form form)
        {
            if (_windowMenuItem == null) return;
            RemoveWindowMenuItem(id, form);
            var item = new ToolStripMenuItem(form.Text) { Tag = id };
            item.Click += (s, e) => ActivateDocument(id);
            EventHandler textChangedHandler = (s, e) =>
            {
                if (s is Form f) item.Text = f.Text;
            };
            _windowMenuTextChangedHandlers[form] = textChangedHandler;
            form.TextChanged += textChangedHandler;
            _windowMenuItem.DropDownItems.Add(item);
        }

        private void RemoveWindowMenuItem(string id, Form? form)
        {
            if (form != null && _windowMenuTextChangedHandlers.TryGetValue(form, out var textChangedHandler))
            {
                try { form.TextChanged -= textChangedHandler; } catch { }
                _windowMenuTextChangedHandlers.Remove(form);
            }

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

        public void DetachWindowMenu()
        {
            foreach (var pair in _windowMenuTextChangedHandlers.ToList())
            {
                try { pair.Key.TextChanged -= pair.Value; } catch { }
            }
            _windowMenuTextChangedHandlers.Clear();

            if (_windowMenu == null || _windowMenuItem == null) return;

            try { _windowMenuItem.DropDownItems.Clear(); } catch { }
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
                var snapshot = new List<string>(_forms.Keys);
                foreach (var id in snapshot)
                    try { RemoveDocument(id, true); } catch { }
                _forms.Clear();
                _descriptors.Clear();
                _suppressedClosingIds.Clear();
                _forceClosingIds.Clear();
                _parentForm = null;
                _manager    = null;
            }
            base.Dispose(disposing);
        }
    }
}
