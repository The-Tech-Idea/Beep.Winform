using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls.Tabs.Helpers
{
    internal static class BeepTabHeaderActionRouter
    {
        public static bool TryExecute(BeepTabs owner, BeepTabHeaderAction action)
        {
            if (owner == null || action == null || !action.IsVisible || !action.IsEnabled)
            {
                return false;
            }

            return action.ActionKind switch
            {
                BeepTabHeaderActionKind.SelectTab => owner.TrySelectHeaderTab(action.TabIndex),
                BeepTabHeaderActionKind.CloseTab => owner.TryCloseHeaderTab(action.TabIndex),
                BeepTabHeaderActionKind.Overflow => owner.TryShowHeaderOverflow(),
                BeepTabHeaderActionKind.CloseCurrent => owner.TryCloseCurrentHeaderTab(),
                _ => false
            };
        }
    }
}