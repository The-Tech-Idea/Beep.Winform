using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// Represents a group of docking panels at the same position (e.g., all left-side tool windows).
    /// Groups can be split and arranged hierarchically in the layout tree.
    /// </summary>
    public class DockGroup
    {
        private List<DockPanel> _panels = new List<DockPanel>();
        private DockGroup _parent;
        private List<DockGroup> _children = new List<DockGroup>();
        private Rectangle _bounds = Rectangle.Empty;
        private float _splitRatio = 0.5f;  // For split groups: 0.5 = 50-50 split

        /// <summary>
        /// Unique identifier for this group (assigned by manager).
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// The dock position this group occupies (Left, Right, Top, Bottom, Fill).
        /// </summary>
        public DockPosition Position { get; set; } = DockPosition.Left;

        /// <summary>
        /// How child panels are arranged (tabs at top, bottom, left, or right).
        /// </summary>
        public TabStyle TabStyle { get; set; } = TabStyle.Top;

        /// <summary>
        /// How child groups are split (horizontal or vertical).
        /// Only relevant if this group has child groups.
        /// </summary>
        public SplitOrientation SplitOrientation { get; set; } = SplitOrientation.Horizontal;

        /// <summary>
        /// The parent group in the layout tree (null if root).
        /// </summary>
        public DockGroup Parent
        {
            get => _parent;
            internal set => _parent = value;
        }

        /// <summary>
        /// Child groups (if this group is split into sub-groups).
        /// Empty list if this group contains only panels.
        /// </summary>
        public IReadOnlyList<DockGroup> Children => _children.AsReadOnly();

        /// <summary>
        /// Panels directly contained in this group.
        /// </summary>
        public IReadOnlyList<DockPanel> Panels => _panels.AsReadOnly();

        /// <summary>
        /// The active (currently visible) panel in this group.
        /// </summary>
        public DockPanel ActivePanel { get; set; }

        /// <summary>
        /// Cached client area rectangle (computed by layout engine).
        /// </summary>
        public Rectangle Bounds
        {
            get => _bounds;
            internal set => _bounds = value;
        }

        /// <summary>
        /// For split groups: the ratio of space given to the first child vs the second.
        /// Range: 0.0 to 1.0. Default: 0.5 (equal split).
        /// </summary>
        public float SplitRatio
        {
            get => _splitRatio;
            set => _splitRatio = Math.Max(0.1f, Math.Min(0.9f, value));  // Clamp to [0.1, 0.9]
        }

        /// <summary>
        /// Gets the index of a panel within this group.
        /// Returns -1 if panel is not in this group.
        /// </summary>
        public int GetPanelIndex(DockPanel panel)
        {
            return _panels.IndexOf(panel);
        }

        /// <summary>
        /// Gets a panel by key.
        /// </summary>
        public DockPanel GetPanelByKey(string panelKey)
        {
            return _panels.FirstOrDefault(p => p.Key == panelKey);
        }

        /// <summary>
        /// Adds a panel to this group.
        /// </summary>
        internal void AddPanel(DockPanel panel)
        {
            if (panel == null)
                throw new ArgumentNullException(nameof(panel));

            if (_panels.Contains(panel))
                return;  // Already in this group

            _panels.Add(panel);
            panel.Group = this;

            // If this is the first panel, make it active
            if (ActivePanel == null)
                ActivePanel = panel;
        }

        /// <summary>
        /// Removes a panel from this group.
        /// </summary>
        internal void RemovePanel(DockPanel panel)
        {
            if (panel == null)
                return;

            _panels.Remove(panel);
            panel.Group = null;

            // If the removed panel was active, pick a new active panel
            if (ActivePanel == panel)
            {
                ActivePanel = _panels.Count > 0 ? _panels[0] : null;
            }
        }

        /// <summary>
        /// Moves a panel to a new index within this group's tab strip.
        /// Follows DockPanelSuite DockPane.SetContentIndex() semantics.
        /// </summary>
        /// <param name="panel">The panel to reorder.</param>
        /// <param name="newIndex">Target zero-based index.</param>
        internal void MovePanelToIndex(DockPanel panel, int newIndex)
        {
            if (panel == null) return;
            int current = _panels.IndexOf(panel);
            if (current < 0) return;   // not in this group

            newIndex = Math.Max(0, Math.Min(newIndex, _panels.Count - 1));
            if (current == newIndex) return;

            _panels.RemoveAt(current);
            _panels.Insert(newIndex, panel);
        }

        /// <summary>
        /// Adds a child group (used when splitting this group).
        /// </summary>
        internal void AddChild(DockGroup child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            _children.Add(child);
            child.Parent = this;
        }

        /// <summary>
        /// Removes a child group.
        /// </summary>
        internal void RemoveChild(DockGroup child)
        {
            if (child == null)
                return;

            _children.Remove(child);
            child.Parent = null;
        }

        /// <summary>
        /// Gets a flattened list of all panels in this group and its children.
        /// </summary>
        public List<DockPanel> GetAllPanelsRecursive()
        {
            var result = new List<DockPanel>(_panels);

            foreach (var child in _children)
            {
                result.AddRange(child.GetAllPanelsRecursive());
            }

            return result;
        }

        /// <summary>
        /// Finds a panel recursively by key in this group and all children.
        /// </summary>
        public DockPanel FindPanelRecursive(string panelKey)
        {
            var panel = GetPanelByKey(panelKey);
            if (panel != null)
                return panel;

            foreach (var child in _children)
            {
                panel = child.FindPanelRecursive(panelKey);
                if (panel != null)
                    return panel;
            }

            return null;
        }

        /// <summary>
        /// Gets a diagnostic string describing this group and its contents.
        /// </summary>
        public override string ToString()
        {
            return $"DockGroup[Id={Id}, Pos={Position}, Panels={_panels.Count}, Children={_children.Count}]";
        }
    }
}
