using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Utilities;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DisplayName("Beep ComboBox")]
    [Category("Beep Controls")]
    [Description("A combo box control that displays a list of items.")]
    public class BeepComboBox : BeepControl
    {
        public event EventHandler PopupOpened;
        public event EventHandler PopupClosed;
        private BeepTextBox _comboTextBox;
        private BeepButton _dropDownButton;
        private BeepPopupListForm _beepListBox;
        private bool _isEditing;
        private SimpleItem _selectedItem;
        private int _selectedItemIndex = -1;
        private int _collapsedHeight = 0;
        private int _buttonWidth = 25;
        private int _maxListHeight = 200;
        private int _padding = 2;
        private int _minWidth = 80;
        private BindingList<SimpleItem> _listItems = new BindingList<SimpleItem>();
    
        private Color tmpfillcolor;
        private Color tmpstrokecolor;
        private bool _isPopupOpen = false;
        private bool _isExpanded;
        private BeepPopupListForm menuDialog;
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }

        private Font _textFont = new Font("Arial", 10);
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
                _textFont = value ?? new Font("Arial", 10);
                UseThemeFont = false;
                Font = _textFont;
                if (_comboTextBox != null)
                {
                    _comboTextBox.TextFont = _textFont;
                }
                GetControlHeight();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _listItems;
            set
            {
                _listItems = value;
                if (_beepListBox == null) _beepListBox = new BeepPopupListForm();
                _beepListBox.ListItems = value;
            }
        }

        public SimpleItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == null) return;
                _selectedItem = value;
                _selectedItemIndex = _listItems.IndexOf(_selectedItem);
                _comboTextBox.Text = value.Text;
                Text = value.Text;
                if(_selectedItem.Item != null)
                {
                    SelectedValue = _selectedItem.Item;
                }
                OnSelectedItemChanged(_selectedItem);
                Invalidate();
            }
        }

        [Browsable(true)]
        public string SelectedText
        {
            get => _comboTextBox.Text;
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedItemIndex;
            set
            {
                if (value >= 0 && value < _listItems.Count)
                {
                    SelectedItem = _listItems[value];
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
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

        public DbFieldCategory Category { get; set; } = DbFieldCategory.Numeric;

        public BeepComboBox()
        {
            ShowAllBorders=true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;

            if (Width < _minWidth) Width = 150;

            _comboTextBox = new BeepTextBox
            {
                IsChild = true,
                ShowAllBorders = false,
                BorderStyle = BorderStyle.None,
                Multiline = false,
                AutoSize = false,
                Visible = false
            };
            _comboTextBox.TextFont = _textFont;
            _comboTextBox.TextChanged += (s, e) => Invalidate();
            Controls.Add(_comboTextBox);

            GetControlHeight();
            Height = _collapsedHeight;

            _dropDownButton = new BeepButton
            {
                IsChild = true,
                IsBorderAffectedByTheme = false,
                IsRoundedAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                ShowAllBorders = false,
                ShowShadow = false,
                HideText = true,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText
            };
            _dropDownButton.Click += (s, e) => ToggleMenu();
            Controls.Add(_dropDownButton);
            SetDropDownButtonImage();

            BorderRadius = 3;
            ApplyTheme();

            Click += (s, e) => StartEditing();
        }

        public void Reset()
        {
            SelectedItem = null;
            _comboTextBox.Text = string.Empty;
            Invalidate();
        }

        private int GetControlHeight()
        {
            int minHeight;
            if (_comboTextBox == null)
            {
                using (Graphics g = CreateGraphics())
                {
                    SizeF textSize = g.MeasureString("A", _textFont);
                    minHeight = (int)Math.Ceiling(textSize.Height);
                }
            }
            else
            {
                minHeight = _comboTextBox.SingleLineHeight;
            }
            // Ensure _collapsedHeight is at least minHeight, adding padding for breathing room
            _collapsedHeight = Math.Max(minHeight, minHeight + (2 * _padding) + 2); // +2 for extra space
            return _collapsedHeight;
        }

        protected override void OnResize(EventArgs e)
        {
            SuspendLayout();
            base.OnResize(e);

            if (Width < _minWidth)
                Width = _minWidth;

            GetControlHeight();

            if (!_isPopupOpen && Height != _collapsedHeight)
                Height = _collapsedHeight;

            Rectangle clientRect = ClientRectangle;
            if (_comboTextBox != null)
            {
                _comboTextBox.Location = new Point(_padding, _padding);
                _comboTextBox.Width = clientRect.Width - _buttonWidth - (2 * _padding);
                _comboTextBox.Height = _comboTextBox.SingleLineHeight; // Use exact minimum height
            }

            if (_dropDownButton != null)
            {
                _dropDownButton.Location = new Point(clientRect.Width - _buttonWidth - _padding, _padding);
                _dropDownButton.Width = _buttonWidth;
                _dropDownButton.Height = _collapsedHeight - (2 * _padding);
                SetDropDownButtonImage();
            }

            if (_isPopupOpen && menuDialog != null)
            {
                menuDialog.ShowPopup(this, BeepPopupFormPosition.Bottom);
            }

            ResumeLayout(true);
            Invalidate();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            BackColor = _currentTheme.ComboBoxBackColor;

            if (_comboTextBox != null)
            {
                _comboTextBox.BackColor = _currentTheme.ComboBoxBackColor;
                _comboTextBox.ForeColor = _currentTheme.ComboBoxForeColor;
                _comboTextBox.BorderStyle = BorderStyle.None;
                _comboTextBox.Multiline = false;
                _comboTextBox.AutoSize = false;
            }

            if (_dropDownButton != null)
            {
                _dropDownButton.BackColor = _currentTheme.ButtonBackColor;
                _dropDownButton.ForeColor = _currentTheme.ButtonForeColor;
                _dropDownButton.Theme = Theme;
                SetDropDownButtonImage();
            }

            if (_beepListBox != null)
            {
                _beepListBox.Theme = Theme;
            }

            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                if (_comboTextBox != null)
                {
                    _comboTextBox.TextFont = _textFont;
                }
                Font = _textFont;
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            Rectangle rectangle = ClientRectangle;
            rectangle.Inflate(-1, -1);
            Draw(g, rectangle);
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            try
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                using (Region clipRegion = new Region(rectangle))
                {
                    graphics.Clip = clipRegion;

                    using (SolidBrush backgroundBrush = new SolidBrush(_currentTheme.ComboBoxBackColor))
                    {
                        graphics.FillRectangle(backgroundBrush, rectangle);
                    }

                    string textToDraw = _comboTextBox.Visible ? _comboTextBox.Text : (SelectedItem?.Text ?? string.Empty);
                    if (!string.IsNullOrEmpty(textToDraw))
                    {
                        Rectangle textRect = new Rectangle(
                            rectangle.X + _padding,
                            rectangle.Y + _padding,
                            rectangle.Width - _buttonWidth - (2 * _padding),
                            rectangle.Height - (2 * _padding));

                        TextRenderer.DrawText(
                            graphics,
                            textToDraw,
                            _textFont,
                            textRect,
                            _currentTheme.ComboBoxForeColor,
                            TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                    }

                    graphics.ResetClip();
                }
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Error in BeepComboBox.Draw: {ex.Message}");
            }
        }

        public override void SetValue(object value)
        {
            if (value is SimpleItem item)
            {
                if (ListItems.Contains(item))
                {
                    SelectedItem = item;
                }
            }
            else if (value is string text)
            {
                SelectedItem = ListItems.FirstOrDefault(x => x.Text == text);
            }
            Invalidate();
        }

        public override object GetValue()
        {
            return SelectedItem;
        }

        #region Popup List Methods (Unchanged from Original)
        private void ToggleMenu()
        {
            if (_isExpanded)
                Collapse();
            else
                Expand();
            SetDropDownButtonImage();
        }

        private void SetDropDownButtonImage()
        {
            _dropDownButton.ImagePath = _isPopupOpen
                ? "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-up.svg"
                : "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-down.svg";
        }

        private void Expand()
        {
            if (!_isPopupOpen)
                ShowPopup();
        }

        private void Collapse()
        {
            if (_isPopupOpen)
                ClosePopup();
        }

        private void TogglePopup()
        {
            if (_isPopupOpen)
                ClosePopup();
            else
                ShowPopup();
        }

        public void ShowPopup()
        {
            if (_isPopupOpen || ListItems.Count == 0)
                return;

            menuDialog = new BeepPopupListForm(ListItems.ToList())
            {
                Theme = Theme
            };
            menuDialog.SelectedItemChanged += MenuDialog_SelectedItemChanged;
            menuDialog.ShowPopup(Text, this, BeepPopupFormPosition.Bottom);
            _isPopupOpen = true;
            _isExpanded = true;
            PopupOpened?.Invoke(this, EventArgs.Empty);
            SetDropDownButtonImage();
        }

        public void ClosePopup()
        {
            if (!_isPopupOpen)
                return;

            if (menuDialog != null)
            {
                menuDialog.SelectedItemChanged -= MenuDialog_SelectedItemChanged;
                menuDialog.Close();
                menuDialog.Dispose();
                menuDialog = null;
            }
            _isPopupOpen = false;
            _isExpanded = false;
            PopupClosed?.Invoke(this, EventArgs.Empty);
            SetDropDownButtonImage();
        }

        private void MenuDialog_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            SelectedItem = e.SelectedItem;
            ClosePopup();
        }
        #endregion Popup List Methods

        private void StartEditing()
        {
            if (_isEditing || _isPopupOpen)
                return;

            _isEditing = true;
            _comboTextBox.Visible = true;
            _comboTextBox.Text = SelectedItem?.Text ?? string.Empty;
            _comboTextBox.Focus();
            _comboTextBox.SelectAll();
            Invalidate();
        }

        private void EndEditing()
        {
            if (!_isEditing)
                return;

            _isEditing = false;
            _comboTextBox.Visible = false;
            if (!string.IsNullOrEmpty(_comboTextBox.Text))
            {
                var item = ListItems.FirstOrDefault(x => x.Text == _comboTextBox.Text);
                if (item != null)
                {
                    SelectedItem = item;
                }
            }
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            EndEditing();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_dropDownButton != null && !_dropDownButton.ClientRectangle.Contains(e.Location))
            {
                StartEditing();
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent != null)
            {
                Parent.Invalidated += (s, ev) => Invalidate();
                Parent.Resize += (s, ev) => Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClosePopup();
                _comboTextBox?.Dispose();
                _dropDownButton?.Dispose();
                _beepListBox?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}