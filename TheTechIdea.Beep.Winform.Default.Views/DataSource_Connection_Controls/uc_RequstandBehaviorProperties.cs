using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_RequstandBehaviorProperties : uc_DataConnectionPropertiesBaseControl
    {
        private TableLayoutPanel _layout;
        private TextBox _timeoutTextBox;
        private TextBox _maxRetriesTextBox;
        private TextBox _retryIntervalTextBox;
        private TextBox _connectionTimeoutTextBox;
        private TextBox _commandTimeoutTextBox;
        private TextBox _minPoolSizeTextBox;
        private TextBox _maxPoolSizeTextBox;
        private CheckBox _poolingCheckBox;
        private CheckBox _keepAliveCheckBox;

        public uc_RequstandBehaviorProperties()
        {
            InitializeComponent();
            BuildUi();
        }

        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            Text = "Request and Behavior";
            if (conn == null)
            {
                return;
            }

            _timeoutTextBox.DataBindings.Clear();
            _maxRetriesTextBox.DataBindings.Clear();
            _retryIntervalTextBox.DataBindings.Clear();

            _timeoutTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.TimeoutMs), true, DataSourceUpdateMode.OnPropertyChanged));
            _maxRetriesTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.MaxRetries), true, DataSourceUpdateMode.OnPropertyChanged));
            _retryIntervalTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.RetryIntervalMs), true, DataSourceUpdateMode.OnPropertyChanged));

            _connectionTimeoutTextBox.DataBindings.Clear();
            _connectionTimeoutTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Timeout), true, DataSourceUpdateMode.OnPropertyChanged));

            _commandTimeoutTextBox.DataBindings.Clear();
            _commandTimeoutTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.TimeoutMs), true, DataSourceUpdateMode.OnPropertyChanged));

            _minPoolSizeTextBox.DataBindings.Clear();
            _maxPoolSizeTextBox.DataBindings.Clear();
            _poolingCheckBox.DataBindings.Clear();
            _keepAliveCheckBox.DataBindings.Clear();

            _minPoolSizeTextBox.ReadOnly = true;
            _maxPoolSizeTextBox.ReadOnly = true;
            _poolingCheckBox.Enabled = false;
            _keepAliveCheckBox.Enabled = false;
            _minPoolSizeTextBox.Text = "Use Driver tab Parameters";
            _maxPoolSizeTextBox.Text = "Use Driver tab Parameters";
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

            _timeoutTextBox = CreateTextBox();
            _maxRetriesTextBox = CreateTextBox();
            _retryIntervalTextBox = CreateTextBox();
            _connectionTimeoutTextBox = CreateTextBox();
            _commandTimeoutTextBox = CreateTextBox();
            _minPoolSizeTextBox = CreateTextBox();
            _maxPoolSizeTextBox = CreateTextBox();
            _poolingCheckBox = new CheckBox();
            _keepAliveCheckBox = new CheckBox();

            AddRow("Timeout (ms)", _timeoutTextBox);
            AddRow("Max Retries", _maxRetriesTextBox);
            AddRow("Retry Interval (ms)", _retryIntervalTextBox);
            AddRow("Connection Timeout (sec)", _connectionTimeoutTextBox);
            AddRow("Command Timeout (sec)", _commandTimeoutTextBox);
            AddRow("Min Pool Size", _minPoolSizeTextBox);
            AddRow("Max Pool Size", _maxPoolSizeTextBox);
            AddRow("Pooling", _poolingCheckBox);
            AddRow("Keepalive", _keepAliveCheckBox);

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
