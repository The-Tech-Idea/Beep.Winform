using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_CertificatesandSSLProperties : uc_DataConnectionPropertiesBaseControl
    {
        private TableLayoutPanel _layout;
        private TextBox _certificatePathTextBox;
        private TextBox _sslModeTextBox;
        private TextBox _sslCertificateTextBox;
        private TextBox _sslKeyTextBox;
        private TextBox _sslRootCertificateTextBox;
        private TextBox _sslCrlTextBox;
        private CheckBox _useSslCheckBox;
        private CheckBox _requireSslCheckBox;
        private CheckBox _trustServerCertificateCheckBox;
        private CheckBox _bypassServerValidationCheckBox;
        private CheckBox _validateCertificateChainCheckBox;

        public uc_CertificatesandSSLProperties()
        {
            InitializeComponent();
            BuildUi();
        }

        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            Text = "Certificate and SSL";
            if (conn == null)
            {
                return;
            }

            _certificatePathTextBox.DataBindings.Clear();
            _certificatePathTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.CertificatePath), true, DataSourceUpdateMode.OnPropertyChanged));

            _useSslCheckBox.DataBindings.Clear();
            _useSslCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.UseSSL), true, DataSourceUpdateMode.OnPropertyChanged));

            _requireSslCheckBox.DataBindings.Clear();
            _requireSslCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.RequireSSL), true, DataSourceUpdateMode.OnPropertyChanged));

            _trustServerCertificateCheckBox.DataBindings.Clear();
            _trustServerCertificateCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.TrustServerCertificate), true, DataSourceUpdateMode.OnPropertyChanged));

            _bypassServerValidationCheckBox.DataBindings.Clear();
            _bypassServerValidationCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.BypassServerCertificateValidation), true, DataSourceUpdateMode.OnPropertyChanged));

            _sslModeTextBox.DataBindings.Clear();
            _sslModeTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.SSLMode), true, DataSourceUpdateMode.OnPropertyChanged));

            _sslCertificateTextBox.DataBindings.Clear();
            _sslCertificateTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ClientCertificatePath), true, DataSourceUpdateMode.OnPropertyChanged));

            _sslKeyTextBox.DataBindings.Clear();
            _sslKeyTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ClientCertificatePassword), true, DataSourceUpdateMode.OnPropertyChanged));

            _sslRootCertificateTextBox.DataBindings.Clear();
            _sslRootCertificateTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ClientCertificateThumbprint), true, DataSourceUpdateMode.OnPropertyChanged));

            _sslCrlTextBox.DataBindings.Clear();
            _sslCrlTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ClientCertificateSubjectName), true, DataSourceUpdateMode.OnPropertyChanged));

            _validateCertificateChainCheckBox.DataBindings.Clear();
            _validateCertificateChainCheckBox.DataBindings.Add(new Binding("Checked", conn, nameof(conn.ValidateServerCertificate), true, DataSourceUpdateMode.OnPropertyChanged));
        }

        private void BuildUi()
        {
            if (_layout != null)
            {
                return;
            }

            _layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                ColumnCount = 2,
                RowCount = 0,
                Padding = new Padding(12)
            };
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _certificatePathTextBox = CreateTextBox();
            _sslModeTextBox = CreateTextBox();
            _sslCertificateTextBox = CreateTextBox();
            _sslKeyTextBox = CreateTextBox();
            _sslRootCertificateTextBox = CreateTextBox();
            _sslCrlTextBox = CreateTextBox();
            _useSslCheckBox = new CheckBox();
            _requireSslCheckBox = new CheckBox();
            _trustServerCertificateCheckBox = new CheckBox();
            _bypassServerValidationCheckBox = new CheckBox();
            _validateCertificateChainCheckBox = new CheckBox();

            AddRow("Certificate Path", _certificatePathTextBox);
            AddRow("SSL Mode", _sslModeTextBox);
            AddRow("SSL Certificate", _sslCertificateTextBox);
            AddRow("SSL Key", _sslKeyTextBox);
            AddRow("SSL Root Certificate", _sslRootCertificateTextBox);
            AddRow("SSL CRL", _sslCrlTextBox);
            AddRow("Use SSL", _useSslCheckBox);
            AddRow("Require SSL", _requireSslCheckBox);
            AddRow("Trust Server Certificate", _trustServerCertificateCheckBox);
            AddRow("Bypass Certificate Validation", _bypassServerValidationCheckBox);
            AddRow("Validate Certificate Chain", _validateCertificateChainCheckBox);

            Controls.Add(_layout);
        }

        private static TextBox CreateTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill
            };
        }

        private void AddRow(string labelText, Control editor)
        {
            int rowIndex = _layout.RowCount++;
            _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true
            };

            editor.Margin = new Padding(0, 2, 0, 8);
            _layout.Controls.Add(label, 0, rowIndex);
            _layout.Controls.Add(editor, 1, rowIndex);
        }
    }
}
