using System;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Tab Management

        private void ActivateTab(AddinTab tab)
        {
            if (tab == _activeTab) return;

            var oldTab = _activeTab;
            var oldControl = oldTab?.Addin as Control;
            
            // CRITICAL: Hide and invalidate old control FIRST before changing active tab
            if (oldControl != null)
            {
                oldControl.Visible = false;
                oldControl.Invalidate(true);
                // Don't call Update() - causes flicker, let it repaint naturally
            }
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
            if (activeControl != null)
            {
                activeControl.Visible = true;
                activeControl.Invalidate(true);
                // Don't call Update() - causes flicker, let it repaint naturally
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
            // Don't call Update() - causes flicker, let it repaint naturally
        }

        private void RemoveTab(AddinTab tab)
        {
            if (tab == null || !tab.CanClose) return;

            var wasActive = (tab == _activeTab);
            var control = tab.Addin as Control;
            
            // Hide control but KEEP it in Controls collection (controls should persist)
            if (control != null)
            {
                control.Visible = false;
                // DO NOT remove from Controls collection - controls should persist
            }

            // Remove from tab collections (removes tab header, but control stays in Controls)
            _tabs.Remove(tab);
            _addins.Remove(tab.Id);
            
            // Clear hover reference if this was the hovered tab
            if (tab == _hoveredTab)
            {
                _hoveredTab = null;
            }

            // DO NOT dispose the addin - controls should persist
            // if (tab.Addin is IDisposable disposable)
            // {
            //     disposable.Dispose();
            // }

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
                _activeTab = _tabs.FirstOrDefault();
                if (_activeTab != null)
                {
                    ActivateTab(_activeTab);
                }
                else
                {
                    // No more tabs - clear active tab
                    _activeTab = null;
                    PositionActiveAddin();
                }
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

        #endregion
    }
}

