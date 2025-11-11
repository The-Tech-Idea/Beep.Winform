using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Filtering
{
    /// <summary>
    /// BeepFilter - Layout partial class
    /// Contains layout calculation and management logic
    /// </summary>
    public partial class BeepFilter
    {
        #region Layout Properties

        /// <summary>
        /// Gets the content rectangle (excluding borders/padding)
        /// </summary>
        public Rectangle ContentRectangle
        {
            get
            {
                if (_currentLayout != null && !_currentLayout.ContentRect.IsEmpty)
                    return _currentLayout.ContentRect;

                // Fallback to client rectangle with padding
                var padding = 8;
                return new Rectangle(
                    padding,
                    padding,
                    Math.Max(0, ClientRectangle.Width - padding * 2),
                    Math.Max(0, ClientRectangle.Height - padding * 2)
                );
            }
        }

        /// <summary>
        /// Gets the available rectangle for filter content
        /// Takes into account DisplayMode and Position
        /// </summary>
        public Rectangle AvailableFilterRect
        {
            get
            {
                var rect = ClientRectangle;

                // Adjust based on display mode
                if (_displayMode == FilterDisplayMode.Collapsible && !_isExpanded)
                {
                    // Collapsed - only show header
                    var metrics = GetCurrentMetrics();
                    rect.Height = metrics.ConnectorHeight + 8;
                }

                return rect;
            }
        }

        #endregion

        #region Layout Methods

        /// <summary>
        /// Forces a layout recalculation
        /// </summary>
        public void RefreshLayout()
        {
            RecalculateLayout();
            Invalidate();
        }

        /// <summary>
        /// Gets the current painter metrics
        /// </summary>
        private FilterPainterMetrics GetCurrentMetrics()
        {
            if (_activePainter != null)
                return _activePainter.GetMetrics(this);

            return FilterPainterMetrics.DefaultFor(_filterStyle, _currentTheme);
        }

        /// <summary>
        /// Calculates the preferred size for the filter control
        /// </summary>
        public Size GetPreferredSize()
        {
            var metrics = GetCurrentMetrics();
            
            // Base size depends on filter style
            switch (_filterStyle)
            {
                case FilterStyle.TagPills:
                case FilterStyle.QuickSearch:
                case FilterStyle.InlineRow:
                    return new Size(600, metrics.FilterHeight);

                case FilterStyle.SidebarPanel:
                    return new Size(metrics.FilterWidth, 400);

                case FilterStyle.GroupedRows:
                case FilterStyle.QueryBuilder:
                case FilterStyle.AdvancedDialog:
                    // Dynamic height based on number of filters
                    int rows = Math.Max(1, _filterCount);
                    int height = metrics.Padding * 2 + (rows * metrics.RowHeight) + ((rows - 1) * metrics.ItemSpacing);
                    return new Size(600, height);

                case FilterStyle.DropdownMultiSelect:
                    return new Size(400, metrics.FilterHeight);

                default:
                    return new Size(600, metrics.FilterHeight);
            }
        }

        /// <summary>
        /// Auto-resizes the control to fit content
        /// </summary>
        public void AutoSize()
        {
            var preferredSize = GetPreferredSize();
            
            // Only adjust height by default, preserve width
            if (Size.Height != preferredSize.Height)
            {
                Size = new Size(Size.Width, preferredSize.Height);
            }
        }

        /// <summary>
        /// Gets the minimum size for the current filter style
        /// </summary>
        public Size GetMinimumSize()
        {
            var metrics = GetCurrentMetrics();

            switch (_filterStyle)
            {
                case FilterStyle.SidebarPanel:
                    return new Size(200, 200);

                case FilterStyle.TagPills:
                case FilterStyle.QuickSearch:
                case FilterStyle.InlineRow:
                    return new Size(200, metrics.FilterHeight);

                default:
                    return new Size(300, metrics.FilterHeight);
            }
        }

        #endregion

        #region Layout Helpers

        /// <summary>
        /// Calculates tag layout for TagPills style
        /// </summary>
        internal Rectangle[] CalculateTagLayout(Rectangle availableRect, int tagCount)
        {
            if (tagCount <= 0)
                return new Rectangle[0];

            var metrics = GetCurrentMetrics();
            var tags = new Rectangle[tagCount];
            
            int x = availableRect.X + metrics.Padding;
            int y = availableRect.Y + (availableRect.Height - metrics.RowHeight) / 2;
            int tagWidth = 120; // Default tag width
            int tagHeight = metrics.RowHeight;

            for (int i = 0; i < tagCount; i++)
            {
                // Check if tag fits on current row
                if (x + tagWidth > availableRect.Right - metrics.Padding)
                {
                    // Wrap to next row
                    x = availableRect.X + metrics.Padding;
                    y += tagHeight + metrics.ItemSpacing;
                }

                tags[i] = new Rectangle(x, y, tagWidth, tagHeight);
                x += tagWidth + metrics.ItemSpacing;
            }

            return tags;
        }

        /// <summary>
        /// Calculates row layout for GroupedRows/QueryBuilder styles
        /// </summary>
        internal Rectangle[] CalculateRowLayout(Rectangle availableRect, int rowCount, int indentLevel = 0)
        {
            if (rowCount <= 0)
                return new Rectangle[0];

            var metrics = GetCurrentMetrics();
            var rows = new Rectangle[rowCount];
            
            int x = availableRect.X + metrics.Padding + (indentLevel * metrics.GroupIndentation);
            int y = availableRect.Y + metrics.Padding;
            int rowWidth = availableRect.Width - metrics.Padding * 2 - (indentLevel * metrics.GroupIndentation);
            int rowHeight = metrics.RowHeight;

            for (int i = 0; i < rowCount; i++)
            {
                rows[i] = new Rectangle(x, y, rowWidth, rowHeight);
                y += rowHeight + metrics.ItemSpacing;
            }

            return rows;
        }

        #endregion
    }
}
