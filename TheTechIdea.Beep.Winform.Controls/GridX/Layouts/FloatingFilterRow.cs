using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Floating filter row for BeepGridPro
    /// Provides inline filtering below column headers (AG Grid style)
    /// </summary>
    public class FloatingFilterRow
    {
        private BeepGridPro _grid;
        private Dictionary<string, IFloatingFilter> _filters = new Dictionary<string, IFloatingFilter>();
        
        /// <summary>
        /// Height of the floating filter row
        /// </summary>
        public int Height { get; set; } = 36;
        
        /// <summary>
        /// Whether the floating filter row is visible
        /// </summary>
        public bool Visible { get; set; } = true;
        
        /// <summary>
        /// Event fired when a filter value changes
        /// </summary>
        public event EventHandler<FilterChangedEventArgs> FilterChanged;
        
        /// <summary>
        /// Initializes the floating filter row
        /// </summary>
        public void Initialize(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            
            // Create default filters for all columns
            if (_grid.Data?.Columns != null)
            {
                foreach (var column in _grid.Data.Columns.Where(c => c.Visible))
                {
                    if (!column.IsSelectionCheckBox && !column.IsRowNumColumn)
                    {
                        _filters[column.ColumnName] = new TextFloatingFilter(column.ColumnName);
                    }
                }
            }
        }
        
        /// <summary>
        /// Renders the floating filter row
        /// </summary>
        public void Render(Graphics g, Rectangle filterRect, IBeepTheme theme)
        {
            if (!Visible || _grid == null)
                return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            // Draw background
            using (var brush = new SolidBrush(theme?.GridHeaderBackColor ?? Color.FromArgb(250, 250, 250)))
            {
                g.FillRectangle(brush, filterRect);
            }
            
            // Draw bottom border
            using (var pen = new Pen(theme?.GridLineColor ?? Color.FromArgb(220, 220, 220)))
            {
                g.DrawLine(pen, filterRect.Left, filterRect.Bottom - 1, filterRect.Right, filterRect.Bottom - 1);
            }
            
            // Render filter cells
            var columns = _grid.Data.Columns.Where(c => c.Visible).ToList();
            int x = filterRect.X;
            
            foreach (var column in columns)
            {
                var cellRect = new Rectangle(x, filterRect.Y, column.Width, filterRect.Height);
                
                if (_filters.TryGetValue(column.ColumnName, out var filter))
                {
                    filter.Render(g, cellRect, theme);
                }
                else
                {
                    RenderEmptyCell(g, cellRect, theme);
                }
                
                // Draw vertical separator
                using (var pen = new Pen(theme?.GridLineColor ?? Color.FromArgb(230, 230, 230)))
                {
                    g.DrawLine(pen, cellRect.Right - 1, cellRect.Top, cellRect.Right - 1, cellRect.Bottom);
                }
                
                x += column.Width;
            }
        }
        
        /// <summary>
        /// Renders an empty filter cell
        /// </summary>
        private void RenderEmptyCell(Graphics g, Rectangle cellRect, IBeepTheme theme)
        {
            // Draw subtle background
            using (var brush = new SolidBrush(Color.FromArgb(252, 252, 252)))
            {
                g.FillRectangle(brush, cellRect);
            }
        }
        
        /// <summary>
        /// Sets a filter for a specific column
        /// </summary>
        public void SetFilter(string columnName, IFloatingFilter filter)
        {
            if (string.IsNullOrEmpty(columnName) || filter == null)
                return;
            
            _filters[columnName] = filter;
        }
        
        /// <summary>
        /// Gets a filter for a specific column
        /// </summary>
        public IFloatingFilter GetFilter(string columnName)
        {
            _filters.TryGetValue(columnName, out var filter);
            return filter;
        }
        
        /// <summary>
        /// Clears all filters
        /// </summary>
        public void ClearFilters()
        {
            foreach (var filter in _filters.Values)
            {
                filter.Clear();
            }
            
            OnFilterChanged();
        }
        
        /// <summary>
        /// Handles input for a filter at the specified location
        /// </summary>
        public bool HandleClick(Point location, Rectangle filterRect)
        {
            var columns = _grid.Data.Columns.Where(c => c.Visible).ToList();
            int x = filterRect.X;
            
            foreach (var column in columns)
            {
                var cellRect = new Rectangle(x, filterRect.Y, column.Width, filterRect.Height);
                
                if (cellRect.Contains(location))
                {
                    // Show input dialog or activate filter
                    if (_filters.TryGetValue(column.ColumnName, out var filter))
                    {
                        ActivateFilter(filter, column.ColumnName, cellRect);
                        return true;
                    }
                }
                
                x += column.Width;
            }
            
            return false;
        }
        
        /// <summary>
        /// Activates a filter for editing
        /// </summary>
        private void ActivateFilter(IFloatingFilter filter, string columnName, Rectangle cellRect)
        {
            // Show input dialog
            // (Implementation would show a text input or dropdown)
            // For now, just trigger the filter changed event
        }
        
        /// <summary>
        /// Fires the FilterChanged event
        /// </summary>
        protected virtual void OnFilterChanged()
        {
            FilterChanged?.Invoke(this, new FilterChangedEventArgs(_filters));
        }
    }
    
    /// <summary>
    /// Interface for floating filter implementations
    /// </summary>
    public interface IFloatingFilter
    {
        /// <summary>
        /// Renders the floating filter
        /// </summary>
        void Render(Graphics g, Rectangle cellRect, IBeepTheme theme);
        
        /// <summary>
        /// Gets the current filter value
        /// </summary>
        string GetValue();
        
        /// <summary>
        /// Sets the filter value
        /// </summary>
        void SetValue(string value);
        
        /// <summary>
        /// Clears the filter
        /// </summary>
        void Clear();
        
        /// <summary>
        /// Whether the filter is active (has a value)
        /// </summary>
        bool IsActive { get; }
    }
    
    /// <summary>
    /// Simple text-based floating filter
    /// </summary>
    public class TextFloatingFilter : IFloatingFilter
    {
        private string _columnName;
        private string _filterValue = string.Empty;
        
        public bool IsActive => !string.IsNullOrWhiteSpace(_filterValue);
        
        public TextFloatingFilter(string columnName)
        {
            _columnName = columnName;
        }
        
        public void Render(Graphics g, Rectangle cellRect, IBeepTheme theme)
        {
            var inputRect = new Rectangle(
                cellRect.X + 4,
                cellRect.Y + 4,
                cellRect.Width - 8,
                cellRect.Height - 8
            );
            
            // Background
            using (var brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, inputRect);
            }
            
            // Border (highlight if active)
            var borderColor = IsActive 
                ? theme?.AccentColor ?? Color.FromArgb(0, 122, 255)
                : Color.FromArgb(200, 200, 200);
            
            using (var pen = new Pen(borderColor, IsActive ? 1.5f : 1f))
            {
                g.DrawRectangle(pen, inputRect);
            }
            
            // Filter icon
            DrawFilterIcon(g, new Rectangle(inputRect.X + 6, inputRect.Y + (inputRect.Height - 14) / 2, 14, 14), IsActive, theme);
            
            // Text
            if (!string.IsNullOrEmpty(_filterValue))
            {
                using (var font = new Font("Segoe UI", 8.5f))
                using (var textBrush = new SolidBrush(Color.FromArgb(60, 60, 60)))
                {
                    var textRect = new Rectangle(inputRect.X + 26, inputRect.Y, inputRect.Width - 26, inputRect.Height);
                    var format = new StringFormat 
                    { 
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    };
                    g.DrawString(_filterValue, font, textBrush, textRect, format);
                }
            }
            else
            {
                // Placeholder text
                using (var font = new Font("Segoe UI", 8.5f, FontStyle.Italic))
                using (var textBrush = new SolidBrush(Color.FromArgb(160, 160, 160)))
                {
                    var textRect = new Rectangle(inputRect.X + 26, inputRect.Y, inputRect.Width - 26, inputRect.Height);
                    var format = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString("Filter...", font, textBrush, textRect, format);
                }
            }
        }
        
        private void DrawFilterIcon(Graphics g, Rectangle rect, bool active, IBeepTheme theme)
        {
            var color = active 
                ? theme?.AccentColor ?? Color.FromArgb(0, 122, 255)
                : Color.FromArgb(150, 150, 150);
            
            using (var pen = new Pen(color, 1.5f))
            {
                // Draw funnel shape
                g.DrawLine(pen, rect.X, rect.Y + 2, rect.Right, rect.Y + 2);
                g.DrawLine(pen, rect.X, rect.Y + 2, rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                g.DrawLine(pen, rect.Right, rect.Y + 2, rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                g.DrawLine(pen, rect.X + rect.Width / 2, rect.Y + rect.Height / 2, rect.X + rect.Width / 2, rect.Bottom - 2);
            }
        }
        
        public string GetValue() => _filterValue;
        
        public void SetValue(string value)
        {
            _filterValue = value ?? string.Empty;
        }
        
        public void Clear()
        {
            _filterValue = string.Empty;
        }
    }
    
    /// <summary>
    /// Event args for filter changes
    /// </summary>
    public class FilterChangedEventArgs : EventArgs
    {
        public Dictionary<string, IFloatingFilter> Filters { get; }
        
        public FilterChangedEventArgs(Dictionary<string, IFloatingFilter> filters)
        {
            Filters = filters;
        }
    }
}

