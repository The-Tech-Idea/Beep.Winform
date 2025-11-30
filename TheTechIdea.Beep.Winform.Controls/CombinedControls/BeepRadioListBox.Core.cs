using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;

namespace TheTechIdea.Beep.Winform.Controls.CombinedControls
{
    /// <summary>
    /// Core fields, constructor, and initialization for BeepRadioListBox
    /// </summary>
    public partial class BeepRadioListBox
    {
        #region Fields

        // Child controls
        private BeepTextBox _searchBox;
        private BeepRadioGroup _radioGroup;
        private BeepListBox _listBox;
        private Panel _searchPanel;
        private Panel _radioPanel;
        private Panel _listPanel;
        private Panel _dividerPanel;

        // Data
        private BindingList<SimpleItem> _items = new BindingList<SimpleItem>();

        // Layout configuration
        private RadioListBoxLayout _layout = RadioListBoxLayout.RadioAboveList;
        private int _searchBoxHeight = 40;
        private int _radioAreaHeight = 60;
        private int _dividerHeight = 1;
        private int _spacing = 8;
        private bool _showSearch = true;
        private bool _showRadioGroup = true;
        private bool _showListBox = true;
        private bool _showDivider = true;

        // Appearance
        private Color _dividerColor = Color.FromArgb(230, 230, 230);
        private int _searchIconSize = 20;

        // Radio styling
        private RadioGroupRenderStyle _radioStyle = RadioGroupRenderStyle.Chip;
        private RadioGroupOrientation _radioOrientation = RadioGroupOrientation.Horizontal;

        // ListBox styling
        private ListBoxType _listBoxType = ListBoxType.Standard;

        // State
        private bool _isInitialized = false;
        private bool _isSyncing = false;

        #endregion

        #region Constructor

        public BeepRadioListBox() : base()
        {
            // Set default size
            Width = 350;
            Height = 450;

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

                // Create radio panel
                _radioPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = _radioAreaHeight,
                    Padding = new Padding(_spacing, 0, _spacing, _spacing),
                    BackColor = Color.Transparent
                };

                // Create radio group (compact, horizontal by default)
                _radioGroup = new BeepRadioGroup
                {
                    Dock = DockStyle.Fill,
                    IsChild = true,
                    RadioGroupStyle = _radioStyle,
                    Orientation = _radioOrientation,
                    AllowMultipleSelection = false, // Radio = single select
                    ShowShadow = false,
                    BorderThickness = 0,
                    IsRounded = false
                };

                _radioPanel.Controls.Add(_radioGroup);

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

                // Create list box (detailed view)
                _listBox = new BeepListBox
                {
                    Dock = DockStyle.Fill,
                    IsChild = true,
                    ListBoxType = _listBoxType,
                    ShowCheckBox = false, // Radio = single select, no checkboxes
                    MultiSelect = false,
                    SelectionMode = SelectionModeEnum.Single,
                    ShowShadow = false,
                    BorderThickness = 0,
                    IsRounded = false
                };

                _listPanel.Controls.Add(_listBox);

                // Add controls in order (bottom to top for Dock.Top)
                Controls.Add(_listPanel);
                Controls.Add(_dividerPanel);
                Controls.Add(_radioPanel);
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
            // Subscribe to radio group selection changes
            _radioGroup.SelectionChanged += RadioGroup_SelectionChanged;
            
            // Subscribe to list box selection changes
            _listBox.SelectionChanged += ListBox_SelectionChanged;
        }

        #endregion

        #region Event Handlers

        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (_isSyncing || DesignMode) return;

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

        private void RadioGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isSyncing) return;

            try
            {
                _isSyncing = true;
                
                // Sync to ListBox
                var selectedItem = e.SelectedItems?.FirstOrDefault();
                if (selectedItem != null && _listBox != null)
                {
                    _listBox.SelectedItem = selectedItem;
                }
                
                // Raise our own event
                OnSelectionChanged(selectedItem, RadioListBoxSelectionSource.RadioGroup);
            }
            finally
            {
                _isSyncing = false;
            }
        }

        private void ListBox_SelectionChanged(object sender, ListBoxSelectionChangedEventArgs e)
        {
            if (_isSyncing) return;

            try
            {
                _isSyncing = true;
                
                // Sync to RadioGroup
                if (e.CurrentItem != null)
                {
                    _radioGroup.SelectItem(e.CurrentItem.Text);
                }
                
                // Raise our own event
                OnSelectionChanged(e.CurrentItem, RadioListBoxSelectionSource.ListBox);
            }
            finally
            {
                _isSyncing = false;
            }
        }

        #endregion

        #region Data Synchronization

        private void SyncDataToControls()
        {
            if (!_isInitialized) return;

            // Clear existing items
            _listBox.ListItems.Clear();
            _radioGroup.ClearItems();

            // Add items to both controls
            foreach (var item in _items)
            {
                _listBox.ListItems.Add(item);
                _radioGroup.AddItem(item);
            }
        }

        private void ApplySearchFilter()
        {
            if (_listBox == null) return;

            var searchText = _searchBox?.Text?.Trim().ToLower() ?? "";

            if (string.IsNullOrEmpty(searchText))
            {
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
            if (!_isInitialized || DesignMode) return;

            SuspendLayout();

            try
            {
                // Update visibility
                _searchPanel.Visible = _showSearch;
                _radioPanel.Visible = _showRadioGroup;
                _dividerPanel.Visible = _showDivider && _showRadioGroup && _showListBox;
                _listPanel.Visible = _showListBox;

                // Update heights
                _searchPanel.Height = _showSearch ? _searchBoxHeight : 0;
                _radioPanel.Height = _showRadioGroup ? _radioAreaHeight : 0;
                _dividerPanel.Height = (_showDivider && _showRadioGroup && _showListBox) ? _dividerHeight : 0;

                // Reorder controls based on layout
                Controls.Clear();

                switch (_layout)
                {
                    case RadioListBoxLayout.RadioAboveList:
                        Controls.Add(_listPanel);
                        Controls.Add(_dividerPanel);
                        Controls.Add(_radioPanel);
                        Controls.Add(_searchPanel);
                        break;

                    case RadioListBoxLayout.RadioBelowList:
                        Controls.Add(_radioPanel);
                        Controls.Add(_dividerPanel);
                        Controls.Add(_listPanel);
                        Controls.Add(_searchPanel);
                        break;

                    case RadioListBoxLayout.RadioOnly:
                        _listPanel.Visible = false;
                        _dividerPanel.Visible = false;
                        Controls.Add(_radioPanel);
                        Controls.Add(_searchPanel);
                        break;

                    case RadioListBoxLayout.ListOnly:
                        _radioPanel.Visible = false;
                        _dividerPanel.Visible = false;
                        Controls.Add(_listPanel);
                        Controls.Add(_searchPanel);
                        break;
                        
                    case RadioListBoxLayout.Compact:
                        // Compact mode: Radio only, list shows as popup (future enhancement)
                        _listPanel.Visible = false;
                        _dividerPanel.Visible = false;
                        Controls.Add(_radioPanel);
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
            if (_radioPanel != null) _radioPanel.BackColor = panelColor;
            if (_listPanel != null) _listPanel.BackColor = panelColor;

            // Propagate theme to child controls
            var themeName = Theme;

            if (_searchBox != null)
            {
                _searchBox.Theme = themeName;
                _searchBox.ApplyTheme();
            }

            if (_radioGroup != null)
            {
                _radioGroup.Theme = themeName;
                _radioGroup.ApplyTheme();
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

                if (_radioGroup != null)
                {
                    _radioGroup.SelectionChanged -= RadioGroup_SelectionChanged;
                }

                if (_listBox != null)
                {
                    _listBox.SelectionChanged -= ListBox_SelectionChanged;
                }

                // Dispose child controls
                _searchBox?.Dispose();
                _radioGroup?.Dispose();
                _listBox?.Dispose();
                _searchPanel?.Dispose();
                _radioPanel?.Dispose();
                _listPanel?.Dispose();
                _dividerPanel?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// Layout options for the RadioListBox control
    /// </summary>
    public enum RadioListBoxLayout
    {
        /// <summary>Radio group displayed above the list</summary>
        RadioAboveList,
        /// <summary>Radio group displayed below the list</summary>
        RadioBelowList,
        /// <summary>Only radio group is displayed</summary>
        RadioOnly,
        /// <summary>Only list is displayed</summary>
        ListOnly,
        /// <summary>Compact mode - radio group only, list as popup</summary>
        Compact
    }

    /// <summary>
    /// Source of selection change for RadioListBox
    /// </summary>
    public enum RadioListBoxSelectionSource
    {
        RadioGroup,
        ListBox,
        Programmatic
    }

    #endregion

    #region Event Args

    /// <summary>
    /// Event arguments for RadioListBox selection changes
    /// </summary>
    public class RadioListBoxSelectionChangedEventArgs : EventArgs
    {
        public SimpleItem SelectedItem { get; }
        public RadioListBoxSelectionSource Source { get; }

        public RadioListBoxSelectionChangedEventArgs(
            SimpleItem selectedItem,
            RadioListBoxSelectionSource source)
        {
            SelectedItem = selectedItem;
            Source = source;
        }
    }

    /// <summary>
    /// Event arguments for search text changes
    /// </summary>
    public class RadioListBoxSearchEventArgs : EventArgs
    {
        public string SearchText { get; }

        public RadioListBoxSearchEventArgs(string searchText)
        {
            SearchText = searchText;
        }
    }

    #endregion
}

