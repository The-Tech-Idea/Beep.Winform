using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design.Forms
{
    public partial class EnumSelectorForm: Form
    {
        private TextBox txtSearch;
        private DataGridView gridEnums;
        private Button btnOK;
        private Button btnCancel;
        private Panel panelButtons;

        public string SelectedEnumType { get; set; }

        public EnumSelectorForm()
        {
            InitializeComponents();
            LoadEnumTypes();
        }

        private void InitializeComponents()
        {
            this.Text = "Select Enum Type";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;

            // Search TextBox
            txtSearch = new TextBox
            {
                Dock = DockStyle.Top,
                PlaceholderText = "Search enums...",
                Height = 25
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // DataGridView for enums
            gridEnums = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            gridEnums.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Enum Name",
                DataPropertyName = "DisplayName",
                Width = 300
            });
            gridEnums.DoubleClick += GridEnums_DoubleClick;

            // Buttons
            btnOK = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Height = 30,
                Width = 75
            };
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Height = 30,
                Width = 75
            };

            panelButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };
            panelButtons.Controls.Add(btnOK);
            panelButtons.Controls.Add(btnCancel);
            btnOK.Location = new System.Drawing.Point(panelButtons.Width - btnOK.Width - btnCancel.Width - 15, 5);
            btnCancel.Location = new System.Drawing.Point(panelButtons.Width - btnCancel.Width - 5, 5);

            this.Controls.Add(gridEnums);
            this.Controls.Add(txtSearch);
            this.Controls.Add(panelButtons);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void LoadEnumTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("TheTechIdea")); // Filter by namespace

            var enumTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsEnum && t.IsPublic && t.Namespace.StartsWith("TheTechIdea")) // Filter by namespace
                .OrderBy(t => t.FullName)
                .Select(t => new EnumTypeItem { DisplayName = t.FullName, AssemblyQualifiedName = t.AssemblyQualifiedName })
                .ToList();

            gridEnums.DataSource = enumTypes;

            // Select the current value if it exists
            if (!string.IsNullOrEmpty(SelectedEnumType))
            {
                var selectedRow = gridEnums.Rows.Cast<DataGridViewRow>()
                    .FirstOrDefault(r => ((EnumTypeItem)r.DataBoundItem).AssemblyQualifiedName == SelectedEnumType);
                if (selectedRow != null)
                {
                    selectedRow.Selected = true;
                    gridEnums.FirstDisplayedScrollingRowIndex = selectedRow.Index;
                }
            }
        }

        private void FilterEnums()
        {
            var filter = txtSearch.Text.ToLower();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && a.GetName().Name.StartsWith("TheTechIdea"));

            var filteredTypes = assemblies
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsEnum && t.IsPublic && t.Namespace.StartsWith("TheTechIdea") && t.FullName.ToLower().Contains(filter))
                .OrderBy(t => t.FullName)
                .Select(t => new EnumTypeItem { DisplayName = t.FullName, AssemblyQualifiedName = t.AssemblyQualifiedName })
                .ToList();

            gridEnums.DataSource = filteredTypes;

            // Reselect if possible
            if (!string.IsNullOrEmpty(SelectedEnumType))
            {
                var selectedRow = gridEnums.Rows.Cast<DataGridViewRow>()
                    .FirstOrDefault(r => ((EnumTypeItem)r.DataBoundItem).AssemblyQualifiedName == SelectedEnumType);
                if (selectedRow != null)
                    selectedRow.Selected = true;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterEnums();
        }

        private void GridEnums_DoubleClick(object sender, EventArgs e)
        {
            if (gridEnums.SelectedRows.Count > 0)
            {
                SelectedEnumType = ((EnumTypeItem)gridEnums.SelectedRows[0].DataBoundItem).AssemblyQualifiedName;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (gridEnums.SelectedRows.Count > 0)
            {
                SelectedEnumType = ((EnumTypeItem)gridEnums.SelectedRows[0].DataBoundItem).AssemblyQualifiedName;
            }
            else
            {
                SelectedEnumType = null; // Optional: clear if no selection
            }
        }
    }

    // Helper class (unchanged)
    public class EnumTypeItem
    {
        public string DisplayName { get; set; }
        public string AssemblyQualifiedName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

}
