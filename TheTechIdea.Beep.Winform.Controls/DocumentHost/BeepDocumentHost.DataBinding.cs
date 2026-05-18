// BeepDocumentHost.DataBinding.cs
// Data-Binding API for BeepDocumentHost (Feature 6.3).
//
// Usage:
//   host.Documents.Add(new DocumentDescriptor { Id = "myDoc", Title = "My Document" });
//   // → automatically opens the panel.  Removing from the list closes it.
//
//   host.DocumentTemplate = desc => new MyEditorControl(desc.Tag);
//   // → called when the panel is created; returned control is placed inside.
//
// Two-way sync:
//   • Consumer adds/removes items → host creates/closes panels.
//   • Consumer changes Title/IconPath/IsModified/IsPinned on a descriptor → tab updates.
//   • Host closes a document (e.g., via tab × button) → matching descriptor is removed.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ── Backing fields ────────────────────────────────────────────────────

        private BindingList<DocumentDescriptor>? _documents;
        private bool _bindingSync; // re-entrancy guard

        // ── Factory delegate ──────────────────────────────────────────────────

        /// <summary>
        /// Optional factory that creates the <see cref="Control"/> placed inside the
        /// <see cref="BeepDocumentPanel"/> when a new <see cref="DocumentDescriptor"/> is
        /// added to <see cref="Documents"/>.
        /// </summary>
        /// <remarks>
        /// When null, the panel is created empty (the consumer can populate it later by
        /// subscribing to <see cref="DocumentAdded"/>).
        /// </remarks>
        public Func<DocumentDescriptor, Control?>? DocumentTemplate { get; set; }

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>
        /// Raised after a panel has been created and added for a descriptor.
        /// Provides access to both the descriptor and the empty panel so the
        /// consumer can populate its content.
        /// </summary>
        public event EventHandler<DocumentAddedEventArgs>? DocumentAdded;

        // ── Documents collection ──────────────────────────────────────────────

        /// <summary>
        /// Two-way bindable collection of open documents.  Adding a descriptor
        /// opens a document panel; removing one closes it.
        /// Changing <c>Title</c>, <c>IconPath</c>, <c>IsModified</c> or
        /// <c>IsPinned</c> on an item automatically updates the corresponding tab.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BindingList<DocumentDescriptor> Documents
        {
            get
            {
                if (_documents == null)
                {
                    _documents = new BindingList<DocumentDescriptor>
                    {
                        AllowNew    = true,
                        AllowEdit   = true,
                        AllowRemove = true
                    };
                    _documents.ListChanged  += OnDocumentsListChanged;
                }
                return _documents;
            }
        }

        // ── List-changed handler ──────────────────────────────────────────────

        private void OnDocumentsListChanged(object? sender, ListChangedEventArgs e)
        {
            if (_bindingSync) return;
            _bindingSync = true;
            try
            {
                switch (e.ListChangedType)
                {
                    case ListChangedType.ItemAdded:
                        if (e.NewIndex >= 0 && e.NewIndex < _documents!.Count)
                            DescriptorAdded(_documents[e.NewIndex]);
                        break;

                    case ListChangedType.ItemDeleted:
                        // Deletion: we don't have the item anymore; sync by open panels
                        SyncRemovedDescriptors();
                        break;

                    case ListChangedType.ItemChanged:
                        if (e.NewIndex >= 0 && e.NewIndex < _documents!.Count)
                            DescriptorPropertyChanged(_documents[e.NewIndex],
                                                       e.PropertyDescriptor?.Name);
                        break;

                    case ListChangedType.Reset:
                        SyncAllDescriptors();
                        break;
                }
            }
            finally
            {
                _bindingSync = false;
            }
        }

        // ── Descriptor → panel operations ────────────────────────────────────

        private void DescriptorAdded(DocumentDescriptor desc)
        {
            if (ContainsOpenDocument(desc.Id))
            {
                ApplyDescriptorState(desc);
                return;
            }

            OpenDocumentCore(
                desc,
                new DocumentOpenOptions
                {
                    Target = DocumentOpenTarget.PrimaryGroup,
                    Activate = false
                },
                attachDescriptorChanges: true,
                raiseDocumentAddedEvent: true);
        }

        private void AttachDescriptorPropertyChanged(DocumentDescriptor desc)
        {
            desc.PropertyChanged -= OnDescriptorPropertyChanged;
            desc.PropertyChanged += OnDescriptorPropertyChanged;
        }

        private void OnDescriptorPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is DocumentDescriptor desc)
                DescriptorPropertyChanged(desc, e.PropertyName);
        }

        private void ApplyDescriptorToOpenDocument(DocumentDescriptor desc,
                                                   BeepDocumentPanel panel,
                                                   bool raiseDocumentAddedEvent)
        {
            ApplyDescriptorState(desc);

            if (panel.Controls.Count == 0)
            {
                var child = DocumentTemplate?.Invoke(desc);
                if (child != null)
                {
                    child.Dock = DockStyle.Fill;
                    panel.Controls.Add(child);
                }
            }

            if (raiseDocumentAddedEvent)
                DocumentAdded?.Invoke(this, new DocumentAddedEventArgs(desc, panel));
        }

        private void ApplyDescriptorState(DocumentDescriptor desc)
        {
            if (!_panels.TryGetValue(desc.Id, out var panel)) return;

            bool wasSyncingPanelMetadata = _syncingDocumentPanelMetadata;
            _syncingDocumentPanelMetadata = true;
            try
            {
                panel.DocumentTitle = desc.Title;
                panel.IconPath = desc.IconPath;
                panel.IsModified = desc.IsModified;
                panel.IsPinned = desc.IsPinned;
                panel.DocumentCategory = desc.Category;
                panel.TooltipText = desc.TooltipText;
                panel.BadgeText = desc.BadgeText;
                panel.BadgeColor = desc.BadgeColor;
                panel.TabColor = desc.TabColor;
                panel.AccentColor = desc.AccentColor;

                PinDocument(desc.Id, desc.IsPinned);
                panel.CanClose = desc.CanClose;
            }
            finally
            {
                _syncingDocumentPanelMetadata = wasSyncingPanelMetadata;
            }

            if (TryGetDocumentTab(desc.Id, out var tabStrip, out var tab))
            {
                tab.Title = desc.Title;
                tab.IconPath = desc.IconPath;
                tab.CanClose = desc.CanClose;
                tab.TooltipText = desc.TooltipText;
                tab.TabColor = desc.TabColor;
                tab.AccentColor = desc.AccentColor;
                tab.DocumentCategory = desc.Category;
                tabStrip.Invalidate();
            }

            SetBadge(desc.Id, desc.BadgeText, desc.BadgeColor);
        }

        private void DescriptorPropertyChanged(DocumentDescriptor desc, string? propName)
        {
            if (!_panels.TryGetValue(desc.Id, out _)) return;
            ApplyDescriptorState(desc);
        }

        private void SyncRemovedDescriptors()
        {
            // Close panels whose ids are no longer in _documents
            var ids = _documents?.Select(d => d.Id).ToHashSet()
                      ?? new System.Collections.Generic.HashSet<string>();

            foreach (var id in _panels.Keys.ToList())
            {
                if (!ids.Contains(id))
                    CloseDocument(id);
            }
        }

        private void SyncAllDescriptors()
        {
            // Close panels not in list
            SyncRemovedDescriptors();
            // Open panels for new descriptors
            foreach (var desc in _documents ?? Enumerable.Empty<DocumentDescriptor>())
                DescriptorAdded(desc);
        }

        // ── Host → descriptor sync (panel closed by tab × button) ────────────

        // Called from OnTabCloseRequested → CloseDocument; we need to remove the
        // matching descriptor so the list stays in sync.
        private void RemoveDescriptorForId(string documentId)
        {
            if (_documents == null || _bindingSync) return;
            _bindingSync = true;
            try
            {
                var desc = _documents.FirstOrDefault(d => d.Id == documentId);
                if (desc != null) _documents.Remove(desc);
            }
            finally
            {
                _bindingSync = false;
            }
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // DocumentAddedEventArgs
    // ════════════════════════════════════════════════════════════════════════

    /// <summary>Event arguments for <see cref="BeepDocumentHost.DocumentAdded"/>.</summary>
    public sealed class DocumentAddedEventArgs : EventArgs
    {
        /// <summary>The descriptor that was added to <see cref="BeepDocumentHost.Documents"/>.</summary>
        public DocumentDescriptor Descriptor { get; }

        /// <summary>
        /// The freshly created (empty) panel — populate it inside the handler.
        /// Will be <see langword="null"/> for views that do not back documents with
        /// a <see cref="BeepDocumentPanel"/> (e.g. <c>BeepNativeMdiView</c>).
        /// </summary>
        public BeepDocumentPanel? Panel { get; }

        public DocumentAddedEventArgs(DocumentDescriptor descriptor, BeepDocumentPanel? panel)
        {
            Descriptor = descriptor ?? throw new ArgumentNullException(nameof(descriptor));
            Panel      = panel;
        }
    }
}
