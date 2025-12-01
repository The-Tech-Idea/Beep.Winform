using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.ContextMenus;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.ToolTips;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep List of Values Box")]
    [Description("A control that displays a list of values with a popup context menu selection, similar to Oracle Forms LOV.")]
    public class BeepListofValuesBox : BaseControl
    {
        #region Fields
        private BeepTextBox _keyTextBox;
        private BeepTextBox _valueTextBox;
        private BeepButton _dropdownButton;
        private BeepContextMenu _lovContextMenu;
        private List<SimpleItem> _items = new List<SimpleItem>();
        private int padding = 1;
        private int spacing = 1;
        private int buttonHeight;
        private object _lastValidKey;
        #endregion

        #region Properties
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SimpleItem> ListItems
        {
            get => _items;
            set
            {
                _items = value ?? new List<SimpleItem>();
                UpdateDisplayValue();
                
                // Update context menu items
                if (_lovContextMenu != null)
                {
                    _lovContextMenu.ClearItems();
                    foreach (var item in _items)
                    {
                        if (item != null)
                        {
                            _lovContextMenu.AddItem(item);
                        }
                    }
                }
                
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The selected key (Value property of SimpleItem).")]
        public string SelectedKey
        {
            get => _keyTextBox?.Text ?? string.Empty;
            set
            {
                if (_keyTextBox == null) return;
                
                if (ValidateKey(value))
                {
                    _keyTextBox.Text = value;
                    UpdateLastValidKey(value);
                    UpdateDisplayValue();
                    Invalidate();
                }
                else if (!string.IsNullOrEmpty(value))
                {
                    _keyTextBox.Text = _lastValidKey?.ToString() ?? string.Empty;
                    UpdateDisplayValue();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The selected display value.")]
        public string SelectedDisplayValue
        {
            get => _valueTextBox?.Text ?? string.Empty;
            private set
            {
                if (_valueTextBox != null)
                {
                    _valueTextBox.Text = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructor
        public BeepListofValuesBox()
        {
            InitializeComponents();
            ApplyTheme();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            // Initialize key textbox (editable) using BeepTextBox
            _keyTextBox = new BeepTextBox
            {
                IsChild = true,
                IsFrameless = true,
                Visible = true,
                PlaceholderText = "Enter key..."
            };
            _keyTextBox.TextChanged += KeyTextBox_TextChanged;

            // Initialize value textbox (read-only display) using BeepTextBox
            _valueTextBox = new BeepTextBox
            {
                IsChild = true,
                IsFrameless = true,
                ReadOnly = true,
                Visible = true,
                PlaceholderText = "Display value..."
            };
            _valueTextBox.TextChanged += (s, e) => Invalidate();

            // Initialize dropdown button using BeepButton
            _dropdownButton = new BeepButton
            {
                Text = "▼",
                HideText = true,
                ShowAllBorders = false,
                IsShadowAffectedByTheme = false,
                IsChild = true,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.Overlay,
                TextAlign = ContentAlignment.MiddleCenter,
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.dropdown-select.svg"
            };
            _dropdownButton.Click += DropdownButton_Click;

            // Initialize context menu for LOV display
            _lovContextMenu = new BeepContextMenu
            {
                ShowSearchBox = true, // Enable search for LOV
                ShowCheckBox = false, // Single selection mode
                MultiSelect = false
            };
            _lovContextMenu.ItemClicked += LovContextMenu_ItemClicked;

            Controls.Add(_keyTextBox);
            Controls.Add(_valueTextBox);
            Controls.Add(_dropdownButton);

            // Forward mouse events for proper hover/focus behavior
            _keyTextBox.MouseEnter += (s, e) => OnMouseEnter(e);
            _keyTextBox.MouseHover += (s, e) => OnMouseHover(e);
            _keyTextBox.MouseLeave += (s, e) => OnMouseLeave(e);
            _valueTextBox.MouseEnter += (s, e) => OnMouseEnter(e);
            _valueTextBox.MouseHover += (s, e) => OnMouseHover(e);
            _valueTextBox.MouseLeave += (s, e) => OnMouseLeave(e);

            _lastValidKey = null;
            AdjustLayout();
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            Width = 300;
            Height = 30;
            AdjustLayout();
        }
        #endregion

        #region Layout and Drawing
        private void GetHeight()
        {
            padding = BorderThickness;
            spacing = 5;
            buttonHeight = _keyTextBox != null ? _keyTextBox.PreferredHeight : 24;
            Height = Math.Max(Height, buttonHeight + (padding * 2));
        }

        private void AdjustLayout()
        {
            UpdateDrawingRect();
            GetHeight();

            int totalWidth = DrawingRect.Width;
            int centerY = DrawingRect.Top + (DrawingRect.Height - buttonHeight) / 2;

            int keyWidth = (int)(totalWidth * 0.2);
            int buttonWidth = (int)(totalWidth * 0.1);
            int valueWidth = totalWidth - keyWidth - buttonWidth - (padding * 2) - (spacing);

            if (_keyTextBox != null)
            {
                _keyTextBox.Location = new Point(DrawingRect.Left + padding, centerY);
                _keyTextBox.Width = Math.Max(keyWidth, 20) - 1;
                _keyTextBox.Height = buttonHeight;
            }

            if (_valueTextBox != null)
            {
                _valueTextBox.Location = new Point(_keyTextBox.Right + spacing, centerY);
                _valueTextBox.Width = Math.Max(valueWidth, 20) - 1;
                _valueTextBox.Height = buttonHeight;
            }

            if (_dropdownButton != null)
            {
                _dropdownButton.Location = new Point(_valueTextBox.Right + spacing, centerY);
                _dropdownButton.Width = Math.Max(buttonWidth, buttonHeight - 2);
                _dropdownButton.Height = buttonHeight - 2;
                _dropdownButton.MaxImageSize = new Size(_dropdownButton.Width - 4, _dropdownButton.Height - 4);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLayout();
            Invalidate();
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            // BaseControl handles the main drawing, we just need to ensure child controls are positioned
            // The BeepTextBox controls handle their own drawing
            base.Draw(graphics, rectangle);
        }
        #endregion

        #region Event Handlers
        private void DropdownButton_Click(object sender, EventArgs e)
        {
            if (_items.Count > 0 && _lovContextMenu != null)
            {
                // Clear existing items
                _lovContextMenu.ClearItems();
                
                // Add all LOV items to context menu
                // Ensure DisplayField is set to Text for proper display (Oracle Forms LOV style)
                foreach (var item in _items)
                {
                    if (item != null)
                    {
                        // Set DisplayField to Text if not already set (for context menu display)
                        if (string.IsNullOrEmpty(item.DisplayField) && !string.IsNullOrEmpty(item.Text))
                        {
                            item.DisplayField = item.Text;
                        }
                        _lovContextMenu.AddItem(item);
                    }
                }
                
                // Apply theme to context menu
                if (UseThemeColors && _currentTheme != null)
                {
                    _lovContextMenu.Theme = _currentTheme.ThemeName;
                }
                else if (Theme != null)
                {
                    _lovContextMenu.Theme = Theme;
                }
                
                // Show context menu below the dropdown button (Oracle Forms LOV style)
                Point showLocation = _dropdownButton.PointToScreen(new Point(0, _dropdownButton.Height));
                _lovContextMenu.Show(showLocation, this);
            }
        }

        private void LovContextMenu_ItemClicked(object sender, MenuItemEventArgs e)
        {
            if (e?.Item != null)
            {
                SetSelectedItem(e.Item);
                
                // Close the context menu
                if (_lovContextMenu != null && _lovContextMenu.Visible)
                {
                    _lovContextMenu.Close();
                }
            }
        }

        private void KeyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_keyTextBox == null) return;
            
            string newKey = _keyTextBox.Text;
            if (ValidateKey(newKey))
            {
                UpdateLastValidKey(newKey);
                UpdateDisplayValue();
            }
            else if (!string.IsNullOrEmpty(newKey))
            {
                // Show validation error using tooltip or notification instead of MessageBox
                ShowNotification("Invalid key. Please enter a valid value from the list.", ToolTipType.Warning, 2000);
                _keyTextBox.Text = _lastValidKey?.ToString() ?? string.Empty;
                UpdateDisplayValue();
            }
        }
        #endregion

        #region Helper Methods
        private bool ValidateKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return true;
            return _items.Any(i => i.Value?.ToString() == key);
        }

        private void UpdateLastValidKey(string key)
        {
            var selectedItem = _items.FirstOrDefault(i => i.Value?.ToString() == key);
            _lastValidKey = selectedItem?.Value;
        }

        private void UpdateDisplayValue()
        {
            var selectedItem = _items.FirstOrDefault(i => i.Value?.ToString() == SelectedKey);
            SelectedDisplayValue = selectedItem?.Text ?? string.Empty;
        }

        private void SetSelectedItem(SimpleItem item)
        {
            if (item == null) return;
            
            if (_keyTextBox != null)
            {
                _keyTextBox.Text = item.Value?.ToString() ?? string.Empty;
            }
            _lastValidKey = item.Value;
            UpdateDisplayValue();
            
            // Raise selection changed event
            OnSelectionChanged();
            
            Invalidate();
        }

        public void Reset()
        {
            _items.Clear();
            if (_keyTextBox != null)
            {
                _keyTextBox.Text = string.Empty;
            }
            if (_valueTextBox != null)
            {
                _valueTextBox.Text = string.Empty;
            }
            _lastValidKey = null;
            if (_lovContextMenu != null)
            {
                _lovContextMenu.ClearItems();
            }
        }
        
        /// <summary>
        /// Raises the SelectionChanged event
        /// </summary>
        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Event raised when the selected item changes
        /// </summary>
        public event EventHandler SelectionChanged;
        #endregion

        #region Theme and Value Management
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            if (_keyTextBox == null || _valueTextBox == null || _dropdownButton == null) 
                return;

            // Apply theme to key textbox
            _keyTextBox.Theme = _currentTheme.ThemeName ?? Theme;
            _keyTextBox.UseThemeColors = UseThemeColors;
            _keyTextBox.ApplyTheme();
            
            // Apply theme to value textbox (read-only display)
            _valueTextBox.Theme = _currentTheme.ThemeName ?? Theme;
            _valueTextBox.UseThemeColors = UseThemeColors;
            _valueTextBox.ApplyTheme();
            
            // Apply theme to dropdown button
            _dropdownButton.Theme = _currentTheme.ThemeName ?? Theme;
            _dropdownButton.UseThemeColors = UseThemeColors;
            _dropdownButton.ApplyTheme();
            
            // Apply theme to context menu
            if (_lovContextMenu != null)
            {
                _lovContextMenu.Theme = _currentTheme.ThemeName ?? Theme;
            }

            Invalidate();
        }

        public override void SetValue(object value)
        {
            if (value is SimpleItem item)
            {
                SetSelectedItem(item);
            }
            else if (value != null)
            {
                SelectedKey = value.ToString();
            }
            else
            {
                SelectedKey = string.Empty;
            }
        }

        public override object GetValue()
        {
            return _items.FirstOrDefault(i => i.Value?.ToString() == SelectedKey);
        }
        
        /// <summary>
        /// Gets the selected SimpleItem
        /// </summary>
        [Browsable(false)]
        public SimpleItem SelectedItem
        {
            get => _items.FirstOrDefault(i => i.Value?.ToString() == SelectedKey);
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_lovContextMenu != null)
                {
                    _lovContextMenu.ItemClicked -= LovContextMenu_ItemClicked;
                    if (!_lovContextMenu.IsDisposed)
                    {
                        _lovContextMenu.Close();
                    }
                    _lovContextMenu.Dispose();
                    _lovContextMenu = null;
                }
                
                if (_keyTextBox != null)
                {
                    _keyTextBox.TextChanged -= KeyTextBox_TextChanged;
                    _keyTextBox.Dispose();
                    _keyTextBox = null;
                }
                
                if (_valueTextBox != null)
                {
                    _valueTextBox.Dispose();
                    _valueTextBox = null;
                }
                
                if (_dropdownButton != null)
                {
                    _dropdownButton.Click -= DropdownButton_Click;
                    _dropdownButton.Dispose();
                    _dropdownButton = null;
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}

