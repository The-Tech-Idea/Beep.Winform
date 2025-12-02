using System;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Manages responsive layout behavior for BeepGridPro
    /// Automatically adjusts column visibility, row heights, and navigation based on screen size
    /// </summary>
    public class ResponsiveLayoutManager : IDisposable
    {
        private BeepGridPro _grid;
        private ResponsiveConfig _config;
        private ScreenSize _currentScreenSize = ScreenSize.Large;
        private int _lastWidth = 0;
        private bool _isInitialized = false;
        
        /// <summary>
        /// Gets the current screen size category
        /// </summary>
        public ScreenSize CurrentScreenSize => _currentScreenSize;
        
        /// <summary>
        /// Event fired when screen size changes
        /// </summary>
        public event EventHandler<ScreenSizeChangedEventArgs> ScreenSizeChanged;
        
        /// <summary>
        /// Initializes the responsive layout manager
        /// </summary>
        public void Initialize(BeepGridPro grid, ResponsiveConfig config)
        {
            if (_isInitialized)
                throw new InvalidOperationException("ResponsiveLayoutManager is already initialized");
            
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            
            // Subscribe to grid resize
            _grid.Resize += OnGridResize;
            _grid.SizeChanged += OnGridResize;
            
            // Initial layout application
            _lastWidth = _grid.Width;
            ApplyResponsiveLayout(_lastWidth);
            
            _isInitialized = true;
        }
        
        /// <summary>
        /// Handles grid resize events
        /// </summary>
        private void OnGridResize(object sender, EventArgs e)
        {
            if (!_config.Enabled)
                return;
            
            int newWidth = _grid.Width;
            
            // Only process if width actually changed
            if (Math.Abs(newWidth - _lastWidth) > 10) // 10px threshold to avoid too frequent updates
            {
                ApplyResponsiveLayout(newWidth);
                _lastWidth = newWidth;
            }
        }
        
        /// <summary>
        /// Applies responsive layout based on current width
        /// </summary>
        private void ApplyResponsiveLayout(int width)
        {
            ScreenSize newScreenSize = DetermineScreenSize(width);
            
            if (newScreenSize != _currentScreenSize)
            {
                ScreenSize oldScreenSize = _currentScreenSize;
                _currentScreenSize = newScreenSize;
                
                // Apply responsive rules
                ApplyColumnVisibility(newScreenSize);
                ApplyRowHeightAdjustment(newScreenSize);
                ApplyNavigationCollapse(newScreenSize);
                ApplyHeaderVisibility(newScreenSize);
                
                // Recalculate layout
                if (_grid.Layout != null)
                {
                    _grid.Layout.Recalculate();
                }
                
                _grid.Invalidate();
                
                // Fire event
                OnScreenSizeChanged(oldScreenSize, newScreenSize);
            }
        }
        
        /// <summary>
        /// Determines the screen size category based on width
        /// </summary>
        private ScreenSize DetermineScreenSize(int width)
        {
            if (width < _config.BreakpointExtraSmall) return ScreenSize.ExtraSmall;
            if (width < _config.BreakpointSmall) return ScreenSize.Small;
            if (width < _config.BreakpointMedium) return ScreenSize.Medium;
            if (width < _config.BreakpointLarge) return ScreenSize.Large;
            if (width < _config.BreakpointExtraLarge) return ScreenSize.ExtraLarge;
            if (width < _config.Breakpoint2XL) return ScreenSize.TwoXL;
            return ScreenSize.TwoXL;
        }
        
        /// <summary>
        /// Applies column visibility rules based on screen size
        /// </summary>
        private void ApplyColumnVisibility(ScreenSize screenSize)
        {
            if (_grid?.Data?.Columns == null)
                return;
            
            foreach (var column in _grid.Data.Columns)
            {
                // Check if there's a rule for this column
                if (_config.ColumnVisibilityRules.TryGetValue(column.ColumnName, out var rule))
                {
                    column.Visible = rule.IsVisibleAt(screenSize);
                }
                else
                {
                    // Default: hide low-priority columns on small screens
                    if (screenSize <= ScreenSize.Small)
                    {
                        // Keep only essential columns on small screens
                        bool isEssential = column.IsSelectionCheckBox || 
                                          column.IsRowNumColumn || 
                                          IsEssentialColumn(column);
                        
                        column.Visible = isEssential;
                    }
                    else
                    {
                        // Show all columns on larger screens
                        column.Visible = true;
                    }
                }
            }
        }
        
        /// <summary>
        /// Determines if a column is essential and should be shown on small screens
        /// </summary>
        private bool IsEssentialColumn(BeepColumnConfig column)
        {
            // First visible column is essential
            var visibleColumns = _grid.Data.Columns.Where(c => c.Visible && !c.IsSelectionCheckBox && !c.IsRowNumColumn).ToList();
            if (visibleColumns.Any() && visibleColumns.First() == column)
                return true;
            
            // Columns with "essential" keywords
            var name = (column.ColumnCaption ?? column.ColumnName ?? "").ToLowerInvariant();
            return name.Contains("name") || 
                   name.Contains("title") || 
                   name.Contains("id") ||
                   name.Contains("status");
        }
        
        /// <summary>
        /// Applies row height adjustments based on screen size
        /// </summary>
        private void ApplyRowHeightAdjustment(ScreenSize screenSize)
        {
            if (_config.RowHeightAdjustments.TryGetValue(screenSize, out int adjustedHeight))
            {
                _grid.RowHeight = adjustedHeight;
            }
        }
        
        /// <summary>
        /// Applies navigation collapse based on screen size
        /// </summary>
        private void ApplyNavigationCollapse(ScreenSize screenSize)
        {
            if (_config.CollapseNavigationOnSmall && screenSize <= ScreenSize.Small)
            {
                // Collapse or minimize navigation on small screens
                if (_grid.Layout != null)
                {
                    _grid.Layout.NavigatorHeight = 40; // Compact navigation
                }
            }
        }
        
        /// <summary>
        /// Applies header visibility based on screen size
        /// </summary>
        private void ApplyHeaderVisibility(ScreenSize screenSize)
        {
            if (_config.HideHeadersOnExtraSmall && screenSize == ScreenSize.ExtraSmall)
            {
                _grid.ShowColumnHeaders = false;
            }
            else
            {
                _grid.ShowColumnHeaders = true;
            }
        }
        
        /// <summary>
        /// Fires the ScreenSizeChanged event
        /// </summary>
        protected virtual void OnScreenSizeChanged(ScreenSize oldSize, ScreenSize newSize)
        {
            ScreenSizeChanged?.Invoke(this, new ScreenSizeChangedEventArgs(oldSize, newSize));
        }
        
        /// <summary>
        /// Manually triggers responsive layout recalculation
        /// </summary>
        public void Refresh()
        {
            if (_grid != null)
            {
                ApplyResponsiveLayout(_grid.Width);
            }
        }
        
        /// <summary>
        /// Adds a column visibility rule
        /// </summary>
        public void AddColumnRule(string columnName, ScreenSize minimumScreenSize, int priority = 5)
        {
            var rule = new ColumnVisibilityRule
            {
                MinimumScreenSize = minimumScreenSize,
                Priority = priority,
                VisibleOnExtraSmall = minimumScreenSize <= ScreenSize.ExtraSmall,
                VisibleOnSmall = minimumScreenSize <= ScreenSize.Small,
                VisibleOnMedium = minimumScreenSize <= ScreenSize.Medium,
                VisibleOnLarge = minimumScreenSize <= ScreenSize.Large,
                VisibleOnExtraLarge = true,
                VisibleOn2XL = true
            };
            
            _config.ColumnVisibilityRules[columnName] = rule;
        }
        
        /// <summary>
        /// Removes a column visibility rule
        /// </summary>
        public void RemoveColumnRule(string columnName)
        {
            _config.ColumnVisibilityRules.Remove(columnName);
        }
        
        /// <summary>
        /// Gets the current screen size as a string
        /// </summary>
        public string GetScreenSizeDescription()
        {
            return _currentScreenSize switch
            {
                ScreenSize.ExtraSmall => "Extra Small (Mobile Portrait)",
                ScreenSize.Small => "Small (Mobile Landscape)",
                ScreenSize.Medium => "Medium (Tablet)",
                ScreenSize.Large => "Large (Laptop)",
                ScreenSize.ExtraLarge => "Extra Large (Desktop)",
                ScreenSize.TwoXL => "2XL (Ultra-Wide)",
                _ => "Unknown"
            };
        }
        
        /// <summary>
        /// Disposes the responsive layout manager
        /// </summary>
        public void Dispose()
        {
            if (_grid != null)
            {
                _grid.Resize -= OnGridResize;
                _grid.SizeChanged -= OnGridResize;
            }
            
            _grid = null;
            _config = null;
            _isInitialized = false;
        }
    }
    
    /// <summary>
    /// Event args for screen size changes
    /// </summary>
    public class ScreenSizeChangedEventArgs : EventArgs
    {
        public ScreenSize OldSize { get; }
        public ScreenSize NewSize { get; }
        
        public ScreenSizeChangedEventArgs(ScreenSize oldSize, ScreenSize newSize)
        {
            OldSize = oldSize;
            NewSize = newSize;
        }
    }
}

