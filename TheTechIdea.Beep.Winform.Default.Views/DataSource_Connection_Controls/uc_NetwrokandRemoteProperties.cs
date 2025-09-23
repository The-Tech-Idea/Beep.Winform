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
    public partial class uc_NetwrokandRemoteProperties : uc_DataConnectionPropertiesBaseControl
    {
        public uc_NetwrokandRemoteProperties()
        {
            InitializeComponent();
        }
        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            ConnectionPropertytabPage.Text = "Network";
            if (conn == null) return;

            Network_UrlbeepTextBox.DataBindings.Clear();
            Network_TimeoutbeepTextBox.DataBindings.Clear();
            Network_UseProxybeepCheckBox.DataBindings.Clear();
            Network_ProxyUrlbeepTextBox.DataBindings.Clear();
            Network_ProxyPortbeepTextBox.DataBindings.Clear();
            Network_ProxyUserbeepTextBox.DataBindings.Clear();
            Network_ProxyPasswordbeepTextBox.DataBindings.Clear();
            Network_BypassProxyOnLocalbeepCheckBox.DataBindings.Clear();
            Network_UseDefaultProxyCredentialsbeepCheckBox.DataBindings.Clear();

            Network_UrlbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Url), true, DataSourceUpdateMode.OnPropertyChanged));
            Network_TimeoutbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Timeout), true, DataSourceUpdateMode.OnPropertyChanged));
            Network_UseProxybeepCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.UseProxy), true, DataSourceUpdateMode.OnPropertyChanged));
            Network_ProxyUrlbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ProxyUrl), true, DataSourceUpdateMode.OnPropertyChanged));
            Network_ProxyPortbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ProxyPort), true, DataSourceUpdateMode.OnPropertyChanged));
            Network_ProxyUserbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ProxyUser), true, DataSourceUpdateMode.OnPropertyChanged));
            Network_ProxyPasswordbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ProxyPassword), true, DataSourceUpdateMode.OnPropertyChanged));
            Network_BypassProxyOnLocalbeepCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.BypassProxyOnLocal), true, DataSourceUpdateMode.OnPropertyChanged));
            Network_UseDefaultProxyCredentialsbeepCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.UseDefaultProxyCredentials), true, DataSourceUpdateMode.OnPropertyChanged));
        }
    }
}
