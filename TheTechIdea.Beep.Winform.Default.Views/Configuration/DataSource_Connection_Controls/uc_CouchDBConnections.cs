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
    [AddinAttribute(Caption = "CouchDB Connections", Name = "uc_CouchDBConnections", misc = "Config", menu = "Configuration", addinType = AddinType.Control, displayType = DisplayType.InControl, ObjectType = "Beep")]
    public partial class uc_CouchDBConnections : TemplateUserControl, IAddinVisSchema
    {
        CouchDBConnectionViewModel viewModel;

        public uc_CouchDBConnections(IServiceProvider services) : base(services)
        {
            InitializeComponent();
        }

        private void uc_CouchDBConnections_Load(object sender, EventArgs e)
        {
            Configure();
        }

        public void Configure()
        {
            viewModel = new CouchDBConnectionViewModel(Editor, VisManager);

            // Set up the grid with CouchDB connections filter
            BeepSimpleGrid1.DataSource = DMEEditor.ConfigEditor.DataConnections
                .Where(c => c.DatabaseType == DataSourceType.CouchDB)
                .ToList();

            // Configure grid columns
            BeepSimpleGrid1.ConfigureGrid();

            // Bind properties to controls
            textBoxServerUrl.DataBindings.Add("Text", viewModel, "ServerUrl", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxDatabaseName.DataBindings.Add("Text", viewModel, "DatabaseName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxUserName.DataBindings.Add("Text", viewModel, "UserName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxPassword.DataBindings.Add("Text", viewModel, "Password", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownConnectionTimeout.DataBindings.Add("Value", viewModel, "ConnectionTimeout", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownMaxConnections.DataBindings.Add("Value", viewModel, "MaxConnections", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxEnableSsl.DataBindings.Add("Checked", viewModel, "EnableSsl", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxSslCertificatePath.DataBindings.Add("Text", viewModel, "SslCertificatePath", true, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxIgnoreSslErrors.DataBindings.Add("Checked", viewModel, "IgnoreSslErrors", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxProxyHost.DataBindings.Add("Text", viewModel, "ProxyHost", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownProxyPort.DataBindings.Add("Value", viewModel, "ProxyPort", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxProxyUserName.DataBindings.Add("Text", viewModel, "ProxyUserName", true, DataSourceUpdateMode.OnPropertyChanged);
            textBoxProxyPassword.DataBindings.Add("Text", viewModel, "ProxyPassword", true, DataSourceUpdateMode.OnPropertyChanged);

            // Bind commands to buttons
            buttonSaveConnection.Click += (s, e) => viewModel.SaveConnection();
            buttonTestConnection.Click += async (s, e) => await viewModel.TestConnection();
            buttonBrowseSslCertificate.Click += (s, e) => viewModel.BrowseSslCertificate();

            // Bind message display
            labelMessage.DataBindings.Add("Text", viewModel, "Message", true, DataSourceUpdateMode.OnPropertyChanged);
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
