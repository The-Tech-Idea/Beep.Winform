using System;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        internal void NotifyHostedPageMetadataChanged(BeepTabPage page)
        {
            if (page == null || !ContainsHostedSourcePage(page))
            {
                return;
            }

            NormalizeHostedTabMetadata(GetOrCreateHostedTabMetadata(page), page);
            UpdateItemSize();
            Invalidate();
        }

        public void ConfigureTabItem(int index, Action<BeepTabItem> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            BeepTabPage? page = GetHostedSourcePageAt(index);
            if (page == null)
            {
                return;
            }

            BeepTabItem metadata = GetOrCreateHostedTabMetadata(page);
            configure(metadata);
            NormalizeHostedTabMetadata(metadata, page);

            UpdateItemSize();
            Invalidate();
        }

        public void ConfigureWorkspaceState(int index, Action<BeepTabWorkspaceState> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            ConfigureTabItem(index, item => configure(item.WorkspaceState));
        }

        public void ClearTabItemConfiguration(int index)
        {
            BeepTabPage? page = GetHostedSourcePageAt(index);
            if (page == null)
            {
                return;
            }

            page.ResetSerializedTabMetadata();
            UpdateItemSize();
            Invalidate();
        }

        internal BeepTabItem? GetHostedSourceSelectedItemSnapshot()
        {
            IReadOnlyList<BeepTabItem> items = GetHostedSourceItemsSnapshot();
            int selectedIndex = GetHostedSourceSelectedIndex();
            if (selectedIndex < 0 || selectedIndex >= items.Count)
            {
                return null;
            }

            return items[selectedIndex];
        }

        internal BeepTabItem CreateHostedTabItemSnapshot(BeepTabPage page, int index, int selectedIndex)
        {
            BeepTabItem metadata = GetOrCreateHostedTabMetadata(page);
            NormalizeHostedTabMetadata(metadata, page);

            return new BeepTabItem
            {
                Index = index,
                Name = metadata.Name,
                Title = metadata.Title,
                Content = page,
                IsSelected = index == selectedIndex,
                IsFocused = index == selectedIndex && Focused,
                IsEnabled = page.Enabled,
                IsVisible = true,
                CanClose = metadata.CanClose,
                CanSelect = page.Enabled && metadata.CanSelect,
                CanReorder = metadata.CanReorder && !(TabMode != BeepTabMode.Navigation && metadata.IsPinned),
                WorkspaceState = metadata.WorkspaceState.Clone(),
                IconPath = metadata.IconPath,
                SubText = metadata.SubText,
                BadgeText = metadata.BadgeText,
                BadgeKind = metadata.BadgeKind,
                IsBusy = metadata.IsBusy,
                CloseVisible = metadata.CloseVisible
            };
        }

        internal BeepTabItem GetOrCreateHostedTabMetadata(BeepTabPage page)
        {
            BeepTabItem metadata = page.TabMetadata;
            if (metadata.Content == null)
            {
                page.ResetTabMetadata();
                metadata = page.TabMetadata;
            }

            return metadata;
        }

        private static void NormalizeHostedTabMetadata(BeepTabItem metadata, BeepTabPage page)
        {
            metadata.Name = string.IsNullOrWhiteSpace(metadata.Name) ? page.Name : metadata.Name;

            if (string.IsNullOrWhiteSpace(metadata.Title))
            {
                metadata.Title = page.Text ?? string.Empty;
            }
            else if (!string.Equals(page.Text, metadata.Title, StringComparison.Ordinal))
            {
                page.Text = metadata.Title;
            }

            metadata.Content = page;
            metadata.IsEnabled = page.Enabled;
            metadata.IsVisible = true;
        }
    }
}