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
            return ModeCapabilities.SupportsPinning && TryGetHeaderTabItem(tabIndex, out _);
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
                && (!ModeCapabilities.SupportsPinning || !item.IsPinned);
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

            return CanReorderTabTo(tabIndex, tabIndex + direction);
        }

        /// <summary>
        /// Whether the tab at <paramref name="tabIndex"/> may begin a drag at all — i.e. it exists,
        /// is usable, and is not itself marked as non-reorderable. The destination is validated
        /// separately by <see cref="CanReorderTabTo"/> once a drop target is known.
        /// </summary>
        internal bool CanDragTab(int tabIndex)
        {
            return TryGetHeaderTabItem(tabIndex, out BeepTabItem? item)
                && item != null
                && item.CanReorder
                && item.IsEnabled
                && item.IsVisible;
        }

        /// <summary>
        /// Whether the tab at <paramref name="fromIndex"/> may be moved to
        /// <paramref name="toIndex"/>. The single reorder rule, used by both the Move Left/Right
        /// commands and by drag-and-drop.
        /// </summary>
        /// <remarks>
        /// Drag-and-drop used to call <c>TryMoveHostedSourceItem</c> directly, checking nothing at
        /// all, while the context menu enforced these rules. A pinned tab could not be moved past an
        /// unpinned one with Move Right, but could be dragged anywhere — the constraints applied
        /// only to the path users were less likely to take.
        /// </remarks>
        internal bool CanReorderTabTo(int fromIndex, int toIndex)
        {
            if (!ModeCapabilities.SupportsDragReorder)
            {
                return false;
            }

            if (fromIndex == toIndex)
            {
                return false;
            }

            if (!TryGetHeaderTabItem(fromIndex, out BeepTabItem? item) || item == null)
            {
                return false;
            }

            if (!item.CanReorder || !item.IsEnabled || !item.IsVisible)
            {
                return false;
            }

            if (!TryGetHeaderTabItem(toIndex, out BeepTabItem? targetItem) || targetItem == null)
            {
                return false;
            }

            // Pinned tabs form their own partition at the head of the run; a move may not cross it.
            if (ModeCapabilities.SupportsPinning && item.IsPinned != targetItem.IsPinned)
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