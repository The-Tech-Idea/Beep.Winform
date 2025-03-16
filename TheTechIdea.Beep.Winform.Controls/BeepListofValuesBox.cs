using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep List of Values Box")]
    [Description("A control that displays a list of values with an embedded grid selection.")]
    public class BeepListofValuesBox : BeepControl
    {
        #region Fields
        private TextBox _keyTextBox;
        private TextBox _valueTextBox;
        private BeepButton _dropdownButton;
        private BeepSimpleGrid _grid;
        private Panel _border1; // Separator between key and value
        private Panel _border2; // Separator between value and button
        private List<SimpleItem> _items = new List<SimpleItem>();
        private int padding=1;
        private int spacing=1;
        private int buttonHeight;
        private object _lastValidKey;
        private readonly IBeepService _beepService;
        private bool _isGridVisible = false;
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
                if (_grid != null)
                {
                    _grid.DataSource = _items; // Update grid data
                    AdjustLayout(); // Reapply column settings
                }
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

            _grid = new BeepSimpleGrid()
            {
                Visible = false,
                ShowColumnHeaders = false,
                ShowHeaderPanel = false,
                ShowNavigator = false,
                IsChild = true,
                DataSource = _items,
                ShowAllBorders=true
                
            };
            _grid.SelectedRowChanged += Grid_SelectedRowChanged;
            _border1 = new Panel { BorderStyle = BorderStyle.FixedSingle };
            _border2 = new Panel { BorderStyle = BorderStyle.FixedSingle };
            Controls.Add(_keyTextBox);
            Controls.Add(_valueTextBox);
            Controls.Add(_dropdownButton);
            Controls.Add(_border1);
            Controls.Add(_border2);
            // Add to grid’s Controls collection


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

            // Proportional layout: key=20%, value=70%, button=10%
            int keyWidth = (int)(totalWidth * 0.2);
            int buttonWidth = (int)(totalWidth * 0.1);
            int valueWidth = totalWidth - keyWidth - buttonWidth - (padding * 2) - (spacing * 2);

            _keyTextBox.Location = new Point(DrawingRect.Left + padding, centerY);
            _keyTextBox.Width = Math.Max(keyWidth, 20) - 1;
            _keyTextBox.Height = buttonHeight;

            _border1.Location = new Point(_keyTextBox.Right + 1, _keyTextBox.Top);
            _border1.Size = new Size(1, _keyTextBox.Height);

            _valueTextBox.Location = new Point(_keyTextBox.Right + spacing, centerY);
            _valueTextBox.Width = Math.Max(valueWidth, 20) - 1;
            _valueTextBox.Height = buttonHeight;

            _border2.Location = new Point(_valueTextBox.Right + 1, _valueTextBox.Top);
            _border2.Size = new Size(1, _valueTextBox.Height);

            _dropdownButton.Location = new Point(_valueTextBox.Right + spacing, centerY);
            _dropdownButton.Width = Math.Max(buttonWidth, buttonHeight - 2);
            _dropdownButton.Height = buttonHeight - 2;
            _dropdownButton.MaxImageSize = new Size(_dropdownButton.Width - 4, _dropdownButton.Height - 4);

          //  System.Diagnostics.Debug.WriteLine($"AdjustLayout: DrawingRect.Width={totalWidth}, KeyWidth={_keyTextBox.Width}, ValueWidth={_valueTextBox.Width}, ButtonWidth={_dropdownButton.Width}, Border1={_border1.Location}, Border2={_border2.Location}");
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustLayout();
            Invalidate();
        }
        public void ShowGrid()
        {
            Control Parent = this.Parent;
            Parent.Controls.Add(_grid);
            if (_isGridVisible)
            {
                _grid.Visible = false;
                _isGridVisible = false;
            }
            else if (_items.Count > 0)
            {
                // Position and configure grid below input area
                if (_grid != null)
                {
                    int gridHeight = Math.Min(_items.Count, 5) * _grid.RowHeight;
                    // grid location should relative to the parent control
                    _grid.Location = new Point(this.Left, this.Bottom);
                    //_grid.Location = new Point(DrawingRect.Left, DrawingRect.Top + buttonHeight + padding);
                    _grid.Size = new Size(this.Width, gridHeight);

                    // Apply your column logic
                    foreach (var column in _grid.Columns)
                    {
                        if (column.ColumnName.Equals("Value", StringComparison.InvariantCultureIgnoreCase))
                        {
                            column.Width = _keyTextBox.Width;
                            column.Visible = true;
                            column.ReadOnly = true;
                        }
                        else if (column.ColumnName.Equals("Text", StringComparison.InvariantCultureIgnoreCase))
                        {
                            column.Width = _valueTextBox.Width;
                            column.Visible = true;
                            column.ReadOnly = true;
                        }
                        else
                        {
                            column.Visible = false;
                        }
                    }
                    // Rearrange columns to ensure "Value" is first
                    _grid.Columns.Sort((a, b) => a.ColumnName.Equals("Value", StringComparison.InvariantCultureIgnoreCase) ? -1 : 1);
                }

                _grid.DataSource = _items; // Ensure latest data
                AdjustLayout(); // Reapply column settings
                _grid.Visible = true;
                _grid.BringToFront();
                _isGridVisible = true;
                _grid.Invalidate();
                System.Diagnostics.Debug.WriteLine($"Dropdown: Grid Visible, Items={_items.Count}, Columns={string.Join(", ", _grid.Columns.Select(c => $"{c.ColumnName}: {c.Width}, Visible={c.Visible}"))}");
            }
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
                //   Rectangle keyRect = new Rectangle(_keyTextBox.Left, _keyTextBox.Top, _keyTextBox.Width, _keyTextBox.Height);
                Rectangle valueRect = rectangle; //new Rectangle(_valueTextBox.Left, _valueTextBox.Top, _valueTextBox.Width, _valueTextBox.Height);
               // Rectangle buttonRect = new Rectangle(_dropdownButton.Left, _dropdownButton.Top, _dropdownButton.Width, _dropdownButton.Height);

                using (SolidBrush textBrush = new SolidBrush(_currentTheme.TextBoxForeColor))
                {
                    string keyText = _keyTextBox.Text ?? string.Empty;
                    string valueText = _valueTextBox.Text ?? string.Empty;

                    //graphics.DrawString(keyText, _keyTextBox.Font ?? Font, textBrush, keyRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                    graphics.DrawString(valueText, _valueTextBox.Font ?? Font, textBrush, valueRect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter });
                }
              //  _dropdownButton.Draw(graphics, buttonRect);

               // System.Diagnostics.Debug.WriteLine($"Draw: KeyText={_keyTextBox.Text}, ValueText={_valueTextBox.Text}, KeyRect={keyRect}, ValueRect={valueRect}");
            }
        }
        #endregion

        #region Event Handlers
        private void DropdownButton_Click(object sender, EventArgs e)
        {
            if (_isGridVisible)
            {
                _grid.Visible = false;
                _isGridVisible = false;
            }
            else if (_items.Count > 0)
            {   
                ShowGrid();
                System.Diagnostics.Debug.WriteLine($"Dropdown: Grid Visible, Items={_items.Count}, Columns={string.Join(", ", _grid.Columns.Select(c => $"{c.ColumnName}: {c.Width}, Visible={c.Visible}"))}");
            }
            Invalidate();
        }

        private void Grid_SelectedRowChanged(object sender, BeepRowSelectedEventArgs e)
        {
            if (e.Row != null && e.Row.RowData is SimpleItem item)
            {
                SetSelectedItem(item);
                _grid.Visible = false;
                _isGridVisible = false;
                System.Diagnostics.Debug.WriteLine($"Grid Selected: Value={item.Value}, Text={item.Text}");
                Invalidate();
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
            if (_grid != null)
            {
                _grid.DataSource = null;
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
            _grid.Theme = Theme;

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
    }
}