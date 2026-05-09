using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Customization;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        private static string GetDisplayText(SimpleItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.DisplayField))
            {
                return item.DisplayField;
            }

            if (!string.IsNullOrWhiteSpace(item.Text))
            {
                return item.Text;
            }

            return item.Name ?? string.Empty;
        }

        private static string GetCommandKey(SimpleItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.ActionID)) return item.ActionID;
            if (!string.IsNullOrWhiteSpace(item.ReferenceID)) return item.ReferenceID;
            if (!string.IsNullOrWhiteSpace(item.GuidId)) return item.GuidId;
            if (!string.IsNullOrWhiteSpace(item.Name)) return item.Name;
            if (!string.IsNullOrWhiteSpace(item.Text)) return item.Text;
            return Guid.NewGuid().ToString("N");
        }

        private void RebuildCommandLookup(IEnumerable<SimpleItem> tabNodes)
        {
            _commandLookup.Clear();
            foreach (var node in tabNodes)
            {
                AddCommandLookupRecursive(node);
            }
        }

        private void AddCommandLookupRecursive(SimpleItem node)
        {
            if (node.IsSeparator) return;
            string key = GetCommandKey(node);
            if (!_commandLookup.ContainsKey(key))
            {
                _commandLookup[key] = node;
            }

            foreach (var child in node.Children.Where(IsVisibleNode))
            {
                AddCommandLookupRecursive(child);
            }
        }

        private List<RibbonTabState> CaptureTabStates()
        {
            var states = new List<RibbonTabState>();
            for (int tabIndex = 0; tabIndex < _commandItems.Count; tabIndex++)
            {
                var tab = _commandItems[tabIndex];
                var tabState = new RibbonTabState
                {
                    TabKey = GetMergeKey(tab),
                    Visible = tab.IsVisible,
                    Order = tabIndex
                };

                for (int groupIndex = 0; groupIndex < tab.Children.Count; groupIndex++)
                {
                    var group = tab.Children[groupIndex];
                    tabState.Groups.Add(new RibbonGroupState
                    {
                        GroupKey = GetMergeKey(group),
                        Visible = group.IsVisible,
                        Order = groupIndex
                    });
                }

                states.Add(tabState);
            }

            return states;
        }

        private void ApplyTabStates(IEnumerable<RibbonTabState>? tabStates)
        {
            if (tabStates == null)
            {
                return;
            }

            var states = tabStates.ToList();
            if (states.Count == 0)
            {
                return;
            }

            var currentByKey = CreateNodeMap(_commandItems);
            var orderedTabs = new List<SimpleItem>();

            foreach (var tabState in states.OrderBy(s => s.Order))
            {
                if (string.IsNullOrWhiteSpace(tabState.TabKey)) continue;
                if (!currentByKey.TryGetValue(tabState.TabKey, out var tabNode)) continue;

                tabNode.IsVisible = tabState.Visible;
                ApplyGroupStates(tabNode, tabState.Groups);
                orderedTabs.Add(tabNode);
                currentByKey.Remove(tabState.TabKey);
            }

            orderedTabs.AddRange(currentByKey.Values);

            _suspendCommandRebuild = true;
            try
            {
                _commandItems.Clear();
                foreach (var tab in orderedTabs)
                {
                    _commandItems.Add(tab);
                }
            }
            finally
            {
                _suspendCommandRebuild = false;
            }
        }

        private static void ApplyGroupStates(SimpleItem tabNode, IEnumerable<RibbonGroupState>? groupStates)
        {
            if (groupStates == null)
            {
                return;
            }

            var states = groupStates.ToList();
            if (states.Count == 0)
            {
                return;
            }

            var currentByKey = CreateNodeMap(tabNode.Children);
            var orderedGroups = new List<SimpleItem>();

            foreach (var groupState in states.OrderBy(s => s.Order))
            {
                if (string.IsNullOrWhiteSpace(groupState.GroupKey)) continue;
                if (!currentByKey.TryGetValue(groupState.GroupKey, out var groupNode)) continue;

                groupNode.IsVisible = groupState.Visible;
                orderedGroups.Add(groupNode);
                currentByKey.Remove(groupState.GroupKey);
            }

            orderedGroups.AddRange(currentByKey.Values);

            tabNode.Children.Clear();
            foreach (var group in orderedGroups)
            {
                tabNode.Children.Add(group);
            }
        }

        private SimpleItem? GetTabNode(string tabKey)
        {
            if (string.IsNullOrWhiteSpace(tabKey)) return null;
            return _commandItems.FirstOrDefault(t => GetMergeKey(t).Equals(tabKey, StringComparison.OrdinalIgnoreCase));
        }

        private int FindTabIndex(string tabKey)
        {
            if (string.IsNullOrWhiteSpace(tabKey)) return -1;
            for (int i = 0; i < _commandItems.Count; i++)
            {
                if (GetMergeKey(_commandItems[i]).Equals(tabKey, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private static SimpleItem? GetGroupNode(string tabKey, string groupKey, IEnumerable<SimpleItem> tabs)
        {
            if (string.IsNullOrWhiteSpace(tabKey) || string.IsNullOrWhiteSpace(groupKey)) return null;
            var tab = tabs.FirstOrDefault(t => GetMergeKey(t).Equals(tabKey, StringComparison.OrdinalIgnoreCase));
            return tab?.Children.FirstOrDefault(g => GetMergeKey(g).Equals(groupKey, StringComparison.OrdinalIgnoreCase));
        }

        private SimpleItem? GetGroupNode(string tabKey, string groupKey)
        {
            return GetGroupNode(tabKey, groupKey, _commandItems);
        }

        private static int FindGroupIndex(SimpleItem tab, string groupKey)
        {
            if (string.IsNullOrWhiteSpace(groupKey)) return -1;
            for (int i = 0; i < tab.Children.Count; i++)
            {
                if (GetMergeKey(tab.Children[i]).Equals(groupKey, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool SetTabVisible(string tabKey, bool visible)
        {
            var tab = GetTabNode(tabKey);
            if (tab == null) return false;
            if (tab.IsVisible == visible) return true;
            tab.IsVisible = visible;
            BuildFromSimpleItems();
            return true;
        }

        public bool MoveTab(string tabKey, int newIndex)
        {
            int oldIndex = FindTabIndex(tabKey);
            if (oldIndex < 0) return false;
            if (newIndex < 0 || newIndex >= _commandItems.Count) return false;
            if (oldIndex == newIndex) return true;

            var tab = _commandItems[oldIndex];
            _suspendCommandRebuild = true;
            try
            {
                _commandItems.RemoveAt(oldIndex);
                _commandItems.Insert(newIndex, tab);
            }
            finally
            {
                _suspendCommandRebuild = false;
            }

            BuildFromSimpleItems();
            return true;
        }

        public bool SetGroupVisible(string tabKey, string groupKey, bool visible)
        {
            var group = GetGroupNode(tabKey, groupKey);
            if (group == null) return false;
            if (group.IsVisible == visible) return true;
            group.IsVisible = visible;
            BuildFromSimpleItems();
            return true;
        }

        public bool MoveGroup(string tabKey, string groupKey, int newIndex)
        {
            var tab = GetTabNode(tabKey);
            if (tab == null) return false;
            int oldIndex = FindGroupIndex(tab, groupKey);
            if (oldIndex < 0) return false;
            if (newIndex < 0 || newIndex >= tab.Children.Count) return false;
            if (oldIndex == newIndex) return true;

            var group = tab.Children[oldIndex];
            tab.Children.RemoveAt(oldIndex);
            tab.Children.Insert(newIndex, group);
            BuildFromSimpleItems();
            return true;
        }

        private static Dictionary<string, SimpleItem> CreateNodeMap(IEnumerable<SimpleItem> nodes)
        {
            var map = new Dictionary<string, SimpleItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var node in nodes)
            {
                string key = GetMergeKey(node);
                if (!map.ContainsKey(key))
                {
                    map[key] = node;
                }
            }

            return map;
        }

        private void ReplaceCommandItems(IEnumerable<SimpleItem> tabs)
        {
            _suspendCommandRebuild = true;
            try
            {
                _commandItems.Clear();
                foreach (var tab in tabs)
                {
                    _commandItems.Add(tab);
                }
            }
            finally
            {
                _suspendCommandRebuild = false;
            }

            BuildFromSimpleItems();
        }

        private static List<SimpleItem> AppendTabs(List<SimpleItem> baseTabs, IEnumerable<SimpleItem> sourceTabs)
        {
            var result = CloneNodeList(baseTabs);
            result.AddRange(CloneNodeList(sourceTabs));
            return result;
        }

        private static List<SimpleItem> MergeByName(List<SimpleItem> baseTabs, IEnumerable<SimpleItem> sourceTabs)
        {
            var result = CloneNodeList(baseTabs);
            foreach (var sourceTab in sourceTabs)
            {
                var existingTab = result.FirstOrDefault(t => HasSameMergeKey(t, sourceTab));
                if (existingTab == null)
                {
                    result.Add(CloneNode(sourceTab));
                    continue;
                }

                MergeChildrenByName(existingTab.Children, sourceTab.Children);
            }

            return result;
        }

        private static void MergeChildrenByName(BindingList<SimpleItem> targetChildren, IEnumerable<SimpleItem> sourceChildren)
        {
            foreach (var source in sourceChildren)
            {
                var existing = targetChildren.FirstOrDefault(t => HasSameMergeKey(t, source));
                if (existing == null)
                {
                    targetChildren.Add(CloneNode(source));
                    continue;
                }

                if (source.Children.Count > 0)
                {
                    MergeChildrenByName(existing.Children, source.Children);
                }
            }
        }

        private static bool HasSameMergeKey(SimpleItem left, SimpleItem right)
        {
            return string.Equals(GetMergeKey(left), GetMergeKey(right), StringComparison.OrdinalIgnoreCase);
        }

        private static string GetMergeKey(SimpleItem node)
        {
            if (!string.IsNullOrWhiteSpace(node.ActionID)) return node.ActionID;
            if (!string.IsNullOrWhiteSpace(node.ReferenceID)) return node.ReferenceID;
            if (!string.IsNullOrWhiteSpace(node.Name)) return node.Name;
            if (!string.IsNullOrWhiteSpace(node.Text)) return node.Text;
            if (!string.IsNullOrWhiteSpace(node.DisplayField)) return node.DisplayField;
            if (!string.IsNullOrWhiteSpace(node.GuidId)) return node.GuidId;
            return $"{node.MenuID}:{node.MenuName}:{node.ItemType}";
        }

        private static List<SimpleItem> CloneNodeList(IEnumerable<SimpleItem> nodes)
        {
            return nodes.Select(CloneNode).ToList();
        }

        private static SimpleItem CloneNode(SimpleItem node)
        {
            var clone = new SimpleItem
            {
                ID = node.ID,
                Guid = node.Guid,
                GuidId = node.GuidId,
                Name = node.Name,
                MenuName = node.MenuName,
                Text = node.Text,
                DisplayField = node.DisplayField,
                Description = node.Description,
                SubText = node.SubText,
                SubText2 = node.SubText2,
                SubText3 = node.SubText3,
                ImagePath = node.ImagePath,
                ToolTip = node.ToolTip,
                Shortcut = node.Shortcut,
                ShortcutText = node.ShortcutText,
                KeyCombination = node.KeyCombination,
                BadgeText = node.BadgeText,
                BadgeBackColor = node.BadgeBackColor,
                BadgeForeColor = node.BadgeForeColor,
                BadgeShape = node.BadgeShape,
                IsCheckable = node.IsCheckable,
                IsChecked = node.IsChecked,
                IsEnabled = node.IsEnabled,
                IsVisible = node.IsVisible,
                IsExpanded = node.IsExpanded,
                IsSelected = node.IsSelected,
                IsSeparator = node.IsSeparator,
                MenuID = node.MenuID,
                ActionID = node.ActionID,
                ReferenceID = node.ReferenceID,
                ParentID = node.ParentID,
                OwnerReferenceID = node.OwnerReferenceID,
                OtherReferenceID = node.OtherReferenceID,
                PointType = node.PointType,
                ObjectType = node.ObjectType,
                BranchClass = node.BranchClass,
                BranchName = node.BranchName,
                BranchType = node.BranchType,
                MethodName = node.MethodName,
                ItemType = node.ItemType,
                Category = node.Category,
                Uri = node.Uri,
                AssemblyClassDefinitionID = node.AssemblyClassDefinitionID,
                ClassDefinitionID = node.ClassDefinitionID,
                PackageName = node.PackageName,
                BranchID = node.BranchID,
                ClassName = node.ClassName,
                GroupName = node.GroupName
            };

            foreach (var child in node.Children)
            {
                clone.Children.Add(CloneNode(child));
            }

            return clone;
        }

        private static bool IsVisibleNode(SimpleItem item)
        {
            return item.IsVisible;
        }

        private static List<SimpleItem> NormalizeSeparators(IEnumerable<SimpleItem> nodes)
        {
            var normalized = new List<SimpleItem>();
            bool previousWasSeparator = true;
            foreach (var node in nodes)
            {
                if (node.IsSeparator)
                {
                    if (previousWasSeparator)
                    {
                        continue;
                    }

                    previousWasSeparator = true;
                    normalized.Add(node);
                    continue;
                }

                previousWasSeparator = false;
                normalized.Add(node);
            }

            while (normalized.Count > 0 && normalized[^1].IsSeparator)
            {
                normalized.RemoveAt(normalized.Count - 1);
            }

            return normalized;
        }

        private static int FindBackstageItemIndexByKey(IList<SimpleItem> items, string itemKey)
        {
            if (string.IsNullOrWhiteSpace(itemKey))
            {
                return -1;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (GetMergeKey(items[i]).Equals(itemKey, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool RemoveBackstageItemByKey(IList<SimpleItem> items, string itemKey)
        {
            int index = FindBackstageItemIndexByKey(items, itemKey);
            if (index < 0)
            {
                return false;
            }

            items.RemoveAt(index);
            return true;
        }
    }
}
