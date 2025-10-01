using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Chips;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// A customizable select control that provides single and multiple selection capabilities with dropdown functionality.
    /// Supports chips display for multiple selections, search functionality, keyboard navigation, and data binding.
    /// </summary>
    [ToolboxItem(true)]
    [Category("Data")]
    [Description("A select control that displays multiple data in a simple table format to select from")]
    [DisplayName("Beep Select")]
    public class BeepSelect : BeepControl
    {
        #region Fields
        private BeepPopupListForm _popupListForm;
        private bool _isDropdownOpen = false;
        private SimpleItem _selectedItem;
        private List<SimpleItem> _selectedItems = new List<SimpleItem>();
        private BindingList<SimpleItem> _items = new BindingList<SimpleItem>();
        private int _dropdownHeight = 200;
        private bool _showImage = false;
        private bool _useSimpleItemText = true;
        private string _placeholderText = "Select an item...";
        private bool _isReadOnly = false;
        private int _dropdownOffset = 0;
        private BeepButton _dropdownButton;
        private bool _showSearchInDropdown = false;
        private string _dropdownTitle = "";
        private BeepMultiChipGroup _chipGroup;
        private bool _allowMultipleSelection = false;
        private int _chipHeight = 24;
        private int _maxDisplayChips = 3;
        private bool _showChipCount = true;
        private int _minHeight = 32;
        private bool _showGroupHeaders = false;
        private bool _enableAutoComplete = false;
        private string _autoCompleteText = string.Empty;
        private System.Windows.Forms.Timer _autoCompleteTimer;
        private bool _isRequired = false;
        private string _validationError = string.Empty;
        private bool _autoSizeDropdown = true;
        private string _displayMember;
        private string _valueMember;
        private object _dataSource;
        private Predicate<SimpleItem> _filter;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the selected item in single selection mode changes.
        /// </summary>
        public event EventHandler SelectedItemChanged;

        /// <summary>
        /// Occurs when the selected items collection in multi-selection mode changes.
        /// </summary>
        public event EventHandler SelectedItemsChanged;

        /// <summary>
        /// Occurs when validation is performed on the control.
        /// </summary>
        public event EventHandler<ValidationEventArgs> Validating;

        /// <summary>
        /// Occurs when a dropdown is about to be displayed, allowing for customization of items.
        /// </summary>
        public event EventHandler<DropdownOpeningEventArgs> DropdownOpening;

        /// <summary>
        /// Occurs when the dropdown is closed.
        /// </summary>
        public event EventHandler DropdownClosed;

        /// <summary>
        /// Occurs when items are requested for dynamic loading.
        /// </summary>
        public event EventHandler<ItemsRequestEventArgs> ItemsRequested;
        #endregion

        #region Properties - Behavior
        /// <summary>
        /// Gets or sets whether auto-complete is enabled when typing.
        /// When enabled, typing characters will select the first matching item.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Enable auto-complete when typing")]
        public bool EnableAutoComplete
        {
            get => _enableAutoComplete;
            set
            {
                _enableAutoComplete = value;
                if (value && _autoCompleteTimer == null)
                {
                    _autoCompleteTimer = new System.Windows.Forms.Timer { Interval = 500 };
                    _autoCompleteTimer.Tick += AutoCompleteTimer_Tick;
                }
                else if (!value && _autoCompleteTimer != null)
                {
                    _autoCompleteTimer.Dispose();
                    _autoCompleteTimer = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether group headers are shown when items have category information.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show group headers if SimpleItems have category/group information")]
        public bool ShowGroupHeaders
        {
            get => _showGroupHeaders;
            set
            {
                _showGroupHeaders = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether multiple items can be selected.
        /// When enabled, selected items appear as chips within the control.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allows the selection of multiple items")]
        public bool AllowMultipleSelection
        {
            get => _allowMultipleSelection;
            set
            {
                _allowMultipleSelection = value;
                if (!DesignMode)
                {
                    UpdateChipVisibility();
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether the control is in read-only mode.
        /// When in read-only mode, the dropdown cannot be opened and selections cannot be changed.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Indicates whether the control is in read-only mode")]
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                _isReadOnly = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether to use the Text property of SimpleItem for display.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether to use the Text property of SimpleItem for display")]
        public bool UseSimpleItemText
        {
            get => _useSimpleItemText;
            set
            {
                _useSimpleItemText = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether to show a search box in the dropdown.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether to show a search box in the dropdown")]
        public bool ShowSearchInDropdown
        {
            get => _showSearchInDropdown;
            set
            {
                _showSearchInDropdown = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether selection is required for validation.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether a selection is required for validation")]
        public bool IsRequired
        {
            get => _isRequired;
            set
            {
                _isRequired = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether the dropdown width is automatically adjusted based on content.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Automatically size the dropdown width based on content")]
        public bool AutoSizeDropdown
        {
            get => _autoSizeDropdown;
            set
            {
                _autoSizeDropdown = value;
                Invalidate();
            }
        }
        #endregion

        #region Properties - Appearance
        /// <summary>
        /// Gets or sets the height of each chip when multiple items are selected.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Height of each chip when multiple items are selected")]
        public int ChipHeight
        {
            get => _chipHeight;
            set
            {
                _chipHeight = value;
                if (!DesignMode && _chipGroup != null)
                {
                    UpdateChipGroupAppearance();
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of chips to display before showing a count indicator.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Maximum number of chips to display before showing a count")]
        public int MaxDisplayChips
        {
            get => _maxDisplayChips;
            set
            {
                _maxDisplayChips = value;
                if (!DesignMode)
                {
                    UpdateChipVisibility();
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether to show a count badge when more items are selected than can be displayed.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show count badge when more items are selected than can be displayed")]
        public bool ShowChipCount
        {
            get => _showChipCount;
            set
            {
                _showChipCount = value;
                if (!DesignMode)
                {
                    UpdateChipVisibility();
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the text to display when no item is selected.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text to display when no item is selected")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether to show images for items.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether to show images for items")]
        public bool ShowImage
        {
            get => _showImage;
            set
            {
                _showImage = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the height of the dropdown list.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Height of the dropdown list")]
        public int DropdownHeight
        {
            get => _dropdownHeight;
            set
            {
                _dropdownHeight = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the vertical offset of the dropdown list from the control.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Vertical offset of the dropdown list")]
        public int DropdownOffset
        {
            get => _dropdownOffset;
            set
            {
                _dropdownOffset = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the title to display in the dropdown.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Title to display in the dropdown")]
        public string DropdownTitle
        {
            get => _dropdownTitle;
            set
            {
                _dropdownTitle = value;
                Invalidate();
            }
        }
        #endregion

        #region Properties - Data
        /// <summary>
        /// Gets or sets the list of items available for selection.
        /// </summary>
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        [Description("The list of items available for selection")]
        public BindingList<SimpleItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected item in single selection mode.
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

                    if (!DesignMode)
                    {
                        if (!AllowMultipleSelection)
                        {
                            // Single select mode
                            _selectedItems.Clear();
                            if (_selectedItem != null)
                            {
                                _selectedItems.Add(_selectedItem);
                            }
                        }
                        else if (_selectedItem != null)
                        {
                            // Multi-select mode: toggle selection
                            if (_selectedItems.Contains(_selectedItem))
                            {
                                _selectedItems.Remove(_selectedItem);
                            }
                            else
                            {
                                _selectedItems.Add(_selectedItem);
                            }
                        }

                        UpdateChipDisplay();
                        OnSelectedItemChanged();
                    }
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the collection of selected items in multiple selection mode.
        /// </summary>
        [Browsable(false)]
        public List<SimpleItem> SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value ?? new List<SimpleItem>();
                _selectedItem = _selectedItems.FirstOrDefault();
                if (!DesignMode)
                {
                    UpdateChipDisplay();
                    OnSelectedItemChanged();
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the index of the selected item in the Items collection.
        /// </summary>
        [Browsable(false)]
        public int SelectedIndex
        {
            get
            {
                if (_selectedItem == null)
                    return -1;
                return _items.IndexOf(_selectedItem);
            }
            set
            {
                if (value >= 0 && value < _items.Count)
                {
                    SelectedItem = _items[value];
                }
                else
                {
                    SelectedItem = null;
                }
            }
        }

        /// <summary>
        /// Gets the validation error message if validation fails.
        /// </summary>
        [Browsable(false)]
        public string ValidationError
        {
            get => _validationError;
            private set => _validationError = value;
        }

        /// <summary>
        /// Gets or sets the data source for the control.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The data source for the control")]
        [AttributeProvider(typeof(IListSource))]
        public object DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                if (!DesignMode && value != null)
                {
                    LoadItemsFromDataSource();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the property to display for each item.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The property to display for each item")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", typeof(UITypeEditor))]
        public string DisplayMember
        {
            get => _displayMember;
            set
            {
                _displayMember = value;
                if (!DesignMode && _dataSource != null)
                {
                    LoadItemsFromDataSource();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the property to use as the value for each item.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The property to use as the value for each item")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", typeof(UITypeEditor))]
        public string ValueMember
        {
            get => _valueMember;
            set
            {
                _valueMember = value;
                if (!DesignMode && _dataSource != null)
                {
                    LoadItemsFromDataSource();
                }
            }
        }

        /// <summary>
        /// Gets or sets a filter predicate for the items.
        /// </summary>
        [Browsable(false)]
        public Predicate<SimpleItem> ItemFilter
        {
            get => _filter;
            set
            {
                _filter = value;
                if (!DesignMode)
                {
                    ApplyFilter();
                }
            }
        }
        #endregion

        #region Constructor and Initialization
        /// <summary>
        /// Initializes a new instance of the BeepSelect control.
        /// </summary>
        public BeepSelect()
        {
            // Default settings
            Width = 200;
            Height = 32;
            MinimumSize = new Size(0, _minHeight);
            BackColor = Color.White;
            ForeColor = Color.Black;
            BorderRadius = 4;
            ShowAllBorders = true;
            BorderColor = Color.LightGray;
            BorderThickness = 1;
            IsRounded = true;

            // Set up data binding properties
            BoundProperty = "SelectedItem";

            try
            {
                // Initialize controls with proper design-time handling
                if (!DesignMode)
                {
                    // Initialize the chip group for multi-select display
                    InitializeChipGroup();
                }

                // Initialize the dropdown button (safe in both runtime and design-time)
                InitializeDropdownButton();

                // Register event handlers
                Click += BeepSelect_Click;
                LostFocus += BeepSelect_LostFocus;
                SizeChanged += BeepSelect_SizeChanged;
            }
            catch (Exception ex)
            {
                // Suppress exceptions during design-time initialization
                if (!DesignMode)
                {
                    throw; // Re-throw in runtime
                }
                // In design mode, log the error but don't crash
                System.Diagnostics.Debug.WriteLine($"BeepSelect initialization error: {ex.Message}");
            }
        }

        /// <summary>
        /// Initializes the chip group for multi-select display.
        /// </summary>
        private void InitializeChipGroup()
        {
            // Skip in design mode
            if (DesignMode)
                return;

            _chipGroup = new BeepMultiChipGroup
            {
                Visible = false,
                Dock = DockStyle.None,
                AutoSize = true,
                BorderThickness = 0,
                ShowAllBorders = false,
                IsRounded = true,
                Margin = new Padding(4, 4, 25, 4), // Leave space for dropdown button
            };

            Controls.Add(_chipGroup);
            UpdateChipGroupAppearance();
        }

        /// <summary>
        /// Updates the appearance of the chip group.
        /// </summary>
        private void UpdateChipGroupAppearance()
        {
            if (_chipGroup == null || DesignMode)
                return;

            _chipGroup.Theme = Theme;
            _chipGroup.BackColor = BackColor;
            _chipGroup.IsFrameless = true;
            _chipGroup.BorderThickness = 0;
            _chipGroup.ShowShadow = false;
            _chipGroup.Padding = new Padding(2, 2, 2, 2);

            // Position and size the chip group
            UpdateChipGroupLayout();
        }

        /// <summary>
        /// Updates the layout of the chip group.
        /// </summary>
        private void UpdateChipGroupLayout()
        {
            if (_chipGroup == null || DesignMode)
                return;

            // Position chip group
            _chipGroup.Location = new Point(5, 5);
            _chipGroup.Width = Width - _dropdownButton.Width - 10;
        }

        /// <summary>
        /// Initializes the dropdown button.
        /// </summary>
        private void InitializeDropdownButton()
        {
            _dropdownButton = new BeepButton
            {
                Size = new Size(20, Height - 6),
                Location = new Point(Width - 24, 3),
                Text = "▼",
                BackColor = Color.Transparent,
                ForeColor = Color.Gray,
                BorderThickness = 0,
                Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };

            _dropdownButton.Click += (s, e) =>
            {
                if (!DesignMode)
                {
                    ToggleDropdown();
                }
            };

            Controls.Add(_dropdownButton);
        }

        /// <summary>
        /// Handles the SizeChanged event of the BeepSelect control.
        /// </summary>
        private void BeepSelect_SizeChanged(object sender, EventArgs e)
        {
            if (!DesignMode && _chipGroup != null)
            {
                UpdateChipGroupLayout();
            }

            if (_dropdownButton != null)
            {
                _dropdownButton.Size = new Size(20, Height - 6);
                _dropdownButton.Location = new Point(Width - 24, 3);
            }
        }
        #endregion

        #region Keyboard Handling
        /// <summary>
        /// Processes dialog key presses for keyboard navigation of the dropdown.
        /// </summary>
        /// <param name="keyData">The key that was pressed.</param>
        /// <returns>True if the key was processed by the control; otherwise, false.</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Don't process keys in design mode
            if (DesignMode)
                return base.ProcessDialogKey(keyData);

            switch (keyData)
            {
                case Keys.Enter:
                case Keys.Space:
                    if (!_isDropdownOpen)
                        ShowDropdown();
                    else
                        CloseDropdown();
                    return true;

                case Keys.Down:
                    if (!_isDropdownOpen)
                    {
                        ShowDropdown();
                        return true;
                    }
                    break;

                case Keys.Escape:
                    if (_isDropdownOpen)
                    {
                        CloseDropdown();
                        return true;
                    }
                    break;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Handles key presses for auto-complete functionality.
        /// </summary>
        /// <param name="e">The key press event arguments.</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (!_enableAutoComplete || DesignMode)
                return;

            // Add the character to our search text
            _autoCompleteText += e.KeyChar;

            // Find the first matching item
            var matchingItem = _items.FirstOrDefault(i =>
                i.Text?.StartsWith(_autoCompleteText, StringComparison.OrdinalIgnoreCase) == true);

            if (matchingItem != null)
            {
                SelectedItem = matchingItem;
                e.Handled = true;
            }

            // Reset timer
            if (_autoCompleteTimer != null)
            {
                _autoCompleteTimer.Stop();
                _autoCompleteTimer.Start();
            }
        }

        /// <summary>
        /// Handles the tick event of the auto-complete timer.
        /// </summary>
        private void AutoCompleteTimer_Tick(object sender, EventArgs e)
        {
            _autoCompleteText = string.Empty;
            _autoCompleteTimer.Stop();
        }
        #endregion

        #region Chip Management
        /// <summary>
        /// Updates the visibility of the chip group.
        /// </summary>
        private void UpdateChipVisibility()
        {
            // Safety check to prevent design-time crashes
            if (DesignMode || _chipGroup == null)
                return;

            _chipGroup.Visible = AllowMultipleSelection && _selectedItems.Count > 0;

            // Adjust control height if needed
            if (AllowMultipleSelection && _selectedItems.Count > 0)
            {
                // Expand height to fit chip group
                int neededHeight = CalculateChipGroupHeight();
                if (neededHeight > _minHeight)
                {
                    Height = neededHeight + 10; // Add some padding
                }
            }
            else
            {
                // Return to minimum height
                Height = _minHeight;
            }
        }

        /// <summary>
        /// Calculates the height needed for the chip group.
        /// </summary>
        /// <returns>The height needed for the chip group.</returns>
        private int CalculateChipGroupHeight()
        {
            if (_chipGroup == null || _selectedItems.Count == 0 || DesignMode)
                return _minHeight;

            int visibleChips = Math.Min(_selectedItems.Count, _maxDisplayChips);
            return visibleChips * (_chipHeight + 4) + 6; // Add spacing and margins
        }

        /// <summary>
        /// Updates the chip display with the current selection.
        /// </summary>
        private void UpdateChipDisplay()
        {
            if (_chipGroup == null || DesignMode)
                return;

            try
            {
                // Get list of items to display as chips
                List<SimpleItem> displayItems;

                if (_selectedItems.Count <= _maxDisplayChips)
                {
                    displayItems = _selectedItems.ToList();
                }
                else
                {
                    // Display limited chips plus potentially a +N indicator
                    displayItems = _selectedItems.Take(_maxDisplayChips).ToList();

                    if (_showChipCount)
                    {
                        // Add a virtual item for showing the count
                        int extraCount = _selectedItems.Count - _maxDisplayChips;
                        if (extraCount > 0)
                        {
                            SimpleItem countItem = new SimpleItem
                            {
                                Text = $"+{extraCount} more",
                                Name = "MoreChips",
                                GuidId = Guid.NewGuid().ToString()
                            };
                            displayItems.Add(countItem);
                        }
                    }
                }

                // Update the chip group
                _chipGroup.ListItems = new BindingList<SimpleItem>(displayItems);

                // Update visibility and control height
                UpdateChipVisibility();
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                System.Diagnostics.Debug.WriteLine($"Error updating chip display: {ex.Message}");
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the Click event of the BeepSelect.
        /// </summary>
        private void BeepSelect_Click(object sender, EventArgs e)
        {
            if (!DesignMode && !_isReadOnly)
            {
                ToggleDropdown();
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the BeepSelect.
        /// </summary>
        private void BeepSelect_LostFocus(object sender, EventArgs e)
        {
            // Skip in design mode
            if (DesignMode)
                return;

            // Check if the focus moved to our dropdown or the dropdown button
            if (_isDropdownOpen && !ContainsFocus &&
                (_popupListForm == null || !_popupListForm.ContainsFocus) &&
                (_dropdownButton == null || !_dropdownButton.ContainsFocus) &&
                (_chipGroup == null || !_chipGroup.ContainsFocus))
            {
                CloseDropdown();
            }
        }

        /// <summary>
        /// Overrides the OnGotFocus event of the BeepSelect.
        /// </summary>
        /// <param name="e">An EventArgs containing event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        /// <summary>
        /// Overrides the OnEnabledChanged event of the BeepSelect.
        /// </summary>
        /// <param name="e">An EventArgs containing event data.</param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (_dropdownButton != null)
            {
                _dropdownButton.Enabled = Enabled;
            }

            if (!DesignMode && _chipGroup != null)
            {
                _chipGroup.Enabled = Enabled;
            }
        }

        /// <summary>
        /// Raises the SelectedItemChanged event.
        /// </summary>
        protected void OnSelectedItemChanged()
        {
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
            SelectedItemsChanged?.Invoke(this, EventArgs.Empty);

            // Update the bound value
            SetValue(AllowMultipleSelection ? _selectedItems : (object)_selectedItem);
        }
        #endregion

        #region Dropdown Handling
        /// <summary>
        /// Toggles the dropdown visibility.
        /// </summary>
        private void ToggleDropdown()
        {
            if (DesignMode)
                return;

            if (_isDropdownOpen)
            {
                CloseDropdown();
            }
            else
            {
                ShowDropdown();
            }
        }

        /// <summary>
        /// Shows the dropdown list.
        /// </summary>
        private void ShowDropdown()
        {
            // Safety checks
            if (DesignMode || _isDropdownOpen || _items.Count == 0)
                return;

            try
            {
                _isDropdownOpen = true;

                // Raise the DropdownOpening event to allow customization
                var args = new DropdownOpeningEventArgs { Items = _items.ToList() };
                DropdownOpening?.Invoke(this, args);

                // Create the popup list form if it doesn't exist or was disposed
                if (_popupListForm == null || _popupListForm.IsDisposed)
                {
                    _popupListForm = new BeepPopupListForm(args.Items);
                    _popupListForm.Theme = Theme;
                    _popupListForm.ShowTitle = !string.IsNullOrEmpty(_dropdownTitle);
                    _popupListForm.IsTitleVisible = !string.IsNullOrEmpty(_dropdownTitle);
                    _popupListForm.SelectedItemChanged += PopupListForm_SelectedItemChanged;
                    _popupListForm.FormClosed += PopupListForm_FormClosed;
                }
                else
                {
                    // Update the items if already created
                    _popupListForm.ListItems = new BindingList<SimpleItem>(args.Items);
                }

                // Set checkbox state for multiple selection
                if (AllowMultipleSelection)
                {
                    foreach (var item in _popupListForm.ListItems)
                    {
                        item.IsChecked = _selectedItems.Any(si => si.GuidId == item.GuidId);
                    }
                }

                // Apply grouping if requested
                if (_showGroupHeaders)
                {
                    OrganizeItemsByGroup(_popupListForm.ListItems.ToList());
                }

                // Ensure the selected item is visible in the dropdown
                if (_selectedItem != null)
                {
                    int index = _items.IndexOf(_selectedItem);
                    if (index >= 0)
                    {
                        _popupListForm.SelectedIndex = index;
                    }
                }

                // Add search box if requested
                if (_showSearchInDropdown)
                {
                    EnableSearchInDropdown();
                }

                // Auto-size the dropdown if requested
                if (_autoSizeDropdown)
                {
                    AutoSizeDropdownWidth();
                }

                // Show the dropdown
                _popupListForm.ShowPopup(_dropdownTitle, this, BeepPopupFormPosition.Bottom, !string.IsNullOrEmpty(_dropdownTitle));

                Invalidate();
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                System.Diagnostics.Debug.WriteLine($"Error showing dropdown: {ex.Message}");
                _isDropdownOpen = false;
            }
        }

        /// <summary>
        /// Closes the dropdown list.
        /// </summary>
        private void CloseDropdown()
        {
            if (DesignMode || !_isDropdownOpen)
                return;

            _isDropdownOpen = false;

            try
            {
                if (_popupListForm != null && !_popupListForm.IsDisposed)
                {
                    _popupListForm.Close();
                    _popupListForm.Dispose();
                    _popupListForm = null;
                }

                // Raise the DropdownClosed event
                DropdownClosed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                System.Diagnostics.Debug.WriteLine($"Error closing dropdown: {ex.Message}");
            }

            Invalidate();
        }

        /// <summary>
        /// Adds a search box to the dropdown.
        /// </summary>
        private void EnableSearchInDropdown()
        {
            if (_popupListForm != null && !_popupListForm.IsDisposed)
            {
                try
                {
                    // BeepPopupListForm doesn't directly expose search controls,
                    // so we'll implement a search filter method
                    TextBox searchBox = new TextBox
                    {
                        Dock = DockStyle.Top,
                        Height = 24,
                        PlaceholderText = "Search...",
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    searchBox.TextChanged += (s, e) =>
                    {
                        _popupListForm.Filter(searchBox.Text);
                    };

                    // Insert at the top of the popup form
                    _popupListForm.Controls.Add(searchBox);
                    _popupListForm.Controls.SetChildIndex(searchBox, 0);
                    searchBox.Focus();
                }
                catch (Exception ex)
                {
                    // Log error but don't crash
                    System.Diagnostics.Debug.WriteLine($"Error enabling search: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Automatically sizes the dropdown width based on content.
        /// </summary>
        private void AutoSizeDropdownWidth()
        {
            if (_popupListForm == null || _popupListForm.IsDisposed || _popupListForm.ListItems == null)
                return;

            try
            {
                // Get the width of the widest item text
                int maxWidth = 0;
                foreach (var item in _popupListForm.ListItems)
                {
                    var textWidth = TextRenderer.MeasureText(item.Text, _popupListForm.TextFont).Width;
                    maxWidth = Math.Max(maxWidth, textWidth);
                }

                // Add padding for icons, checkboxes, etc.
                maxWidth += 40;

                // Set minimum width to the control's width
                maxWidth = Math.Max(maxWidth, Width);

                // Update the form size
                _popupListForm.Size = new Size(maxWidth, _popupListForm.Size.Height);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error auto-sizing dropdown: {ex.Message}");
            }
        }

        /// <summary>
        /// Organizes items by group for display in the dropdown.
        /// </summary>
        /// <param name="items">The list of items to organize.</param>
        private void OrganizeItemsByGroup(List<SimpleItem> items)
        {
            if (!_showGroupHeaders || items == null || items.Count == 0 || _popupListForm == null)
                return;

            try
            {
                // Group items by category (assuming SimpleItem has a Category property)
                var groupedItems = items.GroupBy(item => item.Category)
                                        .OrderBy(g => g.Key)
                                        .ToList();

                // Create a new list with headers
                var organizedItems = new List<SimpleItem>();

                foreach (var group in groupedItems)
                {
                    // Add a header item
                    var headerItem = new SimpleItem
                    {
                        Text = group.Key.ToString() ?? "Uncategorized",
                        Name = $"Group_{group.Key}",
                        GuidId = Guid.NewGuid().ToString(),
                        IsSelected = false,
                        IsVisibleInTree = true,
                        IsEnabled = false // Make headers non-selectable
                    };

                    organizedItems.Add(headerItem);

                    // Add group items
                    organizedItems.AddRange(group);
                }

                // Replace items in the dropdown
                _popupListForm.ListItems = new BindingList<SimpleItem>(organizedItems);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error organizing items by group: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the SelectedItemChanged event of the PopupListForm.
        /// </summary>
        private void PopupListForm_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SelectedItem = e.SelectedItem;

                if (!AllowMultipleSelection)
                {
                    CloseDropdown();
                    this.Focus(); // Return focus to the select control
                }
            }
        }

        /// <summary>
        /// Handles the FormClosed event of the PopupListForm.
        /// </summary>
        private void PopupListForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _isDropdownOpen = false;
            Invalidate();
        }
        #endregion

        #region Painting
        /// <summary>
        /// Paints the control.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DesignMode || !AllowMultipleSelection || _selectedItems.Count == 0)
            {
                // Draw single selection or placeholder
                DrawSingleSelection(e.Graphics);
            }

            // Draw validation error indicator if validation failed
            if (!string.IsNullOrEmpty(_validationError))
            {
                DrawValidationError(e.Graphics);
            }
        }

        /// <summary>
        /// Draws a single selection or placeholder text.
        /// </summary>
        /// <param name="g">The Graphics object to draw with.</param>
        private void DrawSingleSelection(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Calculate the content rectangle
            Rectangle contentRect = new Rectangle(
                DrawingRect.X + 8,
                DrawingRect.Y,
                DrawingRect.Width - 30, // Leave space for dropdown button
                DrawingRect.Height
            );

            // Draw selected item or placeholder text
            string displayText = (_selectedItem != null && UseSimpleItemText)
                ? _selectedItem.Text
                : (_selectedItem != null ? _selectedItem.ToString() : PlaceholderText);

            Color textColor = Enabled
                ? (_selectedItem != null ? ForeColor : Color.Gray)
                : DisabledForeColor;

            using (SolidBrush textBrush = new SolidBrush(textColor))
            using (StringFormat format = new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter })
            {
                // Draw selected item image if available and showing images is enabled
                if (ShowImage && _selectedItem != null && !string.IsNullOrEmpty(_selectedItem.ImagePath))
                {
                    try
                    {
                        // Only attempt to load and draw the image if not in design mode
                        if (!DesignMode && File.Exists(_selectedItem.ImagePath))
                        {
                            using (Image img = Image.FromFile(_selectedItem.ImagePath))
                            {
                                int imgSize = contentRect.Height - 6;
                                Rectangle imgRect = new Rectangle(
                                    contentRect.X,
                                    contentRect.Y + (contentRect.Height - imgSize) / 2,
                                    imgSize,
                                    imgSize
                                );

                                g.DrawImage(img, imgRect);
                                contentRect.X += imgSize + 4;
                                contentRect.Width -= imgSize + 4;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Silently handle image loading errors
                        System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                    }
                }

                // Draw the text
                g.DrawString(displayText, Font, textBrush, contentRect, format);
            }
        }

        /// <summary>
        /// Draws a validation error indicator.
        /// </summary>
        /// <param name="g">The Graphics object to draw with.</param>
        private void DrawValidationError(Graphics g)
        {
            using (var errorBrush = new SolidBrush(Color.Red))
            {
                var errorIcon = "!";
                var textSize = TextRenderer.MeasureText(errorIcon, Font);
                var errorRect = new Rectangle(
                    Width - 20,
                    (Height - textSize.Height) / 2,
                    16,
                    textSize.Height);

                g.DrawString(errorIcon, Font, errorBrush, errorRect);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Clears the current selection.
        /// </summary>
        public void ClearSelection()
        {
            _selectedItems.Clear();
            _selectedItem = null;
            if (!DesignMode)
            {
                UpdateChipDisplay();
                OnSelectedItemChanged();
            }
            Invalidate();
        }

        /// <summary>
        /// Selects an item in the list.
        /// </summary>
        /// <param name="item">The item to select.</param>
        public void SelectItem(SimpleItem item)
        {
            if (item != null && _items.Contains(item))
            {
                SelectedItem = item;
            }
        }

        /// <summary>
        /// Selects multiple items in the list.
        /// </summary>
        /// <param name="items">The collection of items to select.</param>
        public void SelectItems(IEnumerable<SimpleItem> items)
        {
            if (DesignMode)
                return;

            if (!AllowMultipleSelection)
            {
                // In single select mode, just select the first item
                SelectedItem = items.FirstOrDefault();
                return;
            }

            _selectedItems.Clear();
            foreach (var item in items)
            {
                if (_items.Contains(item))
                {
                    _selectedItems.Add(item);
                }
            }

            _selectedItem = _selectedItems.FirstOrDefault();
            UpdateChipDisplay();
            OnSelectedItemChanged();
            Invalidate();
        }

        /// <summary>
        /// Sets the selected index.
        /// </summary>
        /// <param name="index">The index to select.</param>
        public void SetSelectedIndex(int index)
        {
            SelectedIndex = index;
        }

        /// <summary>
        /// Sets the selected item by its value.
        /// </summary>
        /// <param name="value">The value to match.</param>
        public void SetSelectedItemByValue(object value)
        {
            if (value == null)
            {
                SelectedItem = null;
                return;
            }

            SimpleItem item = _items.FirstOrDefault(i =>
                i.Value?.ToString() == value.ToString() ||
                (i.Value == null && value == null));

            SelectedItem = item;
        }

        /// <summary>
        /// Sets the selected item by its text.
        /// </summary>
        /// <param name="text">The text to match.</param>
        public void SetSelectedItemByText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                SelectedItem = null;
                return;
            }

            SimpleItem item = _items.FirstOrDefault(i => i.Text == text);
            SelectedItem = item;
        }

        /// <summary>
        /// Validates the control.
        /// </summary>
        /// <returns>True if validation succeeds; otherwise, false.</returns>
        public bool Validate()
        {
            if (_isRequired && (_selectedItem == null || _selectedItems.Count == 0))
            {
                _validationError = "Selection is required";

                // Raise the Validating event
                var args = new ValidationEventArgs { IsValid = false, ErrorMessage = _validationError };
                Validating?.Invoke(this, args);

                // Draw the control with error styling
                BorderColor = Color.Red;
                Invalidate();

                return false;
            }

            _validationError = string.Empty;
            BorderColor = _currentTheme?.ListBorderColor ?? Color.LightGray;
            Invalidate();

            // Raise the Validating event with success
            Validating?.Invoke(this, new ValidationEventArgs { IsValid = true });

            return true;
        }

        /// <summary>
        /// Applies the filter to the items.
        /// </summary>
        public void ApplyFilter()
        {
            if (_filter == null || DesignMode)
                return;

            var filteredItems = _items.Where(item => _filter(item)).ToList();

            // Update the dropdown if it's open
            if (_popupListForm != null && !_popupListForm.IsDisposed)
            {
                _popupListForm.ListItems = new BindingList<SimpleItem>(filteredItems);
            }
        }

        /// <summary>
        /// Clears any applied filter.
        /// </summary>
        public void ClearFilter()
        {
            _filter = null;

            // Reset the dropdown if it's open
            if (_popupListForm != null && !_popupListForm.IsDisposed)
            {
                _popupListForm.ListItems = new BindingList<SimpleItem>(_items.ToList());
            }
        }
        #endregion

        #region Data Binding Methods
        /// <summary>
        /// Loads items from the data source.
        /// </summary>
        private void LoadItemsFromDataSource()
        {
            if (_dataSource == null)
                return;

            try
            {
                // Clear existing items
                _items.Clear();

                if (_dataSource is IEnumerable enumerable && !(_dataSource is string))
                {
                    foreach (var item in enumerable)
                    {
                        if (item != null)
                        {
                            _items.Add(CreateSimpleItemFromObject(item));
                        }
                    }
                }

                Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading items from data source: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a SimpleItem from an object.
        /// </summary>
        /// <param name="item">The source object.</param>
        /// <returns>A SimpleItem populated from the object.</returns>
        private SimpleItem CreateSimpleItemFromObject(object item)
        {
            var result = new SimpleItem();

            try
            {
                // Set the text from DisplayMember
                if (!string.IsNullOrEmpty(_displayMember) && item != null)
                {
                    var property = item.GetType().GetProperty(_displayMember);
                    if (property != null)
                    {
                        var value = property.GetValue(item);
                        result.Text = value?.ToString();
                    }
                }
                else
                {
                    // Use ToString as fallback
                    result.Text = item.ToString();
                }

                // Set the value from ValueMember
                if (!string.IsNullOrEmpty(_valueMember) && item != null)
                {
                    var property = item.GetType().GetProperty(_valueMember);
                    if (property != null)
                    {
                        result.Value = property.GetValue(item);
                    }
                }
                else
                {
                    // Use the item itself as the value
                    result.Value = item;
                }

                // Store the original object for reference
                result.Item = item;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating SimpleItem from object: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// Sets the value of the control.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public override void SetValue(object value)
        {
            if (DesignMode)
                return;

            if (value is SimpleItem item)
            {
                SelectedItem = item;
            }
            else if (value is IEnumerable<SimpleItem> items)
            {
                SelectItems(items);
            }
            else if (value != null)
            {
                // Try to find an item with matching Value property
                SetSelectedItemByValue(value);
            }
            else
            {
                SelectedItem = null;
            }
        }

        /// <summary>
        /// Gets the value of the control.
        /// </summary>
        /// <returns>The selected item or items.</returns>
        public override object GetValue()
        {
            return AllowMultipleSelection ? _selectedItems : _selectedItem;
        }
        #endregion

        #region Theme Support
        /// <summary>
        /// Applies the current theme to the control.
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (_currentTheme == null)
                return;

            // Apply theme-specific styles
            BackColor = _currentTheme.ListBackColor;
            ForeColor = _currentTheme.ListForeColor;
            BorderColor = _currentTheme.ListBorderColor;

            // Apply theme to dropdown components
            if (_dropdownButton != null)
            {
                _dropdownButton.Theme = Theme;
                _dropdownButton.ForeColor = _currentTheme.ListForeColor;
            }

            if (!DesignMode)
            {
                if (_popupListForm != null && !_popupListForm.IsDisposed)
                {
                    _popupListForm.Theme = Theme;
                }

                if (_chipGroup != null)
                {
                    _chipGroup.Theme = Theme;
                    _chipGroup.Invalidate();
                }
            }

            Invalidate();
        }
        #endregion

        #region Cleanup
        /// <summary>
        /// Releases the unmanaged resources used by the BeepSelect and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up the dropdown form
                if (_popupListForm != null && !_popupListForm.IsDisposed)
                {
                    _popupListForm.FormClosed -= PopupListForm_FormClosed;
                    _popupListForm.SelectedItemChanged -= PopupListForm_SelectedItemChanged;
                    _popupListForm.Dispose();
                    _popupListForm = null;
                }

                // Clean up the timer
                if (_autoCompleteTimer != null)
                {
                    _autoCompleteTimer.Tick -= AutoCompleteTimer_Tick;
                    _autoCompleteTimer.Dispose();
                    _autoCompleteTimer = null;
                }

                // Remove event handlers
                Click -= BeepSelect_Click;
                LostFocus -= BeepSelect_LostFocus;
                SizeChanged -= BeepSelect_SizeChanged;
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Event Arguments Classes
        /// <summary>
        /// Provides data for the DropdownOpening event.
        /// </summary>
        public class DropdownOpeningEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the list of items to be displayed in the dropdown.
            /// </summary>
            public List<SimpleItem> Items { get; set; } = new List<SimpleItem>();
        }

        /// <summary>
        /// Provides data for the Validating event.
        /// </summary>
        public class ValidationEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets whether validation was successful.
            /// </summary>
            public bool IsValid { get; set; }

            /// <summary>
            /// Gets or sets the error message if validation failed.
            /// </summary>
            public string ErrorMessage { get; set; }
        }

        /// <summary>
        /// Provides data for the ItemsRequested event.
        /// </summary>
        public class ItemsRequestEventArgs : EventArgs
        {
            /// <summary>
            /// Gets or sets the current search text.
            /// </summary>
            public string SearchText { get; set; }

            /// <summary>
            /// Gets or sets the page size for pagination.
            /// </summary>
            public int PageSize { get; set; }

            /// <summary>
            /// Gets or sets the page number for pagination.
            /// </summary>
            public int PageNumber { get; set; }

            /// <summary>
            /// Gets or sets the items to be displayed in the dropdown.
            /// </summary>
            public List<SimpleItem> Items { get; set; } = new List<SimpleItem>();
        }
        #endregion
    }
}