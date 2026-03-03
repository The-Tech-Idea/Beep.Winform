using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Backstage
{
    public static class RibbonBackstageTemplateBuilder
    {
        public static List<SimpleItem> CreateStandardTemplate(string appTitle = "Application")
        {
            string app = string.IsNullOrWhiteSpace(appTitle) ? "Application" : appTitle.Trim();
            return
            [
                CreateSection("Info", [
                    CreateCommand("Protect Document", toolTip: $"Manage {app} file permissions."),
                    CreateCommand("Inspect Document", toolTip: $"Inspect {app} file metadata and issues."),
                    CreateCommand("Manage Versions", toolTip: "Browse and restore prior versions.")
                ]),
                CreateSection("New", [
                    CreateCommand("Blank Document"),
                    CreateCommand("From Template"),
                    CreateCommand("From Existing")
                ]),
                CreateSection("Open", [
                    CreateCommand("Browse"),
                    CreateCommand("Open from Cloud"),
                    CreateCommand("Open from Recent")
                ]),
                CreateSection("Save", [
                    CreateCommand("Save"),
                    CreateCommand("Save As"),
                    CreateCommand("Save a Copy")
                ]),
                CreateSection("Print", [
                    CreateCommand("Print"),
                    CreateCommand("Print Preview"),
                    CreateCommand("Printer Setup")
                ]),
                CreateSection("Share", [
                    CreateCommand("Share Link"),
                    CreateCommand("Attach Copy"),
                    CreateCommand("Send as PDF")
                ]),
                CreateSection("Export", [
                    CreateCommand("Export to PDF"),
                    CreateCommand("Export to Image"),
                    CreateCommand("Change File Type")
                ]),
                CreateSection("Options", [
                    CreateCommand("General"),
                    CreateCommand("Appearance"),
                    CreateCommand("Advanced")
                ]),
                CreateSection("Account", [
                    CreateCommand("User Profile"),
                    CreateCommand("Connected Services"),
                    CreateCommand("Sign Out")
                ]),
                CreateSection("Exit", [
                    CreateCommand("Close"),
                    CreateCommand("Exit")
                ])
            ];
        }

        public static SimpleItem CreateRecentSection(string title = "Recent")
        {
            return CreateSection(title, []);
        }

        public static SimpleItem CreateSection(string title, IEnumerable<SimpleItem> commands)
        {
            var section = new SimpleItem
            {
                Text = title,
                IsVisible = true,
                IsEnabled = true
            };

            foreach (var command in commands)
            {
                section.Children.Add(command);
            }

            return section;
        }

        public static SimpleItem CreateCommand(string text, string? actionId = null, string? toolTip = null, string? imagePath = null)
        {
            return new SimpleItem
            {
                Text = text,
                ActionID = string.IsNullOrWhiteSpace(actionId) ? text : actionId,
                ToolTip = toolTip ?? text,
                ImagePath = imagePath,
                IsVisible = true,
                IsEnabled = true
            };
        }
    }
}
