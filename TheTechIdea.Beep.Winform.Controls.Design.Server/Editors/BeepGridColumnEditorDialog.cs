using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Editors
{
    public partial class BeepGridColumnEditorDialog : Form
    {
        private readonly BeepGridPro _grid;
        private ListView _columnListView;
        private PropertyGrid _propertyGrid;
        private Button _btnAdd;
        private Button _btnRemove;
        private Button _btnMoveUp;
        private Button _btnMoveDown;
        private Button _btnOK;
        private Button _btnCancel;
        private Button _btnRetrieveFields;
        private Label _lblColumns;
        private Label _lblProperties;

        public BeepGridColumnEditorDialog(BeepGridPro grid)
        {
            _grid = grid;
            InitializeComponents();
            PopulateColumnList();
        }

        private void InitializeComponents()
        {
            this.Text = "Edit Columns";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            _columnListView = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(12, 30),
                Size = new Size(300, 380),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };
            _columnListView.Columns.Add("Name", 120);
            _columnListView.Columns.Add("Caption", 100);
            _columnListView.Columns.Add("Width", 60);
            _columnListView.SelectedIndexChanged += ColumnListView_SelectedIndexChanged;

            _propertyGrid = new PropertyGrid
            {
                Location = new Point(324, 30),
                Size = new Size(450, 380),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ToolbarVisible = false
            };

            _lblColumns = new Label { Text = "Columns", Location = new Point(12, 12), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            _lblProperties = new Label { Text = "Properties", Location = new Point(324, 12), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };

            int btnY = 420;
            _btnAdd = new Button { Text = "Add", Location = new Point(12, btnY), Size = new Size(70, 28) };
            _btnRemove = new Button { Text = "Remove", Location = new Point(88, btnY), Size = new Size(70, 28) };
            _btnMoveUp = new Button { Text = "▲", Location = new Point(164, btnY), Size = new Size(35, 28) };
            _btnMoveDown = new Button { Text = "▼", Location = new Point(205, btnY), Size = new Size(35, 28) };
            _btnRetrieveFields = new Button { Text = "Retrieve Fields", Location = new Point(246, btnY), Size = new Size(100, 28) };

            _btnOK = new Button { Text = "OK", Location = new Point(590, btnY), Size = new Size(80, 28), DialogResult = DialogResult.OK };
            _btnCancel = new Button { Text = "Cancel", Location = new Point(676, btnY), Size = new Size(80, 28), DialogResult = DialogResult.Cancel };

            _btnAdd.Click += BtnAdd_Click;
            _btnRemove.Click += BtnRemove_Click;
            _btnMoveUp.Click += BtnMoveUp_Click;
            _btnMoveDown.Click += BtnMoveDown_Click;
            _btnRetrieveFields.Click += BtnRetrieveFields_Click;
            _btnOK.Click += (s, e) => ApplyColumns();

            this.Controls.AddRange(new Control[] { _columnListView, _propertyGrid, _lblColumns, _lblProperties, _btnAdd, _btnRemove, _btnMoveUp, _btnMoveDown, _btnRetrieveFields, _btnOK, _btnCancel });
            this.AcceptButton = _btnOK;
            this.CancelButton = _btnCancel;
        }

        private void PopulateColumnList()
        {
            _columnListView.Items.Clear();
            foreach (BeepColumnConfig col in _grid.Columns)
            {
                var item = new ListViewItem(col.ColumnName);
                item.SubItems.Add(col.ColumnCaption);
                item.SubItems.Add(col.Width.ToString());
                item.Tag = col;
                if (!col.Visible)
                    item.ForeColor = Color.Gray;
                _columnListView.Items.Add(item);
            }
        }

        private void ColumnListView_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_columnListView.SelectedItems.Count > 0)
            {
                _propertyGrid.SelectedObject = _columnListView.SelectedItems[0].Tag;
            }
            else
            {
                _propertyGrid.SelectedObject = null;
            }
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            int selectedIndex = _columnListView.SelectedItems.Count > 0 ? _columnListView.SelectedItems[0].Index : -1;
            _btnRemove.Enabled = selectedIndex >= 0;
            _btnMoveUp.Enabled = selectedIndex > 0;
            _btnMoveDown.Enabled = selectedIndex >= 0 && selectedIndex < _grid.Columns.Count - 1;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var newCol = new BeepColumnConfig
            {
                ColumnName = $"Column{_grid.Columns.Count + 1}",
                ColumnCaption = $"Column {_grid.Columns.Count + 1}",
                Width = 100,
                Visible = true,
                AllowSort = true,
                Resizable = DataGridViewTriState.True,
                AllowReorder = true,
                DisplayOrder = _grid.Columns.Count
            };
            _grid.Columns.Add(newCol);
            PopulateColumnList();
            _columnListView.Items[_grid.Columns.Count - 1].Selected = true;
            _columnListView.Focus();
        }

        private void BtnRemove_Click(object? sender, EventArgs e)
        {
            if (_columnListView.SelectedItems.Count > 0)
            {
                var col = _columnListView.SelectedItems[0].Tag as BeepColumnConfig;
                if (col != null)
                {
                    _grid.Columns.Remove(col);
                    PopulateColumnList();
                    _propertyGrid.SelectedObject = null;
                }
            }
        }

        private void BtnMoveUp_Click(object? sender, EventArgs e)
        {
            int index = _columnListView.SelectedItems[0].Index;
            if (index > 0)
            {
                var col = _grid.Columns[index];
                _grid.Columns.RemoveAt(index);
                _grid.Columns.Insert(index - 1, col);
                PopulateColumnList();
                _columnListView.Items[index - 1].Selected = true;
            }
        }

        private void BtnMoveDown_Click(object? sender, EventArgs e)
        {
            int index = _columnListView.SelectedItems[0].Index;
            if (index < _grid.Columns.Count - 1)
            {
                var col = _grid.Columns[index];
                _grid.Columns.RemoveAt(index);
                _grid.Columns.Insert(index + 1, col);
                PopulateColumnList();
                _columnListView.Items[index + 1].Selected = true;
            }
        }

        private void BtnRetrieveFields_Click(object? sender, EventArgs e)
        {
            var dataSource = _grid.DataSource;
            if (dataSource == null)
            {
                MessageBox.Show("No data source is bound to the grid.", "Retrieve Fields", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _grid.AutoGenerateColumns();
            PopulateColumnList();
            MessageBox.Show("Fields retrieved from data source.", "Retrieve Fields", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ApplyColumns()
        {
            var layoutHelper = typeof(BeepGridPro).GetProperty("Layout", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(_grid);
            layoutHelper?.GetType().GetMethod("Recalculate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.Invoke(layoutHelper, null);
            _grid.Invalidate();
            _grid.Refresh();
        }
    }
}
