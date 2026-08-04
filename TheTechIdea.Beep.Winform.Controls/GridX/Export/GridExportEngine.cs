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
        private IReadOnlyList<GridPluginLoadFailure> _loadFailures = Array.Empty<GridPluginLoadFailure>();

        /// <summary>
        /// Plugins found by the most recent <see cref="DiscoverPlugins"/> call that could not be
        /// loaded. Empty until discovery has run.
        /// </summary>
        /// <remarks>
        /// Retained so an export can explain itself. Without this, a plugin that was present but
        /// failed to construct left its stub registered, and the user was told to call
        /// <see cref="DiscoverPlugins"/> - the one thing they had already done.
        /// </remarks>
        public IReadOnlyList<GridPluginLoadFailure> LoadFailures => _loadFailures;

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
        /// Call this after plugin assemblies have been loaded (e.g. via MEF, Assembly.LoadFrom, or
        /// a plugin bootstrapper).
        ///
        /// Anything reaching the constructor below has already passed the concrete-and-assignable
        /// filter, so a failure there is a real exporter that would not load - a missing dependency,
        /// a throwing constructor - not a candidate correctly rejected. The returned report carries
        /// those failures, and <see cref="LoadFailures"/> retains them so a later export can say why
        /// its format is unavailable.
        /// </remarks>
        /// <returns>What was registered, and what was found but failed to load.</returns>
        public GridPluginDiscoveryReport DiscoverPlugins()
        {
            var report = new GridPluginDiscoveryReport();

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
                    // A whole assembly that will not enumerate hides every exporter inside it, so
                    // it is recorded rather than written to a channel Release strips.
                    report.AddFailure(asm.FullName, null, ex);
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
                            report.AddRegistered(instance);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Absorbed so one broken plugin cannot stop the rest being discovered, but
                        // recorded: this type is a concrete IGridExporter - it passed the filter
                        // above - so failing here means a real exporter did not load, and its
                        // format silently keeps the stub that says "install the plugin".
                        report.AddFailure(type.Assembly?.FullName, type.FullName, ex);
                    }
                }
            }

            _loadFailures = report.Failures;
            return report;
        }

        /// <summary>
        /// Resolve the exporter for a format, or explain why it cannot be used.
        /// </summary>
        /// <remarks>
        /// Checked before any output is opened. <see cref="ExportToFile"/> previously created the
        /// file first and let the stub throw afterwards, which left an empty .xlsx on disk from an
        /// export that never happened.
        /// </remarks>
        private IGridExporter ResolveExporter(GridExportFormat format)
        {
            if (!_exporters.TryGetValue(format, out var exporter))
                throw new InvalidOperationException($"No exporter registered for format '{format}'.");

            if (exporter.IsAvailable || _loadFailures.Count == 0)
                return exporter;

            // The format is served by a stub while plugins that were present failed to load. The
            // stub's own message tells the user to run discovery, which is exactly what surfaced
            // these failures - so say what actually happened instead.
            var detail = string.Join("; ", _loadFailures.Select(f => f.ToString()));
            throw new InvalidOperationException(
                $"'{format}' export is unavailable, and {_loadFailures.Count} exporter plugin(s) "
                + $"found during discovery failed to load: {detail}",
                _loadFailures.Count == 1
                    ? _loadFailures[0].Exception
                    : new AggregateException(_loadFailures.Select(f => f.Exception)));
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

            var exporter = ResolveExporter(format);

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

            var exporter = ResolveExporter(format);

            exporter.Export(grid, output, options);
        }

        /// <summary>
        /// Export grid data to a string (useful for clipboard or in-memory processing).
        /// </summary>
        public string ExportToString(BeepGridPro grid, GridExportFormat format, ExportOptions? options = null)
        {
            if (grid == null) throw new ArgumentNullException(nameof(grid));

            var exporter = ResolveExporter(format);

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
