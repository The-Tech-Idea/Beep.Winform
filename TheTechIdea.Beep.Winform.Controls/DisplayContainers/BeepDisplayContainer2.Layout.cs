using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DisplayContainers.Helpers;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;

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
            // Derive tab strip thickness purely from font metrics when AutoTabHeight is on.
            int effectiveTabHeight;
            if (_autoTabHeight)
            {
                // Measure full rendered line height (GDI includes leading/ascent/descent)
                var textSize = TextRenderer.MeasureText("Ag", TextFont,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.SingleLine);

                // Compute vertical chrome: border + tab-specific small inner pad + shadow zone.
                // Do NOT use BeepStyling.GetPadding here — those values (8-20px) are designed
                // for full controls (buttons/input fields), not compact tab header strips.
                float borderW   = BeepStyling.GetBorderThickness(ControlStyle);
                int shadowDepth = StyleShadows.HasShadow(ControlStyle)
                    ? Math.Max(2, StyleShadows.GetShadowBlur(ControlStyle) / 2)
                    : 0;
                // Use the tab-specific VerticalPadding (≈2px at 96 DPI) for a compact strip.
                int tabVPad     = TabHeaderMetrics.VerticalPadding(this);
                int verticalChrome = (int)Math.Ceiling(borderW) * 2   // top + bottom border
                                   + tabVPad * 2                       // top + bottom tab inner pad
                                   + shadowDepth * 2;                  // top + bottom shadow zone

                // Indicator bar needs its own row below the text
                int indicatorRoom = TabHeaderMetrics.IndicatorThickness(this);

                effectiveTabHeight = textSize.Height + verticalChrome + indicatorRoom;
            }
            else
            {
                effectiveTabHeight = Math.Max(1, DpiScalingHelper.ScaleValue(_tabHeight, this));
            }
            
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
            if (_layoutHelper == null) return;
            
            // If tab area is empty, we can't layout tabs
            if (_tabArea.IsEmpty)
            {
                _needsScrolling = false;
                _scrollOffset = 0;
                _scrollLeftButton = Rectangle.Empty;
                _scrollRightButton = Rectangle.Empty;
                _overflowButton = Rectangle.Empty;
                _newTabButton = Rectangle.Empty;

                // Mark all tabs as not visible if tab area is empty
                foreach (var tab in _tabs)
                {
                    tab.IsVisible = false;
                    tab.Bounds = Rectangle.Empty;
                }
                return;
            }

            if (_tabs.Count == 0)
            {
                _scrollOffset = 0;
                var utilityLayout = _layoutHelper.CalculateUtilityButtonLayout(_tabArea, _tabPosition, false);
                _needsScrolling = utilityLayout.NeedsScrolling;
                _scrollLeftButton = utilityLayout.ScrollLeftButton;
                _scrollRightButton = utilityLayout.ScrollRightButton;
                _overflowButton = utilityLayout.OverflowButton;
                _newTabButton = utilityLayout.NewTabButton;
                return;
            }
            
            // Update layout helper with current style and font before calculating layout
            _layoutHelper.UpdateStyle(ControlStyle, TextFont);

            int scaledMinWidth = DpiScalingHelper.ScaleValue(_tabMinWidth, this);
            int scaledMaxWidth = DpiScalingHelper.ScaleValue(_tabMaxWidth, this);
            var result = _layoutHelper.CalculateTabLayout(_tabs, _tabArea, _tabPosition, 
                scaledMinWidth, scaledMaxWidth, _scrollOffset);
            
            _needsScrolling = result.NeedsScrolling;
            _scrollLeftButton = result.ScrollLeftButton;
            _scrollRightButton = result.ScrollRightButton;
            _overflowButton = result.OverflowButton;
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

        private Rectangle GetEmptyStateButtonRect()
        {
            var area = _contentArea.IsEmpty ? ClientRectangle : _contentArea;
            if (area.Width < 40 || area.Height < 40) return Rectangle.Empty;

            // Base sizes from DrawEmptyState
            int iconSize = Math.Min(DpiScalingHelper.ScaleValue(48, this), Math.Min(area.Width, area.Height) / 3);
            iconSize = Math.Max(16, iconSize);
            int iconY = area.Y + (area.Height - iconSize) / 2 - DpiScalingHelper.ScaleValue(12, this);
            int textY = iconY + iconSize + DpiScalingHelper.ScaleValue(30, this); // Account for text

            // Button sizes
            int btnWidth = DpiScalingHelper.ScaleValue(120, this);
            int btnHeight = DpiScalingHelper.ScaleValue(36, this);
            int btnX = area.X + (area.Width - btnWidth) / 2;
            int btnY = textY + DpiScalingHelper.ScaleValue(16, this);

            return new Rectangle(btnX, btnY, btnWidth, btnHeight);
        }

        private void PositionActiveAddin()
        {
            // Skip if in batch mode
            if (_batchMode) return;

            // When all tabs are closed, hide every child except controls that are part
            // of the container itself (e.g. scrollbars added by WinForms or designer).
            if (_tabs.Count == 0)
            {
                foreach (Control child in Controls)
                {
                    if (child.Visible)
                        child.Visible = false;
                }
                return;
            }
            
            // Ensure all tab controls are in the Controls collection and properly positioned
            foreach (var tab in _tabs)
            {
                var control = tab.Addin as Control;
                if (control == null || control.IsDisposed) continue;
                try
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
                        
                        // IMPORTANT: Set Dock=None BEFORE Bounds.
                        // If the child had Dock=Fill, setting Bounds first has no effect
                        // (dock manager overrides it). This caused child controls to cover
                        // the tab area on first add. On resize Dock was already None so
                        // Bounds stuck — which is why resize worked but AddControl didn't.
                        control.Dock = DockStyle.None;
                        control.Bounds = targetBounds;
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
                catch { /* skip controls that fail positioning */ }
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

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            // When a hosted addin (e.g. a UserControl with AutoScaleMode.Font) triggers a layout
            // by self-resizing after PerformAutoScale(), re-clamp all addin bounds back to the
            // content area so the tab header strip remains visible.
            if (!_batchMode && !_contentArea.IsEmpty
                && levent.AffectedControl != null && levent.AffectedControl != this
                && _tabs != null && _tabs.Count > 0)
            {
                if (_tabs.Any(t => t.Addin as Control == levent.AffectedControl))
                    PositionActiveAddin();
            }
        }

        #endregion
    }
}

