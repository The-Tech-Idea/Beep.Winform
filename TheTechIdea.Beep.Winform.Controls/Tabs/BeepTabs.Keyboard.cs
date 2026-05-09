using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        protected override bool IsInputKey(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            if (keyCode == Keys.Left || keyCode == Keys.Right || keyCode == Keys.Up || keyCode == Keys.Down || keyCode == Keys.Home || keyCode == Keys.End || keyCode == Keys.Apps)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (TryProcessHeaderKeyboardCommand(keyData))
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool TryProcessHeaderKeyboardCommand(Keys keyData)
        {
            if (GetHostedSourceItemCount() == 0)
            {
                return false;
            }

            SyncHeaderSnapshot();
            int selectedIndex = GetHostedSourceSelectedIndex();
            Keys keyCode = keyData & Keys.KeyCode;

            if ((keyData & Keys.Control) == Keys.Control && keyCode == Keys.Tab)
            {
                if ((keyData & Keys.Shift) == Keys.Shift)
                {
                    return TrySelectRelativeHeaderTab(forward: false, wrap: true);
                }

                return TrySelectWorkspaceMruHeaderTab() || TrySelectRelativeHeaderTab(forward: true, wrap: true);
            }

            if (keyData == (Keys.Control | Keys.Shift | Keys.T))
            {
                return TryReopenLastClosedTab();
            }

            if (keyCode == Keys.Home)
            {
                return TrySelectBoundaryHeaderTab(last: false);
            }

            if (keyCode == Keys.End)
            {
                return TrySelectBoundaryHeaderTab(last: true);
            }

            if (keyCode == Keys.Delete)
            {
                return BeepTabCommandRouter.TryExecute(this, BeepTabCommandRouter.CloseCurrentCommand, selectedIndex);
            }

            if (keyData == (Keys.Control | Keys.W))
            {
                return BeepTabCommandRouter.TryExecute(this, BeepTabCommandRouter.CloseCurrentCommand, selectedIndex);
            }

            if (keyData == (Keys.Control | Keys.P))
            {
                return TryShowWorkspaceQuickSwitch();
            }

            if ((keyData & Keys.Control) == Keys.Control && TryGetKeyboardShortcutTabIndex(keyCode, out int shortcutIndex))
            {
                return TrySelectHeaderTab(shortcutIndex);
            }

            if (keyCode == Keys.Apps || keyData == (Keys.Shift | Keys.F10))
            {
                return TryShowSelectedHeaderTabContextMenu();
            }

            if (_headerHost.TryResolveArrowNavigationTarget(keyData, selectedIndex, out int targetIndex))
            {
                return TrySelectHeaderTab(targetIndex);
            }

            return false;
        }

        private static bool TryGetKeyboardShortcutTabIndex(Keys keyCode, out int tabIndex)
        {
            if (keyCode >= Keys.D1 && keyCode <= Keys.D9)
            {
                tabIndex = keyCode - Keys.D1;
                return true;
            }

            if (keyCode >= Keys.NumPad1 && keyCode <= Keys.NumPad9)
            {
                tabIndex = keyCode - Keys.NumPad1;
                return true;
            }

            tabIndex = -1;
            return false;
        }

        private bool TrySelectRelativeHeaderTab(bool forward, bool wrap)
        {
            int selectedIndex = GetHostedSourceSelectedIndex();
            if (!_headerHost.TryGetNextSelectableTabIndex(selectedIndex, forward, wrap, out int targetIndex))
            {
                return false;
            }

            return TrySelectHeaderTab(targetIndex);
        }

        private bool TrySelectBoundaryHeaderTab(bool last)
        {
            if (!_headerHost.TryGetBoundarySelectableTabIndex(last, out int targetIndex))
            {
                return false;
            }

            return TrySelectHeaderTab(targetIndex);
        }

        private bool TryShowSelectedHeaderTabContextMenu()
        {
            int selectedIndex = GetHostedSourceSelectedIndex();
            if (selectedIndex < 0)
            {
                return false;
            }

            Rectangle tabBounds = GetTabRect(selectedIndex);
            if (tabBounds.IsEmpty)
            {
                return false;
            }

            Point anchor = new Point(tabBounds.Left + Math.Max(8, tabBounds.Width / 2), tabBounds.Bottom);
            return TryShowHeaderTabContextMenu(anchor);
        }
    }
}