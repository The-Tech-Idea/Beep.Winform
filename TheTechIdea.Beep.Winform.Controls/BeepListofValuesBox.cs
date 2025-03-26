using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep List of Values Box")]
    [Description("A control that displays a list of values with a popup grid selection.")]
    public class BeepListofValuesBox : BeepControl
    {
        #region Fields
        private TextBox _keyTextBox;
        private TextBox _valueTextBox;
        private BeepButton _dropdownButton;
        private BeepPopupGridForm _popupGridForm;
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
                //if (_popupGridForm != null)
                //{
                //    _popupGridForm.DataSource = _items;
                //}
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
            get => _valueTextBox.Text;
            private set
            {
                _valueTextBox.Text = value;
                Invalidate();
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
            _keyTextBox = new TextBox { BorderStyle = BorderStyle.None, Visible = true };
            _keyTextBox.TextChanged += KeyTextBox_TextChanged;

            _valueTextBox = new TextBox { BorderStyle = BorderStyle.None, ReadOnly = true, Visible = true };
            _valueTextBox.TextChanged += (s, e) => Invalidate();

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

            _popupGridForm = new BeepPopupGridForm();
            _popupGridForm.BorderThickness = 4;
            _popupGridForm.RowSelected += PopupGridForm_RowSelected;
            _popupGridForm.Theme = Theme;

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

            _keyTextBox.Location = new Point(DrawingRect.Left + padding, centerY);
            _keyTextBox.Width = Math.Max(keyWidth, 20) - 1;
            _keyTextBox.Height = buttonHeight;

            _valueTextBox.Location = new Point(_keyTextBox.Right + spacing, centerY);
            _valueTextBox.Width = Math.Max(valueWidth, 20) - 1;
            _valueTextBox.Height = buttonHeight;

            _dropdownButton.Location = new Point(_valueTextBox.Right + spacing, centerY);
            _dropdownButton.Width = Math.Max(buttonWidth, buttonHeight - 2);
            _dropdownButton.Height = buttonHeight - 2;
            _dropdownButton.MaxImageSize = new Size(_dropdownButton.Width - 4, _dropdownButton.Height - 4);
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
                Rectangle valueRect = rectangle;

                using (SolidBrush textBrush = new SolidBrush(_currentTheme.TextBoxForeColor))
                {
                    string keyText = _keyTextBox.Text ?? string.Empty;
                    string valueText = _valueTextBox.Text ?? string.Empty;

                    graphics.DrawString(valueText, _valueTextBox.Font ?? Font, textBrush, valueRect,
                        new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                }
            }
        }
        #endregion

        #region Event Handlers
        private void DropdownButton_Click(object sender, EventArgs e)
        {
            if (_items.Count > 0)
            {
                    _popupGridForm = new BeepPopupGridForm();
                    _popupGridForm.RowSelected += PopupGridForm_RowSelected;
                    _popupGridForm.Theme = Theme;
                _popupGridForm.ShowPopupList(this, _items, "Value", "Text", _keyTextBox.Width, _valueTextBox.Width);
            }
        }

        private void PopupGridForm_RowSelected(object sender, object selectedRow)
        {
            if(selectedRow == null) return;
            if(selectedRow is DataRowWrapper rowWrapper)
            {
                if (rowWrapper.OriginalData is SimpleItem item)
                {
                    SetSelectedItem(item);
                    return;
                }
            }
            if(selectedRow is SimpleItem item1)
            {
                SetSelectedItem(item1);
            }
            //DataRowWrapper rowWrapper = (DataRowWrapper)selectedRow;

            //if (rowWrapper.OriginalData is SimpleItem item)
            //{
            //    SetSelectedItem(item);
            //}
        }

        private void KeyTextBox_TextChanged(object sender, EventArgs e)
        {
            string newKey = _keyTextBox.Text;
            if (ValidateKey(newKey))
            {
                UpdateLastValidKey(newKey);
                UpdateDisplayValue();
            }
            else if (!string.IsNullOrEmpty(newKey))
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
            _lastValidKey = null;
            if (_popupGridForm != null)
            {
                _popupGridForm.DataSource = null;
            }
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
            _popupGridForm.Theme = Theme;

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
            return _items.FirstOrDefault(i => i.Value?.ToString() == SelectedKey);
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_popupGridForm != null)
                {
                    _popupGridForm.Dispose();
                    _popupGridForm = null;
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}