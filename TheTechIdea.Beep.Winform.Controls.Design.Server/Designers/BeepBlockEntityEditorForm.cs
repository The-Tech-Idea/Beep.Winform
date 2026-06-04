using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepBlockEntityEditorForm : Form
    {
        private readonly BeepBlockEntityDefinition _working;
        private readonly TextBox _connectionNameTextBox;
        private readonly TextBox _entityNameTextBox;
        private readonly TextBox _datasourceEntityNameTextBox;
        private readonly TextBox _captionTextBox;
        private readonly TextBox _descriptionTextBox;
        private readonly TextBox _dataSourceIdTextBox;
        private readonly CheckBox _isMasterCheckBox;
        private readonly TextBox _masterBlockNameTextBox;
        private readonly TextBox _masterKeyFieldTextBox;
        private readonly TextBox _foreignKeyFieldTextBox;
        private readonly DataGridView _fieldsGrid;
        private readonly BindingList<BeepBlockEntityFieldDefinition> _fieldRows;
        private readonly Button _okButton;
        private readonly Button _cancelButton;

        public BeepBlockEntityDefinition Result => _working;

        public BeepBlockEntityEditorForm(BeepBlockEntityDefinition entity)
        {
            _working = entity?.Clone() ?? throw new ArgumentNullException(nameof(entity));
            _fieldRows = new BindingList<BeepBlockEntityFieldDefinition>(
                _working.Fields.Select(f => f.Clone()).ToList());

            Text = $"Edit Block Entity - {entity.EntityName}";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = false;
            MaximizeBox = true;
            ShowInTaskbar = false;
            Size = new Size(820, 560);
            MinimumSize = new Size(640, 420);

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 240,
                FixedPanel = FixedPanel.Panel1
            };

            // Top: entity-level properties
            var topPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 6,
                Padding = new Padding(12)
            };
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            for (int i = 0; i < 5; i++)
            {
                topPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            }
            topPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _connectionNameTextBox = new TextBox { Dock = DockStyle.Fill };
            _entityNameTextBox = new TextBox { Dock = DockStyle.Fill };
            _datasourceEntityNameTextBox = new TextBox { Dock = DockStyle.Fill };
            _captionTextBox = new TextBox { Dock = DockStyle.Fill };
            _descriptionTextBox = new TextBox { Dock = DockStyle.Fill };
            _dataSourceIdTextBox = new TextBox { Dock = DockStyle.Fill };
            _isMasterCheckBox = new CheckBox { Text = "Block is a master in master-detail", AutoSize = true };
            _masterBlockNameTextBox = new TextBox { Dock = DockStyle.Fill };
            _masterKeyFieldTextBox = new TextBox { Dock = DockStyle.Fill };
            _foreignKeyFieldTextBox = new TextBox { Dock = DockStyle.Fill };

            topPanel.Controls.Add(new Label { Text = "Connection:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
            topPanel.Controls.Add(_connectionNameTextBox, 1, 0);
            topPanel.Controls.Add(new Label { Text = "Entity Name:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 2, 0);
            topPanel.Controls.Add(_entityNameTextBox, 3, 0);
            topPanel.Controls.Add(new Label { Text = "DS Entity Name:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 1);
            topPanel.Controls.Add(_datasourceEntityNameTextBox, 1, 1);
            topPanel.Controls.Add(new Label { Text = "Caption:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 2, 1);
            topPanel.Controls.Add(_captionTextBox, 3, 1);
            topPanel.Controls.Add(new Label { Text = "Description:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 2);
            topPanel.Controls.Add(_descriptionTextBox, 1, 2);
            topPanel.Controls.Add(new Label { Text = "Data Source ID:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 2, 2);
            topPanel.Controls.Add(_dataSourceIdTextBox, 3, 2);
            topPanel.Controls.Add(_isMasterCheckBox, 0, 3);
            topPanel.SetColumnSpan(_isMasterCheckBox, 4);
            topPanel.Controls.Add(new Label { Text = "Master Block:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 4);
            topPanel.Controls.Add(_masterBlockNameTextBox, 1, 4);
            topPanel.Controls.Add(new Label { Text = "Master Key:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 2, 4);
            topPanel.Controls.Add(_masterKeyFieldTextBox, 3, 4);
            topPanel.Controls.Add(new Label { Text = "Foreign Key:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 5);
            topPanel.Controls.Add(_foreignKeyFieldTextBox, 1, 5);

            split.Panel1.Controls.Add(topPanel);

            // Bottom: fields grid with add/remove
            var bottomPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(12)
            };
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            bottomPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            bottomPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var fieldsHeader = new Label
            {
                Text = "Entity Fields",
                Dock = DockStyle.Fill,
                Font = new Font(SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont, FontStyle.Bold)
            };
            bottomPanel.Controls.Add(fieldsHeader, 0, 0);

            var addFieldButton = new Button { Text = "Add Field", Height = 24 };
            addFieldButton.Click += (_, _) => AddField();
            var buttonStack = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false
            };
            buttonStack.Controls.Add(addFieldButton);
            bottomPanel.Controls.Add(buttonStack, 1, 0);

            _fieldsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = _fieldRows,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            _fieldsGrid.Columns[nameof(BeepBlockEntityFieldDefinition.FieldName)].HeaderText = "Field";
            _fieldsGrid.Columns[nameof(BeepBlockEntityFieldDefinition.Label)].HeaderText = "Label";
            _fieldsGrid.Columns[nameof(BeepBlockEntityFieldDefinition.DataType)].HeaderText = "Type";
            _fieldsGrid.Columns[nameof(BeepBlockEntityFieldDefinition.Category)].HeaderText = "Category";
            _fieldsGrid.Columns[nameof(BeepBlockEntityFieldDefinition.IsPrimaryKey)].HeaderText = "PK";
            _fieldsGrid.Columns[nameof(BeepBlockEntityFieldDefinition.IsRequired)].HeaderText = "Req";
            _fieldsGrid.Columns[nameof(BeepBlockEntityFieldDefinition.IsHidden)].HeaderText = "Hide";
            _fieldsGrid.Columns[nameof(BeepBlockEntityFieldDefinition.Order)].HeaderText = "Order";

            bottomPanel.Controls.Add(_fieldsGrid, 0, 1);
            bottomPanel.SetColumnSpan(_fieldsGrid, 2);

            split.Panel2.Controls.Add(bottomPanel);

            _okButton = new Button { Text = "OK", DialogResult = DialogResult.OK, Width = 90, Height = 30 };
            _cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 90, Height = 30 };
            _okButton.Click += OnOkClicked;

            var buttonPanel = new Panel { Dock = DockStyle.Bottom, Height = 44, Padding = new Padding(12, 6, 12, 6) };
            _okButton.Location = new Point(buttonPanel.Width - _okButton.Width - _cancelButton.Width - 12, 7);
            _cancelButton.Location = new Point(buttonPanel.Width - _cancelButton.Width - 6, 7);
            buttonPanel.Resize += (_, _) =>
            {
                _okButton.Location = new Point(buttonPanel.Width - _okButton.Width - _cancelButton.Width - 12, 7);
                _cancelButton.Location = new Point(buttonPanel.Width - _cancelButton.Width - 6, 7);
            };
            buttonPanel.Controls.Add(_okButton);
            buttonPanel.Controls.Add(_cancelButton);

            Controls.Add(split);
            Controls.Add(buttonPanel);
            AcceptButton = _okButton;
            CancelButton = _cancelButton;

            Load += (_, _) => PopulateFromWorking();
        }

        private void PopulateFromWorking()
        {
            _connectionNameTextBox.Text = _working.ConnectionName ?? string.Empty;
            _entityNameTextBox.Text = _working.EntityName ?? string.Empty;
            _datasourceEntityNameTextBox.Text = _working.DatasourceEntityName ?? string.Empty;
            _captionTextBox.Text = _working.Caption ?? string.Empty;
            _descriptionTextBox.Text = _working.Description ?? string.Empty;
            _dataSourceIdTextBox.Text = _working.DataSourceId ?? string.Empty;
            _isMasterCheckBox.Checked = _working.IsMasterBlock;
            _masterBlockNameTextBox.Text = _working.MasterBlockName ?? string.Empty;
            _masterKeyFieldTextBox.Text = _working.MasterKeyField ?? string.Empty;
            _foreignKeyFieldTextBox.Text = _working.ForeignKeyField ?? string.Empty;
        }

        private void AddField()
        {
            int n = _fieldRows.Count + 1;
            _fieldRows.Add(new BeepBlockEntityFieldDefinition
            {
                FieldName = $"Field{n}",
                Label = $"Field {n}",
                Order = n
            });
        }

        private void OnOkClicked(object? sender, EventArgs e)
        {
            _working.ConnectionName = _connectionNameTextBox.Text.Trim();
            _working.EntityName = _entityNameTextBox.Text.Trim();
            _working.DatasourceEntityName = _datasourceEntityNameTextBox.Text.Trim();
            _working.Caption = _captionTextBox.Text.Trim();
            _working.Description = _descriptionTextBox.Text.Trim();
            _working.DataSourceId = _dataSourceIdTextBox.Text.Trim();
            _working.IsMasterBlock = _isMasterCheckBox.Checked;
            _working.MasterBlockName = _masterBlockNameTextBox.Text.Trim();
            _working.MasterKeyField = _masterKeyFieldTextBox.Text.Trim();
            _working.ForeignKeyField = _foreignKeyFieldTextBox.Text.Trim();

            _working.Fields.Clear();
            foreach (var row in _fieldRows)
            {
                if (row == null || string.IsNullOrWhiteSpace(row.FieldName))
                {
                    continue;
                }
                _working.Fields.Add(row.Clone());
            }
        }
    }
}
