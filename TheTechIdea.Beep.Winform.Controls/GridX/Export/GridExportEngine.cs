using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// Orchestrates grid exports. Holds a registry of <see cref="IGridExporter"/> implementations
    /// and provides convenience methods for exporting to files or streams.
    /// Plugin assemblies are discovered at runtime via <see cref="DiscoverPlugins"/>.
    /// </summary>
    public sealed class GridExportEngine
    {
        private readonly Dictionary<GridExportFormat, IGridExporter> _exporters = new();

        public GridExportEngine()
        {
            // Register built-in exporters
            Register(new GridCsvExporter());
            Register(new GridJsonExporter());
            Register(new GridHtmlExporter());

            // Register stub exporters for plugin-based formats
            // Real plugin implementations discovered via DiscoverPlugins() will replace these.
            Register(new GridExcelExporterStub());
            Register(new GridPdfExporterStub());
        }

        /// <summary>
        /// Scans the current application domain for assemblies containing
        /// <see cref="IGridExporter"/> implementations and registers any that
        /// are not already present (or replace stubs if a real implementation
        /// is found).
        /// </summary>
        /// <remarks>
        /// Call this after plugin assemblies have been loaded (e.g. via MEF,
        /// Assembly.LoadFrom, or a plugin bootstrapper). Failures to instantiate
        /// a discovered exporter (missing dependency) are swallowed and logged
        /// to <see cref="System.Diagnostics.Debug"/>.
        /// </remarks>
        public void DiscoverPlugins()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                IEnumerable<Type> types;
                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types.Where(t => t != null)!;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[GridExportEngine] Skipped assembly '{asm.FullName}': {ex.Message}");
                    continue;
                }

                foreach (var type in types)
                {
                    if (type == null) continue;
                    if (type.IsAbstract || type.IsInterface) continue;
                    if (!typeof(IGridExporter).IsAssignableFrom(type)) continue;

                    // Skip built-in exporters that are already registered
                    if (type == typeof(GridCsvExporter) ||
                        type == typeof(GridJsonExporter) ||
                        type == typeof(GridHtmlExporter) ||
                        type == typeof(GridExcelExporterStub) ||
                        type == typeof(GridPdfExporterStub))
                        continue;

                    try
                    {
                        var instance = (IGridExporter?)Activator.CreateInstance(type);
                        if (instance != null)
                        {
                            // If a real plugin is discovered, replace the stub
                            if (_exporters.TryGetValue(instance.Format, out var existing) && !existing.IsAvailable)
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    $"[GridExportEngine] Replaced stub for '{instance.Format}' with '{type.FullName}'.");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    $"[GridExportEngine] Discovered plugin '{type.FullName}' for '{instance.Format}'.");
                            }
                            _exporters[instance.Format] = instance;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[GridExportEngine] Failed to instantiate '{type.FullName}': {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Register a custom exporter. Replaces any existing exporter for the same format.
        /// </summary>
        public void Register(IGridExporter exporter)
        {
            if (exporter == null) throw new ArgumentNullException(nameof(exporter));
            _exporters[exporter.Format] = exporter;
        }

        /// <summary>
        /// Remove an exporter for the given format.
        /// </summary>
        public bool Unregister(GridExportFormat format)
        {
            return _exporters.Remove(format);
        }

        /// <summary>
        /// Check if an exporter is registered for the format.
        /// </summary>
        public bool IsRegistered(GridExportFormat format)
        {
            return _exporters.ContainsKey(format);
        }

        /// <summary>
        /// Check if an exporter is registered AND available for the format.
        /// </summary>
        public bool IsAvailable(GridExportFormat format)
        {
            return _exporters.TryGetValue(format, out var ex) && ex.IsAvailable;
        }

        /// <summary>
        /// Get the registered exporter for a format.
        /// </summary>
        public IGridExporter? GetExporter(GridExportFormat format)
        {
            _exporters.TryGetValue(format, out var ex);
            return ex;
        }

        /// <summary>
        /// Export grid data to a file path.
        /// </summary>
        public void ExportToFile(BeepGridPro grid, string filePath, GridExportFormat format, ExportOptions? options = null)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path is required.", nameof(filePath));

            if (!_exporters.TryGetValue(format, out var exporter))
                throw new InvalidOperationException($"No exporter registered for format '{format}'.");

            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            exporter.Export(grid, fs, options);
        }

        /// <summary>
        /// Export grid data to a stream.
        /// </summary>
        public void ExportToStream(BeepGridPro grid, Stream output, GridExportFormat format, ExportOptions? options = null)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));
            if (output == null) throw new ArgumentNullException(nameof(output));

            if (!_exporters.TryGetValue(format, out var exporter))
                throw new InvalidOperationException($"No exporter registered for format '{format}'.");

            exporter.Export(grid, output, options);
        }

        /// <summary>
        /// Export grid data to a string (useful for clipboard or in-memory processing).
        /// </summary>
        public string ExportToString(BeepGridPro grid, GridExportFormat format, ExportOptions? options = null)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));

            if (!_exporters.TryGetValue(format, out var exporter))
                throw new InvalidOperationException($"No exporter registered for format '{format}'.");

            using var ms = new MemoryStream();
            exporter.Export(grid, ms, options);
            ms.Position = 0;
            using var reader = new StreamReader(ms);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Get the default file extension for a format (including dot).
        /// </summary>
        public string GetFileExtension(GridExportFormat format)
        {
            return _exporters.TryGetValue(format, out var ex) ? ex.FileExtension : ".txt";
        }

        /// <summary>
        /// Get all registered formats.
        /// </summary>
        public IReadOnlyCollection<GridExportFormat> RegisteredFormats => _exporters.Keys;

        /// <summary>
        /// Get all registered exporters.
        /// </summary>
        public IEnumerable<IGridExporter> RegisteredExporters => _exporters.Values;
    }
}
