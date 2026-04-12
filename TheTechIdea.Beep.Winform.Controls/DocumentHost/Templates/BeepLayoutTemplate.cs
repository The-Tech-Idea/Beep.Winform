// BeepLayoutTemplate.cs
// Data model for a named layout template — describes a split-group structure
// that can be applied to a BeepDocumentHost instance.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Well-known template category strings used by <see cref="BeepLayoutTemplateLibrary"/>.
    /// </summary>
    public static class LayoutTemplateCategory
    {
        public const string BuiltIn = "Built-in";
        public const string Custom  = "Custom";
        public const string Shared  = "Shared";
    }

    /// <summary>
    /// A named, serialisable description of a split-group layout structure.
    /// The <see cref="LayoutShapeJson"/> field stores a minimal
    /// <c>LayoutTreeNodeDto</c>-compatible JSON tree (no document IDs — only structure).
    /// </summary>
    public sealed class BeepLayoutTemplate
    {
        /// <summary>Stable unique identifier (GUID string).</summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Human-readable display name shown in the template picker.</summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Template";

        /// <summary>Short description shown as a tooltip or subtitle.</summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>Category string — see <see cref="LayoutTemplateCategory"/>.</summary>
        [JsonPropertyName("category")]
        public string Category { get; set; } = LayoutTemplateCategory.Custom;

        /// <summary>Author name (defaults to current user for custom templates).</summary>
        [JsonPropertyName("author")]
        public string Author { get; set; } = string.Empty;

        /// <summary>SemVer string — "1.0.0" by default.</summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0.0";

        /// <summary>Whether this template was shipped with the library and cannot be deleted.</summary>
        [JsonPropertyName("isBuiltIn")]
        public bool IsBuiltIn { get; set; }

        /// <summary>
        /// JSON that describes the structural tree using the same format as
        /// <c>LayoutTreeNodeDto</c> (Type / Orientation / Ratio / Children / NodeId).
        /// Document IDs in this JSON are used only as slot names and will be replaced
        /// when the template is applied.
        /// </summary>
        [JsonPropertyName("layoutShapeJson")]
        public string LayoutShapeJson { get; set; } = string.Empty;

        /// <summary>
        /// Number of document groups this template creates when applied.
        /// Stored to allow the picker to display it without parsing the JSON.
        /// </summary>
        [JsonPropertyName("groupCount")]
        public int GroupCount { get; set; } = 1;

        /// <summary>
        /// Optional Unicode character (from "Segoe UI Symbol") used as an icon
        /// in the template picker instead of a bitmap.
        /// </summary>
        [JsonPropertyName("iconGlyph")]
        public string? IconGlyph { get; set; }

        /// <summary>Arbitrary key/value metadata (tags, project hints, etc.).</summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new();

        /// <summary>UTC creation timestamp.</summary>
        [JsonPropertyName("created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public override string ToString() => $"[Template] {Name} ({Category})";
    }
}
