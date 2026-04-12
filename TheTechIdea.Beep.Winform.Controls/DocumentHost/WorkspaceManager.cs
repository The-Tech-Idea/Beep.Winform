// WorkspaceManager.cs
// In-memory + file-persisted workspace store for BeepDocumentHost.
// Manages a WorkspaceManifest (list of WorkspaceDefinition) with optional
// auto-save to a directory on disk (workspaces.json).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Manages named workspace snapshots for a <see cref="BeepDocumentHost"/> instance.
    /// Supports in-memory operation and optional file persistence to a chosen directory.
    /// </summary>
    public sealed class WorkspaceManager
    {
        private const string ManifestFileName = "workspaces.json";

        private static readonly JsonSerializerOptions _json = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private WorkspaceManifest _manifest = new WorkspaceManifest();
        private string? _persistDirectory;

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Raised after a workspace is saved or updated.</summary>
        public event EventHandler<WorkspaceEventArgs>? WorkspaceSaved;

        /// <summary>Raised after a workspace is deleted.</summary>
        public event EventHandler<WorkspaceEventArgs>? WorkspaceDeleted;

        // ── Configuration ─────────────────────────────────────────────────────

        /// <summary>
        /// Directory where <c>workspaces.json</c> is read from / written to.
        /// Set to <c>null</c> to keep workspaces in memory only (default).
        /// Changing this value triggers an immediate load attempt from the new path.
        /// </summary>
        public string? PersistDirectory
        {
            get => _persistDirectory;
            set
            {
                _persistDirectory = value;
                if (!string.IsNullOrEmpty(value))
                    LoadFromDisk();
            }
        }

        /// <summary>Id of the currently active workspace, or <c>null</c> when none is active.</summary>
        public string? ActiveWorkspaceId => _manifest.ActiveWorkspaceId;

        // ── Read API ──────────────────────────────────────────────────────────

        /// <summary>Returns all defined workspaces in display order.</summary>
        public IReadOnlyList<WorkspaceDefinition> GetAll()
            => _manifest.Workspaces.AsReadOnly();

        /// <summary>Returns the workspace with the given id, or <c>null</c> if not found.</summary>
        public WorkspaceDefinition? FindById(string id)
        {
            ArgumentNullException.ThrowIfNull(id);
            return _manifest.Workspaces.Find(w => w.Id == id);
        }

        /// <summary>Returns the workspace with the given name (case-insensitive), or <c>null</c>.</summary>
        public WorkspaceDefinition? FindByName(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            return _manifest.Workspaces.Find(w =>
                string.Equals(w.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Returns the currently active workspace, or <c>null</c>.</summary>
        public WorkspaceDefinition? GetActive()
            => string.IsNullOrEmpty(_manifest.ActiveWorkspaceId)
               ? null
               : FindById(_manifest.ActiveWorkspaceId);

        // ── Write API ─────────────────────────────────────────────────────────

        /// <summary>
        /// Saves (creates or overwrites) a workspace with the given name.
        /// If a workspace with that name already exists it is updated in place;
        /// otherwise a new entry is appended.
        /// </summary>
        /// <param name="name">Display name for the workspace.</param>
        /// <param name="layoutJson">JSON from <see cref="BeepDocumentHost.SaveLayout"/>.</param>
        /// <param name="description">Optional description.</param>
        /// <param name="tags">Optional metadata tags.</param>
        /// <returns>The saved <see cref="WorkspaceDefinition"/>.</returns>
        public WorkspaceDefinition Save(string name, string layoutJson,
            string description = "",
            IDictionary<string, string>? tags = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Workspace name must not be empty.", nameof(name));

            var existing = FindByName(name);
            if (existing != null)
            {
                existing.LayoutJson  = layoutJson;
                existing.Description = description;
                existing.SavedAt     = DateTime.UtcNow;
                if (tags != null)
                {
                    existing.Tags.Clear();
                    foreach (var kv in tags) existing.Tags[kv.Key] = kv.Value;
                }
                SaveToDisk();
                WorkspaceSaved?.Invoke(this, new WorkspaceEventArgs(existing));
                return existing;
            }

            var ws = new WorkspaceDefinition
            {
                Name        = name,
                LayoutJson  = layoutJson,
                Description = description,
                SavedAt     = DateTime.UtcNow
            };
            if (tags != null)
                foreach (var kv in tags) ws.Tags[kv.Key] = kv.Value;

            _manifest.Workspaces.Add(ws);
            SaveToDisk();
            WorkspaceSaved?.Invoke(this, new WorkspaceEventArgs(ws));
            return ws;
        }

        /// <summary>Marks the workspace with the given id as active (in-memory only — caller must call SaveLayout on the host).</summary>
        public void SetActive(string id)
        {
            if (!string.IsNullOrEmpty(id) && FindById(id) == null)
                throw new InvalidOperationException($"Workspace '{id}' not found.");
            _manifest.ActiveWorkspaceId = id;
            SaveToDisk();
        }

        /// <summary>Renames a workspace.</summary>
        public void Rename(string id, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("New name must not be empty.", nameof(newName));

            var ws = FindById(id)
                ?? throw new InvalidOperationException($"Workspace '{id}' not found.");

            if (FindByName(newName) is { } clash && clash.Id != id)
                throw new InvalidOperationException($"A workspace named '{newName}' already exists.");

            ws.Name = newName;
            SaveToDisk();
        }

        /// <summary>Deletes the workspace with the given id.</summary>
        /// <returns><c>true</c> if found and removed; <c>false</c> if not found.</returns>
        public bool Delete(string id)
        {
            ArgumentNullException.ThrowIfNull(id);
            var ws = FindById(id);
            if (ws == null) return false;

            _manifest.Workspaces.Remove(ws);
            if (_manifest.ActiveWorkspaceId == id)
                _manifest.ActiveWorkspaceId = null;

            SaveToDisk();
            WorkspaceDeleted?.Invoke(this, new WorkspaceEventArgs(ws));
            return true;
        }

        // ── Persistence ───────────────────────────────────────────────────────

        private void SaveToDisk()
        {
            if (string.IsNullOrEmpty(_persistDirectory)) return;
            try
            {
                Directory.CreateDirectory(_persistDirectory);
                string path = Path.Combine(_persistDirectory, ManifestFileName);
                string json = JsonSerializer.Serialize(_manifest, _json);
                File.WriteAllText(path, json);
            }
            catch (IOException) { /* non-fatal: in-memory state is still consistent */ }
        }

        private void LoadFromDisk()
        {
            if (string.IsNullOrEmpty(_persistDirectory)) return;
            string path = Path.Combine(_persistDirectory, ManifestFileName);
            if (!File.Exists(path)) return;

            try
            {
                string json = File.ReadAllText(path);
                var loaded = JsonSerializer.Deserialize<WorkspaceManifest>(json, _json);
                if (loaded != null)
                    _manifest = loaded;
            }
            catch (Exception) { /* non-fatal: keep empty in-memory manifest */ }
        }

        /// <summary>
        /// Serialises the current manifest to a JSON string (e.g., for embedding in
        /// application settings without a separate file).
        /// </summary>
        public string ExportToJson()
            => JsonSerializer.Serialize(_manifest, _json);

        /// <summary>
        /// Replaces the current in-memory manifest from a JSON string previously
        /// produced by <see cref="ExportToJson"/>.
        /// </summary>
        public void ImportFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON string must not be empty.", nameof(json));

            var loaded = JsonSerializer.Deserialize<WorkspaceManifest>(json, _json)
                ?? throw new InvalidOperationException("Failed to deserialise workspace manifest.");
            _manifest = loaded;
        }
    }
}
