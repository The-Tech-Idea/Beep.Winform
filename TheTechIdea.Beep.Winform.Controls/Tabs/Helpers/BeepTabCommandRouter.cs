using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    internal static class BeepTabCommandRouter
    {
        public const string CloseCurrentCommand = "tab.closeCurrent";
        public const string CloseOthersCommand = "tab.closeOthers";
        public const string CloseAllCommand = "tab.closeAll";
        public const string CloseToRightCommand = "tab.closeToRight";
        public const string TogglePinCommand = "tab.togglePin";
        public const string MoveLeftCommand = "tab.moveLeft";
        public const string MoveRightCommand = "tab.moveRight";
        public const string SelectTabCommand          = "tab.select";
        public const string RevealOverflowCommand      = "tab.revealOverflow";
        public const string ReopenLastClosedCommand    = "tab.reopenLastClosed";

        public static bool CanExecute(BeepTabs owner, string commandName, int tabIndex = -1)
        {
            if (owner == null || string.IsNullOrWhiteSpace(commandName))
            {
                return false;
            }

            return commandName switch
            {
                SelectTabCommand => owner.CanSelectHeaderTab(tabIndex),
                CloseCurrentCommand => owner.CanCloseCurrentHeaderTab(),
                CloseOthersCommand => owner.CanCloseOtherHeaderTabs(tabIndex),
                CloseAllCommand => owner.CanCloseAllHeaderTabs(),
                CloseToRightCommand => owner.CanCloseHeaderTabsToTheRight(tabIndex),
                TogglePinCommand => owner.CanTogglePinHeaderTab(tabIndex),
                MoveLeftCommand => owner.CanMoveHeaderTabLeft(tabIndex),
                MoveRightCommand => owner.CanMoveHeaderTabRight(tabIndex),
                RevealOverflowCommand   => owner.CanShowHeaderOverflow(),
                ReopenLastClosedCommand => owner.TabMode != BeepTabMode.Navigation,
                _ => false
            };
        }

        public static bool TryExecute(BeepTabs owner, string commandName, int tabIndex = -1)
        {
            if (!CanExecute(owner, commandName, tabIndex))
            {
                return false;
            }

            return commandName switch
            {
                SelectTabCommand => owner.TrySelectHeaderTab(tabIndex),
                CloseCurrentCommand => owner.TryCloseCurrentHeaderTab(),
                CloseOthersCommand => owner.TryCloseOtherHeaderTabs(tabIndex),
                CloseAllCommand => owner.TryCloseAllHeaderTabs(),
                CloseToRightCommand => owner.TryCloseHeaderTabsToTheRight(tabIndex),
                TogglePinCommand => owner.TryTogglePinHeaderTab(tabIndex),
                MoveLeftCommand => owner.TryMoveHeaderTabLeft(tabIndex),
                MoveRightCommand => owner.TryMoveHeaderTabRight(tabIndex),
                RevealOverflowCommand   => owner.TryShowHeaderOverflow(),
                ReopenLastClosedCommand => owner.TryReopenLastClosedTab(),
                _ => false
            };
        }
    }
}