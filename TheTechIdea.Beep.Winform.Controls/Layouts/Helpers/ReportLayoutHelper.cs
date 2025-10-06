using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    public static class ReportLayoutHelper
    {
        public static Control Build(Control parent)
        {
            var mainPanel = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(mainPanel);

            var header = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.LightGray };
            mainPanel.Controls.Add(header);
            var title = new Label { Text = "Report Title", Font = new Font("Arial", 18, FontStyle.Bold), AutoSize = true, Location = new Point(10, 10) };
            var filterLabel = new Label { Text = "Filter:", AutoSize = true, Location = new Point(10, 50) };
            var filterBox = new TextBox { Location = new Point(60, 45), Width = 150 };
            header.Controls.Add(title);
            header.Controls.Add(filterLabel);
            header.Controls.Add(filterBox);

            var content = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            var grid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true };
            content.Controls.Add(grid);
            mainPanel.Controls.Add(content);

            var footer = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.LightGray };
            var summary = new Label { Text = "Summary: ...", Font = new Font("Arial", 12), AutoSize = true, Location = new Point(10, 15) };
            footer.Controls.Add(summary);
            mainPanel.Controls.Add(footer);

            return mainPanel;
        }
    }
}
