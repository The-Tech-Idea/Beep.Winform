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
    [AddinAttribute(Caption = "DB2 Connections", Name = "uc_DB2Connections", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    public partial class uc_DB2Connections : TemplateUserControl, IAddinVisSchema
    {
        DB2ConnectionViewModel viewModel;

        public uc_DB2Connections(IServiceProvider services) : base(services)
        {
            InitializeComponent();
        }

        private void uc_DB2Connections_Load(object sender, EventArgs e)
        {
            Configure();
        }

        public void Configure()
        {
            viewModel = new DB2ConnectionViewModel(Editor, VisManager);

            // Set up the grid with DB2 connections filter
            BeepSimpleGrid1.DataSource = DMEEditor.ConfigEditor.DataConnections
                .Where(c => c.DatabaseType == DataSourceType.DB2)
                .ToList();

            // Configure grid columns
            BeepSimpleGrid1.ConfigureGrid();

            // Bind properties to controls
            textBoxServerName.DataBindings.Add("Text", viewModel, "ServerName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxDatabaseName.DataBindings.Add("Text", viewModel, "DatabaseName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxUserName.DataBindings.Add("Text", viewModel, "UserName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPassword.DataBindings.Add("Text", viewModel, "Password", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownPort.DataBindings.Add("Value", viewModel, "Port", true, DataSourceUpdateMode.OnPropertyChanged);
            comboBoxProtocol.DataBindings.Add("Text", viewModel, "Protocol", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownConnectionTimeout.DataBindings.Add("Value", viewModel, "ConnectionTimeout", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownCommandTimeout.DataBindings.Add("Value", viewModel, "CommandTimeout", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxPooling.DataBindings.Add("Checked", viewModel, "Pooling", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownMinPoolSize.DataBindings.Add("Value", viewModel, "MinPoolSize", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownMaxPoolSize.DataBindings.Add("Value", viewModel, "MaxPoolSize", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxCurrentSchema.DataBindings.Add("Text", viewModel, "CurrentSchema", true, DataSourceUpdateMode.OnPropertyChanged);

            // Bind commands to buttons
            buttonSaveConnection.Click += (s, e) => viewModel.SaveConnection();
            buttonTestConnection.Click += (s, e) => viewModel.TestConnection();

            // Bind message display
            labelMessage.DataBindings.Add("Text", viewModel, "Message", true, DataSourceUpdateMode.OnPropertyChanged);

            // Set up protocol dropdown
            comboBoxProtocol.Items.AddRange(new string[] { "TCPIP", "NamedPipe", "SharedMemory" });
            comboBoxProtocol.SelectedIndex = 0;
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
