using System;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls;
using System.ComponentModel; // Ensure correct namespace for BeepProgressBar

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepProgressBarColumn : DataGridViewColumn
    {
        public BeepProgressBarColumn() : base(new BeepProgressBarCell())
        {
        }

        public int Minimum { get; internal set; }
        public int Maximum { get; internal set; }
        public int Step { get; internal set; }
        public Color ProgressBarColor { get; internal set; }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepProgressBarCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepProgressBarEditingControl); // Use BeepProgressBar for editing
        public override Type ValueType => typeof(int); // Store progress value as an integer
        public override object DefaultNewRowValue => 0; // Default progress value is 0

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepProgressBarEditingControl control)
            {
                if (initialFormattedValue is int progressValue)
                {
                    control.Value = progressValue;
                }
                else
                {
                    control.Value = 0;
                }
            }
        }
    }
    [ToolboxItem(false)]
    public class BeepProgressBarEditingControl : BeepProgressBar, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepProgressBarEditingControl()
        {
            this.Size = new Size(200, 30); // Default size
            this.BackColor = Color.White;

            // Handle value change event
            this.TextChanged += BeepProgressBar_ValueChanged;
        }

        private void BeepProgressBar_ValueChanged(object sender, EventArgs e)
        {
            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }

        public object EditingControlFormattedValue
        {
            get => this.Value;
            set
            {
                if (value is int progressValue)
                {
                    this.Value = progressValue;
                }
                else
                {
                    this.Value = 0;
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.Value;

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
