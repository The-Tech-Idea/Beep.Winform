using System;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// Represents the complete hierarchical layout tree for all docking groups and panels.
    /// Provides versioning for future schema migration support.
    /// </summary>
    public class DockLayoutTree
    {
        private DockGroup _root;
        private Dictionary<string, DockPanel> _panelRegistry = new Dictionary<string, DockPanel>();
        private Dictionary<string, DockGroup> _groupRegistry = new Dictionary<string, DockGroup>();

        /// <summary>
        /// Schema version for this layout tree.
        /// Increment when the serialization format changes to enable migration logic.
        /// </summary>
        public int SchemaVersion { get; set; } = 1;

        /// <summary>
        /// Root group containing all docking groups at the top level.
        /// </summary>
        public DockGroup Root
        {
            get
            {
                if (_root == null)
                {
                    _root = new DockGroup
                    {
                        Id = "root",
                        Position = DockPosition.Fill
                    };
                    _groupRegistry["root"] = _root;
                }
                return _root;
            }
        }

        /// <summary>
        /// Timestamp of when this layout was created (UTC).
        /// </summary>
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp of when this layout was last modified (UTC).
        /// </summary>
        public DateTime ModifiedUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional name/description of this layout (e.g., "Debug Layout", "Release Layout").
        /// </summary>
        public string Name { get; set; } = "Default";

        /// <summary>
        /// Registers a panel in the layout tree.
        /// </summary>
        public void RegisterPanel(DockPanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            if (string.IsNullOrEmpty(panel.Key))
                throw new ArgumentException("Panel key cannot be empty", nameof(panel));

            if (_panelRegistry.ContainsKey(panel.Key))
                throw new InvalidOperationException($"Panel with key '{panel.Key}' is already registered");

            _panelRegistry[panel.Key] = panel;
            ModifiedUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Unregisters a panel from the layout tree.
        /// </summary>
        public void UnregisterPanel(string panelKey)
        {
            if (_panelRegistry.Remove(panelKey))
            {
                ModifiedUtc = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Gets a panel by key from the registry.
        /// </summary>
        public DockPanel GetPanel(string panelKey)
        {
            _panelRegistry.TryGetValue(panelKey, out var panel);
            return panel;
        }

        /// <summary>
        /// Gets all registered panels.
        /// </summary>
        public IReadOnlyList<DockPanel> GetAllPanels()
        {
            return _panelRegistry.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// Registers a group in the layout tree.
        /// </summary>
        public void RegisterGroup(DockGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            _groupRegistry[group.Id] = group;
            ModifiedUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Unregisters a group from the layout tree.
        /// </summary>
        public void UnregisterGroup(string groupId)
        {
            if (_groupRegistry.Remove(groupId))
            {
                ModifiedUtc = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Gets a group by ID from the registry.
        /// </summary>
        public DockGroup GetGroup(string groupId)
        {
            _groupRegistry.TryGetValue(groupId, out var group);
            return group;
        }

        /// <summary>
        /// Gets all registered groups.
        /// </summary>
        public IReadOnlyList<DockGroup> GetAllGroups()
        {
            return _groupRegistry.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the root-level groups at each dock position (Left, Right, Top, Bottom, Fill).
        /// </summary>
        public Dictionary<DockPosition, DockGroup> GetRootGroupsByPosition()
        {
            var result = new Dictionary<DockPosition, DockGroup>();

            foreach (var group in Root.Children)
            {
                if (!result.ContainsKey(group.Position))
                {
                    result[group.Position] = group;
                }
            }

            return result;
        }

        /// <summary>
        /// Finds a panel recursively anywhere in the tree.
        /// </summary>
        public DockPanel FindPanel(string panelKey)
        {
            return Root.FindPanelRecursive(panelKey);
        }

        /// <summary>
        /// Gets all panels at a specific dock position (and their children recursively).
        /// </summary>
        public List<DockPanel> GetPanelsAtPosition(DockPosition position)
        {
            var result = new List<DockPanel>();

            var groupsAtPosition = Root.Children.Where(g => g.Position == position).ToList();
            foreach (var group in groupsAtPosition)
            {
                result.AddRange(group.GetAllPanelsRecursive());
            }

            return result;
        }

        /// <summary>
        /// Gets a diagnostic summary of the entire layout tree.
        /// </summary>
        public string GetDiagnostics()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"DockLayoutTree[Version={SchemaVersion}, Name={Name}]");
            sb.AppendLine($"  Panels: {_panelRegistry.Count}");
            sb.AppendLine($"  Groups: {_groupRegistry.Count}");
            sb.AppendLine($"  Created: {CreatedUtc:O}");
            sb.AppendLine($"  Modified: {ModifiedUtc:O}");
            sb.AppendLine();

            foreach (var position in new[] { DockPosition.Top, DockPosition.Left, DockPosition.Fill, DockPosition.Right, DockPosition.Bottom })
            {
                var groups = Root.Children.Where(g => g.Position == position).ToList();
                if (groups.Count > 0)
                {
                    sb.AppendLine($"  Position: {position}");
                    foreach (var group in groups)
                    {
                        sb.AppendLine($"    {group}");
                        foreach (var panel in group.GetAllPanelsRecursive())
                        {
                            sb.AppendLine($"      - {panel}");
                        }
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Clears all groups and panels from the layout tree.
        /// </summary>
        public void Clear()
        {
            _root = null;
            _panelRegistry.Clear();
            _groupRegistry.Clear();
            ModifiedUtc = DateTime.UtcNow;
        }
    }
}
