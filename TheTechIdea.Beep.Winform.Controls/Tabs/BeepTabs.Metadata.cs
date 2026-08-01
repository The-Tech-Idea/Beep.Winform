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
                IsPageBacked = true,
                IsSelected = index == selectedIndex,
                IsFocused = index == selectedIndex && Focused,
                IsEnabled = page.Enabled,
                IsVisible = true,
                CanClose = metadata.CanClose,
                CanSelect = page.Enabled && metadata.CanSelect,
                // Pinning does not make a tab immovable — it confines it to the pinned partition,
                // which CanReorderTabTo enforces by comparing the two tabs' pinned state. This flag
                // used to also clear CanReorder for any pinned tab, which was both redundant with
                // that check and stricter than it: pinned tabs could not be reordered even among
                // themselves, which VS, VS Code and Chrome all allow.
                CanReorder = metadata.CanReorder,
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
            if (!metadata.IsPageBacked)
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

            metadata.IsPageBacked = true;
            metadata.IsEnabled = page.Enabled;
            metadata.IsVisible = true;
        }
    }
}