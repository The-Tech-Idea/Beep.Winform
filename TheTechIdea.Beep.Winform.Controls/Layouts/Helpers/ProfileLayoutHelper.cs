using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    public static class ProfileLayoutHelper
    {
        public static Control Build(Control parent)
        {
            var mainPanel = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(mainPanel);

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 150, BackColor = Color.LightBlue };
            mainPanel.Controls.Add(topPanel);

            var profilePic = new PictureBox { Width = 130, Height = 130, Location = new Point(10, 10), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.Gray, SizeMode = PictureBoxSizeMode.Zoom };
            var nameLabel = new Label { Text = "User Name", Font = new Font("Arial", 20, FontStyle.Bold), AutoSize = true, Location = new Point(150, 50) };
            topPanel.Controls.Add(profilePic);
            topPanel.Controls.Add(nameLabel);

            var detailsPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, CellBorderStyle = TableLayoutPanelCellBorderStyle.Single };
            detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            for (int i = 0; i < 3; i++) detailsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            CommonLayoutHelper.AddDetailRow(detailsPanel, 0, "Email:", "user@example.com");
            CommonLayoutHelper.AddDetailRow(detailsPanel, 1, "Phone:", "123-456-7890");
            CommonLayoutHelper.AddDetailRow(detailsPanel, 2, "Address:", "123 Main St, City, Country");
            mainPanel.Controls.Add(detailsPanel);

            return mainPanel;
        }
    }
}
