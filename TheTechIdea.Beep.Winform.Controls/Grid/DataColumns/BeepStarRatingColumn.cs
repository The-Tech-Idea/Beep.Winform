using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepStarRatingColumn : DataGridViewColumn
    {
        public BeepStarRatingColumn() : base(new BeepStarRatingCell())
        {
            this.CellTemplate = new BeepStarRatingCell();
        }
    }
    public class BeepStarRatingCell : DataGridViewCell
    {
        private BeepStarRating beepStarRating;

        public BeepStarRatingCell()
        {
            beepStarRating = new BeepStarRating
            {
                Size = new Size(120, 30), // Default size, dynamically adjusted
                StarCount = 5,
                SelectedRating = 0
            };
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, formattedValue, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            int ratingValue = 0;
            if (value != null && int.TryParse(value.ToString(), out ratingValue))
            {
                beepStarRating.SelectedRating = ratingValue;
            }
            else
            {
                beepStarRating.SelectedRating = 0;
            }

            beepStarRating.Bounds = cellBounds;
            beepStarRating.DrawToBitmap(new Bitmap(cellBounds.Width, cellBounds.Height), cellBounds);
            graphics.DrawImage(beepStarRating.BackgroundImage, cellBounds.Location);
        }

        protected override object GetValue(int rowIndex)
        {
            return beepStarRating.SelectedRating;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value != null && int.TryParse(value.ToString(), out int newValue))
            {
                beepStarRating.SelectedRating = newValue;
                return true; // Successfully updated the value
            }
            return false; // Indicate that setting the value was unsuccessful
        }
    }
}
