using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepButtonColumn : DataGridViewColumn
    {
        public BeepButtonColumn() : base(new BeepButtonCell())
        {
            this.CellTemplate = new BeepButtonCell();
        }
    }

    public class BeepButtonCell : DataGridViewCell
    {
        private BeepButton beepButton;

        public BeepButtonCell()
        {
            beepButton = new BeepButton
            {
                Size = new Size(100, 30),
                Text = "Click",
                BorderSize = 1,
                BorderColor = Color.Black,
                SelectedBorderColor = Color.Blue,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleCenter
            };

            beepButton.Click += BeepButton_Click;
        }

        private void BeepButton_Click(object sender, EventArgs e)
        {
            if (this.DataGridView != null && this.RowIndex >= 0)
            {
                this.DataGridView.CurrentCell = this;
                // invoke buttonclick event from beepbutton
               
            }
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            base.OnMouseEnter(rowIndex);
            ShowBeepButton(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            HideBeepButton();
        }

        private void ShowBeepButton(int rowIndex)
        {
            if (this.DataGridView == null || rowIndex < 0)
                return;

            Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, true);
            beepButton.Size = new Size(cellBounds.Width - 4, cellBounds.Height - 4);
            beepButton.Location = new Point(cellBounds.X + 2, cellBounds.Y + 2);

            if (!this.DataGridView.Controls.Contains(beepButton))
            {
                this.DataGridView.Controls.Add(beepButton);
            }
        }

        private void HideBeepButton()
        {
            if (this.DataGridView != null && this.DataGridView.Controls.Contains(beepButton))
            {
                this.DataGridView.Controls.Remove(beepButton);
            }
        }

        protected override object GetValue(int rowIndex)
        {
            return beepButton.Text;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value != null)
            {
                beepButton.Text = value.ToString();
                return true; // Successfully updated the value
            }
            return false; // Indicate that setting the value was unsuccessful
        }
    }
}
