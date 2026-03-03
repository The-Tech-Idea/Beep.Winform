// BeepDocumentTabStrip.Layout.cs
// DPI helpers, logical constants, computed dimension properties, tab management API,
// layout calculation, scroll helpers, and indicator animation setup.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentTabStrip
    {
        // ─────────────────────────────────────────────────────────────────────
        // DPI helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// DeviceDpi is 0 before the Win32 HWND is created (constructor, design-time).
        /// When that happens we fall back to the primary screen DPI so that S() never
        /// returns 0 or NaN, preventing the VS designer NullReferenceException.
        /// </summary>
        private float DpiScale
        {
            get
            {
                int dpi = IsHandleCreated ? DeviceDpi : 0;
                if (dpi <= 0)
                {
                    // Try to get real DPI from the screen without a window handle
                    try { dpi = (int)(Screen.PrimaryScreen.Bounds.Width > 0
                              ? 96 * Screen.PrimaryScreen.WorkingArea.Width / Screen.PrimaryScreen.WorkingArea.Width
                              : 96); }
                    catch { dpi = 96; }
                    // Use Graphics to read the real DPI if we can
                    try { using var g = Graphics.FromHwnd(IntPtr.Zero); dpi = (int)g.DpiX; } catch { }
                    if (dpi <= 0) dpi = 96;
                }
                return dpi / 96f;
            }
        }
        private int S(int logical) => (int)Math.Round(logical * DpiScale);

        // ─────────────────────────────────────────────────────────────────────
        // Logical (96-dpi) dimension constants
        // ─────────────────────────────────────────────────────────────────────

        private const int LogicalTabHeight   = 36;
        private const int LogicalTabPadH     = 12;
        private const int LogicalTabMinW     = 88;
        private const int LogicalTabMaxW     = 220;
        private const int LogicalPinnedW     = 40;   // icon-only pinned tab width
        private const int LogicalCloseSize   = 20;   // larger hit area; glyph drawn centred inside
        private const int LogicalIconSize    = 16;
        private const int LogicalAddW        = 32;
        private const int LogicalScrollW     = 24;
        private const int LogicalOverflowW   = 28;   // ▾ overflow drop-down chevron button
        private const int LogicalRadius      = 6;
        private const int LogicalIndicatorH  = 3;
        private const int LogicalAccentBarH  = 2;    // kept for reference; bar drawn as 2 px pen
        private const int LogicalPillPadV    = 4;
        private const int LogicalPillRadius  = 8;
        private const int LogicalDragFloatY  = 40;  // vertical drift (logical px) to trigger float

        // ─────────────────────────────────────────────────────────────────────
        // DPI-scaled dimension properties
        // ─────────────────────────────────────────────────────────────────────

        private int TabHeight        => S(LogicalDensityHeight);  // Sprint 18.3: density-aware
        private int TabPadding       => S(LogicalTabPadH);
        private int TabMinWidth      => S(LogicalTabMinW);
        private int TabMaxWidth      => S(LogicalTabMaxW);
        private int PinnedTabWidth   => S(LogicalPinnedW);
        private int CloseButtonSize  => S(LogicalCloseSize);
        private int IconSize         => S(LogicalIconSize);
        private int AddButtonWidth    => S(LogicalAddW);
        private int ScrollButtonWidth  => S(LogicalScrollW);
        private int OverflowButtonWidth=> S(LogicalOverflowW);
        private int TabRadius        => S(LogicalRadius);
        private int IndicatorHeight  => S(LogicalIndicatorH);
        private int AccentBarHeight  => S(LogicalAccentBarH);
        private int PillPadV          => S(LogicalPillPadV);
        private int PillRadius         => S(LogicalPillRadius);
        private int DragFloatThresholdY=> S(LogicalDragFloatY);

        // Sprint 18.3 — logical height based on density mode
        private int LogicalDensityHeight => _tabDensity switch
        {
            TabDensityMode.Compact => 28,
            TabDensityMode.Dense   => 22,
            _                      => LogicalTabHeight   // Comfortable = 36
        };

        // ─────────────────────────────────────────────────────────────────────
        // Public tab management API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Adds a new tab and optionally activates it.</summary>
        public BeepDocumentTab AddTab(string id, string title,
                                      string? iconPath = null, bool activate = true)
        {
            var tab = new BeepDocumentTab(id, title) { IconPath = iconPath };
            _tabs.Add(tab);
            StartOpenAnimation(tab.Id);   // no-op when AnimationDuration == 0

            if (_paintBatch)
            {
                // In batch mode, defer all expensive work to EndBatchAdd
                if (activate || _tabs.Count == 1) _pendingActivateIndex = _tabs.Count - 1;
                return tab;
            }

            CalculateTabLayout();
            if (activate || _tabs.Count == 1)
                ActiveTabIndex = _tabs.Count - 1;
            Invalidate();
            return tab;
        }

        // ─────────────────────────────────────────────────────────────────
        // Batch-add API (performance — 100+ documents)
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Suspends layout recalculation and repaints during a bulk <see cref="AddTab"/> loop.
        /// Call <see cref="EndBatchAdd"/> to flush: one CalculateTabLayout + one Invalidate.
        /// </summary>
        public void BeginBatchAdd()
        {
            _paintBatch           = true;
            _pendingActivateIndex = -1;
        }

        /// <summary>
        /// Ends a batch add started by <see cref="BeginBatchAdd"/>.
        /// Calls <see cref="CalculateTabLayout"/> and <see cref="Control.Invalidate"/> once.
        /// </summary>
        public void EndBatchAdd()
        {
            _paintBatch = false;
            if (_pendingActivateIndex >= 0 && _pendingActivateIndex < _tabs.Count)
                ActiveTabIndex = _pendingActivateIndex;
            _pendingActivateIndex = -1;
            CalculateTabLayout();
            Invalidate();
        }

        /// <summary>Removes the tab at the given zero-based index.</summary>
        public void RemoveTabAt(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;
            _tabs.RemoveAt(index);
            if (_activeTabIndex >= _tabs.Count) _activeTabIndex = _tabs.Count - 1;
            CalculateTabLayout();
            Invalidate();
        }

        /// <summary>Activates the tab with the specified document id.  Returns false when not found.</summary>
        public bool ActivateTabById(string id)
        {
            int idx = _tabs.FindIndex(t => t.Id == id);
            if (idx < 0) return false;
            ActiveTabIndex = idx;
            return true;
        }

        /// <summary>Returns the data model for the tab with the given id, or null.</summary>
        public BeepDocumentTab? FindTabById(string id) => _tabs.Find(t => t.Id == id);

        /// <summary>Toggles the modified/dirty dot on the tab identified by <paramref name="id"/>.</summary>
        public void SetTabModified(string id, bool isModified)
        {
            var tab = FindTabById(id);
            if (tab == null) return;
            tab.IsModified = isModified;
            Invalidate();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Layout calculation
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Recalculates every tab's bounding rectangles and scroll state.</summary>
        public void CalculateTabLayout()
        {
            if (Width <= 0) return;

            // Sprint 18.2 — update responsive mode based on current strip width
            _responsiveMode = Width > _responsiveBreakpoints.Normal  ? TabResponsiveMode.Normal
                            : Width > _responsiveBreakpoints.Compact ? TabResponsiveMode.Compact
                            : Width > _responsiveBreakpoints.IconOnly ? TabResponsiveMode.IconOnly
                            : TabResponsiveMode.ActiveOnly;

            // Route to specialised layout if needed
            if (IsVertical)      { CalculateVerticalLayout();  return; }
            if (_tabRowMode == TabRowMode.MultiRow) { CalculateMultiRowLayout(); return; }

            int addArea = _showAddButton ? AddButtonWidth : 0;

            // ── Pinned tabs (always visible, no scroll, icon-only) ──────────────
            int pinnedCount   = 0;
            foreach (var t in _tabs) if (t.IsPinned) pinnedCount++;
            int pinnedArea    = pinnedCount * PinnedTabWidth;
            int unpinnedCount = _tabs.Count - pinnedCount;

            // ── Pass 1: detect scroll overflow ──────────────────────────────────
            int available1 = Math.Max(0, Width - pinnedArea - addArea);
            int totalUnpinW;
            if (_tabSizeMode == TabSizeMode.FitToContent)
            {
                totalUnpinW = 0;
                foreach (var t in _tabs) if (!t.IsPinned) totalUnpinW += MeasureTabWidth(t);
            }
            else
            {
                int rawWidth1 = unpinnedCount > 0 ? available1 / unpinnedCount : TabMaxWidth;
                int tabWidth1 = Math.Max(TabMinWidth, Math.Min(TabMaxWidth, rawWidth1));
                totalUnpinW = unpinnedCount * tabWidth1;
            }
            _showScrollButtons = unpinnedCount > 0 && totalUnpinW > available1;

            // ── Pass 2: final geometry with scroll state confirmed ──────────
            int scrollArea   = _showScrollButtons ? ScrollButtonWidth * 2 : 0;
            int overflowArea = _showScrollButtons ? OverflowButtonWidth  : 0;
            int available    = Math.Max(0, Width - pinnedArea - scrollArea - overflowArea - addArea);

            // Equal-mode base width reused by Equal + Compact
            int equalBaseW = _showScrollButtons
                ? TabMinWidth
                : (unpinnedCount > 0
                      ? Math.Max(TabMinWidth, Math.Min(TabMaxWidth, available / unpinnedCount))
                      : TabMaxWidth);

            // Compute total animated width for scroll clamping
            _totalTabsWidth = 0;
            foreach (var t in _tabs)
            {
                if (t.IsPinned) continue;
                int bw = ComputeBaseTabWidth(t, equalBaseW, available, unpinnedCount);
                _totalTabsWidth += GetAnimatedWidth(t.Id, bw);
            }

            int maxScroll = Math.Max(0, _totalTabsWidth - available);
            _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, maxScroll));

            int h = Height;

            // ── Lay out pinned tabs (left-anchored, icon-only, no scroll) ─────
            int px = 0;
            for (int i = 0; i < _tabs.Count; i++)
            {
                var tab = _tabs[i];
                if (!tab.IsPinned) continue;

                tab.TabRect   = new Rectangle(px, 0, PinnedTabWidth, h);
                tab.CloseRect = Rectangle.Empty;   // pinned tabs never show close
                tab.TitleRect = Rectangle.Empty;   // no text
                tab.DirtyRect = Rectangle.Empty;

                // Icon centred
                int iconX     = tab.TabRect.Left + (PinnedTabWidth - IconSize) / 2;
                int iconY     = tab.TabRect.Top  + (h - IconSize) / 2;
                tab.IconRect  = new Rectangle(iconX, iconY, IconSize, IconSize);

                px += PinnedTabWidth;
            }

            // ── Lay out unpinned tabs (scrollable) ─────────────────────────
            int ux = pinnedArea + scrollArea - _scrollOffset;
            for (int i = 0; i < _tabs.Count; i++)
            {
                var tab = _tabs[i];
                if (tab.IsPinned) continue;

                // Per-tab width: mode + animation
                int bw   = ComputeBaseTabWidth(tab, equalBaseW, available, unpinnedCount);
                int tabW = GetAnimatedWidth(tab.Id, bw);
                var rect = new Rectangle(ux, 0, tabW, h);
                tab.TabRect = rect;

                // ── Close button — space always reserved; rendering filters by CloseMode ──
                int closeHit = CloseButtonSize;
                int cx = rect.Right - TabPadding - closeHit;
                int cy = rect.Top   + (h - closeHit) / 2;
                tab.CloseRect = (tab.CanClose && _closeMode != TabCloseMode.Never)
                    ? new Rectangle(cx, cy, closeHit, closeHit)
                    : Rectangle.Empty;

                // ── Icon ─────────────────────────────────────────────────
                int iconX = rect.Left + TabPadding;
                int iconY = rect.Top  + (h - IconSize) / 2;
                tab.IconRect = string.IsNullOrEmpty(tab.IconPath)
                    ? Rectangle.Empty
                    : new Rectangle(iconX, iconY, IconSize, IconSize);

                // ── Title ───────────────────────────────────────────────────
                int titleLeft  = string.IsNullOrEmpty(tab.IconPath)
                    ? iconX
                    : iconX + IconSize + 4;
                int titleRight = tab.CloseRect.IsEmpty
                    ? rect.Right - TabPadding
                    : tab.CloseRect.Left - 4;
                tab.TitleRect = new Rectangle(titleLeft, rect.Top,
                                              Math.Max(0, titleRight - titleLeft), h);

                // ── Dirty dot ─────────────────────────────────────────────────
                int dotSize = S(6);
                int dotX = tab.CloseRect.IsEmpty
                    ? rect.Right - TabPadding - dotSize
                    : tab.CloseRect.Left - dotSize - S(3);
                int dotY = rect.Top + (h - dotSize) / 2;
                tab.DirtyRect = tab.IsModified
                    ? new Rectangle(dotX, dotY, dotSize, dotSize)
                    : Rectangle.Empty;

                ux += tabW;
            }

            // ── Button rects ─────────────────────────────────────────────────
            _addButtonRect = _showAddButton
                ? new Rectangle(Width - addArea, 0, addArea, Height)
                : Rectangle.Empty;

            // Overflow ▾ button — sits to the left of the add button, right-anchored
            _overflowButtonRect = _showScrollButtons
                ? new Rectangle(Width - addArea - overflowArea, 0, overflowArea, Height)
                : Rectangle.Empty;

            // Scroll buttons appear immediately after the pinned area
            if (_showScrollButtons)
            {
                _scrollLeftRect  = new Rectangle(pinnedArea,                   0, ScrollButtonWidth, Height);
                _scrollRightRect = new Rectangle(pinnedArea + ScrollButtonWidth, 0, ScrollButtonWidth, Height);
            }
            else
            {
                _scrollLeftRect = _scrollRightRect = Rectangle.Empty;
            }

            // Visible clip rect for unpinned scroll region
            _tabArea = new Rectangle(pinnedArea + scrollArea, 0, available, Height);

            // RTL mirroring (4.6) — flip all rects when RightToLeft is enabled
            if (RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                MirrorForRtl();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CalculateTabLayout();
        }

        // ─────────────────────────────────────────────────────────────────────
        // RTL mirroring (4.6)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Mirrors all tab and button rectangles horizontally for RTL layouts.</summary>
        private void MirrorForRtl()
        {
            // Local helper: mirror a rectangle about the control's horizontal centre
            Rectangle M(Rectangle r) =>
                r.IsEmpty ? r : new Rectangle(Width - r.Right, r.Y, r.Width, r.Height);

            // Swap scroll left / right (they now sit on opposite visual sides)
            var origLeft  = _scrollLeftRect;
            var origRight = _scrollRightRect;
            _scrollLeftRect  = M(origRight);
            _scrollRightRect = M(origLeft);

            _addButtonRect      = M(_addButtonRect);
            _overflowButtonRect = M(_overflowButtonRect);
            _tabArea            = M(_tabArea);

            foreach (var tab in _tabs)
            {
                tab.TabRect   = M(tab.TabRect);
                tab.CloseRect = M(tab.CloseRect);
                tab.IconRect  = M(tab.IconRect);
                tab.TitleRect = M(tab.TitleRect);
                tab.DirtyRect = M(tab.DirtyRect);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Multi-row tab layout (7.5)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Arranges tabs into multiple rows so they never scroll.
        /// The row containing the active tab is always promoted to the bottom
        /// row (adjacent to content).
        /// </summary>
        private void CalculateMultiRowLayout()
        {
            _showScrollButtons      = false;
            _scrollLeftRect         = Rectangle.Empty;
            _scrollRightRect        = Rectangle.Empty;
            _overflowButtonRect     = Rectangle.Empty;
            _addButtonRect          = Rectangle.Empty;

            if (_tabs.Count == 0)
            {
                RowCount = 1;
                _tabArea = ClientRectangle;
                return;
            }

            int rowW = Math.Max(1, Width);
            int tabH = TabHeight;

            // ── Measure widths ───────────────────────────────────────────────
            int equalW = 0;
            if (_tabSizeMode == TabSizeMode.Equal || _tabSizeMode == TabSizeMode.Compact)
            {
                // Fit as many equal-width tabs per row as possible
                equalW = Math.Max(TabMinWidth, Math.Min(TabMaxWidth, rowW / Math.Max(1, _tabs.Count)));
            }

            // ── Bin tabs into rows ───────────────────────────────────────────
            int row = 0;
            int rowX = 0;
            foreach (var tab in _tabs)
            {
                int tw = (_tabSizeMode == TabSizeMode.FitToContent || _tabSizeMode == TabSizeMode.Fixed)
                    ? ComputeBaseTabWidth(tab, equalW, rowW, _tabs.Count)
                    : equalW;
                tw = Math.Max(TabMinWidth, Math.Min(TabMaxWidth, tw));

                if (rowX + tw > rowW && rowX > 0)
                {
                    row++;
                    rowX = 0;
                }
                tab.RowIndex = row;
                rowX += tw;
            }
            int totalRows = row + 1;
            RowCount = totalRows;

            // ── Promote active tab's row to be the last row ──────────────────
            int activeRow = (_activeTabIndex >= 0 && _activeTabIndex < _tabs.Count)
                ? _tabs[_activeTabIndex].RowIndex : 0;

            // Re-assign RowIndex so activeRow becomes totalRows-1, others shift up
            foreach (var tab in _tabs)
            {
                if (tab.RowIndex == activeRow)
                    tab.RowIndex = totalRows - 1;
                else if (tab.RowIndex > activeRow)
                    tab.RowIndex = tab.RowIndex - 1;
            }

            // ── Assign TabRects ────────────────────────────────────────────
            // Collect tabs per row and lay them out left-to-right
            var rowCursors = new int[totalRows];   // current X per row
            var rowHits    = new int[totalRows];   // number of tabs per row
            foreach (var tab in _tabs) rowHits[tab.RowIndex]++;

            // Equal mode: stretch tabs to fill each row
            var rowTabW = new int[totalRows];
            for (int r = 0; r < totalRows; r++)
                rowTabW[r] = rowHits[r] > 0 ? Math.Max(TabMinWidth, rowW / rowHits[r]) : TabMinWidth;

            // Strip height driven by row count (host will resize us via BeepDocumentHost)
            // We request the height by setting our preferred size — caller must honour.
            int totalH = totalRows * tabH;

            foreach (var tab in _tabs)
            {
                int r   = tab.RowIndex;
                int tw  = (_tabSizeMode == TabSizeMode.FitToContent || _tabSizeMode == TabSizeMode.Fixed)
                    ? ComputeBaseTabWidth(tab, equalW, rowW, rowHits[r])
                    : rowTabW[r];
                int tx  = rowCursors[r];
                int ty  = r * tabH;

                tab.TabRect  = new Rectangle(tx, ty, tw, tabH);
                rowCursors[r] += tw;

                // Close button
                int closeHit = CloseButtonSize;
                int cx = tab.TabRect.Right - TabPadding - closeHit;
                int cy = tab.TabRect.Top   + (tabH - closeHit) / 2;
                tab.CloseRect = (tab.CanClose && _closeMode != TabCloseMode.Never)
                    ? new Rectangle(cx, cy, closeHit, closeHit)
                    : Rectangle.Empty;

                // Icon
                int iconX = tab.TabRect.Left + TabPadding;
                int iconY = tab.TabRect.Top  + (tabH - IconSize) / 2;
                tab.IconRect = string.IsNullOrEmpty(tab.IconPath)
                    ? Rectangle.Empty
                    : new Rectangle(iconX, iconY, IconSize, IconSize);

                // Title
                int titleLeft  = string.IsNullOrEmpty(tab.IconPath) ? iconX : iconX + IconSize + 4;
                int titleRight = tab.CloseRect.IsEmpty ? tab.TabRect.Right - TabPadding : tab.CloseRect.Left - 4;
                tab.TitleRect  = new Rectangle(titleLeft, ty, Math.Max(0, titleRight - titleLeft), tabH);

                // Dirty dot
                int dotSize = S(6);
                int dotX = tab.CloseRect.IsEmpty
                    ? tab.TabRect.Right - TabPadding - dotSize
                    : tab.CloseRect.Left - dotSize - S(3);
                tab.DirtyRect = tab.IsModified
                    ? new Rectangle(dotX, ty + (tabH - dotSize) / 2, dotSize, dotSize)
                    : Rectangle.Empty;
            }

            _tabArea = new Rectangle(0, 0, Width, totalH);

            // Resize the control to fit all rows (host will adapt its content panel)
            if (Height != totalH)
            {
                Height = totalH;
                Parent?.PerformLayout();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Vertical tab layout (7.6)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Arranges tabs vertically (top → bottom) for left/right strip positions.
        /// Tabs are full-width (= control Width) and <see cref="LogicalTabHeight"/> tall.
        /// Vertical scroll is supported via <see cref="_scrollOffset"/>.
        /// </summary>
        private void CalculateVerticalLayout()
        {
            _showScrollButtons   = false;   // replaced by up/down buttons below
            _addButtonRect       = Rectangle.Empty;
            _overflowButtonRect  = Rectangle.Empty;

            if (_tabs.Count == 0)
            {
                _scrollLeftRect = _scrollRightRect = Rectangle.Empty;
                _tabArea = ClientRectangle;
                return;
            }

            int tabH   = TabHeight;
            int w      = Width;
            int totalH = _tabs.Count * tabH;

            // Up/Down scroll buttons (reuse _scrollLeftRect / _scrollRightRect)
            bool needScroll = totalH > Height;
            if (needScroll)
            {
                int btnH = ScrollButtonWidth;   // square buttons
                _scrollLeftRect  = new Rectangle(0, 0, w, btnH);          // "up" button
                _scrollRightRect = new Rectangle(0, Height - btnH, w, btnH); // "down" button

                int maxScroll = Math.Max(0, totalH - (Height - btnH * 2));
                _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, maxScroll));
            }
            else
            {
                _scrollLeftRect  = Rectangle.Empty;
                _scrollRightRect = Rectangle.Empty;
                _scrollOffset    = 0;
            }

            int topBand    = needScroll ? ScrollButtonWidth : 0;
            int vy         = topBand - _scrollOffset;

            foreach (var tab in _tabs)
            {
                var rect      = new Rectangle(0, vy, w, tabH);
                tab.TabRect   = rect;
                tab.RowIndex  = 0;

                // Close button (right side, vertically centered)
                int closeHit = CloseButtonSize;
                int cx       = rect.Right - TabPadding - closeHit;
                int cy       = rect.Top   + (tabH - closeHit) / 2;
                tab.CloseRect = (tab.CanClose && _closeMode != TabCloseMode.Never)
                    ? new Rectangle(cx, cy, closeHit, closeHit)
                    : Rectangle.Empty;

                // Icon
                int iconX    = rect.Left + TabPadding;
                int iconY    = rect.Top  + (tabH - IconSize) / 2;
                tab.IconRect = string.IsNullOrEmpty(tab.IconPath)
                    ? Rectangle.Empty
                    : new Rectangle(iconX, iconY, IconSize, IconSize);

                // Title
                int titleLeft  = string.IsNullOrEmpty(tab.IconPath) ? iconX : iconX + IconSize + 4;
                int titleRight = tab.CloseRect.IsEmpty
                    ? rect.Right - TabPadding
                    : tab.CloseRect.Left - 4;
                tab.TitleRect  = new Rectangle(titleLeft, rect.Top,
                                               Math.Max(0, titleRight - titleLeft), tabH);

                // Dirty dot
                int dotSize = S(6);
                int dotX    = tab.CloseRect.IsEmpty
                    ? rect.Right - TabPadding - dotSize
                    : tab.CloseRect.Left - dotSize - S(3);
                tab.DirtyRect = tab.IsModified
                    ? new Rectangle(dotX, rect.Top + (tabH - dotSize) / 2, dotSize, dotSize)
                    : Rectangle.Empty;

                vy += tabH;
            }

            int visibleTop    = topBand;
            int visibleBottom = needScroll ? Height - ScrollButtonWidth : Height;
            _tabArea = new Rectangle(0, visibleTop, w, Math.Max(0, visibleBottom - visibleTop));
        }

        // ─────────────────────────────────────────────────────────────────────
        // Tab-width helpers (mode-aware)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the base (non-animated) width for a single unpinned tab
        /// based on the current <see cref="TabSizeMode"/>.
        /// </summary>
        private int ComputeBaseTabWidth(BeepDocumentTab tab, int equalBaseW,
                                        int available, int unpinnedCount)
        {
            return _tabSizeMode switch
            {
                TabSizeMode.FitToContent => MeasureTabWidth(tab),
                TabSizeMode.Fixed        => Math.Max(TabMinWidth, S(_fixedTabWidth)),
                TabSizeMode.Compact      => Math.Max(TabMinWidth, Math.Min(S(120),
                                              unpinnedCount > 0 ? available / unpinnedCount : TabMaxWidth)),
                _ /* Equal */            => equalBaseW,
            };
        }

        /// <summary>
        /// Measures the natural content width for a single tab
        /// (icon + text + close button + padding) clamped to [TabMinWidth, TabMaxWidth].
        /// </summary>
        private int MeasureTabWidth(BeepDocumentTab tab)
        {
            int w = TabPadding * 2;
            if (!string.IsNullOrEmpty(tab.IconPath)) w += IconSize + 4;
            if (!string.IsNullOrEmpty(tab.Title))
            {
                var sz = System.Windows.Forms.TextRenderer.MeasureText(tab.Title, Font);
                w += sz.Width;
            }
            if (tab.CanClose && _closeMode != TabCloseMode.Never)
                w += CloseButtonSize + 4;
            return Math.Max(TabMinWidth, Math.Min(TabMaxWidth, w));
        }
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Scrolls the strip so the tab at <paramref name="index"/> is fully visible.</summary>
        public void ScrollTabIntoView(int index)
        {
            if (index < 0 || index >= _tabs.Count) return;

            // Pinned tabs are always visible — no scrolling needed
            if (_tabs[index].IsPinned) return;

            int pinnedArea = 0;
            foreach (var t in _tabs) if (t.IsPinned) pinnedArea += PinnedTabWidth;

            int scrollArea = _showScrollButtons ? ScrollButtonWidth * 2 : 0;
            int addArea    = _showAddButton     ? AddButtonWidth        : 0;
            int viewLeft   = pinnedArea + scrollArea;
            int viewRight  = Width - addArea;

            var r = _tabs[index].TabRect;
            if (r.Left < viewLeft)
                _scrollOffset = Math.Max(0, _scrollOffset - (viewLeft - r.Left));
            else if (r.Right > viewRight)
                _scrollOffset += r.Right - viewRight;

            CalculateTabLayout();
            Invalidate();
        }

        /// <summary>Scrolls the strip by <paramref name="pixels"/> (positive = right, negative = left).</summary>
        public void ScrollBy(int pixels)
        {
            int addArea   = _showAddButton      ? AddButtonWidth        : 0;
            int scrollArea = _showScrollButtons ? ScrollButtonWidth * 2 : 0;
            int visible   = Width - scrollArea - addArea;
            int maxScroll = Math.Max(0, _totalTabsWidth - visible);
            _scrollOffset = Math.Max(0, Math.Min(_scrollOffset + pixels, maxScroll));
            CalculateTabLayout();
            Invalidate();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Indicator animation setup
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Slides the active indicator from the tab at <paramref name="fromIndex"/> to
        /// the tab at <paramref name="toIndex"/>.</summary>
        public void StartIndicatorAnimation(int fromIndex, int toIndex)
        {
            if (toIndex < 0 || toIndex >= _tabs.Count) return;

            var target = _tabs[toIndex].TabRect;
            _indicatorTarget = new Rectangle(
                target.Left,
                target.Bottom - IndicatorHeight,
                target.Width,
                IndicatorHeight);

            if (fromIndex < 0 || fromIndex >= _tabs.Count || _indicatorCurrent.IsEmpty)
            {
                // Snap directly — no animation
                _indicatorCurrent = _indicatorTarget;
                _animating = false;
                _animTimer.Stop();
            }
            else
            {
                _animating = true;
                if (!_animTimer.Enabled) _animTimer.Start();
            }
        }
    }
}
