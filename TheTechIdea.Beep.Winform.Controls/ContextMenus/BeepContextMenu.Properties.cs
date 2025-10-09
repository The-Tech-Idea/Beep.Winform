using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
#pragma warning disable IL2026 // Suppress trimmer warnings for BindingList<T> used in WinForms data binding scenarios
    public partial class BeepContextMenu
    {
        #region Public Properties
        
        /// <summary>
        /// Gets or sets the visual style of the context menu
        /// </summary>
        [Category("Beep")]
        [Description("The visual style of the context menu")]
        [Browsable(true)]
        public ContextMenuType ContextMenuType
        {
            get => _contextMenuType;
            set
            {
                if (_contextMenuType != value)
                {
                    _contextMenuType = value;
                    SetPainter(value);
                    RecalculateSize();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the theme for styling
        /// </summary>
        [Category("Beep")]
        [Description("The theme for styling the context menu")]
        [Browsable(true)]
        public ContextMenuType MenuStyle
        {
            get => menustyle;
            set
            {
                if (menustyle != value)
                {
                    menustyle = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Current theme name from BeepThemesManager (aligns with BaseControl.Theme)
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The current theme name applied from BeepThemesManager.")]
        public string Theme
        {
            get => _themeName;
            set
            {
                _themeName = value;
                _currentTheme = ThemeManagement.BeepThemesManager.GetTheme(value)
                               ?? ThemeManagement.BeepThemesManager.GetDefaultTheme();
                // Adopt fonts on explicit theme set if enabled
                try { ApplyThemeFontsSafely(); } catch { }
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets the menu items
        /// </summary>
        [Category("Beep")]
        [Description("The collection of menu items")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> MenuItems
        {
            get => _menuItems;
            set
            {
                if (_menuItems != value)
                {
                    if (_menuItems != null)
                    {
                        _menuItems.ListChanged -= MenuItems_ListChanged;
                    }
                    
                    _menuItems = value ?? new BindingList<SimpleItem>();
                    _menuItems.ListChanged += MenuItems_ListChanged;
                    RecalculateSize();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        [Browsable(false)]
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    _selectedIndex = _selectedItem != null ? _menuItems.IndexOf(_selectedItem) : -1;
                    OnSelectedItemChanged();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the selected index
        /// </summary>
        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value && value >= -1 && value < _menuItems.Count)
                {
                    _selectedIndex = value;
                    _selectedItem = _selectedIndex >= 0 ? _menuItems[_selectedIndex] : null;
                    OnSelectedItemChanged();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether multi-select is enabled
        /// </summary>
        [Category("Beep")]
        [Description("Enable multi-select mode (menu stays open after selection)")]
        [Browsable(true)]
        [DefaultValue(false)]
        public bool MultiSelect
        {
            get => _multiSelect;
            set
            {
                if (_multiSelect != value)
                {
                    _multiSelect = value;
                    if (_multiSelect)
                    {
                        // Clear selected items when enabling multi-select
                        _selectedItems.Clear();
                    }
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets the list of selected items (when MultiSelect is enabled)
        /// </summary>
        [Browsable(false)]
        public List<SimpleItem> SelectedItems
        {
            get => _selectedItems;
        }
        
        /// <summary>
        /// Gets or sets whether to show checkboxes
        /// </summary>
        [Category("Beep")]
        [Description("Show checkboxes for menu items")]
        [Browsable(true)]
        [DefaultValue(false)]
        public bool ShowCheckBox
        {
            get => _showCheckBox;
            set
            {
                if (_showCheckBox != value)
                {
                    _showCheckBox = value;
                    RecalculateSize();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether to show images
        /// </summary>
        [Category("Beep")]
        [Description("Show images for menu items")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool ShowImage
        {
            get => _showImage;
            set
            {
                if (_showImage != value)
                {
                    _showImage = value;
                    RecalculateSize();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether to show separators
        /// </summary>
        [Category("Beep")]
        [Description("Show separators between menu sections")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool ShowSeparators
        {
            get => _showSeparators;
            set
            {
                if (_showSeparators != value)
                {
                    _showSeparators = value;
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether to show keyboard shortcuts
        /// </summary>
        [Category("Beep")]
        [Description("Show keyboard shortcuts for menu items")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool ShowShortcuts
        {
            get => _showShortcuts;
            set
            {
                if (_showShortcuts != value)
                {
                    _showShortcuts = value;
                    RecalculateSize();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the menu item height
        /// </summary>
        [Category("Beep")]
        [Description("Height of each menu item")]
        [Browsable(true)]
        [DefaultValue(28)]
        public int MenuItemHeight
        {
            get => _menuItemHeight;
            set
            {
                if (_menuItemHeight != value && value > 0)
                {
                    _menuItemHeight = value;
                    RecalculateSize();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the image size
        /// </summary>
        [Category("Beep")]
        [Description("Size of menu item images")]
        [Browsable(true)]
        [DefaultValue(20)]
        public int ImageSize
        {
            get => _imageSize;
            set
            {
                if (_imageSize != value && value > 0)
                {
                    _imageSize = value;
                    RecalculateSize();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the menu width
        /// </summary>
        [Category("Beep")]
        [Description("Width of the context menu")]
        [Browsable(true)]
        [DefaultValue(200)]
        public int MenuWidth
        {
            get => _menuWidth;
            set
            {
                if (_menuWidth != value && value >= _minWidth && value <= _maxWidth)
                {
                    _menuWidth = value;
                    RecalculateSize();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the minimum menu width
        /// </summary>
        [Category("Beep")]
        [Description("Minimum width of the context menu")]
        [Browsable(true)]
        [DefaultValue(150)]
        public int MinWidth
        {
            get => _minWidth;
            set
            {
                if (_minWidth != value && value > 0)
                {
                    _minWidth = value;
                    if (_menuWidth < _minWidth) _menuWidth = _minWidth;
                    RecalculateSize();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the maximum menu width
        /// </summary>
        [Category("Beep")]
        [Description("Maximum width of the context menu")]
        [Browsable(true)]
        [DefaultValue(400)]
        public int MaxWidth
        {
            get => _maxWidth;
            set
            {
                if (_maxWidth != value && value > _minWidth)
                {
                    _maxWidth = value;
                    if (_menuWidth > _maxWidth) _menuWidth = _maxWidth;
                    RecalculateSize();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the text font
        /// </summary>
        [Category("Beep")]
        [Description("Font for menu item text")]
        [Browsable(true)]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                if (_textFont != value)
                {
                    _textFont = value ?? new Font("Segoe UI", 9f);
                    RecalculateSize();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the shortcut font
        /// </summary>
        [Category("Beep")]
        [Description("Font for keyboard shortcuts")]
        [Browsable(true)]
        public Font ShortcutFont
        {
            get => _shortcutFont;
            set
            {
                if (_shortcutFont != value)
                {
                    _shortcutFont = value ?? new Font("Segoe UI", 8f);
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether to close the menu when an item is clicked
        /// </summary>
        [Category("Beep")]
        [Description("Close menu when an item is clicked")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool CloseOnItemClick
        {
            get => _closeOnItemClick;
            set => _closeOnItemClick = value;
        }
        
        /// <summary>
        /// Gets or sets whether to close the menu when focus is lost
        /// </summary>
        [Category("Beep")]
        [Description("Close menu when it loses focus")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool CloseOnFocusLost
        {
            get => _closeOnFocusLost;
            set => _closeOnFocusLost = value;
        }
        
        /// <summary>
        /// Gets or sets the owner control
        /// </summary>
        [Browsable(false)]
        public Control Owner
        {
            get => _owner;
            set => _owner = value;
        }

        /// <summary>
        /// When true (default), TextFont and ShortcutFont adopt fonts from the current theme automatically.
        /// </summary>
        [Category("Beep")]
        [Description("Adopt fonts from the current theme automatically for menu text and shortcuts.")]
        [DefaultValue(true)]
        public bool UseThemeFonts
        {
            get => _useThemeFonts;
            set
            {
                if (_useThemeFonts == value) return;
                _useThemeFonts = value;
                if (value)
                {
                    try { ApplyThemeFontsSafely(); } catch { }
                }
                Invalidate();
            }
        }

        /// <summary>
        /// If true, Close() will dispose the form; if false (default), Close() hides the menu so it can be reused.
        /// </summary>
        [Category("Beep")]
        [Description("If true, Close() disposes the context menu. If false, Close() hides it for reuse.")]
        [DefaultValue(false)]
        public bool DestroyOnClose
        {
            get => _destroyOnClose;
            set => _destroyOnClose = value;
        }
        
        #endregion
        
        #region Private Event Handlers
        
        private void MenuItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            RecalculateSize();
            Invalidate();
        }
        
        #endregion
    }
#pragma warning restore IL2026
}
