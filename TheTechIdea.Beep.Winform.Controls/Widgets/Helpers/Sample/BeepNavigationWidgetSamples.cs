using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepNavigationWidget with all navigation styles
    /// </summary>
    public static class BeepNavigationWidgetSamples
    {
        /// <summary>
        /// Creates a breadcrumb navigation widget
        /// Uses BreadcrumbPainter.cs
        /// </summary>
        public static BeepNavigationWidget CreateBreadcrumbWidget()
        {
            var widget = new BeepNavigationWidget
            {
                Style = NavigationWidgetStyle.Breadcrumb,
                CurrentIndex = 2,
                Size = new Size(400, 40),
                AccentColor = Color.FromArgb(33, 150, 243)
            };

            widget.Items = new List<NavigationItem>
            {
                new NavigationItem { Text = "Home", IsEnabled = true },
                new NavigationItem { Text = "Dashboard", IsEnabled = true },
                new NavigationItem { Text = "Reports", IsEnabled = true },
                new NavigationItem { Text = "Analytics", IsEnabled = false }
            };

            return widget;
        }

        /// <summary>
        /// Creates a step indicator navigation widget
        /// Uses StepIndicatorPainter.cs
        /// </summary>
        public static BeepNavigationWidget CreateStepIndicatorWidget()
        {
            var widget = new BeepNavigationWidget
            {
                Style = NavigationWidgetStyle.StepIndicator,
                CurrentIndex = 1,
                Size = new Size(400, 80),
                AccentColor = Color.FromArgb(76, 175, 80)
            };

            widget.Items = new List<NavigationItem>
            {
                new NavigationItem { Text = "Account", IsEnabled = true },
                new NavigationItem { Text = "Profile", IsEnabled = true },
                new NavigationItem { Text = "Preferences", IsEnabled = true },
                new NavigationItem { Text = "Confirmation", IsEnabled = false }
            };

            return widget;
        }

        /// <summary>
        /// Creates a tab container navigation widget
        /// Uses TabContainerPainter.cs
        /// </summary>
        public static BeepNavigationWidget CreateTabContainerWidget()
        {
            var widget = new BeepNavigationWidget
            {
                Style = NavigationWidgetStyle.TabContainer,
                CurrentIndex = 0,
                Size = new Size(350, 50),
                AccentColor = Color.FromArgb(156, 39, 176)
            };

            widget.Items = new List<NavigationItem>
            {
                new NavigationItem { Text = "Overview", IsEnabled = true },
                new NavigationItem { Text = "Details", IsEnabled = true },
                new NavigationItem { Text = "Settings", IsEnabled = true }
            };

            return widget;
        }

        /// <summary>
        /// Creates a pagination navigation widget
        /// Uses PaginationPainter.cs
        /// </summary>
        public static BeepNavigationWidget CreatePaginationWidget()
        {
            var widget = new BeepNavigationWidget
            {
                Style = NavigationWidgetStyle.Pagination,
                CurrentIndex = 2,
                Size = new Size(300, 50),
                AccentColor = Color.FromArgb(255, 193, 7)
            };

            return widget;
        }

        /// <summary>
        /// Gets all navigation widget samples
        /// </summary>
        public static BeepNavigationWidget[] GetAllSamples()
        {
            return new BeepNavigationWidget[]
            {
                CreateBreadcrumbWidget(),
                CreateStepIndicatorWidget(),
                CreateTabContainerWidget(),
                CreatePaginationWidget()
            };
        }
    }
}