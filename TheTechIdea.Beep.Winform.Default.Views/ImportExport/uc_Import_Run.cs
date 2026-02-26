using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Default.Views.Template;
using TheTechIdea.Beep.Workflow.Mapping;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    [AddinAttribute(Caption = "Run Import", Name = "uc_Import_Run", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    public partial class uc_Import_Run : TemplateUserControl, IWizardStepContent
    {
        private readonly BeepLabel _titleLabel;
        private readonly BeepLabel _statusLabel;
        private readonly BeepTextBox _summaryTextBox;
        private readonly BeepTextBox _logTextBox;
        private readonly BeepCheckBoxBool _runOnFinishCheckBox;
        private readonly BeepCheckBoxBool _runMigrationCheckBox;
        private readonly BeepCheckBoxBool _addMissingColumnsCheckBox;
        private readonly BeepCheckBoxBool _createSyncDraftCheckBox;
        private readonly BeepButton _startButton;
        private readonly BeepButton _pauseButton;
        private readonly BeepButton _resumeButton;
        private readonly BeepButton _cancelButton;
        private readonly BeepButton _clearLogButton;
        private readonly BeepProgressBar _progressBar;
        private readonly Timer _statusTimer;
        private ImportSelectionContext? _selection;
        private EntityDataMap? _mapping;
        private ImportExecutionOptions _options = new();
        private ImportExportOrchestrator? _orchestrator;
        private bool _isRunning;

        public uc_Import_Run(IServiceProvider services) : base(services)
        {
            InitializeComponent();

            _titleLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(4, 0, 0, 0),
                Text = "Review import settings"
            };

            _statusLabel = new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = 28,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(4, 0, 0, 0),
                Text = "Ready to run import."
            };

            _runOnFinishCheckBox = new BeepCheckBoxBool
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "Run import when I click Finish",
                CurrentValue = true
            };
            _runOnFinishCheckBox.StateChanged += RunOptionChanged;

            _runMigrationCheckBox = new BeepCheckBoxBool
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "Run migration preflight before import",
                CurrentValue = false
            };
            _runMigrationCheckBox.StateChanged += RunOptionChanged;

            _addMissingColumnsCheckBox = new BeepCheckBoxBool
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "Allow migration preflight to add missing columns",
                CurrentValue = true
            };
            _addMissingColumnsCheckBox.StateChanged += RunOptionChanged;

            _createSyncDraftCheckBox = new BeepCheckBoxBool
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "Create BeepSync draft profile after successful import",
                CurrentValue = false
            };
            _createSyncDraftCheckBox.StateChanged += RunOptionChanged;

            _summaryTextBox = new BeepTextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            _logTextBox = new BeepTextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            _progressBar = new BeepProgressBar
            {
                Dock = DockStyle.Top,
                Height = 18,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            _startButton = new BeepButton { Text = "Start", Width = 88, Dock = DockStyle.Left };
            _pauseButton = new BeepButton { Text = "Pause", Width = 88, Dock = DockStyle.Left };
            _resumeButton = new BeepButton { Text = "Resume", Width = 88, Dock = DockStyle.Left };
            _cancelButton = new BeepButton { Text = "Cancel", Width = 88, Dock = DockStyle.Left };
            _clearLogButton = new BeepButton { Text = "Clear Logs", Width = 100, Dock = DockStyle.Right };

            _startButton.Click += StartButton_Click;
            _pauseButton.Click += PauseButton_Click;
            _resumeButton.Click += ResumeButton_Click;
            _cancelButton.Click += CancelButton_Click;
            _clearLogButton.Click += ClearLogButton_Click;

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 34
            };
            buttonPanel.Controls.Add(_clearLogButton);
            buttonPanel.Controls.Add(_cancelButton);
            buttonPanel.Controls.Add(_resumeButton);
            buttonPanel.Controls.Add(_pauseButton);
            buttonPanel.Controls.Add(_startButton);

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 170
            };
            splitContainer.Panel1.Controls.Add(_summaryTextBox);
            splitContainer.Panel2.Controls.Add(_logTextBox);

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };
            panel.Controls.Add(splitContainer);
            panel.Controls.Add(buttonPanel);
            panel.Controls.Add(_progressBar);
            panel.Controls.Add(_createSyncDraftCheckBox);
            panel.Controls.Add(_addMissingColumnsCheckBox);
            panel.Controls.Add(_runMigrationCheckBox);
            panel.Controls.Add(_runOnFinishCheckBox);
            panel.Controls.Add(_statusLabel);
            panel.Controls.Add(_titleLabel);
            Controls.Add(panel);

            _statusTimer = new Timer { Interval = 1000 };
            _statusTimer.Tick += StatusTimer_Tick;
            SetButtonState(isRunning: false, isPaused: false);
            Disposed += (_, _) =>
            {
                _statusTimer.Stop();
                _statusTimer.Dispose();
                _orchestrator?.Dispose();
            };
        }

        public event EventHandler<StepValidationEventArgs>? ValidationStateChanged;

        public bool IsComplete => ValidateStep().IsValid;

        public string NextButtonText => string.Empty;

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            UpdateSummary();
            RaiseValidationState();
        }

        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            UpdateSummary();
            RaiseValidationState();
        }

        public void OnStepEnter(WizardContext context)
        {
            _selection =
                ImportExportContextStore.ParseSelection(context.GetAllData()) ??
                ImportExportContextStore.GetSelection();

            _mapping =
                context.GetValue<EntityDataMap?>(ImportExportParameterKeys.Mapping, null) ??
                ImportExportContextStore.GetMapping();

            _options = ImportExportContextStore.GetOptions();
            _options.RunImportOnFinish = context.GetValue(ImportExportParameterKeys.RunImportOnFinish, _options.RunImportOnFinish);
            _options.RunMigrationPreflight = context.GetValue(ImportExportParameterKeys.RunMigrationPreflight, _options.RunMigrationPreflight);
            _options.AddMissingColumns = context.GetValue(ImportExportParameterKeys.AddMissingColumns, _options.AddMissingColumns);
            _options.CreateSyncProfileDraft = context.GetValue(ImportExportParameterKeys.CreateSyncProfileDraft, _options.CreateSyncProfileDraft);

            _runOnFinishCheckBox.CurrentValue = _options.RunImportOnFinish;
            _runMigrationCheckBox.CurrentValue = _options.RunMigrationPreflight;
            _addMissingColumnsCheckBox.CurrentValue = _options.AddMissingColumns;
            _createSyncDraftCheckBox.CurrentValue = _options.CreateSyncProfileDraft;
            _statusLabel.Text = "Ready to run import.";
            _progressBar.Value = 0;

            UpdateSummary();
            RaiseValidationState();
        }

        public void OnStepLeave(WizardContext context)
        {
            UpdateOptionsFromUi();
            context.SetValue(ImportExportParameterKeys.RunImportOnFinish, _options.RunImportOnFinish);
            context.SetValue(ImportExportParameterKeys.RunMigrationPreflight, _options.RunMigrationPreflight);
            context.SetValue(ImportExportParameterKeys.AddMissingColumns, _options.AddMissingColumns);
            context.SetValue(ImportExportParameterKeys.CreateSyncProfileDraft, _options.CreateSyncProfileDraft);
            ImportExportContextStore.SaveOptions(_options);
        }

        WizardValidationResult IWizardStepContent.Validate()
        {
            return ValidateStep();
        }

        private WizardValidationResult ValidateStep()
        {
            if (_selection == null || !_selection.IsValid)
            {
                return WizardValidationResult.Error("Selection data is incomplete. Return to step 1.");
            }

            var mapCount = GetMappingCount(_mapping);
            if (mapCount <= 0)
            {
                return WizardValidationResult.Error("No valid mappings found. Return to step 2 and map at least one field.");
            }

            return WizardValidationResult.Success();
        }

        public Task<WizardValidationResult> ValidateAsync()
        {
            return Task.FromResult(ValidateStep());
        }

        private void RunOptionChanged(object? sender, EventArgs e)
        {
            UpdateOptionsFromUi();
            RaiseValidationState();
        }

        private void UpdateSummary()
        {
            if (_selection == null)
            {
                _summaryTextBox.Text = "No selection available yet.";
                return;
            }

            var mappingCount = GetMappingCount(_mapping);
            var summary =
                $"Source Data Source: {_selection.SourceDataSourceName}{Environment.NewLine}" +
                $"Source Entity: {_selection.SourceEntityName}{Environment.NewLine}" +
                $"Destination Data Source: {_selection.DestinationDataSourceName}{Environment.NewLine}" +
                $"Destination Entity: {_selection.DestinationEntityName}{Environment.NewLine}" +
                $"Create Destination if Missing: {_selection.CreateDestinationIfNotExists}{Environment.NewLine}" +
                $"Mapped Fields: {mappingCount}{Environment.NewLine}" +
                $"Run On Finish: {_runOnFinishCheckBox.CurrentValue}{Environment.NewLine}" +
                $"Migration Preflight: {_runMigrationCheckBox.CurrentValue}{Environment.NewLine}" +
                $"Add Missing Columns: {_addMissingColumnsCheckBox.CurrentValue}{Environment.NewLine}" +
                $"Create Sync Draft: {_createSyncDraftCheckBox.CurrentValue}";

            _summaryTextBox.Text = summary;
        }

        private static int GetMappingCount(EntityDataMap? mapping)
        {
            return mapping?.MappedEntities?
                       .SelectMany(entity => entity.FieldMapping ?? new List<TheTechIdea.Beep.Workflow.Mapping_rep_fields>())
                       .Count() ?? 0;
        }

        private void RaiseValidationState()
        {
            var result = ValidateStep();
            _statusLabel.Text = result.IsValid
                ? (_isRunning ? "Import is running..." : "Ready to run import.")
                : result.ErrorMessage ?? "Review run settings.";
            ValidationStateChanged?.Invoke(this, new StepValidationEventArgs(result.IsValid, result.ErrorMessage));
        }

        private async void StartButton_Click(object? sender, EventArgs e)
        {
            await ExecuteImportAsync().ConfigureAwait(false);
        }

        private void PauseButton_Click(object? sender, EventArgs e)
        {
            _orchestrator?.Pause();
            _statusLabel.Text = "Import paused.";
            SetButtonState(isRunning: true, isPaused: true);
        }

        private void ResumeButton_Click(object? sender, EventArgs e)
        {
            _orchestrator?.Resume();
            _statusLabel.Text = "Import resumed.";
            SetButtonState(isRunning: true, isPaused: false);
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            _orchestrator?.Cancel();
            AppendLog("Cancellation requested.");
        }

        private void ClearLogButton_Click(object? sender, EventArgs e)
        {
            _logTextBox.Text = string.Empty;
        }

        private void StatusTimer_Tick(object? sender, EventArgs e)
        {
            var status = _orchestrator?.GetStatus();
            if (status == null)
            {
                return;
            }

            if (status.IsPaused)
            {
                _statusLabel.Text = "Import paused.";
                SetButtonState(isRunning: true, isPaused: true);
            }
            else if (status.IsRunning)
            {
                _statusLabel.Text = "Import running...";
                SetButtonState(isRunning: true, isPaused: false);
            }
            else if (status.IsCompleted)
            {
                _statusLabel.Text = status.HasErrors ? "Import completed with errors." : "Import completed successfully.";
            }
        }

        private async Task ExecuteImportAsync()
        {
            var validation = ValidateStep();
            if (!validation.IsValid)
            {
                _statusLabel.Text = validation.ErrorMessage ?? "Cannot run import yet.";
                return;
            }

            if (Editor == null)
            {
                _statusLabel.Text = "Editor is unavailable.";
                return;
            }

            UpdateOptionsFromUi();
            _progressBar.Value = 0;
            _statusLabel.Text = "Starting import...";
            SetButtonState(isRunning: true, isPaused: false);
            _statusTimer.Start();

            _orchestrator ??= new ImportExportOrchestrator(Editor);
            var request = new ImportExecutionRequest
            {
                Selection = _selection ?? new ImportSelectionContext(),
                Mapping = _mapping,
                Options = _options
            };

            IProgress<IPassedArgs> progress = new Progress<IPassedArgs>(args =>
            {
                var message = args?.Messege;
                if (!string.IsNullOrWhiteSpace(message))
                {
                    AppendLog(message);
                    var parsedProgress = TryExtractPercent(message);
                    if (parsedProgress.HasValue)
                    {
                        _progressBar.Value = Math.Max(_progressBar.Minimum, Math.Min(_progressBar.Maximum, parsedProgress.Value));
                    }
                }
            });

            try
            {
                var result = await _orchestrator
                    .ExecuteAsync(request, progress, AppendLog)
                    .ConfigureAwait(true);

                if (result.ImportResult == null)
                {
                    _statusLabel.Text = "Import did not return a result.";
                    return;
                }

                var success = result.ImportResult.Flag == Errors.Ok;
                _statusLabel.Text = success
                    ? "Import completed successfully."
                    : $"Import failed: {result.ImportResult.Message}";
                _progressBar.Value = success ? _progressBar.Maximum : _progressBar.Value;
                AppendLog(result.ImportResult.Message);
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Import failed: {ex.Message}";
                AppendLog($"ERROR: {ex.Message}");
            }
            finally
            {
                _statusTimer.Stop();
                SetButtonState(isRunning: false, isPaused: false);
                RaiseValidationState();
            }
        }

        private void SetButtonState(bool isRunning, bool isPaused)
        {
            _isRunning = isRunning;
            _startButton.Enabled = !isRunning;
            _pauseButton.Enabled = isRunning && !isPaused;
            _resumeButton.Enabled = isRunning && isPaused;
            _cancelButton.Enabled = isRunning;
        }

        private void UpdateOptionsFromUi()
        {
            _options.RunImportOnFinish = _runOnFinishCheckBox.CurrentValue;
            _options.RunMigrationPreflight = _runMigrationCheckBox.CurrentValue;
            _options.AddMissingColumns = _addMissingColumnsCheckBox.CurrentValue;
            _options.CreateSyncProfileDraft = _createSyncDraftCheckBox.CurrentValue;
        }

        private void AppendLog(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            if (_logTextBox.InvokeRequired)
            {
                _logTextBox.BeginInvoke(new Action<string>(AppendLog), message);
                return;
            }

            var line = $"{DateTime.Now:HH:mm:ss} - {message}";
            _logTextBox.Text = string.IsNullOrWhiteSpace(_logTextBox.Text)
                ? line
                : $"{_logTextBox.Text}{Environment.NewLine}{line}";
        }

        private static int? TryExtractPercent(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return null;
            }

            var percentIndex = message.IndexOf('%');
            if (percentIndex <= 0)
            {
                return null;
            }

            var digits = new string(message
                .Take(percentIndex)
                .Reverse()
                .TakeWhile(char.IsDigit)
                .Reverse()
                .ToArray());

            return int.TryParse(digits, out var parsed) ? parsed : null;
        }

    }
}
