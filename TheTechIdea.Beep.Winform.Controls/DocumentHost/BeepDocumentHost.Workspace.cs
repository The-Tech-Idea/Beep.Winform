// BeepDocumentHost.Workspace.cs
// Named workspace save / load / switch API for BeepDocumentHost.
// A "workspace" is a named snapshot of the full layout JSON produced by SaveLayout().
// Workspaces can be persisted to a directory (workspaces.json) or kept in memory.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ── Workspace manager (lazy) ──────────────────────────────────────────

        private WorkspaceManager? _workspaceManager;

        private WorkspaceManager Workspaces => _workspaceManager ??= new WorkspaceManager();

        // ─────────────────────────────────────────────────────────────────────
        // Events
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Raised after a workspace has been saved (created or updated).</summary>
        [Browsable(true)]
        [Description("Raised after a workspace is saved (created or updated).")]
        public event EventHandler<WorkspaceEventArgs>? WorkspaceSaved;

        /// <summary>Raised after a workspace has been deleted.</summary>
        [Browsable(true)]
        [Description("Raised after a workspace is deleted.")]
        public event EventHandler<WorkspaceEventArgs>? WorkspaceDeleted;

        /// <summary>Raised after the active workspace has been switched.</summary>
        [Browsable(true)]
        [Description("Raised after the active workspace is switched.")]
        public event EventHandler<WorkspaceEventArgs>? WorkspaceSwitched;

        // ─────────────────────────────────────────────────────────────────────
        // Configuration
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Directory where <c>workspaces.json</c> is persisted.
        /// Set to <c>null</c> (default) to keep workspaces in memory only.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? WorkspacePersistDirectory
        {
            get => Workspaces.PersistDirectory;
            set => Workspaces.PersistDirectory = value;
        }

        /// <summary>Name of the currently active workspace, or <c>null</c>.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? ActiveWorkspaceName => Workspaces.GetActive()?.Name;

        // ─────────────────────────────────────────────────────────────────────
        // Public workspace API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Saves the current layout as a named workspace.
        /// If a workspace with <paramref name="name"/> already exists it is overwritten.
        /// </summary>
        /// <param name="name">Display name (must not be empty or whitespace).</param>
        /// <param name="description">Optional description shown in workspace switcher.</param>
        /// <returns>The saved <see cref="WorkspaceDefinition"/>.</returns>
        public WorkspaceDefinition SaveWorkspace(string name, string description = "")
        {
            string layoutJson = SaveLayout();
            var ws = Workspaces.Save(name, layoutJson, description);
            Workspaces.SetActive(ws.Id);

            SubscribeWorkspaceManagerEvents();
            WorkspaceSaved?.Invoke(this, new WorkspaceEventArgs(ws));
            return ws;
        }

        /// <summary>
        /// Switches to the workspace with the given name — saves the current layout
        /// (under the current workspace name if one is active), then restores the target.
        /// </summary>
        /// <param name="name">Target workspace name.</param>
        /// <returns>
        /// The <see cref="LayoutRestoreReport"/> from <see cref="TryRestoreLayout"/>.
        /// </returns>
        public LayoutRestoreReport SwitchWorkspace(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Workspace name must not be empty.", nameof(name));

            // Auto-save current workspace before switching
            var current = Workspaces.GetActive();
            if (current != null)
                Workspaces.Save(current.Name, SaveLayout(), current.Description);

            var target = Workspaces.FindByName(name)
                ?? throw new InvalidOperationException($"Workspace '{name}' not found.");

            var report = TryRestoreLayout(target.LayoutJson, out _);
            Workspaces.SetActive(target.Id);

            WorkspaceSwitched?.Invoke(this, new WorkspaceEventArgs(target));
            return report;
        }

        /// <summary>
        /// Loads a workspace by name without switching — returns its definition
        /// so the caller can inspect or preview it before applying.
        /// </summary>
        public WorkspaceDefinition? GetWorkspace(string name)
            => Workspaces.FindByName(name);

        /// <summary>Returns all defined workspaces in display order.</summary>
        public IReadOnlyList<WorkspaceDefinition> GetAllWorkspaces()
            => Workspaces.GetAll();

        /// <summary>Renames the workspace identified by its current name.</summary>
        public void RenameWorkspace(string currentName, string newName)
        {
            var ws = Workspaces.FindByName(currentName)
                ?? throw new InvalidOperationException($"Workspace '{currentName}' not found.");
            Workspaces.Rename(ws.Id, newName);
        }

        /// <summary>
        /// Deletes the workspace with the given name.
        /// Returns <c>false</c> when the workspace was not found.
        /// </summary>
        public bool DeleteWorkspace(string name)
        {
            var ws = Workspaces.FindByName(name);
            if (ws == null) return false;
            return Workspaces.Delete(ws.Id);
        }

        /// <summary>
        /// Serialises all workspaces to a JSON string suitable for embedding
        /// in application settings.
        /// </summary>
        public string ExportWorkspaces() => Workspaces.ExportToJson();

        /// <summary>
        /// Replaces all workspaces from a JSON string produced by <see cref="ExportWorkspaces"/>.
        /// </summary>
        public void ImportWorkspaces(string json) => Workspaces.ImportFromJson(json);

        // ─────────────────────────────────────────────────────────────────────
        // Internal — forward WorkspaceManager events to host-level events
        // ─────────────────────────────────────────────────────────────────────

        private bool _workspaceEventsSubscribed;

        private void SubscribeWorkspaceManagerEvents()
        {
            if (_workspaceEventsSubscribed || _workspaceManager == null) return;
            _workspaceEventsSubscribed = true;

            _workspaceManager.WorkspaceSaved   += (s, e) => WorkspaceSaved?.Invoke(this, e);
            _workspaceManager.WorkspaceDeleted += (s, e) => WorkspaceDeleted?.Invoke(this, e);
        }
    }
}
