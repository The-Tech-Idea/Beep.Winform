using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// Designer-serializable description of one docking group (tab cell or split container).
    /// References its panels by <see cref="DockPanel.Key"/> so the live panels — which are
    /// themselves designer-created components — own their own property serialization. This type
    /// records only the <b>structure</b>: position, split orientation/ratio, tab order, active
    /// tab, and any nested child groups.
    /// </summary>
    [Serializable]
    public sealed class DockGroupDefinition
    {
        public DockGroupDefinition() { }

        /// <summary>Edge/area this group occupies (Left/Right/Top/Bottom/Fill).</summary>
        [DefaultValue(DockPosition.Left)]
        public DockPosition Position { get; set; } = DockPosition.Left;

        /// <summary>How child groups are split (only relevant when <see cref="Children"/> is non-empty).</summary>
        [DefaultValue(SplitOrientation.Horizontal)]
        public SplitOrientation SplitOrientation { get; set; } = SplitOrientation.Horizontal;

        /// <summary>Proportional size of this group relative to its siblings.</summary>
        [DefaultValue(0.5f)]
        public float SplitRatio { get; set; } = 0.5f;

        /// <summary>Keys of the panels in this group, in tab order. Get-only so the designer emits Add calls.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<string> PanelKeys { get; } = new List<string>();

        /// <summary>Key of the active (front) panel in this group.</summary>
        [DefaultValue(null)]
        public string ActivePanelKey { get; set; }

        /// <summary>Nested child groups (split containers). Empty for a leaf tab cell.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<DockGroupDefinition> Children { get; } = new List<DockGroupDefinition>();
    }

    /// <summary>Designer-serializable record of a floating panel (key + last screen bounds).</summary>
    [Serializable]
    public sealed class FloatingPanelInfo
    {
        public FloatingPanelInfo() { }

        [DefaultValue(null)]
        public string Key { get; set; }

        public Rectangle Bounds { get; set; }

        /// <summary>The dock position to return to when re-docked.</summary>
        [DefaultValue(DockPosition.Left)]
        public DockPosition LastDockPosition { get; set; } = DockPosition.Left;
    }

    /// <summary>Designer-serializable record of an auto-hidden panel (key + edge).</summary>
    [Serializable]
    public sealed class AutoHiddenPanelInfo
    {
        public AutoHiddenPanelInfo() { }

        [DefaultValue(null)]
        public string Key { get; set; }

        [DefaultValue(DockPosition.Left)]
        public DockPosition Edge { get; set; } = DockPosition.Left;
    }

    /// <summary>
    /// Designer-serializable snapshot of an entire docking layout: the top-level groups (each of
    /// which may nest), plus the floating and auto-hidden panel lists. Persisted by the WinForms
    /// designer into the host <c>*.Designer.cs</c> via <c>BeepDockingManager.LayoutDefinition</c>.
    /// </summary>
    [Serializable]
    public sealed class DockLayoutDefinition
    {
        public DockLayoutDefinition() { }

        /// <summary>Schema version for forward-compatible migration.</summary>
        [DefaultValue(1)]
        public int SchemaVersion { get; set; } = 1;

        /// <summary>Top-level docked groups (mirrors <c>DockLayoutTree.Root.Children</c>).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<DockGroupDefinition> Groups { get; } = new List<DockGroupDefinition>();

        /// <summary>Panels currently floating.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<FloatingPanelInfo> Floating { get; } = new List<FloatingPanelInfo>();

        /// <summary>Panels currently auto-hidden on an edge strip.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<AutoHiddenPanelInfo> AutoHidden { get; } = new List<AutoHiddenPanelInfo>();

        /// <summary>True when there is nothing meaningful to serialize.</summary>
        [System.ComponentModel.Browsable(false)]
        public bool IsEmpty =>
            (Groups == null || Groups.Count == 0) &&
            (Floating == null || Floating.Count == 0) &&
            (AutoHidden == null || AutoHidden.Count == 0);
    }
}
