using System.Collections.Generic;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Menus.Helpers
{
    /// <summary>
    /// Context and data container for menu bar painters
    /// </summary>
    public sealed class MenuBarContext
    {
        #region Layout Rectangles
        /// <summary>
        /// The main drawing rectangle for the menu bar
        /// </summary>
        public Rectangle DrawingRect;

        /// <summary>
        /// The content area rectangle (excluding padding)
        /// </summary>
        public Rectangle ContentRect;

        /// <summary>
        /// Rectangle for menu bar title/logo area
        /// </summary>
        public Rectangle TitleRect;

        /// <summary>
        /// Rectangle for the main menu items area
        /// </summary>
        public Rectangle MenuItemsRect;

        /// <summary>
        /// Rectangle for additional controls/actions area
        /// </summary>
        public Rectangle ActionsRect;
        #endregion

        #region Display Properties
        /// <summary>
        /// Whether to show icons in menu items
        /// </summary>
        public bool ShowIcons = true;

        /// <summary>
        /// Whether to show text labels in menu items
        /// </summary>
        public bool ShowText = true;

        /// <summary>
        /// Whether the menu bar is laid out horizontally
        /// </summary>
        public bool IsHorizontal = true;

        /// <summary>
        /// Whether to show dropdown indicators for items with children
        /// </summary>
        public bool ShowDropdownIndicators = true;

        /// <summary>
        /// Whether to show separators between menu items
        /// </summary>
        public bool ShowSeparators = false;

        /// <summary>
        /// Corner radius for rounded elements
        /// </summary>
        public int CornerRadius = 4;
        #endregion

        #region Style Properties
        /// <summary>
        /// Primary accent color for the menu bar
        /// </summary>
        public Color AccentColor = Color.FromArgb(33, 150, 243);

        /// <summary>
        /// Background color for menu items
        /// </summary>
        public Color ItemBackColor = Color.White;

        /// <summary>
        /// Foreground color for menu items
        /// </summary>
        public Color ItemForeColor = Color.Black;

        /// <summary>
        /// Border color for menu items
        /// </summary>
        public Color ItemBorderColor = Color.FromArgb(200, 200, 200);

        /// <summary>
        /// Background color for hovered menu items
        /// </summary>
        public Color ItemHoverBackColor = Color.FromArgb(245, 245, 245);

        /// <summary>
        /// Foreground color for hovered menu items
        /// </summary>
        public Color ItemHoverForeColor = Color.Black;

        /// <summary>
        /// Background color for selected menu items
        /// </summary>
        public Color ItemSelectedBackColor = Color.FromArgb(33, 150, 243);

        /// <summary>
        /// Foreground color for selected menu items
        /// </summary>
        public Color ItemSelectedForeColor = Color.White;

        /// <summary>
        /// Background color for disabled menu items
        /// </summary>
        public Color ItemDisabledBackColor = Color.FromArgb(250, 250, 250);

        /// <summary>
        /// Foreground color for disabled menu items
        /// </summary>
        public Color ItemDisabledForeColor = Color.FromArgb(160, 160, 160);
        #endregion

        #region Data Properties
        /// <summary>
        /// List of menu items to display
        /// </summary>
        public List<SimpleItem> MenuItems = new List<SimpleItem>();

        /// <summary>
        /// Currently selected menu item index
        /// </summary>
        public int SelectedIndex = -1;

        /// <summary>
        /// Currently hovered menu item name/id
        /// </summary>
        public string HoveredItemName = string.Empty;

        /// <summary>
        /// Font for menu item text
        /// </summary>
        public Font TextFont;

        /// <summary>
        /// Size for menu item icons
        /// </summary>
        public Size IconSize = new Size(16, 16);

        /// <summary>
        /// Height of menu items
        /// </summary>
        public int ItemHeight = 32;

        /// <summary>
        /// Width of menu items (for fixed-width layout)
        /// </summary>
        public int ItemWidth = 80;

        /// <summary>
        /// Spacing between menu items
        /// </summary>
        public int ItemSpacing = 4;

        /// <summary>
        /// Padding inside menu items
        /// </summary>
        public int ItemPadding = 8;
        #endregion

        #region Interaction Properties
        /// <summary>
        /// Whether the menu bar supports interaction
        /// </summary>
        public bool IsInteractive = true;

        /// <summary>
        /// Whether menu items can be focused
        /// </summary>
        public bool CanBeFocused = true;

        /// <summary>
        /// Whether to enable keyboard navigation
        /// </summary>
        public bool EnableKeyboardNavigation = true;

        /// <summary>
        /// Menu item rectangles for hit testing
        /// </summary>
        public List<(Rectangle Rect, int Index, SimpleItem Item)> ItemRects = new List<(Rectangle, int, SimpleItem)>();
        #endregion

        #region Custom Data
        /// <summary>
        /// Custom data dictionary for painter-specific properties
        /// </summary>
        public Dictionary<string, object> CustomData = new Dictionary<string, object>();

        /// <summary>
        /// Title text for menu bar
        /// </summary>
        public string Title = string.Empty;

        /// <summary>
        /// Path to title/logo icon
        /// </summary>
        public string TitleIconPath = string.Empty;
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets menu item at the specified index
        /// </summary>
        public SimpleItem GetMenuItem(int index)
        {
            return index >= 0 && index < MenuItems.Count ? MenuItems[index] : null;
        }

        /// <summary>
        /// Gets currently selected menu item
        /// </summary>
        public SimpleItem GetSelectedMenuItem()
        {
            return GetMenuItem(SelectedIndex);
        }

        /// <summary>
        /// Checks if the specified item has children (submenu)
        /// </summary>
        public bool HasChildren(SimpleItem item)
        {
            return item?.Children?.Count > 0;
        }

        /// <summary>
        /// Checks if the specified item is enabled
        /// </summary>
        public bool IsItemEnabled(SimpleItem item)
        {
            return item?.IsEnabled ?? false;
        }

        /// <summary>
        /// Gets the display text for an item
        /// </summary>
        public string GetItemText(SimpleItem item)
        {
            return item?.Text ?? string.Empty;
        }

        /// <summary>
        /// Gets the icon path for an item
        /// </summary>
        public string GetItemIconPath(SimpleItem item)
        {
            return item?.ImagePath ?? string.Empty;
        }
        #endregion
    }
}