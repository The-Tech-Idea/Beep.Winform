using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.Managers
{
    public static class LayoutTemplateDesigner
    {
        // Supported layout template types.
        public enum TemplateType
        {
            Invoice,
            Product,
            Profile,
            Report,
            VerticalStack,
            HorizontalStack,
            Grid,
            SplitContainer,
            Dock
        }

        /// <summary>
        /// Applies the requested layout template to the given parent control.
        /// </summary>
        /// <param name="parent">The container control (form or panel) where the template is injected.</param>
        /// <param name="templateType">The desired layout template type.</param>
        /// <returns>The main control representing the injected layout.</returns>
        public static Control ApplyTemplate(Control parent, TemplateType templateType)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            switch (templateType)
            {
                case TemplateType.Invoice:
                    return AddInvoiceLayout(parent);
                case TemplateType.Product:
                    return AddProductLayout(parent);
                case TemplateType.Profile:
                    return AddProfileLayout(parent);
                case TemplateType.Report:
                    return AddReportLayout(parent);
                case TemplateType.VerticalStack:
                    return AddVerticalStackLayout(parent);
                case TemplateType.HorizontalStack:
                    return AddHorizontalStackLayout(parent);
                case TemplateType.Grid:
                    // Defaulting to a 3x3 grid – modify as necessary.
                    return AddGridLayout(parent, 3, 3);
                case TemplateType.SplitContainer:
                    return AddSplitContainerLayout(parent, Orientation.Vertical);
                case TemplateType.Dock:
                    return AddDockLayout(parent);
                default:
                    throw new ArgumentException("Unknown TemplateType", nameof(templateType));
            }
        }

        #region Practical Layout Templates

        /// <summary>
        /// Creates an Invoice layout template.
        /// Consists of a header (with invoice number), a content area (using a TableLayoutPanel for items),
        /// and a footer (for totals).
        /// </summary>
        private static Control AddInvoiceLayout(Control parent)
        {
            // Main container for the invoice layout.
            string mainPanelGuid;
            Control mainPanelControl = DesignerHelper.AddControl(parent, typeof(Panel), out mainPanelGuid);
            Panel mainPanel = mainPanelControl as Panel;
            if (mainPanel != null)
            {
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.BorderStyle = BorderStyle.FixedSingle;

                // ---------- Header ----------
                string headerPanelGuid;
                Control headerPanelControl = DesignerHelper.AddControl(mainPanel, typeof(Panel), out headerPanelGuid);
                Panel headerPanel = headerPanelControl as Panel;
                if (headerPanel != null)
                {
                    headerPanel.Dock = DockStyle.Top;
                    headerPanel.Height = 100;
                    headerPanel.BackColor = SystemColors.ControlLight;

                    // Invoice number label.
                    string invoiceLabelGuid;
                    Control invoiceLabelControl = DesignerHelper.AddControl(headerPanel, typeof(Label), out invoiceLabelGuid);
                    Label invoiceLabel = invoiceLabelControl as Label;
                    if (invoiceLabel != null)
                    {
                        invoiceLabel.Text = "Invoice # 12345";
                        invoiceLabel.Font = new Font("Arial", 14, FontStyle.Bold);
                        invoiceLabel.AutoSize = true;
                        invoiceLabel.Location = new Point(10, 10);
                    }
                }

                // ---------- Content Area ----------
                string contentPanelGuid;
                Control contentPanelControl = DesignerHelper.AddControl(mainPanel, typeof(TableLayoutPanel), out contentPanelGuid);
                TableLayoutPanel contentPanel = contentPanelControl as TableLayoutPanel;
                if (contentPanel != null)
                {
                    contentPanel.Dock = DockStyle.Fill;
                    contentPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                    contentPanel.ColumnCount = 4; // For example: Item, Description, Quantity, Price.
                    contentPanel.RowCount = 1;    // Header row.
                    contentPanel.ColumnStyles.Clear();
                    for (int i = 0; i < 4; i++)
                        contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

                    // Add header labels to each column.
                    AddTableCellLabel(contentPanel, "Item", 0, 0);
                    AddTableCellLabel(contentPanel, "Description", 1, 0);
                    AddTableCellLabel(contentPanel, "Quantity", 2, 0);
                    AddTableCellLabel(contentPanel, "Price", 3, 0);
                }

                // ---------- Footer ----------
                string footerPanelGuid;
                Control footerPanelControl = DesignerHelper.AddControl(mainPanel, typeof(Panel), out footerPanelGuid);
                Panel footerPanel = footerPanelControl as Panel;
                if (footerPanel != null)
                {
                    footerPanel.Dock = DockStyle.Bottom;
                    footerPanel.Height = 50;
                    footerPanel.BackColor = SystemColors.ControlDark;

                    // Total label.
                    string totalLabelGuid;
                    Control totalLabelControl = DesignerHelper.AddControl(footerPanel, typeof(Label), out totalLabelGuid);
                    Label totalLabel = totalLabelControl as Label;
                    if (totalLabel != null)
                    {
                        totalLabel.Text = "Total: $0.00";
                        totalLabel.Font = new Font("Arial", 12, FontStyle.Bold);
                        totalLabel.AutoSize = true;
                        totalLabel.Location = new Point(10, 15);
                    }
                }
            }
            return mainPanel;
        }

        /// <summary>
        /// Creates a Product layout template.
        /// Uses a SplitContainer to divide the screen with a product image on the left and product details on the right.
        /// </summary>
        private static Control AddProductLayout(Control parent)
        {
            // Main container panel.
            string mainPanelGuid;
            Control mainPanelControl = DesignerHelper.AddControl(parent, typeof(Panel), out mainPanelGuid);
            Panel mainPanel = mainPanelControl as Panel;
            if (mainPanel != null)
            {
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.BorderStyle = BorderStyle.FixedSingle;

                // SplitContainer divides image (left) and details (right).
                string splitContainerGuid;
                Control splitContainerControl = DesignerHelper.AddControl(mainPanel, typeof(SplitContainer), out splitContainerGuid);
                SplitContainer splitContainer = splitContainerControl as SplitContainer;
                if (splitContainer != null)
                {
                    splitContainer.Dock = DockStyle.Fill;
                    splitContainer.Orientation = Orientation.Vertical;
                    splitContainer.SplitterDistance = 200;

                    // Left Panel: PictureBox for product image.
                    string pictureBoxGuid;
                    Control pictureBoxControl = DesignerHelper.AddControl(splitContainer.Panel1, typeof(PictureBox), out pictureBoxGuid);
                    PictureBox productPicture = pictureBoxControl as PictureBox;
                    if (productPicture != null)
                    {
                        productPicture.Dock = DockStyle.Fill;
                        productPicture.BorderStyle = BorderStyle.FixedSingle;
                        productPicture.BackColor = Color.LightGray;
                        productPicture.SizeMode = PictureBoxSizeMode.Zoom;
                    }

                    // Right Panel: Product details.
                    Panel detailPanel = splitContainer.Panel2;

                    // Product Name.
                    string nameLabelGuid;
                    Control nameLabelControl = DesignerHelper.AddControl(detailPanel, typeof(Label), out nameLabelGuid);
                    Label nameLabel = nameLabelControl as Label;
                    if (nameLabel != null)
                    {
                        nameLabel.Text = "Product Name";
                        nameLabel.Font = new Font("Arial", 16, FontStyle.Bold);
                        nameLabel.AutoSize = true;
                        nameLabel.Location = new Point(10, 10);
                    }

                    // Product Price.
                    string priceLabelGuid;
                    Control priceLabelControl = DesignerHelper.AddControl(detailPanel, typeof(Label), out priceLabelGuid);
                    Label priceLabel = priceLabelControl as Label;
                    if (priceLabel != null)
                    {
                        priceLabel.Text = "Price: $0.00";
                        priceLabel.Font = new Font("Arial", 12, FontStyle.Regular);
                        priceLabel.AutoSize = true;
                        priceLabel.Location = new Point(10, 40);
                    }

                    // Product Description.
                    string descriptionGuid;
                    Control descriptionControl = DesignerHelper.AddControl(detailPanel, typeof(TextBox), out descriptionGuid);
                    TextBox descriptionBox = descriptionControl as TextBox;
                    if (descriptionBox != null)
                    {
                        descriptionBox.Multiline = true;
                        descriptionBox.Dock = DockStyle.Bottom;
                        descriptionBox.Height = 100;
                        descriptionBox.Text = "Product description goes here...";
                    }
                }
            }
            return mainPanel;
        }

        /// <summary>
        /// Creates a Profile layout template.
        /// Contains a top section for the profile picture and name, and a details section for additional info.
        /// </summary>
        private static Control AddProfileLayout(Control parent)
        {
            // Main container panel.
            string mainPanelGuid;
            Control mainPanelControl = DesignerHelper.AddControl(parent, typeof(Panel), out mainPanelGuid);
            Panel mainPanel = mainPanelControl as Panel;
            if (mainPanel != null)
            {
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.BorderStyle = BorderStyle.FixedSingle;

                // Top Panel: Profile picture and name.
                string topPanelGuid;
                Control topPanelControl = DesignerHelper.AddControl(mainPanel, typeof(Panel), out topPanelGuid);
                Panel topPanel = topPanelControl as Panel;
                if (topPanel != null)
                {
                    topPanel.Dock = DockStyle.Top;
                    topPanel.Height = 150;
                    topPanel.BackColor = Color.LightBlue;

                    // Profile Picture.
                    string picBoxGuid;
                    Control picBoxControl = DesignerHelper.AddControl(topPanel, typeof(PictureBox), out picBoxGuid);
                    PictureBox profilePic = picBoxControl as PictureBox;
                    if (profilePic != null)
                    {
                        profilePic.Width = 130;
                        profilePic.Height = 130;
                        profilePic.Location = new Point(10, 10);
                        profilePic.BorderStyle = BorderStyle.FixedSingle;
                        profilePic.BackColor = Color.Gray;
                        profilePic.SizeMode = PictureBoxSizeMode.Zoom;
                    }

                    // Profile Name.
                    string nameLabelGuid;
                    Control nameLabelControl = DesignerHelper.AddControl(topPanel, typeof(Label), out nameLabelGuid);
                    Label nameLabel = nameLabelControl as Label;
                    if (nameLabel != null)
                    {
                        nameLabel.Text = "User Name";
                        nameLabel.Font = new Font("Arial", 20, FontStyle.Bold);
                        nameLabel.AutoSize = true;
                        nameLabel.Location = new Point(150, 50);
                    }
                }

                // Details Panel: Using a TableLayoutPanel to list additional details.
                string detailsPanelGuid;
                Control detailsPanelControl = DesignerHelper.AddControl(mainPanel, typeof(TableLayoutPanel), out detailsPanelGuid);
                TableLayoutPanel detailsPanel = detailsPanelControl as TableLayoutPanel;
                if (detailsPanel != null)
                {
                    detailsPanel.Dock = DockStyle.Fill;
                    detailsPanel.ColumnCount = 2;
                    detailsPanel.RowCount = 3;
                    detailsPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                    detailsPanel.ColumnStyles.Clear();
                    detailsPanel.RowStyles.Clear();
                    detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
                    detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
                    for (int i = 0; i < 3; i++)
                        detailsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));

                    // Add sample detail rows (e.g., Email, Phone, Address).
                    AddDetailRow(detailsPanel, 0, "Email:", "user@example.com");
                    AddDetailRow(detailsPanel, 1, "Phone:", "123-456-7890");
                    AddDetailRow(detailsPanel, 2, "Address:", "123 Main St, City, Country");
                }
            }
            return mainPanel;
        }

        /// <summary>
        /// Creates a Report layout template.
        /// Comprises a header (title and filter), a content panel (for a DataGridView or similar control),
        /// and a footer for summary information.
        /// </summary>
        private static Control AddReportLayout(Control parent)
        {
            // Main container panel.
            string mainPanelGuid;
            Control mainPanelControl = DesignerHelper.AddControl(parent, typeof(Panel), out mainPanelGuid);
            Panel mainPanel = mainPanelControl as Panel;
            if (mainPanel != null)
            {
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.BorderStyle = BorderStyle.FixedSingle;

                // Header Panel: Title and filter.
                string headerPanelGuid;
                Control headerPanelControl = DesignerHelper.AddControl(mainPanel, typeof(Panel), out headerPanelGuid);
                Panel headerPanel = headerPanelControl as Panel;
                if (headerPanel != null)
                {
                    headerPanel.Dock = DockStyle.Top;
                    headerPanel.Height = 100;
                    headerPanel.BackColor = Color.LightGray;

                    // Report Title.
                    string titleLabelGuid;
                    Control titleLabelControl = DesignerHelper.AddControl(headerPanel, typeof(Label), out titleLabelGuid);
                    Label titleLabel = titleLabelControl as Label;
                    if (titleLabel != null)
                    {
                        titleLabel.Text = "Report Title";
                        titleLabel.Font = new Font("Arial", 18, FontStyle.Bold);
                        titleLabel.AutoSize = true;
                        titleLabel.Location = new Point(10, 10);
                    }

                    // Filter label and textbox.
                    string filterLabelGuid;
                    Control filterLabelControl = DesignerHelper.AddControl(headerPanel, typeof(Label), out filterLabelGuid);
                    Label filterLabel = filterLabelControl as Label;
                    if (filterLabel != null)
                    {
                        filterLabel.Text = "Filter:";
                        filterLabel.AutoSize = true;
                        filterLabel.Location = new Point(10, 50);
                    }
                    string filterBoxGuid;
                    Control filterBoxControl = DesignerHelper.AddControl(headerPanel, typeof(TextBox), out filterBoxGuid);
                    TextBox filterBox = filterBoxControl as TextBox;
                    if (filterBox != null)
                    {
                        filterBox.Location = new Point(60, 45);
                        filterBox.Width = 150;
                    }
                }

                // Content Panel: Placeholder for a DataGridView.
                string contentPanelGuid;
                Control contentPanelControl = DesignerHelper.AddControl(mainPanel, typeof(Panel), out contentPanelGuid);
                Panel contentPanel = contentPanelControl as Panel;
                if (contentPanel != null)
                {
                    contentPanel.Dock = DockStyle.Fill;
                    contentPanel.BackColor = Color.White;

                    string gridGuid;
                    Control gridControl = DesignerHelper.AddControl(contentPanel, typeof(DataGridView), out gridGuid);
                    DataGridView reportGrid = gridControl as DataGridView;
                    if (reportGrid != null)
                    {
                        reportGrid.Dock = DockStyle.Fill;
                        reportGrid.AutoGenerateColumns = true;
                    }
                }

                // Footer Panel: Summary information.
                string footerPanelGuid;
                Control footerPanelControl = DesignerHelper.AddControl(mainPanel, typeof(Panel), out footerPanelGuid);
                Panel footerPanel = footerPanelControl as Panel;
                if (footerPanel != null)
                {
                    footerPanel.Dock = DockStyle.Bottom;
                    footerPanel.Height = 50;
                    footerPanel.BackColor = Color.LightGray;

                    string summaryLabelGuid;
                    Control summaryLabelControl = DesignerHelper.AddControl(footerPanel, typeof(Label), out summaryLabelGuid);
                    Label summaryLabel = summaryLabelControl as Label;
                    if (summaryLabel != null)
                    {
                        summaryLabel.Text = "Summary: ...";
                        summaryLabel.Font = new Font("Arial", 12, FontStyle.Regular);
                        summaryLabel.AutoSize = true;
                        summaryLabel.Location = new Point(10, 15);
                    }
                }
            }
            return mainPanel;
        }

        #endregion

        #region Standard Layout Templates

        /// <summary>
        /// Creates a vertical stack layout using a FlowLayoutPanel with top–to–bottom flow.
        /// Adds three sample buttons.
        /// </summary>
        private static Control AddVerticalStackLayout(Control parent)
        {
            string layoutGuid;
            Control layoutControl = DesignerHelper.AddControl(parent, typeof(FlowLayoutPanel), out layoutGuid);
            FlowLayoutPanel flowPanel = layoutControl as FlowLayoutPanel;
            if (flowPanel != null)
            {
                flowPanel.FlowDirection = FlowDirection.TopDown;
                flowPanel.WrapContents = false;
                flowPanel.Dock = DockStyle.Fill;
                flowPanel.AutoScroll = true;

                for (int i = 1; i <= 3; i++)
                {
                    string childGuid;
                    Control btnControl = DesignerHelper.AddControl(flowPanel, typeof(Button), out childGuid);
                    Button btn = btnControl as Button;
                    if (btn != null)
                    {
                        btn.Text = $"Button {i}";
                        btn.AutoSize = true;
                        btn.Margin = new Padding(5);
                    }
                }
            }
            return layoutControl;
        }

        /// <summary>
        /// Creates a horizontal stack layout using a FlowLayoutPanel with left–to–right flow.
        /// Adds three sample buttons.
        /// </summary>
        private static Control AddHorizontalStackLayout(Control parent)
        {
            string layoutGuid;
            Control layoutControl = DesignerHelper.AddControl(parent, typeof(FlowLayoutPanel), out layoutGuid);
            FlowLayoutPanel flowPanel = layoutControl as FlowLayoutPanel;
            if (flowPanel != null)
            {
                flowPanel.FlowDirection = FlowDirection.LeftToRight;
                flowPanel.WrapContents = true;
                flowPanel.Dock = DockStyle.Fill;
                flowPanel.AutoScroll = true;

                for (int i = 1; i <= 3; i++)
                {
                    string childGuid;
                    Control btnControl = DesignerHelper.AddControl(flowPanel, typeof(Button), out childGuid);
                    Button btn = btnControl as Button;
                    if (btn != null)
                    {
                        btn.Text = $"Button {i}";
                        btn.AutoSize = true;
                        btn.Margin = new Padding(5);
                    }
                }
            }
            return layoutControl;
        }

        /// <summary>
        /// Creates a grid layout using a TableLayoutPanel.
        /// </summary>
        private static Control AddGridLayout(Control parent, int rows, int columns)
        {
            string layoutGuid;
            Control layoutControl = DesignerHelper.AddControl(parent, typeof(TableLayoutPanel), out layoutGuid);
            TableLayoutPanel tablePanel = layoutControl as TableLayoutPanel;
            if (tablePanel != null)
            {
                tablePanel.RowCount = rows;
                tablePanel.ColumnCount = columns;
                tablePanel.Dock = DockStyle.Fill;
                tablePanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                tablePanel.ColumnStyles.Clear();
                tablePanel.RowStyles.Clear();

                for (int c = 0; c < columns; c++)
                    tablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columns));
                for (int r = 0; r < rows; r++)
                    tablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / rows));

                // Insert a placeholder Panel in each cell.
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        string childGuid;
                        Control cellControl = DesignerHelper.AddControl(tablePanel, typeof(Panel), out childGuid);
                        Panel cellPanel = cellControl as Panel;
                        if (cellPanel != null)
                        {
                            cellPanel.Dock = DockStyle.Fill;
                            cellPanel.BorderStyle = BorderStyle.FixedSingle;
                        }
                        tablePanel.Controls.Add(cellControl, c, r);
                    }
                }
            }
            return layoutControl;
        }

        /// <summary>
        /// Creates a layout using a SplitContainer.
        /// </summary>
        private static Control AddSplitContainerLayout(Control parent, Orientation orientation = Orientation.Vertical)
        {
            string layoutGuid;
            Control layoutControl = DesignerHelper.AddControl(parent, typeof(SplitContainer), out layoutGuid);
            SplitContainer splitContainer = layoutControl as SplitContainer;
            if (splitContainer != null)
            {
                splitContainer.Orientation = orientation;
                splitContainer.Dock = DockStyle.Fill;

                // Panel1: Add a placeholder Panel.
                string childGuid1;
                Control panel1Child = DesignerHelper.AddControl(splitContainer.Panel1, typeof(Panel), out childGuid1);
                Panel p1 = panel1Child as Panel;
                if (p1 != null)
                {
                    p1.Dock = DockStyle.Fill;
                    p1.BorderStyle = BorderStyle.FixedSingle;
                }

                // Panel2: Add a placeholder Panel.
                string childGuid2;
                Control panel2Child = DesignerHelper.AddControl(splitContainer.Panel2, typeof(Panel), out childGuid2);
                Panel p2 = panel2Child as Panel;
                if (p2 != null)
                {
                    p2.Dock = DockStyle.Fill;
                    p2.BorderStyle = BorderStyle.FixedSingle;
                }
            }
            return layoutControl;
        }

        /// <summary>
        /// Creates a dock layout template with five child panels (Top, Bottom, Left, Right and Fill).
        /// </summary>
        private static Control AddDockLayout(Control parent)
        {
            string containerGuid;
            Control containerControl = DesignerHelper.AddControl(parent, typeof(Panel), out containerGuid);
            Panel mainPanel = containerControl as Panel;
            if (mainPanel != null)
            {
                mainPanel.Dock = DockStyle.Fill;
                mainPanel.Controls.Clear();

                // Top Panel.
                string topGuid;
                Control topControl = DesignerHelper.AddControl(mainPanel, typeof(Panel), out topGuid);
                Panel topPanel = topControl as Panel;
                if (topPanel != null)
                {
                    topPanel.Dock = DockStyle.Top;
                    topPanel.Height = 50;
                    topPanel.BackColor = SystemColors.ControlLight;
                }

                // Bottom Panel.
                string bottomGuid;
                Control bottomControl = DesignerHelper.AddControl(mainPanel, typeof(Panel), out bottomGuid);
                Panel bottomPanel = bottomControl as Panel;
                if (bottomPanel != null)
                {
                    bottomPanel.Dock = DockStyle.Bottom;
                    bottomPanel.Height = 50;
                    bottomPanel.BackColor = SystemColors.ControlDark;
                }

                // Left Panel.
                string leftGuid;
                Control leftControl = DesignerHelper.AddControl(mainPanel, typeof(Panel), out leftGuid);
                Panel leftPanel = leftControl as Panel;
                if (leftPanel != null)
                {
                    leftPanel.Dock = DockStyle.Left;
                    leftPanel.Width = 100;
                    leftPanel.BackColor = SystemColors.ControlLightLight;
                }

                // Right Panel.
                string rightGuid;
                Control rightControl = DesignerHelper.AddControl(mainPanel, typeof(Panel), out rightGuid);
                Panel rightPanel = rightControl as Panel;
                if (rightPanel != null)
                {
                    rightPanel.Dock = DockStyle.Right;
                    rightPanel.Width = 100;
                    rightPanel.BackColor = SystemColors.ControlLightLight;
                }

                // Center (Fill) Panel.
                string fillGuid;
                Control fillControl = DesignerHelper.AddControl(mainPanel, typeof(Panel), out fillGuid);
                Panel fillPanel = fillControl as Panel;
                if (fillPanel != null)
                {
                    fillPanel.Dock = DockStyle.Fill;
                    fillPanel.BackColor = SystemColors.Window;
                }
            }
            return containerControl;
        }

        #endregion

        #region Helper Methods for Layouts

        /// <summary>
        /// Helper method to add a centered label to a TableLayoutPanel cell.
        /// </summary>
        private static void AddTableCellLabel(TableLayoutPanel table, string text, int column, int row)
        {
            string cellLabelGuid;
            Control cellLabelControl = DesignerHelper.AddControl(table, typeof(Label), out cellLabelGuid);
            Label label = cellLabelControl as Label;
            if (label != null)
            {
                label.Text = text;
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleCenter;
            }
            table.Controls.Add(label, column, row);
        }

        /// <summary>
        /// Helper method to add a row of details to a TableLayoutPanel.
        /// </summary>
        private static void AddDetailRow(TableLayoutPanel table, int rowIndex, string labelText, string valueText)
        {
            // Label cell.
            string labelGuid;
            Control labelControl = DesignerHelper.AddControl(table, typeof(Label), out labelGuid);
            Label label = labelControl as Label;
            if (label != null)
            {
                label.Text = labelText;
                label.Dock = DockStyle.Fill;
                label.TextAlign = ContentAlignment.MiddleRight;
            }

            // Value cell.
            string valueGuid;
            Control valueControl = DesignerHelper.AddControl(table, typeof(Label), out valueGuid);
            Label valueLabel = valueControl as Label;
            if (valueLabel != null)
            {
                valueLabel.Text = valueText;
                valueLabel.Dock = DockStyle.Fill;
                valueLabel.TextAlign = ContentAlignment.MiddleLeft;
            }

            table.Controls.Add(label, 0, rowIndex);
            table.Controls.Add(valueLabel, 1, rowIndex);
        }

        #endregion
    }
}
