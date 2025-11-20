using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Helpers
{
    /// <summary>
    /// Handles layout calculations for BeepContextMenu
    /// </summary>
    public class BeepContextMenuLayoutHelper
    {
        private readonly BeepContextMenu _owner;
        
        public BeepContextMenuLayoutHelper(BeepContextMenu owner)
        {
            _owner = owner;
        }
        
        /// <summary>
        /// Gets the rectangle for a menu item
        /// </summary>
        public Rectangle GetItemRect(SimpleItem item)
        {
            if (item == null || _owner.MenuItems == null)
            {
                return Rectangle.Empty;
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
                    return new Rectangle(0, y, _owner.Width, height);
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
        /// Gets the text rectangle for a menu item
        /// </summary>
        public Rectangle GetTextRect(SimpleItem item)
        {
            if (item == null)
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
            
            if (_owner.ShowImage)
            {
                x += _owner.ImageSize + 4; // After icon
            }
            
            // Calculate width (leave space for shortcut or arrow)
            int rightMargin =8;
            
            if (_owner.ShowShortcuts && !string.IsNullOrEmpty(item.KeyCombination))
            {
                // Measure shortcut width
                using (var g = _owner.CreateGraphics())
                {
                    var shortcutSize = TextRenderer.MeasureText(g, item.KeyCombination, _owner.ShortcutFont);
                    rightMargin += shortcutSize.Width + 16;
                }
            }
            else if (item.Children != null && item.Children.Count > 0)
            {
                rightMargin += 20; // Submenu arrow
            }
            
            int width = _owner.Width - x - rightMargin;
            
            return new Rectangle(x, itemRect.Top, width, itemRect.Height);
        }
        
        /// <summary>
        /// Gets the shortcut text rectangle for a menu item
        /// </summary>
        public Rectangle GetShortcutRect(SimpleItem item)
        {
            if (!_owner.ShowShortcuts || item == null || string.IsNullOrEmpty(item.KeyCombination))
            {
                return Rectangle.Empty;
            }
            
            var itemRect = GetItemRect(item);
            if (itemRect.IsEmpty)
            {
                return Rectangle.Empty;
            }
            
            // Measure shortcut width
            using (var g = _owner.CreateGraphics())
            {
                var shortcutSize = TextRenderer.MeasureText(g, item.KeyCombination, _owner.ShortcutFont);
                int width = shortcutSize.Width;
                int x = _owner.Width - width - 24; // Leave space for arrow if submenu
                
                return new Rectangle(x, itemRect.Top, width, itemRect.Height);
            }
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
