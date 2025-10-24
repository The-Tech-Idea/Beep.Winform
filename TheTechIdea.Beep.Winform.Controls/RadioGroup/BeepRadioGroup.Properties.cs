using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    public partial class BeepRadioGroup
    {
        private bool _useThemeColors = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use theme colors instead of custom accent color.")]
        [DefaultValue(true)]
        public bool UseThemeColors
        {
            get => _useThemeColors;
            set
            {
                _useThemeColors = value;
                Invalidate();
            }
        }
        private BeepControlStyle _style = BeepControlStyle.Material3;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual Style/painter to use for rendering the sidebar.")]
        [DefaultValue(BeepControlStyle.Material3)]
        public BeepControlStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;

                    Invalidate();
                }
            }
        }
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
                Invalidate();
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
                        RadioGroupStyle = RadioGroupRenderStyle.Material;
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

        #region Appearance/Layout Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The render Style for the radio group items.")]
        [DefaultValue(RadioGroupRenderStyle.Material)]
        public RadioGroupRenderStyle RadioGroupStyle
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
    }
}
