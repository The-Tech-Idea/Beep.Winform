using System;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Mouse Handling

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Skip tab hover handling in Single mode
            if (_displayMode == ContainerDisplayMode.Single)
            {
                return;
            }

            // ---- Enhancement 5: drag-to-reorder tracking ----
            if (_dragTab != null && e.Button == MouseButtons.Left)
            {
                int dx = Math.Abs(e.X - _dragStartPoint.X);
                int dy = Math.Abs(e.Y - _dragStartPoint.Y);
                if (!_isDragging && (dx > _dragThreshold || dy > _dragThreshold))
                    _isDragging = true;

                if (_isDragging)
                {
                    _dragGhostLoc    = e.Location;
                    _dropInsertIndex = ComputeDropInsertIndex(e.Location);
                    Cursor = Cursors.SizeAll;
                    Invalidate(_tabArea);
                    return; // skip normal hover while dragging
                }
            }

            var hitTab = GetTabAt(e.Location);
            
            if (hitTab != _hoveredTab)
            {
                // Update hover state
                if (_hoveredTab != null)
                {
                    _hoveredTab.IsCloseHovered = false;
                    StartAnimation(_hoveredTab, 0f);
                }

                _hoveredTab = hitTab;
                
                if (_hoveredTab != null)
                {
                    StartAnimation(_hoveredTab, 1f);
                }

                Invalidate();
            }

            // Check close button hover
            if (_hoveredTab != null && _showCloseButtons && _hoveredTab.CanClose)
            {
                var closeRect = GetCloseButtonRect(_hoveredTab.Bounds);
                bool isCloseHovered = closeRect.Contains(e.Location);
                
                if (isCloseHovered != _hoveredTab.IsCloseHovered)
                {
                    _hoveredTab.IsCloseHovered = isCloseHovered;
                    Cursor = isCloseHovered ? Cursors.Hand : Cursors.Default;
                    Invalidate();
                }
            }
            else if (_hoveredTab == null)
            {
                Cursor = Cursors.Default;
            }

            // --- Scroll / utility button hover ---
            if (_needsScrolling)
            {
                int prev = _hoveredScrollButton;
                if (_scrollLeftButton.Contains(e.Location))
                    _hoveredScrollButton = 1;
                else if (_scrollRightButton.Contains(e.Location))
                    _hoveredScrollButton = 2;
                else if (_newTabButton.Contains(e.Location))
                    _hoveredScrollButton = 3;
                else
                    _hoveredScrollButton = 0;

                if (_hoveredScrollButton != prev)
                {
                    Cursor = _hoveredScrollButton != 0 ? Cursors.Hand : Cursors.Default;
                    Invalidate();
                }
            }
            else if (_hoveredScrollButton != 0)
            {
                _hoveredScrollButton = 0;
                Cursor = Cursors.Default;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredTab != null)
            {
                _hoveredTab.IsCloseHovered = false;
                StartAnimation(_hoveredTab, 0f);
                _hoveredTab = null;
            }

            bool changed = (_hoveredScrollButton != 0 || _pressedScrollButton != 0);
            _hoveredScrollButton = 0;
            _pressedScrollButton = 0;

            // Cancel any in-progress drag if mouse leaves the control
            if (_isDragging)
            {
                _dragTab         = null;
                _isDragging      = false;
                _dropInsertIndex = -1;
                _dragGhostLoc    = Point.Empty;
                changed = true;
            }

            Cursor = Cursors.Default;
            if (changed || _hoveredTab == null)
                Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Claim keyboard focus when clicked (needed for keyboard nav).
            if (!Focused) Focus();

            if (e.Button != MouseButtons.Left || _displayMode == ContainerDisplayMode.Single)
                return;

            // Track pressed state on scroll / utility buttons.
            if (_needsScrolling)
            {
                if (_scrollLeftButton.Contains(e.Location))
                    _pressedScrollButton = 1;
                else if (_scrollRightButton.Contains(e.Location))
                    _pressedScrollButton = 2;
                else if (_newTabButton.Contains(e.Location))
                    _pressedScrollButton = 3;
                else
                    _pressedScrollButton = 0;

                if (_pressedScrollButton != 0)
                    Invalidate();
            }

            // ---- Enhancement 5: begin potential drag ----
            if (_allowTabReordering && e.Button == MouseButtons.Left)
            {
                var hitTab = GetTabAt(e.Location);
                if (hitTab != null)
                {
                    _dragTab        = hitTab;
                    _dragStartPoint = e.Location;
                    _isDragging     = false;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_pressedScrollButton != 0)
            {
                _pressedScrollButton = 0;
                Invalidate();
            }

            // ---- Enhancement 5: commit drag-to-reorder ----
            if (_isDragging && _dragTab != null && _dropInsertIndex >= 0)
            {
                int fromIndex = _tabs.IndexOf(_dragTab);
                if (fromIndex >= 0 && _dropInsertIndex != fromIndex)
                {
                    _tabs.RemoveAt(fromIndex);
                    // Adjust target index after removal
                    int target = _dropInsertIndex > fromIndex
                        ? Math.Min(_dropInsertIndex - 1, _tabs.Count)
                        : Math.Min(_dropInsertIndex, _tabs.Count);
                    _tabs.Insert(target, _dragTab);
                    RecalculateLayout();
                    Invalidate();
                }
            }

            // Clear drag state and restore cursor
            _dragTab         = null;
            _isDragging      = false;
            _dropInsertIndex = -1;
            _dragGhostLoc    = Point.Empty;
            if (Cursor == Cursors.SizeAll)
                Cursor = Cursors.Default;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Skip tab click handling in Single mode
            if (_displayMode == ContainerDisplayMode.Single)
            {
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                var hitTab = GetTabAt(e.Location);
                if (hitTab != null)
                {
                    // Check if clicking close button
                    if (_showCloseButtons && hitTab.CanClose)
                    {
                        var closeRect = GetCloseButtonRect(hitTab.Bounds);
                        if (closeRect.Contains(e.Location))
                        {
                            RemoveTab(hitTab);
                            return;
                        }
                    }

                    // Activate tab
                    ActivateTab(hitTab);
                }
                else
                {
                    // Check scroll buttons
                    if (_needsScrolling)
                    {
                        if (_scrollLeftButton.Contains(e.Location))
                        {
                            ScrollTabs(-1);
                        }
                        else if (_scrollRightButton.Contains(e.Location))
                        {
                            ScrollTabs(1);
                        }
                        else if (_newTabButton.Contains(e.Location))
                        {
                            // Trigger new tab event
                            OnNewTabRequested();
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Handle right-click for context menu (follows BaseControl pattern)
                var hitTab = GetTabAt(e.Location);
                if (hitTab != null)
                {
                    ShowTabContextMenu(hitTab, e.Location);
                }
            }
        }
        
        /// <summary>
        /// Shows context menu for a tab (follows BaseControl pattern)
        /// </summary>
        private void ShowTabContextMenu(AddinTab tab, Point location)
        {
            if (tab == null) return;
            
            // Create context menu items with MethodName for action handling
            var menuItems = new System.Collections.Generic.List<SimpleItem>();

            // Close tab option
            if (tab.CanClose)
            {
                menuItems.Add(new SimpleItem
                {
                    Text = "Close Tab",
                    Description = "Close this tab",
                    ImagePath = "close",
                    MethodName = "CloseTab",
                    Tag = tab // Store the tab reference
                });
            }
            
            // Close other tabs option
            if (_tabs.Count > 1)
            {
                menuItems.Add(new SimpleItem
                {
                    Text = "Close Other Tabs",
                    Description = "Close all tabs except this one",
                    ImagePath = "close_multiple",
                    MethodName = "CloseOtherTabs",
                    Tag = tab
                });
            }
            
            // Close all tabs option
            if (_tabs.Count > 0)
            {
                menuItems.Add(new SimpleItem
                {
                    Text = "Close All Tabs",
                    Description = "Close all tabs",
                    ImagePath = "close_all",
                    MethodName = "CloseAllTabs",
                    Tag = tab
                });
            }
            
            // Show context menu using BaseControl's ShowContextMenu
            // This is a BLOCKING call - it returns when menu closes
            var screenLocation = this.PointToScreen(location);
            var selectedItem = base.ShowContextMenu(menuItems, screenLocation, false, FormStyle.Modern);
            
            // IMPORTANT: Handle action AFTER menu returns (menu is already disposed by ContextMenuManager)
            // We must extract all needed data from selectedItem before any async operations
            if (selectedItem != null && !string.IsNullOrEmpty(selectedItem.MethodName))
            {
                // Extract data synchronously before menu disposal
                string action = selectedItem.MethodName;
                AddinTab targetTab = selectedItem.Tag as AddinTab ?? tab;
                
                // Use BeginInvoke to handle action after menu is fully disposed
                this.BeginInvoke(new Action(() =>
                {
                    HandleTabContextMenuAction(action, targetTab);
                }));
            }
        }
        
        /// <summary>
        /// Handles context menu actions for tabs
        /// </summary>
        private void HandleTabContextMenuAction(string action, AddinTab tab)
        {
            if (tab == null) return;
            
            switch (action)
            {
                case "CloseTab":
                    RemoveTab(tab);
                    break;
                case "CloseOtherTabs":
                    CloseOtherTabs(tab);
                    break;
                case "CloseAllTabs":
                    CloseAllTabs();
                    break;
            }
        }
        
        /// <summary>
        /// Closes all tabs except the specified one
        /// </summary>
        private void CloseOtherTabs(AddinTab keepTab)
        {
            BeginUpdate();
            try
            {
                var tabsToRemove = _tabs.Where(t => t != keepTab && t.CanClose).ToList();
                foreach (var tab in tabsToRemove)
                {
                    RemoveTab(tab);
                }
            }
            finally
            {
                EndUpdate();
            }
        }
        
        /// <summary>
        /// Closes all tabs
        /// </summary>
        private void CloseAllTabs()
        {
            BeginUpdate();
            try
            {
                var tabsToRemove = _tabs.Where(t => t.CanClose).ToList();
                foreach (var tab in tabsToRemove)
                {
                    RemoveTab(tab);
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        private void ScrollTabs(int direction)
        {
            _scrollOffset += direction;
            _scrollOffset = Math.Max(0, Math.Min(_tabs.Count - 1, _scrollOffset));
            CalculateTabLayout();
            Invalidate();
        }

        /// <summary>
        /// Returns true if there are more tabs to the right/below the current
        /// scroll offset that are not yet fully visible.
        /// </summary>
        private bool CanScrollRight()
        {
            if (!_needsScrolling || _tabs == null) return false;
            // Any tab that is marked not-visible and lies beyond the scroll offset
            // means we can still scroll further.
            return _tabs.Skip(_scrollOffset).Any(t => !t.IsVisible);
        }

        private void OnNewTabRequested()
        {
            // Override in derived classes or handle via events
        }

        #endregion

        // =====================================================================
        #region Keyboard Navigation  (P4 Enhancement 4)
        // =====================================================================

        /// <summary>
        /// Let WinForms know this control wants to handle navigation keys itself
        /// rather than yielding to the parent form.
        /// </summary>
        protected override bool IsInputKey(Keys keyData)
        {
            if (_enableKeyboardNav && _displayMode != ContainerDisplayMode.Single)
            {
                switch (keyData & ~Keys.Modifiers)
                {
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Home:
                    case Keys.End:
                        return true;
                    case Keys.Tab when (keyData & Keys.Control) != 0:
                        return true;
                    case Keys.W when (keyData & Keys.Control) != 0:
                        return true;
                }
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!_enableKeyboardNav || _displayMode == ContainerDisplayMode.Single || _tabs.Count == 0)
                return;

            int current = _activeTab != null ? _tabs.IndexOf(_activeTab) : -1;

            // Determine direction of navigation from key combo.
            int delta = 0;
            bool absolute = false;
            int absoluteIndex = 0;

            bool vertical = (_tabPosition == TabPosition.Left || _tabPosition == TabPosition.Right);

            switch (e.KeyCode)
            {
                // Arrow keys that point "forward" in the current tab orientation.
                case Keys.Right when !vertical:
                case Keys.Down  when  vertical:
                    delta = 1; break;

                // Arrow keys that point "back" in the current tab orientation.
                case Keys.Left when !vertical:
                case Keys.Up   when  vertical:
                    delta = -1; break;

                case Keys.Home:
                    absolute = true; absoluteIndex = 0; break;

                case Keys.End:
                    absolute = true; absoluteIndex = _tabs.Count - 1; break;

                case Keys.Tab when e.Control:
                    delta = e.Shift ? -1 : 1; break;

                case Keys.W when e.Control:
                    // Close active tab.
                    if (_activeTab != null && _activeTab.CanClose)
                    {
                        RemoveTab(_activeTab);
                        e.Handled = true;
                    }
                    return;
            }

            if (delta != 0 || absolute)
            {
                int next;
                if (absolute)
                {
                    next = absoluteIndex;
                }
                else
                {
                    // Wrap around.
                    next = current < 0 ? 0 : (current + delta + _tabs.Count) % _tabs.Count;
                }

                next = Math.Max(0, Math.Min(_tabs.Count - 1, next));
                if (next != current)
                    ActivateTab(_tabs[next]);

                e.Handled = true;
            }
        }

        #endregion

        // =====================================================================
        #region Drag-to-Reorder Helpers  (P4 Enhancement 5)
        // =====================================================================

        /// <summary>
        /// Returns the list index at which the dragged tab should be inserted,
        /// based on where the drag cursor is relative to visible tab midpoints.
        /// </summary>
        private int ComputeDropInsertIndex(Point cursor)
        {
            if (_tabs == null || _tabs.Count == 0) return 0;

            bool vertical = _tabPosition == TabPosition.Left || _tabPosition == TabPosition.Right;
            int pos = vertical ? cursor.Y : cursor.X;

            // Walk visible tabs and find the first whose midpoint is to the right/below cursor.
            for (int i = 0; i < _tabs.Count; i++)
            {
                var t = _tabs[i];
                if (t.Bounds.IsEmpty) continue;
                int mid = vertical
                    ? t.Bounds.Top + t.Bounds.Height / 2
                    : t.Bounds.Left + t.Bounds.Width / 2;

                if (pos < mid)
                    return i;
            }
            return _tabs.Count; // append to end
        }

        #endregion
    }
}

