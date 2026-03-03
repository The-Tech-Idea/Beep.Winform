// BeepDocumentHost.MVVM.cs
// MVVM document-source binding pipeline for BeepDocumentHost (Features 6.4 / 16.2).
//
// Two parallel APIs — use either or both:
//
//  API A (IBindingList / IDocumentViewModel - Feature 6.4):
//   host.DocumentContentTemplate = vm => new MyView((MyViewModel)vm);
//   host.DocumentSource = myBindingList;   // BindingList<T : IDocumentViewModel>
//
//  API B (INotifyCollectionChanged + selector delegates - Sprint 16.2):
//   host.DocumentIdSelector    = vm => ((MyVM)vm).Id;
//   host.DocumentTitleSelector = vm => ((MyVM)vm).Title;
//   host.ViewModelTemplate     = vm => new MyView((MyVM)vm);
//   host.DocumentsSource       = myObservableCollection;  // any IEnumerable + INCC
//
// Both APIs are independent of the DocumentDescriptor DataBinding API (6.3).
// ─────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ─────────────────────────────────────────────────────────────────────
        // Backing state
        // ─────────────────────────────────────────────────────────────────────

        private IBindingList? _documentSource;
        private bool          _mvvmSync; // re-entrancy guard

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Factory that creates the <see cref="Control"/> placed inside a
        /// <see cref="BeepDocumentPanel"/> when a new <see cref="IDocumentViewModel"/>
        /// is added to <see cref="DocumentSource"/>.
        /// </summary>
        /// <remarks>
        /// When null the panel is created empty; subscribe to <see cref="DocumentAdded"/>
        /// to populate it manually.
        /// </remarks>
        public Func<IDocumentViewModel, Control?>? DocumentContentTemplate { get; set; }

        /// <summary>
        /// Bindable list of <see cref="IDocumentViewModel"/> objects.
        /// Assign a <see cref="BindingList{T}"/> (where T implements
        /// <see cref="IDocumentViewModel"/>) to automatically open/close document
        /// panels as items are added or removed.
        ///
        /// Each view-model must also implement <see cref="INotifyPropertyChanged"/>
        /// for live title, icon, IsModified and badge updates.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IBindingList? DocumentSource
        {
            get => _documentSource;
            set
            {
                // Detach old
                if (_documentSource != null)
                    _documentSource.ListChanged -= OnVmListChanged;

                _documentSource = value;

                // Attach new
                if (_documentSource != null)
                {
                    _documentSource.ListChanged += OnVmListChanged;
                    SyncFromViewModels();
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Collection-change handler
        // ─────────────────────────────────────────────────────────────────────

        private void OnVmListChanged(object? sender, ListChangedEventArgs e)
        {
            if (_mvvmSync) return;
            _mvvmSync = true;
            try
            {
                switch (e.ListChangedType)
                {
                    case ListChangedType.ItemAdded:
                        if (e.NewIndex >= 0 && e.NewIndex < _documentSource!.Count)
                        {
                            var vm = _documentSource[e.NewIndex] as IDocumentViewModel;
                            if (vm != null) OpenVmDocument(vm);
                        }
                        break;

                    case ListChangedType.ItemDeleted:
                        // Sync by removing panels not present in the list any more
                        SyncRemovedVmDocuments();
                        break;

                    case ListChangedType.ItemChanged:
                        if (e.NewIndex >= 0 && e.NewIndex < _documentSource!.Count)
                        {
                            var vm = _documentSource[e.NewIndex] as IDocumentViewModel;
                            if (vm != null)
                                ApplyVmProperties(vm, e.PropertyDescriptor?.Name);
                        }
                        break;

                    case ListChangedType.Reset:
                        SyncFromViewModels();
                        break;
                }
            }
            finally
            {
                _mvvmSync = false;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Open / sync helpers
        // ─────────────────────────────────────────────────────────────────────

        private void OpenVmDocument(IDocumentViewModel vm)
        {
            if (_panels.ContainsKey(vm.Id)) return; // already open

            var panel = AddDocument(vm.Id, vm.Title, vm.IconPath, activate: false);
            panel.IsModified = vm.IsModified;
            panel.CanClose   = vm.CanClose;

            if (vm.IsPinned)
                PinDocument(vm.Id, true);

            var tab = _tabStrip.FindTabById(vm.Id);
            if (tab != null)
            {
                tab.TooltipText = vm.TooltipText;
                if (!string.IsNullOrEmpty(vm.BadgeText))
                    _tabStrip.SetBadge(vm.Id, vm.BadgeText);
            }

            // Create content via factory
            var child = DocumentContentTemplate?.Invoke(vm);
            if (child != null)
            {
                child.Dock = DockStyle.Fill;
                panel.Controls.Add(child);
            }

            // Subscribe to future property changes
            vm.PropertyChanged += OnVmPropertyChanged;
        }

        private void SyncFromViewModels()
        {
            if (_documentSource == null) return;

            // Close panels whose VMs are no longer present
            SyncRemovedVmDocuments();

            // Open panels for VMs that are new
            foreach (var item in _documentSource)
            {
                if (item is IDocumentViewModel vm)
                    OpenVmDocument(vm);
            }
        }

        private void SyncRemovedVmDocuments()
        {
            if (_documentSource == null) return;

            var ids = new System.Collections.Generic.HashSet<string>();
            foreach (var item in _documentSource)
                if (item is IDocumentViewModel vm) ids.Add(vm.Id);

            foreach (var id in _panels.Keys.ToList())
            {
                // Only close panels that were opened via MVVM and are no longer in the source
                // (panels opened via Documents API should not be touched here)
                if (!ids.Contains(id) && !IsDescriptorManaged(id))
                    CloseDocument(id);
            }
        }

        /// <summary>Returns true when the document id is managed by the DocumentDescriptor API.</summary>
        private bool IsDescriptorManaged(string id)
        {
            if (_documents == null) return false;
            foreach (var d in _documents)
                if (d.Id == id) return true;
            return false;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Property change on a single view-model
        // ─────────────────────────────────────────────────────────────────────

        private void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is IDocumentViewModel vm)
                ApplyVmProperties(vm, e.PropertyName);
        }

        private void ApplyVmProperties(IDocumentViewModel vm, string? propName)
        {
            if (!_panels.TryGetValue(vm.Id, out var panel)) return;
            var tab = _tabStrip.FindTabById(vm.Id);

            // null propName → refresh all
            bool all = propName == null;

            if (all || propName == nameof(IDocumentViewModel.Title))
            {
                panel.DocumentTitle = vm.Title;
                if (tab != null) { tab.Title = vm.Title; _tabStrip.Invalidate(); }
            }
            if (all || propName == nameof(IDocumentViewModel.IconPath))
            {
                panel.IconPath = vm.IconPath;
                if (tab != null) { tab.IconPath = vm.IconPath; _tabStrip.Invalidate(); }
            }
            if (all || propName == nameof(IDocumentViewModel.IsModified))
                panel.IsModified = vm.IsModified;
            if (all || propName == nameof(IDocumentViewModel.IsPinned))
                PinDocument(vm.Id, vm.IsPinned);
            if (all || propName == nameof(IDocumentViewModel.CanClose))
            {
                panel.CanClose = vm.CanClose;
                if (tab != null) { tab.CanClose = vm.CanClose; _tabStrip.Invalidate(); }
            }
            if (all || propName == nameof(IDocumentViewModel.TooltipText))
            {
                if (tab != null) tab.TooltipText = vm.TooltipText;
            }
            if (all || propName == nameof(IDocumentViewModel.BadgeText))
                _tabStrip.SetBadge(vm.Id, vm.BadgeText);
        }

        /// <summary>
        /// Detaches all MVVM subscriptions.
        /// Called from the main <c>Dispose</c>.
        /// </summary>
        private void DisposeMvvm()
        {
            // API A: IBindingList
            if (_documentSource != null)
            {
                _documentSource.ListChanged -= OnVmListChanged;
                foreach (var item in _documentSource)
                    if (item is IDocumentViewModel vm)
                        vm.PropertyChanged -= OnVmPropertyChanged;
            }

            // API B: INotifyCollectionChanged
            if (_documentsSource is INotifyCollectionChanged incc)
                incc.CollectionChanged -= OnDocumentsSourceChanged;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Sprint 16.2 — INotifyCollectionChanged source binding
        // ─────────────────────────────────────────────────────────────────────

        private IEnumerable? _documentsSource;
        private bool         _inccSync;  // re-entrancy guard for API B

        /// <summary>
        /// Factory that receives a raw view-model object (from
        /// <see cref="DocumentsSource"/>) and returns the <see cref="Control"/>
        /// to place inside the document panel.
        /// </summary>
        public Func<object, Control?>? ViewModelTemplate { get; set; }

        /// <summary>Extracts a stable, unique document ID from a raw view-model object.</summary>
        public Func<object, string>? DocumentIdSelector { get; set; }

        /// <summary>Extracts the display title from a raw view-model object.</summary>
        public Func<object, string>? DocumentTitleSelector { get; set; }

        /// <summary>
        /// Bindable collection of view-model objects (any type — not limited to
        /// <see cref="IDocumentViewModel"/>).  Assign any <c>IEnumerable</c> that also
        /// implements <see cref="INotifyCollectionChanged"/> (e.g.
        /// <c>System.Collections.ObjectModel.ObservableCollection&lt;T&gt;</c>) to
        /// automatically open/close document panels as items are added or removed.
        /// <para>
        /// Requires <see cref="DocumentIdSelector"/> and <see cref="DocumentTitleSelector"/>
        /// to be set before assignment.
        /// </para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable? DocumentsSource
        {
            get => _documentsSource;
            set
            {
                // Detach old
                if (_documentsSource is INotifyCollectionChanged oldIncc)
                    oldIncc.CollectionChanged -= OnDocumentsSourceChanged;

                _documentsSource = value;

                // Attach new
                if (_documentsSource is INotifyCollectionChanged newIncc)
                    newIncc.CollectionChanged += OnDocumentsSourceChanged;

                // Initial full sync
                if (_documentsSource != null)
                    SyncFromDocumentsSource();
            }
        }

        private void OnDocumentsSourceChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_inccSync) return;
            _inccSync = true;
            try
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems != null)
                            foreach (var item in e.NewItems)
                                OpenInccDocument(item);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems != null)
                            foreach (var item in e.OldItems)
                                CloseInccDocument(item);
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        SyncFromDocumentsSource();
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        if (e.OldItems != null)
                            foreach (var item in e.OldItems)
                                CloseInccDocument(item);
                        if (e.NewItems != null)
                            foreach (var item in e.NewItems)
                                OpenInccDocument(item);
                        break;
                }
            }
            finally { _inccSync = false; }
        }

        private void SyncFromDocumentsSource()
        {
            if (_documentsSource == null) return;

            // Build expected-id set
            var expected = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            foreach (var item in _documentsSource)
            {
                var id = ResolveInccId(item);
                if (id != null) expected.Add(id);
            }

            // Close panels not in the new collection
            foreach (var id in _panels.Keys.ToList())
            {
                if (!expected.Contains(id) && !IsDescriptorManaged(id) && !IsApiAManaged(id))
                    CloseDocument(id);
            }

            // Open new panels
            foreach (var item in _documentsSource)
                OpenInccDocument(item);
        }

        private void OpenInccDocument(object item)
        {
            var id    = ResolveInccId(item);
            var title = DocumentTitleSelector?.Invoke(item) ?? item.ToString() ?? "Document";
            if (id == null || _panels.ContainsKey(id)) return;

            var panel = AddDocument(id, title, null, activate: false);

            var child = ViewModelTemplate?.Invoke(item);
            if (child != null)
            {
                child.Dock = DockStyle.Fill;
                panel.Controls.Add(child);
            }
        }

        private void CloseInccDocument(object item)
        {
            var id = ResolveInccId(item);
            if (id != null) CloseDocument(id);
        }

        private string? ResolveInccId(object item)
        {
            if (DocumentIdSelector != null) return DocumentIdSelector(item);
            if (item is IDocumentViewModel dvm) return dvm.Id;
            return null;
        }

        /// <summary>True when the document id belongs to API A (IBindingList / IDocumentViewModel).</summary>
        private bool IsApiAManaged(string id)
        {
            if (_documentSource == null) return false;
            foreach (var item in _documentSource)
                if (item is IDocumentViewModel vm && vm.Id == id) return true;
            return false;
        }
    }
}
