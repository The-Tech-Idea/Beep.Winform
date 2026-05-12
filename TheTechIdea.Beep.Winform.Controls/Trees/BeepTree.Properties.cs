using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Models;
using TheTechIdea.Beep.Winform.Controls.Trees.Editors;
using TheTechIdea.Beep.Winform.Controls.Editors;
using System.Drawing.Design;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepTree - Properties partial class.
    /// Contains all public properties that expose private fields from Core.cs
    /// </summary>
    public partial class BeepTree
    {
        #region Tree Style and Visual Properties
        
        /// <summary>
        /// Gets or sets the tree Style which determines which painter is used for rendering.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual Style of the tree.")]
        public TreeStyle TreeStyle
        {
            get => _treeStyle;
            set
            {
                if (_treeStyle != value)
                {
                    _treeStyle = value;
                    InitializePainter();
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Overrides ControlStyle to recalculate layout when FormStyle/ControlStyle changes.
        /// This ensures node layout is refreshed when DrawingRect dimensions change due to style borders/padding/shadows.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual style/painter to use for rendering the control.")]
        [DefaultValue(BeepControlStyle.None)]
        public new BeepControlStyle ControlStyle
        {
            get => base.ControlStyle;
            set
            {
                if (base.ControlStyle != value)
                {
                    base.ControlStyle = value;
                    // CRITICAL: Invalidate BOTH layout caches and recalculate
                    // The layout helper cache must be cleared before recalculating
                    try { _layoutHelper?.InvalidateCache(); } catch { }
                    RecalculateLayoutCache();
                    // Sync the layout helper's cache
                    try { _layoutHelper?.RecalculateLayout(); } catch { }
                    UpdateScrollBars();
                    // Update hit areas since layout changed
                    try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Overrides UseFormStylePaint to recalculate layout when style painting mode changes.
        /// This ensures node layout is refreshed when switching between classic and FormStyle painting.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether to use FormStyle-based painting for borders/shadows.")]
        [DefaultValue(true)]
        public new bool UseFormStylePaint
        {
            get => base.UseFormStylePaint;
            set
            {
                if (base.UseFormStylePaint != value)
                {
                    base.UseFormStylePaint = value;
                    // CRITICAL: Invalidate BOTH layout caches and recalculate
                    // The layout helper cache must be cleared before recalculating
                    try { _layoutHelper?.InvalidateCache(); } catch { }
                    RecalculateLayoutCache();
                    // Sync the layout helper's cache
                    try { _layoutHelper?.RecalculateLayout(); } catch { }
                    UpdateScrollBars();
                    // Update hit areas since layout changed
                    try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
                    Invalidate();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets whether to use scaled font based on DPI.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        public bool UseScaledFont
        {
            get => _useScaledFont;
            set
            {
                _useScaledFont = value;
                ApplyTheme();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets the font used for text rendering.
        /// </summary>
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Font TextFont
        {
            get => _textFont;
            set
            {
                _textFont = value;
                UseThemeFont = false;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets whether to use the theme font.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("If true, the tree's font is always set to the theme font.")]
        public new bool UseThemeFont
        {
            get => _useThemeFont;
            set
            {
                _useThemeFont = value;
                ApplyTheme();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets the text alignment for node labels.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        public TextAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                _textAlignment = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets whether checkboxes are shown for each node.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        public bool ShowCheckBox
        {
            get => _showCheckBox;
            set
            {
                _showCheckBox = value;
                RecalculateLayoutCache();
                UpdateScrollBars();
                try { _treeHitTestHelper?.RegisterHitAreas(); } catch { }
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether checkboxes support three states (unchecked, checked, indeterminate)
        /// with automatic cascade behavior to child and parent nodes.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("If true, checkboxes support three states with cascade behavior to children and parents.")]
        [DefaultValue(false)]
        public bool EnableThreeStateCheckboxes
        {
            get => _enableThreeStateCheckboxes;
            set
            {
                _enableThreeStateCheckboxes = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the text shown when the tree has no nodes.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text displayed in the center of the tree when there are no nodes.")]
        [DefaultValue("No items to display")]
        public string EmptyStateText
        {
            get => _emptyStateText;
            set { _emptyStateText = value ?? string.Empty; Invalidate(); }
        }
        
        #endregion
        
        #region Tree-Specific Theme Colors
        
        /// <summary>
        /// Gets the tree background color from the current theme.
        /// </summary>
        public Color TreeBackColor => _currentTheme?.TreeBackColor ?? BackColor;
        
        /// <summary>
        /// Gets the tree foreground color from the current theme.
        /// </summary>
        public Color TreeForeColor => _currentTheme?.TreeForeColor ?? ForeColor;
        
        /// <summary>
        /// Gets the selected node background color from the current theme.
        /// </summary>
        public Color TreeNodeSelectedBackColor => _currentTheme?.TreeNodeSelectedBackColor ?? Color.Blue;
        
        /// <summary>
        /// Gets the selected node foreground color from the current theme.
        /// </summary>
        public Color TreeNodeSelectedForeColor => _currentTheme?.TreeNodeSelectedForeColor ?? Color.White;
        
        /// <summary>
        /// Gets the hovered node background color from the current theme.
        /// </summary>
        public Color TreeNodeHoverBackColor => _currentTheme?.TreeNodeHoverBackColor ?? Color.LightBlue;
        
        /// <summary>
        /// Gets the hovered node foreground color from the current theme.
        /// </summary>
        public Color TreeNodeHoverForeColor => _currentTheme?.TreeNodeHoverForeColor ?? Color.Black;
        
        #endregion
        
        #region Node Data and Selection Properties
        
        /// <summary>
        /// Gets or sets the collection of root nodes in the tree.
        /// Setting this property will automatically rebuild the visible nodes and refresh the display.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The collection of root nodes displayed in the tree.")]
        [Localizable(true)]
        [MergableProperty(false)]
        [Editor(typeof(MenuItemCollectionEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SimpleItem> Nodes
        {
            get => _nodes;
            set
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"BeepTree.Nodes setter called with {value?.Count ?? 0} items");
#endif
                // Replace the list
                _nodes = value ?? new List<SimpleItem>();
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"BeepTree._nodes now has {_nodes.Count} items");
#endif
                RefreshTree();
            }
        }
        
        /// <summary>
        /// Refreshes the entire tree by rebuilding visible nodes and invalidating display.
        /// Call this method after making changes to node properties or structure.
        /// </summary>
        public void RefreshTree()
        {
            RebuildVisible();
            UpdateScrollBars();
            Invalidate();
            
            // Force update in design mode
            if (DesignMode)
            {
                Refresh();
            }
        }
        
        /// <summary>
        /// Gets or sets the currently selected node.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SimpleItem SelectedNode
        {
            get => _lastSelectedNode;
            set
            {
                if (_lastSelectedNode != value)
                {
                    if (_lastSelectedNode != null)
                        _lastSelectedNode.IsSelected = false;
                    
                    _lastSelectedNode = value;
                    
                    if (_lastSelectedNode != null)
                    {
                        _lastSelectedNode.IsSelected = true;
                    }

                    // Fire selection events consistently for all selection changes
                    var args = new BeepMouseEventArgs("SelectionChanged", _lastSelectedNode);
                    NodeSelected?.Invoke(this, args);
                    OnSelectedItemChanged(_lastSelectedNode);

                    // Notify accessibility clients (screen readers)
                    var treeAccessibleObject = AccessibilityObject as BeepTreeAccessibleObject;
                    treeAccessibleObject?.NotifySelectionChanged(_lastSelectedNode);
                }
                else if (_lastSelectedNode != null)
                {
                    // Ensure IsSelected is set even if re-selecting same node
                    _lastSelectedNode.IsSelected = true;
                }
            }
        }
        
        /// <summary>
        /// Gets the list of currently selected nodes (for multi-select).
        /// </summary>
        public List<SimpleItem> SelectedNodes
        {
            get => _selectedNodes;
            private set => _selectedNodes = value;
        }
        
        /// <summary>
        /// Gets the most recently clicked node.
        /// </summary>
        public SimpleItem ClickedNode { get; internal set; }
        
        /// <summary>
        /// Gets or sets whether multiple nodes can be selected at once.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        public bool AllowMultiSelect
        {
            get => _allowMultiSelect;
            set
            {
                _allowMultiSelect = value;
                if (!_allowMultiSelect && SelectedNodes != null && SelectedNodes.Count > 1)
                {
                    // Deselect all but the first node
                    for (int i = 1; i < SelectedNodes.Count; i++)
                    {
                        SelectedNodes[i].IsSelected = false;
                    }
                    SelectedNodes = SelectedNodes.Take(1).ToList();
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether drag and drop operations are allowed.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("If true, nodes can be dragged and dropped to reorder or reparent.")]
        [DefaultValue(false)]
        public bool AllowDragDrop
        {
            get => _allowDragDrop;
            set
            {
                _allowDragDrop = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether inline editing is allowed.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("If true, nodes can be edited inline by pressing F2 or slow double-clicking.")]
        [DefaultValue(false)]
        public bool AllowEdit { get; set; } = false;

        /// <summary>
        /// Gets the collection of conditional formatting rules.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Conditional formatting rules that change row/cell appearance based on values.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BeepTreeConditionalFormatCollection ConditionalFormats { get; } = new BeepTreeConditionalFormatCollection();

        /// <summary>
        /// Gets or sets whether to show breadcrumb navigation above the tree.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("If true, shows breadcrumb navigation above the tree showing the path to the selected node.")]
        [DefaultValue(false)]
        public bool ShowBreadcrumb
        {
            get => _showBreadcrumb;
            set
            {
                _showBreadcrumb = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether animations are enabled for expand/collapse and selection.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("If true, enables smooth animations for expand/collapse and selection transitions.")]
        [DefaultValue(false)]
        public bool EnableAnimations
        {
            get => _enableAnimations;
            set
            {
                _enableAnimations = value;
                if (!value)
                {
                    _animationHelper?.StopAnimation();
                }
            }
        }

        /// <summary>
        /// Gets the breadcrumb path from root to the selected node.
        /// </summary>
        [Browsable(false)]
        public List<SimpleItem> BreadcrumbPath
        {
            get
            {
                var path = new List<SimpleItem>();
                var current = SelectedNode;
                while (current != null)
                {
                    path.Insert(0, current);
                    current = current.ParentItem;
                }
                return path;
            }
        }

        #region Theme Overrides

        /// <summary>
        /// Gets or sets the background color for selected nodes.
        /// Set to Empty to use the theme default.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background color for selected nodes. Set to Empty to use theme default.")]
        public Color SelectedNodeBackColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the foreground color for selected nodes.
        /// Set to Empty to use the theme default.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Foreground color for selected nodes. Set to Empty to use theme default.")]
        public Color SelectedNodeForeColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the background color for hovered nodes.
        /// Set to Empty to use the theme default.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background color for hovered nodes. Set to Empty to use theme default.")]
        public Color HoverNodeBackColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the foreground color for hovered nodes.
        /// Set to Empty to use the theme default.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Foreground color for hovered nodes. Set to Empty to use theme default.")]
        public Color HoverNodeForeColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the color for the focus indicator (dotted line around selected node).
        /// Set to Empty to use the theme default.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color for the focus indicator. Set to Empty to use theme default.")]
        public new Color FocusIndicatorColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the color for column header text.
        /// Set to Empty to use the theme default.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color for column header text. Set to Empty to use theme default.")]
        public Color ColumnHeaderForeColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the background color for column headers.
        /// Set to Empty to use the theme default.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background color for column headers. Set to Empty to use theme default.")]
        public Color ColumnHeaderBackColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the color for grid lines.
        /// Set to Empty to use the theme default.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color for grid lines. Set to Empty to use theme default.")]
        public Color GridLineColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the color for the sort indicator in column headers.
        /// Set to Empty to use the theme default.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color for the sort indicator in column headers. Set to Empty to use theme default.")]
        public Color SortIndicatorColor { get; set; } = Color.Empty;

        /// <summary>
        /// Gets or sets the color for the filter indicator in column headers.
        /// Set to Empty to use the theme default.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color for the filter indicator in column headers. Set to Empty to use theme default.")]
        public Color FilterIndicatorColor { get; set; } = Color.Empty;

        #endregion

        #region Async Image Loading

        /// <summary>
        /// Gets or sets whether async image loading from URLs is enabled.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("If true, node images from URLs are loaded asynchronously with a loading placeholder.")]
        [DefaultValue(true)]
        public bool EnableAsyncImageLoading
        {
            get => _enableAsyncImageLoading;
            set
            {
                _enableAsyncImageLoading = value;
                if (!value)
                    _asyncImageLoader?.ClearCache();
            }
        }

        /// <summary>
        /// Gets the async image loader instance for preloading or managing cached images.
        /// </summary>
        [Browsable(false)]
        public BeepTreeAsyncImageLoader AsyncImageLoader => _asyncImageLoader;

        #endregion

        /// <summary>
        /// Gets or sets whether kinetic (momentum) scrolling is enabled.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("If true, scrolling continues with momentum after mouse drag release.")]
        [DefaultValue(false)]
        public bool EnableKineticScrolling
        {
            get => _enableKineticScrolling;
            set
            {
                _enableKineticScrolling = value;
                if (!value && _kineticTimer != null)
                {
                    _kineticTimer.Stop();
                    _isKineticScrolling = false;
                }
            }
        }
        
        #endregion
        
        #region Performance and Layout Properties
        
        /// <summary>
        /// Gets or sets whether to virtualize layout for performance.
        /// </summary>
        [Browsable(true)]
        [Category("Performance")]
        [Description("If true, only measures nodes near the viewport to reduce work on massive trees.")]
        public bool VirtualizeLayout
        {
            get => _virtualizeLayout;
            set
            {
                _virtualizeLayout = value;
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets the extra rows to measure when virtualizing.
        /// </summary>
        [Browsable(true)]
        [Category("Performance")]
        [Description("Extra rows to measure above/below the viewport when virtualization is enabled.")]
        public int VirtualizationBufferRows
        {
            get => _virtualizationBufferRows;
            set
            {
                _virtualizationBufferRows = Math.Max(0, value);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether background layout calculation is enabled for massive trees.
        /// When enabled and the tree has more than 10,000 nodes, layout is calculated
        /// on a background thread to keep the UI responsive.
        /// </summary>
        [Browsable(true)]
        [Category("Performance")]
        [Description("If true, layout for massive trees (>10,000 nodes) is calculated on a background thread.")]
        [DefaultValue(true)]
        public bool EnableBackgroundLayout
        {
            get => _enableBackgroundLayout;
            set => _enableBackgroundLayout = value;
        }
        
        #endregion
        
        #region Scrollbar Properties
        
        /// <summary>
        /// Gets or sets whether the vertical scrollbar is shown.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowVerticalScrollBar
        {
            get => _showVerticalScrollBar;
            set
            {
                _showVerticalScrollBar = value;
                UpdateScrollBars();
                Invalidate();
            }
        }
        
        /// <summary>
        /// Gets or sets whether the horizontal scrollbar is shown.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowHorizontalScrollBar
        {
            get => _showHorizontalScrollBar;
            set
            {
                _showHorizontalScrollBar = value;
                UpdateScrollBars();
                Invalidate();
            }
        }
        
        #endregion
        
        #region Context Menu Properties
        
        /// <summary>
        /// Gets or sets the current menu items.
        /// </summary>
        public BindingList<SimpleItem> CurrentMenutems
        {
            get => _currentMenuItems;
            set
            {
                _currentMenuItems = value;
                Invalidate();
            }
        }
        
        #endregion

        #region Search and Filter Properties

        /// <summary>
        /// Gets or sets the filter text. Setting this property immediately filters the visible nodes
        /// to only those whose <see cref="SimpleItem.Text"/> contains the value (case-insensitive).
        /// Set to <see langword="null"/> or an empty string to clear the filter.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("When non-empty, only nodes whose text contains this value (and their ancestors) are shown.")]
        [DefaultValue("")]
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value ?? string.Empty;
                ApplyFilter();
            }
        }

        #endregion

        #region Columns

        private BeepTreeColumnCollection _columns;

        /// <summary>
        /// Gets the collection of columns for multi-column tree display.
        /// When columns are present, the tree displays data in a grid-like format
        /// with column headers and cell values.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The collection of columns for multi-column tree display.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(BeepTreeColumnCollectionEditor), typeof(UITypeEditor))]
        public BeepTreeColumnCollection Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new BeepTreeColumnCollection(this);
                    _columns.CollectionChanged += (s, e) =>
                    {
                        RecalculateLayoutCache();
                        UpdateScrollBars();
                        Invalidate();
                    };
                }
                return _columns;
            }
        }

        /// <summary>
        /// Gets whether the tree is in multi-column mode (has visible columns).
        /// </summary>
        [Browsable(false)]
        public bool IsMultiColumn => Columns != null && Columns.GetVisibleColumns().GetEnumerator().MoveNext();

        /// <summary>
        /// Gets or sets the height of the column header row.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The height of the column header row in pixels.")]
        [DefaultValue(24)]
        public int ColumnHeaderHeight { get; set; } = 24;

        /// <summary>
        /// Gets or sets whether to show column headers.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether to show column headers when in multi-column mode.")]
        [DefaultValue(true)]
        public bool ShowColumnHeaders { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to show grid lines between columns.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Whether to show grid lines between columns and rows.")]
        [DefaultValue(false)]
        public bool ShowGridLines { get; set; } = false;

        /// <summary>
        /// Gets or sets whether selecting a node selects the entire row (true) or just the cell (false).
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Whether selecting a node selects the entire row (true) or just the cell (false).")]
        [DefaultValue(true)]
        public bool FullRowSelect { get; set; } = true;

        #endregion

        #region Data Binding

        private object _dataSource;
        private string _dataMember;
        private string _keyFieldName;
        private string _parentFieldName;
        private string _displayMember;
        private string _valueMember;
        private string _imageMember;

        /// <summary>
        /// Gets or sets the data source for the tree.
        /// Supports DataTable, BindingList, IList, and other bindable collections.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The data source for the tree. Supports DataTable, BindingList, IList, and other bindable collections.")]
        [AttributeProvider(typeof(IListSource))]
        public object DataSource
        {
            get => _dataSource;
            set
            {
                if (_dataSource != value)
                {
                    _dataSource = value;
                    OnDataSourceChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the data member in a complex data source.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The data member in a complex data source (e.g., DataSet table name).")]
        public string DataMember
        {
            get => _dataMember;
            set
            {
                if (_dataMember != value)
                {
                    _dataMember = value;
                    OnDataSourceChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the field name that uniquely identifies each row (for self-referencing hierarchies).
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The field name that uniquely identifies each row (for self-referencing hierarchies).")]
        public string KeyFieldName
        {
            get => _keyFieldName;
            set
            {
                if (_keyFieldName != value)
                {
                    _keyFieldName = value;
                    OnDataSourceChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the field name that identifies the parent row (for self-referencing hierarchies).
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The field name that identifies the parent row (for self-referencing hierarchies).")]
        public string ParentFieldName
        {
            get => _parentFieldName;
            set
            {
                if (_parentFieldName != value)
                {
                    _parentFieldName = value;
                    OnDataSourceChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the field name to display as node text.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The field name to display as node text.")]
        public string DisplayMember
        {
            get => _displayMember;
            set
            {
                if (_displayMember != value)
                {
                    _displayMember = value;
                    OnDataSourceChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the field name to use as the node value.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The field name to use as the node value.")]
        public string ValueMember
        {
            get => _valueMember;
            set
            {
                if (_valueMember != value)
                {
                    _valueMember = value;
                    OnDataSourceChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the field name to use for node icons.
        /// </summary>
        [Browsable(true)]
        [Category("Data")]
        [Description("The field name to use for node icons.")]
        public string ImageMember
        {
            get => _imageMember;
            set
            {
                if (_imageMember != value)
                {
                    _imageMember = value;
                    OnDataSourceChanged();
                }
            }
        }

        /// <summary>
        /// Gets whether the tree is bound to a data source.
        /// </summary>
        [Browsable(false)]
        public bool IsDataBound => _dataSource != null;

        /// <summary>
        /// Gets or sets whether to load child nodes on demand when expanding.
        /// When true, the NodesNeeded event is fired when a node is expanded and has no children.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("When true, child nodes are loaded on demand via the NodesNeeded event.")]
        [DefaultValue(false)]
        public bool LazyLoad { get; set; } = false;

        /// <summary>
        /// Gets or sets whether the tree is currently loading data.
        /// </summary>
        [Browsable(false)]
        public bool IsLoading { get; set; } = false;

        /// <summary>
        /// Gets or sets the text displayed when the tree is loading.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text displayed when the tree is loading data.")]
        [DefaultValue("Loading...")]
        public string LoadingText { get; set; } = "Loading...";

        /// <summary>
        /// Occurs when the data source changes.
        /// </summary>
        public event EventHandler DataSourceChanged;

        /// <summary>
        /// Raises the DataSourceChanged event and rebuilds the tree.
        /// </summary>
        protected virtual void OnDataSourceChanged()
        {
            DataSourceChanged?.Invoke(this, EventArgs.Empty);
            // Rebuild tree from data source
            if (_dataSource != null)
            {
                try
                {
                    RebuildFromDataSource();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[BeepTree] Failed to rebuild from data source: {ex.Message}");
                }
            }
        }

        #endregion
    }
}
