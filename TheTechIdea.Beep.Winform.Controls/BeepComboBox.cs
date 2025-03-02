using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Utilities;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
// using TheTechIdea.Beep.Desktop.Common; // if needed

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// A stable combo box that:
    ///  - Computes collapsedHeight from beepTextBox's SingleLineHeight + padding
    ///  - Prevents vertical resizing in collapsed mode
    ///  - Allows expansion to show beepListBox, then repositions if user changes width
    ///  - Ensures button & text box remain the same height
    /// </summary>
    /// 
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
        private BeepListBox _beepListBox;
        private bool _isExpanded;

        private int _collapsedHeight = 0;
        private int _buttonWidth = 25;
        private int _maxListHeight = 200;
        private int _padding = 2;
        private int _extraspace = 1; // extra space for the dropdown button

        // If you like, define a min or max width
        private int _minWidth = 80;



        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
        protected virtual void OnSelectedItemChanged(SimpleItem selectedItem)
        {
            SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(selectedItem));
        }

        // Delegate beepListBox CurrentMenutems
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _beepListBox.ListItems;
            set
            {
                _beepListBox.ListItems = value;
                _beepListBox.InitializeMenu();
            }
        }
        public SimpleItem SelectedItem
        {
            get => _beepListBox.SelectedItem;
            set
            {
                if (value == null)
                {
                    // Reset case: clear selection
                    _beepListBox.SelectedItem = null;
                    _beepListBox.SelectedIndex = -1; // Ensure no item is selected
                    _comboTextBox.Text = string.Empty; // Clear the text display
                    OnSelectedItemChanged(null); // Notify listeners of reset
                }
                else if (_beepListBox.ListItems != null && _beepListBox.ListItems.Count > 0)
                {
                    // Set to a valid item
                    int index = _beepListBox.ListItems.IndexOf(value);
                    if (index >= 0)
                    {
                        _beepListBox.SelectedItem = value;
                        SelectedIndex = index; // This will update text and raise event
                    }
                    // If value isn't in the list, do nothing or optionally add it
                }
                Invalidate(); // Ensure redraw in grid
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _beepListBox.SelectedIndex;
            set
            {
                if (_beepListBox.ListItems == null || _beepListBox.ListItems.Count == 0)
                {
                    _beepListBox.SelectedIndex = -1;
                    _comboTextBox.Text = string.Empty;
                    _beepListBox.SelectedItem = null;
                    OnSelectedItemChanged(null);
                }
                else if (value >= -1 && value < _beepListBox.ListItems.Count)
                {
                    _beepListBox.SelectedIndex = value;
                    if (value == -1)
                    {
                        // Reset case
                        _beepListBox.SelectedItem = null;
                        _comboTextBox.Text = string.Empty;
                        OnSelectedItemChanged(null);
                    }
                    else
                    {
                        // Valid selection
                        _beepListBox.SelectedItem = _beepListBox.ListItems[value];
                        _comboTextBox.Text = _beepListBox.ListItems[value].Text;
                        OnSelectedItemChanged(_beepListBox.SelectedItem);
                    }
                }
                Invalidate(); // Ensure redraw in grid
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
        private BeepPopupFormPosition _beepPopupFormPosition = BeepPopupFormPosition.Bottom;
        [Browsable(true)]
        [Category("Appearance")]

        public BeepPopupFormPosition PopPosition
        {
            get { return _beepPopupFormPosition; }
            set { _beepPopupFormPosition = value; }

        }
        public DbFieldCategory Category { get; set; } = DbFieldCategory.Numeric;

        public BeepComboBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
             ControlStyles.UserPaint |
             ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            // Ensure child controls don't handle their own painting when in a grid
           
            // If user did not specify size, define default
            if (Width < _minWidth) Width = 150;

            // 1) Create & add beepTextBox
            _comboTextBox = new BeepTextBox { IsChild = true,ShowAllBorders=false,ReadOnly=true };
            // Toggle dropdown if user clicks text
           // _comboTextBox.Click += (s, e) => ToggleMenu();
            Controls.Add(_comboTextBox);

            // 2) Compute collapsed height from beepTextBox SingleLineHeight
            GetControlHeight();

            // If no height specified or too small, set to collapsed
            if (Height < _collapsedHeight)
                Height = _collapsedHeight;

            _isExpanded = false;

            // 3) Create & add dropDownButton
            _dropDownButton = new BeepButton { 
                IsChild = true,
                IsBorderAffectedByTheme=false,
                IsRoundedAffectedByTheme=false,
                IsShadowAffectedByTheme=false,
                ShowAllBorders = false,
                ShowShadow = false,
                HideText=true,
                ImageAlign = ContentAlignment.MiddleCenter,
                TextAlign = ContentAlignment.MiddleCenter,
                 TextImageRelation= TextImageRelation.ImageBeforeText
            };
            _dropDownButton.Click += (s, e) => ToggleMenu();
            Controls.Add(_dropDownButton);
            SetDropDownButtonImage();
            // 4) Create beepListBox
            _beepListBox = new BeepListBox
            {
                IsChild = false,
                Visible = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsRoundedAffectedByTheme = false,
                ImageSize =  20,
                ShowTitle= false,
            };
            _beepListBox.ItemClicked += (sender, item) =>
            {
                _comboTextBox.Text = item.Text;
                SelectedIndex = _beepListBox.ListItems.IndexOf(item);
                Collapse();
            };
            Controls.Add(_beepListBox);

            // Theming, etc.
            ApplyTheme();
        }
        public void Reset()
        {
            SelectedItem = null; // This will trigger the reset logic in the setter
        }
        private void ToggleMenu()
        {
            SetDropDownButtonImage();
            if (_isExpanded)
                Collapse();
            else
                Expand();
        }
        /// <summary>
        /// Sets the dropdown button's image based on the expansion state.
        /// </summary>
        private void SetDropDownButtonImage()
        {
            if (_isExpanded)
            {
                _dropDownButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-up.svg";
            }
            else
            {
                _dropDownButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.angle-small-down.svg";
            }
        }
        private void Expand()
        {
            _isExpanded = true;
            this.BringToFront();
            if (IsPopupOpen)
            {
                TogglePopup();
            }
           
        }

        private void Collapse()
        {
            _isExpanded = false;
            TogglePopup();
        }

        #region Resizing Logic

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (Width < _minWidth)
                Width = _minWidth;

            if (_comboTextBox == null) return;

            GetControlHeight();
            if (!_isExpanded && Height != _collapsedHeight)
                Height = _collapsedHeight;

            UpdateDrawingRect();
            int topSectionHeight = _collapsedHeight;

            // Position components relative to control's client area
            if (_comboTextBox != null)
            {
                _comboTextBox.Location = new Point(_padding, _padding);
                _comboTextBox.Width = Math.Max(1, Width - _buttonWidth - (_padding * 2));
                _comboTextBox.Height = Math.Max(1, topSectionHeight - (_padding * 2));
            }

            if (_dropDownButton != null)
            {
                _dropDownButton.Location = new Point(Width - _buttonWidth - _padding, _padding);
                _dropDownButton.Width = _buttonWidth;
                _dropDownButton.Height = _comboTextBox.Height;
                _dropDownButton.Text = _isExpanded ? "▲" : "▼";
            }

            if (_beepListBox != null)
            {
                _beepListBox.Visible = _isExpanded;
                if (_isExpanded)
                {
                    _beepListBox.Location = new Point(_padding, topSectionHeight + _padding);
                    _beepListBox.Width = Math.Max(1, Width - (_padding * 2));
                    _beepListBox.Height = Math.Max(1, Height - topSectionHeight - _padding);
                }
            }

            Invalidate(); // Force redraw after resize
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // Enforce minWidth
            if ((specified & BoundsSpecified.Width) == BoundsSpecified.Width && width < _minWidth)
            {
                width = _minWidth;
            }

            // If not expanded, forcibly revert height to collapsed
            if (!_isExpanded && (specified & BoundsSpecified.Height) == BoundsSpecified.Height)
            {
               
                height = GetControlHeight(); 
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        #endregion
        private int GetControlHeight()
        {
            int singleLine = Math.Max(1, _comboTextBox.SingleLineHeight);
            _collapsedHeight = singleLine + (_padding * 2) + _extraspace;
            return _collapsedHeight;
        }
        public override void ApplyTheme()
        {
            base.ApplyTheme();
        //    if (_comboTextBox != null) _comboTextBox.Theme = Theme;
            if (_dropDownButton != null) _dropDownButton.Theme = Theme;
            if (_beepListBox != null) _beepListBox.Theme = Theme;
            BackColor = _currentTheme.PanelBackColor;
            _comboTextBox.BackColor = _currentTheme.PanelBackColor;
            _comboTextBox.ForeColor = _currentTheme.TitleForColor;
        }
        #region "Binding and Control Type"


        #endregion "Binding and Control Type"
        #region "IBeepComponent"

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            try
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Store and set clip region
                Region originalClip = graphics.Clip;
                graphics.SetClip(rectangle);

                // Draw background
                using (SolidBrush backgroundBrush = new SolidBrush(_currentTheme.PanelBackColor))
                {
                    graphics.FillRectangle(backgroundBrush, rectangle);
                }

                // Draw border if applicable
                if (BorderThickness > 0)
                {
                    using (Pen borderPen = new Pen(_currentTheme.BorderColor, BorderThickness))
                    {
                        Rectangle borderRect = rectangle;
                        borderRect.Inflate(-BorderThickness / 2, -BorderThickness / 2);
                        graphics.DrawRectangle(borderPen, borderRect);
                    }
                }

                // Define areas for text and dropdown indicator
                int buttonWidth = _buttonWidth; // Use the control's button width
                Rectangle textRect = new Rectangle(
                    rectangle.X + _padding,
                    rectangle.Y + _padding,
                    rectangle.Width - buttonWidth - (2 * _padding),
                    rectangle.Height - (2 * _padding));

                Rectangle buttonRect = new Rectangle(
                    rectangle.Right - buttonWidth - _padding,
                    rectangle.Y + _padding,
                    buttonWidth,
                    rectangle.Height - (2 * _padding));

                // Draw text (selected item or textbox content)
                string textToDraw = _comboTextBox?.Text ?? SelectedItem?.Text ?? string.Empty;
                if (!string.IsNullOrEmpty(textToDraw))
                {
                    TextRenderer.DrawText(
                        graphics,
                        textToDraw,
                        Font,
                        textRect,
                        _currentTheme.TitleForColor,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                }

                // Draw dropdown indicator (simple triangle)
                using (SolidBrush buttonBrush = new SolidBrush(_currentTheme.ButtonBackColor))
                {
                    graphics.FillRectangle(buttonBrush, buttonRect);
                }

                using (Pen arrowPen = new Pen(_currentTheme.ButtonForeColor, 2))
                {
                    PointF[] arrowPoints = _isExpanded
                        ? new PointF[] // Up arrow
                        {
                    new PointF(buttonRect.X + buttonRect.Width / 4, buttonRect.Bottom - buttonRect.Height / 4),
                    new PointF(buttonRect.X + buttonRect.Width / 2, buttonRect.Y + buttonRect.Height / 4),
                    new PointF(buttonRect.Right - buttonRect.Width / 4, buttonRect.Bottom - buttonRect.Height / 4)
                        }
                        : new PointF[] // Down arrow
                        {
                    new PointF(buttonRect.X + buttonRect.Width / 4, buttonRect.Y + buttonRect.Height / 4),
                    new PointF(buttonRect.X + buttonRect.Width / 2, buttonRect.Bottom - buttonRect.Height / 4),
                    new PointF(buttonRect.Right - buttonRect.Width / 4, buttonRect.Y + buttonRect.Height / 4)
                        };
                    graphics.DrawLines(arrowPen, arrowPoints);
                }

                // Draw button border
                using (Pen buttonBorderPen = new Pen(_currentTheme.BorderColor, 1))
                {
                    graphics.DrawRectangle(buttonBorderPen, buttonRect);
                }

                // Restore graphics state
                graphics.Clip = originalClip;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BeepComboBox.Draw: {ex.Message}");
            }
        }
        public override void SetValue(object value)
        {
            if (value is SimpleItem item)
            {
                // check if item exists in the list
                if (ListItems.Contains(item))
                {
                    SelectedItem = item;
                }
               
            }
        }
        public override object GetValue()
        {
            return SelectedItem;
        }
        #endregion "IBeepComponent"
        #region "Popup List Methods"
        BeepPopupListForm menuDialog;
        private Color tmpfillcolor;
        private Color tmpstrokecolor;
        private bool _isPopupOpen=false;

        private void TogglePopup()
        {
            if (_isPopupOpen)
                ClosePopup();
            else
                ShowPopup();
        }
        public void ShowPopup()
        {
            if (_isPopupOpen) return;
            if (ListItems.Count == 0)
            {
                return;
            }
            menuDialog = new BeepPopupListForm(ListItems.ToList());

            menuDialog.Theme = Theme;

            menuDialog.SelectedItemChanged += MenuDialog_SelectedItemChanged;
            SimpleItem x = menuDialog.ShowPopup(Text, this, _beepPopupFormPosition);
            _isPopupOpen = true;
            PopupOpened?.Invoke(this, EventArgs.Empty);
        }

        private void MenuDialog_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            SelectedItem = e.SelectedItem;
            _comboTextBox.Text = e.SelectedItem.Text;
            OnSelectedItemChanged(e.SelectedItem);
            ClosePopup();
        }
        public void ClosePopup()
        {
            _isPopupOpen = false;
            if (menuDialog != null)
            {
                menuDialog.SelectedItemChanged -= MenuDialog_SelectedItemChanged;
                menuDialog.Close();
                menuDialog.Dispose();
                menuDialog = null;
            }
        }
        #endregion "Popup List Methods"
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent != null)
            {
                Parent.Invalidated += (s, ev) => Invalidate();
                Parent.Resize += (s, ev) => Invalidate();
            }
        }
        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{
        //    if (keyData == Keys.Enter && _isPopupOpen)
        //    {
        //        if (_beepListBox.SelectedItem != null)
        //        {
        //            MenuDialog_SelectedItemChanged(this,
        //                new SelectedItemChangedEventArgs(_beepListBox.SelectedItem));
        //        }
        //        return true;
        //    }
        //    else if (keyData == Keys.Down && !_isPopupOpen)
        //    {
        //        ToggleMenu();
        //        return true;
        //    }
        //    if(keyData == Keys.Escape && _isPopupOpen)
        //    {
        //        ClosePopup();
        //        return true;
        //    }
        //    if(keyData==Keys.Tab && _isPopupOpen)
        //    {
        //        ClosePopup();
        //        return true;
        //    }
        //    if(keyData == Keys.Tab && !_isPopupOpen)
        //    {
              
        //        return false;
        //    }
        //    return base.ProcessCmdKey(ref msg, keyData);
        //}
        //#region Keyboard Event Handlers
       

        //// Provide visual feedback when the control receives focus
        //protected override void OnGotFocus(EventArgs e)
        //{
        //    base.OnGotFocus(e);
        //    Invalidate(); // Redraw to show focus indication
        //}

        //protected override void OnLostFocus(EventArgs e)
        //{
        //    base.OnLostFocus(e);
        //    Invalidate(); // Redraw to remove focus indication
        //}

        //// Optional: Handle arrow keys or other navigation if desired
        //protected override bool IsInputKey(Keys keyData)
        //{
        //    // Allow arrow keys and other navigation keys to be processed
        //    if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
        //    {
        //        return true;
        //    }
        //    return base.IsInputKey(keyData);
        //}
        //#endregion
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
