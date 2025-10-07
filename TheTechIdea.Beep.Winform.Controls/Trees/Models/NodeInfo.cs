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
        /// Text rectangle in content space.
        /// </summary>
        public Rectangle TextRectContent { get; set; }

        /// <summary>
        /// Legacy bounds property (same as RowRectContent).
        /// </summary>
        public Rectangle Bounds
        {
            get => RowRectContent;
            set => RowRectContent = value;
        }
    }
}
