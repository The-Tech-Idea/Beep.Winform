using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Models;
using Timer = System.Windows.Forms.Timer;

// Add enum for sort direction


namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    [ToolboxItem(true)]
    [Category("Data")]
    [Description("High-performance grid control with virtual scrolling, custom cell rendering, and modern UI features Found in Web.")]
    [DisplayName("Beep Grid")]
    public class BeepGrid : BeepControl
    {
        #region Fields

        // Grid Component References - Modern Component Architecture
        private GridTitleHeaderPanel _titleHeaderPanel;
        private GridColumnHeaderPanel _columnHeaderPanel;
        private GridFilterPanel _filterPanel;
        private GridNavigationPanel _navigationPanel;
        private bool _useComponentArchitecture = true;

        // Performance optimization fields
        private bool _needsRedraw = true;
        private bool _layoutDirty = true;
        private bool _isInitializing = true;
        private bool _deferRedraw = false;
        private Dictionary<int, Rectangle> _cachedCellBounds = new Dictionary<int, Rectangle>();
        private Dictionary<string, Size> _cachedTextSizes = new Dictionary<string, Size>();

        // Enhanced data source update management
        private bool _enableTransactions = true;
        private Dictionary<object, Dictionary<string, object>> _pendingChanges = new Dictionary<object, Dictionary<string, object>>();
        private Dictionary<object, Dictionary<string, object>> _originalValues = new Dictionary<object, Dictionary<string, object>>();
        private HashSet<object> _dirtyItems = new HashSet<object>();
        private bool _batchUpdateMode = false;
        private Dictionary<Type, HashSet<string>> _readOnlyProperties = new Dictionary<Type, HashSet<string>>();
        private Dictionary<string, Func<object, object, bool>> _customValidators = new Dictionary<string, Func<object, object, bool>>();
        private bool _autoCommitChanges = true;
        private int _maxRetryAttempts = 3;
        private bool _validateOnUpdate = true;

        // Virtual scrolling and layout
        private int _virtualRowCount = 0;
        private int _visibleRowStart = 0;
        private int _visibleRowEnd = 0;
        private int _virtualRowHeight = 28;
        private int _headerHeight = 32;

        // Data management
        private object _dataSource;
        private List<object> _filteredData = new List<object>();
        private List<object> _originalData = new List<object>();
        private List<object> _fullData = new List<object>();
        private EntityStructure _entityStructure;
        private Type _dataType;
        private Type _entityType;
        private int _dataOffset = 0;

        // Grid configuration and layout
        private List<BeepColumnConfig> _columns = new List<BeepColumnConfig>();
        private Dictionary<int, BeepCellConfig> _visibleCells = new Dictionary<int, BeepCellConfig>();
        private Rectangle _gridBounds;
        private Rectangle _headerBounds;
        private Rectangle _footerBounds;
        private Rectangle _scrollableBounds;
        private Rectangle filterPanelRect;
        private Rectangle headerPanelRect;
        private Rectangle columnsheaderPanelRect;
        private Rectangle bottomagregationPanelRect;
        private Rectangle navigatorPanelRect;
        private Rectangle footerPanelRect;

        // Panel heights and spacing
        private int filtercontrolsheight = 5;
        private int filterPanelHeight = 60;
        private int headerPanelHeight = 30;
        private int bottomagregationPanelHeight = 20;
        private int footerPanelHeight = 12;
        private int navigatorPanelHeight = 30;
        private int _panelSpacing = 2;

        // Row management and display
        public BindingList<BeepRowConfig> Rows { get; set; } = new BindingList<BeepRowConfig>();
        public BeepRowConfig aggregationRow { get; set; }
        private Dictionary<int, int> _rowHeights = new Dictionary<int, int>();
        private int _rowHeight = 25;
        private int _startviewrow = 0;
        private int _endviewrow = 0;

        // Selection and interaction
        private int _selectedRowIndex = -1;
        private int _selectedColumnIndex = -1;
        private int _hoveredRowIndex = -1;
        private int _hoveredColumnIndex = -1;
        private int _currentRowIndex = -1;
        private BeepCellConfig _selectedCell;
        private BeepCellConfig _editingCell;
        private BeepRowConfig _hoveredRow = null;
        private BeepRowConfig _currentRow = null;
        private BeepCellConfig _hoveredCell = null;
        private Control _cellEditor;

        // Header interaction
        private int _hoveredColumnHeaderIndex = -1;
        private int _selectedColumnHeaderIndex = -1;
        private int _hoveredRowHeaderIndex = -1;
        private int _selectedRowHeaderIndex = -1;
        private int _hoveredSortIconIndex = -1;
        private int _hoveredFilterIconIndex = -1;
        private int _selectedSortIconIndex = -1;
        private int _selectedFilterIconIndex = -1;

        // Bounds and areas for interaction
        private List<Rectangle> columnHeaderBounds = new List<Rectangle>();
        private List<Rectangle> rowHeaderBounds = new List<Rectangle>();
        private List<Rectangle> sortIconBounds = new List<Rectangle>();
        private List<Rectangle> filterIconBounds = new List<Rectangle>();

        // Scrollbars and scrolling
        private BeepScrollBar _vScrollBar;
        private BeepScrollBar _hScrollBar;
        private int _horizontalOffset = 0;
        private int _verticalOffset = 0;
        private int _xOffset = 0;
        private int _scrollTargetVertical;
        private int _scrollTargetHorizontal;
        private Timer _scrollTimer;
        private int _scrollTarget;
        private int _scrollStep = 10;
        private int _cachedTotalColumnWidth = 0;
        private int _cachedMaxXOffset = 0;

        // Filtering and sorting
        private bool _showFilter = false;
        private bool _showFilterpanel = false;
        private Dictionary<string, string> _columnFilters = new Dictionary<string, string>();
        private List<Control> _filterControls = new List<Control>();
        private string _sortColumn;
        private bool _sortAscending = true;
        private List<object> _filteredDataCache = new List<object>();

        // Panel management
        private BeepPanel _headerPanel;
        private BeepPanel _filterPanelControl;
        private BeepPanel _footerPanel;
        private BeepLabel _titleLabel;
        private BeepTextBox filterTextBox;
        private BeepComboBox filterColumnComboBox;
        private BeepButton filterButton;
        private BeepLabel percentageLabel;

        // Hit testing for performance
        private List<HitTestArea> _hitAreas = new List<HitTestArea>();

        // Cell rendering controls
        private Dictionary<string, IBeepUIComponent> _columnEditors = new Dictionary<string, IBeepUIComponent>();

        // Modern styling
        private Dictionary<int, SortDirection> _columnSortStates = new Dictionary<int, SortDirection>();
        private Dictionary<int, bool> _columnFilterStates = new Dictionary<int, bool>();
        private int _hoveredHeaderColumn = -1;
        private BeepGridStyle _gridStyle = BeepGridStyle.Classic;

        // Row Selection Checkbox Fields
        private bool _showCheckboxes = false;
        private BeepCheckBoxBool _selectAllCheckBox;
        private Dictionary<int, bool> _selectedRows = new Dictionary<int, bool>();
        private HashSet<object> _selectedDataItems = new HashSet<object>();
        private Dictionary<int, bool> _persistentSelectedRows = new Dictionary<int, bool>();
        private List<BeepRowConfig> _selectedgridrows = new List<BeepRowConfig>();
        private int _selectionColumnWidth = 30;
        private bool _hasSelectionColumn = false;
        private int _stickyWidth = 0;
        private Rectangle _stickyRegion;
        private Rectangle _scrollableRegion;

        // Title and appearance properties
        private string _titletext = "";
        private string _titleimagestring = "simpleinfoapps.svg";
        private Font _textFont;
        private Font _columnHeadertextFont = new Font("Arial", 8);
        private TextImageRelation textImageRelation = TextImageRelation.ImageAboveText;

        // Column configuration
        private int _defaultcolumnheaderheight = 40;
        private int _defaultcolumnheaderwidth = 50;
        private bool columnssetupusingeditordontchange = false;

        // Display flags
        private bool _showverticalgridlines = true;
        private bool _showhorizontalgridlines = true;
        private bool _showSortIcons = true;
        private bool _showRowNumbers = true;
        private bool _showColumnHeaders = true;
        private bool _showRowHeaders = true;
        private bool _showNavigator = true;
        private bool _showaggregationRow = false;
        private bool _showFooter = false;
        private bool _showHeaderPanel = true;
        private bool _showHeaderPanelBorder = true;
        private bool _showVerticalScrollBar = true;
        private bool _showHorizontalScrollBar = true;
        private bool _showFilterButton = true;

        // Control pool and caching
        private Dictionary<BeepColumnType, List<IBeepUIComponent>> _controlPool =
            new Dictionary<BeepColumnType, List<IBeepUIComponent>>();
        private Dictionary<string, Size> _columnTextSizes = new Dictionary<string, Size>();
        private Dictionary<int, Rectangle> _cellBounds = new Dictionary<int, Rectangle>();

        // Resizing logic
        private bool _resizingColumn = false;
        private bool _resizingRow = false;
        private int _resizingIndex = -1;
        private int _resizeMargin = 2;
        private Point _lastMousePos;
        private Timer _resizeTimer;
        private bool _isResizing = false;
        private Size _pendingSize;
        private Size _lastCalculatedSize;

        // Editor state
        private BeepControl _editingControl = null;
        private BeepCellConfig _tempcell = null;
        private int _editingRowIndex = -1;
        public bool IsEditorShown { get; private set; }

        // Data source and binding
        private bool _isAddingNewRecord = false;
        private BindingSource _bindingSource;
        private object finalData;

        // Change tracking and logging
        private List<object> originalList = new List<object>();
        private List<object> deletedList = new List<object>();
        public List<Tracking> Trackings { get; set; } = new List<Tracking>();
        private Dictionary<object, Dictionary<string, object>> ChangedValues = new Dictionary<object, Dictionary<string, object>>();
        public bool IsLogging { get; set; } = false;
        public Dictionary<DateTime, EntityUpdateInsertLog> UpdateLog { get; set; } = new Dictionary<DateTime, EntityUpdateInsertLog>();

        // Navigation and paging
        private bool _navigatorDrawn = false;
        private bool _filterpaneldrawn = false;
        private bool tooltipShown = false;
        private int _currentPage = 1;
        private int _totalPages = 1;

        // Special display properties
        private string _percentageText = string.Empty;
        public GridDataSourceType DataSourceType { get; set; } = GridDataSourceType.Fixed;

        // Navigation controls
        public string QueryFunctionName { get; set; }
        public string QueryFunction { get; set; }
        public bool VerifyDelete = false;

        // External data navigator
        private BeepBindingNavigator _dataNavigator;

        #endregion

        #region Properties

        // Enhanced Data Source Update Properties
        [Browsable(true)]
        [Category("Data")]
        [DefaultValue(true)]
        public bool EnableTransactions
        {
            get => _enableTransactions;
            set => _enableTransactions = value;
        }

        [Browsable(true)]
        [Category("Data")]
        [DefaultValue(true)]
        public bool AutoCommitChanges
        {
            get => _autoCommitChanges;
            set => _autoCommitChanges = value;
        }

        [Browsable(true)]
        [Category("Data")]
        [DefaultValue(true)]
        public bool ValidateOnUpdate
        {
            get => _validateOnUpdate;
            set => _validateOnUpdate = value;
        }

        [Browsable(false)]
        public bool HasPendingChanges => _pendingChanges.Count > 0;

        // Data Source Properties
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsInitializing
        {
            get => _isInitializing;
            set => _isInitializing = value;
        }

        [Browsable(true)]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string EntityName { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EntityStructure Entity
        {
            get => _entityStructure;
            set
            {
                _entityStructure = value;
                if (_entityStructure != null && !string.IsNullOrEmpty(_entityStructure.EntityName))
                {
                    try
                    {
                        EntityName = _entityStructure.EntityName;
                        if (_entityType == null)
                            _entityType = Type.GetType(_entityStructure.EntityName);
                    }
                    catch (Exception)
                    {
                        // Handle type resolution errors
                    }
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [AttributeProvider(typeof(IListSource))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public new object DataSource
        {
            get => _dataSource;
            set
            {
                if (_isInitializing)
                {
                    // Defer setting until initialization is complete
                    return;
                }

                if (_dataSource != value)
                {
                    _dataSource = value;
                    RefreshData();
                }
            }
        }

        // Column Configuration Properties
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ColumnsSetupUsingEditorDontChange
        {
            get => columnssetupusingeditordontchange;
            set => columnssetupusingeditordontchange = value;
        }

        [Browsable(true)]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<BeepColumnConfig> Columns
        {
            get => _columns;
            set
            {
                _columns = value ?? new List<BeepColumnConfig>();
                if (_columns != null && _columns.Any())
                {
                    // Reindex columns based on sequence in list
                    for (int i = 0; i < _columns.Count; i++)
                    {
                        _columns[i].Index = i;
                    }
                    columnssetupusingeditordontchange = true;
                }
                _layoutDirty = true;
                InvalidateLayout();
            }
        }

        [Browsable(true)]
        [Category("Data")]
        public EntityStructure EntityStructure
        {
            get => _entityStructure;
            set
            {
                _entityStructure = value;
                if (_entityStructure != null)
                {
                    GenerateColumnsFromEntity();
                }
            }
        }

        // Layout and Appearance Properties
        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(true)]
        [Description("Shows or hides the navigation bar")]
        public bool ShowNavigator
        {
            get => _showNavigator;
            set
            {
                if (_showNavigator != value)
                {
                    _showNavigator = value;
                    _layoutDirty = true;
                    InvalidateLayout();
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(28)]
        public int VirtualRowHeight
        {
            get => _virtualRowHeight;
            set
            {
                if (_virtualRowHeight != value && value > 10)
                {
                    _virtualRowHeight = value;
                    RecalculateVirtualRows();
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        public int RowHeight
        {
            get => _rowHeight;
            set
            {
                _rowHeight = value;
                InitializeRows();
                UpdateScrollBars();
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(32)]
        public int HeaderHeight
        {
            get => _headerHeight;
            set
            {
                if (_headerHeight != value && value > 16)
                {
                    _headerHeight = value;
                    _layoutDirty = true;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DefaultColumnHeaderWidth
        {
            get => _defaultcolumnheaderwidth;
            set
            {
                _defaultcolumnheaderwidth = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ColumnHeaderHeight
        {
            get => _defaultcolumnheaderheight;
            set
            {
                _defaultcolumnheaderheight = value;
                Invalidate();
            }
        }

        // Filter Properties
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ShowFilter
        {
            get => _showFilter;
            set
            {
                if (_showFilter != value)
                {
                    _showFilter = value;
                    _showFilterpanel = value;
                    ToggleFilterPanel();
                }
            }
        }

        // Title Properties
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleText
        {
            get => _titletext;
            set
            {
                _titletext = value;
                if (_titleLabel != null)
                {
                    _titleLabel.Text = value;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Image alignment relative to text.")]
        public TextImageRelation TextImageRelation
        {
            get => textImageRelation;
            set
            {
                textImageRelation = value;
                if (_titleLabel != null)
                {
                    _titleLabel.TextImageRelation = value;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text font for the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TitleTextFont
        {
            get => _textFont ?? Font;
            set
            {
                _textFont = value;
                if (_titleLabel != null)
                {
                    _titleLabel.TextFont = _textFont;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the Header image file (SVG, PNG, JPG, etc.) to load.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleHeaderImage
        {
            get => _titleimagestring ?? "simpleinfoapps.svg";
            set
            {
                _titleimagestring = value;
                if (_titleLabel != null)
                {
                    _titleLabel.ImagePath = _titleimagestring;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text font for column headers.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font ColumnHeaderFont
        {
            get => _columnHeadertextFont;
            set
            {
                _columnHeadertextFont = value;
                Invalidate();
            }
        }

        // Grid Style Properties
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(BeepGridStyle.Classic)]
        [Description("Visual style of the grid")]
        public BeepGridStyle GridStyle
        {
            get => _gridStyle;
            set
            {
                if (_gridStyle != value)
                {
                    _gridStyle = value;
                    ApplyGridStyle();
                    Invalidate();
                }
            }
        }

        // Display Options
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowVerticalGridLines
        {
            get => _showverticalgridlines;
            set
            {
                _showverticalgridlines = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowHorizontalGridLines
        {
            get => _showhorizontalgridlines;
            set
            {
                _showhorizontalgridlines = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowSortIcons
        {
            get => _showSortIcons;
            set
            {
                _showSortIcons = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowRowNumbers
        {
            get => _showRowNumbers;
            set
            {
                _showRowNumbers = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowColumnHeaders
        {
            get => _showColumnHeaders;
            set
            {
                _showColumnHeaders = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowRowHeaders
        {
            get => _showRowHeaders;
            set
            {
                _showRowHeaders = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowAggregationRow
        {
            get => _showaggregationRow;
            set
            {
                _showaggregationRow = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowFooter
        {
            get => _showFooter;
            set
            {
                _showFooter = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowHeaderPanel
        {
            get => _showHeaderPanel;
            set
            {
                _showHeaderPanel = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowHeaderPanelBorder
        {
            get => _showHeaderPanelBorder;
            set
            {
                _showHeaderPanelBorder = value;
                Invalidate();
            }
        }

        // Scrollbar Properties
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

        [Browsable(true)]
        [Category("Layout")]
        [Description("Horizontal offset for drawing grid cells.")]
        public int XOffset
        {
            get => _xOffset;
            set
            {
                _xOffset = value;
                UpdateScrollBars();
                Invalidate();
            }
        }

        // Selection Properties
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("Shows or hides row selection checkboxes")]
        public bool ShowCheckboxes
        {
            get => _showCheckboxes;
            set
            {
                if (_showCheckboxes != value)
                {
                    _showCheckboxes = value;
                    EnsureSelectionColumn();
                    _layoutDirty = true;
                    Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(30)]
        [Description("Width of the selection checkbox column")]
        public int SelectionColumnWidth
        {
            get => _selectionColumnWidth;
            set
            {
                if (_selectionColumnWidth != value && value > 20)
                {
                    _selectionColumnWidth = value;
                    if (_hasSelectionColumn)
                    {
                        var selColumn = _columns.FirstOrDefault(c => c.IsSelectionCheckBox);
                        if (selColumn != null)
                        {
                            selColumn.Width = value;
                            _layoutDirty = true;
                            Invalidate();
                        }
                    }
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<int> SelectedRows { get; set; } = new List<int>();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<BeepRowConfig> SelectedGridRows { get; set; } = new List<BeepRowConfig>();

        [Browsable(false)]
        public Dictionary<int, bool> SelectedRowsDict => new Dictionary<int, bool>(_selectedRows);

        [Browsable(false)]
        public HashSet<object> SelectedDataItems => new HashSet<object>(_selectedDataItems);

        // State Properties
        [Browsable(false)]
        public BeepCellConfig SelectedCell => _selectedCell;

        [Browsable(false)]
        public int SelectedRowIndex => _selectedRowIndex;

        [Browsable(false)]
        public int SelectedColumnIndex => _selectedColumnIndex;

        [Browsable(false)]
        public BeepRowConfig CurrentRow => _currentRow;

        // External Data Navigator
        [Browsable(true)]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BeepBindingNavigator DataNavigator
        {
            get => _dataNavigator;
            set
            {
                if (_dataNavigator != value)
                {
                    // Detach events from old navigator
                    if (_dataNavigator != null)
                    {
                        DetachNavigatorEvents();
                    }

                    _dataNavigator = value;
                    if (_dataNavigator != null)
                    {
                        _dataNavigator.Theme = Theme;
                        AttachNavigatorEvents();
                    }
                    Invalidate();
                }
            }
        }

        // Display Properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("If true, grid will be drawn in black and white (monochrome) instead of color.")]
        public bool DrawInBlackAndWhite { get; set; } = false;

        [Browsable(true)]
        [Category("Appearance")]
        public string PercentageText
        {
            get => _percentageText;
            set
            {
                _percentageText = value;
                if (percentageLabel != null)
                {
                    percentageLabel.Text = value;
                }
                Invalidate();
            }
        }

        #endregion

        #region Events

        // Enhanced Data Source Update Events
        public event EventHandler<DataUpdateEventArgs> BeforeDataUpdate;
        public event EventHandler<DataUpdateEventArgs> AfterDataUpdate;
        public event EventHandler<DataUpdateFailedEventArgs> DataUpdateFailed;
        public event EventHandler<ValidationFailedEventArgs> ValidationFailed;

        // Navigation Events
        public event EventHandler CallPrinter;
        public event EventHandler SendMessage;
        public event EventHandler ShowSearch;
        public event EventHandler NewRecordCreated;
        public event EventHandler SaveCalled;
        public event EventHandler DeleteCalled;
        public event EventHandler EditCalled;

        // Selection Events
        public event EventHandler SelectedRowsChanged;
        public event EventHandler<BeepCellSelectedEventArgs> CurrentCellChanged;
        public event EventHandler<BeepRowSelectedEventArgs> CurrentRowChanged;

        // Cell Events
        public event EventHandler<BeepCellEventArgs> CellValueChanged;
        public event EventHandler<BeepCellEventArgs> CellValueChanging;
        public event EventHandler<BeepCellEventArgs> CellValidating;
        public event EventHandler<BeepCellEventArgs> CellValidated;
        public event EventHandler<BeepCellEventArgs> CellFormatting;
        public event EventHandler<BeepCellEventArgs> CellFormatted;
        public event EventHandler<BeepCellEventArgs> CellClick;
        public event EventHandler<BeepCellEventArgs> CellDoubleClick;
        public event EventHandler<BeepCellEventArgs> CellMouseEnter;
        public event EventHandler<BeepCellEventArgs> CellMouseLeave;
        public event EventHandler<BeepCellEventArgs> CellMouseDown;
        public event EventHandler<BeepCellEventArgs> CellMouseUp;
        public event EventHandler<BeepCellEventArgs> CellPreUpdateCellValue;
        public event EventHandler<BeepCellEventArgs> CellCustomCellDraw;

        // Grid Events
        public event EventHandler CellSelected;
        public event EventHandler CellValueChangedEvent;
        public event EventHandler<GridHitTestInfo> CellClicked;
        public event EventHandler<GridHitTestInfo> CellDoubleClicked;
        public event EventHandler FilterChanged;
        public event EventHandler EditorClosed;

        #endregion

        #region Constructor

        public BeepGrid() : base()
        {
            // Set control styles for optimal performance and appearance
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.DoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.Selectable |
                    ControlStyles.ContainerControl, true);

            // Set focus and interaction properties
            TabStop = true;

            // Skip expensive operations in design mode
            if (DesignMode)
            {
                _isInitializing = true;
                _filterpaneldrawn = true;
                _navigatorDrawn = true;
                return;
            }

            // Initialize state
            _isInitializing = true;

            // Set default size
            Width = 400;
            Height = 300;

            // Initialize collections
            Rows = new BindingList<BeepRowConfig>();

            // Initialize core components
            InitializeComponents();
            InitializeScrollBars();
            InitializePanels();
            InitializeFilterControls();
            InitializeNavigationControls();

            // Setup event handlers
            SetupEventHandlers();

            // Initialize scroll timer for smooth scrolling
            _scrollTimer = new Timer { Interval = 16 }; // ~60 FPS
            _scrollTimer.Tick += ScrollTimer_Tick;

            // Initialize resize timer for performance
            _resizeTimer = new Timer();
            _resizeTimer.Interval = 150;
            _resizeTimer.Tick += ResizeTimer_Tick;

            // Apply theme initially
            ApplyThemeToChilds = false;

            // Subscribe to row changes
            Rows.ListChanged += Rows_ListChanged;

            // Complete initialization
            _isInitializing = false;
        }

        #endregion

        #region Initialization

        private void InitializeComponents()
        {
            // Initialize title label
            _titleLabel = new BeepLabel
            {
                Text = "BeepGrid",
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Height = 30,
                Theme = Theme,
                IsChild = true,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                MaxImageSize = new Size(20, 20),
                ShowAllBorders = false,
                ShowShadow = false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                ImagePath = _titleimagestring
            };

            // Initialize percentage label
            percentageLabel = new BeepLabel
            {
                Text = _percentageText,
                Theme = Theme,
                IsChild = true,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = false
            };

            // Initialize the Select All checkbox
            _selectAllCheckBox = new BeepCheckBoxBool
            {
                Theme = Theme,
                IsChild = true,
                CheckBoxSize = 16,
                CheckedValue = true,
                UncheckedValue = false,
                CurrentValue = false,
                Visible = false,
                IsFrameless = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                HideText = true
            };

            _selectAllCheckBox.StateChanged += SelectAllCheckBox_StateChanged;

            // Add components to controls
            Controls.Add(_titleLabel);
            Controls.Add(percentageLabel);
            Controls.Add(_selectAllCheckBox);
        }

        private void InitializeScrollBars()
        {
            // Vertical scrollbar
            _vScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Vertical,
                Theme = Theme,
                IsChild = true,
                Visible = false
            };
            _vScrollBar.Scroll += VScrollBar_Scroll;
            Controls.Add(_vScrollBar);

            // Horizontal scrollbar
            _hScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Horizontal,
                Theme = Theme,
                IsChild = true,
                Visible = false
            };
            _hScrollBar.Scroll += HScrollBar_Scroll;
            Controls.Add(_hScrollBar);
        }

        private void InitializePanels()
        {
            // Header panel
            _headerPanel = new BeepPanel
            {
                Height = headerPanelHeight,
                Dock = DockStyle.Top,
                Theme = Theme,
                IsChild = true,
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false
            };
            _headerPanel.Controls.Add(_titleLabel);
            Controls.Add(_headerPanel);

            // Filter panel (initially hidden)
            _filterPanelControl = new BeepPanel
            {
                Height = filterPanelHeight,
                Dock = DockStyle.Top,
                Theme = Theme,
                IsChild = true,
                Visible = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false
            };
            Controls.Add(_filterPanelControl);

            // Footer panel
            _footerPanel = new BeepPanel
            {
                Height = footerPanelHeight,
                Dock = DockStyle.Bottom,
                Theme = Theme,
                IsChild = true,
                Visible = false,
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false
            };
            Controls.Add(_footerPanel);
        }

        private void InitializeFilterControls()
        {
            // Initialize filter text box
            filterTextBox = new BeepTextBox
            {
                Theme = Theme,
                IsChild = true,
                Width = 150,
                Height = 20,
                PlaceholderText = "Filter ......"
            };
            filterTextBox.TextChanged += FilterTextBox_TextChanged;
            Controls.Add(filterTextBox);

            // Initialize filter column combo box
            filterColumnComboBox = new BeepComboBox
            {
                Theme = Theme,
                IsChild = true,
                Width = 120,
                Height = 20
            };
            filterColumnComboBox.SelectedItemChanged += FilterColumnComboBox_SelectedIndexChanged;
            Controls.Add(filterColumnComboBox);
        }

        private void InitializeNavigationControls()
        {
            // Initialize navigation controls if using built-in navigator
            // This will be implemented in the navigation section
        }

        private void SetupEventHandlers()
        {
            // Mouse events
            MouseDown += BeepGrid_MouseDown;
            MouseMove += BeepGrid_MouseMove;
            MouseUp += BeepGrid_MouseUp;
            MouseClick += BeepGrid_MouseClick;
            MouseWheel += BeepGrid_MouseWheel;

            // Keyboard events
            KeyDown += BeepGrid_KeyDown;
            PreviewKeyDown += BeepGrid_PreviewKeyDown;

            // Layout events
            Resize += BeepGrid_Resize;

            // Handle creation event
            HandleCreated += BeepGrid_HandleCreated;
        }

        // Handle creation event for deferred initialization
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (_isInitializing)
            {
                try
                {
                    // Initialize core collections to prevent null reference exceptions
                    if (_columns == null) _columns = new List<BeepColumnConfig>();
                    if (_fullData == null) _fullData = new List<object>();
                    if (Rows == null) Rows = new BindingList<BeepRowConfig>();
                    if (originalList == null) originalList = new List<object>();
                    if (deletedList == null) deletedList = new List<object>();
                    if (Trackings == null) Trackings = new List<Tracking>();
                    if (ChangedValues == null) ChangedValues = new Dictionary<object, Dictionary<string, object>>();

                    // Add default columns
                    EnsureDefaultColumns();

                    // Initialize with empty data when dragged from toolbox
                    InitializeRows();
                    UpdateScrollBars();
                    Invalidate();

                    _isInitializing = false; // Initialization complete
                }
                catch (Exception ex)
                {
                    //MiscFunctions.SendLog($"Error during initialization: {ex.Message}");
                    _isInitializing = false;
                    Invalidate();
                }
            }
        }

        private void BeepGrid_HandleCreated(object sender, EventArgs e)
        {
            // Additional setup when handle is created
            if (!DesignMode)
            {
                Focus();
            }
        }

        #endregion

        #region Data Management and Initialization

        private void EnsureDefaultColumns()
        {
            if (_columns == null || _columns.Count == 0)
            {
                _columns = new List<BeepColumnConfig>();

                // Add a basic column for design-time
                var defaultColumn = new BeepColumnConfig
                {
                    ColumnName = "Column1",
                    ColumnCaption = "Column 1",
                    Width = 100,
                    Index = 0,
                    Visible = true,
                    CellEditor = BeepColumnType.Text,
                    PropertyTypeName = typeof(string).AssemblyQualifiedName
                };

                _columns.Add(defaultColumn);
                columnssetupusingeditordontchange = false;
            }
        }

        private void EnsureSelectionColumn()
        {
            if (_showCheckboxes)
            {
                // Check if selection column already exists
                var existingSelColumn = _columns.FirstOrDefault(c => c.IsSelectionCheckBox);
                if (existingSelColumn == null)
                {
                    // Create selection column
                    var selectionColumn = new BeepColumnConfig
                    {
                        ColumnName = "_Selection",
                        ColumnCaption = "☑",
                        Width = _selectionColumnWidth,
                        IsSelectionCheckBox = true,
                        CellEditor = BeepColumnType.CheckBoxBool,
                        ReadOnly = false,
                        Visible = true,
                        Index = 0,
                        Sticked = true, // Keep visible during horizontal scroll
                        Resizable = DataGridViewTriState.False,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };

                    // Insert at the beginning
                    _columns.Insert(0, selectionColumn);

                    // Update indices of other columns
                    for (int i = 1; i < _columns.Count; i++)
                    {
                        _columns[i].Index = i;
                    }

                    _hasSelectionColumn = true;
                }

                // Show select all checkbox
                if (_selectAllCheckBox != null)
                {
                    _selectAllCheckBox.Visible = true;
                }
            }
            else
            {
                // Remove selection column if it exists
                var selectionColumn = _columns.FirstOrDefault(c => c.IsSelectionCheckBox);
                if (selectionColumn != null)
                {
                    _columns.Remove(selectionColumn);

                    // Update indices
                    for (int i = 0; i < _columns.Count; i++)
                    {
                        _columns[i].Index = i;
                    }

                    _hasSelectionColumn = false;
                }

                // Hide select all checkbox
                if (_selectAllCheckBox != null)
                {
                    _selectAllCheckBox.Visible = false;
                }

                // Clear selection data
                _selectedRows.Clear();
                _selectedDataItems.Clear();
            }
        }

        private void InvalidateLayout()
        {
            _layoutDirty = true;
            BeginInvoke(new Action(() =>
            {
                if (_layoutDirty)
                {
                    RecalculateLayout();
                    Invalidate();
                }
            }));
        }

        private void RefreshData()
        {
            if (_isInitializing) return;

            if (_dataSource == null)
            {
                _originalData.Clear();
                _filteredData.Clear();
                _fullData.Clear();
                _virtualRowCount = 0;
                Rows.Clear();
                Invalidate();
                return;
            }

            SuspendLayout();
            try
            {
                // Set up data source
                DataSetup();

                // Initialize data based on source type
                InitializeData();

                // Wrap data into rows
                WrapFullData();

                // Initialize rows for display
                InitializeRows();

                // Apply current filters
                ApplyFilters();

                // Update virtual row count
                _virtualRowCount = _filteredData.Count;

                // Recalculate layout
                RecalculateLayout();

                // Update scrollbars
                UpdateScrollBars();
            }
            finally
            {
                ResumeLayout();
                Invalidate();
            }
        }

        private void DataSetup()
        {
            try
            {
                if (_dataSource == null) return;

                // Handle BindingSource
                if (_dataSource is BindingSource bindingSource)
                {
                    AssignBindingSource(bindingSource);
                    return;
                }

                // Create new binding source for other data types
                if (_bindingSource == null)
                {
                    _bindingSource = new BindingSource();
                    _bindingSource.ListChanged += BindingSource_ListChanged;
                }

                _bindingSource.DataSource = _dataSource;
                finalData = _dataSource;
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error in DataSetup: {ex.Message}");
            }
        }

        private void InitializeData()
        {
            try
            {
                _fullData.Clear();

                if (_dataSource == null) return;

                // Handle different data source types
                if (_dataSource is DataTable dataTable)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        _fullData.Add(row);
                    }
                    _entityType = typeof(DataRow);
                }
                else if (_dataSource is IEnumerable enumerable && !(_dataSource is string))
                {
                    foreach (var item in enumerable)
                    {
                        _fullData.Add(item);
                        if (_entityType == null && item != null)
                        {
                            _entityType = item.GetType();
                        }
                    }
                }
                else
                {
                    _fullData.Add(_dataSource);
                    _entityType = _dataSource.GetType();
                }

                // Auto-generate columns if none exist and not set manually
                if ((_columns == null || _columns.Count == 0 || !columnssetupusingeditordontchange) && _entityType != null)
                {
                    CreateColumnsFromType(_entityType);
                }

                originalList = new List<object>(_fullData);
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error in InitializeData: {ex.Message}");
            }
        }

        private void WrapFullData()
        {
            try
            {
                if (_fullData == null || _fullData.Count == 0) return;

                // Wrap data items into BeepRowConfig objects
                for (int i = 0; i < _fullData.Count; i++)
                {
                    var dataItem = _fullData[i];

                    // Check if we already have a row for this data item
                    var existingRow = Rows.FirstOrDefault(r => r.RowData == dataItem);
                    if (existingRow == null)
                    {
                        var row = new BeepRowConfig
                        {
                            RowIndex = i,
                            RowData = dataItem,
                            EntityStructure = _entityStructure
                        };

                        // Create cells for each column
                        CreateCellsForRow(row, dataItem);

                        Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error in WrapFullData: {ex.Message}");
            }
        }

        private void CreateCellsForRow(BeepRowConfig row, object dataItem)
        {
            if (_columns == null) return;

            row.Cells.Clear();

            for (int colIndex = 0; colIndex < _columns.Count; colIndex++)
            {
                var column = _columns[colIndex];

                var cell = new BeepCellConfig
                {
                    RowIndex = row.RowIndex,
                    ColumnIndex = colIndex,
                    ColumnName = column.ColumnName,
                    CellEditor = column.CellEditor
                };

                // Set cell value
                if (column.IsSelectionCheckBox)
                {
                    cell.CellValue = false; // Default unchecked
                }
                else
                {
                    cell.CellValue = GetCellValue(dataItem, column);
                }

                row.Cells.Add(cell);
            }
        }

        private void InitializeRows()
        {
            try
            {
                if (_isInitializing || _fullData == null) return;

                // Update row indices and ensure all rows have proper cell structure
                for (int i = 0; i < Rows.Count; i++)
                {
                    var row = Rows[i];
                    row.RowIndex = i;

                    // Ensure cells exist for all columns
                    if (row.Cells.Count != _columns.Count)
                    {
                        CreateCellsForRow(row, row.RowData);
                    }
                }

                // Update filtered data
                _filteredData = _fullData.ToList();
                _originalData = _fullData.ToList();

                // Recalculate virtual rows
                RecalculateVirtualRows();
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error in InitializeRows: {ex.Message}");
            }
        }

        private void CreateColumnsFromType(Type entityType)
        {
            try
            {
                if (entityType == null) return;

                _columns.Clear();

                if (entityType == typeof(DataRow))
                {
                    // Handle DataRow - get columns from DataTable
                    if (_dataSource is DataTable dataTable)
                    {
                        CreateColumnsFromDataTable(dataTable);
                    }
                }
                else
                {
                    // Handle regular objects
                    var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                               .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

                    int index = 0;
                    foreach (var prop in properties)
                    {
                        var column = new BeepColumnConfig
                        {
                            ColumnName = prop.Name,
                            ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(prop.Name),
                            PropertyTypeName = prop.PropertyType.AssemblyQualifiedName,
                            Width = 120,
                            Index = index++,
                            Visible = true,
                            CellEditor = MapPropertyTypeToCellEditor(prop.PropertyType)
                        };

                        _columns.Add(column);
                    }
                }

                // Ensure selection column if checkboxes are enabled
                if (_showCheckboxes)
                {
                    EnsureSelectionColumn();
                }

                _layoutDirty = true;
                columnssetupusingeditordontchange = true;
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error in CreateColumnsFromType: {ex.Message}");
            }
        }

        private void CreateColumnsFromDataTable(DataTable dataTable)
        {
            int index = 0;
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                var column = new BeepColumnConfig
                {
                    ColumnName = dataColumn.ColumnName,
                    ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(dataColumn.ColumnName),
                    PropertyTypeName = dataColumn.DataType.AssemblyQualifiedName,
                    Width = 120,
                    Index = index++,
                    Visible = true,
                    CellEditor = MapPropertyTypeToCellEditor(dataColumn.DataType)
                };

                _columns.Add(column);
            }
        }

        private BeepColumnType MapPropertyTypeToCellEditor(Type type)
        {
            if (type == typeof(bool) || type == typeof(bool?))
                return BeepColumnType.CheckBoxBool;
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return BeepColumnType.DateTime;
            if (type == typeof(int) || type == typeof(long) || type == typeof(float) ||
                type == typeof(double) || type == typeof(decimal) ||
                type == typeof(int?) || type == typeof(long?) || type == typeof(float?) ||
                type == typeof(double?) || type == typeof(decimal?))
                return BeepColumnType.NumericUpDown;
            if (type.IsEnum)
                return BeepColumnType.ComboBox;
            if (type == typeof(byte[]))
                return BeepColumnType.Image;

            return BeepColumnType.Text;
        }

        private object GetCellValue(object dataItem, BeepColumnConfig column)
        {
            try
            {
                if (dataItem == null || column == null) return null;

                if (dataItem is DataRow dataRow)
                {
                    return dataRow[column.ColumnName];
                }
                else
                {
                    var property = dataItem.GetType().GetProperty(column.ColumnName);
                    return property?.GetValue(dataItem);
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Layout and Virtual Scrolling

        private void RecalculateLayout()
        {
            if (Width <= 0 || Height <= 0) return;

            var clientBounds = ClientRectangle;
            int yOffset = 0;

            // Header panel bounds
            if (_headerPanel != null && _headerPanel.Visible)
            {
                headerPanelRect = new Rectangle(0, yOffset, clientBounds.Width, headerPanelHeight);
                yOffset += headerPanelHeight + _panelSpacing;
            }

            // Filter panel bounds
            if (_filterPanelControl != null && _filterPanelControl.Visible)
            {
                filterPanelRect = new Rectangle(0, yOffset, clientBounds.Width, filterPanelHeight);
                yOffset += filterPanelHeight + _panelSpacing;
            }

            // Column headers bounds
            if (_showColumnHeaders)
            {
                columnsheaderPanelRect = new Rectangle(0, yOffset, clientBounds.Width, _defaultcolumnheaderheight);
                yOffset += _defaultcolumnheaderheight;
            }

            // Calculate available space for navigation and footer
            int bottomSpace = 0;
            if (_showNavigator)
            {
                bottomSpace += navigatorPanelHeight + _panelSpacing;
            }
            if (_footerPanel != null && _footerPanel.Visible)
            {
                bottomSpace += footerPanelHeight + _panelSpacing;
            }

            // Scrollbar space
            int vScrollWidth = _vScrollBar != null && _vScrollBar.Visible ? _vScrollBar.Width : 0;
            int hScrollHeight = _hScrollBar != null && _hScrollBar.Visible ? _hScrollBar.Height : 0;

            // Main grid bounds
            _gridBounds = new Rectangle(
                0,
                yOffset,
                clientBounds.Width - vScrollWidth,
                clientBounds.Height - yOffset - bottomSpace - hScrollHeight
            );

            // Navigation panel bounds
            if (_showNavigator)
            {
                navigatorPanelRect = new Rectangle(
                    0,
                    _gridBounds.Bottom,
                    clientBounds.Width,
                    navigatorPanelHeight
                );
            }

            // Footer panel bounds
            if (_footerPanel != null && _footerPanel.Visible)
            {
                footerPanelRect = new Rectangle(
                    0,
                    clientBounds.Height - footerPanelHeight - hScrollHeight,
                    clientBounds.Width,
                    footerPanelHeight
                );
            }

            // Scrollable content bounds
            _scrollableBounds = _gridBounds;

            RecalculateVirtualRows();
            PositionScrollBars();
            _layoutDirty = false;
        }

        private void RecalculateVirtualRows()
        {
            if (_scrollableBounds.Height <= 0 || _virtualRowHeight <= 0) return;

            int visibleRowCount = _scrollableBounds.Height / _virtualRowHeight;

            _visibleRowStart = Math.Max(0, _verticalOffset / _virtualRowHeight);
            _visibleRowEnd = Math.Min(_virtualRowCount, _visibleRowStart + visibleRowCount + 1);

            // Update start and end view rows for compatibility
            _startviewrow = _visibleRowStart;
            _endviewrow = _visibleRowEnd;

            // Cache visible cell bounds for performance
            CacheVisibleCellBounds();
        }

        private void CacheVisibleCellBounds()
        {
            _cachedCellBounds.Clear();
            _cellBounds.Clear();

            if (_columns == null || _columns.Count == 0) return;

            int xOffset = -_horizontalOffset;

            for (int rowIndex = _visibleRowStart; rowIndex < _visibleRowEnd; rowIndex++)
            {
                int yPos = _scrollableBounds.Y + (rowIndex - _visibleRowStart) * _virtualRowHeight;
                int currentX = _scrollableBounds.X + xOffset;

                for (int colIndex = 0; colIndex < _columns.Count; colIndex++)
                {
                    if (!_columns[colIndex].Visible) continue;

                    var cellBounds = new Rectangle(
                        currentX,
                        yPos,
                        _columns[colIndex].Width,
                        _virtualRowHeight
                    );

                    int key = rowIndex * 1000 + colIndex; // Pack row and column into key
                    _cachedCellBounds[key] = cellBounds;
                    _cellBounds[key] = cellBounds;

                    currentX += _columns[colIndex].Width;
                }
            }
        }

        #endregion

        #region Event Handlers

        #region Mouse Event Handlers

        private void BeepGrid_MouseDown(object sender, MouseEventArgs e)
        {
            Focus();

            if (e.Button == MouseButtons.Left)
            {
                var hitInfo = HitTest(e.Location);

                // Handle different hit areas
                switch (hitInfo.Type)
                {
                    case HitTestType.Cell:
                        HandleCellMouseDown(hitInfo, e);
                        break;
                    case HitTestType.ColumnHeader:
                        HandleColumnHeaderMouseDown(hitInfo, e);
                        break;
                    case HitTestType.NavigationButton:
                        HandleNavigationButtonClick(hitInfo.Name);
                        break;
                    case HitTestType.SelectionCheckbox:
                        HandleSelectionCheckboxClick(hitInfo, e);
                        break;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ShowContextMenu(e.Location);
            }
        }

        private void HandleNavigationButtonClick(object name)
        {
            throw new NotImplementedException();
        }

        private void BeepGrid_MouseMove(object sender, MouseEventArgs e)
        {
            var hitInfo = HitTest(e.Location);

            // Update hover states
            UpdateHoverStates(hitInfo);

            // Handle column/row resizing
            if (_resizingColumn || _resizingRow)
            {
                HandleResizeMove(e.Location);
                return;
            }

            // Check for resize cursors
            CheckResizeCursors(e.Location);

            // Update tooltip if needed
            UpdateTooltip(e.Location, hitInfo);
        }

        private void BeepGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (_resizingColumn || _resizingRow)
            {
                EndResize();
            }

            Cursor = Cursors.Default;
        }

        private void BeepGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var hitInfo = HitTest(e.Location);
                OnCellClick(hitInfo, e);
            }
        }

        private void BeepGrid_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_vScrollBar != null && _vScrollBar.Visible)
            {
                int scrollAmount = e.Delta / 120 * _virtualRowHeight * 3;
                int newValue = _vScrollBar.Value - scrollAmount;
                newValue = Math.Max(0, Math.Min(newValue, _vScrollBar.Maximum));

                if (_vScrollBar.Value != newValue)
                {
                    _vScrollBar.Value = newValue;
                    VScrollBar_Scroll(_vScrollBar, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Keyboard Event Handlers

        private void BeepGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsEditorShown && _editingControl != null)
            {
                // Let the editor handle the key if it's editing
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Up:
                    MoveSelection(0, -1);
                    e.Handled = true;
                    break;
                case Keys.Down:
                    MoveSelection(0, 1);
                    e.Handled = true;
                    break;
                case Keys.Left:
                    MoveSelection(-1, 0);
                    e.Handled = true;
                    break;
                case Keys.Right:
                    MoveSelection(1, 0);
                    e.Handled = true;
                    break;
                case Keys.Home:
                    if (e.Control)
                        MoveToCell(0, 0);
                    else
                        MoveToCell(_selectedRowIndex, 0);
                    e.Handled = true;
                    break;
                case Keys.End:
                    if (e.Control)
                        MoveToCell(_virtualRowCount - 1, _columns.Count - 1);
                    else
                        MoveToCell(_selectedRowIndex, GetLastVisibleColumn());
                    e.Handled = true;
                    break;
                case Keys.PageUp:
                    PageUp();
                    e.Handled = true;
                    break;
                case Keys.PageDown:
                    PageDown();
                    e.Handled = true;
                    break;
                case Keys.Enter:
                case Keys.F2:
                    BeginEdit();
                    e.Handled = true;
                    break;
                case Keys.Escape:
                    CancelEdit();
                    e.Handled = true;
                    break;
                case Keys.Delete:
                    if (e.Control)
                    {
                        DeleteSelectedRows();
                    }
                    else
                    {
                        ClearSelectedCells();
                    }
                    e.Handled = true;
                    break;
                case Keys.Space:
                    if (_showCheckboxes && _selectedRowIndex >= 0)
                    {
                        ToggleRowSelection(_selectedRowIndex);
                        e.Handled = true;
                    }
                    break;
                case Keys.A:
                    if (e.Control && _showCheckboxes)
                    {
                        SelectAllRows();
                        e.Handled = true;
                    }
                    break;
                case Keys.C:
                    if (e.Control)
                    {
                        CopySelectedCells();
                        e.Handled = true;
                    }
                    break;
                case Keys.V:
                    if (e.Control)
                    {
                        PasteToSelectedCells();
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void BeepGrid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // Mark navigation keys as input keys so they get passed to KeyDown
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                case Keys.Tab:
                    e.IsInputKey = true;
                    break;
            }
        }

        #endregion

        #region Scrollbar Event Handlers

        private void VScrollBar_Scroll(object sender, EventArgs e)
        {
            _verticalOffset = _vScrollBar.Value;
            RecalculateVirtualRows();
            InvalidateDataArea();
        }

        private void HScrollBar_Scroll(object sender, EventArgs e)
        {
            _horizontalOffset = _hScrollBar.Value;
            _xOffset = _horizontalOffset; // Update for compatibility
            CacheVisibleCellBounds();
            UpdateFilterControlPositions();
            InvalidateDataArea();
        }

        #endregion

        #region Timer Event Handlers

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            // Smooth scrolling animation
            bool targetReached = true;

            if (_scrollTargetVertical >= 0)
            {
                int diff = _scrollTargetVertical - _verticalOffset;
                if (Math.Abs(diff) > 1)
                {
                    int step = Math.Max(1, Math.Abs(diff) / 5);
                    _verticalOffset += diff > 0 ? step : -step;
                    _vScrollBar.Value = Math.Max(0, Math.Min(_verticalOffset, _vScrollBar.Maximum));
                    targetReached = false;
                }
                else
                {
                    _verticalOffset = _scrollTargetVertical;
                    _vScrollBar.Value = Math.Max(0, Math.Min(_verticalOffset, _vScrollBar.Maximum));
                    _scrollTargetVertical = -1;
                }
            }

            if (_scrollTargetHorizontal >= 0)
            {
                int diff = _scrollTargetHorizontal - _horizontalOffset;
                if (Math.Abs(diff) > 1)
                {
                    int step = Math.Max(1, Math.Abs(diff) / 5);
                    _horizontalOffset += diff > 0 ? step : -step;
                    _hScrollBar.Value = Math.Max(0, Math.Min(_horizontalOffset, _hScrollBar.Maximum));
                    targetReached = false;
                }
                else
                {
                    _horizontalOffset = _scrollTargetHorizontal;
                    _hScrollBar.Value = Math.Max(0, Math.Min(_horizontalOffset, _hScrollBar.Maximum));
                    _scrollTargetHorizontal = -1;
                }
            }

            if (targetReached)
            {
                _scrollTimer.Stop();
            }

            RecalculateVirtualRows();
            Invalidate();
        }

        private void ResizeTimer_Tick(object sender, EventArgs e)
        {
            _resizeTimer.Stop();
            _isResizing = false;

            if (_pendingSize != _lastCalculatedSize)
            {
                _lastCalculatedSize = _pendingSize;
                PerformResize();
            }
        }

        #endregion

        #region Selection Event Handlers

        private void SelectAllCheckBox_StateChanged(object sender, EventArgs e)
        {
            if (_selectAllCheckBox == null) return;

            bool selectAll = _selectAllCheckBox.CurrentValue;

            // Clear current selections
            _selectedRows.Clear();
            _selectedDataItems.Clear();
            SelectedRows.Clear();
            SelectedGridRows.Clear();

            if (selectAll)
            {
                // Select all visible rows
                for (int i = 0; i < _filteredData.Count; i++)
                {
                    _selectedRows[i] = true;
                    _selectedDataItems.Add(_filteredData[i]);
                    SelectedRows.Add(i);

                    if (i < Rows.Count)
                    {
                        SelectedGridRows.Add(Rows[i]);
                    }
                }
            }

            // Update persistent selections
            _persistentSelectedRows.Clear();
            foreach (var kvp in _selectedRows)
            {
                _persistentSelectedRows[kvp.Key] = kvp.Value;
            }

            // Raise selection changed event
            OnSelectedRowsChanged();

            // Refresh display
            InvalidateDataArea();
        }

        #endregion

        #region Filter Event Handlers

        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            if (filterTextBox == null) return;

            string filterText = filterTextBox.Text?.Trim() ?? "";
            string selectedColumn = GetSelectedFilterColumn();

            ApplyFilter(filterText, selectedColumn);
        }

        private void FilterColumnComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (filterColumnComboBox?.SelectedItem is SimpleItem selectedItem)
            {
                string filterText = filterTextBox?.Text?.Trim() ?? "";
                if (!string.IsNullOrEmpty(filterText))
                {
                    ApplyFilter(filterText, selectedItem.Value?.ToString());
                }
            }
        }

        #endregion

        #region Layout Event Handlers

        private void BeepGrid_Resize(object sender, EventArgs e)
        {
            if (_isResizing) return;

            _isResizing = true;
            _pendingSize = Size;

            _resizeTimer.Stop();
            _resizeTimer.Start();
        }

        private void Rows_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    HandleRowAdded(e.NewIndex);
                    break;
                case ListChangedType.ItemDeleted:
                    HandleRowDeleted(e.NewIndex);
                    break;
                case ListChangedType.ItemChanged:
                    HandleRowChanged(e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    HandleRowsReset();
                    break;
                case ListChangedType.ItemMoved:
                    HandleRowMoved(e.OldIndex, e.NewIndex);
                    break;
            }
        }

        #endregion

        #region Binding Event Handlers

        private void BindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            try
            {
                switch (e.ListChangedType)
                {
                    case ListChangedType.ItemAdded:
                        HandleItemAdded(e.NewIndex);
                        break;
                    case ListChangedType.ItemChanged:
                        HandleItemChanged(e.NewIndex);
                        break;
                    case ListChangedType.ItemDeleted:
                        HandleItemDeleted(e.NewIndex);
                        break;
                    case ListChangedType.ItemMoved:
                        HandleItemMoved(e.OldIndex, e.NewIndex);
                        break;
                    case ListChangedType.Reset:
                        RefreshData();
                        break;
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error in BindingSource_ListChanged: {ex.Message}");
            }
        }

        private void AssignBindingSource(BindingSource bindingSource)
        {
            try
            {
                if (_bindingSource != null)
                {
                    _bindingSource.ListChanged -= BindingSource_ListChanged;
                }

                _bindingSource = bindingSource;

                if (_bindingSource != null)
                {
                    _bindingSource.ListChanged += BindingSource_ListChanged;
                    finalData = _bindingSource.DataSource;
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error in AssignBindingSource: {ex.Message}");
            }
        }

        #endregion

        #region Navigation Event Handlers

        private void AttachNavigatorEvents()
        {
            if (_dataNavigator == null) return;

            _dataNavigator.PositionChanged += DataNavigator_PositionChanged;
            _dataNavigator.CurrentChanged += DataNavigator_CurrentChanged;
            _dataNavigator.CallPrinter += DataNavigator_CallPrinter;
            _dataNavigator.SendMessage += DataNavigator_SendMessage;
            _dataNavigator.ShowSearch += DataNavigator_ShowSearch;
            _dataNavigator.DeleteCalled += DataNavigator_DeleteCalled;
            _dataNavigator.NewRecordCreated += DataNavigator_NewRecordCreated;
            _dataNavigator.EditCalled += DataNavigator_EditCalled;
            _dataNavigator.SaveCalled += DataNavigator_SaveCalled;
        }

        private void DetachNavigatorEvents()
        {
            if (_dataNavigator == null) return;

            _dataNavigator.PositionChanged -= DataNavigator_PositionChanged;
            _dataNavigator.CurrentChanged -= DataNavigator_CurrentChanged;
            _dataNavigator.CallPrinter -= DataNavigator_CallPrinter;
            _dataNavigator.SendMessage -= DataNavigator_SendMessage;
            _dataNavigator.ShowSearch -= DataNavigator_ShowSearch;
            _dataNavigator.DeleteCalled -= DataNavigator_DeleteCalled;
            _dataNavigator.NewRecordCreated -= DataNavigator_NewRecordCreated;
            _dataNavigator.EditCalled -= DataNavigator_EditCalled;
            _dataNavigator.SaveCalled -= DataNavigator_SaveCalled;
        }

        private void DataNavigator_PositionChanged(object sender, EventArgs e)
        {
            SyncWithNavigatorPosition();
        }

        private void DataNavigator_CurrentChanged(object sender, EventArgs e)
        {
            SyncWithNavigatorPosition();
        }

        private void SyncWithNavigatorPosition()
        {
            if (_dataNavigator?.BindingSource != null)
            {
                int position = _dataNavigator.BindingSource.Position;
                if (position >= 0 && position < _filteredData.Count)
                {
                    MoveToCell(position, _selectedColumnIndex >= 0 ? _selectedColumnIndex : 0);
                }
            }
        }

        private void DataNavigator_CallPrinter(object sender, BindingSource bs)
        {
            CallPrinter?.Invoke(this, EventArgs.Empty);
        }

        private void DataNavigator_SendMessage(object sender, BindingSource bs)
        {
            SendMessage?.Invoke(this, EventArgs.Empty);
        }

        private void DataNavigator_ShowSearch(object sender, BindingSource bs)
        {
            ShowSearch?.Invoke(this, EventArgs.Empty);
        }

        private void DataNavigator_DeleteCalled(object sender, BindingSource bs)
        {
            DeleteCalled?.Invoke(this, EventArgs.Empty);
        }

        private void DataNavigator_NewRecordCreated(object sender, BindingSource bs)
        {
            NewRecordCreated?.Invoke(this, EventArgs.Empty);
        }

        private void DataNavigator_EditCalled(object sender, BindingSource bs)
        {
            EditCalled?.Invoke(this, EventArgs.Empty);
        }

        private void DataNavigator_SaveCalled(object sender, BindingSource bs)
        {
            SaveCalled?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #endregion
        #region Helper Methods and Utilities

        #region Hit Testing

        private GridHitTestInfo HitTest(Point location)
        {
            // Test column headers
            if (_showColumnHeaders && columnsheaderPanelRect.Contains(location))
            {
                return HitTestColumnHeader(location);
            }

            // Test filter panel
            if (_showFilterpanel && filterPanelRect.Contains(location))
            {
                return HitTestFilterPanel(location);
            }

            // Test navigation panel
            if (_showNavigator && navigatorPanelRect.Contains(location))
            {
                return HitTestNavigationPanel(location);
            }

            // Test data cells
            if (_gridBounds.Contains(location))
            {
                return HitTestDataCell(location);
            }

            return new GridHitTestInfo(HitTestType.None, -1, -1);
        }

        private GridHitTestInfo HitTestColumnHeader(Point location)
        {
            int xOffset = -_horizontalOffset;
            for (int i = 0; i < _columns.Count; i++)
            {
                if (!_columns[i].Visible) continue;

                var headerRect = new Rectangle(
                    columnsheaderPanelRect.X + xOffset,
                    columnsheaderPanelRect.Y,
                    _columns[i].Width,
                    columnsheaderPanelRect.Height
                );

                if (headerRect.Contains(location))
                {
                    return new GridHitTestInfo(HitTestType.ColumnHeader, -1, i);
                }

                xOffset += _columns[i].Width;
            }

            return new GridHitTestInfo(HitTestType.None, -1, -1);
        }

        private GridHitTestInfo HitTestDataCell(Point location)
        {
            if (_columns == null || _columns.Count == 0)
                return new GridHitTestInfo(HitTestType.None, -1, -1);

            // Calculate row
            int relativeY = location.Y - _gridBounds.Y;
            int rowIndex = _visibleRowStart + (relativeY / _virtualRowHeight);

            if (rowIndex < 0 || rowIndex >= _virtualRowCount)
                return new GridHitTestInfo(HitTestType.None, -1, -1);

            // Calculate column
            int xOffset = -_horizontalOffset;
            for (int i = 0; i < _columns.Count; i++)
            {
                if (!_columns[i].Visible) continue;

                var cellRect = new Rectangle(
                    _gridBounds.X + xOffset,
                    _gridBounds.Y + (rowIndex - _visibleRowStart) * _virtualRowHeight,
                    _columns[i].Width,
                    _virtualRowHeight
                );

                if (cellRect.Contains(location))
                {
                    return new GridHitTestInfo(HitTestType.Cell, rowIndex, i);
                }

                xOffset += _columns[i].Width;
            }

            return new GridHitTestInfo(HitTestType.None, -1, -1);
        }

        private GridHitTestInfo HitTestFilterPanel(Point location)
        {
            // Simple implementation - can be enhanced
            return new GridHitTestInfo(HitTestType.None, -1, -1);
        }

        private GridHitTestInfo HitTestNavigationPanel(Point location)
        {
            // Simple implementation - can be enhanced  
            return new GridHitTestInfo(HitTestType.None, -1, -1);
        }

        #endregion

        #region Scrollbar Management

        private void UpdateScrollBars()
        {
            UpdateVerticalScrollBar();
            UpdateHorizontalScrollBar();
            PositionScrollBars();
        }

        private void UpdateVerticalScrollBar()
        {
            if (_vScrollBar == null) return;

            bool needsVerticalScroll = _virtualRowCount * _virtualRowHeight > _gridBounds.Height;
            _vScrollBar.Visible = _showVerticalScrollBar && needsVerticalScroll;

            if (_vScrollBar.Visible)
            {
                int maxScroll = Math.Max(0, (_virtualRowCount * _virtualRowHeight) - _gridBounds.Height);
                _vScrollBar.Maximum = maxScroll;
                _vScrollBar.LargeChange = _gridBounds.Height;
                _vScrollBar.SmallChange = _virtualRowHeight;
                _vScrollBar.Value = Math.Min(_verticalOffset, maxScroll);
            }
        }

        private void UpdateHorizontalScrollBar()
        {
            if (_hScrollBar == null) return;

            int totalWidth = _columns?.Where(c => c.Visible).Sum(c => c.Width) ?? 0;
            bool needsHorizontalScroll = totalWidth > _gridBounds.Width;
            _hScrollBar.Visible = _showHorizontalScrollBar && needsHorizontalScroll;

            if (_hScrollBar.Visible)
            {
                int maxScroll = Math.Max(0, totalWidth - _gridBounds.Width);
                _hScrollBar.Maximum = maxScroll;
                _hScrollBar.LargeChange = _gridBounds.Width;
                _hScrollBar.SmallChange = 20;
                _hScrollBar.Value = Math.Min(_horizontalOffset, maxScroll);
            }
        }

        private void PositionScrollBars()
        {
            if (_vScrollBar != null && _vScrollBar.Visible)
            {
                _vScrollBar.SetBounds(
                    Width - _vScrollBar.Width,
                    _gridBounds.Y,
                    _vScrollBar.Width,
                    _gridBounds.Height
                );
            }

            if (_hScrollBar != null && _hScrollBar.Visible)
            {
                int width = Width - (_vScrollBar?.Visible == true ? _vScrollBar.Width : 0);
                _hScrollBar.SetBounds(
                    0,
                    Height - _hScrollBar.Height,
                    width,
                    _hScrollBar.Height
                );
            }
        }

        #endregion

        #region Filter Management

        private void ToggleFilterPanel()
        {
            if (_filterPanelControl != null)
            {
                _filterPanelControl.Visible = _showFilterpanel;
                SetupFilterColumnComboBox();
                _layoutDirty = true;
                InvalidateLayout();
            }
        }

        private void SetupFilterColumnComboBox()
        {
            if (filterColumnComboBox == null || _columns == null) return;

            filterColumnComboBox.Items.Clear();

            foreach (var column in _columns.Where(c => c.Visible && !c.IsSelectionCheckBox))
            {
                var item = new SimpleItem
                {
                    Text = column.ColumnCaption,
                    Value = column.ColumnName,
                    Name = column.ColumnName
                };
                filterColumnComboBox.Items.Add(item);
            }

            if (filterColumnComboBox.Items.Count > 0)
            {
                filterColumnComboBox.SelectedIndex = 0;
            }
        }

        private string GetSelectedFilterColumn()
        {
            if (filterColumnComboBox?.SelectedItem is SimpleItem item)
            {
                return item.Value?.ToString();
            }
            return _columns?.FirstOrDefault(c => c.Visible && !c.IsSelectionCheckBox)?.ColumnName ?? "";
        }

        private void ApplyFilter(string filterText, string columnName)
        {
            try
            {
                if (string.IsNullOrEmpty(filterText))
                {
                    _filteredData = _fullData.ToList();
                }
                else
                {
                    _filteredData = _fullData.Where(item =>
                        MatchesFilter(item, filterText, columnName)).ToList();
                }

                _virtualRowCount = _filteredData.Count;
                RecalculateVirtualRows();
                UpdateScrollBars();
                OnFilterChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error applying filter: {ex.Message}");
            }
        }

        private bool MatchesFilter(object item, string filterText, string columnName)
        {
            try
            {
                if (item == null) return false;

                var column = _columns.FirstOrDefault(c => c.ColumnName == columnName);
                if (column == null) return false;

                var value = GetCellValue(item, column);
                return value?.ToString()?.Contains(filterText, StringComparison.OrdinalIgnoreCase) == true;
            }
            catch
            {
                return false;
            }
        }

        private void ApplyFilters()
        {
            // Apply all active filters
            string filterText = filterTextBox?.Text?.Trim() ?? "";
            string columnName = GetSelectedFilterColumn();

            if (!string.IsNullOrEmpty(filterText) && !string.IsNullOrEmpty(columnName))
            {
                ApplyFilter(filterText, columnName);
            }
            else
            {
                _filteredData = _fullData.ToList();
                _virtualRowCount = _filteredData.Count;
                RecalculateVirtualRows();
                UpdateScrollBars();
                Invalidate();
            }
        }

        #endregion

        #region Column Generation

        private void GenerateColumnsFromEntity()
        {
            if (_entityStructure?.Fields == null) return;

            try
            {
                _columns.Clear();

                for (int i = 0; i < _entityStructure.Fields.Count; i++)
                {
                    var field = _entityStructure.Fields[i];

                    var column = new BeepColumnConfig
                    {
                        ColumnName = field.fieldname,
                        ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(field.fieldname),
                        Width = 120,
                        Index = i,
                        Visible = true,
                        CellEditor = MapFieldCategoryToCellEditor(field.fieldCategory),
                        PropertyTypeName = GetPropertyTypeName(field.fieldCategory)
                    };

                    _columns.Add(column);
                }

                // Ensure selection column if checkboxes are enabled
                if (_showCheckboxes)
                {
                    EnsureSelectionColumn();
                }

                _layoutDirty = true;
                columnssetupusingeditordontchange = true;
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error generating columns from entity: {ex.Message}");
            }
        }

        private BeepColumnType MapFieldCategoryToCellEditor(DbFieldCategory category)
        {
            return category switch
            {
                DbFieldCategory.Boolean => BeepColumnType.CheckBoxBool,
                DbFieldCategory.Date => BeepColumnType.DateTime,
                DbFieldCategory.Numeric or DbFieldCategory.Currency => BeepColumnType.NumericUpDown,
                DbFieldCategory.Binary => BeepColumnType.Image,
                _ => BeepColumnType.Text
            };
        }

        private string GetPropertyTypeName(DbFieldCategory category)
        {
            return category switch
            {
                DbFieldCategory.Boolean => typeof(bool).AssemblyQualifiedName,
                DbFieldCategory.Date => typeof(DateTime).AssemblyQualifiedName,
                DbFieldCategory.Numeric => typeof(decimal).AssemblyQualifiedName,
                DbFieldCategory.Currency => typeof(decimal).AssemblyQualifiedName,
                DbFieldCategory.Binary => typeof(byte[]).AssemblyQualifiedName,
                _ => typeof(string).AssemblyQualifiedName
            };
        }

        #endregion

        #region Navigation and Selection

        private void MoveSelection(int columnDelta, int rowDelta)
        {
            int newRow = Math.Max(0, Math.Min(_selectedRowIndex + rowDelta, _virtualRowCount - 1));
            int newCol = Math.Max(0, Math.Min(_selectedColumnIndex + columnDelta, _columns.Count - 1));

            MoveToCell(newRow, newCol);
        }

        private void MoveToCell(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= _virtualRowCount) return;
            if (columnIndex < 0 || columnIndex >= _columns.Count) return;

            _selectedRowIndex = rowIndex;
            _selectedColumnIndex = columnIndex;

            // Ensure cell is visible
            EnsureCellVisible(rowIndex, columnIndex);

            // Update selected cell
            UpdateSelectedCell();

            // Raise events
            OnCurrentCellChanged();
            OnCurrentRowChanged();

            Invalidate();
        }

        private void EnsureCellVisible(int rowIndex, int columnIndex)
        {
            // Ensure row is visible
            if (rowIndex < _visibleRowStart)
            {
                _verticalOffset = rowIndex * _virtualRowHeight;
                if (_vScrollBar != null) _vScrollBar.Value = _verticalOffset;
                RecalculateVirtualRows();
            }
            else if (rowIndex >= _visibleRowEnd)
            {
                _verticalOffset = (rowIndex - (_visibleRowEnd - _visibleRowStart) + 1) * _virtualRowHeight;
                if (_vScrollBar != null) _vScrollBar.Value = Math.Min(_verticalOffset, _vScrollBar.Maximum);
                RecalculateVirtualRows();
            }

            // Ensure column is visible
            EnsureColumnVisible(columnIndex);
        }

        private void EnsureColumnVisible(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= _columns.Count) return;

            int columnLeft = _columns.Take(columnIndex).Where(c => c.Visible).Sum(c => c.Width);
            int columnRight = columnLeft + _columns[columnIndex].Width;

            if (columnLeft < _horizontalOffset)
            {
                _horizontalOffset = columnLeft;
                if (_hScrollBar != null) _hScrollBar.Value = _horizontalOffset;
            }
            else if (columnRight > _horizontalOffset + _gridBounds.Width)
            {
                _horizontalOffset = columnRight - _gridBounds.Width;
                if (_hScrollBar != null) _hScrollBar.Value = Math.Min(_horizontalOffset, _hScrollBar.Maximum);
            }

            CacheVisibleCellBounds();
        }

        private int GetLastVisibleColumn()
        {
            for (int i = _columns.Count - 1; i >= 0; i--)
            {
                if (_columns[i].Visible) return i;
            }
            return 0;
        }

        private void PageUp()
        {
            int visibleRows = _gridBounds.Height / _virtualRowHeight;
            int newRow = Math.Max(0, _selectedRowIndex - visibleRows);
            MoveToCell(newRow, _selectedColumnIndex);
        }

        private void PageDown()
        {
            int visibleRows = _gridBounds.Height / _virtualRowHeight;
            int newRow = Math.Min(_virtualRowCount - 1, _selectedRowIndex + visibleRows);
            MoveToCell(newRow, _selectedColumnIndex);
        }

        #endregion

        #region Selection Management

        private void ToggleRowSelection(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _virtualRowCount) return;

            bool isSelected = _selectedRows.ContainsKey(rowIndex) && _selectedRows[rowIndex];
            _selectedRows[rowIndex] = !isSelected;

            // Update data item selection
            if (rowIndex < _filteredData.Count)
            {
                var dataItem = _filteredData[rowIndex];
                if (!isSelected)
                {
                    _selectedDataItems.Add(dataItem);
                    if (!SelectedRows.Contains(rowIndex)) SelectedRows.Add(rowIndex);
                }
                else
                {
                    _selectedDataItems.Remove(dataItem);
                    SelectedRows.Remove(rowIndex);
                }
            }

            OnSelectedRowsChanged();
            InvalidateDataArea();
        }

        private void SelectAllRows()
        {
            _selectedRows.Clear();
            _selectedDataItems.Clear();
            SelectedRows.Clear();
            SelectedGridRows.Clear();

            for (int i = 0; i < _filteredData.Count; i++)
            {
                _selectedRows[i] = true;
                _selectedDataItems.Add(_filteredData[i]);
                SelectedRows.Add(i);

                if (i < Rows.Count)
                {
                    SelectedGridRows.Add(Rows[i]);
                }
            }

            if (_selectAllCheckBox != null)
            {
                _selectAllCheckBox.CurrentValue = true;
            }

            OnSelectedRowsChanged();
            InvalidateDataArea();
        }

        #endregion

        #region Event Triggers

        protected virtual void OnCurrentCellChanged()
        {
            if (_selectedRowIndex >= 0 && _selectedColumnIndex >= 0)
            {
                var args = new BeepCellSelectedEventArgs(_selectedRowIndex, _selectedColumnIndex);
                CurrentCellChanged?.Invoke(this, args);
            }
        }

        protected virtual void OnCurrentRowChanged()
        {
            if (_selectedRowIndex >= 0 && _selectedRowIndex < Rows.Count)
            {
                _currentRow = Rows[_selectedRowIndex];
                var args = new BeepRowSelectedEventArgs(_selectedRowIndex,_currentRow);
                CurrentRowChanged?.Invoke(this, args);
            }
        }

        protected virtual void OnSelectedRowsChanged()
        {
            SelectedRowsChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFilterChanged()
        {
            FilterChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCellClick(GridHitTestInfo hitInfo, MouseEventArgs e)
        {
            if (hitInfo.Type == HitTestType.Cell)
            {
                CellClicked?.Invoke(this, hitInfo);
            }
        }

        #endregion

        #region Utility Methods

        private void UpdateSelectedCell()
        {
            if (_selectedRowIndex >= 0 && _selectedRowIndex < Rows.Count &&
                _selectedColumnIndex >= 0 && _selectedColumnIndex < _columns.Count)
            {
                var row = Rows[_selectedRowIndex];
                if (_selectedColumnIndex < row.Cells.Count)
                {
                    _selectedCell = row.Cells[_selectedColumnIndex];
                }
            }
        }

        private void InvalidateDataArea()
        {
            if (_gridBounds.Width > 0 && _gridBounds.Height > 0)
            {
                Invalidate(_gridBounds);
            }
        }

        private void UpdateFilterControlPositions()
        {
            // Position filter controls based on column positions
            if (!_showFilterpanel || _filterPanelControl == null) return;

            // Implementation for positioning filter controls
        }

        private void PerformResize()
        {
            RecalculateLayout();
            UpdateScrollBars();
            Invalidate();
        }

        #endregion

        #region Placeholder Methods (to be implemented)

        // These methods are referenced in event handlers but need implementation
        private void HandleCellMouseDown(GridHitTestInfo hitInfo, MouseEventArgs e)
        {
            MoveToCell(hitInfo.RowIndex, hitInfo.ColumnIndex);
        }

        private void HandleColumnHeaderMouseDown(GridHitTestInfo hitInfo, MouseEventArgs e)
        {
            // Handle column sorting, resizing, etc.
        }

        private void HandleNavigationButtonClick(string buttonName)
        {
            // Handle navigation button clicks
        }

        private void HandleSelectionCheckboxClick(GridHitTestInfo hitInfo, MouseEventArgs e)
        {
            ToggleRowSelection(hitInfo.RowIndex);
        }

        private void UpdateHoverStates(GridHitTestInfo hitInfo)
        {
            _hoveredRowIndex = hitInfo.RowIndex;
            _hoveredColumnIndex = hitInfo.ColumnIndex;
        }

        private void HandleResizeMove(Point location)
        {
            // Handle column/row resizing
        }

        private void EndResize()
        {
            _resizingColumn = false;
            _resizingRow = false;
        }
        private void HandleRowsReset() { RefreshData(); }

        private void CheckResizeCursors(Point location)
        {
            // Check if cursor should change for resizing
        }

        #region Real Method Implementations (from existing BeepGrid code)

        // Replace all placeholder methods with actual implementations

        public void BeginEdit()
        {
            if (_selectedCell == null || _editingControl != null)
                return;

            try
            {
                ShowCellEditor(_selectedCell, Point.Empty);
                IsEditorShown = true;
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error beginning edit: {ex.Message}");
            }
        }

        public void CancelEdit()
        {
            if (_editingControl != null)
            {
                try
                {
                    CloseCurrentEditor();
                    IsEditorShown = false;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    //MiscFunctions.SendLog($"Error canceling edit: {ex.Message}");
                }
            }
        }

        private void DeleteSelectedRows()
        {
            if (_selectedRows.Count == 0) return;

            if (VerifyDelete)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete {_selectedRows.Count} selected row(s)?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes) return;
            }

            try
            {
                var rowsToDelete = new List<int>(_selectedRows.Keys.Where(k => _selectedRows[k]));
                rowsToDelete.Sort((a, b) => b.CompareTo(a)); // Delete from highest index first

                foreach (int rowIndex in rowsToDelete)
                {
                    if (rowIndex >= 0 && rowIndex < _filteredData.Count)
                    {
                        var dataItem = _filteredData[rowIndex];

                        // Remove from data collections
                        _filteredData.RemoveAt(rowIndex);
                        _fullData.Remove(dataItem);
                        originalList.Remove(dataItem);
                        deletedList.Add(dataItem);

                        // Track deletion for logging
                        if (IsLogging)
                        {
                            var tracking = new Tracking(Guid.NewGuid(), rowIndex)
                            {
                               
                                EntityName = EntityName,
                               
                            };
                            Trackings.Add(tracking);
                        }
                    }
                }

                // Clear selections
                _selectedRows.Clear();
                _selectedDataItems.Clear();
                SelectedRows.Clear();
                SelectedGridRows.Clear();

                // Update grid
                _virtualRowCount = _filteredData.Count;
                RefreshData();
                OnSelectedRowsChanged();

                DeleteCalled?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error deleting rows: {ex.Message}");
                MessageBox.Show($"Error deleting rows: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopySelectedCells()
        {
            if (_selectedRows.Count == 0) return;

            try
            {
                var dataText = new StringBuilder();
                var headers = _columns.Where(c => c.Visible && !c.IsSelectionCheckBox)
                                     .Select(c => c.ColumnCaption)
                                     .ToArray();

                // Add headers
                dataText.AppendLine(string.Join("\t", headers));

                // Add selected row data
                foreach (var rowIndex in _selectedRows.Keys.Where(k => _selectedRows[k]).OrderBy(k => k))
                {
                    if (rowIndex >= 0 && rowIndex < _filteredData.Count)
                    {
                        var dataItem = _filteredData[rowIndex];
                        var values = new List<string>();

                        foreach (var column in _columns.Where(c => c.Visible && !c.IsSelectionCheckBox))
                        {
                            var value = GetCellValue(dataItem, column);
                            values.Add(value?.ToString() ?? "");
                        }

                        dataText.AppendLine(string.Join("\t", values));
                    }
                }

                if (dataText.Length > 0)
                {
                    Clipboard.SetText(dataText.ToString());
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error copying cells: {ex.Message}");
                MessageBox.Show($"Error copying to clipboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PasteToSelectedCells()
        {
            if (!Clipboard.ContainsText() || _selectedCell == null) return;

            try
            {
                string clipboardText = Clipboard.GetText();
                string[] lines = clipboardText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length == 0) return;

                int startRow = _selectedRowIndex;
                int startCol = _selectedColumnIndex;

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split('\t');
                    int currentRow = startRow + i;

                    if (currentRow >= _filteredData.Count) break;

                    for (int j = 0; j < values.Length; j++)
                    {
                        int currentCol = startCol + j;

                        if (currentCol >= _columns.Count) break;

                        var column = _columns[currentCol];
                        if (column.ReadOnly || column.IsSelectionCheckBox) continue;

                        // Update cell value
                        var dataItem = _filteredData[currentRow];
                        try
                        {
                            SetCellValue(dataItem, column, values[j]);
                        }
                        catch (Exception ex)
                        {
                            //MiscFunctions.SendLog($"Error pasting value to cell [{currentRow},{currentCol}]: {ex.Message}");
                        }
                    }
                }

                Invalidate();
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error pasting cells: {ex.Message}");
                MessageBox.Show($"Error pasting from clipboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetCellValue(object dataItem, BeepColumnConfig column, string value)
        {
            try
            {
                if (dataItem is DataRow dataRow)
                {
                    dataRow[column.ColumnName] = ConvertValue(value, dataRow.Table.Columns[column.ColumnName].DataType);
                }
                else
                {
                    var property = dataItem.GetType().GetProperty(column.ColumnName);
                    if (property != null && property.CanWrite)
                    {
                        var convertedValue = ConvertValue(value, property.PropertyType);
                        property.SetValue(dataItem, convertedValue);
                    }
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error setting cell value: {ex.Message}");
            }
        }

        private object ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value) && targetType.IsGenericType &&
                targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return null;
            }

            Type actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (actualType == typeof(string)) return value;
            if (actualType == typeof(int)) return int.Parse(value);
            if (actualType == typeof(double)) return double.Parse(value);
            if (actualType == typeof(decimal)) return decimal.Parse(value);
            if (actualType == typeof(DateTime)) return DateTime.Parse(value);
            if (actualType == typeof(bool)) return bool.Parse(value);

            return Convert.ChangeType(value, actualType);
        }

        private void ClearSelectedCells()
        {
            if (_selectedRows.Count == 0) return;

            try
            {
                foreach (var rowIndex in _selectedRows.Keys.Where(k => _selectedRows[k]))
                {
                    if (rowIndex >= 0 && rowIndex < _filteredData.Count)
                    {
                        var dataItem = _filteredData[rowIndex];

                        foreach (var column in _columns.Where(c => !c.ReadOnly && !c.IsSelectionCheckBox))
                        {
                            SetCellValue(dataItem, column, "");
                        }
                    }
                }

                Invalidate();
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error clearing cells: {ex.Message}");
            }
        }

        private void ShowContextMenu(Point location)
        {
            // Create a simple context menu for the grid
            var contextMenu = new ContextMenuStrip();

            // Add standard menu items
            contextMenu.Items.Add("Copy", null, (s, e) => CopySelectedCells());
            contextMenu.Items.Add("Paste", null, (s, e) => PasteToSelectedCells());
            contextMenu.Items.Add("Clear", null, (s, e) => ClearSelectedCells());
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Delete Rows", null, (s, e) => DeleteSelectedRows());
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Select All", null, (s, e) => SelectAllRows());

            // Enable/disable items based on state
            contextMenu.Items["Copy"].Enabled = _selectedRows.Count > 0;
            contextMenu.Items["Paste"].Enabled = Clipboard.ContainsText();
            contextMenu.Items["Clear"].Enabled = _selectedRows.Count > 0;
            contextMenu.Items["Delete Rows"].Enabled = _selectedRows.Count > 0;

            contextMenu.Show(this, location);
        }

        private void ApplyGridStyle()
        {
            switch (_gridStyle)
            {
                case BeepGridStyle.Modern:
                    ApplyModernStyle();
                    break;
                case BeepGridStyle.Flat:
                    ApplyFlatStyle();
                    break;
                case BeepGridStyle.Material:
                    ApplyMaterialStyle();
                    break;
                case BeepGridStyle.Dark:
                    ApplyDarkStyle();
                    break;
                default:
                    ApplyClassicStyle();
                    break;
            }
        }

        private void ApplyModernStyle()
        {
            // Modern style implementation
            _virtualRowHeight = 32;
            _defaultcolumnheaderheight = 40;
            Invalidate();
        }

        private void ApplyFlatStyle()
        {
            // Flat style implementation
            _virtualRowHeight = 28;
            _defaultcolumnheaderheight = 35;
            Invalidate();
        }

        private void ApplyMaterialStyle()
        {
            // Material style implementation
            _virtualRowHeight = 36;
            _defaultcolumnheaderheight = 45;
            Invalidate();
        }

        private void ApplyDarkStyle()
        {
            // Dark style implementation
            _virtualRowHeight = 30;
            _defaultcolumnheaderheight = 38;
            Invalidate();
        }

        private void ApplyClassicStyle()
        {
            // Classic style implementation
            _virtualRowHeight = 28;
            _defaultcolumnheaderheight = 32;
            Invalidate();
        }

        private void ShowCellEditor(BeepCellConfig cell, Point location)
        {
            if (cell == null || cell.ColumnIndex < 0 || cell.ColumnIndex >= _columns.Count) return;

            var column = _columns[cell.ColumnIndex];
            if (column.ReadOnly || column.IsSelectionCheckBox) return;

            try
            {
                // Create appropriate editor based on column type
                _editingControl = CreateCellControlForEditing(cell);
                if (_editingControl == null) return;

                // Position the editor
                var cellRect = GetCellRectangleIn(cell);
                _editingControl.SetBounds(cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                // Add to controls and focus
                Controls.Add(_editingControl);
                _editingControl.BringToFront();
                _editingControl.Focus();

                _editingCell = cell;
                IsEditorShown = true;
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error showing cell editor: {ex.Message}");
            }
        }

        private BeepControl CreateCellControlForEditing(BeepCellConfig cell)
        {
            var column = _columns[cell.ColumnIndex];

            switch (column.CellEditor)
            {
                case BeepColumnType.Text:
                    var textBox = new BeepTextBox
                    {
                        Theme = Theme,
                        IsChild = true,
                        Text = cell.CellValue?.ToString() ?? ""
                    };
                    textBox.LostFocus += (s, e) => SaveEditedValue();
                    textBox.KeyDown += (s, e) => {
                        if (e.KeyCode == Keys.Enter) SaveEditedValue();
                        if (e.KeyCode == Keys.Escape) CancelEdit();
                    };
                    return textBox;

                case BeepColumnType.CheckBoxBool:
                    var checkBox = new BeepCheckBoxBool
                    {
                        Theme = Theme,
                        IsChild = true,
                        CurrentValue = cell.CellValue is bool b && b
                    };
                    checkBox.LostFocus += (s, e) => SaveEditedValue();
                    return checkBox;

                case BeepColumnType.NumericUpDown:
                    var numericBox = new BeepTextBox
                    {
                        Theme = Theme,
                        IsChild = true,
                        Text = cell.CellValue?.ToString() ?? "0"
                    };
                    numericBox.LostFocus += (s, e) => SaveEditedValue();
                    numericBox.KeyDown += (s, e) => {
                        if (e.KeyCode == Keys.Enter) SaveEditedValue();
                        if (e.KeyCode == Keys.Escape) CancelEdit();
                    };
                    return numericBox;

                default:
                    return null;
            }
        }

        private BeepCellConfig CloseCurrentEditor()
        {
            if (_editingControl == null) return null;

            try
            {
                var editedCell = _editingCell;
                Controls.Remove(_editingControl);
                _editingControl.Dispose();
                _editingControl = null;
                _editingCell = null;
                IsEditorShown = false;

                return editedCell;
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error closing editor: {ex.Message}");
                return null;
            }
        }

        private void SaveEditedValue()
        {
            if (_editingControl == null || _editingCell == null) return;

            try
            {
                object newValue = null;

                if (_editingControl is BeepTextBox textBox)
                {
                    newValue = textBox.Text;
                }
                else if (_editingControl is BeepCheckBoxBool checkBox)
                {
                    newValue = checkBox.CurrentValue;
                }

                // Update the cell value
                _editingCell.CellValue = newValue;

                // Update the data source
                if (_editingCell.RowIndex < _filteredData.Count)
                {
                    var dataItem = _filteredData[_editingCell.RowIndex];
                    var column = _columns[_editingCell.ColumnIndex];
                    SetCellValue(dataItem, column, newValue?.ToString() ?? "");
                }

                CloseCurrentEditor();
                Invalidate();
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error saving edited value: {ex.Message}");
                CancelEdit();
            }
        }

        private Rectangle GetCellRectangleIn(BeepCellConfig cell)
        {
            int key = cell.RowIndex * 1000 + cell.ColumnIndex;
            if (_cachedCellBounds.ContainsKey(key))
            {
                return _cachedCellBounds[key];
            }

            // Calculate cell rectangle if not cached
            int xOffset = -_horizontalOffset;
            for (int i = 0; i < cell.ColumnIndex; i++)
            {
                if (_columns[i].Visible)
                    xOffset += _columns[i].Width;
            }

            int yPos = _gridBounds.Y + (cell.RowIndex - _visibleRowStart) * _virtualRowHeight;

            return new Rectangle(
                _gridBounds.X + xOffset,
                yPos,
                _columns[cell.ColumnIndex].Width,
                _virtualRowHeight
            );
        }

        // Handle row and item changes
        private void HandleRowAdded(int index)
        {
            _virtualRowCount = _filteredData.Count;
            UpdateScrollBars();
            Invalidate();
        }

        private void HandleRowDeleted(int index)
        {
            _virtualRowCount = _filteredData.Count;
            UpdateScrollBars();
            Invalidate();
        }

        private void HandleRowChanged(int index)
        {
            if (index >= _visibleRowStart && index <= _visibleRowEnd)
            {
                InvalidateDataArea();
            }
        }

        private void HandleItemAdded(int index) { RefreshData(); }
        private void HandleItemChanged(int index) { InvalidateDataArea(); }
        private void HandleItemDeleted(int index) { RefreshData(); }
        private void HandleItemMoved(int oldIndex, int newIndex) { RefreshData(); }
        
        private void HandleRowMoved(int oldIndex, int newIndex) 
        { 
            // Handle when a row is moved from one position to another
            try
            {
                if (oldIndex >= 0 && oldIndex < Rows.Count && 
                    newIndex >= 0 && newIndex < Rows.Count)
                {
                    // Update row indices after move
                    var movedRow = Rows[oldIndex];
                    Rows.RemoveAt(oldIndex);
                    Rows.Insert(newIndex, movedRow);
                    
                    // Update row indices
                    for (int i = 0; i < Rows.Count; i++)
                    {
                        Rows[i].RowIndex = i;
                    }
                    
                    // Update selection if needed
                    if (_selectedRowIndex == oldIndex)
                    {
                        _selectedRowIndex = newIndex;
                    }
                    else if (_selectedRowIndex > oldIndex && _selectedRowIndex <= newIndex)
                    {
                        _selectedRowIndex--;
                    }
                    else if (_selectedRowIndex < oldIndex && _selectedRowIndex >= newIndex)
                    {
                        _selectedRowIndex++;
                    }
                    
                    InvalidateDataArea();
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error handling row move: {ex.Message}");
            }
        }
        
        private void UpdateTooltip(Point location, GridHitTestInfo hitInfo)
        {
            // Update tooltip based on location and hit info
            try
            {
                string tooltipText = null;
                
                switch (hitInfo.Type)
                {
                    case HitTestType.Cell:
                        if (hitInfo.RowIndex >= 0 && hitInfo.RowIndex < Rows.Count &&
                            hitInfo.ColumnIndex >= 0 && hitInfo.ColumnIndex < _columns.Count)
                        {
                            var row = Rows[hitInfo.RowIndex];
                            var column = _columns[hitInfo.ColumnIndex];
                            
                            if (hitInfo.ColumnIndex < row.Cells.Count)
                            {
                                var cell = row.Cells[hitInfo.ColumnIndex];
                                var cellValue = cell.CellValue?.ToString() ?? "";
                                
                                // Show tooltip if text is longer than cell width or has special content
                                if (!string.IsNullOrEmpty(cellValue) && cellValue.Length > 20)
                                {
                                    tooltipText = $"{column.ColumnCaption}: {cellValue}";
                                }
                            }
                        }
                        break;
                        
                    case HitTestType.ColumnHeader:
                        if (hitInfo.ColumnIndex >= 0 && hitInfo.ColumnIndex < _columns.Count)
                        {
                            var column = _columns[hitInfo.ColumnIndex];
                            tooltipText = $"Column: {column.ColumnCaption}\nType: {column.CellEditor}\nClick to sort";
                        }
                        break;
                        
                    case HitTestType.None:
                    default:
                        // Clear tooltip
                        break;
                }
                
                // Update tooltip if we have one
                if (!string.IsNullOrEmpty(tooltipText))
                {
                    if (!tooltipShown)
                    {
                        ShowTooltip(tooltipText, location);
                        tooltipShown = true;
                    }
                }
                else
                {
                    if (tooltipShown)
                    {
                        HideTooltip();
                        tooltipShown = false;
                    }
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error updating tooltip: {ex.Message}");
            }
        }
        
        private ToolTip _currentTooltip;
        
        private void ShowTooltip(string text, Point location)
        {
            try
            {
                if (_currentTooltip == null)
                {
                    _currentTooltip = new ToolTip
                    {
                        AutoPopDelay = 5000,
                        InitialDelay = 500,
                        ReshowDelay = 100,
                        ShowAlways = true,
                        IsBalloon = false
                    };
                }
                
                var screenLocation = PointToScreen(location);
                _currentTooltip.Show(text, this, location.X, location.Y - 20, 3000);
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error showing tooltip: {ex.Message}");
            }
        }
        
        private void HideTooltip()
        {
            try
            {
                _currentTooltip?.Hide(this);
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error hiding tooltip: {ex.Message}");
            }
        }

        #endregion

        #endregion
        #endregion
    }
}