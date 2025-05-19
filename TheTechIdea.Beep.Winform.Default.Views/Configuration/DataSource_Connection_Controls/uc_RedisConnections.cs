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
    public partial class uc_RedisConnections : TemplateUserControl
    {
        RedisConnectionViewModel viewModel;

        public uc_RedisConnections()
        {
            InitializeComponent();
        }

        private void uc_RedisConnections_Load(object sender, EventArgs e)
        {
            Configure();
        }

        public void Configure()
        {
            viewModel = new RedisConnectionViewModel(Editor, VisManager);
            BeepSimpleGrid1.DataSource = viewModel.DataConnections;
            BeepSimpleGrid1.AddColumn("ConnectionName", "Connection Name", 150);
            BeepSimpleGrid1.AddColumn("Database", "Database", 100);
            BeepSimpleGrid1.AddColumn("DriverName", "Driver Name", 150);
            BeepSimpleGrid1.AddColumn("DriverVersion", "Driver Version", 100);
            BeepSimpleGrid1.AddColumn("Url", "URL", 200);

            // Bind controls to ViewModel properties
            textBoxConnectionName.DataBindings.Add("Text", viewModel, "CurrentDataSourceName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxDatabase.DataBindings.Add("Text", viewModel, "Database", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPassword.DataBindings.Add("Text", viewModel, "Password", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxHost.DataBindings.Add("Text", viewModel, "Host", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownPort.DataBindings.Add("Value", viewModel, "Port", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxSsl.DataBindings.Add("Checked", viewModel, "Ssl", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSslCertificate.DataBindings.Add("Text", viewModel, "SslCertificate", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSslKey.DataBindings.Add("Text", viewModel, "SslKey", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSslCaCertificate.DataBindings.Add("Text", viewModel, "SslCaCertificate", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxSslHostnameValidation.DataBindings.Add("Checked", viewModel, "SslHostnameValidation", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownConnectTimeout.DataBindings.Add("Value", viewModel, "ConnectTimeout", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownSyncTimeout.DataBindings.Add("Value", viewModel, "SyncTimeout", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownConnectRetry.DataBindings.Add("Value", viewModel, "ConnectRetry", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownKeepAlive.DataBindings.Add("Value", viewModel, "KeepAlive", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxAbortOnConnectFail.DataBindings.Add("Checked", viewModel, "AbortOnConnectFail", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxClientName.DataBindings.Add("Text", viewModel, "ClientName", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxAllowAdmin.DataBindings.Add("Checked", viewModel, "AllowAdmin", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxHighPriorityThreads.DataBindings.Add("Checked", viewModel, "HighPriorityThreads", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxTieBreaker.DataBindings.Add("Text", viewModel, "TieBreaker", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxConfigurationChannel.DataBindings.Add("Text", viewModel, "ConfigurationChannel", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPasswordFile.DataBindings.Add("Text", viewModel, "PasswordFile", true, DataSourceUpdateMode.OnPropertyChanged);

            // Setup driver type dropdown
            comboBoxDriverType.DataSource = viewModel.RedisDatabaseTypes;
            comboBoxDriverType.DisplayMember = "PackageName";
            comboBoxDriverType.ValueMember = "PackageName";
            comboBoxDriverType.DataBindings.Add("SelectedItem", viewModel, "SelectedRedisDatabaseType", true, DataSourceUpdateMode.OnPropertyChanged);

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
    }
}
