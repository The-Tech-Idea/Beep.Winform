using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    public static class ProductLayoutHelper
    {
        public static Control Build(Control parent)
        {
            var mainPanel = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(mainPanel);

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 200 };
            mainPanel.Controls.Add(split);

            var productPicture = new PictureBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.LightGray, SizeMode = PictureBoxSizeMode.Zoom };
            split.Panel1.Controls.Add(productPicture);

            var nameLabel = new Label { Text = "Product Name", Font = new Font("Arial", 16, FontStyle.Bold), AutoSize = true, Location = new Point(10, 10) };
            var priceLabel = new Label { Text = "Price: $0.00", Font = new Font("Arial", 12), AutoSize = true, Location = new Point(10, 40) };
            var description = new TextBox { Multiline = true, Dock = DockStyle.Bottom, Height = 100, Text = "Product description goes here..." };
            split.Panel2.Controls.Add(description);
            split.Panel2.Controls.Add(priceLabel);
            split.Panel2.Controls.Add(nameLabel);

            return mainPanel;
        }
    }
}
