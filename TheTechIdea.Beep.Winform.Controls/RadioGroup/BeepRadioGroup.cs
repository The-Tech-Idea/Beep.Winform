using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Report;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    [ToolboxItem(true)]
    [DisplayName("Beep Radio Group Advanced")]
    [Category("Beep Controls")]
    [Description("A modern, flexible radio group control with multiple selection modes, layouts, and render styles.")]
    public class BeepRadioGroup : BaseControl
    {
        #region Fields
        private readonly RadioGroupLayoutHelper _layoutHelper;
        private readonly RadioGroupHitTestHelper _hitTestHelper;
        private readonly RadioGroupStateHelper _stateHelper;
        private readonly Dictionary<RadioGroupRenderStyle, IRadioGroupRenderer> _renderers;
        
        private List<SimpleItem> _items = new List<SimpleItem>();
        private List<Rectangle> _itemRectangles = new List<Rectangle>();
        private List<RadioItemState> _itemStates = new List<RadioItemState>();
        
        private RadioGroupRenderStyle _renderStyle = RadioGroupRenderStyle.Material;
        private IRadioGroupRenderer _currentRenderer;
        private bool _allowMultipleSelection = false;
        private bool _autoSizeItems = true;
        private Size _maxImageSize = new Size(24, 24);
        private object _oldValue;
        #endregion

        #region Constructor
        public BeepRadioGroup() : base()
        {
            // Initialize helpers
            _layoutHelper = new RadioGroupLayoutHelper(this);
            _hitTestHelper = new RadioGroupHitTestHelper(this);
            _stateHelper = new RadioGroupStateHelper(this);

            // Initialize renderers
            _renderers = new Dictionary<RadioGroupRenderStyle, IRadioGroupRenderer>
            {
                { RadioGroupRenderStyle.Material, new MaterialRadioRenderer() },
                { RadioGroupRenderStyle.Card, new CardRadioRenderer() },
                { RadioGroupRenderStyle.Chip, new ChipRadioRenderer() },
                { RadioGroupRenderStyle.Circular, new CircularRadioRenderer() },
                { RadioGroupRenderStyle.Flat, new FlatRadioRenderer() },
                { RadioGroupRenderStyle.Checkbox, new CheckboxRadioRenderer() },
                { RadioGroupRenderStyle.Button, new ButtonRadioRenderer() },
                { RadioGroupRenderStyle.Toggle, new ToggleRadioRenderer() }
            };

            // Set default renderer
            _currentRenderer = _renderers[_renderStyle];
            _currentRenderer.Initialize(this, _currentTheme);
            _layoutHelper.ItemMeasurer = (item, g) => _currentRenderer.MeasureItem(item, g);

            // Initialize MaxImageSize for all renderers
            foreach (var renderer in _renderers.Values)
            {
                if (renderer is IImageAwareRenderer imageRenderer)
                {
                    imageRenderer.MaxImageSize = _maxImageSize;
                }
            }

            // Configure base control
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                    ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            
            Size = new Size(300, 200);
            
            // Subscribe to events
            SetupEventHandlers();
            
            // Configure layout helper defaults
            _layoutHelper.Orientation = RadioGroupOrientation.Vertical;
            _layoutHelper.ItemSpacing = 8;
            _layoutHelper.ItemPadding = new Padding(8);
            _layoutHelper.AutoSize = true;

            // Apply theme
            ApplyTheme();
        }

        private void SetupEventHandlers()
        {
            // Hit test events
            _hitTestHelper.ItemClicked += OnItemClicked;
            _hitTestHelper.ItemHoverEnter += OnItemHoverEnter;
            _hitTestHelper.ItemHoverLeave += OnItemHoverLeave;
            _hitTestHelper.HoveredIndexChanged += OnHoveredIndexChanged;
            _hitTestHelper.FocusedIndexChanged += OnFocusedIndexChanged;

            // State events
            _stateHelper.SelectionChanged += OnSelectionChanged;
            _stateHelper.ItemSelectionChanged += OnItemSelectionChanged;
        }
        #endregion

        #region Properties

        #region Data Properties
        private Font _textFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
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

                SafeApplyFont(_textFont);
                UseThemeFont = false;
                Invalidate();


            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new bool IsChild
        {
            get => _isChild;
            set
            {
                _isChild = value;
                base.IsChild = value;

                Invalidate();  // Trigger repaint
            }
        }
        [Browsable(true)]
        [Category("Data")]
        [Description("The list of items displayed in the radio group.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(UITypeEditor))]
        public List<SimpleItem> Items
        {
            get => _items;
            set
            {
                _items = value ?? new List<SimpleItem>();
                UpdateItemsAndLayout();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether multiple items can be selected simultaneously.")]
        [DefaultValue(false)]
        public bool AllowMultipleSelection
        {
            get => _allowMultipleSelection;
            set
            {
                if (_allowMultipleSelection != value)
                {
                    _allowMultipleSelection = value;
                    _stateHelper.AllowMultipleSelection = value;
                    
                    // Update renderer if it doesn't support the new mode
                    if (!_currentRenderer.SupportsMultipleSelection && value)
                    {
                        // Switch to a renderer that supports multiple selection
                        RenderStyle = RadioGroupRenderStyle.Material;
                    }
                    
                    UpdateItemStates();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The currently selected value (single selection mode).")]
        public string SelectedValue
        {
            get => _stateHelper.SelectedValue;
            set
            {
                if (_stateHelper.SelectedValue != value)
                {
                    _stateHelper.SelectedValue = value;
                    UpdateItemStates();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [Description("The currently selected values (multiple selection mode).")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<string> SelectedValues => _stateHelper.SelectedValues;

        [Browsable(false)]
        public List<SimpleItem> SelectedItems => _stateHelper.SelectedItems;

        public int SelectedCount => _stateHelper.SelectedCount;
        #endregion

        #region Appearance Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The render style for the radio group items.")]
        [DefaultValue(RadioGroupRenderStyle.Material)]
        public RadioGroupRenderStyle RenderStyle
        {
            get => _renderStyle;
            set
            {
                if (_renderStyle != value && _renderers.ContainsKey(value))
                {
                    _renderStyle = value;
                    _currentRenderer = _renderers[value];
                    _currentRenderer.Initialize(this, _currentTheme);

                    // Update measurer to the new renderer
                    _layoutHelper.ItemMeasurer = (item, g) => _currentRenderer?.MeasureItem(item, g) ?? Size.Empty;
                    
                    // Check if new renderer supports current selection mode
                    if (!_currentRenderer.SupportsMultipleSelection && _allowMultipleSelection)
                    {
                        AllowMultipleSelection = false;
                    }
                    
                    UpdateItemsAndLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("The orientation/layout of the radio group items.")]
        [DefaultValue(RadioGroupOrientation.Vertical)]
        public RadioGroupOrientation Orientation
        {
            get => _layoutHelper.Orientation;
            set
            {
                if (_layoutHelper.Orientation != value)
                {
                    _layoutHelper.Orientation = value;
                    UpdateItemsAndLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("The alignment of items within the control.")]
        [DefaultValue(RadioGroupAlignment.Left)]
        public RadioGroupAlignment Alignment
        {
            get => _layoutHelper.Alignment;
            set
            {
                if (_layoutHelper.Alignment != value)
                {
                    _layoutHelper.Alignment = value;
                    UpdateItemsAndLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("The spacing between items.")]
        [DefaultValue(8)]
        public int ItemSpacing
        {
            get => _layoutHelper.ItemSpacing;
            set
            {
                if (_layoutHelper.ItemSpacing != value && value >= 0)
                {
                    _layoutHelper.ItemSpacing = value;
                    UpdateItemsAndLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("The padding around each item.")]
        public Padding ItemPadding
        {
            get => _layoutHelper.ItemPadding;
            set
            {
                if (_layoutHelper.ItemPadding != value)
                {
                    _layoutHelper.ItemPadding = value;
                    UpdateItemsAndLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("The number of columns for grid layout.")]
        [DefaultValue(1)]
        public int ColumnCount
        {
            get => _layoutHelper.ColumnCount;
            set
            {
                if (_layoutHelper.ColumnCount != value && value > 0)
                {
                    _layoutHelper.ColumnCount = value;
                    if (Orientation == RadioGroupOrientation.Grid)
                    {
                        UpdateItemsAndLayout();
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Whether items should auto-size to their content.")]
        [DefaultValue(true)]
        public bool AutoSizeItems
        {
            get => _autoSizeItems;
            set
            {
                if (_autoSizeItems != value)
                {
                    _autoSizeItems = value;
                    _layoutHelper.AutoSize = value;
                    UpdateLayout();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Maximum size for images rendered from ImagePath property.")]
        [DefaultValue(typeof(Size), "24, 24")]
        public Size MaxImageSize
        {
            get => _maxImageSize;
            set
            {
                if (_maxImageSize != value && value.Width > 0 && value.Height > 0)
                {
                    _maxImageSize = value;
                    
                    // Update all renderers with new image size
                    foreach (var renderer in _renderers.Values)
                    {
                        if (renderer is IImageAwareRenderer imageRenderer)
                        {
                            imageRenderer.MaxImageSize = value;
                        }
                    }
                    
                    UpdateLayout();
                    Invalidate();
                }
            }
        }
        #endregion

        #endregion

        #region Events
        [Category("Action")]
        [Description("Occurs when the selection changes.")]
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        [Category("Action")]
        [Description("Occurs when an individual item's selection state changes.")]
        public event EventHandler<ItemSelectionChangedEventArgs> ItemSelectionChanged;

        [Category("Action")]
        [Description("Occurs when an item is clicked.")]
        public event EventHandler<ItemClickEventArgs> ItemClicked;

        [Category("Action")]
        [Description("Occurs when an item is double-clicked.")]
        public event EventHandler<ItemClickEventArgs> ItemDoubleClicked;

        [Category("Mouse")]
        [Description("Occurs when the mouse enters an item.")]
        public event EventHandler<ItemHoverEventArgs> ItemHoverEnter;

        [Category("Mouse")]
        [Description("Occurs when the mouse leaves an item.")]
        public event EventHandler<ItemHoverEventArgs> ItemHoverLeave;
        #endregion

        #region Layout and Rendering
        private void UpdateItemsAndLayout()
        {
            // Update helpers with new data
            _stateHelper.UpdateItems(_items);
            
            // Calculate layout
            UpdateLayout();
            
            // Update states
            UpdateItemStates();
            
            // Update hit testing
            _hitTestHelper.UpdateItems(_items, _itemRectangles);
            
            // Auto-size control if needed
            if (AutoSize)
            {
                var totalSize = _layoutHelper.CalculateTotalSize(_items, MaximumSize);
                Size = totalSize;
            }
            
            Invalidate();
        }

        private void UpdateLayout()
        {
            if (_items == null || _items.Count == 0)
            {
                _itemRectangles.Clear();
                return;
            }

            // Use DrawingRect instead of full Size for layout calculations
            var containerRect = DrawingRect;
            if (containerRect.IsEmpty)
                containerRect = new Rectangle(Point.Empty, Size);
            
            // Make container rect relative (start from 0,0 since we'll offset later)
            containerRect = new Rectangle(Point.Empty, containerRect.Size);
            
            _itemRectangles = _layoutHelper.CalculateItemRectangles(_items, containerRect);
            
            // Apply alignment
            _layoutHelper.ApplyAlignment(_itemRectangles, containerRect);
        }

        private void UpdateItemStates()
        {
            _itemStates.Clear();
            
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var state = new RadioItemState
                {
                    IsSelected = _stateHelper.IsSelected(item),
                    IsHovered = _hitTestHelper.HoveredIndex == i,
                    IsFocused = _hitTestHelper.FocusedIndex == i,
                    IsEnabled = true, // Could be made configurable per item
                    Index = i
                };
                
                _itemStates.Add(state);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            // Ensure DrawingRect is updated
            UpdateDrawingRect();
            
            UpdateLayout();
            _hitTestHelper.UpdateItems(_items, _itemRectangles);
            Invalidate();
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            if (_currentRenderer == null || _items == null || _items.Count == 0)
                return;

            // Update states before drawing
            UpdateItemStates();

            // Use DrawingRect from BaseControl for proper bounds
            var drawingBounds = DrawingRect;
            if (drawingBounds.IsEmpty)
                drawingBounds = ClientRectangle;

            // Draw group decorations first
            _currentRenderer.RenderGroupDecorations(g, drawingBounds, _items, _itemRectangles, _itemStates);

            // Draw each item within the drawing bounds
            for (int i = 0; i < Math.Min(_items.Count, Math.Min(_itemRectangles.Count, _itemStates.Count)); i++)
            {
                // Offset item rectangles to be relative to DrawingRect
                var adjustedRect = new Rectangle(
                    _itemRectangles[i].X + drawingBounds.X,
                    _itemRectangles[i].Y + drawingBounds.Y,
                    _itemRectangles[i].Width,
                    _itemRectangles[i].Height
                );

                // Clip to drawing bounds
                if (adjustedRect.IntersectsWith(drawingBounds))
                {
                    _currentRenderer.RenderItem(g, _items[i], adjustedRect, _itemStates[i]);
                }
            }
        }
        #endregion

        #region Mouse Handling
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            // Convert mouse location to be relative to DrawingRect
            var adjustedLocation = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );
            
            _hitTestHelper.HandleMouseMove(adjustedLocation);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hitTestHelper.HandleMouseLeave();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            // Convert mouse location to be relative to DrawingRect
            var adjustedLocation = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );
            
            _hitTestHelper.HandleMouseClick(adjustedLocation, e.Button);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            
            // Convert mouse location to be relative to DrawingRect
            var adjustedLocation = new Point(
                e.Location.X - DrawingRect.X,
                e.Location.Y - DrawingRect.Y
            );
            
            _hitTestHelper.HandleMouseDoubleClick(adjustedLocation, e.Button);
        }
        #endregion

        #region Keyboard Handling
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (_hitTestHelper.HandleKeyDown(e.KeyCode, Orientation))
            {
                e.Handled = true;
            }
        }

        protected override bool ProcessTabKey(bool forward)
        {
            if (forward)
            {
                return _hitTestHelper.MoveFocusNext();
            }
            else
            {
                return _hitTestHelper.MoveFocusPrevious();
            }
        }
        #endregion

        #region Event Handlers
        private void OnItemClicked(object sender, ItemClickEventArgs e)
        {
            // Handle selection logic
            if (AllowMultipleSelection)
            {
                _stateHelper.ToggleItem(e.Item);
            }
            else
            {
                _stateHelper.SelectItem(e.Item);
            }

            // Raise public event
            ItemClicked?.Invoke(this, e);
        }

        private void OnItemHoverEnter(object sender, ItemHoverEventArgs e)
        {
            ItemHoverEnter?.Invoke(this, e);
        }

        private void OnItemHoverLeave(object sender, ItemHoverEventArgs e)
        {
            ItemHoverLeave?.Invoke(this, e);
        }

        private void OnHoveredIndexChanged(object sender, IndexChangedEventArgs e)
        {
            UpdateItemStates();
            Invalidate();
        }

        private void OnFocusedIndexChanged(object sender, IndexChangedEventArgs e)
        {
            UpdateItemStates();
            Invalidate();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = GetValue();
            if (!Equals(_oldValue, newValue))
            {
                _oldValue = newValue;
                // Raise IBeepUIComponent value-changed via BaseControl helper
                InvokeOnValueChanged(new BeepComponentEventArgs(this, "SelectedValue", LinkedProperty, newValue));
            }

            SelectionChanged?.Invoke(this, e);
            UpdateItemStates();
            Invalidate();
        }

        private void OnItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            var newValue = GetValue();
            if (!Equals(_oldValue, newValue))
            {
                _oldValue = newValue;
                InvokeOnValueChanged(new BeepComponentEventArgs(this, "SelectedValue", LinkedProperty, newValue));
            }

            ItemSelectionChanged?.Invoke(this, e);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a new item to the radio group
        /// </summary>
        public void AddItem(SimpleItem item)
        {
            if (item != null)
            {
                _items.Add(item);
                UpdateItemsAndLayout();
            }
        }

        /// <summary>
        /// Adds a new item with text and optional image
        /// </summary>
        public void AddItem(string text, string imagePath = null, string subText = null)
        {
            var item = new SimpleItem
            {
                Text = text,
                ImagePath = imagePath,
                SubText = subText
            };
            AddItem(item);
        }

        /// <summary>
        /// Removes an item from the radio group
        /// </summary>
        public bool RemoveItem(SimpleItem item)
        {
            if (item != null && _items.Contains(item))
            {
                _items.Remove(item);
                UpdateItemsAndLayout();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears all items
        /// </summary>
        public void ClearItems()
        {
            _items.Clear();
            UpdateItemsAndLayout();
        }

        /// <summary>
        /// Selects an item by value
        /// </summary>
        public bool SelectItem(string value)
        {
            return _stateHelper.SelectValue(value);
        }

        /// <summary>
        /// Deselects an item by value
        /// </summary>
        public bool DeselectItem(string value)
        {
            return _stateHelper.DeselectValue(value);
        }

        /// <summary>
        /// Clears all selections
        /// </summary>
        public void ClearSelection()
        {
            _stateHelper.ClearSelection();
        }

        /// <summary>
        /// Selects all items (multiple selection mode only)
        /// </summary>
        public void SelectAll()
        {
            if (AllowMultipleSelection)
            {
                _stateHelper.SelectAll();
            }
        }

        /// <summary>
        /// Registers a custom renderer
        /// </summary>
        public void RegisterRenderer(RadioGroupRenderStyle style, IRadioGroupRenderer renderer)
        {
            if (renderer != null)
            {
                _renderers[style] = renderer;
                
                // If this is the current style, update the current renderer
                if (_renderStyle == style)
                {
                    _currentRenderer = renderer;
                    _currentRenderer.Initialize(this, _currentTheme);
                    UpdateItemsAndLayout();
                }
            }
        }
        #endregion

        #region BaseControl Overrides
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            // Update current renderer with new theme
            _currentRenderer?.UpdateTheme(_currentTheme);
            
            // Update all renderers
            foreach (var renderer in _renderers.Values)
            {
                renderer.UpdateTheme(_currentTheme);
            }

            // Ensure measurer stays valid
            _layoutHelper.ItemMeasurer = (item, g) => _currentRenderer?.MeasureItem(item, g) ?? Size.Empty;
            
            Invalidate();
        }

        public override void SetValue(object value)
        {
            var oldValue = GetValue();

            if (value is string stringValue)
            {
                SelectedValue = stringValue;
            }
            else if (value is List<string> stringList)
            {
                _stateHelper.SetMultipleSelection(stringList);
                UpdateItemStates();
                Invalidate();
            }
            else if (value is SimpleItem item)
            {
                _stateHelper.SelectItem(item);
            }
            else if (value is List<SimpleItem> itemList)
            {
                var values = itemList.Where(i => !string.IsNullOrEmpty(i.Text)).Select(i => i.Text);
                _stateHelper.SetMultipleSelection(values);
                UpdateItemStates();
                Invalidate();
            }

            var newValue = GetValue();
            if (!Equals(oldValue, newValue))
            {
                _oldValue = newValue;
                InvokeOnValueChanged(new BeepComponentEventArgs(this, "SelectedValue", LinkedProperty, newValue));
            }
        }

        public override object GetValue()
        {
            if (AllowMultipleSelection)
            {
                return SelectedItems;
            }
            else
            {
                return SelectedItems.FirstOrDefault();
            }
        }
        public bool Reset()
        {
            var oldValue = GetValue();
            ClearSelection();
            var newValue = GetValue();
            if (!Equals(oldValue, newValue))
            {
                _oldValue = newValue;
                InvokeOnValueChanged(new BeepComponentEventArgs(this, "SelectedValue", LinkedProperty, newValue));
                return true;
            }
            return false;
        }
        #endregion
       
    }
}