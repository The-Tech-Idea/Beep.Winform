using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    public static class InvoiceLayoutHelper
    {
        public static Control Build(Control parent)
        {
            var mainPanel = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(mainPanel);

            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = SystemColors.ControlLight };
            mainPanel.Controls.Add(headerPanel);

            var invoiceLabel = new Label
            {
                Text = "Invoice # 12345",
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            headerPanel.Controls.Add(invoiceLabel);

            var contentPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                ColumnCount = 4,
                RowCount = 1
            };
            for (int i = 0; i < 4; i++) contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            CommonLayoutHelper.AddTableCellLabel(contentPanel, "Item", 0, 0);
            CommonLayoutHelper.AddTableCellLabel(contentPanel, "Description", 1, 0);
            CommonLayoutHelper.AddTableCellLabel(contentPanel, "Quantity", 2, 0);
            CommonLayoutHelper.AddTableCellLabel(contentPanel, "Price", 3, 0);
            mainPanel.Controls.Add(contentPanel);

            var footerPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = SystemColors.ControlDark };
            var totalLabel = new Label
            {
                Text = "Total: $0.00",
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 15)
            };
            footerPanel.Controls.Add(totalLabel);
            mainPanel.Controls.Add(footerPanel);

            return mainPanel;
        }
    }
}
