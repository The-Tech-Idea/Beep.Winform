using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;

namespace TheTechIdea.Beep.Winform.Default.Views.DataSource_Connection_Controls
{
    public partial class uc_MetaDataProperties : uc_DataConnectionPropertiesBaseControl
    {
        private TableLayoutPanel _layout;
        private TextBox _schemaNameTextBox;
        private TextBox _metadataCatalogTextBox;
        private TextBox _entityFilterTextBox;
        private TextBox _refreshSecondsTextBox;
        private CheckBox _includeViewsCheckBox;
        private CheckBox _includeSystemObjectsCheckBox;

        public uc_MetaDataProperties()
        {
            InitializeComponent();
            BuildUi();
        }

        public override void SetupBindings(ConnectionProperties conn)
        {
            base.SetupBindings(conn);
            Text = "Metadata";
            if (conn == null)
            {
                return;
            }

            _schemaNameTextBox.DataBindings.Clear();
            _schemaNameTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.SchemaName), true, DataSourceUpdateMode.OnPropertyChanged));

            _metadataCatalogTextBox.DataBindings.Clear();
            _metadataCatalogTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Database), true, DataSourceUpdateMode.OnPropertyChanged));

            _entityFilterTextBox.DataBindings.Clear();
            _entityFilterTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.CompositeLayerName), true, DataSourceUpdateMode.OnPropertyChanged));

            _refreshSecondsTextBox.DataBindings.Clear();
            _refreshSecondsTextBox.DataBindings.Add(new Binding("Text", conn, nameof(conn.Timeout), true, DataSourceUpdateMode.OnPropertyChanged));

            _includeViewsCheckBox.DataBindings.Clear();
            _includeSystemObjectsCheckBox.DataBindings.Clear();
            _includeViewsCheckBox.Enabled = false;
            _includeSystemObjectsCheckBox.Enabled = false;
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

            _schemaNameTextBox = CreateTextBox();
            _metadataCatalogTextBox = CreateTextBox();
            _entityFilterTextBox = CreateTextBox();
            _refreshSecondsTextBox = CreateTextBox();
            _includeViewsCheckBox = new CheckBox();
            _includeSystemObjectsCheckBox = new CheckBox();

            AddRow("Schema Name", _schemaNameTextBox);
            AddRow("Metadata Catalog", _metadataCatalogTextBox);
            AddRow("Entity Filter", _entityFilterTextBox);
            AddRow("Refresh Seconds", _refreshSecondsTextBox);
            AddRow("Include Views", _includeViewsCheckBox);
            AddRow("Include System Objects", _includeSystemObjectsCheckBox);

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
