using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Chips;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.CombinedControls
{
    /// <summary>
    /// Properties for BeepChipListBox
    /// </summary>
    public partial class BeepChipListBox
    {
        #region Style Coordination Fields

        private ChipListBoxStylePreset _stylePreset = ChipListBoxStylePreset.Default;
        private bool _coordinateStyles = true;
        private StyleCoordinationMode _coordinationMode = StyleCoordinationMode.Automatic;

        #endregion

        #region Style Coordination Properties

        /// <summary>
        /// Gets or sets the unified style preset that coordinates both ListBox and Chip styles
        /// Setting this will automatically update both ListBoxType and ChipStyle
        /// </summary>
        [Browsable(true)]
        [Category("Style")]
        [Description("Unified style preset that coordinates both ListBox and Chip styles")]
        [DefaultValue(ChipListBoxStylePreset.Default)]
        public ChipListBoxStylePreset StylePreset
        {
            get => _stylePreset;
            set
            {
                if (_stylePreset != value)
                {
                    _stylePreset = value;
                    ApplyStylePreset(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether styles should be automatically coordinated
        /// When true, changing ListBoxType will update ChipStyle and vice versa
        /// </summary>
        [Browsable(true)]
        [Category("Style")]
        [Description("Automatically coordinate ListBox and Chip styles when one changes")]
        [DefaultValue(true)]
        public bool CoordinateStyles
        {
            get => _coordinateStyles;
            set => _coordinateStyles = value;
        }

        /// <summary>
        /// Gets or sets the style coordination mode
        /// </summary>
        [Browsable(true)]
        [Category("Style")]
        [Description("How styles are coordinated between ListBox and ChipGroup")]
        [DefaultValue(StyleCoordinationMode.Automatic)]
        public StyleCoordinationMode CoordinationMode
        {
            get => _coordinationMode;
            set
            {
                if (_coordinationMode != value)
                {
                    _coordinationMode = value;
                    if (value == StyleCoordinationMode.Automatic)
                    {
                        SyncStylesFromListBox();
                    }
                }
            }
        }

        #endregion

        #region Style Coordination Methods

        /// <summary>
        /// Applies a unified style preset to both ListBox and ChipGroup
        /// </summary>
        private void ApplyStylePreset(ChipListBoxStylePreset preset)
        {
            var (listStyle, chipStyle) = ChipListBoxStyleCoordinator.GetCoordinatedStyles(preset);

            // Temporarily disable coordination to prevent loops
            var wasCoordinating = _coordinateStyles;
            _coordinateStyles = false;

            try
            {
                _listBoxType = listStyle;
                _chipStyle = chipStyle;

                if (_listBox != null)
                {
                    _listBox.ListBoxType = listStyle;
                    
                    // Apply recommended settings based on style
                    _listBox.ShowCheckBox = ChipListBoxStyleCoordinator.ShouldShowCheckboxes(listStyle);
                    _listBox.ShowImage = ChipListBoxStyleCoordinator.ShouldShowImages(listStyle);
                }

                if (_chipGroup != null)
                {
                    _chipGroup.ChipStyle = chipStyle;
                    _chipGroup.ChipSize = ChipListBoxStyleCoordinator.GetChipSizeForItemHeight(_listBox?.MenuItemHeight ?? 32);
                }

                // Update search visibility based on style
                if (ChipListBoxStyleCoordinator.ShouldShowSearch(listStyle))
                {
                    ShowSearch = true;
                }
            }
            finally
            {
                _coordinateStyles = wasCoordinating;
            }

            Invalidate();
        }

        /// <summary>
        /// Synchronizes ChipStyle based on current ListBoxType
        /// </summary>
        private void SyncStylesFromListBox()
        {
            if (!_coordinateStyles || _coordinationMode == StyleCoordinationMode.Manual)
                return;

            var recommendedChipStyle = ChipListBoxStyleCoordinator.GetChipStyleFor(_listBoxType);
            
            if (_chipGroup != null && _chipGroup.ChipStyle != recommendedChipStyle)
            {
                _chipStyle = recommendedChipStyle;
                _chipGroup.ChipStyle = recommendedChipStyle;
            }
        }

        /// <summary>
        /// Synchronizes ListBoxType based on current ChipStyle
        /// </summary>
        private void SyncStylesFromChipGroup()
        {
            if (!_coordinateStyles || _coordinationMode == StyleCoordinationMode.Manual)
                return;

            var recommendedListType = ChipListBoxStyleCoordinator.GetListBoxTypeFor(_chipStyle);
            
            if (_listBox != null && _listBox.ListBoxType != recommendedListType)
            {
                _listBoxType = recommendedListType;
                _listBox.ListBoxType = recommendedListType;
            }
        }

        #endregion

        #region Data Properties

        /// <summary>
        /// Gets or sets the collection of items displayed in both the list and chip group
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The collection of items displayed in both the list and chip group")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Localizable(true)]
        [MergableProperty(false)]
        public BindingList<SimpleItem> Items
        {
            get => _items;
            set
            {
                if (_items != null)
                {
                    _items.ListChanged -= Items_ListChanged;
                }

                _items = value ?? new BindingList<SimpleItem>();
                _items.ListChanged += Items_ListChanged;

                SyncDataToControls();
            }
        }

        /// <summary>
        /// Gets the currently selected item
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SimpleItem SelectedItem
        {
            get => _listBox?.SelectedItem ?? _chipGroup?.SelectedItem;
            set
            {
                if (_listBox != null)
                {
                    _listBox.SelectedItem = value;
                }
            }
        }

        /// <summary>
        /// Gets the list of selected items
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<SimpleItem> SelectedItems
        {
            get
            {
                if (_listBox != null)
                {
                    return _listBox.SelectedItems;
                }
                if (_chipGroup != null)
                {
                    return _chipGroup.SelectedItems.ToList();
                }
                return new List<SimpleItem>();
            }
        }

        /// <summary>
        /// Gets or sets the search text
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The current search text")]
        [DefaultValue("")]
        public string SearchText
        {
            get => _searchBox?.Text ?? "";
            set
            {
                if (_searchBox != null)
                {
                    _searchBox.Text = value ?? "";
                }
            }
        }

        #endregion

        #region Layout Properties

        /// <summary>
        /// Gets or sets the layout arrangement of chips and list
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The layout arrangement of chips and list")]
        [DefaultValue(ChipListBoxLayout.ChipsAboveList)]
        public ChipListBoxLayout Layout
        {
            get => _layout;
            set
            {
                if (_layout != value)
                {
                    _layout = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the search box is visible
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether the search box is visible")]
        [DefaultValue(true)]
        public bool ShowSearch
        {
            get => _showSearch;
            set
            {
                if (_showSearch != value)
                {
                    _showSearch = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the chip group is visible
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether the chip group is visible")]
        [DefaultValue(true)]
        public bool ShowChips
        {
            get => _showChips;
            set
            {
                if (_showChips != value)
                {
                    _showChips = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the list box is visible
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether the list box is visible")]
        [DefaultValue(true)]
        public bool ShowList
        {
            get => _showList;
            set
            {
                if (_showList != value)
                {
                    _showList = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the divider between chips and list is visible
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether the divider between chips and list is visible")]
        [DefaultValue(true)]
        public bool ShowDivider
        {
            get => _showDivider;
            set
            {
                if (_showDivider != value)
                {
                    _showDivider = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the search box area
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The height of the search box area")]
        [DefaultValue(40)]
        public int SearchBoxHeight
        {
            get => _searchBoxHeight;
            set
            {
                if (_searchBoxHeight != value && value > 0)
                {
                    _searchBoxHeight = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the chip area
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The height of the chip area")]
        [DefaultValue(80)]
        public int ChipAreaHeight
        {
            get => _chipAreaHeight;
            set
            {
                if (_chipAreaHeight != value && value > 0)
                {
                    _chipAreaHeight = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the spacing between elements
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The spacing between elements")]
        [DefaultValue(8)]
        public int Spacing
        {
            get => _spacing;
            set
            {
                if (_spacing != value && value >= 0)
                {
                    _spacing = value;
                    UpdateLayout();
                }
            }
        }

        #endregion

        #region Appearance Properties

        /// <summary>
        /// Gets or sets the color of the divider
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The color of the divider between chips and list")]
        public Color DividerColor
        {
            get => _dividerColor;
            set
            {
                if (_dividerColor != value)
                {
                    _dividerColor = value;
                    if (_dividerPanel != null)
                    {
                        _dividerPanel.BackColor = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the placeholder text for the search box
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The placeholder text for the search box")]
        [DefaultValue("Search...")]
        public string SearchPlaceholderText
        {
            get => _searchBox?.PlaceholderText ?? "Search...";
            set
            {
                if (_searchBox != null)
                {
                    _searchBox.PlaceholderText = value ?? "Search...";
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the search icon
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The size of the search icon")]
        [DefaultValue(20)]
        public int SearchIconSize
        {
            get => _searchIconSize;
            set
            {
                if (_searchIconSize != value && value > 0)
                {
                    _searchIconSize = value;
                    if (_searchBox != null)
                    {
                        _searchBox.MaxImageSize = new Size(value, value);
                    }
                }
            }
        }

        #endregion

        #region Selection Properties

        /// <summary>
        /// Gets or sets whether multiple selection is allowed
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether multiple selection is allowed")]
        [DefaultValue(true)]
        public bool AllowMultiSelect
        {
            get => _allowMultiSelect;
            set
            {
                if (_allowMultiSelect != value)
                {
                    _allowMultiSelect = value;

                    if (_listBox != null)
                    {
                        _listBox.MultiSelect = value;
                        _listBox.SelectionMode = value ? SelectionModeEnum.MultiExtended : SelectionModeEnum.Single;
                    }

                    if (_chipGroup != null)
                    {
                        _chipGroup.SelectionMode = value ? ChipSelectionMode.Multiple : ChipSelectionMode.Single;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets whether checkboxes are shown in the list
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether checkboxes are shown in the list")]
        [DefaultValue(true)]
        public bool ShowCheckBoxes
        {
            get => _listBox?.ShowCheckBox ?? true;
            set
            {
                if (_listBox != null)
                {
                    _listBox.ShowCheckBox = value;
                }
            }
        }

        #endregion

        #region Chip Properties

        /// <summary>
        /// Gets or sets the visual style of chips
        /// When CoordinateStyles is true, this will also update ListBoxType
        /// </summary>
        [Browsable(true)]
        [Category("Chip Style")]
        [Description("The visual style of chips")]
        [DefaultValue(ChipStyle.Default)]
        public ChipStyle ChipStyle
        {
            get => _chipStyle;
            set
            {
                if (_chipStyle != value)
                {
                    _chipStyle = value;
                    if (_chipGroup != null)
                    {
                        _chipGroup.ChipStyle = value;
                    }

                    // Coordinate list style if enabled
                    if (_coordinateStyles && _coordinationMode != StyleCoordinationMode.ListLeads)
                    {
                        SyncStylesFromChipGroup();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of chips
        /// </summary>
        [Browsable(true)]
        [Category("Chip Style")]
        [Description("The size of chips")]
        [DefaultValue(ChipSize.Medium)]
        public ChipSize ChipSize
        {
            get => _chipSize;
            set
            {
                if (_chipSize != value)
                {
                    _chipSize = value;
                    if (_chipGroup != null)
                    {
                        _chipGroup.ChipSize = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the title text for the chip group
        /// </summary>
        [Browsable(true)]
        [Category("Chip Style")]
        [Description("The title text for the chip group")]
        [DefaultValue("")]
        public string ChipGroupTitle
        {
            get => _chipGroup?.TitleText ?? "";
            set
            {
                if (_chipGroup != null)
                {
                    _chipGroup.TitleText = value ?? "";
                }
            }
        }

        #endregion

        #region ListBox Properties

        /// <summary>
        /// Gets or sets the visual type of the list box
        /// When CoordinateStyles is true, this will also update ChipStyle
        /// </summary>
        [Browsable(true)]
        [Category("List Style")]
        [Description("The visual type of the list box")]
        [DefaultValue(ListBoxType.Standard)]
        public ListBoxType ListBoxType
        {
            get => _listBoxType;
            set
            {
                if (_listBoxType != value)
                {
                    _listBoxType = value;
                    if (_listBox != null)
                    {
                        _listBox.ListBoxType = value;
                    }

                    // Coordinate chip style if enabled
                    if (_coordinateStyles && _coordinationMode != StyleCoordinationMode.ChipLeads)
                    {
                        SyncStylesFromListBox();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of each list item
        /// </summary>
        [Browsable(true)]
        [Category("List Style")]
        [Description("The height of each list item")]
        [DefaultValue(32)]
        public int ListItemHeight
        {
            get => _listBox?.MenuItemHeight ?? 32;
            set
            {
                if (_listBox != null && value > 0)
                {
                    _listBox.MenuItemHeight = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether images are shown in list items
        /// </summary>
        [Browsable(true)]
        [Category("List Style")]
        [Description("Whether images are shown in list items")]
        [DefaultValue(true)]
        public bool ShowListImages
        {
            get => _listBox?.ShowImage ?? true;
            set
            {
                if (_listBox != null)
                {
                    _listBox.ShowImage = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the empty state text for the list
        /// </summary>
        [Browsable(true)]
        [Category("List Style")]
        [Description("The text shown when the list is empty")]
        [DefaultValue("No items")]
        public string EmptyStateText
        {
            get => _listBox?.EmptyStateText ?? "No items";
            set
            {
                if (_listBox != null)
                {
                    _listBox.EmptyStateText = value ?? "No items";
                }
            }
        }

        #endregion

        #region Child Control Access

        /// <summary>
        /// Gets direct access to the internal BeepListBox control
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepListBox ListBox => _listBox;

        /// <summary>
        /// Gets direct access to the internal BeepMultiChipGroup control
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepMultiChipGroup ChipGroup => _chipGroup;

        /// <summary>
        /// Gets direct access to the internal BeepTextBox search control
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BeepTextBox SearchBox => _searchBox;

        #endregion
    }
}

