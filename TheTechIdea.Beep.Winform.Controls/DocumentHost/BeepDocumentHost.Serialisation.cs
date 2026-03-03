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
using System.Windows.Forms;
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
                SchemaVersion    = 2,
                ActiveDocumentId = _activeDocumentId,
                LayoutTree       = layoutTree,
                FloatingWindows  = floats,
                AutoHideEntries  = autoHides,
                MruSnapshot      = mru
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

            if (string.IsNullOrWhiteSpace(json))
            {
                report.Failed.Add(("(none)", "JSON string is empty."));
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
                return report;
            }

            if (snapshot == null)
            {
                report.Failed.Add(("(all)", "Deserialised snapshot is null."));
                return report;
            }

            report.SchemaVersion = snapshot.SchemaVersion;
            report.WasMigrated   = wasMigrated;

            // ── Restore tab groups ────────────────────────────────────────────
            if (snapshot.LayoutTree != null)
                RestoreLayoutTreeNode(snapshot.LayoutTree, report);

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
            foreach (var ah in snapshot.AutoHideEntries ?? Enumerable.Empty<AutoHideDto>())
            {
                if (string.IsNullOrEmpty(ah.DocumentId)) continue;
                try
                {
                    if (_panels.ContainsKey(ah.DocumentId))
                    {
                        if (Enum.TryParse<AutoHideSide>(ah.Side, out var side))
                            AutoHideDocument(ah.DocumentId, side);
                    }
                }
                catch (Exception ex)
                {
                    report.Failed.Add((ah.DocumentId, $"Auto-hide restore: {ex.Message}"));
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

            return report;
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
                if (!_panels.TryGetValue(docId, out var panel)) continue;

                var tab = grp.TabStrip.Tabs.FirstOrDefault(t => t.Id == docId);
                var evArgs = new DocumentLayoutEventArgs(
                    docId, tab?.Title ?? string.Empty, tab?.IconPath,
                    tab?.IsPinned ?? false, panel.IsModified);
                LayoutSerialising?.Invoke(this, evArgs);
                if (evArgs.Cancel) continue;

                docList.Add(new TabLayoutEntryDto
                {
                    Id         = docId,
                    Title      = tab?.Title ?? string.Empty,
                    IconPath   = tab?.IconPath,
                    IsPinned   = tab?.IsPinned ?? false,
                    IsModified = panel.IsModified,
                    CustomData = evArgs.CustomData
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

        // ─────────────────────────────────────────────────────────────────────
        // Private: restore from layout tree DTO
        // ─────────────────────────────────────────────────────────────────────

        private void RestoreLayoutTreeNode(LayoutTreeNodeDto node, LayoutRestoreReport report)
        {
            if (node.Type == "tabGroup")
            {
                RestoreGroupNode(node, report);
                return;
            }

            if (node.Type == "split" && node.Children != null)
            {
                foreach (var child in node.Children)
                    RestoreLayoutTreeNode(child, report);

                // Apply split orientation + ratio to host
                if (node.Children.Count >= 2)
                {
                    _splitHorizontal = node.Orientation != "Vertical";
                    _splitRatio      = Math.Max(0.1f, Math.Min(0.9f, node.Ratio));
                }
            }
        }

        private void RestoreGroupNode(LayoutTreeNodeDto grpNode, LayoutRestoreReport report)
        {
            foreach (var entry in grpNode.Documents ?? Enumerable.Empty<TabLayoutEntryDto>())
            {
                if (string.IsNullOrEmpty(entry.Id)) continue;

                var evArgs = new DocumentLayoutEventArgs(
                    entry.Id, entry.Title ?? string.Empty, entry.IconPath,
                    entry.IsPinned, entry.IsModified);
                if (entry.CustomData != null)
                    foreach (var kv in entry.CustomData)
                        evArgs.CustomData[kv.Key] = kv.Value;

                LayoutRestoring?.Invoke(this, evArgs);

                if (evArgs.Cancel) { report.Skipped.Add(entry.Id); continue; }

                if (_panels.ContainsKey(entry.Id)) { report.Skipped.Add(entry.Id); continue; }

                try
                {
                    var panel = AddDocument(entry.Id, entry.Title ?? entry.Id,
                                            entry.IconPath, activate: false);
                    if (entry.IsPinned)  PinDocument(entry.Id, true);
                    if (entry.IsModified) panel.IsModified = true;
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
            public string  Id         { get; set; } = string.Empty;
            public string? Title      { get; set; }
            public string? IconPath   { get; set; }
            public bool    IsPinned   { get; set; }
            public bool    IsModified { get; set; }
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
