using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns
{
    [ToolboxItem(false)]
    public class BeepDataGridViewNumericColumn : DataGridViewColumn
    {
        public BeepDataGridViewNumericColumn() : base(new DataGridViewNumericCell())
        {
        }

        public override object Clone()
        {
            var clone = (BeepDataGridViewNumericColumn)base.Clone();
            return clone;
        }
    }

    public class DataGridViewNumericCell : DataGridViewTextBoxCell
    {
        private string _format = "N2"; // Default format (numeric with 2 decimal places).

        public DataGridViewNumericCell()
            : base()
        {
            this.Style.Format = _format; // Set the default numeric format.
        }

        public override object Clone()
        {
            var clone = (DataGridViewNumericCell)base.Clone();
            return clone;
        }

        // Property to specify the format (e.g., "C2" for currency, "P" for percentage, "N2" for decimal).
        public string NumericFormat
        {
            get => _format;
            set
            {
                _format = value;
                this.Style.Format = value;
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(decimal); // Use decimal as the value type for numeric values.
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                return 0m; // Default new row value to 0.
            }
        }

        public override Type EditType
        {
            get
            {
                return typeof(NumericEditingControl); // Use custom editing control.
            }
        }

        // This method will initialize the custom editing control with the current value.
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            var control = DataGridView.EditingControl as NumericEditingControl;
            if (control != null)
            {
                // Set the format for the numeric input.
                control.NumericFormat = this.NumericFormat;

                // Set the current value in the editing control.
                if (this.Value == null || this.Value == DBNull.Value)
                {
                    control.Value = 0m; // Default to 0 if the value is null.
                }
                else
                {
                    control.Value = Convert.ToDecimal(this.Value);
                }
            }
        }
    }

    // The editing control used for numeric entry.
    public class NumericEditingControl : TextBox, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged = false;
        private int rowIndex;
        private string _numericFormat = "N2"; // Default to numeric with 2 decimal places.
        private decimal _value = 0m; // Default value.

        public NumericEditingControl()
        {
            this.TextAlign = HorizontalAlignment.Right; // Right-align numeric values.
        }

        // Property to get or set the numeric value in the control.
        public decimal Value
        {
            get
            {
                if (decimal.TryParse(this.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out var result))
                {
                    return result;
                }
                return _value;
            }
            set
            {
                _value = value;
                this.Text = value.ToString(_numericFormat);
            }
        }

        // Property to get or set the numeric format.
        public string NumericFormat
        {
            get => _numericFormat;
            set
            {
                _numericFormat = value;
                // Apply the format to the existing value.
                this.Text = _value.ToString(_numericFormat);
            }
        }

        public object EditingControlFormattedValue
        {
            get => this.Text;
            set => this.Text = value?.ToString() ?? string.Empty;
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.Text;
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
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

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            // Handle arrow keys and other numeric keypad keys.
            return (keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Decimal);
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
            {
                this.SelectAll();
            }
        }

        public bool RepositionEditingControlOnValueChange => false;

        public Cursor EditingPanelCursor => base.Cursor;

        public bool EditingControlValueChanged
        {
            get => valueChanged;
            set => valueChanged = value;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            valueChanged = true;
            base.OnTextChanged(e);
        }
    }
}
