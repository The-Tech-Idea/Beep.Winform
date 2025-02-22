using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Desktop.Common;
using System.ComponentModel; // Ensure correct namespace for BeepListBox

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepListBoxColumn : DataGridViewColumn
    {
        public BeepListBoxColumn() : base(new BeepListBoxCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepListBoxCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepListBoxEditingControl); // Use BeepListBox for editing
        public override Type ValueType => typeof(string); // Store selected item text
        public override object DefaultNewRowValue => string.Empty; // Default to empty

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepListBoxEditingControl control)
            {
                if (initialFormattedValue is string selectedText)
                {
                    var item = control.ListItems.FirstOrDefault(i => i.Text == selectedText);
                    control.SelectedItem = item;
                }
            }
        }
    }
    [ToolboxItem(false)]
    public class BeepListBoxEditingControl : BeepListBox, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepListBoxEditingControl()
        {
            this.Size = new Size(150, 100); // Default size
            this.BackColor = Color.White;
            this.Collapsed = true; // Start as collapsed
            this.ShowCheckBox = false; // By default, no checkboxes

            // Handle item clicked event
            this.ItemClicked += BeepListBox_ItemClicked;
        }

        private void BeepListBox_ItemClicked(object sender, SimpleItem selectedItem)
        {
            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }

        public object EditingControlFormattedValue
        {
            get => this.SelectedItem?.Text ?? string.Empty;
            set
            {
                if (value is string textValue)
                {
                    var item = this.ListItems.FirstOrDefault(i => i.Text == textValue);
                    if (item != null)
                    {
                        this.SelectedItem = item;
                    }
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.SelectedItem?.Text ?? string.Empty;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        public DataGridView EditingControlDataGridView
        {
            get => dataGridView;
            set => dataGridView = value;
        }

        public int EditingControlRowIndex
        {
            get => rowIndex;
            set => rowIndex = value;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey) => true;

        public void PrepareEditingControlForEdit(bool selectAll) { }

        public bool RepositionEditingControlOnValueChange => false;

        public Cursor EditingPanelCursor => base.Cursor;

        public bool EditingControlValueChanged
        {
            get => valueChanged;
            set => valueChanged = value;
        }
    }
}
