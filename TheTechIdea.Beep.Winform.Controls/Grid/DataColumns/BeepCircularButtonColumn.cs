using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepCircularButtonColumn : DataGridViewColumn
    {
        public BeepCircularButtonColumn() : base(new BeepCircularButtonCell())
        {
            this.CellTemplate = new BeepCircularButtonCell();
        }
    }

    public class BeepCircularButtonCell : DataGridViewButtonCell
    {
        private BeepCircularButton beepCircularButton;

        public BeepCircularButtonCell()
        {
            beepCircularButton = new BeepCircularButton
            {
                Size = new Size(50, 50),
                Text = "Click",
                ShowBorder = true,
                TextLocation = TextLocation.Below
            };

            beepCircularButton.Click += BeepCircularButton_Click;
        }

        private void BeepCircularButton_Click(object sender, EventArgs e)
        {
            if (this.DataGridView != null && this.RowIndex >= 0)
            {
                this.DataGridView.CurrentCell = this;
                MessageBox.Show($"BeepCircularButton clicked in row {this.RowIndex}!");
            }
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            base.OnMouseEnter(rowIndex);
            ShowBeepCircularButton(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            HideBeepCircularButton();
        }

        private void ShowBeepCircularButton(int rowIndex)
        {
            if (this.DataGridView == null || rowIndex < 0)
                return;

            Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, true);
            beepCircularButton.Size = new Size(cellBounds.Width - 4, cellBounds.Height - 4);
            beepCircularButton.Location = new Point(cellBounds.X + 2, cellBounds.Y + 2);

            if (!this.DataGridView.Controls.Contains(beepCircularButton))
            {
                this.DataGridView.Controls.Add(beepCircularButton);
            }
        }

        private void HideBeepCircularButton()
        {
            if (this.DataGridView != null && this.DataGridView.Controls.Contains(beepCircularButton))
            {
                this.DataGridView.Controls.Remove(beepCircularButton);
            }
        }

        protected override object GetValue(int rowIndex)
        {
            return beepCircularButton.Text;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value != null)
            {
                beepCircularButton.Text = value.ToString();
                return true;
            }
            return false;
        }
    }
}
