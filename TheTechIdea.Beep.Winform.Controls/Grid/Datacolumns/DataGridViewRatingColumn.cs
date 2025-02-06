using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls; // Ensure correct namespace for BeepStarRating

namespace TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns
{
    [ToolboxItem(true)]
    public class BeepDataGridViewRatingColumn : DataGridViewColumn
    {
        public BeepDataGridViewRatingColumn() : base(new BeepDataGridViewRatingCell())
        {
        }

        public override object Clone()
        {
            var clone = (BeepDataGridViewRatingColumn)base.Clone();
            return clone;
        }
    }

    public class BeepDataGridViewRatingCell : DataGridViewTextBoxCell
    {
        public BeepDataGridViewRatingCell()
        {
            this.ValueType = typeof(int); // Rating value (0 to N stars)
        }

        public override Type EditType => typeof(BeepDataGridViewRatingEditingControl);
        public override Type ValueType => typeof(int);
        public override object DefaultNewRowValue => 0;

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                                      DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
                                      DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {
            int ratingValue = value != null && int.TryParse(value.ToString(), out int result) ? result : 0;

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, null, null, errorText, cellStyle, advancedBorderStyle,
                      (paintParts & ~DataGridViewPaintParts.ContentForeground));

            using (BeepStarRating starRating = new BeepStarRating())
            {
                starRating.SelectedRating = ratingValue;
                starRating.StarSize = Math.Min(cellBounds.Height - 6, 20);
                starRating.StarCount = 5;
                starRating.Spacing = 3;
                starRating.FilledStarColor = Color.Gold;
                starRating.EmptyStarColor = Color.Gray;

                using (Bitmap bmp = new Bitmap(cellBounds.Width, cellBounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(cellStyle.BackColor);
                        starRating.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    }
                    graphics.DrawImage(bmp, cellBounds.Location);
                }
            }
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepDataGridViewRatingEditingControl control)
            {
                control.SelectedRating = initialFormattedValue != null ? Convert.ToInt32(initialFormattedValue) : 0;
            }
        }
    }

    public class BeepDataGridViewRatingEditingControl : BeepStarRating, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepDataGridViewRatingEditingControl()
        {
            this.StarCount = 5;
            this.StarSize = 20;
            this.Spacing = 3;
            this.FilledStarColor = Color.Gold;
            this.EmptyStarColor = Color.Gray;

            this.RatingChanged += BeepDataGridViewRatingEditingControl_ValueChanged;
        }

        public object EditingControlFormattedValue
        {
            get => this.SelectedRating;
            set => this.SelectedRating = Convert.ToInt32(value);
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.SelectedRating;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        public DataGridView EditingControlDataGridView
        {
            get => dataGridView;
            set => dataGridView = value;
        }

        public int EditingControlRowIndex
        {
            get => rowIndex;
            set => rowIndex = value;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey) => true;

        public void PrepareEditingControlForEdit(bool selectAll) { }

        public bool RepositionEditingControlOnValueChange => false;

        public Cursor EditingPanelCursor => base.Cursor;

        public bool EditingControlValueChanged
        {
            get => valueChanged;
            set => valueChanged = value;
        }

        private void BeepDataGridViewRatingEditingControl_ValueChanged(object sender, EventArgs e)
        {
            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.RatingChanged -= BeepDataGridViewRatingEditingControl_ValueChanged;
            }
            base.Dispose(disposing);
        }
    }
}
