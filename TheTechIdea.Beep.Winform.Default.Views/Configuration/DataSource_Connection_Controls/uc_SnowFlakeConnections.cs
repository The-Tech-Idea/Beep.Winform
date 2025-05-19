using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.MVVM.ViewModels.BeepConfig;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Winform.Controls.Basic;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
    [AddinAttribute(Caption = "Snowflake Connections", Name = "uc_SnowFlakeConnections", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    public partial class uc_SnowFlakeConnections : TemplateUserControl, IAddinVisSchema
    {
        SnowFlakeConnectionViewModel viewModel;

        public uc_SnowFlakeConnections(IServiceProvider services) : base(services)
        {
            InitializeComponent();
        }

        private void uc_SnowFlakeConnections_Load(object sender, EventArgs e)
        {
            Configure();
        }

        public void Configure()
        {
            viewModel = new SnowFlakeConnectionViewModel(Editor, VisManager);

            // Set up the grid with Snowflake connections filter
            BeepSimpleGrid1.DataSource = DMEEditor.ConfigEditor.DataConnections
                .Where(c => c.DatabaseType == DataSourceType.SnowFlake)
                .ToList();

            // Configure grid columns
            BeepSimpleGrid1.ConfigureGrid();

            // Bind properties to controls
            textBoxAccount.DataBindings.Add("Text", viewModel, "Account", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxWarehouse.DataBindings.Add("Text", viewModel, "Warehouse", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxDatabase.DataBindings.Add("Text", viewModel, "Database", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSchema.DataBindings.Add("Text", viewModel, "Schema", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxUserName.DataBindings.Add("Text", viewModel, "UserName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPassword.DataBindings.Add("Text", viewModel, "Password", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxRole.DataBindings.Add("Text", viewModel, "Role", true, DataSourceUpdateMode.OnPropertyChanged);
            comboBoxRegion.DataBindings.Add("Text", viewModel, "Region", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownConnectionTimeout.DataBindings.Add("Value", viewModel, "ConnectionTimeout", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxValidateDefaultParameters.DataBindings.Add("Checked", viewModel, "ValidateDefaultParameters", true, DataSourceUpdateMode.OnPropertyChanged);
            comboBoxAuthenticator.DataBindings.Add("Text", viewModel, "Authenticator", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPrivateKeyFile.DataBindings.Add("Text", viewModel, "PrivateKeyFile", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPrivateKeyFilePwd.DataBindings.Add("Text", viewModel, "PrivateKeyFilePwd", true, DataSourceUpdateMode.OnPropertyChanged);

            // Bind commands to buttons
            buttonSaveConnection.Click += (s, e) => viewModel.SaveConnection();
            buttonTestConnection.Click += (s, e) => viewModel.TestConnection();
            buttonBrowsePrivateKey.Click += (s, e) => viewModel.BrowsePrivateKeyFile();

            // Bind message display
            labelMessage.DataBindings.Add("Text", viewModel, "Message", true, DataSourceUpdateMode.OnPropertyChanged);

            // Set up region dropdown
            comboBoxRegion.Items.AddRange(new string[] {
                "us-west-2", "us-east-1", "eu-west-1", "eu-central-1",
                "ap-southeast-1", "ap-northeast-1", "ca-central-1", "sa-east-1"
            });
            comboBoxRegion.SelectedIndex = 0;

            // Set up authenticator dropdown
            comboBoxAuthenticator.Items.AddRange(new string[] {
                "snowflake", "oauth", "keypair", "externalbrowser"
            });
            comboBoxAuthenticator.SelectedIndex = 0;
        }

        private void BeepSimpleGrid1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && BeepSimpleGrid1.Rows[e.RowIndex].DataBoundItem is ConnectionProperties conn)
            {
                viewModel.ParseConnectionString(conn.ConnectionString);
            }
        }

        public void Run(IPassedArgs pPassedarg)
        {
            // Implementation for running the control
        }

        public void SetConfig(IDMEEditor pDMEEditor, IDM_AddinTree pParentNode, IBranch pBr, IPassedArgs pPassedarg)
        {
            // Implementation for setting configuration
        }

        public IAddinVisSchema GetAddinVisSchema()
        {
            return this;
        }

        public string GetConfigValue(string fieldname)
        {
            return "";
        }

        public void SetConfigValue(string fieldname, string fieldvalue)
        {
            // Implementation for setting config values
        }

        public void OnNavigatedTo(IPassedArgs e)
        {
            // Implementation for navigation
        }

        public void SetDMEditor(IDMEEditor pDMEEditor)
        {
            // Implementation for setting DM editor
        }
    }
}
