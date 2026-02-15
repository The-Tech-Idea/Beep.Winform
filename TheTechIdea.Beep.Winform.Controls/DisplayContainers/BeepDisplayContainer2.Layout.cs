using System;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Layout Management

        public void RecalculateLayout()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }
            
            // Skip if in batch mode - will recalculate once at EndUpdate
            if (_batchMode)
            {
                return;
            }

            // Single mode: no tabs, full area for content
            if (_displayMode == ContainerDisplayMode.Single)
            {
                _tabArea = Rectangle.Empty;
                _contentArea = new Rectangle(0, 0, Width, Height);
                PositionActiveAddin();
                // Don't invalidate here - caller will handle it
                return;
            }

            // Tabbed mode: calculate tab and content areas
            // Ensure tab height is at least 24 pixels (DPI-scaled)
            int minTabHeight = DpiScalingHelper.ScaleValue(24, this);
            int effectiveTabHeight = Math.Max(minTabHeight, DpiScalingHelper.ScaleValue(_tabHeight, this));
            
            switch (_tabPosition)
            {
                case TabPosition.Top:
                    _tabArea = new Rectangle(0, 0, Width, effectiveTabHeight);
                    _contentArea = new Rectangle(0, effectiveTabHeight, Width, Math.Max(0, Height - effectiveTabHeight));
                    break;
                case TabPosition.Bottom:
                    _contentArea = new Rectangle(0, 0, Width, Math.Max(0, Height - effectiveTabHeight));
                    _tabArea = new Rectangle(0, Height - effectiveTabHeight, Width, effectiveTabHeight);
                    break;
                case TabPosition.Left:
                    _tabArea = new Rectangle(0, 0, effectiveTabHeight, Height);
                    _contentArea = new Rectangle(effectiveTabHeight, 0, Math.Max(0, Width - effectiveTabHeight), Height);
                    break;
                case TabPosition.Right:
                    _contentArea = new Rectangle(0, 0, Math.Max(0, Width - effectiveTabHeight), Height);
                    _tabArea = new Rectangle(Width - effectiveTabHeight, 0, effectiveTabHeight, Height);
                    break;
            }
            
            CalculateTabLayout();
            PositionActiveAddin();
            
            // Don't invalidate here - let caller handle it to avoid excessive repaints
        }

        private void CalculateTabLayout()
        {
            if (_tabs.Count == 0 || _layoutHelper == null) return;
            
            // If tab area is empty, we can't layout tabs
            if (_tabArea.IsEmpty)
            {
                // Mark all tabs as not visible if tab area is empty
                foreach (var tab in _tabs)
                {
                    tab.IsVisible = false;
                }
                return;
            }
            
            // Update layout helper with current style and font before calculating layout
            _layoutHelper.UpdateStyle(ControlStyle, Font);

            int scaledMinWidth = DpiScalingHelper.ScaleValue(_tabMinWidth, this);
            int scaledMaxWidth = DpiScalingHelper.ScaleValue(_tabMaxWidth, this);
            var result = _layoutHelper.CalculateTabLayout(_tabs, _tabArea, _tabPosition, 
                scaledMinWidth, scaledMaxWidth, _scrollOffset);
            
            _needsScrolling = result.NeedsScrolling;
            _scrollLeftButton = result.ScrollLeftButton;
            _scrollRightButton = result.ScrollRightButton;
            _newTabButton = result.NewTabButton;
            
            // Ensure all tabs have valid bounds and visibility after layout calculation
            foreach (var tab in _tabs)
            {
                if (tab.Bounds.IsEmpty && tab.IsVisible)
                {
                    // Tab was marked visible but has no bounds - this shouldn't happen, but mark as not visible
                    tab.IsVisible = false;
                }
            }
        }

        private void PositionActiveAddin()
        {
            // Skip if in batch mode
            if (_batchMode) return;
            
            // Ensure all tab controls are in the Controls collection and properly positioned
            foreach (var tab in _tabs)
            {
                var control = tab.Addin as Control;
                if (control != null)
                {
                    // Ensure control is always in Controls collection
                    if (!Controls.Contains(control))
                    {
                        Controls.Add(control);
                    }
                    
                    // Check if this is the active tab - simple immediate switch, no transitions
                    bool isActive = (tab == _activeTab);
                    
                    // Position control in content area
                    if (_displayMode == ContainerDisplayMode.Single)
                    {
                        // In Single mode, use full container bounds
                        control.Bounds = ClientRectangle;
                        control.Dock = DockStyle.Fill;
                    }
                    else
                    {
                        // In Tabbed mode, use content area (not tab area)
                        var targetBounds = new Rectangle(
                            _contentArea.X,
                            _contentArea.Y,
                            Math.Max(0, _contentArea.Width),
                            Math.Max(0, _contentArea.Height)
                        );
                        
                        control.Bounds = targetBounds;
                        control.Dock = DockStyle.None;
                        control.Region = null; // Don't clip with Region - causes rendering issues
                    }
                    
                    // Simple visibility: show only active control, hide all others
                    // Force visibility change even if it seems redundant - ensures proper state
                    if (control.Visible != isActive)
                    {
                        control.Visible = isActive;
                    }
                    
                    // Don't invalidate during positioning - let caller handle it
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateLayout();
            // Trigger repaint after resize
            if (!_batchMode && IsHandleCreated)
            {
                Invalidate();
            }
        }

        #endregion
    }
}

