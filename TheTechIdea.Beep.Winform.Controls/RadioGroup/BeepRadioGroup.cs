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
    public partial class BeepRadioGroup : BaseControl
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
            // Configure base control FIRST
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | 
                    ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            
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
                { RadioGroupRenderStyle.Toggle, new ToggleRadioRenderer() },
                { RadioGroupRenderStyle.Segmented, new SegmentedRadioRenderer() },
                { RadioGroupRenderStyle.Pill, new PillRadioRenderer() },
                { RadioGroupRenderStyle.Tile, new TileRadioRenderer() }
            };

            // Only initialize renderer if not in design mode
            if (!DesignMode)
            {
                _currentRenderer = _renderers[_renderStyle];
                _currentRenderer.Initialize(this, _currentTheme);
                _layoutHelper.ItemMeasurer = (item, g) => _currentRenderer?.MeasureItem(item, g) ?? Size.Empty;

                // Initialize MaxImageSize for all renderers
                foreach (var renderer in _renderers.Values)
                {
                    if (renderer is IImageAwareRenderer imageRenderer)
                    {
                        imageRenderer.MaxImageSize = _maxImageSize;
                    }
                }
            }
            
            Size = new Size(300, 200);
            
            // Subscribe to events
            SetupEventHandlers();
            
            // Configure layout helper defaults
            _layoutHelper.Orientation = RadioGroupOrientation.Vertical;
            _layoutHelper.ItemSpacing = 8;
            _layoutHelper.ItemPadding = new Padding(8);
            _layoutHelper.AutoSize = true;

            // Apply theme only at runtime
            if (!DesignMode)
            {
                ApplyTheme();
            }
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
                
                // If this is the current Style, update the current renderer
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
            
            // Apply font theme based on ControlStyle
            RadioGroupFontHelpers.ApplyFontTheme(Style);
            
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