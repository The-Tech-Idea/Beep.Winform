using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepListofValuesBoxColumn : DataGridViewColumn
    {
        public BeepListofValuesBoxColumn() : base(new BeepListofValuesBoxCell())
        {
            this.CellTemplate = new BeepListofValuesBoxCell();
        }
    }
    public class BeepListofValuesBoxCell : DataGridViewCell
    {
        private BeepListofValuesBox beepListofValuesBox;

        public BeepListofValuesBoxCell()
        {
            beepListofValuesBox = new BeepListofValuesBox
            {
                Size = new Size(150, 30), // Default size, adjusted dynamically
            };

            beepListofValuesBox.OnSelected += BeepListofValuesBox_SelectedIndexChanged;
        }

        private void BeepListofValuesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.DataGridView != null && this.RowIndex >= 0)
            {
                this.DataGridView.CurrentCell = this;
                this.DataGridView.NotifyCurrentCellDirty(true); // Notify that cell data has changed
                this.Value = beepListofValuesBox.SelectedKey; // Store the key value in the DataGridView
            }
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            base.OnMouseEnter(rowIndex);
            ShowBeepListofValuesBox(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            HideBeepListofValuesBox();
        }

        private void ShowBeepListofValuesBox(int rowIndex)
        {
            if (this.DataGridView == null || rowIndex < 0)
                return;

            Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, true);
            beepListofValuesBox.Size = new Size(cellBounds.Width - 4, cellBounds.Height - 4);
            beepListofValuesBox.Location = new Point(cellBounds.X + 2, cellBounds.Y + 2);

            if (!this.DataGridView.Controls.Contains(beepListofValuesBox))
            {
                this.DataGridView.Controls.Add(beepListofValuesBox);
            }
        }

        private void HideBeepListofValuesBox()
        {
            if (this.DataGridView != null && this.DataGridView.Controls.Contains(beepListofValuesBox))
            {
                this.DataGridView.Controls.Remove(beepListofValuesBox);
            }
        }

        protected override object GetValue(int rowIndex)
        {
            return beepListofValuesBox.SelectedKey;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value != null)
            {
                beepListofValuesBox.SelectedKey = value.ToString();
                return true; // Successfully updated the value
            }
            return false; // Indicate that setting the value was unsuccessful
        }
    }
}
