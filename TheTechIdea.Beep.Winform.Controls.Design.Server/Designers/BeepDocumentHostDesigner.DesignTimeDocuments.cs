// BeepDocumentHostDesigner.DesignTimeDocuments.cs
// Phase 03 — split out of BeepDocumentHostDesigner.cs.
//
// Public design-time CRUD surface for BeepDocumentHost (called from verbs,
// smart-tag action lists, and the right-click context menu) plus the
// internal helpers that mutate DesignTimeDocuments inside a single
// DesignerTransaction so every change is undoable with Ctrl+Z.
//
//   ExecuteDesignTimeDocumentsAction — wraps an action in a transaction,
//   notifies IComponentChangeService for both DesignTimeDocuments and
//   DesignTimeLayoutJson properties, re-runs surface wiring, re-sites any
//   newly created panels, and refreshes the smart-tag UI.
//
//   AddDesignTimeDocumentInternal / CreateSplitDesignTimeDocumentInternal /
//   EnsureDesignTimeDocumentOpen / EnsureActiveDesignDocumentSurface /
//   EnsureDesignSurfaceForDroppedContent — used by both the public API and
//   by OnDragDrop in BeepDocumentHostDesigner.DragDrop.cs.
//
//   SyncHostWithDesignTimeDocuments / ApplyDescriptorToOpenDocument /
//   CloseDesignTimeDocument / CollectDocumentIds — keep the open documents
//   in lockstep with the DesignTimeDocuments collection so reordering or
//   removing descriptors at design time produces matching live panels.
//
//   CaptureDocumentDescriptor / CloneDescriptor /
//   AddDescriptorSnapshotToDesignTimeDocuments — snapshot helpers used by
//   pin/unpin/rename actions and by the "Reopen Last Closed" stack.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Dialogs;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public partial class BeepDocumentHostDesigner
    {
        // ── Public design-time API (verbs / smart-tag / context menu) ────────

        public void AddDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction("Add Document", (h, docs) =>
            {
                new DesignTimeDocumentCoordinator(this, h, docs)
                    .AddNewDocument(activate: true, selectSurface: true);
            });
        }

        internal void ExecuteSharedDocumentAction(string transactionName, Action<BeepDocumentHost, Collection<DocumentDescriptor>> action)
            => ExecuteDesignTimeDocumentsActionCore(transactionName, action, createTransaction: true);

        public void CloseActiveDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            ExecuteDesignTimeDocumentsAction($"Close Document '{activeDocumentId}'", (h, docs) =>
            {
                var coordinator = new DesignTimeDocumentCoordinator(this, h, docs);
                if (!coordinator.RemoveDocument(activeDocumentId, selectSurface: true, out DocumentDescriptor? snapshot)
                    || snapshot == null)
                {
                    return;
                }
                _designTimeClosedDocuments.Push(snapshot);
            });
        }

        public void CloseAllDesignTimeDocuments()
        {
            if (Component is not BeepDocumentHost)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction("Close All Documents", (h, docs) =>
            {
                var coordinator = new DesignTimeDocumentCoordinator(this, h, docs);
                foreach (DocumentDescriptor descriptor in docs.ToList())
                {
                    if (string.IsNullOrWhiteSpace(descriptor.Id))
                    {
                        continue;
                    }

                    if (coordinator.RemoveDocument(descriptor.Id, selectSurface: false, out DocumentDescriptor? snapshot)
                        && snapshot != null)
                    {
                        _designTimeClosedDocuments.Push(snapshot);
                    }
                }

                InternalSyncDesignerSelection(h);
            });
        }

        public void ReopenLastClosedDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost || _designTimeClosedDocuments.Count == 0)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction("Reopen Last Closed Document", (h, docs) =>
            {
                DocumentDescriptor descriptor = CloneDescriptor(_designTimeClosedDocuments.Pop());
                new DesignTimeDocumentCoordinator(this, h, docs)
                    .AddOrUpdateDocument(descriptor, activate: true, selectSurface: true);
            });
        }

        public void CreateSplitDesignTimeDocument(bool horizontal)
        {
            if (Component is not BeepDocumentHost)
            {
                return;
            }

            string description = horizontal ? "Split Horizontal" : "Split Vertical";
            ExecuteDesignTimeDocumentsAction(description, (h, docs) =>
            {
                CreateSplitDesignTimeDocumentInternal(h, docs, horizontal, selectSurface: true);
            });
        }

        public void ApplyDesignTimeLayoutPreset(LayoutPreset preset)
        {
            if (Component is not BeepDocumentHost)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction($"Apply Layout Preset: {preset}", (h, docs) =>
            {
                ApplyDesignTimeLayoutPresetCore(h, docs, preset, selectSurface: true);
            });
        }

        public void SelectActiveDocumentSurface()
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            if (host.ActivePanel != null)
            {
                SyncDesignerSelection(host.ActivePanel);
                return;
            }

            ExecuteDesignTimeDocumentsAction("Select Active Document Surface", (h, docs) =>
            {
                BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(h, docs, selectSurface: true);
                SyncDesignerSelection((object?)panel ?? h);
            });
        }

        public string GetActiveDocumentTitle()
            => (Component as BeepDocumentHost)?.ActivePanel?.DocumentTitle ?? string.Empty;

        public void SetActiveDocumentTitle(string value)
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string title = string.IsNullOrWhiteSpace(value) ? "Document" : value.Trim();
            string activeDocumentId = host.ActiveDocumentId!;

            ExecuteDesignTimeDocumentsAction($"Rename Document '{activeDocumentId}'", (h, docs) =>
            {
                DocumentDescriptor descriptor = FindDesignTimeDocument(docs, activeDocumentId)
                    ?? AddDescriptorSnapshotToDesignTimeDocuments(h, docs, activeDocumentId);

                descriptor.Title = title;

                if (h.GetPanel(activeDocumentId) is BeepDocumentPanel panel)
                {
                    panel.DocumentTitle = title;
                }

                foreach (BeepDocumentGroup group in h.Groups)
                {
                    BeepDocumentTab? tab = group.TabStrip.FindTabById(activeDocumentId);
                    if (tab != null)
                    {
                        tab.Title = title;
                        group.TabStrip.Invalidate();
                    }
                }

                h.RecalculateLayout();
                SyncDesignerSelection((object?)h.ActivePanel ?? h);
            });
        }

        public void SetActiveDocumentPinned(bool pinned)
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            string description = pinned ? $"Pin Document '{activeDocumentId}'" : $"Unpin Document '{activeDocumentId}'";
            ExecuteDesignTimeDocumentsAction(description, (h, docs) =>
            {
                DocumentDescriptor descriptor = FindDesignTimeDocument(docs, activeDocumentId)
                    ?? AddDescriptorSnapshotToDesignTimeDocuments(h, docs, activeDocumentId);
                descriptor.IsPinned = pinned;
                h.PinDocument(activeDocumentId, pinned);
                SyncDesignerSelection((object?)h.ActivePanel ?? h);
            });
        }

        public void MergeAllDesignTimeGroups()
        {
            if (Component is not BeepDocumentHost)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction("Merge All Groups", (h, docs) =>
            {
                h.MergeAllGroups();
                BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(h, docs, selectSurface: true);
                SyncDesignerSelection((object?)panel ?? h);
            });
        }

        public void FloatActiveDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            ExecuteDesignTimeDocumentsAction($"Float Document '{activeDocumentId}'", (h, docs) =>
            {
                h.FloatDocument(activeDocumentId);
                SyncDesignerSelection(h);
            });
        }

        public void DockBackActiveDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            ExecuteDesignTimeDocumentsAction($"Dock Document '{activeDocumentId}'", (h, docs) =>
            {
                h.DockBackDocument(activeDocumentId);
                BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(h, docs, selectSurface: true);
                SyncDesignerSelection((object?)panel ?? h);
            });
        }

        public void AutoHideActiveDesignTimeDocument(AutoHideSide side)
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            ExecuteDesignTimeDocumentsAction($"Auto Hide Document '{activeDocumentId}'", (h, docs) =>
            {
                h.AutoHideDocument(activeDocumentId, side);
                SyncDesignerSelection(h);
            });
        }

        public void RestoreAutoHideActiveDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            ExecuteDesignTimeDocumentsAction($"Restore Auto Hide Document '{activeDocumentId}'", (h, docs) =>
            {
                h.RestoreAutoHideDocument(activeDocumentId);
                BeepDocumentPanel? panel = EnsureActiveDesignDocumentSurface(h, docs, selectSurface: true);
                SyncDesignerSelection((object?)panel ?? h);
            });
        }

        public void ShowLayoutAssistant()
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            using var dialog = new DocumentHostLayoutAssistantDialog(host.DesignTimeDocuments);
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction("Layout Assistant", (h, docs) =>
            {
                ApplyLayoutAssistant(h, docs, dialog.SelectedPreset, dialog.Documents);
            });
        }

        public void RenameActiveDesignTimeDocumentWithPrompt()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            using var dialog = new DocumentHostTextPromptDialog(
                "Rename Document",
                "Document title:",
                GetActiveDocumentTitle());

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SetActiveDocumentTitle(dialog.Value);
            }
        }

        public void OpenDesignTimeDocumentsEditor()
        {
            if (Component is BeepDocumentHost host)
            {
                EditDesignTimeDocuments(host);
            }
        }

        public bool CanReopenLastClosedDesignTimeDocument => _designTimeClosedDocuments.Count > 0;

        public DocumentDockState GetActiveDocumentDockState()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return DocumentDockState.None;
            }

            return host.GetDocumentDockState(host.ActiveDocumentId!);
        }

        // ── Transactional core ───────────────────────────────────────────────

        /// <summary>
        /// Wraps <paramref name="action"/> in a DesignerTransaction and notifies
        /// IComponentChangeService for both DesignTimeDocuments and
        /// DesignTimeLayoutJson so the change participates in Ctrl+Z undo and
        /// gets serialized into InitializeComponent.
        /// </summary>
        private void ExecuteDesignTimeDocumentsAction(string description, Action<BeepDocumentHost, Collection<DocumentDescriptor>> action)
            => ExecuteDesignTimeDocumentsActionCore(description, action, createTransaction: true);

        internal void ExecuteDesignTimeDocumentsActionInPlace(string description, Action<BeepDocumentHost, Collection<DocumentDescriptor>> action)
            => ExecuteDesignTimeDocumentsActionCore(description, action, createTransaction: false);

        private void ExecuteDesignTimeDocumentsActionCore(string description, Action<BeepDocumentHost, Collection<DocumentDescriptor>> action, bool createTransaction)
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            IDesignerHost?           designerHost     = GetDesignerHost();
            IComponentChangeService? changeService    = GetChangeService();
            PropertyDescriptor?      property         = GetDesignTimeDocumentsProperty();
            PropertyDescriptor?      layoutProperty   = GetDesignTimeLayoutProperty();
            PropertyDescriptor?      panelsProperty   = GetDocumentPanelsProperty();
            string                   previousLayout   = host.DesignTimeLayoutJson;
            List<BeepDocumentManager> syncedManagers = new();

            DesignerTransaction? transaction = null;
            try
            {
                if (createTransaction)
                {
                    transaction = designerHost?.CreateTransaction(description);
                }

                changeService?.OnComponentChanging(host, property);
                changeService?.OnComponentChanging(host, panelsProperty);
                if (layoutProperty != null)
                    changeService?.OnComponentChanging(host, layoutProperty);

                action(host, host.DesignTimeDocuments);

                string currentLayout = CaptureDesignTimeLayout(host, host.DesignTimeDocuments);
                host.DesignTimeLayoutJson = currentLayout;
                syncedManagers = SyncBoundManagersDesignTimeDocuments(host, changeService);

                changeService?.OnComponentChanged(host, property, null, host.DesignTimeDocuments);
                changeService?.OnComponentChanged(host, panelsProperty, null, host.DocumentPanels);
                if (layoutProperty != null)
                    changeService?.OnComponentChanged(host, layoutProperty, previousLayout, host.DesignTimeLayoutJson);
                transaction?.Commit();
            }
            catch
            {
                transaction?.Cancel();
                throw;
            }

            WireDesignContextMenuSurfaces(host);

            // Re-site any newly created panels so they appear as designer components
            // (selectable in the Properties window, undoable, named in the component tray)
            // immediately after every add/split/move/close action.
            SiteAllDesignPanels();

            DesignerActionUIService? actionUiService = GetDesignerActionUiService();
            foreach (BeepDocumentManager manager in syncedManagers)
            {
                actionUiService?.Refresh(manager);
            }

            RefreshDesignerActionUI();
        }

        private List<BeepDocumentManager> SyncBoundManagersDesignTimeDocuments(
            BeepDocumentHost host,
            IComponentChangeService? changeService)
        {
            var syncedManagers = new List<BeepDocumentManager>();

            foreach (BeepDocumentManager manager in GetBoundManagers(host))
            {
                if (AreEquivalentDocumentCollections(host.DesignTimeDocuments, manager.DesignTimeDocuments))
                {
                    continue;
                }

                PropertyDescriptor? managerProperty = TypeDescriptor.GetProperties(manager)[nameof(BeepDocumentManager.DesignTimeDocuments)];
                changeService?.OnComponentChanging(manager, managerProperty);

                manager.BeginDesignTimeDocumentUpdate();
                try
                {
                    manager.DesignTimeDocuments.Clear();
                    foreach (DocumentDescriptor descriptor in host.DesignTimeDocuments)
                    {
                        manager.DesignTimeDocuments.Add(InternalCloneDescriptor(descriptor));
                    }

                    manager.EndDesignTimeDocumentUpdate(applyChanges: false);
                }
                catch
                {
                    try { manager.EndDesignTimeDocumentUpdate(applyChanges: false); }
                    catch { /* design-time guard */ }
                    throw;
                }

                changeService?.OnComponentChanged(manager, managerProperty, null, manager.DesignTimeDocuments);
                syncedManagers.Add(manager);
            }

            return syncedManagers;
        }

        private IEnumerable<BeepDocumentManager> GetBoundManagers(BeepDocumentHost host)
        {
            IContainer? container = host.Site?.Container;
            if (container == null)
            {
                yield break;
            }

            foreach (IComponent component in container.Components)
            {
                if (component is BeepDocumentManager manager
                    && ReferenceEquals(manager.Host, host))
                {
                    yield return manager;
                }
            }
        }

        private static bool AreEquivalentDocumentCollections(
            ICollection<DocumentDescriptor> left,
            ICollection<DocumentDescriptor> right)
        {
            if (left.Count != right.Count)
            {
                return false;
            }

            using IEnumerator<DocumentDescriptor> leftEnumerator = left.GetEnumerator();
            using IEnumerator<DocumentDescriptor> rightEnumerator = right.GetEnumerator();

            while (leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
            {
                if (!AreEquivalentDocumentDescriptor(leftEnumerator.Current, rightEnumerator.Current))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool AreEquivalentDocumentDescriptor(DocumentDescriptor left, DocumentDescriptor right)
        {
            if (!string.Equals(left.Id, right.Id, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(left.PersistenceKey, right.PersistenceKey, StringComparison.Ordinal)
                || !string.Equals(left.PreviousGroupId, right.PreviousGroupId, StringComparison.OrdinalIgnoreCase)
                || !string.Equals(left.Title, right.Title, StringComparison.Ordinal)
                || !string.Equals(left.IconPath, right.IconPath, StringComparison.Ordinal)
                || left.IsModified != right.IsModified
                || left.IsPinned != right.IsPinned
                || left.CanClose != right.CanClose
                || left.Category != right.Category
                || !string.Equals(left.TooltipText, right.TooltipText, StringComparison.Ordinal)
                || !Equals(left.Tag, right.Tag)
                || !string.Equals(left.BadgeText, right.BadgeText, StringComparison.Ordinal)
                || left.BadgeColor != right.BadgeColor
                || left.TabColor != right.TabColor
                || left.AccentColor != right.AccentColor
                || left.InitialContent != right.InitialContent
                || left.CustomData.Count != right.CustomData.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, string> pair in left.CustomData)
            {
                if (!right.CustomData.TryGetValue(pair.Key, out string? otherValue)
                    || !string.Equals(pair.Value, otherValue, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        // ── Document/descriptor sync helpers ─────────────────────────────────

        private void SyncHostWithDesignTimeDocuments(BeepDocumentHost host, Collection<DocumentDescriptor> docs)
        {
            new DesignTimeDocumentCoordinator(this, host, docs)
                .SyncDocuments(docs.Where(descriptor => !string.IsNullOrWhiteSpace(descriptor.Id)).ToList());
        }

        internal void SyncCurrentDesignTimeDocuments()
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            ExecuteDesignTimeDocumentsAction("Sync Design-Time Documents", (h, docs) =>
            {
                SyncHostWithDesignTimeDocuments(h, docs);
            });
        }

        internal void SyncCurrentDesignTimeDocumentsInPlace()
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            ExecuteDesignTimeDocumentsActionInPlace("Sync Design-Time Documents", (h, docs) =>
            {
                SyncHostWithDesignTimeDocuments(h, docs);
            });
        }

        private void ApplyDescriptorToOpenDocument(BeepDocumentHost host, DocumentDescriptor descriptor)
        {
            if (host.GetPanel(descriptor.Id) is BeepDocumentPanel panel)
            {
                panel.DocumentTitle = descriptor.Title;
                panel.IconPath = descriptor.IconPath;
                panel.CanClose = descriptor.CanClose;
                panel.IsModified = descriptor.IsModified;
            }

            foreach (BeepDocumentGroup group in host.Groups)
            {
                BeepDocumentTab? tab = group.TabStrip.FindTabById(descriptor.Id);
                if (tab == null)
                {
                    continue;
                }

                tab.Title = descriptor.Title;
                tab.IconPath = descriptor.IconPath;
                tab.CanClose = descriptor.CanClose;
                tab.IsModified = descriptor.IsModified;
                tab.IsPinned = descriptor.IsPinned;
                group.TabStrip.Invalidate();
            }

            if (host.GetDocumentDockState(descriptor.Id) == DocumentDockState.Docked)
            {
                host.PinDocument(descriptor.Id, descriptor.IsPinned);
            }
        }

        private bool CloseDesignTimeDocument(BeepDocumentHost host, string documentId)
        {
            switch (host.GetDocumentDockState(documentId))
            {
                case DocumentDockState.Floating:
                    host.DockBackDocument(documentId);
                    break;
                case DocumentDockState.AutoHide:
                    host.RestoreAutoHideDocument(documentId);
                    break;
            }

            return host.CloseDocument(documentId);
        }

        private static IReadOnlyList<string> GetOpenDocumentIds(BeepDocumentHost host)
        {
            try
            {
                using JsonDocument json = JsonDocument.Parse(host.SaveLayout());
                var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                CollectDocumentIds(json.RootElement, ids);
                return ids.ToList();
            }
            catch
            {
                return host.Groups
                    .SelectMany(group => group.DocumentIds)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
        }

        private static void CollectDocumentIds(JsonElement element, HashSet<string> ids)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (JsonProperty property in element.EnumerateObject())
                    {
                        if ((property.NameEquals("id") || property.NameEquals("documentId"))
                            && property.Value.ValueKind == JsonValueKind.String)
                        {
                            string? value = property.Value.GetString();
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                ids.Add(value);
                            }
                        }

                        CollectDocumentIds(property.Value, ids);
                    }
                    break;

                case JsonValueKind.Array:
                    foreach (JsonElement item in element.EnumerateArray())
                    {
                        CollectDocumentIds(item, ids);
                    }
                    break;
            }
        }

        private static string CaptureDesignTimeLayout(BeepDocumentHost host, Collection<DocumentDescriptor> docs)
        {
            if (!docs.Any(descriptor => !string.IsNullOrWhiteSpace(descriptor.Id)))
            {
                return string.Empty;
            }

            try
            {
                return host.SaveLayout();
            }
            catch
            {
                return host.DesignTimeLayoutJson;
            }
        }

        private static string NormalizeDocumentTitle(string? title, int ordinal)
            => string.IsNullOrWhiteSpace(title) ? $"Document {ordinal}" : title.Trim();

        // ── Surface / descriptor creation helpers ────────────────────────────

        private BeepDocumentPanel? EnsureDesignSurfaceForDroppedContent()
        {
            if (Component is not BeepDocumentHost host)
            {
                return null;
            }

            if (host.ActivePanel != null)
            {
                return host.ActivePanel;
            }

            BeepDocumentPanel? panel = null;
            ExecuteDesignTimeDocumentsAction("Create Document For Dropped Control", (h, docs) =>
            {
                panel = EnsureActiveDesignDocumentSurface(h, docs, selectSurface: false);
            });

            return (Component as BeepDocumentHost)?.ActivePanel ?? panel;
        }

        private BeepDocumentPanel? EnsureActiveDesignDocumentSurface(BeepDocumentHost host, Collection<DocumentDescriptor> docs, bool selectSurface)
        {
            if (host.ActivePanel != null)
            {
                if (selectSurface)
                {
                    SyncDesignerSelection(host.ActivePanel);
                }

                return host.ActivePanel;
            }

            if (docs.Count == 0)
            {
                return AddDesignTimeDocumentInternal(host, docs, activate: true, selectSurface: selectSurface);
            }

            DocumentDescriptor? descriptor = docs.FirstOrDefault(doc => !string.IsNullOrWhiteSpace(doc.Id));
            if (descriptor == null)
            {
                return AddDesignTimeDocumentInternal(host, docs, activate: true, selectSurface: selectSurface);
            }

            BeepDocumentPanel? panel = EnsureDesignTimeDocumentOpen(host, descriptor, activate: true);
            if (selectSurface)
            {
                SyncDesignerSelection((object?)panel ?? host);
            }

            return panel;
        }

        private bool EnsureDesignDocumentCount(BeepDocumentHost host, Collection<DocumentDescriptor> docs, int count)
        {
            while (docs.Count(doc => !string.IsNullOrWhiteSpace(doc.Id)) < count)
            {
                if (new DesignTimeDocumentCoordinator(this, host, docs)
                    .AddNewDocument(activate: false, selectSurface: false) == null)
                {
                    return false;
                }
            }

            return docs.Count(doc => !string.IsNullOrWhiteSpace(doc.Id)) >= count;
        }

        private BeepDocumentPanel? AddDesignTimeDocumentInternal(BeepDocumentHost host, Collection<DocumentDescriptor> docs, bool activate, bool selectSurface)
        {
            DocumentDescriptor? descriptor = new DesignTimeDocumentCoordinator(this, host, docs)
                .AddNewDocument(activate, selectSurface);
            return descriptor == null ? null : host.GetPanel(descriptor.Id);
        }

        private BeepDocumentPanel? CreateSplitDesignTimeDocumentInternal(BeepDocumentHost host, Collection<DocumentDescriptor> docs, bool horizontal, bool selectSurface)
        {
            BeepDocumentPanel? anchorPanel = EnsureActiveDesignDocumentSurface(host, docs, selectSurface: false);
            if (anchorPanel == null)
            {
                return null;
            }

            var coordinator = new DesignTimeDocumentCoordinator(this, host, docs);
            DocumentDescriptor? descriptor = coordinator.AddNewDocument(activate: true, selectSurface: false);
            if (descriptor == null)
            {
                return null;
            }

            BeepDocumentPanel? panel = host.GetPanel(descriptor.Id);
            if (panel == null)
            {
                return null;
            }

            if (horizontal)
            {
                host.SplitDocumentHorizontal(descriptor.Id);
            }
            else
            {
                host.SplitDocumentVertical(descriptor.Id);
            }

            host.SetActiveDocument(descriptor.Id);

            if (selectSurface)
            {
                SyncDesignerSelection(panel);
            }

            return panel;
        }

        private BeepDocumentPanel? CreateRegisteredDesignPanel(BeepDocumentHost host, DocumentDescriptor descriptor, bool activate)
        {
            if (host.GetPanel(descriptor.Id) is BeepDocumentPanel existing)
            {
                // If the panel is already in DocumentPanels it was created via the
                // designer component path and is properly serialised — just activate.
                if (host.DocumentPanels.Contains(existing))
                {
                    if (activate)
                        host.SetActiveDocument(descriptor.Id);
                    return existing;
                }

                // Panel was created by the runtime AddDocument path (unsited).
                // Close it so we can recreate it as a proper designer component
                // that ends up in Designer.cs under the host's Controls collection.
                host.CloseDocument(descriptor.Id);
            }

            BeepDocumentPanel? panel = CreateDesignerComponentPanel(descriptor);
            if (panel != null)
            {
                panel.DocumentId      = descriptor.Id;
                panel.DocumentTitle   = descriptor.Title;
                panel.IconPath        = descriptor.IconPath;
                panel.CanClose        = descriptor.CanClose;
                panel.IsModified      = descriptor.IsModified;
                panel.IsPinned        = descriptor.IsPinned;
                panel.DocumentCategory = descriptor.Category;
                panel.TooltipText     = descriptor.TooltipText;
                panel.BadgeText       = descriptor.BadgeText;
                panel.BadgeColor      = descriptor.BadgeColor;
                panel.TabColor        = descriptor.TabColor;
                panel.AccentColor     = descriptor.AccentColor;

                // Add panel directly to host.Controls so it serializes to designer.cs
                // as a first-class child control that developers can interact with.
                // The host's ControlAdded event handles registration and siting.
                if (!host.Controls.Contains(panel))
                    host.Controls.Add(panel);

                if (activate)
                    host.SetActiveDocument(panel.DocumentId);

                return panel;
            }

            return null;
        }

        /// <summary>
        /// Syncs the <paramref name="managerDocuments"/> collection (owned by
        /// <see cref="BeepDocumentManager"/>) to the host's
        /// <see cref="BeepDocumentHost.DocumentPanels"/>, creating or replacing
        /// any panels that were provisioned via the runtime <c>AddDocument</c> path
        /// with proper designer components so they appear in Designer.cs.
        /// Called by <see cref="BeepDocumentManagerDesigner"/> after every
        /// <c>MutateDesignTimeDocuments</c> call.
        /// </summary>
        internal void SyncFromManagerDocuments(
            BeepDocumentHost host,
            IReadOnlyList<DocumentDescriptor> managerDocuments)
            => SyncFromManagerDocumentsCore(host, managerDocuments, createTransaction: true);

        internal void SyncFromManagerDocumentsInPlace(
            BeepDocumentHost host,
            IReadOnlyList<DocumentDescriptor> managerDocuments)
            => SyncFromManagerDocumentsCore(host, managerDocuments, createTransaction: false);

        private void SyncFromManagerDocumentsCore(
            BeepDocumentHost host,
            IReadOnlyList<DocumentDescriptor> managerDocuments,
            bool createTransaction)
        {
            if (Component != host) return;

            Action<BeepDocumentHost, Collection<DocumentDescriptor>> syncAction = (h, docs) =>
            {
                new DesignTimeDocumentCoordinator(this, h, docs)
                    .SyncDocuments(managerDocuments);
            };

            if (createTransaction)
            {
                ExecuteDesignTimeDocumentsAction("Sync Documents from Manager", syncAction);
            }
            else
            {
                ExecuteDesignTimeDocumentsActionInPlace("Sync Documents from Manager", syncAction);
            }
        }

        private BeepDocumentPanel? CreateDesignerComponentPanel(DocumentDescriptor descriptor)
        {
            IDesignerHost? designerHost = GetDesignerHost();
            if (designerHost == null)
            {
                return null;
            }

            string baseName = BuildPanelComponentName(descriptor.Id);
            for (int suffix = 0; suffix < 32; suffix++)
            {
                string name = suffix == 0 ? baseName : baseName + "_" + (suffix + 1);
                try
                {
                    return designerHost.CreateComponent(typeof(BeepDocumentPanel), name) as BeepDocumentPanel;
                }
                catch (ArgumentException)
                {
                    continue;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        internal BeepDocumentPanel? InternalEnsureDesignTimeDocumentOpen(BeepDocumentHost host, DocumentDescriptor descriptor, bool activate)
        {
            BeepDocumentPanel? panel = host.GetPanel(descriptor.Id);
            if (panel == null)
            {
                panel = CreateRegisteredDesignPanel(host, descriptor, activate: false);
            }

            if (panel != null)
            {
                ApplyDescriptorToOpenDocument(host, descriptor);

                if (activate)
                {
                    host.SetActiveDocument(descriptor.Id);
                }
            }

            return panel;
        }

        private BeepDocumentPanel? EnsureDesignTimeDocumentOpen(BeepDocumentHost host, DocumentDescriptor descriptor, bool activate)
            => InternalEnsureDesignTimeDocumentOpen(host, descriptor, activate);

        internal DocumentDescriptor InternalCreateNextDesignTimeDocumentDescriptor(BeepDocumentHost host, IEnumerable<DocumentDescriptor> docs)
        {
            var usedIds = new HashSet<string>(
                docs.Where(doc => !string.IsNullOrWhiteSpace(doc.Id)).Select(doc => doc.Id),
                StringComparer.OrdinalIgnoreCase);

            int index = 1;
            string id;
            do
            {
                id = $"doc{index}";
                index++;
            }
            while (usedIds.Contains(id) || host.GetPanel(id) != null);

            return DesignTimeDocumentCoordinator.CreateDetachedDescriptor(id, $"Document {index - 1}");
        }

        private DocumentDescriptor CreateNextDesignTimeDocumentDescriptor(BeepDocumentHost host, IEnumerable<DocumentDescriptor> docs)
            => InternalCreateNextDesignTimeDocumentDescriptor(host, docs);

        internal DocumentDescriptor? InternalFindDesignTimeDocument(IEnumerable<DocumentDescriptor> docs, string documentId)
            => docs.FirstOrDefault(doc => string.Equals(doc.Id, documentId, StringComparison.OrdinalIgnoreCase));

        private DocumentDescriptor? FindDesignTimeDocument(IEnumerable<DocumentDescriptor> docs, string documentId)
            => InternalFindDesignTimeDocument(docs, documentId);

        private DocumentDescriptor AddDescriptorSnapshotToDesignTimeDocuments(BeepDocumentHost host, Collection<DocumentDescriptor> docs, string documentId)
        {
            DocumentDescriptor descriptor = CaptureDocumentDescriptor(host, docs, documentId);
            docs.Add(descriptor);
            return descriptor;
        }

        internal DocumentDescriptor InternalCaptureDocumentDescriptor(BeepDocumentHost host, IEnumerable<DocumentDescriptor> docs, string documentId)
        {
            DocumentDescriptor? existing = InternalFindDesignTimeDocument(docs, documentId);
            if (existing != null)
            {
                return InternalCloneDescriptor(existing);
            }

            BeepDocumentPanel? panel = host.GetPanel(documentId);
            return new DocumentDescriptor
            {
                Id = documentId,
                Title = panel?.DocumentTitle ?? documentId,
                IconPath = panel?.IconPath,
                IsModified = panel?.IsModified ?? false,
                IsPinned = panel?.IsPinned ?? false,
                CanClose = panel?.CanClose ?? true,
                Category = panel?.DocumentCategory,
                TooltipText = panel?.TooltipText,
                BadgeText = panel?.BadgeText,
                BadgeColor = panel?.BadgeColor ?? System.Drawing.Color.Empty,
                TabColor = panel?.TabColor ?? System.Drawing.Color.Empty,
                AccentColor = panel?.AccentColor ?? System.Drawing.Color.Empty,
                InitialContent = DocumentInitialContent.Empty
            };
        }

        private DocumentDescriptor CaptureDocumentDescriptor(BeepDocumentHost host, IEnumerable<DocumentDescriptor> docs, string documentId)
            => InternalCaptureDocumentDescriptor(host, docs, documentId);

        internal DocumentDescriptor InternalCloneDescriptor(DocumentDescriptor source)
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

            foreach (KeyValuePair<string, string> pair in source.CustomData)
            {
                clone.CustomData[pair.Key] = pair.Value;
            }

            return clone;
        }

        private DocumentDescriptor CloneDescriptor(DocumentDescriptor source)
            => InternalCloneDescriptor(source);

        internal void InternalCopyDescriptorState(DocumentDescriptor source, DocumentDescriptor target)
        {
            target.PersistenceKey = source.PersistenceKey;
            target.PreviousGroupId = source.PreviousGroupId;
            target.Title = source.Title;
            target.IconPath = source.IconPath;
            target.IsModified = source.IsModified;
            target.IsPinned = source.IsPinned;
            target.CanClose = source.CanClose;
            target.Category = source.Category;
            target.TooltipText = source.TooltipText;
            target.Tag = source.Tag;
            target.BadgeText = source.BadgeText;
            target.BadgeColor = source.BadgeColor;
            target.TabColor = source.TabColor;
            target.AccentColor = source.AccentColor;
            target.InitialContent = source.InitialContent;

            target.CustomData.Clear();
            foreach (KeyValuePair<string, string> pair in source.CustomData)
            {
                target.CustomData[pair.Key] = pair.Value;
            }
        }

        private void CopyDescriptorState(DocumentDescriptor source, DocumentDescriptor target)
            => InternalCopyDescriptorState(source, target);

        internal bool InternalCloseDesignTimeDocument(BeepDocumentHost host, string documentId)
            => CloseDesignTimeDocument(host, documentId);
    }
}
