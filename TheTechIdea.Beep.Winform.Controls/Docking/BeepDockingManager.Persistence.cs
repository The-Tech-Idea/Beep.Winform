using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Docking.Layout;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Layout persistence for <see cref="BeepDockingManager"/>. The layout is described by a
    /// designer-serializable <see cref="DockLayoutDefinition"/> exposed through
    /// <see cref="LayoutDefinition"/>. Because it is marked
    /// <see cref="DesignerSerializationVisibility.Content"/> and exposes a <b>stable</b> backing
    /// instance with get-only collections, the WinForms designer writes the whole layout into the
    /// host (Form/UserControl) <c>*.Designer.cs</c> inside <c>InitializeComponent()</c> as a series
    /// of property assignments and <c>.Add(...)</c> calls. No external XML/JSON file is used.
    ///
    /// Each <see cref="DockPanel"/> is itself a designer-created component, so its own properties
    /// serialize independently; the definition only records <b>structure</b> (grouping, ratios,
    /// active tab, float/auto-hide), referencing panels by <see cref="DockPanel.Key"/>.
    /// </summary>
    public partial class BeepDockingManager
    {
        // Stable backing instance — the designer mutates this same object's collections, so it
        // must NOT be replaced on each get (that would break Content serialization round-trip).
        private readonly DockLayoutDefinition _layoutDefinition = new DockLayoutDefinition();

        /// <summary>
        /// The serialized docking layout, persisted by the designer into the host
        /// <c>*.Designer.cs</c>. At runtime the getter refreshes it from the live tree; before the
        /// host is attached it returns the (possibly deserialized) backing instance unchanged so
        /// the design-time values survive until <see cref="ManageControl"/> materializes them.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(false)]
        public DockLayoutDefinition LayoutDefinition
        {
            get
            {
                // Reflect the live tree only when there actually is one (runtime, or design-time
                // once the designer has built groups). Otherwise return the backing instance as-is
                // so freshly-deserialized values are preserved for ManageControl to apply.
                if (_layoutTree != null &&
                    (_layoutTree.Root.Children.Count > 0 || _layoutTree.GetAllPanels().Count > 0))
                {
                    FillDefinition(_layoutDefinition);
                }
                return _layoutDefinition;
            }
        }

        /// <summary>Snapshots the current live layout into a fresh definition (runtime convenience).</summary>
        public DockLayoutDefinition CaptureDefinition()
        {
            var def = new DockLayoutDefinition();
            FillDefinition(def);
            return def;
        }

        /// <summary>Populates the supplied definition in place from the current live tree + runtime state.</summary>
        private void FillDefinition(DockLayoutDefinition def)
        {
            def.SchemaVersion = DockLayoutDefinition.CurrentSchemaVersion;
            def.Groups.Clear();
            def.Floating.Clear();
            def.AutoHidden.Clear();
            def.Hidden.Clear();

            // Skip empty edge groups: panels that float/auto-hide/close are removed from their
            // group but the (now empty) group lingers in the tree. Serializing it would add noise
            // and recreate dead groups on load.
            foreach (var group in _layoutTree.Root.Children)
                if (GroupHasMembers(group))
                    def.Groups.Add(CaptureGroup(group));

            if (_hostForm != null)
            {
                foreach (var kv in _floatWindowsByKey)
                {
                    var fw = kv.Value;
                    if (fw?.Panel == null)
                        continue;

                    def.Floating.Add(DescribeFloat(fw));
                }

                foreach (var owned in _hostForm.OwnedForms)
                {
                    if (owned is FloatWindow fw && fw.Panel != null &&
                        !_floatWindowsByKey.ContainsKey(fw.Panel.Key))
                    {
                        def.Floating.Add(DescribeFloat(fw));
                    }
                }
            }

            foreach (var kv in _autoHideStrips)
            {
                foreach (var panel in kv.Value.Panels)
                    def.AutoHidden.Add(new AutoHiddenPanelInfo { Key = panel.Key, Edge = kv.Key });
            }

            foreach (var panel in _panelsByKey.Values)
            {
                if (panel?.Key != null && panel.State == DockPanelState.Hidden)
                    def.Hidden.Add(panel.Key);
            }
        }

        /// <summary>
        /// Describes a float for persistence, including which display it is on.
        /// </summary>
        private FloatingPanelInfo DescribeFloat(FloatWindow fw)
        {
            var monitor = MonitorFor(fw.Bounds);
            return new FloatingPanelInfo
            {
                Key = fw.Panel.Key,
                Bounds = fw.Bounds,
                LastDockPosition = fw.Panel.DockPosition,
                DeviceName = monitor.DeviceName,
                MonitorWorkingArea = monitor.WorkingArea
            };
        }

        /// <summary>The display a rectangle mostly sits on, or the primary when it sits on none.</summary>
        private MonitorInfo MonitorFor(Rectangle bounds)
        {
            var monitors = Monitors.GetMonitors();
            if (monitors.Count == 0)
                return default;

            MonitorInfo best = default;
            long bestArea = -1;
            foreach (var m in monitors)
            {
                var overlap = Rectangle.Intersect(bounds, m.Bounds);
                long area = (long)Math.Max(0, overlap.Width) * Math.Max(0, overlap.Height);
                if (area > bestArea)
                {
                    bestArea = area;
                    best = m;
                }
            }

            return bestArea > 0
                ? best
                : monitors.FirstOrDefault(m => m.IsPrimary) is { } p && !string.IsNullOrEmpty(p.DeviceName)
                    ? p
                    : monitors[0];
        }

        private static DockGroupDefinition CaptureGroup(DockGroup group)
        {
            var def = new DockGroupDefinition
            {
                Position = group.Position,
                SplitOrientation = group.SplitOrientation,
                SplitRatio = group.SplitRatio,
                ActivePanelKey = group.ActivePanel?.Key,
                HeaderPosition = group.HeaderPosition
            };

            // Membership, not visibility. Floating, auto-hiding and closing all detach the panel
            // from its group, so group.Panels is exactly the set that belongs here; hidden panels
            // are still members and are recorded as hidden separately.
            foreach (var panel in group.Panels)
            {
                if (panel?.Key != null)
                    def.PanelKeys.Add(panel.Key);
            }

            foreach (var child in group.Children)
                if (GroupHasMembers(child))
                    def.Children.Add(CaptureGroup(child));

            return def;
        }

        /// <summary>
        /// Rebuilds the live docking tree from a definition. Panels are matched by key against the
        /// already-registered <see cref="DockPanel"/> components; unknown keys are skipped.
        /// </summary>
        public void MaterializeFromDefinition(DockLayoutDefinition def)
        {
            if (def == null)
                return;

            // Refuse a definition this build cannot fully understand, before touching anything.
            // Materialising it would apply the parts we recognise and silently drop the rest,
            // producing an arrangement that is subtly wrong rather than obviously absent - which is
            // what users report as "it forgot my windows". The current layout is left intact,
            // because it is strictly better than the alternative.
            if (def.SchemaVersion > DockLayoutDefinition.CurrentSchemaVersion)
            {
                OnDockingError("RestoreLayout.Version", null, new NotSupportedException(
                    $"Layout schema version {def.SchemaVersion} is newer than the supported "
                    + $"version {DockLayoutDefinition.CurrentSchemaVersion}; the layout was not "
                    + "applied and the current arrangement was kept."));
                return;
            }

            CloseAllFloatWindows();
            ClearAllAutoHidePanels();

            var root = _layoutTree.Root;

            // Tear down the current group structure (panels stay registered; we re-attach them).
            // Detaching each panel matters: a panel the incoming definition does not mention would
            // otherwise keep pointing at a group that has been removed from the tree and
            // unregistered, leaving it docked, unplaced and unreachable - the same stranding shape
            // that pruning a hidden panel's group produced.
            foreach (var child in root.Children.ToList())
            {
                foreach (var panel in child.GetAllPanelsRecursive())
                {
                    if (panel != null)
                        panel.Group = null;
                }

                root.RemoveChild(child);
                _layoutTree.UnregisterGroup(child.Id);
            }

            if (def.Groups != null)
            {
                foreach (var groupDef in def.Groups)
                    root.AddChild(BuildGroup(groupDef));
            }

            // Sync dockspace TabPosition from restored group HeaderPosition values.
            foreach (var group in root.Children)
                SyncDockspaceHeaderPosition(group);

            if (def.Floating != null)
            {
                foreach (var info in def.Floating)
                {
                    var panel = GetPanel(info.Key);
                    if (panel == null) continue;

                    panel.DockPosition = info.LastDockPosition;
                    if (panel.State != DockPanelState.Floating && panel.CanFloat)
                    {
                        // Where the float lands is decided against the displays that exist now, not
                        // the ones that existed when it was saved. Restoring the raw rectangle is
                        // what puts a tool window onto a monitor the user has since unplugged.
                        var placement = FloatBoundsResolver.Resolve(info, Monitors.GetMonitors());
                        if (placement.Match != FloatBoundsResolver.MatchKind.DeviceName ||
                            placement.Clamped)
                        {
                            OnFloatRelocated(panel, info, placement);
                        }

                        try { FloatPanel(info.Key, placement.Bounds); }
                        catch (Exception ex) { OnDockingError("RestoreLayout.Float", info.Key, ex); }
                    }
                }
            }

            if (def.AutoHidden != null)
            {
                foreach (var info in def.AutoHidden)
                {
                    var panel = GetPanel(info.Key);
                    if (panel == null) continue;

                    panel.DockPosition = info.Edge;
                    if (panel.State != DockPanelState.AutoHidden && panel.CanAutoHide)
                    {
                        try { AutoHidePanel(info.Key); }
                        catch (Exception ex) { OnDockingError("RestoreLayout.AutoHide", info.Key, ex); }
                    }
                }
            }

            // Hidden panels: members of their group, restored to Hidden rather than left visible.
            // A version 1 definition has an empty list, which is the correct reading of "no hidden
            // panels were recorded" - that is what makes the v1 -> v2 migration the identity.
            if (def.Hidden != null)
            {
                foreach (var key in def.Hidden)
                {
                    var panel = GetPanel(key);
                    if (panel == null || panel.State != DockPanelState.Docked)
                        continue;

                    try { HidePanel(key); }
                    catch (Exception ex) { OnDockingError("RestoreLayout.Hide", key, ex); }
                }
            }

            ReHomeUnplacedPanels();

            _layoutController?.InvalidateLayout();
            ApplyLayout();
            ValidateAfterStructuralChange("RestoreLayout");
        }

        /// <summary>
        /// Gives a group back to every docked panel the definition left out.
        /// </summary>
        /// <remarks>
        /// A definition need not mention every registered panel - it may predate one, or a plugin
        /// may have added panels since it was written. Those panels are still docked and still
        /// expected on screen, so they rejoin the group for their own
        /// <see cref="DockPanel.DockPosition"/> rather than being left with no group and no bounds.
        /// Without this, loading a definition that places nothing left <i>every</i> panel unplaced.
        /// </remarks>
        private void ReHomeUnplacedPanels()
        {
            foreach (var panel in _panelsByKey.Values.ToList())
            {
                if (panel == null || panel.Group != null)
                    continue;
                if (panel.State != DockPanelState.Docked && panel.State != DockPanelState.Hidden)
                    continue;

                var group = GetOrCreateGroupAtPosition(panel.DockPosition);
                group.AddPanel(panel);
                group.ActivePanel ??= panel;

                if (panel.State == DockPanelState.Docked)
                    EnsurePanelHosted(panel, makeActive: false);
            }
        }

        private DockGroup BuildGroup(DockGroupDefinition def)
        {
            var group = new DockGroup
            {
                Position = def.Position,
                SplitOrientation = def.SplitOrientation,
                SplitRatio = def.SplitRatio,
                RatioInitialized = true,
                HeaderPosition = def.HeaderPosition
            };
            _layoutTree.RegisterGroup(group);

            if (def.PanelKeys != null)
            {
                foreach (var key in def.PanelKeys)
                {
                    var panel = GetPanel(key);
                    if (panel == null) continue;
                    panel.DockPosition = def.Position;
                    panel.State = DockPanelState.Docked;
                    panel.ShowCaption = true;
                    panel.Visible = true;
                    group.AddPanel(panel);
                }
            }

            if (!string.IsNullOrEmpty(def.ActivePanelKey))
            {
                var active = GetPanel(def.ActivePanelKey);
                if (active != null && group.Panels.Contains(active))
                    group.ActivePanel = active;
            }

            if (def.Children != null)
            {
                foreach (var childDef in def.Children)
                    group.AddChild(BuildGroup(childDef));
            }

            return group;
        }

        /// <summary>Runtime convenience: capture the current layout definition.</summary>
        public DockLayoutDefinition SaveLayout() => CaptureDefinition();

        /// <summary>Runtime convenience: apply a previously captured layout definition.</summary>
        public void LoadLayout(DockLayoutDefinition definition) => MaterializeFromDefinition(definition);

        private void SyncDockspaceHeaderPosition(DockGroup group)
        {
            if (group == null || _hostForm == null) return;
            var ds = FindDockspaceAt(_hostForm, group.Position);
            if (ds != null)
                ds.TabPosition = group.HeaderPosition;
            foreach (var child in group.Children)
                SyncDockspaceHeaderPosition(child);
        }
    }
}
