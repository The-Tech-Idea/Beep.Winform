using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        private ContextMenuStrip? _headerTabContextMenu;

        private bool TryShowHeaderTabContextMenu(System.Drawing.Point location)
        {
            if (TabMode == BeepTabMode.Navigation || GetHostedSourceItemCount() == 0)
            {
                return false;
            }

            SyncHeaderSnapshot();
            if (!_headerHost.TryResolveContextMenuTab(location, out int tabIndex))
            {
                return false;
            }

            if (!TryGetHeaderTabItem(tabIndex, out BeepTabItem? item))
            {
                return false;
            }

            ShowHeaderTabContextMenu(location, tabIndex, item);
            return true;
        }

        private void ShowHeaderTabContextMenu(System.Drawing.Point location, int tabIndex, BeepTabItem item)
        {
            DisposeHeaderTabContextMenu();

            ContextMenuStrip menu = BuildHeaderTabContextMenu(tabIndex, item);
            if (menu.Items.Count == 0)
            {
                menu.Dispose();
                return;
            }

            _headerTabContextMenu = menu;
            _headerTabContextMenu.Closed += HeaderTabContextMenu_Closed;

            if (IsPopupOpen)
            {
                CloseChildPopup();
            }

            OnPopupOpened();
            _headerTabContextMenu.Show(this, location);
        }

        private ContextMenuStrip BuildHeaderTabContextMenu(int tabIndex, BeepTabItem item)
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            AddHeaderTabCommand(menu, "Close", BeepTabCommandRouter.CloseCurrentCommand, tabIndex, CanCloseHeaderTab(tabIndex));
            AddHeaderTabCommand(menu, "Close Others", BeepTabCommandRouter.CloseOthersCommand, tabIndex, CanCloseOtherHeaderTabs(tabIndex));
            AddHeaderTabCommand(menu, "Close All", BeepTabCommandRouter.CloseAllCommand, tabIndex, CanCloseAllHeaderTabs());
            AddHeaderTabCommand(menu, "Close to the Right", BeepTabCommandRouter.CloseToRightCommand, tabIndex, CanCloseHeaderTabsToTheRight(tabIndex));

            if (TabMode != BeepTabMode.Navigation)
            {
                menu.Items.Add(new ToolStripSeparator());
                AddHeaderTabCommand(menu, item.IsPinned ? "Unpin" : "Pin", BeepTabCommandRouter.TogglePinCommand, tabIndex, CanTogglePinHeaderTab(tabIndex));
                AddHeaderTabCommand(menu, "Move Left", BeepTabCommandRouter.MoveLeftCommand, tabIndex, CanMoveHeaderTabLeft(tabIndex));
                AddHeaderTabCommand(menu, "Move Right", BeepTabCommandRouter.MoveRightCommand, tabIndex, CanMoveHeaderTabRight(tabIndex));
            }

            return menu;
        }

        private void AddHeaderTabCommand(ContextMenuStrip menu, string text, string commandName, int tabIndex, bool enabled)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text)
            {
                Enabled = enabled
            };

            item.Click += (sender, args) => ExecuteHeaderTabCommand(commandName, tabIndex);
            menu.Items.Add(item);
        }

        private void ExecuteHeaderTabCommand(string commandName, int tabIndex)
        {
            if (BeepTabCommandRouter.TryExecute(this, commandName, tabIndex))
            {
                Invalidate();
            }
        }

        private void HeaderTabContextMenu_Closed(object? sender, ToolStripDropDownClosedEventArgs e)
        {
            DisposeHeaderTabContextMenu();
            CloseChildPopup();
        }

        private void DisposeHeaderTabContextMenu()
        {
            if (_headerTabContextMenu == null)
            {
                return;
            }

            _headerTabContextMenu.Closed -= HeaderTabContextMenu_Closed;
            _headerTabContextMenu.Dispose();
            _headerTabContextMenu = null;
        }
    }
}