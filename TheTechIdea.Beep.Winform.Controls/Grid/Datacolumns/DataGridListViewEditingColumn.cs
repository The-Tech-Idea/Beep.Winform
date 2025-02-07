using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(true)]
    public class BeepDataGridViewListBoxColumn : DataGridViewColumn
    {
        public BeepDataGridViewListBoxColumn() : base(new BeepDataGridViewListBoxCell())
        {
        }

        public override object Clone()
        {
            var clone = (BeepDataGridViewListBoxColumn)base.Clone();
            return clone;
        }
    }

    public class BeepDataGridViewListBoxCell : DataGridViewTextBoxCell
    {
        public BeepDataGridViewListBoxCell()
        {
            this.ValueType = typeof(string); // The selected item value
        }

        public override Type EditType => typeof(BeepDataGridViewListBoxEditingControl);
        public override Type ValueType => typeof(string);
        public override object DefaultNewRowValue => "";

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                                      DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
                                      DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {
            string displayText = value?.ToString() ?? "";

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, displayText, displayText, errorText, cellStyle, advancedBorderStyle,
                      (paintParts & ~DataGridViewPaintParts.ContentForeground));

            using (Brush textBrush = new SolidBrush(cellStyle.ForeColor))
            {
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };

                graphics.DrawString(displayText, cellStyle.Font, textBrush, cellBounds, format);
            }
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepDataGridViewListBoxEditingControl control)
            {
               // control.SelectedItem = initialFormattedValue;
            }
        }
    }

    public class BeepDataGridViewListBoxEditingControl : BeepListBox, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepDataGridViewListBoxEditingControl()
        {
            this.SelectedItemChanged += BeepDataGridViewListBoxEditingControl_SelectedItemChanged;
        }

        public object EditingControlFormattedValue
        {
            get => this.SelectedItem?.Text ?? "";
            set => this.SelectedItem = this.ListItems.FirstOrDefault(item => item.Text == value?.ToString());
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => EditingControlFormattedValue;

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

        private void BeepDataGridViewListBoxEditingControl_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.SelectedItemChanged -= BeepDataGridViewListBoxEditingControl_SelectedItemChanged;
            }
            base.Dispose(disposing);
        }
    }
}
