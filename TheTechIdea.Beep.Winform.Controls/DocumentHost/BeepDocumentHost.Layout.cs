// BeepDocumentHost.Layout.cs
// Layout calculation for BeepDocumentHost — positions the tab strip and content area,
// and keeps all visible document panels sized to fill the content area.
// Supports single-group (original) and multi-group nested split-view via ILayoutNode tree.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        internal bool GetLayoutSuspended() => _layoutSuspended;

        internal void SetLayoutSuspended(bool value) => _layoutSuspended = value;

        internal string? GetActiveGroupId()
            => _activeGroup?.GroupId;

        internal void ValidateAndRepairLayoutTree(string operationName)
        {
            if (_layoutRoot == null)
            {
                SyncLayoutTree();
                return;
            }

            var validation = Layout.LayoutTreeValidator.Validate(_layoutRoot);
            if (validation.IsValid) return;

            var (repairedRoot, _) = Layout.LayoutTreeRepairer.Repair(_layoutRoot);

            // Only accept repaired trees that still reference known runtime groups;
            // otherwise rebuild from runtime group state to prevent drift.
            bool allGroupsKnown = true;
            foreach (var leaf in repairedRoot.AllLeaves())
            {
                if (!_groupById.ContainsKey(leaf.NodeId))
                {
                    allGroupsKnown = false;
                    break;
                }
            }

            if (allGroupsKnown)
                _layoutRoot = repairedRoot;
            else
                SyncLayoutTree();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Override hooks
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!IsHandleCreated) return;
            RecalculateLayout();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (!IsHandleCreated) return;
            if (_inLayoutRecalc) return;
            RecalculateLayout();
            SyncPanelBounds();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Splitter mouse handling
        // ─────────────────────────────────────────────────────────────────────

        private bool _draggingSplitter;
        private int  _splitterDragStart;
        private float _splitRatioAtDragStart;
        private SplitLayoutNode? _draggedSplitNode;

        private void EnsureSplitterBar()
        {
            if (_splitterBar != null) return;
            _splitterBar = new BeepDocumentSplitterBar();
            _splitterBar.IsHorizontal = _splitHorizontal;
            _splitterBar.ApplyTheme(_currentTheme);
            _splitterBar.MouseDown  += Splitter_MouseDown;
            _splitterBar.MouseMove  += Splitter_MouseMove;
            _splitterBar.MouseUp    += Splitter_MouseUp;
            Controls.Add(_splitterBar);
            _splitterBar.BringToFront();
        }

        private void Splitter_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _draggingSplitter     = true;
            _splitterDragStart    = _splitHorizontal
                ? _splitterBar!.Left + e.X
                : _splitterBar!.Top  + e.Y;
            _splitRatioAtDragStart = _splitRatio;

            // Find the split node associated with this splitter
            if (_layoutRoot is SplitLayoutNode rootSplit)
                _draggedSplitNode = rootSplit;
        }

        private void Splitter_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_draggingSplitter) return;
            int pos = _splitHorizontal
                ? (_splitterBar!.Left + e.X)
                : (_splitterBar!.Top  + e.Y);
            int total = _splitHorizontal ? ClientSize.Width : ClientSize.Height;
            int thickness = DocTokens.SplitterBarThickness;
            if (total <= thickness * 2) return;

            float newRatio = Math.Max(0.1f,
                           Math.Min(0.9f, (float)pos / total));

            // Update the ratio on the dragged split node
            if (_draggedSplitNode != null)
                _draggedSplitNode.Ratio = newRatio;
            else
                _splitRatio = newRatio;

            RecalculateLayout();
        }

        private void Splitter_MouseUp(object? sender, MouseEventArgs e)
        {
            _draggingSplitter = false;
            _draggedSplitNode = null;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Layout engine — tree-based
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Positions the tab strip(s) and content area(s) according to the
        /// <see cref="_layoutRoot"/> tree.  Falls back to flat layout when the
        /// tree is a single leaf.
        /// </summary>
        public void RecalculateLayout()
        {
            if (_layoutSuspended) return;
            if (_inLayoutRecalc) return;
            if (_tabStrip == null || _contentArea == null) return;

            _inLayoutRecalc = true;
            SuspendLayout();
            try
            {
                // Sync the tree with the current group topology
                SyncLayoutTree();

                _layoutRoot.Bounds = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

                if (_layoutRoot is SplitLayoutNode rootSplit)
                    ApplyTreeLayout(rootSplit);
                else
                    ApplySingleGroupLayout();

                PositionAutoHideStrips();
            }
            finally
            {
                ResumeLayout(true);
                _inLayoutRecalc = false;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Tree sync — builds/updates the layout tree from the flat group list
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Synchronises <see cref="_layoutRoot"/> with the current <see cref="_groups"/> list.
        /// When there is only one group, the root is a <see cref="GroupLayoutNode"/>.
        /// When there are multiple groups, the root is a <see cref="SplitLayoutNode"/>
        /// with the primary group on the left/top and a nested subtree for the rest.
        /// </summary>
        private void SyncLayoutTree()
        {
            if (_groups.Count <= 1)
            {
                _layoutRoot = new GroupLayoutNode(_primaryGroup.GroupId);
                return;
            }

            // Build a left-leaning binary tree from the group list:
            //   Split(Primary, Split(Group2, Split(Group3, Group4)))
            // This preserves the flat ordering while enabling nested layout.
            ILayoutNode BuildSubtree(int startIdx)
            {
                if (startIdx >= _groups.Count)
                    throw new InvalidOperationException("Not enough groups to build layout tree.");

                if (startIdx == _groups.Count - 1)
                {
                    var leaf = new GroupLayoutNode(_groups[startIdx].GroupId);
                    return leaf;
                }

                // If exactly 2 groups remain, create a simple split
                if (startIdx == _groups.Count - 2)
                {
                    var left  = new GroupLayoutNode(_groups[startIdx].GroupId);
                    var right = new GroupLayoutNode(_groups[startIdx + 1].GroupId);
                    return new SplitLayoutNode(left, right,
                        _splitHorizontal ? Orientation.Horizontal : Orientation.Vertical,
                        _splitRatio);
                }

                // Otherwise: Split(current, BuildSubtree(startIdx + 1))
                var current = new GroupLayoutNode(_groups[startIdx].GroupId);
                var rest    = BuildSubtree(startIdx + 1);
                return new SplitLayoutNode(current, rest,
                    _splitHorizontal ? Orientation.Horizontal : Orientation.Vertical,
                    _splitRatio);
            }

            _layoutRoot = BuildSubtree(0);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Tree layout application
        // ─────────────────────────────────────────────────────────────────────

        private void ApplyTreeLayout(SplitLayoutNode rootSplit)
        {
            EnsureSplitterBar();

            var (fb, sb, rb) = rootSplit.ComputeChildBounds(DocTokens.SplitterBarThickness);

            // Root split uses the primary interactive splitter bar
            _splitterBar!.IsHorizontal = rootSplit.Orientation == Orientation.Horizontal;
            _splitterBar.SetBounds(sb.X, sb.Y, sb.Width, sb.Height);
            _splitterBar.Visible = true;
            _splitterBar.BringToFront();
            _splitterBar.Tag = rootSplit;

            rootSplit.First.Bounds  = fb;
            rootSplit.Second.Bounds = rb;

            int extraIdx = 0;
            ApplyNodeBounds(rootSplit.First, ref extraIdx);
            ApplyNodeBounds(rootSplit.Second, ref extraIdx);

            // Hide extra splitters that are no longer needed
            for (int i = extraIdx; i < _extraSplitters.Count; i++)
                _extraSplitters[i].Visible = false;
        }

        /// <summary>
        /// Recursively applies bounds from the layout tree to groups and splitter bars.
        /// </summary>
        private void ApplyNodeBounds(ILayoutNode node, ref int extraIdx)
        {
            if (node is SplitLayoutNode split)
            {
                var (fb, sb, rb) = split.ComputeChildBounds(DocTokens.SplitterBarThickness);

                BeepDocumentSplitterBar bar;
                if (extraIdx < _extraSplitters.Count)
                {
                    bar = _extraSplitters[extraIdx];
                }
                else
                {
                    bar = new BeepDocumentSplitterBar();
                    bar.ApplyTheme(_currentTheme);
                    bar.MouseDown  += ExtraSplitter_MouseDown;
                    bar.MouseMove  += ExtraSplitter_MouseMove;
                    bar.MouseUp    += ExtraSplitter_MouseUp;
                    Controls.Add(bar);
                    _extraSplitters.Add(bar);
                }
                extraIdx++;

                bar.IsHorizontal = split.Orientation == Orientation.Horizontal;
                bar.SetBounds(sb.X, sb.Y, sb.Width, sb.Height);
                bar.Visible = true;
                bar.BringToFront();
                bar.Tag = split;

                split.First.Bounds  = fb;
                split.Second.Bounds = rb;

                ApplyNodeBounds(split.First, ref extraIdx);
                ApplyNodeBounds(split.Second, ref extraIdx);
            }
            else if (node is GroupLayoutNode grpNode)
            {
                var grp = _groupById.TryGetValue(grpNode.GroupId, out var g) ? g : null;
                if (grp != null)
                {
                    grp.GroupBounds = node.Bounds;
                    grp.ApplyBounds(AhS(LogicalStripH));
                }
            }
        }

        // ── Extra splitter mouse handlers (for nested splits) ────────────────

        private void ExtraSplitter_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || sender is not BeepDocumentSplitterBar bar) return;
            _draggingSplitter = true;
            _splitterDragStart = bar.IsHorizontal ? bar.Left + e.X : bar.Top + e.Y;
            _draggedSplitNode = bar.Tag as SplitLayoutNode;
            if (_draggedSplitNode != null)
                _splitRatioAtDragStart = _draggedSplitNode.Ratio;
        }

        private void ExtraSplitter_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_draggingSplitter || _draggedSplitNode == null) return;
            if (sender is not BeepDocumentSplitterBar bar) return;

            int pos = bar.IsHorizontal ? bar.Left + e.X : bar.Top + e.Y;
            int total = bar.IsHorizontal ? ClientSize.Width : ClientSize.Height;
            int thickness = DocTokens.SplitterBarThickness;
            if (total <= thickness * 2) return;

            _draggedSplitNode.Ratio = Math.Max(0.1f,
                                      Math.Min(0.9f, (float)pos / total));
            RecalculateLayout();
        }

        private void ExtraSplitter_MouseUp(object? sender, MouseEventArgs e)
        {
            _draggingSplitter = false;
            _draggedSplitNode = null;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Single-group layout (leaf node)
        // ─────────────────────────────────────────────────────────────────────

        private void ApplySingleGroupLayout()
        {
            if (_splitterBar != null) _splitterBar.Visible = false;
            foreach (var b in _extraSplitters) b.Visible = false;

            int stripH = AhS(LogicalStripH);
            int w = ClientSize.Width;
            int h = ClientSize.Height;

            _primaryGroup.GroupBounds = new Rectangle(0, 0, w, h);
            _primaryGroup.TabPosition = _tabPosition;
            _primaryGroup.ApplyBounds(stripH);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        private const int LogicalStripH = 36;

        /// <summary>Resizes every visible panel to fill its group's content area.</summary>
        private void SyncPanelBounds()
        {
            foreach (var grp in _groups)
            {
                if (!grp.ContentArea.IsHandleCreated) continue;
                foreach (var id in grp.DocumentIds)
                    if (_panels.TryGetValue(id, out var p) && p.Visible)
                        p.Bounds = grp.ContentArea.ClientRectangle;
            }
        }

        /// <summary>Returns the group that owns <paramref name="documentId"/>, or the primary group.</summary>
        private BeepDocumentGroup GetGroupForDocument(string documentId)
        {
            if (_docGroupMap.TryGetValue(documentId, out var groupId))
                if (_groupById.TryGetValue(groupId, out var g)) return g;
            return _primaryGroup;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Group-collapse animation (Sprint 15.2)
        // ─────────────────────────────────────────────────────────────────────

        internal void CollapseEmptyGroupAnimated(BeepDocumentGroup grp)
        {
            if (grp.IsPrimary || !grp.IsEmpty) return;

            if (_groups.Count != 2 || _splitterBar == null || !_splitterBar.Visible)
            {
                CollapseEmptyGroupImmediate(grp);
                return;
            }

            grp.TabStrip.Enabled = false;

            _collapseAnimGroup   = grp;
            _collapseAnimFrom    = _splitRatio;
            _collapseAnimStartMs = Stopwatch.GetTimestamp();

            if (_collapseAnimTimer == null)
            {
                _collapseAnimTimer          = new System.Windows.Forms.Timer { Interval = 16 };
                _collapseAnimTimer.Tick    += OnCollapseAnimTimerTick;
            }
            _collapseAnimTimer.Start();
        }

        private void OnCollapseAnimTimerTick(object? sender, EventArgs e)
        {
            if (_collapseAnimGroup == null) { _collapseAnimTimer?.Stop(); return; }

            double elapsedMs = (Stopwatch.GetTimestamp() - _collapseAnimStartMs)
                               * 1000.0 / Stopwatch.Frequency;

            double progress  = Math.Min(1.0, elapsedMs / Math.Max(1, DocTokens.GroupCollapseMs));
            double t         = 1.0 - Math.Pow(1.0 - progress, 3.0);

            _splitRatio = (float)(_collapseAnimFrom + (1.0 - _collapseAnimFrom) * t);
            RecalculateLayout();

            if (progress >= 1.0)
            {
                _collapseAnimTimer!.Stop();
                var grp = _collapseAnimGroup;
                _collapseAnimGroup = null;
                CollapseEmptyGroupImmediate(grp);
                _splitRatio = 0.5f;
            }
        }

        internal void CollapseEmptyGroupImmediate(BeepDocumentGroup grp)
            => CollapseEmptyGroupImmediate(grp, useCoordinator: true);

        internal void CollapseEmptyGroupImmediate(BeepDocumentGroup grp, bool useCoordinator)
        {
            if (grp.IsPrimary || !grp.IsEmpty) return;
            Action mutation = () =>
            {
                _groups.Remove(grp);
                _groupById.Remove(grp.GroupId);
                Controls.Remove(grp.TabStrip);
                Controls.Remove(grp.ContentArea);
                grp.Dispose();
            };

            if (useCoordinator)
            {
                var coordinator = new DocumentHostTreeMutationCoordinator(this);
                coordinator.Execute(DocumentHostOperationNames.CollapseEmptyGroup, mutation);
            }
            else
            {
                mutation();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // MergeAllGroups
        // ─────────────────────────────────────────────────────────────────────

        public void MergeAllGroups()
        {
            if (_groups.Count <= 1) return;

            var coordinator = new DocumentHostTreeMutationCoordinator(this);
            coordinator.Execute(DocumentHostOperationNames.MergeAllGroups, () =>
            {
                var secondary = new List<BeepDocumentGroup>(_groups.Skip(1));

                foreach (var grp in secondary)
                {
                    var docIds = new List<string>(grp.DocumentIds);
                    foreach (var docId in docIds)
                        MoveDocumentToGroupCore(docId, _primaryGroup.GroupId, recalcLayout: false);
                }

                for (int i = _groups.Count - 1; i >= 1; i--)
                {
                    var grp = _groups[i];
                    if (!grp.IsEmpty) continue;
                    CollapseEmptyGroupImmediate(grp, useCoordinator: false);
                }

                if (_splitterBar != null) _splitterBar.Visible = false;
                foreach (var b in _extraSplitters) b.Visible = false;

                // Reset to single-group tree
                _layoutRoot = new GroupLayoutNode(_primaryGroup.GroupId);
            });
        }
    }
}
