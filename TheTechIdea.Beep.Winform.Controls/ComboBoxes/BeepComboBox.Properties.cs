using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;

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
        [DefaultValue(ComboBoxType.Standard)]
        [Description("The visual Style/variant of the combo box.")]
        public ComboBoxType ComboBoxType
        {
            get => _comboBoxType;
            set
            {
                if (_comboBoxType == value) return;
                _comboBoxType = value;
                _comboBoxPainter = null; // Force painter recreation
                
                // Force re-application of painter defaults for new type
                _layoutDefaultsInitialized = false;
                ApplyLayoutDefaultsFromPainter(force: true);
                
                InvalidateLayout();
                // Update dropdown properties
                if (BeepContextMenu != null)
                {
                    BeepContextMenu.ShowSearchBox = (_comboBoxType == ComboBoxType.SearchableDropdown) || ShowSearchInDropdown;
                }
            }
        }
        
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                if (_textFont != value)
                {
                    _textFont = value ?? new Font("Segoe UI", 9f);
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
                    BeepContextMenu.ShowSearchBox = (_comboBoxType == ComboBoxType.SearchableDropdown) || _showSearchInDropdown;
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
                    if (!_selectedItems.Contains(it)) added.Add(it);
                }
                foreach (var it in _selectedItems)
                {
                    if (!newList.Contains(it)) removed.Add(it);
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
        
        #endregion
    }
}
