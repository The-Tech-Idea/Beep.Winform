using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;

namespace TheTechIdea.Beep.Winform.Controls.CombinedControls
{
    /// <summary>
    /// Properties for BeepRadioListBox
    /// </summary>
    public partial class BeepRadioListBox
    {
        #region Data Properties

        /// <summary>
        /// Gets or sets the items displayed in both the radio group and list box
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The items displayed in both the radio group and list box.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(UITypeEditor))]
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
        /// Gets or sets the currently selected item
        /// </summary>
        [Browsable(false)]
        public SimpleItem SelectedItem
        {
            get => _radioGroup?.SelectedItems?.FirstOrDefault();
            set
            {
                if (value != null && _radioGroup != null)
                {
                    _isSyncing = true;
                    try
                    {
                        _radioGroup.SelectItem(value.Text);
                        if (_listBox != null)
                        {
                            _listBox.SelectedItem = value;
                        }
                    }
                    finally
                    {
                        _isSyncing = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected value (text of selected item)
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The text value of the currently selected item.")]
        public string SelectedValue
        {
            get => _radioGroup?.SelectedValue;
            set
            {
                if (_radioGroup != null && value != null)
                {
                    _isSyncing = true;
                    try
                    {
                        _radioGroup.SelectedValue = value;
                        var item = _items.FirstOrDefault(i => i.Text == value);
                        if (item != null && _listBox != null)
                        {
                            _listBox.SelectedItem = item;
                        }
                    }
                    finally
                    {
                        _isSyncing = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the search text
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The current search text.")]
        public string SearchText
        {
            get => _searchBox?.Text ?? string.Empty;
            set
            {
                if (_searchBox != null)
                {
                    _searchBox.Text = value ?? string.Empty;
                }
            }
        }

        #endregion

        #region Layout Properties

        /// <summary>
        /// Gets or sets the layout arrangement of the control
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The layout arrangement of the radio group and list box.")]
        [DefaultValue(RadioListBoxLayout.RadioAboveList)]
        public RadioListBoxLayout Layout
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
        [Category("Layout")]
        [Description("Whether the search box is visible.")]
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
        /// Gets or sets whether the radio group is visible
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("Whether the radio group is visible.")]
        [DefaultValue(true)]
        public bool ShowRadioGroup
        {
            get => _showRadioGroup;
            set
            {
                if (_showRadioGroup != value)
                {
                    _showRadioGroup = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the list box is visible
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("Whether the list box is visible.")]
        [DefaultValue(true)]
        public bool ShowListBox
        {
            get => _showListBox;
            set
            {
                if (_showListBox != value)
                {
                    _showListBox = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the divider is visible
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("Whether the divider between radio group and list is visible.")]
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
        [Description("The height of the search box area.")]
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
        /// Gets or sets the height of the radio group area
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The height of the radio group area.")]
        [DefaultValue(60)]
        public int RadioAreaHeight
        {
            get => _radioAreaHeight;
            set
            {
                if (_radioAreaHeight != value && value > 0)
                {
                    _radioAreaHeight = value;
                    UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the spacing between components
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The spacing between components.")]
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
        /// Gets or sets the divider color
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The color of the divider between radio group and list.")]
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
        /// Gets or sets the BeepControlStyle for the control
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style for the control.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _radioGroup?.Style ?? BeepControlStyle.Material3;
            set
            {
                if (_radioGroup != null)
                {
                    _radioGroup.Style = value;
                }
            }
        }

        #endregion

        #region Radio Group Styling Properties

        /// <summary>
        /// Gets or sets the render style for the radio group
        /// </summary>
        [Browsable(true)]
        [Category("Radio Group")]
        [Description("The render style for the radio group items.")]
        [DefaultValue(RadioGroupRenderStyle.Chip)]
        public RadioGroupRenderStyle RadioStyle
        {
            get => _radioStyle;
            set
            {
                if (_radioStyle != value)
                {
                    _radioStyle = value;
                    if (_radioGroup != null)
                    {
                        _radioGroup.RadioGroupStyle = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the radio group
        /// </summary>
        [Browsable(true)]
        [Category("Radio Group")]
        [Description("The orientation of the radio group items.")]
        [DefaultValue(RadioGroupOrientation.Horizontal)]
        public RadioGroupOrientation RadioOrientation
        {
            get => _radioOrientation;
            set
            {
                if (_radioOrientation != value)
                {
                    _radioOrientation = value;
                    if (_radioGroup != null)
                    {
                        _radioGroup.Orientation = value;
                    }
                }
            }
        }

        #endregion

        #region List Box Styling Properties

        /// <summary>
        /// Gets or sets the type/style of the list box
        /// </summary>
        [Browsable(true)]
        [Category("List Box")]
        [Description("The type/style of the list box.")]
        [DefaultValue(ListBoxType.Standard)]
        public ListBoxType ListStyle
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
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the selection changes
        /// </summary>
        [Category("Action")]
        [Description("Occurs when the selection changes.")]
        public event EventHandler<RadioListBoxSelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Occurs when the search text changes
        /// </summary>
        [Category("Action")]
        [Description("Occurs when the search text changes.")]
        public event EventHandler<RadioListBoxSearchEventArgs> SearchTextChanged;

        /// <summary>
        /// Raises the SelectionChanged event
        /// </summary>
        protected virtual void OnSelectionChanged(SimpleItem selectedItem, RadioListBoxSelectionSource source)
        {
            SelectionChanged?.Invoke(this, new RadioListBoxSelectionChangedEventArgs(selectedItem, source));
        }

        /// <summary>
        /// Raises the SearchTextChanged event
        /// </summary>
        protected virtual void OnSearchTextChanged()
        {
            SearchTextChanged?.Invoke(this, new RadioListBoxSearchEventArgs(_searchBox?.Text ?? string.Empty));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an item to the control
        /// </summary>
        public void AddItem(SimpleItem item)
        {
            if (item != null)
            {
                _items.Add(item);
            }
        }

        /// <summary>
        /// Adds an item with text and optional properties
        /// </summary>
        public void AddItem(string text, string imagePath = null, string subText = null)
        {
            var item = new SimpleItem
            {
                Text = text,
                ImagePath = imagePath,
                SubText = subText
            };
            AddItem(item);
        }

        /// <summary>
        /// Removes an item from the control
        /// </summary>
        public bool RemoveItem(SimpleItem item)
        {
            if (item != null && _items.Contains(item))
            {
                return _items.Remove(item);
            }
            return false;
        }

        /// <summary>
        /// Clears all items
        /// </summary>
        public void ClearItems()
        {
            _items.Clear();
        }

        /// <summary>
        /// Selects an item by its text value
        /// </summary>
        public bool SelectItemByText(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            
            var item = _items.FirstOrDefault(i => i.Text == text);
            if (item != null)
            {
                SelectedItem = item;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears the current selection
        /// </summary>
        public void ClearSelection()
        {
            _radioGroup?.ClearSelection();
            _listBox?.ClearSelection();
        }

        #endregion
    }
}

