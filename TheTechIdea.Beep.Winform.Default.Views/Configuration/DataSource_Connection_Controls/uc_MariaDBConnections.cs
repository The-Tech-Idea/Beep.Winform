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
    public partial class uc_MariaDBConnections : TemplateUserControl
    {
        MariaDBConnectionViewModel viewModel;

        public uc_MariaDBConnections()
        {
            InitializeComponent();
        }

        private void uc_MariaDBConnections_Load(object sender, EventArgs e)
        {
            Configure();
        }

        public void Configure()
        {
            viewModel = new MariaDBConnectionViewModel(Editor, VisManager);
            BeepSimpleGrid1.DataSource = viewModel.DataConnections;
            BeepSimpleGrid1.AddColumn("ConnectionName", "Connection Name", 150);
            BeepSimpleGrid1.AddColumn("Database", "Database", 100);
            BeepSimpleGrid1.AddColumn("UserID", "User ID", 100);
            BeepSimpleGrid1.AddColumn("DriverName", "Driver Name", 150);
            BeepSimpleGrid1.AddColumn("DriverVersion", "Driver Version", 100);
            BeepSimpleGrid1.AddColumn("Url", "URL", 200);

            // Bind controls to ViewModel properties
            textBoxConnectionName.DataBindings.Add("Text", viewModel, "CurrentDataSourceName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxDatabase.DataBindings.Add("Text", viewModel, "DatabaseName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxUserId.DataBindings.Add("Text", viewModel, "UserId", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPassword.DataBindings.Add("Text", viewModel, "Password", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxServer.DataBindings.Add("Text", viewModel, "Server", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownPort.DataBindings.Add("Value", viewModel, "Port", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxAllowBatch.DataBindings.Add("Checked", viewModel, "AllowBatch", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxAllowLoadLocalInfile.DataBindings.Add("Checked", viewModel, "AllowLoadLocalInfile", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxAllowPublicKeyRetrieval.DataBindings.Add("Checked", viewModel, "AllowPublicKeyRetrieval", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxAllowUserVariables.DataBindings.Add("Checked", viewModel, "AllowUserVariables", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxAutoEnlist.DataBindings.Add("Checked", viewModel, "AutoEnlist", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxCertificateFile.DataBindings.Add("Text", viewModel, "CertificateFile", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxCertificatePassword.DataBindings.Add("Text", viewModel, "CertificatePassword", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxCertificateStoreLocation.DataBindings.Add("Text", viewModel, "CertificateStoreLocation", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxCertificateThumbprint.DataBindings.Add("Text", viewModel, "CertificateThumbprint", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxCharacterSet.DataBindings.Add("Text", viewModel, "CharacterSet", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownCommandTimeout.DataBindings.Add("Value", viewModel, "CommandTimeout", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownConnectionTimeout.DataBindings.Add("Value", viewModel, "ConnectionTimeout", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxConvertZeroDateTime.DataBindings.Add("Checked", viewModel, "ConvertZeroDateTime", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxEnforcePooledConnection.DataBindings.Add("Checked", viewModel, "EnforcePooledConnection", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxIgnoreCommandTransaction.DataBindings.Add("Checked", viewModel, "IgnoreCommandTransaction", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxIgnorePrepare.DataBindings.Add("Checked", viewModel, "IgnorePrepare", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxInteractiveSession.DataBindings.Add("Checked", viewModel, "InteractiveSession", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxLogging.DataBindings.Add("Checked", viewModel, "Logging", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownMaximumPoolSize.DataBindings.Add("Value", viewModel, "MaximumPoolSize", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownMinimumPoolSize.DataBindings.Add("Value", viewModel, "MinimumPoolSize", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPasswordChar.DataBindings.Add("Text", viewModel, "PasswordChar", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPipeName.DataBindings.Add("Text", viewModel, "PipeName", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxPooling.DataBindings.Add("Checked", viewModel, "Pooling", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownPortNumber.DataBindings.Add("Value", viewModel, "PortNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxProcedureCacheSize.DataBindings.Add("Text", viewModel, "ProcedureCacheSize", true, DataSourceUpdateMode.OnPropertyChanged);
            comboBoxProtocol.DataBindings.Add("SelectedItem", viewModel, "Protocol", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxRespectBinaryFlags.DataBindings.Add("Checked", viewModel, "RespectBinaryFlags", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxServerRsaPublicKeyFile.DataBindings.Add("Text", viewModel, "ServerRsaPublicKeyFile", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSharedMemoryName.DataBindings.Add("Text", viewModel, "SharedMemoryName", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxSslMode.DataBindings.Add("Checked", viewModel, "SslMode", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSslCa.DataBindings.Add("Text", viewModel, "SslCa", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSslCert.DataBindings.Add("Text", viewModel, "SslCert", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSslKey.DataBindings.Add("Text", viewModel, "SslKey", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxTreatTinyAsBoolean.DataBindings.Add("Checked", viewModel, "TreatTinyAsBoolean", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxUseAffectedRows.DataBindings.Add("Checked", viewModel, "UseAffectedRows", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxUseCompression.DataBindings.Add("Checked", viewModel, "UseCompression", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxUseDefaultCommandTimeoutForRead.DataBindings.Add("Checked", viewModel, "UseDefaultCommandTimeoutForRead", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxUsePerformanceMonitor.DataBindings.Add("Checked", viewModel, "UsePerformanceMonitor", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxUserIdChar.DataBindings.Add("Text", viewModel, "UserIdChar", true, DataSourceUpdateMode.OnPropertyChanged);

            // Setup protocol dropdown
            comboBoxProtocol.Items.AddRange(new string[] { "socket", "pipe", "memory", "tcp", "unix" });

            // Setup driver type dropdown
            comboBoxDriverType.DataSource = viewModel.MariaDBDatabaseTypes;
            comboBoxDriverType.DisplayMember = "PackageName";
            comboBoxDriverType.ValueMember = "PackageName";
            comboBoxDriverType.DataBindings.Add("SelectedItem", viewModel, "SelectedMariaDBDatabaseType", true, DataSourceUpdateMode.OnPropertyChanged);

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
