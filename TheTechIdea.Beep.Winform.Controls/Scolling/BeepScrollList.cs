using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;

using System.Windows.Forms;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Layouts.Helpers;



namespace TheTechIdea.Beep.Winform.Controls.Scolling
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Scroll List")]
    [Description("A carousel-Style scrollable list control using BeepButtons for items, supporting vertical or horizontal orientation without a visible scrollbar.")]
    public class BeepScrollList : BeepPanel
    {
        protected override Size DefaultSize => BeepLayoutMetrics.ScrollList;
        protected internal override Padding StylePadding => new Padding(0);
        #region Fields
        private BindingList<SimpleItem> _listItems = new BindingList<SimpleItem>();
        private BeepImage _image;    // Single image instance for drawing item icons
        private int _itemHeight = 30; // Height for vertical, width for horizontal
        private int _itemWidth = 100; // Width for vertical orientation
        private int _visibleItemsCount;
        private float _scrollOffset = 0; // Float for smoother scrolling
        private float _targetScrollOffset = 0; // Target position for smooth scrolling
        private int _selectedIndex = -1;
        private ScrollOrientation _orientation = ScrollOrientation.VerticalScroll;
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private float _dragStartOffset;
        private int padding = 3;
        private const float ScrollSpeed = 0.5f; // Adjust for scroll sensitivity
        private Timer _animationTimer;
        private const float ScrollAnimationSpeed = 0.15f; // Lower = slower, smoother animation
        private int _itemSpacing = 2; // Space between items
        private SimpleItem _hoveredItem;  // Track hovered item
        private bool _showCheckBox = false;
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Shows checkboxes next to list items")]
        [DefaultValue(false)]
        public bool ShowCheckBox
        {
            get => _showCheckBox;
            set
            {
                _showCheckBox = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text alignment for list items")]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment ItemTextAlign { get; set; } = ContentAlignment.MiddleLeft;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Padding for list items")]
        public Padding ItemPadding { get; set; } = new Padding(8, 0, 8, 0);

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show scroll indicators")]
        [DefaultValue(true)]
        public bool ShowScrollIndicators { get; set; } = true;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Custom colour of the scroll indicators; Empty follows the theme's scrollbar thumb slot.")]
        public Color ScrollIndicatorColor { get; set; } = Color.Empty;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Space between list items")]
        [DefaultValue(2)]
        public int ItemSpacing
        {
            get => _itemSpacing;
            set
            {
                _itemSpacing = value;
                UpdateVirtualItems();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Enable keyboard navigation")]
        [DefaultValue(true)]
        public bool EnableKeyboardNavigation { get; set; } = true;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Enable hover effects for list items")]
        [DefaultValue(true)]
        public bool EnableItemHoverEffects { get; set; } = true;

        private Font _textFont = SystemFonts.DefaultFont;
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
                UpdateVirtualItems();
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
                UpdateVirtualItems();
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
                UpdateVirtualItems();
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
                UpdateVirtualItems();
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
                UpdateVirtualItems();
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
                    _selectedIndex = value;
                    UpdateSelectedState();
                    ScrollToItem(_selectedIndex); // Auto-scroll to selected item
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

            // Enable hit testing
            HitAreaEventOn = true;

            _image = new BeepImage
            {
                IsChild = true,
                Dock = DockStyle.None,
                Margin = new Padding(0)
            };


            // Accessibility
            AccessibleRole = AccessibleRole.List;
            AccessibleName = "Scroll List";
            AccessibleDescription = "A scrollable list of items";
        }

        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            UpdateVirtualItems();
            UpdateScrollBounds();
            Invalidate();
        }

        private void InitializeControl()
        {
            DoubleBuffered = true;
            ShowTitle = false;
            ShowTitleLine = false;

            // Initialize animation timer
            _animationTimer = new Timer();
            _animationTimer.Interval = 16; // ~60fps
            _animationTimer.Tick += AnimationTimer_Tick;
            _animationTimer.Start();

            UpdateScrollBounds();
        }
        #endregion

        #region Virtual Button Management
        public void UpdateVirtualItems()
        {
            UpdateItemPositions();
        }

        private void UpdateSelectedState()
        {
            // Clear previous selection state
            foreach (var item in _listItems)
            {
                item.IsSelected = false;
            }

            // Set new selection
            if (_selectedIndex >= 0 && _selectedIndex < _listItems.Count)
            {
                _listItems[_selectedIndex].IsSelected = true;
            }
        }

        /// <summary>
        /// The ONE item-rect authority. Drawing, hit areas and clicks all use it - the
        /// previous three copies disagreed (a centred pre-paint layout vs the drawn row
        /// layout vs a third copy in OnMouseClick), so hit rects never matched the rows
        /// and every click selected twice.
        /// </summary>
        private Rectangle GetItemRect(int i)
        {
            float itemPosition = (i - _scrollOffset) * (_itemHeight + _itemSpacing);
            if (_orientation == ScrollOrientation.VerticalScroll)
            {
                int yPos = DrawingRect.Top + Padding.Top + (int)itemPosition;
                return new Rectangle(DrawingRect.Left, yPos, DrawingRect.Width, _itemHeight);
            }

            int xPos = DrawingRect.Left + Padding.Left + (int)itemPosition;
            return new Rectangle(xPos, DrawingRect.Top, _itemHeight, DrawingRect.Height);
        }

        private void UpdateItemPositions()
        {
            ClearHitList();
            if (_listItems.Count == 0)
                return;

            int startIndex = Math.Max(0, (int)Math.Floor(_scrollOffset));
            int endIndex = Math.Min(startIndex + _visibleItemsCount + 2, _listItems.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                int index = i;
                AddHitArea($"Item_{index}", GetItemRect(i), null, () => ItemClicked(index));
            }
        }

        // One method decides: typing, clicks and programmatic assignment all funnel
        // through the SelectedIndex setter (selection state, auto-scroll, event, repaint).
        private void ItemClicked(int index)
        {
            if (index >= 0 && index < _listItems.Count)
            {
                SelectedIndex = index;
            }
        }
        #endregion

        #region Layout and Drawing
        // Scroll to the specified item with smooth animation
        public void ScrollToItem(int index)
        {
            if (index < 0 || index >= _listItems.Count)
                return;

            // Calculate the scroll position needed to show this item
            if (_orientation == ScrollOrientation.VerticalScroll)
            {
                int visibleHeight = DrawingRect.Height - (Padding.Top + Padding.Bottom);
                int visibleItems = Math.Max(1, visibleHeight / (_itemHeight + _itemSpacing));

                // Center the item if possible
                _targetScrollOffset = Math.Max(0, index - visibleItems / 2);

                // Ensure we don't scroll past the end
                _targetScrollOffset = Math.Min(_targetScrollOffset, Math.Max(0, _listItems.Count - visibleItems));
            }
            else
            {
                int visibleWidth = DrawingRect.Width - (Padding.Left + Padding.Right);
                int visibleItems = Math.Max(1, visibleWidth / (_itemHeight + _itemSpacing));

                // Center the item if possible
                _targetScrollOffset = Math.Max(0, index - visibleItems / 2);

                // Ensure we don't scroll past the end
                _targetScrollOffset = Math.Min(_targetScrollOffset, Math.Max(0, _listItems.Count - visibleItems));
            }
        }

        private void UpdateScrollBounds()
        {
            UpdateDrawingRect();
            if (_orientation == ScrollOrientation.VerticalScroll)
            {
                int contentHeight = DrawingRect.Height - (this.Padding.Top + this.Padding.Bottom);
                _visibleItemsCount = (int)Math.Ceiling((float)contentHeight / (_itemHeight + _itemSpacing));
            }
            else
            {
                int contentWidth = DrawingRect.Width - (this.Padding.Left + this.Padding.Right);
                _visibleItemsCount = (int)Math.Ceiling((float)contentWidth / (_itemHeight + _itemSpacing));
            }

            _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, Math.Max(0, _listItems.Count - _visibleItemsCount)));
            UpdateItemPositions();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateVirtualItems();
            UpdateScrollBounds();
            Invalidate();
        }

        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            DrawContent(graphics);
        }

        // Override DrawContent to render scroll indicators and virtual buttons
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            if (_listItems == null || _listItems.Count == 0)
                return;

            int startIndex = Math.Max(0, (int)Math.Floor(_scrollOffset));
            int endIndex = Math.Min(startIndex + _visibleItemsCount + 1, _listItems.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                var item = _listItems[i];
                Rectangle itemRect = GetItemRect(i);

                // Item colours from the theme's OWN List* family (the generic control
                // slots painted every item the panel colour; the List* slots were only
                // ever stamped into a helper button nothing drew with).
                Color backColor = _currentTheme.ListBackColor;
                Color foreColor = _currentTheme.ListItemForeColor;

                if (item.IsSelected)
                {
                    backColor = _currentTheme.ListItemSelectedBackColor;
                    foreColor = _currentTheme.ListItemSelectedForeColor;
                }
                else if (item == _hoveredItem && EnableItemHoverEffects)
                {
                    backColor = _currentTheme.ListItemHoverBackColor;
                    foreColor = _currentTheme.ListItemHoverForeColor;
                }

                using (var itemBrush = new SolidBrush(backColor))
                {
                    g.FillRectangle(itemBrush, itemRect);
                }

                // Draw checkbox if enabled
                int currentX = itemRect.Left + padding;
                if (ShowCheckBox)
                {
                    int checkSize = 16;
                    Rectangle checkboxRect = new Rectangle(
                        currentX,
                        itemRect.Top + (itemRect.Height - checkSize) / 2,
                        checkSize,
                        checkSize
                    );

                    ControlPaint.DrawCheckBox(
                        g,
                        checkboxRect,
                        item.IsSelected ? ButtonState.Checked : ButtonState.Normal
                    );

                    currentX += checkSize + padding;
                }

                // Draw image if available
                if (_image != null && !string.IsNullOrEmpty(item.ImagePath))
                {
                    _image.ImagePath = item.ImagePath;
                    Rectangle imageRect = new Rectangle(
                        currentX,
                        itemRect.Top + padding,
                        _itemHeight - (padding * 2),
                        _itemHeight - (padding * 2)
                    );
                    _image.Draw(g, imageRect);
                    currentX += imageRect.Width + padding;
                }

                // Draw text
                Rectangle textRect = new Rectangle(
                    currentX,
                    itemRect.Top,
                    itemRect.Right - currentX - padding,
                    itemRect.Height
                );

                TextRenderer.DrawText(
                    g,
                    item.Text,
                    TextFont,
                    textRect,
                    foreColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
                );
            }

            // Draw scroll indicators if needed
            if (ShowScrollIndicators && _listItems.Count > _visibleItemsCount)
            {
                DrawScrollIndicators(g);
            }
        }

        private void DrawScrollIndicators(Graphics g)
        {
            float contentSize = _listItems.Count * (_itemHeight + _itemSpacing) - _itemSpacing;
            float visibleSize = _orientation == ScrollOrientation.VerticalScroll
                ? DrawingRect.Height - (Padding.Top + Padding.Bottom)
                : DrawingRect.Width - (Padding.Left + Padding.Right);

            if (contentSize <= visibleSize)
                return;

            // Calculate the scroll percentage
            float scrollPercentage = _scrollOffset / Math.Max(1, _listItems.Count - _visibleItemsCount);
            float visiblePercentage = Math.Min(1.0f, visibleSize / contentSize);

            Color indicatorColor = ScrollIndicatorColor != Color.Empty
                ? ScrollIndicatorColor
                : _currentTheme.ScrollBarThumbColor;
            using var indicatorBrush = new SolidBrush(indicatorColor);
            if (_orientation == ScrollOrientation.VerticalScroll)
            {
                    // Vertical indicators - right side
                    int indicatorWidth = 4;
                    int right = DrawingRect.Right - 2;
                    int left = right - indicatorWidth;
                    int totalHeight = DrawingRect.Height - 10;
                    int indicatorHeight = Math.Max(20, (int)(totalHeight * visiblePercentage));
                    int top = DrawingRect.Top + 5 + (int)(scrollPercentage * (totalHeight - indicatorHeight));

                    Rectangle indicatorRect = new Rectangle(left, top, indicatorWidth, indicatorHeight);

                    if (visiblePercentage < 1)
                    {
                        using (GraphicsPath path = GetRoundedRectPath(indicatorRect, 2))
                        {
                            g.FillPath(indicatorBrush, path);
                        }
                    }
                }
                else
                {
                    // Horizontal indicators - bottom
                    int indicatorHeight = 4;
                    int bottom = DrawingRect.Bottom - 2;
                    int top = bottom - indicatorHeight;
                    int totalWidth = DrawingRect.Width - 10;
                    int indicatorWidth = Math.Max(20, (int)(totalWidth * visiblePercentage));
                    int left = DrawingRect.Left + 5 + (int)(scrollPercentage * (totalWidth - indicatorWidth));

                    Rectangle indicatorRect = new Rectangle(left, top, indicatorWidth, indicatorHeight);

                    if (visiblePercentage < 1)
                    {
                        using (GraphicsPath path = GetRoundedRectPath(indicatorRect, 2))
                        {
                            g.FillPath(indicatorBrush, path);
                        }
                    }
                }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            // Top left arc
            path.AddArc(arcRect, 180, 90);

            // Top right arc
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);

            // Bottom right arc
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);

            // Bottom left arc
            arcRect.X = rect.Left;
            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();
            return path;
        }
        #endregion

        #region Mouse and Scroll Interaction
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                // First check if a hit area (item) was clicked
                if (!HitTest(e.Location))
                {
                    // Start dragging for scrolling if not on an item
                    _isDragging = true;
                    _dragStartPoint = e.Location;
                    _dragStartOffset = _scrollOffset;
                }
                else
                {
                    // If hit test detected an item click, no need to handle drag
                    _isDragging = false;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Update hover state for items
            SimpleItem previousHovered = _hoveredItem;
            _hoveredItem = null;

            // Check if mouse is over any item to update hover state using hit testing
            if (EnableItemHoverEffects && HitList != null)
            {
                foreach (var hitArea in HitList)
                {
                    if (hitArea.TargetRect.Contains(e.Location) && hitArea.Name.StartsWith("Item_"))
                    {
                        // Extract index from hit area name
                        string indexStr = hitArea.Name.Substring(5); // "Item_X" --> "X"
                        if (int.TryParse(indexStr, out int index) && index >= 0 && index < _listItems.Count)
                        {
                            _hoveredItem = _listItems[index];
                            break;
                        }
                    }
                }
            }

            // Redraw if hover state changed
            if (_hoveredItem != previousHovered)
            {
                Invalidate();
            }

            // Handle dragging for scrolling
            if (_isDragging)
            {
                float delta = _orientation == ScrollOrientation.VerticalScroll
                    ? (e.Y - _dragStartPoint.Y) / (float)(_itemHeight + _itemSpacing)
                    : (e.X - _dragStartPoint.X) / (float)(_itemHeight + _itemSpacing);

                _targetScrollOffset = Math.Max(0, Math.Min(_dragStartOffset - delta,
                    Math.Max(0, _listItems.Count - _visibleItemsCount)));
                _scrollOffset = _targetScrollOffset; // Direct update during drag

                UpdateItemPositions();
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

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // Clear hover state when mouse leaves control
            if (_hoveredItem != null)
            {
                _hoveredItem = null;
                Invalidate();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            float delta = (e.Delta / 120f) * ScrollSpeed;
            _targetScrollOffset -= delta;
            _targetScrollOffset = Math.Max(0, Math.Min(_targetScrollOffset,
                Math.Max(0, _listItems.Count - _visibleItemsCount)));
        }


        // Add keyboard navigation support
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!EnableKeyboardNavigation || _listItems.Count == 0)
                return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (_orientation == ScrollOrientation.VerticalScroll)
                    {
                        SelectedIndex = Math.Max(0, SelectedIndex - 1);
                        e.Handled = true;
                    }
                    break;

                case Keys.Down:
                    if (_orientation == ScrollOrientation.VerticalScroll)
                    {
                        SelectedIndex = Math.Min(_listItems.Count - 1, SelectedIndex + 1);
                        e.Handled = true;
                    }
                    break;

                case Keys.Left:
                    if (_orientation == ScrollOrientation.HorizontalScroll)
                    {
                        SelectedIndex = Math.Max(0, SelectedIndex - 1);
                        e.Handled = true;
                    }
                    break;

                case Keys.Right:
                    if (_orientation == ScrollOrientation.HorizontalScroll)
                    {
                        SelectedIndex = Math.Min(_listItems.Count - 1, SelectedIndex + 1);
                        e.Handled = true;
                    }
                    break;

                case Keys.PageUp:
                    SelectedIndex = Math.Max(0, SelectedIndex - _visibleItemsCount);
                    e.Handled = true;
                    break;

                case Keys.PageDown:
                    SelectedIndex = Math.Min(_listItems.Count - 1, SelectedIndex + _visibleItemsCount);
                    e.Handled = true;
                    break;

                case Keys.Home:
                    SelectedIndex = 0;
                    e.Handled = true;
                    break;

                case Keys.End:
                    SelectedIndex = _listItems.Count - 1;
                    e.Handled = true;
                    break;
            }
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
                var matchedItem = ListItems.FirstOrDefault(i => i.Value?.ToString() == value.ToString());
                if (matchedItem != null)
                {
                    SelectedItem = matchedItem;
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

            if (_image != null)
            {
                _image.Theme = Theme;
                _image.ApplyThemeOnImage = _currentTheme.ApplyThemeToIcons;
            }

            // The themed list font reaches the ACTUAL text rendering now - it used to be
            // stamped into a BeepButton helper that nothing ever drew with.
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.ListUnSelectedFont) ?? _textFont;
            }

            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();

                _image?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Animation
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Skip animation if dragging or if we're close enough to target
            if (_isDragging || Math.Abs(_scrollOffset - _targetScrollOffset) < 0.01)
            {
                _scrollOffset = _targetScrollOffset;
                return;
            }

            // Animate scroll position
            _scrollOffset += (_targetScrollOffset - _scrollOffset) * ScrollAnimationSpeed;
            UpdateItemPositions();
            Invalidate();
        }
        #endregion
    }
}