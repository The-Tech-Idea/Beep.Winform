using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    public static class GridLayoutHelper
    {
        public static Control Build(Control parent, int rows, int columns)
        {
            var table = new TableLayoutPanel { RowCount = rows, ColumnCount = columns, Dock = DockStyle.Fill, CellBorderStyle = TableLayoutPanelCellBorderStyle.Single };
            for (int c = 0; c < columns; c++) table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));
            for (int r = 0; r < rows; r++) table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    var cell = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
                    table.Controls.Add(cell, c, r);
                }
            }
            parent.Controls.Add(table);
            return table;
        }
    }
}
