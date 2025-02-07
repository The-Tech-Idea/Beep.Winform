using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepButtonColumn : DataGridViewColumn
        {
            public BeepButtonColumn() : base(new BeepButtonCell())
            {
            }

            public override object Clone()
            {
                return base.Clone();
            }
        }

        public class BeepButtonCell : DataGridViewTextBoxCell
        {
            public override Type EditType => typeof(BeepButtonEditingControl); // Use BeepButton for editing
            public override Type ValueType => typeof(string); // Store button text
            public override object DefaultNewRowValue => "Click"; // Default button text

            public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
            {
                base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

                if (DataGridView.EditingControl is BeepButtonEditingControl control)
                {
                    control.Text = initialFormattedValue?.ToString() ?? "Click";
                }
            }
        }

        public class BeepButtonEditingControl : BeepButton, IDataGridViewEditingControl
        {
            private DataGridView dataGridView;
            private int rowIndex;
            private bool valueChanged;

            public BeepButtonEditingControl()
            {
                this.Size = new Size(100, 30);
                this.Text = "Click";

                // Handle Click event to update DataGridView
                this.Click += BeepButton_Click;
            }

            private void BeepButton_Click(object sender, EventArgs e)
            {
                if (dataGridView != null)
                {
                    MessageBox.Show($"BeepButton clicked in row {rowIndex}!");
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
                this.Font = dataGridViewCellStyle.Font;
                this.BackColor = dataGridViewCellStyle.BackColor;
                this.ForeColor = dataGridViewCellStyle.ForeColor;
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

            public void PrepareEditingControlForEdit(bool selectAll)
            {
                if (selectAll)
                {
                    this.Select();
                }
            }

            public bool RepositionEditingControlOnValueChange => false;

            public Cursor EditingPanelCursor => base.Cursor;

            public bool EditingControlValueChanged
            {
                get => valueChanged;
                set => valueChanged = value;
            }
        }
    }


