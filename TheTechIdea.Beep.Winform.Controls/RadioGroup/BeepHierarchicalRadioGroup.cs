using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    [ToolboxItem(true)]
    [DisplayName("Beep Hierarchical Radio Group")]
    [Category("Beep Controls")]
    [Description("A hierarchical radio group control with tree-like structure support using SimpleItem.Children property.")]
    public class BeepHierarchicalRadioGroup : BaseControl
    {
        #region Fields
        private readonly RadioGroupLayoutHelper _layoutHelper;
        private readonly RadioGroupHitTestHelper _hitTestHelper;
        private readonly RadioGroupStateHelper _stateHelper;
        private readonly Dictionary<RadioGroupRenderStyle, IRadioGroupRenderer> _renderers;
        
        private List<SimpleItem> _rootItems = new List<SimpleItem>();
        private List<SimpleItem> _flattenedItems = new List<SimpleItem>(); // All items flattened for rendering
        private List<Rectangle> _itemRectangles = new List<Rectangle>();
        private List<RadioItemState> _itemStates = new List<RadioItemState>();
        private Dictionary<SimpleItem, int> _itemIndentLevels = new Dictionary<SimpleItem, int>();
        
        private RadioGroupRenderStyle _renderStyle = RadioGroupRenderStyle.Flat;
        private IRadioGroupRenderer _currentRenderer;
        private bool _allowMultipleSelection = false;
        private bool _autoSizeItems = true;
        private bool _showExpanderButtons = true;
        private int _indentSize = 20;
        private Size _maxImageSize = new Size(24, 24);
        #endregion

        #region Constructor
        public BeepHierarchicalRadioGroup() : base()
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
                { RadioGroupRenderStyle.Flat, new FlatRadioRenderer() }
            };

            // Set default renderer
            _currentRenderer = _renderers[_renderStyle];
            _currentRenderer.Initialize(this, _currentTheme);

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
            
            Size = new Size(300, 400);
            
            // Subscribe to events
            SetupEventHandlers();
            
            // Configure layout helper defaults
            _layoutHelper.Orientation = RadioGroupOrientation.Vertical;
            _layoutHelper.ItemSpacing = 4; // Smaller spacing for hierarchy
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
        [Browsable(true)]
        [Category("Data")]
        [Description("The root items with hierarchy support using Children property.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(UITypeEditor))]
        public List<SimpleItem> RootItems
        {
            get => _rootItems;
            set
            {
                _rootItems = value ?? new List<SimpleItem>();
                UpdateHierarchy();
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
                    
                    UpdateItemStates();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether to show expand/collapse buttons for parent items.")]
        [DefaultValue(true)]
        public bool ShowExpanderButtons
        {
            get => _showExpanderButtons;
            set
            {
                if (_showExpanderButtons != value)
                {
                    _showExpanderButtons = value;
                    UpdateHierarchy();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The indentation size in pixels for each hierarchy level.")]
        [DefaultValue(20)]
        public int IndentSize
        {
            get => _indentSize;
            set
            {
                if (_indentSize != value && value > 0)
                {
                    _indentSize = value;
                    UpdateHierarchy();
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

        [Browsable(false)]
        public int SelectedCount => _stateHelper.SelectedCount;
        #endregion

        #region Appearance Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The render style for the radio group items.")]
        [DefaultValue(RadioGroupRenderStyle.Flat)]
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
                    
                    UpdateHierarchy();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("The spacing between items.")]
        [DefaultValue(4)]
        public int ItemSpacing
        {
            get => _layoutHelper.ItemSpacing;
            set
            {
                if (_layoutHelper.ItemSpacing != value && value >= 0)
                {
                    _layoutHelper.ItemSpacing = value;
                    UpdateHierarchy();
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
                    UpdateHierarchy();
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
                    
                    UpdateHierarchy();
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
        [Description("Occurs when an item is expanded or collapsed.")]
        public event EventHandler<ItemExpandedEventArgs> ItemExpandedChanged;

        [Category("Mouse")]
        [Description("Occurs when the mouse enters an item.")]
        public event EventHandler<ItemHoverEventArgs> ItemHoverEnter;

        [Category("Mouse")]
        [Description("Occurs when the mouse leaves an item.")]
        public event EventHandler<ItemHoverEventArgs> ItemHoverLeave;
        #endregion

        #region Hierarchy Management
        private void UpdateHierarchy()
        {
            // Flatten the hierarchy for rendering
            _flattenedItems.Clear();
            _itemIndentLevels.Clear();
            
            FlattenHierarchy(_rootItems, 0);
            
            // Update helpers with flattened data
            _stateHelper.UpdateItems(_flattenedItems);
            
            // Calculate layout
            UpdateLayout();
            
            // Update states
            UpdateItemStates();
            
            // Update hit testing
            _hitTestHelper.UpdateItems(_flattenedItems, _itemRectangles);
            
            // Auto-size control if needed
            if (AutoSize)
            {
                var totalSize = CalculateHierarchicalSize();
                Size = totalSize;
            }
            
            Invalidate();
        }

        private void FlattenHierarchy(List<SimpleItem> items, int level)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                // Add the item to flattened list
                _flattenedItems.Add(item);
                _itemIndentLevels[item] = level;

                // If item is expanded and has children, add them recursively
                if (item.IsExpanded && item.Children != null && item.Children.Count > 0)
                {
                    FlattenHierarchy(item.Children.ToList(), level + 1);
                }
            }
        }

        private Size CalculateHierarchicalSize()
        {
            if (_flattenedItems.Count == 0)
                return new Size(200, 100);

            int totalHeight = 20; // Top padding
            int maxWidth = 200;

            using (var g = CreateGraphics())
            {
                foreach (var item in _flattenedItems)
                {
                    var itemSize = _currentRenderer.MeasureItem(item, g);
                    int indentLevel = _itemIndentLevels.ContainsKey(item) ? _itemIndentLevels[item] : 0;
                    int itemWidth = itemSize.Width + (indentLevel * _indentSize) + (_showExpanderButtons ? 20 : 0);
                    
                    maxWidth = Math.Max(maxWidth, itemWidth);
                    totalHeight += itemSize.Height + _layoutHelper.ItemSpacing;
                }
            }

            totalHeight += 20; // Bottom padding
            return new Size(maxWidth + 40, totalHeight); // Add some horizontal padding
        }

        private void UpdateLayout()
        {
            if (_flattenedItems == null || _flattenedItems.Count == 0)
            {
                _itemRectangles.Clear();
                return;
            }

            _itemRectangles.Clear();
            int currentY = 10; // Top padding

            using (var g = CreateGraphics())
            {
                foreach (var item in _flattenedItems)
                {
                    var itemSize = _currentRenderer.MeasureItem(item, g);
                    int indentLevel = _itemIndentLevels.ContainsKey(item) ? _itemIndentLevels[item] : 0;
                    int indentOffset = indentLevel * _indentSize;
                    int expanderOffset = _showExpanderButtons ? 20 : 0;

                    Rectangle itemRect = new Rectangle(
                        10 + indentOffset + expanderOffset, // Left padding + indent + expander space
                        currentY,
                        Width - 20 - indentOffset - expanderOffset, // Account for padding and indent
                        itemSize.Height
                    );

                    _itemRectangles.Add(itemRect);
                    currentY += itemSize.Height + _layoutHelper.ItemSpacing;
                }
            }
        }

        private void UpdateItemStates()
        {
            _itemStates.Clear();
            
            for (int i = 0; i < _flattenedItems.Count; i++)
            {
                var item = _flattenedItems[i];
                var state = new RadioItemState
                {
                    IsSelected = _stateHelper.IsSelected(item),
                    IsHovered = _hitTestHelper.HoveredIndex == i,
                    IsFocused = _hitTestHelper.FocusedIndex == i,
                    IsEnabled = item.IsEnabled,
                    Index = i
                };
                
                _itemStates.Add(state);
            }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            if (_currentRenderer == null || _flattenedItems == null || _flattenedItems.Count == 0)
                return;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Update states before drawing
            UpdateItemStates();

            // Draw each item with hierarchy visualization
            for (int i = 0; i < Math.Min(_flattenedItems.Count, Math.Min(_itemRectangles.Count, _itemStates.Count)); i++)
            {
                var item = _flattenedItems[i];
                var itemRect = _itemRectangles[i];
                var itemState = _itemStates[i];
                
                // Draw hierarchy lines if not top level
                if (_itemIndentLevels.ContainsKey(item) && _itemIndentLevels[item] > 0)
                {
                    DrawHierarchyLines(g, item, itemRect, i);
                }

                // Draw expander button if item has children
                if (_showExpanderButtons && item.Children != null && item.Children.Count > 0)
                {
                    DrawExpanderButton(g, item, itemRect);
                }

                // Draw the item using the renderer
                _currentRenderer.RenderItem(g, item, itemRect, itemState);
            }
        }

        private void DrawHierarchyLines(Graphics g, SimpleItem item, Rectangle itemRect, int itemIndex)
        {
            int indentLevel = _itemIndentLevels[item];
            Color lineColor = Color.FromArgb(128, _currentTheme?.BorderColor ?? Color.Gray);

            using (var pen = new Pen(lineColor, 1f))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                // Draw vertical line from parent
                for (int level = 1; level <= indentLevel; level++)
                {
                    int x = 10 + (level - 1) * _indentSize + 10; // Center of expander button area
                    
                    // Check if this level continues down
                    bool hasLowerSibling = HasLowerSiblingAtLevel(itemIndex, level);
                    
                    if (hasLowerSibling || level < indentLevel)
                    {
                        g.DrawLine(pen, x, itemRect.Top, x, itemRect.Bottom);
                    }
                }

                // Draw horizontal line to item
                int parentX = 10 + (indentLevel - 1) * _indentSize + 10;
                int itemY = itemRect.Top + itemRect.Height / 2;
                g.DrawLine(pen, parentX, itemY, parentX + _indentSize - 5, itemY);
            }
        }

        private bool HasLowerSiblingAtLevel(int currentIndex, int level)
        {
            for (int i = currentIndex + 1; i < _flattenedItems.Count; i++)
            {
                var checkItem = _flattenedItems[i];
                int checkLevel = _itemIndentLevels.ContainsKey(checkItem) ? _itemIndentLevels[checkItem] : 0;
                
                if (checkLevel < level)
                    break; // Moved to parent level, no more siblings
                
                if (checkLevel == level)
                    return true; // Found sibling at same level
            }
            return false;
        }

        private void DrawExpanderButton(Graphics g, SimpleItem item, Rectangle itemRect)
        {
            Rectangle expanderRect = new Rectangle(
                10 + (_itemIndentLevels.ContainsKey(item) ? _itemIndentLevels[item] * _indentSize : 0),
                itemRect.Y + (itemRect.Height - 16) / 2,
                16,
                16
            );

            Color expanderColor = _currentTheme?.ForeColor ?? Color.Black;
            using (var brush = new SolidBrush(Color.FromArgb(240, expanderColor)))
            using (var pen = new Pen(expanderColor, 1f))
            {
                // Draw expander background
                g.FillEllipse(brush, expanderRect);
                g.DrawEllipse(pen, expanderRect);

                // Draw + or - sign
                int centerX = expanderRect.X + expanderRect.Width / 2;
                int centerY = expanderRect.Y + expanderRect.Height / 2;
                
                // Horizontal line (always present)
                g.DrawLine(pen, centerX - 4, centerY, centerX + 4, centerY);
                
                // Vertical line (only if collapsed)
                if (!item.IsExpanded)
                {
                    g.DrawLine(pen, centerX, centerY - 4, centerX, centerY + 4);
                }
            }

            // Add hit area for expander
            AddHitArea($"Expander_{item.GuidId}", expanderRect, null, () => ToggleItemExpansion(item));
        }

        private void ToggleItemExpansion(SimpleItem item)
        {
            if (item.Children != null && item.Children.Count > 0)
            {
                item.IsExpanded = !item.IsExpanded;
                ItemExpandedChanged?.Invoke(this, new ItemExpandedEventArgs(item, item.IsExpanded));
                UpdateHierarchy();
            }
        }
        #endregion

        #region Mouse Handling
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _hitTestHelper.HandleMouseMove(e.Location);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hitTestHelper.HandleMouseLeave();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            _hitTestHelper.HandleMouseClick(e.Location, e.Button);
        }
        #endregion

        #region Keyboard Handling
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            // Handle expand/collapse with keyboard
            if (e.KeyCode == Keys.Right && _hitTestHelper.FocusedIndex >= 0 && _hitTestHelper.FocusedIndex < _flattenedItems.Count)
            {
                var item = _flattenedItems[_hitTestHelper.FocusedIndex];
                if (item.Children != null && item.Children.Count > 0 && !item.IsExpanded)
                {
                    ToggleItemExpansion(item);
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Left && _hitTestHelper.FocusedIndex >= 0 && _hitTestHelper.FocusedIndex < _flattenedItems.Count)
            {
                var item = _flattenedItems[_hitTestHelper.FocusedIndex];
                if (item.IsExpanded)
                {
                    ToggleItemExpansion(item);
                    e.Handled = true;
                }
            }
            else if (_hitTestHelper.HandleKeyDown(e.KeyCode, RadioGroupOrientation.Vertical))
            {
                e.Handled = true;
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
            SelectionChanged?.Invoke(this, e);
            UpdateItemStates();
            Invalidate();
        }

        private void OnItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            ItemSelectionChanged?.Invoke(this, e);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a new root item to the hierarchy
        /// </summary>
        public void AddRootItem(SimpleItem item)
        {
            if (item != null)
            {
                _rootItems.Add(item);
                UpdateHierarchy();
            }
        }

        /// <summary>
        /// Adds a child item to a parent item
        /// </summary>
        public void AddChildItem(SimpleItem parent, SimpleItem child)
        {
            if (parent != null && child != null)
            {
                if (parent.Children == null)
                    parent.Children = new System.ComponentModel.BindingList<SimpleItem>();
                
                child.ParentItem = parent;
                parent.Children.Add(child);
                UpdateHierarchy();
            }
        }

        /// <summary>
        /// Expands all items in the hierarchy
        /// </summary>
        public void ExpandAll()
        {
            ExpandAllRecursive(_rootItems);
            UpdateHierarchy();
        }

        /// <summary>
        /// Collapses all items in the hierarchy
        /// </summary>
        public void CollapseAll()
        {
            CollapseAllRecursive(_rootItems);
            UpdateHierarchy();
        }

        private void ExpandAllRecursive(List<SimpleItem> items)
        {
            if (items == null) return;
            
            foreach (var item in items)
            {
                if (item.Children != null && item.Children.Count > 0)
                {
                    item.IsExpanded = true;
                    ExpandAllRecursive(item.Children.ToList());
                }
            }
        }

        private void CollapseAllRecursive(List<SimpleItem> items)
        {
            if (items == null) return;
            
            foreach (var item in items)
            {
                item.IsExpanded = false;
                if (item.Children != null && item.Children.Count > 0)
                {
                    CollapseAllRecursive(item.Children.ToList());
                }
            }
        }

        /// <summary>
        /// Finds an item by its GuidId in the hierarchy
        /// </summary>
        public SimpleItem FindItem(string guidId)
        {
            return FindItemRecursive(_rootItems, guidId);
        }

        private SimpleItem FindItemRecursive(List<SimpleItem> items, string guidId)
        {
            if (items == null) return null;
            
            foreach (var item in items)
            {
                if (item.GuidId == guidId)
                    return item;
                
                if (item.Children != null)
                {
                    var found = FindItemRecursive(item.Children.ToList(), guidId);
                    if (found != null)
                        return found;
                }
            }
            return null;
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
            
            Invalidate();
        }

        public override void SetValue(object value)
        {
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
        #endregion
    }

    #region Event Args Classes
    public class ItemExpandedEventArgs : EventArgs
    {
        public SimpleItem Item { get; }
        public bool IsExpanded { get; }

        public ItemExpandedEventArgs(SimpleItem item, bool isExpanded)
        {
            Item = item;
            IsExpanded = isExpanded;
        }
    }
    #endregion
}