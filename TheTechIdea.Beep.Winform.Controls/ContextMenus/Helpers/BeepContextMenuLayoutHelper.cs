using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Helpers
{
    /// <summary>
    /// Handles layout calculations for BeepContextMenu with caching support
    /// </summary>
    public class BeepContextMenuLayoutHelper
    {
        private readonly BeepContextMenu _owner;
        
        // Layout cache
        private Dictionary<SimpleItem, Rectangle> _itemRectCache = new Dictionary<SimpleItem, Rectangle>();
        private bool _layoutCacheValid = false;
        private int _cachedItemCount = -1;
        private int _cachedWidth = -1;
        private int _cachedMenuItemHeight = -1;
        private int _cachedImageSize = -1;
        private bool _cachedShowCheckBox = false;
        private bool _cachedShowImage = false;
        private bool _cachedShowShortcuts = false;
        private bool _cachedShowSearchBox = false;
        private int _cachedSearchBoxHeight = -1;
        
        public BeepContextMenuLayoutHelper(BeepContextMenu owner)
        {
            _owner = owner;
        }
        
        /// <summary>
        /// Invalidates the layout cache, forcing recalculation on next access
        /// </summary>
        public void InvalidateCache()
        {
            _layoutCacheValid = false;
            _itemRectCache.Clear();
        }
        
        private bool IsCacheValid()
        {
            return _layoutCacheValid &&
                _cachedItemCount == (_owner.MenuItems?.Count ?? 0) &&
                _cachedWidth == _owner.Width &&
                _cachedMenuItemHeight == _owner.PreferredItemHeight &&
                _cachedImageSize == _owner.ImageSize &&
                _cachedShowCheckBox == _owner.ShowCheckBox &&
                _cachedShowImage == _owner.ShowImage &&
                _cachedShowShortcuts == _owner.ShowShortcuts &&
                _cachedShowSearchBox == _owner.ShowSearchBox &&
                _cachedSearchBoxHeight == _owner.SearchBoxHeight;
        }
        
        private void UpdateCacheTracking()
        {
            _cachedItemCount = _owner.MenuItems?.Count ?? 0;
            _cachedWidth = _owner.Width;
            _cachedMenuItemHeight = _owner.PreferredItemHeight;
            _cachedImageSize = _owner.ImageSize;
            _cachedShowCheckBox = _owner.ShowCheckBox;
            _cachedShowImage = _owner.ShowImage;
            _cachedShowShortcuts = _owner.ShowShortcuts;
            _cachedShowSearchBox = _owner.ShowSearchBox;
            _cachedSearchBoxHeight = _owner.SearchBoxHeight;
            _layoutCacheValid = true;
        }
        
        /// <summary>
        /// Gets the rectangle for a menu item (with caching)
        /// </summary>
        public Rectangle GetItemRect(SimpleItem item)
        {
            if (item == null || _owner.MenuItems == null)
            {
                return Rectangle.Empty;
            }
            
            // Check cache first
            if (IsCacheValid() && _itemRectCache.TryGetValue(item, out Rectangle cachedRect))
            {
                return cachedRect;
            }
            
            // Recalculate if cache invalid
            if (!IsCacheValid())
            {
                InvalidateCache();
            }
            
            int internalPadding = _owner.GetInternalPadding();
            int y = internalPadding;
            // Account for optional search area
            if (_owner.ShowSearchBox)
            {
                y += _owner.SearchBoxHeight + _owner.GetSearchSpacing();
            }
            
            foreach (var menuItem in _owner.MenuItems)
            {
                if (menuItem == item)
                {
                    int height = _owner.GetMenuItemLayoutHeight(item);
                    var rect = new Rectangle(0, y, _owner.Width, height);
                    _itemRectCache[item] = rect;
                    UpdateCacheTracking();
                    return rect;
                }
                
                y += _owner.GetMenuItemLayoutHeight(menuItem);
            }
            
            return Rectangle.Empty;
        }
        
        /// <summary>
        /// Gets the checkbox rectangle for a menu item
        /// </summary>
        public Rectangle GetCheckBoxRect(SimpleItem item)
        {
            if (!_owner.ShowCheckBox || item == null)
            {
                return Rectangle.Empty;
            }
            
            var itemRect = GetItemRect(item);
            if (itemRect.IsEmpty)
            {
                return Rectangle.Empty;
            }
            
            int size = _owner.ScaleLogical(16);
            int x = _owner.ScaleLogical(8);
            int y = itemRect.Top + (itemRect.Height - size) / 2;
            
            return new Rectangle(x, y, size, size);
        }
    }
}
