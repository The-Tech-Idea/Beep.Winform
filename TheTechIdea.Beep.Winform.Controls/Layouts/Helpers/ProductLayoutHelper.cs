using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Helper for creating product layout templates.
    /// Creates a split container with product image on the left and product details on the right.
    /// </summary>
    public static class ProductLayoutHelper
    {
        /// <summary>
        /// Builds a product layout with default options.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <returns>The main Panel containing the product layout.</returns>
        public static Control Build(Control parent)
        {
            return Build(parent, LayoutOptions.Default);
        }

        /// <summary>
        /// Builds a product layout with theme-aware styling.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="options">Layout configuration options for theming and styling.</param>
        /// <returns>The main Panel containing the product layout.</returns>
        public static Control Build(Control parent, LayoutOptions options)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            options = options ?? LayoutOptions.Default;

            var mainPanel = BaseLayoutHelper.CreateStyledPanel(options);
            mainPanel.Dock = DockStyle.Fill;
            parent.Controls.Add(mainPanel);

            var split = new SplitContainer 
            { 
                Dock = DockStyle.Fill, 
                Orientation = Orientation.Vertical, 
                SplitterDistance = 200,
                BackColor = BaseLayoutHelper.GetBackgroundColor(options)
            };
            mainPanel.Controls.Add(split);

            var productPicture = new PictureBox 
            { 
                Dock = DockStyle.Fill, 
                BorderStyle = options.ShowBorders ? BorderStyle.FixedSingle : BorderStyle.None, 
                BackColor = BaseLayoutHelper.GetThemeColor(options, "Muted", Color.LightGray), 
                SizeMode = PictureBoxSizeMode.Zoom 
            };
            split.Panel1.Controls.Add(productPicture);

            var nameLabel = BaseLayoutHelper.CreateStyledLabel("Product Name", options, ContentAlignment.TopLeft);
            nameLabel.AutoSize = true;
            nameLabel.Font = BaseLayoutHelper.GetFont(options, 16f);
            nameLabel.Font = new Font(nameLabel.Font, FontStyle.Bold);
            nameLabel.Location = new Point(10, 10);

            var priceLabel = BaseLayoutHelper.CreateStyledLabel("Price: $0.00", options, ContentAlignment.TopLeft);
            priceLabel.AutoSize = true;
            priceLabel.Font = BaseLayoutHelper.GetFont(options, 12f);
            priceLabel.Location = new Point(10, 40);

            var description = BaseLayoutHelper.CreateBeepTextBox("Product description goes here...", options);
            description.Multiline = true;
            description.Dock = DockStyle.Bottom;
            description.Height = 100;

            split.Panel2.Controls.Add(description);
            split.Panel2.Controls.Add(priceLabel);
            split.Panel2.Controls.Add(nameLabel);

            return mainPanel;
        }
    }
}
