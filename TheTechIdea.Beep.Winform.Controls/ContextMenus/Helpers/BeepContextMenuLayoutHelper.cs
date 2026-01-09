using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Helpers;

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
        private Dictionary<SimpleItem, Rectangle> _iconRectCache = new Dictionary<SimpleItem, Rectangle>();
        private Dictionary<SimpleItem, Rectangle> _textRectCache = new Dictionary<SimpleItem, Rectangle>();
        private Dictionary<SimpleItem, Rectangle> _shortcutRectCache = new Dictionary<SimpleItem, Rectangle>();
        private Dictionary<SimpleItem, Size> _shortcutSizeCache = new Dictionary<SimpleItem, Size>();
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
            _iconRectCache.Clear();
            _textRectCache.Clear();
            _shortcutRectCache.Clear();
            _shortcutSizeCache.Clear();
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
            
            int y = 4; // Top padding
            // Account for optional search area
            if (_owner.ShowSearchBox)
            {
                y += _owner.SearchBoxHeight + 8; // same spacing as DrawMenuItemsSimple
            }
            
            foreach (var menuItem in _owner.MenuItems)
            {
                if (menuItem == item)
                {
                    int height = IsSeparator(item) ? 8 : _owner.PreferredItemHeight;
                    var rect = new Rectangle(0, y, _owner.Width, height);
                    _itemRectCache[item] = rect;
                    UpdateCacheTracking();
                    return rect;
                }
                
                if (IsSeparator(menuItem))
                {
                    y += 8;
                }
                else
                {
                    y += _owner.PreferredItemHeight;
                }
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
            
            int size = 16;
            int x = 8;
            int y = itemRect.Top + (itemRect.Height - size) / 2;
            
            return new Rectangle(x, y, size, size);
        }
        
        /// <summary>
        /// Gets the icon rectangle for a menu item
        /// </summary>
        public Rectangle GetIconRect(SimpleItem item)
        {
            if (!_owner.ShowImage || item == null)
            {
                return Rectangle.Empty;
            }
            
            var itemRect = GetItemRect(item);
            if (itemRect.IsEmpty)
            {
                return Rectangle.Empty;
            }
            
            int x =8;
            if (_owner.ShowCheckBox)
            {
                x += 20; // After checkbox
            }
            
            int size = _owner.ImageSize;
            int y = itemRect.Top + (itemRect.Height - size) / 2;
            
            return new Rectangle(x, y, size, size);
        }
        
        /// <summary>
        /// Gets the text rectangle for a menu item (with caching)
        /// </summary>
        public Rectangle GetTextRect(SimpleItem item)
        {
            if (item == null)
            {
                return Rectangle.Empty;
            }
            
            // Check cache first
            if (IsCacheValid() && _textRectCache.TryGetValue(item, out Rectangle cachedRect))
            {
                return cachedRect;
            }
            
            var itemRect = GetItemRect(item);
            if (itemRect.IsEmpty)
            {
                return Rectangle.Empty;
            }
            
            int x =8;
            
            if (_owner.ShowCheckBox)
            {
                x += 20; // After checkbox
            }
            
            if (_owner.ShowImage)
            {
                x += _owner.ImageSize + 4; // After icon
            }
            
            // Calculate width (leave space for shortcut or arrow)
            int rightMargin =8;
            
            if (_owner.ShowShortcuts && !string.IsNullOrEmpty(item.KeyCombination))
            {
                // Get shortcut size (cached)
                Size shortcutSize = GetShortcutSize(item);
                rightMargin += shortcutSize.Width + 16;
            }
            else if (item.Children != null && item.Children.Count > 0)
            {
                rightMargin += 20; // Submenu arrow
            }
            
            int width = _owner.Width - x - rightMargin;
            
            var rect = new Rectangle(x, itemRect.Top, width, itemRect.Height);
            _textRectCache[item] = rect;
            UpdateCacheTracking();
            return rect;
        }
        
        /// <summary>
        /// Gets the shortcut text rectangle for a menu item (with caching)
        /// </summary>
        public Rectangle GetShortcutRect(SimpleItem item)
        {
            if (!_owner.ShowShortcuts || item == null || string.IsNullOrEmpty(item.KeyCombination))
            {
                return Rectangle.Empty;
            }
            
            // Check cache first
            if (IsCacheValid() && _shortcutRectCache.TryGetValue(item, out Rectangle cachedRect))
            {
                return cachedRect;
            }
            
            var itemRect = GetItemRect(item);
            if (itemRect.IsEmpty)
            {
                return Rectangle.Empty;
            }
            
            // Get shortcut size (cached)
            Size shortcutSize = GetShortcutSize(item);
            int width = shortcutSize.Width;
            int x = _owner.Width - width - 24; // Leave space for arrow if submenu
            
            var rect = new Rectangle(x, itemRect.Top, width, itemRect.Height);
            _shortcutRectCache[item] = rect;
            UpdateCacheTracking();
            return rect;
        }
        
        /// <summary>
        /// Gets the submenu arrow rectangle for a menu item
        /// </summary>
        public Rectangle GetArrowRect(SimpleItem item)
        {
            if (item == null || item.Children == null || item.Children.Count == 0)
            {
                return Rectangle.Empty;
            }
            
            var itemRect = GetItemRect(item);
            if (itemRect.IsEmpty)
            {
                return Rectangle.Empty;
            }
            
            int size = 16;
            int x = _owner.Width - size -8;
            int y = itemRect.Top + (itemRect.Height - size) / 2;
            
            return new Rectangle(x, y, size, size);
        }
        
        /// <summary>
        /// Gets the size of a shortcut text (with caching)
        /// </summary>
        private Size GetShortcutSize(SimpleItem item)
        {
            if (item == null || string.IsNullOrEmpty(item.KeyCombination))
            {
                return Size.Empty;
            }
            
            // Check cache first
            if (_shortcutSizeCache.TryGetValue(item, out Size cachedSize))
            {
                return cachedSize;
            }
            
            // Measure shortcut text using TextUtils
            SizeF shortcutSizeF = TextUtils.MeasureText(item.KeyCombination, _owner.ShortcutFont, int.MaxValue);
            var shortcutSize = new Size((int)shortcutSizeF.Width, (int)shortcutSizeF.Height);
            
            // Cache the result
            _shortcutSizeCache[item] = shortcutSize;
            
            return shortcutSize;
        }
        
        /// <summary>
        /// Gets the separator line rectangle
        /// </summary>
        public Rectangle GetSeparatorRect(SimpleItem item)
        {
            if (!IsSeparator(item))
            {
                return Rectangle.Empty;
            }
            
            var itemRect = GetItemRect(item);
            if (itemRect.IsEmpty)
            {
                return Rectangle.Empty;
            }
            
            int x =8;
            int width = _owner.Width - 16;
            int y = itemRect.Top + itemRect.Height / 2;
            
            return new Rectangle(x, y, width, 1);
        }
        
        private bool IsSeparator(SimpleItem item)
        {
            return item != null && (item.DisplayField == "-" || item.Tag?.ToString() == "separator");
        }
    }
}
