// BeepLayoutTemplateLibrary.cs
// CRUD store for layout templates + 10 built-in presets.
// Supports optional file-system persistence (one JSON file per custom template).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Manages <see cref="BeepLayoutTemplate"/> instances.
    /// Built-in templates are registered automatically; custom templates can be
    /// registered in memory or persisted to a directory.
    /// </summary>
    public sealed class BeepLayoutTemplateLibrary
    {
        private static readonly JsonSerializerOptions _json = new()
        {
            WriteIndented            = true,
            PropertyNameCaseInsensitive = true
        };

        private readonly Dictionary<string, BeepLayoutTemplate> _templates = new();
        private string? _persistDirectory;

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Raised when a template is added or updated.</summary>
        public event EventHandler<TemplateEventArgs>? TemplateChanged;

        /// <summary>Raised when a template is removed.</summary>
        public event EventHandler<TemplateEventArgs>? TemplateRemoved;

        // ── Construction ──────────────────────────────────────────────────────

        public BeepLayoutTemplateLibrary()
        {
            foreach (var t in CreateBuiltIns())
                _templates[t.Id] = t;
        }

        // ── Persistence directory ─────────────────────────────────────────────

        /// <summary>
        /// Directory where custom templates are persisted as <c>*.template.json</c> files.
        /// Set to <c>null</c> to keep templates in memory only.
        /// Changing this value triggers an immediate load from the new path.
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

        // ── CRUD ──────────────────────────────────────────────────────────────

        /// <summary>Registers or replaces a template. Built-in templates are never replaced.</summary>
        public void Register(BeepLayoutTemplate template)
        {
            ArgumentNullException.ThrowIfNull(template);
            if (_templates.TryGetValue(template.Id, out var existing) && existing.IsBuiltIn)
                return;

            _templates[template.Id] = template;
            if (!template.IsBuiltIn && !string.IsNullOrEmpty(_persistDirectory))
                SaveTemplate(template);

            TemplateChanged?.Invoke(this, new TemplateEventArgs(template));
        }

        /// <summary>Removes a custom template by id. Built-in templates cannot be removed.</summary>
        public bool Remove(string id)
        {
            if (!_templates.TryGetValue(id, out var t) || t.IsBuiltIn) return false;
            _templates.Remove(id);

            if (!string.IsNullOrEmpty(_persistDirectory))
            {
                var file = GetTemplateFilePath(_persistDirectory, id);
                if (File.Exists(file)) File.Delete(file);
            }

            TemplateRemoved?.Invoke(this, new TemplateEventArgs(t));
            return true;
        }

        /// <summary>Returns all registered templates.</summary>
        public IReadOnlyList<BeepLayoutTemplate> GetAll() =>
            _templates.Values.OrderBy(t => t.IsBuiltIn ? 0 : 1).ThenBy(t => t.Name).ToList();

        /// <summary>Returns templates belonging to the given category.</summary>
        public IReadOnlyList<BeepLayoutTemplate> GetByCategory(string category) =>
            _templates.Values.Where(t => t.Category == category).OrderBy(t => t.Name).ToList();

        /// <summary>Looks up a template by id; returns <c>null</c> if not found.</summary>
        public BeepLayoutTemplate? FindById(string id) =>
            _templates.TryGetValue(id, out var t) ? t : null;

        // ── Persistence helpers ───────────────────────────────────────────────

        /// <summary>Loads custom templates from <paramref name="directory"/>.</summary>
        public void LoadFromDirectory(string directory)
        {
            if (!Directory.Exists(directory)) return;
            foreach (var file in Directory.GetFiles(directory, "*.template.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var t    = JsonSerializer.Deserialize<BeepLayoutTemplate>(json, _json);
                    if (t != null && !t.IsBuiltIn)
                        _templates[t.Id] = t;
                }
                catch { /* skip malformed files */ }
            }
        }

        private void SaveTemplate(BeepLayoutTemplate t)
        {
            if (string.IsNullOrEmpty(_persistDirectory)) return;
            Directory.CreateDirectory(_persistDirectory);
            File.WriteAllText(GetTemplateFilePath(_persistDirectory, t.Id),
                JsonSerializer.Serialize(t, _json));
        }

        private static string GetTemplateFilePath(string dir, string id) =>
            Path.Combine(dir, $"{id}.template.json");

        // ── Apply template to a host ──────────────────────────────────────────

        /// <summary>
        /// Applies the structural shape of <paramref name="templateId"/> to
        /// <paramref name="host"/>.  Open documents are redistributed across
        /// the new groups; excess documents go to the primary group.
        /// </summary>
        public bool ApplyTemplate(string templateId, BeepDocumentHost host)
        {
            ArgumentNullException.ThrowIfNull(host);
            var template = FindById(templateId);
            if (template == null) return false;

            host.ApplyLayoutTemplate(template);
            return true;
        }

        // ── Built-in template factory ─────────────────────────────────────────

        private static IEnumerable<BeepLayoutTemplate> CreateBuiltIns()
        {
            // Helper: build a split-tree JSON string
            static string Single() =>
                """{"Type":"tabGroup","NodeId":"g1"}""";

            static string HorizontalSplit(float ratio, string glyph = "⬜") =>
                $$"""{"Type":"split","NodeId":"root","Orientation":"Horizontal","Ratio":{{ratio:F2}},"Children":[{"Type":"tabGroup","NodeId":"g1"},{"Type":"tabGroup","NodeId":"g2"}]}""";

            static string VerticalSplit(float ratio) =>
                $$"""{"Type":"split","NodeId":"root","Orientation":"Vertical","Ratio":{{ratio:F2}},"Children":[{"Type":"tabGroup","NodeId":"g1"},{"Type":"tabGroup","NodeId":"g2"}]}""";

            static string ThreeWay() =>
                """{"Type":"split","NodeId":"root","Orientation":"Horizontal","Ratio":0.40,"Children":[{"Type":"tabGroup","NodeId":"g1"},{"Type":"split","NodeId":"r","Orientation":"Vertical","Ratio":0.50,"Children":[{"Type":"tabGroup","NodeId":"g2"},{"Type":"tabGroup","NodeId":"g3"}]}]}""";

            static string FourUp() =>
                """{"Type":"split","NodeId":"root","Orientation":"Vertical","Ratio":0.50,"Children":[{"Type":"split","NodeId":"top","Orientation":"Horizontal","Ratio":0.50,"Children":[{"Type":"tabGroup","NodeId":"g1"},{"Type":"tabGroup","NodeId":"g2"}]},{"Type":"split","NodeId":"bot","Orientation":"Horizontal","Ratio":0.50,"Children":[{"Type":"tabGroup","NodeId":"g3"},{"Type":"tabGroup","NodeId":"g4"}]}]}""";

            static string DataExplorer() =>
                """{"Type":"split","NodeId":"root","Orientation":"Horizontal","Ratio":0.25,"Children":[{"Type":"tabGroup","NodeId":"g1"},{"Type":"split","NodeId":"r","Orientation":"Vertical","Ratio":0.70,"Children":[{"Type":"tabGroup","NodeId":"g2"},{"Type":"tabGroup","NodeId":"g3"}]}]}""";

            yield return T("single",        "Single",          "One group — full area",                                      1, Single(),                    "⬜");
            yield return T("side-by-side",  "Side by Side",    "Two groups — horizontal split 50/50",                        2, HorizontalSplit(0.50f),      "⬛⬛");
            yield return T("stacked",       "Stacked",         "Two groups — vertical split 50/50",                          2, VerticalSplit(0.50f),         "🔳");
            yield return T("three-way",     "Three Way",       "Three groups — left column + right top/bottom",               3, ThreeWay(),                  "⬚");
            yield return T("four-up",       "Four Up",         "Four groups — 2×2 grid",                                     4, FourUp(),                    "⊞");
            yield return T("code-review",   "Code Review",     "Left: source file  ·  Right: diff/compare (60/40)",          2, HorizontalSplit(0.60f),      "⇌");
            yield return T("debug",         "Debug",           "Top: editor  ·  Bottom: output/watch panel (70/30)",          2, VerticalSplit(0.70f),        "🐛");
            yield return T("data-explorer", "Data Explorer",   "Left: tree  ·  Right-top: grid  ·  Right-bottom: query",     3, DataExplorer(),              "📊");
            yield return T("designer",      "Designer",        "Wide center canvas + narrow side panels (same as Three Way)", 3, ThreeWay(),                  "✏");
            yield return T("terminal",      "Terminal Focus",  "Top: editor  ·  Bottom: terminal (70/30)",                   2, VerticalSplit(0.70f),        "⌨");
        }

        private static BeepLayoutTemplate T(
            string id, string name, string desc, int groups, string shapeJson, string glyph) =>
            new BeepLayoutTemplate
            {
                Id             = id,
                Name           = name,
                Description    = desc,
                Category       = LayoutTemplateCategory.BuiltIn,
                Author         = "Beep",
                Version        = "1.0.0",
                IsBuiltIn      = true,
                LayoutShapeJson = shapeJson,
                GroupCount     = groups,
                IconGlyph      = glyph,
                Created        = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };
    }

    // ── Event args ────────────────────────────────────────────────────────────

    public sealed class TemplateEventArgs : EventArgs
    {
        public BeepLayoutTemplate Template { get; }
        public TemplateEventArgs(BeepLayoutTemplate template) => Template = template;
    }
}
