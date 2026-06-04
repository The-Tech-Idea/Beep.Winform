using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public sealed class BeepConnectionListEditorForm : Form
    {
        private readonly BeepDataConnection _owner;
        private readonly IDMEEditor? _editor;
        private readonly DataGridView _grid;
        private readonly TextBox _statusTextBox;
        private readonly Button _addButton;
        private readonly Button _editButton;
        private readonly Button _removeButton;
        private readonly Button _duplicateButton;
        private readonly Button _setCurrentButton;
        private readonly Button _testAllButton;
        private readonly Button _saveButton;
        private readonly Button _closeButton;

        public bool SaveRequested { get; private set; }

        public BeepConnectionListEditorForm(BeepDataConnection owner, IDMEEditor? editor)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _editor = editor;

            Text = "Connection List Editor";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = false;
            MaximizeBox = true;
            ShowInTaskbar = false;
            Size = new Size(820, 480);
            MinimumSize = new Size(640, 360);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoGenerateColumns = false,
                BackgroundColor = SystemColors.Window
            };

            var nameColumn = new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = nameof(ConnectionProperties.ConnectionName), Width = 200 };
            var driverColumn = new DataGridViewTextBoxColumn { HeaderText = "Driver", DataPropertyName = nameof(ConnectionProperties.DriverName), Width = 160 };
            var categoryColumn = new DataGridViewTextBoxColumn { HeaderText = "Category", DataPropertyName = nameof(ConnectionProperties.Category), Width = 110 };
            var hostColumn = new DataGridViewTextBoxColumn { HeaderText = "Host", DataPropertyName = nameof(ConnectionProperties.Host), Width = 140 };
            var databaseColumn = new DataGridViewTextBoxColumn { HeaderText = "Database", DataPropertyName = nameof(ConnectionProperties.Database), Width = 140 };
            _grid.Columns.AddRange(nameColumn, driverColumn, categoryColumn, hostColumn, databaseColumn);
            _grid.DataSource = new BindingSource { DataSource = new List<ConnectionProperties>(_owner.DataConnections) };
            _grid.SelectionChanged += (_, _) => UpdateButtonState();

            _addButton = new Button { Text = "Add...", Width = 90 };
            _editButton = new Button { Text = "Edit...", Width = 90 };
            _removeButton = new Button { Text = "Remove", Width = 90 };
            _duplicateButton = new Button { Text = "Duplicate", Width = 90 };
            _setCurrentButton = new Button { Text = "Set as Current", Width = 110 };
            _testAllButton = new Button { Text = "Test All...", Width = 90 };

            _addButton.Click += OnAddClicked;
            _editButton.Click += OnEditClicked;
            _removeButton.Click += OnRemoveClicked;
            _duplicateButton.Click += OnDuplicateClicked;
            _setCurrentButton.Click += OnSetCurrentClicked;
            _testAllButton.Click += OnTestAllClicked;

            var leftPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                FlowDirection = FlowDirection.TopDown,
                Width = 110,
                Padding = new Padding(6),
                WrapContents = false
            };
            leftPanel.Controls.AddRange(new Control[] { _addButton, _editButton, _removeButton, _duplicateButton, _setCurrentButton, _testAllButton });

            _saveButton = new Button { Text = "Save All", Width = 100, Height = 30 };
            _closeButton = new Button { Text = "Close", DialogResult = DialogResult.Cancel, Width = 90, Height = 30 };
            _saveButton.Click += OnSaveClicked;

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                Padding = new Padding(12, 6, 12, 6)
            };
            _saveButton.Location = new Point(12, 7);
            _closeButton.Location = new Point(buttonPanel.Width - _closeButton.Width - 6, 7);
            buttonPanel.Resize += (_, _) => _closeButton.Location = new Point(buttonPanel.Width - _closeButton.Width - 6, 7);
            buttonPanel.Controls.Add(_saveButton);
            buttonPanel.Controls.Add(_closeButton);

            _statusTextBox = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = 24,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = SystemColors.Control,
                Font = new Font(SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont, FontStyle.Italic)
            };

            Controls.Add(_grid);
            Controls.Add(leftPanel);
            Controls.Add(buttonPanel);
            Controls.Add(_statusTextBox);
            CancelButton = _closeButton;

            UpdateButtonState();
            UpdateStatus();
        }

        private void UpdateButtonState()
        {
            bool hasSelection = _grid.SelectedRows.Count > 0;
            _editButton.Enabled = hasSelection;
            _removeButton.Enabled = hasSelection;
            _duplicateButton.Enabled = hasSelection;
            _setCurrentButton.Enabled = hasSelection;
        }

        private void UpdateStatus(string? message = null)
        {
            int count = _owner.DataConnections?.Count ?? 0;
            _statusTextBox.Text = message ?? $"{count} connection(s) loaded.";
        }

        private ConnectionProperties? GetSelected()
        {
            if (_grid.SelectedRows.Count == 0)
            {
                return null;
            }
            return _grid.SelectedRows[0].DataBoundItem as ConnectionProperties;
        }

        private void RefreshGrid()
        {
            ((BindingSource)_grid.DataSource).DataSource = new List<ConnectionProperties>(_owner.DataConnections);
            ((BindingSource)_grid.DataSource).ResetBindings(false);
            UpdateStatus();
        }

        private void OnAddClicked(object? sender, EventArgs e)
        {
            var newConnection = new ConnectionProperties { ConnectionName = "NewConnection" };
            using var dialog = new BeepConnectionEditorForm(newConnection, _editor, isNew: true);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (_owner.AddOrUpdateConnection(dialog.Result, persist: false))
                {
                    RefreshGrid();
                }
            }
        }

        private void OnEditClicked(object? sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null)
            {
                return;
            }
            using var dialog = new BeepConnectionEditorForm(selected, _editor, isNew: false);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _owner.AddOrUpdateConnection(dialog.Result, persist: false);
                RefreshGrid();
            }
        }

        private void OnRemoveClicked(object? sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null || string.IsNullOrWhiteSpace(selected.ConnectionName))
            {
                return;
            }
            var confirm = MessageBox.Show(this,
                $"Remove connection '{selected.ConnectionName}'?",
                "Confirm",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.OK)
            {
                return;
            }
            if (_owner.RemoveConnection(selected.ConnectionName, persist: false))
            {
                RefreshGrid();
            }
        }

        private void OnDuplicateClicked(object? sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null)
            {
                return;
            }
            var copy = new ConnectionProperties
            {
                ConnectionName = (selected.ConnectionName ?? "Connection") + "_Copy",
                DriverName = selected.DriverName,
                Host = selected.Host,
                Port = selected.Port,
                Database = selected.Database,
                UserID = selected.UserID,
                Password = selected.Password,
                ConnectionString = selected.ConnectionString,
                Category = selected.Category,
                Timeout = selected.Timeout,
                UseSSL = selected.UseSSL
            };
            int suffix = 1;
            while (_owner.DataConnections.Any(c => string.Equals(c.ConnectionName, copy.ConnectionName, StringComparison.OrdinalIgnoreCase)))
            {
                suffix++;
                copy.ConnectionName = $"{selected.ConnectionName}_Copy{suffix}";
            }
            _owner.AddOrUpdateConnection(copy, persist: false);
            RefreshGrid();
        }

        private void OnSetCurrentClicked(object? sender, EventArgs e)
        {
            var selected = GetSelected();
            if (selected == null)
            {
                return;
            }
            _owner.CurrentConnection = selected;
            UpdateStatus($"'{selected.ConnectionName}' set as current connection.");
        }

        private void OnTestAllClicked(object? sender, EventArgs e)
        {
            if (_owner.DataConnections == null || _owner.DataConnections.Count == 0)
            {
                UpdateStatus("No connections to test.");
                return;
            }
            if (_editor == null)
            {
                UpdateStatus("Editor service unavailable; cannot test.");
                return;
            }
            var outcomes = new List<BeepConnectionTestOutcome>(_owner.DataConnections.Count);
            foreach (var connection in _owner.DataConnections)
            {
                outcomes.Add(TestOne(connection));
            }
            using var report = new BeepConnectionTestReportForm(outcomes);
            report.ShowDialog(this);
        }

        private BeepConnectionTestOutcome TestOne(ConnectionProperties connection)
        {
            if (string.IsNullOrWhiteSpace(connection.ConnectionName))
            {
                return new BeepConnectionTestOutcome { ConnectionName = "(unnamed)", Success = false, Message = "Empty connection name." };
            }
            if (_editor == null)
            {
                return new BeepConnectionTestOutcome { ConnectionName = connection.ConnectionName, Success = false, Message = "Editor not available." };
            }
            var sw = Stopwatch.StartNew();
            try
            {
                var ds = _editor.GetDataSource(connection.ConnectionName);
                if (ds == null)
                {
                    sw.Stop();
                    return new BeepConnectionTestOutcome
                    {
                        ConnectionName = connection.ConnectionName,
                        DriverName = connection.DriverName,
                        Success = false,
                        LatencyMs = sw.ElapsedMilliseconds,
                        Message = "Datasource not registered."
                    };
                }
                var state = ds.Openconnection();
                sw.Stop();
                return new BeepConnectionTestOutcome
                {
                    ConnectionName = connection.ConnectionName,
                    DriverName = connection.DriverName,
                    Success = state == ConnectionState.Open,
                    LatencyMs = sw.ElapsedMilliseconds,
                    Message = state == ConnectionState.Open ? "Connected." : $"State: {state}."
                };
            }
            catch (Exception ex)
            {
                sw.Stop();
                return new BeepConnectionTestOutcome
                {
                    ConnectionName = connection.ConnectionName,
                    DriverName = connection.DriverName,
                    Success = false,
                    LatencyMs = sw.ElapsedMilliseconds,
                    Message = $"{ex.GetType().Name}: {ex.Message}"
                };
            }
        }

        private void OnSaveClicked(object? sender, EventArgs e)
        {
            if (_owner.SaveConnections())
            {
                SaveRequested = true;
                UpdateStatus("Saved.");
            }
            else
            {
                UpdateStatus("Save failed (no repository or no changes).");
            }
        }
    }
}
