using System;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls;
using System.ComponentModel; // Ensure correct namespace for BeepStarRating

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(false)]
    public class BeepStarRatingColumn : DataGridViewColumn
    {
        public BeepStarRatingColumn() : base(new BeepStarRatingCell())
        {
        }
        public int MaxStars { get; set; } = 5; // Default to 5 stars
        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepStarRatingCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepStarRatingEditingControl); // Use BeepStarRating for editing
        public override Type ValueType => typeof(int); // Store selected rating as an integer
        public override object DefaultNewRowValue => 0; // Default to zero stars

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepStarRatingEditingControl control)
            {
                if (initialFormattedValue is int rating)
                {
                    control.SelectedRating = rating;
                }
                else
                {
                    control.SelectedRating = 0;
                }
            }
        }
    }

    public class BeepStarRatingEditingControl : BeepStarRating, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepStarRatingEditingControl()
        {
            this.Size = new Size(120, 30); // Default size
            this.BackColor = Color.White;

            // Handle rating change event
            this.RatingChanged += BeepStarRating_RatingChanged;
        }

        private void BeepStarRating_RatingChanged(object sender, EventArgs e)
        {
            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }

        public object EditingControlFormattedValue
        {
            get => this.SelectedRating;
            set
            {
                if (value is int rating)
                {
                    this.SelectedRating = rating;
                }
                else
                {
                    this.SelectedRating = 0;
                }
            }
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
    }
}
