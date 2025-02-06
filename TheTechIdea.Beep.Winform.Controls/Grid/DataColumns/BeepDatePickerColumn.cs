using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepDatePickerColumn : DataGridViewColumn
    {
        public BeepDatePickerColumn() : base(new BeepDatePickerCell())
        {
            this.CellTemplate = new BeepDatePickerCell();
        }
    }

    public class BeepDatePickerCell : DataGridViewTextBoxCell
    {
        private BeepDatePicker beepDatePicker;

        public BeepDatePickerCell()
        {
            beepDatePicker = new BeepDatePicker
            {
                Size = new Size(120, 30),
                BackColor = Color.White
            };

            beepDatePicker.TextChanged += BeepDatePicker_TextChanged;
        }

        private void BeepDatePicker_TextChanged(object sender, EventArgs e)
        {
            if (this.DataGridView != null && this.RowIndex >= 0)
            {
                this.DataGridView.CurrentCell = this;
                this.DataGridView.NotifyCurrentCellDirty(true);
                this.Value = beepDatePicker.SelectedDate;
            }
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            base.OnMouseEnter(rowIndex);
            ShowBeepDatePicker(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            HideBeepDatePicker();
        }

        private void ShowBeepDatePicker(int rowIndex)
        {
            if (this.DataGridView == null || rowIndex < 0)
                return;

            Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, true);
            beepDatePicker.Size = new Size(cellBounds.Width - 4, cellBounds.Height - 4);
            beepDatePicker.Location = new Point(cellBounds.X + 2, cellBounds.Y + 2);

            if (!this.DataGridView.Controls.Contains(beepDatePicker))
            {
                this.DataGridView.Controls.Add(beepDatePicker);
            }
        }

        private void HideBeepDatePicker()
        {
            if (this.DataGridView != null && this.DataGridView.Controls.Contains(beepDatePicker))
            {
                this.DataGridView.Controls.Remove(beepDatePicker);
            }
        }

        protected override object GetValue(int rowIndex)
        {
            return beepDatePicker.SelectedDate ?? string.Empty;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value is string dateValue)
            {
                beepDatePicker.SelectedDate = dateValue;
                return true;
            }
            return false;
        }
    }
}
