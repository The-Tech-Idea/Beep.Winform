using System.ComponentModel;
using System.Linq;
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
            def.SchemaVersion = _layoutTree.SchemaVersion;
            def.Groups.Clear();
            def.Floating.Clear();
            def.AutoHidden.Clear();

            // Skip empty edge groups: panels that float/auto-hide/close are removed from their
            // group but the (now empty) group lingers in the tree. Serializing it would add noise
            // and recreate dead groups on load.
            foreach (var group in _layoutTree.Root.Children)
                if (GroupHasContent(group))
                    def.Groups.Add(CaptureGroup(group));

            if (_hostForm != null)
            {
                foreach (var kv in _floatWindowsByKey)
                {
                    var fw = kv.Value;
                    if (fw?.Panel == null)
                        continue;

                    def.Floating.Add(new FloatingPanelInfo
                    {
                        Key = fw.Panel.Key,
                        Bounds = fw.Bounds,
                        LastDockPosition = fw.Panel.DockPosition
                    });
                }

                foreach (var owned in _hostForm.OwnedForms)
                {
                    if (owned is FloatWindow fw && fw.Panel != null &&
                        !_floatWindowsByKey.ContainsKey(fw.Panel.Key))
                    {
                        def.Floating.Add(new FloatingPanelInfo
                        {
                            Key = fw.Panel.Key,
                            Bounds = fw.Bounds,
                            LastDockPosition = fw.Panel.DockPosition
                        });
                    }
                }
            }

            foreach (var kv in _autoHideStrips)
            {
                foreach (var panel in kv.Value.Panels)
                    def.AutoHidden.Add(new AutoHiddenPanelInfo { Key = panel.Key, Edge = kv.Key });
            }
        }

        private static DockGroupDefinition CaptureGroup(DockGroup group)
        {
            var def = new DockGroupDefinition
            {
                Position = group.Position,
                SplitOrientation = group.SplitOrientation,
                SplitRatio = group.SplitRatio,
                ActivePanelKey = group.ActivePanel?.Key
            };

            foreach (var panel in group.Panels)
            {
                if (panel?.Key != null && panel.State == DockPanelState.Docked)
                    def.PanelKeys.Add(panel.Key);
            }

            foreach (var child in group.Children)
                if (GroupHasContent(child))
                    def.Children.Add(CaptureGroup(child));

            return def;
        }

        /// <summary>True when the group (or any descendant) holds at least one docked panel.</summary>
        private static bool GroupHasContent(DockGroup group)
            => group != null &&
               group.GetAllPanelsRecursive().Any(p => p != null && p.State == DockPanelState.Docked);

        /// <summary>
        /// Rebuilds the live docking tree from a definition. Panels are matched by key against the
        /// already-registered <see cref="DockPanel"/> components; unknown keys are skipped.
        /// </summary>
        public void MaterializeFromDefinition(DockLayoutDefinition def)
        {
            if (def == null)
                return;

            CloseAllFloatWindows();
            ClearAllAutoHidePanels();

            var root = _layoutTree.Root;

            // Tear down the current group structure (panels stay registered; we re-attach them).
            foreach (var child in root.Children.ToList())
            {
                root.RemoveChild(child);
                _layoutTree.UnregisterGroup(child.Id);
            }

            if (def.Groups != null)
            {
                foreach (var groupDef in def.Groups)
                    root.AddChild(BuildGroup(groupDef));
            }

            if (def.Floating != null)
            {
                foreach (var info in def.Floating)
                {
                    var panel = GetPanel(info.Key);
                    if (panel == null) continue;

                    panel.DockPosition = info.LastDockPosition;
                    if (panel.State != DockPanelState.Floating && panel.CanFloat)
                    {
                        try { FloatPanel(info.Key, info.Bounds); } catch { /* best-effort restore */ }
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
                        try { AutoHidePanel(info.Key); } catch { /* best-effort restore */ }
                    }
                }
            }

            _layoutController?.InvalidateLayout();
            ApplyLayout();
        }

        private DockGroup BuildGroup(DockGroupDefinition def)
        {
            var group = new DockGroup
            {
                Position = def.Position,
                SplitOrientation = def.SplitOrientation,
                SplitRatio = def.SplitRatio,
                RatioInitialized = true
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
    }
}
