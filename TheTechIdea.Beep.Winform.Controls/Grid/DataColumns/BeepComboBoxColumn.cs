using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Models; // Ensure correct namespace for BeepComboBox

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepComboBoxColumn : DataGridViewColumn
    {
        public BeepComboBoxColumn() : base(new BeepComboBoxCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepComboBoxCell : DataGridViewComboBoxCell
    {
        public override Type EditType => typeof(BeepComboBoxEditingControl); // Use BeepComboBox for editing
        public override Type ValueType => typeof(string); // Store selected item text
        public override object DefaultNewRowValue => string.Empty; // Default to empty

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepComboBoxEditingControl control)
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
    public class BeepComboBoxEditingControl : BeepComboBox, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepComboBoxEditingControl()
        {
            this.Size = new Size(120, 30);
            this.BackColor = Color.White;

            // Handle selection change event
            this.SelectedItemChanged += BeepComboBoxEditingControl_SelectedItemChanged;
        }

        private void BeepComboBoxEditingControl_SelectedItemChanged(object? sender,SelectedItemChangedEventArgs e)
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
