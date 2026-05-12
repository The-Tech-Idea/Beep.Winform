using System;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Trees.Models
{
    /// <summary>
    /// Event arguments for column header click events.
    /// </summary>
    public class BeepTreeColumnClickEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the column that was clicked.
        /// </summary>
        public BeepTreeColumn Column { get; }

        /// <summary>
        /// Gets the index of the clicked column.
        /// </summary>
        public int ColumnIndex { get; }

        /// <summary>
        /// Gets the new sort direction after the click.
        /// </summary>
        public BeepTreeSortDirection NewSortDirection { get; }

        /// <summary>
        /// Initializes a new instance of the BeepTreeColumnClickEventArgs class.
        /// </summary>
        public BeepTreeColumnClickEventArgs(BeepTreeColumn column, int columnIndex, BeepTreeSortDirection newSortDirection)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            ColumnIndex = columnIndex;
            NewSortDirection = newSortDirection;
        }
    }
}
