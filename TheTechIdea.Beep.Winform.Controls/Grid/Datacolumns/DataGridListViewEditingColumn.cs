using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns
{
    public class BeepListViewEditingControl : ListView, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged = false;
        private int rowIndex;

        public BeepListViewEditingControl()
        {
            this.View = View.List; // Set default view to List for better item visibility
        }

        public DataGridView EditingControlDataGridView
        {
            get { return dataGridView; }
            set { dataGridView = value; }
        }

        public object EditingControlFormattedValue
        {
            get
            {
                return SelectedItems.Count > 0 ? SelectedItems[0].Text : string.Empty;
            }
            set
            {
                if (value is string stringValue)
                {
                    foreach (ListViewItem item in Items)
                    {
                        if (item.Text == stringValue)
                        {
                            item.Selected = true;
                            item.Focused = true;
                            break;
                        }
                    }
                }
            }
        }

        public int EditingControlRowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        public bool EditingControlValueChanged
        {
            get { return valueChanged; }
            set { valueChanged = value; }
        }

        public Cursor EditingPanelCursor => base.Cursor;

        public bool RepositionEditingControlOnValueChange => false;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            return true; // Let ListView handle navigation keys
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // Optionally handle the case where the control is prepared for editing (e.g., select item, etc.)
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            if (SelectedItems.Count > 0)
            {
                valueChanged = true;
                EditingControlDataGridView?.NotifyCurrentCellDirty(true);
            }
        }

        public void UpdateContentsBasedOnMasterValue(string masterValue)
        {
            // Example: Update ListView based on masterValue (e.g., dynamic filtering)
            this.Items.Clear();
            if (masterValue == "Example")
            {
                this.Items.Add(new ListViewItem("Item 1"));
                this.Items.Add(new ListViewItem("Item 2"));
            }
            else
            {
                this.Items.Add(new ListViewItem("Default 1"));
                this.Items.Add(new ListViewItem("Default 2"));
            }
        }
    }

    public class ListViewEditingCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepListViewEditingControl);

        public override Type ValueType => typeof(string);

        public override object DefaultNewRowValue => string.Empty;
    }

    
}
