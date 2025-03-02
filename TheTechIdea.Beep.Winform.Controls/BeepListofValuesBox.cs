using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep List of Values Box")]
    [Description("A control that displays a list of values with a popup selection.")]
    public class BeepListofValuesBox : BeepControl
    {
        #region Fields
        private TextBox _keyTextBox;
        private TextBox _valueTextBox;
        private BeepButton _dropdownButton;
        private BeepPopupListForm _popupForm;
        private List<SimpleItem> _items = new List<SimpleItem>();
        private int padding;
        private int spacing;
        private int buttonHeight;
        private object _lastValidKey; // Store the last valid key as an object
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
                if (_popupForm != null)
                {
                    _popupForm.ListItems = new BindingList<SimpleItem>(_items);
                }
                UpdateDisplayValue();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The selected key (Value property of SimpleItem).")]
        public string SelectedKey
        {
            get => _keyTextBox.Text;
            set
            {
                if (ValidateKey(value))
                {
                    _keyTextBox.Text = value;
                    UpdateLastValidKey(value);
                    UpdateDisplayValue();
                    Invalidate();
                }
                else
                {
                    _keyTextBox.Text = _lastValidKey?.ToString() ?? string.Empty;
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The selected display value.")]
        public string SelectedDisplayValue
        {
            get => _valueTextBox.Text;
            private set => _valueTextBox.Text = value;
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
            _keyTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None
            };
            _keyTextBox.TextChanged += KeyTextBox_TextChanged;

            _valueTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                ReadOnly = true
            };

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

            _popupForm = new BeepPopupListForm(_items)
            {
                Theme = Theme
            };
            _popupForm.SelectedItemChanged += PopupForm_SelectedItemChanged;

            Controls.Add(_keyTextBox);
            Controls.Add(_valueTextBox);
            Controls.Add(_dropdownButton);

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
            buttonHeight = _keyTextBox.PreferredHeight;
            Height = buttonHeight + (padding * 2);
        }

        private void AdjustLayout()
        {
            UpdateDrawingRect();
            GetHeight();

            int centerY = DrawingRect.Top + (DrawingRect.Height - buttonHeight) / 2;

            _keyTextBox.Location = new Point(DrawingRect.Left + padding, centerY);
            _keyTextBox.Width = 60;
            _keyTextBox.Height = buttonHeight;

            _valueTextBox.Location = new Point(_keyTextBox.Right + spacing, centerY);
            _valueTextBox.Width = DrawingRect.Width - _keyTextBox.Width - buttonHeight - (padding * 2) - (spacing * 2);
            _valueTextBox.Height = buttonHeight;

            _dropdownButton.Location = new Point(_valueTextBox.Right + spacing, centerY);
            _dropdownButton.Width = buttonHeight - 2;
            _dropdownButton.Height = buttonHeight - 2;
            _dropdownButton.MaxImageSize = new Size(buttonHeight - 4, buttonHeight - 4);

            if (_popupForm.Visible)
            {
                PositionPopupForm();
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
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            using (SolidBrush backgroundBrush = new SolidBrush(_currentTheme.ButtonBackColor))
            {
                graphics.FillRectangle(backgroundBrush, rectangle);
            }

            if (BorderThickness > 0)
            {
                using (Pen borderPen = new Pen(_currentTheme.BorderColor, BorderThickness))
                {
                    graphics.DrawRectangle(borderPen, rectangle);
                }
            }

            if (_keyTextBox != null && _valueTextBox != null && _dropdownButton != null)
            {
                Rectangle keyRect = new Rectangle(_keyTextBox.Left, _keyTextBox.Top, _keyTextBox.Width, _keyTextBox.Height);
                Rectangle valueRect = new Rectangle(_valueTextBox.Left, _valueTextBox.Top, _valueTextBox.Width, _valueTextBox.Height);
                Rectangle buttonRect = new Rectangle(_dropdownButton.Left, _dropdownButton.Top, _dropdownButton.Width, _dropdownButton.Height);

                TextRenderer.DrawText(graphics, _keyTextBox.Text, _keyTextBox.Font, keyRect, _currentTheme.TextBoxForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                TextRenderer.DrawText(graphics, _valueTextBox.Text, _valueTextBox.Font, valueRect, _currentTheme.TextBoxForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                _dropdownButton.Draw(graphics, buttonRect);
            }
        }
        #endregion

        #region Event Handlers
        private void DropdownButton_Click(object sender, EventArgs e)
        {
            if (_popupForm.Visible)
            {
                _popupForm.Hide();
            }
            else
            {
                if(_popupForm.ListItems.Count == 0)
                {
                    _popupForm.ListItems = new BindingList<SimpleItem>(_items);
                    return;
                }
                _popupForm.ListItems = new BindingList<SimpleItem>(_items);
                _popupForm.ShowPopup("Select an Item", this, BeepPopupFormPosition.Bottom);
            }
        }

        private void PopupForm_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SetSelectedItem(e.SelectedItem);
                _popupForm.Hide();
            }
        }

        private void KeyTextBox_TextChanged(object sender, EventArgs e)
        {
            string newKey = _keyTextBox.Text;
            if (ValidateKey(newKey))
            {
                UpdateLastValidKey(newKey);
                UpdateDisplayValue();
            }
            else
            {
                MessageBox.Show("Invalid key. Please enter a valid value from the list.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _keyTextBox.Text = _lastValidKey?.ToString() ?? string.Empty;
                UpdateDisplayValue();
            }
        }
        #endregion

        #region Helper Methods
        private bool ValidateKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return true; // Allow empty input to clear selection
            }

            foreach (var item in _items)
            {
                if (item.Value != null && item.Value.ToString() == key)
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateLastValidKey(string key)
        {
            var selectedItem = _items.FirstOrDefault(i => i.Value?.ToString() == key);
            _lastValidKey = selectedItem?.Value;
        }

        private void PositionPopupForm()
        {
            var location = PointToScreen(new Point(0, Height));
            _popupForm.Location = new Point(location.X, location.Y);
        }

        private void UpdateDisplayValue()
        {
            var selectedItem = _items.FirstOrDefault(i => i.Value?.ToString() == SelectedKey);
            SelectedDisplayValue = selectedItem?.Text ?? string.Empty;
        }

        private void SetSelectedItem(SimpleItem item)
        {
            _keyTextBox.Text = item.Value?.ToString() ?? string.Empty;
            _lastValidKey = item.Value;
            UpdateDisplayValue();
            Invalidate();
        }

        public void Reset()
        {
            _items.Clear();
            _keyTextBox.Text = string.Empty;
            _valueTextBox.Text = string.Empty;
            if (_popupForm != null)
            {
                _popupForm.ListItems.Clear();
            }
            _lastValidKey = null;
        }
        #endregion

        #region Theme and Value Management
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_keyTextBox == null) return;

            _keyTextBox.BackColor = _currentTheme.AltRowBackColor;
            _keyTextBox.ForeColor = _currentTheme.LatestForColor;
            _valueTextBox.BackColor = _currentTheme.AltRowBackColor;
            _valueTextBox.ForeColor = _currentTheme.AccentTextColor;
            _dropdownButton.BackColor = _currentTheme.ButtonBackColor;
            _dropdownButton.ForeColor = _currentTheme.ButtonForeColor;
            _dropdownButton.ApplyThemeOnImage = true;
            _popupForm.Theme = Theme;

            Invalidate();
        }

        public override void SetValue(object value)
        {
            if (value is SimpleItem item)
            {
                SetSelectedItem(item);
            }
            else
            {
                SelectedKey = value?.ToString();
            }
        }

        public override object GetValue()
        {
            var selectedItem = _items.FirstOrDefault(i => i.Value?.ToString() == SelectedKey);
            return selectedItem;
        }
        #endregion
    }
}