using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;

namespace TheTechIdea.Beep.Winform.Controls
{
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
                if (_popupForm != null)
                {
                    _popupForm.ListItems = _listItems;
                }
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
        [Description("The visual style/variant of the combo box.")]
        public ComboBoxType ComboBoxType
        {
            get => _comboBoxType;
            set
            {
                if (_comboBoxType == value) return;
                _comboBoxType = value;
                _comboBoxPainter = null; // Force painter recreation
                InvalidateLayout();
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
                _textFont = value ?? new Font("Segoe UI", 9f);
                UseThemeFont = false;
                InvalidateLayout();
            }
        }
        
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Placeholder text shown when no item is selected.")]
        public string PlaceholderText { get; set; } = "Select an item...";
        
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether the popup is currently open.")]
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
                if (_isPopupOpen == value) return;
                
                _isPopupOpen = value;
                if (_isPopupOpen)
                {
                    ShowPopup();
                }
                else
                {
                    ClosePopup();
                }
            }
        }
        
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
        [Description("Whether to auto-complete as the user types.")]
        [DefaultValue(true)]
        public bool AutoComplete { get; set; } = true;
        
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
        
        #region Layout Properties
        
        [Browsable(true)]
        [Category("Layout")]
        [Description("Width of the dropdown button in pixels.")]
        [DefaultValue(32)]
        public int DropdownButtonWidth { get; set; } = 32;
        
        [Browsable(true)]
        [Category("Layout")]
        [Description("Padding inside the combo box.")]
        public Padding InnerPadding { get; set; } = new Padding(8, 4, 8, 4);
        
        #endregion
    }
}
