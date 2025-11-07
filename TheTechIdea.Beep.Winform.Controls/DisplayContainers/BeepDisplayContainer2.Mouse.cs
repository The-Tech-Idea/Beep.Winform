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
            else
            {
                Cursor = Cursors.Default;
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
                Cursor = Cursors.Default;
                Invalidate();
            }
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

        private void OnNewTabRequested()
        {
            // Override in derived classes or handle via events
        }

        #endregion
    }
}

