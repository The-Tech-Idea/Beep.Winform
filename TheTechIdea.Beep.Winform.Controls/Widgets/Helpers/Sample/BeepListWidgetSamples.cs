using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepListWidget with all list styles
    /// </summary>
    public static class BeepListWidgetSamples
    {
        /// <summary>
        /// Creates an activity feed widget
        /// Uses ActivityFeedPainter.cs
        /// </summary>
        public static BeepListWidget CreateActivityFeedWidget()
        {
            var widget = new BeepListWidget
            {
                Style = ListWidgetStyle.ActivityFeed,
                Title = "Recent Activities",
                ShowHeader = true,
                Size = new Size(350, 300),
                AccentColor = Color.FromArgb(33, 150, 243)
            };

            widget.Items = new List<Dictionary<string, object>>
            {
                new() { ["Name"] = "John Doe", ["Value"] = "Updated project documentation", ["Time"] = "2 hours ago" },
                new() { ["Name"] = "Jane Smith", ["Value"] = "Completed feature review", ["Time"] = "4 hours ago" },
                new() { ["Name"] = "Bob Wilson", ["Value"] = "Fixed critical bug", ["Time"] = "6 hours ago" },
                new() { ["Name"] = "Alice Brown", ["Value"] = "Deployed to production", ["Time"] = "8 hours ago" }
            };

            return widget;
        }

        /// <summary>
        /// Creates a data table widget
        /// Uses DataTablePainter.cs
        /// </summary>
        public static BeepListWidget CreateDataTableWidget()
        {
            var widget = new BeepListWidget
            {
                Style = ListWidgetStyle.DataTable,
                Title = "Employee Data",
                ShowHeader = true,
                Columns = new List<string> { "Name", "Department", "Status" },
                Size = new Size(400, 250),
                AccentColor = Color.FromArgb(76, 175, 80)
            };

            widget.Items = new List<Dictionary<string, object>>
            {
                new() { ["Name"] = "John Smith", ["Department"] = "Engineering", ["Status"] = "Active" },
                new() { ["Name"] = "Sarah Johnson", ["Department"] = "Design", ["Status"] = "Active" },
                new() { ["Name"] = "Mike Brown", ["Department"] = "Marketing", ["Status"] = "On Leave" },
                new() { ["Name"] = "Lisa Davis", ["Department"] = "Sales", ["Status"] = "Active" }
            };

            return widget;
        }

        /// <summary>
        /// Creates a ranking list widget
        /// Uses RankingListPainter.cs
        /// </summary>
        public static BeepListWidget CreateRankingListWidget()
        {
            var widget = new BeepListWidget
            {
                Style = ListWidgetStyle.RankingList,
                Title = "Top Performers",
                ShowHeader = true,
                Size = new Size(300, 280),
                AccentColor = Color.FromArgb(255, 193, 7)
            };

            widget.Items = new List<Dictionary<string, object>>
            {
                new() { ["Name"] = "Alice Johnson", ["Value"] = "98 points" },
                new() { ["Name"] = "Bob Smith", ["Value"] = "95 points" },
                new() { ["Name"] = "Carol Brown", ["Value"] = "92 points" },
                new() { ["Name"] = "David Wilson", ["Value"] = "89 points" },
                new() { ["Name"] = "Eve Davis", ["Value"] = "86 points" }
            };

            return widget;
        }

        /// <summary>
        /// Creates a status list widget
        /// Uses StatusListPainter.cs
        /// </summary>
        public static BeepListWidget CreateStatusListWidget()
        {
            var widget = new BeepListWidget
            {
                Style = ListWidgetStyle.StatusList,
                Title = "System Services",
                ShowHeader = true,
                Size = new Size(320, 250),
                AccentColor = Color.FromArgb(76, 175, 80)
            };

            widget.Items = new List<Dictionary<string, object>>
            {
                new() { ["Name"] = "Web Server", ["Status"] = "Running" },
                new() { ["Name"] = "Database", ["Status"] = "Running" },
                new() { ["Name"] = "Cache Service", ["Status"] = "Warning" },
                new() { ["Name"] = "Mail Service", ["Status"] = "Stopped" },
                new() { ["Name"] = "Backup Service", ["Status"] = "Running" }
            };

            return widget;
        }

        /// <summary>
        /// Creates a profile list widget
        /// Uses ProfileListPainter.cs
        /// </summary>
        public static BeepListWidget CreateProfileListWidget()
        {
            var widget = new BeepListWidget
            {
                Style = ListWidgetStyle.ProfileList,
                Title = "Team Members",
                ShowHeader = true,
                Size = new Size(350, 300),
                AccentColor = Color.FromArgb(156, 39, 176)
            };

            widget.Items = new List<Dictionary<string, object>>
            {
                new() { ["Name"] = "Sarah Johnson", ["Value"] = "Senior Developer" },
                new() { ["Name"] = "Mike Chen", ["Value"] = "UI/UX Designer" },
                new() { ["Name"] = "Lisa Rodriguez", ["Value"] = "Project Manager" },
                new() { ["Name"] = "David Kim", ["Value"] = "DevOps Engineer" },
                new() { ["Name"] = "Anna Williams", ["Value"] = "QA Specialist" }
            };

            return widget;
        }

        /// <summary>
        /// Creates a task list widget
        /// Uses TaskListPainter.cs
        /// </summary>
        public static BeepListWidget CreateTaskListWidget()
        {
            var widget = new BeepListWidget
            {
                Style = ListWidgetStyle.TaskList,
                Title = "Project Tasks",
                ShowHeader = true,
                Size = new Size(350, 280),
                AccentColor = Color.FromArgb(33, 150, 243)
            };

            widget.Items = new List<Dictionary<string, object>>
            {
                new() { ["Name"] = "Update project documentation", ["Status"] = "completed" },
                new() { ["Name"] = "Implement user authentication", ["Status"] = "in-progress" },
                new() { ["Name"] = "Write unit tests", ["Status"] = "pending" },
                new() { ["Name"] = "Deploy to staging", ["Status"] = "pending" },
                new() { ["Name"] = "Code review", ["Status"] = "completed" }
            };

            return widget;
        }

        /// <summary>
        /// Gets all list widget samples
        /// </summary>
        public static BeepListWidget[] GetAllSamples()
        {
            return new BeepListWidget[]
            {
                CreateActivityFeedWidget(),
                CreateDataTableWidget(),
                CreateRankingListWidget(),
                CreateStatusListWidget(),
                CreateProfileListWidget(),
                CreateTaskListWidget()
            };
        }
    }
}