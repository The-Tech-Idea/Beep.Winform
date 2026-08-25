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
            
            var effectiveStyle = _owner.ControlStyle;
            float styleBorder = TheTechIdea.Beep.Winform.Controls.Styling.BeepStyling.GetBorderThickness(effectiveStyle);
            int stylePadding = TheTechIdea.Beep.Winform.Controls.Styling.BeepStyling.GetPadding(effectiveStyle);
            int styleShadow = TheTechIdea.Beep.Winform.Controls.Styling.Shadows.StyleShadows.HasShadow(effectiveStyle) 
                ? System.Math.Max(2, TheTechIdea.Beep.Winform.Controls.Styling.Shadows.StyleShadows.GetShadowBlur(effectiveStyle) / 2) 
                : 0;
            int beepInsets = (int)System.Math.Ceiling(styleBorder) + stylePadding + styleShadow;
            
            int internalPadding = _owner.GetInternalPadding();
            int searchSpacing = _owner.GetSearchSpacing();
            int contentStartX = beepInsets + internalPadding;
            int contentStartY = beepInsets + internalPadding;
            int searchAreaHeight = _owner.ShowSearchBox ? _owner.SearchBoxHeight : 0;
            if (searchAreaHeight > 0)
            {
                contentStartY += searchAreaHeight + searchSpacing;
            }
            
            int contentWidth = _owner.Width - (beepInsets * 2) - (internalPadding * 2) - (_owner.NeedsScrolling ? 17 : 0);
            
            int adjustedY = location.Y - contentStartY + (_owner.NeedsScrolling ? _owner.ScrollOffset : 0);
            
            if (location.X < contentStartX || location.X > contentStartX + contentWidth)
                return null;
            
            if (_owner.ShowSearchBox && location.Y >= beepInsets + internalPadding && location.Y < contentStartY)
                return null;
            
            int yOffset = 0;

            for (int i = 0; i < _owner.MenuItems.Count; i++)
            {
                var item = _owner.MenuItems[i];
                
                if (IsSeparator(item))
                {
                    yOffset += _owner.GetSeparatorHeight();
                    continue;
                }
                
                int itemHeight = _owner.GetMenuItemLayoutHeight(item);
                
                if (adjustedY >= yOffset && adjustedY < yOffset + itemHeight)
                {
                    return item.IsEnabled ? item : null;
                }
                
                yOffset += itemHeight;
            }
            
            return null;
        }
        
        private bool IsSeparator(SimpleItem item)
        {
            return item != null && (item.DisplayField == "-" || item.Tag?.ToString() == "separator");
        }
    }
}
