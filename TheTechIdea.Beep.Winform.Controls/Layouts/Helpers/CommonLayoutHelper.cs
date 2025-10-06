using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    public static class CommonLayoutHelper
    {
        public static void AddTableCellLabel(TableLayoutPanel table, string text, int column, int row)
        {
            var label = new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            table.Controls.Add(label, column, row);
        }

        public static void AddDetailRow(TableLayoutPanel table, int rowIndex, string labelText, string valueText)
        {
            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight
            };
            var valueLabel = new Label
            {
                Text = valueText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            table.Controls.Add(label, 0, rowIndex);
            table.Controls.Add(valueLabel, 1, rowIndex);
        }
    }
}
