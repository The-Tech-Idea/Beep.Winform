using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.SideBar
{
    public partial class BeepSideBar
    {
        #region Accordion Fields
        private Dictionary<SimpleItem, bool> _expandedState = new Dictionary<SimpleItem, bool>();
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
        /// </summary>
        private void InitializeExpandedState()
        {
            if (_items == null) return;

            foreach (var item in _items)
            {
                if (item.Children != null && item.Children.Count > 0)
                {
                    if (!_expandedState.ContainsKey(item))
                    {
                        _expandedState[item] = false; // Default to collapsed
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
                if (child.Children != null && child.Children.Count > 0)
                {
                    if (!_expandedState.ContainsKey(child))
                    {
                        _expandedState[child] = false;
                    }

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

            if (_expandedState.ContainsKey(item))
            {
                _expandedState[item] = !_expandedState[item];
            }
            else
            {
                _expandedState[item] = true;
            }

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

            _expandedState[item] = true;
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

            _expandedState[item] = false;
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
            }

            RefreshHitAreas();
            Invalidate();
        }
        #endregion
    }
}
