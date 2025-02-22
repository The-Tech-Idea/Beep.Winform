using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
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

        // Override Dispose to ensure proper cleanup
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // Perform any additional cleanup here if needed
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error disposing BeepDataGridViewNumericColumn: {ex.Message}");
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }

    public class DataGridViewNumericCell : DataGridViewTextBoxCell
    {
        private string _format = "N2"; // Default format (numeric with 2 decimal places).

        public DataGridViewNumericCell() : base()
        {
            this.Style.Format = _format; // Set the default numeric format.
        }

        public override object Clone()
        {
            var clone = (DataGridViewNumericCell)base.Clone();
            clone._format = this._format;
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
            get => typeof(decimal); // Use decimal as the value type for numeric values.
        }

        public override object DefaultNewRowValue
        {
            get => 0m; // Default new row value to 0.
        }

        public override Type EditType
        {
            get => typeof(NumericEditingControl); // Use custom editing control.
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            var control = DataGridView?.EditingControl as NumericEditingControl;
            if (control != null)
            {
                // Safely handle null or invalid values
                control.NumericFormat = this.NumericFormat;
                control.Value = this.Value == null || this.Value == DBNull.Value ? 0m : Convert.ToDecimal(this.Value);
            }
        }

        // Override Dispose to ensure proper cleanup
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // Perform any additional cleanup here if needed
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error disposing DataGridViewNumericCell: {ex.Message}");
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
    [ToolboxItem(false)]
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

        public string NumericFormat
        {
            get => _numericFormat;
            set
            {
                _numericFormat = value;
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

        // Override Dispose to ensure proper cleanup
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    // Perform any additional cleanup here if needed
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error disposing NumericEditingControl: {ex.Message}");
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}