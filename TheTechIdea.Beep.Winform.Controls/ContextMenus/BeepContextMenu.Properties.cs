using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus
{
#pragma warning disable IL2026 // Suppress trimmer warnings for BindingList<T> used in WinForms data binding scenarios
    public partial class BeepContextMenu
    {
        #region Public Properties
        
        /// <summary>
        /// Gets or sets the visual Style of the context menu
        /// </summary>
        [Category("Beep")]
        [Description("The visual Style of the context menu")]
        [Browsable(true)]
        public FormStyle ContextMenuType
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
        /// Gets or sets the control style for BeepStyling integration
        /// </summary>
        [Category("Appearance")]
        [Description("The visual style/painter to use for rendering the context menu using BeepStyling system")]
        [Browsable(true)]
        [DefaultValue(BeepControlStyle.None)]
        public BeepControlStyle ControlStyle
        {
            get => _controlStyle;
            set
            {
                if (_controlStyle != value)
                {
                    _controlStyle = value;
                    // Invalidate region when style changes (rounded corners may change)
                    if (Region != null)
                    {
                        Region.Dispose();
                        Region = null;
                    }
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to use theme colors instead of custom colors
        /// </summary>
        [Category("Appearance")]
        [Description("Use theme colors instead of custom accent color")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                if (_useThemeColors != value)
                {
                    _useThemeColors = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the FormStyle for visual appearance (maps to ContextMenuType)
        /// </summary>
        [Category("Beep")]
        [Description("The form style for the context menu appearance")]
        [Browsable(true)]
        public FormStyle FormStyle
        {
            get => _contextMenuType;
            set => ContextMenuType = value;
        }

        /// <summary>
        /// Gets or sets the border color
        /// </summary>
        [Category("Appearance")]
        [Description("The border color of the context menu")]
        [Browsable(true)]
        public Color BorderColor
        {
            get => _currentTheme?.BorderColor ?? Color.FromArgb(200, 200, 200);
            set { /* Read-only - derived from theme */ }
        }

        /// <summary>
        /// Gets or sets the corner radius
        /// </summary>
        [Category("Appearance")]
        [Description("The corner radius of the context menu")]
        [Browsable(true)]
        public Forms.ModernForm.Painters.CornerRadius CornerRadius
        {
            get => _cornerRadius ?? (_cornerRadius = new Forms.ModernForm.Painters.CornerRadius(8));
            set
            {
                if (_cornerRadius != value)
                {
                    _cornerRadius = value;
                    // Invalidate region when corner radius changes (rounded corners will change)
                    if (Region != null)
                    {
                        Region.Dispose();
                        Region = null;
                    }
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current theme
        /// </summary>
        [Browsable(false)]
        public IBeepTheme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the shadow effect
        /// </summary>
        [Category("Appearance")]
        [Description("The shadow effect for the context menu")]
        [Browsable(true)]
        public Forms.ModernForm.Painters.ShadowEffect ShadowEffect
        {
            get => _shadowEffect ?? (_shadowEffect = new Forms.ModernForm.Painters.ShadowEffect());
            set
            {
                if (_shadowEffect != value)
                {
                    _shadowEffect = value;
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
                    _fullMenuItems = _menuItems.ToList();
                    _menuItems.ListChanged += MenuItems_ListChanged;
                    InvalidateLayoutCache();
                    InvalidateSizeCache();
                    RecalculateSize();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Invalidates the layout helper cache
        /// </summary>
        private void InvalidateLayoutCache()
        {
            _layoutHelper?.InvalidateCache();
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
                    InvalidateLayoutCache();
                    InvalidateSizeCache();
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
                    InvalidateLayoutCache();
                    InvalidateSizeCache();
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
                    InvalidateLayoutCache();
                    InvalidateSizeCache();
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
                    InvalidateLayoutCache();
                    InvalidateSizeCache();
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
                    InvalidateSizeCache();
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
                // Validate font before setting
             
                    if (_textFont != value)
                    {
                        _textFont = value;
                        InvalidateSizeCache();
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
                        _shortcutFont = value;
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
        
        /// <summary>
        /// Gets or sets the maximum height before scrolling is enabled
        /// </summary>
        [Category("Beep")]
        [Description("Maximum height of the context menu before scrolling is enabled")]
        [Browsable(true)]
        [DefaultValue(600)]
        public int MaxHeight
        {
            get => _maxHeight;
            set
            {
                if (_maxHeight != value && value >= _minHeight)
                {
                    _maxHeight = value;
                    RecalculateSize();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets the minimum height of the context menu (calculated as one item height + padding)
        /// </summary>
        [Category("Beep")]
        [Description("Minimum height of the context menu (automatically calculated as one item + padding)")]
        [Browsable(false)]
        public int MinHeight
        {
            get => PreferredItemHeight + 8;
        }
        
        /// <summary>
        /// Gets whether the menu currently needs scrolling
        /// </summary>
        [Browsable(false)]
        public bool NeedsScrolling => _needsScrolling;
        
        /// <summary>
        /// Gets the current scroll offset
        /// </summary>
        [Browsable(false)]
        public int ScrollOffset => _scrollOffset;
     

        #endregion

        #region Private Event Handlers

        private void MenuItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            _fullMenuItems = _menuItems.ToList();
            InvalidateLayoutCache();
            InvalidateSizeCache();
            RecalculateSize();
            Invalidate();
        }

        /// <summary>
        /// Gets or sets whether a search box is shown at the top of the context menu
        /// </summary>
        [Category("Beep")]
        [Description("Show a search box at the top of the context menu for filtering items")]
        [Browsable(true)]
        [DefaultValue(false)]
        public bool ShowSearchBox
        {
            get => _showSearchBox;
            set
            {
                if (_showSearchBox != value)
                {
                    _showSearchBox = value;
                    // Ensure search textbox control existence
                    if (_showSearchBox)
                    {
                        EnsureSearchTextBox();
                    }
                    else
                    {
                        EnsureSearchTextBox(); // will dispose/hide the search control
                    }
                    InvalidateSizeCache();
                    RecalculateSize();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Returns height of the search area, if present (used by layout/drawing helpers)
        /// </summary>
        [Browsable(false)]
        public int SearchBoxHeight => _showSearchBox ? (_searchTextBox != null ? _searchTextBox.Height : 40) : 0;
        
        /// <summary>
        /// Accessible name for screen readers
        /// </summary>
        [Browsable(true)]
        [Category("Accessibility")]
        [Description("Name of the control for accessibility and screen readers.")]
        public new string AccessibleName
        {
            get => base.AccessibleName;
            set => base.AccessibleName = value;
        }

        /// <summary>
        /// Accessible description for screen readers
        /// </summary>
        [Browsable(true)]
        [Category("Accessibility")]
        [Description("Description of the control for accessibility and screen readers.")]
        public new string AccessibleDescription
        {
            get => base.AccessibleDescription;
            set => base.AccessibleDescription = value;
        }

        /// <summary>
        /// Duration of animations in milliseconds (fade-in/fade-out, item selection)
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Duration of animations in milliseconds (fade-in/fade-out, item selection).")]
        [DefaultValue(200)]
        public int AnimationDuration
        {
            get => _animationDuration;
            set
            {
                if (_animationDuration != value && value >= 0)
                {
                    _animationDuration = value;
                }
            }
        }

        /// <summary>
        /// Whether animations are enabled
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether animations are enabled.")]
        [DefaultValue(true)]
        public bool EnableAnimations
        {
            get => _enableAnimations;
            set
            {
                if (_enableAnimations != value)
                {
                    _enableAnimations = value;
                    if (!_enableAnimations)
                    {
                        _opacity = 1.0;
                        Opacity = 1.0;
                        _fadeTimer?.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Duration of fade-in animation in milliseconds
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Duration of fade-in animation in milliseconds.")]
        [DefaultValue(150)]
        public int FadeInDuration
        {
            get => _fadeInDuration;
            set
            {
                if (_fadeInDuration != value && value >= 0)
                {
                    _fadeInDuration = value;
                }
            }
        }

        /// <summary>
        /// Duration of fade-out animation in milliseconds
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Duration of fade-out animation in milliseconds.")]
        [DefaultValue(100)]
        public int FadeOutDuration
        {
            get => _fadeOutDuration;
            set
            {
                if (_fadeOutDuration != value && value >= 0)
                {
                    _fadeOutDuration = value;
                }
            }
        }
        
        #endregion
    }
#pragma warning restore IL2026
}
