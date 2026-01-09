using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Chips;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Images;

namespace TheTechIdea.Beep.Winform.Controls.CombinedControls
{
    /// <summary>
    /// Core fields, constructor, and initialization for BeepChipListBox
    /// </summary>
    public partial class BeepChipListBox
    {
        #region Fields

        // Child controls
        private BeepTextBox _searchBox;
        private BeepMultiChipGroup _chipGroup;
        private BeepListBox _listBox;
        private BeepImage _searchIcon;
        private Panel _searchPanel;
        private Panel _chipPanel;
        private Panel _listPanel;
        private Panel _dividerPanel;

        // Synchronization helper
        private SelectionSyncHelper _syncHelper;

        // Data
        private BindingList<SimpleItem> _items = new BindingList<SimpleItem>();

        // Layout configuration
        private ChipListBoxLayout _layout = ChipListBoxLayout.ChipsAboveList;
        private int _searchBoxHeight = 40;
        private int _chipAreaHeight = 80;
        private int _dividerHeight = 1;
        private int _spacing = 8;
        private bool _showSearch = true;
        private bool _showChips = true;
        private bool _showList = true;
        private bool _showDivider = true;

        // Appearance
        private Color _dividerColor = Color.FromArgb(230, 230, 230);
        private Color _searchBoxBackColor = Color.White;
        private int _searchIconSize = 20;

        // Selection
        private SelectionModeEnum _selectionMode = SelectionModeEnum.MultiExtended;
        private bool _allowMultiSelect = true;

        // Chip styling
        private ChipStyle _chipStyle = ChipStyle.Default;
        private ChipSize _chipSize = ChipSize.Medium;

        // ListBox styling
        private ListBoxType _listBoxType = ListBoxType.Standard;

        // State
        private bool _isInitialized = false;
        private bool _isSyncing = false;

        #endregion

        #region Constructor

        public BeepChipListBox() : base()
        {
            // Set default size
            Width = 300;
            Height = 400;

            // Base control settings
            DoubleBuffered = true;
            IsRounded = true;
            BorderThickness = 1;
            ShowShadow = false;
            BorderRadius = 8;

            // Initialize components
            InitializeComponents();

            // Setup data binding
            _items.ListChanged += Items_ListChanged;
        }

        #endregion

        #region Initialization

        private void InitializeComponents()
        {
            SuspendLayout();

            try
            {
                // Create search panel with icon
                _searchPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = _searchBoxHeight,
                    Padding = new Padding(_spacing),
                    BackColor = Color.Transparent
                };

                // Create search icon
                _searchIcon = new BeepImage
                {
                    Size = new Size(_searchIconSize, _searchIconSize),
                    ImagePath = SvgsUI.Search,
                    ApplyThemeOnImage = true,
                    IsChild = true,
                    BackColor = Color.Transparent
                };

                // Create search box
                _searchBox = new BeepTextBox
                {
                    Dock = DockStyle.Fill,
                    PlaceholderText = "Search...",
                    IsChild = true,
                    BorderRadius = 6,
                    ImageVisible = true,
                    ImagePath = SvgsUI.Search,
                    TextImageRelation = TextImageRelation.ImageBeforeText,
                    MaxImageSize = new Size(_searchIconSize, _searchIconSize),
                    ApplyThemeOnImage = true
                };
                _searchBox.TextChanged += SearchBox_TextChanged;

                _searchPanel.Controls.Add(_searchBox);

                // Create chip panel
                _chipPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = _chipAreaHeight,
                    Padding = new Padding(_spacing, 0, _spacing, _spacing),
                    BackColor = Color.Transparent,
                    AutoScroll = true
                };

                // Create chip group
                _chipGroup = new BeepMultiChipGroup
                {
                    Dock = DockStyle.Fill,
                    IsChild = true,
                    TitleText = "",
                    ChipStyle = _chipStyle,
                    ChipSize = _chipSize,
                    SelectionMode = ChipSelectionMode.Multiple,
                    ShowShadow = false,
                    BorderThickness = 0,
                    IsRounded = false
                };

                _chipPanel.Controls.Add(_chipGroup);

                // Create divider
                _dividerPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = _dividerHeight,
                    BackColor = _dividerColor
                };

                // Create list panel
                _listPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(_spacing),
                    BackColor = Color.Transparent
                };

                // Create list box
                _listBox = new BeepListBox
                {
                    Dock = DockStyle.Fill,
                    IsChild = true,
                    ListBoxType = _listBoxType,
                    ShowCheckBox = true,
                    MultiSelect = true,
                    SelectionMode = _selectionMode,
                    ShowShadow = false,
                    BorderThickness = 0,
                    IsRounded = false
                };

                _listPanel.Controls.Add(_listBox);

                // Add controls in order (bottom to top for Dock.Top)
                Controls.Add(_listPanel);
                Controls.Add(_dividerPanel);
                Controls.Add(_chipPanel);
                Controls.Add(_searchPanel);

                // Setup synchronization
                SetupSynchronization();

                _isInitialized = true;
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        private void SetupSynchronization()
        {
            // Create sync helper for bidirectional synchronization
            _syncHelper = new SelectionSyncHelper(_listBox, _chipGroup);
            _syncHelper.IsEnabled = true;
            _syncHelper.SyncListBoxToChipGroup = true;
            _syncHelper.SyncChipGroupToListBox = true;

            // Subscribe to events for external notification
            _listBox.SelectionChanged += ListBox_SelectionChanged;
            _chipGroup.SelectionChanged += ChipGroup_SelectionChanged;
            _chipGroup.ChipRemoved += ChipGroup_ChipRemoved;
        }

        #endregion

        #region Event Handlers

        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_isSyncing) return;

            try
            {
                _isSyncing = true;
                SyncDataToControls();
            }
            finally
            {
                _isSyncing = false;
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            ApplySearchFilter();
            OnSearchTextChanged();
        }

        private void ListBox_SelectionChanged(object sender, ListBoxSelectionChangedEventArgs e)
        {
            if (_isSyncing) return;

            // Raise our own event
            OnSelectionChanged(e.CurrentItem, e.SelectedItems.ToList(), SelectionChangeSource.ListBox);
        }

        private void ChipGroup_SelectionChanged(object sender, ChipSelectionChangedEventArgs e)
        {
            if (_isSyncing) return;

            // Raise our own event
            OnSelectionChanged(e.SelectedItem, e.SelectedItems.ToList(), SelectionChangeSource.ChipGroup);
        }

        private void ChipGroup_ChipRemoved(object sender, ChipClickedEventArgs e)
        {
            // When a chip is removed, update selection
            if (e.Item != null)
            {
                _listBox.SetItemCheckbox(e.Item, false);
                _listBox.RemoveFromSelection(e.Item);
            }

            OnChipRemoved(e.Item);
        }

        #endregion

        #region Data Synchronization

        private void SyncDataToControls()
        {
            if (!_isInitialized) return;

            // Clear existing items
            _listBox.ListItems.Clear();
            _chipGroup.ListItems.Clear();

            // Add items to both controls
            foreach (var item in _items)
            {
                _listBox.ListItems.Add(item);
                _chipGroup.ListItems.Add(item);
            }
        }

        private void ApplySearchFilter()
        {
            if (_listBox == null) return;

            var searchText = _searchBox?.Text?.Trim().ToLower() ?? "";

            if (string.IsNullOrEmpty(searchText))
            {
                // Show all items
                foreach (var item in _items)
                {
                    // Reset visibility in ListBox if it supports filtering
                    // For now, we use the ListBox's built-in search
                }
                _listBox.SearchText = "";
            }
            else
            {
                _listBox.SearchText = searchText;
            }
        }

        #endregion

        #region Layout

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            if (!_isInitialized) return;

            SuspendLayout();

            try
            {
                // Update visibility
                _searchPanel.Visible = _showSearch;
                _chipPanel.Visible = _showChips;
                _dividerPanel.Visible = _showDivider && _showChips && _showList;
                _listPanel.Visible = _showList;

                // Update heights
                _searchPanel.Height = _showSearch ? _searchBoxHeight : 0;
                _chipPanel.Height = _showChips ? _chipAreaHeight : 0;
                _dividerPanel.Height = (_showDivider && _showChips && _showList) ? _dividerHeight : 0;

                // Reorder controls based on layout
                Controls.Clear();

                switch (_layout)
                {
                    case ChipListBoxLayout.ChipsAboveList:
                        Controls.Add(_listPanel);
                        Controls.Add(_dividerPanel);
                        Controls.Add(_chipPanel);
                        Controls.Add(_searchPanel);
                        break;

                    case ChipListBoxLayout.ChipsBelowList:
                        Controls.Add(_chipPanel);
                        Controls.Add(_dividerPanel);
                        Controls.Add(_listPanel);
                        Controls.Add(_searchPanel);
                        break;

                    case ChipListBoxLayout.ChipsOnly:
                        _listPanel.Visible = false;
                        _dividerPanel.Visible = false;
                        Controls.Add(_chipPanel);
                        Controls.Add(_searchPanel);
                        break;

                    case ChipListBoxLayout.ListOnly:
                        _chipPanel.Visible = false;
                        _dividerPanel.Visible = false;
                        Controls.Add(_listPanel);
                        Controls.Add(_searchPanel);
                        break;
                }
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        #endregion

        #region Theme

        /// <summary>
        /// Applies the current theme to this control and all child controls
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null) return;

            // Update divider color from theme
            _dividerColor = _currentTheme.BorderColor;
            if (_dividerPanel != null)
            {
                _dividerPanel.BackColor = _dividerColor;
            }

            // Update panel backgrounds from theme
            var panelColor = _currentTheme.PanelBackColor;
            if (_searchPanel != null) _searchPanel.BackColor = panelColor;
            if (_chipPanel != null) _chipPanel.BackColor = panelColor;
            if (_listPanel != null) _listPanel.BackColor = panelColor;

            // Propagate theme to child controls
            var themeName = Theme;

            if (_searchBox != null)
            {
                _searchBox.Theme = themeName;
                _searchBox.ApplyTheme();
            }

            if (_searchIcon != null)
            {
                _searchIcon.Theme = themeName;
                _searchIcon.ApplyTheme();
            }

            if (_chipGroup != null)
            {
                _chipGroup.Theme = themeName;
                _chipGroup.ApplyTheme();
            }

            if (_listBox != null)
            {
                _listBox.Theme = themeName;
                _listBox.ApplyTheme();
            }

            Invalidate();
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Unsubscribe from events
                if (_items != null)
                {
                    _items.ListChanged -= Items_ListChanged;
                }

                if (_searchBox != null)
                {
                    _searchBox.TextChanged -= SearchBox_TextChanged;
                }

                if (_listBox != null)
                {
                    _listBox.SelectionChanged -= ListBox_SelectionChanged;
                }

                if (_chipGroup != null)
                {
                    _chipGroup.SelectionChanged -= ChipGroup_SelectionChanged;
                    _chipGroup.ChipRemoved -= ChipGroup_ChipRemoved;
                }

                // Dispose sync helper
                _syncHelper?.Dispose();

                // Dispose child controls
                _searchBox?.Dispose();
                _chipGroup?.Dispose();
                _listBox?.Dispose();
                _searchIcon?.Dispose();
                _searchPanel?.Dispose();
                _chipPanel?.Dispose();
                _listPanel?.Dispose();
                _dividerPanel?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// Layout options for the ChipListBox control
    /// </summary>
    public enum ChipListBoxLayout
    {
        /// <summary>Chips displayed above the list</summary>
        ChipsAboveList,
        /// <summary>Chips displayed below the list</summary>
        ChipsBelowList,
        /// <summary>Only chips are displayed</summary>
        ChipsOnly,
        /// <summary>Only list is displayed</summary>
        ListOnly
    }

    /// <summary>
    /// Source of selection change
    /// </summary>
    public enum SelectionChangeSource
    {
        ListBox,
        ChipGroup,
        Programmatic
    }

    #endregion

    #region Event Args

    /// <summary>
    /// Event arguments for ChipListBox selection changes
    /// </summary>
    public class ChipListBoxSelectionChangedEventArgs : EventArgs
    {
        public SimpleItem CurrentItem { get; }
        public IReadOnlyList<SimpleItem> SelectedItems { get; }
        public SelectionChangeSource Source { get; }

        public ChipListBoxSelectionChangedEventArgs(
            SimpleItem currentItem,
            IReadOnlyList<SimpleItem> selectedItems,
            SelectionChangeSource source)
        {
            CurrentItem = currentItem;
            SelectedItems = selectedItems;
            Source = source;
        }
    }

    /// <summary>
    /// Event arguments for search text changes
    /// </summary>
    public class ChipListBoxSearchEventArgs : EventArgs
    {
        public string SearchText { get; }

        public ChipListBoxSearchEventArgs(string searchText)
        {
            SearchText = searchText;
        }
    }

    #endregion
}

