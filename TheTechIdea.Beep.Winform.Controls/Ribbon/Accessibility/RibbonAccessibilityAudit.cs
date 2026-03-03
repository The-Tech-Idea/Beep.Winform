namespace TheTechIdea.Beep.Winform.Controls.Accessibility
{
    public enum RibbonAccessibilityAuditSeverity
    {
        Info,
        Warning,
        Error
    }

    public sealed class RibbonAccessibilityAuditIssue
    {
        public RibbonAccessibilityAuditSeverity Severity { get; set; } = RibbonAccessibilityAuditSeverity.Warning;
        public string TargetPath { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public sealed class RibbonAccessibilityAuditReport
    {
        public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
        public int ControlCount { get; set; }
        public int ToolStripItemCount { get; set; }
        public List<RibbonAccessibilityAuditIssue> Issues { get; set; } = [];
        public bool Passed => Issues.Count == 0;
    }

    public static class RibbonAccessibilityAudit
    {
        public static RibbonAccessibilityAuditReport Audit(Control root)
        {
            var report = new RibbonAccessibilityAuditReport();
            if (root == null)
            {
                return report;
            }

            TraverseControlTree(root, root.Name, report);
            return report;
        }

        private static void TraverseControlTree(Control control, string path, RibbonAccessibilityAuditReport report)
        {
            report.ControlCount++;
            ValidateControl(control, path, report);

            if (control is ToolStrip strip)
            {
                TraverseToolStripItems(strip.Items, $"{path}/Items", report);
            }

            foreach (Control child in control.Controls)
            {
                string childName = string.IsNullOrWhiteSpace(child.Name) ? child.GetType().Name : child.Name;
                TraverseControlTree(child, $"{path}/{childName}", report);
            }
        }

        private static void TraverseToolStripItems(ToolStripItemCollection items, string path, RibbonAccessibilityAuditReport report)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item == null)
                {
                    continue;
                }

                report.ToolStripItemCount++;
                string itemName = string.IsNullOrWhiteSpace(item.Name) ? item.GetType().Name : item.Name;
                string itemPath = $"{path}/{i}:{itemName}";
                ValidateToolStripItem(item, itemPath, report);

                if (item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 0)
                {
                    TraverseToolStripItems(dropDownItem.DropDownItems, $"{itemPath}/DropDownItems", report);
                }
            }
        }

        private static void ValidateControl(Control control, string path, RibbonAccessibilityAuditReport report)
        {
            bool interactive = IsInteractiveControl(control);
            if (!interactive)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(control.AccessibleName))
            {
                AddIssue(report, RibbonAccessibilityAuditSeverity.Warning, path, "Interactive control is missing AccessibleName.");
            }

            if (control.AccessibleRole == AccessibleRole.Default)
            {
                AddIssue(report, RibbonAccessibilityAuditSeverity.Info, path, "Interactive control uses default AccessibleRole.");
            }
        }

        private static void ValidateToolStripItem(ToolStripItem item, string path, RibbonAccessibilityAuditReport report)
        {
            if (item is ToolStripSeparator)
            {
                return;
            }

            bool interactive = item.Available && item.Enabled;
            if (!interactive)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(item.AccessibleName))
            {
                AddIssue(report, RibbonAccessibilityAuditSeverity.Warning, path, "ToolStrip item is missing AccessibleName.");
            }

            if (item.AccessibleRole == AccessibleRole.Default)
            {
                AddIssue(report, RibbonAccessibilityAuditSeverity.Info, path, "ToolStrip item uses default AccessibleRole.");
            }
        }

        private static bool IsInteractiveControl(Control control)
        {
            if (control is null)
            {
                return false;
            }

            if (control is Label or Panel or FlowLayoutPanel or TableLayoutPanel or ToolStripPanel)
            {
                return false;
            }

            if (control is TabPage)
            {
                return false;
            }

            if (control.TabStop)
            {
                return true;
            }

            return control is ButtonBase or TextBoxBase or ComboBox or ListControl or TabControl or ToolStrip;
        }

        private static void AddIssue(
            RibbonAccessibilityAuditReport report,
            RibbonAccessibilityAuditSeverity severity,
            string path,
            string message)
        {
            report.Issues.Add(new RibbonAccessibilityAuditIssue
            {
                Severity = severity,
                TargetPath = path,
                Message = message
            });
        }
    }
}
