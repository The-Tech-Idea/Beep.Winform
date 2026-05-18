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
                AddDesignTimeDocumentInternal(h, docs, activate: true, selectSurface: true);
            });
        }

        public void CloseActiveDesignTimeDocument()
        {
            if (Component is not BeepDocumentHost host || string.IsNullOrWhiteSpace(host.ActiveDocumentId))
            {
                return;
            }

            string activeDocumentId = host.ActiveDocumentId!;
            ExecuteDesignTimeDocumentsAction($"Close Document '{activeDocumentId}'", (h, docs) =>
            {
                DocumentDescriptor snapshot = CaptureDocumentDescriptor(h, docs, activeDocumentId);
                if (!CloseDesignTimeDocument(h, activeDocumentId))
                {
                    return;
                }

                DocumentDescriptor? existing = FindDesignTimeDocument(docs, activeDocumentId);
                if (existing != null)
                {
                    docs.Remove(existing);
                }

                _designTimeClosedDocuments.Push(snapshot);
                SyncDesignerSelection((object?)h.ActivePanel ?? h);
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
                foreach (DocumentDescriptor descriptor in docs.ToList())
                {
                    if (string.IsNullOrWhiteSpace(descriptor.Id))
                    {
                        continue;
                    }

                    DocumentDescriptor snapshot = CloneDescriptor(descriptor);
                    if (CloseDesignTimeDocument(h, descriptor.Id))
                    {
                        _designTimeClosedDocuments.Push(snapshot);
                        docs.Remove(descriptor);
                    }
                }

                SyncDesignerSelection(h);
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
                if (FindDesignTimeDocument(docs, descriptor.Id) == null)
                {
                    docs.Add(descriptor);
                }

                BeepDocumentPanel? panel = EnsureDesignTimeDocumentOpen(h, descriptor, activate: true);
                SyncDesignerSelection((object?)panel ?? h);
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
        {
            if (Component is not BeepDocumentHost host)
            {
                return;
            }

            IDesignerHost?           designerHost   = GetDesignerHost();
            IComponentChangeService? changeService  = GetChangeService();
            PropertyDescriptor?      property       = GetDesignTimeDocumentsProperty();
            PropertyDescriptor?      layoutProperty = GetDesignTimeLayoutProperty();
            string                   previousLayout = host.DesignTimeLayoutJson;

            DesignerTransaction? transaction = null;
            try
            {
                transaction = designerHost?.CreateTransaction(description);
                changeService?.OnComponentChanging(host, property);
                if (layoutProperty != null)
                    changeService?.OnComponentChanging(host, layoutProperty);

                action(host, host.DesignTimeDocuments);

                string currentLayout = CaptureDesignTimeLayout(host, host.DesignTimeDocuments);
                host.DesignTimeLayoutJson = currentLayout;

                changeService?.OnComponentChanged(host, property, null, host.DesignTimeDocuments);
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

            // Phase 02: re-site any newly created panels so they appear as nested designer
            // components (selectable in the Properties window, undoable, named in the
            // component tray) immediately after every add/split/move/close action.
            SiteAllDesignPanels();

            RefreshDesignerActionUI();
        }

        // ── Document/descriptor sync helpers ─────────────────────────────────

        private void SyncHostWithDesignTimeDocuments(BeepDocumentHost host, Collection<DocumentDescriptor> docs)
        {
            var desiredDescriptors = docs
                .Where(descriptor => !string.IsNullOrWhiteSpace(descriptor.Id))
                .ToList();
            var desiredIds = new HashSet<string>(desiredDescriptors.Select(descriptor => descriptor.Id), StringComparer.OrdinalIgnoreCase);

            foreach (string openDocumentId in GetOpenDocumentIds(host))
            {
                if (!desiredIds.Contains(openDocumentId))
                {
                    CloseDesignTimeDocument(host, openDocumentId);
                }
            }

            foreach (DocumentDescriptor descriptor in desiredDescriptors)
            {
                EnsureDesignTimeDocumentOpen(host, descriptor, activate: false);
                ApplyDescriptorToOpenDocument(host, descriptor);
            }

            if (string.IsNullOrWhiteSpace(host.ActiveDocumentId) && desiredDescriptors.Count > 0)
            {
                host.SetActiveDocument(desiredDescriptors[0].Id);
            }
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

            host.ApplyDesignTimeDocuments();

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

        private void EnsureDesignDocumentCount(BeepDocumentHost host, Collection<DocumentDescriptor> docs, int count)
        {
            while (docs.Count(doc => !string.IsNullOrWhiteSpace(doc.Id)) < count)
            {
                AddDesignTimeDocumentInternal(host, docs, activate: false, selectSurface: false);
            }
        }

        private BeepDocumentPanel? AddDesignTimeDocumentInternal(BeepDocumentHost host, Collection<DocumentDescriptor> docs, bool activate, bool selectSurface)
        {
            DocumentDescriptor descriptor = CreateNextDesignTimeDocumentDescriptor(host, docs);
            BeepDocumentPanel? panel = CreateRegisteredDesignPanel(host, descriptor, activate);
            docs.Add(descriptor);

            if (selectSurface)
            {
                SyncDesignerSelection((object?)panel ?? host);
            }

            return panel;
        }

        private BeepDocumentPanel? CreateSplitDesignTimeDocumentInternal(BeepDocumentHost host, Collection<DocumentDescriptor> docs, bool horizontal, bool selectSurface)
        {
            BeepDocumentPanel? anchorPanel = EnsureActiveDesignDocumentSurface(host, docs, selectSurface: false);
            if (anchorPanel == null)
            {
                return null;
            }

            DocumentDescriptor descriptor = CreateNextDesignTimeDocumentDescriptor(host, docs);
            BeepDocumentPanel? panel = CreateRegisteredDesignPanel(host, descriptor, activate: true);
            docs.Add(descriptor);
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
                if (activate)
                {
                    host.SetActiveDocument(descriptor.Id);
                }

                return existing;
            }

            BeepDocumentPanel? panel = CreateDesignerComponentPanel(descriptor);
            if (panel != null)
            {
                panel.DocumentId = descriptor.Id;
                panel.DocumentTitle = descriptor.Title;
                panel.IconPath = descriptor.IconPath;
                panel.CanClose = descriptor.CanClose;
                panel.IsModified = descriptor.IsModified;
                host.RegisterDocumentPanel(panel, activate);
                return panel;
            }

            return host.AddDocument(descriptor.Id, descriptor.Title, descriptor.IconPath, activate);
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

        private BeepDocumentPanel? EnsureDesignTimeDocumentOpen(BeepDocumentHost host, DocumentDescriptor descriptor, bool activate)
        {
            if (host.GetPanel(descriptor.Id) == null)
            {
                host.ApplyDesignTimeDocuments();
            }

            if (activate)
            {
                host.SetActiveDocument(descriptor.Id);
            }

            return host.GetPanel(descriptor.Id);
        }

        private DocumentDescriptor CreateNextDesignTimeDocumentDescriptor(BeepDocumentHost host, IEnumerable<DocumentDescriptor> docs)
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

            return new DocumentDescriptor
            {
                Id = id,
                Title = $"Document {index - 1}",
                InitialContent = DocumentInitialContent.Empty
            };
        }

        private static DocumentDescriptor? FindDesignTimeDocument(IEnumerable<DocumentDescriptor> docs, string documentId)
            => docs.FirstOrDefault(doc => string.Equals(doc.Id, documentId, StringComparison.OrdinalIgnoreCase));

        private DocumentDescriptor AddDescriptorSnapshotToDesignTimeDocuments(BeepDocumentHost host, Collection<DocumentDescriptor> docs, string documentId)
        {
            DocumentDescriptor descriptor = CaptureDocumentDescriptor(host, docs, documentId);
            docs.Add(descriptor);
            return descriptor;
        }

        private DocumentDescriptor CaptureDocumentDescriptor(BeepDocumentHost host, IEnumerable<DocumentDescriptor> docs, string documentId)
        {
            DocumentDescriptor? existing = FindDesignTimeDocument(docs, documentId);
            if (existing != null)
            {
                return CloneDescriptor(existing);
            }

            BeepDocumentPanel? panel = host.GetPanel(documentId);
            return new DocumentDescriptor
            {
                Id = documentId,
                Title = panel?.DocumentTitle ?? documentId,
                IconPath = panel?.IconPath,
                IsModified = panel?.IsModified ?? false,
                CanClose = panel?.CanClose ?? true,
                InitialContent = DocumentInitialContent.Empty
            };
        }

        private static DocumentDescriptor CloneDescriptor(DocumentDescriptor source)
        {
            var clone = new DocumentDescriptor
            {
                Id = source.Id,
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
    }
}
