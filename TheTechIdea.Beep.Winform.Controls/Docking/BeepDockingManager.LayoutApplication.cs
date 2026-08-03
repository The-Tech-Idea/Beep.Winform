using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Layout;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Turning a computed layout into actual control bounds.
    /// </summary>
    /// <remarks>
    /// The division of labour with <see cref="DockingLayoutController"/> is the thing to preserve:
    /// the controller decides <b>geometry</b> from the tree and never touches a control; this file
    /// applies that geometry and never computes any. Every rectangle here comes from the result.
    /// <para>
    /// Recomputing a bound locally — even one that "obviously" follows — is how a layout ends up
    /// with two implementations that disagree, and the disagreement only shows up as a panel a few
    /// pixels out of place. If a number is needed that the result does not carry, it belongs in the
    /// controller.
    /// </para>
    /// </remarks>
    public partial class BeepDockingManager
    {
        /// <summary>
        /// Single layout pass: seeds edge-group ratios from preferred sizes (first time only),
        /// runs the <see cref="DockingLayoutController"/> against the host client area, then
        /// positions every docked panel and reconciles the edge splitters from the result.
        /// This is the sole positioning path — all callers route through here.
        ///
        /// Panels that live inside a <see cref="BeepDockspace"/> are positioned by the dockspace
        /// itself (its <c>LayoutPanels</c> runs in <c>OnLayout</c>); we just call PerformLayout
        /// on each dockspace. Panels that live directly on the host form (legacy paths) get
        /// bounds from the layout controller result, as before.
        /// </summary>
        public void ApplyLayout()
        {
            if (_hostForm == null || IsDesignHosted || _disposed || _layoutController == null)
                return;

            PruneEmptyRootGroups();

            var client = GetLayoutClientBounds(_hostForm.DisplayRectangle);
            _layoutController.ContainerBounds = client;
            SeedEdgeRatios(client);

            var result = _layoutController.CalculateLayout(client);

            _hostForm.SuspendLayout();
            try
            {
                // 1) Position dockspaces on the host form via DockStyle. The WinForms layout
                //    engine will then handle their actual Bounds in its own layout pass.
                SyncDockspaceDockStyles();

                foreach (var panel in _panelsByKey.Values)
                {
                    if (panel == null)
                        continue;
                    if (panel.State != DockPanelState.Docked)
                        continue;

                    // 2) Panels hosted by a dockspace: let the dockspace do the layout.
                    if (panel.Parent is BeepDockspace dockspace)
                    {
                        dockspace.PerformLayout();
                        continue;
                    }

                    // 3) Legacy path: panels directly on the host form get bounds from the engine.
                    if (panel.Parent != _hostForm)
                        continue;

                    var bounds = result.GetPanelBounds(panel.Key);
                    if (!bounds.HasValue)
                        continue;

                    panel.Bounds = bounds.Value;
                    panel.LayoutBounds = bounds.Value;
                }

                SyncSplitters(result);

                // Raise the active panel of each tabbed group above its stack-mates.
                foreach (var group in _layoutTree.Root.Children)
                    BringActivePanelToFrontRecursive(group);
            }
            finally
            {
                _hostForm.ResumeLayout();
            }
        }

        /// <summary>
        /// Sets the <see cref="DockStyle"/> on every managed <see cref="BeepDockspace"/> based on
        /// its <see cref="BeepDockspace.DockPosition"/>. The WinForms layout engine then sizes
        /// each dockspace to the matching edge of the host form (or Fill for the central
        /// workspace dockspace). Dockspaces whose DockStyle is already correct are skipped.
        /// </summary>
        private void SyncDockspaceDockStyles()
        {
            foreach (var dockspace in GetManagedDockspaces())
            {
                if (dockspace == null || dockspace.IsDisposed)
                    continue;

                DockStyle desired = ConvertDockPositionToStyle(dockspace.DockPosition);
                if (dockspace.Dock != desired)
                    dockspace.Dock = desired;
            }
        }

        private static DockStyle ConvertDockPositionToStyle(DockPosition position)
        {
            switch (position)
            {
                case DockPosition.Left:   return DockStyle.Left;
                case DockPosition.Right:  return DockStyle.Right;
                case DockPosition.Top:    return DockStyle.Top;
                case DockPosition.Bottom: return DockStyle.Bottom;
                default:                  return DockStyle.Fill;
            }
        }

        /// <summary>
        /// Seeds each edge group's <see cref="DockGroup.SplitRatio"/> from its active panel's
        /// preferred size the first time it is laid out, so the engine reproduces the panel's
        /// requested width/height. Once seeded (or after a user splitter drag) the ratio is the
        /// canonical resize state and is not overwritten.
        /// </summary>
        private void SeedEdgeRatios(Rectangle client)
        {
            foreach (var group in _layoutTree.Root.Children)
                SeedGroupAndDescendants(group, client);
        }

        /// <summary>Removes root-level groups that no longer contain any docked panels.</summary>
        /// <summary>
        /// Drops root groups that no longer own any panel. Floating, auto-hiding and closing a panel
        /// all detach it from its group (<c>Group.RemovePanel</c>), so a group emptied that way is
        /// genuinely dead and would otherwise linger in the tree.
        /// </summary>
        /// <remarks>
        /// The test is <b>membership</b>, not visibility. <see cref="HidePanel"/> leaves the panel in
        /// its group and only flips <see cref="DockPanel.State"/> — so a group whose panels are all
        /// hidden is still a live group waiting for them to come back, and pruning it strands the
        /// panel: <see cref="ShowPanel"/>'s re-join branch is guarded on <c>panel.Group == null</c>,
        /// which a pruned-but-still-referenced group does not satisfy, leaving the panel with no
        /// allocation and its former siblings holding the space forever.
        /// <para>
        /// Keeping such a group costs no space: <c>DockingLayoutController</c> independently skips
        /// groups with no <see cref="DockPanelState.Docked"/> panel when it allocates bounds, so the
        /// edge collapses while hidden and is restored when the panel returns.
        /// </para>
        /// </remarks>
        private void PruneEmptyRootGroups()
        {
            foreach (var child in _layoutTree.Root.Children.ToList())
            {
                if (GroupHasMembers(child))
                    continue;

                _layoutTree.Root.RemoveChild(child);
                _layoutTree.UnregisterGroup(child.Id);
            }
        }

        /// <summary>True when the group, or any descendant, still owns at least one panel.</summary>
        private static bool GroupHasMembers(DockGroup group)
            => group != null && group.GetAllPanelsRecursive().Any(p => p != null);

        /// <summary>
        /// Back-compat shim: any legacy caller that asked to position a single panel now
        /// triggers a full engine-driven layout pass.
        /// </summary>
        private void ApplyLayoutBounds(DockPanel panel) => ApplyLayout();

        /// <summary>
        /// Back-compat shim: any legacy caller that asked to position a group now triggers a
        /// full engine-driven layout pass.
        /// </summary>
        private void ApplyDockGroupBounds(DockGroup group) => ApplyLayout();

        /// <summary>
        /// Shrinks the layout engine client area so docked panels do not draw under auto-hide strips.
        /// </summary>
        private Rectangle GetLayoutClientBounds(Rectangle fullClient)
        {
            int left = fullClient.X;
            int top = fullClient.Y;
            int right = fullClient.Right;
            int bottom = fullClient.Bottom;

            foreach (var kv in _autoHideStrips)
            {
                var strip = kv.Value;
                if (strip == null || strip.Panels.Count == 0)
                    continue;

                int inset = AutoHideStrip.TabSize + strip.SlideExtent;
                switch (kv.Key)
                {
                    case DockPosition.Left:
                        left += inset;
                        break;
                    case DockPosition.Right:
                        right -= inset;
                        break;
                    case DockPosition.Top:
                        top += inset;
                        break;
                    case DockPosition.Bottom:
                        bottom -= inset;
                        break;
                }
            }

            int width = Math.Max(0, right - left);
            int height = Math.Max(0, bottom - top);
            return new Rectangle(left, top, width, height);
        }

        private static void BringActivePanelToFrontRecursive(DockGroup group)
        {
            if (group == null) return;
            group.ActivePanel?.BringToFront();
            foreach (var child in group.Children)
                BringActivePanelToFrontRecursive(child);
        }
    }
}
