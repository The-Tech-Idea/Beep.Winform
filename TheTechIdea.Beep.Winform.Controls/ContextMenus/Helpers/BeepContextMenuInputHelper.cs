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
            System.Diagnostics.Debug.WriteLine($"[HitTest] Input location: {location}");
            
            if (_owner.MenuItems == null || _owner.MenuItems.Count == 0)
            {
                return null;
            }
            
            // Calculate beepInsets (border + padding + shadow from FormStyle)
            var effectiveStyle = _owner.ControlStyle;
            float styleBorder = TheTechIdea.Beep.Winform.Controls.Styling.BeepStyling.GetBorderThickness(effectiveStyle);
            int stylePadding = TheTechIdea.Beep.Winform.Controls.Styling.BeepStyling.GetPadding(effectiveStyle);
            int styleShadow = TheTechIdea.Beep.Winform.Controls.Styling.Shadows.StyleShadows.HasShadow(effectiveStyle) 
                ? System.Math.Max(2, TheTechIdea.Beep.Winform.Controls.Styling.Shadows.StyleShadows.GetShadowBlur(effectiveStyle) / 2) 
                : 0;
            int beepInsets = (int)System.Math.Ceiling(styleBorder) + stylePadding + styleShadow;
            
            System.Diagnostics.Debug.WriteLine($"[HitTest] BeepInsets: {beepInsets}, NeedsScrolling: {_owner.NeedsScrolling}, ScrollOffset: {_owner.ScrollOffset}");
            
            // Adjust location for scroll offset
            Point adjustedLocation = location;
            if (_owner.NeedsScrolling)
            {
                adjustedLocation = new Point(location.X, location.Y + _owner.ScrollOffset);
            }

            // Content area starting position (must match DrawMenuItemsSimple)
            int contentStartX = beepInsets + 4;
            int contentStartY = beepInsets + 4;
            int searchAreaHeight = _owner.ShowSearchBox ? _owner.SearchBoxHeight : 0;
            if (searchAreaHeight > 0)
            {
                // If the click is in the search area, do not treat it as an item
                var searchRect = new Rectangle(contentStartX, contentStartY, _owner.Width - (beepInsets * 2) - 8 - (_owner.NeedsScrolling ? 17 : 0), searchAreaHeight + 8);
                if (searchRect.Contains(adjustedLocation)) return null;
                contentStartY += searchAreaHeight + 8;
            }
            int contentWidth = _owner.Width - (beepInsets * 2) - 8;

            int yOffset = 0;

            for (int i = 0; i < _owner.MenuItems.Count; i++)
            {
                var item = _owner.MenuItems[i];
                
                if (IsSeparator(item))
                {
                    yOffset += 8; // Separator height
                    continue;
                }
                
                int itemHeight = _owner.PreferredItemHeight;
                var itemRect = new Rectangle(
                    contentStartX,
                    contentStartY + yOffset,
                    contentWidth,
                    itemHeight
                );
                
                System.Diagnostics.Debug.WriteLine($"[HitTest] Item {i} ({item.DisplayField}): Rect={itemRect}, AdjustedLoc={adjustedLocation}");
                
                if (itemRect.Contains(adjustedLocation))
                {
                    System.Diagnostics.Debug.WriteLine($"[HitTest] HIT! Returning item: {item.DisplayField}");
                    return item;
                }
                
                yOffset += itemHeight;
            }
            
            System.Diagnostics.Debug.WriteLine($"[HitTest] No hit found, returning null");
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
