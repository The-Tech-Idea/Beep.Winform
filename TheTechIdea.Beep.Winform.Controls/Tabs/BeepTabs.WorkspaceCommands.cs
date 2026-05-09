using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        internal bool CanSelectHeaderTab(int tabIndex)
        {
            return TryGetHeaderTabItem(tabIndex, out BeepTabItem? item)
                && item.CanSelect
                && item.IsEnabled
                && item.IsVisible;
        }

        internal bool CanCloseCurrentHeaderTab()
        {
            return CanCloseHeaderTab(GetHostedSourceSelectedIndex());
        }

        internal bool CanCloseOtherHeaderTabs(int keepIndex)
        {
            if (!TryGetHeaderTabItem(keepIndex, out _))
            {
                return false;
            }

            var items = GetHostedSourceItemsSnapshot();
            for (int index = 0; index < items.Count; index++)
            {
                if (index == keepIndex)
                {
                    continue;
                }

                if (CanCloseHeaderTab(index))
                {
                    return true;
                }
            }

            return false;
        }

        internal bool CanCloseAllHeaderTabs()
        {
            var items = GetHostedSourceItemsSnapshot();
            for (int index = 0; index < items.Count; index++)
            {
                if (CanCloseHeaderTab(index))
                {
                    return true;
                }
            }

            return false;
        }

        internal bool CanCloseHeaderTabsToTheRight(int tabIndex)
        {
            var items = GetHostedSourceItemsSnapshot();
            if (tabIndex < 0 || tabIndex >= items.Count)
            {
                return false;
            }

            for (int index = tabIndex + 1; index < items.Count; index++)
            {
                if (CanCloseHeaderTab(index))
                {
                    return true;
                }
            }

            return false;
        }

        internal bool CanTogglePinHeaderTab(int tabIndex)
        {
            return TabMode != BeepTabMode.Navigation && TryGetHeaderTabItem(tabIndex, out _);
        }

        internal bool CanMoveHeaderTabLeft(int tabIndex)
        {
            return CanMoveHeaderTab(tabIndex, -1);
        }

        internal bool CanMoveHeaderTabRight(int tabIndex)
        {
            return CanMoveHeaderTab(tabIndex, 1);
        }

        internal bool CanShowHeaderOverflow()
        {
            return HeaderOverflowPolicy == BeepTabOverflowPolicy.OverflowMenu && GetHeaderOverflowState().HasOverflow;
        }

        internal bool TryCloseOtherHeaderTabs(int keepIndex)
        {
            if (!CanCloseOtherHeaderTabs(keepIndex))
            {
                return false;
            }

            bool closedAny = false;
            for (int index = GetHostedSourceItemCount() - 1; index >= 0; index--)
            {
                if (index == keepIndex || !CanCloseHeaderTab(index))
                {
                    continue;
                }

                closedAny |= TryCloseHeaderTab(index);
            }

            return closedAny;
        }

        internal bool TryCloseAllHeaderTabs()
        {
            if (!CanCloseAllHeaderTabs())
            {
                return false;
            }

            bool closedAny = false;
            for (int index = GetHostedSourceItemCount() - 1; index >= 0; index--)
            {
                if (!CanCloseHeaderTab(index))
                {
                    continue;
                }

                closedAny |= TryCloseHeaderTab(index);
            }

            return closedAny;
        }

        internal bool TryCloseHeaderTabsToTheRight(int tabIndex)
        {
            if (!CanCloseHeaderTabsToTheRight(tabIndex))
            {
                return false;
            }

            bool closedAny = false;
            for (int index = GetHostedSourceItemCount() - 1; index > tabIndex; index--)
            {
                if (!CanCloseHeaderTab(index))
                {
                    continue;
                }

                closedAny |= TryCloseHeaderTab(index);
            }

            return closedAny;
        }

        internal bool TryTogglePinHeaderTab(int tabIndex)
        {
            if (!CanTogglePinHeaderTab(tabIndex))
            {
                return false;
            }

            BeepTabPage? page = GetHostedSourcePageAt(tabIndex);
            if (page == null)
            {
                return false;
            }

            BeepTabItem metadata = GetOrCreateHostedTabMetadata(page);
            metadata.IsPinned = !metadata.IsPinned;
            MoveHostedPageForPinnedState(page, metadata.IsPinned);
            RefreshWorkspaceCommandState();
            return true;
        }

        internal bool TryMoveHeaderTabLeft(int tabIndex)
        {
            if (!CanMoveHeaderTabLeft(tabIndex))
            {
                return false;
            }

            bool moved = TryMoveHostedSourceItem(tabIndex, tabIndex - 1);
            if (moved)
            {
                RefreshWorkspaceCommandState();
            }

            return moved;
        }

        internal bool TryMoveHeaderTabRight(int tabIndex)
        {
            if (!CanMoveHeaderTabRight(tabIndex))
            {
                return false;
            }

            bool moved = TryMoveHostedSourceItem(tabIndex, tabIndex + 1);
            if (moved)
            {
                RefreshWorkspaceCommandState();
            }

            return moved;
        }

        private bool CanCloseHeaderTab(int tabIndex)
        {
            return TryGetHeaderTabItem(tabIndex, out BeepTabItem? item)
                && item.CanClose
                && item.IsEnabled
                && item.IsVisible
                && (TabMode == BeepTabMode.Navigation || !item.IsPinned);
        }

        private bool CanMoveHeaderTab(int tabIndex, int direction)
        {
            if (!TryGetHeaderTabItem(tabIndex, out BeepTabItem? item)
                || !item.CanReorder
                || !item.IsEnabled
                || !item.IsVisible)
            {
                return false;
            }

            int targetIndex = tabIndex + direction;
            if (!TryGetHeaderTabItem(targetIndex, out BeepTabItem? targetItem))
            {
                return false;
            }

            if (TabMode != BeepTabMode.Navigation && item.IsPinned != targetItem.IsPinned)
            {
                return false;
            }

            return true;
        }

        private bool TryGetHeaderTabItem(int tabIndex, out BeepTabItem? item)
        {
            item = null;
            var items = GetHostedSourceItemsSnapshot();
            if (tabIndex < 0 || tabIndex >= items.Count)
            {
                return false;
            }

            item = items[tabIndex];
            return true;
        }

        private void MoveHostedPageForPinnedState(BeepTabPage page, bool isPinned)
        {
            int currentIndex = _hostedPages.IndexOf(page);
            if (currentIndex < 0)
            {
                return;
            }

            if (isPinned)
            {
                int targetIndex = 0;
                while (targetIndex < _hostedPages.Count)
                {
                    BeepTabPage candidate = _hostedPages[targetIndex];
                    if (ReferenceEquals(candidate, page) || !GetOrCreateHostedTabMetadata(candidate).IsPinned)
                    {
                        break;
                    }

                    targetIndex++;
                }

                if (currentIndex != targetIndex)
                {
                    TryMoveHostedSourceItem(currentIndex, targetIndex);
                }

                return;
            }

            int pinnedCount = 0;
            foreach (BeepTabPage candidate in _hostedPages)
            {
                if (ReferenceEquals(candidate, page))
                {
                    continue;
                }

                if (GetOrCreateHostedTabMetadata(candidate).IsPinned)
                {
                    pinnedCount++;
                }
            }

            if (currentIndex < pinnedCount)
            {
                TryMoveHostedSourceItem(currentIndex, pinnedCount);
            }
        }

        private void RefreshWorkspaceCommandState()
        {
            UpdateItemSize();
            Invalidate();
        }
    }
}