using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls; // Ensure correct namespace for BeepProgressBar

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    [ToolboxItem(true)]
    public class BeepDataGridViewProgressBarColumn : DataGridViewColumn
    {
        public BeepDataGridViewProgressBarColumn() : base(new BeepDataGridViewProgressBarCell())
        {
        }

        public Color ProgressBarColor { get; set; } = Color.LightGreen;

        private int _minimum = 0;
        public int Minimum
        {
            get => _minimum;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(Minimum), "Minimum must be ≥ 0.");
                _minimum = value;
            }
        }

        private int _maximum = 100;
        public int Maximum
        {
            get => _maximum;
            set
            {
                if (value <= _minimum) throw new ArgumentOutOfRangeException(nameof(Maximum), "Maximum must be greater than Minimum.");
                _maximum = value;
            }
        }

        private int _step = 1;
        public int Step
        {
            get => _step;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(Step), "Step must be > 0.");
                _step = value;
            }
        }

        public override object Clone()
        {
            var clone = (BeepDataGridViewProgressBarColumn)base.Clone();
            clone.ProgressBarColor = this.ProgressBarColor;
            clone.Minimum = this.Minimum;
            clone.Maximum = this.Maximum;
            clone.Step = this.Step;
            return clone;
        }
    }

    public class BeepDataGridViewProgressBarCell : DataGridViewTextBoxCell
    {
        public BeepDataGridViewProgressBarCell()
        {
            this.ValueType = typeof(int);
        }

        public override Type EditType => typeof(BeepDataGridViewProgressBarEditingControl);
        public override Type ValueType => typeof(int);
        public override object DefaultNewRowValue => 0;

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
                                      DataGridViewElementStates cellState, object value, object formattedValue, string errorText,
                                      DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {
            BeepDataGridViewProgressBarColumn owningColumn = this.OwningColumn as BeepDataGridViewProgressBarColumn;

            int minimum = owningColumn?.Minimum ?? 0;
            int maximum = owningColumn?.Maximum ?? 100;
            int progressValue = value != null && int.TryParse(value.ToString(), out int result) ? result : minimum;
            progressValue = Math.Max(minimum, Math.Min(maximum, progressValue));

            float percentage = (float)(progressValue - minimum) / (maximum - minimum);
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, null, null, errorText, cellStyle, advancedBorderStyle, (paintParts & ~DataGridViewPaintParts.ContentForeground));

            Color progressBarColor = owningColumn?.ProgressBarColor ?? Color.LightGreen;
            using (Brush progressBarBrush = new SolidBrush(progressBarColor))
            {
                int barWidth = (int)(percentage * (cellBounds.Width - 4));
                Rectangle progressBarRect = new Rectangle(cellBounds.X + 2, cellBounds.Y + 2, barWidth, cellBounds.Height - 4);
                graphics.FillRectangle(progressBarBrush, progressBarRect);
            }

            string text = $"{progressValue}%";
            SizeF textSize = graphics.MeasureString(text, cellStyle.Font);
            using (Brush textBrush = new SolidBrush(cellStyle.ForeColor))
            {
                float textX = cellBounds.X + (cellBounds.Width - textSize.Width) / 2;
                float textY = cellBounds.Y + (cellBounds.Height - textSize.Height) / 2;
                graphics.DrawString(text, cellStyle.Font, textBrush, textX, textY);
            }
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepDataGridViewProgressBarEditingControl control)
            {
                control.Value = initialFormattedValue != null ? Convert.ToInt32(initialFormattedValue) : 0;
            }
        }
    }

    public class BeepDataGridViewProgressBarEditingControl : BeepProgressBar, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepDataGridViewProgressBarEditingControl()
        {
            this.Minimum = 0;
            this.Maximum = 100;
            this.Step = 1;
            this.BackColor = Color.White;
            // Subscribe to the ValueChanged event correctly
            this.OnValueChanged += BeepDataGridViewProgressBarEditingControl_ValueChanged;
        }

        public object EditingControlFormattedValue
        {
            get => this.Value;
            set => this.Value = Convert.ToInt32(value);
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.Value;

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
        // Proper event handler method
        private void BeepDataGridViewProgressBarEditingControl_ValueChanged(object sender, EventArgs e)
        {
            valueChanged = true;

            if (dataGridView != null)
            {
                dataGridView.NotifyCurrentCellDirty(true);
            }
        }


    }
}
