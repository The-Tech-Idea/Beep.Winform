using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepCheckBoxColumn : DataGridViewColumn
    {
        public BeepCheckBoxColumn() : base(new BeepCheckBoxCell())
        {
            this.CellTemplate = new BeepCheckBoxCell();
        }
    }

    public class BeepCheckBoxCell : DataGridViewCheckBoxCell
    {
        private BeepCheckBox<bool> beepCheckBox;

        public BeepCheckBoxCell()
        {
            beepCheckBox = new BeepCheckBox<bool>
            {
                CheckedValue = true,
                UncheckedValue = false,
                CurrentValue = false,
                Size = new Size(20, 20)
            };

            beepCheckBox.StateChanged += BeepCheckBox_StateChanged;
        }

        private void BeepCheckBox_StateChanged(object sender, EventArgs e)
        {
            if (this.DataGridView != null && this.RowIndex >= 0)
            {
                this.DataGridView.CurrentCell = this;
                this.DataGridView.NotifyCurrentCellDirty(true);
                this.Value = beepCheckBox.CurrentValue;
            }
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            base.OnMouseEnter(rowIndex);
            ShowBeepCheckBox(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            HideBeepCheckBox();
        }

        private void ShowBeepCheckBox(int rowIndex)
        {
            if (this.DataGridView == null || rowIndex < 0)
                return;

            Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, true);
            beepCheckBox.Size = new Size(cellBounds.Height - 4, cellBounds.Height - 4);
            beepCheckBox.Location = new Point(cellBounds.X + (cellBounds.Width - beepCheckBox.Width) / 2, cellBounds.Y + 2);

            if (!this.DataGridView.Controls.Contains(beepCheckBox))
            {
                this.DataGridView.Controls.Add(beepCheckBox);
            }
        }

        private void HideBeepCheckBox()
        {
            if (this.DataGridView != null && this.DataGridView.Controls.Contains(beepCheckBox))
            {
                this.DataGridView.Controls.Remove(beepCheckBox);
            }
        }

        protected override object GetValue(int rowIndex)
        {
            return beepCheckBox.CurrentValue;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value is bool boolValue)
            {
                beepCheckBox.CurrentValue = boolValue;
                return true;
            }
            return false;
        }
    }
}
