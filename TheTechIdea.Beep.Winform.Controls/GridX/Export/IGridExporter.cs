using System.IO;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Supported export formats for BeepGridPro.
    /// </summary>
    public enum GridExportFormat
    {
        Csv,
        Json,
        Html,
        Excel,
        Pdf
    }

    /// <summary>
    /// Contract for grid data exporters.
    /// Implementations must be stateless and thread-safe.
    /// </summary>
    public interface IGridExporter
    {
        /// <summary>
        /// Unique key used to register/resolve the exporter in <see cref="GridExportEngine"/>.
        /// </summary>
        GridExportFormat Format { get; }

        /// <summary>
        /// Human-readable description (used in UI dropdowns).
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Default file extension including the dot (e.g. ".csv").
        /// </summary>
        string FileExtension { get; }

        /// <summary>
        /// Whether this exporter can actually perform exports. Plugin-based exporters
        /// return <c>false</c> when their heavy dependency assembly is not loaded.
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Export grid data to a stream.
        /// </summary>
        /// <param name="grid">Source grid.</param>
        /// <param name="output">Output stream. Caller is responsible for disposal.</param>
        /// <param name="options">Export options (column filtering, row filtering, formatting).</param>
        void Export(BeepGridPro grid, Stream output, ExportOptions? options = null);
    }
}
