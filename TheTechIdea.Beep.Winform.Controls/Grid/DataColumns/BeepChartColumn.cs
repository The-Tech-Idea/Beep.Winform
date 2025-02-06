using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules; // Ensure correct namespace for BeepChartBase

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepChartColumn : DataGridViewColumn
    {
        public BeepChartColumn() : base(new BeepChartCell())
        {
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }

    public class BeepChartCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(BeepChartEditingControl); // Use BeepChartBase for editing
        public override Type ValueType => typeof(List<ChartDataSeries>); // Store data as a list of series
        public override object DefaultNewRowValue => new List<ChartDataSeries>(); // Default empty chart data

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            if (DataGridView.EditingControl is BeepChartEditingControl control)
            {
                if (initialFormattedValue is List<ChartDataSeries> series)
                {
                    control.DataSeries = series;
                }
                else
                {
                    control.DataSeries = new List<ChartDataSeries>();
                }
                control.Invalidate(); // Redraw chart with new data
            }
        }
    }

    public class BeepChartEditingControl : BeepChartBase, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private int rowIndex;
        private bool valueChanged;

        public BeepChartEditingControl()
        {
            this.Size = new Size(150, 80); // Adjust the default size
            this.ChartType = ChartType.Line;
            this.ShowLegend = false; // Hide legend for compact view
        }

        public object EditingControlFormattedValue
        {
            get => this.DataSeries;
            set
            {
                if (value is List<ChartDataSeries> series)
                {
                    this.DataSeries = series;
                    this.Invalidate();
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) => this.DataSeries;

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

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            // Open a custom chart data editor (if needed)
            MessageBox.Show($"Editing Chart in row {rowIndex}");

            valueChanged = true;
            dataGridView?.NotifyCurrentCellDirty(true);
        }
    }
}
