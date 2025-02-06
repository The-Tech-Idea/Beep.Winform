using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Grid.DataColumns
{
    public class BeepChartColumn : DataGridViewColumn
    {
        public BeepChartColumn() : base(new BeepChartCell())
        {
            this.CellTemplate = new BeepChartCell();
        }
    }

    public class BeepChartCell : DataGridViewCell
    {
        private BeepChartBase beepChart;

        public BeepChartCell()
        {
            beepChart = new BeepChartBase
            {
                Size = new Size(100, 50),
                ChartType = ChartType.Line,
                ShowLegend = false // Hide legend for small cells
            };
        }

        protected override void OnMouseEnter(int rowIndex)
        {
            base.OnMouseEnter(rowIndex);
            ShowBeepChart(rowIndex);
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            HideBeepChart();
        }

        private void ShowBeepChart(int rowIndex)
        {
            if (this.DataGridView == null || rowIndex < 0)
                return;

            Rectangle cellBounds = this.DataGridView.GetCellDisplayRectangle(this.ColumnIndex, rowIndex, true);
            beepChart.Size = new Size(cellBounds.Width - 4, cellBounds.Height - 4);
            beepChart.Location = new Point(cellBounds.X + 2, cellBounds.Y + 2);

            if (!this.DataGridView.Controls.Contains(beepChart))
            {
                this.DataGridView.Controls.Add(beepChart);
            }
        }

        private void HideBeepChart()
        {
            if (this.DataGridView != null && this.DataGridView.Controls.Contains(beepChart))
            {
                this.DataGridView.Controls.Remove(beepChart);
            }
        }

        protected override object GetValue(int rowIndex)
        {
            return beepChart.DataSeries;
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            if (value is List<ChartDataSeries> seriesList)
            {
                beepChart.DataSeries = seriesList;
                beepChart.Invalidate(); // Redraw chart
                return true;
            }
            return false;
        }
    }
}
