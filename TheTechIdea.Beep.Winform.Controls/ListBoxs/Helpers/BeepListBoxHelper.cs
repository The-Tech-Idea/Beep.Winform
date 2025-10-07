using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs.Helpers
{
    /// <summary>
    /// Helper class for BeepListBox - handles layout calculations and common logic
    /// </summary>
    internal class BeepListBoxHelper
    {
        private readonly BeepListBox _owner;
        
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
            
            var items = _owner.ListItems
                .Where(p => p.ItemType == MenuItemType.Main)
                .ToList();
            
            // Apply search filter if needed
            if (_owner.ShowSearch && !string.IsNullOrWhiteSpace(_owner.SearchText))
            {
                string searchLower = _owner.SearchText.ToLower();
                items = items.Where(i => i.Text?.ToLower().Contains(searchLower) == true).ToList();
            }
            
            return items;
        }
        
        /// <summary>
        /// Measure text size without creating Graphics object
        /// </summary>
        public Size MeasureText(string text, Font font)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;
            
            return TextRenderer.MeasureText(text, font);
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
