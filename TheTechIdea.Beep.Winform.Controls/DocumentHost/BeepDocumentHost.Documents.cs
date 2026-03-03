using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    public partial class BeepDocumentHost
    {
        // ─────────────────────────────────────────────────────────────────
        // Batch-Add API (performance — 100+ documents)
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Suspends layout recalculation and tab-strip repaints during a bulk
        /// <see cref="AddDocument"/> loop.  Call <see cref="EndBatchAddDocuments"/>
        /// when done to flush with a single layout pass.
        /// </summary>
        public void BeginBatchAddDocuments()
        {
            _batchAdding    = true;
            _layoutSuspended = true;
            foreach (var grp in _groups)
                grp.TabStrip.BeginBatchAdd();
        }

        /// <summary>
        /// Ends a batch started by <see cref="BeginBatchAddDocuments"/>.
        /// Calls one <see cref="RecalculateLayout"/> and one tab-strip flush.
        /// </summary>
        public void EndBatchAddDocuments()
        {
            _batchAdding     = false;
            _layoutSuspended = false;
            foreach (var grp in _groups)
                grp.TabStrip.EndBatchAdd();
            RecalculateLayout();
        }
        // ─────────────────────────────────────────────────────────────────────
        // Add
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Adds a new empty <see cref="BeepDocumentPanel"/> with an auto-generated id.
        /// Returns it so the caller can populate the content.
        /// </summary>
        public BeepDocumentPanel AddDocument(string title,
                                             string? iconPath = null,
                                             bool activate = true)
            => AddDocument(Guid.NewGuid().ToString(), title, iconPath, activate);

        /// <summary>Adds a new empty panel with an explicit document id.</summary>
        public BeepDocumentPanel AddDocument(string documentId,
                                             string title,
                                             string? iconPath = null,
                                             bool activate = true)
        {
            if (_panels.ContainsKey(documentId))
                throw new InvalidOperationException($"Document '{documentId}' already exists.");

            var panel = new BeepDocumentPanel(documentId, title)
            {
                Bounds    = _contentArea.ClientRectangle,
                ThemeName = _themeName,
                IconPath  = iconPath,
                Visible   = false
            };

            _contentArea.Controls.Add(panel);
            _panels[documentId] = panel;

            // Register to primary group
            _docGroupMap[documentId] = _primaryGroup.GroupId;
            _primaryGroup.DocumentIds.Add(documentId);

            _tabStrip.AddTab(documentId, title, iconPath, activate: false);

            if (activate || _panels.Count == 1)
                SetActiveDocument(documentId);

            panel.ModifiedChanged += (s, _) =>
                _tabStrip.SetTabModified(documentId, panel.IsModified);

            // Skip the expensive layout pass when batching — EndBatchAddDocuments will flush
            if (!_batchAdding)
                RecalculateLayout();

            return panel;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Activate
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Activates the document with the given id.  Returns false if not found.</summary>
        public bool SetActiveDocument(string documentId)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return false;

            // Hide the previously active panel (and capture preview if enabled)
            if (_activeDocumentId != null
                && _panels.TryGetValue(_activeDocumentId, out var prev)
                && prev != panel)
            {
                // Lazy deferred snapshot — schedule via BeginInvoke so the tab
                // switch itself is not blocked by DrawToBitmap.
                if (_tabPreviewEnabled)
                {
                    var captureId = _activeDocumentId;  // capture local copy for lambda
                    if (IsHandleCreated)
                        BeginInvoke(new Action(() => CaptureSnapshot(captureId)));
                    else
                        CaptureSnapshot(captureId);
                }
                prev.Visible = false;
            }

            // Show the new panel — use the group's content area, not always _contentArea
            var activeGrp = GetGroupForDocument(documentId);
            panel.Bounds  = activeGrp.ContentArea.ClientRectangle;
            panel.Anchor  = AnchorStyles.Top | AnchorStyles.Bottom
                          | AnchorStyles.Left | AnchorStyles.Right;
            panel.Visible = true;
            panel.BringToFront();

            _activeDocumentId = documentId;
            _activeGroup      = activeGrp;
            PushMru(documentId);
            activeGrp.TabStrip.ActivateTabById(documentId);

            ActiveDocumentChanged?.Invoke(this,
                new DocumentEventArgs(documentId, panel.DocumentTitle));
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Close
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Closes and removes the document.  Returns false if not found.</summary>
        public bool CloseDocument(string documentId)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return false;

            string title = panel.DocumentTitle;

            // Push to closed-tab history BEFORE destroying the panel
            PushClosedTab(new ClosedTabRecord(documentId, title, panel.IconPath));
            RemoveMru(documentId);

            // Remove from whichever group owns this document
            if (_docGroupMap.TryGetValue(documentId, out var closingGroupId))
            {
                _docGroupMap.Remove(documentId);
                var ownerGrp = _groups.Find(g => g.GroupId == closingGroupId);
                if (ownerGrp != null)
                {
                    ownerGrp.DocumentIds.Remove(documentId);
                    int gTabIdx = ownerGrp.TabStrip.Tabs.ToList().FindIndex(t => t.Id == documentId);
                    if (gTabIdx >= 0) ownerGrp.TabStrip.RemoveTabAt(gTabIdx);

                    if (ownerGrp.IsEmpty && !ownerGrp.IsPrimary)
                        CollapseEmptyGroup(ownerGrp);
                }
            }
            else
            {
                // Fallback: remove from primary strip
                int tabIdx = _tabStrip.Tabs.ToList().FindIndex(t => t.Id == documentId);
                if (tabIdx >= 0) _tabStrip.RemoveTabAt(tabIdx);
            }

            _panels.Remove(documentId);
            panel.Parent?.Controls.Remove(panel);
            panel.Dispose();

            if (_activeDocumentId == documentId)
            {
                _activeDocumentId = null;
                if (_panels.Count > 0)
                    SetActiveDocument(_panels.Keys.Last());
            }

            DocumentClosed?.Invoke(this, new DocumentEventArgs(documentId, title));
            RemoveDescriptorForId(documentId);          // keep Documents list in sync
            return true;
        }

        /// <summary>
        /// Closes every open document.  Returns <see langword="true"/> when at least one
        /// document was closed.
        /// </summary>
        public bool CloseAllDocuments()
        {
            var ids = new System.Collections.Generic.List<string>(_panels.Keys);
            if (ids.Count == 0) return false;
            foreach (var id in ids)
                CloseDocument(id);
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Float / Dock
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Floats the document panel into its own <see cref="BeepDocumentFloatWindow"/>.</summary>
        public bool FloatDocument(string documentId)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return false;
            if (_floatWindows.ContainsKey(documentId)) return false;   // already floated

            string title = panel.DocumentTitle;

            _contentArea.Controls.Remove(panel);
            _panels.Remove(documentId);

            int tabIdx = _tabStrip.Tabs.ToList().FindIndex(t => t.Id == documentId);
            if (tabIdx >= 0) _tabStrip.RemoveTabAt(tabIdx);

            if (_activeDocumentId == documentId)
            {
                _activeDocumentId = null;
                if (_panels.Count > 0) SetActiveDocument(_panels.Keys.Last());
            }

            var fw = new BeepDocumentFloatWindow(panel, _themeName);
            fw.DockBack       += (s, ea) => DockBackDocument(documentId);
            fw.WindowMoved    += (s, screenPt) => OnFloatWindowMoved(documentId, screenPt);
            fw.TitleBarMouseUp += (s, ea) => OnFloatWindowDropped(documentId);
            fw.FormClosed  += (s, _) =>
            {
                _dockOverlay?.HideOverlay();
                _floatWindows.Remove(documentId);
                DocumentClosed?.Invoke(this, new DocumentEventArgs(documentId, title));
            };
            _floatWindows[documentId] = fw;
            fw.Show();

            DocumentFloated?.Invoke(this, new DocumentEventArgs(documentId, title));
            return true;
        }

        /// <summary>Docks a previously floated document back into this host.</summary>
        public bool DockBackDocument(string documentId)
        {
            if (!_floatWindows.TryGetValue(documentId, out var fw)) return false;

            var panel = fw.DetachPanel();
            _floatWindows.Remove(documentId);
            fw.Close();

            string title = panel.DocumentTitle;
            _panels[documentId] = panel;
            _contentArea.Controls.Add(panel);
            _tabStrip.AddTab(documentId, title, panel.IconPath, activate: false);

            SetActiveDocument(documentId);
            DocumentDocked?.Invoke(this, new DocumentEventArgs(documentId, title));
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Dock overlay helpers  (private)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Called by a float window's WindowMoved event.
        /// Shows (or updates) the dock overlay when the window is dragged over this host.
        /// </summary>
        private void OnFloatWindowMoved(string documentId, System.Drawing.Point floatWindowScreenPt)
        {
            // Convert our client area to screen coords
            Rectangle hostScreen = RectangleToScreen(ClientRectangle);

            if (hostScreen.Contains(floatWindowScreenPt))
            {
                if (_dockOverlay == null)
                    _dockOverlay = new BeepDocumentDockOverlay();

                if (!_dockOverlay.Visible)
                    _dockOverlay.ShowOverlay(hostScreen);
                else
                    _dockOverlay.Bounds = hostScreen; // keep in sync if host was resized

                // Use the float window's top-left corner for hit-testing
                _dockOverlay.UpdateHighlight(floatWindowScreenPt);
            }
            else
            {
                _dockOverlay?.HideOverlay();
            }
        }

        /// <summary>
        /// Called when the user releases the left mouse button on the float window's title bar.
        /// If a dock zone is highlighted, docks the document back.
        /// </summary>
        private void OnFloatWindowDropped(string documentId)
        {
            if (_dockOverlay == null || !_dockOverlay.Visible) return;

            DockZone zone = _dockOverlay.HitTest(System.Windows.Forms.Cursor.Position);
            _dockOverlay.HideOverlay();

            switch (zone)
            {
                case DockZone.Centre:
                    // Dock back as a tab in the primary group
                    DockBackDocument(documentId);
                    break;

                case DockZone.Right:
                    // Floating panel docks into a new RIGHT pane
                    DockBackDocument(documentId);
                    SplitDocumentHorizontal(documentId);   // driver goes to g1 (right)
                    break;

                case DockZone.Bottom:
                    // Floating panel docks into a new BOTTOM pane
                    DockBackDocument(documentId);
                    SplitDocumentVertical(documentId);     // driver goes to g1 (bottom)
                    break;

                case DockZone.Left:
                    // Floating panel docks into a new LEFT pane;
                    // push existing primary docs to the right via a horizontal split.
                    DockBackDocument(documentId);
                    var leftOther = _primaryGroup.DocumentIds
                        .Find(id => id != documentId);
                    if (leftOther != null)
                        SplitDocumentHorizontal(leftOther); // leftOther moves to g1 (right)
                    // documentId stays in primary (left)
                    break;

                case DockZone.Top:
                    // Floating panel docks into a new TOP pane;
                    // push existing primary docs downward via a vertical split.
                    DockBackDocument(documentId);
                    var topOther = _primaryGroup.DocumentIds
                        .Find(id => id != documentId);
                    if (topOther != null)
                        SplitDocumentVertical(topOther);    // topOther moves to g1 (bottom)
                    // documentId stays in primary (top)
                    break;

                // DockZone.None → user released outside any zone; do nothing
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Lookup
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Returns the panel for a given document id, or null.</summary>
        public BeepDocumentPanel? GetPanel(string documentId)
            => _panels.TryGetValue(documentId, out var p) ? p : null;

        // ─────────────────────────────────────────────────────────────────────
        // Reopen Closed Tab
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Reopens the most recently closed document.
        /// Raises <see cref="ReopenDocumentRequested"/> so the consumer can rebuild
        /// the panel and restore state from <see cref="ClosedTabRecord.RestoreData"/>.
        /// </summary>
        /// <returns>False if there is nothing to reopen.</returns>
        public bool ReopenLastClosed()
        {
            if (_closedStack.Count == 0) return false;
            var record = _closedStack.Pop();
            ReopenDocumentRequested?.Invoke(this, record);
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // MRU + Closed-stack helpers  (private)
        // ─────────────────────────────────────────────────────────────────────

        private void PushMru(string documentId)
        {
            _mruList.Remove(documentId);
            _mruList.AddFirst(documentId);
            while (_mruList.Count > _maxRecentHistory)
                _mruList.RemoveLast();
        }

        private void RemoveMru(string documentId) => _mruList.Remove(documentId);

        private void PushClosedTab(ClosedTabRecord record)
        {
            if (_maxClosedHistory <= 0) return;
            _closedStack.Push(record);

            // Trim to max: Stack is LIFO — rebuild from array to discard oldest entries
            if (_closedStack.Count > _maxClosedHistory)
            {
                var items = _closedStack.ToArray();
                _closedStack.Clear();
                for (int i = Math.Min(_maxClosedHistory, items.Length) - 1; i >= 0; i--)
                    _closedStack.Push(items[i]);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Quick-Switch Popup
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Opens the Quick-Switch document picker above the tab strip.
        /// The user can type a filter, navigate with arrow keys, and press Enter
        /// to switch to the selected document.
        /// </summary>
        public void ShowQuickSwitch()
        {
            var tabs = _tabStrip.Tabs.ToList();
            if (tabs.Count == 0) return;

            // Position the popup centred over the tab strip
            var stripScreenPt = _tabStrip.PointToScreen(Point.Empty);
            int popW = 420;
            int popH = 340;
            int x    = stripScreenPt.X + (_tabStrip.Width  - popW) / 2;
            int y    = stripScreenPt.Y;

            // Keep on screen
            var screen = Screen.FromControl(this).WorkingArea;
            x = Math.Max(screen.Left, Math.Min(x, screen.Right  - popW));
            y = Math.Max(screen.Top,  Math.Min(y, screen.Bottom - popH));

            var popup = new BeepDocumentQuickSwitch(
                tabs,
                _activeDocumentId,
                _theme,
                new System.Drawing.Point(x, y));

            popup.ShowDialog(this);

            if (!string.IsNullOrEmpty(popup.SelectedDocumentId))
                SetActiveDocument(popup.SelectedDocumentId);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Pin / Unpin
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Pins or unpins the document.  A pinned document collapses to an icon-only tab
        /// and cannot be closed by the user until unpinned.
        /// </summary>
        /// <param name="documentId">Id of the document to pin or unpin.</param>
        /// <param name="pin">True to pin, false to unpin.</param>
        /// <returns>False if the document was not found.</returns>
        public bool PinDocument(string documentId, bool pin = true)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return false;

            var tab = _tabStrip.FindTabById(documentId);
            if (tab == null) return false;

            if (tab.IsPinned == pin) return true;   // already in requested state

            // Find current index and tell the strip to toggle
            int index = _tabStrip.Tabs.ToList().IndexOf(tab);
            if (index >= 0)
                _tabStrip.TogglePin(index);

            // Sync CanClose on the panel
            panel.CanClose = !pin;
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Badge API (7.4)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Sets or clears the notification badge on the tab for the given document.
        /// </summary>
        /// <param name="documentId">Target document id.</param>
        /// <param name="badgeText">Short label (e.g. "3", "!"). Null/empty clears the badge.</param>
        /// <param name="badgeColor">Badge colour. <see cref="System.Drawing.Color.Empty"/> = use theme ErrorColor.</param>
        public void SetBadge(string documentId, string? badgeText,
                              System.Drawing.Color badgeColor = default)
            => _tabStrip.SetBadge(documentId, badgeText, badgeColor);

        /// <summary>Removes the notification badge from the given document's tab.</summary>
        public void ClearBadge(string documentId)
            => _tabStrip.ClearBadge(documentId);

        // ─────────────────────────────────────────────────────────────────────
        // Cross-host document transfer
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Transfers a document from <paramref name="sourceHost"/> into this host.
        /// Called by <see cref="BeepDocumentDragManager"/> routing in <c>Events.cs</c>.
        /// </summary>
        internal void AttachExternalDocument(BeepDocumentHost sourceHost, string documentId)
        {
            if (!sourceHost._panels.TryGetValue(documentId, out var panel)) return;

            string title    = panel.DocumentTitle;
            string? iconPath = panel.IconPath;

            // ── Remove from source host ──────────────────────────────────────
            int tabIdx = sourceHost._tabStrip.Tabs
                             .ToList()
                             .FindIndex(t => t.Id == documentId);
            if (tabIdx >= 0) sourceHost._tabStrip.RemoveTabAt(tabIdx);
            sourceHost._contentArea.Controls.Remove(panel);
            sourceHost._panels.Remove(documentId);
            if (sourceHost._activeDocumentId == documentId)
            {
                sourceHost._activeDocumentId = null;
                if (sourceHost._panels.Count > 0)
                    sourceHost.SetActiveDocument(sourceHost._panels.Keys.Last());
            }

            // ── Add to this host ─────────────────────────────────────────────
            panel.Parent = _contentArea;
            _panels[documentId] = panel;
            _docGroupMap[documentId] = _primaryGroup.GroupId;
            _primaryGroup.DocumentIds.Add(documentId);
            _tabStrip.AddTab(documentId, title, iconPath, activate: false);
            SetActiveDocument(documentId);

            DocumentAttached?.Invoke(this, new DocumentEventArgs(documentId, title));
        }

        // ─────────────────────────────────────────────────────────────────────
        // Document Groups — Split View (Feature 2.1)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Moves <paramref name="documentId"/> into a new side-by-side pane
        /// (horizontal split — one pane to the left, one to the right).
        /// </summary>
        public bool SplitDocumentHorizontal(string documentId)
            => SplitInternal(documentId, horizontal: true);

        /// <summary>
        /// Moves <paramref name="documentId"/> into a new stacked pane
        /// (vertical split — one pane above, one below).
        /// </summary>
        public bool SplitDocumentVertical(string documentId)
            => SplitInternal(documentId, horizontal: false);

        private bool SplitInternal(string documentId, bool horizontal)
        {
            if (!_panels.ContainsKey(documentId)) return false;
            if (_groups.Count >= _maxGroups) return false;

            // Create a fresh secondary group
            var grp = new BeepDocumentGroup(
                Guid.NewGuid().ToString(),
                _themeName,
                _showAddButton,
                _closeMode,
                _tabStyle,
                _theme);

            // Register the group's new controls with this host
            Controls.Add(grp.ContentArea);
            Controls.Add(grp.TabStrip);

            // Wire events from this group back to the host's handlers
            WireSecondaryGroup(grp);

            _groups.Add(grp);
            _splitHorizontal = horizontal;

            // Move the document into the new group
            MoveDocumentToGroup(documentId, grp.GroupId);

            RecalculateLayout();
            SetActiveDocument(documentId);
            return true;
        }

        /// <summary>
        /// Moves a document from its current group to the group identified by
        /// <paramref name="targetGroupId"/>.  Returns false if either id is unknown.
        /// </summary>
        public bool MoveDocumentToGroup(string documentId, string targetGroupId)
        {
            if (!_panels.TryGetValue(documentId, out var panel)) return false;
            var targetGroup = _groups.Find(g => g.GroupId == targetGroupId);
            if (targetGroup == null) return false;

            // Remove from current group
            if (_docGroupMap.TryGetValue(documentId, out var currentGroupId))
            {
                var currentGroup = _groups.Find(g => g.GroupId == currentGroupId);
                if (currentGroup != null && currentGroup != targetGroup)
                {
                    currentGroup.DocumentIds.Remove(documentId);
                    int idx = currentGroup.TabStrip.Tabs.ToList()
                                          .FindIndex(t => t.Id == documentId);
                    if (idx >= 0) currentGroup.TabStrip.RemoveTabAt(idx);

                    if (currentGroup.IsEmpty && !currentGroup.IsPrimary)
                        CollapseEmptyGroup(currentGroup);
                }
            }

            // Reparent the panel to the target group's content area
            panel.Parent?.Controls.Remove(panel);
            targetGroup.ContentArea.Controls.Add(panel);

            // Add to the target group's tab strip
            targetGroup.TabStrip.AddTab(
                documentId, panel.DocumentTitle, panel.IconPath, activate: false);
            targetGroup.DocumentIds.Add(documentId);
            _docGroupMap[documentId] = targetGroupId;

            panel.ModifiedChanged -= null; // avoid duplicate wiring
            panel.ModifiedChanged += (s, _) =>
                targetGroup.TabStrip.SetTabModified(documentId, panel.IsModified);

            RecalculateLayout();
            return true;
        }

        /// <summary>Wires a secondary group's forwarded events to this host's handlers.</summary>
        private void WireSecondaryGroup(BeepDocumentGroup grp)
        {
            grp.TabSelected       += (s, e) => OnTabSelected(grp.TabStrip, e);
            grp.TabCloseRequested += (s, e) => OnTabCloseRequested(grp.TabStrip, e);
            grp.TabClosing        += (s, e) => OnTabClosing(grp.TabStrip, e);
            grp.TabFloatRequested += (s, e) => OnTabFloatRequested(grp.TabStrip, e);
            grp.TabPinToggled     += (s, e) => OnTabPinToggled(grp.TabStrip, e);
            grp.TabReordered      += (s, e) => OnTabReordered(grp.TabStrip, e);
            grp.AddButtonClicked  += (s, e) => OnAddButtonClicked(grp.TabStrip, e);
        }

        /// <summary>
        /// Removes a now-empty secondary group, unregisters its controls from
        /// this host, disposes it, and triggers a layout recalculation.
        /// Uses animated collapse when a splitter is visible; immediate otherwise.
        /// </summary>
        private void CollapseEmptyGroup(BeepDocumentGroup grp)
        {
            CollapseEmptyGroupAnimated(grp);
        }
    }
}