using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepImageColumn : DataGridViewColumn
    {
        public BeepImageColumn() : base(new BeepImageCell())
        {
            this.CellTemplate = new BeepImageCell();
        }
    }
    public class BeepImageCell : DataGridViewCell
    {
        private BeepImage beepImage;

        public BeepImageCell()
        {
            beepImage = new BeepImage
            {
                ScaleMode= Vis.Modules.ImageScaleMode.KeepAspectRatio,
                Size = new Size(50, 50) // Default size, adjusted dynamically
            };
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            base.OnMouseEnter(rowIndex);
            ShowBeepImage(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            HideBeepImage();
        }

        private void ShowBeepImage(int rowIndex)
        {
            if (this.DataGridView == null || rowIndex < 0)
                return;

            Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, true);
            beepImage.Size = new Size(cellBounds.Width - 4, cellBounds.Height - 4);
            beepImage.Location = new Point(cellBounds.X + 2, cellBounds.Y + 2);

            if (!this.DataGridView.Controls.Contains(beepImage))
            {
                this.DataGridView.Controls.Add(beepImage);
            }
        }

        private void HideBeepImage()
        {
            if (this.DataGridView != null && this.DataGridView.Controls.Contains(beepImage))
            {
                this.DataGridView.Controls.Remove(beepImage);
            }
        }

        protected override object GetValue(int rowIndex)
        {
            return beepImage.ImagePath;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value != null)
            {
                beepImage.ImagePath = value.ToString();
                return true; // Successfully updated the value
            }
            return false; // Indicate that setting the value was unsuccessful
        }
    }
}
