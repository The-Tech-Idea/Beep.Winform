// WorkspaceDefinition.cs
// Data model for a named workspace snapshot — wraps a BeepDocumentHost layout JSON
// together with display metadata so the user can save, switch and restore workspaces.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// A named snapshot of a <see cref="BeepDocumentHost"/> layout.
    /// Persisted as part of a <see cref="WorkspaceManifest"/>.
    /// </summary>
    public sealed class WorkspaceDefinition
    {
        /// <summary>Unique identifier for this workspace (stable across renames).</summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Human-readable display name shown in workspace switcher / menus.</summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "Workspace";

        /// <summary>
        /// Optional description shown in the workspace switcher tooltip.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>UTC timestamp when this workspace was last saved.</summary>
        [JsonPropertyName("savedAt")]
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// JSON string produced by <see cref="BeepDocumentHost.SaveLayout"/>.
        /// Restored via <see cref="BeepDocumentHost.RestoreLayout"/>.
        /// </summary>
        [JsonPropertyName("layoutJson")]
        public string LayoutJson { get; set; } = string.Empty;

        /// <summary>
        /// Arbitrary key/value tags the consumer may attach (e.g., project name, branch).
        /// </summary>
        [JsonPropertyName("tags")]
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

        public override string ToString() => $"[Workspace] {Name} ({SavedAt:yyyy-MM-dd HH:mm})";
    }

    /// <summary>
    /// Collection of <see cref="WorkspaceDefinition"/> objects persisted to a single
    /// manifest file (workspaces.json) managed by <see cref="WorkspaceManager"/>.
    /// </summary>
    public sealed class WorkspaceManifest
    {
        /// <summary>Schema version — incremented when the format changes.</summary>
        [JsonPropertyName("schemaVersion")]
        public int SchemaVersion { get; set; } = 1;

        /// <summary>Id of the workspace that was active when the manifest was saved.</summary>
        [JsonPropertyName("activeWorkspaceId")]
        public string? ActiveWorkspaceId { get; set; }

        /// <summary>Ordered list of workspaces (display order preserved).</summary>
        [JsonPropertyName("workspaces")]
        public List<WorkspaceDefinition> Workspaces { get; set; } = new List<WorkspaceDefinition>();
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Event argument for workspace lifecycle events
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Arguments for <see cref="BeepDocumentHost"/> workspace events.</summary>
    public sealed class WorkspaceEventArgs : EventArgs
    {
        /// <summary>The workspace involved in the event.</summary>
        public WorkspaceDefinition Workspace { get; }

        public WorkspaceEventArgs(WorkspaceDefinition workspace)
        {
            ArgumentNullException.ThrowIfNull(workspace);
            Workspace = workspace;
        }
    }
}
