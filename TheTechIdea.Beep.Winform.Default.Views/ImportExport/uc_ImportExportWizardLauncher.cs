using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Default.Views.Template;
using TheTechIdea.Beep.Workflow.Mapping;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    [AddinAttribute(Caption = "Import/Export Wizard", Name = "uc_ImportExportWizardLauncher", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    public class uc_ImportExportWizardLauncher : TemplateUserControl
    {
        private readonly IServiceProvider _services;
        private readonly BeepButton _launchWizardButton;
        private readonly BeepTextBox _logTextBox;

        public uc_ImportExportWizardLauncher(IServiceProvider services) : base(services)
        {
            _services = services;
            Details.AddinName = "Import / Export Wizard";

            _launchWizardButton = new BeepButton
            {
                Dock = DockStyle.Top,
                Height = 34,
                Text = "Start Import / Export Wizard"
            };
            _launchWizardButton.Click += LaunchWizardButton_Click;

            _logTextBox = new BeepTextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            Controls.Add(_logTextBox);
            Controls.Add(_launchWizardButton);
        }

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            AppendLog("Ready. Click the button to run the 3-step import/export wizard.");
        }

        private void LaunchWizardButton_Click(object? sender, EventArgs e)
        {
            LaunchWizard();
        }

        private void LaunchWizard()
        {
            ImportExportContextStore.Reset();

            var selectStep = new uc_Import_SelectDSandEntity(_services);
            var mapStep = new uc_Import_MapFields(_services);
            var runStep = new uc_Import_Run(_services);

            var config = new WizardConfig
            {
                Key = $"ImportExportWizard_{Guid.NewGuid():N}",
                Title = "Import / Export Wizard",
                Description = "Select source/destination, map fields, then run import.",
                Style = WizardStyle.HorizontalStepper,
                ShowProgressBar = true,
                ShowStepList = true,
                AllowBack = true,
                AllowCancel = true,
                ShowInlineErrors = true,
                Steps = new List<WizardStep>
                {
                    new WizardStep
                    {
                        Key = "select",
                        Title = "Select Source/Destination",
                        Description = "Choose data sources and entities.",
                        Content = selectStep
                    },
                    new WizardStep
                    {
                        Key = "map",
                        Title = "Map Fields",
                        Description = "Map source fields to destination fields.",
                        Content = mapStep
                    },
                    new WizardStep
                    {
                        Key = "run",
                        Title = "Review & Run",
                        Description = "Review and run import.",
                        Content = runStep
                    }
                }
            };

            config.OnProgress = (current, total, title) =>
                AppendLog($"Wizard progress: {current}/{total} ({title})");
            config.OnComplete = RunImportFromWizardContext;
            config.OnCancel = _ => AppendLog("Wizard cancelled by user.");

            AppendLog("Opening wizard...");
            var owner = FindForm();
            var result = owner == null
                ? WizardManager.ShowWizard(config)
                : WizardManager.ShowWizard(config, owner);

            AppendLog($"Wizard closed with result: {result}");
        }

        private void RunImportFromWizardContext(WizardContext context)
        {
            var runImport = context.GetValue(ImportExportParameterKeys.RunImportOnFinish, true);
            if (!runImport)
            {
                AppendLog("Run-on-finish disabled. Import was not executed.");
                return;
            }

            if (Editor == null)
            {
                AppendLog("Editor is not available. Cannot run import.");
                return;
            }

            var selection =
                ImportExportContextStore.ParseSelection(context.GetAllData()) ??
                ImportExportContextStore.GetSelection();

            var mapping =
                context.GetValue<EntityDataMap?>(ImportExportParameterKeys.Mapping, null) ??
                ImportExportContextStore.GetMapping();
            var options = ImportExportContextStore.GetOptions();
            options.RunImportOnFinish = runImport;
            options.RunMigrationPreflight = context.GetValue(ImportExportParameterKeys.RunMigrationPreflight, options.RunMigrationPreflight);
            options.AddMissingColumns = context.GetValue(ImportExportParameterKeys.AddMissingColumns, options.AddMissingColumns);
            options.CreateSyncProfileDraft = context.GetValue(ImportExportParameterKeys.CreateSyncProfileDraft, options.CreateSyncProfileDraft);

            if (selection == null || !selection.IsValid)
            {
                AppendLog("Selection is incomplete. Import aborted.");
                return;
            }

            var mappedCount = mapping?.MappedEntities?
                                  .SelectMany(entity => entity.FieldMapping ?? new List<TheTechIdea.Beep.Workflow.Mapping_rep_fields>())
                                  .Count() ?? 0;

            if (mappedCount <= 0)
            {
                AppendLog("No field mappings were provided. Import aborted.");
                return;
            }

            AppendLog("Running import...");

            try
            {
                using var orchestrator = new ImportExportOrchestrator(Editor);
                var request = new ImportExecutionRequest
                {
                    Selection = selection,
                    Mapping = mapping,
                    Options = options
                };
                IProgress<IPassedArgs> progress = new Progress<IPassedArgs>(args =>
                {
                    var message = args?.Messege;
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        AppendLog(message);
                    }
                });
                var result = Task.Run(async () => await orchestrator.ExecuteAsync(request, progress, AppendLog))
                    .GetAwaiter().GetResult().ImportResult;

                AppendLog(result?.Message ?? "Import run completed.");
                if (result?.Flag == Errors.Ok)
                {
                    MessageBox.Show("Import completed successfully.", "Import Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Import failed: {result?.Message}", "Import Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Import failed: {ex.Message}");
                MessageBox.Show($"Import failed: {ex.Message}", "Import Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
    }
}
