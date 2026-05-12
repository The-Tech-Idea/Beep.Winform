using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Models
{
    /// <summary>
    /// Cached layout information for a tree node.
    /// Stores pre-calculated positions and sizes to avoid recalculation during rendering.
    /// </summary>
    public struct NodeInfo
    {
        /// <summary>
        /// The SimpleItem this layout corresponds to.
        /// </summary>
        public SimpleItem Item { get; set; }

        /// <summary>
        /// Nesting level (0 = root).
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Y position in content space (before scroll offset).
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Height of this row.
        /// </summary>
        public int RowHeight { get; set; }

        /// <summary>
        /// Width of this row's content.
        /// </summary>
        public int RowWidth { get; set; }

        /// <summary>
        /// Measured text size.
        /// </summary>
        public Size TextSize { get; set; }

        /// <summary>
        /// Full row bounds in content space.
        /// </summary>
        public Rectangle RowRectContent { get; set; }

        /// <summary>
        /// Toggle/expand button rectangle in content space.
        /// </summary>
        public Rectangle ToggleRectContent { get; set; }

        /// <summary>
        /// Checkbox rectangle in content space.
        /// </summary>
        public Rectangle CheckRectContent { get; set; }

        /// <summary>
        /// Icon rectangle in content space.
        /// </summary>
        public Rectangle IconRectContent { get; set; }

        /// <summary>
        /// Text rectangle in content space (single-column mode).
        /// </summary>
        public Rectangle TextRectContent { get; set; }

        /// <summary>
        /// Cell rectangles per column index (multi-column mode).
        /// Key = column index, Value = cell bounds in content space.
        /// </summary>
        public Dictionary<int, Rectangle> CellRects { get; set; }

        /// <summary>
        /// Cell text sizes per column index (multi-column mode).
        /// Key = column index, Value = measured text size.
        /// </summary>
        public Dictionary<int, Size> CellTextSizes { get; set; }

        /// <summary>
        /// Row configuration for custom height and appearance.
        /// </summary>
        public BeepTreeRowConfig RowConfig { get; set; }

        /// <summary>
        /// Cell configurations per column index.
        /// Key = column index, Value = cell configuration.
        /// </summary>
        public Dictionary<int, BeepTreeCellConfig> CellConfigs { get; set; }

        /// <summary>
        /// Legacy bounds property (same as RowRectContent).
        /// </summary>
        public Rectangle Bounds
        {
            get => RowRectContent;
            set => RowRectContent = value;
        }

        /// <summary>
        /// Gets the cell rectangle for the specified column index.
        /// Returns Empty if not found.
        /// </summary>
        public Rectangle GetCellRect(int columnIndex)
        {
            if (CellRects != null && CellRects.TryGetValue(columnIndex, out var rect))
                return rect;
            return Rectangle.Empty;
        }

        /// <summary>
        /// Sets the cell rectangle for the specified column index.
        /// </summary>
        public void SetCellRect(int columnIndex, Rectangle rect)
        {
            CellRects ??= new Dictionary<int, Rectangle>();
            CellRects[columnIndex] = rect;
        }

        /// <summary>
        /// Gets the cell text size for the specified column index.
        /// Returns Empty if not found.
        /// </summary>
        public Size GetCellTextSize(int columnIndex)
        {
            if (CellTextSizes != null && CellTextSizes.TryGetValue(columnIndex, out var size))
                return size;
            return Size.Empty;
        }

        /// <summary>
        /// Sets the cell text size for the specified column index.
        /// </summary>
        public void SetCellTextSize(int columnIndex, Size size)
        {
            CellTextSizes ??= new Dictionary<int, Size>();
            CellTextSizes[columnIndex] = size;
        }
    }
}
