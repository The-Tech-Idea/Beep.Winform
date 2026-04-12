// BeepLayoutHistory.cs
// Bounded version history for BeepDocumentHost layout snapshots.
// Stores up to MaxDepth (default: 20) JSON snapshots with timestamps and metadata.
// Supports optional file-system persistence to a directory.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// A single versioned snapshot of a layout.
    /// </summary>
    public sealed class LayoutVersionEntry
    {
        /// <summary>Stable unique identifier (GUID string).</summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>UTC timestamp when this version was saved.</summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>Full layout JSON produced by <see cref="BeepDocumentHost.SaveLayout"/>.</summary>
        [JsonPropertyName("layoutJson")]
        public string LayoutJson { get; set; } = string.Empty;

        /// <summary>Number of open documents in this snapshot.</summary>
        [JsonPropertyName("documentCount")]
        public int DocumentCount { get; set; }

        /// <summary>Number of split groups in this snapshot.</summary>
        [JsonPropertyName("splitCount")]
        public int SplitCount { get; set; }

        /// <summary>Optional human label (e.g., "Before refactor").</summary>
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        public override string ToString() =>
            $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Label ?? $"{DocumentCount} docs"} ({Id[..8]})";
    }

    /// <summary>
    /// Maintains a bounded ring-buffer of layout snapshots with optional
    /// file-system persistence.
    /// </summary>
    public sealed class BeepLayoutHistory
    {
        private static readonly JsonSerializerOptions _json = new()
        {
            WriteIndented            = true,
            PropertyNameCaseInsensitive = true
        };

        private const string ManifestFileName = "layout-history.json";

        private readonly LinkedList<LayoutVersionEntry> _entries = new();
        private string? _persistDirectory;
        private int _maxDepth = 20;

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Raised when a new version is pushed.</summary>
        public event EventHandler<LayoutVersionEventArgs>? VersionAdded;

        /// <summary>Raised when a version is deleted.</summary>
        public event EventHandler<LayoutVersionEventArgs>? VersionDeleted;

        // ── Configuration ─────────────────────────────────────────────────────

        /// <summary>
        /// Maximum number of snapshots kept.  Oldest entries are evicted when this
        /// limit is exceeded.  Defaults to 20.
        /// </summary>
        public int MaxDepth
        {
            get => _maxDepth;
            set => _maxDepth = Math.Max(1, Math.Min(200, value));
        }

        /// <summary>
        /// Directory where the history manifest is persisted.
        /// Set to <c>null</c> to keep the history in memory only.
        /// Changing this value triggers an immediate load.
        /// </summary>
        public string? PersistDirectory
        {
            get => _persistDirectory;
            set
            {
                _persistDirectory = value;
                if (!string.IsNullOrEmpty(value))
                    LoadFromDirectory(value);
            }
        }

        /// <summary>Number of snapshots currently stored.</summary>
        public int Count => _entries.Count;

        // ── Core operations ───────────────────────────────────────────────────

        /// <summary>
        /// Saves a new layout snapshot.  Oldest entries are evicted if <see cref="MaxDepth"/>
        /// is exceeded.
        /// </summary>
        /// <param name="layoutJson">JSON produced by <see cref="BeepDocumentHost.SaveLayout"/>.</param>
        /// <param name="label">Optional human-readable label for this version.</param>
        /// <param name="documentCount">Hint: number of open documents.</param>
        /// <param name="splitCount">Hint: number of split groups.</param>
        public LayoutVersionEntry Push(
            string  layoutJson,
            string? label         = null,
            int     documentCount = 0,
            int     splitCount    = 1)
        {
            if (string.IsNullOrWhiteSpace(layoutJson))
                throw new ArgumentException("Layout JSON must not be empty.", nameof(layoutJson));

            var entry = new LayoutVersionEntry
            {
                LayoutJson    = layoutJson,
                Label         = label,
                DocumentCount = documentCount,
                SplitCount    = splitCount,
            };

            _entries.AddFirst(entry); // newest first

            // Evict oldest if over limit
            while (_entries.Count > _maxDepth)
                _entries.RemoveLast();

            Persist();
            VersionAdded?.Invoke(this, new LayoutVersionEventArgs(entry));
            return entry;
        }

        /// <summary>Returns all stored versions ordered newest first.</summary>
        public IReadOnlyList<LayoutVersionEntry> GetAll() => _entries.ToList();

        /// <summary>Returns the version at zero-based <paramref name="index"/> (0 = newest).</summary>
        public LayoutVersionEntry? GetAt(int index)
        {
            if (index < 0 || index >= _entries.Count) return null;
            return _entries.ElementAt(index);
        }

        /// <summary>Returns the most recent version; <c>null</c> if history is empty.</summary>
        public LayoutVersionEntry? Latest => _entries.First?.Value;

        /// <summary>Removes the version with the given <paramref name="id"/>.</summary>
        public bool Delete(string id)
        {
            var node = _entries.FirstOrDefault(e => e.Id == id);
            if (node == null) return false;

            var entry = node;
            _entries.Remove(node);
            Persist();
            VersionDeleted?.Invoke(this, new LayoutVersionEventArgs(entry));
            return true;
        }

        /// <summary>Removes all stored versions.</summary>
        public void Clear()
        {
            _entries.Clear();
            Persist();
        }

        // ── Persistence ───────────────────────────────────────────────────────

        public void SaveToDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
            var manifest = JsonSerializer.Serialize(_entries.ToList(), _json);
            File.WriteAllText(Path.Combine(directory, ManifestFileName), manifest);
        }

        public void LoadFromDirectory(string directory)
        {
            var path = Path.Combine(directory, ManifestFileName);
            if (!File.Exists(path)) return;

            try
            {
                var list = JsonSerializer.Deserialize<List<LayoutVersionEntry>>(
                    File.ReadAllText(path), _json) ?? new List<LayoutVersionEntry>();

                _entries.Clear();
                foreach (var e in list.Take(_maxDepth))
                    _entries.AddLast(e);
            }
            catch { /* skip malformed manifest */ }
        }

        private void Persist()
        {
            if (!string.IsNullOrEmpty(_persistDirectory))
                SaveToDirectory(_persistDirectory);
        }
    }

    // ── Event args ────────────────────────────────────────────────────────────

    public sealed class LayoutVersionEventArgs : EventArgs
    {
        public LayoutVersionEntry Entry { get; }
        public LayoutVersionEventArgs(LayoutVersionEntry entry) => Entry = entry;
    }
}
