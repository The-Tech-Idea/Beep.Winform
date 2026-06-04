using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepConnectionEditorForm : Form
    {
        private readonly ConnectionProperties _working;
        private readonly IDMEEditor? _editor;
        private readonly bool _isNew;
        private readonly TabControl _tabControl;
        private readonly TextBox _nameTextBox;
        private readonly ComboBox _driverComboBox;
        private readonly ComboBox _categoryComboBox;
        private readonly TextBox _hostTextBox;
        private readonly NumericUpDown _portNumeric;
        private readonly TextBox _databaseTextBox;
        private readonly TextBox _userIdTextBox;
        private readonly TextBox _passwordTextBox;
        private readonly TextBox _connectionStringTextBox;
        private readonly NumericUpDown _timeoutNumeric;
        private readonly CheckBox _useSslCheckBox;
        private readonly TextBox _statusTextBox;
        private readonly Button _testButton;
        private readonly Button _okButton;
        private readonly Button _cancelButton;

        public ConnectionProperties Result => _working;

        public BeepConnectionEditorForm(ConnectionProperties connection, IDMEEditor? editor, bool isNew)
        {
            _working = connection ?? throw new ArgumentNullException(nameof(connection));
            _editor = editor;
            _isNew = isNew;

            Text = isNew ? "Add Connection" : $"Edit Connection - {connection.ConnectionName}";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = false;
            MaximizeBox = true;
            ShowInTaskbar = false;
            Size = new Size(720, 520);
            MinimumSize = new Size(560, 380);

            _tabControl = new TabControl { Dock = DockStyle.Fill };

            var generalTab = new TabPage("General");
            BuildGeneralTab(generalTab, out _nameTextBox, out _driverComboBox, out _categoryComboBox, out _connectionStringTextBox, out _timeoutNumeric, out _useSslCheckBox);
            _tabControl.TabPages.Add(generalTab);

            var endpointTab = new TabPage("Endpoint");
            BuildEndpointTab(endpointTab, out _hostTextBox, out _portNumeric, out _databaseTextBox);
            _tabControl.TabPages.Add(endpointTab);

            var securityTab = new TabPage("Security");
            BuildSecurityTab(securityTab, out _userIdTextBox, out _passwordTextBox);
            _tabControl.TabPages.Add(securityTab);

            _statusTextBox = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = 28,
                ReadOnly = true,
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.None,
                Font = new Font(SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont, FontStyle.Italic)
            };

            _testButton = new Button
            {
                Text = "Test Connection",
                Width = 130,
                Height = 30
            };
            _testButton.Click += OnTestClicked;

            _okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Width = 90,
                Height = 30
            };
            _okButton.Click += OnOkClicked;

            _cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Width = 90,
                Height = 30
            };

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                Padding = new Padding(12, 6, 12, 6)
            };

            _testButton.Location = new Point(12, 7);
            _okButton.Location = new Point(buttonPanel.Width - _okButton.Width - _cancelButton.Width - 12, 7);
            _cancelButton.Location = new Point(buttonPanel.Width - _cancelButton.Width - 6, 7);
            buttonPanel.Resize += (_, _) =>
            {
                _okButton.Location = new Point(buttonPanel.Width - _okButton.Width - _cancelButton.Width - 12, 7);
                _cancelButton.Location = new Point(buttonPanel.Width - _cancelButton.Width - 6, 7);
            };
            buttonPanel.Controls.Add(_testButton);
            buttonPanel.Controls.Add(_okButton);
            buttonPanel.Controls.Add(_cancelButton);

            Controls.Add(_tabControl);
            Controls.Add(buttonPanel);
            Controls.Add(_statusTextBox);

            AcceptButton = _okButton;
            CancelButton = _cancelButton;

            Load += (_, _) => PopulateFromWorking();
        }

        private void BuildGeneralTab(TabPage tab,
            out TextBox nameBox, out ComboBox driverBox, out ComboBox categoryBox,
            out TextBox connectionStringBox, out NumericUpDown timeoutNumeric, out CheckBox useSslBox)
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(12)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 5; i++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            }
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            nameBox = new TextBox { Dock = DockStyle.Fill };
            driverBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            categoryBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            connectionStringBox = new TextBox { Dock = DockStyle.Fill };
            timeoutNumeric = new NumericUpDown { Dock = DockStyle.Left, Width = 120, Minimum = 0, Maximum = 600, Value = 30 };
            useSslBox = new CheckBox { Text = "Use SSL", AutoSize = true };

            layout.Controls.Add(new Label { Text = "Name:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
            layout.Controls.Add(nameBox, 1, 0);
            layout.Controls.Add(new Label { Text = "Driver:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 1);
            layout.Controls.Add(driverBox, 1, 1);
            layout.Controls.Add(new Label { Text = "Category:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 2);
            layout.Controls.Add(categoryBox, 1, 2);
            layout.Controls.Add(new Label { Text = "Connection String:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 3);
            layout.Controls.Add(connectionStringBox, 1, 3);

            var timeoutRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            timeoutRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            timeoutRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            timeoutRow.Controls.Add(timeoutNumeric, 0, 0);
            timeoutRow.Controls.Add(useSslBox, 1, 0);
            layout.Controls.Add(new Label { Text = "Timeout / SSL:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 4);
            layout.Controls.Add(timeoutRow, 1, 4);

            tab.Controls.Add(layout);

            // Populate driver and category combos
            if (_editor?.ConfigEditor?.DataDriversClasses != null)
            {
                foreach (var driver in _editor.ConfigEditor.DataDriversClasses)
                {
                    if (driver != null && !string.IsNullOrWhiteSpace(driver.PackageName))
                    {
                        driverBox.Items.Add(driver.PackageName);
                    }
                }
            }
            foreach (var category in Enum.GetValues(typeof(DatasourceCategory)))
            {
                categoryBox.Items.Add(category);
            }
        }

        private void BuildEndpointTab(TabPage tab, out TextBox hostBox, out NumericUpDown portNumeric, out TextBox databaseBox)
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(12)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 3; i++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            }

            hostBox = new TextBox { Dock = DockStyle.Fill };
            portNumeric = new NumericUpDown { Dock = DockStyle.Left, Width = 120, Minimum = 0, Maximum = 65535, Value = 0 };
            databaseBox = new TextBox { Dock = DockStyle.Fill };

            layout.Controls.Add(new Label { Text = "Host:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
            layout.Controls.Add(hostBox, 1, 0);
            layout.Controls.Add(new Label { Text = "Port:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 1);
            layout.Controls.Add(portNumeric, 1, 1);
            layout.Controls.Add(new Label { Text = "Database / Service:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 2);
            layout.Controls.Add(databaseBox, 1, 2);

            tab.Controls.Add(layout);
        }

        private void BuildSecurityTab(TabPage tab, out TextBox userIdBox, out TextBox passwordBox)
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(12)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 2; i++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            }

            userIdBox = new TextBox { Dock = DockStyle.Fill };
            passwordBox = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };

            layout.Controls.Add(new Label { Text = "User ID:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
            layout.Controls.Add(userIdBox, 1, 0);
            layout.Controls.Add(new Label { Text = "Password:", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 1);
            layout.Controls.Add(passwordBox, 1, 1);

            tab.Controls.Add(layout);
        }

        private void PopulateFromWorking()
        {
            _nameTextBox.Text = _working.ConnectionName ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(_working.DriverName))
            {
                int idx = _driverComboBox.Items.IndexOf(_working.DriverName);
                _driverComboBox.SelectedIndex = idx >= 0 ? idx : -1;
            }
            int catIdx = _categoryComboBox.Items.IndexOf(_working.Category);
            _categoryComboBox.SelectedIndex = catIdx >= 0 ? catIdx : -1;
            _hostTextBox.Text = _working.Host ?? string.Empty;
            _portNumeric.Value = Math.Max(0, Math.Min(65535, _working.Port));
            _databaseTextBox.Text = _working.Database ?? _working.FileName ?? string.Empty;
            _userIdTextBox.Text = _working.UserID ?? string.Empty;
            _passwordTextBox.Text = _working.Password ?? string.Empty;
            _connectionStringTextBox.Text = _working.ConnectionString ?? string.Empty;
            _timeoutNumeric.Value = Math.Max(0, Math.Min(600, _working.Timeout));
            _useSslCheckBox.Checked = _working.UseSSL;
        }

        private void OnOkClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_nameTextBox.Text))
            {
                _statusTextBox.Text = "Connection name is required.";
                _statusTextBox.ForeColor = Color.Firebrick;
                DialogResult = DialogResult.None;
                return;
            }

            _working.ConnectionName = _nameTextBox.Text.Trim();
            _working.DriverName = _driverComboBox.SelectedItem?.ToString() ?? _working.DriverName;
            if (_categoryComboBox.SelectedItem is DatasourceCategory category)
            {
                _working.Category = category;
            }
            _working.Host = _hostTextBox.Text.Trim();
            _working.Port = (int)_portNumeric.Value;
            _working.Database = _databaseTextBox.Text.Trim();
            _working.UserID = _userIdTextBox.Text.Trim();
            _working.Password = _passwordTextBox.Text;
            _working.ConnectionString = _connectionStringTextBox.Text;
            _working.Timeout = (int)_timeoutNumeric.Value;
            _working.UseSSL = _useSslCheckBox.Checked;
        }

        private void OnTestClicked(object? sender, EventArgs e)
        {
            _statusTextBox.Text = "Testing...";
            _statusTextBox.ForeColor = SystemColors.ControlText;
            Application.DoEvents();

            if (_editor == null)
            {
                _statusTextBox.Text = "Editor service not available in this context.";
                _statusTextBox.ForeColor = Color.DarkOrange;
                return;
            }

            OnOkClicked(sender, e);
            if (DialogResult == DialogResult.None)
            {
                return;
            }
            DialogResult = DialogResult.None;

            var sw = Stopwatch.StartNew();
            try
            {
                var dataSource = _editor.GetDataSource(_working.ConnectionName);
                if (dataSource == null)
                {
                    sw.Stop();
                    _statusTextBox.Text = $"Datasource '{_working.ConnectionName}' is not registered with the editor.";
                    _statusTextBox.ForeColor = Color.Firebrick;
                    return;
                }

                var state = dataSource.Openconnection();
                sw.Stop();
                bool open = state == System.Data.ConnectionState.Open;
                _statusTextBox.Text = open
                    ? $"Connected in {sw.ElapsedMilliseconds} ms."
                    : $"Connection did not open (state: {state}).";
                _statusTextBox.ForeColor = open ? Color.DarkGreen : Color.Firebrick;
            }
            catch (Exception ex)
            {
                sw.Stop();
                _statusTextBox.Text = $"Failed in {sw.ElapsedMilliseconds} ms: {ex.GetType().Name}: {ex.Message}";
                _statusTextBox.ForeColor = Color.Firebrick;
            }
        }
    }
}
