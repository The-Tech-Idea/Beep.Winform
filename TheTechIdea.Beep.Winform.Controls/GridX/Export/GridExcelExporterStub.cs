using System;
using System.IO;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Stub exporter for Excel (.xlsx). Registered by default so the format appears in UI menus.
    /// When a real plugin assembly implementing <see cref="IGridExporter"/> with
    /// <see cref="GridExportFormat.Excel"/> is discovered, it replaces this stub.
    /// </summary>
    public sealed class GridExcelExporterStub : IGridExporter
    {
        public GridExportFormat Format => GridExportFormat.Excel;
        public string Description => "Excel (.xlsx) — install plugin";
        public string FileExtension => ".xlsx";
        public bool IsAvailable => false;

        public void Export(BeepGridPro grid, Stream output, ExportOptions? options = null)
        {
            throw new InvalidOperationException(
                "Excel export is not available. " +
                "Install the plugin package 'TheTechIdea.Beep.Winform.Controls.GridX.Export.Excel' " +
                "and ensure it is loaded in the application domain. " +
                "Call grid.ExportEngine.DiscoverPlugins() after plugin assemblies are loaded.");
        }
    }
}
