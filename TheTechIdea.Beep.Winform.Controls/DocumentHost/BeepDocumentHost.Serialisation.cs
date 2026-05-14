// BeepDocumentHost.Serialisation.cs
// SaveLayout / RestoreLayout (schema v2) for BeepDocumentHost.
//
// Schema v2 persists:
//   • Tab order, active doc, pin/modified state per group (layout tree)
//   • Split group configuration (orientation + ratio)
//   • Floating window geometry (Bounds + WindowState)
//   • Auto-hide entries (side)
//   • MRU snapshot
//
// Uses System.Text.Json (ships with .NET 6+; no extra package required).
// Older v1 payloads are automatically upgraded via LayoutMigrationService.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ─────────────────────────────────────────────────────────────────────
        // Events
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Raised for each document entry just before it is serialised into the
        /// layout JSON.  Allows consumers to attach extra data or to skip a document.
        /// </summary>
        public event EventHandler<DocumentLayoutEventArgs>? LayoutSerialising;

        /// <summary>
        /// Raised for each document entry read from the layout JSON, before
        /// <see cref="AddDocument"/> is called.  Set <see cref="DocumentLayoutEventArgs.Cancel"/>
        /// to <c>true</c> to skip re-opening that document.
        /// </summary>
        public event EventHandler<DocumentLayoutEventArgs>? LayoutRestoring;

        /// <summary>
        /// Optional callback used during layout restore to recreate a descriptor for a
        /// saved document before it is reopened.
        /// </summary>
        public Func<DocumentLayoutEventArgs, DocumentDescriptor?>? RestoreDocumentFactory { get; set; }

        // ─────────────────────────────────────────────────────────────────────
        // SaveLayout  (v2)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Serialises the full layout state (tab order, split groups, float windows,
        /// auto-hide entries, MRU snapshot) to a JSON string at schema version 2.
        /// </summary>
        public string SaveLayout()
        {
            // ── Layout tree ───────────────────────────────────────────────────
            LayoutTreeNodeDto layoutTree = BuildLayoutTreeDto();

            // ── Float windows ─────────────────────────────────────────────────
            var floats = new List<FloatWindowDto>();
            foreach (var (docId, fw) in _floatWindows)
            {
                floats.Add(new FloatWindowDto
                {
                    DocumentId  = docId,
                    Bounds      = new BoundsDto(fw.Bounds),
                    WindowState = fw.WindowState.ToString()
                });
            }

            // ── Auto-hide entries ─────────────────────────────────────────────
            var autoHides = new List<AutoHideDto>();
            foreach (var (docId, side) in GetAutoHideEntries())
                autoHides.Add(new AutoHideDto { DocumentId = docId, Side = side.ToString() });

            // ── MRU ───────────────────────────────────────────────────────────
            var mru = _mruList?.Select(id => id).ToList() ?? new List<string>();

            var snapshot = new LayoutSnapshotV2
            {
                SchemaVersion    = 4,
                ActiveDocumentId = _activeDocumentId,
                LayoutTree       = layoutTree,
                FloatingWindows  = floats,
                AutoHideEntries  = autoHides,
                MruSnapshot      = mru,
                DockPanels       = CollectDockPanelDtos()
            };

            return JsonSerializer.Serialize(snapshot, _jsonOptions);
        }

        // ─────────────────────────────────────────────────────────────────────
        // RestoreLayout  (backward-compatible entry point)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Restores a saved layout from a JSON string.  Accepts both v1 and v2 payloads;
        /// v1 payloads are automatically migrated before restore.
        /// </summary>
        /// <returns><c>true</c> if parsing and restore succeeded with no failures.</returns>
        public bool RestoreLayout(string json)
            => TryRestoreLayout(json, out _).IsSuccess;

        /// <summary>
        /// Restores a saved layout and returns a detailed <see cref="LayoutRestoreReport"/>.
        /// </summary>
        /// <param name="json">JSON string from <see cref="SaveLayout"/> (v1 or v2).</param>
        /// <param name="report">
        /// Populated with the IDs of restored, skipped, and failed documents,
        /// plus the schema version and whether migration was applied.
        /// </param>
        /// <returns>The same <paramref name="report"/> instance for fluent chaining.</returns>
        public LayoutRestoreReport TryRestoreLayout(string json, out LayoutRestoreReport report)
        {
            report = new LayoutRestoreReport();
            var sw = Stopwatch.StartNew();
            string correlationId = Guid.NewGuid().ToString("N");

            if (string.IsNullOrWhiteSpace(json))
            {
                report.Failed.Add(("(none)", "JSON string is empty."));
                EmitRestoreTelemetry(correlationId, false, sw.Elapsed.TotalMilliseconds, "JSON string is empty.");
                return report;
            }

            // ── Migrate if needed ─────────────────────────────────────────────
            string migratedJson = json;
            bool   wasMigrated  = false;
            if (!LayoutMigrationService.IsCurrentVersion(json))
            {
                try
                {
                    migratedJson = LayoutMigrationService.MigrateToLatest(json);
                    wasMigrated  = true;
                }
                catch (Exception ex)
                {
                    report.Failed.Add(("(all)", $"Migration failed: {ex.Message}"));
                    EmitRestoreTelemetry(correlationId, false, sw.Elapsed.TotalMilliseconds, ex.Message);
                    return report;
                }
            }

            // ── Deserialise ───────────────────────────────────────────────────
            LayoutSnapshotV2? snapshot;
            try
            {
                snapshot = JsonSerializer.Deserialize<LayoutSnapshotV2>(migratedJson, _jsonOptions);
            }
            catch (JsonException ex)
            {
                report.Failed.Add(("(all)", $"JSON parse error: {ex.Message}"));
                EmitRestoreTelemetry(correlationId, false, sw.Elapsed.TotalMilliseconds, ex.Message);
                return report;
            }

            if (snapshot == null)
            {
                report.Failed.Add(("(all)", "Deserialised snapshot is null."));
                EmitRestoreTelemetry(correlationId, false, sw.Elapsed.TotalMilliseconds, "Deserialised snapshot is null.");
                return report;
            }

            if (snapshot.SchemaVersion <= 0)
            {
                report.Failed.Add(("(all)", "Invalid schema version."));
                EmitRestoreTelemetry(correlationId, false, sw.Elapsed.TotalMilliseconds, "Invalid schema version.");
                return report;
            }

            report.SchemaVersion = snapshot.SchemaVersion;
            report.WasMigrated   = wasMigrated;

            ILayoutNode? layoutTree = snapshot.LayoutTree != null
                ? BuildLayoutNode(snapshot.LayoutTree)
                : null;

            var floatingIds = new HashSet<string>(
                snapshot.FloatingWindows?
                    .Where(fw => !string.IsNullOrWhiteSpace(fw.DocumentId))
                    .Select(fw => fw.DocumentId!)
                ?? Enumerable.Empty<string>(),
                StringComparer.Ordinal);

            var autoHideLookup = new Dictionary<string, AutoHideSide>(StringComparer.Ordinal);
            foreach (var ah in snapshot.AutoHideEntries ?? Enumerable.Empty<AutoHideDto>())
            {
                if (string.IsNullOrWhiteSpace(ah.DocumentId))
                    continue;

                if (Enum.TryParse<AutoHideSide>(ah.Side, out var side))
                    autoHideLookup[ah.DocumentId!] = side;
            }

            // ── Restore tab groups ────────────────────────────────────────────
            if (snapshot.LayoutTree != null)
                RestoreLayoutTreeNode(snapshot.LayoutTree, report, floatingIds, autoHideLookup);

            if (layoutTree != null)
            {
                var layoutReport = LayoutTreeApplier.Apply(this, layoutTree);
                foreach (var missingId in layoutReport.MissingDocumentIds)
                {
                    if (!report.Failed.Any(f => string.Equals(f.Id, missingId, StringComparison.Ordinal)))
                        report.Failed.Add((missingId, "Layout tree apply could not place the restored document."));
                }

                foreach (var warning in layoutReport.Warnings)
                    report.Failed.Add(("(layout)", warning));
            }

            // ── Restore float windows ─────────────────────────────────────────
            foreach (var fw in snapshot.FloatingWindows ?? Enumerable.Empty<FloatWindowDto>())
            {
                if (string.IsNullOrEmpty(fw.DocumentId)) continue;
                try
                {
                    if (_panels.TryGetValue(fw.DocumentId, out var panel) && fw.Bounds != null)
                    {
                        FloatDocument(fw.DocumentId);
                        if (_floatWindows.TryGetValue(fw.DocumentId, out var floatWin))
                        {
                            floatWin.Bounds = fw.Bounds.ToRectangle();
                            if (Enum.TryParse<FormWindowState>(fw.WindowState, out var ws))
                                floatWin.WindowState = ws;
                        }
                    }
                }
                catch (Exception ex)
                {
                    report.Failed.Add((fw.DocumentId, $"Float restore: {ex.Message}"));
                }
            }

            // ── Restore auto-hide ─────────────────────────────────────────────
            foreach (var (documentId, side) in autoHideLookup)
            {
                try
                {
                    if (_panels.ContainsKey(documentId))
                        AutoHideDocument(documentId, side);
                }
                catch (Exception ex)
                {
                    report.Failed.Add((documentId, $"Auto-hide restore: {ex.Message}"));
                }
            }

            // ── Restore active document ───────────────────────────────────────
            if (!string.IsNullOrEmpty(snapshot.ActiveDocumentId)
                && _panels.ContainsKey(snapshot.ActiveDocumentId))
            {
                SetActiveDocument(snapshot.ActiveDocumentId);
            }
            else if (_panels.Count > 0)
            {
                SetActiveDocument(_panels.Keys.First());
            }

            // ── Restore dock panels ───────────────────────────────────────────
            RestoreDockPanels(snapshot.DockPanels);

            EmitRestoreTelemetry(correlationId, report.IsSuccess, sw.Elapsed.TotalMilliseconds, report.IsSuccess ? null : "Restore completed with failures.");
            return report;
        }

        private void EmitRestoreTelemetry(string correlationId, bool success, double durationMs, string? error)
        {
            if (!EnableHostTelemetry) return;
            Profiler.Emit(new DocumentHostTelemetryEvent
            {
                CorrelationId = correlationId,
                OperationType = DocumentHostOperationType.RestoreLayout,
                EventName = "layout.restore",
                Success = success,
                DurationMs = durationMs,
                Error = error
            });
        }

        // ─────────────────────────────────────────────────────────────────────
        // Dock-panel serialisation helpers
        // ─────────────────────────────────────────────────────────────────────

        private List<DockPanelDto> CollectDockPanelDtos()
        {
            var result = new List<DockPanelDto>();
            // Walk all BeepDockManager instances whose Host is this DocumentHost.
            foreach (var mgr in BeepDockManager.FindManagersForHost(this))
            {
                foreach (var desc in mgr.Panels)
                {
                    result.Add(new DockPanelDto
                    {
                        PersistenceKey = desc.PersistenceKey,
                        Edge           = desc.Edge.ToString(),
                        SizePercent    = desc.SizePercent,
                        IsAutoHidden   = desc.IsAutoHidden,
                        IsVisible      = desc.IsVisible
                    });
                }
            }
            return result;
        }

        private void RestoreDockPanels(List<DockPanelDto>? dtos)
        {
            if (dtos is null || dtos.Count == 0) return;

            foreach (var mgr in BeepDockManager.FindManagersForHost(this))
            {
                foreach (var dto in dtos)
                {
                    if (string.IsNullOrWhiteSpace(dto.PersistenceKey)) continue;

                    var desc = mgr.Panels.FirstOrDefault(p =>
                        string.Equals(p.PersistenceKey, dto.PersistenceKey, StringComparison.Ordinal));

                    if (desc is null) continue;

                    if (Enum.TryParse<DockEdge>(dto.Edge, out var edge))
                        desc.Edge = edge;

                    desc.SizePercent  = dto.SizePercent;
                    desc.IsAutoHidden = dto.IsAutoHidden;
                    desc.IsVisible    = dto.IsVisible;

                    // Re-apply the restored state
                    if (!desc.IsVisible)
                        mgr.HidePanel(desc.Id);
                    else if (desc.IsAutoHidden)
                        mgr.AutoHidePanel(desc.Id);
                    else
                        mgr.DockPanel(desc.Id, desc.Edge);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // RestoreLayoutAsync  (5.5 — non-blocking restore for 100+ docs)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Asynchronously restores a saved layout.  The actual restore runs on the UI
        /// message queue via <see cref="Control.BeginInvoke"/> so the calling thread is
        /// never blocked.  An optional <paramref name="progress"/> callback receives
        /// 100 when the restore completes.
        /// </summary>
        public Task<LayoutRestoreReport> RestoreLayoutAsync(
            string json,
            IProgress<int>? progress = null,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<LayoutRestoreReport>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            using var reg = cancellationToken.Register(
                () => tcs.TrySetCanceled(cancellationToken));

            void DoRestore()
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var r = TryRestoreLayout(json, out _);
                    progress?.Report(100);
                    tcs.TrySetResult(r);
                }
                catch (OperationCanceledException) { tcs.TrySetCanceled(cancellationToken); }
                catch (Exception ex)               { tcs.TrySetException(ex); }
            }

            if (IsHandleCreated)
                BeginInvoke(new Action(DoRestore));
            else
                DoRestore();

            return tcs.Task;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Session file helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Saves the current layout to a file.
        /// If <paramref name="filePath"/> is null, <see cref="SessionFile"/> is used.
        /// </summary>
        public void SaveLayoutToFile(string? filePath = null)
        {
            var path = filePath ?? _sessionFile;
            if (string.IsNullOrWhiteSpace(path)) return;
            try { System.IO.File.WriteAllText(path, SaveLayout()); }
            catch { /* swallow I/O errors silently */ }
        }

        /// <summary>
        /// Restores a saved layout from a file.
        /// If <paramref name="filePath"/> is null, <see cref="SessionFile"/> is used.
        /// </summary>
        public bool RestoreLayoutFromFile(string? filePath = null)
        {
            var path = filePath ?? _sessionFile;
            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path)) return false;
            try { return RestoreLayout(System.IO.File.ReadAllText(path)); }
            catch { return false; }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Named workspace support (Track D)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Saves the current layout under a named workspace alongside the
        /// <see cref="BeepDocumentHost.SessionFile"/> base path.
        /// Files are written as <c>{SessionFile}.{name}.json</c>.
        /// </summary>
        /// <param name="name">Workspace name (must be a valid file-name segment).</param>
        /// <returns>
        /// The full file path that was written, or <c>null</c> if
        /// <see cref="BeepDocumentHost.SessionFile"/> is not configured.
        /// </returns>
        public string? SaveWorkspace(string name)
        {
            if (string.IsNullOrWhiteSpace(_sessionFile)) return null;
            var path = WorkspacePath(name);
            try { System.IO.File.WriteAllText(path, SaveLayout()); return path; }
            catch { return null; }
        }

        /// <summary>
        /// Restores a named workspace.
        /// </summary>
        /// <param name="name">Workspace name previously saved via <see cref="SaveWorkspace"/>.</param>
        /// <returns><c>true</c> if the file was found and restored without failures.</returns>
        public bool LoadWorkspace(string name)
        {
            if (string.IsNullOrWhiteSpace(_sessionFile)) return false;
            var path = WorkspacePath(name);
            if (!System.IO.File.Exists(path)) return false;
            try { return RestoreLayout(System.IO.File.ReadAllText(path)); }
            catch { return false; }
        }

        /// <summary>
        /// Deletes a named workspace file, if it exists.
        /// </summary>
        public void DeleteWorkspace(string name)
        {
            if (string.IsNullOrWhiteSpace(_sessionFile)) return;
            var path = WorkspacePath(name);
            try { if (System.IO.File.Exists(path)) System.IO.File.Delete(path); }
            catch { /* swallow */ }
        }

        /// <summary>
        /// Returns the names of all saved workspaces associated with the current
        /// <see cref="BeepDocumentHost.SessionFile"/> base path.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<string> ListWorkspaces()
        {
            if (string.IsNullOrWhiteSpace(_sessionFile))
                return System.Array.Empty<string>();

            var dir  = System.IO.Path.GetDirectoryName(_sessionFile) ?? ".";
            var stem = System.IO.Path.GetFileNameWithoutExtension(_sessionFile);
            var ext  = System.IO.Path.GetExtension(_sessionFile);      // e.g. ".json"
            var suffix = ext + ".workspace";                            // e.g. ".json.workspace"

            var names = new System.Collections.Generic.List<string>();
            try
            {
                foreach (var file in System.IO.Directory.EnumerateFiles(dir, $"{stem}.*.workspace"))
                {
                    // File name: {stem}.{name}.workspace
                    var fn   = System.IO.Path.GetFileName(file);
                    var name = fn.Substring(stem.Length + 1,
                                            fn.Length - stem.Length - 1 - ".workspace".Length);
                    if (!string.IsNullOrWhiteSpace(name)) names.Add(name);
                }
            }
            catch { /* swallow */ }
            return names;
        }

        /// <summary>Builds the file path for a named workspace.</summary>
        private string WorkspacePath(string name)
        {
            var dir  = System.IO.Path.GetDirectoryName(_sessionFile) ?? ".";
            var stem = System.IO.Path.GetFileNameWithoutExtension(_sessionFile);
            // e.g. "C:\App\session.json" → "C:\App\session.MyLayout.workspace"
            var safeName = string.Concat(name.Split(System.IO.Path.GetInvalidFileNameChars()));
            return System.IO.Path.Combine(dir, $"{stem}.{safeName}.workspace");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private: build layout tree DTO from live group state
        // ─────────────────────────────────────────────────────────────────────

        private LayoutTreeNodeDto BuildLayoutTreeDto()
        {
            if (_groups.Count == 1)
                return BuildGroupDto(_groups[0]);

            // Two-group flat split (current Phase 1 topology) — wrap in a single split node
            var split = new LayoutTreeNodeDto
            {
                Type        = "split",
                NodeId      = Guid.NewGuid().ToString(),
                Orientation = _splitHorizontal ? "Horizontal" : "Vertical",
                Ratio       = _splitRatio,
                Children    = new List<LayoutTreeNodeDto>
                {
                    BuildGroupDto(_groups[0]),
                    _groups.Count > 1 ? BuildGroupDto(_groups[1]) : BuildEmptyGroupDto()
                }
            };

            return split;
        }

        private LayoutTreeNodeDto BuildGroupDto(BeepDocumentGroup grp)
        {
            var docList = new List<TabLayoutEntryDto>();
            foreach (var docId in grp.DocumentIds)
            {
                if (!TryGetLayoutDocumentSnapshot(docId, out var panel, out var dockState, out var side)
                    || panel == null)
                    continue;

                var tab = grp.TabStrip.Tabs.FirstOrDefault(t => t.Id == docId);
                string title = tab?.Title ?? panel.DocumentTitle;
                string? iconPath = tab?.IconPath ?? panel.IconPath;
                bool isPinned = tab?.IsPinned ?? false;
                // Stable persistence key + previous group — from descriptor if available (P7-007, P4-002)
                var docDesc = _documents?.FirstOrDefault(d => d.Id == docId);
                string? persistenceKey  = docDesc?.PersistenceKey;
                string? previousGroupId = docDesc?.PreviousGroupId;
                var evArgs = new DocumentLayoutEventArgs(
                    docId, title, iconPath,
                    isPinned, panel.IsModified,
                    dockState, grp.GroupId, side,
                    persistenceKey: persistenceKey);
                LayoutSerialising?.Invoke(this, evArgs);
                if (evArgs.Cancel) continue;

                docList.Add(new TabLayoutEntryDto
                {
                    Id              = docId,
                    PersistenceKey  = persistenceKey,
                    PreviousGroupId = previousGroupId,
                    Title           = title,
                    IconPath        = iconPath,
                    IsPinned        = isPinned,
                    IsModified      = panel.IsModified,
                    CustomData      = evArgs.CustomData
                });
            }

            return new LayoutTreeNodeDto
            {
                Type               = "tabGroup",
                NodeId             = grp.GroupId,
                GroupId            = grp.GroupId,
                SelectedDocumentId = grp.DocumentIds.FirstOrDefault(id => id == _activeDocumentId)
                                     ?? grp.DocumentIds.FirstOrDefault(),
                Documents          = docList
            };
        }

        private static LayoutTreeNodeDto BuildEmptyGroupDto() => new()
        {
            Type      = "tabGroup",
            NodeId    = Guid.NewGuid().ToString(),
            GroupId   = Guid.NewGuid().ToString(),
            Documents = new List<TabLayoutEntryDto>()
        };

        private static ILayoutNode BuildLayoutNode(LayoutTreeNodeDto node)
        {
            if (string.Equals(node.Type, "split", StringComparison.OrdinalIgnoreCase))
            {
                var children = node.Children ?? new List<LayoutTreeNodeDto>();
                ILayoutNode first = children.Count > 0
                    ? BuildLayoutNode(children[0])
                    : new GroupLayoutNode();
                ILayoutNode second = children.Count > 1
                    ? BuildLayoutNode(children[1])
                    : new GroupLayoutNode();

                var orientation = string.Equals(node.Orientation, "Vertical", StringComparison.OrdinalIgnoreCase)
                    ? Orientation.Vertical
                    : Orientation.Horizontal;

                return new SplitLayoutNode(node.NodeId ?? Guid.NewGuid().ToString(),
                                           first,
                                           second,
                                           orientation,
                                           node.Ratio);
            }

            var documentIds = (node.Documents ?? Enumerable.Empty<TabLayoutEntryDto>())
                .Where(doc => !string.IsNullOrWhiteSpace(doc.Id))
                .Select(doc => doc.Id)
                .ToList();

            return new GroupLayoutNode(node.NodeId ?? Guid.NewGuid().ToString(),
                                       node.GroupId ?? node.NodeId ?? Guid.NewGuid().ToString(),
                                       documentIds,
                                       node.SelectedDocumentId);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private: restore from layout tree DTO
        // ─────────────────────────────────────────────────────────────────────

        private bool TryGetLayoutDocumentSnapshot(string documentId,
                                                  out BeepDocumentPanel? panel,
                                                  out DocumentDockState dockState,
                                                  out AutoHideSide? side)
        {
            if (_floatWindows.TryGetValue(documentId, out var floatWindow)
                && floatWindow.HostedPanel != null)
            {
                panel = floatWindow.HostedPanel;
                dockState = DocumentDockState.Floating;
                side = null;
                return true;
            }

            if (_panels.TryGetValue(documentId, out var openPanel))
            {
                panel = openPanel;
                if (_autoHideMap.TryGetValue(documentId, out var autoHideSide))
                {
                    dockState = DocumentDockState.AutoHide;
                    side = autoHideSide;
                }
                else
                {
                    dockState = DocumentDockState.Docked;
                    side = null;
                }

                return true;
            }

            panel = null;
            dockState = DocumentDockState.None;
            side = null;
            return false;
        }

        private void RestoreLayoutTreeNode(LayoutTreeNodeDto node,
                                           LayoutRestoreReport report,
                                           HashSet<string> floatingIds,
                                           Dictionary<string, AutoHideSide> autoHideLookup)
        {
            if (node.Type == "tabGroup")
            {
                RestoreGroupNode(node, report, floatingIds, autoHideLookup);
                return;
            }

            if (node.Type == "split" && node.Children != null)
            {
                foreach (var child in node.Children)
                    RestoreLayoutTreeNode(child, report, floatingIds, autoHideLookup);

                // Apply split orientation + ratio to host
                if (node.Children.Count >= 2)
                {
                    _splitHorizontal = node.Orientation != "Vertical";
                    _splitRatio      = Math.Max(0.1f, Math.Min(0.9f, node.Ratio));
                }
            }
        }

        private static DocumentDescriptor CreateRestoreDescriptor(DocumentLayoutEventArgs evArgs)
        {
            var descriptor = DocumentDescriptor.Create(
                evArgs.DocumentId,
                string.IsNullOrWhiteSpace(evArgs.Title) ? evArgs.DocumentId : evArgs.Title,
                evArgs.IconPath);

            ApplyRestoreDescriptorState(descriptor, evArgs);
            return descriptor;
        }

        private static void ApplyRestoreDescriptorState(DocumentDescriptor descriptor,
                                                        DocumentLayoutEventArgs evArgs)
        {
            descriptor.Id = evArgs.DocumentId;

            if (string.IsNullOrWhiteSpace(descriptor.Title))
                descriptor.Title = string.IsNullOrWhiteSpace(evArgs.Title)
                    ? evArgs.DocumentId
                    : evArgs.Title;

            if (string.IsNullOrWhiteSpace(descriptor.IconPath)
                && !string.IsNullOrWhiteSpace(evArgs.IconPath))
            {
                descriptor.IconPath = evArgs.IconPath;
            }

            descriptor.IsPinned = evArgs.IsPinned;
            descriptor.IsModified = evArgs.IsModified;

            if (descriptor.Tag == null && evArgs.Tag != null)
                descriptor.Tag = evArgs.Tag;

            foreach (var kv in evArgs.CustomData)
            {
                if (!descriptor.CustomData.ContainsKey(kv.Key))
                    descriptor.CustomData[kv.Key] = kv.Value;
            }
        }

        private void RestoreGroupNode(LayoutTreeNodeDto grpNode,
                                      LayoutRestoreReport report,
                                      HashSet<string> floatingIds,
                                      Dictionary<string, AutoHideSide> autoHideLookup)
        {
            foreach (var entry in grpNode.Documents ?? Enumerable.Empty<TabLayoutEntryDto>())
            {
                if (string.IsNullOrEmpty(entry.Id)) continue;

                DocumentDockState dockState = DocumentDockState.Docked;
                AutoHideSide? side = null;
                if (autoHideLookup.TryGetValue(entry.Id, out var autoHideSide))
                {
                    dockState = DocumentDockState.AutoHide;
                    side = autoHideSide;
                }
                else if (floatingIds.Contains(entry.Id))
                {
                    dockState = DocumentDockState.Floating;
                }

                var evArgs = new DocumentLayoutEventArgs(
                    entry.Id, entry.Title ?? string.Empty, entry.IconPath,
                    entry.IsPinned, entry.IsModified,
                    dockState, grpNode.GroupId, side,
                    persistenceKey: entry.PersistenceKey);
                if (entry.CustomData != null)
                    foreach (var kv in entry.CustomData)
                        evArgs.CustomData[kv.Key] = kv.Value;

                if (RestoreDocumentFactory != null)
                    evArgs.RestoreDescriptor ??= RestoreDocumentFactory(evArgs);

                LayoutRestoring?.Invoke(this, evArgs);

                if (evArgs.Cancel) { report.Skipped.Add(entry.Id); continue; }

                if (ContainsOpenDocument(entry.Id)) { report.Skipped.Add(entry.Id); continue; }

                try
                {
                    var descriptor = evArgs.RestoreDescriptor ?? CreateRestoreDescriptor(evArgs);
                    ApplyRestoreDescriptorState(descriptor, evArgs);

                    // Restore PreviousGroupId hint so post-restore moves can be intelligent (P4-002)
                    if (!string.IsNullOrEmpty(entry.PreviousGroupId))
                        descriptor.PreviousGroupId = entry.PreviousGroupId;

                    OpenDocumentCore(
                        descriptor,
                        new DocumentOpenOptions
                        {
                            Target = DocumentOpenTarget.SpecificGroup,
                            TargetGroupId = grpNode.GroupId,
                            Activate = false
                        },
                        attachDescriptorChanges: false,
                        raiseDocumentAddedEvent: false);

                    report.Restored.Add(entry.Id);
                }
                catch (Exception ex)
                {
                    report.Failed.Add((entry.Id, ex.Message));
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private helpers: auto-hide enumeration
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns all currently auto-hidden documents with their side.
        /// Derived from the AutoHide partial's internal data structures.
        /// </summary>
        private IEnumerable<(string DocumentId, AutoHideSide Side)> GetAutoHideEntries()
        {
            // _autoHideMap is defined in BeepDocumentHost.AutoHide.cs as
            // Dictionary<string, AutoHideSide>.
            if (_autoHideMap == null) yield break;
            foreach (var kv in _autoHideMap)
                yield return (kv.Key, kv.Value);
        }

        // ─────────────────────────────────────────────────────────────────────
        // JSON options (shared, created once)
        // ─────────────────────────────────────────────────────────────────────

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented           = true,
            DefaultIgnoreCondition  = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy    = JsonNamingPolicy.CamelCase
        };

        // ─────────────────────────────────────────────────────────────────────
        // Private DTO types — schema v2
        // ─────────────────────────────────────────────────────────────────────

        private sealed class LayoutSnapshotV2
        {
            public int                   SchemaVersion    { get; set; }
            public string?               ActiveDocumentId { get; set; }
            public LayoutTreeNodeDto?    LayoutTree       { get; set; }
            public List<FloatWindowDto>? FloatingWindows  { get; set; }
            public List<AutoHideDto>?    AutoHideEntries  { get; set; }
            public List<string>?         MruSnapshot      { get; set; }
            public List<DockPanelDto>?   DockPanels       { get; set; }
        }

        private sealed class LayoutTreeNodeDto
        {
            /// <summary>"tabGroup" | "split"</summary>
            public string              Type               { get; set; } = "tabGroup";
            public string?             NodeId             { get; set; }
            public string?             GroupId            { get; set; }
            public string?             SelectedDocumentId { get; set; }
            /// <summary>For split nodes: "Horizontal" | "Vertical"</summary>
            public string?             Orientation        { get; set; }
            /// <summary>For split nodes: first-child fraction (0–1).</summary>
            public float               Ratio              { get; set; } = 0.5f;
            public List<TabLayoutEntryDto>?  Documents    { get; set; }
            public List<LayoutTreeNodeDto>?  Children     { get; set; }
        }

        private sealed class TabLayoutEntryDto
        {
            public string  Id              { get; set; } = string.Empty;
            public string? PersistenceKey  { get; set; }
            public string? PreviousGroupId { get; set; }
            public string? Title           { get; set; }
            public string? IconPath        { get; set; }
            public bool    IsPinned        { get; set; }
            public bool    IsModified      { get; set; }
            public Dictionary<string, string>? CustomData { get; set; }
        }

        private sealed class FloatWindowDto
        {
            public string?    DocumentId  { get; set; }
            public BoundsDto? Bounds      { get; set; }
            public string?    WindowState { get; set; }
        }

        private sealed class AutoHideDto
        {
            public string? DocumentId { get; set; }
            public string? Side       { get; set; }
        }

        private sealed class DockPanelDto
        {
            public string? PersistenceKey { get; set; }
            public string? Edge           { get; set; }
            public double  SizePercent    { get; set; }
            public bool    IsAutoHidden   { get; set; }
            public bool    IsVisible      { get; set; }
        }

        private sealed class BoundsDto
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int W { get; set; }
            public int H { get; set; }

            public BoundsDto() { }
            public BoundsDto(Rectangle r) { X = r.X; Y = r.Y; W = r.Width; H = r.Height; }
            public Rectangle ToRectangle() => new Rectangle(X, Y, W, H);
        }
    }
}
