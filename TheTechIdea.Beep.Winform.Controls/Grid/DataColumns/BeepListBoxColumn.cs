using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepListBoxColumn : DataGridViewColumn
    {
        public BeepListBoxColumn() : base(new BeepListBoxCell())
        {
            this.CellTemplate = new BeepListBoxCell();
        }
    }
    public class BeepListBoxCell : DataGridViewCell
    {
        private BeepListBox beepListBox;

        public BeepListBoxCell()
        {
            beepListBox = new BeepListBox
            {
                Size = new Size(120, 100), // Default size, adjusted dynamically
                Collapsed = true, // Start as collapsed
                ShowCheckBox = false, // By default, no checkboxes
            };

            beepListBox.ItemClicked += BeepListBox_ItemClicked;
        }

        private void BeepListBox_ItemClicked(object sender, SimpleItem selectedItem)
        {
            if (this.DataGridView != null && this.RowIndex >= 0)
            {
                this.DataGridView.CurrentCell = this;
                this.DataGridView.NotifyCurrentCellDirty(true); // Notify that cell data has changed
                this.Value = selectedItem.Text;
            }
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            base.OnMouseEnter(rowIndex);
            ShowBeepListBox(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            HideBeepListBox();
        }

        private void ShowBeepListBox(int rowIndex)
        {
            if (this.DataGridView == null || rowIndex < 0)
                return;

            Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, true);
            beepListBox.Size = new Size(cellBounds.Width - 4, Math.Max(60, cellBounds.Height)); // Adjust height
            beepListBox.Location = new Point(cellBounds.X + 2, cellBounds.Y + 2);

            if (!this.DataGridView.Controls.Contains(beepListBox))
            {
                this.DataGridView.Controls.Add(beepListBox);
            }
        }

        private void HideBeepListBox()
        {
            if (this.DataGridView != null && this.DataGridView.Controls.Contains(beepListBox))
            {
                this.DataGridView.Controls.Remove(beepListBox);
            }
        }

        protected override object GetValue(int rowIndex)
        {
            return beepListBox.SelectedItem?.Text;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value != null && beepListBox.ListItems.Any(i => i.Text == value.ToString()))
            {
                beepListBox.SelectedItem = beepListBox.ListItems.First(i => i.Text == value.ToString());
                return true; // Successfully updated the value
            }
            return false; // Indicate that setting the value was unsuccessful
        }
    }
}
