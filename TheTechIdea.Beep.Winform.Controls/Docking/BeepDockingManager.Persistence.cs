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
            => CaptureDefinition(DockLayoutScope.All);

        /// <summary>
        /// Snapshots the current layout, limited to <paramref name="scope"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="DockLayoutScope.Structure"/> omits the active tab of each group and the list
        /// of hidden panels, so the result describes an arrangement without carrying one window's
        /// working state into another.
        /// </remarks>
        public DockLayoutDefinition CaptureDefinition(DockLayoutScope scope)
        {
            var def = new DockLayoutDefinition();
            FillDefinition(def, scope);
            return def;
        }

        /// <summary>Populates the supplied definition in place from the current live tree + runtime state.</summary>
        private void FillDefinition(DockLayoutDefinition def)
            => FillDefinition(def, DockLayoutScope.All);

        private void FillDefinition(DockLayoutDefinition def, DockLayoutScope scope)
        {
            bool session = (scope & DockLayoutScope.Session) != 0;

            def.SchemaVersion = DockLayoutDefinition.CurrentSchemaVersion;
            def.Groups.Clear();
            def.Floating.Clear();
            def.AutoHidden.Clear();
            def.Hidden.Clear();
            def.Perspectives.Clear();

            // Skip empty edge groups: panels that float/auto-hide/close are removed from their
            // group but the (now empty) group lingers in the tree. Serializing it would add noise
            // and recreate dead groups on load.
            foreach (var group in _layoutTree.Root.Children)
                if (GroupHasMembers(group))
                    def.Groups.Add(CaptureGroup(group, session));

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

            // Which panels are hidden is working state, not shape.
            if (session)
            {
                foreach (var panel in _panelsByKey.Values)
                {
                    if (panel?.Key != null && panel.State == DockPanelState.Hidden)
                        def.Hidden.Add(panel.Key);
                }
            }

            // Named layouts travel with the layout they were saved beside. Without this they live
            // only on the manager and are gone when the application closes, which makes saving one
            // close to pointless.
            foreach (var perspective in _perspectives)
            {
                if (perspective?.Layout == null || string.IsNullOrWhiteSpace(perspective.Name))
                    continue;

                def.Perspectives.Add(new DockPerspectiveDefinition
                {
                    Name = perspective.Name,
                    IsDefault = perspective.IsDefault,
                    Layout = perspective.Layout,
                });
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
            => CaptureGroup(group, includeSession: true);

        private static DockGroupDefinition CaptureGroup(DockGroup group, bool includeSession)
        {
            var def = new DockGroupDefinition
            {
                Position = group.Position,
                SplitOrientation = group.SplitOrientation,
                SplitRatio = group.SplitRatio,
                // Which tab is in front is working state; the group's membership is not.
                ActivePanelKey = includeSession ? group.ActivePanel?.Key : null,
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
                    def.Children.Add(CaptureGroup(child, includeSession));

            return def;
        }

        /// <summary>
        /// Rebuilds the live docking tree from a definition. Panels are matched by key against the
        /// already-registered <see cref="DockPanel"/> components; unknown keys are skipped.
        /// </summary>
        public void MaterializeFromDefinition(DockLayoutDefinition def)
            => MaterializeFromDefinition(def, restorePerspectives: true, scope: DockLayoutScope.All);

        /// <summary>
        /// Rebuilds the live tree from a definition.
        /// </summary>
        /// <param name="restorePerspectives">
        /// True for a top-level load, which brings the stored named layouts with it. <b>False when
        /// applying a perspective</b>: a perspective's arrangement is materialised through this same
        /// method, and restoring from it would replace the manager's whole perspective list with
        /// whatever copy that one perspective happened to carry.
        /// </param>
        private void MaterializeFromDefinition(DockLayoutDefinition def, bool restorePerspectives)
            => MaterializeFromDefinition(def, restorePerspectives, DockLayoutScope.All);

        private void MaterializeFromDefinition(DockLayoutDefinition def, bool restorePerspectives,
                                               DockLayoutScope scope)
        {
            bool applySession = (scope & DockLayoutScope.Session) != 0;

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
                    root.AddChild(BuildGroup(groupDef, applySession));
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
            if (applySession && def.Hidden != null)
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

            if (restorePerspectives)
                RestorePerspectives(def);

            ReHomeUnplacedPanels();

            _layoutController?.InvalidateLayout();
            ApplyLayout();
            ValidateAfterStructuralChange("RestoreLayout");
        }

        /// <summary>
        /// Repopulates the named layouts from a definition, without applying any of them.
        /// </summary>
        /// <remarks>
        /// Restoring a layout restores <i>that</i> layout; the perspectives it carries are choices
        /// available afterwards. Applying one here would silently override the arrangement the user
        /// just asked for.
        /// <para>
        /// A definition carrying no perspectives leaves the existing ones alone rather than clearing
        /// them - a version 1 or 2 layout predates the field, and reading "absent" as "delete them
        /// all" would destroy the user's saved layouts on the first load of an older file.
        /// </para>
        /// </remarks>
        private void RestorePerspectives(DockLayoutDefinition def)
        {
            if (def?.Perspectives == null || def.Perspectives.Count == 0)
                return;

            _perspectives.Clear();
            foreach (var saved in def.Perspectives)
            {
                if (saved == null || string.IsNullOrWhiteSpace(saved.Name))
                    continue;

                _perspectives.Add(new DockPerspective
                {
                    Name = saved.Name,
                    IsDefault = saved.IsDefault,
                    Layout = saved.Layout ?? new DockLayoutDefinition(),
                });
            }

            // The active perspective is a session fact, not a stored one: what is on screen after a
            // restore is the restored layout, which may match no saved perspective at all.
            _activePerspectiveName = null;
            OnPerspectivesChanged();
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
            => BuildGroup(def, includeSession: true);

        /// <param name="includeSession">
        /// False when only the arrangement's shape is being applied. The group is then rebuilt with
        /// the same membership, but the active tab is left as it is and a panel the user had hidden
        /// stays hidden - re-docking it would be applying working state under the guise of shape.
        /// </param>
        private DockGroup BuildGroup(DockGroupDefinition def, bool includeSession)
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

                    // A hidden panel keeps its state when only the shape is being applied; it is
                    // still a member of the group, just not on screen.
                    bool keepHidden = !includeSession && panel.State == DockPanelState.Hidden;
                    if (!keepHidden)
                    {
                        panel.State = DockPanelState.Docked;
                        panel.ShowCaption = true;
                        panel.Visible = true;
                    }

                    group.AddPanel(panel);
                }
            }

            if (includeSession && !string.IsNullOrEmpty(def.ActivePanelKey))
            {
                var active = GetPanel(def.ActivePanelKey);
                if (active != null && group.Panels.Contains(active))
                    group.ActivePanel = active;
            }

            if (def.Children != null)
            {
                foreach (var childDef in def.Children)
                    group.AddChild(BuildGroup(childDef, includeSession));
            }

            return group;
        }

        /// <summary>Runtime convenience: capture the current layout definition.</summary>
        public DockLayoutDefinition SaveLayout() => CaptureDefinition(DockLayoutScope.All);

        /// <summary>Saves the layout, limited to <paramref name="scope"/>.</summary>
        public DockLayoutDefinition SaveLayout(DockLayoutScope scope) => CaptureDefinition(scope);

        /// <summary>Runtime convenience: apply a previously captured layout definition.</summary>
        public void LoadLayout(DockLayoutDefinition definition) => MaterializeFromDefinition(definition);

        /// <summary>
        /// Applies a saved arrangement, limited to <paramref name="scope"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="DockLayoutScope.Structure"/> rearranges the panels but leaves the working
        /// state alone: whichever tab was in front stays in front, and panels the user had hidden
        /// stay hidden. That is what makes "reset my layout" usable without also throwing away what
        /// they had open.
        /// </remarks>
        public void LoadLayout(DockLayoutDefinition definition, DockLayoutScope scope)
            => MaterializeFromDefinition(definition, restorePerspectives: true, scope: scope);

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
