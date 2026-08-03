using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Layout;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Painters;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// The draggable splitter controls, reconciled against each layout pass.
    /// </summary>
    /// <remarks>
    /// Splitters are <b>derived</b> from the layout result, not stored alongside it: every pass
    /// creates the ones the result calls for, repositions those that already exist, and disposes
    /// any that are no longer named. Nothing outside this file should create or position one, or
    /// the two sets drift and the user drags a splitter that moves nothing.
    /// <para>
    /// That reconciliation is also why a maximised layout needs no special handling here — it simply
    /// produces no splitters, so they are all disposed as orphans and rebuilt on restore.
    /// </para>
    /// </remarks>
    public partial class BeepDockingManager
    {
        /// <summary>
        /// Reconciles the live <see cref="BeepDockSplitter"/> controls with the splitter
        /// rectangles produced by the layout engine.
        ///
        /// Two kinds of splitters:
        /// <list type="bullet">
        ///   <item><b>Root-level edge splitters</b> (GroupId matches a root-level edge group's id)
        ///   stay on the host form, positioned in container coordinates. They control the size
        ///   of a dockspace docked at a form edge.</item>
        ///   <item><b>Child splitters</b> (GroupId has <c>_child_</c> in it, format
        ///   <c>{parentId}_child_{i}</c>) are owned by the dockspace that hosts the parent group.
        ///   Their bounds are translated to dockspace-local coordinates and the splitter is
        ///   parented to that dockspace, so it floats over the dockspace's child panels.</item>
        /// </list>
        ///
        /// Orphaned splitters (no longer in the result) are disposed in both populations.
        /// </summary>
        private void SyncSplitters(DockLayoutResult result)
        {
            if (_hostForm == null || IsDesignHosted || result == null)
                return;

            // 1) Index managed dockspaces by their primary group's id so we can route child
            //    splitters to the right owner. A dockspace's primary group is the group of any
            //    one of its child panels.
            var dockspaceByGroupId = new Dictionary<string, BeepDockspace>(StringComparer.Ordinal);
            foreach (var dockspace in GetManagedDockspaces())
            {
                if (dockspace == null || dockspace.IsDisposed)
                    continue;

                foreach (var panel in dockspace.Panels)
                {
                    var groupId = panel?.Group?.Id;
                    if (!string.IsNullOrEmpty(groupId) && !dockspaceByGroupId.ContainsKey(groupId))
                        dockspaceByGroupId[groupId] = dockspace;
                }
            }

            // 2) Bucket each hit into root-level (host form) or child (dockspace-owned).
            var desiredRoot = new HashSet<string>(StringComparer.Ordinal);
            var desiredChildByDockspace = new Dictionary<BeepDockspace, Dictionary<string, (Rectangle Bounds, bool IsVertical)>>();

            foreach (var hit in result.Splitters)
            {
                if (string.IsNullOrEmpty(hit.GroupId))
                    continue;

                int childMarker = hit.GroupId.IndexOf("_child_", StringComparison.Ordinal);
                if (childMarker > 0)
                {
                    string parentId = hit.GroupId.Substring(0, childMarker);
                    if (!dockspaceByGroupId.TryGetValue(parentId, out var dockspace) || dockspace == null)
                        continue;

                    // Translate the engine's container-coordinate bounds into the dockspace's
                    // local client coords.
                    var local = new Rectangle(
                        hit.Bounds.X - dockspace.Bounds.X,
                        hit.Bounds.Y - dockspace.Bounds.Y,
                        hit.Bounds.Width,
                        hit.Bounds.Height);

                    if (!desiredChildByDockspace.TryGetValue(dockspace, out var map))
                        desiredChildByDockspace[dockspace] = map = new Dictionary<string, (Rectangle, bool)>(StringComparer.Ordinal);
                    map[hit.GroupId] = (local, hit.IsVertical);
                    continue;
                }

                desiredRoot.Add(hit.GroupId);

                if (!_splitters.TryGetValue(hit.GroupId, out var splitter) || splitter == null || splitter.IsDisposed)
                {
                    splitter = new BeepDockSplitter { GroupId = hit.GroupId };
                    splitter.ControlStyle = _style;
                    splitter.ApplyDockingTheme(_themeColors);
                    splitter.SplitterMoved += OnEngineSplitterMoved;
                    _splitters[hit.GroupId] = splitter;
                    _hostForm.Controls.Add(splitter);
                }

                splitter.Orientation = hit.IsVertical ? SplitterOrientation.Vertical : SplitterOrientation.Horizontal;
                splitter.Bounds = hit.Bounds;
                splitter.Visible = true;
                splitter.BringToFront();
            }

            // 3) Reconcile dockspace-owned child splitters.
            foreach (var ds in GetManagedDockspaces())
            {
                if (ds == null || ds.IsDisposed)
                    continue;
                if (desiredChildByDockspace.TryGetValue(ds, out var map))
                    ds.UpdateChildSplitters(map, OnEngineSplitterMoved);
                else
                    ds.ClearChildSplitters();
            }

            // 4) Dispose root-level orphans.
            var orphans = _splitters.Keys.Where(k => !desiredRoot.Contains(k)).ToList();
            foreach (var key in orphans)
            {
                var splitter = _splitters[key];
                if (splitter != null)
                {
                    splitter.SplitterMoved -= OnEngineSplitterMoved;
                    if (_hostForm.Controls.Contains(splitter))
                        _hostForm.Controls.Remove(splitter);
                    splitter.Dispose();
                }
                _splitters.Remove(key);
            }
        }

        /// <summary>
        /// Handles a live splitter drag: converts the pixel delta into an edge-group ratio
        /// via the layout engine, then re-applies the whole layout.
        /// </summary>
        private void OnEngineSplitterMoved(object sender, SplitterMovedEventArgs e)
        {
            if (sender is BeepDockSplitter splitter && !string.IsNullOrEmpty(splitter.GroupId))
            {
                _layoutController?.DragSplitter(splitter.GroupId, e.Delta);
                ApplyLayout();

                var group = _layoutTree.GetGroup(splitter.GroupId);
                var panel = group?.ActivePanel;
                if (panel == null || panel.State != DockPanelState.Docked)
                    panel = group?.Panels.FirstOrDefault(p => p.State == DockPanelState.Docked);

                OnDockspaceSeparatorResize(new SeparatorResizeEventArgs(panel, splitter.Bounds));
            }
        }
    }
}
