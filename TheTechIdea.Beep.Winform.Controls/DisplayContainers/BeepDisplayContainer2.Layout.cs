using System;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DisplayContainers
{
    public partial class BeepDisplayContainer2
    {
        #region Layout Management

        private void RecalculateLayout()
        {
            if (Width <= 0 || Height <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"RecalculateLayout: Skipping - Width: {Width}, Height: {Height}");
                return;
            }

            // Single mode: no tabs, full area for content
            if (_displayMode == ContainerDisplayMode.Single)
            {
                _tabArea = Rectangle.Empty;
                _contentArea = new Rectangle(0, 0, Width, Height);
                PositionActiveAddin();
                Invalidate(true);
                return;
            }

            // Tabbed mode: calculate tab and content areas
            // Ensure tab height is at least 24 pixels
            int effectiveTabHeight = Math.Max(24, _tabHeight);
            
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

            System.Diagnostics.Debug.WriteLine($"RecalculateLayout: TabArea={_tabArea}, ContentArea={_contentArea}, TabCount={_tabs?.Count ?? 0}, Visible={Visible}, Enabled={Enabled}, IsHandleCreated={IsHandleCreated}");
            
            CalculateTabLayout();
            PositionActiveAddin();
            
            // Force invalidation of tab area specifically if in Tabbed mode
            if (IsHandleCreated)
            {
                if (_displayMode == ContainerDisplayMode.Tabbed && !_tabArea.IsEmpty)
                {
                    // Invalidate tab area specifically
                    Invalidate(_tabArea, false);
                    // Also invalidate entire control
                    Invalidate(true);
                }
                else
                {
                    Invalidate(true);
                }
            }
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

            var result = _layoutHelper.CalculateTabLayout(_tabs, _tabArea, _tabPosition, 
                _tabMinWidth, _tabMaxWidth, _scrollOffset);
            
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
                        if (!isActive)
                        {
                            // Explicitly hide and invalidate non-active controls
                            control.Invalidate(true);
                        }
                    }
                    
                    // If this is the active control, ensure it's repainted
                    if (isActive && control.Visible)
                    {
                        control.Invalidate(true);
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RecalculateLayout();
        }

        #endregion
    }
}

