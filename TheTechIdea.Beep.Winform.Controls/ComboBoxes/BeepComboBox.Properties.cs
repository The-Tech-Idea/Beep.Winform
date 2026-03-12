using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Auto-complete matching mode
    /// </summary>
    public enum BeepAutoCompleteMode
    {
        None,
        Prefix,
        Fuzzy,
        Full
    }

    /// <summary>
    /// Validation state that drives border colour and status icon.
    /// </summary>
    public enum BeepComboBoxValidationState
    {
        None,
        Error,
        Warning,
        Success
    }

    /// <summary>
    /// Predefined height tokens that mirror design-system size scales.
    /// </summary>
    public enum BeepComboBoxSize
    {
        Small,
        Medium,
        Large
    }

    public partial class BeepComboBox
    {
        #region List and Selection Properties
        
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Category("Data")]
        [Description("The list of items in the combo box.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _listItems;
            set
            {
                _listItems = value ?? new BindingList<SimpleItem>();
              
                Invalidate();
            }
        }
        
        [Browsable(true)]
        [Category("Data")]
        [Description("The currently selected item.")]
        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value) return;
                
                if (value == null)
                {
                    _selectedItem = null;
                    _selectedItemIndex = -1;
                    _inputText = string.Empty;
                    Text = string.Empty;
                }
                else
                {
                    _selectedItem = value;
                    _selectedItemIndex = _listItems.IndexOf(_selectedItem);
                    _inputText = value.Text;
                    Text = value.Text;
                    
                    if (_selectedItem.Item != null)
                    {
                        SelectedValue = _selectedItem.Item;
                    }
                    
                    // Clear error state on valid selection
                    if (HasError && !string.IsNullOrEmpty(_selectedItem.Text))
                    {
                        HasError = false;
                        ErrorText = string.Empty;
                    }
                }
                
                OnSelectedItemChanged(_selectedItem);
                // ENH-12: keep AccessibleObject.Value in sync with current text
                AccessibilityNotifyClients(System.Windows.Forms.AccessibleEvents.ValueChange, -1);
                Invalidate();
            }
        }
        
        [Browsable(true)]
        [Category("Data")]
        [Description("The text of the currently selected item.")]
        public string SelectedText
        {
            get => _inputText;
        }
        
        [Browsable(false)]
        [Category("Data")]
        [Description("The index of the currently selected item.")]
        public int SelectedIndex
        {
            get => _selectedItemIndex;
            set
            {
                if (value >= 0 && value < _listItems.Count)
                {
                    SelectedItem = _listItems[value];
                }
                else
                {
                    SelectedItem = null;
                }
            }
        }
        
        #endregion
        
        #region Appearance Properties
        
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(ComboBoxType.OutlineDefault)]
        [Description("The visual Style/variant of the combo box.")]
        public ComboBoxType ComboBoxType
        {
            get => _comboBoxType;
            set
            {
                if (_comboBoxType == value) return;

                // Hide inline editor before switching — the new type may not support it
                HideInlineEditor(false);

                if (!_suppressComboBoxTypeExplicitTracking)
                {
                    _comboBoxTypeWasExplicitlySet = true;
                }
                _comboBoxType = value;
                _comboBoxPainter = null; // Force painter recreation

                if (_comboBoxType == ComboBoxType.MultiChipCompact || _comboBoxType == ComboBoxType.MultiChipSearch)
                {
                    _allowMultipleSelection = true;
                }
                if (ComboBoxVisualTokenCatalog.SupportsSearch(_comboBoxType))
                {
                    _showSearchInDropdown = true;
                }
                
                // Force re-application of painter defaults for new type
                _layoutDefaultsInitialized = false;
                ApplyLayoutDefaultsFromPainter(force: true);
                
                InvalidateLayout();
                // Update dropdown properties
                if (BeepContextMenu != null)
                {
                    BeepContextMenu.ShowSearchBox = ComboBoxVisualTokenCatalog.SupportsSearch(_comboBoxType) || ShowSearchInDropdown;
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Maps Beep ControlStyle to ComboBoxType unless ComboBoxType was set manually.")]
        public new BeepControlStyle ControlStyle
        {
            get => base.ControlStyle;
            set
            {
                if (base.ControlStyle == value) return;
                base.ControlStyle = value;
                ApplyComboBoxTypeFromControlStyleIfNeeded();
            }
        }
        
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        /// <summary>
        /// The raw text the user has typed into the inline editor.
        /// Exposed so painters and the helper can read it.
        /// </summary>
        internal string InputText => _inputText;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                if (_textFont != value)
                {
                    _textFont = value;
                    UseThemeFont = false;
                    InvalidateLayout();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Placeholder text shown when no item is selected.")]
        public string PlaceholderText { get; set; } = "Select an item...";

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
        
        //[Browsable(true)]
        //[Category("Appearance")]
        //[Description("Whether the popup is currently open.")]
        //public bool IsPopupOpen
        //{
        //    get => _isPopupOpen;
        //    set
        //    {
        //        if (_isPopupOpen == value) return;
                
        //        _isPopupOpen = value;
        //        if (_isPopupOpen)
        //        {
        //            ShowPopup();
        //        }
        //        else
        //        {
        //            ClosePopup();
        //        }
        //    }
        //}
        
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Path to the dropdown icon image.")]
        public string DropdownIconPath
        {
            get => _dropdownIconPath;
            set
            {
                _dropdownIconPath = value;
                Invalidate();
            }
        }
        

        
        #endregion
        
        #region Behavior Properties
        
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether the combo box allows text editing.")]
        [DefaultValue(false)]
        public bool IsEditable { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show a search box in the dropdown (useful for long lists).")]
        [DefaultValue(false)]
        private bool _showSearchInDropdown = false;
        public bool ShowSearchInDropdown
        {
            get => _showSearchInDropdown;
            set
            {
                _showSearchInDropdown = value;
                if (BeepContextMenu != null)
                {
                    BeepContextMenu.ShowSearchBox = ComboBoxVisualTokenCatalog.SupportsSearch(_comboBoxType) || _showSearchInDropdown;
                }
                Invalidate();
            }
        }
        
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether to auto-complete as the user types.")]
        [DefaultValue(true)]
        public bool AutoComplete { get; set; } = true;

        /// <summary>
        /// The mode for auto-complete matching
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The mode for auto-complete matching.")]
        [DefaultValue(BeepAutoCompleteMode.Prefix)]
        public BeepAutoCompleteMode AutoCompleteMode { get; set; } = BeepAutoCompleteMode.Prefix;

        /// <summary>
        /// Minimum length of input before triggering auto-complete
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Minimum length of input before triggering auto-complete.")]
        [DefaultValue(1)]
        public int AutoCompleteMinLength { get; set; } = 1;

        /// <summary>
        /// Maximum number of suggestions to show
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Maximum number of suggestions to show.")]
        [DefaultValue(10)]
        public int MaxSuggestions { get; set; } = 10;

        /// <summary>
        /// Delay in milliseconds before triggering auto-complete (for debouncing)
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Delay in milliseconds before triggering auto-complete (for debouncing).")]
        [DefaultValue(0)]
        public int AutoCompleteDelay { get; set; } = 0;

        /// <summary>
        /// Duration of animations in milliseconds (dropdown open/close, item selection)
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Duration of animations in milliseconds (dropdown open/close, item selection).")]
        [DefaultValue(200)]
        public int AnimationDuration { get; set; } = 200;

        /// <summary>
        /// Whether animations are enabled
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether animations are enabled.")]
        [DefaultValue(true)]
        public bool EnableAnimations { get; set; } = true;

        /// <summary>
        /// Indicates whether the combo box is in a loading state. When true, shows a spinner and disables interaction.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Indicates whether the combo box is in a loading state. When true, shows a spinner and disables interaction.")]
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    if (_isLoading)
                    {
                        StartLoadingAnimation();
                    }
                    else
                    {
                        StopLoadingAnimation();
                    }
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Maximum height of the dropdown list.")]
        [DefaultValue(200)]
        public int MaxDropdownHeight { get; set; } = 200;
        
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Category type for the combo box.")]
        public DbFieldCategory Category { get; set; } = DbFieldCategory.String;
        
        #endregion

        #region Multi-Select Properties

        private bool _allowMultipleSelection = false;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allow multiple items to be selected.")]
        [DefaultValue(false)]
        public bool AllowMultipleSelection
        {
            get => _allowMultipleSelection;
            set
            {
                if (_allowMultipleSelection == value) return;
                _allowMultipleSelection = value;
                if (BeepContextMenu != null)
                {
                    BeepContextMenu.MultiSelect = value;
                    BeepContextMenu.ShowCheckBox = value;
                }
                Invalidate();
            }
        }

        private System.Collections.Generic.List<SimpleItem> _selectedItems = new System.Collections.Generic.List<SimpleItem>();
        [Browsable(false)]
        public System.Collections.Generic.List<SimpleItem> SelectedItems
        {
            get => _selectedItems;
            set
            {
                var newList = value ?? new System.Collections.Generic.List<SimpleItem>();
                // Diff: start animations for added/removed chips
                var added = new System.Collections.Generic.List<SimpleItem>();
                var removed = new System.Collections.Generic.List<SimpleItem>();
                foreach (var it in newList)
                {
                    bool exists = _selectedItems.Exists(existing => IsSameSimpleItem(existing, it));
                    if (!exists) added.Add(it);
                }
                foreach (var it in _selectedItems)
                {
                    bool stillSelected = newList.Exists(updated => IsSameSimpleItem(updated, it));
                    if (!stillSelected) removed.Add(it);
                }
                _selectedItems = newList;
                // Start appearing animations for added items
                foreach (var it in added)
                {
                    StartChipAnimation(it, true);
                }
                // Start disappearing animations for removed items
                foreach (var it in removed)
                {
                    StartChipAnimation(it, false);
                }
                // Keep SelectedItem in sync when one selected
                _selectedItem = _selectedItems.Count > 0 ? _selectedItems[0] : null;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Number of chips to show in the input when multiple items are selected.")]
        [DefaultValue(3)]
        public int MaxDisplayChips { get; set; } = 3;

        public event EventHandler SelectedItemsChanged;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Duration in milliseconds for chip add/remove animations.")]
        [DefaultValue(200)]
        public int ChipAnimationDuration { get; set; } = 200;

        #endregion
        
        #region Layout Properties
        
        [Browsable(true)]
        [Category("Layout")]
        [Description("Width of the dropdown button in pixels.")]
        [DefaultValue(32)]
        public int DropdownButtonWidth 
        { 
            get => _dropdownButtonWidth; 
            set 
            { 
                if (value <= 0)
                {
                    // Reset to painter default
                    if (_dropdownButtonWidthSetExplicitly)
                    {
                        _dropdownButtonWidthSetExplicitly = false;
                        _layoutDefaultsInitialized = false;
                        ApplyLayoutDefaultsFromPainter(force: true);
                        InvalidateLayout();
                    }
                    return;
                }

                if (_dropdownButtonWidth != value || !_dropdownButtonWidthSetExplicitly)
                {
                    _dropdownButtonWidthSetExplicitly = true;
                    _dropdownButtonWidth = value;
                    InvalidateLayout();
                }
            }
        }
        private int _dropdownButtonWidth = DefaultDropdownButtonWidthLogical;
        
        [Browsable(true)]
        [Category("Layout")]
        [Description("Padding inside the combo box.")]
        public Padding InnerPadding 
        { 
            get => _innerPadding; 
            set 
            { 
                // Allow reset to painter default by passing Padding.Empty
                if (value == Padding.Empty && _innerPaddingSetExplicitly)
                {
                    _innerPaddingSetExplicitly = false;
                    _layoutDefaultsInitialized = false;
                    ApplyLayoutDefaultsFromPainter(force: true);
                    InvalidateLayout();
                    return;
                }
                
                if (_innerPadding != value || !_innerPaddingSetExplicitly)
                {
                    _innerPaddingSetExplicitly = true;
                    _innerPadding = value;
                    InvalidateLayout();
                }
            }
        }
        private Padding _innerPadding = DefaultInnerPaddingLogical;

        [Browsable(true)]
        [Category("Layout")]
        [Description("Predefined height variant: Small (24 px), Medium (32 px), Large (40 px).")]
        [DefaultValue(BeepComboBoxSize.Medium)]
        public BeepComboBoxSize SizeVariant
        {
            get => _sizeVariant;
            set
            {
                if (_sizeVariant == value) return;
                _sizeVariant = value;
                _layoutDefaultsInitialized = false;
                ApplyLayoutDefaultsFromPainter(force: true, applyHeight: true);
                InvalidateLayout();
            }
        }
        private BeepComboBoxSize _sizeVariant = BeepComboBoxSize.Medium;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show a × button inside the control when a value is selected.")]
        [DefaultValue(false)]
        public bool ShowClearButton
        {
            get => _showClearButton;
            set
            {
                if (_showClearButton == value) return;
                _showClearButton = value;
                Invalidate();
            }
        }
        private bool _showClearButton = false;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Validation state that controls border colour and status icon (None, Error, Warning, Success).")]
        [DefaultValue(BeepComboBoxValidationState.None)]
        public BeepComboBoxValidationState ValidationState
        {
            get => _validationState;
            set
            {
                if (_validationState == value) return;
                _validationState = value;
                Invalidate();
            }
        }
        private BeepComboBoxValidationState _validationState = BeepComboBoxValidationState.None;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Animate the chevron icon when the dropdown opens / closes.")]
        [DefaultValue(true)]
        public bool AnimateChevron { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Skip all timer-driven animations (chevron, spinner) and snap to final state immediately.")]
        [DefaultValue(false)]
        public bool ReduceMotion { get; set; } = false;

        // ── ENH-07 ──────────────────────────────────────────────────────────
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Text shown as a disabled placeholder row when the dropdown list is empty or all items are filtered out.")]
        [DefaultValue("No options")]
        public string EmptyStateText { get; set; } = "No options";

        // ── ENH-08 ──────────────────────────────────────────────────────────
        [Browsable(true)]
        [Category("Appearance")]
        [Description("When true, the dropdown renders the ImagePath icon for each SimpleItem row.")]
        [DefaultValue(false)]
        public bool ShowStatusIcons { get; set; } = false;

        // ── ENH-11 ──────────────────────────────────────────────────────────
        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, the dropdown opens upward if there is insufficient space below the control.")]
        [DefaultValue(true)]
        public bool AutoFlip { get; set; } = true;

        // ── ENH-13 ──────────────────────────────────────────────────────────
        [Browsable(true)]
        [Category("Layout")]
        [Description("Minimum width (in pixels) of the dropdown popup. 0 means the popup matches the control width.")]
        [DefaultValue(0)]
        public int MinDropdownWidth { get; set; } = 0;

        // ── ENH-18 ──────────────────────────────────────────────────────────
        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true and AllowMultipleSelection is enabled, a 'Select all / Clear all' row is pinned at the top of the dropdown.")]
        [DefaultValue(true)]
        public bool ShowSelectAll { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true in multi-select mode, selection changes stay pending until Apply is pressed; Cancel restores snapshot.")]
        [DefaultValue(false)]
        public bool UseApplyCancelFooter { get; set; } = false;

        // ── ENH-16 ──────────────────────────────────────────────────────────
        [Browsable(true)]
        [Category("Appearance")]
        [Description("When true, the dropdown renders each item's SubText (description) as a second line below the label. Rows expand automatically to fit.")]
        [DefaultValue(true)]
        public bool ShowOptionDescription { get; set; } = true;

        // ── ENH-22 ──────────────────────────────────────────────────────────
        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, typing a token delimiter in the editable input converts the current token into a chip (multi-select).")]
        [DefaultValue(false)]
        public bool AllowFreeText { get; set; } = false;

        [Browsable(false)] // char[] isn't designer-serialisable
        [Description("Characters that trigger tokenization when AllowFreeText is enabled. Defaults to ',' and ';'.")]
        public char[] TokenDelimiters { get; set; } = new[] { ',', ';' };

        // ── ENH-23 ──────────────────────────────────────────────────────────
        [Browsable(true)]
        [Category("Appearance")]
        [Description("When true, renders an animated shimmer skeleton instead of content while data is loading.")]
        [DefaultValue(false)]
        public bool ShowSkeleton
        {
            get => _showSkeleton;
            set
            {
                if (_showSkeleton == value) return;
                _showSkeleton = value;
                if (_showSkeleton)
                    StartSkeletonAnimation();
                else
                    StopSkeletonAnimation();
                Invalidate();
            }
        }
        private bool _showSkeleton = false;

        // ── ENH-24 ──────────────────────────────────────────────────────────
        [Browsable(true)]
        [Category("Layout")]
        [Description("When true, the control layout mirrors horizontally: dropdown button on the left, text right-aligned, chips flow right-to-left.")]
        [DefaultValue(false)]
        public bool IsRtl
        {
            get => _isRtl;
            set
            {
                if (_isRtl == value) return;
                _isRtl = value;
                Invalidate();
            }
        }
        private bool _isRtl = false;

        #endregion
    }
}
