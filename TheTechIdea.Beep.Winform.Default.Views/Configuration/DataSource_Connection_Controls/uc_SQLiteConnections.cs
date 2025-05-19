using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.DriversConfigurations;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class uc_SQLiteConnections : TemplateUserControl
    {
        SQLiteConnectionViewModel viewModel;

        public uc_SQLiteConnections()
        {
            InitializeComponent();
        }

        private void uc_SQLiteConnections_Load(object sender, EventArgs e)
        {
            Configure();
        }

        public void Configure()
        {
            viewModel = new SQLiteConnectionViewModel(Editor, VisManager);
            BeepSimpleGrid1.DataSource = viewModel.DataConnections;
            BeepSimpleGrid1.AddColumn("ConnectionName", "Connection Name", 150);
            BeepSimpleGrid1.AddColumn("Database", "Database", 100);
            BeepSimpleGrid1.AddColumn("DriverName", "Driver Name", 150);
            BeepSimpleGrid1.AddColumn("DriverVersion", "Driver Version", 100);
            BeepSimpleGrid1.AddColumn("Url", "URL", 200);

            // Bind controls to ViewModel properties
            textBoxConnectionName.DataBindings.Add("Text", viewModel, "CurrentDataSourceName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxDatabaseFilePath.DataBindings.Add("Text", viewModel, "DatabaseFilePath", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPassword.DataBindings.Add("Text", viewModel, "Password", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxUseUri.DataBindings.Add("Checked", viewModel, "UseUri", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxReadOnly.DataBindings.Add("Checked", viewModel, "ReadOnly", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxForeignKeys.DataBindings.Add("Checked", viewModel, "ForeignKeys", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxRecursiveTriggers.DataBindings.Add("Checked", viewModel, "RecursiveTriggers", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownCacheSize.DataBindings.Add("Value", viewModel, "CacheSize", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownPageSize.DataBindings.Add("Value", viewModel, "PageSize", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownMaxPageCount.DataBindings.Add("Value", viewModel, "MaxPageCount", true, DataSourceUpdateMode.OnPropertyChanged);
            comboBoxJournalMode.DataBindings.Add("SelectedItem", viewModel, "JournalMode", true, DataSourceUpdateMode.OnPropertyChanged);
            comboBoxSynchronous.DataBindings.Add("SelectedItem", viewModel, "Synchronous", true, DataSourceUpdateMode.OnPropertyChanged);
            comboBoxTempStore.DataBindings.Add("SelectedItem", viewModel, "TempStore", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownBusyTimeout.DataBindings.Add("Value", viewModel, "BusyTimeout", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxFailIfMissing.DataBindings.Add("Checked", viewModel, "FailIfMissing", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxPoolConnection.DataBindings.Add("Checked", viewModel, "PoolConnection", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownMaxPoolSize.DataBindings.Add("Value", viewModel, "MaxPoolSize", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownMinPoolSize.DataBindings.Add("Value", viewModel, "MinPoolSize", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownPoolLifetime.DataBindings.Add("Value", viewModel, "PoolLifetime", true, DataSourceUpdateMode.OnPropertyChanged);

            // Setup journal mode dropdown
            comboBoxJournalMode.Items.AddRange(new string[] { "DELETE", "TRUNCATE", "PERSIST", "MEMORY", "WAL", "OFF" });

            // Setup synchronous dropdown
            comboBoxSynchronous.Items.AddRange(new string[] { "FULL", "NORMAL", "FAST", "EXTRA", "OFF" });

            // Setup temp store dropdown
            comboBoxTempStore.Items.AddRange(new string[] { "DEFAULT", "FILE", "MEMORY" });

            // Setup driver type dropdown
            comboBoxDriverType.DataSource = viewModel.SqliteDatabaseTypes;
            comboBoxDriverType.DisplayMember = "PackageName";
            comboBoxDriverType.ValueMember = "PackageName";
            comboBoxDriverType.DataBindings.Add("SelectedItem", viewModel, "SelectedSQLiteDatabaseType", true, DataSourceUpdateMode.OnPropertyChanged);

            // Setup package dropdown
            comboBoxPackage.DataSource = viewModel.PackageNames;
            comboBoxPackage.DataBindings.Add("SelectedItem", viewModel, "Selectedpackage", true, DataSourceUpdateMode.OnPropertyChanged);

            // Setup version dropdown
            comboBoxVersion.DataSource = viewModel.PackageVersions;
            comboBoxVersion.DataBindings.Add("SelectedItem", viewModel, "Selectedversion", true, DataSourceUpdateMode.OnPropertyChanged);

            // Bind buttons to commands
            buttonSave.DataBindings.Add("Command", viewModel, "SaveConnectionCommand", true);
            buttonTest.DataBindings.Add("Command", viewModel, "TestConnectionCommand", true);
            buttonNew.DataBindings.Add("Command", viewModel, "CreateNewConnectionCommand", true);
            buttonLoad.DataBindings.Add("Command", viewModel, "LoadConnectionCommand", true);
            buttonBrowse.DataBindings.Add("Command", viewModel, "BrowseDatabaseFileCommand", true);

            // Setup grid selection
            BeepSimpleGrid1.CellClick += BeepSimpleGrid1_CellClick;
            BeepSimpleGrid1.SelectionChanged += BeepSimpleGrid1_SelectionChanged;

            // Handle driver type changes
            comboBoxDriverType.SelectedIndexChanged += ComboBoxDriverType_SelectedIndexChanged;
        }

        private void BeepSimpleGrid1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                viewModel.Selectedconnectionidx = e.RowIndex;
                viewModel.LoadConnection();
            }
        }

        private void BeepSimpleGrid1_SelectionChanged(object sender, EventArgs e)
        {
            if (BeepSimpleGrid1.SelectedRows.Count > 0)
            {
                viewModel.Selectedconnectionidx = BeepSimpleGrid1.SelectedRows[0].Index;
                viewModel.LoadConnection();
            }
        }

        private void ComboBoxDriverType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDriverType.SelectedItem is ConnectionDriversConfig selectedDriver)
            {
                // Update version dropdown based on selected driver
                var versions = Editor.ConfigEditor.DataDriversClasses
                    .Where(x => x.PackageName == selectedDriver.PackageName)
                    .Select(x => x.version)
                    .Distinct()
                    .ToList();

                comboBoxVersion.DataSource = versions;
                if (versions.Count > 0)
                {
                    comboBoxVersion.SelectedIndex = 0;
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            viewModel.SaveConnection();
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            viewModel.TestConnection();
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            viewModel.CreateNewConnection();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            viewModel.LoadConnection();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            viewModel.BrowseDatabaseFile();
        }
    }
}
