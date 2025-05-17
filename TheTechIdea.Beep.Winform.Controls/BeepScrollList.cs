using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;
using Timer = System.Windows.Forms.Timer;
using System.Windows.Forms;
using System.Linq;

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
        private List<BeepButton> _virtualButtons = new List<BeepButton>(); // Virtual buttons for drawing only
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
        #endregion

        #region Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show scroll indicators")]
        [DefaultValue(true)]
        public bool ShowScrollIndicators { get; set; } = true;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the scroll indicators")]
        public Color ScrollIndicatorColor { get; set; } = Color.Gray;

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
        }

        private void Items_ListChanged(object sender, ListChangedEventArgs e) => UpdateVirtualItems();

        private void InitializeControl()
        {
            DoubleBuffered = true;

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
            // Clear old hit test areas
            ClearHitList();

            // Clear old virtual buttons
            foreach (var button in _virtualButtons)
            {
                button.Dispose();
            }
            _virtualButtons.Clear();

            if (_listItems == null || _listItems.Count == 0)
                return;

            // Create virtual buttons
            for (int i = 0; i < _listItems.Count; i++)
            {
                var item = _listItems[i];

                // Create a virtual BeepButton (not added to Controls)
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
                    BorderThickness = 0,
                    UseScaledFont = true,
                    Font = TextFont,
                    IsSelected = i == _selectedIndex,
                    Tag = i // Store the index for later reference
                };

                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    button.ImagePath = item.ImagePath;
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    button.TextImageRelation = TextImageRelation.ImageBeforeText;
                }

                _virtualButtons.Add(button);
            }

            // Calculate and set up hit areas
            UpdateItemPositions();
        }

        private void UpdateSelectedState()
        {
            for (int i = 0; i < _virtualButtons.Count; i++)
            {
                _virtualButtons[i].IsSelected = (i == _selectedIndex);
            }
        }

        private void UpdateItemPositions()
        {
            if (_virtualButtons.Count == 0 || _listItems.Count == 0)
                return;

            // Calculate visible area dimensions accounting for padding
            int visibleWidth = DrawingRect.Width - (Padding.Left + Padding.Right);
            int visibleHeight = DrawingRect.Height - (Padding.Top + Padding.Bottom);

            // Calculate the range of items to display based on scroll position
            int startIndex = (int)Math.Floor(_scrollOffset);
            int endIndex = Math.Min(startIndex + _visibleItemsCount + 1, _listItems.Count);

            // Calculate the total content size
            float totalSize = _listItems.Count * (_itemHeight + _itemSpacing) - _itemSpacing;
            float visibleSize = _orientation == ScrollOrientation.VerticalScroll ? visibleHeight : visibleWidth;

            // Calculate centering offset when content is smaller than visible area
            float centeringOffset = 0;
            if (totalSize < visibleSize)
            {
                centeringOffset = (visibleSize - totalSize) / 2;
            }

            // Clear existing hit areas
            ClearHitList();

            // Create hit areas for visible items
            for (int i = 0; i < _virtualButtons.Count; i++)
            {
                // Only show items in the visible range
                bool isVisible = i >= startIndex && i < endIndex;

                if (!isVisible) continue;

                var button = _virtualButtons[i];

                // Calculate position based on scroll offset and spacing
                float itemPosition = (i - _scrollOffset) * (_itemHeight + _itemSpacing);
                Rectangle itemRect;

                if (_orientation == ScrollOrientation.VerticalScroll)
                {
                    // For vertical orientation
                    int yPos = (int)(Padding.Top + centeringOffset + itemPosition);

                    // Center button horizontally, or use full width if itemWidth >= visibleWidth
                    int buttonWidth = Math.Min(_itemWidth, visibleWidth);
                    int xPos = Padding.Left + (visibleWidth - buttonWidth) / 2;

                    // Define the item rectangle
                    itemRect = new Rectangle(xPos, yPos, buttonWidth, _itemHeight);
                }
                else
                {
                    // For horizontal orientation
                    int xPos = (int)(Padding.Left + centeringOffset + itemPosition);

                    // Use the height of visible area for buttons
                    int buttonHeight = Math.Min(_itemHeight, visibleHeight);
                    int yPos = Padding.Top + (visibleHeight - buttonHeight) / 2;

                    // Define the item rectangle
                    itemRect = new Rectangle(xPos, yPos, _itemHeight, buttonHeight);
                }

                // Add a hit area for this item
                int index = i; // Capture the current index for the lambda
                AddHitArea(
                    $"Item_{i}",
                    itemRect,
                    button,
                    () => ItemClicked(index)
                );
            }
        }

        // Handler for item click
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        // Override DrawContent to render scroll indicators and virtual buttons
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            // Draw each virtual button using its corresponding hit area
            foreach (var hitTest in HitList)
            {
                if (hitTest.IsVisible && hitTest.uIComponent is BeepButton button)
                {
                    // Render the button at its target rectangle
                    button.Draw(g, hitTest.TargetRect);
                }
            }

            // Draw scroll indicators if enabled and needed
            if (ShowScrollIndicators && _listItems.Count > _visibleItemsCount)
            {
                DrawScrollIndicators(g);
            }
        }

        private void DrawScrollIndicators(Graphics g)
        {
            // Calculate the total content size and visible percentage
            float contentSize = _listItems.Count * (_itemHeight + _itemSpacing) - _itemSpacing;
            int visibleSize = _orientation == ScrollOrientation.VerticalScroll
                ? DrawingRect.Height - (Padding.Top + Padding.Bottom)
                : DrawingRect.Width - (Padding.Left + Padding.Right);

            if (contentSize <= visibleSize)
                return; // No need for indicators if everything is visible

            // Calculate the scroll percentage
            float scrollPercentage = _scrollOffset / Math.Max(1, _listItems.Count - _visibleItemsCount);
            float visiblePercentage = Math.Min(1.0f, visibleSize / contentSize);

            // Draw indicators based on orientation
            using (SolidBrush indicatorBrush = new SolidBrush(
                _currentTheme?.ScrollBarThumbColor ?? ScrollIndicatorColor))
            {
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
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Always update hover states
            HitTest(e.Location);

            if (_isDragging)
            {
                float delta = _orientation == ScrollOrientation.VerticalScroll
                    ? (e.Y - _dragStartPoint.Y) / (float)(_itemHeight + _itemSpacing)
                    : (e.X - _dragStartPoint.X) / (float)(_itemHeight + _itemSpacing);

                _targetScrollOffset = _dragStartOffset - delta;
                _scrollOffset = _targetScrollOffset; // Direct update during drag

                // Ensure we stay within bounds
                _targetScrollOffset = Math.Max(0, Math.Min(_targetScrollOffset,
                    Math.Max(0, _listItems.Count - _visibleItemsCount)));

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
            if (Theme == null) return;

            // Set background color
            if (IsChild)
            {
                BackColor = Parent?.BackColor ?? _currentTheme.ListBackColor;
                ParentBackColor = BackColor;
            }
            else
            {
                BackColor = _currentTheme.ListBackColor;
            }

            // Apply theme to fonts
            if (UseThemeFont)
            {
                TextFont = BeepThemesManager.ToFont(_currentTheme.BodySmall);
            }

            // Apply theme to virtual buttons
            foreach (var button in _virtualButtons)
            {
                button.Font = TextFont;
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
                button.Theme = Theme;
            }

            // Apply theme to hit areas
            ApplyThemeToHitAreas();

            Invalidate();
        }

        private void ApplyThemeToHitAreas()
        {
            foreach (var hitTest in HitList)
            {
                if (hitTest.uIComponent is BeepButton button)
                {
                    button.Font = TextFont;
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
                    button.Theme = Theme;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();

                // Dispose all virtual buttons
                foreach (var button in _virtualButtons)
                {
                    button.Dispose();
                }
                _virtualButtons.Clear();

                // Clear hit areas
                ClearHitList();
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