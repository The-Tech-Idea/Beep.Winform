using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Helper for creating profile layout templates.
    /// Creates a layout with profile picture and name at the top, and details table below.
    /// </summary>
    public static class ProfileLayoutHelper
    {
        /// <summary>
        /// Builds a profile layout with default options.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <returns>The main Panel containing the profile layout.</returns>
        public static Control Build(Control parent)
        {
            return Build(parent, LayoutOptions.Default);
        }

        /// <summary>
        /// Builds a profile layout with theme-aware styling.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="options">Layout configuration options for theming and styling.</param>
        /// <returns>The main Panel containing the profile layout.</returns>
        public static Control Build(Control parent, LayoutOptions options)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            options = options ?? LayoutOptions.Default;

            var mainPanel = BaseLayoutHelper.CreateStyledPanel(options);
            mainPanel.Dock = DockStyle.Fill;
            parent.Controls.Add(mainPanel);

            var topPanel = BaseLayoutHelper.CreateStyledPanel(options);
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 150;
            topPanel.BackColor = BaseLayoutHelper.GetBackgroundColor(options);
            mainPanel.Controls.Add(topPanel);

            var profilePic = new PictureBox 
            { 
                Width = 130, 
                Height = 130, 
                Location = new Point(10, 10), 
                BorderStyle = options.ShowBorders ? BorderStyle.FixedSingle : BorderStyle.None, 
                BackColor = BaseLayoutHelper.GetThemeColor(options, "Muted", Color.Gray), 
                SizeMode = PictureBoxSizeMode.Zoom 
            };

            var nameLabel = BaseLayoutHelper.CreateStyledLabel("User Name", options, ContentAlignment.MiddleLeft);
            nameLabel.AutoSize = true;
            nameLabel.Font = BaseLayoutHelper.GetFont(options, 20f);
            nameLabel.Font = new Font(nameLabel.Font, FontStyle.Bold);
            nameLabel.Location = new Point(150, 50);

            topPanel.Controls.Add(profilePic);
            topPanel.Controls.Add(nameLabel);

            var detailsPanel = new TableLayoutPanel 
            { 
                Dock = DockStyle.Fill, 
                ColumnCount = 2, 
                RowCount = 3, 
                CellBorderStyle = options.ShowBorders ? TableLayoutPanelCellBorderStyle.Single : TableLayoutPanelCellBorderStyle.None,
                BackColor = BaseLayoutHelper.GetBackgroundColor(options)
            };
            detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            detailsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            for (int i = 0; i < 3; i++) 
                detailsPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33f));
            
            CommonLayoutHelper.AddDetailRow(detailsPanel, 0, "Email:", "user@example.com", options);
            CommonLayoutHelper.AddDetailRow(detailsPanel, 1, "Phone:", "123-456-7890", options);
            CommonLayoutHelper.AddDetailRow(detailsPanel, 2, "Address:", "123 Main St, City, Country", options);
            mainPanel.Controls.Add(detailsPanel);

            return mainPanel;
        }
    }
}
