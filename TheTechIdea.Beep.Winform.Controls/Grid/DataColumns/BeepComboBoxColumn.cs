using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepComboBoxColumn : DataGridViewColumn
    {
        public BeepComboBoxColumn() : base(new BeepComboBoxCell())
        {
            this.CellTemplate = new BeepComboBoxCell();
        }
    }

    public class BeepComboBoxCell : DataGridViewComboBoxCell
    {
        private BeepComboBox beepComboBox;

        public BeepComboBoxCell()
        {
            beepComboBox = new BeepComboBox
            {
                Size = new Size(120, 30),
                BackColor = Color.White
            };

            beepComboBox.SelectedItemChanged += BeepComboBox_SelectedItemChanged;
        }

        private void BeepComboBox_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (this.DataGridView != null && this.RowIndex >= 0)
            {
                this.DataGridView.CurrentCell = this;
                this.DataGridView.NotifyCurrentCellDirty(true);
                this.Value = e.SelectedItem.Text;
            }
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            base.OnMouseEnter(rowIndex);
            ShowBeepComboBox(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            HideBeepComboBox();
        }

        private void ShowBeepComboBox(int rowIndex)
        {
            if (this.DataGridView == null || rowIndex < 0)
                return;

            Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, true);
            beepComboBox.Size = new Size(cellBounds.Width - 4, cellBounds.Height - 4);
            beepComboBox.Location = new Point(cellBounds.X + 2, cellBounds.Y + 2);

            if (!this.DataGridView.Controls.Contains(beepComboBox))
            {
                this.DataGridView.Controls.Add(beepComboBox);
            }
        }

        private void HideBeepComboBox()
        {
            if (this.DataGridView != null && this.DataGridView.Controls.Contains(beepComboBox))
            {
                this.DataGridView.Controls.Remove(beepComboBox);
            }
        }

        protected override object GetValue(int rowIndex)
        {
            return beepComboBox.SelectedItem?.Text ?? string.Empty;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value is string textValue)
            {
                var item = beepComboBox.ListItems.FirstOrDefault(i => i.Text == textValue);
                if (item != null)
                {
                    beepComboBox.SelectedItem = item;
                    return true;
                }
            }
            return false;
        }
    }
}
