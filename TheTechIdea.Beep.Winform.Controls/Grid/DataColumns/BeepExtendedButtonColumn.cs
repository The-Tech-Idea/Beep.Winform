using System;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Editor; // Ensure correct namespace for BeepExtendedButton

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepExtendedButtonColumn : DataGridViewColumn
    {
        public BeepExtendedButtonColumn() : base(new BeepExtendedButtonCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepExtendedButtonCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepExtendedButtonEditingControl); // Use BeepExtendedButton for editing
        public override Type ValueType => typeof(string); // Store button text
        public override object DefaultNewRowValue => "Click"; // Default text

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepExtendedButtonEditingControl control)
            {
                control.Text = initialFormattedValue?.ToString() ?? "Click";
            }
        }
    }

    public class BeepExtendedButtonEditingControl : BeepExtendedButton, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepExtendedButtonEditingControl()
        {
            this.Size = new Size(200, 30);
            this.BackColor = Color.White;

            // Handle button click event
            this.ButtonClick += BeepExtendedButton_Click;
            this.ExtendButtonClick += BeepExtendedButton_ExtendClick;
        }

        private void BeepExtendedButton_Click(object sender, BeepEventDataArgs e)
        {
            MessageBox.Show($"Button Clicked: {e.EventName}");
            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }

        private void BeepExtendedButton_ExtendClick(object sender, BeepEventDataArgs e)
        {
            MessageBox.Show($"Extended Button Clicked: {e.EventName}");
            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }

        public object EditingControlFormattedValue
        {
            get => this.Text;
            set => this.Text = value?.ToString() ?? string.Empty;
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.Text;

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
