using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// What a panel state transition is, and the invariant every one of them must leave true.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="DockPanel.State"/> is assigned in seventeen places across five files. That is not
    /// itself the problem — the operations genuinely differ. The problem is that a state is only
    /// half the truth about a panel. Four things move together:
    /// </para>
    /// <list type="number">
    /// <item><b>State</b> — <see cref="DockPanel.State"/>.</item>
    /// <item><b>Membership</b> — <see cref="DockPanel.Group"/>, and whether that group is still
    /// reachable from the tree root.</item>
    /// <item><b>Hosting</b> — the control the panel is parented to: the host form, a dockspace, a
    /// float window, or nothing.</item>
    /// <item><b>Allocation</b> — whether the layout gives it bounds.</item>
    /// </list>
    /// <para>
    /// <b>A transition is the point where all four change together.</b> Every defect this program
    /// found in that area was one of them moving without the others, and each stayed invisible until
    /// something downstream read the stale one:
    /// </para>
    /// <list type="bullet">
    /// <item>Hiding a panel pruned its group from the tree while the panel kept pointing at it, so
    /// showing it again produced no bounds.</item>
    /// <item>Restoring a layout tore down every group while panels still referenced them, so a panel
    /// the definition did not mention stayed docked, unplaced and unreachable.</item>
    /// <item>Closing a float window left the panel at <see cref="DockPanelState.Floating"/> with
    /// nothing backing it, so three separate decisions skipped it.</item>
    /// </list>
    /// <para>
    /// So the seam here is <b>not</b> another file split. Splitting these operations apart would
    /// separate members that must agree and make the coupling harder to see, not easier. The seam is
    /// the invariant, stated and checked: <see cref="ValidatePanelStates"/> catches all three of the
    /// shapes above.
    /// </para>
    /// </remarks>
    public partial class BeepDockingManager
    {
        /// <summary>A panel whose four facts do not agree.</summary>
        public sealed class PanelStateViolation
        {
            public PanelStateViolation(string panelKey, DockPanelState state, string problem)
            {
                PanelKey = panelKey;
                State = state;
                Problem = problem;
            }

            public string PanelKey { get; }

            public DockPanelState State { get; }

            /// <summary>Which part of the invariant is broken, in plain terms.</summary>
            public string Problem { get; }

            public override string ToString() => PanelKey + " (" + State + "): " + Problem;
        }

        /// <summary>
        /// Checks every panel's state against its membership, hosting and allocation.
        /// </summary>
        /// <remarks>
        /// The rules, one per state:
        /// <list type="bullet">
        /// <item><b>Docked</b> — belongs to a group that is reachable from the root.</item>
        /// <item><b>Hidden</b> — still belongs to a reachable group. Hiding changes visibility, not
        /// membership; a hidden panel is one that is coming back.</item>
        /// <item><b>Floating</b> — detached from any live group, and a float window exists for it.</item>
        /// <item><b>AutoHidden</b> — detached from any live group, and a strip holds it.</item>
        /// <item><b>Closed</b> — detached from any live group.</item>
        /// </list>
        /// </remarks>
        public IReadOnlyList<PanelStateViolation> ValidatePanelStates()
        {
            var violations = new List<PanelStateViolation>();
            if (_layoutTree?.Root == null)
                return violations;

            var reachable = new HashSet<string>(StringComparer.Ordinal);
            CollectGroupIds(_layoutTree.Root, reachable);

            foreach (var panel in _panelsByKey.Values)
            {
                if (panel == null || panel.IsDisposed)
                    continue;

                bool inGroup = panel.Group != null;
                bool groupLive = inGroup && reachable.Contains(panel.Group.Id);

                switch (panel.State)
                {
                    case DockPanelState.Docked:
                    case DockPanelState.Hidden:
                        if (!inGroup)
                            Add(panel, "belongs to no group, so nothing will ever lay it out");
                        else if (!groupLive)
                            Add(panel, "references group " + panel.Group.Id + ", which is not in the tree");
                        else if (!panel.Group.Panels.Contains(panel))
                            Add(panel, "points at group " + panel.Group.Id + " but is not one of its panels");
                        break;

                    case DockPanelState.Floating:
                        if (groupLive)
                            Add(panel, "is floating yet still a member of a live group");
                        if (!_floatWindowsByKey.ContainsKey(panel.Key))
                            Add(panel, "is floating with no float window backing it");
                        break;

                    case DockPanelState.AutoHidden:
                        if (groupLive)
                            Add(panel, "is auto-hidden yet still a member of a live group");
                        if (!_autoHideStrips.Values.Any(s => s != null && s.Panels.Contains(panel)))
                            Add(panel, "is auto-hidden but no strip holds it");
                        break;

                    case DockPanelState.Closed:
                        if (groupLive)
                            Add(panel, "is closed yet still a member of a live group");
                        break;
                }
            }

            return violations;

            void Add(DockPanel panel, string problem)
                => violations.Add(new PanelStateViolation(panel.Key, panel.State, problem));
        }

        private static void CollectGroupIds(DockGroup group, HashSet<string> into)
        {
            if (group == null || !into.Add(group.Id))
                return;

            foreach (var child in group.Children)
                CollectGroupIds(child, into);
        }

        /// <summary>
        /// Reports any panel whose state no longer agrees with the rest of its facts.
        /// </summary>
        /// <remarks>
        /// Runs alongside the layout-consistency check after a structural change, and reports
        /// through <see cref="DockingError"/> rather than throwing: a panel in a contradictory state
        /// is worth surfacing, but tearing down the user's session over it is worse than carrying on
        /// with a layout that is merely wrong in one place.
        /// </remarks>
        private void ReportPanelStateViolations(string context)
        {
            if (!ValidateLayoutOnChange || _disposed || IsDesignHosted)
                return;

            var violations = ValidatePanelStates();
            if (violations.Count == 0)
                return;

            OnDockingError(context, violations[0].PanelKey, new InvalidOperationException(
                "Panel state is inconsistent after " + context + ": "
                + string.Join("; ", violations.Select(v => v.ToString()))));
        }
    }
}
