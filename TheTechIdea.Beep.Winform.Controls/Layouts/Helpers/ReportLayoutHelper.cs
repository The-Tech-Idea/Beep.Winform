using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Helper for creating report layout templates.
    /// Creates a layout with header (title and filter), content area (DataGridView), and footer (summary).
    /// </summary>
    public static class ReportLayoutHelper
    {
        /// <summary>
        /// Builds a report layout with default options.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <returns>The main Panel containing the report layout.</returns>
        public static Control Build(Control parent)
        {
            return Build(parent, LayoutOptions.Default);
        }

        /// <summary>
        /// Builds a report layout with theme-aware styling.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="options">Layout configuration options for theming and styling.</param>
        /// <returns>The main Panel containing the report layout.</returns>
        public static Control Build(Control parent, LayoutOptions options)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            options = options ?? LayoutOptions.Default;

            var mainPanel = BaseLayoutHelper.CreateStyledPanel(options);
            mainPanel.Dock = DockStyle.Fill;
            parent.Controls.Add(mainPanel);

            var header = BaseLayoutHelper.CreateStyledPanel(options);
            header.Dock = DockStyle.Top;
            header.Height = 100;
            header.BackColor = BaseLayoutHelper.GetBackgroundColor(options);
            mainPanel.Controls.Add(header);

            var title = BaseLayoutHelper.CreateStyledLabel("Report Title", options, ContentAlignment.TopLeft);
            title.AutoSize = true;
            title.Font = BaseLayoutHelper.GetFont(options, 18f);
            title.Font = new Font(title.Font, FontStyle.Bold);
            title.Location = new Point(10, 10);

            var filterLabel = BaseLayoutHelper.CreateStyledLabel("Filter:", options, ContentAlignment.TopLeft);
            filterLabel.AutoSize = true;
            filterLabel.Location = new Point(10, 50);

            var filterBox = BaseLayoutHelper.CreateBeepTextBox("", options);
            filterBox.Location = new Point(60, 45);
            filterBox.Width = 150;

            header.Controls.Add(title);
            header.Controls.Add(filterLabel);
            header.Controls.Add(filterBox);

            var content = BaseLayoutHelper.CreateStyledPanel(options);
            content.Dock = DockStyle.Fill;
            content.BackColor = BaseLayoutHelper.GetBackgroundColor(options);

            var grid = new DataGridView 
            { 
                Dock = DockStyle.Fill, 
                AutoGenerateColumns = true,
                BackColor = BaseLayoutHelper.GetBackgroundColor(options),
                ForeColor = BaseLayoutHelper.GetForegroundColor(options)
            };
            content.Controls.Add(grid);
            mainPanel.Controls.Add(content);

            var footer = BaseLayoutHelper.CreateStyledPanel(options);
            footer.Dock = DockStyle.Bottom;
            footer.Height = 50;
            footer.BackColor = BaseLayoutHelper.GetBackgroundColor(options);

            var summary = BaseLayoutHelper.CreateStyledLabel("Summary: ...", options, ContentAlignment.MiddleLeft);
            summary.AutoSize = true;
            summary.Font = BaseLayoutHelper.GetFont(options, 12f);
            summary.Location = new Point(10, 15);
            footer.Controls.Add(summary);
            mainPanel.Controls.Add(footer);

            return mainPanel;
        }
    }
}
