using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.VerticalTables.Helpers;
using System.ComponentModel;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.VerticalTables.Painters
{
    public interface IVerticalTablePainter
    {
        /// <summary>
        /// Calculate layout for the columns (items) and their children (rows).
        /// Each SimpleItem = a Column, SimpleItem.Children = Rows in that column.
        /// Store the result in the provided layout helper.
        /// </summary>
        void CalculateLayout(BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, int headerHeight, int rowHeight, int columnWidth, int padding, bool showImage);

        /// <summary>
        /// Paint the columns and rows using the provided layout.
        /// </summary>
        void Paint(Graphics g, Rectangle bounds, BindingList<SimpleItem> columns, VerticalTableLayoutHelper layout, object owner);

        /// <summary>
        /// Called when a cell or column header is selected.
        /// </summary>
        void OnCellSelected(VerticalTableLayoutHelper layout, SimpleItem? item, int columnIndex, int rowIndex);

        /// <summary>
        /// Called when hover changes on a cell or column header.
        /// </summary>
        void OnCellHoverChanged(VerticalTableLayoutHelper layout, int columnIndex, int rowIndex);
    }
}
