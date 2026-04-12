// BeepDocumentTabStrip.Mouse.cs
// All mouse-event handlers for BeepDocumentTabStrip:
// hover tracking, tooltip, click dispatch, middle-click-close, drag-to-reorder, wheel scroll.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // ─────────────────────────────────────────────────────────────────────
        // Rich-tooltip state (created lazily)
        // ─────────────────────────────────────────────────────────────────────

        private BeepDocumentRichTooltip? _richTooltipPopup;
        private System.Windows.Forms.Timer? _tooltipHoverTimer;

        /// <summary>Ensure the hover timer exists and attach its Tick handler once.</summary>
        private System.Windows.Forms.Timer GetOrCreateTooltipTimer()
        {
            if (_tooltipHoverTimer == null)
            {
                _tooltipHoverTimer          = new System.Windows.Forms.Timer();
                _tooltipHoverTimer.Tick    += OnTooltipHoverTick;
            }
            return _tooltipHoverTimer;
        }

        private void OnTooltipHoverTick(object? sender, EventArgs e)
        {
            _tooltipHoverTimer?.Stop();
            if (_hoverTabIndex < 0 || _hoverTabIndex >= _tabs.Count) return;

            var tab = _tabs[_hoverTabIndex];
            var screenPt = PointToScreen(new Point(
                tab.TabRect.Left + tab.TabRect.Width / 2,
                tab.TabRect.Bottom));

            Bitmap? thumb = null;
            if (ThumbnailProvider != null)
                thumb = ThumbnailProvider(tab.Id);

            (_richTooltipPopup ??= new BeepDocumentRichTooltip())
                .ShowForTab(tab, thumb, screenPt, _currentTheme);
        }

        private void HideRichTooltip()
        {
            _tooltipHoverTimer?.Stop();
            _richTooltipPopup?.HidePopup();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Hover + tooltip
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int prevHover        = _hoverTabIndex;
            bool prevClose       = _hoverClose;
            bool prevAdd         = _hoverAdd;
            bool prevScrollLeft  = _hoverScrollLeft;
            bool prevScrollRight = _hoverScrollRight;
            bool prevOverflow    = _hoverOverflow;

            _hoverTabIndex    = -1;
            _hoverClose       = false;
            _hoverAdd         = !_addButtonRect.IsEmpty      && _addButtonRect.Contains(e.Location);
            _hoverScrollLeft  = !_scrollLeftRect.IsEmpty      && _scrollLeftRect.Contains(e.Location);
            _hoverScrollRight = !_scrollRightRect.IsEmpty     && _scrollRightRect.Contains(e.Location);
            _hoverOverflow    = !_overflowButtonRect.IsEmpty  && _overflowButtonRect.Contains(e.Location);

            for (int i = 0; i < _tabs.Count; i++)
            {
                if (_tabs[i].TabRect.Contains(e.Location))
                {
                    _hoverTabIndex = i;
                    _hoverClose    = !_tabs[i].CloseRect.IsEmpty
                                     && _tabs[i].CloseRect.Contains(e.Location);
                    break;
                }
            }

            // Tooltip
            if (_hoverTabIndex != _lastTooltipIndex)
            {
                _lastTooltipIndex = _hoverTabIndex;

                switch (_tooltipMode)
                {
                    case TabTooltipMode.None:
                        _tooltip.SetToolTip(this, null);
                        HideRichTooltip();
                        break;

                    case TabTooltipMode.Simple:
                        HideRichTooltip();
                        _tooltip.SetToolTip(this,
                            _hoverTabIndex >= 0 ? _tabs[_hoverTabIndex].TooltipText : null);
                        break;

                    case TabTooltipMode.Rich:
                        _tooltip.SetToolTip(this, null);
                        HideRichTooltip();
                        if (_hoverTabIndex >= 0)
                        {
                            var timer = GetOrCreateTooltipTimer();
                            timer.Interval = Math.Max(1, _tooltipHoverDelay);
                            timer.Start();
                        }
                        break;
                }
            }

            bool changed = _hoverTabIndex    != prevHover
                        || _hoverClose       != prevClose
                        || _hoverAdd         != prevAdd
                        || _hoverScrollLeft  != prevScrollLeft
                        || _hoverScrollRight != prevScrollRight
                        || _hoverOverflow    != prevOverflow;

            if (changed) Invalidate();

            // Drag-reorder
            if (_dragStartTab >= 0 && e.Button == MouseButtons.Left)
                HandleDragMove(e.Location);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverTabIndex = _lastTooltipIndex = -1;
            _hoverClose = _hoverAdd = _hoverScrollLeft = _hoverScrollRight = _hoverOverflow = false;
            HideRichTooltip();
            Invalidate();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Mouse down
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // ── Scroll buttons ──────────────────────────────────────────────
            if (e.Button == MouseButtons.Left)
            {
                if (!_scrollLeftRect.IsEmpty  && _scrollLeftRect.Contains(e.Location))
                { ScrollBy(-TabMinWidth); return; }
                if (!_scrollRightRect.IsEmpty && _scrollRightRect.Contains(e.Location))
                { ScrollBy(+TabMinWidth); return; }                if (!_overflowButtonRect.IsEmpty && _overflowButtonRect.Contains(e.Location))
                { ShowOverflowMenu(); return; }            }

            // ── Add button ──────────────────────────────────────────────────
            if (e.Button == MouseButtons.Left
                && !_addButtonRect.IsEmpty && _addButtonRect.Contains(e.Location))
            {
                AddButtonClicked?.Invoke(this, EventArgs.Empty);
                return;
            }

            // ── Group header hit-test — left-click toggles collapse (7.3) ──
            if (e.Button == MouseButtons.Left && _tabGroups.Count > 0)
            {
                foreach (var grp in _tabGroups)
                {
                    if (!grp.HeaderRect.IsEmpty && grp.HeaderRect.Contains(e.Location))
                    {
                        grp.IsCollapsed = !grp.IsCollapsed;
                        CalculateTabLayout();
                        Invalidate();
                        return;
                    }
                }
            }

            // ── Tab hit-test ────────────────────────────────────────────────
            for (int i = 0; i < _tabs.Count; i++)
            {
                var tab = _tabs[i];
                if (!tab.TabRect.Contains(e.Location)) continue;

                // Middle-click → close
                if (e.Button == MouseButtons.Middle && tab.CanClose)
                {
                    RequestClose(i, tab);
                    return;
                }

                if (e.Button != MouseButtons.Left) return;

                // Left on close glyph → close
                if (tab.CanClose && !tab.CloseRect.IsEmpty && tab.CloseRect.Contains(e.Location))
                {
                    RequestClose(i, tab);
                    return;
                }

                // Left on tab body → activate
                int prev = _activeTabIndex;
                ActiveTabIndex = i;
                if (i != prev)
                    TabSelected?.Invoke(this, new TabEventArgs(i, tab));

                // Arm drag
                _dragStartTab   = i;
                _dragStartPoint = e.Location;
                return;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Mouse up
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            // ── Finish drag-to-float ─────────────────────────────────────────
            if (_dragFloating)
            {
                _dragFloatGhost?.Close();
                _dragFloatGhost?.Dispose();
                _dragFloatGhost = null;

                if (_dragStartTab >= 0 && _dragStartTab < _tabs.Count)
                    TabFloatRequested?.Invoke(this,
                        new TabEventArgs(_dragStartTab, _tabs[_dragStartTab]));

                _dragFloating    = false;
                _dragging        = false;
                _dragStartTab    = -1;
                _dragInsertIndex = -1;
                Cursor           = Cursors.Default;
                return;
            }

            // ── Commit insert-mode drag reorder ──────────────────────────────
            if (_dragging && _dragInsertIndex >= 0)
                CommitDragReorder();

            _dragging     = false;
            _dragStartTab = -1;
            Cursor = Cursors.Default;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Double-click — empty strip area = open new document
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button != MouseButtons.Left) return;

            // Only fire if the double-click is on empty strip space (not on a tab)
            bool onTab = false;
            foreach (var tab in _tabs)
                if (tab.TabRect.Contains(e.Location)) { onTab = true; break; }

            if (!onTab
                && (_addButtonRect.IsEmpty || !_addButtonRect.Contains(e.Location))
                && (_scrollLeftRect.IsEmpty  || !_scrollLeftRect.Contains(e.Location))
                && (_scrollRightRect.IsEmpty || !_scrollRightRect.Contains(e.Location)))
            {
                AddButtonClicked?.Invoke(this, EventArgs.Empty);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Mouse wheel — scroll tabs
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_tabs.Count == 0) return;
            // Allow wheel scrolling even when the scroll arrows are not visible yet;
            // CalculateTabLayout will clamp _scrollOffset if everything already fits.
            ScrollBy(e.Delta > 0 ? -TabMinWidth / 2 : +TabMinWidth / 2);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Drag-to-reorder
        // ─────────────────────────────────────────────────────────────────────

        private void HandleDragMove(Point current)
        {
            // Start drag only after threshold
            if (!_dragging && !_dragFloating)
            {
                bool movedH = Math.Abs(current.X - _dragStartPoint.X) >= 6;
                bool movedV = Math.Abs(current.Y - _dragStartPoint.Y) >= 6;
                if (!movedH && !movedV) return;
                _dragging = movedH;   // only arm horizontal reorder if horizontal motion
            }

            // ── Float detection: vertical drift beyond threshold ───────────────
            if (_allowDragFloat
                && _dragStartTab >= 0 && _dragStartTab < _tabs.Count
                && !_tabs[_dragStartTab].IsPinned
                && Math.Abs(current.Y - _dragStartPoint.Y) > DragFloatThresholdY)
            {
                if (!_dragFloating)
                {
                    _dragFloating    = true;
                    _dragging        = false;
                    _dragInsertIndex = -1;
                    ShowDragFloatGhost(_tabs[_dragStartTab].Title, PointToScreen(current));
                }
                Cursor = Cursors.SizeAll;
                MoveDragFloatGhost(PointToScreen(current));
                return;
            }

            if (_dragFloating)
            {
                MoveDragFloatGhost(PointToScreen(current));
                return;
            }

            if (!_dragging) return;
            Cursor = Cursors.SizeWE;

            // ── Insert-mode: compute target insert slot from cursor position ───
            // The insert index is the position (0..Count) where the dragged tab
            // would land if the mouse button were released now.  We only skip
            // moving from the pinned section into the unpinned section (and vice-versa).
            _dragCurrentCursorX = current.X;
            bool draggingPinned = _dragStartTab >= 0 && _dragStartTab < _tabs.Count
                                  && _tabs[_dragStartTab].IsPinned;

            int insert = _tabs.Count;   // default: append at end
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (i == _dragStartTab) continue;
                var r = _tabs[i].TabRect;
                if (r.IsEmpty) continue;
                // Skip crossing pinned/unpinned boundary
                if (draggingPinned != _tabs[i].IsPinned) continue;

                if (current.X < r.Left + r.Width / 2)
                {
                    insert = i;
                    break;
                }
                insert = i + 1;
            }

            // Clamp: pinned tabs cannot move past the pinned block
            if (draggingPinned)
            {
                int lastPinned = -1;
                for (int i = 0; i < _tabs.Count; i++)
                    if (_tabs[i].IsPinned) lastPinned = i;
                insert = Math.Min(insert, lastPinned + 1);
            }
            else
            {
                int firstUnpinned = _tabs.Count;
                for (int i = 0; i < _tabs.Count; i++)
                    if (!_tabs[i].IsPinned) { firstUnpinned = i; break; }
                insert = Math.Max(insert, firstUnpinned);
            }

            if (_dragInsertIndex != insert)
            {
                _dragInsertIndex = insert;
                Invalidate();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // CommitDragReorder — called from OnMouseUp to apply the pending insert
        // ─────────────────────────────────────────────────────────────────────

        private void CommitDragReorder()
        {
            int from = _dragStartTab;
            int to   = _dragInsertIndex;

            _dragInsertIndex    = -1;
            _dragCurrentCursorX = 0;

            if (from < 0 || from >= _tabs.Count || to < 0) return;

            // Adjust 'to' for the removal shift
            int insertAt = to > from ? to - 1 : to;

            if (insertAt == from) return;   // no real movement

            var tab = _tabs[from];
            _tabs.RemoveAt(from);
            _tabs.Insert(Math.Min(insertAt, _tabs.Count), tab);

            if (_activeTabIndex == from) _activeTabIndex = insertAt;
            else if (_activeTabIndex > from && _activeTabIndex <= insertAt) _activeTabIndex--;
            else if (_activeTabIndex < from && _activeTabIndex >= insertAt) _activeTabIndex++;

            CalculateTabLayout();
            Invalidate();
            TabReordered?.Invoke(this, new TabReorderArgs(from, insertAt, tab));
        }

        // ─────────────────────────────────────────────────────────────────────
        // Drag-to-float ghost window helpers
        // ─────────────────────────────────────────────────────────────────────

        private void ShowDragFloatGhost(string title, Point screenPt)
        {
            if (_dragFloatGhost != null) return;

            _dragFloatGhost = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                BackColor       = System.Drawing.Color.FromArgb(48, 54, 70),
                Opacity         = 0.55,
                ShowInTaskbar   = false,
                Size            = new System.Drawing.Size(S(140), S(28)),
                StartPosition   = FormStartPosition.Manual,
                TopMost         = true
            };

            var lbl = new Label
            {
                Text      = title,
                ForeColor = System.Drawing.Color.White,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock      = DockStyle.Fill,
                Font      = new System.Drawing.Font(Font, Font.Style)
            };
            _dragFloatGhost.Controls.Add(lbl);
            _dragFloatGhost.Location = new System.Drawing.Point(screenPt.X - S(70), screenPt.Y - S(14));
            _dragFloatGhost.Show();
        }

        private void MoveDragFloatGhost(Point screenPt)
        {
            if (_dragFloatGhost == null) return;
            _dragFloatGhost.Location = new System.Drawing.Point(screenPt.X - S(70), screenPt.Y - S(14));
        }

        // ─────────────────────────────────────────────────────────────────────
        // RequestClose — fires TabClosing (cancellable) then TabCloseRequested
        // Used by context menu, keyboard shortcuts, and mouse close glyph.
        // ─────────────────────────────────────────────────────────────────────

        private void RequestClose(int index, BeepDocumentTab tab)
        {
            // Phase 1: let subscribers cancel
            var closing = new TabClosingEventArgs(index, tab);
            TabClosing?.Invoke(this, closing);
            if (closing.Cancel) return;

            // Phase 2: commit
            TabCloseRequested?.Invoke(this, new TabEventArgs(index, tab));
        }
    }
}
