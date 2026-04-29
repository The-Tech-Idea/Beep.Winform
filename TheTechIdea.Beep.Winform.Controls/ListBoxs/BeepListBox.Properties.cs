using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Models;
using TheTechIdea.Beep.Winform.Controls.ListBoxs.Tokens;
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
        /// Gets or sets the visual Style type for the list box
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual Style type of the list box")]
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
                    // Anchor the selection for potential range selection
                    _anchorItem = _selectedItem;
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
                if (MultiSelect)
                {
                    return _selectedItems.ToList();
                }
                var selected = new List<SimpleItem>();
                foreach (var kvp in _itemCheckBoxes)
                {
                    if (kvp.Value.State == CheckBoxState.Checked)
                    {
                        selected.Add(kvp.Key);
                    }
                }
                return selected;
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
                    if (_showSearch)
                        InitializeSearchTextBox();
                    else
                        DisposeSearchTextBox();
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
                    if (!PersistCollapsedGroups && _collapsedGroupKeys.Count > 0)
                    {
                        _collapsedGroupKeys.Clear();
                    }
                    // Sync to the search text box (avoid re-entrant loop)
                    if (_searchTextBox != null && !_isUpdatingSearchText && _searchTextBox.Text != _searchText)
                    {
                        _isUpdatingSearchText = true;
                        try { _searchTextBox.Text = _searchText; }
                        finally { _isUpdatingSearchText = false; }
                    }
                    OnSearchTextChanged();
                    OnSearchChanged(_searchText, _helper?.GetVisibleItems()?.Count ?? 0);
                    InvalidateLayoutCache();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Search matching strategy used when filtering list items.")]
        [DefaultValue(ListSearchMode.Contains)]
        public ListSearchMode SearchMode
        {
            get => _searchMode;
            set
            {
                if (_searchMode != value)
                {
                    _searchMode = value;
                    InvalidateLayoutCache();
                }
            }
        }
        private ListSearchMode _searchMode = ListSearchMode.Contains;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Highlights search matches in item labels when SearchText is active.")]
        [DefaultValue(true)]
        public bool HighlightSearchMatches { get; set; } = true;
        
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
                    InvalidateLayoutCache();
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
                    InvalidateLayoutCache();
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

        /// <summary>
        /// Enable hover animations for the list items
        /// </summary>
        [Browsable(true)]
        [Category("Animation")]
        [Description("Enable subtle hover animations for items")]
        [DefaultValue(true)]
        public bool EnableHoverAnimation { get; set; } = true;

        /// <summary>
        /// Duration in milliseconds for hover animation
        /// </summary>
        [Browsable(true)]
        [Category("Animation")]
        [Description("Duration in milliseconds for hover animations")]
        [DefaultValue(200)]
        public int HoverAnimationDuration
        {
            get => _hoverAnimationDuration;
            set
            {
                if (value <= 0) value = 200;
                if (_hoverAnimationDuration != value)
                {
                    _hoverAnimationDuration = value;
                    // Make sure the timer step is recalculated in owner
                    try
                    {
                        _hoverAnimationStep = 16f / Math.Max(1f, (float)_hoverAnimationDuration);
                    }
                    catch { }
                }
            }
        }
        private int _hoverAnimationDuration = 200;
        
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
                    InvalidateLayoutCache();
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

        #region Selection Visuals

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background color used for selection overlays")]
        [DefaultValue(null)]
        public Color SelectionBackColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Selection overlay alpha (0..255)")]
        [DefaultValue(90)]
        public int SelectionOverlayAlpha { get; set; } = 90;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Border color for selected items")]
        [DefaultValue(null)]
        public Color SelectionBorderColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Border thickness for selected item border")]
        [DefaultValue(1)]
        public int SelectionBorderThickness { get; set; } = 1;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Outline color when the control and item are focused")]
        [DefaultValue(null)]
        public Color FocusOutlineColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Outline thickness when focused")]
        [DefaultValue(2)]
        public int FocusOutlineThickness { get; set; } = 2;

        #endregion

        #region MultiSelect

        /// <summary>
        /// When true, selecting multiple items is allowed using Shift/Ctrl (non-checkbox multi selection)
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow multiple selection with Shift/Ctrl when no checkboxes are shown")]
        [DefaultValue(false)]
        private bool _multiSelect = false;
        public bool MultiSelect
        {
            get => _multiSelect;
            set
            {
                if (_multiSelect != value)
                {
                    _multiSelect = value;
                    if (_multiSelect && SelectionMode == SelectionModeEnum.Single)
                    {
                        SelectionMode = SelectionModeEnum.MultiExtended; // keep behavior consistent
                    }
                    RequestDelayedInvalidate();
                }
            }
        }

        #endregion

        #region Selection Mode

        /// <summary>
        /// Selection mode for the list box - Single, MultiSimple (click toggles), MultiExtended (Shift range + Ctrl toggle)
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Selection mode of the ListBox (Single, MultiSimple, MultiExtended)")]
        [DefaultValue(SelectionModeEnum.Single)]
        public SelectionModeEnum SelectionMode { get; set; } = SelectionModeEnum.Single;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, keyboard focus movement updates selection (recommended for single-select listbox behavior).")]
        [DefaultValue(true)]
        public bool SelectionFollowsFocus { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, Space toggles focused item selection in multi-select modes.")]
        [DefaultValue(true)]
        public bool SpaceTogglesSelectionInMulti { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, Enter invokes primary activation for the focused item.")]
        [DefaultValue(true)]
        public bool EnterInvokesPrimaryAction { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, focus enters the selected item first; otherwise first visible item is focused.")]
        [DefaultValue(true)]
        public bool FocusFirstSelectedOnFocusEnter { get; set; } = true;

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
        public string SearchPlaceholderText
        {
            get => _searchPlaceholderText;
            set
            {
                _searchPlaceholderText = value ?? "Search...";
                if (_searchTextBox != null)
                    _searchTextBox.PlaceholderText = _searchPlaceholderText;
            }
        }
        private string _searchPlaceholderText = "Search...";

        /// <summary>
        /// Show a friendly empty state when there are no items
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show a friendly empty state when there are no items")]
        [DefaultValue(true)]
        public bool ShowEmptyState { get; set; } = true;

        /// <summary>
        /// Text displayed when the list has no items
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Placeholder text to show when the list is empty")]
        [DefaultValue("No items")] 
        public string EmptyStateText { get; set; } = "No items";
        
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

        // ════════════════════════════════════════════════════════════════════════════
        //  Sprint 2 — Grouping & rich item support
        // ════════════════════════════════════════════════════════════════════════════

        #region Grouping

        /// <summary>Group items by their Category property.</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Group list items by their Category field.")]
        [DefaultValue(false)]
        public bool ShowGroups
        {
            get => _showGroups;
            set
            {
                if (_showGroups != value)
                {
                    _showGroups = value;
                    InvalidateLayoutCache();
                }
            }
        }
        private bool _showGroups;

        /// <summary>Allow groups to be collapsed / expanded by clicking the header.</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow group headers to be collapsed/expanded.")]
        [DefaultValue(true)]
        public bool CollapsibleGroups { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, collapsed group state persists while filtering and refreshing.")]
        [DefaultValue(true)]
        public bool PersistCollapsedGroups { get; set; } = true;

        #endregion

        // ════════════════════════════════════════════════════════════════════════════
        //  Hierarchy
        // ════════════════════════════════════════════════════════════════════════════

        #region Hierarchy

        /// <summary>
        /// When true, items with Children are rendered as an expandable tree.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Render items with Children as an expandable hierarchical list.")]
        [DefaultValue(false)]
        public bool ShowHierarchy
        {
            get => _showHierarchy;
            set
            {
                if (_showHierarchy != value)
                {
                    _showHierarchy = value;
                    InvalidateLayoutCache();
                }
            }
        }
        private bool _showHierarchy;

        /// <summary>Raised when an item is expanded.</summary>
        public event EventHandler<ListBoxItemEventArgs> ItemExpanded;

        /// <summary>Raised when an item is collapsed.</summary>
        public event EventHandler<ListBoxItemEventArgs> ItemCollapsed;

        protected virtual void OnItemExpanded(int index, SimpleItem item)
            => ItemExpanded?.Invoke(this, new ListBoxItemEventArgs(index, item));

        protected virtual void OnItemCollapsed(int index, SimpleItem item)
            => ItemCollapsed?.Invoke(this, new ListBoxItemEventArgs(index, item));

        #endregion

        // ════════════════════════════════════════════════════════════════════════════
        //  Sprint 4 — Loading / skeleton state
        // ════════════════════════════════════════════════════════════════════════════

        #region Loading state

        /// <summary>
        /// When true the list renders animated skeleton rows instead of real items.
        /// Set to true before starting an async data load; set back to false when done.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show skeleton placeholder rows during data loading.")]
        [DefaultValue(false)]
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    if (value) StartSkeletonAnimation();
                    else StopSkeletonAnimation();
                    Invalidate();
                }
            }
        }
        private bool _isLoading;

        /// <summary>Number of skeleton rows shown while IsLoading = true.</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Number of skeleton placeholder rows to show while loading.")]
        [DefaultValue(5)]
        public int SkeletonRowCount { get; set; } = 5;

        #endregion

        // ════════════════════════════════════════════════════════════════════════════
        //  Sprint 6 — Density mode
        // ════════════════════════════════════════════════════════════════════════════

        #region Density

        /// <summary>Row height density — maps to ListBoxTokens.ItemHeight* constants.</summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Row height density: Comfortable (52 px), Compact (40 px), Dense (28 px).")]
        [DefaultValue(ListDensityMode.Comfortable)]
        public ListDensityMode Density
        {
            get => _density;
            set
            {
                if (_density != value)
                {
                    _density = value;
                    // Sync MenuItemHeight with the token value so painters don't need extra logic
                    _menuItemHeight = DensityToPixels(value);
                    InvalidateLayoutCache();
                }
            }
        }
        private ListDensityMode _density = ListDensityMode.Comfortable;

        /// <summary>Converts a density enum to the corresponding logical-pixel height.</summary>
        private int DensityToPixels(ListDensityMode d) => d switch
        {
            ListDensityMode.Dense   => ListBoxTokens.ItemHeightDense,
            ListDensityMode.Compact => ListBoxTokens.ItemHeightCompact,
            _                       => ListBoxTokens.ItemHeightComfortable
        };

        /// <summary>Whether items may have non-uniform heights (painters call GetItemHeight).</summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("Allow painters to return variable row heights per item.")]
        [DefaultValue(false)]
        public bool AutoItemHeight
        {
            get => _autoItemHeight;
            set
            {
                if (_autoItemHeight != value)
                {
                    _autoItemHeight = value;
                    InvalidateLayoutCache();
                }
            }
        }
        private bool _autoItemHeight;

        #endregion

        // ════════════════════════════════════════════════════════════════════════════
        //  Sprint 6 — Data binding (DataSource / DisplayMember / ValueMember)
        // ════════════════════════════════════════════════════════════════════════════

        #region Data Binding

        /// <summary>
        /// Gets or sets the data source. Accepts IList / IBindingList / IEnumerable.
        /// When set, ListItems is auto-populated from the source.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("Binding list source. Accepts IList or IBindingList.")]
        [DefaultValue(null)]
        public object? DataSource
        {
            get => _dataSource;
            set
            {
                if (_dataSource != value)
                {
                    _dataSource = value;
                    ApplyDataSource();
                }
            }
        }
        private object? _dataSource;

        /// <summary>Property name on the data source object used as the display text.</summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("Property name used as display text when DataSource is set.")]
        [DefaultValue("")]
        public string DisplayMember
        {
            get => _displayMember;
            set { _displayMember = value ?? ""; if (_dataSource != null) ApplyDataSource(); }
        }
        private string _displayMember = "";

        /// <summary>Property name on the data source object used as the underlying value.</summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("Property name used as value when DataSource is set.")]
        [DefaultValue("")]
        public string ValueMember
        {
            get => _valueMember;
            set { _valueMember = value ?? ""; if (_dataSource != null) ApplyDataSource(); }
        }
        private string _valueMember = "";

        /// <summary>Gets the underlying value of the currently selected item.</summary>
        [Browsable(false)]
        public object? SelectedValue
        {
            get
            {
                if (_selectedItem == null || string.IsNullOrEmpty(_valueMember)) return null;
                var prop = _selectedItem.GetType().GetProperty(_valueMember);
                return prop?.GetValue(_selectedItem);
            }
        }

        #endregion

        // ════════════════════════════════════════════════════════════════════════════
        //  Sprint 3 — Interaction features
        // ════════════════════════════════════════════════════════════════════════════

        #region Interaction features

        /// <summary>Allow items to be reordered by drag-and-drop.</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow items to be reordered by dragging.")]
        [DefaultValue(false)]
        public bool AllowItemReorder { get; set; }

        /// <summary>Show a default context menu (Select / Copy / Edit / Delete) on right-click.</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show a context menu on right-click.")]
        [DefaultValue(true)]
        public bool ShowContextMenu { get; set; } = true;

        /// <summary>Optional consumer-provided context menu used instead of the default one.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Windows.Forms.ContextMenuStrip? ItemContextMenu { get; set; }

        /// <summary>Allow inline edit of item text via F2 or double-click.</summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow inline item text editing with F2 or double-click.")]
        [DefaultValue(false)]
        public bool AllowInlineEdit { get; set; }

        #endregion

        // ════════════════════════════════════════════════════════════════════════════
        //  Sprint 6 — New events
        // ════════════════════════════════════════════════════════════════════════════

        #region New events (Sprint 6)

        /// <summary>Raised when an item is activated (Enter, double-click).</summary>
        public event EventHandler<ListBoxItemEventArgs>? ItemActivated;

        /// <summary>Raised when the Delete key is pressed over a selected item.</summary>
        public event EventHandler<ListBoxItemEventArgs>? ItemDeleteRequested;

        /// <summary>Raised after an inline-edit (F2) is committed by the user.</summary>
        public event EventHandler<ListBoxItemTextChangedEventArgs>? ItemTextChanged;

        /// <summary>Raised after a drag-reorder completes.</summary>
        public event EventHandler<ListBoxReorderEventArgs>? ItemReordered;

        /// <summary>Raised before the context menu is shown. Set Cancel = true to suppress.</summary>
        public event EventHandler<ListBoxContextMenuEventArgs>? ContextMenuOpening;

        /// <summary>Raised when the infinite-scroll sentinel is reached.</summary>
        public event EventHandler? LoadMoreRequested;

        /// <summary>Raised when a group header is collapsed.</summary>
        public event EventHandler<ListBoxGroupEventArgs>? GroupCollapsed;

        /// <summary>Raised when a group header is expanded.</summary>
        public event EventHandler<ListBoxGroupEventArgs>? GroupExpanded;

        /// <summary>Raised on every SearchText change; includes match count.</summary>
        public event EventHandler<ListBoxSearchEventArgs>? SearchChanged;

        // Protected raise helpers

        protected virtual void OnItemActivated(int index, SimpleItem item)
            => ItemActivated?.Invoke(this, new ListBoxItemEventArgs(index, item));

        protected virtual void OnItemDeleteRequested(int index, SimpleItem item)
            => ItemDeleteRequested?.Invoke(this, new ListBoxItemEventArgs(index, item));

        protected virtual void OnItemTextChanged(SimpleItem item, string oldText, string newText)
            => ItemTextChanged?.Invoke(this, new ListBoxItemTextChangedEventArgs(item, oldText, newText));

        protected virtual void OnItemReordered(int oldIdx, int newIdx, SimpleItem item)
            => ItemReordered?.Invoke(this, new ListBoxReorderEventArgs(oldIdx, newIdx, item));

        protected virtual bool OnContextMenuOpening(int index, SimpleItem? item, System.Windows.Forms.ContextMenuStrip menu)
        {
            var args = new ListBoxContextMenuEventArgs(index, item, menu);
            ContextMenuOpening?.Invoke(this, args);
            return !args.Cancel;
        }

        protected virtual void OnLoadMoreRequested()
            => LoadMoreRequested?.Invoke(this, EventArgs.Empty);

        protected virtual void OnGroupCollapsed(string groupKey)
            => GroupCollapsed?.Invoke(this, new ListBoxGroupEventArgs(groupKey, true));

        protected virtual void OnGroupExpanded(string groupKey)
            => GroupExpanded?.Invoke(this, new ListBoxGroupEventArgs(groupKey, false));

        protected virtual void OnSearchChanged(string query, int matchCount)
            => SearchChanged?.Invoke(this, new ListBoxSearchEventArgs(query, matchCount));

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ListBoxVariantMetadata ActiveVariantMetadata => ListBoxVariantMetadataCatalog.Resolve(ListBoxType);

        #endregion
    }
}
