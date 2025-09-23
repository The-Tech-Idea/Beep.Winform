using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_WebApiProperties : uc_DataConnectionPropertiesBaseControl
    {
        public uc_WebApiProperties()
        {
            InitializeComponent();
        }
        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            ConnectionPropertytabPage.Text = "Web Api Prop.";
            if (conn == null) return;

            WebApi_HttpMethodbeepTextBox.DataBindings.Clear();
            WebApi_TimeoutMsbeepTextBox.DataBindings.Clear();
            WebApi_MaxRetriesbeepTextBox.DataBindings.Clear();
            WebApi_RetryIntervalMsbeepTextBox.DataBindings.Clear();
            WebApi_IgnoreSSLErrorsbeepCheckBox.DataBindings.Clear();
            WebApi_ValidateServerCertificatebeepCheckBox.DataBindings.Clear();
            WebApi_RequiresAuthenticationbeepCheckBox.DataBindings.Clear();
            WebApi_RequiresTokenRefreshbeepCheckBox.DataBindings.Clear();

            WebApi_HttpMethodbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.HttpMethod), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApi_TimeoutMsbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.TimeoutMs), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApi_MaxRetriesbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.MaxRetries), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApi_RetryIntervalMsbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.RetryIntervalMs), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApi_IgnoreSSLErrorsbeepCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.IgnoreSSLErrors), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApi_ValidateServerCertificatebeepCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.ValidateServerCertificate), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApi_RequiresAuthenticationbeepCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.RequiresAuthentication), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApi_RequiresTokenRefreshbeepCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.RequiresTokenRefresh), true, DataSourceUpdateMode.OnPropertyChanged));
        }
    }
}
