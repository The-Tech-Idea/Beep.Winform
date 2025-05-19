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
    public partial class uc_CassandraConnections : TemplateUserControl
    {
        CassandraConnectionViewModel viewModel;

        public uc_CassandraConnections()
        {
            InitializeComponent();
        }

        private void uc_CassandraConnections_Load(object sender, EventArgs e)
        {
            Configure();
        }

        public void Configure()
        {
            viewModel = new CassandraConnectionViewModel(Editor, VisManager);
            BeepSimpleGrid1.DataSource = viewModel.DataConnections;
            BeepSimpleGrid1.AddColumn("ConnectionName", "Connection Name", 150);
            BeepSimpleGrid1.AddColumn("Database", "Keyspace", 100);
            BeepSimpleGrid1.AddColumn("UserID", "User ID", 100);
            BeepSimpleGrid1.AddColumn("DriverName", "Driver Name", 150);
            BeepSimpleGrid1.AddColumn("DriverVersion", "Driver Version", 100);
            BeepSimpleGrid1.AddColumn("Url", "URL", 200);

            // Bind controls to ViewModel properties
            textBoxConnectionName.DataBindings.Add("Text", viewModel, "CurrentDataSourceName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxKeyspace.DataBindings.Add("Text", viewModel, "Keyspace", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxUserId.DataBindings.Add("Text", viewModel, "UserId", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPassword.DataBindings.Add("Text", viewModel, "Password", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxContactPoints.DataBindings.Add("Text", viewModel, "ContactPoints", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownPort.DataBindings.Add("Value", viewModel, "Port", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxSsl.DataBindings.Add("Checked", viewModel, "Ssl", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSslCertificate.DataBindings.Add("Text", viewModel, "SslCertificate", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSslKey.DataBindings.Add("Text", viewModel, "SslKey", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSslCaCertificate.DataBindings.Add("Text", viewModel, "SslCaCertificate", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxSslHostnameValidation.DataBindings.Add("Checked", viewModel, "SslHostnameValidation", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownConnectionTimeout.DataBindings.Add("Value", viewModel, "ConnectionTimeout", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownReadTimeout.DataBindings.Add("Value", viewModel, "ReadTimeout", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownMaxConnectionsPerHost.DataBindings.Add("Value", viewModel, "MaxConnectionsPerHost", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownCoreConnectionsPerHost.DataBindings.Add("Value", viewModel, "CoreConnectionsPerHost", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownMaxRequestsPerConnection.DataBindings.Add("Value", viewModel, "MaxRequestsPerConnection", true, DataSourceUpdateMode.OnPropertyChanged);
            comboBoxConsistencyLevel.DataBindings.Add("SelectedItem", viewModel, "ConsistencyLevel", true, DataSourceUpdateMode.OnPropertyChanged);
            comboBoxLoadBalancingPolicy.DataBindings.Add("SelectedItem", viewModel, "LoadBalancingPolicy", true, DataSourceUpdateMode.OnPropertyChanged);
            comboBoxReconnectionPolicy.DataBindings.Add("SelectedItem", viewModel, "ReconnectionPolicy", true, DataSourceUpdateMode.OnPropertyChanged);
            comboBoxRetryPolicy.DataBindings.Add("SelectedItem", viewModel, "RetryPolicy", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxApplicationName.DataBindings.Add("Text", viewModel, "ApplicationName", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxUseCompression.DataBindings.Add("Checked", viewModel, "UseCompression", true, DataSourceUpdateMode.OnPropertyChanged);

            // Setup consistency level dropdown
            comboBoxConsistencyLevel.Items.AddRange(new string[] {
                "ANY", "ONE", "TWO", "THREE", "QUORUM", "LOCAL_QUORUM",
                "EACH_QUORUM", "LOCAL_ONE", "ALL", "SERIAL", "LOCAL_SERIAL"
            });

            // Setup load balancing policy dropdown
            comboBoxLoadBalancingPolicy.Items.AddRange(new string[] {
                "DefaultLoadBalancingPolicy", "RoundRobinPolicy", "DCAwareRoundRobinPolicy",
                "TokenAwarePolicy", "LatencyAwarePolicy"
            });

            // Setup reconnection policy dropdown
            comboBoxReconnectionPolicy.Items.AddRange(new string[] {
                "ExponentialReconnectionPolicy", "ConstantReconnectionPolicy"
            });

            // Setup retry policy dropdown
            comboBoxRetryPolicy.Items.AddRange(new string[] {
                "DefaultRetryPolicy", "DowngradingConsistencyRetryPolicy", "FallthroughRetryPolicy",
                "LoggingRetryPolicy"
            });

            // Setup driver type dropdown
            comboBoxDriverType.DataSource = viewModel.CassandraDatabaseTypes;
            comboBoxDriverType.DisplayMember = "PackageName";
            comboBoxDriverType.ValueMember = "PackageName";
            comboBoxDriverType.DataBindings.Add("SelectedItem", viewModel, "SelectedCassandraDatabaseType", true, DataSourceUpdateMode.OnPropertyChanged);

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
