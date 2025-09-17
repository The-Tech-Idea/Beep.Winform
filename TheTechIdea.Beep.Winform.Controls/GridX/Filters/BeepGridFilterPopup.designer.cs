using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Filters
{
    internal partial class BeepGridFilterPopup
    {
        private Button btnSortAsc;
        private Button btnSortDesc;
        private Button btnClear;
        private Label lblFilter;
        private TextBox txtSearch;
        private CheckBox chkSelectAll;
        private CheckedListBox clbValues;
        private Button btnApply;
        private Button btnCancel;

        private void InitializeComponent()
        {
            btnSortAsc = new Button();
            btnSortDesc = new Button();
            btnClear = new Button();
            lblFilter = new Label();
            txtSearch = new TextBox();
            chkSelectAll = new CheckBox();
            clbValues = new CheckedListBox();
            btnApply = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // btnSortAsc
            // 
            btnSortAsc.FlatStyle = FlatStyle.Flat;
            btnSortAsc.Location = new Point(6, 6);
            btnSortAsc.Name = "btnSortAsc";
            btnSortAsc.Size = new Size(248, 28);
            btnSortAsc.TabIndex = 0;
            btnSortAsc.Text = "Sort Smallest to Largest";
            btnSortAsc.Click += btnSortAsc_Click;
            // 
            // btnSortDesc
            // 
            btnSortDesc.FlatStyle = FlatStyle.Flat;
            btnSortDesc.Location = new Point(6, 40);
            btnSortDesc.Name = "btnSortDesc";
            btnSortDesc.Size = new Size(248, 28);
            btnSortDesc.TabIndex = 1;
            btnSortDesc.Text = "Sort Largest to Smallest";
            btnSortDesc.Click += btnSortDesc_Click;
            // 
            // btnClear
            // 
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Location = new Point(6, 74);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(248, 28);
            btnClear.TabIndex = 2;
            btnClear.Text = "Clear Filter";
            btnClear.Click += btnClear_Click;
            // 
            // lblFilter
            // 
            lblFilter.Location = new Point(6, 110);
            lblFilter.Name = "lblFilter";
            lblFilter.Size = new Size(248, 20);
            lblFilter.TabIndex = 3;
            lblFilter.Text = "Column";
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(6, 132);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(248, 23);
            txtSearch.TabIndex = 4;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // chkSelectAll
            // 
            chkSelectAll.AutoSize = true;
            chkSelectAll.Checked = true;
            chkSelectAll.CheckState = CheckState.Checked;
            chkSelectAll.Location = new Point(6, 158);
            chkSelectAll.Name = "chkSelectAll";
            chkSelectAll.Size = new Size(74, 19);
            chkSelectAll.TabIndex = 5;
            chkSelectAll.Text = "Select All";
            chkSelectAll.CheckedChanged += chkSelectAll_CheckedChanged;
            // 
            // clbValues
            // 
            clbValues.CheckOnClick = true;
            clbValues.IntegralHeight = false;
            clbValues.Location = new Point(6, 180);
            clbValues.Name = "clbValues";
            clbValues.Size = new Size(248, 90);
            clbValues.TabIndex = 6;
            // 
            // btnApply
            // 
            btnApply.FlatStyle = FlatStyle.Flat;
            btnApply.Location = new Point(98, 276);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(75, 28);
            btnApply.TabIndex = 7;
            btnApply.Text = "Apply";
            btnApply.Click += btnApply_Click;
            // 
            // btnCancel
            // 
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(179, 276);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 28);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // BeepGridFilterPopup
            // 
            BackColor = SystemColors.Window;
            ClientSize = new Size(258, 311);
            Controls.Add(btnSortAsc);
            Controls.Add(btnSortDesc);
            Controls.Add(btnClear);
            Controls.Add(lblFilter);
            Controls.Add(txtSearch);
            Controls.Add(chkSelectAll);
            Controls.Add(clbValues);
            Controls.Add(btnApply);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.None;
            Name = "BeepGridFilterPopup";
            Padding = new Padding(6);
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
