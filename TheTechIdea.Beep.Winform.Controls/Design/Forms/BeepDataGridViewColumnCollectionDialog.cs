
using TheTechIdea.Beep.Winform.Controls.Grid.DataColumns;
using TheTechIdea.Beep.Winform.Controls.Grid;

namespace TheTechIdea.Beep.Winform.Controls.Design.Forms
{
    public partial class BeepDataGridViewColumnCollectionDialog: Form
    {
        private BeepDataGridView _beepDataGridView;
        private ListBox lstColumns;
        private PropertyGrid propertyGrid;
        private ComboBox cmbColumnType;
        private Button btnAdd, btnRemove, btnMoveUp, btnMoveDown, btnOK, btnCancel;

        public BeepDataGridViewColumnCollectionDialog(BeepDataGridView beepDataGridView)
        {
            _beepDataGridView = beepDataGridView;
            InitializeComponents();
            LoadColumns();
        }

        private void InitializeComponents()
        {
            this.Text = "BeepDataGridView Column Editor";
            this.Size = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            lstColumns = new ListBox { Dock = DockStyle.Left, Width = 200 };
            lstColumns.SelectedIndexChanged += (s, e) => propertyGrid.SelectedObject = lstColumns.SelectedItem;

            propertyGrid = new PropertyGrid { Dock = DockStyle.Fill };

            cmbColumnType = new ComboBox { Dock = DockStyle.Top, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbColumnType.Items.AddRange(new string[] { "Text", "ComboBox", "CheckBox", "Button", "Image", "Link", "Numeric", "DateTime" });
            cmbColumnType.SelectedIndex = 0;

            btnAdd = new Button { Text = "Add", Dock = DockStyle.Top };
            btnAdd.Click += (s, e) => AddColumn();

            btnRemove = new Button { Text = "Remove", Dock = DockStyle.Top };
            btnRemove.Click += (s, e) => RemoveColumn();

            btnMoveUp = new Button { Text = "Move Up", Dock = DockStyle.Top };
            btnMoveUp.Click += (s, e) => MoveColumn(-1);

            btnMoveDown = new Button { Text = "Move Down", Dock = DockStyle.Top };
            btnMoveDown.Click += (s, e) => MoveColumn(1);

            btnOK = new Button { Text = "OK", Dock = DockStyle.Bottom };
            btnOK.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };

            btnCancel = new Button { Text = "Cancel", Dock = DockStyle.Bottom };
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            Panel buttonPanel = new Panel { Dock = DockStyle.Right, Width = 120 };
            buttonPanel.Controls.AddRange(new Control[] { cmbColumnType, btnAdd, btnRemove, btnMoveUp, btnMoveDown, btnOK, btnCancel });

            this.Controls.Add(lstColumns);
            this.Controls.Add(propertyGrid);
            this.Controls.Add(buttonPanel);
        }

        private void LoadColumns()
        {
            lstColumns.Items.Clear();
            foreach (var column in _beepDataGridView.TargetGrid.Columns)
            {
                lstColumns.Items.Add(column);
            }
        }

        private void AddColumn()
        {
            DataGridViewColumn newColumn = cmbColumnType.SelectedItem switch
            {
                "ComboBox" => new BeepDataGridViewComboBoxColumn { HeaderText = "New Combo Column" },
                "CheckBox" => new DataGridViewCheckBoxColumn { HeaderText = "New Check Column" },
                "Button" => new DataGridViewButtonColumn { HeaderText = "New Button Column" },
                "Image" => new DataGridViewImageColumn { HeaderText = "New Image Column" },
                "Link" => new DataGridViewLinkColumn { HeaderText = "New Link Column" },
                "Numeric" => new BeepDataGridViewNumericColumn { HeaderText = "New Numeric Column" },
                "DateTime" => new BeepDataGridViewDateTimePickerColumn { HeaderText = "New DateTime Column" },
                _ => new DataGridViewTextBoxColumn { HeaderText = "New Text Column" }
            };

            _beepDataGridView.TargetGrid.Columns.Add(newColumn);
            lstColumns.Items.Add(newColumn);
            propertyGrid.SelectedObject = newColumn;
        }

        private void RemoveColumn()
        {
            if (lstColumns.SelectedItem is DataGridViewColumn column)
            {
                _beepDataGridView.TargetGrid.Columns.Remove(column);
                lstColumns.Items.Remove(column);
                propertyGrid.SelectedObject = null;
            }
        }

        private void MoveColumn(int direction)
        {
            int index = lstColumns.SelectedIndex;
            if (index < 0 || (index + direction) < 0 || (index + direction) >= lstColumns.Items.Count)
                return;

            var column = lstColumns.SelectedItem as DataGridViewColumn;
            _beepDataGridView.TargetGrid.Columns.Remove(column);
            _beepDataGridView.TargetGrid.Columns.Insert(index + direction, column);

            LoadColumns();
            lstColumns.SelectedIndex = index + direction;
        }
    }
}
