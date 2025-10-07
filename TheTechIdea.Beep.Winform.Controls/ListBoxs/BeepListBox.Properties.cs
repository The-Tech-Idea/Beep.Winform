using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Properties for BeepListBox
    /// </summary>
    public partial class BeepListBox
    {
        #region ListBoxType Property
        
        /// <summary>
        /// Gets or sets the visual style type for the list box
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style type of the list box")]
        [DefaultValue(ListBoxType.Standard)]
        public ListBoxType ListBoxType
        {
            get => _listBoxType;
            set
            {
                if (_listBoxType != value)
                {
                    _listBoxType = value;
                    
                    // Recreate painter for new type
                    _listBoxPainter = CreatePainter(_listBoxType);
                    _listBoxPainter?.Initialize(this, _currentTheme);
                    
                    _needsLayoutUpdate = true;
                    RequestDelayedInvalidate();
                }
            }
        }
        
        #endregion
        
        #region List Items and Selection
        
        /// <summary>
        /// Gets or sets the collection of items in the list box
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The collection of items displayed in the list box")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [MergableProperty(false)]
        public BindingList<SimpleItem> ListItems
        {
            get => _listItems;
            set
            {
                if (_listItems != null)
                {
                    _listItems.ListChanged -= ListItems_ListChanged;
                }
                
                _listItems = value ?? new BindingList<SimpleItem>();
                _listItems.ListChanged += ListItems_ListChanged;
                
                _needsLayoutUpdate = true;
                RequestDelayedInvalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets the currently selected item
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    
                    // Update index
                    if (_selectedItem != null && _listItems != null)
                    {
                        _selectedIndex = _listItems.IndexOf(_selectedItem);
                    }
                    else
                    {
                        _selectedIndex = -1;
                    }
                    
                    OnSelectedItemChanged(_selectedItem);
                    RequestDelayedInvalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets the list of selected items (when checkboxes are enabled)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<SimpleItem> SelectedItems
        {
            get
            {
                var selectedItems = new List<SimpleItem>();
                foreach (var kvp in _itemCheckBoxes)
                {
                    if (kvp.Value.State == CheckBoxState.Checked)
                    {
                        selectedItems.Add(kvp.Key);
                    }
                }
                return selectedItems;
            }
        }
        
        /// <summary>
        /// Gets or sets the index of the currently selected item
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < _listItems.Count)
                {
                    _selectedIndex = value;
                    _selectedItem = _listItems[_selectedIndex];
                    OnSelectedItemChanged(_selectedItem);
                    RequestDelayedInvalidate();
                }
            }
        }
        
        #endregion
        
        #region Search Properties
        
        /// <summary>
        /// Gets or sets whether the search box is displayed
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Indicates whether the search box is displayed")]
        [DefaultValue(false)]
        public bool ShowSearch
        {
            get => _showSearch;
            set
            {
                if (_showSearch != value)
                {
                    _showSearch = value;
                    _needsLayoutUpdate = true;
                    RequestDelayedInvalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the search text for filtering items
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The search text for filtering items")]
        [DefaultValue("")]
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value ?? string.Empty;
                    OnSearchTextChanged();
                    RequestDelayedInvalidate();
                }
            }
        }
        
        #endregion
        
        #region Visual Options
        
        /// <summary>
        /// Gets or sets whether checkboxes are displayed for items
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Indicates whether checkboxes are displayed for items")]
        [DefaultValue(false)]
        public bool ShowCheckBox
        {
            get => _showCheckBox;
            set
            {
                if (_showCheckBox != value)
                {
                    _showCheckBox = value;
                    RequestDelayedInvalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether images are displayed for items
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Indicates whether images are displayed for items")]
        [DefaultValue(true)]
        public bool ShowImage
        {
            get => _showImage;
            set
            {
                if (_showImage != value)
                {
                    _showImage = value;
                    RequestDelayedInvalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether a highlight box is shown when hovering over items
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Indicates whether a highlight box is shown when hovering over items")]
        [DefaultValue(true)]
        public bool ShowHilightBox
        {
            get => _showHilightBox;
            set
            {
                if (_showHilightBox != value)
                {
                    _showHilightBox = value;
                    RequestDelayedInvalidate();
                }
            }
        }
        
        #endregion
        
        #region Layout Properties
        
        /// <summary>
        /// Gets or sets the height of each menu item
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The height of each menu item")]
        [DefaultValue(32)]
        public int MenuItemHeight
        {
            get => _menuItemHeight;
            set
            {
                if (_menuItemHeight != value && value > 0)
                {
                    _menuItemHeight = value;
                    _needsLayoutUpdate = true;
                    RequestDelayedInvalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the size of the item image
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The size of the item image")]
        [DefaultValue(24)]
        public int ImageSize
        {
            get => _imageSize;
            set
            {
                if (_imageSize != value && value > 0)
                {
                    if (value >= _menuItemHeight)
                    {
                        _imageSize = _menuItemHeight - 2;
                    }
                    else
                    {
                        _imageSize = value;
                    }
                    RequestDelayedInvalidate();
                }
            }
        }
        
        #endregion
        
        #region Font Property
        
        /// <summary>
        /// Gets or sets the font used for text in the list box
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The font used for text in the list box")]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                if (_textFont != value)
                {
                    _textFont = value;
                    RequestDelayedInvalidate();
                }
            }
        }
        
        #endregion
        
        #region Legacy Compatibility Properties
        
        /// <summary>
        /// Gets or sets whether to apply theme coloring to images
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Indicates whether to apply theme coloring to images")]
        [DefaultValue(false)]
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                if (_applyThemeOnImage != value)
                {
                    _applyThemeOnImage = value;
                    ApplyTheme();
                    RequestDelayedInvalidate();
                }
            }
        }
        
        private bool _applyThemeOnImage = false;
        
        /// <summary>
        /// Gets the current item button (legacy property for compatibility)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepButton CurrenItemButton { get; private set; }
        
        /// <summary>
        /// Gets or sets whether the control is collapsed to title only
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Indicates whether the control is collapsed to show only the title")]
        [DefaultValue(false)]
        public bool Collapsed { get; set; } = false;
        
        /// <summary>
        /// Gets or sets whether items can have children (legacy property)
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Indicates whether items can have children")]
        [DefaultValue(true)]
        public bool IsItemChilds { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the height of the search area
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The height of the search area when search is enabled")]
        [DefaultValue(36)]
        public int SearchAreaHeight { get; set; } = 36;
        
        /// <summary>
        /// Gets or sets the placeholder text for the search box
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The placeholder text displayed in the search box")]
        [DefaultValue("Search...")]
        public string SearchPlaceholderText { get; set; } = "Search...";
        
        #endregion
        
        #region Custom Painter Property
        
        /// <summary>
        /// Gets or sets a custom item renderer delegate for custom drawing
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action<Graphics, Rectangle, SimpleItem, bool, bool> CustomItemRenderer
        {
            get => _customItemRenderer;
            set
            {
                _customItemRenderer = value;
                
                // If we're in Custom type and painter exists, update it
                if (_listBoxType == ListBoxType.Custom && _listBoxPainter is ListBoxs.Painters.CustomListPainter customPainter)
                {
                    customPainter.CustomItemRenderer = _customItemRenderer;
                    RequestDelayedInvalidate();
                }
            }
        }
        
        #endregion
    }
}
