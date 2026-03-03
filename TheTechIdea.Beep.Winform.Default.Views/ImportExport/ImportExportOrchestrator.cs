using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Editor.Importing;
using TheTechIdea.Beep.Editor.Importing.Interfaces;
using TheTechIdea.Beep.Editor.Migration;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Workflow;
using TheTechIdea.Beep.Workflow.Mapping;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    /// <summary>Defines how a failed import batch should be handled.</summary>
    internal enum BatchErrorStrategy { Abort, Skip, Retry }

    /// <summary>
    /// Typed run context that can be passed between wizard steps and the orchestrator.
    /// All properties are flat — no dependency on BeepDM source types.
    /// </summary>
    internal sealed class ImportContext
    {
        public string RunId                      { get; set; } = Guid.NewGuid().ToString("N");
        public string SourceEntityName           { get; set; } = string.Empty;
        public string SourceDataSourceName       { get; set; } = string.Empty;
        public string DestinationEntityName      { get; set; } = string.Empty;
        public string DestinationDataSourceName  { get; set; } = string.Empty;
        public bool   CreateDestinationIfNotExists { get; set; } = true;
        public EntityDataMap? Mapping            { get; set; }
        public int    BatchSize                  { get; set; } = 100;
        public bool   RunMigrationPreflight      { get; set; }
        public bool   AddMissingColumns          { get; set; } = true;
        public bool   CreateSyncProfileDraft     { get; set; }
        public BatchErrorStrategy OnBatchError   { get; set; } = BatchErrorStrategy.Abort;
        public int    MaxRetries                 { get; set; } = 3;
    }

    internal sealed class ImportExecutionOptions
    {
        public bool RunMigrationPreflight { get; set; }
        public bool AddMissingColumns { get; set; } = true;
        public bool CreateSyncProfileDraft { get; set; }
        public bool RunImportOnFinish { get; set; } = true;
        public int BatchSize { get; set; } = 100;
        public BatchErrorStrategy OnBatchError { get; set; } = BatchErrorStrategy.Abort;
        public int MaxRetries { get; set; } = 3;
    }

    internal sealed class ImportExecutionRequest
    {
        public ImportSelectionContext Selection { get; set; } = new();
        public EntityDataMap? Mapping { get; set; }
        public ImportExecutionOptions Options { get; set; } = new();
    }

    internal sealed class ImportExecutionResult
    {
        public IErrorsInfo? ImportResult { get; set; }
        public DataSyncSchema? SyncDraft { get; set; }
    }

    internal sealed class ImportExportOrchestrator : IDisposable
    {
        private readonly IDMEEditor _editor;
        private readonly object _syncRoot = new();
        private DataImportManager? _importManager;
        private CancellationTokenSource? _runCancellation;
        private bool _disposed;

        public ImportExportOrchestrator(IDMEEditor editor)
        {
            _editor = editor ?? throw new ArgumentNullException(nameof(editor));
        }

        public async Task<WizardValidationResult> ValidateRequestAsync(ImportExecutionRequest request)
        {
            if (request == null)
            {
                return WizardValidationResult.Error("Import request cannot be null.");
            }

            if (request.Selection == null || !request.Selection.IsValid)
            {
                return WizardValidationResult.Error("Selection is incomplete. Return to step 1.");
            }

            var mappedCount = GetMappingCount(request.Mapping);
            if (mappedCount <= 0)
            {
                return WizardValidationResult.Error("No field mappings found. Return to step 2.");
            }

            try
            {
                using var manager = new DataImportManager(_editor);
                var config = BuildImportConfiguration(manager, request);
                var testResult = await manager.TestImportConfigurationAsync(config).ConfigureAwait(false);
                if (testResult.Flag == Errors.Ok)
                {
                    return WizardValidationResult.Success();
                }

                return WizardValidationResult.Error($"Import validation failed: {testResult.Message}");
            }
            catch (Exception ex)
            {
                return WizardValidationResult.Error($"Import validation failed: {ex.Message}");
            }
        }

        public async Task<ImportExecutionResult> ExecuteAsync(
            ImportExecutionRequest request,
            IProgress<IPassedArgs>? progress,
            Action<string>? log)
        {
            EnsureNotDisposed();

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Selection == null || !request.Selection.IsValid)
            {
                throw new InvalidOperationException("Selection is incomplete.");
            }

            if (GetMappingCount(request.Mapping) <= 0)
            {
                throw new InvalidOperationException("No valid mapping is available.");
            }

            CleanupRunState();

            var manager = new DataImportManager(_editor);
            var cancellation = new CancellationTokenSource();

            lock (_syncRoot)
            {
                _importManager = manager;
                _runCancellation = cancellation;
            }

            IErrorsInfo? importResult = null;
            try
            {
                var config = BuildImportConfiguration(manager, request);

                if (request.Options.RunMigrationPreflight)
                {
                    var migrationResult = RunMigrationPreflight(request, log);
                    if (migrationResult.Flag != Errors.Ok)
                    {
                        return new ImportExecutionResult { ImportResult = migrationResult };
                    }
                }

                importResult = await manager
                    .RunImportAsync(config, progress, cancellation.Token)
                    .ConfigureAwait(false);

                DataSyncSchema? syncDraft = null;
                if (importResult.Flag == Errors.Ok && request.Options.CreateSyncProfileDraft)
                {
                    syncDraft = BuildAndSaveSyncDraft(request);
                    log?.Invoke($"Sync draft profile '{syncDraft.Id}' saved.");
                }

                return new ImportExecutionResult
                {
                    ImportResult = importResult,
                    SyncDraft = syncDraft
                };
            }
            finally
            {
                manager.Dispose();
                cancellation.Dispose();
                CleanupRunState();
            }
        }

        public void Pause()
        {
            EnsureNotDisposed();
            lock (_syncRoot)
            {
                _importManager?.PauseImport();
            }
        }

        public void Resume()
        {
            EnsureNotDisposed();
            lock (_syncRoot)
            {
                _importManager?.ResumeImport();
            }
        }

        public void Cancel()
        {
            EnsureNotDisposed();
            lock (_syncRoot)
            {
                _importManager?.CancelImport();
                _runCancellation?.Cancel();
            }
        }

        public ImportStatus? GetStatus()
        {
            EnsureNotDisposed();
            lock (_syncRoot)
            {
                return _importManager?.GetImportStatus();
            }
        }

        public static int GetMappingCount(EntityDataMap? mapping)
        {
            return mapping?.MappedEntities?
                       .SelectMany(entity => entity.FieldMapping ?? new List<Mapping_rep_fields>())
                       .Count() ?? 0;
        }

        private IErrorsInfo RunMigrationPreflight(ImportExecutionRequest request, Action<string>? log)
        {
            try
            {
                var destDs = _editor.GetDataSource(request.Selection.DestinationDataSourceName);
                var srcDs  = _editor.GetDataSource(request.Selection.SourceDataSourceName);

                if (destDs == null)
                    return CreateError($"Destination datasource '{request.Selection.DestinationDataSourceName}' is not available.");
                if (srcDs == null)
                    return CreateError($"Source datasource '{request.Selection.SourceDataSourceName}' is not available.");

                if (destDs.ConnectionStatus != ConnectionState.Open) destDs.Openconnection();
                if (srcDs.ConnectionStatus  != ConnectionState.Open) srcDs.Openconnection();

                var srcStructure = srcDs.GetEntityStructure(request.Selection.SourceEntityName, false);
                if (srcStructure == null)
                    return CreateError($"Could not load source entity structure '{request.Selection.SourceEntityName}'.");

                srcStructure.EntityName = request.Selection.DestinationEntityName;
                log?.Invoke($"Running migration preflight for '{request.Selection.DestinationEntityName}'.");

                var migMgr = new MigrationManager(_editor, destDs);
                var result = migMgr.EnsureEntity(
                    srcStructure,
                    createIfMissing: request.Selection.CreateDestinationIfNotExists,
                    addMissingColumns: request.Options.AddMissingColumns);

                if (result.Flag == Errors.Ok) log?.Invoke("Migration preflight completed.");
                return result;
            }
            catch (Exception ex)
            {
                return CreateError($"Migration preflight failed: {ex.Message}");
            }
        }

        private DataSyncSchema BuildAndSaveSyncDraft(ImportExecutionRequest request)
        {
            var schema = new DataSyncSchema
            {
                EntityName                = request.Selection.DestinationEntityName,
                SourceEntityName          = request.Selection.SourceEntityName,
                DestinationEntityName     = request.Selection.DestinationEntityName,
                SourceDataSourceName      = request.Selection.SourceDataSourceName,
                DestinationDataSourceName = request.Selection.DestinationDataSourceName,
                SyncType          = "ImportHandoff",
                SyncDirection     = "SourceToDestination",
                SyncStatus        = "Draft",
                SyncStatusMessage = "Generated from Import/Export wizard."
            };

            foreach (var field in request.Mapping?.MappedEntities?
                         .SelectMany(e => e.FieldMapping ?? new List<Mapping_rep_fields>())
                         ?? Enumerable.Empty<Mapping_rep_fields>())
            {
                schema.MappedFields.Add(new FieldSyncData
                {
                    SourceField          = field.FromFieldName,
                    DestinationField     = field.ToFieldName,
                    SourceFieldType      = field.FromFieldType,
                    DestinationFieldType = field.ToFieldType
                });
            }

            using var syncMgr = new DataSyncManager(_editor);
            syncMgr.AddSyncSchema(schema);
            syncMgr.SaveSchemas();
            return schema;
        }

        private static DataImportConfiguration BuildImportConfiguration(
            DataImportManager importManager,
            ImportExecutionRequest request)
        {
            var config = importManager.CreateImportConfiguration(
                request.Selection.SourceEntityName,
                request.Selection.SourceDataSourceName,
                request.Selection.DestinationEntityName,
                request.Selection.DestinationDataSourceName);

            config.CreateDestinationIfNotExists = request.Selection.CreateDestinationIfNotExists;
            config.Mapping = request.Mapping;
            config.BatchSize = request.Options.BatchSize <= 0 ? 100 : request.Options.BatchSize;
            config.ApplyDefaults = true;
            return config;
        }

        private static IErrorsInfo CreateError(string message)
        {
            return new ErrorsInfo
            {
                Flag = Errors.Failed,
                Message = message
            };
        }

        private void CleanupRunState()
        {
            lock (_syncRoot)
            {
                _importManager = null;
                _runCancellation = null;
            }
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ImportExportOrchestrator));
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                Cancel();
            }
            catch
            {
                // Best effort cancellation on dispose.
            }

            _disposed = true;
        }
    }
}
