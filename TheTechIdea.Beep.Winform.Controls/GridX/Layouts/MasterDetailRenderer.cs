using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Layouts
{
    /// <summary>
    /// Renders expandable master/detail rows for BeepGridPro
    /// Allows rows to expand and show detailed information
    /// </summary>
    public class MasterDetailRenderer : IDisposable
    {
        private BeepGridPro _grid;
        private Dictionary<int, MasterDetailRow> _detailRows = new Dictionary<int, MasterDetailRow>();
        private const int ToggleColumnWidth = 32;
        
        /// <summary>
        /// Event fired when a detail row is expanded
        /// </summary>
        public event EventHandler<DetailRowEventArgs> DetailExpanded;
        
        /// <summary>
        /// Event fired when a detail row is collapsed
        /// </summary>
        public event EventHandler<DetailRowEventArgs> DetailCollapsed;
        
        /// <summary>
        /// Initializes the master/detail renderer
        /// </summary>
        public void Initialize(BeepGridPro grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }
        
        /// <summary>
        /// Renders a master row with expand/collapse toggle
        /// </summary>
        public void RenderMasterRow(Graphics g, Rectangle rowRect, BeepRowConfig row, int rowIndex, IBeepTheme theme)
        {
            if (_grid == null) return;
            
            // Draw expand/collapse toggle
            var toggleRect = new Rectangle(
                rowRect.X + (ToggleColumnWidth - 16) / 2,
                rowRect.Y + (rowRect.Height - 16) / 2,
                16,
                16
            );
            
            bool isExpanded = _detailRows.TryGetValue(rowIndex, out var detailRow) && detailRow.IsExpanded;
            
            DrawToggleIcon(g, toggleRect, isExpanded, theme);
            
            // Row content is shifted right to make room for toggle
            // (This would be handled in the main grid rendering)
        }
        
        /// <summary>
        /// Renders a detail row
        /// </summary>
        public void RenderDetailRow(Graphics g, Rectangle detailRect, MasterDetailRow detailRow, IBeepTheme theme)
        {
            if (detailRow == null || !detailRow.IsExpanded)
                return;
            
            // Draw detail background
            using (var brush = new SolidBrush(Color.FromArgb(250, 252, 255)))
            {
                g.FillRectangle(brush, detailRect);
            }
            
            // Draw border
            using (var pen = new Pen(Color.FromArgb(220, 230, 240), 1.5f))
            {
                g.DrawRectangle(pen, detailRect);
            }
            
            // Draw subtle inner shadow for depth
            DrawInnerShadow(g, detailRect);
            
            // Detail control is rendered separately in WinForms control tree
            // Just render the container here
            
            if (detailRow.DetailControl == null)
            {
                // Show placeholder if no control set
                DrawDetailPlaceholder(g, detailRect, theme);
            }
        }
        
        /// <summary>
        /// Draws the expand/collapse toggle icon
        /// </summary>
        private void DrawToggleIcon(Graphics g, Rectangle rect, bool expanded, IBeepTheme theme)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            var iconColor = theme?.GridHeaderForeColor ?? Color.FromArgb(100, 100, 100);
            using (var pen = new Pen(iconColor, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                
                if (expanded)
                {
                    // Draw down arrow (chevron)
                    Point[] points = new[]
                    {
                        new Point(rect.X + 4, rect.Y + 6),
                        new Point(rect.X + 8, rect.Y + 10),
                        new Point(rect.X + 12, rect.Y + 6)
                    };
                    g.DrawLines(pen, points);
                }
                else
                {
                    // Draw right arrow (chevron)
                    Point[] points = new[]
                    {
                        new Point(rect.X + 6, rect.Y + 4),
                        new Point(rect.X + 10, rect.Y + 8),
                        new Point(rect.X + 6, rect.Y + 12)
                    };
                    g.DrawLines(pen, points);
                }
            }
            
            // Draw hover background
            if (rect.Contains(_grid.PointToClient(Cursor.Position)))
            {
                using (var hoverBrush = new SolidBrush(Color.FromArgb(20, iconColor)))
                {
                    g.FillEllipse(hoverBrush, rect);
                }
            }
        }
        
        /// <summary>
        /// Draws inner shadow for detail row
        /// </summary>
        private void DrawInnerShadow(Graphics g, Rectangle rect)
        {
            // Top shadow
            using (var brush = new LinearGradientBrush(
                new Rectangle(rect.X, rect.Y, rect.Width, 4),
                Color.FromArgb(30, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, new Rectangle(rect.X, rect.Y, rect.Width, 4));
            }
        }
        
        /// <summary>
        /// Draws placeholder for empty detail content
        /// </summary>
        private void DrawDetailPlaceholder(Graphics g, Rectangle rect, IBeepTheme theme)
        {
            using (var font = new Font("Segoe UI", 9f, FontStyle.Italic))
            using (var brush = new SolidBrush(Color.FromArgb(160, 160, 160)))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString("Detail content not configured", font, brush, rect, format);
            }
        }
        
        /// <summary>
        /// Toggles a detail row's expanded state
        /// </summary>
        public void ToggleDetail(int rowIndex)
        {
            if (_detailRows.TryGetValue(rowIndex, out var detailRow))
            {
                detailRow.IsExpanded = !detailRow.IsExpanded;
                
                if (detailRow.IsExpanded)
                {
                    ShowDetail(rowIndex, detailRow);
                    OnDetailExpanded(rowIndex);
                }
                else
                {
                    HideDetail(rowIndex, detailRow);
                    OnDetailCollapsed(rowIndex);
                }
            }
            else
            {
                // Create new detail row
                detailRow = new MasterDetailRow
                {
                    MasterRowIndex = rowIndex,
                    IsExpanded = true,
                    DetailHeight = 200
                };
                
                _detailRows[rowIndex] = detailRow;
                ShowDetail(rowIndex, detailRow);
                OnDetailExpanded(rowIndex);
            }
            
            // Recalculate layout
            if (_grid.Layout != null)
            {
                _grid.Layout.Recalculate();
            }
            _grid.Invalidate();
        }
        
        /// <summary>
        /// Shows a detail row
        /// </summary>
        private void ShowDetail(int rowIndex, MasterDetailRow detailRow)
        {
            if (detailRow.DetailControl != null && _grid != null)
            {
                // Add control to grid
                if (!_grid.Controls.Contains(detailRow.DetailControl))
                {
                    _grid.Controls.Add(detailRow.DetailControl);
                }
                
                // Position it
                PositionDetailControl(rowIndex, detailRow);
                
                detailRow.DetailControl.Visible = true;
                detailRow.DetailControl.BringToFront();
            }
        }
        
        /// <summary>
        /// Hides a detail row
        /// </summary>
        private void HideDetail(int rowIndex, MasterDetailRow detailRow)
        {
            if (detailRow.DetailControl != null && _grid != null)
            {
                detailRow.DetailControl.Visible = false;
            }
        }
        
        /// <summary>
        /// Positions a detail control below its master row
        /// </summary>
        private void PositionDetailControl(int rowIndex, MasterDetailRow detailRow)
        {
            if (detailRow.DetailControl == null || _grid == null)
                return;
            
            // Calculate position based on master row
            // Calculate data area manually
            int headerHeight = _grid.ShowColumnHeaders ? _grid.ColumnHeaderHeight : 0;
            int dataY = headerHeight;
            int y = dataY + (rowIndex * _grid.RowHeight) + _grid.RowHeight;
            
            detailRow.DetailControl.Location = new Point(ToggleColumnWidth, y);
            detailRow.DetailControl.Size = new Size(_grid.Width - ToggleColumnWidth, detailRow.DetailHeight);
        }
        
        /// <summary>
        /// Sets the detail control for a row
        /// </summary>
        public void SetDetailControl(int rowIndex, Control detailControl, int height = 200)
        {
            if (!_detailRows.TryGetValue(rowIndex, out var detailRow))
            {
                detailRow = new MasterDetailRow
                {
                    MasterRowIndex = rowIndex,
                    IsExpanded = false,
                    DetailHeight = height
                };
                _detailRows[rowIndex] = detailRow;
            }
            
            detailRow.DetailControl = detailControl;
            detailRow.DetailHeight = height;
        }
        
        /// <summary>
        /// Handles click on master row to toggle detail
        /// </summary>
        public bool HandleClick(Point location, int rowIndex, Rectangle rowRect)
        {
            var toggleRect = new Rectangle(
                rowRect.X + (ToggleColumnWidth - 16) / 2,
                rowRect.Y + (rowRect.Height - 16) / 2,
                16,
                16
            );
            
            if (toggleRect.Contains(location))
            {
                ToggleDetail(rowIndex);
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Gets the detail row for a master row
        /// </summary>
        public MasterDetailRow GetDetailRow(int rowIndex)
        {
            _detailRows.TryGetValue(rowIndex, out var detailRow);
            return detailRow;
        }
        
        /// <summary>
        /// Checks if a row has an expanded detail
        /// </summary>
        public bool IsExpanded(int rowIndex)
        {
            return _detailRows.TryGetValue(rowIndex, out var detailRow) && detailRow.IsExpanded;
        }
        
        /// <summary>
        /// Collapses all detail rows
        /// </summary>
        public void CollapseAll()
        {
            foreach (var detailRow in _detailRows.Values)
            {
                if (detailRow.IsExpanded)
                {
                    detailRow.IsExpanded = false;
                    HideDetail(detailRow.MasterRowIndex, detailRow);
                }
            }
            
            _grid?.Layout?.Recalculate();
            _grid?.Invalidate();
        }
        
        /// <summary>
        /// Expands all detail rows
        /// </summary>
        public void ExpandAll()
        {
            // Get all row indices
            if (_grid?.Data?.Rows == null) return;
            
            for (int i = 0; i < _grid.Data.Rows.Count; i++)
            {
                if (!_detailRows.ContainsKey(i))
                {
                    _detailRows[i] = new MasterDetailRow
                    {
                        MasterRowIndex = i,
                        IsExpanded = true,
                        DetailHeight = 200
                    };
                }
                else
                {
                    var detailRow = _detailRows[i];
                    detailRow.IsExpanded = true;
                    ShowDetail(i, detailRow);
                }
            }
            
            _grid?.Layout?.Recalculate();
            _grid?.Invalidate();
        }
        
        /// <summary>
        /// Fires the DetailExpanded event
        /// </summary>
        protected virtual void OnDetailExpanded(int rowIndex)
        {
            DetailExpanded?.Invoke(this, new DetailRowEventArgs(rowIndex));
        }
        
        /// <summary>
        /// Fires the DetailCollapsed event
        /// </summary>
        protected virtual void OnDetailCollapsed(int rowIndex)
        {
            DetailCollapsed?.Invoke(this, new DetailRowEventArgs(rowIndex));
        }
        
        /// <summary>
        /// Disposes the master/detail renderer
        /// </summary>
        public void Dispose()
        {
            // Remove all detail controls
            if (_grid != null)
            {
                foreach (var detailRow in _detailRows.Values)
                {
                    if (detailRow.DetailControl != null && _grid.Controls.Contains(detailRow.DetailControl))
                    {
                        _grid.Controls.Remove(detailRow.DetailControl);
                    }
                }
            }
            
            _detailRows.Clear();
            _grid = null;
        }
    }
    
    /// <summary>
    /// Represents a master/detail row configuration
    /// </summary>
    public class MasterDetailRow
    {
        /// <summary>
        /// Index of the master row
        /// </summary>
        public int MasterRowIndex { get; set; }
        
        /// <summary>
        /// Whether the detail is currently expanded
        /// </summary>
        public bool IsExpanded { get; set; }
        
        /// <summary>
        /// Control to display in the detail area
        /// </summary>
        public Control DetailControl { get; set; }
        
        /// <summary>
        /// Height of the detail area when expanded
        /// </summary>
        public int DetailHeight { get; set; } = 200;
        
        /// <summary>
        /// Custom data associated with this detail row
        /// </summary>
        public object Tag { get; set; }
        
        /// <summary>
        /// Factory function to create detail control on demand
        /// </summary>
        public Func<int, Control> DetailControlFactory { get; set; }
    }
    
    /// <summary>
    /// Event args for detail row events
    /// </summary>
    public class DetailRowEventArgs : EventArgs
    {
        public int RowIndex { get; }
        
        public DetailRowEventArgs(int rowIndex)
        {
            RowIndex = rowIndex;
        }
    }
}

