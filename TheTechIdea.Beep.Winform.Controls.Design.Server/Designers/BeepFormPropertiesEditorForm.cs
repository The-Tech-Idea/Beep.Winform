using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Integrated.Forms.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepFormPropertiesEditorForm : Form
    {
        private readonly BeepFormsDefinition _working;
        private readonly TextBox _idTextBox;
        private readonly TextBox _formNameTextBox;
        private readonly TextBox _titleTextBox;
        private readonly DataGridView _metadataGrid;
        private readonly Button _okButton;
        private readonly Button _cancelButton;

        public BeepFormsDefinition Result => _working;

        public BeepFormPropertiesEditorForm(BeepFormsDefinition definition)
        {
            _working = definition?.Clone() ?? throw new ArgumentNullException(nameof(definition));

            Text = $"Form Properties - {_working.FormName}";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            Size = new Size(620, 460);
            MinimumSize = new Size(520, 360);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(12)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _idTextBox = new TextBox { Dock = DockStyle.Fill };
            _formNameTextBox = new TextBox { Dock = DockStyle.Fill };
            _titleTextBox = new TextBox { Dock = DockStyle.Fill };

            layout.Controls.Add(new Label { Text = "Form ID:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
            layout.Controls.Add(_idTextBox, 1, 0);
            layout.Controls.Add(new Label { Text = "Form Name:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 1);
            layout.Controls.Add(_formNameTextBox, 1, 1);
            layout.Controls.Add(new Label { Text = "Title:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 2);
            layout.Controls.Add(_titleTextBox, 1, 2);
            layout.Controls.Add(new Label { Text = "Metadata (key/value):", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 3);
            layout.SetColumnSpan(layout.GetControlFromPosition(0, 3), 2);

            _metadataGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            _metadataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Key", Name = "Key" });
            _metadataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Value", Name = "Value" });
            layout.Controls.Add(_metadataGrid, 0, 4);
            layout.SetColumnSpan(_metadataGrid, 2);

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

            Controls.Add(layout);
            Controls.Add(buttonPanel);

            AcceptButton = _okButton;
            CancelButton = _cancelButton;

            Load += (_, _) => PopulateFromWorking();
        }

        private void PopulateFromWorking()
        {
            _idTextBox.Text = _working.Id ?? string.Empty;
            _formNameTextBox.Text = _working.FormName ?? string.Empty;
            _titleTextBox.Text = _working.Title ?? string.Empty;
            _metadataGrid.Rows.Clear();
            foreach (var entry in _working.Metadata)
            {
                _metadataGrid.Rows.Add(entry.Key, entry.Value);
            }
        }

        private void OnOkClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_formNameTextBox.Text))
            {
                MessageBox.Show(this, "Form Name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            _working.Id = _idTextBox.Text.Trim();
            _working.FormName = _formNameTextBox.Text.Trim();
            _working.Title = _titleTextBox.Text.Trim();

            _working.Metadata.Clear();
            foreach (DataGridViewRow row in _metadataGrid.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }
                string? key = row.Cells[0].Value?.ToString();
                string? value = row.Cells[1].Value?.ToString();
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }
                _working.Metadata[key!] = value ?? string.Empty;
            }
        }
    }
}
