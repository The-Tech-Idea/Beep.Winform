using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Helpers
{
    /// <summary>
    /// Handles user input and hit-testing for BeepContextMenu
    /// </summary>
    public class BeepContextMenuInputHelper
    {
        private readonly BeepContextMenu _owner;
        
        public BeepContextMenuInputHelper(BeepContextMenu owner)
        {
            _owner = owner;
        }
        
        /// <summary>
        /// Performs hit-testing to find the menu item at the specified point
        /// </summary>
        public SimpleItem HitTest(Point location)
        {
            if (_owner.MenuItems == null || _owner.MenuItems.Count == 0)
            {
                return null;
            }
            
            int y = _owner.ScaleDpi(4); // Top padding
            
            foreach (var item in _owner.MenuItems)
            {
                if (IsSeparator(item))
                {
                    y += _owner.ScaleDpi(8); // Separator height
                    continue;
                }
                
                int itemHeight = _owner.PreferredItemHeight;
                var itemRect = new Rectangle(0, y, _owner.Width, itemHeight);
                
                if (itemRect.Contains(location))
                {
                    return item;
                }
                
                y += itemHeight;
            }
            
            return null;
        }
        
        /// <summary>
        /// Gets the index of the menu item at the specified point
        /// </summary>
        public int GetItemIndex(Point location)
        {
            var item = HitTest(location);
            return item != null ? _owner.MenuItems.IndexOf(item) : -1;
        }
        
        /// <summary>
        /// Checks if a point is within the checkbox area of an item
        /// </summary>
        public bool IsInCheckBox(Point location, SimpleItem item)
        {
            if (!_owner.ShowCheckBox || item == null)
            {
                return false;
            }
            
            var checkRect = _owner.LayoutHelper.GetCheckBoxRect(item);
            return checkRect.Contains(location);
        }
        
        /// <summary>
        /// Checks if a point is within the icon area of an item
        /// </summary>
        public bool IsInIcon(Point location, SimpleItem item)
        {
            if (!_owner.ShowImage || item == null)
            {
                return false;
            }
            
            var iconRect = _owner.LayoutHelper.GetIconRect(item);
            return iconRect.Contains(location);
        }
        
        /// <summary>
        /// Checks if a point is within the text area of an item
        /// </summary>
        public bool IsInText(Point location, SimpleItem item)
        {
            if (item == null)
            {
                return false;
            }
            
            var textRect = _owner.LayoutHelper.GetTextRect(item);
            return textRect.Contains(location);
        }
        
        private bool IsSeparator(SimpleItem item)
        {
            return item != null && (item.DisplayField == "-" || item.Tag?.ToString() == "separator");
        }
    }
}
