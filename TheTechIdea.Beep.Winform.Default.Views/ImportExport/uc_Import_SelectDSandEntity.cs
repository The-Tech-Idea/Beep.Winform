using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Wizards;
using TheTechIdea.Beep.Winform.Default.Views.Template;

namespace TheTechIdea.Beep.Winform.Default.Views.ImportExport
{
    [AddinAttribute(Caption = "Select Datasource and Entity", Name = "uc_Import_SelectDSandEntity", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    public partial class uc_Import_SelectDSandEntity : TemplateUserControl, IWizardStepContent
    {
        private readonly ComboBox _sourceEntityCombo;
        private readonly ComboBox _destinationEntityCombo;
        private readonly Label _sourceEntityLabel;
        private readonly Label _destinationEntityLabel;
        private bool _isInitializing;

        public uc_Import_SelectDSandEntity(IServiceProvider services) : base(services)
        {
            InitializeComponent();

            _sourceEntityLabel = CreateEntityLabel("Source Entity", new System.Drawing.Point(35, 337));
            _sourceEntityCombo = CreateEntityCombo(new System.Drawing.Point(232, 335));
            _sourceEntityCombo.SelectedIndexChanged += EntityCombo_SelectedIndexChanged;

            _destinationEntityLabel = CreateEntityLabel("Destination Entity", new System.Drawing.Point(35, 369));
            _destinationEntityCombo = CreateEntityCombo(new System.Drawing.Point(232, 367));
            _destinationEntityCombo.SelectedIndexChanged += EntityCombo_SelectedIndexChanged;

            Controls.Add(_sourceEntityLabel);
            Controls.Add(_sourceEntityCombo);
            Controls.Add(_destinationEntityLabel);
            Controls.Add(_destinationEntityCombo);

            SourcebeepComboBox.SelectedItemChanged += SourcebeepComboBox_SelectedItemChanged;
            beepComboBox1.SelectedItemChanged += DestinationbeepComboBox_SelectedItemChanged;
            AddSourcebeepButton.Click += AddSourcebeepButton_Click;
            beepCheckBoxBool1.StateChanged += BeepCheckBoxBool1_StateChanged;
            beepCheckBoxBool1.CurrentValue = true;
        }

        public event EventHandler<StepValidationEventArgs>? ValidationStateChanged;

        public bool IsComplete => BuildSelectionFromUi().IsValid;

        public string NextButtonText => string.Empty;

        public override void OnNavigatedTo(Dictionary<string, object> parameters)
        {
            base.OnNavigatedTo(parameters);
            LoadDataSources();
            RaiseValidationState();
        }

        public override void Configure(Dictionary<string, object> settings)
        {
            base.Configure(settings);
            LoadDataSources();
            RaiseValidationState();
        }

        public void OnStepEnter(WizardContext context)
        {
            _isInitializing = true;
            try
            {
                LoadDataSources();

                var selection =
                    ImportExportContextStore.ParseSelection(context.GetAllData()) ??
                    ImportExportContextStore.GetSelection() ??
                    new ImportSelectionContext();

                RestoreSelectionToUi(selection);
            }
            finally
            {
                _isInitializing = false;
            }

            RaiseValidationState();
        }

        public void OnStepLeave(WizardContext context)
        {
            var selection = BuildSelectionFromUi();
            ImportExportContextStore.SaveSelection(selection);

            context.SetValue(ImportExportParameterKeys.SourceDataSourceName, selection.SourceDataSourceName);
            context.SetValue(ImportExportParameterKeys.SourceEntityName, selection.SourceEntityName);
            context.SetValue(ImportExportParameterKeys.DestinationDataSourceName, selection.DestinationDataSourceName);
            context.SetValue(ImportExportParameterKeys.DestinationEntityName, selection.DestinationEntityName);
            context.SetValue(ImportExportParameterKeys.CreateDestinationIfNotExists, selection.CreateDestinationIfNotExists);
        }

        WizardValidationResult IWizardStepContent.Validate()
        {
            return ValidateStep();
        }

        private WizardValidationResult ValidateStep()
        {
            var selection = BuildSelectionFromUi();
            if (string.IsNullOrWhiteSpace(selection.SourceDataSourceName))
            {
                return WizardValidationResult.Error("Select a source data source.");
            }

            if (string.IsNullOrWhiteSpace(selection.SourceEntityName))
            {
                return WizardValidationResult.Error("Select a source entity.");
            }

            if (string.IsNullOrWhiteSpace(selection.DestinationDataSourceName))
            {
                return WizardValidationResult.Error("Select a destination data source.");
            }

            if (string.IsNullOrWhiteSpace(selection.DestinationEntityName))
            {
                return WizardValidationResult.Error("Select a destination entity.");
            }

            return WizardValidationResult.Success();
        }

        public Task<WizardValidationResult> ValidateAsync()
        {
            return Task.FromResult(ValidateStep());
        }

        private static Label CreateEntityLabel(string text, System.Drawing.Point location)
        {
            return new Label
            {
                AutoSize = false,
                Text = text,
                Location = location,
                Size = new System.Drawing.Size(194, 23),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
        }

        private static ComboBox CreateEntityCombo(System.Drawing.Point location)
        {
            return new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = location,
                Size = new System.Drawing.Size(194, 23),
                Anchor = AnchorStyles.None
            };
        }

        private void AddSourcebeepButton_Click(object? sender, EventArgs e)
        {
            LoadDataSources();
            RaiseValidationState();
        }

        private void SourcebeepComboBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (_isInitializing)
            {
                return;
            }

            LoadEntitiesForSelectedSource();
            RaiseValidationState();
        }

        private void DestinationbeepComboBox_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (_isInitializing)
            {
                return;
            }

            LoadEntitiesForSelectedDestination();
            RaiseValidationState();
        }

        private void EntityCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_isInitializing)
            {
                return;
            }

            RaiseValidationState();
        }

        private void BeepCheckBoxBool1_StateChanged(object? sender, EventArgs e)
        {
            RaiseValidationState();
        }

        private void LoadDataSources()
        {
            var sourceNames = GetDataSourceNames();
            var items = new BindingList<SimpleItem>(sourceNames
                .Select(name => new SimpleItem { Text = name, Item = name })
                .ToList());

            SourcebeepComboBox.ListItems = new BindingList<SimpleItem>(items.ToList());
            beepComboBox1.ListItems = new BindingList<SimpleItem>(items.ToList());
        }

        private List<string> GetDataSourceNames()
        {
            var connections = Editor?.ConfigEditor?.DataConnections ?? new List<ConnectionProperties>();

            return connections
                .Select(p => p.ConnectionName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(System.StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .ToList();
        }

        private void LoadEntitiesForSelectedSource()
        {
            var sourceDataSourceName = SourcebeepComboBox.SelectedItem?.Text ?? string.Empty;
            PopulateEntityCombo(_sourceEntityCombo, sourceDataSourceName);
        }

        private void LoadEntitiesForSelectedDestination()
        {
            var destinationDataSourceName = beepComboBox1.SelectedItem?.Text ?? string.Empty;
            PopulateEntityCombo(_destinationEntityCombo, destinationDataSourceName);
        }

        private void PopulateEntityCombo(ComboBox combo, string dataSourceName)
        {
            var currentSelection = combo.SelectedItem?.ToString() ?? string.Empty;
            combo.Items.Clear();

            foreach (var entityName in GetEntityNames(dataSourceName))
            {
                combo.Items.Add(entityName);
            }

            if (!string.IsNullOrWhiteSpace(currentSelection) && combo.Items.Contains(currentSelection))
            {
                combo.SelectedItem = currentSelection;
            }
            else if (combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
        }

        private List<string> GetEntityNames(string dataSourceName)
        {
            if (Editor == null || string.IsNullOrWhiteSpace(dataSourceName))
            {
                return new List<string>();
            }

            try
            {
                var dataSource = Editor.GetDataSource(dataSourceName);
                if (dataSource == null)
                {
                    return new List<string>();
                }

                if (dataSource.ConnectionStatus != ConnectionState.Open)
                {
                    dataSource.Openconnection();
                }

                var entities = dataSource.GetEntitesList()?.ToList() ?? new List<string>();
                if (entities.Count == 0 && dataSource.EntitiesNames != null)
                {
                    entities = dataSource.EntitiesNames.ToList();
                }

                return entities
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(System.StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name)
                    .ToList();
            }
            catch (System.Exception ex)
            {
                Editor.AddLogMessage("ImportExport", $"Error loading entities for '{dataSourceName}': {ex.Message}", System.DateTime.Now, 0, null, Errors.Failed);
                return new List<string>();
            }
        }

        private void RestoreSelectionToUi(ImportSelectionContext selection)
        {
            _isInitializing = true;
            try
            {
                if (!string.IsNullOrWhiteSpace(selection.SourceDataSourceName))
                {
                    SourcebeepComboBox.SelectItemByText(selection.SourceDataSourceName);
                }

                if (!string.IsNullOrWhiteSpace(selection.DestinationDataSourceName))
                {
                    beepComboBox1.SelectItemByText(selection.DestinationDataSourceName);
                }

                LoadEntitiesForSelectedSource();
                LoadEntitiesForSelectedDestination();

                SelectComboItem(_sourceEntityCombo, selection.SourceEntityName);
                SelectComboItem(_destinationEntityCombo, selection.DestinationEntityName);
                beepCheckBoxBool1.CurrentValue = selection.CreateDestinationIfNotExists;
            }
            finally
            {
                _isInitializing = false;
            }
        }

        private static void SelectComboItem(ComboBox combo, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var index = combo.Items.IndexOf(value);
            if (index >= 0)
            {
                combo.SelectedIndex = index;
            }
        }

        private ImportSelectionContext BuildSelectionFromUi()
        {
            return new ImportSelectionContext
            {
                SourceDataSourceName = SourcebeepComboBox.SelectedItem?.Text ?? string.Empty,
                SourceEntityName = _sourceEntityCombo.SelectedItem?.ToString() ?? string.Empty,
                DestinationDataSourceName = beepComboBox1.SelectedItem?.Text ?? string.Empty,
                DestinationEntityName = _destinationEntityCombo.SelectedItem?.ToString() ?? string.Empty,
                CreateDestinationIfNotExists = beepCheckBoxBool1.CurrentValue
            };
        }

        private void RaiseValidationState()
        {
            var result = ValidateStep();
            ValidationStateChanged?.Invoke(this, new StepValidationEventArgs(result.IsValid, result.ErrorMessage));
        }
    }
}
