using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Default.Views.Template;
using TheTechIdea.Beep.Workflow.Mapping;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    [AddinAttribute(Caption = "Run Import", Name = "uc_Import_Run", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    public partial class uc_Import_Run : TemplateUserControl, IWizardStepContent
    {
        private readonly Label _titleLabel;
        private readonly TextBox _summaryTextBox;
        private readonly CheckBox _runOnFinishCheckBox;
        private ImportSelectionContext? _selection;
        private EntityDataMap? _mapping;

        public uc_Import_Run(IServiceProvider services) : base(services)
        {
            InitializeComponent();

            _titleLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 36,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(4, 0, 0, 0),
                Text = "Review import settings"
            };

            _runOnFinishCheckBox = new CheckBox
            {
                Dock = DockStyle.Top,
                Height = 28,
                Text = "Run import when I click Finish",
                Checked = true
            };
            _runOnFinishCheckBox.CheckedChanged += RunOnFinishCheckBox_CheckedChanged;

            _summaryTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };
            panel.Controls.Add(_summaryTextBox);
            panel.Controls.Add(_runOnFinishCheckBox);
            panel.Controls.Add(_titleLabel);
            Controls.Add(panel);
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

            _runOnFinishCheckBox.Checked = context.GetValue(ImportExportParameterKeys.RunImportOnFinish, true);

            UpdateSummary();
            RaiseValidationState();
        }

        public void OnStepLeave(WizardContext context)
        {
            context.SetValue(ImportExportParameterKeys.RunImportOnFinish, _runOnFinishCheckBox.Checked);
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

        private void RunOnFinishCheckBox_CheckedChanged(object? sender, EventArgs e)
        {
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
                $"Run On Finish: {_runOnFinishCheckBox.Checked}";

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
            ValidationStateChanged?.Invoke(this, new StepValidationEventArgs(result.IsValid, result.ErrorMessage));
        }
    }
}
