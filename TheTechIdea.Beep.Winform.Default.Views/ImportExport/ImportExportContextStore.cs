using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Workflow;
using TheTechIdea.Beep.Workflow.Mapping;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    internal sealed class ImportSelectionContext
    {
        public string SourceDataSourceName { get; set; } = string.Empty;
        public string SourceEntityName { get; set; } = string.Empty;
        public string DestinationDataSourceName { get; set; } = string.Empty;
        public string DestinationEntityName { get; set; } = string.Empty;
        public bool CreateDestinationIfNotExists { get; set; } = true;

        public bool IsValid =>
            !string.IsNullOrWhiteSpace(SourceDataSourceName) &&
            !string.IsNullOrWhiteSpace(SourceEntityName) &&
            !string.IsNullOrWhiteSpace(DestinationDataSourceName) &&
            !string.IsNullOrWhiteSpace(DestinationEntityName);
    }

    internal sealed class ImportFieldMapRow
    {
        public bool Selected { get; set; }
        public string SourceField { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public string DestinationField { get; set; } = string.Empty;
        public string DestinationType { get; set; } = string.Empty;
    }

    internal static class ImportExportParameterKeys
    {
        public const string SourceDataSourceName = "SourceDataSourceName";
        public const string SourceEntityName = "SourceEntityName";
        public const string DestinationDataSourceName = "DestinationDataSourceName";
        public const string DestinationEntityName = "DestinationEntityName";
        public const string CreateDestinationIfNotExists = "CreateDestinationIfNotExists";
        public const string RunImportOnFinish = "RunImportOnFinish";
        public const string Mapping = "Mapping";
    }

    internal static class ImportExportContextStore
    {
        private static readonly object Sync = new();
        private static ImportSelectionContext? _selection;
        private static EntityDataMap? _mapping;

        public static void Reset()
        {
            lock (Sync)
            {
                _selection = null;
                _mapping = null;
            }
        }

        public static void SaveSelection(ImportSelectionContext selection)
        {
            if (selection == null)
            {
                return;
            }

            lock (Sync)
            {
                _selection = CloneSelection(selection);
            }
        }

        public static ImportSelectionContext? GetSelection()
        {
            lock (Sync)
            {
                return _selection == null ? null : CloneSelection(_selection);
            }
        }

        public static void SaveMapping(EntityDataMap? mapping)
        {
            lock (Sync)
            {
                _mapping = mapping;
            }
        }

        public static EntityDataMap? GetMapping()
        {
            lock (Sync)
            {
                return _mapping;
            }
        }

        public static Dictionary<string, object> BuildParameters(ImportSelectionContext selection, EntityDataMap? mapping = null)
        {
            var parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                [ImportExportParameterKeys.SourceDataSourceName] = selection.SourceDataSourceName,
                [ImportExportParameterKeys.SourceEntityName] = selection.SourceEntityName,
                [ImportExportParameterKeys.DestinationDataSourceName] = selection.DestinationDataSourceName,
                [ImportExportParameterKeys.DestinationEntityName] = selection.DestinationEntityName,
                [ImportExportParameterKeys.CreateDestinationIfNotExists] = selection.CreateDestinationIfNotExists
            };

            if (mapping != null)
            {
                parameters[ImportExportParameterKeys.Mapping] = mapping;
            }

            return parameters;
        }

        public static ImportSelectionContext? ParseSelection(Dictionary<string, object>? parameters)
        {
            if (parameters == null)
            {
                return null;
            }

            var context = new ImportSelectionContext
            {
                SourceDataSourceName = GetString(parameters, ImportExportParameterKeys.SourceDataSourceName, "SourceDatasourceName", "SourceDataSource"),
                SourceEntityName = GetString(parameters, ImportExportParameterKeys.SourceEntityName, "SourceEntity"),
                DestinationDataSourceName = GetString(parameters, ImportExportParameterKeys.DestinationDataSourceName, "DestDataSourceName", "DestinationDatasourceName"),
                DestinationEntityName = GetString(parameters, ImportExportParameterKeys.DestinationEntityName, "DestEntityName", "DestinationEntity"),
                CreateDestinationIfNotExists = GetBool(parameters, ImportExportParameterKeys.CreateDestinationIfNotExists, "CreateIfMissing", defaultValue: true)
            };

            return context.IsValid ? context : null;
        }

        public static EntityDataMap? ParseMapping(Dictionary<string, object>? parameters)
        {
            if (parameters == null)
            {
                return null;
            }

            if (parameters.TryGetValue(ImportExportParameterKeys.Mapping, out var mappingObj) && mappingObj is EntityDataMap mapping)
            {
                return mapping;
            }

            return null;
        }

        private static string GetString(Dictionary<string, object> parameters, params string[] keys)
        {
            foreach (var key in keys.Where(k => !string.IsNullOrWhiteSpace(k)))
            {
                if (parameters.TryGetValue(key, out var value) && value != null)
                {
                    return value.ToString() ?? string.Empty;
                }
            }

            return string.Empty;
        }

        private static bool GetBool(Dictionary<string, object> parameters, string key, string alternateKey, bool defaultValue)
        {
            if (parameters.TryGetValue(key, out var value) && value != null)
            {
                if (value is bool b)
                {
                    return b;
                }

                if (bool.TryParse(value.ToString(), out var parsed))
                {
                    return parsed;
                }
            }

            if (parameters.TryGetValue(alternateKey, out var altValue) && altValue != null)
            {
                if (altValue is bool ab)
                {
                    return ab;
                }

                if (bool.TryParse(altValue.ToString(), out var parsedAlt))
                {
                    return parsedAlt;
                }
            }

            return defaultValue;
        }

        private static ImportSelectionContext CloneSelection(ImportSelectionContext selection)
        {
            return new ImportSelectionContext
            {
                SourceDataSourceName = selection.SourceDataSourceName,
                SourceEntityName = selection.SourceEntityName,
                DestinationDataSourceName = selection.DestinationDataSourceName,
                DestinationEntityName = selection.DestinationEntityName,
                CreateDestinationIfNotExists = selection.CreateDestinationIfNotExists
            };
        }
    }
}
