using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using System;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    public partial class BeepSideBar
    {
        #region Accordion Fields
        private Dictionary<SimpleItem, bool> _expandedState = new Dictionary<SimpleItem, bool>();
        private HashSet<SimpleItem> _subscribedItems = new HashSet<SimpleItem>();
        private int _childItemHeight = 34;
        private int _indentationWidth = 20;
        #endregion

        #region Accordion Properties
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Category("Layout")]
        [System.ComponentModel.Description("Height of child menu items in accordion.")]
        [System.ComponentModel.DefaultValue(34)]
        public int ChildItemHeight
        {
            get => _childItemHeight;
            set
            {
                _childItemHeight = System.Math.Max(20, value);
                Invalidate();
            }
        }

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.Category("Layout")]
        [System.ComponentModel.Description("Indentation width for child items.")]
        [System.ComponentModel.DefaultValue(20)]
        public int IndentationWidth
        {
            get => _indentationWidth;
            set
            {
                _indentationWidth = System.Math.Max(10, value);
                Invalidate();
            }
        }
        #endregion

        #region Accordion Methods
        /// <summary>
        /// Initializes the expanded state for all menu items with children
        /// and ensures that event handlers for property/child-list changes are wired.
        /// </summary>
        private void InitializeExpandedState()
        {
            if (_items == null) return;

            // Ensure we subscribe to property changed on items (and their children)
            SubscribeToItems();

            foreach (var item in _items)
            {
                if (item.Children != null && item.Children.Count > 0)
                {
                    // Use the item's IsExpanded value if present, otherwise fall back to collapsed
                    if (!_expandedState.ContainsKey(item))
                    {
                        _expandedState[item] = item.IsExpanded;
                    }
                    else
                    {
                        _expandedState[item] = item.IsExpanded;
                    }

                    // Initialize nested children
                    InitializeExpandedStateRecursive(item);
                }
            }
        }

        /// <summary>
        /// Recursively initializes expanded state for nested items
        /// </summary>
        private void InitializeExpandedStateRecursive(SimpleItem parentItem)
        {
            if (parentItem.Children == null) return;

            foreach (var child in parentItem.Children.Cast<SimpleItem>())
            {
                // Initialize expanded state for this child
                if (!_expandedState.ContainsKey(child))
                {
                    _expandedState[child] = child.IsExpanded;
                }
                else
                {
                    _expandedState[child] = child.IsExpanded;
                }

                if (child.Children != null && child.Children.Count > 0)
                {
                    InitializeExpandedStateRecursive(child);
                }
            }
        }

        /// <summary>
        /// Toggles the expanded state of a menu item
        /// </summary>
        public void ToggleItemExpansion(SimpleItem item)
        {
            if (item == null || item.Children == null || item.Children.Count == 0)
                return;

            bool newState = !_expandedState.ContainsKey(item) ? true : !_expandedState[item];
            // Allow consumers to cancel expansion change
            if (!((BeepSideBar)this).OnItemExpansionChanging(item, newState))
                return;

            if (_expandedState.ContainsKey(item))
            {
                _expandedState[item] = newState;
            }
            else
            {
                _expandedState[item] = newState;
            }

            // Keep the SimpleItem.IsExpanded property in sync
            try { item.IsExpanded = _expandedState[item]; }
            catch (Exception)
            {
                // Protect against property changes causing unexpected behavior
            }

            // Raise expansion changed event
            try { OnItemExpansionChanged(item, _expandedState[item]); } catch { }

            RefreshHitAreas();
            Invalidate();
        }

        /// <summary>
        /// Gets the expanded state of a menu item
        /// </summary>
        public bool IsItemExpanded(SimpleItem item)
        {
            if (item == null || item.Children == null || item.Children.Count == 0)
                return false;

            return _expandedState.ContainsKey(item) && _expandedState[item];
        }

        /// <summary>
        /// Expands a menu item
        /// </summary>
        public void ExpandItem(SimpleItem item)
        {
            if (item == null || item.Children == null || item.Children.Count == 0)
                return;

            if (!((BeepSideBar)this).OnItemExpansionChanging(item, true)) return;
            _expandedState[item] = true;
            try
            {
                item.IsExpanded = true;
            }
            catch { }
            try { OnItemExpansionChanged(item, true); } catch { }
            try
            {
                OnItemExpansionChanged(item, true);
            }
            catch { }
            RefreshHitAreas();
            Invalidate();
        }

        /// <summary>
        /// Collapses a menu item
        /// </summary>
        public void CollapseItem(SimpleItem item)
        {
            if (item == null)
                return;
            if (!((BeepSideBar)this).OnItemExpansionChanging(item, false)) return;
            _expandedState[item] = false;
            try
            {
                item.IsExpanded = false;
            }
            catch { }
            try { OnItemExpansionChanged(item, false); } catch { }
            try
            {
                OnItemExpansionChanged(item, false);
            }
            catch { }
            RefreshHitAreas();
            Invalidate();
        }

        /// <summary>
        /// Collapses all menu items
        /// </summary>
        public void CollapseAll()
        {
            foreach (var key in _expandedState.Keys.ToList())
            {
                _expandedState[key] = false;
            }

            RefreshHitAreas();
            Invalidate();
        }

        /// <summary>
        /// Expands all menu items
        /// </summary>
        public void ExpandAll()
        {
            foreach (var key in _expandedState.Keys.ToList())
            {
                _expandedState[key] = true;
                try
                {
                    key.IsExpanded = true;
                }
                catch { }
            }

            RefreshHitAreas();
            Invalidate();
        }
        #endregion

        #region Subscription Helpers
        /// <summary>
        /// Attach handlers to top-level items and recursively to children so that we can
        /// reflect property changes (IsExpanded) and adjust subscriptions if the Children list changes.
        /// </summary>
        private void SubscribeToItems()
        {
            // Unsubscribe previous handlers first
            UnsubscribeFromItems();

            if (_items == null) return;

            foreach (var item in _items)
            {
                SubscribeToItemTree(item);
            }
        }

        private void UnsubscribeFromItems()
        {
            if (_subscribedItems == null || _subscribedItems.Count == 0) return;

            // Unsubscribe from all previously subscribed items
            foreach (var item in _subscribedItems.ToList())
            {
                UnsubscribeFromItemTree(item);
            }

            _subscribedItems.Clear();
        }

        private void SubscribeToItemTree(SimpleItem item)
        {
            if (item == null) return;

            if (!_subscribedItems.Contains(item))
            {
                item.PropertyChanged -= Item_PropertyChanged;
                item.PropertyChanged += Item_PropertyChanged;
                _subscribedItems.Add(item);
            }

            if (item.Children != null)
            {
                item.Children.ListChanged -= Children_ListChanged;
                item.Children.ListChanged += Children_ListChanged;

                foreach (var child in item.Children.Cast<SimpleItem>())
                {
                    SubscribeToItemTree(child);
                }
            }
        }

        private void UnsubscribeFromItemTree(SimpleItem item)
        {
            if (item == null) return;

            try
            {
                if (_subscribedItems.Contains(item))
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                    _subscribedItems.Remove(item);
                }
            }
            catch { }

            if (item.Children != null)
            {
                try
                {
                    item.Children.ListChanged -= Children_ListChanged;
                }
                catch { }

                foreach (var child in item.Children.Cast<SimpleItem>())
                {
                    UnsubscribeFromItemTree(child);
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is SimpleItem item)
            {
                if (e.PropertyName == nameof(SimpleItem.IsExpanded))
                {
                    // Reflect changes to _expandedState
                    if (item.Children != null && item.Children.Count > 0)
                    {
                        _expandedState[item] = item.IsExpanded;
                        try
                        {
                            OnItemExpansionChanged(item, item.IsExpanded);
                        }
                        catch { }
                        RefreshHitAreas();
                        Invalidate();
                    }
                }
            }
        }

        private void Children_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            // When children list changes (added/removed), re-subscribe all to keep up-to-date
            SubscribeToItems();
            InitializeExpandedState();
            RefreshHitAreas();
            Invalidate();
        }
        #endregion
    }
}
