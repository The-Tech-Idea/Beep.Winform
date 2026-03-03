// BeepDocumentHost.Layout.cs
// Layout calculation for BeepDocumentHost — positions the tab strip and content area,
// and keeps all visible document panels sized to fill the content area.
// Supports single-group (original) and multi-group split-view (feature 2.1).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ─────────────────────────────────────────────────────────────────────
        // Override hooks
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!IsHandleCreated) return;  // safe — no layout before handle
            RecalculateLayout();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (!IsHandleCreated) return;  // safe — no layout before handle
            RecalculateLayout();
            SyncPanelBounds();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Splitter mouse handling
        // ─────────────────────────────────────────────────────────────────────

        private const int SplitterThickness = 5;
        private bool _draggingSplitter;
        private int  _splitterDragStart;
        private float _splitRatioAtDragStart;

        private void EnsureSplitterBar()
        {
            if (_splitterBar != null) return;
            _splitterBar = new Panel
            {
                BackColor = _theme?.BorderColor ?? SystemColors.ControlDark
            };
            _splitterBar.Cursor      = _splitHorizontal ? Cursors.VSplit : Cursors.HSplit;
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
        }

        private void Splitter_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_draggingSplitter) return;
            int pos = _splitHorizontal
                ? (_splitterBar!.Left + e.X)
                : (_splitterBar!.Top  + e.Y);
            int total = _splitHorizontal ? ClientSize.Width : ClientSize.Height;
            if (total <= SplitterThickness * 2) return;
            _splitRatio = Math.Max(0.1f,
                          Math.Min(0.9f, (float)pos / total));
            RecalculateLayout();
        }

        private void Splitter_MouseUp(object? sender, MouseEventArgs e)
            => _draggingSplitter = false;

        // ─────────────────────────────────────────────────────────────────────
        // Layout engine
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Positions the tab strip(s) and content area(s) according to <see cref="TabPosition"/>
        /// and the active group split.
        /// </summary>
        public void RecalculateLayout()
        {
            // Performance guard: skip while a batch add is in progress
            if (_layoutSuspended) return;
            if (_tabStrip == null || _contentArea == null) return;

            SuspendLayout();
            try
            {
                if (_groups.Count <= 1)
                    RecalculateSingleGroupLayout();
                else
                    RecalculateMultiGroupLayout();

                // Position auto-hide strips along content area edges (3.3)
                PositionAutoHideStrips();
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Single-group layout (original behaviour)
        // ─────────────────────────────────────────────────────────────────────

        private void RecalculateSingleGroupLayout()
        {
            // Hide the splitter bar if it exists
            if (_splitterBar != null) _splitterBar.Visible = false;

            int stripH = AhS(LogicalStripH);
            int w = ClientSize.Width;
            int h = ClientSize.Height;

            switch (_tabPosition)
            {
                case TabStripPosition.Top:
                    _tabStrip.IsVertical = false;
                    _tabStrip.SetBounds(0, 0, w, stripH);
                    _contentArea.SetBounds(0, stripH, w, Math.Max(0, h - stripH));
                    _tabStrip.Visible = true;
                    break;

                case TabStripPosition.Bottom:
                    _tabStrip.IsVertical = false;
                    _contentArea.SetBounds(0, 0, w, Math.Max(0, h - stripH));
                    _tabStrip.SetBounds(0, h - stripH, w, stripH);
                    _tabStrip.Visible = true;
                    break;

                case TabStripPosition.Left:
                    _tabStrip.IsVertical = true;
                    _tabStrip.SetBounds(0, 0, stripH, h);
                    _contentArea.SetBounds(stripH, 0, Math.Max(0, w - stripH), h);
                    _tabStrip.Visible = true;
                    break;

                case TabStripPosition.Right:
                    _tabStrip.IsVertical = true;
                    _contentArea.SetBounds(0, 0, Math.Max(0, w - stripH), h);
                    _tabStrip.SetBounds(w - stripH, 0, stripH, h);
                    _tabStrip.Visible = true;
                    break;

                case TabStripPosition.Hidden:
                    _tabStrip.IsVertical = false;
                    _tabStrip.Visible = false;
                    _contentArea.SetBounds(0, 0, w, h);
                    break;
            }

            _primaryGroup.GroupBounds = new Rectangle(0, 0, w, h);
            _tabStrip.CalculateTabLayout();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Multi-group layout (feature 2.1)
        // ─────────────────────────────────────────────────────────────────────

        private const int LogicalStripH   = 36;
        private const int LogicalSplitterH = SplitterThickness;

        private void RecalculateMultiGroupLayout()
        {
            EnsureSplitterBar();

            int w      = ClientSize.Width;
            int h      = ClientSize.Height;
            int splT   = SplitterThickness;

            // We support exactly 2 groups in this release; clip to that.
            var g0 = _groups[0];
            var g1 = _groups[1];

            Rectangle b0, b1, splRect;

            if (_splitHorizontal)
            {
                // Side by side
                int g0W = (int)Math.Round((w - splT) * _splitRatio);
                int g1W = Math.Max(0, w - splT - g0W);
                b0      = new Rectangle(0,         0, g0W, h);
                splRect = new Rectangle(g0W,       0, splT, h);
                b1      = new Rectangle(g0W + splT, 0, g1W, h);
                _splitterBar!.Cursor = Cursors.VSplit;
            }
            else
            {
                // Stacked vertically
                int g0H = (int)Math.Round((h - splT) * _splitRatio);
                int g1H = Math.Max(0, h - splT - g0H);
                b0      = new Rectangle(0, 0,         w, g0H);
                splRect = new Rectangle(0, g0H,       w, splT);
                b1      = new Rectangle(0, g0H + splT, w, g1H);
                _splitterBar!.Cursor = Cursors.HSplit;
            }

            int stripH = AhS(LogicalStripH);

            g0.GroupBounds = b0;
            g0.ApplyBounds(_tabPosition, stripH);

            g1.GroupBounds = b1;
            g1.ApplyBounds(_tabPosition, stripH);

            _splitterBar.SetBounds(splRect.X, splRect.Y, splRect.Width, splRect.Height);
            _splitterBar.Visible = true;
            _splitterBar.BringToFront();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Resizes every visible panel to fill its group's content area.</summary>
        private void SyncPanelBounds()
        {
            // Primary group
            if (_contentArea != null)
                foreach (var panel in _panels.Values)
                    if (panel.Visible && GetGroupForDocument(panel.DocumentId) == _primaryGroup)
                        panel.Bounds = _contentArea.ClientRectangle;

            // Secondary groups
            for (int i = 1; i < _groups.Count; i++)
            {
                var grp = _groups[i];
                foreach (var id in grp.DocumentIds)
                    if (_panels.TryGetValue(id, out var p) && p.Visible)
                        p.Bounds = grp.ContentArea.ClientRectangle;
            }
        }

        /// <summary>Returns the group that owns <paramref name="documentId"/>, or the primary group.</summary>
        private BeepDocumentGroup GetGroupForDocument(string documentId)
        {
            if (_docGroupMap.TryGetValue(documentId, out var groupId))
                foreach (var g in _groups)
                    if (g.GroupId == groupId) return g;
            return _primaryGroup;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Group-collapse animation (Sprint 15.2)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Animates the secondary group's splitter to the edge over
        /// <see cref="DocTokens.GroupCollapseMs"/> ms (ease-out cubic), then removes it.
        /// If on a multi-group split, disables the collapsing strip's input while animating.
        /// Falls back to immediate removal when not in a 2-group split mode.
        /// </summary>
        internal void CollapseEmptyGroupAnimated(BeepDocumentGroup grp)
        {
            if (grp.IsPrimary || !grp.IsEmpty) return;

            // Only animate when there is exactly one secondary group with a visible splitter
            if (_groups.Count != 2 || _splitterBar == null || !_splitterBar.Visible)
            {
                CollapseEmptyGroupImmediate(grp);
                return;
            }

            // Disable interaction on the collapsing group during animation
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
            // Ease-out cubic:  t = 1 − (1−p)^3
            double t         = 1.0 - Math.Pow(1.0 - progress, 3.0);

            // Animate toward 1.0 (primary group fills all space, secondary shrinks to 0)
            _splitRatio = (float)(_collapseAnimFrom + (1.0 - _collapseAnimFrom) * t);
            RecalculateLayout();

            if (progress >= 1.0)
            {
                _collapseAnimTimer!.Stop();
                var grp = _collapseAnimGroup;
                _collapseAnimGroup = null;
                CollapseEmptyGroupImmediate(grp);
                // Reset ratio for next split
                _splitRatio = 0.5f;
            }
        }

        /// <summary>Internal synchronous collapse used after the animation completes.</summary>
        internal void CollapseEmptyGroupImmediate(BeepDocumentGroup grp)
        {
            if (grp.IsPrimary || !grp.IsEmpty) return;
            _groups.Remove(grp);
            Controls.Remove(grp.TabStrip);
            Controls.Remove(grp.ContentArea);
            grp.Dispose();
            RecalculateLayout();
        }

        // ─────────────────────────────────────────────────────────────────────
        // MergeAllGroups
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Collapses every secondary group back into the primary group by moving
        /// all their documents, then removes the now-empty extra groups.
        /// The splitter bar is hidden and the layout is recalculated.
        /// </summary>
        public void MergeAllGroups()
        {
            if (_groups.Count <= 1) return;

            // Snapshot secondary groups — the list mutates as empty groups auto-collapse
            var secondary = new List<BeepDocumentGroup>(_groups.Skip(1));

            foreach (var grp in secondary)
            {
                // Copy DocumentIds before the list is mutated by MoveDocumentToGroup
                var docIds = new List<string>(grp.DocumentIds);
                foreach (var docId in docIds)
                    MoveDocumentToGroup(docId, _primaryGroup.GroupId);
            }

            // Safety pass: dispose any remaining empty secondary groups not yet cleaned up
            for (int i = _groups.Count - 1; i >= 1; i--)
            {
                var grp = _groups[i];
                if (!grp.IsEmpty) continue;
                _groups.RemoveAt(i);
                Controls.Remove(grp.TabStrip);
                Controls.Remove(grp.ContentArea);
                grp.Dispose();
            }

            if (_splitterBar != null) _splitterBar.Visible = false;
            RecalculateLayout();
        }
    }
}
