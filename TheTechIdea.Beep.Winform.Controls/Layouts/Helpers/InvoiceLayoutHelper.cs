using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Helper for creating invoice layout templates.
    /// Creates a layout with header (invoice number), content area (table), and footer (totals).
    /// </summary>
    public static class InvoiceLayoutHelper
    {
        /// <summary>
        /// Builds an invoice layout with default options.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <returns>The main Panel containing the invoice layout.</returns>
        public static Control Build(Control parent)
        {
            return Build(parent, LayoutOptions.Default);
        }

        /// <summary>
        /// Builds an invoice layout with theme-aware styling.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="options">Layout configuration options for theming and styling.</param>
        /// <returns>The main Panel containing the invoice layout.</returns>
        public static Control Build(Control parent, LayoutOptions options)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            options = options ?? LayoutOptions.Default;

            var mainPanel = BaseLayoutHelper.CreateStyledPanel(options);
            mainPanel.Dock = DockStyle.Fill;
            parent.Controls.Add(mainPanel);

            var headerPanel = BaseLayoutHelper.CreateStyledPanel(options);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 100;
            headerPanel.BackColor = BaseLayoutHelper.GetBackgroundColor(options);
            mainPanel.Controls.Add(headerPanel);

            var invoiceLabel = BaseLayoutHelper.CreateStyledLabel("Invoice # 12345", options, ContentAlignment.TopLeft);
            invoiceLabel.AutoSize = true;
            invoiceLabel.Font = BaseLayoutHelper.GetFont(options, 14f);
            invoiceLabel.Font = new Font(invoiceLabel.Font, FontStyle.Bold);
            invoiceLabel.Location = new Point(10, 10);
            headerPanel.Controls.Add(invoiceLabel);

            var contentPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                CellBorderStyle = options.ShowBorders ? TableLayoutPanelCellBorderStyle.Single : TableLayoutPanelCellBorderStyle.None,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = BaseLayoutHelper.GetBackgroundColor(options)
            };
            for (int i = 0; i < 4; i++) 
                contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            
            CommonLayoutHelper.AddTableCellLabel(contentPanel, "Item", 0, 0, options);
            CommonLayoutHelper.AddTableCellLabel(contentPanel, "Description", 1, 0, options);
            CommonLayoutHelper.AddTableCellLabel(contentPanel, "Quantity", 2, 0, options);
            CommonLayoutHelper.AddTableCellLabel(contentPanel, "Price", 3, 0, options);
            mainPanel.Controls.Add(contentPanel);

            var footerPanel = BaseLayoutHelper.CreateStyledPanel(options);
            footerPanel.Dock = DockStyle.Bottom;
            footerPanel.Height = 50;
            footerPanel.BackColor = BaseLayoutHelper.GetBackgroundColor(options);
            
            var totalLabel = BaseLayoutHelper.CreateStyledLabel("Total: $0.00", options, ContentAlignment.MiddleLeft);
            totalLabel.AutoSize = true;
            totalLabel.Font = BaseLayoutHelper.GetFont(options, 12f);
            totalLabel.Font = new Font(totalLabel.Font, FontStyle.Bold);
            totalLabel.Location = new Point(10, 15);
            footerPanel.Controls.Add(totalLabel);
            mainPanel.Controls.Add(footerPanel);

            return mainPanel;
        }
    }
}
