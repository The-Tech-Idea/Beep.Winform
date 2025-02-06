
using TheTechIdea.Beep.Editor;


namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepExtendedButtonColumn : DataGridViewColumn
    {
        public BeepExtendedButtonColumn() : base(new BeepExtendedButtonCell())
        {
            this.CellTemplate = new BeepExtendedButtonCell();
        }
    }

    public class BeepExtendedButtonCell : DataGridViewTextBoxCell
    {
        private BeepExtendedButton beepExtendedButton;

        public BeepExtendedButtonCell()
        {
            beepExtendedButton = new BeepExtendedButton
            {
                Size = new Size(200, 30),
                BackColor = Color.White
            };

            beepExtendedButton.ButtonClick += BeepExtendedButton_Click;
            beepExtendedButton.ExtendButtonClick += BeepExtendedButton_ExtendClick;
        }

        private void BeepExtendedButton_Click(object sender, BeepEventDataArgs e)
        {
            if (this.DataGridView != null && this.RowIndex >= 0)
            {
                MessageBox.Show($"Button Clicked: {e.EventName}");
            }
        }

        private void BeepExtendedButton_ExtendClick(object sender, BeepEventDataArgs e)
        {
            if (this.DataGridView != null && this.RowIndex >= 0)
            {
                MessageBox.Show($"Extended Button Clicked: {e.EventName}");
            }
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            base.OnMouseEnter(rowIndex);
            ShowBeepExtendedButton(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            HideBeepExtendedButton();
        }

        private void ShowBeepExtendedButton(int rowIndex)
        {
            if (this.DataGridView == null || rowIndex < 0)
                return;

            Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, true);
            beepExtendedButton.Size = new Size(cellBounds.Width - 4, cellBounds.Height - 4);
            beepExtendedButton.Location = new Point(cellBounds.X + 2, cellBounds.Y + 2);

            if (!this.DataGridView.Controls.Contains(beepExtendedButton))
            {
                this.DataGridView.Controls.Add(beepExtendedButton);
            }
        }

        private void HideBeepExtendedButton()
        {
            if (this.DataGridView != null && this.DataGridView.Controls.Contains(beepExtendedButton))
            {
                this.DataGridView.Controls.Remove(beepExtendedButton);
            }
        }

        protected override object GetValue(int rowIndex)
        {
            return beepExtendedButton.Text;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value is string textValue)
            {
                beepExtendedButton.Text = textValue;
                return true;
            }
            return false;
        }
    }
}
