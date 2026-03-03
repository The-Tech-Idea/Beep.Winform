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
            if (_panels.ContainsKey(desc.Id)) return;   // already open

            var panel = AddDocument(desc.Id, desc.Title, desc.IconPath, activate: false);
            panel.IsModified = desc.IsModified;
            panel.CanClose   = desc.CanClose;

            if (desc.IsPinned)
                PinDocument(desc.Id, true);

            // Set tooltip
            var tab = _tabStrip.FindTabById(desc.Id);
            if (tab != null) tab.TooltipText = desc.TooltipText;

            // Set badge
            if (!string.IsNullOrEmpty(desc.BadgeText))
                _tabStrip.SetBadge(desc.Id, desc.BadgeText, desc.BadgeColor);

            // Set tab colour
            if (desc.TabColor != System.Drawing.Color.Empty)
            {
                var t = _tabStrip.FindTabById(desc.Id);
                if (t != null) { t.TabColor = desc.TabColor; _tabStrip.Invalidate(); }
            }

            // Populate content via factory
            var child = DocumentTemplate?.Invoke(desc);
            if (child != null)
            {
                child.Dock = DockStyle.Fill;
                panel.Controls.Add(child);
            }

            DocumentAdded?.Invoke(this, new DocumentAddedEventArgs(desc, panel));

            // Subscribe to future property changes on this descriptor
            desc.PropertyChanged += (s, e) =>
            {
                if (s is DocumentDescriptor d)
                    DescriptorPropertyChanged(d, e.PropertyName);
            };
        }

        private void DescriptorPropertyChanged(DocumentDescriptor desc, string? propName)
        {
            if (!_panels.TryGetValue(desc.Id, out var panel)) return;
            var tab = _tabStrip.FindTabById(desc.Id);

            switch (propName)
            {
                case nameof(DocumentDescriptor.Title):
                    panel.DocumentTitle = desc.Title;
                    if (tab != null) { tab.Title = desc.Title; _tabStrip.Invalidate(); }
                    break;

                case nameof(DocumentDescriptor.IconPath):
                    panel.IconPath = desc.IconPath;
                    if (tab != null) { tab.IconPath = desc.IconPath; _tabStrip.Invalidate(); }
                    break;

                case nameof(DocumentDescriptor.IsModified):
                    panel.IsModified = desc.IsModified;
                    break;

                case nameof(DocumentDescriptor.IsPinned):
                    PinDocument(desc.Id, desc.IsPinned);
                    break;

                case nameof(DocumentDescriptor.CanClose):
                    panel.CanClose = desc.CanClose;
                    if (tab != null) { tab.CanClose = desc.CanClose; _tabStrip.Invalidate(); }
                    break;

                case nameof(DocumentDescriptor.TooltipText):
                    if (tab != null) tab.TooltipText = desc.TooltipText;
                    break;

                case nameof(DocumentDescriptor.BadgeText):
                case nameof(DocumentDescriptor.BadgeColor):
                    _tabStrip.SetBadge(desc.Id, desc.BadgeText, desc.BadgeColor);
                    break;

                case nameof(DocumentDescriptor.TabColor):
                    if (tab != null) { tab.TabColor = desc.TabColor; _tabStrip.Invalidate(); }
                    break;
            }
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

        /// <summary>The freshly created (empty) panel.  Populate it inside the handler.</summary>
        public BeepDocumentPanel Panel { get; }

        internal DocumentAddedEventArgs(DocumentDescriptor descriptor, BeepDocumentPanel panel)
        {
            Descriptor = descriptor;
            Panel      = panel;
        }
    }
}
