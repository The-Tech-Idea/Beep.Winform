using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Editors;
using TheTechIdea.Beep.Winform.Controls.Models;





namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep List of Values Box")]
    public class BeepListofValuesBox : BeepControl
    {
        private TextBox _keyTextBox;
        private TextBox _valueTextBox;
        private BeepButton _dropdownButton;
        private ListBox _dropdownListBox;
        private Form _popupForm;

        private SimpleItemCollection _items = new SimpleItemCollection();

        private string _listField;
        private string _displayField;
        private int _valueTextBoxWidth=80;

        public BeepListofValuesBox()
        {
            Width = 300;
            Height = 30;
        

        }
        protected override void CreateHandle()
        {
            base.CreateHandle();
            _keyTextBox.MouseEnter += (s, e) => OnMouseEnter(e);
            _keyTextBox.MouseHover += (s, e) => OnMouseHover(e);
            _keyTextBox.MouseLeave += (s, e) => OnMouseLeave(e);
            _valueTextBox.MouseEnter += (s, e) => OnMouseEnter(e);
            _valueTextBox.MouseHover += (s, e) => OnMouseHover(e);
            _valueTextBox.MouseLeave += (s, e) => OnMouseLeave(e);
        }
        protected override void InitLayout()
        {
            base.InitLayout();
            InitializeComponents();
        }

        #region Properties
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleItemCollection ListItems
        {
            get => _items;
            set
            {
                _items = value;
                // InitializeMenu();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The field name in the item list to use as the value.")]
        public string ListField
        {
            get => _listField;
            set
            {
                _listField = value;
                UpdateDropdownItems();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The field name in the item list to use as the display text.")]
        public string DisplayField
        {
            get => _displayField;
            set
            {
                
               
                _displayField = value; 
                UpdateDropdownItems();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The selected key.")]
        public string SelectedKey
        {
            get => _keyTextBox.Text;
            set
            {
                if (_keyTextBox == null) return;
                _keyTextBox.Text = value;
                UpdateDisplayValue();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The selected display value.")]
        public string SelectedDisplayValue
        {
            get => _valueTextBox.Text;
            private set { if (_valueTextBox == null) return; _valueTextBox.Text = value; }
        }

        #endregion

        private void InitializeComponents()
        {
            // Key TextBox
            _keyTextBox = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall),
              

            };
            _keyTextBox.TextChanged += KeyTextBox_TextChanged;

            // Value TextBox
            _valueTextBox = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
               
                Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall),
               
            };

            // Dropdown Button
            _dropdownButton = new BeepButton
            {
                Text = "▼",
                Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall),
                ShowAllBorders=false,
                IsShadowAffectedByTheme = false,
                IsChild = true
            };
            _dropdownButton.Click += DropdownButton_Click;

            // Dropdown ListBox
            _dropdownListBox = new ListBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Font = BeepThemesManager.ToFont(_currentTheme.LabelSmall),
            };
            _dropdownListBox.DoubleClick += DropdownListBox_DoubleClick;

            // Popup Form
            _popupForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                BackColor = Color.White,
                Size = new Size(Width, 150),
                Padding = new Padding(2)
            };
            _popupForm.Controls.Add(_dropdownListBox);
            _dropdownListBox.Dock = DockStyle.Fill;
            // Add controls
            Controls.Add(_keyTextBox);
            Controls.Add(_valueTextBox);
            Controls.Add(_dropdownButton);

            AdjustLayout();
        }

        private void AdjustLayout()
        {
            if (DrawingRect == Rectangle.Empty)
                UpdateDrawingRect();

            int padding = BorderThickness + 5;
            int spacing = 5;
            // Calculate button height to match the TextBox height
            int buttonHeight = _keyTextBox.PreferredHeight;
            Height = _keyTextBox.PreferredHeight + (padding * 2);
            // Ensure the height of the control matches the TextBox height

            int centerY = DrawingRect.Top + (DrawingRect.Height - _keyTextBox.PreferredHeight) / 2;
            // Set TextBox dimensions
            _keyTextBox.Location = new Point(DrawingRect.Left + BorderThickness, centerY);
            _keyTextBox.Width = 30; // Divide space for two TextBoxes and the button
            _keyTextBox.Height = buttonHeight;
            // Set dropdown button dimensions
            _dropdownButton.Location = new Point(DrawingRect.Right- buttonHeight - BorderThickness, centerY);
            _dropdownButton.Width = buttonHeight;
            _dropdownButton.Height = buttonHeight;

            // Set display TextBox dimensions
            _valueTextBox.Location = new Point(_keyTextBox.Right + BorderThickness, _keyTextBox.Top);
            _valueTextBox.Width = _dropdownButton.Left- _keyTextBox.Right -(BorderThickness * 2);
            _valueTextBox.Height = buttonHeight;

          

            // Adjust the popup location when resizing
            if (_popupForm.Visible)
            {
                PositionPopupForm();
            }
           
        }



      
        private void UpdateDropdownItems()
        {
            if (_dropdownListBox == null) return;
            _dropdownListBox.Items.Clear();
            if (_items == null) return;

            foreach (var item in _items)
            {
                _dropdownListBox.Items.Add(item);
            }
        }
        private void UpdateDisplayValue()
        {
            var selectedItem = _items.FirstOrDefault(i => i.Id == SelectedKey);
            SelectedDisplayValue = selectedItem?.DisplayField ?? string.Empty;
        }

        #region "Generic Handling"
        private void UpdateDropdownItemsGeneric()
        {
            if (_dropdownListBox == null) return;
            _dropdownListBox.Items.Clear();
            if (_items == null || string.IsNullOrEmpty(_listField) || string.IsNullOrEmpty(_displayField)) return;

            foreach (var item in _items)
            {
                var key = item.GetType().GetProperty(_listField)?.GetValue(item)?.ToString();
                var display = item.GetType().GetProperty(_displayField)?.GetValue(item)?.ToString();
                if (key != null && display != null)
                {
                    _dropdownListBox.Items.Add(new KeyValuePair<string, string>(key, display));
                }
            }
        }
        private void UpdateDisplayValueGeneric()
        {
            if (_items == null || string.IsNullOrEmpty(_listField) || string.IsNullOrEmpty(_displayField)) return;

            var item = _items.FirstOrDefault(i =>
            {
                var key = i.GetType().GetProperty(_listField)?.GetValue(i)?.ToString();
                return key == _keyTextBox.Text;
            });

            if (item != null)
            {
                SelectedDisplayValue = item.GetType().GetProperty(_displayField)?.GetValue(item)?.ToString();
            }
            else
            {
                SelectedDisplayValue = string.Empty;
            }
        }
        #endregion "Generic Handling"
        private void DropdownButton_Click(object sender, EventArgs e)
        {
            if (_popupForm.Visible)
            {
                _popupForm.Hide();
            }
            else
            {
                PositionPopupForm();
                _popupForm.Show();
            }
        }

        private void DropdownListBox_DoubleClick(object sender, EventArgs e)
        {
            if (_dropdownListBox.SelectedItem is SimpleItem selectedItem)
            {
                SelectedKey = selectedItem.Id;
                _popupForm.Hide();
            }
        }

        private void PositionPopupForm()
        {
            var location = PointToScreen(new Point(0, Height));
            _popupForm.Location = new Point(location.X, location.Y);
        }

        private void KeyTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateDisplayValue();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_keyTextBox != null && _valueTextBox != null)
            {
                //int padding = BorderThickness +5;
                //// Ensure the height is fixed
                //Height = _valueTextBox.PreferredHeight+(padding * 2);
                AdjustLayout();
            }
          
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if(_keyTextBox == null) return;
            _keyTextBox.BackColor = _currentTheme.TextBoxBackColor;
            _keyTextBox.ForeColor = _currentTheme.TextBoxForeColor;

            _valueTextBox.BackColor = _currentTheme.TextBoxBackColor;
            _valueTextBox.ForeColor = _currentTheme.TextBoxForeColor;

            _dropdownButton.BackColor = _currentTheme.TextBoxBackColor;
            _dropdownButton.ForeColor = _currentTheme.TextBoxForeColor;
        }
        private  void OnMouseEnter(EventArgs e)
        {
           
           
            IsHovered = true;
           
            //Invalidate();
        }
        protected  void OnMouseHover(EventArgs e)
        {
            IsHovered = true;
        }
        protected  void OnMouseLeave(EventArgs e)
        {
           
            BorderColor = _currentTheme.BorderColor;
            IsPressed = false;
            IsFocused = false;
            IsHovered = false;
            HideToolTip(); // Hide tooltip on mouse leave
                           // Invalidate();
        }
    }
}
