using System;
using System.IO;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Stub exporter for PDF. Registered by default so the format appears in UI menus.
    /// When a real plugin assembly implementing <see cref="IGridExporter"/> with
    /// <see cref="GridExportFormat.Pdf"/> is discovered, it replaces this stub.
    /// </summary>
    public sealed class GridPdfExporterStub : IGridExporter
    {
        public GridExportFormat Format => GridExportFormat.Pdf;
        public string Description => "PDF — install plugin";
        public string FileExtension => ".pdf";
        public bool IsAvailable => false;

        public void Export(BeepGridPro grid, Stream output, ExportOptions? options = null)
        {
            throw new InvalidOperationException(
                "PDF export is not available. " +
                "Install the plugin package 'TheTechIdea.Beep.Winform.Controls.GridX.Export.Pdf' " +
                "and ensure it is loaded in the application domain. " +
                "Call grid.ExportEngine.DiscoverPlugins() after plugin assemblies are loaded.");
        }
    }
}
