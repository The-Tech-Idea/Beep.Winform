using System;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls; // Ensure correct namespace for BeepCircularButton

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepCircularButtonColumn : DataGridViewColumn
    {
        public BeepCircularButtonColumn() : base(new BeepCircularButtonCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepCircularButtonCell : DataGridViewButtonCell
    {
        public override Type EditType => typeof(BeepCircularButtonEditingControl); // Use BeepCircularButton for editing
        public override Type ValueType => typeof(string); // Store button text
        public override object DefaultNewRowValue => "Click"; // Default text

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepCircularButtonEditingControl control)
            {
                control.Text = initialFormattedValue?.ToString() ?? "Click";
            }
        }
    }

    public class BeepCircularButtonEditingControl : BeepCircularButton, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepCircularButtonEditingControl()
        {
            this.Size = new Size(50, 50);
            this.Text = "Click";
            this.ShowBorder = true;
            this.TextLocation = TextLocation.Below;

            // Handle click event
            this.Click += BeepCircularButton_Click;
        }

        private void BeepCircularButton_Click(object sender, EventArgs e)
        {
            if (dataGridView != null)
            {
                MessageBox.Show($"BeepCircularButton clicked in row {rowIndex}!");
                valueChanged = true;
                dataGridView.NotifyCurrentCellDirty(true);
            }
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
