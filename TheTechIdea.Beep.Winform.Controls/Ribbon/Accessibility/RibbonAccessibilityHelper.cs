using TheTechIdea.Beep.Vis.Modules;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Accessibility
{
    public static class RibbonAccessibilityHelper
    {
        public static void ApplyCommandAccessibility(
            ToolStripItem item,
            SimpleItem command,
            string fallbackText,
            AccessibleRole role = AccessibleRole.PushButton)
        {
            if (item == null || command == null)
            {
                return;
            }

            string name = GetDisplayText(command, fallbackText);
            string description = BuildDescription(command);
            item.AccessibleName = name;
            item.AccessibleDescription = description;
            item.AccessibleRole = role;
            if (string.IsNullOrWhiteSpace(item.AccessibleDefaultActionDescription))
            {
                item.AccessibleDefaultActionDescription = command.IsEnabled ? "Activate command" : "Command unavailable";
            }
        }

        public static void ApplyControlAccessibility(
            Control control,
            string name,
            string? description = null,
            AccessibleRole role = AccessibleRole.Client)
        {
            if (control == null)
            {
                return;
            }

            control.AccessibleName = name;
            control.AccessibleDescription = string.IsNullOrWhiteSpace(description) ? name : description;
            control.AccessibleRole = role;
        }

        private static string GetDisplayText(SimpleItem item, string fallbackText)
        {
            if (!string.IsNullOrWhiteSpace(item.DisplayField)) return item.DisplayField;
            if (!string.IsNullOrWhiteSpace(item.Text)) return item.Text;
            if (!string.IsNullOrWhiteSpace(item.Name)) return item.Name;
            return fallbackText;
        }

        private static string BuildDescription(SimpleItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.ToolTip) && !string.IsNullOrWhiteSpace(item.ShortcutText))
            {
                return $"{item.ToolTip} ({item.ShortcutText})";
            }

            if (!string.IsNullOrWhiteSpace(item.ToolTip))
            {
                return item.ToolTip;
            }

            if (!string.IsNullOrWhiteSpace(item.Description))
            {
                return item.Description;
            }

            if (!string.IsNullOrWhiteSpace(item.ShortcutText))
            {
                return item.ShortcutText;
            }

            return GetDisplayText(item, string.Empty);
        }

        public static AccessibleRole GetCommandRole(SimpleItem command, ToolStripItem item)
        {
            if (command == null)
            {
                return AccessibleRole.PushButton;
            }

            if (command.IsSeparator || item is ToolStripSeparator)
            {
                return AccessibleRole.Separator;
            }

            if (item is ToolStripControlHost)
            {
                return AccessibleRole.Grouping;
            }

            if (item is ToolStripDropDownButton || command.Children.Count > 0)
            {
                return AccessibleRole.ButtonDropDown;
            }

            if (command.IsCheckable)
            {
                return AccessibleRole.CheckButton;
            }

            return AccessibleRole.PushButton;
        }
    }
}
