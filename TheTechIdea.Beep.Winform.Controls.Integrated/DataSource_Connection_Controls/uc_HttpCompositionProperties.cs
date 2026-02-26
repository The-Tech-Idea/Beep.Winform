using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_HttpCompositionProperties : uc_DataConnectionPropertiesBaseControl
    {
        private TableLayoutPanel _layout;
        private TextBox _basePathTextBox;
        private TextBox _acceptHeaderTextBox;
        private TextBox _contentTypeTextBox;
        private TextBox _defaultHeadersTextBox;
        private TextBox _defaultQueryParamsTextBox;
        private TextBox _userAgentTextBox;
        private CheckBox _useCompressionCheckBox;
        private CheckBox _followRedirectsCheckBox;

        public uc_HttpCompositionProperties()
        {
            InitializeComponent();
            BuildUi();
        }

        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            Text = "Http Composition";
            if (conn == null)
            {
                return;
            }

            _basePathTextBox.DataBindings.Clear();
            _basePathTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Url), true, DataSourceUpdateMode.OnPropertyChanged));

            _acceptHeaderTextBox.DataBindings.Clear();
            _acceptHeaderTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.ApiKeyHeader), true, DataSourceUpdateMode.OnPropertyChanged));

            _contentTypeTextBox.DataBindings.Clear();
            _contentTypeTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.AuthenticationType), true, DataSourceUpdateMode.OnPropertyChanged));

            _defaultHeadersTextBox.DataBindings.Clear();
            _defaultQueryParamsTextBox.DataBindings.Clear();
            _userAgentTextBox.DataBindings.Clear();
            _useCompressionCheckBox.DataBindings.Clear();
            _followRedirectsCheckBox.DataBindings.Clear();

            _defaultHeadersTextBox.ReadOnly = true;
            _defaultQueryParamsTextBox.ReadOnly = true;
            _userAgentTextBox.ReadOnly = true;
            _useCompressionCheckBox.Enabled = false;
            _followRedirectsCheckBox.Enabled = false;
            _defaultHeadersTextBox.Text = "Use Driver tab Parameters";
            _defaultQueryParamsTextBox.Text = "Use Driver tab Parameters";
            _userAgentTextBox.Text = "Use Driver tab Parameters";
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
                Padding = new Padding(12)
            };
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _basePathTextBox = CreateTextBox();
            _acceptHeaderTextBox = CreateTextBox();
            _contentTypeTextBox = CreateTextBox();
            _defaultHeadersTextBox = CreateTextBox(multiline: true, height: 70);
            _defaultQueryParamsTextBox = CreateTextBox(multiline: true, height: 70);
            _userAgentTextBox = CreateTextBox();
            _useCompressionCheckBox = new CheckBox();
            _followRedirectsCheckBox = new CheckBox();

            AddRow("Base Path", _basePathTextBox);
            AddRow("Accept", _acceptHeaderTextBox);
            AddRow("Content Type", _contentTypeTextBox);
            AddRow("Default Headers", _defaultHeadersTextBox);
            AddRow("Default Query Params", _defaultQueryParamsTextBox);
            AddRow("User Agent", _userAgentTextBox);
            AddRow("Use Compression", _useCompressionCheckBox);
            AddRow("Follow Redirects", _followRedirectsCheckBox);

            Controls.Add(_layout);
        }

        private static TextBox CreateTextBox(bool multiline = false, int height = 24)
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = multiline,
                Height = height,
                ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None
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
