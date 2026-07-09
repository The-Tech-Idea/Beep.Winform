using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepMultiSplitter
    {
        #region "Adornment options"

        private bool _showSplitterGrips = true;
        private bool _showCollapseHandles = true;

        /// <summary>Draw subtle themed grip dots on the internal row/column splitter borders.</summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Draw subtle themed grip handles on the resizable splitter borders.")]
        [DefaultValue(true)]
        public bool ShowSplitterGrips
        {
            get => _showSplitterGrips;
            set { _showSplitterGrips = value; _tableLayoutPanel?.Invalidate(); }
        }

        /// <summary>Show a collapse/expand chevron on fixed-size (Absolute) panes such as sidebars/headers.</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show a collapse/expand chevron on fixed-size panes (sidebars, header/footer bars).")]
        [DefaultValue(true)]
        public bool ShowCollapseHandles
        {
            get => _showCollapseHandles;
            set { _showCollapseHandles = value; _tableLayoutPanel?.Invalidate(); }
        }

        #endregion

        #region "Collapse / expand state"

        // Remembers the original (SizeType, size) of a collapsed Absolute pane so it can be restored.
        private readonly Dictionary<int, float> _collapsedColumns = new();
        private readonly Dictionary<int, float> _collapsedRows = new();

        private int CollapsedStripPx => DpiScalingHelper.ScaleValue(14, this);
        private int HandleSize => DpiScalingHelper.ScaleValue(16, this);

        public bool IsColumnCollapsed(int columnIndex) => _collapsedColumns.ContainsKey(columnIndex);
        public bool IsRowCollapsed(int rowIndex) => _collapsedRows.ContainsKey(rowIndex);

        /// <summary>Collapses a fixed-width column to a thin strip, remembering its width for restore.</summary>
        public void CollapseColumn(int columnIndex)
        {
            if (_tableLayoutPanel == null || columnIndex < 0 || columnIndex >= _tableLayoutPanel.ColumnStyles.Count) return;
            if (_collapsedColumns.ContainsKey(columnIndex)) return;
            var style = _tableLayoutPanel.ColumnStyles[columnIndex];
            if (style.SizeType != SizeType.Absolute) return; // only fixed panes collapse
            _collapsedColumns[columnIndex] = style.Width;
            style.Width = CollapsedStripPx;
            _tableLayoutPanel.Invalidate();
        }

        /// <summary>Restores a previously collapsed column to its original width.</summary>
        public void ExpandColumn(int columnIndex)
        {
            if (_tableLayoutPanel == null || !_collapsedColumns.TryGetValue(columnIndex, out float width)) return;
            if (columnIndex < _tableLayoutPanel.ColumnStyles.Count)
                _tableLayoutPanel.ColumnStyles[columnIndex].Width = width;
            _collapsedColumns.Remove(columnIndex);
            _tableLayoutPanel.Invalidate();
        }

        public void ToggleColumnCollapsed(int columnIndex)
        {
            if (IsColumnCollapsed(columnIndex)) ExpandColumn(columnIndex);
            else CollapseColumn(columnIndex);
        }

        /// <summary>Collapses a fixed-height row to a thin strip, remembering its height for restore.</summary>
        public void CollapseRow(int rowIndex)
        {
            if (_tableLayoutPanel == null || rowIndex < 0 || rowIndex >= _tableLayoutPanel.RowStyles.Count) return;
            if (_collapsedRows.ContainsKey(rowIndex)) return;
            var style = _tableLayoutPanel.RowStyles[rowIndex];
            if (style.SizeType != SizeType.Absolute) return;
            _collapsedRows[rowIndex] = style.Height;
            style.Height = CollapsedStripPx;
            _tableLayoutPanel.Invalidate();
        }

        public void ExpandRow(int rowIndex)
        {
            if (_tableLayoutPanel == null || !_collapsedRows.TryGetValue(rowIndex, out float height)) return;
            if (rowIndex < _tableLayoutPanel.RowStyles.Count)
                _tableLayoutPanel.RowStyles[rowIndex].Height = height;
            _collapsedRows.Remove(rowIndex);
            _tableLayoutPanel.Invalidate();
        }

        public void ToggleRowCollapsed(int rowIndex)
        {
            if (IsRowCollapsed(rowIndex)) ExpandRow(rowIndex);
            else CollapseRow(rowIndex);
        }

        #endregion

        #region "Adornment painting + hit testing"

        /// <summary>Subscribes to the table panel's paint pass to overlay grips and collapse handles.</summary>
        private void HookAdornments()
        {
            if (_tableLayoutPanel == null) return;
            _tableLayoutPanel.Paint -= TableLayoutPanel_PaintAdornments;
            _tableLayoutPanel.Paint += TableLayoutPanel_PaintAdornments;
        }

        private void TableLayoutPanel_PaintAdornments(object sender, PaintEventArgs e)
        {
            if (_tableLayoutPanel == null) return;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color gripColor = _currentTheme?.BorderColor ?? Color.FromArgb(180, 180, 180);
            Color handleFore = _currentTheme?.ForeColor ?? Color.FromArgb(90, 90, 90);
            Color handleBack = _currentTheme?.PanelBackColor ?? _currentTheme?.BackColor ?? Color.FromArgb(245, 245, 245);

            var colX = CumulativeBorders(_tableLayoutPanel.GetColumnWidths());
            var rowY = CumulativeBorders(_tableLayoutPanel.GetRowHeights());
            int fullH = _tableLayoutPanel.ClientSize.Height;
            int fullW = _tableLayoutPanel.ClientSize.Width;

            if (_showSplitterGrips)
            {
                using var dot = new SolidBrush(Color.FromArgb(160, gripColor));
                // internal column borders → vertical 3-dot grip centered
                for (int i = 0; i < colX.Count; i++)
                    DrawGripDots(g, dot, colX[i], fullH / 2, vertical: true);
                // internal row borders → horizontal 3-dot grip centered
                for (int i = 0; i < rowY.Count; i++)
                    DrawGripDots(g, dot, fullW / 2, rowY[i], vertical: false);
            }

            if (_showCollapseHandles)
            {
                foreach (var (rect, _, _, collapsed) in GetCollapseHandles())
                    DrawCollapseHandle(g, rect, collapsed, handleFore, handleBack, gripColor);
            }
        }

        /// <summary>Draws three small dots forming a drag grip at a border position.</summary>
        private void DrawGripDots(Graphics g, Brush brush, int x, int y, bool vertical)
        {
            int d = DpiScalingHelper.ScaleValue(2, this);
            int gap = DpiScalingHelper.ScaleValue(4, this);
            for (int k = -1; k <= 1; k++)
            {
                int cx = vertical ? x : x + k * gap;
                int cy = vertical ? y + k * gap : y;
                g.FillEllipse(brush, cx - d / 2, cy - d / 2, d, d);
            }
        }

        /// <summary>Draws a collapse/expand chevron handle (rounded surface + chevron glyph).</summary>
        private void DrawCollapseHandle(Graphics g, Rectangle rect, bool collapsed, Color fore, Color back, Color border)
        {
            using (var path = RoundedRect(rect, DpiScalingHelper.ScaleValue(3, this)))
            {
                using var bg = new SolidBrush(Color.FromArgb(235, back));
                using var pen = new Pen(Color.FromArgb(120, border), 1f);
                g.FillPath(bg, path);
                g.DrawPath(pen, path);
            }

            // Chevron: pointing toward the pane's inner edge (collapse) or outward (expand).
            using var chevron = new Pen(fore, DpiScalingHelper.ScaleValue(1.5f, this))
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };
            int pad = rect.Width / 4;
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            // '‹' when expanded (click to collapse), '›' when collapsed (click to expand)
            int dir = collapsed ? 1 : -1;
            g.DrawLine(chevron, cx - dir * pad / 2, cy - pad, cx + dir * pad / 2, cy);
            g.DrawLine(chevron, cx + dir * pad / 2, cy, cx - dir * pad / 2, cy + pad);
        }

        /// <summary>
        /// Returns the clickable collapse-handle rects for every fixed (Absolute) column/row,
        /// with metadata: (rect, index, isColumn, isCollapsed).
        /// </summary>
        private IEnumerable<(Rectangle rect, int index, bool isColumn, bool collapsed)> GetCollapseHandles()
        {
            if (_tableLayoutPanel == null) yield break;
            int hs = HandleSize;
            int inset = DpiScalingHelper.ScaleValue(2, this);

            // Columns: fixed-width panes get a handle near the top of their inner (right) edge.
            var widths = _tableLayoutPanel.GetColumnWidths();
            int x = 0;
            for (int c = 0; c < widths.Length && c < _tableLayoutPanel.ColumnStyles.Count; c++)
            {
                int right = x + widths[c];
                if (_tableLayoutPanel.ColumnStyles[c].SizeType == SizeType.Absolute)
                {
                    var rect = new Rectangle(right - hs - inset, inset, hs, hs);
                    yield return (rect, c, true, IsColumnCollapsed(c));
                }
                x = right;
            }

            // Rows: fixed-height panes get a handle near the left of their inner (bottom) edge.
            var heights = _tableLayoutPanel.GetRowHeights();
            int y = 0;
            for (int r = 0; r < heights.Length && r < _tableLayoutPanel.RowStyles.Count; r++)
            {
                int bottom = y + heights[r];
                if (_tableLayoutPanel.RowStyles[r].SizeType == SizeType.Absolute)
                {
                    var rect = new Rectangle(inset, bottom - hs - inset, hs, hs);
                    yield return (rect, r, false, IsRowCollapsed(r));
                }
                y = bottom;
            }
        }

        /// <summary>
        /// If <paramref name="localPoint"/> (table-panel client coords) hits a collapse handle,
        /// toggles that pane and returns true. Called from the mouse-down handler before resizing.
        /// </summary>
        private bool TryToggleCollapseAt(Point localPoint)
        {
            if (!_showCollapseHandles) return false;
            foreach (var (rect, index, isColumn, _) in GetCollapseHandles())
            {
                if (rect.Contains(localPoint))
                {
                    if (isColumn) ToggleColumnCollapsed(index);
                    else ToggleRowCollapsed(index);
                    return true;
                }
            }
            return false;
        }

        private static List<int> CumulativeBorders(int[] sizes)
        {
            // Returns the x/y of internal borders only (excludes the trailing outer edge).
            var borders = new List<int>();
            int acc = 0;
            for (int i = 0; i < sizes.Length - 1; i++)
            {
                acc += sizes[i];
                borders.Add(acc);
            }
            return borders;
        }

        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion
    }
}
