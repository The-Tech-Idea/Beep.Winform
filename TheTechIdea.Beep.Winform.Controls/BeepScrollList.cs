using System;
using System.Collections.Generic;
using System.ComponentModel;

using TheTechIdea.Beep.Winform.Controls.Models;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Scroll List")]
    [Description("A carousel-style scrollable list control using BeepButtons for items, supporting vertical or horizontal orientation without a visible scrollbar.")]
    public class BeepScrollList : BeepControl
    {
        #region Fields
        private BindingList<SimpleItem> _listItems = new BindingList<SimpleItem>();
        private List<BeepButton> _buttonItems = new List<BeepButton>();
        private int _itemHeight = 30; // Height for vertical, width for horizontal
        private int _itemWidth = 100; // Width for vertical orientation
        private int _visibleItemsCount;
        private float _scrollOffset = 0; // Float for smoother scrolling
        private int _selectedIndex = -1;
        private ScrollOrientation _orientation = ScrollOrientation.VerticalScroll;
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private float _dragStartOffset;
        private int padding = 3;
        private const float ScrollSpeed = 0.5f; // Adjust for scroll sensitivity
        #endregion

        #region Properties
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

                _textFont = value;
                UseThemeFont = false;
                Invalidate();


            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The list of items to display.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> ListItems
        {
            get => _listItems;
            set
            {
                _listItems = value ?? new BindingList<SimpleItem>();
                UpdateButtonItems();
                UpdateScrollBounds();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The size of each item (height for vertical, width for horizontal).")]
        [DefaultValue(30)]
        public int ItemHeight
        {
            get => _itemHeight;
            set
            {
                _itemHeight = value;
                UpdateButtonItems();
                UpdateScrollBounds();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The width of each item in vertical orientation (ignored in horizontal orientation).")]
        [DefaultValue(100)]
        public int ItemWidth
        {
            get => _itemWidth;
            set
            {
                _itemWidth = value;
                UpdateButtonItems();
                UpdateScrollBounds();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The orientation of the scroll list (vertical or horizontal).")]
        [DefaultValue(ScrollOrientation.VerticalScroll)]
        public ScrollOrientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                UpdateButtonItems();
                UpdateScrollBounds();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The index of the currently selected item.")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= -1 && value < _listItems.Count)
                {
                    if (_selectedIndex >= 0 && _selectedIndex < _buttonItems.Count)
                    {
                        _buttonItems[_selectedIndex].IsSelected = false;
                    }

                    _selectedIndex = value;

                    if (_selectedIndex >= 0 && _selectedIndex < _buttonItems.Count)
                    {
                        _buttonItems[_selectedIndex].IsSelected = true;
                    }

                    OnItemSelected();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The currently selected item.")]
        public SimpleItem SelectedItem
        {
            get => _selectedIndex >= 0 && _selectedIndex < _listItems.Count ? _listItems[_selectedIndex] : null;
            set
            {
                int index = _listItems.IndexOf(value);
                if (index >= 0)
                {
                    SelectedIndex = index;
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler<SimpleItem> ItemSelected;

        protected virtual void OnItemSelected()
        {
            ItemSelected?.Invoke(this, SelectedItem);
        }
        #endregion

        #region Constructor
        public BeepScrollList()
        {
            Padding = new Padding(2);
            InitializeControl();
            ListItems.ListChanged += Items_ListChanged;
        }

        private void Items_ListChanged(object sender, ListChangedEventArgs e) => UpdateButtonItems();

        private void InitializeControl()
        {
            DoubleBuffered = true;
            UpdateScrollBounds();
        }
        #endregion

        #region Button Management
        public void UpdateButtonItems()
        {
            foreach (var button in _buttonItems)
            {
                button.Click -= Button_Click;
                Controls.Remove(button);
                button.Dispose();
            }
            _buttonItems.Clear();

            for (int i = 0; i < _listItems.Count; i++)
            {
                var item = _listItems[i];
                var button = new BeepButton
                {
                    Text = item.Text,
                    IsChild = true,
                    ShowAllBorders = false,
                    ShowTopBorder = false,
                    ShowBottomBorder = false,
                    ShowLeftBorder = false,
                    ShowRightBorder = false,
                    IsShadowAffectedByTheme = false,
                    Theme = Theme,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ShowFocusIndicator = false,
                    BorderThickness = 0
                };
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    button.ImagePath = item.ImagePath;
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    button.TextImageRelation = TextImageRelation.ImageBeforeText;
                }
                if (_orientation == ScrollOrientation.VerticalScroll)
                {
                    int contentWidth = DrawingRect.Width - (this.Padding.Left + this.Padding.Right);
                    int buttonWidth = _itemWidth;

                    button.Size = new Size(buttonWidth, _itemHeight);
                }
                else
                {
                    int contentHeight = DrawingRect.Height - (this.Padding.Top + this.Padding.Bottom);
                    int buttonHeight = DrawingRect.Height - (this.Padding.Top + this.Padding.Bottom);
                    button.Size = new Size(_itemWidth, buttonHeight);
                   
                }
                
                button.Theme = Theme;
                //button.IsRounded = IsRounded;
                //button.Margin = new Padding(0);
                //button.Padding = new Padding(0);
                button.Click += Button_Click;
                _buttonItems.Add(button);
                Controls.Add(button);
            }

            if (_selectedIndex >= 0 && _selectedIndex < _buttonItems.Count)
            {
                _buttonItems[_selectedIndex].IsSelected = true;
            }

            UpdateButtonPositions();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            var button = sender as BeepButton;
            if (button != null)
            {
                int index = _buttonItems.IndexOf(button);
                if (index >= 0)
                {
                    SelectedIndex = index;
                }
            }
        }

        private void UpdateButtonPositions()
        {
            int startIndex = (int)Math.Floor(_scrollOffset);
            float offsetFraction = _scrollOffset - startIndex;
            int endIndex = Math.Min(startIndex + _visibleItemsCount + 1, _listItems.Count);

            // Calculate the total size of items to be displayed
            float totalItemsSize = _listItems.Count * _itemHeight;
            float visibleAreaSize = _orientation == ScrollOrientation.VerticalScroll
                ? DrawingRect.Height - (this.Padding.Top + this.Padding.Bottom)
                : DrawingRect.Width - (this.Padding.Left + this.Padding.Right);

            // Centering offset for the scrolling direction
            float centeringOffsetAlongScroll = 0;
            if (totalItemsSize < visibleAreaSize)
            {
                centeringOffsetAlongScroll = (visibleAreaSize - totalItemsSize) / 2;
            }

            // Centering offset for the perpendicular direction
            float centeringOffsetPerpendicular = 0;
            if (_orientation == ScrollOrientation.VerticalScroll)
            {
                int contentWidth = DrawingRect.Width - (this.Padding.Left + this.Padding.Right);
                int buttonWidth = _itemWidth;
                if (contentWidth > buttonWidth)
                {
                    centeringOffsetPerpendicular = (contentWidth - buttonWidth) / 2;
                }
            }
            else
            {
                int contentHeight = DrawingRect.Height - (this.Padding.Top + this.Padding.Bottom);
                int buttonHeight = DrawingRect.Height - (this.Padding.Top + this.Padding.Bottom);
                if (contentHeight > buttonHeight)
                {
                    centeringOffsetPerpendicular = (contentHeight - buttonHeight) / 2;
                }
            }

            for (int i = 0; i < _buttonItems.Count; i++)
            {
                var button = _buttonItems[i];
                bool isVisible = i >= startIndex && i < endIndex;
                button.Visible = isVisible;

                if (!isVisible) continue;

                float position = i * _itemHeight - (_scrollOffset * _itemHeight);

                if (_orientation == ScrollOrientation.VerticalScroll)
                {
                    int xPos = (int)(this.Padding.Left + centeringOffsetPerpendicular);
                    int yPos = (int)(this.Padding.Top + centeringOffsetAlongScroll + position);
                    button.Location = new Point(xPos, yPos);
                    button.Size = new Size(_itemWidth, _itemHeight);
                }
                else
                {
                    int xPos = (int)(this.Padding.Left + centeringOffsetAlongScroll + position);
                    int yPos = (int)(this.Padding.Top + centeringOffsetPerpendicular);
                    button.Location = new Point(xPos, yPos);
                    button.Size = new Size(_itemHeight, DrawingRect.Height - (this.Padding.Top + this.Padding.Bottom));
                }
            }
        }
        #endregion

        #region Layout and Drawing
        private void UpdateScrollBounds()
        {
            UpdateDrawingRect();
            if (_orientation == ScrollOrientation.VerticalScroll)
            {
                int contentHeight = DrawingRect.Height - (this.Padding.Top + this.Padding.Bottom);
                _visibleItemsCount = (int)Math.Ceiling((float)contentHeight / _itemHeight);
            }
            else
            {
                int contentWidth = DrawingRect.Width - (this.Padding.Left + this.Padding.Right);
                _visibleItemsCount = (int)Math.Ceiling((float)contentWidth / _itemHeight);
            }

            _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, Math.Max(0, _listItems.Count - _visibleItemsCount)));
            UpdateButtonPositions();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateButtonItems();
            UpdateScrollBounds();
            Invalidate();
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
        #endregion

        #region Mouse and Scroll Interaction
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _dragStartPoint = e.Location;
                _dragStartOffset = _scrollOffset;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDragging)
            {
                float delta = _orientation == ScrollOrientation.VerticalScroll
                    ? (e.Y - _dragStartPoint.Y) / (float)_itemHeight
                    : (e.X - _dragStartPoint.X) / (float)_itemHeight;
                _scrollOffset = _dragStartOffset - delta;
                _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, Math.Max(0, _listItems.Count - _visibleItemsCount)));
                UpdateButtonPositions();
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = false;
            }
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;
            //// Console.WriteLine("Font Changed");
            if (AutoSize)
            {
                Size textSize = TextRenderer.MeasureText(Text, _textFont);
                this.Size = new Size(textSize.Width + Padding.Horizontal, textSize.Height + Padding.Vertical);
            }
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            float delta = (e.Delta / 120f) * ScrollSpeed;
            _scrollOffset -= delta;
            _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, Math.Max(0, _listItems.Count - _visibleItemsCount)));
            UpdateButtonPositions();
            Invalidate();
        }
        #endregion

        #region Value Management
        public override void SetValue(object value)
        {
            if (value is SimpleItem item)
            {
                SelectedItem = item;
            }
            else if (value != null)
            {
                var item1 = ListItems.FirstOrDefault(i => i.Value?.ToString() == value.ToString());
                if (item1 != null)
                {
                    SelectedItem = item1;
                }
            }
        }

        public override object GetValue()
        {
            return SelectedItem;
        }
        #endregion

        #region Theme and Disposal
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (Theme == null) return;
            if (IsChild)
            {
                BackColor = Parent.BackColor;
                ParentBackColor = BackColor;
            }
            else
            {
                BackColor =_currentTheme.ListBackColor ;

            }
            foreach (var button in _buttonItems)
            {
                TextFont = BeepThemesManager.ToFont(_currentTheme.BodySmall);
                Font = _textFont;
                button.Font = _textFont;
                // button.Theme = Theme;
                button.IsChild = true;
                button.UseScaledFont = true;
                button.ParentBackColor = BackColor;
                button.BackColor = _currentTheme.ListBackColor;
                button.ForeColor = _currentTheme.ListItemForeColor;
                button.HoverBackColor = _currentTheme.ListItemHoverBackColor;
                button.HoverForeColor = _currentTheme.ListItemHoverForeColor;
                button.SelectedBackColor = _currentTheme.ListItemSelectedBackColor;
                button.SelectedForeColor = _currentTheme.ListItemSelectedForeColor;
                button.DisabledBackColor = _currentTheme.DisabledBackColor;
                button.DisabledForeColor = _currentTheme.DisabledForeColor;
                button.FocusBackColor = _currentTheme.ListItemSelectedBackColor;
                button.FocusForeColor = _currentTheme.ListItemSelectedForeColor;
            }
           
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var button in _buttonItems)
                {
                    button.Click -= Button_Click;
                    Controls.Remove(button);
                    button.Dispose();
                }
                _buttonItems.Clear();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}