using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Helpers
{
    /// <summary>
    /// Helper class for BeepListBox - handles layout calculations and common logic
    /// </summary>
    internal class BeepListBoxHelper
    {
        private readonly BeepListBox _owner;
        private readonly Dictionary<SimpleItem, int> _itemDepthMap = new();
        
        public BeepListBoxHelper(BeepListBox owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }
        
        /// <summary>
        /// Get the background color based on current state
        /// </summary>
        public Color GetBackgroundColor()
        {
            return _owner.BackColor;
        }
        
        /// <summary>
        /// Get the text color based on theme and state
        /// </summary>
        public Color GetTextColor()
        {
            return _owner.ForeColor;
        }
        
        /// <summary>
        /// Get the selected item background color
        /// </summary>
        public Color GetSelectedBackColor()
        {
            return _owner.SelectedBackColor;
        }
        
        /// <summary>
        /// Get the hover background color
        /// </summary>
        public Color GetHoverBackColor()
        {
            return _owner.HoverBackColor;
        }
        
        /// <summary>
        /// Calculate the visible items (filtered by search)
        /// </summary>
        public System.Collections.Generic.List<SimpleItem> GetVisibleItems()
        {
            if (_owner.ListItems == null || _owner.ListItems.Count == 0)
                return new System.Collections.Generic.List<SimpleItem>();
            
            var items = _owner.ListItems.ToList();

            // Flatten hierarchy if enabled
            if (_owner.ShowHierarchy)
            {
                _itemDepthMap.Clear();
                items = FlattenHierarchy(items);
            }
            
            // Apply search filter if needed
            if (_owner.ShowSearch && !string.IsNullOrWhiteSpace(_owner.SearchText))
            {
                items = _owner.ShowHierarchy
                    ? FilterHierarchyBySearch(items, _owner.SearchText)
                    : items.Where(i => IsSearchMatch(i, _owner.SearchText)).ToList();
            }

            if (_owner.ShowGroups && !_owner.ShowHierarchy)
            {
                return BuildGroupedRows(items);
            }
            
            return items;
        }

        private System.Collections.Generic.List<SimpleItem> BuildGroupedRows(System.Collections.Generic.List<SimpleItem> items)
        {
            var output = new System.Collections.Generic.List<SimpleItem>(items.Count + 16);
            var grouped = items.GroupBy(ResolveGroupKey).OrderBy(g => g.Key);

            foreach (var group in grouped)
            {
                string key = group.Key;
                bool collapsed = _owner.IsGroupCollapsed(key);
                int childCount = group.Count();

                output.Add(new BeepListItem(key)
                {
                    Category = key,
                    IsGroupHeader = true,
                    GroupItemCount = childCount,
                    IsEnabled = true,
                    GuidId = $"group::{key}"
                });

                if (collapsed)
                {
                    continue;
                }

                output.AddRange(group);
            }

            return output;
        }

        private static string ResolveGroupKey(SimpleItem item)
        {
            if (item is BeepListItem rich && !string.IsNullOrWhiteSpace(rich.Category))
            {
                return rich.Category.Trim();
            }

            if (!string.IsNullOrWhiteSpace(item?.GroupName))
            {
                return item.GroupName.Trim();
            }

            return "Ungrouped";
        }

        // ── Hierarchy helpers ─────────────────────────────────────────────────────

        private List<SimpleItem> FlattenHierarchy(IEnumerable<SimpleItem> items, int depth = 0)
        {
            var result = new List<SimpleItem>();
            if (depth > ListBoxTokens.MaxHierarchyDepth) return result;

            foreach (var item in items)
            {
                if (!item.IsVisible) continue;
                _itemDepthMap[item] = depth;
                result.Add(item);

                if (item.IsExpanded && item.Children != null && item.Children.Count > 0)
                {
                    result.AddRange(FlattenHierarchy(item.Children, depth + 1));
                }
            }
            return result;
        }

        private List<SimpleItem> FilterHierarchyBySearch(List<SimpleItem> flatItems, string query)
        {
            var matching = new HashSet<SimpleItem>();
            foreach (var item in flatItems)
            {
                if (IsSearchMatch(item, query))
                {
                    matching.Add(item);
                    var parent = item.ParentItem;
                    while (parent != null)
                    {
                        matching.Add(parent);
                        parent = parent.ParentItem;
                    }
                }
            }
            return flatItems.Where(i => matching.Contains(i)).ToList();
        }

        internal int GetItemDepth(SimpleItem item)
            => _itemDepthMap.TryGetValue(item, out int d) ? d : 0;

        internal bool ItemHasChildren(SimpleItem item)
            => item.Children != null && item.Children.Count > 0;

        // ── Search ────────────────────────────────────────────────────────────────

        private bool IsSearchMatch(SimpleItem item, string query)
        {
            if (item == null || string.IsNullOrWhiteSpace(query))
            {
                return true;
            }

            string q = query.Trim();
            string text = item.Text ?? string.Empty;
            string sub = (item as BeepListItem)?.SubText ?? item.SubText ?? string.Empty;
            return IsMatchCore(text, q) || IsMatchCore(sub, q);
        }

        private bool IsMatchCore(string text, string query)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            switch (_owner.SearchMode)
            {
                case ListSearchMode.StartsWith:
                    return text.StartsWith(query, StringComparison.OrdinalIgnoreCase);
                case ListSearchMode.Fuzzy:
                    return FuzzyScore(text, query) > 0;
                default:
                    return text.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        private static int FuzzyScore(string source, string query)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(query))
            {
                return 0;
            }

            if (source.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            {
                return 100;
            }

            if (source.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return 60;
            }

            int score = 0;
            int qi = 0;
            string s = source.ToLowerInvariant();
            string q = query.ToLowerInvariant();
            for (int i = 0; i < s.Length && qi < q.Length; i++)
            {
                if (s[i] == q[qi])
                {
                    qi++;
                    score += 10;
                }
            }

            return qi == q.Length ? score : 0;
        }
        
        /// <summary>
        /// Measure text size without creating Graphics object
        /// Uses TextUtils for caching performance
        /// </summary>
        public Size MeasureText(string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;
            
            SizeF textSizeF = TextUtils.MeasureText(text, font);
            return new Size((int)textSizeF.Width, (int)textSizeF.Height);
        }
        
        /// <summary>
        /// Find an item by its text
        /// </summary>
        public SimpleItem FindItemByText(string text)
        {
            if (string.IsNullOrEmpty(text) || _owner.ListItems == null)
                return null;
            
            return _owner.ListItems.FirstOrDefault(i => 
                string.Equals(i.Text, text, StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Get the item at a specific point
        /// </summary>
        public SimpleItem GetItemAtPoint(Point point, Rectangle contentArea, int itemHeight)
        {
            var visibleItems = GetVisibleItems();
            if (visibleItems == null || visibleItems.Count == 0)
                return null;
            
            int currentY = contentArea.Top;
            foreach (var item in visibleItems)
            {
                Rectangle itemRect = new Rectangle(
                    contentArea.Left,
                    currentY,
                    contentArea.Width,
                    itemHeight);
                
                if (itemRect.Contains(point))
                    return item;
                
                currentY += itemHeight;
                if (currentY >= contentArea.Bottom)
                    break;
            }
            
            return null;
        }
    }
}
