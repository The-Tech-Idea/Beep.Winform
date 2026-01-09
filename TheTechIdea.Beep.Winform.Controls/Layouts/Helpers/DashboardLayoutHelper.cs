using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Layouts.Helpers
{
    /// <summary>
    /// Helper for creating dashboard layouts.
    /// Creates a multi-panel dashboard with header, sidebar, main content, and footer areas.
    /// </summary>
    public static class DashboardLayoutHelper
    {
        /// <summary>
        /// Builds a dashboard layout with default options.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <returns>The main Panel containing the dashboard layout.</returns>
        public static Control Build(Control parent)
        {
            return Build(parent, LayoutOptions.Default);
        }

        /// <summary>
        /// Builds a dashboard layout with theme-aware styling.
        /// </summary>
        /// <param name="parent">The parent control container.</param>
        /// <param name="options">Layout configuration options for theming and styling.</param>
        /// <returns>The main Panel containing the dashboard layout.</returns>
        public static Control Build(Control parent, LayoutOptions options)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            options = options ?? LayoutOptions.Default;

            var mainPanel = BaseLayoutHelper.CreateStyledPanel(options);
            mainPanel.Dock = DockStyle.Fill;
            parent.Controls.Add(mainPanel);

            var bgColor = BaseLayoutHelper.GetBackgroundColor(options);

            // Header panel
            var headerPanel = BaseLayoutHelper.CreateStyledPanel(options);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 60;
            headerPanel.BackColor = bgColor;

            var headerLabel = BaseLayoutHelper.CreateStyledLabel("Dashboard", options, ContentAlignment.MiddleLeft);
            headerLabel.Font = BaseLayoutHelper.GetFont(options, 18f);
            headerLabel.Font = new Font(headerLabel.Font, FontStyle.Bold);
            headerLabel.Dock = DockStyle.Fill;
            headerLabel.Padding = new Padding(10, 0, 0, 0);
            headerPanel.Controls.Add(headerLabel);
            mainPanel.Controls.Add(headerPanel);

            // Content area with sidebar
            var contentPanel = BaseLayoutHelper.CreateStyledPanel(options);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = bgColor;
            mainPanel.Controls.Add(contentPanel);

            // Sidebar
            var sidebar = BaseLayoutHelper.CreateStyledPanel(options);
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 200;
            sidebar.BackColor = bgColor;

            var sidebarLabel = BaseLayoutHelper.CreateStyledLabel("Sidebar", options, ContentAlignment.TopLeft);
            sidebarLabel.Dock = DockStyle.Top;
            sidebarLabel.Height = 30;
            sidebarLabel.Padding = new Padding(10, 5, 0, 0);
            sidebar.Controls.Add(sidebarLabel);
            contentPanel.Controls.Add(sidebar);

            // Main content area
            var mainContent = BaseLayoutHelper.CreateStyledPanel(options);
            mainContent.Dock = DockStyle.Fill;
            mainContent.BackColor = bgColor;

            var contentLabel = BaseLayoutHelper.CreateStyledLabel("Main Content Area", options, ContentAlignment.MiddleCenter);
            contentLabel.Dock = DockStyle.Fill;
            mainContent.Controls.Add(contentLabel);
            contentPanel.Controls.Add(mainContent);

            // Footer panel
            var footerPanel = BaseLayoutHelper.CreateStyledPanel(options);
            footerPanel.Dock = DockStyle.Bottom;
            footerPanel.Height = 40;
            footerPanel.BackColor = bgColor;

            var footerLabel = BaseLayoutHelper.CreateStyledLabel("Footer", options, ContentAlignment.MiddleLeft);
            footerLabel.Dock = DockStyle.Fill;
            footerLabel.Padding = new Padding(10, 0, 0, 0);
            footerPanel.Controls.Add(footerLabel);
            mainPanel.Controls.Add(footerPanel);

            return mainPanel;
        }
    }
}
