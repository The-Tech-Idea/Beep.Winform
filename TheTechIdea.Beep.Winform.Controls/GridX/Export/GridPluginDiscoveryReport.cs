using System;
using System.Collections.Generic;
using System.Linq;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Export
{
    /// <summary>
    /// One exporter plugin that was found but could not be loaded.
    /// </summary>
    public sealed class GridPluginLoadFailure
    {
        public GridPluginLoadFailure(string assemblyName, string typeName, Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));

            AssemblyName = assemblyName ?? "(unknown assembly)";
            TypeName = typeName;

            // Activator.CreateInstance wraps anything a constructor throws, and the wrapper's
            // message is "Exception has been thrown by the target of an invocation" - which
            // describes the reflection call, not why the plugin would not load. Unwrap it so the
            // message a user reads is the real cause, such as a missing dependency assembly.
            Exception = exception is System.Reflection.TargetInvocationException { InnerException: { } cause }
                ? cause
                : exception;
        }

        /// <summary>The assembly the failure came from.</summary>
        public string AssemblyName { get; }

        /// <summary>
        /// The exporter type that could not be constructed, or <c>null</c> when the assembly
        /// itself could not be scanned and no individual type is implicated.
        /// </summary>
        public string TypeName { get; }

        /// <summary>The failure. Never null.</summary>
        public Exception Exception { get; }

        /// <summary>True when the whole assembly was skipped rather than a single type.</summary>
        public bool IsAssemblyLevel => TypeName == null;

        public override string ToString() =>
            IsAssemblyLevel
                ? $"{AssemblyName} (whole assembly): {Exception.Message}"
                : $"{TypeName}: {Exception.Message}";
    }

    /// <summary>
    /// The outcome of a <see cref="GridExportEngine.DiscoverPlugins"/> scan.
    /// </summary>
    /// <remarks>
    /// Discovery is a public API with no grid in scope, so it hands its outcome back to the caller
    /// rather than raising an event nothing is subscribed to. Returning a report also makes the
    /// failure case impossible to miss by accident: a caller that ignores the return value has made
    /// a choice, where a <c>Debug.WriteLine</c> that Release strips made the choice for them.
    /// </remarks>
    public sealed class GridPluginDiscoveryReport
    {
        private readonly List<IGridExporter> _registered = new();
        private readonly List<GridPluginLoadFailure> _failures = new();

        /// <summary>Exporters discovered and registered by this scan.</summary>
        public IReadOnlyList<IGridExporter> Registered => _registered;

        /// <summary>Plugins that were found but could not be loaded.</summary>
        public IReadOnlyList<GridPluginLoadFailure> Failures => _failures;

        /// <summary>True when at least one plugin was found but could not be loaded.</summary>
        public bool HasFailures => _failures.Count > 0;

        internal void AddRegistered(IGridExporter exporter) => _registered.Add(exporter);

        internal void AddFailure(string assemblyName, string typeName, Exception exception) =>
            _failures.Add(new GridPluginLoadFailure(assemblyName, typeName, exception));

        /// <summary>
        /// A one-line summary suitable for a status bar or log.
        /// </summary>
        public override string ToString()
        {
            if (_registered.Count == 0 && _failures.Count == 0)
                return "No exporter plugins found.";

            var parts = new List<string>();
            if (_registered.Count > 0)
                parts.Add($"{_registered.Count} exporter plugin(s) registered: "
                          + string.Join(", ", _registered.Select(e => e.Format.ToString())));
            if (_failures.Count > 0)
                parts.Add($"{_failures.Count} failed to load: "
                          + string.Join("; ", _failures.Select(f => f.ToString())));
            return string.Join(". ", parts) + ".";
        }
    }
}
