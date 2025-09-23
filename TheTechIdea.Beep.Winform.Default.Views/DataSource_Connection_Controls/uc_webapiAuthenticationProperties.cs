using System;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_webapiAuthenticationProperties : uc_DataConnectionPropertiesBaseControl
    {
        public uc_webapiAuthenticationProperties()
        {
            InitializeComponent();
        }
        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            ConnectionPropertytabPage.Text = "Web Api Auth.";
            if (conn == null) return;

            WebApiAuth_AuthTypebeepComboBox.DataBindings.Clear();
            WebApiAuth_ApiKeybeepTextBox.DataBindings.Clear();
            WebApiAuth_ApiKeyHeaderbeepTextBox.DataBindings.Clear();
            WebApiAuth_ClientIdbeepTextBox.DataBindings.Clear();
            WebApiAuth_ClientSecretbeepTextBox.DataBindings.Clear();
            WebApiAuth_AuthUrlbeepTextBox.DataBindings.Clear();
            WebApiAuth_TokenUrlbeepTextBox.DataBindings.Clear();
            WebApiAuth_ScopebeepTextBox.DataBindings.Clear();
            WebApiAuth_GrantTypebeepTextBox.DataBindings.Clear();
            WebApiAuth_RedirectUribeepTextBox.DataBindings.Clear();
            WebApiAuth_AuthCodebeepTextBox.DataBindings.Clear();

            // Populate AuthType enum into combo
            WebApiAuth_AuthTypebeepComboBox.ListItems = Enum.GetValues(typeof(AuthTypeEnum))
                .Cast<AuthTypeEnum>()
                .Select(e => new TheTechIdea.Beep.Winform.Controls.Models.SimpleItem { Text = e.ToString(), Value = e })
                .ToBindingList();

            WebApiAuth_AuthTypebeepComboBox.DataBindings.Add(new Binding("SelectedValue", conn, nameof(conn.AuthType), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApiAuth_ApiKeybeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ApiKey), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApiAuth_ApiKeyHeaderbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ApiKeyHeader), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApiAuth_ClientIdbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ClientId), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApiAuth_ClientSecretbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ClientSecret), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApiAuth_AuthUrlbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.AuthUrl), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApiAuth_TokenUrlbeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.TokenUrl), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApiAuth_ScopebeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Scope), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApiAuth_GrantTypebeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.GrantType), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApiAuth_RedirectUribeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.RedirectUri), true, DataSourceUpdateMode.OnPropertyChanged));
            WebApiAuth_AuthCodebeepTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.AuthCode), true, DataSourceUpdateMode.OnPropertyChanged));
        }
    }
}
