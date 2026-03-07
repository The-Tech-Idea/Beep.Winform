using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    /// <summary>
    /// Lightweight read-only wrapper over <see cref="BeepRowConfig"/> that
    /// exposes a <see cref="Cells"/> indexer compatible with
    /// <c>grid.CurrentRow?.Cells["ColumnName"]?.Value</c> access patterns.
    /// </summary>
    public sealed class BeepGridCurrentRow
    {
        private readonly BeepRowConfig _row;

        internal BeepGridCurrentRow(BeepRowConfig row) => _row = row;

        /// <summary>Index of this row in the grid's <c>Data.Rows</c> collection, or -1 if unknown.</summary>
        public int Index { get; internal set; } = -1;

        /// <summary>The underlying <see cref="BeepRowConfig"/>.</summary>
        public BeepRowConfig RowConfig => _row;

        /// <summary>The underlying data object for this row (entity / DataRowView / etc.).</summary>
        public object? RowDataObject => _row.RowData;

        /// <summary>
        /// Cell accessor keyed by column name. Returns <see langword="null"/>
        /// when no cell with the given column name exists.
        /// </summary>
        public BeepGridCellAccessor? this[string columnName]
        {
            get
            {
                var cell = _row.Cells.FirstOrDefault(c => c.ColumnName == columnName);
                return cell != null ? new BeepGridCellAccessor(cell) : null;
            }
        }

        /// <summary>
        /// Convenience property that mirrors <c>DataGridView.CurrentRow.Cells</c>
        /// so callers can write <c>CurrentRow.Cells["col"]?.Value</c>.
        /// Returns this same instance (it already supports the string indexer).
        /// </summary>
        public BeepGridCurrentRow Cells => this;

        /// <summary>Cell value helper — same as <c>this[columnName]?.Value</c>.</summary>
        public object? GetCellValue(string columnName) => this[columnName]?.Value;
    }

    /// <summary>
    /// Read-only accessor for a single <see cref="BeepCellConfig"/>,
    /// exposing a <see cref="Value"/> property so callers can write
    /// <c>cell?.Value?.ToString()</c>.
    /// </summary>
    public sealed class BeepGridCellAccessor
    {
        private readonly BeepCellConfig _cell;

        internal BeepGridCellAccessor(BeepCellConfig cell) => _cell = cell;

        /// <summary>Gets the cell value (<see cref="BeepCellConfig.CellValue"/>).</summary>
        public object? Value => _cell.CellValue;

        /// <summary>Gets the underlying <see cref="BeepCellConfig"/>.</summary>
        public BeepCellConfig CellConfig => _cell;
    }
}
