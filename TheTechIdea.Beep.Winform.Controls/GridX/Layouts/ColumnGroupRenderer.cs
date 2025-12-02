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
    /// Renders column groups for BeepGridPro
    /// Supports multi-level column headers for complex data hierarchies
    /// </summary>
    public class ColumnGroupRenderer
    {
        private BeepGridPro _grid;
        private List<ColumnGroup> _groups = new List<ColumnGroup>();
        
        /// <summary>
        /// Gets or sets the collection of column groups
        /// </summary>
        public List<ColumnGroup> Groups
        {
            get => _groups;
            set => _groups = value ?? new List<ColumnGroup>();
        }
        
        /// <summary>
        /// Initializes the column group renderer
        /// </summary>
        public void Initialize(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }
        
        /// <summary>
        /// Renders all column groups
        /// </summary>
        public void Render(Graphics g, Rectangle headerRect, IBeepTheme theme)
        {
            if (_grid == null || _groups == null || !_groups.Any())
                return;
            
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            // Calculate total group header height
            int maxLevel = _groups.Max(gr => gr.Level);
            int groupHeight = 32;
            int totalGroupHeight = (maxLevel + 1) * groupHeight;
            
            // Render groups by level
            for (int level = 0; level <= maxLevel; level++)
            {
                var levelGroups = _groups.Where(g => g.Level == level).ToList();
                var levelRect = new Rectangle(
                    headerRect.X,
                    headerRect.Y + (level * groupHeight),
                    headerRect.Width,
                    groupHeight
                );
                
                foreach (var group in levelGroups)
                {
                    RenderColumnGroup(g, levelRect, group, theme);
                }
            }
        }
        
        /// <summary>
        /// Renders a single column group
        /// </summary>
        private void RenderColumnGroup(Graphics g, Rectangle levelRect, ColumnGroup group, IBeepTheme theme)
        {
            // Get columns in this group
            var groupColumns = _grid.Data.Columns
                .Where(c => group.ColumnNames.Contains(c.ColumnName) && c.Visible)
                .ToList();
            
            if (!groupColumns.Any())
                return;
            
            // Calculate group bounds - sum widths of previous columns to get X position
            int startX = 0;
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                var col = _grid.Data.Columns[i];
                if (col == groupColumns.First())
                    break;
                if (col.Visible)
                    startX += col.Width;
            }
            int width = groupColumns.Sum(c => c.Width);
            
            var groupRect = new Rectangle(startX, levelRect.Y, width, levelRect.Height);
            
            // Draw group background
            using (var brush = new LinearGradientBrush(
                groupRect,
                Color.FromArgb(245, 245, 250),
                Color.FromArgb(235, 235, 245),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, groupRect);
            }
            
            // Draw group border
            using (var pen = new Pen(theme?.GridLineColor ?? Color.FromArgb(220, 220, 220)))
            {
                g.DrawRectangle(pen, groupRect);
            }
            
            // Draw collapse/expand icon if collapsible
            if (group.Collapsible)
            {
                DrawToggleIcon(g, groupRect, group.Collapsed, theme);
            }
            
            // Draw group header text
            DrawGroupText(g, groupRect, group, theme);
        }
        
        /// <summary>
        /// Draws the group header text
        /// </summary>
        private void DrawGroupText(Graphics g, Rectangle rect, ColumnGroup group, IBeepTheme theme)
        {
            using (var font = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (var brush = new SolidBrush(theme?.GridHeaderForeColor ?? Color.FromArgb(60, 60, 70)))
            {
                var textRect = rect;
                if (group.Collapsible)
                {
                    textRect.X += 20; // Make room for toggle icon
                    textRect.Width -= 20;
                }
                
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                
                g.DrawString(group.HeaderText, font, brush, textRect, format);
            }
        }
        
        /// <summary>
        /// Draws the collapse/expand toggle icon
        /// </summary>
        private void DrawToggleIcon(Graphics g, Rectangle rect, bool collapsed, IBeepTheme theme)
        {
            int iconSize = 12;
            var iconRect = new Rectangle(
                rect.X + 6,
                rect.Y + (rect.Height - iconSize) / 2,
                iconSize,
                iconSize
            );
            
            using (var pen = new Pen(theme?.GridHeaderForeColor ?? Color.FromArgb(100, 100, 100), 2))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                
                if (collapsed)
                {
                    // Draw "+" icon
                    int cx = iconRect.X + iconSize / 2;
                    int cy = iconRect.Y + iconSize / 2;
                    g.DrawLine(pen, cx, iconRect.Y, cx, iconRect.Bottom);
                    g.DrawLine(pen, iconRect.X, cy, iconRect.Right, cy);
                }
                else
                {
                    // Draw "-" icon
                    int cy = iconRect.Y + iconSize / 2;
                    g.DrawLine(pen, iconRect.X, cy, iconRect.Right, cy);
                }
            }
        }
        
        /// <summary>
        /// Calculates the total height needed for column groups
        /// </summary>
        public int CalculateTotalHeight()
        {
            if (_groups == null || !_groups.Any())
                return 0;
            
            int maxLevel = _groups.Max(g => g.Level);
            return (maxLevel + 1) * 32; // 32px per level
        }
        
        /// <summary>
        /// Handles click on column group header
        /// </summary>
        public bool HandleClick(Point location, Rectangle headerRect)
        {
            if (_groups == null || !_groups.Any())
                return false;
            
            int groupHeight = 32;
            int maxLevel = _groups.Max(g => g.Level);
            
            for (int level = 0; level <= maxLevel; level++)
            {
                var levelRect = new Rectangle(
                    headerRect.X,
                    headerRect.Y + (level * groupHeight),
                    headerRect.Width,
                    groupHeight
                );
                
                if (!levelRect.Contains(location))
                    continue;
                
                var levelGroups = _groups.Where(g => g.Level == level).ToList();
                
                foreach (var group in levelGroups)
                {
                    if (group.Collapsible && IsClickOnGroup(location, group, levelRect))
                    {
                        ToggleGroup(group);
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Checks if a point is within a column group
        /// </summary>
        private bool IsClickOnGroup(Point location, ColumnGroup group, Rectangle levelRect)
        {
            var groupColumns = _grid.Data.Columns
                .Where(c => group.ColumnNames.Contains(c.ColumnName) && c.Visible)
                .ToList();
            
            if (!groupColumns.Any())
                return false;
            
            // Calculate X position - sum widths of previous columns
            int startX = 0;
            for (int i = 0; i < _grid.Data.Columns.Count; i++)
            {
                var col = _grid.Data.Columns[i];
                if (col == groupColumns.First())
                    break;
                if (col.Visible)
                    startX += col.Width;
            }
            int width = groupColumns.Sum(c => c.Width);
            
            var groupRect = new Rectangle(startX, levelRect.Y, width, levelRect.Height);
            return groupRect.Contains(location);
        }
        
        /// <summary>
        /// Toggles a column group's collapsed state
        /// </summary>
        private void ToggleGroup(ColumnGroup group)
        {
            group.Collapsed = !group.Collapsed;
            
            // Hide/show columns in the group
            foreach (var columnName in group.ColumnNames)
            {
                var column = _grid.Data.Columns.FirstOrDefault(c => c.ColumnName == columnName);
                if (column != null)
                {
                    column.Visible = !group.Collapsed;
                }
            }
            
            // Recalculate and repaint
            if (_grid.Layout != null)
            {
                _grid.Layout.Recalculate();
            }
            _grid.Invalidate();
        }
        
        /// <summary>
        /// Adds a column group
        /// </summary>
        public void AddGroup(ColumnGroup group)
        {
            if (group != null && !_groups.Contains(group))
            {
                _groups.Add(group);
            }
        }
        
        /// <summary>
        /// Removes a column group
        /// </summary>
        public void RemoveGroup(ColumnGroup group)
        {
            _groups.Remove(group);
        }
        
        /// <summary>
        /// Clears all column groups
        /// </summary>
        public void ClearGroups()
        {
            _groups.Clear();
        }
    }
    
    /// <summary>
    /// Represents a column group
    /// </summary>
    public class ColumnGroup
    {
        /// <summary>
        /// Unique identifier for the group
        /// </summary>
        public string GroupId { get; set; }
        
        /// <summary>
        /// Display text for the group header
        /// </summary>
        public string HeaderText { get; set; }
        
        /// <summary>
        /// List of column names in this group
        /// </summary>
        public List<string> ColumnNames { get; set; } = new List<string>();
        
        /// <summary>
        /// Whether this group can be collapsed
        /// </summary>
        public bool Collapsible { get; set; } = false;
        
        /// <summary>
        /// Whether this group is currently collapsed
        /// </summary>
        public bool Collapsed { get; set; } = false;
        
        /// <summary>
        /// Level in the group hierarchy (0 = top level)
        /// </summary>
        public int Level { get; set; } = 0;
        
        /// <summary>
        /// Custom data associated with this group
        /// </summary>
        public object Tag { get; set; }
    }
}

