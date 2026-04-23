using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Virtualization
{
    /// <summary>
    /// Contract for a data source that materializes rows on-demand.
    /// Used by <see cref="GridRowVirtualizer"/> to support large datasets
    /// without loading all rows into memory.
    /// </summary>
    public interface IVirtualDataSource
    {
        /// <summary>
        /// Total number of rows available in the underlying data store.
        /// </summary>
        long TotalRowCount { get; }

        /// <summary>
        /// Materialize a contiguous range of rows.
        /// </summary>
        /// <param name="startIndex">Zero-based start index.</param>
        /// <param name="count">Number of rows requested.</param>
        /// <returns>Materialized rows (may be fewer than <paramref name="count"/> near the end).</returns>
        IEnumerable<VirtualRowData> GetRows(long startIndex, int count);

        /// <summary>
        /// Optional: pre-fetch a window of rows before/after the visible range.
        /// Called by the virtualizer after every scroll operation.
        /// </summary>
        void PreloadWindow(long startIndex, int visibleCount, int overscan);

        /// <summary>
        /// Event raised when the total row count changes (e.g., after filtering).
        /// </summary>
        event EventHandler? TotalRowCountChanged;
    }

    /// <summary>
    /// A single materialized row from a virtual data source.
    /// </summary>
    public sealed class VirtualRowData
    {
        /// <summary>
        /// Zero-based index in the full dataset.
        /// </summary>
        public long Index { get; set; }

        /// <summary>
        /// Cell values keyed by column name.
        /// </summary>
        public Dictionary<string, object?> Values { get; set; } = new Dictionary<string, object?>();

        /// <summary>
        /// Optional reference to the original underlying data object.
        /// Used for editing and property change tracking.
        /// </summary>
        public object? OriginalItem { get; set; }
    }
}
