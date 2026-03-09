using System;
using System.Drawing;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Tab Management

        private void ActivateTab(AddinTab tab)
        {
            if (tab == null) return;
            if (tab == _activeTab) return;

            var oldTab = _activeTab;
            var oldControl = oldTab?.Addin as Control;
            
            // CRITICAL: Hide and invalidate old control FIRST before changing active tab
            if (oldControl != null && !oldControl.IsDisposed)
            {
                try { oldControl.Visible = false; } catch { /* ignore */ }
                try { oldControl.Invalidate(true); } catch { /* ignore */ }
            }

            _indicatorProgress = 1f; // no animation — prevent timer from invalidating only _tabArea
            _previousTab = null;
            
            _activeTab = tab;

            // Raise events
            if (oldTab != null)
            {
                OnAddinChanging(new ContainerEvents 
                { 
                    TitleText = oldTab.Id, 
                    Control = oldTab.Addin, 
                    ContainerType = _containerType, 
                    Guidid = oldTab.Addin?.GuidID 
                });
            }

            // Recalculate layout to ensure tab bounds are correct
            RecalculateLayout();
            
            // Position controls immediately - this will set visibility for all controls
            PositionActiveAddin();
            
            // Ensure active control is visible and properly positioned
            var activeControl = tab?.Addin as Control;
            if (activeControl != null && !activeControl.IsDisposed)
            {
                try { activeControl.Visible = true; } catch { /* ignore */ }
                try { activeControl.Invalidate(true); } catch { /* ignore */ }
            }
            
            OnAddinChanged(new ContainerEvents 
            { 
                TitleText = tab.Id, 
                Control = tab.Addin, 
                ContainerType = _containerType, 
                Guidid = tab.Addin?.GuidID 
            });
            
            // Force full repaint of container including content area and tabs
            Invalidate(true);
        }

        private void RemoveTab(AddinTab tab)
        {
            if (tab == null || !tab.CanClose) return;
            // Pinned tabs cannot be closed
            if (tab.IsPinned) return;

            var wasActive = (tab == _activeTab);
            var control = tab.Addin as Control;
            
            // Remove the associated control from the container
            if (control != null)
            {
                control.Visible = false;
                if (Controls.Contains(control))
                {
                    Controls.Remove(control);
                }
            }

            // Remove from tab collections
            _tabs.Remove(tab);
            _addins.Remove(tab.Id);
            
            // Clear hover reference if this was the hovered tab
            if (tab == _hoveredTab)
            {
                _hoveredTab = null;
            }

            // DO NOT dispose the addin - controls should persist

            // Raise event before activating another tab
            OnAddinRemoved(new ContainerEvents 
            { 
                TitleText = tab.Id, 
                Control = tab.Addin, 
                ContainerType = _containerType, 
                Guidid = tab.Addin?.GuidID 
            });

            // If this was the active tab, activate another
            if (wasActive)
            {
                var nextTab = _tabs.FirstOrDefault();
                if (nextTab != null)
                {
                    ActivateTab(nextTab);
                }
                else
                {
                    // No more tabs - clear active state and reset strip interaction state.
                    ResetTabHeaderStateForEmpty();
                    PositionActiveAddin();
                }
            }
            else if (_tabs.Count == 0)
            {
                // Defensive path: tab list became empty while removing a non-active tab.
                ResetTabHeaderStateForEmpty();
                PositionActiveAddin();
            }

            // Recalculate layout to update tab positions and remove the tab header
            RecalculateLayout();
            
            // Invalidate the tab area specifically to redraw headers (removed tab won't be drawn)
            if (!_tabArea.IsEmpty && _displayMode == ContainerDisplayMode.Tabbed)
            {
                Invalidate(_tabArea, false);
            }
            
            // Invalidate entire control to ensure everything is redrawn
            Invalidate(true);
        }

        private void ResetTabHeaderStateForEmpty()
        {
            _activeTab = null;
            _hoveredTab = null;
            _previousTab = null;

            _scrollOffset = 0;
            _needsScrolling = false;
            _scrollLeftButton = Rectangle.Empty;
            _scrollRightButton = Rectangle.Empty;
            _newTabButton = Rectangle.Empty;

            _hoveredScrollButton = 0;
            _pressedScrollButton = 0;

            _dragTab = null;
            _isDragging = false;
            _dropInsertIndex = -1;
            _dragGhostLoc = Point.Empty;

            _indicatorFrom = Rectangle.Empty;
            _indicatorTo = Rectangle.Empty;
            _indicatorProgress = 1f;
        }

        /// <summary>
        /// Pins a tab — makes it compact (icon-only), grouped to the left, and non-closable.
        /// </summary>
        public void PinTab(AddinTab tab)
        {
            if (tab == null || tab.IsPinned) return;
            tab.IsPinned = true;
            // Move pinned tabs to the front of the list
            SortPinnedTabsFirst();
            RecalculateLayout();
            Invalidate(true);
        }

        /// <summary>
        /// Unpins a previously-pinned tab, restoring its full title and close button.
        /// </summary>
        public void UnpinTab(AddinTab tab)
        {
            if (tab == null || !tab.IsPinned) return;
            tab.IsPinned = false;
            SortPinnedTabsFirst();
            RecalculateLayout();
            Invalidate(true);
        }

        /// <summary>
        /// Re-orders the tab list so that all pinned tabs precede unpinned tabs,
        /// preserving the relative order within each group.
        /// </summary>
        private void SortPinnedTabsFirst()
        {
            var pinned = _tabs.Where(t => t.IsPinned).ToList();
            var unpinned = _tabs.Where(t => !t.IsPinned).ToList();
            _tabs.Clear();
            _tabs.AddRange(pinned);
            _tabs.AddRange(unpinned);
        }

        #endregion
    }
}

