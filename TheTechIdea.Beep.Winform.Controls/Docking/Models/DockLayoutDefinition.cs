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

        /// <summary>Tab strip position for this group.</summary>
        [DefaultValue(HeaderPosition.Top)]
        public HeaderPosition HeaderPosition { get; set; } = HeaderPosition.Top;

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

        /// <summary>
        /// Device name of the display the float was on, e.g. <c>\.\DISPLAY2</c>. Empty for a
        /// layout written before this was recorded, which restore treats as "match by geometry".
        /// </summary>
        /// <remarks>
        /// Stored alongside <see cref="Bounds"/> because a rectangle alone cannot say <i>which</i>
        /// display it belonged to. Rearranging two monitors changes every coordinate while the
        /// identity is unchanged, and a bare rectangle restores such a float to the wrong screen -
        /// or to no screen at all, if the display it referred to has been detached.
        /// </remarks>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// Working area of that display when the layout was saved. Kept so a restore can tell a
        /// display that merely moved from one that was replaced by a different size.
        /// </summary>
        public Rectangle MonitorWorkingArea { get; set; }
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

        /// <summary>
        /// Highest schema version this build understands.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item><b>1</b> — groups, floating and auto-hidden panels.</item>
        /// <item><b>2</b> — adds <see cref="Hidden"/>. Before it, a group's
        /// <see cref="DockGroupDefinition.PanelKeys"/> held only docked panels, so a hidden panel
        /// was dropped from the layout entirely and did not come back when shown.</item>
        /// <item><b>3</b> — adds <see cref="Perspectives"/>. Before it, named layouts lived only on
        /// the manager and were lost when the application closed, which made saving one close to
        /// pointless.</item>
        /// </list>
        /// A version 1 definition loads unchanged: it simply has no hidden panels, which is what an
        /// absent <see cref="Hidden"/> list already means. Migration forward is therefore the
        /// identity, and is asserted rather than assumed.
        /// </remarks>
        public const int CurrentSchemaVersion = 3;

        /// <summary>Schema version for forward-compatible migration.</summary>
        [DefaultValue(CurrentSchemaVersion)]
        public int SchemaVersion { get; set; } = CurrentSchemaVersion;

        /// <summary>Top-level docked groups (mirrors <c>DockLayoutTree.Root.Children</c>).</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<DockGroupDefinition> Groups { get; } = new List<DockGroupDefinition>();

        /// <summary>Panels currently floating.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<FloatingPanelInfo> Floating { get; } = new List<FloatingPanelInfo>();

        /// <summary>Panels currently auto-hidden on an edge strip.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<AutoHiddenPanelInfo> AutoHidden { get; } = new List<AutoHiddenPanelInfo>();

        /// <summary>
        /// Keys of panels that are members of a group but currently hidden.
        /// </summary>
        /// <remarks>
        /// Hidden panels stay in their group's <see cref="DockGroupDefinition.PanelKeys"/> - hiding
        /// does not change membership, only visibility - and are listed here so the state is
        /// restored rather than silently promoted back to visible.
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<string> Hidden { get; } = new List<string>();

        /// <summary>
        /// Named layouts saved alongside this one.
        /// </summary>
        /// <remarks>
        /// A perspective's arrangement is itself a <see cref="DockLayoutDefinition"/>, so the shape
        /// nests. That is deliberate: a perspective then inherits schema versioning, missing-panel
        /// degradation and hidden-panel membership from the same materialiser, rather than needing
        /// a parallel format that would drift from it.
        /// <para>
        /// A perspective's own <see cref="SchemaVersion"/> is not written - the outer definition
        /// carries the version for the whole file, and a nested copy could only ever disagree.
        /// </para>
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<DockPerspectiveDefinition> Perspectives { get; } = new List<DockPerspectiveDefinition>();

        /// <summary>True when there is nothing meaningful to serialize.</summary>
        [System.ComponentModel.Browsable(false)]
        public bool IsEmpty =>
            (Groups == null || Groups.Count == 0) &&
            (Floating == null || Floating.Count == 0) &&
            (AutoHidden == null || AutoHidden.Count == 0) &&
            (Perspectives == null || Perspectives.Count == 0);
    }

    /// <summary>A named layout, as persisted.</summary>
    public sealed class DockPerspectiveDefinition
    {
        public DockPerspectiveDefinition() { }

        /// <summary>Name the perspective is applied by.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>True for the perspective <c>ApplyDefaultPerspective</c> restores.</summary>
        public bool IsDefault { get; set; }

        /// <summary>The arrangement itself.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DockLayoutDefinition Layout { get; set; } = new DockLayoutDefinition();

        public override string ToString() => IsDefault ? $"{Name} (default)" : Name;
    }
}
