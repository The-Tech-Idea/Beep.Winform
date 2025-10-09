using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.Globalization;
using System.Reflection;
using TheTechIdea.Beep.DataBase;

using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.TextFields;




namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Data")]
    [Description("A grid control that displays data in a simple table format with scrollbars.")]
    [DisplayName("Beep Simple Grid")]
    public class BeepSimpleGrid : BeepControl
    {
        #region Properties
        #region Fields

        // Add these fields to track state
        private bool _fillVisibleRowsPending = false;
        private bool _isUpdatingRows = false;
        private DateTime _lastFillTime = DateTime.MinValue;
        private const int FILL_DEBOUNCE_MS = 50; // Minimum time between fills
        private Timer _fillRowsTimer;
        int filtercontrolsheight = 3;
        int filterPanelHeight = 60;
        bool _layoutDirty = false;
        private string _titleimagestring = "simpleinfoapps.svg";
        private BeepCheckBoxBool _selectAllCheckBox;
        private bool _deferRedraw = false;
        private Dictionary<int, int> _rowHeights = new Dictionary<int, int>(); // Key: DisplayIndex, Value: Height
        private bool _navigatorDrawn = false;
        private bool _showFilterpanel=false;
        private Rectangle filterPanelRect;
        private bool _filterpaneldrawn=false;

        private BeepTextBox filterTextBox;
        private BeepComboBox filterColumnComboBox;
        protected int headerPanelHeight = 30;
        protected int bottomagregationPanelHeight = 20;
        protected int footerPanelHeight = 12;
        protected int navigatorPanelHeight = 30;
        private int _stickyWidth = 0; // Cache sticky column width
        private Rectangle footerPanelRect;
        private Rectangle headerPanelRect;
        private Rectangle columnsheaderPanelRect;
        private Rectangle bottomagregationPanelRect;
        
        private Rectangle navigatorPanelRect;
        private Rectangle filterButtonRect;
        private BeepLabel titleLabel;
        private BeepButton filterButton;
        private BeepLabel percentageLabel; // New label for percentage (e.g., "36%")
        private int visibleHeight;
        private int visibleRowCount;
        private int bottomPanelY;
        private int botomspacetaken = 0;
        private int topPanelY;
        private Rectangle gridRect;
        private object rowsrect;
        private int _rowHeight = 25;
        private int _xOffset = 0;
        private bool _showFilterButton = true;
        private bool _resizingColumn = false;
        private bool _resizingRow = false;
        private int _resizingIndex = -1;
        private int _resizeMargin = 2;
        private Point _lastMousePos;
        private int _editingRowIndex = -1; // Add this field to track absolute row index in _fullData
        private BeepRowConfig _hoveredRow = null;
        private BeepRowConfig _currentRow = null;
        private BeepCellConfig _hoveredCell = null;
        private BeepCellConfig _selectedCell = null;
        private int _hoveredRowIndex = -1;
        private int _hoveredCellIndex = -1;
        private int _currentRowIndex = -1;
        private int _hoveredColumnIndex = -1;
        private int _selectedColumnIndex = -1;
        private int _hoveredColumnHeaderIndex = -1;
        private int _selectedColumnHeaderIndex = -1;
        private int _hoveredRowHeaderIndex = -1;
        private int _selectedRowHeaderIndex = -1;
        private int _hoveredSortIconIndex = -1;
        private int _hoveredFilterIconIndex = -1;
     
        private int _defaultcolumnheaderheight = 40;
        private int _defaultcolumnheaderwidth = 50;
        private TextImageRelation textImageRelation = TextImageRelation.ImageAboveText;
        // Create a pool of controls based on column type
        private Dictionary<BeepColumnType, List<IBeepUIComponent>> _controlPool =
            new Dictionary<BeepColumnType, List<IBeepUIComponent>>();
        // Cache layout information to reduce recalculation
      
        private Dictionary<int, Rectangle> _cellBounds = new Dictionary<int, Rectangle>();
        private int _startviewrow = 0;
        private int _endviewrow = 0;
        #endregion Fields
        #region Title Properties
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        private BeepRowConfig CurrentRow
        {
            get => _currentRow;
            set
            {
                _currentRow = value;
                if (value != null)
                {
                    _currentRowIndex = Rows.IndexOf(value);
                }
                else
                {
                    _currentRowIndex = -1;
                }
            }
        }
        [Browsable(true)]

        private string _titletext = "";
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleText
        {
            get => _titletext;
            set
            {
                _titletext = value;
                if (titleLabel != null)
                {
                    titleLabel.Text = value;
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
                if (titleLabel != null)
                {
                    titleLabel.TextImageRelation = value;
                }
                Invalidate();
            }
        }
        private Font _textFont;
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
                if (titleLabel != null)
                {
                    titleLabel.TextFont = _textFont;
                  //  titleLabel.MaxImageSize = new Size(titleLabel.Height-2, titleLabel.Height - 2); // Adjust image size if needed
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
                if (titleLabel != null)
                {
                    titleLabel.ImagePath = _titleimagestring;
                }
                Invalidate();
            }
        }
        private Font _columnHeadertextFont = new Font("Arial", 8);
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
        #endregion "Title Properties"
        #region "Data Source Properties"
        public string QueryFunctionName { get; set; }
        public string QueryFunction { get; set; }
        private Dictionary<int, bool> _persistentSelectedRows = new Dictionary<int, bool>(); // Add this field to track selection by RowID
        private object _dataSource;
        object finalData;
        private bool _isInitializing = true; // Flag to track initialization
        private object _pendingDataSource; // Store DataSource until initialization is complete
        private List<object> _fullData;
        Type _entityType;
        private int _dataOffset = 0;
        private string entityname;
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
        public string EntityName
        {
            get => entityname;
            set
            {
                entityname = value;
                //if (!string.IsNullOrEmpty(entityname))
                //{
                //    _entityType = Type.GetType(entityname);
                //}
            }
        }
        private EntityStructure _entity;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EntityStructure Entity
        {
            get => _entity;
            set
            {
                //MiscFunctions.SendLog("1 Entity Setter: " + value?.EntityName);
                _entity = value;
                if (_entity != null)
                {
                   
                    //MiscFunctions.SendLog("2 Entity Setter: " + _entity.EntityName);
                    if (!string.IsNullOrEmpty(_entity.EntityName))
                    {
                        try
                        {
                            entityname = _entity.EntityName;
                            //MiscFunctions.SendLog("3 Entity Setter: " + _entity.EntityName);
                            if(_entityType==null)   _entityType = Type.GetType(_entity.EntityName);
                        }
                        catch (Exception)
                        {
                            //MiscFunctions.SendLog("4 Entity Setter: " + _entity.EntityName);

                        }
                       
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
                //MiscFunctions.SendLog($"DataSource Setter: Type = {value?.GetType()?.Name}, DesignMode = {DesignMode}, IsInitializing = {_isInitializing}");
                if (_isInitializing)
                {
                    _pendingDataSource = value; // Defer setting until initialization is complete
                    //MiscFunctions.SendLog("Deferring DataSource setup until initialization completes");
                }
                else
                {
                    _dataSource = value;
                    DataSetup();
                    InitializeData();
                    FillVisibleRows();
                    UpdateScrollBars();
                    Invalidate();
                }
            }
        }

        #endregion "Data Source Properties"
        #region "Appearance Properties"
        #region "Column Properties"
        public bool ShowSortIcon { get; set; } = false;
        public bool ShowFilterIcon { get; set; } = false;
        public SortDirection SortDirection { get; set; } = SortDirection.None;
        public bool IsFiltered { get; set; } = false;

        private int _selectedSortIconIndex = -1;
        private int _selectedFilterIconIndex = -1;
        private List<Rectangle> columnHeaderBounds = new List<Rectangle>();
        private List<Rectangle> rowHeaderBounds = new List<Rectangle>();
        private List<Rectangle> sortIconBounds = new List<Rectangle>();
        private List<Rectangle> filterIconBounds = new List<Rectangle>();
        private Dictionary<string, Size> _columnTextSizes = new Dictionary<string, Size>();
        private bool _fitColumntoContent = false;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool FitColumnToContent
        {
            get => _fitColumntoContent;
            set
            {
                _fitColumntoContent = value;
                if (_fitColumntoContent)
                {
                    // Resize columns to fit content
                    foreach (var column in Columns)
                    {
                        column.Width = GetColumnWidth(column);
                    }
                }
                Invalidate();
            }
        }
        #endregion "Column Properties"

        public BindingList<BeepRowConfig> Rows { get; set; } = new BindingList<BeepRowConfig>();
        public BeepRowConfig aggregationRow { get; set; }
        private List<BeepColumnConfig> _columns = new List<BeepColumnConfig>();
        private bool columnssetupusingeditordontchange = false;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ColumnsSetupUsingEditorDontChange
        {
            get => columnssetupusingeditordontchange;
            set => columnssetupusingeditordontchange = value;
        }
        [Browsable(true)]
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)] // Changed from Visible to Content
        [Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
        public List<BeepColumnConfig> Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                //MiscFunctions.SendLog($"Columns Setter: Count = {value?.Count}, DesignMode = {DesignMode}");

                //MiscFunctions.SendLog("Columns set via property, marking as designer-configured");
                _columns = value;
                if (_columns != null)
                {
                    // reindex columns based on sequence in list
                    for (int i = 0; i < _columns.Count; i++)
                    {
                        _columns[i].Index = i;
                    }
                    if (_columns.Any())
                    {
                        columnssetupusingeditordontchange = true;
                    }
                }

                UpdateScrollBars();
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DefaultColumnHeaderWidth
        {
            get => _defaultcolumnheaderwidth;
            set { _defaultcolumnheaderwidth = value; Invalidate(); }
        }

        private bool _showverticalgridlines = true;
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
        private bool _showhorizontalgridlines = true;
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
        public bool ShowFilter
        {
            get => _showFilterpanel;
            set
            {
                _showFilterpanel = value;
                _filterpaneldrawn = false;
                //   filterButton.Visible= value;
                _layoutDirty = true; // Mark the layo
                filterTextBox.Visible = value;
                filterColumnComboBox.Visible = value;
            
                Invalidate();
            }
        }
        private bool _showSortIcons = true;
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
        private bool _showRowNumbers = true;
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
        private bool _showColumnHeaders = true;
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
        private bool _showRowHeaders = true;
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
        private bool _showNavigator = true;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowNavigator
        {
            get => _showNavigator;
            set
            {
                _showNavigator = value;
                if (_showNavigator)
                {
                  
                    ShowNavigationButtons();
                }
                else
                {
                   HideNavigationButtons();
                }
                    Invalidate();
            }
        }
        private bool _showaggregationRow = false;
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
        private bool _showFooter = false;
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
        private bool _showHeaderPanel = true;
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
        private bool _showHeaderPanelBorder = true;
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

        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ColumnHeaderHeight
        {
            get => _defaultcolumnheaderheight;
            set { _defaultcolumnheaderheight = value; Invalidate(); }
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
        public GridDataSourceType DataSourceType { get; set; } = GridDataSourceType.Fixed;
      

        // Scrollbar Properties
        private int _scrollTargetVertical; // Target scroll position (vertical)
        private int _scrollTargetHorizontal; // Target scroll position (horizontal)
        private BeepScrollBar _verticalScrollBar;
        private BeepScrollBar _horizontalScrollBar;
        private bool _showVerticalScrollBar = true;
        private Timer _scrollTimer; // For smooth scrolling
        private int _scrollTarget; // Target scroll position
        private int _scrollStep = 10; // Pixels per animation step
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
        private bool _showHorizontalScrollBar = true;
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

        public bool IsEditorShown { get; private set; }
        #endregion "Appearance Properties"
        #region "Configuration Properties"
        private BeepBindingNavigator _dataNavigator;
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
                        _dataNavigator.Theme = Theme; // Optional: sync theme
                        AttachNavigatorEvents();
                    }
                    Invalidate();
                }
            }
        }

        #endregion "Configuration Properties"
        #region "Checkboxes and Selections"
        private bool _showCheckboxes = false;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowCheckboxes
        {
            get => _showCheckboxes;
            set
            {
                _showCheckboxes = value;
                if (Columns is null) return;
                if (Columns.Count == 0) return;
                var selColumn = Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
                if (_showCheckboxes)
                {
                    selColumn.Visible = true;
                }
                else
                    selColumn.Visible = false;
                Invalidate();
            }
        }

        //private bool _showSelection = true;
        //[Browsable(true)]
        //[Category("Layout")]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //public bool ShowSelection
        //{
        //    get => _showSelection;
        //    set
        //    {
        //        _showSelection = value;

        //        Invalidate();
        //    }
        //}

        private List<int> _selectedRows = new List<int>();
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<int> SelectedRows
        {
            get => _selectedRows;
            set
            {
                _selectedRows = value ?? new List<int>(); // Ensure it's never null
                Invalidate();
            }
        }

        private List<BeepRowConfig> _selectedgridrows = new List<BeepRowConfig>();
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<BeepRowConfig> SelectedGridRows
        {
            get => _selectedgridrows;
            set
            {
                _selectedgridrows = value ?? new List<BeepRowConfig>(); // Ensure it's never null
                Invalidate();
            }
        }

        // New property for selection column width
        private int _selectionColumnWidth = 30;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SelectionColumnWidth
        {
            get => _selectionColumnWidth;
            set
            {
                _selectionColumnWidth = value;
                Invalidate();
            }
        }
        public event EventHandler SelectedRowsChanged;

        protected virtual void OnSelectedRowsChanged()
        {
            SelectedRowsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseSelectedRowsChanged()
        {
            OnSelectedRowsChanged();
        }
        #endregion "Checkboxes and Selections"

        private bool _allowUserToAddRows = false;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool AllowUserToAddRows
        {
            get => _allowUserToAddRows;
            set
            {
                _allowUserToAddRows = value;
                Invalidate();
            }
        }

        private bool _allowUserToDeleteRows = false;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool AllowUserToDeleteRows
        {
            get => _allowUserToDeleteRows;
            set
            {
                _allowUserToDeleteRows = value;
                Invalidate();
            }
        }
        private DataGridViewSelectionMode _selectionMode = DataGridViewSelectionMode.FullRowSelect;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public DataGridViewSelectionMode SelectionMode
        {
            get => _selectionMode;
            set
            {
                _selectionMode = value;
                Invalidate();
            }
        }
        private DataGridViewAutoSizeColumnsMode _autoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode
        {
            get => _autoSizeColumnsMode;
            set
            {
                _autoSizeColumnsMode = value;
                Invalidate();
            }
        }
        private bool _readOnly = false;
        [Browsable(true)]
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                _readOnly = value;
                foreach (var column in Columns)
                {
                    column.ReadOnly = value;
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("If true, grid will be drawn in black and white (monochrome) instead of color.")]
        public bool DrawInBlackAndWhite { get; set; } = false;

        private string _percentageText = string.Empty;
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
        #endregion "Properties"
        #region Constructor
        public BeepSimpleGrid():base()
        {
            //// This ensures child controls are painted properly
            // Enable double buffering for smoother rendering
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer, true);
            // Add high-DPI specific optimizations
            if (DpiScaleFactor > 1.5f)
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer, true);

                // Reduce scroll timer frequency for high DPI
                _scrollTimer = new Timer { Interval = 32 }; // Slower for high DPI
            }
            else
            {
                _scrollTimer = new Timer { Interval = 16 }; // Normal
            }
            UpdateStyles();

            // Check for design mode to prevent unnecessary operations
            if (DesignMode)
            {
                // Skip expensive operations in design mode
                _isInitializing = true;
                _filterpaneldrawn = true;
                _navigatorDrawn = true;
                return;
            }
            TabStop = true;
            this.Focus();
            _isInitializing = true;
            this.PreviewKeyDown += BeepGrid_PreviewKeyDown;

            this.KeyDown += BeepSimpleGrid_KeyDown;

            _scrollTimer = new Timer { Interval = 16 }; // ~60 FPS for smooth animation
            _scrollTimer.Tick += ScrollTimer_Tick;
            ApplyThemeToChilds = false;
            Width = 200;
            Height = 200;
            // Attach event handlers for mouse actions
           
            
            this.MouseDown += BeepGrid_MouseDown;
           
            this.MouseUp += BeepGrid_MouseUp;
            TabKeyPressed += Tabhandler;
            
            Rows.ListChanged += Rows_ListChanged;
          
            // Initialize scrollbars immediately to ensure they’re never null
            _verticalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Vertical,
                Theme = Theme,
                Visible = false,
                IsChild = true
            };
            _verticalScrollBar.Scroll += VerticalScrollBar_Scroll;
            Controls.Add(_verticalScrollBar);

            _horizontalScrollBar = new BeepScrollBar
            {
                ScrollOrientation = Orientation.Horizontal,
                Theme = Theme,
                Visible = false,
                IsChild = true
            };
        
            _horizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
            Controls.Add(_horizontalScrollBar);
            //DataNavigator = new BeepBindingNavigator
            //{
            //    ShowAllBorders = false,
            //    ShowShadow = false,
            //    IsBorderAffectedByTheme = false,
            //    IsShadowAffectedByTheme = false,
            //    MenuStyle = MenuStyle,

            //};
            //AttachNavigatorEvents();
            
            //Controls.Add(DataNavigator);

            titleLabel = new BeepLabel
            {
                TextAlign = ContentAlignment.MiddleLeft,
                Theme = Theme,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                MaxImageSize = new Size(20, 20),
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true,
                Visible=false,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false
            };
            titleLabel.ImagePath = _titleimagestring;
            // titleLabel.BackColor = _currentTheme?.BackColor ?? Color.White;
            Controls.Add(titleLabel);
           
            // Initialize filter controls
            filterTextBox = new BeepTextBox
            {
                Theme = Theme,
                IsChild = true,
                Width = 150,
                Height = 20,
                Visible=false,
                PlaceholderText = "Filter ......"
            };
            filterTextBox.TextChanged += FilterTextBox_TextChanged;
            Controls.Add(filterTextBox);

            filterColumnComboBox = new BeepComboBox
            {
                Theme = Theme,
                IsChild = true,
                Width = 120,
                Visible = false,
                Height = 20,
               
            };
            filterColumnComboBox.SelectedItemChanged += FilterColumnComboBox_SelectedIndexChanged;
            Controls.Add(filterColumnComboBox);
            CreateNavigationButtons();
            
          
            // Initialize the Select All checkbox
            _selectAllCheckBox = new BeepCheckBoxBool
            {
                Theme = Theme,
                IsChild = true,
                CheckBoxSize = 16, // Matches typical checkbox size, adjustable
                CheckedValue = true,
                UncheckedValue = false,
                CurrentValue = false,
                Visible = false,
                IsFrameless=true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false,
                HideText = true // No text needed in header
            };
            _selectAllCheckBox.StateChanged += SelectAllCheckBox_StateChanged;
            Controls.Add(_selectAllCheckBox);
            //filterButton = new BeepButton
            //{
            //    Text = "Filters",
            //    MenuStyle = MenuStyle,
            //    IsChild = true,
            //    Width = 80,
            //    Height = 24,
            //    BorderRadius = 12,
            //    BorderColor = Color.LightGray,
            //    BackColor = Color.White,
            //    ForeColor = Color.Black
            //};
            //filterButton.MouseClick += FilterButton_Click;
            //Controls.Add(filterButton);

            percentageLabel = new BeepLabel
            {
                Text = _percentageText,
                Theme = Theme,
                IsChild = true,
                AutoSize = true,
                Visible=false,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Black
            };
            Controls.Add(percentageLabel);
            // Subscribe to the Resize event to handle control resizing
            tooltipShown = false;
            InitializeRows();
        }

        #endregion
        #region External Data Navigator
        private void AttachNavigatorEvents()
        {
            DataNavigator.CallPrinter += DataNavigator_CallPrinter;
            DataNavigator.SendMessage += DataNavigator_SendMessage;
            DataNavigator.ShowSearch += DataNavigator_ShowSearch;
            DataNavigator.NewRecordCreated += DataNavigator_NewRecordCreated;
            DataNavigator.SaveCalled += DataNavigator_SaveCalled;
            DataNavigator.DeleteCalled += DataNavigator_DeleteCalled;
            DataNavigator.EditCalled += DataNavigator_EditCalled;
            // Subscribe to propagated events
            DataNavigator.PositionChanged += DataNavigator_PositionChanged;
            DataNavigator.CurrentChanged += DataNavigator_CurrentChanged; // Optional, for completeness
            DataNavigator.ListChanged += DataNavigator_ListChanged;       // Optional, for list changes
            DataNavigator.DataSourceChanged += DataNavigator_DataSourceChanged; // Optional, for data source changes
        }
        private void DetachNavigatorEvents()
        {
            DataNavigator.CallPrinter -= DataNavigator_CallPrinter;
            DataNavigator.SendMessage -= DataNavigator_SendMessage;
            DataNavigator.ShowSearch -= DataNavigator_ShowSearch;
            DataNavigator.NewRecordCreated -= DataNavigator_NewRecordCreated;
            DataNavigator.SaveCalled -= DataNavigator_SaveCalled;
            DataNavigator.DeleteCalled -= DataNavigator_DeleteCalled;
            DataNavigator.EditCalled -= DataNavigator_EditCalled;
            // Subscribe to propagated events
            DataNavigator.PositionChanged -= DataNavigator_PositionChanged;
            DataNavigator.CurrentChanged -= DataNavigator_CurrentChanged; // Optional, for completeness
            DataNavigator.ListChanged -= DataNavigator_ListChanged;       // Optional, for list changes
            DataNavigator.DataSourceChanged -= DataNavigator_DataSourceChanged; // Optional, for data source changes
        }
        private void DataNavigator_PositionChanged(object sender, EventArgs e)
        {
            int targetIndex = DataNavigator?.BindingSource.Position ?? -1;
            if (targetIndex >= 0 && targetIndex < _fullData.Count)
            {
                // Check if the grid’s selection already matches to avoid loop
                if (_currentRow == null || _currentRow.DisplayIndex != targetIndex)
                {
                    int visibleRowCount = GetVisibleRowCount();
                    int newOffset = Math.Max(0, Math.Min(targetIndex - (visibleRowCount / 2), _fullData.Count - visibleRowCount));
                    _dataOffset = newOffset;

                    FillVisibleRows();
                    int newRowIndex = targetIndex - _dataOffset;
                    if (newRowIndex >= 0 && newRowIndex < Rows.Count)
                    {
                        SelectCell(newRowIndex, _selectedColumnIndex >= 0 ? _selectedColumnIndex : 0);
                    }

                    UpdateScrollBars();
                    Invalidate();
                }
            }
        }
        private void DataNavigator_CurrentChanged(object sender, EventArgs e)
        {
            // Optional: Handle current item change if needed (e.g., highlight current row)
            FillVisibleRows();
            Invalidate();
        }
        private void DataNavigator_ListChanged(object sender, ListChangedEventArgs e)
        {
            // Optional: Handle list changes (e.g., add/remove items)
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }
        private void DataNavigator_DataSourceChanged(object sender, EventArgs e)
        {
            // Optional: Handle data source change (e.g., reload grid)
          //  _fullData = DataNavigator.BindingSource.DataSource as List<object> ?? new List<object>();
            InitializeRows();
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }
        private void DataNavigator_CallPrinter(object sender, BindingSource bs)
        {
            try
            {
                // Placeholder for printing functionality
            //    //MiscFunctions.SendLog("CallPrinter triggered - implement printing logic here.");
                // Example: Print the current _fullData
                // You could raise an event or call a printing method
                MessageBox.Show("Printing functionality not implemented yet.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
             //   //MiscFunctions.SendLog($"CallPrinter Error: {ex.Message}");
                //MiscFunctions.SendLog($"Error in printing: {ex.Message}");
            }
        }
        private void DataNavigator_SendMessage(object sender, BindingSource bs)
        {
            try
            {
                // Placeholder for sending/sharing functionality
              //  //MiscFunctions.SendLog("SendMessage triggered - implement sharing logic here.");
                // Example: Share the current selected row or entire _fullData
                if (_currentRow != null && _currentRow.DisplayIndex >= 0 && _currentRow.DisplayIndex < _fullData.Count)
                {
                    var selectedItem = _fullData[_currentRow.DisplayIndex];
                    MessageBox.Show($"Sharing item: {selectedItem}", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No item selected to share.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
             //   //MiscFunctions.SendLog($"SendMessage Error: {ex.Message}");
                //MiscFunctions.SendLog($"Error in sending message: {ex.Message}");
            }
        }
        private void DataNavigator_ShowSearch(object sender, BindingSource bs)
        {
            try
            {
                // Placeholder for search/filter functionality
             //   //MiscFunctions.SendLog("ShowSearch triggered - implement filter UI here.");
                // Example: Show a filter dialog and apply filtering to _fullData
                MessageBox.Show("Search/Filter UI not implemented yet.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Potential implementation:
                // var filterDialog = new FilterDialog();
                // if (filterDialog.ShowDialog() == DialogResult.OK)
                // {
                //     var filteredData = ApplyFilter(filterDialog.Criteria);
                //     _fullData.Clear();
                //     _fullData.AddRange(filteredData);
                //     bs.ResetBindings(false);
                //     FillVisibleRows();
                // }
            }
            catch (Exception ex)
            {
            //    //MiscFunctions.SendLog($"ShowSearch Error: {ex.Message}");
                //MiscFunctions.SendLog($"Error in search: {ex.Message}");
            }
        }
        // Simplify DataNavigator_DeleteCalled to focus on grid state updates
        private void DataNavigator_DeleteCalled(object sender, BindingSource bs)
        {
            try
            {
                if (_currentRow != null && _fullData.Any())
                {
                    int dataIndex = _currentRow.DisplayIndex;
                    if (dataIndex >= 0 && dataIndex < _fullData.Count)
                    {
                        var item = _fullData[dataIndex];
                        var wrapper = item as DataRowWrapper;
                        if (wrapper == null)
                        {
                            //MiscFunctions.SendLog($"DataNavigator_DeleteCalled: Item at index {dataIndex} is not a DataRowWrapper");
                            return;
                        }

                        // Remove from the data source and update _fullData, Trackings, originalList, etc.
                        DeleteFromDataSource(wrapper.OriginalData);

                        // Update the grid's state
                        if (_fullData.Any())
                        {
                            int newSelectedIndex = Math.Min(dataIndex, _fullData.Count - 1);
                            if (_dataSource is BindingSource && bs == _bindingSource)
                            {
                                bs.Position = newSelectedIndex;
                            }
                            StartSmoothScroll(Math.Max(0, newSelectedIndex - GetVisibleRowCount() + 1));
                            FillVisibleRows();
                            if (newSelectedIndex >= _dataOffset && newSelectedIndex < _dataOffset + Rows.Count)
                            {
                                SelectCell(newSelectedIndex - _dataOffset, _selectedColumnIndex >= 0 ? _selectedColumnIndex : 0);
                            }
                        }
                        else
                        {
                            _currentRow = null;
                            _currentRowIndex = -1;
                            if (_dataSource is BindingSource && bs == _bindingSource)
                            {
                                bs.Position = -1;
                            }
                            FillVisibleRows();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"DeleteCalled Error: {ex.Message}");
                //MiscFunctions.SendLog($"Error deleting record: {ex.Message}");
            }
        }
        // Update DataNavigator_NewRecordCreated to use CreateNewRecordForBindingSource when _dataSource is a BindingSource
        private void DataNavigator_NewRecordCreated(object sender, BindingSource bs)
        {
            try
            {
                if (_dataSource is BindingSource)
                {
                    // Create a new record for the BindingSource
                    CreateNewRecordForBindingSource();
                }
                else if (_dataSource is IList)
                {
                    // Create a new record for the IList
                    CreateNewRecordForIList();
                }
                else
                {
                    //MiscFunctions.SendLog("DataNavigator_NewRecordCreated: Unsupported data source type");
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"NewRecordCreated Error: {ex.Message}");
                //MiscFunctions.SendLog($"Error adding new record: {ex.Message}");
            }
        }
        private void DataNavigator_EditCalled(object sender, BindingSource bs)
        {
            try
            {
                if (_selectedCell != null)
                {
                    BeginEdit();
                 //   //MiscFunctions.SendLog($"EditCalled: Editing cell at row {_selectedCell.RowIndex}, column {_selectedCell.ColumnIndex}");
                }
                else
                {
                //    //MiscFunctions.SendLog("EditCalled: No cell selected to edit.");
                    MessageBox.Show("Please select a cell to edit.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"EditCalled Error: {ex.Message}");
                //MiscFunctions.SendLog($"Error starting edit: {ex.Message}");
            }
        }
        private void DataNavigator_SaveCalled(object sender, BindingSource bs)
        {
            try
            {
                // Save changes to _fullData (external persistence delegated to caller)
                CloseCurrentEditor(); // Ensure any open editor is saved
                foreach (var row in Rows)
                {
                    if (row.IsDirty)
                    {
                        int dataIndex = _dataOffset + Rows.IndexOf(row);
                        if (dataIndex >= 0 && dataIndex < _fullData.Count)
                        {
                            var dataItem = _fullData[dataIndex];
                            Tracking tracking = GetTrackingItem(dataItem);
                            if (tracking != null && tracking.EntityState != EntityState.Added)
                            {
                                tracking.EntityState = EntityState.Modified;
                            }
                        }
                        row.IsDirty = false; // Reset dirty flag
                    }
                }
                bs.ResetBindings(false); // Refresh navigator
                Invalidate();
                //  //MiscFunctions.SendLog("Data saved locally in grid.");
                MessageBox.Show("Changes saved locally. Implement external persistence if needed.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"SaveCalled Error: {ex.Message}");
                //MiscFunctions.SendLog($"Error saving data: {ex.Message}");
            }
        }
      
        private void SyncWithNavigatorPosition()
        {
            int targetIndex = DataNavigator.BindingSource.Position;
            if (targetIndex >= 0 && targetIndex < _fullData.Count)
            {
                int visibleRowCount = GetVisibleRowCount();
                int newOffset = Math.Max(0, Math.Min(targetIndex - (visibleRowCount / 2), _fullData.Count - visibleRowCount));
                if (newOffset != _dataOffset)
                {
                    StartSmoothScroll(newOffset);
                }

                int newRowIndex = targetIndex - _dataOffset;
                if (newRowIndex >= 0 && newRowIndex < Rows.Count)
                {
                    SelectCell(newRowIndex, _selectedColumnIndex >= 0 ? _selectedColumnIndex : 0);
                }
            }
        }
        #endregion External Data Navigator
        #region DataSource Update

        // Existing CreateNewRecordForIList (already includes logging and error handling)
        private void CreateNewRecordForIList()
        {
            try
            {
                if (Entity == null)
                {
                    //MiscFunctions.SendLog("CreateNewRecordForIList: Cannot add new record: Entity structure not defined.");
                    return;
                }

                // Create a new record of the appropriate type
                var newItem = Activator.CreateInstance(_entityType);

                // Add to the IList data source
                if (_dataSource is IList list)
                {
                    list.Add(newItem);
                    //MiscFunctions.SendLog($"CreateNewRecordForIList: Added new record to IList: {newItem}");
                }
                else
                {
                    //MiscFunctions.SendLog($"CreateNewRecordForIList: _dataSource is not an IList");
                    return;
                }

                // Wrap the new record in a DataRowWrapper and add to _fullData
                var wrapper = new DataRowWrapper(newItem, _fullData.Count)
                {
                    TrackingUniqueId = Guid.NewGuid(),
                    RowState = DataRowState.Added,
                    DateTimeChange = DateTime.Now
                };
                _fullData.Add(wrapper);

                // Update Trackings and originalList
                Tracking newTracking = new Tracking(Guid.NewGuid(), originalList.Count, _fullData.Count - 1)
                {
                    EntityState = EntityState.Added
                };
                originalList.Add(wrapper);
                Trackings.Add(newTracking);

                // Update the grid's state
                int newOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
                StartSmoothScroll(newOffset);
                FillVisibleRows();

                int newRowIndex = _fullData.Count - 1 - _dataOffset;
                if (newRowIndex >= 0 && newRowIndex < Rows.Count)
                {
                    SelectCell(newRowIndex, 0);
                    BeginEdit();
                }

                if (IsLogging)
                {
                    UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                    {
                        TrackingRecord = newTracking,
                        LogAction = LogAction.Insert,
                        UpdatedFields = new Dictionary<string, object>()
                    };
                }

                //MiscFunctions.SendLog($"CreateNewRecordForIList: Added new record to _fullData at index {_fullData.Count - 1}");
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"CreateNewRecordForIList Error: {ex.Message}");
                //MiscFunctions.SendLog($"Error adding new record: {ex.Message}");
            }
        }
        private void UpdateIListDataSource(int dataIndex, object updatedItem)
        {
            if (_dataSource is IList list && dataIndex >= 0 && dataIndex < list.Count)
            {
                list[dataIndex] = updatedItem; // Update the item in the IList
                //MiscFunctions.SendLog($"UpdateIListDataSource: Updated item at index {dataIndex} in IList");
            }
            else
            {
                //MiscFunctions.SendLog($"UpdateIListDataSource: Invalid index {dataIndex} or _dataSource is not an IList");
            }
        }
        private void InsertIntoDataSource(object newData)
        {
            if (_dataSource is BindingSource)
            {
                if (_bindingSource != null)
                {
                    _bindingSource.Add(newData); // Add to BindingSource, which notifies listeners
                    //MiscFunctions.SendLog($"InsertIntoDataSource: Added new record to BindingSource: {newData}");
                }
                else
                {
                    //MiscFunctions.SendLog($"InsertIntoDataSource: _bindingSource is null, cannot add new record");
                }
            }
            else if (_dataSource is IList list)
            {
                list.Add(newData); // Add to IList
                //MiscFunctions.SendLog($"InsertIntoDataSource: Added new record to IList: {newData}");
            }
            else
            {
                //MiscFunctions.SendLog($"InsertIntoDataSource: Unsupported data source type: {_dataSource?.GetType()}");
            }
        }
        private void UpdateInDataSource(object originalData, Dictionary<string, object> changes)
        {
            if (_dataSource is BindingSource)
            {
                if (_bindingSource != null)
                {
                    int dataIndex = _bindingSource.IndexOf(originalData);
                    if (dataIndex >= 0 && dataIndex < _bindingSource.Count)
                    {
                        _bindingSource[dataIndex] = originalData; // Update the item in the BindingSource
                        _bindingSource.ResetItem(dataIndex); // Notify listeners of the change
                        //MiscFunctions.SendLog($"UpdateInDataSource: Updated record in BindingSource at index {dataIndex}: Original={originalData}, Changes={string.Join(", ", changes.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
                    }
                    else
                    {
                        // If the item is no longer in the BindingSource's List (e.g., filtered out), refresh _fullData
                        DataSetup();
                        InitializeData();
                        //MiscFunctions.SendLog($"UpdateInDataSource: Item not found in BindingSource, refreshed grid");
                    }
                }
                else
                {
                    //MiscFunctions.SendLog($"UpdateInDataSource: _bindingSource is null, cannot update record");
                }
            }
            else if (_dataSource is IList list)
            {
                int dataIndex = list.IndexOf(originalData);
                if (dataIndex >= 0 && dataIndex < list.Count)
                {
                    list[dataIndex] = originalData; // Update the item in the IList
                    //MiscFunctions.SendLog($"UpdateInDataSource: Updated record in IList at index {dataIndex}: Original={originalData}, Changes={string.Join(", ", changes.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
                }
                else
                {
                    //MiscFunctions.SendLog($"UpdateInDataSource: Item not found in IList at index {dataIndex}");
                }
            }
            else
            {
                //MiscFunctions.SendLog($"UpdateInDataSource: Unsupported data source type: {_dataSource?.GetType()}");
            }
        }
        private void CreateNewRecordForBindingSource()
        {
            try
            {
                if (Entity == null)
                {
                    //MiscFunctions.SendLog("CreateNewRecordForBindingSource: Cannot add new record: Entity structure not defined.");
                    MessageBox.Show("Entity structure not defined.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

         //       Type entityType = Type.GetType(Entity.EntityName);
                if (_entityType == null)
                {
                    //MiscFunctions.SendLog($"CreateNewRecordForBindingSource: Invalid entity type name: {Entity.EntityName}");
                    MessageBox.Show($"Invalid entity type: {Entity.EntityName}", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_bindingSource == null)
                {
                    //MiscFunctions.SendLog("CreateNewRecordForBindingSource: _bindingSource is null, cannot add new record");
                    MessageBox.Show("BindingSource is not initialized.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var newItem = Activator.CreateInstance(_entityType);
                _isAddingNewRecord = true; // Set flag before adding
                _bindingSource.Add(newItem);
                //MiscFunctions.SendLog($"CreateNewRecordForBindingSource: Added new record to BindingSource: {newItem}");
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"CreateNewRecordForBindingSource Error: {ex.Message}");
                MessageBox.Show($"Error adding new record: {ex.Message}", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _isAddingNewRecord = false; // Reset on error
            }
        }
        private void DeleteFromDataSource(object originalData)
        {
            try
            {
                if (originalData == null)
                {
                    //MiscFunctions.SendLog("DeleteFromDataSource: Original data is null, cannot delete record");
                    return;
                }

                DataRowWrapper wrapper = _fullData.OfType<DataRowWrapper>().FirstOrDefault(w => w.OriginalData == originalData);
                int dataIndex = wrapper != null ? _fullData.IndexOf(wrapper) : -1;
                if (wrapper == null || dataIndex < 0)
                {
                    //MiscFunctions.SendLog("DeleteFromDataSource: Could not find DataRowWrapper for originalData in _fullData");
                    return;
                }

                Tracking tracking = GetTrackingItem(wrapper);
                if (tracking != null)
                {
                    tracking.EntityState = EntityState.Deleted;
                }

                if (_dataSource is BindingSource && _bindingSource != null)
                {
                    int bsIndex = _bindingSource.IndexOf(originalData);
                    if (bsIndex >= 0 && bsIndex < _bindingSource.Count)
                    {
                        _bindingSource.RemoveAt(bsIndex); // Let HandleItemDeleted update state
                        //MiscFunctions.SendLog($"DeleteFromDataSource: Triggered deletion from BindingSource at index {bsIndex}: {originalData}");
                    }
                    else
                    {
                        //MiscFunctions.SendLog($"DeleteFromDataSource: Item not found in BindingSource at index {bsIndex}");
                    }
                }
                else if (_dataSource is IList list)
                {
                    int listIndex = list.IndexOf(originalData);
                    if (listIndex >= 0 && listIndex < list.Count)
                    {
                        list.RemoveAt(listIndex);
                        _fullData.RemoveAt(dataIndex);
                        for (int i = 0; i < _fullData.Count; i++)
                        {
                            if (_fullData[i] is DataRowWrapper dataRow) dataRow.RowID = i;
                        }
                        originalList.RemoveAt(dataIndex);
                        if (tracking != null) Trackings.Remove(tracking);
                        deletedList.Add(wrapper);

                        if (IsLogging)
                        {
                            UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                            {
                                TrackingRecord = tracking,
                                LogAction = LogAction.Delete,
                                UpdatedFields = new Dictionary<string, object>()
                            };
                            if (tracking == null)
                            {
                                //MiscFunctions.SendLog("DeleteFromDataSource: Tracking not found for IList deletion");
                            }
                        }

                        //MiscFunctions.SendLog($"DeleteFromDataSource: Deleted record from IList at index {listIndex} and updated state");
                    }
                    else
                    {
                        //MiscFunctions.SendLog($"DeleteFromDataSource: Item not found in IList at index {listIndex}");
                    }
                }
                else
                {
                    //MiscFunctions.SendLog($"DeleteFromDataSource: Unsupported data source type: {_dataSource?.GetType()}");
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"DeleteFromDataSource Error: {ex.Message}");
            }
        }
        private void UpdateBindingSourceDataSource(int dataIndex, object updatedItem)
        {
            try
            {
                if (_bindingSource == null)
                {
                    //MiscFunctions.SendLog("UpdateBindingSourceDataSource: _bindingSource is null, cannot update");
                    return;
                }

                if (dataIndex >= 0 && dataIndex < _bindingSource.Count)
                {
                    _bindingSource[dataIndex] = updatedItem;
                    _bindingSource.ResetItem(dataIndex);
                    //MiscFunctions.SendLog($"UpdateBindingSourceDataSource: Updated item at index {dataIndex} in BindingSource");
                }
                else
                {
                    //MiscFunctions.SendLog($"UpdateBindingSourceDataSource: Index {dataIndex} out of bounds (Count: {_bindingSource.Count}), attempting to resolve");

                    if (dataIndex >= 0 && dataIndex < _fullData.Count)
                    {
                        var wrapper = _fullData[dataIndex] as DataRowWrapper;
                        if (wrapper != null)
                        {
                            object originalItem = wrapper.OriginalData;
                            int bsIndex = _bindingSource.IndexOf(originalItem);
                            if (bsIndex >= 0)
                            {
                                _bindingSource[bsIndex] = updatedItem;
                                _bindingSource.ResetItem(bsIndex);
                                //MiscFunctions.SendLog($"UpdateBindingSourceDataSource: Updated original item at index {bsIndex} in BindingSource");
                            }
                            else
                            {
                                //MiscFunctions.SendLog($"UpdateBindingSourceDataSource: Original item not found in BindingSource, skipping update (possibly filtered out)");
                                // Optionally update _fullData only and log
                                wrapper.OriginalData = updatedItem;
                                wrapper.RowState = DataRowState.Modified;
                                wrapper.DateTimeChange = DateTime.Now;
                            }
                        }
                        else
                        {
                            //MiscFunctions.SendLog($"UpdateBindingSourceDataSource: No valid DataRowWrapper at index {dataIndex} in _fullData");
                        }
                    }
                    else
                    {
                        //MiscFunctions.SendLog($"UpdateBindingSourceDataSource: Index {dataIndex} invalid for _fullData (Count: {_fullData.Count}), skipping update");
                    }
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"UpdateBindingSourceDataSource Error at index {dataIndex}: {ex.Message}");
            }
        }
        private void UpdateDataRecordFromRow(BeepCellConfig editingCell)
        {
            if (Rows == null || editingCell == null || editingCell.RowIndex < 0 || editingCell.RowIndex >= Rows.Count || _fullData == null || !_fullData.Any())
                return;

            BeepRowConfig row = Rows[editingCell.RowIndex];

            // Find the "RowID" cell to get the stable identifier
            BeepColumnConfig rowIdCol = Columns.Find(p => p.IsRowID);
            var rowIdCell = row.Cells.FirstOrDefault(c => Columns[c.ColumnIndex].IsRowID);
            if (rowIdCell == null || rowIdCell.CellValue == null || !int.TryParse(rowIdCell.CellValue.ToString(), out int rowID))
            {
                //MiscFunctions.SendLog($"UpdateDataRecordFromRow: Failed to retrieve RowID for row at index {editingCell.RowIndex}");
                return;
            }

            // Find the corresponding DataRowWrapper in _fullData using RowID
            DataRowWrapper dataItem = null;
            int dataIndex = -1;
            for (int i = 0; i < _fullData.Count; i++)
            {
                var wrapper = _fullData[i] as DataRowWrapper;
                if (wrapper != null && wrapper.RowID == rowID)
                {
                    dataItem = wrapper;
                    dataIndex = i;
                    break;
                }
            }

            if (dataItem == null || dataIndex < 0)
            {
                //MiscFunctions.SendLog($"UpdateDataRecordFromRow: Could not find record with RowID {rowID} in _fullData");
                return;
            }

            object originalData = dataItem.OriginalData;
            bool hasChanges = false;
            int bsIndex = -1;

            // Find the index in BindingSource *before* modifying originalData
            if (_bindingSource != null)
            {
                bsIndex = _bindingSource.IndexOf(originalData);
                if (bsIndex < 0)
                {
                    //MiscFunctions.SendLog($"UpdateDataRecordFromRow: Original item not found in BindingSource before update, RowID={rowID}");
                }
            }

            // Apply changes to originalData
            foreach (var cell in row.Cells)
            {
                if (cell.IsDirty)
                {
                    var col = Columns[cell.ColumnIndex];
                    if (col.IsSelectionCheckBox || col.IsRowNumColumn || col.IsRowID)
                        continue;

                    var prop = originalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                    if (prop != null)
                    {
                        object convertedValue = MiscFunctions.ConvertValueToPropertyType(prop.PropertyType, cell.CellValue);
                        object originalValue = prop.GetValue(originalData);
                        if (!Equals(convertedValue, originalValue))
                        {
                            prop.SetValue(originalData, convertedValue);
                            hasChanges = true;
                        }
                    }
                    row.IsDirty = true;
                }
            }

            if (hasChanges && _bindingSource != null)
            {
                if (bsIndex >= 0 && bsIndex < _bindingSource.Count)
                {
                    // Update BindingSource with the modified object using the pre-found index
                    _bindingSource[bsIndex] = originalData;
                    _bindingSource.ResetItem(bsIndex); // Trigger ItemChanged event
                    //MiscFunctions.SendLog($"UpdateDataRecordFromRow: Updated BindingSource at index {bsIndex} with RowID={rowID}");
                }
                else
                {
                    // Fallback: Re-search using RowID if index is invalid post-modification
                    for (int i = 0; i < _bindingSource.Count; i++)
                    {
                        var bsItem = _bindingSource[i];
                        var wrapper = _fullData.FirstOrDefault(w => w is DataRowWrapper dw && dw.OriginalData == bsItem) as DataRowWrapper;
                        if (wrapper != null && wrapper.RowID == rowID)
                        {
                            _bindingSource[i] = originalData;
                            _bindingSource.ResetItem(i);
                            //MiscFunctions.SendLog($"UpdateDataRecordFromRow: Updated BindingSource via RowID={rowID} at index {i}");
                            break;
                        }
                    }
                    if (bsIndex < 0)
                    {
                        //MiscFunctions.SendLog($"UpdateDataRecordFromRow: Item with RowID={rowID} not found in BindingSource after update, skipping");
                    }
                }

                // Update the wrapper in _fullData
                dataItem.OriginalData = originalData;
                dataItem.RowState = DataRowState.Modified;
                dataItem.DateTimeChange = DateTime.Now;
            }
        }
        private void SaveToDataSource()
        {
            try
            {
                if (_fullData == null || originalList == null || DataSource == null)
                {
                    //MiscFunctions.SendLog("SaveToDataSource: _fullData, originalList, or DataSource is null. Cannot sync changes.");
                    return;
                }

                // Handle the DataSource based on its type
                if (DataSource is BindingSource bindingSource)
                {
                    // Scenario 1: DataSource is BindingSource
                    // Use BindingSource to manage changes
                    foreach (var tracking in Trackings.Where(t => t.EntityState == EntityState.Added || t.EntityState == EntityState.Modified))
                    {
                        var wrappedItem = _fullData[tracking.CurrentIndex] as DataRowWrapper;
                        if (wrappedItem != null)
                        {
                            switch (wrappedItem.RowState)
                            {
                                case DataRowState.Added:
                                    if (tracking.EntityState == EntityState.Added)
                                    {
                                     //   //MiscFunctions.SendLog($"Saving Added Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
                                        bindingSource.Add(wrappedItem.OriginalData); // Add to BindingSource
                                        originalList.Add(wrappedItem);
                                        tracking.EntityState = EntityState.Unchanged;
                                        wrappedItem.RowState = DataRowState.Unchanged;
                                    }
                                    break;

                                case DataRowState.Modified:
                                    if (tracking.EntityState == EntityState.Modified && ChangedValues.ContainsKey(wrappedItem))
                                    {
                                        int originalIndex = originalList.IndexOf(wrappedItem);
                                        var originalWrapped = originalList[originalIndex] as DataRowWrapper;
                                      //  //MiscFunctions.SendLog($"Saving Modified Record - RowID: {wrappedItem.RowID}, OriginalIndex: {originalIndex}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
                                        // Modifications already applied to OriginalData (referenced by BindingSource)
                                        // Example: UpdateInDataSource(originalWrapped?.OriginalData, ChangedValues[wrappedItem]);
                                        tracking.EntityState = EntityState.Unchanged;
                                        wrappedItem.RowState = DataRowState.Unchanged;
                                        ChangedValues.Remove(wrappedItem);
                                    }
                                    break;
                            }
                        }
                    }

                    // Process deleted records
                    for (int i = 0; i < deletedList.Count; i++)
                    {
                        var wrappedItem = deletedList[i] as DataRowWrapper;
                        if (wrappedItem != null && wrappedItem.RowState == DataRowState.Deleted)
                        {
                            Tracking tracking = GetTrackingItem(wrappedItem);
                            if (tracking != null && tracking.EntityState == EntityState.Deleted)
                            {
                             //   //MiscFunctions.SendLog($"Saving Deleted Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
                                int sourceIndex = bindingSource.IndexOf(wrappedItem.OriginalData);
                                if (sourceIndex >= 0)
                                {
                                    bindingSource.RemoveAt(sourceIndex); // Remove from BindingSource
                                }
                                originalList.Remove(wrappedItem);
                                Trackings.Remove(tracking);
                                deletedList.RemoveAt(i);
                                i--; // Adjust index after removal
                            }
                        }
                    }

                    // Notify BindingSource of changes
                    bindingSource.ResetBindings(false);
                }
                else if (DataSource is IList list)
                {
                    // Scenario 2: DataSource is IList (propagate to finalData)
                    IList dataSourceList = list;

                    // Process added and modified records using Trackings
                    foreach (var tracking in Trackings.Where(t => t.EntityState == EntityState.Added || t.EntityState == EntityState.Modified))
                    {
                        var wrappedItem = _fullData[tracking.CurrentIndex] as DataRowWrapper;
                        if (wrappedItem != null)
                        {
                            switch (wrappedItem.RowState)
                            {
                                case DataRowState.Added:
                                    if (tracking.EntityState == EntityState.Added)
                                    {
                                 //       //MiscFunctions.SendLog($"Saving Added Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
                                        dataSourceList.Add(wrappedItem.OriginalData);
                                        originalList.Add(wrappedItem);
                                        tracking.EntityState = EntityState.Unchanged;
                                        wrappedItem.RowState = DataRowState.Unchanged;
                                    }
                                    break;

                                case DataRowState.Modified:
                                    if (tracking.EntityState == EntityState.Modified && ChangedValues.ContainsKey(wrappedItem))
                                    {
                                        int originalIndex = originalList.IndexOf(wrappedItem);
                                        var originalWrapped = originalList[originalIndex] as DataRowWrapper;
                                 //      //MiscFunctions.SendLog($"Saving Modified Record - RowID: {wrappedItem.RowID}, OriginalIndex: {originalIndex}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
                                        // Modifications already applied to OriginalData
                                        // Example: UpdateInDataSource(originalWrapped?.OriginalData, ChangedValues[wrappedItem]);
                                        tracking.EntityState = EntityState.Unchanged;
                                        wrappedItem.RowState = DataRowState.Unchanged;
                                        ChangedValues.Remove(wrappedItem);
                                    }
                                    break;
                            }
                        }
                    }

                    // Process deleted records
                    for (int i = 0; i < deletedList.Count; i++)
                    {
                        var wrappedItem = deletedList[i] as DataRowWrapper;
                        if (wrappedItem != null && wrappedItem.RowState == DataRowState.Deleted)
                        {
                            Tracking tracking = GetTrackingItem(wrappedItem);
                            if (tracking != null && tracking.EntityState == EntityState.Deleted)
                            {
                          //      //MiscFunctions.SendLog($"Saving Deleted Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
                                int sourceIndex = -1;
                                for (int j = 0; j < dataSourceList.Count; j++)
                                {
                                    if (ReferenceEquals(dataSourceList[j], wrappedItem.OriginalData))
                                    {
                                        sourceIndex = j;
                                        break;
                                    }
                                }
                                if (sourceIndex >= 0)
                                {
                                    dataSourceList.RemoveAt(sourceIndex);
                                }
                                originalList.Remove(wrappedItem);
                                Trackings.Remove(tracking);
                                deletedList.RemoveAt(i);
                                i--; // Adjust index after removal
                            }
                        }
                    }

                    // For IList, rely on its notification mechanism (if any) or manual grid update
                    FillVisibleRows();
                }
                else
                {
                    //MiscFunctions.SendLog("SaveToDataSource: DataSource is neither BindingSource nor IList. Cannot sync changes.");
                    return;
                }

                // Update the grid state
                UpdateScrollBars();
                Invalidate();

                //MiscFunctions.SendLog("SaveToDataSource: All changes processed and synchronized with DataSource.");
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"SaveToDataSource Error: {ex.Message}");
                //MiscFunctions.SendLog($"Error saving to data source: {ex.Message}");
            }
        }
        #endregion DataSource Update
        #region Initialization
        #region Column Setup
        private void EnsureDefaultColumns()
        {
            if (_columns == null)
                _columns = new List<BeepColumnConfig>();

            // Check if Sel column exists
            if (!_columns.Any(c => c.ColumnName == "Sel"))
            {
                var selColumn = new BeepColumnConfig
                {
                    ColumnCaption = "☑",
                    ColumnName = "Sel",
                    Width = _selectionColumnWidth > 0 ? _selectionColumnWidth : 30,
                    Index = 0,
                    Visible = true,
                    Sticked = true,
                    IsUnbound = true,
                    IsSelectionCheckBox = true,
                    PropertyTypeName = typeof(bool).AssemblyQualifiedName,
                    CellEditor = BeepColumnType.CheckBoxBool,
                    GuidID = Guid.NewGuid().ToString(),
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    Resizable = DataGridViewTriState.False,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                };
                selColumn.ColumnType = MapPropertyTypeToDbFieldCategory(selColumn.PropertyTypeName);
                _columns.Add(selColumn);
            }

            // Check if RowNum column exists
            if (!_columns.Any(c => c.ColumnName == "RowNum"))
            {
                var rowNumColumn = new BeepColumnConfig
                {
                    ColumnCaption = "#",
                    ColumnName = "RowNum",
                    Width = 30,
                    Index = 1,
                    Visible = true,
                    Sticked = true,
                    ReadOnly = true,
                    IsRowNumColumn = true,
                    IsUnbound = true,
                    PropertyTypeName = typeof(int).AssemblyQualifiedName,
                    CellEditor = BeepColumnType.Text,
                    GuidID = Guid.NewGuid().ToString(),
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    Resizable = DataGridViewTriState.False,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    AggregationType = AggregationType.Count
                };
                rowNumColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowNumColumn.PropertyTypeName);
                _columns.Add(rowNumColumn);
            }

            // Check if RowID column exists
            if (!_columns.Any(c => c.ColumnName == "RowID"))
            {
                var rowIdColumn = new BeepColumnConfig
                {
                    ColumnCaption = "RowID",
                    ColumnName = "RowID",
                    Width = 30,
                    Index = 2,
                    Visible = false,
                    Sticked = true,
                    ReadOnly = true,
                    IsRowID = true,
                    IsUnbound = true,
                    PropertyTypeName = typeof(int).AssemblyQualifiedName,
                    CellEditor = BeepColumnType.Text,
                    GuidID = Guid.NewGuid().ToString(),
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    Resizable = DataGridViewTriState.False,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                };
                rowIdColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowIdColumn.PropertyTypeName);
                _columns.Add(rowIdColumn);
            }
        }
        private Tuple<object, EntityStructure> SetupBindingSource()
        {
            object resolvedData = null;
            EntityStructure entity = null;

            try
            {
                if (_bindingSource == null)
                {
                    //MiscFunctions.SendLog("SetupBindingSource: _bindingSource is null");
                    return Tuple.Create(resolvedData, entity);
                }

                object dataSourceInstance = _bindingSource.DataSource;
                Type dataSourceType = dataSourceInstance as Type ?? dataSourceInstance?.GetType();
                Type itemType = null;

                if (dataSourceType == null)
                {
                    //MiscFunctions.SendLog("SetupBindingSource: No DataSource Type found");
                    return Tuple.Create(resolvedData, entity);
                }

                if (!string.IsNullOrEmpty(_bindingSource.DataMember))
                {
                    // DataMember is provided
                    //MiscFunctions.SendLog($"SetupBindingSource: DataMember = {_bindingSource.DataMember}");

                    // If DataSource is a TYPE (e.g., typeof(Company)), resolve the property type
                    PropertyInfo memberProp = dataSourceType.GetProperty(
                        _bindingSource.DataMember,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    if (memberProp != null)
                    {
                        Type memberType = memberProp.PropertyType;

                        if (typeof(IEnumerable).IsAssignableFrom(memberType) && memberType != typeof(string))
                        {
                            // It's a collection (List<T> or Array)
                            itemType = GetItemTypeFromDataMember(memberType);
                        }
                        else
                        {
                            // Single object
                            itemType = memberType;
                        }

                        // If instance, try to get actual value
                        if (!(dataSourceInstance is Type))
                        {
                            resolvedData = memberProp.GetValue(dataSourceInstance);
                        }
                    }
                    else
                    {
                        //MiscFunctions.SendLog($"SetupBindingSource: DataMember '{_bindingSource.DataMember}' not found in type {dataSourceType.FullName}");
                    }
                }
                else
                {
                    // No DataMember provided
                    //MiscFunctions.SendLog($"SetupBindingSource: No DataMember, resolving directly from {dataSourceType.FullName}");

                    if (typeof(IEnumerable).IsAssignableFrom(dataSourceType) && dataSourceType != typeof(string))
                    {
                        itemType = GetItemTypeFromDataMember(dataSourceType);

                        if (!(dataSourceInstance is Type))
                        {
                            resolvedData = _bindingSource.List ?? dataSourceInstance;
                        }
                    }
                    else
                    {
                        // It's a single object
                        itemType = dataSourceType;
                        resolvedData = dataSourceInstance;
                    }
                }

                // Now create EntityStructure based on type
                if (itemType != null)
                {
                    _entityType = itemType;
                    entity = EntityHelper.GetEntityStructureFromType(itemType);
                    //MiscFunctions.SendLog($"SetupBindingSource: Entity built from itemType {itemType.FullName}");
                }
                else if (resolvedData != null)
                {
                    // fallback: create from actual data list
                    entity = EntityHelper.GetEntityStructureFromListorTable(resolvedData);
                    if (entity != null && !string.IsNullOrEmpty(entity.EntityName))
                    {
                        _entityType = Type.GetType(entity.EntityName);
                    }
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"SetupBindingSource: Error {ex.Message}");
            }

            return Tuple.Create(resolvedData, entity);
        }
        public void CreateColumnsForEntity()
        {
            try
            {
                if (Entity == null || Entity.Fields == null)
                {
                    //MiscFunctions.SendLog("CreateColumnsForEntity: Entity or Entity.Fields is null");
                    return;
                }

                // Initialize columns collection if null
                if (_columns == null)
                    _columns = new List<BeepColumnConfig>();

                // Ensure we have the system columns
                EnsureSystemColumns();

                // Get the starting index for data columns (after system columns)
                int startIndex = _columns.Count(c => c.IsSelectionCheckBox || c.IsRowNumColumn || c.IsRowID);

                // Create a dictionary of existing columns for quick lookup
                Dictionary<string, BeepColumnConfig> existingColumns = _columns
                    .Where(c => c.ColumnName != null && !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID)
                    .ToDictionary(c => c.ColumnName, StringComparer.OrdinalIgnoreCase);

                // Track which entity fields we've processed
                HashSet<string> processedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // Update existing columns or add new ones for entity fields
                foreach (var field in Entity.Fields)
                {
                    if (string.IsNullOrEmpty(field.fieldname))
                        continue;

                    processedFields.Add(field.fieldname);

                    if (existingColumns.TryGetValue(field.fieldname, out BeepColumnConfig existingColumn))
                    {
                        // Update properties of existing column
                        existingColumn.PropertyTypeName = field.fieldtype;
                        existingColumn.ColumnType = MapPropertyTypeToDbFieldCategory(field.fieldtype);
                        existingColumn.CellEditor = MapPropertyTypeToCellEditor(field.fieldtype);

                        // Only update display properties if they aren't customized
                        if (string.IsNullOrEmpty(existingColumn.ColumnCaption) ||
                            existingColumn.ColumnCaption == existingColumn.ColumnName)
                        {
                            existingColumn.ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(field.fieldname);
                        }

                        // Update format for date/time fields if not already set
                        if (string.IsNullOrEmpty(existingColumn.Format))
                        {
                            Type fieldType = Type.GetType(field.fieldtype, throwOnError: false) ?? typeof(object);
                            if (fieldType == typeof(DateTime))
                            {
                                existingColumn.Format = "g";
                            }
                        }

                        // Update enum items if applicable
                        Type fieldType2 = Type.GetType(field.fieldtype, throwOnError: false);
                        if (fieldType2?.IsEnum == true && (existingColumn.Items == null || !existingColumn.Items.Any()))
                        {
                            existingColumn.Items = CreateEnumItems(fieldType2);
                        }

                        //MiscFunctions.SendLog($"CreateColumnsForEntity: Updated existing column {field.fieldname}");
                    }
                    else
                    {
                        // Create a new column for this field
                        var colConfig = new BeepColumnConfig
                        {
                            ColumnName = field.fieldname,
                            ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(field.fieldname),
                            PropertyTypeName = field.fieldtype,
                            GuidID = Guid.NewGuid().ToString(),
                            Visible = true,
                            Width = 100,
                            SortMode = DataGridViewColumnSortMode.Automatic,
                            Resizable = DataGridViewTriState.True,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                            Index = startIndex++
                        };

                        colConfig.ColumnType = MapPropertyTypeToDbFieldCategory(colConfig.PropertyTypeName);
                        colConfig.CellEditor = MapPropertyTypeToCellEditor(colConfig.PropertyTypeName);

                        Type fieldType = Type.GetType(colConfig.PropertyTypeName, throwOnError: false);
                        if (fieldType == typeof(DateTime))
                        {
                            colConfig.Format = "g";
                        }
                        else if (fieldType?.IsEnum == true)
                        {
                            colConfig.Items = CreateEnumItems(fieldType);
                        }

                        _columns.Add(colConfig);
                        //MiscFunctions.SendLog($"CreateColumnsForEntity: Added new column {field.fieldname}");
                    }
                }

                // Reindex columns to ensure proper ordering
                for (int i = 0; i < _columns.Count; i++)
                {
                    _columns[i].Index = i;
                }

                //MiscFunctions.SendLog($"CreateColumnsForEntity: Processed {processedFields.Count} fields, total columns: {_columns.Count}");
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error in CreateColumnsForEntity: {ex.Message}");
            }
        }

        private void CreateColumnsFromType(Type entityType)
        {
            try
            {
                if (entityType == null)
                {
                    //MiscFunctions.SendLog("CreateColumnsFromType: Entity type is null");
                    return;
                }

                //MiscFunctions.SendLog($"Creating columns from Type: {entityType.FullName}");

                // Initialize columns collection if null
                if (_columns == null)
                    _columns = new List<BeepColumnConfig>();

                // Ensure we have the system columns
                EnsureSystemColumns();

                // Get the starting index for data columns (after system columns)
                int startIndex = _columns.Count(c => c.IsSelectionCheckBox || c.IsRowNumColumn || c.IsRowID);

                // Create a dictionary of existing columns for quick lookup
                Dictionary<string, BeepColumnConfig> existingColumns = _columns
                    .Where(c => c.ColumnName != null && !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID)
                    .ToDictionary(c => c.ColumnName, StringComparer.OrdinalIgnoreCase);

                // Track which properties we've processed
                HashSet<string> processedProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // Update existing columns or add new ones for entity properties
                foreach (var prop in entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    // Skip indexers and other non-data properties
                    if (prop.GetIndexParameters().Length > 0) continue;

                    string propertyName = prop.Name;
                    processedProperties.Add(propertyName);
                    string propertyTypeName = prop.PropertyType.AssemblyQualifiedName;

                    if (existingColumns.TryGetValue(propertyName, out BeepColumnConfig existingColumn))
                    {
                        // Update properties of existing column
                        existingColumn.PropertyTypeName = propertyTypeName;
                        existingColumn.ColumnType = MapPropertyTypeToDbFieldCategory(propertyTypeName);
                        existingColumn.CellEditor = MapPropertyTypeToCellEditor(propertyTypeName);

                        // Only update display properties if they aren't customized
                        if (string.IsNullOrEmpty(existingColumn.ColumnCaption) ||
                            existingColumn.ColumnCaption == existingColumn.ColumnName)
                        {
                            existingColumn.ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(propertyName);
                        }

                        // Update format for date/time fields if not already set
                        if (string.IsNullOrEmpty(existingColumn.Format) && prop.PropertyType == typeof(DateTime))
                        {
                            existingColumn.Format = "g";
                        }

                        // Update enum items if applicable
                        if (prop.PropertyType.IsEnum && (existingColumn.Items == null || !existingColumn.Items.Any()))
                        {
                            existingColumn.Items = CreateEnumItems(prop.PropertyType);
                        }

                        //MiscFunctions.SendLog($"CreateColumnsFromType: Updated existing column {propertyName}");
                    }
                    else
                    {
                        // Create a new column for this property
                        var colConfig = new BeepColumnConfig
                        {
                            ColumnName = propertyName,
                            ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(propertyName),
                            PropertyTypeName = propertyTypeName,
                            GuidID = Guid.NewGuid().ToString(),
                            Visible = true,
                            Width = 100,
                            SortMode = DataGridViewColumnSortMode.Automatic,
                            Resizable = DataGridViewTriState.True,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                            Index = startIndex++
                        };

                        colConfig.ColumnType = MapPropertyTypeToDbFieldCategory(colConfig.PropertyTypeName);
                        colConfig.CellEditor = MapPropertyTypeToCellEditor(colConfig.PropertyTypeName);

                        // Set format for date/time columns
                        if (prop.PropertyType == typeof(DateTime))
                        {
                            colConfig.Format = "g";
                        }

                        // Handle enum properties
                        if (prop.PropertyType.IsEnum)
                        {
                            colConfig.Items = CreateEnumItems(prop.PropertyType);
                        }

                        _columns.Add(colConfig);
                        //MiscFunctions.SendLog($"CreateColumnsFromType: Added new column {propertyName}");
                    }
                }

                // Reindex columns to ensure proper ordering
                for (int i = 0; i < _columns.Count; i++)
                {
                    _columns[i].Index = i;
                }

                //MiscFunctions.SendLog($"CreateColumnsFromType: Processed {processedProperties.Count} properties, total columns: {_columns.Count}");
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error creating columns from Type {entityType?.FullName}: {ex.Message}");
                // Don't call EnsureDefaultColumns() here to preserve existing columns on error
            }
        }

        // Helper method to ensure system columns exist
        private void EnsureSystemColumns()
        {
            // Add selection checkbox column if missing
            if (!_columns.Any(c => c.IsSelectionCheckBox))
            {
                var selColumn = new BeepColumnConfig
                {
                    ColumnCaption = "☑",
                    ColumnName = "Sel",
                    Width = _selectionColumnWidth,
                    Index = 0,
                    Visible = ShowCheckboxes,
                    Sticked = true,
                    IsUnbound = true,
                    IsSelectionCheckBox = true,
                    PropertyTypeName = typeof(bool).AssemblyQualifiedName,
                    CellEditor = BeepColumnType.CheckBoxBool,
                    GuidID = Guid.NewGuid().ToString(),
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    Resizable = DataGridViewTriState.False,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                };
                selColumn.ColumnType = MapPropertyTypeToDbFieldCategory(selColumn.PropertyTypeName);
                _columns.Add(selColumn);
            }

            // Add row number column if missing
            if (!_columns.Any(c => c.IsRowNumColumn))
            {
                var rowNumColumn = new BeepColumnConfig
                {
                    ColumnCaption = "#",
                    ColumnName = "RowNum",
                    Width = 30,
                    Index = _columns.Count(c => c.IsSelectionCheckBox),
                    Visible = ShowRowNumbers,
                    Sticked = true,
                    ReadOnly = true,
                    IsRowNumColumn = true,
                    IsUnbound = true,
                    PropertyTypeName = typeof(int).AssemblyQualifiedName,
                    CellEditor = BeepColumnType.Text,
                    GuidID = Guid.NewGuid().ToString(),
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    Resizable = DataGridViewTriState.False,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    AggregationType = AggregationType.Count
                };
                rowNumColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowNumColumn.PropertyTypeName);
                _columns.Add(rowNumColumn);
            }

            // Add RowID column if missing
            if (!_columns.Any(c => c.IsRowID))
            {
                var rowIdColumn = new BeepColumnConfig
                {
                    ColumnCaption = "RowID",
                    ColumnName = "RowID",
                    Width = 30,
                    Index = _columns.Count(c => c.IsSelectionCheckBox || c.IsRowNumColumn),
                    Visible = false,
                    Sticked = true,
                    ReadOnly = true,
                    IsRowID = true,
                    IsUnbound = true,
                    PropertyTypeName = typeof(int).AssemblyQualifiedName,
                    CellEditor = BeepColumnType.Text,
                    GuidID = Guid.NewGuid().ToString(),
                    SortMode = DataGridViewColumnSortMode.NotSortable,
                    Resizable = DataGridViewTriState.False,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                };
                rowIdColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowIdColumn.PropertyTypeName);
                _columns.Add(rowIdColumn);
            }
        }

        // Helper method to create enum items list
        private List<SimpleItem> CreateEnumItems(Type enumType)
        {
            var items = new List<SimpleItem>();
            if (enumType != null && enumType.IsEnum)
            {
                foreach (var val in Enum.GetValues(enumType))
                {
                    items.Add(new SimpleItem
                    {
                        Value = val,
                        Text = val.ToString(),
                        DisplayField = val.ToString()
                    });
                }
            }
            return items;
        }

        private object GetCollectionPropertyFromInstance(object instance)
        {
            if (instance == null) return null;

            var properties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo collectionProp = null;
            int collectionCount = 0;

            foreach (var prop in properties)
            {
                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    var value = prop.GetValue(instance);
                    if (value != null)
                    {
                        collectionProp = prop;
                        collectionCount++;
                        //   //MiscFunctions.SendLog($"Found collection property: {prop.Name}, Type = {prop.PropertyType}");
                    }
                }
            }

            if (collectionCount == 1 && collectionProp != null)
            {
                return collectionProp.GetValue(instance);
            }
            else if (collectionCount > 1)
            {
                //   //MiscFunctions.SendLog("Multiple collection properties found, please specify DataMember");
            }
            return null; // No single collection found or all are null
        }
        private Type GetItemTypeFromDataMember(Type propertyType)
        {
            // //MiscFunctions.SendLog($"GetItemTypeFromDataMember: PropertyType = {propertyType.FullName}");

            // Handle generic IEnumerable<T> (e.g., List<T>)
            if (propertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                Type[] genericArgs = propertyType.GetGenericArguments();
                if (genericArgs.Length > 0)
                {
                    //   //MiscFunctions.SendLog($"Extracted item type: {genericArgs[0].FullName}");
                    return genericArgs[0];
                }
            }

            // Handle DataTable
            if (propertyType == typeof(DataTable))
            {
                //  //MiscFunctions.SendLog("DataTable detected, returning DataRow");
                return typeof(DataRow);
            }

            // Handle arrays
            if (propertyType.IsArray)
            {
                //   //MiscFunctions.SendLog($"Array detected, element type: {propertyType.GetElementType().FullName}");
                return propertyType.GetElementType();
            }

            //  //MiscFunctions.SendLog("No item type extracted");
            return null;
        }


        #endregion Column Setup
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (_isInitializing)
            {
                // Existing initialization code...
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

                    // Add default row ID and selection columns
                    EnsureDefaultColumns();

                    if (_pendingDataSource != null)
                    {
                        _dataSource = _pendingDataSource;
                        DataSetup();
                        InitializeData();
                        FillVisibleRows();
                        UpdateScrollBars();
                        Invalidate();
                    }
                    else
                    {
                        // Initialize with empty data when dragged from toolbox
                        InitializeRows();
                        UpdateScrollBars();
                        Invalidate();
                    }
                    _isInitializing = false; // Initialization complete
                }
                catch (Exception ex)
                {
                    // Initialize with minimal UI to avoid crashing
                    _isInitializing = false;
                    Invalidate();
                }
            }
            else
            {
                // If not initializing, just update scrollbars
                UpdateScrollBars();
            }
        }
        // Add this method to ensure required columns are always present


        // Make DataSetup more resilient
        private void DataSetup()
        {
            try
            {
                //if (DesignMode)
                //{
                //    // Create minimal columns for design-time display
                //    if (_columns == null || _columns.Count == 0)
                //        EnsureDefaultColumns();
                //    return;
                //}

                EntityStructure entity = null;
                object resolvedData = null;

                //MiscFunctions.SendLog($"DataSetup Started: _dataSource Type = {_dataSource?.GetType()}, DesignMode = {DesignMode}");

                // Create default binding source if null
                if (_bindingSource == null)
                {
                    _bindingSource = new BindingSource();
                }

                // Process datasource
                if (_dataSource == null)
                {
                    // Create empty binding source and minimal structure
                    finalData = _bindingSource;
                }
                else if (_dataSource is BindingSource bindingSrc)
                {
                    AssignBindingSource(bindingSrc);
                    var ret = SetupBindingSource();
                    resolvedData = ret.Item1;
                    entity = ret.Item2;
                    finalData = _bindingSource;
                }
                else if (_dataSource is Type typeDataSource)
                {
                    // Direct Type as data source - important for empty collections
                    _entityType = typeDataSource;
                    _bindingSource.DataSource = _dataSource;
                    entity = EntityHelper.GetEntityStructureFromType(typeDataSource);
                    finalData = _bindingSource;
                    //MiscFunctions.SendLog($"DataSetup: DataSource is Type = {typeDataSource.FullName}, created entity structure");
                }
                else
                {
                    // Handle basic data types and wrap with binding source
                    _bindingSource.DataSource = _dataSource;
                    resolvedData = _dataSource;
                    finalData = _bindingSource;

                    // Try to determine _entityType from _dataSource even if empty
                    if (_entityType == null && _dataSource != null)
                    {
                        Type dataSourceType = _dataSource.GetType();

                        // Handle IList<T>, IEnumerable<T>, T[], etc.
                        if (dataSourceType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(dataSourceType))
                        {
                            Type[] genericArgs = dataSourceType.GetGenericArguments();
                            if (genericArgs.Length > 0)
                            {
                                _entityType = genericArgs[0];
                                //MiscFunctions.SendLog($"Determined _entityType from datasource generic arguments: {_entityType.FullName}");
                            }
                        }
                        else if (dataSourceType.IsArray)
                        {
                            _entityType = dataSourceType.GetElementType();
                            //MiscFunctions.SendLog($"Determined _entityType from datasource array element type: {_entityType.FullName}");
                        }
                    }
                }

                // Ensure _columns is initialized
                //_columns = new List<BeepColumnConfig>();

                // Attempt to create entity structure if possible
                if (entity == null && _entityType != null)
                {
                    entity = EntityHelper.GetEntityStructureFromType(_entityType);
                    //MiscFunctions.SendLog($"Created entity structure from _entityType: {_entityType.FullName}");
                }
                else if (entity == null && resolvedData != null)
                {
                    entity = EntityHelper.GetEntityStructureFromListorTable(resolvedData);
                }

                // Process entity and columns
                if (entity != null)
                {
                    Entity = entity;
                    if (!_columns.Any() || !columnssetupusingeditordontchange)
                    {
                        CreateColumnsForEntity();
                    }
                }
                else if (_entityType != null)
                {
                    // Create columns directly from _entityType when entity is null
                    //MiscFunctions.SendLog($"Creating columns from _entityType: {_entityType.FullName}");
                    CreateColumnsFromType(_entityType);
                }
                else
                {
                    // Ensure we have at least default columns
                    EnsureDefaultColumns();
                }

                //MiscFunctions.SendLog($"DataSetup Completed: finalData = {finalData?.GetType()}, Entity = {Entity?.EntityName}, _entityType = {_entityType?.FullName}, Columns Count = {_columns.Count}");
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Error in DataSetup: {ex.Message}");
                EnsureDefaultColumns();
            }
        }
  
        // Make InitializeRows more resilient
        private void InitializeRows()
        {
            if (Rows == null) Rows = new BindingList<BeepRowConfig>();
            Rows.Clear();
            if(_fullData == null || !_fullData.Any())
            {
                //MiscFunctions.SendLog("InitializeRows: _fullData is null or empty, skipping row initialization");
                return;
            }
            //// Ensure we have valid columns
            //if (_columns == null || !_columns.Any())
            //{
            //    EnsureDefaultColumns();
            //}

            // Create at least one empty row to show something when dragged from toolbox
            int displayRows = Math.Max(1, Math.Min((_fullData?.Count ?? 0),
                DrawingRect.Height / (_rowHeight > 0 ? _rowHeight : 25)));

            for (int i = 0; i < displayRows; i++)
            {
                var row = new BeepRowConfig();
                foreach (var col in Columns)
                {
                    // Create a new cell
                    var cell = new BeepCellConfig
                    {
                        CellValue = null,
                        CellData = null,
                        IsEditable = !col.ReadOnly,
                        ColumnIndex = col.Index,
                        IsVisible = col.Visible,
                        RowIndex = i
                    };
                    row.Cells.Add(cell);
                }

                row.RowIndex = i;
                row.DisplayIndex = i;
                row.Height = _rowHeight > 0 ? _rowHeight : 25;
                Rows.Add(row);
            }

            // Initialize aggregation row if needed
            aggregationRow = new BeepRowConfig
            {
                RowIndex = Rows.Count,
                DisplayIndex = -1,
                IsAggregation = true,
                Height = _rowHeight > 0 ? _rowHeight : 25
            };

            foreach (var col in Columns)
            {
                var cell = new BeepCellConfig
                {
                    CellValue = null,
                    CellData = null,
                    IsEditable = false,
                    ColumnIndex = col.Index,
                    IsVisible = col.Visible,
                    RowIndex = Rows.Count,
                    IsAggregation = true
                };
                aggregationRow.Cells.Add(cell);
            }

            UpdateScrollBars();
        }
        private void InitializeData()
        {
            if (_bindingSource != null)
            {
                SyncFullDataFromBindingSource(); // Initial sync from BindingSource
            }
            else
            {
                // Fallback (shouldn’t occur with updated DataSetup, but kept for robustness)
                _fullData = finalData is IEnumerable<object> enumerable ? enumerable.ToList() : new List<object>();
                WrapFullData();
            }
            _dataOffset = 0;
            // Ensure originalList is populated with the initial wrapped data
            if (originalList == null)
            {
                originalList = new List<object>();
            }
            originalList.Clear();
            originalList.AddRange(_fullData); // Copy the initial wrapped data to originalList
            // Determine _entityType if not already set (moved from DataSetup for consistency)
            if (_fullData != null && _fullData.Count > 0 && _entityType == null)
            {
                var firstItem = _fullData.First() as DataRowWrapper;
                if (firstItem != null)
                {
                    _entityType = firstItem.OriginalData.GetType();
                    //MiscFunctions.SendLog($"InitializeData: Determined _entityType from first item in _fullData: {_entityType.FullName}");
                }
            }
             InitializeColumnsAndTracking();
            InitializeRows();
            UpdateScrollBars();
            // fill column in filterColumnComboBox
            filterColumnComboBox.ListItems.Clear();
            filterColumnComboBox.ListItems.Add(new SimpleItem { Text = "All Columns", Value = null });
            foreach (var col in Columns)
            {
                if (col.Visible && !col.IsSelectionCheckBox & !col.IsRowNumColumn)
                    filterColumnComboBox.ListItems.Add(new SimpleItem { DisplayField=  col.ColumnCaption ?? col.ColumnName, Text = col.ColumnName ?? col.ColumnCaption , Value = col.ColumnName });
            }
            if (DataNavigator != null) DataNavigator.DataSource = _fullData;
        }
        private void WrapFullData()
        {
            var wrappedData = new List<object>();
            for (int i = 0; i < _fullData.Count; i++)
            {
                var wrapper = new DataRowWrapper(_fullData[i], i)
                {
                    TrackingUniqueId = Guid.NewGuid(),
                    RowState = DataRowState.Unchanged,
                    DateTimeChange = DateTime.Now
                };
                wrappedData.Add(wrapper);
            }
            _fullData = wrappedData;
        }
        private void SyncFullDataFromBindingSource()
        {
            _fullData = new List<object>();
            var enumerator = _bindingSource.GetEnumerator();
            int i = 0;
            bool isFirstItem = true;

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item == null) continue;

                // Set _entityType based on the first valid item
                if (isFirstItem && _entityType==null)
                {
                    _entityType = item.GetType();
                    //MiscFunctions.SendLog($"SyncFullDataFromBindingSource: Set _entityType to {_entityType.FullName}");
                    isFirstItem = false;
                }

                var wrapper = new DataRowWrapper(item, i++)
                {
                    TrackingUniqueId = Guid.NewGuid(),
                    RowState = DataRowState.Unchanged,
                    DateTimeChange = DateTime.Now
                };
                _fullData.Add(wrapper);
            }

            if (_entityType == null)
            {
                //MiscFunctions.SendLog("SyncFullDataFromBindingSource: No valid items found to set _entityType.");
            }
        }
        private void InitializeColumnsAndTracking()
        {
            if (_fullData == null || !_fullData.Any())
                return;
            // Ensure column list is initialized
            if (_columns == null)
                _columns = new List<BeepColumnConfig>();

            if (_fullData?.Any() == true)
            {
                var firstItem = _fullData.First() as DataRowWrapper;
                if (firstItem?.OriginalData != null)
                {
                    _entityType = firstItem.OriginalData.GetType();
                    var properties = _entityType.GetProperties();

                    int currentIndex = 0;

                    // Add Sel column if missing
                    if (!_columns.Any(c => c.ColumnName == "Sel"))
                    {
                        var selColumn = new BeepColumnConfig
                        {
                            ColumnCaption = "☑",
                            ColumnName = "Sel",
                            Width = _selectionColumnWidth,
                            Index = currentIndex++,
                            Visible = ShowCheckboxes,
                            Sticked = true,
                            IsUnbound = true,
                            IsSelectionCheckBox = true,
                            PropertyTypeName = typeof(bool).AssemblyQualifiedName,
                            CellEditor = BeepColumnType.CheckBoxBool,
                            GuidID = Guid.NewGuid().ToString(),
                            SortMode = DataGridViewColumnSortMode.NotSortable,
                            Resizable = DataGridViewTriState.False,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                            ColumnType = MapPropertyTypeToDbFieldCategory(typeof(bool).AssemblyQualifiedName)
                        };
                        _columns.Add(selColumn);
                    }

                    // Add RowNum column if missing
                    if (!_columns.Any(c => c.ColumnName == "RowNum"))
                    {
                        var rowNumColumn = new BeepColumnConfig
                        {
                            ColumnCaption = "#",
                            ColumnName = "RowNum",
                            Width = 30,
                            Index = currentIndex++,
                            Visible = ShowRowNumbers,
                            Sticked = true,
                            ReadOnly = true,
                            IsRowNumColumn = true,
                            IsUnbound = true,
                            PropertyTypeName = typeof(int).AssemblyQualifiedName,
                            CellEditor = BeepColumnType.Text,
                            GuidID = Guid.NewGuid().ToString(),
                            SortMode = DataGridViewColumnSortMode.NotSortable,
                            Resizable = DataGridViewTriState.False,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                            AggregationType = AggregationType.Count,
                            ColumnType = MapPropertyTypeToDbFieldCategory(typeof(int).AssemblyQualifiedName)
                        };
                        _columns.Add(rowNumColumn);
                    }

                    // Add RowID column if missing
                    if (!_columns.Any(c => c.ColumnName == "RowID"))
                    {
                        var rowIdColumn = new BeepColumnConfig
                        {
                            ColumnCaption = "RowID",
                            ColumnName = "RowID",
                            Width = 30,
                            Index = currentIndex++,
                            Visible = false,
                            Sticked = true,
                            ReadOnly = true,
                            IsRowID = true,
                            IsUnbound = true,
                            PropertyTypeName = typeof(int).AssemblyQualifiedName,
                            CellEditor = BeepColumnType.Text,
                            GuidID = Guid.NewGuid().ToString(),
                            SortMode = DataGridViewColumnSortMode.NotSortable,
                            Resizable = DataGridViewTriState.False,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                            ColumnType = MapPropertyTypeToDbFieldCategory(typeof(int).AssemblyQualifiedName)
                        };
                        _columns.Add(rowIdColumn);
                    }

                    // Add data-bound columns from entity properties (if not already present)
                    foreach (var prop in properties)
                    {
                        if (!_columns.Any(c => c.ColumnName == prop.Name))
                        {
                            string propertyTypeName = prop.PropertyType.AssemblyQualifiedName;
                            var columnConfig = new BeepColumnConfig
                            {
                                ColumnCaption = prop.Name,
                                ColumnName = prop.Name,
                                Width = 100,
                                Index = currentIndex++,
                                Visible = true,
                                PropertyTypeName = propertyTypeName,
                                GuidID = Guid.NewGuid().ToString(),
                                CellEditor = MapPropertyTypeToCellEditor(propertyTypeName),
                                ColumnType = MapPropertyTypeToDbFieldCategory(propertyTypeName)
                            };
                            _columns.Add(columnConfig);
                        }
                    }

                    // Reassign correct index to all columns
                    for (int i = 0; i < _columns.Count; i++)
                    {
                        _columns[i].Index = i;
                    }
                }
            }

            // Initialize tracking and snapshot
            originalList.Clear();
            originalList.AddRange(_fullData);
            Trackings.Clear();
            for (int i = 0; i < _fullData.Count; i++)
            {
                Trackings.Add(new Tracking(Guid.NewGuid(), i, i) { EntityState = EntityState.Unchanged });
            }

            // Populate filter combo box
            filterColumnComboBox.Items.Clear();
            filterColumnComboBox.Items.Add(new SimpleItem { Text = "All Columns", Value = null });
            foreach (var col in _columns)
            {
                if (col.Visible)
                    filterColumnComboBox.Items.Add(new SimpleItem { Text = col.ColumnCaption ?? col.ColumnName, Value = col.ColumnName });
            }
            filterColumnComboBox.SelectedIndex = 0;
        }

        //private void InitializeData()
        //{// Determine _entityType from finalData before wrapping


        //    // Wrap _fullData objects with RowID
        //    _fullData = finalData is IEnumerable<object> enumerable ? enumerable.ToList() : new List<object>();
        //    if (_fullData != null)
        //    {
        //        if (_fullData.Count > 0)
        //        {
        //            var firstItem1 = _fullData.First();
        //            if (firstItem1 != null && _entityType==null)
        //            {
        //                _entityType = firstItem1.GetType();
        //                //MiscFunctions.SendLog($"InitializeData: Determined _entityType from first item in finalData: {_entityType.FullName}");
        //            }
        //        }

        //    }
        //    var wrappedData = new List<object>();
        //    for (int i = 0; i < _fullData.Count; i++)
        //    {
        //        var wrapper = new DataRowWrapper(_fullData[i], i)
        //        {
        //            TrackingUniqueId = Guid.NewGuid(), // Set a unique identifier for tracking
        //            RowState = DataRowState.Unchanged, // Initial state
        //            DateTimeChange = DateTime.Now
        //        };
        //        wrappedData.Add(wrapper);
        //    }
        //    _fullData = wrappedData;
        //    _dataOffset = 0;


        //    if (_columns.Count == 0 && _fullData.Any())
        //    {
        //        var firstItem = _fullData.First() as DataRowWrapper;
        //        if (firstItem != null)
        //        {

        //            var originalData = firstItem.OriginalData;
        //            _entityType = originalData.GetType();
        //            var properties = originalData.GetType().GetProperties(); // Get properties of the original data object
        //            int index = 0;

        //            // Add checkbox/selection column if enabled

        //                var selColumn = new BeepColumnConfig
        //                {
        //                    ColumnCaption = "☑",
        //                    ColumnName = "Sel",
        //                    Width = _selectionColumnWidth,
        //                    RowIndex = index++,
        //                    Visible = true,
        //                    Sticked = true,
        //                    ReadOnly = false,
        //                    IsSelectionCheckBox = true,
        //                    PropertyTypeName = typeof(bool).AssemblyQualifiedName,
        //                    CellEditor = BeepColumnType.CheckBoxBool
        //                };
        //                selColumn.ColumnType = MapPropertyTypeToDbFieldCategory(selColumn.PropertyTypeName);
        //                _columns.Add(selColumn);


        //            // Add row number column if enabled

        //                var rowNumColumn = new BeepColumnConfig
        //                {
        //                    ColumnCaption = "#",
        //                    ColumnName = "RowNum",
        //                    Width = 30,
        //                    RowIndex = index++,
        //                    Visible = true,
        //                    Sticked = true,
        //                    ReadOnly = true,
        //                    IsRowNumColumn = true,
        //                    PropertyTypeName = typeof(int).AssemblyQualifiedName,
        //                    CellEditor = BeepColumnType.Text
        //                };
        //                rowNumColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowNumColumn.PropertyTypeName);
        //                _columns.Add(rowNumColumn);


        //            // Add RowID column (hidden, for internal use)
        //            var rowIDColumn = new BeepColumnConfig
        //            {
        //                ColumnCaption = "RowID",
        //                ColumnName = "RowID",
        //                Width = 0, // Hidden
        //                RowIndex = index++,
        //                Visible = false,
        //                Sticked = false,
        //                ReadOnly = true,
        //                IsRowID=true,
        //                PropertyTypeName = typeof(int).AssemblyQualifiedName,
        //                CellEditor = BeepColumnType.Text
        //            };
        //            rowIDColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowIDColumn.PropertyTypeName);
        //            _columns.Add(rowIDColumn);

        //            // Add data columns from properties of the original data object
        //            foreach (var prop in properties)
        //            {
        //                string propertyTypeName = prop.PropertyType.AssemblyQualifiedName;
        //                var columnConfig = new BeepColumnConfig
        //                {
        //                    ColumnCaption = prop.Name,
        //                    ColumnName = prop.Name,
        //                    Width = 100,
        //                    RowIndex = index++,
        //                    Visible = true,
        //                    PropertyTypeName = propertyTypeName
        //                };

        //                columnConfig.ColumnType = MapPropertyTypeToDbFieldCategory(propertyTypeName);
        //                columnConfig.CellEditor = MapPropertyTypeToCellEditor(propertyTypeName);

        //                _columns.Add(columnConfig);
        //            }
        //        }
        //    }

        //    originalList.Clear();
        //    originalList.AddRange(_fullData); // Snapshot initial data
        //    Trackings.Clear();
        //    for (int i = 0; i < _fullData.Count; i++)
        //    {
        //        Trackings.Add(new Tracking(Guid.NewGuid(), i, i) { EntityState = EntityState.Unchanged });
        //    }
        //    InitializeRows();
        //    UpdateScrollBars();

        //    // Populate columns in ComboBox (including "All Columns" option)
        //    filterColumnComboBox.Items.Add(new SimpleItem { Text = "All Columns", Value = null });
        //    foreach (var col in Columns)
        //    {
        //        if (col.Visible)
        //            filterColumnComboBox.Items.Add(new SimpleItem { Text = col.ColumnCaption ?? col.ColumnName, Value = col.ColumnName });
        //    }
        //    filterColumnComboBox.SelectedIndex = 0; // Default to "All Columns"

        //    // Attach _fullData to DataNavigator as an IList
        //    if (DataNavigator != null) DataNavigator.DataSource = _fullData;
        //}
        #endregion
        #region Size and Layout Management
        private void AutoResizeColumnsToFitContent()
        { 
            // want to resize all  columns its content
            if (Columns == null || Columns.Count == 0)
                return;
            // Calculate total width of all visible columns
            int totalWidth = Columns.Where(c => c.Visible).Sum(c => c.Width);
            int availableWidth = gridRect.Width - (ShowCheckboxes ? _selectionColumnWidth : 0) - (ShowRowNumbers ? 30 : 0);
            // If total width exceeds available width, resize columns proportionally
            if (totalWidth > availableWidth)
            {
                float scaleFactor = (float)availableWidth / totalWidth;
                foreach (var col in Columns.Where(c => c.Visible))
                {
                    col.Width = (int)(col.Width * scaleFactor);
                }
            }

           
        }
        #endregion
        #region Data Filling and Navigation
        public void RefreshGrid()
        {
            DataSetup();
            InitializeData();
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }
        private void InvalidateCell(BeepCellConfig cell)
        {
            if (cell?.Rect != Rectangle.Empty)
            {
                // Add padding for DPI scaling
                var invalidRect = cell.Rect;
                if (DpiScaleFactor > 1.0f)
                {
                    invalidRect.Inflate(1, 1);
                }
                Invalidate(invalidRect);
                Update(); // Force immediate update for better responsiveness
            }
        }
        private void InvalidateRow(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < Rows.Count)
            {
                var row = Rows[rowIndex];
                Rectangle rowRect = new Rectangle(gridRect.Left, row.UpperY, gridRect.Width, row.Height);
                if (DpiScaleFactor > 1.0f)
                {
                    rowRect.Inflate(0, 1);
                }
                Invalidate(rowRect);
                Update(); // Force immediate update
            }
        }
        private void FillVisibleRows()
        {
            // Prevent recursive calls
            if (_isUpdatingRows)
                return;

            // Debounce rapid calls
            var now = DateTime.Now;
            if ((now - _lastFillTime).TotalMilliseconds < FILL_DEBOUNCE_MS)
            {
                ScheduleFillVisibleRows();
                return;
            }

            _lastFillTime = now;
            _fillVisibleRowsPending = false;
            _isUpdatingRows = true;

            try
            {
                FillVisibleRowsCore();
            }
            finally
            {
                _isUpdatingRows = false;
            }
        }
        // Add a scheduler for deferred updates
        private void ScheduleFillVisibleRows()
        {
            if (_fillVisibleRowsPending)
                return;

            _fillVisibleRowsPending = true;

            if (_fillRowsTimer == null)
            {
                _fillRowsTimer = new Timer();
                _fillRowsTimer.Interval = FILL_DEBOUNCE_MS;
                _fillRowsTimer.Tick += (s, e) =>
                {
                    _fillRowsTimer.Stop();
                    if (_fillVisibleRowsPending)
                    {
                        FillVisibleRows();
                    }
                };
            }

            _fillRowsTimer.Stop();
            _fillRowsTimer.Start();
        }

        // Move the actual work to a separate method
        private void FillVisibleRowsCore()
        {
            if (_fullData == null || !_fullData.Any())
            {
                Rows.Clear();
                return;
            }

            int visibleRowCount = GetVisibleRowCount() == 1 ? _fullData.Count : GetVisibleRowCount();
            int startRow = Math.Max(0, _dataOffset);
            int endRow = Math.Min(_dataOffset + visibleRowCount, _fullData.Count);
            int requiredRows = endRow - startRow;

            // Only update if the visible range has actually changed
            if (_startviewrow == startRow && _endviewrow == endRow && Rows.Count == requiredRows)
            {
                // Just update cell values without recreating rows
                UpdateVisibleRowValues(startRow, endRow);
                return;
            }

            _startviewrow = startRow;
            _endviewrow = endRow;

            SuspendLayout();
            try
            {
                // Store previous selected row for targeted invalidation
                int previousSelectedDataIndex = _currentRow?.DisplayIndex ?? -1;

                // Clear and recreate rows
                Rows.Clear();
               
                for (int i = startRow; i < endRow; i++)
                {
                    int maxrowheight = 32;
                    
                    int dataIndex = i;
                    var row = new BeepRowConfig
                    {
                        RowIndex = i - _dataOffset,
                        DisplayIndex = dataIndex,
                        IsAggregation = false,
                        Height = _rowHeights.ContainsKey(dataIndex) ? _rowHeights[dataIndex] : _rowHeight,
                        OldDisplayIndex = dataIndex
                    };

                    var dataItem = _fullData[dataIndex] as DataRowWrapper;
                    if (dataItem != null)
                    {
                        EnsureTrackingForItem(dataItem);
                        row.IsDataLoaded = true;

                        for (int j = 0; j < Columns.Count; j++)
                        {
                            int maxcolumnwidth = 32;
                            var col = Columns[j];
                            var cell = new BeepCellConfig
                            {
                                RowIndex = i - _dataOffset,
                                ColumnIndex = col.Index,
                                IsVisible = col.Visible,
                                IsEditable = true,
                                IsAggregation = false,
                                Height = row.Height
                            };

                            if (col.IsSelectionCheckBox)
                            {
                                int rowID = dataItem.RowID;
                                bool isSelected = _persistentSelectedRows.ContainsKey(rowID) && _persistentSelectedRows[rowID];
                                cell.CellValue = isSelected;
                                cell.CellData = isSelected;
                            }
                            else if (col.IsRowNumColumn)
                            {
                                cell.CellValue = dataIndex + 1;
                                cell.CellData = dataIndex + 1;
                            }
                            else if (col.IsRowID)
                            {
                                cell.CellValue = dataItem.RowID;
                                cell.CellData = dataItem.RowID;
                            }
                            else
                            {
                                var prop = dataItem.OriginalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                                var value = prop?.GetValue(dataItem.OriginalData) ?? string.Empty;
                                cell.CellValue = value;
                                cell.CellData = value;
                            }
                            row.Cells.Add(cell);
                            if (AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.AllCells)
                            {

                                if (cell.CellValue != null)
                                {
                                    SizeF textSize = TextRenderer.MeasureText(cell.CellValue.ToString(), this.Font);
                                    maxrowheight = Math.Max(maxrowheight, (int)textSize.Height + 4); // Add padding
                                    maxcolumnwidth = Math.Max(maxcolumnwidth, (int)textSize.Width + 4); // Add padding
                                                                                                        // set row height and column width
                                    row.Height = Math.Max(maxrowheight, _rowHeight);

                                    if (cell.IsVisible && !col.IsSelectionCheckBox && !col.IsRowNumColumn && !col.IsRowID)
                                    {
                                        
                                        Columns[cell.ColumnIndex].Width = Math.Max(Columns[cell.ColumnIndex].Width, maxcolumnwidth);
                                    }
                                }
                            }
                          

                        
                    }
                    }
                    Rows.Add(row);

                      
                    
                }
                
                //// Update aggregationRow separately
                //if (_showaggregationRow && aggregationRow != null)
                //{
                //    for (int j = 0; j < aggregationRow.Cells.Count && j < Columns.Count; j++)
                //    {
                //        var col = Columns[j];
                //        var cell = aggregationRow.Cells[j];
                //        if (cell.IsAggregation)
                //        {
                //            object aggregatedValue = ComputeAggregation(col, _fullData);
                //            cell.CellValue = aggregatedValue?.ToString() ?? "";
                //            cell.CellData = aggregatedValue;
                //        }
                //    }
                //}

                //// Update _selectedRows and _selectedgridrows
                //var newSelectedRows = new List<int>();
                //var newSelectedGridRows = new List<BeepRowConfig>();
                //for (int i = 0; i < Rows.Count; i++)
                //{
                //    int dataIndex = _dataOffset + i;
                //    var dataItem = _fullData[dataIndex] as DataRowWrapper;
                //    if (dataItem != null)
                //    {
                //        int rowID = dataItem.RowID;
                //        if (_persistentSelectedRows.ContainsKey(rowID) && _persistentSelectedRows[rowID])
                //        {
                //            newSelectedRows.Add(i);
                //            newSelectedGridRows.Add(Rows[i]);
                //        }
                //    }
                //}
                //_selectedRows = newSelectedRows;
                //_selectedgridrows = newSelectedGridRows;

                UpdateTrackingIndices();
                SyncSelectedRowIndexAndEditor();
                UpdateCellPositions();
                UpdateScrollBars();
                UpdateRecordNumber();
                UpdateSelectionState();
                UpdateNavigationButtonState();
                PositionScrollBars();
            }
            finally
            {
                ResumeLayout(false);
            }
        }

        // Add method to update only values without recreating rows
        private void UpdateVisibleRowValues(int startRow, int endRow)
        {
            for (int i = 0; i < Rows.Count && i < (endRow - startRow); i++)
            {
                int dataIndex = startRow + i;
                if (dataIndex >= _fullData.Count)
                    break;

                var row = Rows[i];
                var dataItem = _fullData[dataIndex] as DataRowWrapper;
                if (dataItem == null)
                    continue;

                // Update only changed cells
                for (int j = 0; j < row.Cells.Count && j < Columns.Count; j++)
                {
                    var cell = row.Cells[j];
                    var col = Columns[j];

                    object newValue = null;
                    if (col.IsSelectionCheckBox)
                    {
                        int rowID = dataItem.RowID;
                        newValue = _persistentSelectedRows.ContainsKey(rowID) && _persistentSelectedRows[rowID];
                    }
                    else if (col.IsRowNumColumn)
                    {
                        newValue = dataIndex + 1;
                    }
                    else if (col.IsRowID)
                    {
                        newValue = dataItem.RowID;
                    }
                    else
                    {
                        var prop = dataItem.OriginalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                        newValue = prop?.GetValue(dataItem.OriginalData) ?? string.Empty;
                    }

                    // Only update if value changed
                    if (!Equals(cell.CellValue, newValue))
                    {
                        cell.CellValue = newValue;
                        cell.CellData = newValue;
                        InvalidateCell(cell);
                    }
                }
            }
        }

        // Optimize keyboard navigation
        private void MoveNextRow()
        {
            if (_currentRowIndex < Rows.Count - 1)
            {
                // Just move selection without FillVisibleRows
                SelectCell(_currentRowIndex + 1, _selectedColumnIndex);
            }
            else if (_dataOffset + Rows.Count < _fullData.Count)
            {
                // Only when we need to scroll
                _dataOffset++;
                // Clamp the offset to prevent scrolling beyond the last record
                int maxOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
                _dataOffset = Math.Min(_dataOffset, maxOffset);
                FillVisibleRows();
                SelectCell(Rows.Count - 1, _selectedColumnIndex);
            }
        }

        private void MovePreviousRow()
        {
            if (_currentRowIndex > 0)
            {
                // Just move selection without FillVisibleRows
                SelectCell(_currentRowIndex - 1, _selectedColumnIndex);
            }
            else if (_dataOffset > 0)
            {
                // Only when we need to scroll
                _dataOffset--;
                // Clamp the offset to prevent scrolling before the first record
                _dataOffset = Math.Max(0, _dataOffset);
                FillVisibleRows();
                SelectCell(0, _selectedColumnIndex);
            }
        }

        // Optimize MoveNextCell
        public void MoveNextCell()
        {
            if (Rows == null || Rows.Count == 0 || Columns == null || Columns.Count == 0)
                return;

            int lastVisibleColumn = GetLastVisibleColumn();
            int nextColumn = GetNextVisibleColumn(_selectedColumnIndex);

            if (nextColumn != -1)
            {
                // Move to next column in same row - no FillVisibleRows needed
                SelectCell(_currentRowIndex, nextColumn);
            }
            else if (_currentRowIndex < Rows.Count - 1)
            {
                // Move to first column of next row - no FillVisibleRows needed
                int firstColumn = GetNextVisibleColumn(-1);
                SelectCell(_currentRowIndex + 1, firstColumn);
            }
            else if (_dataOffset + Rows.Count < _fullData.Count)
            {
                // Only fill when scrolling to next page
                _dataOffset++;
                FillVisibleRows();
                int firstColumn = GetNextVisibleColumn(-1);
                SelectCell(Rows.Count - 1, firstColumn);
            }
        }

        private int GetLastVisibleColumn()
        {
            try
            {
                if (Columns == null || Columns.Count == 0)
                {
                 //   //MiscFunctions.SendLog("GetLastVisibleColumn: Columns is null or empty.");
                    return -1;
                }

                for (int i = Columns.Count - 1; i >= 0; i--)
                {
                    if (Columns[i] != null && Columns[i].Visible)
                        return i;
                }
                return -1; // No visible columns
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"GetLastVisibleColumn crashed: {ex.Message}");
                return -1;
            }
        }
        private int GetNextVisibleColumn(int currentIndex)
        {
            try
            {
                if (Columns == null || Columns.Count == 0)
                {
                  //  //MiscFunctions.SendLog("GetNextVisibleColumn: Columns is null or empty.");
                    return -1;
                }

                for (int i = currentIndex + 1; i < Columns.Count; i++)
                {
                    if (Columns[i] != null && Columns[i].Visible)
                        return i;
                }
                return -1; // No visible column found
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"GetNextVisibleColumn crashed: {ex.Message}");
                return -1;
            }
        }
        public void MovePreviousCell()
        {
            // If there are no rows or columns, do nothing.
            if (Rows.Count == 0 || Columns.Count == 0)
                return;

            // If we're at the first cell, do nothing.
            if (_currentRowIndex == 0 && _selectedColumnIndex == 0)
                return;

            if (_selectedColumnIndex > 0)
            {
                // Ensure previous column index is within bounds
                int prevColumn = Math.Max(_selectedColumnIndex - 1, 0);
                SelectCell(_currentRowIndex, prevColumn);
                EnsureColumnVisible(prevColumn);
            }
            else if (_currentRowIndex > 0)
            {
                // Move to last column of the previous row
                int lastColumn = Columns.Count - 1;
                SelectCell(_currentRowIndex - 1, lastColumn);
                EnsureColumnVisible(lastColumn);
            }
        }
        private void EnsureColumnVisible(int colIndex)
        {
            try
            {
                if (colIndex < 0 || colIndex >= Columns.Count || Columns[colIndex] == null || !Columns[colIndex].Visible)
                {
                 //   //MiscFunctions.SendLog($"EnsureColumnVisible: Invalid column index {colIndex}");
                    return;
                }

                int columnLeft = 0;
                for (int i = 0; i < colIndex; i++)
                {
                    if (Columns[i] != null && Columns[i].Visible)
                        columnLeft += Columns[i].Width;
                }

                int columnRight = columnLeft + Columns[colIndex].Width;

                int visibleLeft = _xOffset;
                int visibleWidth = DrawingRect.Width - (_verticalScrollBar?.Visible == true ? _verticalScrollBar.Width : 0);
                int visibleRight = visibleLeft + visibleWidth;

                // 🔹 Adjust scroll offset based on position
                if (columnLeft < visibleLeft)
                {
                    _xOffset = columnLeft; // Scroll left
                }
                else if (columnRight > visibleRight)
                {
                    _xOffset = columnRight - visibleWidth; // Scroll right
                }

                _xOffset = Math.Max(0, Math.Min(_xOffset, _horizontalScrollBar?.Maximum ?? 0));

                if (_horizontalScrollBar != null && _horizontalScrollBar.Visible)
                {
                    _horizontalScrollBar.Value = _xOffset;
                }

                UpdateScrollBars();
                UpdateCellPositions(); // 🔹 Ensure cells get updated after scrolling
                Invalidate();
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"EnsureColumnVisible crashed: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
        protected override bool ProcessKeyPreview(ref Message m)
        {
            if (m.Msg == 0x0100) // WM_KEYDOWN
            {
                Keys keyData = (Keys)m.WParam | ModifierKeys;
                switch (keyData)
                {
                    case Keys.Enter:
                        MoveNextCell();
                        break;
                    case Keys.Tab:

                        MoveNextCell();
                        Invalidate();
                        break;
                    case Keys.Up:
                        if (_currentRowIndex > 0)
                        {
                            MovePreviousRow();
                        }
                        break;
                    case Keys.Down:
                        if (_currentRowIndex < Rows.Count - 1)
                        {
                           MoveNextRow();
                        }
                        break;
                    case Keys.Left:
                        if (_selectedColumnIndex > 0)
                        {
                            SelectCell(_currentRowIndex, _selectedColumnIndex - 1);
                        }
                        break;
                    case Keys.Right:
                        if (_selectedColumnIndex < Columns.Count - 1)
                        {
                            SelectCell(_currentRowIndex, _selectedColumnIndex + 1);
                        }
                        break;
                    case Keys.Escape:
                        CancelEditing();
                        CloseCurrentEditor();
                        break;
                }

                return true; // Consume the key
            }

            UpdateRecordNumber();
            return base.ProcessKeyPreview(ref m);
        }
        private void BeepGrid_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {
            
            e.IsInputKey = e.KeyCode switch
            {
                Keys.Tab or Keys.Left or Keys.Right or Keys.Up or Keys.Down or Keys.Enter => true,
                _ => e.IsInputKey
            };
        }
        private void BeepSimpleGrid_KeyDown(object? sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true; // 🔹 Prevent default tab behavior
          
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    MoveNextCell();
                    break;
                case Keys.Tab:
                  
                    MoveNextCell();
                    break;
                case Keys.Up:
                    MovePreviousRow();
                    break;
                case Keys.Down:
                    MoveNextRow();
                    break;
                case Keys.Left:
                    if (_selectedColumnIndex > 0)
                    {
                        SelectCell(_currentRowIndex, _selectedColumnIndex - 1);
                    }
                    break;
                case Keys.Right:
                    if (_selectedColumnIndex < Columns.Count - 1)
                    {
                        SelectCell(_currentRowIndex, _selectedColumnIndex + 1);
                    }
                    break;
                case Keys.Escape:
                    CancelEditing();
                    CloseCurrentEditor();
                    break;
            }
        }
       
        #endregion
        #region Control Creation and Updating
        private IBeepUIComponent CreateCellControlForDrawing(BeepCellConfig cell)
        {
            // Get the column definition based on cell.ColumnIndex.
            var column = Columns[cell.ColumnIndex];

            switch (column.CellEditor)
            {
                case BeepColumnType.Text:
                    return new BeepTextBox { Theme = Theme, IsChild = true ,GridMode=true};
                case BeepColumnType.CheckBoxBool:
                    return new BeepCheckBoxBool { Theme = Theme,HideText=true,Text=string.Empty, IsChild = true, GridMode = true };
                case BeepColumnType.CheckBoxString:
                    return new BeepCheckBoxString { Theme = Theme , HideText = true, Text = string.Empty, IsChild = true, GridMode = true };
                case BeepColumnType.CheckBoxChar:
                    return new BeepCheckBoxChar { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true, GridMode = true };
                case BeepColumnType.ComboBox:
                    return new BeepComboBox { Theme = Theme, IsChild = true, ListItems = new BindingList<SimpleItem>(column.Items), GridMode = true };
                case BeepColumnType.DateTime:
                    return new BeepDatePicker { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.Image:
                    return new BeepImage { Theme = Theme, IsChild = true, GridMode = true        };
                case BeepColumnType.Button:
                    return new BeepButton { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.ProgressBar:
                    return new BeepProgressBar { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.NumericUpDown:
                    return new BeepNumericUpDown { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.Radio:
                    return new BeepRadioGroup { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.ListBox:
                    return new BeepListBox { Theme = Theme, IsChild = true, GridMode = true };
                case BeepColumnType.ListOfValue:
                    return new BeepListofValuesBox { Theme = Theme, IsChild = true, GridMode = true };
                default:
                    return new BeepTextBox { Theme = Theme, IsChild = true, GridMode = true };
            }
        }
        private IBeepUIComponent CreateCellControlForEditing(BeepCellConfig cell)
        {
            // Get the column definition based on cell.ColumnIndex.
            var column = Columns[cell.ColumnIndex];

            switch (column.CellEditor)
            {
                case BeepColumnType.Text:
                    return new BeepTextBox { Theme = Theme, IsChild = true };
                case BeepColumnType.CheckBoxBool:
                    return new BeepCheckBoxBool { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true };
                case BeepColumnType.CheckBoxString:
                    return new BeepCheckBoxString { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true };
                case BeepColumnType.CheckBoxChar:
                    return new BeepCheckBoxChar { Theme = Theme, HideText = true, Text = string.Empty, IsChild = true };
                case BeepColumnType.ComboBox:
                    return new BeepComboBox { Theme = Theme, IsChild = true, ListItems = new BindingList<SimpleItem>(column.Items) };
                case BeepColumnType.DateTime:
                    return new BeepDatePicker { Theme = Theme, IsChild = true };
                case BeepColumnType.Image:
                    return new BeepImage { Theme = Theme, IsChild = true };
                case BeepColumnType.Button:
                    return new BeepButton { Theme = Theme, IsChild = true };
                case BeepColumnType.ProgressBar:
                    return new BeepProgressBar { Theme = Theme, IsChild = true };
                case BeepColumnType.NumericUpDown:
                    return new BeepNumericUpDown { Theme = Theme, IsChild = true };
                case BeepColumnType.Radio:
                    return new BeepRadioGroup { Theme = Theme, IsChild = true};
                case BeepColumnType.ListBox:
                    return new BeepListBox { Theme = Theme, IsChild = true };
                case BeepColumnType.ListOfValue:
                    return new BeepListofValuesBox { Theme = Theme, IsChild = true };
                default:
                    return new BeepTextBox { Theme = Theme, IsChild = true };
            }
        }

        private void UpdateCellControl(IBeepUIComponent control, BeepColumnConfig column,BeepCellConfig cell, object value)
        {
            if (control == null) return;
    //        BeepRowConfig row = Rows[cell.RowIndex];
            if (value == null)
            {
                switch (control)
                {
                    case BeepTextBox textBox:
                        textBox.Text = string.Empty;
                        break;
                    case BeepCheckBoxBool checkBox:
                        checkBox.State = CheckBoxState.Indeterminate;
                        break;
                    case BeepCheckBoxChar checkBox:
                        checkBox.State = CheckBoxState.Indeterminate;
                        break;
                    case BeepCheckBoxString checkBox:
                        checkBox.State = CheckBoxState.Indeterminate;
                        break;
                    case BeepComboBox comboBox:
                        comboBox.Reset();
                        break;
                    case BeepDatePicker datePicker:
                        datePicker.SelectedDate = null;
                        break;
                    case BeepRadioGroup radioButton:
                        radioButton.Reset();
                        break;
                    case BeepListofValuesBox listBox:
                        listBox.Reset();
                        break;
                    case BeepListBox listBox:
                        listBox.Reset();
                        break;
                    case BeepImage image:
                        image.ImagePath = null;
                        break;
                    case BeepButton button:
                        button.Text = string.Empty;
                        break;
                    case BeepProgressBar progressBar:
                        progressBar.Value = 0;
                        break;
                    case BeepStarRating starRating:
                        starRating.SelectedRating = 0;
                        break;
                    case BeepNumericUpDown numericUpDown:
                        numericUpDown.Value = 0;
                        break;
                    case BeepSwitch switchControl:
                        switchControl.Checked = false;
                        break;
                    case BeepLabel label:
                        label.Text = string.Empty;
                        break;
                }
                return;
            }

            switch (control)
            {
                case BeepTextBox textBox:
                    textBox.Text = value.ToString();
                    if (column != null)
                        textBox.MaskFormat = GetTextBoxMaskFormat(column.ColumnType);
                    break;

                case BeepCheckBoxBool checkBox:
                    checkBox.CurrentValue = (bool)MiscFunctions.ConvertValueToPropertyType(typeof(bool), value);
                    break;

                case BeepCheckBoxChar checkBox:
                    checkBox.CurrentValue = (char)MiscFunctions.ConvertValueToPropertyType(typeof(char), value);
                    break;

                case BeepCheckBoxString checkBox:
                    checkBox.CurrentValue = (string)MiscFunctions.ConvertValueToPropertyType(typeof(string), value);
                    break;

                case BeepComboBox comboBox:
                    comboBox.Reset();
                    if (column.ParentColumnName != null)
                    {
                        BeepColumnConfig parentColumn = Columns.FirstOrDefault(c => c.ColumnName == column.ParentColumnName);
                        if (parentColumn != null)
                        {
                            UpdateCellFromParent(cell);
                            List<SimpleItem> filterditems = column.Items.Where(i => i.ParentValue?.ToString() == cell.ParentCellValue.ToString()).ToList();
                            comboBox.ListItems = new BindingList<SimpleItem>(filterditems);
                          
                        }
                    }
                    else
                     if (cell.FilterdList.Count > 0)
                    {
                        comboBox.ListItems = new BindingList<SimpleItem>(cell.FilterdList);
                    }
                    else if (cell.ParentCellValue != null)
                    {
                        BeepColumnConfig parentColumn = Columns.FirstOrDefault(c => c.ColumnName == column.ParentColumnName);
                        if (parentColumn != null)
                        {
                            List<SimpleItem> filterditems = column.Items.Where(i => i.ParentValue?.ToString() == cell.ParentCellValue.ToString()).ToList();
                            comboBox.ListItems = new BindingList<SimpleItem>(filterditems);
                        }
                    }
                    else if (column?.Items != null)
                    {
                        comboBox.ListItems = new BindingList<SimpleItem>(column.Items);

                    }
                    
                    if(cell.CellValue != null)
                    {
                        if (value is SimpleItem simpleItem)
                        {
                            var item = comboBox.ListItems.FirstOrDefault(i => i.Text == simpleItem.Text && i.ImagePath == simpleItem.ImagePath);
                            if (item != null)
                            {
                                comboBox.SelectedItem = item;
                            }
                            else
                                comboBox.SelectedItem = comboBox.ListItems.FirstOrDefault();
                        }
                        else
                        {
                            string stringValue = value.ToString();
                            var item = comboBox.ListItems.FirstOrDefault(i => i.Value?.ToString() == stringValue || i.Text == stringValue);
                            if (item != null)
                            {
                                comboBox.SelectedItem = item;
                            }else
                                comboBox.SelectedItem = comboBox.ListItems.FirstOrDefault();
                        }
                    }
                    // Check if Column is a parent column

                    break;
                case BeepListBox listBox:
                    listBox.Reset();
                    if (column?.Items != null)
                    {
                        listBox.ListItems = new BindingList<SimpleItem>(column.Items);
                        if (value is SimpleItem simpleItem)
                        {
                            var item = column.Items.FirstOrDefault(i => i.Text == simpleItem.Text && i.ImagePath == simpleItem.ImagePath);
                            if (item != null)
                            {
                                listBox.SetValue(item);
                            }
                        }
                        else
                        {
                            string stringValue = value.ToString();
                            var item = column.Items.FirstOrDefault(i => i.Value?.ToString() == stringValue || i.Text == stringValue);
                            if (item != null)
                            {
                                listBox.SetValue(item);
                            }
                        }
                    }
                    break;
                case BeepDatePicker datePicker:
                    if (value is DateTime dateTime)
                    {
                        datePicker.SelectedDate = dateTime.ToString(datePicker.GetCurrentFormat(), datePicker.Culture);
                    }
                    else if (DateTime.TryParse(value.ToString(), datePicker.Culture, DateTimeStyles.None, out DateTime dateValue))
                    {
                        datePicker.SelectedDate = dateValue.ToString(datePicker.GetCurrentFormat(), datePicker.Culture);
                    }
                    else
                    {
                        datePicker.SelectedDate = null;
                    }
                    break;

                case BeepRadioGroup radioButton:
                    radioButton.Reset();
                    if (column?.Items != null)
                    {
                        radioButton.Items = new List<SimpleItem>(column.Items);
                        if (value is SimpleItem simpleItem)
                        {
                            var item = column.Items.FirstOrDefault(i => i.Text == simpleItem.Text && i.ImagePath == simpleItem.ImagePath);
                            if (item != null)
                            {
                                radioButton.SetValue(item);
                            }
                        }
                        else
                        {
                            string stringValue = value.ToString();
                            var item = column.Items.FirstOrDefault(i => i.Value?.ToString() == stringValue || i.Text == stringValue);
                            if (item != null)
                            {
                                radioButton.SetValue(item);
                            }
                        }
                    }
                    break;
                case BeepListofValuesBox listBox:
                    listBox.Reset();
                    if (column?.Items != null)
                    {
                        listBox.ListItems = new List<SimpleItem>(column.Items);
                        if (value is SimpleItem simpleItem)
                        {
                            var item = column.Items.FirstOrDefault(i => i.GuidId == simpleItem.GuidId || (i.Text == simpleItem.Text && i.ImagePath == simpleItem.ImagePath));
                            if (item != null)
                            {
                                listBox.SetValue(item);
                            }
                        }
                        else
                        {
                            string stringValue = value.ToString();
                            var item = column.Items.FirstOrDefault(i => i.GuidId == stringValue || i.Text == stringValue);
                            if (item != null)
                            {
                                listBox.SetValue(item);
                            }
                        }
                    }
                    break;

                case BeepImage image:
                    if (column.ParentColumnName != null)
                    {
                        BeepColumnConfig parentColumn = Columns.FirstOrDefault(c => c.ColumnName == column.ParentColumnName);
                        if (parentColumn != null)
                        {
                            UpdateCellFromParent(cell);
                        }
                    }
                  //  image.ImagePath = ImageListHelper.GetImagePathFromName(cell.CellValue.ToString());
                    break;

                case BeepButton button:
                    button.Text = value.ToString();
                    break;

                case BeepProgressBar progressBar:
                    progressBar.Value = int.TryParse(value.ToString(), out int progress) ? progress : 0;
                    break;

                case BeepStarRating starRating:
                    starRating.SelectedRating = int.TryParse(value.ToString(), out int rating) ? rating : 0;
                    break;

                case BeepNumericUpDown numericUpDown:
                    numericUpDown.Value = (decimal)MiscFunctions.ConvertValueToPropertyType(typeof(decimal), value);
                    break;

                case BeepSwitch switchControl:
                    switchControl.Checked = bool.TryParse(value.ToString(), out bool switchVal) && switchVal;
                    break;

                case BeepLabel label:
                    label.Text = value.ToString();
                    break;
            }

            // check if Child column exist for the changed value column and update the child column

            //List<BeepColumnConfig> childColumns = Columns.Where(c => c.ParentColumnName == column.ColumnName).ToList();
            //if (childColumns.Count > 0)
            //{
            //    foreach (var childColumn in childColumns)
            //    {
            //        var childCell = row.Cells.FirstOrDefault(c => c.ColumnIndex == childColumn.RowIndex);
            //        if (childCell != null)
            //        {
            //            UpdateCellFromParent(childCell);
            //        }
            //    }
            //}

        }
        private void UpdateCellFromParent(BeepCellConfig cell)
        {
            // Get the column definition based on cell.ColumnIndex.
            var column = Columns[cell.ColumnIndex];
            if (column == null) return;
            if (column.ParentColumnName == null) return;
            var parentColumn = Columns.FirstOrDefault(c => c.ColumnName == column.ParentColumnName);
            if (parentColumn == null) return;
            var parentCell = Rows[cell.RowIndex].Cells.FirstOrDefault(c => c.ColumnIndex == parentColumn.Index);
            if (parentCell == null) return;
            if (parentCell.CellValue == null) return;
            

                cell.ParentCellValue = parentCell.CellValue;    
                cell.ParentItem = parentCell.Item;
           

        }
        private TextBoxMaskFormat GetTextBoxMaskFormat(DbFieldCategory columnType)
        {
            return columnType switch
            {
                DbFieldCategory.Numeric => TextBoxMaskFormat.Numeric,
                DbFieldCategory.Date => TextBoxMaskFormat.Date,
                DbFieldCategory.Timestamp => TextBoxMaskFormat.DateTime,
                DbFieldCategory.Currency => TextBoxMaskFormat.Currency,
                _ => TextBoxMaskFormat.None
            };
        }
        #endregion
        #region Painting
        private void RecalculateGridRect()
        {
            UpdateDrawingRect();

            var drawingBounds = DrawingRect;

            // Update scrollbar visibility first
            UpdateScrollBars();

            // Draw Bottom Items before Drawing the Grid
            bottomPanelY = drawingBounds.Bottom;
            botomspacetaken = 0;
            topPanelY = drawingBounds.Top;

            // Reserve space for horizontal scrollbar if visible
            if (_horizontalScrollBar.Visible)
            {
                bottomPanelY -= _horizontalScrollBar.Height;
                botomspacetaken += _horizontalScrollBar.Height;
            }

            if (_showNavigator)
            {
                bottomPanelY -= navigatorPanelHeight;
                botomspacetaken += navigatorPanelHeight;
            }
            //if (!_navigatorDrawn)
            //{
                _navigatorDrawn = true;
                if (_showNavigator)
                {
                    navigatorPanelRect = new Rectangle(drawingBounds.Left, drawingBounds.Bottom - navigatorPanelHeight, drawingBounds.Width, navigatorPanelHeight);
                 //   DrawNavigationRow(g, navigatorPanelRect);
                }
                else
                {

                    navigatorPanelRect = new Rectangle(-100, -100, drawingBounds.Width, navigatorPanelHeight);
                   // DrawNavigationRow(g, navigatorPanelRect);
                }
          //  }

            if (_showFooter)
            {
                bottomPanelY -= footerPanelHeight;
                botomspacetaken += footerPanelHeight;
                footerPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width, footerPanelHeight);
                //DrawFooterRow(g, footerPanelRect);
            }

            if (_showaggregationRow)
            {
                bottomPanelY -= bottomagregationPanelHeight;
                botomspacetaken += bottomagregationPanelHeight;
                bottomagregationPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width, bottomagregationPanelHeight);
                //DrawBottomAggregationRow(g, bottomagregationPanelRect);
            }

            // Draw Top Items before Drawing the Grid
           
            if (!_filterpaneldrawn)
            {
                _filterpaneldrawn = true;
                if (_showFilterpanel)
                {
                    filterPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, filterPanelHeight);
                  //  DrawFilterPanel(g, filterPanelRect);
                }
                else
                {
                    filterPanelRect = new Rectangle(-100, -100, drawingBounds.Width, filterPanelHeight);
                    //DrawFilterPanel(g, filterPanelRect);
                }
            }
            if (_showFilterpanel)
            {
                topPanelY += filterPanelHeight + 10;
            }
            if (_showHeaderPanel)
            {
                int ttitleLabelHeight = headerPanelHeight;

                ttitleLabelHeight = titleLabel.GetPreferredSize(Size.Empty).Height;

                if (ttitleLabelHeight > headerPanelHeight)
                {
                    headerPanelHeight = ttitleLabelHeight;

                }

                headerPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, headerPanelHeight);
                //DrawHeaderPanel(g, headerPanelRect);
                topPanelY += headerPanelHeight;
            }
            else
            {
                headerPanelHeight = 0;
                headerPanelRect = new Rectangle(-100, -100, drawingBounds.Width, headerPanelHeight);
                //DrawHeaderPanel(g, headerPanelRect);
            }

            if (_showColumnHeaders)
            {
                columnsheaderPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, ColumnHeaderHeight);
                //PaintColumnHeaders(g, columnsheaderPanelRect);
                topPanelY += ColumnHeaderHeight;
            }

            // Grid would Draw on the remaining space
            int availableHeight = drawingBounds.Height - topPanelY - botomspacetaken;
            int availableWidth = drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);
            gridRect = new Rectangle(drawingBounds.Left, topPanelY, availableWidth, availableHeight);

            // Dr
        }
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            // Set rendering hints based on DPI
            if (DpiScaleFactor > 1.5f)
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            }
            // Cache rectangles to avoid recalculation
            if (_layoutDirty)
            {
                RecalculateGridRect();
                _layoutDirty = false;
            }

            var drawingBounds = DrawingRect;

            // Draw Bottom Items before Drawing the Grid
            bottomPanelY = drawingBounds.Bottom;
            botomspacetaken = 0;
            topPanelY = drawingBounds.Top;

            // Reserve space for horizontal scrollbar if visible
            if (_horizontalScrollBar.Visible)
            {
                bottomPanelY -= _horizontalScrollBar.Height;
                botomspacetaken += _horizontalScrollBar.Height;
            }

            if (_showNavigator)
            {
                bottomPanelY -= navigatorPanelHeight;
                botomspacetaken += navigatorPanelHeight;
                navigatorPanelRect = new Rectangle(drawingBounds.Left, drawingBounds.Bottom - navigatorPanelHeight - (_horizontalScrollBar.Visible ? _horizontalScrollBar.Height : 0), drawingBounds.Width, navigatorPanelHeight);
                DrawNavigationRow(g, navigatorPanelRect);
            }

            if (_showFooter)
            {
                bottomPanelY -= footerPanelHeight;
                botomspacetaken += footerPanelHeight;
                footerPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width, footerPanelHeight);
                DrawFooterRow(g, footerPanelRect);
            }

            if (_showaggregationRow)
            {
                bottomPanelY -= bottomagregationPanelHeight;
                botomspacetaken += bottomagregationPanelHeight;
                bottomagregationPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width, bottomagregationPanelHeight);
                DrawBottomAggregationRow(g, bottomagregationPanelRect);
            }

            // Draw Top Items before Drawing the Grid
            if (_showFilterpanel)
            {
                filterPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, filterPanelHeight);
                DrawFilterPanel(g, filterPanelRect);
                topPanelY += filterPanelHeight + 10;
            }

            if (_showHeaderPanel)
            {
                int ttitleLabelHeight = headerPanelHeight;
                ttitleLabelHeight = titleLabel.GetPreferredSize(Size.Empty).Height;
                if (ttitleLabelHeight > headerPanelHeight)
                {
                    headerPanelHeight = ttitleLabelHeight;
                }
                headerPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, headerPanelHeight);
                DrawHeaderPanel(g, headerPanelRect);
                topPanelY += headerPanelHeight;
            }

            if (_showColumnHeaders)
            {
                columnsheaderPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width, ColumnHeaderHeight);
                PaintColumnHeaders(g, columnsheaderPanelRect);
                topPanelY += ColumnHeaderHeight;
            }

            // Grid would Draw on the remaining space
            int availableHeight = drawingBounds.Height - topPanelY - botomspacetaken;
            int availableWidth = drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);
            gridRect = new Rectangle(drawingBounds.Left, topPanelY, availableWidth, availableHeight);

            // Draw grid content
            PaintRows(g, gridRect);

            // Draw grid lines if enabled
            if (_showverticalgridlines)
            {
                DrawColumnBorders(g, gridRect);
            }
            if (_showhorizontalgridlines)
            {
                DrawRowsBorders(g, gridRect);
            }
            // Update DPI cache if needed
            UpdateDpiCache();

            // Rest of existing DrawContent code...
            PositionScrollBars();
            // Reset quality at the end
            if (DpiScaleFactor > 1.5f)
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            }

        }
        private void DrawFilterPanel(Graphics g, Rectangle filterPanelRect)
        {
            int padding = 10; // Consistent padding from the edge
            int spacing = 5;  // Spacing between controls
            int controlHeight = filterPanelRect.Height - (2 * filtercontrolsheight);

            // Position controls from the right edge of the filter panel
            if (filterColumnComboBox != null && filterTextBox != null)
            {
                // Position the ComboBox first, aligned to the far right
                int comboBoxWidth = 200;
                int comboBoxX = filterPanelRect.Right - comboBoxWidth - padding;
                int controlY = filterPanelRect.Y + filtercontrolsheight;

                filterColumnComboBox.Location = new Point(comboBoxX, controlY);
                filterColumnComboBox.Size = new Size(comboBoxWidth, controlHeight);

                // Position the TextBox to the left of the ComboBox
                int textBoxWidth = 200;
                int textBoxX = comboBoxX - textBoxWidth - spacing;

                filterTextBox.Location = new Point(textBoxX, controlY);
                filterTextBox.Size = new Size(textBoxWidth, controlHeight);
            }
        }
        private void DrawBottomAggregationRow(Graphics g, Rectangle rect)
        {
            using (var brush = new SolidBrush(_currentTheme.BackColor))
            using (var pen = new Pen(_currentTheme.GridLineColor))
            using (var thickPen = new Pen(_currentTheme.GridLineColor, 2)) // Thicker pen for emphasis
            {
                g.FillRectangle(brush, rect);

                // Draw a prominent top line
                g.DrawLine(thickPen, rect.Left, rect.Top, rect.Right, rect.Top);
                // Optional: Add a second line for extra emphasis (e.g., 1 pixel below)
                // g.DrawLine(pen, rect.Left, rect.Top + 1, rect.Right, rect.Top + 1);

                if (aggregationRow != null)
                {
                    // Draw scrolling columns, adjusted for horizontal scroll
                    var scrollingColumns = Columns.Where(c => !c.Sticked && c.Visible).ToList();
                    int scrollingXOffset = rect.Left + _stickyWidth - _xOffset; // Start after sticky columns, adjusted by scroll
                    foreach (var scrollingCol in scrollingColumns)
                    {
                        int columnIndex = Columns.IndexOf(scrollingCol);
                        if (columnIndex >= 0 && columnIndex < aggregationRow.Cells.Count)
                        {
                            var cell = aggregationRow.Cells[columnIndex];
                            var cellRect = new Rectangle(scrollingXOffset, rect.Top, scrollingCol.Width, rect.Height);
                            PaintCell(g, cell, cellRect, _currentTheme.GridBackColor);
                            scrollingXOffset += scrollingCol.Width;
                        }
                    }
                    // Draw sticky columns first
                    var stickyColumns = Columns.Where(c => c.Sticked && c.Visible).ToList();
                    int stickyXOffset = rect.Left;
                    foreach (var stickyCol in stickyColumns)
                    {
                        int columnIndex = Columns.IndexOf(stickyCol);
                        if (columnIndex >= 0 && columnIndex < aggregationRow.Cells.Count)
                        {
                            var cell = aggregationRow.Cells[columnIndex];
                            int stickyX = stickyXOffset;
                            var cellRect = new Rectangle(stickyX, rect.Top, stickyCol.Width, rect.Height);
                            PaintCell(g, cell, cellRect, _currentTheme.GridBackColor);
                            stickyXOffset += stickyCol.Width;
                        }
                    }
                }
            }
        }
        private void DrawFooterRow(Graphics g, Rectangle rect)
        {
            using (var brush = new SolidBrush(_currentTheme.BackColor))
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.FillRectangle(brush, rect);
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                g.DrawString("Footer", Font, new SolidBrush(_currentTheme.PrimaryTextColor), rect,
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }
        private void UpdateStickyWidth()
        {
            var selColumn = Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_showCheckboxes)
            {
                selColumn.Visible = true;
            }
            else
                selColumn.Visible = false;
            var stickyColumns = Columns.Where(c => c.Sticked && c.Visible).ToList();
            int baseStickyWidth = stickyColumns.Sum(c => c.Width);

            // Cap _stickyWidth to prevent overflow within gridRect
            _stickyWidth = Math.Min(baseStickyWidth, gridRect.Width);
          //   //MiscFunctions.SendLog($"UpdateStickyWidth: _stickyWidth={_stickyWidth}, BaseSticky={baseStickyWidth}, GridRect.Width={gridRect.Width}");
        }
        private void PaintColumnHeaders(Graphics g, Rectangle headerRect)
        {
            int xOffset = headerRect.Left;
            var selColumn = Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_showCheckboxes)
            {
                selColumn.Visible = true;
            }
            else
                selColumn.Visible = false;
            // Ensure _stickyWidth is calculated and capped
            UpdateStickyWidth();
            int stickyWidth = _stickyWidth;
            stickyWidth = Math.Min(stickyWidth, headerRect.Width); // Prevent overflow

            // Draw Border line on Top
            using (Pen borderPen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(borderPen, headerRect.Left, headerRect.Top, headerRect.Right, headerRect.Top);
            }
            StringFormat centerFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            // Define sticky and scrolling regions
            Rectangle stickyRegion = new Rectangle(headerRect.Left, headerRect.Top, stickyWidth, headerRect.Height);
            Rectangle scrollingRegion = new Rectangle(headerRect.Left + stickyWidth, headerRect.Top,
                                                     headerRect.Width - stickyWidth, headerRect.Height);

            // Draw scrolling column headers first
            using (Region clipRegion = new Region(scrollingRegion))
            {
                g.Clip = clipRegion;
                int scrollingXOffset = headerRect.Left + stickyWidth - _xOffset; // Adjusted for horizontal scroll
                foreach (var col in Columns.Where(c => !c.Sticked && c.Visible))
                {
                    var headerCellRect = new Rectangle(scrollingXOffset, headerRect.Top, col.Width, headerRect.Height);
                    PaintHeaderCell(g, col, headerCellRect, centerFormat);
                    scrollingXOffset += col.Width;

                    // Debug output for scrolling headers
                 //   //MiscFunctions.SendLog($"PaintColumnHeaders Scrolling: Col={col.ColumnName}, X={scrollingXOffset}, Width={col.Width}, _xOffset={_xOffset}");
                }
            }

            // Draw sticky column headers last
            using (Region clipRegion = new Region(stickyRegion))
            {
                g.Clip = clipRegion;
                xOffset = headerRect.Left;

                foreach (var col in Columns.Where(c => c.Sticked && c.Visible))
                {
                    var headerCellRect = new Rectangle(xOffset, headerRect.Top, col.Width, headerRect.Height);
                    PaintHeaderCell(g, col, headerCellRect, centerFormat);
                    xOffset += col.Width;

                    // Debug output for sticky headers
                   // //MiscFunctions.SendLog($"PaintColumnHeaders Sticky: Col={col.ColumnName}, X={xOffset}, Width={col.Width}");
                }
            }

            if (stickyWidth > 0)
            {
                g.ResetClip();
                using (Pen borderPen = new Pen(_currentTheme.GridLineColor))
                {
                    g.DrawLine(borderPen, headerRect.Left + stickyWidth, headerRect.Top, headerRect.Left + stickyWidth, headerRect.Bottom);
                }
            }

          //  //MiscFunctions.SendLog($"PaintColumnHeaders: StickyWidth={stickyWidth}, HeaderRect={headerRect}, _xOffset={_xOffset}");
        }
        // Update the PaintHeaderCell method to use custom header colors
        private void PaintHeaderCell(Graphics g, BeepColumnConfig col, Rectangle cellRect, StringFormat format)
        {
            // Determine header colors
            Color headerBackColor = _currentTheme.GridHeaderBackColor;
            Color headerForeColor = _currentTheme.GridHeaderForeColor;

            if (col.UseCustomColors)
            {
                if (col.HasCustomHeaderBackColor)
                    headerBackColor = col.ColumnHeaderBackColor;
                if (col.HasCustomHeaderForeColor)
                    headerForeColor = col.ColumnHeaderForeColor;
            }

            using (Brush bgBrush = new SolidBrush(headerBackColor))
            using (Brush textBrush = new SolidBrush(headerForeColor))
            {
                g.FillRectangle(bgBrush, cellRect);

                if (!col.IsSelectionCheckBox)
                {
                    // Calculate text area (reserve space for icons in corners)
                    Rectangle textRect = cellRect;
                    int iconSpace = 0;
                    int iconSize = 14; // Smaller icon size
                    int iconPadding = 2;

                    if (col.ShowSortIcon) iconSpace += iconSize + iconPadding;
                    if (col.ShowFilterIcon) iconSpace += iconSize + iconPadding;

                    if (iconSpace > 0)
                    {
                        textRect.Width -= iconSpace + 4; // Reserve space for icons
                        textRect.Height -= iconSize + iconPadding; // Reserve vertical space too
                    }

                    // Draw column text (centered in the available space)
                    string caption = col.ColumnCaption ?? col.ColumnName ?? "";
                    var centeredFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(caption, _columnHeadertextFont ?? Font, textBrush, textRect, centeredFormat);

                    // Position icons in corners
                    if (col.ShowSortIcon && col.ShowFilterIcon)
                    {
                        // Sort icon in UPPER RIGHT corner
                        Rectangle sortIconRect = new Rectangle(
                            cellRect.Right - iconSize - iconPadding,
                            cellRect.Top + iconPadding,
                            iconSize,
                            iconSize
                        );
                        DrawSortIcon(g, sortIconRect, col.SortDirection, headerForeColor);
                        AddHitArea($"SortIcon_{col.Index}", sortIconRect);

                        // Filter icon in BOTTOM RIGHT corner
                        Rectangle filterIconRect = new Rectangle(
                            cellRect.Right - iconSize - iconPadding,
                            cellRect.Bottom - iconSize - iconPadding,
                            iconSize,
                            iconSize
                        );
                        DrawFilterIcon(g, filterIconRect, col.IsFiltered, headerForeColor);
                        AddHitArea($"FilterIcon_{col.Index}", filterIconRect);
                    }
                    else if (col.ShowSortIcon)
                    {
                        // Only sort icon - place in upper right
                        Rectangle sortIconRect = new Rectangle(
                            cellRect.Right - iconSize - iconPadding,
                            cellRect.Top + iconPadding,
                            iconSize,
                            iconSize
                        );
                        DrawSortIcon(g, sortIconRect, col.SortDirection, headerForeColor);
                        AddHitArea($"SortIcon_{col.Index}", sortIconRect);
                    }
                    else if (col.ShowFilterIcon)
                    {
                        // Only filter icon - place in upper right  
                        Rectangle filterIconRect = new Rectangle(
                            cellRect.Right - iconSize - iconPadding,
                            cellRect.Top + iconPadding,
                            iconSize,
                            iconSize
                        );
                        DrawFilterIcon(g, filterIconRect, col.IsFiltered, headerForeColor);
                        AddHitArea($"FilterIcon_{col.Index}", filterIconRect);
                    }
                }
            }

            // Draw border
            Color borderColor = col.HasCustomBorderColor ? col.ColumnBorderColor : _currentTheme.GridLineColor;
            using (Pen borderPen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(borderPen, cellRect);
            }
        }

        private void DrawSortIcon(Graphics g, Rectangle iconRect, SortDirection direction, Color color)
        {
            using (Pen pen = new Pen(color, 1.5f))
            using (Brush brush = new SolidBrush(color))
            {
                int centerX = iconRect.X + iconRect.Width / 2;
                int centerY = iconRect.Y + iconRect.Height / 2;
                int arrowSize = Math.Min(iconRect.Width, iconRect.Height) / 3; // Smaller arrows

                switch (direction)
                {
                    case SortDirection.Ascending:
                        // Draw up arrow (smaller and more refined)
                        Point[] upArrowAsc = {
                    new Point(centerX, iconRect.Y + 2),
                    new Point(centerX - arrowSize, centerY + 1),
                    new Point(centerX + arrowSize, centerY + 1)
                };
                        g.FillPolygon(brush, upArrowAsc);
                        break;

                    case SortDirection.Descending:
                        // Draw down arrow (smaller and more refined)
                        Point[] downArrowDesc = {
                    new Point(centerX, iconRect.Bottom - 2),
                    new Point(centerX - arrowSize, centerY - 1),
                    new Point(centerX + arrowSize, centerY - 1)
                };
                        g.FillPolygon(brush, downArrowDesc);
                        break;

                    case SortDirection.None:
                        // Draw both arrows (inactive, lighter)
                        using (Pen lightPen = new Pen(Color.FromArgb(100, color), 1))
                        {
                            Point[] upArrowNone = {
                        new Point(centerX, iconRect.Y + 2),
                        new Point(centerX - arrowSize + 1, centerY),
                        new Point(centerX + arrowSize - 1, centerY)
                    };
                            Point[] downArrowNone = {
                        new Point(centerX, iconRect.Bottom - 2),
                        new Point(centerX - arrowSize + 1, centerY),
                        new Point(centerX + arrowSize - 1, centerY)
                    };
                            g.DrawPolygon(lightPen, upArrowNone);
                            g.DrawPolygon(lightPen, downArrowNone);
                        }
                        break;
                }
            }
        }

        private void DrawFilterIcon(Graphics g, Rectangle iconRect, bool isActive, Color color)
        {
            using (Pen pen = new Pen(isActive ? color : Color.FromArgb(100, color), isActive ? 1.5f : 1f))
            using (Brush brush = new SolidBrush(isActive ? color : Color.FromArgb(100, color)))
            {
                // Draw a more compact filter funnel shape
                int padding = 1;
                int funnelWidth = iconRect.Width - (padding * 2);
                int funnelHeight = iconRect.Height - (padding * 2);

                Point[] funnel = {
            new Point(iconRect.X + padding, iconRect.Y + padding),
            new Point(iconRect.Right - padding, iconRect.Y + padding),
            new Point(iconRect.X + iconRect.Width / 2 + 2, iconRect.Y + funnelHeight / 2),
            new Point(iconRect.X + iconRect.Width / 2 + 2, iconRect.Bottom - padding),
            new Point(iconRect.X + iconRect.Width / 2 - 2, iconRect.Bottom - padding),
            new Point(iconRect.X + iconRect.Width / 2 - 2, iconRect.Y + funnelHeight / 2)
        };

                if (isActive)
                    g.FillPolygon(brush, funnel);
                else
                    g.DrawPolygon(pen, funnel);

                // Add a small circle or dot to indicate active filter
                if (isActive)
                {
                    using (Brush dotBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
                    {
                        int dotSize = 2;
                        g.FillEllipse(dotBrush,
                            iconRect.X + iconRect.Width / 2 - dotSize / 2,
                            iconRect.Y + iconRect.Height / 2 - dotSize / 2,
                            dotSize, dotSize);
                    }
                }
            }
        }
        private void PaintRows(Graphics g, Rectangle bounds)
        {
            int yOffset = bounds.Top;
            var selColumn = Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            if (_showCheckboxes)
            {
                selColumn.Visible = true;
            }else
                selColumn.Visible = false;
            var stickyColumns = Columns.Where(c => c.Sticked && c.Visible).ToList();
          
            // Ensure _stickyWidth is calculated and capped
            _stickyWidth = stickyColumns.Sum(c => c.Width);
            _stickyWidth = Math.Min(_stickyWidth, bounds.Width); // Prevent overflow

            // Define sticky and scrolling regions
            Rectangle stickyRegion = new Rectangle(bounds.Left, bounds.Top, _stickyWidth, bounds.Height);
            Rectangle scrollingRegion = new Rectangle(bounds.Left + _stickyWidth, bounds.Top, bounds.Width - _stickyWidth, bounds.Height);

            // Draw scrolling columns first
            using (Region clipRegion = new Region(scrollingRegion))
            {
                g.Clip = clipRegion;
                for (int i = 0; i < Rows.Count; i++)
                {
                    var row = Rows[i];
                   // int displayY = row.IsAggregation && _showaggregationRow ? bounds.Bottom - bottomagregationPanelHeight : yOffset;
                    int displayY =  yOffset;
                    // Stop if yOffset exceeds bounds for non-aggregation rows
                    //if (!row.IsAggregation && yOffset + RowHeight > bounds.Bottom - (_showaggregationRow ? bottomagregationPanelHeight : 0))
                    //    break;
                    // Stop if yOffset exceeds bounds for non-aggregation rows
                    if (yOffset + row.Height > bounds.Bottom)
                        break;
                    // Align rowRect with scrollingRegion, accounting for scroll
                    //int scrollingStartX = scrollingRegion.Left - _xOffset;
                    //int totalScrollableWidth = Columns.Where(c => !c.Sticked && c.Visible).Sum(c => c.Width) +
                    //                          (Columns.Count(c => !c.Sticked && c.Visible) - 1) * 1;
                    //int scrollingWidth = Math.Max(scrollingRegion.Width + _xOffset, totalScrollableWidth);
                    //var rowRect = new Rectangle(scrollingStartX, displayY, scrollingWidth, row.Height);
                    var rowRect = new Rectangle(scrollingRegion.Left, displayY, scrollingRegion.Width, row.Height);

                    PaintScrollingRow(g, row, rowRect);
                    if (!row.IsAggregation) yOffset += row.Height; // Only increment yOffset for non-aggregation rows

                }
            }

            // Reset yOffset for sticky columns
            yOffset = bounds.Top;

            // Draw sticky columns last (on top)
            using (Region clipRegion = new Region(stickyRegion))
            {
                g.Clip = clipRegion;
                foreach (var stickyCol in stickyColumns)
                {
                    int stickyX = bounds.Left + stickyColumns.TakeWhile(c => c != stickyCol).Sum(c => c.Width);
                    for (int i = 0; i < Rows.Count; i++)
                    {
                        var row = Rows[i];
                        // int displayY = row.IsAggregation && _showaggregationRow ? bounds.Bottom - bottomagregationPanelHeight : yOffset;
                        int displayY = yOffset;
                        // Stop if yOffset exceeds bounds for non-aggregation rows
                        //if (!row.IsAggregation && yOffset + RowHeight > bounds.Bottom - (_showaggregationRow ? bottomagregationPanelHeight : 0))
                        //    break;
                        // Stop if yOffset exceeds bounds for non-aggregation rows
                        if (yOffset + row.Height > bounds.Bottom)
                            break;

                        var cell = row.Cells[Columns.IndexOf(stickyCol)];
                        var cellRect = new Rectangle(stickyX, displayY, stickyCol.Width, row.Height);
                        Color backcolor = cell.RowIndex == _currentRowIndex ? _currentTheme.SelectedRowBackColor : _currentTheme.GridBackColor;
                        PaintCell(g, cell, cellRect, backcolor);
                        // set cell coordinates and size in cell
                        cell.X = cellRect.X;
                        cell.Y = cellRect.Y;
                        cell.Width = cellRect.Width;
                        cell.Height = cellRect.Height;
                        if (!row.IsAggregation) yOffset += row.Height; // Only increment yOffset for non-aggregation rows
                    }
                    yOffset = bounds.Top;
                }
            }

            g.ResetClip();
        }
        private void PaintScrollingRow(Graphics g, BeepRowConfig row, Rectangle rowRect)
        {
      
            // Calculate effective right boundary by subtracting the vertical scrollbar width if it's visible.
            // Compute boundaries for the scrolling area:
            // Assume gridRect represents the entire grid, and sticky columns occupy the left part.
            int scrollingLeft = gridRect.Left + _stickyWidth;
            int scrollingRight = gridRect.Right;
            // If the vertical scrollbar is visible, subtract its width from the right boundary.
            if (_verticalScrollBar != null && _verticalScrollBar.Visible)
            {
                scrollingRight -= _verticalScrollBar.Width;
            }

            // We use rowRect for initial positioning, but clamp our effective boundaries:
            int effectiveLeft = Math.Max(rowRect.Left, scrollingLeft);
            int effectiveRight = Math.Min(rowRect.Right, scrollingRight);

            int accumulatedWidth = 0;
            foreach (var column in Columns.Where(c => !c.Sticked && c.Visible))
            {
                // Calculate the X coordinate for the cell using the accumulated width and horizontal scroll.
                int cellX = rowRect.Left + accumulatedWidth - _xOffset;
                int cellWidth = column.Width;

                // If the cell starts before the effective left boundary, adjust it.
                if (cellX < effectiveLeft)
                {
                    int overflow = effectiveLeft - cellX;
                    cellX = effectiveLeft;
                    cellWidth = Math.Max(0, cellWidth - overflow);
                }

                // If the cell would extend beyond the effective right boundary, truncate its width.
                if (cellX + cellWidth > effectiveRight)
                {
                    cellWidth = Math.Max(0, effectiveRight - cellX);
                }

                // Skip the cell if there’s no visible width.
                if (cellWidth <= 0)
                {
                    accumulatedWidth += column.Width;
                    continue;
                }

                // Define the cell rectangle and paint the cell.
                Rectangle cellRect = new Rectangle(cellX, rowRect.Top, cellWidth, rowRect.Height);
                Color backcolor = row.Cells[Columns.IndexOf(column)].RowIndex == _currentRowIndex
                                  ? _currentTheme.SelectedRowBackColor
                                  : _currentTheme.GridBackColor;
                PaintCell(g, row.Cells[Columns.IndexOf(column)], cellRect, backcolor);

                accumulatedWidth += column.Width;
                // If we've reached the effective right boundary, exit the loop.
                if (rowRect.Left + accumulatedWidth - _xOffset >= effectiveRight)
                    break;
            }
        }
        // Cache brushes and pens at the class level
        private SolidBrush _cellBrush;
        private SolidBrush _selectedCellBrush;
        private Pen _borderPen;
        private Pen _selectedBorderPen;
        private void InitializePaintingResources()
        {
            _cellBrush = new SolidBrush(_currentTheme.GridBackColor);
            _selectedCellBrush = new SolidBrush(_currentTheme.GridRowHoverBackColor);
            _borderPen = new Pen(_currentTheme.GridLineColor);
            _selectedBorderPen = new Pen(_currentTheme.PrimaryTextColor, 2);
        }
        private void PaintCell(Graphics g, BeepCellConfig cell, Rectangle cellRect, Color backcolor)
        {
            // If this cell is being edited, skip drawing so that
            // the editor control remains visible.
            Rectangle TargetRect = cellRect;
            BeepColumnConfig column;
            cell.Rect = TargetRect;

            if (cell.IsAggregation)
            {
                column = Columns[cell.ColumnIndex];
            }
            else
            {
                column = Columns[cell.ColumnIndex];
            }

            // Determine colors to use - check for custom column colors first
            Color cellBackColor = backcolor;
            Color cellForeColor = _currentTheme.GridForeColor;
            Color cellBorderColor = _currentTheme.GridLineColor;

            // Apply custom column colors if enabled
            if (column.UseCustomColors)
            {
                if (column.HasCustomBackColor)
                {
                    cellBackColor = column.ColumnBackColor;
                }
                if (column.HasCustomForeColor)
                {
                    cellForeColor = column.ColumnForeColor;
                }
                if (column.HasCustomBorderColor)
                {
                    cellBorderColor = column.ColumnBorderColor;
                }
            }

            // Use cached brushes with custom colors
            using (var brush = new SolidBrush(cellBackColor))
            {
                // Use faster fill for high DPI
                if (DpiScaleFactor > 1.5f)
                {
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                }

                g.FillRectangle(brush, cellRect);
            }

            // Draw selection border if this is the selected cell
            if (_selectedCell == cell)
            {
                using (var pen = new Pen(_currentTheme.PrimaryTextColor, 2))
                {
                    g.DrawRectangle(pen, cellRect);
                }
            }
            else
            {
                // Draw normal border with custom color if specified
                using (var pen = new Pen(cellBorderColor, 1))
                {
                    g.DrawRectangle(pen, cellRect);
                }
            }

            // Get the column editor if available
            if (!_columnDrawers.TryGetValue(Columns[cell.ColumnIndex].ColumnName, out IBeepUIComponent columnDrawer))
            {
                // Create a new control if it doesn't exist (failsafe)
                columnDrawer = CreateCellControlForDrawing(cell);
                _columnDrawers[Columns[cell.ColumnIndex].ColumnName] = columnDrawer;
            }

            if (columnDrawer != null)
            {
                var editor = (Control)columnDrawer;
                editor.Bounds = new Rectangle(TargetRect.X, TargetRect.Y, TargetRect.Width, TargetRect.Height);

                var checkValueupdate = new BeepCellEventArgs(cell);
                CellPreUpdateCellValue?.Invoke(this, checkValueupdate);

                if (!checkValueupdate.Cancel)
                {
                    UpdateCellControl(columnDrawer, Columns[cell.ColumnIndex], cell, cell.CellValue);
                }

                // Force BeepTextBox for aggregation cells
                if (cell.IsAggregation)
                {
                    BeepTextBox textBox = columnDrawer as BeepTextBox ?? new BeepTextBox
                    {
                        Theme = Theme,
                        IsReadOnly = true,
                        Text = cell.CellValue?.ToString() ?? ""
                    };
                    textBox.ForeColor = cellForeColor;
                    textBox.BackColor = cellBackColor;
                    textBox.Draw(g, TargetRect);
                }
                else
                {
                    var checkCustomDraw = new BeepCellEventArgs(cell);
                    checkCustomDraw.Graphics = g;
                    CellCustomCellDraw?.Invoke(this, checkCustomDraw);

                    if (checkCustomDraw.Cancel)
                    {
                        if (DpiScaleFactor > 1.5f)
                        {
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Default;
                        }
                        return;
                    }

                    columnDrawer.IsFrameless = true;

                    // Apply custom colors to the editor controls
                    if (columnDrawer is Control editorControl)
                    {
                        editorControl.ForeColor = cellForeColor;
                        editorControl.BackColor = cellBackColor;
                    }

                    // Draw the editor based on column type for non-aggregation cells
                    switch (columnDrawer)
                    {
                        case BeepTextBox textBox:
                            textBox.ForeColor = cellForeColor;
                            textBox.BackColor = cellBackColor;
                            textBox.Draw(g, TargetRect);
                            break;
                        case BeepCheckBoxBool checkBox1:
                            checkBox1.ForeColor = cellForeColor;
                            checkBox1.BackColor = cellBackColor;
                            checkBox1.Draw(g, TargetRect);
                            break;
                        case BeepCheckBoxChar checkBox2:
                            checkBox2.ForeColor = cellForeColor;
                            checkBox2.BackColor = cellBackColor;
                            checkBox2.Draw(g, TargetRect);
                            break;
                        case BeepCheckBoxString checkBox3:
                            checkBox3.ForeColor = cellForeColor;
                            checkBox3.BackColor = cellBackColor;
                            checkBox3.Draw(g, TargetRect);
                            break;
                        case BeepComboBox comboBox:
                            comboBox.ForeColor = cellForeColor;
                            comboBox.BackColor = cellBackColor;
                            comboBox.Draw(g, TargetRect);
                            break;
                        case BeepDatePicker datePicker:
                            datePicker.ForeColor = cellForeColor;
                            datePicker.BackColor = cellBackColor;
                            datePicker.Draw(g, TargetRect);
                            break;
                        case BeepImage image:
                            image.Size = new Size(column.MaxImageWidth, column.MaxImageHeight);
                            image.DrawImage(g, TargetRect);
                            break;
                        case BeepButton button:
                            button.ForeColor = cellForeColor;
                            button.BackColor = cellBackColor;
                            button.Draw(g, TargetRect);
                            break;
                        case BeepProgressBar progressBar:
                            progressBar.ForeColor = cellForeColor;
                            progressBar.BackColor = cellBackColor;
                            progressBar.Draw(g, TargetRect);
                            break;
                        case BeepStarRating starRating:
                            starRating.ForeColor = cellForeColor;
                            starRating.BackColor = cellBackColor;
                            starRating.Draw(g, TargetRect);
                            break;
                        case BeepNumericUpDown numericUpDown:
                            numericUpDown.ForeColor = cellForeColor;
                            numericUpDown.BackColor = cellBackColor;
                            numericUpDown.Draw(g, TargetRect);
                            break;
                        case BeepSwitch switchControl:
                            switchControl.ForeColor = cellForeColor;
                            switchControl.BackColor = cellBackColor;
                            switchControl.Draw(g, TargetRect);
                            break;
                        case BeepListofValuesBox listBox:
                            listBox.ForeColor = cellForeColor;
                            listBox.BackColor = cellBackColor;
                            listBox.Draw(g, TargetRect);
                            break;
                        case BeepLabel label:
                            label.ForeColor = cellForeColor;
                            label.BackColor = cellBackColor;
                            label.Draw(g, TargetRect);
                            break;
                        default:
                            using (var textBrush = new SolidBrush(cellForeColor))
                            {
                                g.DrawString(cell.CellValue?.ToString() ?? "", Font, textBrush, TargetRect,
                                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                            }
                            break;
                    }
                }
            }

            // Reset quality settings
            if (DpiScaleFactor > 1.5f)
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Default;
            }
        }
        private void DrawRowsBorders(Graphics g, Rectangle bounds)
        {
            int yOffset = bounds.Top;
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    var row = Rows[i];
                    yOffset += row.Height;
                    if (yOffset < bounds.Bottom)
                        g.DrawLine(pen, bounds.Left, yOffset, bounds.Right, yOffset);
                }
            }
        }
        private void DrawColumnBorders(Graphics g, Rectangle bounds)
        {
            int xOffset = bounds.Left;
            int stickyWidth = _stickyWidth;
            var stickyColumns = Columns.Where(c => c.Sticked && c.Visible).ToList();

            // Draw sticky column borders
            using (Region clipRegion = new Region(new Rectangle(bounds.Left, bounds.Top, stickyWidth, bounds.Height)))
            {
                g.Clip = clipRegion;
                foreach (var col in stickyColumns)
                {
                    xOffset += col.Width;
                    if (xOffset < bounds.Left + stickyWidth) // Internal sticky borders
                    {
                        using (Pen borderPen = new Pen(_currentTheme.GridLineColor))
                        {
                            g.DrawLine(borderPen, xOffset, bounds.Top, xOffset, bounds.Bottom);
                        }
                    }
                }
            }

            // Draw scrolling column borders
            using (Region clipRegion = new Region(new Rectangle(bounds.Left + stickyWidth, bounds.Top, bounds.Width - stickyWidth, bounds.Height)))
            {
                g.Clip = clipRegion;
                xOffset = bounds.Left + stickyWidth - _xOffset; // Start after sticky, shift with _xOffset
                foreach (var col in Columns.Where(c => !c.Sticked && c.Visible))
                {
                    xOffset += col.Width;
                    using (Pen borderPen = new Pen(_currentTheme.GridLineColor))
                    {
                        g.DrawLine(borderPen, xOffset, bounds.Top, xOffset, bounds.Bottom);
                    }
                }
            }

            // Separator after sticky columns
            if (stickyWidth > 0)
            {
                g.ResetClip();
                using (Pen borderPen = new Pen(_currentTheme.GridLineColor))
                {
                    g.DrawLine(borderPen, bounds.Left + stickyWidth, bounds.Top, bounds.Left + stickyWidth, bounds.Bottom);
                }
            }

            // //MiscFunctions.SendLog($"DrawColumnBorders: StickyWidth={stickyWidth}, LastXOffset={xOffset}, Bounds={bounds}, XOffset={_xOffset}");
        }
        private void DrawHeaderPanel(Graphics g, Rectangle rect)
        {
            using (var borderPen = new Pen(_currentTheme.BorderColor))
            {
                if (ShowHeaderPanelBorder)
                {
                    g.DrawLine(borderPen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                }
            }

            // Position the title label with proper padding
            int padding = 10;
            int titleX = rect.Left + padding;
            int titleY = rect.Top + (rect.Height - titleLabel.Height) / 2;
            titleLabel.Location = new Point(titleX, titleY);

            // Draw title label
            titleLabel.TextImageRelation = TextImageRelation.ImageBeforeText;

            if (Entity != null )
            {
                if (Entity?.EntityName != null)
                {
                    titleLabel.Text = Entity.EntityName;
                }
            }
            else
            {
                titleLabel.Text = TitleText;
            }

            titleLabel.Draw(g, rect);

            // Calculate right margin and control dimensions
            int rightMargin = padding;

            // Configure and position the combo box at the right side of the panel
            int comboBoxWidth = 120;
            int filterTextBoxWidth = 150;
            int controlHeight = Math.Min(24, rect.Height);

            // Position ComboBox on the right side with proper margin
            int comboBoxX = rect.Right - comboBoxWidth - rightMargin;
            int comboBoxY = rect.Top + (rect.Height - controlHeight) / 2; // Center vertically
            filterColumnComboBox.Location = new Point(comboBoxX, comboBoxY);
            filterColumnComboBox.Size = new Size(comboBoxWidth, controlHeight);

            // Position TextBox to the left of ComboBox with proper spacing
            int filterTextBoxX = comboBoxX - filterTextBoxWidth - rightMargin;
            int filterTextBoxY = comboBoxY; // Same Y as the combo box
            filterTextBox.Location = new Point(filterTextBoxX, filterTextBoxY);
            filterTextBox.Size = new Size(filterTextBoxWidth, controlHeight);

            // Position the percentage label (if needed)
            int percentageX = filterTextBoxX - percentageLabel.Width - padding;
            int percentageY = rect.Top + (rect.Height - percentageLabel.Height) / 2;
            percentageLabel.Location = new Point(percentageX, percentageY);
            percentageLabel.Visible = false;

            // Make sure title label is correctly positioned
            titleLabel.Location = new Point(titleX, titleY);
        }

        #endregion
        #region Selection Methods
        private void UpdateRowsSelection()
        {
            if (_showCheckboxes && Rows != null)
            {
                int selColumnIndex = _columns.FindIndex(c => c.IsSelectionCheckBox);
                if (selColumnIndex == -1) return;

                foreach (var row in Rows)
                {
                    int dataIndex = row.DisplayIndex; // Assuming DisplayIndex maps to _fullData
                    if (dataIndex >= 0 && dataIndex < _fullData.Count)
                    {
                        var wrapper = _fullData[dataIndex] as DataRowWrapper;
                        if (wrapper != null)
                        {
                            bool isSelected = _persistentSelectedRows.ContainsKey(wrapper.RowID) && _persistentSelectedRows[wrapper.RowID];
                            var cell = row.Cells[selColumnIndex];
                            if (cell.UIComponent is BeepCheckBoxBool checkBox)
                            {
                                checkBox.State = isSelected ? CheckBoxState.Checked : CheckBoxState.Unchecked;
                            }
                            else
                            {
                                cell.CellValue = isSelected; // Fallback if no UIComponent
                            }
                        }
                    }
                }
                //MiscFunctions.SendLog($"UpdateRowsSelection: Updated {Rows.Count} rows with selection state");
            }
        }

        private void SelectAllCheckBox_StateChanged(object sender, EventArgs e)
        {
            bool selectAll = _selectAllCheckBox.State == CheckBoxState.Checked;
            _selectedRows.Clear();
            _selectedgridrows.Clear();

            if (selectAll)
            {
                for (int i = 0; i < _fullData.Count; i++)
                {
                    _selectedRows.Add(i);
                    if (i >= _dataOffset && i < _dataOffset + Rows.Count)
                    {
                        _selectedgridrows.Add(Rows[i - _dataOffset]);
                    }
                }
            }

            _persistentSelectedRows.Clear();
            if (selectAll)
            {
                foreach (var row in _fullData.OfType<DataRowWrapper>())
                {
                    _persistentSelectedRows[row.RowID] = true;
                }
            }
            UpdateRowsSelection();
            RaiseSelectedRowsChanged();
            Invalidate();
        }

        private void UpdateSelectionState()
        {
            if (_showCheckboxes && _fullData.Any())
            {
                bool allSelected = _fullData.All(d =>
                {
                    var wrapper = d as DataRowWrapper;
                    return wrapper != null && _persistentSelectedRows.ContainsKey(wrapper.RowID) && _persistentSelectedRows[wrapper.RowID];
                });
                _selectAllCheckBox.State = allSelected ? CheckBoxState.Checked : CheckBoxState.Unchecked;
            }
            else
            {
                _selectAllCheckBox.State = CheckBoxState.Unchecked;
            }
        }
        #endregion 
        #region Aggregation Panel
        private object ComputeAggregation(BeepColumnConfig column, List<object> data)
        {
            if (column.AggregationType == AggregationType.None || data == null || !data.Any())
                return null;
            var wrappedData = data.Cast<DataRowWrapper>().Where(w => w != null && w.RowState != DataRowState.Deleted);
           

            // Handle unbound columns by getting value from the aggregation row
            // Aggregate values from Rows for unbound columns
            if (column.IsUnbound)
            {
                if(column.AggregationType==  AggregationType.Count)
                {
                    return wrappedData.Count();
                }
                if (Rows == null || !Rows.Any())
                    return "";

                int columnIndex = Columns.IndexOf(column);
                if (columnIndex < 0 || columnIndex >= Columns.Count)
                    return "";

                var cellValues = Rows
                    .Select(r => r.Cells.Count > columnIndex ? r.Cells[columnIndex].CellValue : null)
                    .Where(v => v != null);

                if (!cellValues.Any())
                    return "";

                switch (column.AggregationType)
                {
                    case AggregationType.Sum:
                        if (cellValues.All(v => IsNumeric(v)))
                            return ConvertValuesToDouble(cellValues).Sum();
                        return "";

                    case AggregationType.Average:
                        if (cellValues.All(v => IsNumeric(v)))
                            return ConvertValuesToDouble(cellValues).Average();
                        return "";

                    case AggregationType.Count:
                        return cellValues.Count();

                    case AggregationType.Min:
                        if (cellValues.All(v => IsComparable(v)))
                            return cellValues.Cast<IComparable>().Min();
                        return "";

                    case AggregationType.Max:
                        if (cellValues.All(v => IsComparable(v)))
                            return cellValues.Cast<IComparable>().Max();
                        return "";

                    case AggregationType.First:
                        return cellValues.FirstOrDefault();

                    case AggregationType.Last:
                        return cellValues.LastOrDefault();

                    case AggregationType.DistinctCount:
                        return cellValues.Distinct().Count();

                    case AggregationType.Custom:
                        return "Custom (Not Implemented)";

                    default:
                        return null;
                }
            }

            var property = column.ColumnName != null ? wrappedData.FirstOrDefault()?.OriginalData?.GetType().GetProperty(column.ColumnName) : null;

            IEnumerable<object> values = wrappedData.Select(w => property.GetValue(w.OriginalData)).Where(v => v != null);

            if (property == null)
                return "";
            switch (column.AggregationType)
            {
                case AggregationType.Sum:
                    if (IsNumericType(property.PropertyType))
                        return values.Cast<double>().Sum();
                    return "";

                case AggregationType.Average:
                    if (IsNumericType(property.PropertyType))
                        return values.Cast<double>().Average();
                    return "";

                case AggregationType.Count:
                    return values.Count();

                case AggregationType.Min:
                    if (IsNumericType(property.PropertyType) || property.PropertyType == typeof(DateTime))
                        return values.Cast<IComparable>().Min();
                    return "";

                case AggregationType.Max:
                    if (IsNumericType(property.PropertyType) || property.PropertyType == typeof(DateTime))
                        return values.Cast<IComparable>().Max();
                    return "";

                case AggregationType.First:
                    return values.FirstOrDefault();

                case AggregationType.Last:
                    return values.LastOrDefault();

                case AggregationType.DistinctCount:
                    return values.Distinct().Count();

                case AggregationType.Custom:
                    // Implement custom aggregation logic here (e.g., via a delegate or configuration)
                    return "Custom (Not Implemented)";

                default:
                    return null;
            }
        }
        private bool IsComparable(object value)
        {
            return value is IComparable || IsNumeric(value) || value is DateTime;
        }
        // Helper methods to handle type conversion and validation
        private bool IsNumeric(object value)
        {
            return value is sbyte || value is byte || value is short || value is ushort ||
                   value is int || value is uint || value is long || value is ulong ||
                   value is float || value is double || value is decimal;
        }
        private IEnumerable<double> ConvertValuesToDouble(IEnumerable<object> values)
        {
            return values.Select(v => Convert.ToDouble(v));
        }
        private bool IsNumericType(Type type)
        {
            return type == typeof(int) ||
                   type == typeof(long) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal);
        }
        #endregion Aggregation Panel
        #region Filter Panel
        private List<object> _filteredData; // Add this field to BeepSimpleGrid to store the filtered data

        /// <summary>
        /// Applies a filter to the grid based on the given text and column name.
        /// </summary>
        /// <param name="filterText">The text to filter by.</param>
        /// <param name="columnName">The name of the column to filter on, or null/empty for a global search.</param>
        private void ApplyFilter(string filterText, string columnName)
        {
            // Initialize _filteredData if null
            if (_filteredData == null)
            {
                _filteredData = new List<object>();
            }

            // Handle cases where data is not available
            if (_fullData == null || !originalList.Any())
            {
                //MiscFunctions.SendLog("ApplyFilter: Cannot apply filter - _fullData is null or originalList is empty");
                _filteredData.Clear();
                _fullData?.Clear(); // Clear _fullData if it exists, but don't reset to originalList
                _dataOffset = 0;
                InitializeRows(); // Reinitialize rows to reflect empty state
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
                OnFilterChanged();
                return;
            }

            // If filter text is empty, reset to original data
            if (string.IsNullOrWhiteSpace(filterText))
            {
                _filteredData.Clear();
                _filteredData.AddRange(originalList); // Reset to original wrapped data
                _fullData.Clear();
                _fullData.AddRange(originalList); // Restore _fullData to the original state
                _dataOffset = 0; // Reset scroll position
                InitializeRows(); // Reinitialize rows to match the reset data
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
                OnFilterChanged(); // Notify that the filter has been reset
                return;
            }

            // Use _fullData as the source for filtering to apply the filter to the current working data
            filterText = filterText.ToLowerInvariant();
            List<object> filteredData;

            try
            {
                if (string.IsNullOrEmpty(columnName)) // Global search (All Columns)
                {
                    filteredData = _fullData.Where(wrapper =>
                    {
                        var dataItem = wrapper as DataRowWrapper;
                        if (dataItem == null || dataItem.OriginalData == null) return false;

                        var item = dataItem.OriginalData;
                        return Columns.Where(c => c.Visible).Any(col =>
                        {
                            try
                            {
                                var prop = item.GetType().GetProperty(col.ColumnName);
                                if (prop == null) return false; // Property doesn't exist, skip
                                var value = prop.GetValue(item);
                                return value != null && value.ToString().ToLowerInvariant().Contains(filterText);
                            }
                            catch (Exception ex)
                            {
                                //MiscFunctions.SendLog($"ApplyFilter (Global): Error accessing property {col.ColumnName} on item {item}: {ex.Message}");
                                return false;
                            }
                        });
                    }).ToList();
                }
                else // Column-specific search
                {
                    // Validate columnName
                    var column = Columns.FirstOrDefault(c => c.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                    if (column == null)
                    {
                        //MiscFunctions.SendLog($"ApplyFilter: Column '{columnName}' not found, resetting filter");
                        filteredData = new List<object>(originalList);
                        _filteredData.Clear();
                        _filteredData.AddRange(originalList);
                        _fullData.Clear();
                        _fullData.AddRange(originalList);
                    }
                    else
                    {
                        filteredData = _fullData.Where(wrapper =>
                        {
                            var dataItem = wrapper as DataRowWrapper;
                            if (dataItem == null || dataItem.OriginalData == null) return false;

                            var item = dataItem.OriginalData;
                            try
                            {
                                var prop = item.GetType().GetProperty(columnName);
                                if (prop == null) return false; // Property doesn't exist, skip
                                var value = prop.GetValue(item);
                                return value != null && value.ToString().ToLowerInvariant().Contains(filterText);
                            }
                            catch (Exception ex)
                            {
                                //MiscFunctions.SendLog($"ApplyFilter (Column-Specific): Error accessing property {columnName} on item {item}: {ex.Message}");
                                return false;
                            }
                        }).ToList();
                    }
                }

                // Update the filtered data and grid state
                _filteredData.Clear();
                _filteredData.AddRange(filteredData);

                // Update _fullData to reflect the filtered data (still as DataRowWrapper objects)
                _fullData.Clear();
                _fullData.AddRange(filteredData);

                // Reset scroll position
                _dataOffset = 0;

                // Reinitialize rows to match the new data count
                InitializeRows();
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();

                // Notify that the filter has changed
                OnFilterChanged();
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"ApplyFilter: Failed to apply filter - {ex.Message}");
                // Reset to original data on failure
                _filteredData.Clear();
                _filteredData.AddRange(originalList);
                _fullData.Clear();
                _fullData.AddRange(originalList);
                _dataOffset = 0;
                InitializeRows();
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
                OnFilterChanged();
            }
            var sortColumn = Columns.FirstOrDefault(c => c.SortDirection != SortDirection.None);
            if (sortColumn != null)
            {
                ApplySortAndFilter(); // Use combined method
            }
            else
            {
                // Your existing filter-only logic
                Invalidate();
                OnFilterChanged();
            }
        }

        /// <summary>
        /// Event to notify that the filter has changed.
        /// </summary>
        public event EventHandler FilterChanged;

        protected virtual void OnFilterChanged()
        {
            FilterChanged?.Invoke(this, EventArgs.Empty);
        }
        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            string selectedColumn = filterColumnComboBox.SelectedItem is SimpleItem item && item.Value != null
                ? item.Value.ToString()
                : null;
            ApplyFilter(filterTextBox.Text, selectedColumn);
        }
        private void FilterButton_Click(object sender, MouseEventArgs e)
        {
            ShowFilter = !ShowFilter; // Toggle filter panel visibility
        }
        private void FilterColumnComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedColumn = filterColumnComboBox.SelectedItem is SimpleItem item && item.Value != null
                ? item.Value.ToString()
                : null;
            if (selectedColumn == null) return;
            if(_editingControl != null) CloseCurrentEditor();
            ApplyFilter(filterTextBox.Text, selectedColumn);
        }
        #endregion Filter Panel
        #region Cell Editing
        public void SelectCell(int rowIndex, int columnIndex)
        {
            if (IsEditorShown)
            {
                CloseCurrentEditor();
            }

            if (rowIndex < 0 || rowIndex >= Rows.Count) return;
            if (columnIndex < 0 || columnIndex >= Columns.Count) return;

            // Store previous selection
            var previousRowIndex = _currentRowIndex;
            var previousCell = _selectedCell;

            _currentRowIndex = rowIndex;
            _selectedColumnIndex = columnIndex;
            _selectedCell = Rows[rowIndex].Cells[columnIndex];

            CurrentRow = Rows[rowIndex];
            CurrentRow.RowData = _fullData[_dataOffset + rowIndex];

            // Only invalidate the changed rows
            if (previousRowIndex != rowIndex)
            {
                if (previousRowIndex >= 0 && previousRowIndex < Rows.Count)
                {
                    InvalidateRowOptimized(previousRowIndex);
                }
                InvalidateRowOptimized(rowIndex);
            }

            // Raise events
            CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(_dataOffset + rowIndex, CurrentRow));
            CurrentCellChanged?.Invoke(this, new BeepCellSelectedEventArgs(rowIndex, columnIndex, _selectedCell));

            UpdateRecordNumber();
            // DON'T call FillVisibleRows here!
        }
        // Add optimized row invalidation
        private void InvalidateRowOptimized(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count) return;

            var row = Rows[rowIndex];
            Rectangle rowRect = new Rectangle(
                gridRect.Left,
                gridRect.Top + (rowIndex * _rowHeight),
                gridRect.Width,
                _rowHeight
            );

            Invalidate(rowRect);
        }
        public void SelectCell(BeepCellConfig cell)
        {
            if (cell == null) return;
            _editingRowIndex = cell.RowIndex; // Absolute index in _fullData
            SelectCell(cell.RowIndex, cell.ColumnIndex);
        }

        private BeepCellConfig GetCellAtLocation(Point location)
        {
            // Use cached DPI values
            UpdateDpiCache();

            if (!gridRect.Contains(location))
                return null;

            int yRelative = location.Y - gridRect.Top;
            if (yRelative < 0)
                return null;

            // Use binary search for row finding in high DPI scenarios
            int rowIndex = -1;
            if (Rows.Count > 50 && DpiScaleFactor > 1.5f)
            {
                rowIndex = BinarySearchRow(yRelative);
            }
            else
            {
                // Original linear search for small datasets
                int currentY = 0;
                for (int i = 0; i < Rows.Count; i++)
                {
                    int rowHeight = Rows[i].Height;
                    if (yRelative >= currentY && yRelative < currentY + rowHeight)
                    {
                        rowIndex = i;
                        break;
                    }
                    currentY += rowHeight;
                }
            }

            if (rowIndex == -1 || rowIndex >= Rows.Count)
                return null;

            // Rest of the method remains the same...
            var row = Rows[rowIndex];
            int xRelative = location.X - gridRect.Left;
            int stickyWidthTotal = _stickyWidth;

            // Find column using cached widths
            if (xRelative < stickyWidthTotal)
            {
                int currentX = 0;
                for (int i = 0; i < Columns.Count; i++)
                {
                    var column = Columns[i];
                    if (!column.Visible || !column.Sticked) continue;
                    int width = _scaledColumnWidths.ContainsKey(i) ? _scaledColumnWidths[i] : column.Width;
                    if (xRelative >= currentX && xRelative < currentX + width)
                    {
                        return row.Cells[i];
                    }
                    currentX += width;
                }
            }
            else
            {
                int xAdjusted = xRelative + _xOffset;
                int currentX = stickyWidthTotal;
                for (int i = 0; i < Columns.Count; i++)
                {
                    var column = Columns[i];
                    if (!column.Visible || column.Sticked) continue;
                    int width = _scaledColumnWidths.ContainsKey(i) ? _scaledColumnWidths[i] : column.Width;
                    if (xAdjusted >= currentX && xAdjusted < currentX + width)
                    {
                        return row.Cells[i];
                    }
                    currentX += width;
                }
            }

            return null;
        }

        // Add binary search helper for large row counts
        private int BinarySearchRow(int yRelative)
        {
            int left = 0;
            int right = Rows.Count - 1;
            int currentY = 0;

            while (left <= right)
            {
                int mid = (left + right) / 2;

                // Calculate Y position of mid row
                currentY = 0;
                for (int i = 0; i < mid; i++)
                {
                    currentY += Rows[i].Height;
                }

                if (yRelative < currentY)
                {
                    right = mid - 1;
                }
                else if (yRelative >= currentY + Rows[mid].Height)
                {
                    left = mid + 1;
                }
                else
                {
                    return mid;
                }
            }

            return -1;
        }
        private Rectangle GetCellRectangleIn(BeepCellConfig cell)
        {
            if (cell == null)
            {
                //MiscFunctions.SendLog("GetCellRectangleIn: Cell is null");
                return Rectangle.Empty;
            }

            // Find cell’s row and column indices
            int rowIndex = -1;
            int colIndex = -1;
            for (int r = 0; r < Rows.Count; r++)
            {
                int c = Rows[r].Cells.IndexOf(cell);
                if (c != -1)
                {
                    rowIndex = r;
                    colIndex = c;
                    break;
                }
            }
            if (rowIndex == -1 || colIndex == -1)
            {
                //MiscFunctions.SendLog("GetCellRectangleIn: Cell not found in Rows");
                return Rectangle.Empty;
            }

            // Calculate Y position by summing row heights up to this row
            int y = gridRect.Top;
            for (int i = 0; i < rowIndex; i++)
            {
                y += Rows[i].Height;
            }

            // Calculate X position
            int x = gridRect.Left;
            var column = Columns[colIndex];
            if (column.Sticked)
            {
                // Sticky columns start at gridRect.Left with offset
                for (int i = 0; i < colIndex; i++)
                {
                    if (Columns[i].Visible && Columns[i].Sticked)
                        x += Columns[i].Width;
                }
            }
            else
            {
                // Non-sticky columns adjust for _xOffset and sticky width
                x += _stickyWidth - _xOffset;
                for (int i = 0; i < colIndex; i++)
                {
                    if (Columns[i].Visible && !Columns[i].Sticked)
                        x += Columns[i].Width;
                }
            }

            int width = column.Width;
            int height = Rows[rowIndex].Height;

            Rectangle rect = new Rectangle(x, y, width, height);
            //MiscFunctions.SendLog($"GetCellRectangleIn: Cell={x},{y}, Size={width}x{height}, RowIndex={rowIndex}, ColIndex={colIndex}");
            return rect;
        }
        #endregion Cell Editing
        #region Scrollbar Management
        private int _cachedTotalColumnWidth = 0; // Cache total column width
        private int _cachedMaxXOffset = 0; // Cache maximum horizontal offset
        private bool _scrollBarsUpdatePending = false;

        private void StartSmoothScroll(int targetVertical, int targetHorizontal = -1)
        {
            // Calculate the maximum possible offset
            int maxOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());

            // Clamp the target vertical scroll position within the valid range
            targetVertical = Math.Max(0, Math.Min(targetVertical, maxOffset));

            int verticalDistance = Math.Abs(_dataOffset - targetVertical);
            int horizontalDistance = targetHorizontal >= 0 ? Math.Abs(_xOffset - targetHorizontal) : 0;

            // For small distances, update immediately without animation
            if (verticalDistance <= 3 && horizontalDistance <= 50)
            {
                _dataOffset = targetVertical;
                if (targetHorizontal >= 0) _xOffset = targetHorizontal;

                FillVisibleRows();
                UpdateScrollBars();
                Invalidate(gridRect); // Only invalidate grid area
                return;
            }

            // Use animation only for larger movements
            // ... existing smooth scroll code ...
        }
        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            bool updated = false;
            double easingFactor = 0.2;

            // ... existing scroll calculation code ...

            if (!updated)
            {
                if (_scrollTimer != null)
                {
                    _scrollTimer.Stop();
                    _scrollTimer.Dispose();
                    _scrollTimer = null;
                }
            }
            else
            {
                UpdateCellPositions();
                // Only call FillVisibleRows if data offset actually changed
                if (Math.Abs(_dataOffset - _scrollTargetVertical) > 0)
                {
                    FillVisibleRows();
                }
                else
                {
                    // Just invalidate the scrolled region
                    Invalidate(gridRect);
                }
            }
        }

        private void UpdateHeaderLayout()
        {
            if (_showCheckboxes && _showColumnHeaders && _columns.Any(c => c.IsSelectionCheckBox))
            {
                var selColumn = _columns.First(c => c.IsSelectionCheckBox);
                int colIndex = _columns.IndexOf(selColumn);
                int x = gridRect.X + (colIndex == 0 ? 0 : _columns.Take(colIndex).Sum(c => c.Width)) - _xOffset;
                int y = columnsheaderPanelRect.Y + (columnsheaderPanelRect.Height - _selectAllCheckBox.CheckBoxSize) / 2;

                // Ensure the checkbox stays within the header bounds
                x = Math.Max(columnsheaderPanelRect.Left, Math.Min(x, columnsheaderPanelRect.Right - _selectAllCheckBox.CheckBoxSize));
                y = Math.Max(columnsheaderPanelRect.Top, Math.Min(y, columnsheaderPanelRect.Bottom - _selectAllCheckBox.CheckBoxSize));
                // Temporary hardcoded position for debugging
                // x = 10; y = 10; // Uncomment to test at a fixed position
                _selectAllCheckBox.Location = new Point(x + (_selectionColumnWidth - _selectAllCheckBox.CheckBoxSize) / 2, y);
                _selectAllCheckBox.Size = new Size(_selectAllCheckBox.CheckBoxSize, _selectAllCheckBox.CheckBoxSize);
                _selectAllCheckBox.Visible = true;
                _selectAllCheckBox.BringToFront(); // Ensure it’s on top of any custom painting
                // //MiscFunctions.SendLog($"SelectAllCheckBox positioned at X={_selectAllCheckBox.Location.X}, Y={_selectAllCheckBox.Location.Y}, Visible={_selectAllCheckBox.Visible}");
            }
            else
            {
                _selectAllCheckBox.Visible = false;
                // //MiscFunctions.SendLog("SelectAllCheckBox hidden due to conditions not met");
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (_verticalScrollBar.Visible)
            {
                int delta = e.Delta > 0 ? -_verticalScrollBar.SmallChange : _verticalScrollBar.SmallChange;
                StartSmoothScroll(_dataOffset + delta);
            }
        }
        private void ScrollBy(int delta)
        {
            int newOffset = _dataOffset + delta;
            int maxOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
            newOffset = Math.Max(0, Math.Min(newOffset, maxOffset));
            if (newOffset != _dataOffset)
            {
                StartSmoothScroll(newOffset);
            }
        }
        private void UpdateScrollBarsCore()
        {
            if (DesignMode)
                return;
            if (_verticalScrollBar == null || _horizontalScrollBar == null)
                return;

            if (_fullData == null || !_fullData.Any())
            {
                _verticalScrollBar.Visible = false;
                _horizontalScrollBar.Visible = false;
                return;
            }

            int totalRowHeight = (_fullData.Count * RowHeight) + (_showaggregationRow ? bottomagregationPanelHeight : 0);
            int visibleHeight = gridRect.Height;
            int visibleRowCount = GetVisibleRowCount();
            int aggregationRows = _showaggregationRow ? 1 : 0;

            int maxOffset = Math.Max(0, _fullData.Count - visibleRowCount);

            if (_showVerticalScrollBar && _fullData.Count >= visibleRowCount)
            {
                _verticalScrollBar.Minimum = 0;
                _verticalScrollBar.Maximum = maxOffset + visibleRowCount;
                _verticalScrollBar.LargeChange = visibleRowCount;
                _verticalScrollBar.SmallChange = 1;
                _verticalScrollBar.Value = Math.Min(_dataOffset, maxOffset);
                _verticalScrollBar.Visible = true;
            }
            else
            {
                if (_verticalScrollBar.Visible)
                {
                    _verticalScrollBar.Visible = false;
                    _dataOffset = 0;
                    FillVisibleRows();
                }
            }

            int totalColumnWidth = Columns.Where(o => o.Visible).Sum(col => col.Width);
            int visibleColumnCount = Columns.Count(o => o.Visible);
            int borderWidth = 1;
            int totalBorderWidth = visibleColumnCount > 0 ? (visibleColumnCount - 1) * borderWidth : 0;
            totalColumnWidth += totalBorderWidth;
            int visibleWidth = gridRect.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);

            bool horizontalScrollNeeded = _showHorizontalScrollBar && totalColumnWidth > visibleWidth;
            if (horizontalScrollNeeded)
            {
                int maxXOffset = Math.Max(0, totalColumnWidth - visibleWidth);
                _horizontalScrollBar.Minimum = 0;
                _horizontalScrollBar.Maximum = totalColumnWidth;
                _horizontalScrollBar.LargeChange = visibleWidth;
                _horizontalScrollBar.SmallChange = Columns.Where(c => !c.Sticked && c.Visible).Min(c => c.Width) / 2;
                _horizontalScrollBar.Value = Math.Max(0, Math.Min(_xOffset, maxXOffset));
                _horizontalScrollBar.Visible = true;

                //  //MiscFunctions.SendLog($"UpdateScrollBars Horizontal: totalColumnWidth={totalColumnWidth}, visibleWidth={visibleWidth}, stickyWidth={stickyWidth}, maxXOffset={maxXOffset}, _xOffset={_xOffset}");
            }
            else
            {
                if (_horizontalScrollBar.Visible)
                {
                    _horizontalScrollBar.Visible = false;
                    _xOffset = 0;
                }
            }
            UpdateHeaderLayout();
        }
        private void UpdateScrollBars()
        {
            // Check if we can safely update scrollbars
            if (!IsHandleCreated || DesignMode)
            {
                // If handle isn't created yet, we can't use BeginInvoke
                // Just return - scrollbars will be updated later when handle is created
                return;
            }

            if (_scrollBarsUpdatePending) return;

            _scrollBarsUpdatePending = true;

            // Use BeginInvoke only if handle exists
            if (IsHandleCreated && !IsDisposed)
            {
                BeginInvoke(new Action(() =>
                {
                    _scrollBarsUpdatePending = false;
                    UpdateScrollBarsCore();
                }));
            }
            else
            {
                // If we can't use BeginInvoke, just reset the flag
                _scrollBarsUpdatePending = false;
            }
        }

        private void UpdateCachedHorizontalMetrics()
        {
            int visibleColumnCount = Columns.Count(o => o.Visible);
            int borderWidth = 1; // Adjust if your border thickness differs
            int totalBorderWidth = visibleColumnCount > 0 ? (visibleColumnCount - 1) * borderWidth : 0; // Between columns only
            _cachedTotalColumnWidth = Columns.Where(o => o.Visible).Sum(col => col.Width) + totalBorderWidth;
            _cachedMaxXOffset = Math.Max(0, _cachedTotalColumnWidth - (gridRect.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0)));
        }
        private int GetVisibleRowCount()
        {
            if (_fullData == null || _fullData.Count == 0)
            {
                //MiscFunctions.SendLog("GetVisibleRowCount: No data available");
                return 0;
            }

            // Calculate how many rows can fit in the visible area based on row height
            int availableHeight = gridRect.Height;
            if (availableHeight <= 0)
            {
                //MiscFunctions.SendLog("GetVisibleRowCount: Invalid grid height");
                return 1;
            }

            // If we have custom row heights, calculate based on actual rows starting from _dataOffset
            if (_rowHeights.Any())
            {
                int totalHeight = 0;
                int visibleCount = 0;

                for (int i = _dataOffset; i < _fullData.Count; i++)
                {
                    // Get the height for this data index
                    int rowHeight = _rowHeights.ContainsKey(i) ? _rowHeights[i] : _rowHeight;

                    if (totalHeight + rowHeight > availableHeight)
                        break;

                    totalHeight += rowHeight;
                    visibleCount++;
                }

                return Math.Max(1, visibleCount);
            }
            else
            {
                // Simple calculation when all rows have the same height
                int visibleCount = availableHeight / _rowHeight;
                return Math.Max(1, Math.Min(visibleCount, _fullData.Count - _dataOffset));
            }
        }
        private void PositionScrollBars()
        {
            if (_verticalScrollBar == null || _horizontalScrollBar == null) return;

            int verticalScrollWidth = _verticalScrollBar.Width;
            int horizontalScrollHeight = _horizontalScrollBar.Height;
            int visibleHeight = gridRect.Height; // Use gridRect for row area
            int visibleWidth = gridRect.Width;

            // Calculate the bottom position considering the aggregation row
            int aggregationRowBottom = bottomPanelY + (_showaggregationRow ? bottomagregationPanelHeight : 0);

            if (_verticalScrollBar.Visible)
            {
                _verticalScrollBar.Location = new Point(gridRect.Right - verticalScrollWidth, gridRect.Top); // Align with gridRect top
                _verticalScrollBar.Height = gridRect.Height;// aggregationRowBottom - gridRect.Top - (_horizontalScrollBar.Visible ? horizontalScrollHeight : 0);
            }

            if (_horizontalScrollBar.Visible)
            {
                _horizontalScrollBar.Location = new Point(gridRect.Left, aggregationRowBottom);
                _horizontalScrollBar.Width = visibleWidth ;
            }
        }
        private void VerticalScrollBar_Scroll(object sender, EventArgs e)
        {
            // Immediately apply the new value without animation for direct clicks
            _dataOffset = _verticalScrollBar.Value;

            // Stop any ongoing animations first
            if (_scrollTimer != null && _scrollTimer.Enabled)
            {
                _scrollTimer.Stop();
            }

            // Update UI
            FillVisibleRows();
            UpdateCellPositions();
            MoveEditorIn(); // Move editor if active
            Invalidate();
        }

        private void HorizontalScrollBar_Scroll(object sender, EventArgs e)
        {
            // Immediately apply the new value without animation for direct clicks
            _xOffset = _horizontalScrollBar.Value;

            // Stop any ongoing animations first
            if (_scrollTimer != null && _scrollTimer.Enabled)
            {
                _scrollTimer.Stop();
            }

            // Update UI
            UpdateCellPositions();
            MoveEditorIn(); // Move editor if active
            Invalidate();
        }

        //private void VerticalScrollBar_ValueChanged(object sender, EventArgs e)
        //{
        //    StartSmoothScroll(_verticalScrollBar.Value);
        //}
       
        //private void HorizontalScrollBar_ValueChanged(object sender, EventArgs e)
        //{
        //    StartSmoothScroll(_dataOffset, _horizontalScrollBar.Value);


        //}
        private void UpdateCellPositions()
        {
            if (Rows == null || Rows.Count == 0)
            {
                //MiscFunctions.SendLog("UpdateCellPositions: No rows to update");
                return;
            }
            // Clear cache when positions need to be recalculated
            _cellBounds.Clear();
            // yOffset is now 0 since Rows only contains visible rows
            int currentY = gridRect.Top;

            // Update positions for all visible rows
            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                var row = Rows[rowIndex];
                row.UpperY = currentY;

                int x = gridRect.Left;
                int stickyWidthTotal = _stickyWidth;
                int scrollableX = x + stickyWidthTotal - _xOffset;

                for (int colIndex = 0; colIndex < Columns.Count; colIndex++)
                {
                    if (Columns[colIndex].Visible)
                    {
                        var cell = row.Cells[colIndex];
                        if (Columns[colIndex].Sticked)
                        {
                            cell.X = x;
                            x += Columns[colIndex].Width;
                        }
                        else
                        {
                            cell.X = scrollableX;
                            scrollableX += Columns[colIndex].Width;
                        }
                        cell.Y = row.UpperY;
                        cell.Width = Columns[colIndex].Width;
                        cell.Height = row.Height;
                    }
                }
               
                currentY += row.Height;
            }

            ////MiscFunctions.SendLog($"UpdateCellPositions: yOffset=0, xOffset={_xOffset}, VisibleRows={Rows.Count}, TotalRows={Rows.Count}");
        }
        private void UpdateRowCount()
        {
            if (_fullData == null) return;
            if (_fullData.Count == 0) return;
            RecalculateGridRect();
            visibleHeight = gridRect.Height;
            visibleRowCount = visibleHeight / _rowHeight;
            int dataRowCount = _fullData.Count;
            int currentRowCount = Rows.Count;

            // Calculate the number of regular rows needed, reserving space for aggregation row
            int requiredRegularRows = visibleRowCount - (_showaggregationRow ? 1 : 0);

            if (requiredRegularRows > currentRowCount)
            {
                // Add new regular rows
                int rowCountToAdd = requiredRegularRows - currentRowCount;
                int index = currentRowCount;

                for (int i = 0; i < rowCountToAdd; i++)
                {
                    var row = new BeepRowConfig
                    {
                        RowIndex = index + i,
                        DisplayIndex = _dataOffset + (index + i), // Map to visible data index
                        IsAggregation = false
                    };
                    foreach (var col in Columns)
                    {
                        var cell = new BeepCellConfig
                        {
                            CellValue = null,
                            CellData = null,
                            IsEditable = true,
                            ColumnIndex = col.Index,
                            IsVisible = col.Visible,
                            RowIndex = index + i,
                            IsAggregation = false
                        };
                        row.Cells.Add(cell);
                    }
                    Rows.Add(row); // Add at the end
                }
            }
        }
        /// <summary>
        /// Retrieves all rows from the data source. and not just displayed rows.
        /// and Fill data in all rows
        /// </summary>
        /// <summary>
        /// Retrieves all rows from the data source and ensures all rows are populated with data.
        /// Unlike FillVisibleRows(), this method processes the entire data set, not just visible rows.
        /// </summary>
        private void GetAllRows()
        {
            if (_fullData == null || _fullData.Count == 0)
                return;

            // Calculate how many rows we need to add/ensure exist
            int dataRowCount = _fullData.Count;
            int currentRowCount = Rows.Count;
            int rowsToAdd = dataRowCount - currentRowCount;

            // If we already have all rows, just update their data
            if (rowsToAdd <= 0)
            {
                // Update all existing rows with fresh data
                for (int i = 0; i < currentRowCount; i++)
                {
                    if (i < dataRowCount)
                    {
                        UpdateRowData(i, _fullData[i]);
                    }
                }
                return;
            }

            // Add new rows to match the full data set size
            for (int i = 0; i < rowsToAdd; i++)
            {
                int dataIndex = currentRowCount + i;
                var row = new BeepRowConfig
                {
                    RowIndex = dataIndex,
                    DisplayIndex = dataIndex,
                    IsAggregation = false,
                    Height = _rowHeights.ContainsKey(dataIndex) ? _rowHeights[dataIndex] : _rowHeight,
                    RowData = _fullData[dataIndex]
                };

                // Create cells for each column in the row
                foreach (var col in Columns)
                {
                    var cell = new BeepCellConfig
                    {
                        RowIndex = dataIndex,
                        ColumnIndex = col.Index,
                        IsVisible = col.Visible,
                        IsEditable = !col.ReadOnly,
                        IsAggregation = false,
                        Height = row.Height
                    };

                    // Get cell data based on column type
                    if (col.IsSelectionCheckBox)
                    {
                        var wrapper = _fullData[dataIndex] as DataRowWrapper;
                        if (wrapper != null)
                        {
                            int rowID = wrapper.RowID;
                            bool isSelected = _persistentSelectedRows.ContainsKey(rowID) && _persistentSelectedRows[rowID];
                            cell.CellValue = isSelected;
                            cell.CellData = isSelected;
                        }
                    }
                    else if (col.IsRowNumColumn)
                    {
                        cell.CellValue = dataIndex + 1;
                        cell.CellData = dataIndex + 1;
                    }
                    else if (col.IsRowID)
                    {
                        var wrapper = _fullData[dataIndex] as DataRowWrapper;
                        if (wrapper != null)
                        {
                            cell.CellValue = wrapper.RowID;
                            cell.CellData = wrapper.RowID;
                        }
                    }
                    else
                    {
                        // Process regular data column
                        var wrapper = _fullData[dataIndex] as DataRowWrapper;
                        if (wrapper != null && wrapper.OriginalData != null)
                        {
                            var prop = wrapper.OriginalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                            if (prop != null)
                            {
                                var value = prop.GetValue(wrapper.OriginalData);
                                cell.CellValue = value;
                                cell.CellData = value;
                            }
                        }
                    }

                    row.Cells.Add(cell);
                }

                row.IsDataLoaded = true;
                Rows.Add(row);
            }

            // Make sure tracking and scroll state are properly updated
            UpdateTrackingIndices();
            UpdateScrollBars();
            Invalidate();
        }

        // Helper method to update an existing row with fresh data
        private void UpdateRowData(int rowIndex, object dataItem)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count || dataItem == null)
                return;

            var row = Rows[rowIndex];
            row.RowData = dataItem;

            var wrapper = dataItem as DataRowWrapper;
            if (wrapper == null || wrapper.OriginalData == null)
                return;

            EnsureTrackingForItem(wrapper);

            foreach (var cell in row.Cells)
            {
                var col = Columns[cell.ColumnIndex];
                if (col.IsSelectionCheckBox)
                {
                    bool isSelected = _persistentSelectedRows.ContainsKey(wrapper.RowID) && _persistentSelectedRows[wrapper.RowID];
                    cell.CellValue = isSelected;
                    cell.CellData = isSelected;
                }
                else if (col.IsRowNumColumn)
                {
                    cell.CellValue = rowIndex + 1;
                    cell.CellData = rowIndex + 1;
                }
                else if (col.IsRowID)
                {
                    cell.CellValue = wrapper.RowID;
                    cell.CellData = wrapper.RowID;
                }
                else
                {
                    var prop = wrapper.OriginalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                    if (prop != null)
                    {
                        var value = prop.GetValue(wrapper.OriginalData);
                        cell.CellValue = value;
                        cell.CellData = value;
                    }
                }
            }

            row.IsDataLoaded = true;
        }

        #endregion
        #region Resizing Logic
        private System.Windows.Forms.Timer _resizeTimer;
        private bool _isResizing = false;
        private Size _pendingSize;
        private Size _lastCalculatedSize;
        public void SetColumnWidth(string columnName, int width)
        {
         
            var column = GetColumnByName(columnName); 
            if (column != null)
            {
                column.Width = Math.Max(20, width); // Ensure minimum width
              
                Invalidate(); // Redraw grid with updated column width
            }
            else
            {
              //   //MiscFunctions.SendLog($"SetColumnWidth: Column '{columnName}' not found.");
            }
        }
    
        private bool IsNearColumnBorder(Point location, out int columnIndex)
        {
            // First, check if the location is within the grid area
            if (!gridRect.Contains(location))
            {
                columnIndex = -1;
                return false;
            }

            // Calculate the X coordinate relative to the grid's content area
            int xRelative = location.X - gridRect.Left + XOffset;

            // Track current X position as we iterate through columns
            int currentX = 0;

            // Check each column
            for (int i = 0; i < Columns.Count; i++)
            {
                if (!Columns[i].Visible)
                    continue;

                currentX += Columns[i].Width;

                // Check if the point is within _resizeMargin pixels of this column's right edge
                if (Math.Abs(xRelative - currentX) <= _resizeMargin)
                {
                    columnIndex = i;
                    return true;
                }
            }

            columnIndex = -1;
            return false;
        }

        private bool IsNearRowBorder(Point location, out int rowIndex)
        {
            // Calculate the Y coordinate relative to the grid's drawing area (rows start at gridRect.Top)
            int yRelative = location.Y - gridRect.Top;
            if (yRelative < 0) // Above the grid
            {
                rowIndex = -1;
                return false;
            }

            // Iterate through rows to find the one containing yRelative
            int currentY = 0;
            for (int i = 0; i < Rows.Count; i++)
            {
                int rowHeight = Rows[i].Height; // Use individual row height
                int rowBottom = currentY + rowHeight;

                // Check if yRelative is within this row
                if (yRelative >= currentY && yRelative <= rowBottom)
                {
                    // Check if near the bottom border of this row
                    if (Math.Abs(yRelative - rowBottom) <= _resizeMargin)
                    {
                        rowIndex = i;
                        return true;
                    }
                    // If near the top border of the first row (optional)
                    if (i == 0 && Math.Abs(yRelative - currentY) <= _resizeMargin)
                    {
                        rowIndex = -1; // Could return 0 if resizing from top is allowed
                        return false; // Typically, we don’t resize from the top of the first row
                    }
                    rowIndex = -1;
                    return false; // Not near a border within this row
                }

                currentY += rowHeight; // Move to the next row’s top
            }

            // If beyond the last row
            rowIndex = -1;
            return false;
        }
        protected override void OnResize(EventArgs e)
        {
            // Only update critical elements during resize
            if (!_isResizing || _lastCalculatedSize == _pendingSize)
            {
                base.OnResize(e);
                return;
            }
            // For high DPI, increase debounce time
            if (_resizeTimer == null)
            {
                _resizeTimer = new System.Windows.Forms.Timer();
                _resizeTimer.Interval = DpiScaleFactor > 1.5f ? 350 : 250;
                _resizeTimer.Tick += ResizeTimer_Tick;
                _resizeTimer.Stop();
                _resizeTimer.Start();
            }

            _pendingSize = this.Size;
            _isResizing = true;

         

            // Suspend layout during resize for high DPI
            if (DpiScaleFactor > 1.5f)
            {
                SuspendLayout();
            }

            // Position scrollbars without full recalculation
            PositionScrollBars();

         

            if (DpiScaleFactor > 1.5f)
            {
                ResumeLayout(false);
            }
            _resizeTimer.Stop();
            // Now call base
            base.OnResize(e);
        }

        private void ResizeTimer_Tick(object sender, EventArgs e)
        {
            // The resize has ended, stop the timer
            _resizeTimer.Stop();

            // Now perform the expensive operations
            if (_isResizing && _lastCalculatedSize != _pendingSize)
            {
                SuspendLayout();
                // Perform expensive operations
                _lastCalculatedSize = _pendingSize;

                // Reset flags to force redraw of these elements
                _navigatorDrawn = false;
                _filterpaneldrawn = false;

                // Full refresh now that resizing has stopped
                UpdateDrawingRect();
                UpdateRowCount();
                FillVisibleRows();
                UpdateScrollBars();

                ResumeLayout();
                Invalidate();

                _isResizing = false;
            }
        }
        #endregion
        #region Editor
        // One editor control per column (keyed by column index)
        private Dictionary<string, IBeepUIComponent> _columnDrawers = new Dictionary<string, IBeepUIComponent>();
        private Dictionary<string, IBeepUIComponent> _columnEditors = new Dictionary<string, IBeepUIComponent>();
        // The currently active editor and cell being edited
        private BeepControl _editingControl = null;
        private BeepCellConfig _editingCell = null;
        private bool _isEndingEdit = false;
        private BeepCellConfig _tempcell = null;
      //  private IBeepComponentForm _editorPopupForm;
        private void MoveEditorIn()
        {
            if (_editingCell == null || _editingControl == null || !IsEditorShown)
            {
                //   //MiscFunctions.SendLog("MoveEditor: Skipped - null reference or editor not shown");
                return;
            }

            // Get the cell’s rectangle relative to gridRect
            Rectangle cellRect = GetCellRectangleIn(_editingCell);

            // Adjust for scrolling offsets
            int yOffset = _dataOffset * RowHeight;
            int xOffset = _xOffset;
            cellRect.Y -= yOffset; // Shift up by vertical scroll
                                   //  cellRect.X -= xOffset; // Shift left by horizontal scroll
            var column = Columns[_editingCell.ColumnIndex];
            if (!column.Sticked) // Only adjust X for scrollable columns
            {
                cellRect.X -= _xOffset; // Shift left by horizontal scroll
            }
            // Define grid bounds
            int gridLeft = 0; // Relative to gridRect’s client area
            int gridRight = gridRect.Width;
            int gridTop = 0;
            int gridBottom = gridRect.Height;
            // Define sticky column region
            int stickyWidthTotal = _stickyWidth; // Set in PaintRows
            int stickyLeft = gridRect.Left;
            int stickyRight = gridRect.Left + stickyWidthTotal;
            // Check if the editor overlaps the sticky column region (for scrollable columns only)
            bool overlapsStickyRegion = !column.Sticked && stickyWidthTotal > 0 &&
                                        cellRect.X < stickyRight && cellRect.Right > stickyLeft;
            // Check if the editor is fully out of view
            bool isFullyOutOfView =
                (cellRect.Right < gridLeft) ||  // Completely off to the left
                (cellRect.Left > gridRight) ||  // Completely off to the right
                (cellRect.Bottom < gridTop) ||  // Scrolled out past the top
                (cellRect.Top > gridBottom);    // Completely off below

            if (isFullyOutOfView || overlapsStickyRegion)
            {
                // //MiscFunctions.SendLog($"MoveEditor: Editor out of view - Hiding (CellRect={cellRect})");
                _editingControl.Visible = false;
            }
            else
            {
                // Position editor within gridRect’s client coordinates
                _editingControl.Location = new Point(cellRect.X, cellRect.Y);
                _editingControl.Visible = true;
                //    //MiscFunctions.SendLog($"MoveEditor: Editor moved to {cellRect.X},{cellRect.Y}");
            }
        }
        private void ShowCellEditor(BeepCellConfig cell, Point location)
        {
            if (ReadOnly) return;
            if (cell == null || !cell.IsEditable)
                return;

            int colIndex = cell.ColumnIndex;
            if (colIndex < 0 || colIndex >= Columns.Count) return;

            var column = Columns[colIndex];

            // Close any existing editor, committing changes.
            EndEdit(true);

            _editingCell = cell;
            _tempcell = cell;
            Size cellSize = new Size(column.Width, cell.Height);

            // Create or reuse editor control from a pool
            if (!_columnEditors.TryGetValue(column.ColumnName, out IBeepUIComponent columnEditor))
            {
                columnEditor = CreateCellControlForEditing(cell);
                _columnEditors[column.ColumnName] = columnEditor;
            }
            _editingControl = (BeepControl)columnEditor;
            _editingControl.SetValue(cell.CellValue); // Set initial value for the editor
         
            if (_editingControl == null) return;

            _editingControl.Size = cellSize;
            _editingControl.Location = cell.Rect.Location; // Use the cell's rectangle location directly
            _editingControl.Theme = Theme;

            // Ensure event handlers are attached
            _editingControl.KeyDown -= OnEditorKeyDown;
            _editingControl.KeyDown += OnEditorKeyDown;
            _editingControl.LostFocus -= LostFocusHandler;
            _editingControl.LostFocus += LostFocusHandler;
            _editingControl.TabKeyPressed -= Tabhandler;
            _editingControl.TabKeyPressed += Tabhandler;
            _editingControl.EscapeKeyPressed -= Cancelhandler;
            _editingControl.EscapeKeyPressed += Cancelhandler;

            UpdateCellControl(_editingControl, column, cell, cell.CellValue);

            // Add to grid’s Controls collection if not already present
            if (!this.Controls.Contains(_editingControl))
            {
                this.Controls.Add(_editingControl);
            }

            _editingControl.Visible = true;
            _editingControl.BringToFront();
            this.ActiveControl = _editingControl;
            IsEditorShown = true;
        }
        private void OnEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EndEdit(true);
                MoveNextCell();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Tab)
            {
                EndEdit(true);
                MoveNextCell();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CancelEdit();
                e.Handled = true;
            }
        }
        private void LostFocusHandler(object? sender, EventArgs e)
        {
            EndEdit(true);
        }
        private void Cancelhandler(object? sender, EventArgs e)
        {
            CancelEditing();
        }
        private void Tabhandler(object? sender, EventArgs e)
        {
           MoveNextCell();
        }
       
        private BeepCellConfig CloseCurrentEditor()
        {
            var previouslyEditingCell = _editingCell;
            EndEdit(true); // Commit changes
            return previouslyEditingCell;
        }
        private void SaveEditedValue()
        {
            if (_tempcell == null )
            {
           //     //MiscFunctions.SendLog($"⚠️ Editing control or cell is null!");
                return;
            }

            object newValue = _tempcell.CellValue;
          //  //MiscFunctions.SendLog($"🔄 Saving value: {newValue} (Old: {_editingCell.CellData})");

            // 🔹 Check if the new value is empty or null
            //if (newValue == null || string.IsNullOrWhiteSpace(newValue.ToString()))
            //{
            // //   //MiscFunctions.SendLog($"⚠️ New value is empty. Skipping update.");
            //    return;
            //}

            // 🔹 Retrieve PropertyType from the corresponding column
            BeepColumnConfig columnConfig = Columns.FirstOrDefault(c => c.Index == _tempcell.ColumnIndex);
            if (columnConfig == null)
            {
            //    //MiscFunctions.SendLog($"⚠️ Column config not found. Skipping update.");
                return;
            }
            Type propertyType = Type.GetType(columnConfig.PropertyTypeName, throwOnError: false) ?? typeof(string); // Default to string if null

             //🔹 Convert new value to the correct type before comparing
            object convertedNewValue = MiscFunctions.ConvertValueToPropertyType(propertyType, newValue);
            object convertedOldValue = MiscFunctions.ConvertValueToPropertyType(propertyType, _tempcell.CellData);

            // 🔹 Skip update if the new value is the same as the old value
            if (convertedNewValue != null && convertedOldValue != null && convertedNewValue.Equals(convertedOldValue))
            {
            //    //MiscFunctions.SendLog($"⚠️ New value is the same as old. Skipping update.");
                return;
            }

            // 🔹 Update cell's stored value
            _selectedCell.OldValue = _tempcell.CellData;
            _selectedCell.CellData =  convertedNewValue;
            _selectedCell.CellValue = convertedNewValue;
            _selectedCell.IsDirty = true;

            // 🔹 Update the corresponding data record
            UpdateDataRecordFromRow(_selectedCell);
            CellValueChanged?.Invoke(this, new BeepCellEventArgs(_selectedCell));
            // 🔹 Trigger validation if necessary
            _selectedCell.ValidateCell();

            //MiscFunctions.SendLog($"✅ Cell updated. New: {_selectedCell.CellValue}");
            _tempcell = null;
           // Invalidate(); // 🔹 Redraw grid if necessary
        }
        private void CancelEditing()
        {
            // Optionally, revert the editor's value if needed.
            CloseCurrentEditor();
        }



        private void EndEdit(bool acceptChanges)
        {
            if (_isEndingEdit || _editingControl == null || !IsEditorShown || _editingCell == null)
            {
                return;
            }

            _isEndingEdit = true;
            try
            {
                BeepCellConfig editedCell = _editingCell;

                if (acceptChanges)
                {
                    object newValue = _editingControl.GetValue();

                    BeepColumnConfig columnConfig = Columns[editedCell.ColumnIndex];
                    Type propertyType = Type.GetType(columnConfig.PropertyTypeName, throwOnError: false) ?? typeof(string);

                    object convertedNewValue = MiscFunctions.ConvertValueToPropertyType(propertyType, newValue);
                    object convertedOldValue = MiscFunctions.ConvertValueToPropertyType(propertyType, editedCell.CellData);

                    if (!Equals(convertedNewValue, convertedOldValue))
                    {
                        editedCell.CellValue = convertedNewValue;
                        editedCell.IsDirty = true;
                        if (editedCell.RowIndex < Rows.Count)
                        {
                            Rows[editedCell.RowIndex].IsDirty = true;
                        }
                        UpdateDataRecordFromRow(editedCell);
                    }
                }

                if (this.Controls.Contains(_editingControl))
                {
                    this.Controls.Remove(_editingControl);
                }

                _editingCell = null;
                IsEditorShown = false;
                EditorClosed?.Invoke(this, EventArgs.Empty);

                InvalidateCell(editedCell);
            }
            finally
            {
                _isEndingEdit = false;
            }
        }

        private void CancelEdit()
        {
            EndEdit(false);
        }
       

        #endregion Editor
        #region "Validation"
        private void InvalidateColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= Columns.Count || !Columns[columnIndex].Visible)
                return;

            int x = gridRect.Left;
            for (int i = 0; i < columnIndex; i++)
            {
                if (Columns[i].Visible)
                    x += Columns[i].Width;
            }

            Rectangle columnRect = new Rectangle(x - _xOffset, gridRect.Top, Columns[columnIndex].Width, gridRect.Height);
            Invalidate(columnRect);
        }
        #endregion "Validation"
        #region Events
        // Add an event for editor closing
        public event EventHandler EditorClosed;
        public event EventHandler<BeepCellSelectedEventArgs> CurrentCellChanged;
        public event EventHandler<BeepRowSelectedEventArgs> CurrentRowChanged;
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


        private void Rows_ListChanged(object sender, ListChangedEventArgs e) => Invalidate();
        #endregion
        #region Change Management
        private List<object> originalList = new List<object>(); // Original data snapshot (wrapped objects)
        private List<object> deletedList = new List<object>();  // Tracks deleted items (wrapped objects)
        public List<Tracking> Trackings { get; set; } = new List<Tracking>(); // Tracks item indices and states
        private Dictionary<object, Dictionary<string, object>> ChangedValues = new Dictionary<object, Dictionary<string, object>>(); // Tracks changed fields per item

        public bool IsLogging { get; set; } = false; // Toggle logging
        public Dictionary<DateTime, EntityUpdateInsertLog> UpdateLog { get; set; } = new Dictionary<DateTime, EntityUpdateInsertLog>(); // Logs updates

        // Adjust _currentRowIndex based on _fullData position
        private void AdjustSelectedIndexForView()
        {
            if (_currentRow != null && _fullData.Any())
            {
                int selectedDataIndex = -1;
                for (int i = 0; i < Rows.Count; i++)
                {
                    if (Rows[i] == _currentRow)
                    {
                        selectedDataIndex = _dataOffset + i;
                        break;
                    }
                }

                if (selectedDataIndex >= 0 && selectedDataIndex < _fullData.Count)
                {
                    var dataItem = _fullData[selectedDataIndex] as DataRowWrapper;
                    if (dataItem != null)
                    {
                        int newRowIndex = selectedDataIndex - _dataOffset;
                        if (newRowIndex >= 0 && newRowIndex < Rows.Count)
                        {
                            if (_currentRowIndex != newRowIndex || _currentRow != Rows[newRowIndex])
                            {
                                _currentRowIndex = newRowIndex;
                                _currentRow = Rows[newRowIndex];
                                CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(selectedDataIndex, _currentRow));
                            }
                        }
                        else
                        {
                            _currentRowIndex = -1;
                            _currentRow = null;
                            CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(-1, null));
                        }
                    }
                }
                else
                {
                    _currentRowIndex = -1;
                    _currentRow = null;
                    CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(-1, null));
                }
            }
        }

        // Helper to ensure tracking for an item
        private void EnsureTrackingForItem(object item)
        {
            var wrappedItem = item as DataRowWrapper;
            if (wrappedItem == null) return;

            object originalData = wrappedItem.OriginalData;
            int currentIndex = _fullData.IndexOf(wrappedItem);
            if (currentIndex < 0) return;

            int originalIndex = originalList.IndexOf(wrappedItem);
            if (originalIndex < 0)
            {
                originalList.Add(wrappedItem);
                originalIndex = originalList.Count - 1;
                wrappedItem.RowState = DataRowState.Added; // Set initial state for new items
                wrappedItem.DateTimeChange = DateTime.Now;
            }

            Tracking tracking = Trackings.FirstOrDefault(t => t.OriginalIndex == originalIndex);
            if (tracking == null)
            {
                Guid uniqueId = wrappedItem.TrackingUniqueId != Guid.Empty ? wrappedItem.TrackingUniqueId : Guid.NewGuid();
                wrappedItem.TrackingUniqueId = uniqueId; // Ensure TrackingUniqueId is set
                tracking = new Tracking(uniqueId, originalIndex, currentIndex)
                {
                    EntityState = MapRowStateToEntityState(wrappedItem.RowState)
                };
                Trackings.Add(tracking);
            }
            else if (tracking.CurrentIndex != currentIndex)
            {
                tracking.CurrentIndex = currentIndex;
                wrappedItem.RowState = DataRowState.Modified; // Update state on index change
                wrappedItem.DateTimeChange = DateTime.Now;
                tracking.EntityState = MapRowStateToEntityState(wrappedItem.RowState);
                if (IsLogging)
                {
                    UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                    {
                        TrackingRecord = tracking,
                        UpdatedFields = ChangedValues.ContainsKey(wrappedItem) ? ChangedValues[wrappedItem] : null
                    };
                }
            }
        }

        // Map DataRowState to EntityState
        private EntityState MapRowStateToEntityState(DataRowState rowState)
        {
            switch (rowState)
            {
                case DataRowState.Unchanged:
                    return EntityState.Unchanged;
                case DataRowState.Added:
                    return EntityState.Added;
                case DataRowState.Modified:
                    return EntityState.Modified;
                case DataRowState.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Unchanged;
            }
        }

        // Sync tracking indices with _fullData
        private void UpdateTrackingIndices()
        {
            // Create a list to store Tracking entries that need to be removed
            var staleTrackings = new List<Tracking>();

            foreach (var tracking in Trackings)
            {
                // Validate OriginalIndex to prevent out-of-range access
                if (tracking.OriginalIndex < 0 || tracking.OriginalIndex >= originalList.Count)
                {
                  //  //MiscFunctions.SendLog($"UpdateTrackingIndices: Invalid OriginalIndex {tracking.OriginalIndex} for Tracking {tracking.UniqueId}, marking for removal");
                    staleTrackings.Add(tracking);
                    continue;
                }

                // Get the original DataRowWrapper from originalList
                var originalWrapped = originalList[tracking.OriginalIndex] as DataRowWrapper;
                if (originalWrapped == null)
                {
                 //   //MiscFunctions.SendLog($"UpdateTrackingIndices: No DataRowWrapper found at OriginalIndex {tracking.OriginalIndex} for Tracking {tracking.UniqueId}, marking for removal");
                    staleTrackings.Add(tracking);
                    continue;
                }

                // Find the current index in _fullData by matching TrackingUniqueId
                int currentIndex = -1;
                for (int i = 0; i < _fullData.Count; i++)
                {
                    var wrappedItem = _fullData[i] as DataRowWrapper;
                    if (wrappedItem != null && wrappedItem.TrackingUniqueId == tracking.UniqueId)
                    {
                        currentIndex = i;
                        break;
                    }
                }

                if (currentIndex >= 0)
                {
                    // Record found in _fullData, update CurrentIndex if needed
                    if (tracking.CurrentIndex != currentIndex)
                    {
                        var wrappedItem = _fullData[currentIndex] as DataRowWrapper;
                        if (wrappedItem != null)
                        {
                            tracking.CurrentIndex = currentIndex;
                            // Only update RowState to Modified if data has changed (handled elsewhere); for now, sync state
                            tracking.EntityState = MapRowStateToEntityState(wrappedItem.RowState);
                            if (IsLogging)
                            {
                                UpdateLogEntries(tracking, currentIndex);
                            }
                          //  //MiscFunctions.SendLog($"UpdateTrackingIndices: Tracking {tracking.UniqueId}, OriginalIndex {tracking.OriginalIndex}, Updated CurrentIndex to {currentIndex}, RowState {wrappedItem.RowState}");
                        }
                    }
                }
                else
                {
                    // Record not found in _fullData, mark Tracking for removal
                 //   //MiscFunctions.SendLog($"UpdateTrackingIndices: Record for Tracking {tracking.UniqueId} not found in _fullData, marking for removal");
                    staleTrackings.Add(tracking);
                }
            }

            // Remove stale Tracking entries
            foreach (var staleTracking in staleTrackings)
            {
                Trackings.Remove(staleTracking);
              //  //MiscFunctions.SendLog($"UpdateTrackingIndices: Removed stale Tracking {staleTracking.UniqueId}");
            }
        }

        // Sync _currentRowIndex and move the editor
        private void SyncSelectedRowIndexAndEditor()
        {
            if (_fullData == null || !_fullData.Any())
            {
                _currentRowIndex = -1;
                _currentRow = null;
                _selectedCell = null;
                if (_editingControl != null && _editingControl.Visible)
                {
                    _editingControl.Visible = false;
                }
                CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(-1, null));
                return;
            }

            // Use _currentRowIndex as the visible index within Rows, map to DisplayIndex
            int oldRowIndex = _currentRowIndex;
            int oldColumnIndex = _selectedCell != null ? _selectedCell.ColumnIndex : -1;
            int selectedDataIndex = _currentRow != null ? _currentRow.DisplayIndex : -1;

            if (selectedDataIndex >= 0 && selectedDataIndex < _fullData.Count)
            {
                int newRowIndex = -1;
                for (int i = 0; i < Rows.Count; i++)
                {
                    if (Rows[i].DisplayIndex == selectedDataIndex)
                    {
                        newRowIndex = i;
                        break;
                    }
                }

                if (newRowIndex >= 0)
                {
                    // Row is visible in the current view
                    _currentRowIndex = newRowIndex;
                    _currentRow = Rows[newRowIndex];

                    if (oldColumnIndex >= 0 && oldColumnIndex < Columns.Count)
                    {
                        _selectedCell = _currentRow.Cells[oldColumnIndex];
                    }

                    if (_selectedCell != null && _editingControl != null && _editingControl.Visible)
                    {
                        MoveEditorIn();
                    }

                    if (oldRowIndex != newRowIndex && oldRowIndex != -1)
                    {
                        string direction = newRowIndex < oldRowIndex ? "up" : "down";
                        //MiscFunctions.SendLog($"Selected row moved {direction}: OldRowIndex={oldRowIndex}, NewRowIndex={newRowIndex}, DisplayIndex={_currentRow.DisplayIndex}");
                    }

                    CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(selectedDataIndex, _currentRow));
                }
                else
                {
                    // Row is not visible; keep selectedDataIndex but clear visible selection
                    _currentRowIndex = -1;
                    _currentRow = null; // No visible row instance
                    _selectedCell = null;
                    if (_editingControl != null && _editingControl.Visible)
                    {
                        _editingControl.Visible = false;
                    }
                    CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(selectedDataIndex, null));
                }
            }
            else
            {
                // Invalid or no selection
                _currentRowIndex = -1;
                _currentRow = null;
                _selectedCell = null;
                if (_editingControl != null && _editingControl.Visible)
                {
                    _editingControl.Visible = false;
                }
                CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(-1, null));
            }

          //  //MiscFunctions.SendLog($"SyncSelectedRowIndexAndEditor: selectedDataIndex={selectedDataIndex}, newRowIndex={newRowIndex}, _currentRowIndex={_currentRowIndex}");
        }

        // Clear all tracking data
        private void ClearAll()
        {
            originalList.Clear();
            deletedList.Clear();
            Trackings.Clear();
            ChangedValues.Clear();
            UpdateLog.Clear();
            _persistentSelectedRows.Clear(); // Clear persistent selection state
        }

        // Start editing a cell and initialize tracking if needed
        public void BeginEdit()
        {
            if (_selectedCell != null)
            {
                ShowCellEditor(_selectedCell, new Point(_selectedCell.X, _selectedCell.Y));
                EnsureTrackingForItem(_fullData[_dataOffset + _selectedCell.RowIndex]);
            }
        }

        // Update tracking indices after filtering or sorting visible rows
        private void UpdateIndexTrackingAfterFilterOrSort()
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                int dataIndex = _dataOffset + i;
                if (dataIndex >= 0 && dataIndex < _fullData.Count)
                {
                    object item = _fullData[dataIndex];
                    var wrappedItem = item as DataRowWrapper;
                    if (wrappedItem != null)
                    {
                        object originalData = wrappedItem.OriginalData;
                        int originalIndex = originalList.IndexOf(wrappedItem);

                        if (originalIndex != -1)
                        {
                            int trackingIdx = Trackings.FindIndex(t => t.OriginalIndex == originalIndex);
                            if (trackingIdx != -1)
                            {
                                Trackings[trackingIdx].CurrentIndex = dataIndex;
                                UpdateLogEntries(Trackings[trackingIdx], dataIndex);
                            }
                            else
                            {
                                Guid uniqueId = wrappedItem.TrackingUniqueId != Guid.Empty ? wrappedItem.TrackingUniqueId : Guid.NewGuid();
                                wrappedItem.TrackingUniqueId = uniqueId;
                                Tracking newTracking = new Tracking(uniqueId, originalIndex, dataIndex)
                                {
                                    EntityState = MapRowStateToEntityState(wrappedItem.RowState)
                                };
                                Trackings.Add(newTracking);
                            }
                        }
                    }
                }
            }
        }

        // Ensure tracking consistency between originalList and _fullData
        private void EnsureTrackingConsistency()
        {
            for (int i = 0; i < _fullData.Count; i++)
            {
                object item = _fullData[i];
                var wrappedItem = item as DataRowWrapper;
                if (wrappedItem != null)
                {
                    object originalData = wrappedItem.OriginalData;
                    int originalIndex = originalList.IndexOf(wrappedItem);
                    Tracking tracking = Trackings.FirstOrDefault(t => t.CurrentIndex == i);

                    if (tracking != null)
                    {
                        tracking.OriginalIndex = originalIndex >= 0 ? originalIndex : i;
                        tracking.EntityState = MapRowStateToEntityState(wrappedItem.RowState);
                    }
                    else if (originalIndex >= 0)
                    {
                        Guid uniqueId = wrappedItem.TrackingUniqueId != Guid.Empty ? wrappedItem.TrackingUniqueId : Guid.NewGuid();
                        wrappedItem.TrackingUniqueId = uniqueId;
                        Trackings.Add(new Tracking(uniqueId, originalIndex, i)
                        {
                            EntityState = MapRowStateToEntityState(wrappedItem.RowState)
                        });
                    }
                }
            }
        }

        // Update log entries with new index
        private void UpdateLogEntries(Tracking tracking, int newIndex)
        {
            if (IsLogging && UpdateLog != null)
            {
                foreach (var logEntry in UpdateLog.Values)
                {
                    if (logEntry.TrackingRecord != null && logEntry.TrackingRecord.UniqueId == tracking.UniqueId)
                    {
                        logEntry.TrackingRecord.CurrentIndex = newIndex;
                    }
                }
            }
        }

        // Reset _fullData to original state
        private void ResetToOriginal()
        {
            _fullData.Clear();
            _fullData.AddRange(originalList);
            deletedList.Clear();
            Trackings.Clear();
            ChangedValues.Clear();
            InitializeRows();
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }

        // Update tracking after inserting or deleting an item
        private void UpdateItemIndexMapping(int startIndex, bool isInsert)
        {
            if (isInsert)
            {
                for (int i = startIndex; i < _fullData.Count; i++)
                {
                    object item = _fullData[i];
                    var wrappedItem = item as DataRowWrapper;
                    if (wrappedItem != null)
                    {
                        object originalData = wrappedItem.OriginalData;
                        int originalIndex = originalList.IndexOf(wrappedItem);
                        if (originalIndex == -1) // New item
                        {
                            originalList.Add(wrappedItem);
                            originalIndex = originalList.Count - 1;
                            wrappedItem.RowState = DataRowState.Added;
                            wrappedItem.DateTimeChange = DateTime.Now;
                        }
                        Guid uniqueId = wrappedItem.TrackingUniqueId != Guid.Empty ? wrappedItem.TrackingUniqueId : Guid.NewGuid();
                        wrappedItem.TrackingUniqueId = uniqueId;
                        Tracking tracking = new Tracking(uniqueId, originalIndex, i)
                        {
                            EntityState = MapRowStateToEntityState(wrappedItem.RowState)
                        };
                        Trackings.Add(tracking);
                    }
                }
            }
            else // Deletion
            {
                for (int i = startIndex; i < _fullData.Count; i++)
                {
                    Tracking tracking = Trackings.FirstOrDefault(t => t.CurrentIndex == i);
                    if (tracking != null)
                    {
                        tracking.CurrentIndex = i;
                        var wrappedItem = _fullData[i] as DataRowWrapper;
                        if (wrappedItem != null)
                        {
                            wrappedItem.RowState = DataRowState.Deleted;
                            wrappedItem.DateTimeChange = DateTime.Now;
                            tracking.EntityState = MapRowStateToEntityState(wrappedItem.RowState);
                            deletedList.Add(_fullData[i]); // Move to deleted list
                            _fullData.RemoveAt(i); // Remove from _fullData
                            i--; // Adjust index after removal
                        }
                    }
                }
            }
        }

        // Get original index of an item
        public int GetOriginalIndex(object item)
        {
            var wrappedItem = item as DataRowWrapper;
            if (wrappedItem != null)
            {
                return originalList.IndexOf(wrappedItem);
            }
            return -1;
        }

        // Get item from original list by index
        public object GetItemFromOriginalList(int index)
        {
            return (index >= 0 && index < originalList.Count) ? originalList[index] : null;
        }

        // Get item from current _fullData by index
        public object GetItemFromCurrentList(int index)
        {
            return (index >= 0 && index < _fullData.Count) ? _fullData[index] : null;
        }

        // Get tracking info for an item
        public Tracking GetTrackingItem(object item)
        {
            var wrappedItem = item as DataRowWrapper;
            if (wrappedItem != null)
            {
                object originalData = wrappedItem.OriginalData;
                int originalIndex = originalList.IndexOf(wrappedItem);
                if (originalIndex >= 0)
                {
                    return Trackings.FirstOrDefault(t => t.OriginalIndex == originalIndex);
                }

                int currentIndex = _fullData.IndexOf(wrappedItem);
                if (currentIndex >= 0)
                {
                    return Trackings.FirstOrDefault(t => t.CurrentIndex == currentIndex);
                }

                int deletedIndex = deletedList.IndexOf(wrappedItem);
                if (deletedIndex >= 0)
                {
                    return Trackings.FirstOrDefault(t => t.CurrentIndex == deletedIndex && t.EntityState == EntityState.Deleted);
                }
            }
            return null;
        }

        // Get changed fields between old and new versions of an item
        private Dictionary<string, object> GetChangedFields(object oldItem, object newItem)
        {
            var changedFields = new Dictionary<string, object>();
            var oldWrapped = oldItem as DataRowWrapper;
            var newWrapped = newItem as DataRowWrapper;

            if (oldWrapped == null || newWrapped == null || oldWrapped.OriginalData.GetType() != newWrapped.OriginalData.GetType())
                return changedFields;

            object oldData = oldWrapped.OriginalData;
            object newData = newWrapped.OriginalData;

            foreach (var property in oldData.GetType().GetProperties())
            {
                var oldValue = property.GetValue(oldData);
                var newValue = property.GetValue(newData);

                if (!Equals(oldValue, newValue))
                {
                    changedFields[property.Name] = newValue;
                    newWrapped.RowState = DataRowState.Modified;
                    newWrapped.DateTimeChange = DateTime.Now;
                }
            }

            return changedFields;
        }

        #endregion Change Management
        #region Theme
        public override void ApplyTheme()
        {
      //      base.ApplyTheme();
            this.BackColor = _currentTheme.GridBackColor;
            this.ForeColor = _currentTheme.GridForeColor;
            SelectedForeColor = _currentTheme.GridForeColor;
            SelectedBackColor=_currentTheme.GridBackColor;
            HoverBackColor= _currentTheme.GridBackColor;
            HoverForeColor= _currentTheme.GridForeColor;
            FocusBackColor=_currentTheme.GridBackColor;
            FocusForeColor= _currentTheme.GridForeColor;
            Color footerback= _currentTheme.GridHeaderBackColor;
            //if (MainPanel == null) return;
            //MainPanel.BackColor = _currentTheme.GridBackColor;
            if (titleLabel != null)
            {
                titleLabel.ParentBackColor = _currentTheme.GridHeaderBackColor; ;
                titleLabel.BackColor = _currentTheme.GridHeaderBackColor; ;
                titleLabel.IsChild = true;
                titleLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.CardHeaderStyle);
             //   titleLabel.MenuStyle = MenuStyle;
                titleLabel.ForeColor = _currentTheme.GridForeColor;
                titleLabel.Invalidate();
            }
            if (Recordnumberinglabel1 != null) {
                //   Recordnumberinglabel1.ForeColor = _currentTheme.GridForeColor;
                Recordnumberinglabel1.TextFont = BeepThemesManager.ToFont(_currentTheme.SmallText);
                Recordnumberinglabel1.ForeColor = _currentTheme.GridHeaderForeColor;
                Recordnumberinglabel1.BackColor = footerback;
                Recordnumberinglabel1.ParentBackColor = footerback;
                Recordnumberinglabel1.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
                Recordnumberinglabel1.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
                Recordnumberinglabel1.BorderColor = _currentTheme.GridForeColor;
                Recordnumberinglabel1.DisabledBackColor = _currentTheme.DisabledBackColor;
                Recordnumberinglabel1.DisabledForeColor = _currentTheme.DisabledForeColor;
                Recordnumberinglabel1.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
                Recordnumberinglabel1.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;

            }
            if (PageLabel != null)
            {
                //PageLabel.ForeColor = _currentTheme.GridForeColor;
                PageLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.SmallText);
                PageLabel.ForeColor = _currentTheme.GridHeaderForeColor;
                PageLabel.BackColor = footerback;
                PageLabel.ParentBackColor = footerback;
                PageLabel.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
                PageLabel.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
                PageLabel.BorderColor = _currentTheme.GridForeColor;
                PageLabel.DisabledBackColor = _currentTheme.DisabledBackColor;
                PageLabel.DisabledForeColor = _currentTheme.DisabledForeColor;
                PageLabel.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
                PageLabel.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;

          
                foreach (BeepButton x in buttons)
                {
                    x.Theme = Theme;
                    x.ForeColor = _currentTheme.GridHeaderForeColor;
                    x.BackColor = footerback;
                    x.ParentBackColor = footerback;
                    x.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
                    x.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
                    x.BorderColor = _currentTheme.GridForeColor;
                    x.DisabledBackColor = _currentTheme.DisabledBackColor;
                    x.DisabledForeColor = _currentTheme.DisabledForeColor;
                    x.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
                    x.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;
                    x.ApplyThemeToSvg();
                }
                foreach (BeepButton x in pagingButtons)
                {
                    x.Theme = Theme;
                    x.ForeColor = _currentTheme.GridHeaderForeColor;
                    x.BackColor = footerback;
                    x.ParentBackColor = footerback;
                    x.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
                    x.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
                    x.BorderColor = _currentTheme.GridForeColor;
                    x.DisabledBackColor = _currentTheme.DisabledBackColor;
                    x.DisabledForeColor = _currentTheme.DisabledForeColor;
                    x.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
                    x.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;
                    x.ApplyThemeToSvg();
                }
                //_selectAllCheckBox
                _selectAllCheckBox.Theme = Theme;
                _selectAllCheckBox.ForeColor = _currentTheme.GridHeaderForeColor;
                _selectAllCheckBox.BackColor = footerback;
                _selectAllCheckBox.ParentBackColor = footerback;
                _selectAllCheckBox.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
                _selectAllCheckBox.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
                _selectAllCheckBox.BorderColor = _currentTheme.GridForeColor;
                _selectAllCheckBox.DisabledBackColor = _currentTheme.DisabledBackColor;
                _selectAllCheckBox.DisabledForeColor = _currentTheme.DisabledForeColor;
                _selectAllCheckBox.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
                _selectAllCheckBox.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;
               // _selectAllCheckBox.ApplyThemeToSvg();
                //  PageLabel.TextFont = BeepThemesManager.ToFont(_currentTheme.ButtonStyle);
            }
            //if (DataNavigator != null)
            //{
            //    _currentTheme.ButtonBackColor = _currentTheme.GridBackColor;
            //    _currentTheme.ButtonForeColor = _currentTheme.GridForeColor;
            //    DataNavigator.MenuStyle = MenuStyle;
            //}
            if (_verticalScrollBar != null)
            {
                _verticalScrollBar.Theme = Theme;
            }
            if (_horizontalScrollBar != null)
            {
                _horizontalScrollBar.Theme = Theme;
            }
            foreach (var row in Rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (cell.UIComponent is BeepControl ctrl)
                    {
                        ctrl.Theme = Theme;
                    }
                }
            }
            filterTextBox.TextFont = BeepThemesManager.ToFont(_currentTheme.SmallText);
            filterTextBox.ForeColor = _currentTheme.GridHeaderForeColor;
            filterTextBox.BackColor = _currentTheme.GridHeaderBackColor; ;
            filterTextBox.ParentBackColor = _currentTheme.GridHeaderBackColor; ;
            filterTextBox.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
            filterTextBox.HoverForeColor = _currentTheme.GridHeaderHoverForeColor;
            filterTextBox.BorderColor = _currentTheme.GridForeColor;
            filterTextBox.DisabledBackColor = _currentTheme.DisabledBackColor;
            filterTextBox.DisabledForeColor = _currentTheme.DisabledForeColor;
            filterTextBox.SelectedBackColor = _currentTheme.GridHeaderSelectedBackColor;
            filterTextBox.SelectedForeColor = _currentTheme.GridHeaderSelectedForeColor;
           
            filterColumnComboBox.TextFont = BeepThemesManager.ToFont(_currentTheme.SmallText);
            filterColumnComboBox.ForeColor = _currentTheme.GridHeaderForeColor;
            filterColumnComboBox.BackColor = _currentTheme.GridHeaderBackColor; ;
            filterColumnComboBox.ParentBackColor = _currentTheme.GridHeaderBackColor; ;
            filterColumnComboBox.HoverBackColor = _currentTheme.GridHeaderHoverBackColor;
            InitializePaintingResources();
            Invalidate();
        }
        #endregion
        #region BindingSource Management
        private bool _isAddingNewRecord = false;
        private BindingSource _bindingSource;
        private void AssignBindingSource(BindingSource bindingSrc)
        {
            // //MiscFunctions.SendLog($"AssignBindingSource: BindingSource Detected, DataSource = {bindingSrc?.DataSource?.GetType()}, DataMember = {bindingSrc?.DataMember}");
            _bindingSource = bindingSrc;
            // Unsubscribe from previous BindingSource if it exists
            if (_bindingSource != null)
            {
                _bindingSource.ListChanged -= BindingSource_ListChanged;
            }

            // Set the new BindingSource
            _bindingSource = bindingSrc;

            // If _dataSource is an IList, wrap it in a BindingSource
            if (_dataSource is IList list && !(_dataSource is BindingSource))
            {
                _bindingSource = new BindingSource { DataSource = list };
            }

            // Subscribe to ListChanged event
            if (_bindingSource != null)
            {
                _bindingSource.ListChanged += BindingSource_ListChanged;
            }
        }
        private void BindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
          //  //MiscFunctions.SendLog($"BindingSource_ListChanged: Type={e.ListChangedType}, NewIndex={e.NewIndex}, OldIndex={e.OldIndex}, _fullData.Count={_fullData?.Count}");

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    HandleItemAdded(e.NewIndex);
                    if (_isAddingNewRecord && e.NewIndex >= 0 && e.NewIndex < _fullData.Count)
                    {
                        // UI updates specific to new record addition
                        int newOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
                        StartSmoothScroll(newOffset);
                        int newRowIndex = _fullData.Count - 1 - _dataOffset;
                        if (newRowIndex >= 0 && newRowIndex < Rows.Count)
                        {
                            SelectCell(newRowIndex, 0);
                            BeginEdit();
                        }
                        _isAddingNewRecord = false; // Reset flag after handling

                        // Optional: User feedback
                        MessageBox.Show("New record added successfully.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;

                case ListChangedType.ItemChanged:
                    HandleItemChanged(e.NewIndex);
                    break;

                case ListChangedType.ItemDeleted:
                    HandleItemDeleted(e.NewIndex);
                    break;

                case ListChangedType.Reset:
                    // Full reset (e.g., filter or sort applied, or data source changed)
                    DataSetup();
                    InitializeData();
                    break;

                case ListChangedType.ItemMoved:
                    HandleItemMoved(e.OldIndex, e.NewIndex);
                    break;

                default:
                    // Handle other changes (e.g., property descriptor changes) by refreshing
                    DataSetup();
                    InitializeData();
                    break;
            }

            // Update tracking indices and selection state
            UpdateTrackingIndices();
            SyncSelectedRowIndexAndEditor();

            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }
        private void HandleItemAdded(int newIndex)
        {
            try
            {
                if (newIndex < 0 || _bindingSource == null || newIndex >= _bindingSource.Count)
                {
                    //MiscFunctions.SendLog($"HandleItemAdded: Invalid index {newIndex} or null BindingSource, skipping");
                    return;
                }

                var newItem = _bindingSource[newIndex];
                var wrapper = new DataRowWrapper(newItem, newIndex)
                {
                    TrackingUniqueId = Guid.NewGuid(),
                    RowState = DataRowState.Added,
                    DateTimeChange = DateTime.Now
                };

                // Append to _fullData to avoid order issues; BindingSource_ListChanged will re-sync if needed
                if (newIndex == _fullData.Count)
                {
                    _fullData.Add(wrapper);
                }
                else
                {
                    _fullData.Insert(newIndex, wrapper);
                    //MiscFunctions.SendLog($"HandleItemAdded: Inserted at {newIndex} instead of appending due to BindingSource order");
                }

                // Update RowIDs
                for (int i = 0; i < _fullData.Count; i++)
                {
                    if (_fullData[i] is DataRowWrapper dataRow)
                    {
                        dataRow.RowID = i;
                    }
                }

                // Add to originalList if not present
                if (!originalList.Contains(wrapper))
                {
                    originalList.Add(wrapper);
                }

                // Ensure tracking is added before logging
                EnsureTrackingForItem(wrapper);

                // Adjust _dataOffset
                if (_dataOffset > newIndex)
                {
                    _dataOffset++;
                }

                // Log the operation
                if (IsLogging)
                {
                    var tracking = Trackings.FirstOrDefault(t => t.UniqueId == wrapper.TrackingUniqueId);
                    UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                    {
                        TrackingRecord = tracking,
                        LogAction = LogAction.Insert,
                        UpdatedFields = new Dictionary<string, object>()
                    };
                    if (tracking == null)
                    {
                        //MiscFunctions.SendLog("HandleItemAdded: Tracking entry not found for new item, logging without tracking");
                    }
                }

                UpdateRowCount();
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"HandleItemAdded Error at index {newIndex}: {ex.Message}");
                // Continue to allow BindingSource_ListChanged to proceed with UI updates
            }
        }
        private void HandleItemChanged(int index)
        {
            try
            {
                if (index < 0 || index >= _fullData.Count || _bindingSource == null || index >= _bindingSource.Count)
                {
                    //MiscFunctions.SendLog($"HandleItemChanged: Invalid index {index} or null BindingSource, skipping");
                    return;
                }

                var updatedItem = _bindingSource[index];
                var wrapper = _fullData[index] as DataRowWrapper;
                if (wrapper != null)
                {
                    var oldData = wrapper.OriginalData;
                    var changes = GetChangedFields(oldData, updatedItem);
                    if (changes.Any()) // Only proceed if there are actual changes
                    {
                        wrapper.OriginalData = updatedItem;
                        wrapper.RowState = DataRowState.Modified;
                        wrapper.DateTimeChange = DateTime.Now;

                        EnsureTrackingForItem(wrapper);

                        if (IsLogging)
                        {
                            Tracking tracking = GetTrackingItem(wrapper);
                            if (tracking != null)
                            {
                                tracking.EntityState = MapRowStateToEntityState(wrapper.RowState);
                                if (!ChangedValues.ContainsKey(wrapper))
                                {
                                    ChangedValues[wrapper] = changes;
                                }
                                else
                                {
                                    foreach (var kvp in changes)
                                    {
                                        ChangedValues[wrapper][kvp.Key] = kvp.Value;
                                    }
                                }
                                UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                                {
                                    TrackingRecord = tracking,
                                    LogAction = LogAction.Update,
                                    UpdatedFields = ChangedValues[wrapper]
                                };
                            }
                            else
                            {
                                //MiscFunctions.SendLog($"HandleItemChanged: Tracking not found for item at index {index}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"HandleItemChanged Error at index {index}: {ex.Message}");
            }
        }
        private void HandleItemDeleted(int index)
        {
            try
            {
                if (index < 0 || index >= _fullData.Count || _bindingSource == null || index >= _bindingSource.Count)
                {
                    //MiscFunctions.SendLog($"HandleItemDeleted: Invalid index {index} or null BindingSource, skipping");
                    return;
                }

                var wrapper = _fullData[index] as DataRowWrapper;
                if (wrapper != null)
                {
                    wrapper.RowState = DataRowState.Deleted;
                    wrapper.DateTimeChange = DateTime.Now;

                    EnsureTrackingForItem(wrapper);

                    var tracking = Trackings.FirstOrDefault(t => t.UniqueId == wrapper.TrackingUniqueId);
                    if (tracking != null)
                    {
                        tracking.EntityState = EntityState.Deleted;
                        Trackings.Remove(tracking);

                        if (IsLogging)
                        {
                            UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                            {
                                TrackingRecord = tracking,
                                LogAction = LogAction.Delete,
                                UpdatedFields = new Dictionary<string, object>()
                            };
                        }
                    }
                    else if (IsLogging)
                    {
                        //MiscFunctions.SendLog($"HandleItemDeleted: Tracking not found for item at index {index}");
                    }

                    originalList.RemoveAt(index);
                    deletedList.Add(wrapper);
                }

                _fullData.RemoveAt(index);

                for (int i = 0; i < _fullData.Count; i++)
                {
                    if (_fullData[i] is DataRowWrapper dataRow)
                    {
                        dataRow.RowID = i;
                    }
                }

                if (_dataOffset > index)
                {
                    _dataOffset--;
                }

                UpdateRowCount();
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"HandleItemDeleted Error at index {index}: {ex.Message}");
            }
        }
        private void HandleItemMoved(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= _fullData.Count || newIndex < 0 || newIndex >= _fullData.Count || _bindingSource == null) return;

            var item = _fullData[oldIndex];
            _fullData.RemoveAt(oldIndex);
            _fullData.Insert(newIndex, item);

            for (int i = 0; i < _fullData.Count; i++)
            {
                if (_fullData[i] is DataRowWrapper dataRow)
                {
                    dataRow.RowID = i;
                    EnsureTrackingForItem(dataRow);
                }
            }

            var wrapper = item as DataRowWrapper;
            if (wrapper != null)
            {
                var tracking = Trackings.FirstOrDefault(t => t.UniqueId == wrapper.TrackingUniqueId);
                if (tracking != null)
                {
                    tracking.OriginalIndex = originalList.IndexOf(wrapper); // Ensure OriginalIndex is correct
                    tracking.CurrentIndex = newIndex;

                    if (IsLogging)
                    {
                        UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                        {
                            TrackingRecord = tracking,
                            LogAction = LogAction.Update, // Move treated as an update
                            UpdatedFields = new Dictionary<string, object>()
                        };
                    }
                }
            }

            if (_dataOffset == oldIndex)
            {
                _dataOffset = newIndex;
            }
            else if (_dataOffset > oldIndex && _dataOffset <= newIndex)
            {
                _dataOffset--;
            }
            else if (_dataOffset < oldIndex && _dataOffset >= newIndex)
            {
                _dataOffset++;
            }
        }
        #endregion BindingSource Management
        #region Internal Navigator
        public event EventHandler CallPrinter;
        public event EventHandler SendMessage;
        public event EventHandler ShowSearch;
        public event EventHandler NewRecordCreated;
        public event EventHandler SaveCalled;
        public event EventHandler DeleteCalled;
        public event EventHandler EditCalled;
        public bool VerifyDelete = false;
        private BeepButton Recordnumberinglabel1; // Now a BeepButton
        private BeepButton FindButton, NewButton, EditButton, PreviousButton, NextButton, RemoveButton, RollbackButton, SaveButton, PrinterButton, MessageButton;
        private BeepButton FirstPageButton, PrevPageButton, NextPageButton, LastPageButton;
        private BeepButton PageLabel; // Now a BeepButton
        private int spacing = 5; // Spacing between buttons
        private int labelWidth = 100; // Width of the record label
        private int pageLabelWidth = 60; // Width of the page label
        private Size buttonSize = new Size(20, 20);
        private List<Control> buttons = new List<Control>();
        private List<Control> pagingButtons = new List<Control>();
      //  private Panel MainPanel;
        private int _currentPage = 1;
        // Use visibleRowCount for paging instead of a hardcoded _recordsPerPage
        private int _totalPages;// _fullData != null ? (int)Math.Ceiling((double)_fullData.Count / (GetVisibleRowCount() == 1 ? _fullData.Count : GetVisibleRowCount())) : 1;


        #region Drawing Buttons and hittest
        private Dictionary<string, BeepButton> _navigationButtonCache = new Dictionary<string, BeepButton>();

        private void DrawNavigationRow(Graphics g, Rectangle rect)
        {
            // Fill background
            using (var brush = new SolidBrush(_currentTheme.GridHeaderBackColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Draw top border line
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            }

            // Calculate layout positions
            int buttonHeight = 24;
            int buttonWidth = 24;
            int padding = 6;
            int y = rect.Top + (rect.Height - buttonHeight) / 2; // Center buttons vertically
            int x = rect.Left + padding;

            // Draw navigation buttons (left side)
            DrawNavigationButton(g, "FindButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.079-search.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "EditButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.062-pencil.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "PrinterButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.072-printer.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "MessageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.083-share.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "SaveButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.036-floppy disk.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "NewButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.068-plus.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "RemoveButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.035-eraser.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            DrawNavigationButton(g, "RollbackButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.005-back arrow.svg",
                new Rectangle(x, y, buttonWidth, buttonHeight));
            x += buttonWidth + padding;

            // Draw record counter (center) with navigation buttons
            string recordCounter = (_fullData != null && _fullData.Any())
                ? $"{(_currentRowIndex + _dataOffset + 1)} - {_fullData.Count}"
                : "0 - 0";

            // Calculate center area for record counter display
            using (var font = new Font(Font.FontFamily, 9f))
            using (var brush = new SolidBrush(_currentTheme.GridHeaderForeColor))
            {
                SizeF textSize = g.MeasureString(recordCounter, font);

                // Center x position for record counter text
                float recordX = rect.Left + ((rect.Width - textSize.Width) / 2);

                // Add record navigation buttons around the counter
                int navButtonWidth = 20;
                int navButtonSpacing = 6;

                // First Record button - left of counter
                DrawNavigationButton(g, "FirstRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-left.svg",
                    new Rectangle((int)recordX - navButtonWidth * 2 - navButtonSpacing * 2, y, navButtonWidth, buttonHeight));

                // Previous Record button - left of counter
                DrawNavigationButton(g, "PreviousRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg",
                    new Rectangle((int)recordX - navButtonWidth - navButtonSpacing, y, navButtonWidth, buttonHeight));

                // Draw the record counter text
                g.DrawString(recordCounter, font, brush, recordX, y + (buttonHeight - textSize.Height) / 2);

                // Next Record button - right of counter
                DrawNavigationButton(g, "NextRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg",
                    new Rectangle((int)(recordX + textSize.Width + navButtonSpacing), y, navButtonWidth, buttonHeight));

                // Last Record button - right of counter
                DrawNavigationButton(g, "LastRecordButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-right.svg",
                    new Rectangle((int)(recordX + textSize.Width + navButtonWidth + navButtonSpacing * 2), y, navButtonWidth, buttonHeight));
            }


            // Draw pagination controls (right side)
            int pageButtonX = rect.Right - padding - buttonWidth;

            DrawNavigationButton(g, "LastPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-right.svg",
                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));
            pageButtonX -= buttonWidth + padding;

            DrawNavigationButton(g, "NextPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg",
                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));
            pageButtonX -= buttonWidth + padding;

            // Draw page counter
            int visrowcount = GetVisibleRowCount();
            _totalPages = _fullData != null ?
                (int)Math.Ceiling((double)_fullData.Count / (visrowcount == 1 ? _fullData.Count : visrowcount)) : 1;
            _currentPage = Math.Max(1, Math.Min(_currentPage, _totalPages));
            string pageCounter = $"{_currentPage} of {_totalPages}";

            using (var font = new Font(Font.FontFamily, 9f))
            using (var brush = new SolidBrush(_currentTheme.GridHeaderForeColor))
            {
                SizeF textSize = g.MeasureString(pageCounter, font);
                pageButtonX -= (int)textSize.Width + padding;
                g.DrawString(pageCounter, font, brush, pageButtonX, y + (buttonHeight - textSize.Height) / 2);
            }

            pageButtonX -= buttonWidth + padding;
            DrawNavigationButton(g, "PrevPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg",
                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));
            pageButtonX -= buttonWidth + padding;

            DrawNavigationButton(g, "FirstPageButton", "TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-left.svg",
                new Rectangle(pageButtonX, y, buttonWidth, buttonHeight));

            _navigatorDrawn = true;
        }

        private void DrawNavigationButton(Graphics g, string buttonName, string imagePath, Rectangle buttonRect)
        {
            // Create a temporary button to use its drawing mechanism
            if (!_navigationButtonCache.TryGetValue(buttonName, out BeepButton tempButton))
            {
                 tempButton = new BeepButton
                {
                    ImagePath = imagePath,
                    ImageAlign = ContentAlignment.MiddleCenter,
                    HideText = true,
                    IsFrameless = true,
                    Size = buttonRect.Size,
                    Theme = Theme,
                    IsChild = true,
                    MaxImageSize = new Size(buttonRect.Width - 4, buttonRect.Height - 4),
                    ApplyThemeOnImage = true,
                    Name = buttonName,
                    ComponentName = buttonName // Set ComponentName for tooltip support
                };
                _navigationButtonCache[buttonName] = tempButton;
            }
            // Check if mouse is hovering over this button
            bool isHovered = false;
            Point mousePos = PointToClient(MousePosition);
            if (buttonRect.Contains(mousePos))
            {
                isHovered = true;
                tempButton.IsHovered = true;

                // Show tooltip if button is being hovered
                if (!tooltipShown)
                {
                    tempButton.ShowToolTip(buttonName.Replace("Button", ""));
                    tooltipShown = true;
                }
            }
            tempButton.Size = buttonRect.Size;
            // Draw the button with hover effect
            tempButton.Draw(g, buttonRect);

            // Add to the hit test list for click detection
            AddHitArea(buttonName, buttonRect);
        }

        // Field to track tooltip state
        private bool tooltipShown = false;

     

        #endregion
        private void HideNavigationButtons()
        {
            foreach (var button in buttons)
            {
                button.Visible = false;
            }
            foreach (var button in pagingButtons)
            {
                button.Visible = false;
            }
        }
        private void ShowNavigationButtons()
        {
            foreach (var button in buttons)
            {
                button.Visible = true;
            }
            foreach (var button in pagingButtons)
            {
                button.Visible = true;
            }
        }
        private void CreateNavigationButtons()
        {

            // MainPanel = new Panel();

            // Main navigation buttons
            //FindButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.search_1.svg", buttonSize, FindpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            //EditButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.pencil.svg", buttonSize, EditpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            //PrinterButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.print.svg", buttonSize, PrinterpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            //MessageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.mail.svg", buttonSize, MessagepictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            //SaveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.check.svg", buttonSize, SavepictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            //NewButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.add.svg", buttonSize, NewButton_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            //RemoveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.remove.svg", buttonSize, RemovepictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            //RollbackButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.undo.svg", buttonSize, RollbackpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            //NextButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.forward.svg", buttonSize, NextpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            //PreviousButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.backwards.svg", buttonSize, PreviouspictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            FindButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.079-search.svg", buttonSize, FindpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            EditButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.062-pencil.svg", buttonSize, EditpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            PrinterButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.072-printer.svg", buttonSize, PrinterpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            MessageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.083-share.svg", buttonSize, MessagepictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            SaveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.036-floppy disk.svg", buttonSize, SavepictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            NewButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.068-plus.svg", buttonSize, NewButton_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            RemoveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.035-eraser.svg", buttonSize, RemovepictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            RollbackButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.005-back arrow.svg", buttonSize, RollbackpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            NextButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg", buttonSize, NextpictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            PreviousButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg", buttonSize, PreviouspictureBox_Click, AnchorStyles.Left | AnchorStyles.Bottom);
            // Page label (as a BeepButton)
            PageLabel = new BeepButton
            {
                Text = "1 of 1",
                Size = new Size(pageLabelWidth, buttonSize.Height),
                HideText = false,
                IsFrameless = true,
                IsChild = true,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };

            // Record number label (as a BeepButton)
            Recordnumberinglabel1 = new BeepButton
            {
                Text = "0 - 0",
                Size = new Size(labelWidth, buttonSize.Height),
                HideText = false,
                IsFrameless = true,
                IsChild = true,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };

            // Paging buttons with event handlers
            FirstPageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-left.svg", buttonSize, FirstPageButton_Click, AnchorStyles.Right | AnchorStyles.Bottom);
            PrevPageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-left.svg", buttonSize, PrevPageButton_Click, AnchorStyles.Right | AnchorStyles.Bottom);
            NextPageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-small-right.svg", buttonSize, NextPageButton_Click, AnchorStyles.Right | AnchorStyles.Bottom);
            LastPageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.NAV.angle-double-small-right.svg", buttonSize, LastPageButton_Click, AnchorStyles.Right | AnchorStyles.Bottom);
         
          

            buttons = new List<Control>
    {
        FindButton,  PrinterButton, MessageButton
        , NextButton, NewButton,EditButton, SaveButton, RollbackButton,PreviousButton, Recordnumberinglabel1,NextButton, RemoveButton
    };

            pagingButtons = new List<Control>
    {
        FirstPageButton, PrevPageButton, PageLabel, NextPageButton, LastPageButton,
    };

           // Controls.Add(MainPanel);


            //
            // Initial update to ensure labels are populated
            UpdateRecordNumber();
            UpdatePagingControls();
        }
        private BeepButton CreateButton(string imagePath, Size size, EventHandler clickHandler,AnchorStyles anchorStyles)
        {
            var button = new BeepButton
            {
                ImagePath = imagePath,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFrameless = true,
                Size = size,
                IsChild = true,
                Anchor = anchorStyles,
                Margin = new Padding(0),
                ApplyThemeOnImage = true,
                Padding = new Padding(0),
                IsRounded=false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme=false,
                ImageEmbededin = ImageEmbededin.DataGridView,
                MaxImageSize = new Size(size.Width - 4, size.Height - 4),
                Visible = true // Ensure buttons are visible
                
            };
            button.Click += clickHandler;
           // Controls.Add(button);
            return button;
        }
        private void PositionControls(Rectangle rect,int spacing)
        {
            //if (MainPanel == null || buttons == null || buttons.Count == 0) return;

            // Calculate the vertical center for the single row
            int rowHeight = buttonSize.Height;
            int centerY = rect.Top + (( rect.Height - rowHeight) / 2); // Center the single row vertically
            if (centerY < 0) centerY = spacing; // Ensure buttons don't go above the top edge

            // Position the main buttons on the left
            int startX = rect.X+spacing; // Start with a small padding from the left edge
            int currentX = startX;

            foreach (var control in buttons)
            {
                //if (!Controls.Contains(control))
                //{
                //    Controls.Add(control);
                //}
                control.Left = currentX;
                control.Top = centerY;
                currentX += control.Width + spacing;
            }

            // Position the paging buttons and labels on the right
            int pagingTotalWidth = pagingButtons.Sum(c => c.Width) + spacing * (pagingButtons.Count - 1);
            int pagingStartX = rect.Width - pagingTotalWidth - (2*spacing); // Align to the right with padding
            if (pagingStartX < currentX) pagingStartX = currentX; // Ensure no overlap with main buttons
            currentX = pagingStartX;

            foreach (var control in pagingButtons)
            {
                //if (!Controls.Contains(control))
                //{
                //    Controls.Add(control);
                //}
                control.Visible = true; // Ensure all paging controls are visible
                control.Left = currentX;
                control.Top = centerY; // Same Y position as the main buttons (single row)
                currentX += control.Width + spacing;
            }

            UpdatePagingControls();
            UpdateRecordNumber();
        }

        private void UpdateRecordNumber()
        {
            if (Recordnumberinglabel1 == null) return;

            if (_fullData != null && _fullData.Any())
            {
                int position = _currentRowIndex + _dataOffset + 1;
                Recordnumberinglabel1.Text = $"{position} - {_fullData.Count}";
            }
            else
            {
                Recordnumberinglabel1.Text = "0 - 0";
            }
            UpdatePagingControls();
        }

        private void UpdateNavigationButtonState()
        {
            return;
            if (_fullData == null)
            {
                PreviousButton.Enabled = false;
                NextButton.Enabled = false;
                RemoveButton.Enabled = false;
                SaveButton.Enabled = false;
                FirstPageButton.Enabled = false;
                PrevPageButton.Enabled = false;
                NextPageButton.Enabled = false;
                LastPageButton.Enabled = false;
                return;
            }

            int position = _currentRowIndex + _dataOffset;
            PreviousButton.Enabled = position > 0;
            NextButton.Enabled = position < _fullData.Count - 1;
            RemoveButton.Enabled = _fullData.Count > 0;
            SaveButton.Enabled = _fullData.Count > 0;

            FirstPageButton.Enabled = _currentPage > 1;
            PrevPageButton.Enabled = _currentPage > 1;
            NextPageButton.Enabled = _currentPage < _totalPages;
            LastPageButton.Enabled = _currentPage < _totalPages;
        }

        private void UpdatePagingControls()
        {
            if (PageLabel == null) return;
           
            if (_fullData != null && _fullData.Any())
            {
                int visrowcount = GetVisibleRowCount();
                _totalPages = _fullData != null ? (int)Math.Ceiling((double)_fullData.Count / (visrowcount == 1 ? _fullData.Count : visrowcount)) : 1;
                _currentPage = Math.Max(1, Math.Min(_currentPage, _totalPages));
                PageLabel.Text = $"{_currentPage} of {_totalPages}";
                UpdateNavigationButtonState();
            }

           
        }

        private void FirstPageButton_Click(object sender, EventArgs e)
        {
            if (_currentPage == 1) return;
            _currentPage = 1;
            UpdatePage();
        }

        private void PrevPageButton_Click(object sender, EventArgs e)
        {
            if (_currentPage <= 1) return;
            _currentPage--;
            UpdatePage();
        }

        private void NextPageButton_Click(object sender, EventArgs e)
        {
            if (_currentPage >= _totalPages) return;
            _currentPage++;
            UpdatePage();
        }

        private void LastPageButton_Click(object sender, EventArgs e)
        {
            if (_currentPage == _totalPages) return;
            _currentPage = _totalPages;
            UpdatePage();
        }

        private void UpdatePage()
        {
            if (_fullData == null || !_fullData.Any())
            {
                _dataOffset = 0;
                _currentPage = 1;
                _currentRowIndex = -1;
                _currentRow = null;
                Rows.Clear();
                UpdateScrollBars();
                UpdateRecordNumber();
                UpdatePagingControls();
                Invalidate();
                return;
            }

            int visibleRows = GetVisibleRowCount();
            _totalPages = (int)Math.Ceiling((double)_fullData.Count / visibleRows);
            _currentPage = Math.Max(1, Math.Min(_currentPage, _totalPages));

            int recordsPerPage = visibleRows;
            int newOffset = (_currentPage - 1) * recordsPerPage;

            // Adjust newOffset for the last page to fill as many rows as possible.
            if (newOffset + recordsPerPage > _fullData.Count)
            {
                newOffset = Math.Max(0, _fullData.Count - recordsPerPage);
            }

            _dataOffset = newOffset;
            FillVisibleRows();

            if (_fullData.Count - _dataOffset > 0)
            {
                // Start at the first row of the page.
                _currentRowIndex = 0;
                if (_currentRowIndex < Rows.Count)
                {
                    SelectCell(_currentRowIndex, _selectedColumnIndex >= 0 ? _selectedColumnIndex : 0);
                }
                else
                {
                    _currentRowIndex = -1;
                    _currentRow = null;
                }
            }
            else
            {
                _currentRowIndex = -1;
                _currentRow = null;
            }

            UpdateScrollBars();
            UpdateRecordNumber();
            UpdatePagingControls();
    
        }


        private void SavepictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                SaveCalled?.Invoke(sender, null);
                MessageBox.Show(Parent, "Record Saved", "Beep", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Binding Navigator {ex.Message}");
                MessageBox.Show(Parent, ex.Message, "Beep", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            try
            {
                //MiscFunctions.SendLog("Add New Record");

                if (_bindingSource != null)
                {
                    CreateNewRecordForBindingSource();
                }
                else if (_dataSource is IList)
                {
                    CreateNewRecordForIList();
                }
                else
                {
                    //MiscFunctions.SendLog("NewButton_Click: Unsupported data source type");
                    MessageBox.Show("Unsupported data source type.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                UpdateRecordNumber();
                UpdateNavigationButtonState();
                UpdatePagingControls();
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
                NewRecordCreated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"NewButton_Click Error: {ex.Message}");
                MessageBox.Show($"Error adding new record: {ex.Message}", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemovepictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                //MiscFunctions.SendLog("Remove Record");
                DeleteCalled?.Invoke(sender, EventArgs.Empty);

                if (_fullData == null || !_fullData.Any() || _currentRow == null)
                {
                    //MiscFunctions.SendLog("No record selected to delete.");
                    return;
                }

                if (VerifyDelete && MessageBox.Show(Parent, "Are you sure you want to delete the record?", "BeepSimpleGrid", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                    return;

                int dataIndex = _currentRow.DisplayIndex;
                if (dataIndex < 0 || dataIndex >= _fullData.Count)
                {
                    //MiscFunctions.SendLog($"Invalid data index: {dataIndex}");
                    return;
                }

                var item = _fullData[dataIndex];
                var wrapper = item as DataRowWrapper;
                if (wrapper == null)
                {
                    //MiscFunctions.SendLog($"Item at index {dataIndex} is not a DataRowWrapper");
                    return;
                }

                if (_dataSource is BindingSource && _bindingSource != null)
                {
                    int bsIndex = _bindingSource.IndexOf(wrapper.OriginalData);
                    if (bsIndex >= 0 && bsIndex < _bindingSource.Count)
                    {
                        _bindingSource.RemoveCurrent();
                        //MiscFunctions.SendLog($"Removed record from BindingSource at index {bsIndex}");
                    }
                    else
                    {
                        //MiscFunctions.SendLog($"Item not found in BindingSource at index {bsIndex}");
                        return;
                    }
                }
                else if (_dataSource is IList)
                {
                    DeleteFromDataSource(wrapper.OriginalData);
                    //MiscFunctions.SendLog($"Removed record from IList at index {dataIndex}");
                }
                else
                {
                    //MiscFunctions.SendLog($"Remove: Unsupported data source type: {_dataSource?.GetType()}");
                    MessageBox.Show("Unsupported data source type.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                UpdateRecordNumber();
                UpdateNavigationButtonState();
                UpdatePagingControls();

                if (_fullData.Any())
                {
                    int newSelectedIndex = Math.Min(dataIndex, _fullData.Count - 1);
                    StartSmoothScroll(Math.Max(0, newSelectedIndex - GetVisibleRowCount() + 1));
                    if (newSelectedIndex >= _dataOffset && newSelectedIndex < _dataOffset + Rows.Count)
                    {
                        _currentRowIndex = newSelectedIndex - _dataOffset;
                        _currentRow = Rows[_currentRowIndex];
                        int colIndex = _selectedColumnIndex >= 0 && _selectedColumnIndex < Columns.Count ? _selectedColumnIndex : 0;
                        SelectCell(_currentRowIndex, colIndex);
                    }
                }
                else
                {
                    _currentRow = null;
                    _currentRowIndex = -1;
                }

                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();

                MessageBox.Show("Record deleted successfully.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"RemovepictureBox_Click Error: {ex.Message}");
                MessageBox.Show($"Error deleting record: {ex.Message}", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NextpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                //MiscFunctions.SendLog("Next Record");
                MoveNextRow();

                UpdateRecordNumber();
                UpdateNavigationButtonState();
                UpdatePagingControls();
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void PreviouspictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                //MiscFunctions.SendLog("Previous Record");
                MovePreviousRow();

                UpdateRecordNumber();
                UpdateNavigationButtonState();
                UpdatePagingControls();
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void RollbackpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(Parent, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    if (_currentRow != null)
                    {
                        int dataIndex = _currentRow.DisplayIndex;
                        if (dataIndex >= 0 && dataIndex < _fullData.Count)
                        {
                            var wrapper = _fullData[dataIndex] as DataRowWrapper;
                            if (wrapper != null)
                            {
                                foreach (var cell in _currentRow.Cells)
                                {
                                    var col = Columns[cell.ColumnIndex];
                                    if (col.IsSelectionCheckBox || col.IsRowNumColumn || col.IsRowID)
                                        continue;

                                    var prop = wrapper.OriginalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                                    if (prop != null)
                                    {
                                        cell.CellValue = prop.GetValue(wrapper.OriginalData);
                                        cell.IsDirty = false;
                                    }
                                }
                                wrapper.RowState = DataRowState.Unchanged;
                                wrapper.DateTimeChange = DateTime.Now;
                                _currentRow.IsDirty = false;

                                EnsureTrackingForItem(wrapper);
                            }
                        }
                    }

                    UpdateRecordNumber();
                    UpdateNavigationButtonState();
                    UpdatePagingControls();
                    FillVisibleRows();
                    UpdateScrollBars();
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                //MiscFunctions.SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void EditpictureBox_Click(object sender, EventArgs e)
        {
            EditCalled?.Invoke(sender, EventArgs.Empty);
        }

        private void FindpictureBox_Click(object sender, EventArgs e)
        {
            ShowSearch?.Invoke(this, EventArgs.Empty);
        }

        private void PrinterpictureBox_Click(object sender, EventArgs e)
        {
            CallPrinter?.Invoke(this, EventArgs.Empty);
            if (Rows.Count > 0)
            {
                PrintGridAsReportDocument(false, $"{Entity.EntityName} Report", true);
            }
            else
            {
                MessageBox.Show("No data to print.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void MessagepictureBox_Click(object sender, EventArgs e)
        {
            SendMessage?.Invoke(this, EventArgs.Empty);
        }
        #endregion
        #region Print
        public void PrintGridAsReportDocument(
       bool blackAndWhite = false,
       string reportTitle = null,
       bool showDateTime = true,
       bool showPreview = true,
       bool showPrintDialogBeforePrint = false,
       string saveAsPdfPath = null
   )
        {
            // 1) Gather all rows we actually want to print
            GetAllRows(); // your existing method to ensure Rows contains everything

            if (Rows.Count == 0)
            {
                MessageBox.Show("No data to print.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

          
            // make a snapshot so we don’t accidentally re-evaluate Rows inside the loop
            var printRows = Rows.ToList();

            // pagination state
            int _printRowIndex = 0;
            int _printPageNumber = 1;

            var pd = new PrintDocument
            {
                DefaultPageSettings = { Landscape = true }
            };
            // Calculate the total number of pages by simulating pagination
            int totalPages = CalculateTotalPages(pd.DefaultPageSettings);

            pd.BeginPrint += (s, e) =>
            {
                _printRowIndex = 0;
                _printPageNumber = 1;
            };

            pd.PrintPage += (s, e) =>
            {
                bool origBW = DrawInBlackAndWhite;
                DrawInBlackAndWhite = blackAndWhite;

                var g = e.Graphics;
                int left = e.MarginBounds.Left;
                int right = e.MarginBounds.Right;
                int top = e.MarginBounds.Top;
                int bottom = e.MarginBounds.Bottom;

                // fonts
                using var titleFont = new Font(this.Font.FontFamily, 16f, FontStyle.Bold);
                using var smallFont = new Font(this.Font.FontFamily, 10f, FontStyle.Regular);

                int y = top;

                // --- 1) Title & Date at the very top ---
                float titleH = 0, dateH = 0;
                if (!string.IsNullOrEmpty(reportTitle))
                {
                    var sz = g.MeasureString(reportTitle, titleFont);
                    float tx = left + (e.MarginBounds.Width - sz.Width) / 2;
                    g.DrawString(reportTitle, titleFont, Brushes.Black, tx, y);
                    titleH = sz.Height;
                }

                if (showDateTime)
                {
                    string dtText = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    var dtSz = g.MeasureString(dtText, smallFont);
                    float dx = right - dtSz.Width;
                    g.DrawString(dtText, smallFont, Brushes.Black, dx, y);
                    dateH = dtSz.Height;
                }

                // move past title/date
                y += (int)(Math.Max(titleH, dateH) + 10);

                // --- 2) Column headers ---
                int cx = left;
                int headerH = ColumnHeaderHeight;
                using var hdrBack = new SolidBrush(_currentTheme.GridHeaderBackColor);
                using var hdrFore = new SolidBrush(_currentTheme.PrimaryTextColor);
                using var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                foreach (var col in Columns.Where(c => c.Visible))
                {
                    var r = new Rectangle(cx, y, col.Width, headerH);
                    g.FillRectangle(hdrBack, r);
                    g.DrawString(
                        col.ColumnCaption ?? "",
                        ColumnHeaderFont ?? this.Font,
                        hdrFore,
                        r,
                        sf
                    );
                    g.DrawRectangle(Pens.Gray, r);
                    cx += col.Width;
                }

                // advance past headers
                y += headerH;

                // --- 3) Data rows pagination loop ---
                int rowH = RowHeight;
                while (_printRowIndex < printRows.Count)
                {
                    // if next row would overflow bottomMargin, break for next page
                    if (y + rowH > bottom - 50)
                        break;

                    var row = printRows[_printRowIndex];
                    int cellX = left;

                    foreach (var cell in row.Cells)
                    {
                        var col = Columns[cell.ColumnIndex];
                        if (!col.Visible) continue;

                        var cellRect = new Rectangle(cellX, y, col.Width, rowH);
                        var bgColor = (row.RowIndex == _currentRowIndex)
                                      ? _currentTheme.SelectedRowBackColor
                                      : _currentTheme.GridBackColor;

                        // if it’s a simple text cell, draw it directly
                        if (_columnDrawers.TryGetValue(col.ColumnName, out var editor)
                            && (editor is BeepLabel || editor is BeepTextBox))
                        {
                            using var txtFmt = new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center,
                                Trimming = StringTrimming.EllipsisCharacter
                            };
                            using var txtBrush = new SolidBrush(_currentTheme.GridForeColor);
                            g.DrawString(
                                cell.CellValue?.ToString() ?? "",
                                ColumnHeaderFont ?? this.Font,
                                txtBrush,
                                cellRect,
                                txtFmt
                            );
                        }
                        else
                        {
                            // otherwise fall back to your PaintCell for rich controls
                            PaintCell(g, cell, cellRect, bgColor);
                        }

                        cellX += col.Width;
                    }

                    y += rowH;
                    _printRowIndex++;
                }

                // --- 4) Footer: page number ---
                using var footerFont = smallFont;
                string pg = $"Page {_printPageNumber}- {totalPages}";
                var pgSz = g.MeasureString(pg, footerFont);
                float px = left + (e.MarginBounds.Width - pgSz.Width) / 2;
                float py = bottom + 10;
                g.DrawString(pg, footerFont, Brushes.Black, px, py);

                // restore B/W mode
                DrawInBlackAndWhite = origBW;

                // tell the framework if we’ve got more pages
                e.HasMorePages = (_printRowIndex < printRows.Count);
                if (e.HasMorePages)
                    _printPageNumber++;
            };

            // PDF output?
            if (!string.IsNullOrEmpty(saveAsPdfPath))
            {
                pd.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                pd.PrinterSettings.PrintToFile = true;
                pd.PrinterSettings.PrintFileName = saveAsPdfPath;
            }

            // preview or direct print
            if (showPreview && string.IsNullOrEmpty(saveAsPdfPath))
            {
                using var preview = new BeepPrintPreviewForm(pd);
                preview.ShowDialog();
            }
            else if (showPrintDialogBeforePrint)
            {
                using var dlg = new PrintDialog { Document = pd };
                if (dlg.ShowDialog() == DialogResult.OK)
                    pd.Print();
            }
            else
            {
                pd.Print();
            }
        }

        private int CalculateTotalPages(PageSettings pageSettings)
        {
            // If no rows, there's just one empty page
            if (Rows.Count == 0)
                return 1;

            // Create a bitmap to measure text with the same DPI as printer
            using Bitmap bmp = new Bitmap(1, 1);
            using Graphics g = Graphics.FromImage(bmp);
            g.PageUnit = GraphicsUnit.Display;

            // Get page dimensions
            int pageWidth = pageSettings.Bounds.Width;
            int pageHeight = pageSettings.Bounds.Height;
            int marginLeft = pageSettings.Margins.Left;
            int marginTop = pageSettings.Margins.Top;
            int marginRight = pageSettings.Margins.Right;
            int marginBottom = pageSettings.Margins.Bottom;

            // Calculate printable area
            int printableWidth = pageWidth - marginLeft - marginRight;
            int printableHeight = pageHeight - marginTop - marginBottom;

            // Estimate header height (title + date + column headers)
            int headerHeight = 0;
            using (var titleFont = new Font(this.Font.FontFamily, 16f, FontStyle.Bold))
            {
                headerHeight += (int)g.MeasureString("Sample", titleFont).Height + 10;
            }
            headerHeight += ColumnHeaderHeight;

            // Reserve space for page footer
            int footerHeight = 50;

            // Calculate available space for rows
            int availableHeight = printableHeight - headerHeight - footerHeight;

            // Calculate how many rows fit on a page
            int rowsPerPage = Math.Max(1, availableHeight / RowHeight);

            // Calculate total pages
            return (int)Math.Ceiling((double)Rows.Count / rowsPerPage);
        }

        #endregion Print
        #region Helper Methods
        private int GetColumnWidth(BeepColumnConfig column)
        {
            if (column == null) return _defaultcolumnheaderwidth;

            int maxWidth = 50; // Minimum width
            int padding = 8; // Padding for text

            using (Graphics g = CreateGraphics())
            {
                // Measure header text
                string headerText = column.ColumnCaption ?? column.ColumnName ?? "";
                if (!string.IsNullOrEmpty(headerText))
                {
                    SizeF headerSize = g.MeasureString(headerText, _columnHeadertextFont ?? Font);
                    maxWidth = Math.Max(maxWidth, (int)headerSize.Width + padding);
                }

                // Add space for sort/filter icons if enabled
                if (column.ShowSortIcon || column.ShowFilterIcon)
                {
                    int iconSpace = 0;
                    if (column.ShowSortIcon) iconSpace += 20; // Sort icon width
                    if (column.ShowFilterIcon) iconSpace += 20; // Filter icon width
                    maxWidth += iconSpace + 4; // Extra padding for icons
                }

                // Measure cell content if we have data
                if (_fullData != null && _fullData.Any())
                {
                    int sampleSize = Math.Min(100, _fullData.Count); // Sample first 100 rows for performance

                    for (int i = 0; i < sampleSize; i++)
                    {
                        var wrapper = _fullData[i] as DataRowWrapper;
                        if (wrapper?.OriginalData != null)
                        {
                            var prop = wrapper.OriginalData.GetType().GetProperty(column.ColumnName ?? column.ColumnCaption);
                            if (prop != null)
                            {
                                var value = prop.GetValue(wrapper.OriginalData);
                                string cellText = value?.ToString() ?? "";

                                if (!string.IsNullOrEmpty(cellText))
                                {
                                    SizeF cellSize = g.MeasureString(cellText, Font);
                                    maxWidth = Math.Max(maxWidth, (int)cellSize.Width + padding);
                                }
                            }
                        }
                    }
                }
            }

            // Cap the maximum width to prevent extremely wide columns
            return Math.Min(maxWidth, 300);
        }
        public override void SuspendFormLayout()
        {
            
          base.SuspendFormLayout();
            this.SuspendLayout();
            //    this.SuspendDrawing();

           // suspend all child controls
            foreach (Control ctrl in this.Controls)
            {
                //      ctrl.SuspendDrawing();
                ctrl.SuspendLayout();
            }
            _navigatorDrawn = true;
            _filterpaneldrawn = true;
            // suspend all tabs and their controls


        }
        public override void ResumeFormLayout()
        {
            
            base.ResumeFormLayout();
           // this.ResumeDrawing();

            // resume all child controls
            foreach (Control ctrl in this.Controls)
            {
                //      ctrl.ResumeDrawing();
                ctrl.ResumeLayout();
            }
            this.ResumeLayout();
            _navigatorDrawn = false;
            _filterpaneldrawn = false;
        }

        public BeepColumnConfig GetColumnByName(string columnName)
        {
            // Find index first
            int index = Columns.FindIndex(c => c.ColumnName.Equals(columnName,StringComparison.InvariantCultureIgnoreCase));
            if (index == -1) return null;
            return Columns[index];
            
        }
        public Dictionary<string,BeepColumnConfig> GetDictionaryColumns()
        {
            Dictionary<string, BeepColumnConfig> dict = new Dictionary<string, BeepColumnConfig>();
            foreach (var col in Columns)
            {
                dict.Add(col.ColumnName, col);
            }
            return dict;
        }
        public BeepColumnConfig GetColumnByIndex(int index)
        {  // Find index first
            int retindex = Columns.FindIndex(c => c.Index == index);
            if (retindex == -1) return null;
            return Columns[retindex];
        }
        public BeepColumnConfig GetColumnByCaption(string caption)
        {
            int index = Columns.FindIndex(c => c.ColumnName.Equals(caption, StringComparison.InvariantCultureIgnoreCase));
            if (index == -1) return null;

            return Columns[index];
        }
        private DbFieldCategory MapPropertyTypeToDbFieldCategory(Type type)
        {
            if (type == typeof(string)) return DbFieldCategory.String;
            if (type == typeof(Char)) return DbFieldCategory.Char;
            if (type == typeof(int) || type == typeof(long)) return DbFieldCategory.Numeric;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return DbFieldCategory.Numeric;
            if (type == typeof(DateTime)) return DbFieldCategory.Date;
            if (type == typeof(bool)) return DbFieldCategory.Boolean;
            if (type == typeof(Guid)) return DbFieldCategory.Guid;

            // Default to string for unknown types
            return DbFieldCategory.String;
        }

        private BeepColumnType MapPropertyTypeToCellEditor(Type type)
        {
            if (type == typeof(string)) return BeepColumnType.Text;
            if (type == typeof(int) || type == typeof(long)) return BeepColumnType.NumericUpDown; // Integer-based controls
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return BeepColumnType.Text; // Use a formatted text field for decimal numbers
            if (type == typeof(DateTime)) return BeepColumnType.DateTime;
            if (type == typeof(bool)) return BeepColumnType.CheckBoxBool;
            if (type == typeof(Char)) return BeepColumnType.CheckBoxChar;
            if (type == typeof(Guid)) return BeepColumnType.Text; // Could be a readonly label or a dropdown if referencing something

            // Fallback to text if the type is unknown
            return BeepColumnType.Text;
        }
        private DbFieldCategory MapPropertyTypeToDbFieldCategory(string propertyTypeName)
        {
            // Default to String if typeName is null or empty
            if (string.IsNullOrWhiteSpace(propertyTypeName))
            {
            //    //MiscFunctions.SendLog("MapPropertyTypeToDbFieldCategory: propertyTypeName is null or empty, defaulting to String");
                return DbFieldCategory.String;
            }

            // Attempt to resolve the Type from the AssemblyQualifiedName
            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
            {
             //   //MiscFunctions.SendLog($"MapPropertyTypeToDbFieldCategory: Could not resolve type '{propertyTypeName}', defaulting to String");
                return DbFieldCategory.String;
            }

            // Map resolved Type to DbFieldCategory
            if (type == typeof(string)) return DbFieldCategory.String;
            if (type == typeof(int) || type == typeof(long)) return DbFieldCategory.Numeric;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return DbFieldCategory.Numeric;
            if (type == typeof(DateTime)) return DbFieldCategory.Date;
            if (type == typeof(bool)) return DbFieldCategory.Boolean;
            if (type == typeof(Char)) return DbFieldCategory.Char;
            if (type == typeof(Guid)) return DbFieldCategory.Guid;

            // Default to String for unknown types
          //  //MiscFunctions.SendLog($"MapPropertyTypeToDbFieldCategory: Unknown type '{type.FullName}', defaulting to String");
            return DbFieldCategory.String;
        }
        private BeepColumnType MapPropertyTypeToCellEditor(string propertyTypeName)
        {
            // Default to Text if typeName is null or empty
            if (string.IsNullOrWhiteSpace(propertyTypeName))
            {
            //    //MiscFunctions.SendLog("MapPropertyTypeToCellEditor: propertyTypeName is null or empty, defaulting to Text");
                return BeepColumnType.Text;
            }

            // Attempt to resolve the Type from the AssemblyQualifiedName
            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
            {
              //  //MiscFunctions.SendLog($"MapPropertyTypeToCellEditor: Could not resolve type '{propertyTypeName}', defaulting to Text");
                return BeepColumnType.Text;
            }

            // Map resolved Type to BeepColumnType
            if (type == typeof(string)) return BeepColumnType.Text;
            if (type == typeof(int) || type == typeof(long)) return BeepColumnType.NumericUpDown;
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return BeepColumnType.Text; // Use formatted text for decimals
            if (type == typeof(DateTime)) return BeepColumnType.DateTime;
            if (type == typeof(bool)) return BeepColumnType.CheckBoxBool;
            if (type == typeof(Char)) return BeepColumnType.CheckBoxChar;
            if (type == typeof(Guid)) return BeepColumnType.Text;

            // Fallback to Text for unknown types
          //  //MiscFunctions.SendLog($"MapPropertyTypeToCellEditor: Unknown type '{type.FullName}', defaulting to Text");
            return BeepColumnType.Text;
        }
        private Color GetEffectiveColor(Color originalColor)
        {
            if (!DrawInBlackAndWhite)
                return originalColor;

            // Convert to grayscale
            int gray = (int)(0.3 * originalColor.R + 0.59 * originalColor.G + 0.11 * originalColor.B);
            return Color.FromArgb(originalColor.A, gray, gray, gray);
        }

        #endregion
        #region "Mouse Events"
        // Override OnMouseMove to handle button hover effects
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // First call base implementation
            base.OnMouseMove(e);

            // Reset tooltip shown flag when mouse moves
            tooltipShown = false;

            // Handle column/row resizing (from BeepGrid_MouseMove)
            if (_resizingColumn && _resizingIndex >= 0)
            {
                int deltaX = e.X - _lastMousePos.X;
                if (_resizingIndex < Columns.Count)
                {
                    Columns[_resizingIndex].Width = Math.Max(20, Columns[_resizingIndex].Width + deltaX);
                }
                _lastMousePos = e.Location;
                UpdateScrollBars();
                InvalidateColumn(_resizingIndex);
                return; // Early return when resizing
            }
            else if (_resizingRow && _resizingIndex >= 0)
            {
                int deltaY = e.Y - _lastMousePos.Y;
                int currentRowHeight = Rows[_resizingIndex].Height;
                int newHeight = Math.Max(10, currentRowHeight + deltaY);
                if (newHeight != currentRowHeight)
                {
                    int displayIndex = Rows[_resizingIndex].DisplayIndex;
                    Rows[_resizingIndex].Height = newHeight;
                    foreach (var cell in Rows[_resizingIndex].Cells)
                    {
                        cell.Height = newHeight;
                    }
                    _rowHeights[displayIndex] = newHeight; // Track resized height

                    _lastMousePos = e.Location;
                    FillVisibleRows(); // Refresh visible rows with new height
                    UpdateCellPositions();
                    UpdateScrollBars();
                    //Invalidate();
                }
                return; // Early return when resizing
            }
            else
            {
                // Cursor handling based on where we are hovering
                if (IsNearColumnBorder(e.Location, out _resizingIndex))
                {
                    this.Cursor = Cursors.SizeWE;
                    return; // Set cursor and return
                }
                else if (IsNearRowBorder(e.Location, out _resizingIndex))
                {
                    this.Cursor = Cursors.SizeNS;
                    return; // Set cursor and return
                }
            }

            // Handle navigator button hover (from the original OnMouseMove)
            if (_navigatorDrawn)
            {
                // Check if mouse is over any hit area
                if (HitTest(e.Location, out var hitTest))
                {
                    if (hitTest.Name.EndsWith("Button"))
                    {
                        Cursor = Cursors.Hand;
                        Invalidate(hitTest.TargetRect); // Redraw just this button area
                        return;
                    }
                }
            }

            // Reset cursor if we reached this point and none of the above conditions matched
            this.Cursor = Cursors.Default;
        }
   
        // Override OnMouseLeave to reset hover state
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            tooltipShown = false;

            if (Cursor == Cursors.Hand)
            {
                Cursor = Cursors.Default;
                Invalidate(navigatorPanelRect);
            }
        }
        // New method to handle navigation button clicks via hit testing
        protected override void OnMouseClick(MouseEventArgs e)
        {
            // If the editor is shown and the click is within its bounds,
            // let the editor handle the click and do nothing in the grid.
            if (IsEditorShown && _editingControl != null && _editingControl.Bounds.Contains(e.Location))
            {
                return;
            }
            base.OnMouseClick(e);

            // Handle hit-testing for navigation, sorting, and filtering icons first
            if (HitTest(e.Location, out var hitTest))
            {
                if (hitTest.Name.StartsWith("SortIcon_"))
                {
                    int columnIndex = int.Parse(hitTest.Name.Substring("SortIcon_".Length));
                    HandleSortIconClick(columnIndex);
                    return;
                }
                if (hitTest.Name.StartsWith("FilterIcon_"))
                {
                    int columnIndex = int.Parse(hitTest.Name.Substring("FilterIcon_".Length));
                    HandleFilterIconClick(columnIndex);
                    return;
                }
                // Handle navigation button clicks
                switch (hitTest.Name)
                {
                    case "FirstRecordButton": FirstPageButton_Click(this, EventArgs.Empty); return;
                    case "PreviousRecordButton": PreviouspictureBox_Click(this, EventArgs.Empty); return;
                    case "NextRecordButton": NextpictureBox_Click(this, EventArgs.Empty); return;
                    case "LastRecordButton": LastPageButton_Click(this, EventArgs.Empty); return;
                    case "FindButton": FindpictureBox_Click(this, EventArgs.Empty); return;
                    case "EditButton": EditpictureBox_Click(this, EventArgs.Empty); return;
                    case "PrinterButton": PrinterpictureBox_Click(this, EventArgs.Empty); return;
                    case "MessageButton": MessagepictureBox_Click(this, EventArgs.Empty); return;
                    case "SaveButton": SavepictureBox_Click(this, EventArgs.Empty); return;
                    case "NewButton": NewButton_Click(this, EventArgs.Empty); return;
                    case "RemoveButton": RemovepictureBox_Click(this, EventArgs.Empty); return;
                    case "RollbackButton": RollbackpictureBox_Click(this, EventArgs.Empty); return;
                    case "FirstPageButton": FirstPageButton_Click(this, EventArgs.Empty); return;
                    case "PrevPageButton": PrevPageButton_Click(this, EventArgs.Empty); return;
                    case "NextPageButton": NextPageButton_Click(this, EventArgs.Empty); return;
                    case "LastPageButton": LastPageButton_Click(this, EventArgs.Empty); return;
                }
            }

            // Handle clicks within the grid area for cell selection and editing
            var clickedCell = GetCellAtLocation(e.Location);
            if (clickedCell == null)
            {
                CloseCurrentEditor();
                return;
            }

            // If the clicked cell is already being edited, do nothing.
            if (_editingCell != null && _editingCell.Id == clickedCell.Id)
            {
                return;
            }

            // If another cell is being edited, close the editor first.
            if (_editingCell != null)
            {
                CloseCurrentEditor();
            }

            // Select the new cell.
            SelectCell(clickedCell);

            // Handle checkbox clicks immediately.
            var column = Columns[clickedCell.ColumnIndex];
            if (column.IsSelectionCheckBox)
            {
                int dataIndex = _dataOffset + clickedCell.RowIndex;
                if (dataIndex < _fullData.Count)
                {
                    var dataItem = _fullData[dataIndex] as DataRowWrapper;
                    if (dataItem != null)
                    {
                        bool isSelected = !(bool)(clickedCell.CellValue ?? false);
                        _persistentSelectedRows[dataItem.RowID] = isSelected;
                        clickedCell.CellValue = isSelected;
                        RaiseSelectedRowsChanged();
                        InvalidateCell(clickedCell);
                    }
                }
                return; // No editor needed for checkbox.
            }

            // Show editor for editable cells.
            if (!column.ReadOnly)
            {
                ShowCellEditor(clickedCell, e.Location);
            }

            CellClick?.Invoke(this, new BeepCellEventArgs(clickedCell));
        }
        private void BeepGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (IsNearColumnBorder(e.Location, out _resizingIndex))
            {
                _resizingColumn = true;
                _lastMousePos = e.Location;
                this.Cursor = Cursors.SizeWE;
            }
            else if (IsNearRowBorder(e.Location, out _resizingIndex))
            {
                _resizingRow = true;
                _lastMousePos = e.Location;
                this.Cursor = Cursors.SizeNS;
            }
        }
      
        private void BeepGrid_MouseUp(object sender, MouseEventArgs e)
        {
            _resizingColumn = false;
            _resizingRow = false;
            this.Cursor = Cursors.Default;
            // FillVisibleRows(); // Ensure data is redrawn after resizing
            Invalidate();
        }
        #endregion "Mouse Events"
        #region "Sorting"
        private void HandleSortIconClick(int columnIndex)
        {
            var column = Columns.FirstOrDefault(c => c.Index == columnIndex);
            if (column == null) return;

            // Cycle through sort directions
            switch (column.SortDirection)
            {
                case SortDirection.None:
                    column.SortDirection = SortDirection.Ascending;
                    break;
                case SortDirection.Ascending:
                    column.SortDirection = SortDirection.Descending;
                    break;
                case SortDirection.Descending:
                    column.SortDirection = SortDirection.None;
                    break;
            }

            // Clear other column sort directions (single column sort)
            foreach (var col in Columns)
            {
                if (col.Index != columnIndex)
                    col.SortDirection = SortDirection.None;
            }

            // Apply sorting using the same efficient methodology as your filter
            ApplySorting(column);
            Invalidate();
        }
        public void ClearSort()
        {
            // Reset all column sort directions
            foreach (var col in Columns)
            {
                col.SortDirection = SortDirection.None;
            }

            // Reset to original data (same pattern as your filter reset)
            if (originalList != null && originalList.Any())
            {
                _fullData.Clear();
                _fullData.AddRange(originalList);

                // Update RowIDs
                for (int i = 0; i < _fullData.Count; i++)
                {
                    if (_fullData[i] is DataRowWrapper dataRow)
                    {
                        dataRow.RowID = i;
                    }
                }

                _dataOffset = 0;
                UpdateTrackingIndices();
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
        }
        private void ApplySortAndFilter()
        {
            // **EXACT SAME STARTING PATTERN AS YOUR FILTER**
            // Start with original data
            List<object> workingData = new List<object>(originalList);

            // **SAME FILTER APPLICATION AS YOUR EXISTING METHOD**
            // Apply filter first if active (same as your filter logic)
            if (_filteredData != null && _filteredData.Any())
            {
                workingData = new List<object>(_filteredData);
            }

            // **THEN APPLY SORT USING SAME METHODOLOGY**
            var sortColumn = Columns.FirstOrDefault(c => c.SortDirection != SortDirection.None);
            if (sortColumn != null)
            {
                var propertyName = sortColumn.ColumnName;
                if (!string.IsNullOrEmpty(propertyName))
                {
                    // **SAME PRE-EXTRACTION PATTERN AS BOTH FILTER AND SORT**
                    var sortData = new List<(object wrapper, object sortValue)>();

                    foreach (var item in workingData)
                    {
                        var wrapper = item as DataRowWrapper;
                        if (wrapper?.OriginalData != null)
                        {
                            object sortValue = null;
                            try
                            {
                                var prop = wrapper.OriginalData.GetType().GetProperty(propertyName);
                                sortValue = prop?.GetValue(wrapper.OriginalData);
                            }
                            catch (Exception ex)
                            {
                                //MiscFunctions.SendLog($"ApplySortAndFilter: Error accessing property {propertyName} on item {wrapper.OriginalData}: {ex.Message}");
                                sortValue = null;
                            }
                            sortData.Add((wrapper, sortValue));
                        }
                    }

                    // **SAME SORTING LOGIC**
                    IEnumerable<(object wrapper, object sortValue)> sortedData;
                    if (sortColumn.SortDirection == SortDirection.Ascending)
                    {
                        sortedData = sortData.OrderBy(item => item.sortValue, new SafeComparer());
                    }
                    else
                    {
                        sortedData = sortData.OrderByDescending(item => item.sortValue, new SafeComparer());
                    }

                    workingData = sortedData.Select(item => item.wrapper).ToList();
                }
            }

            // **SAME FINAL UPDATE PATTERN AS YOUR FILTER**
            _fullData.Clear();
            _fullData.AddRange(workingData);

            // **SAME STATE MANAGEMENT AS YOUR FILTER**
            for (int i = 0; i < _fullData.Count; i++)
            {
                if (_fullData[i] is DataRowWrapper dataRow)
                {
                    dataRow.RowID = i;
                }
            }

            _dataOffset = 0;
            UpdateTrackingIndices();
            FillVisibleRows();
            UpdateScrollBars();
        }
        private void ApplySorting(BeepColumnConfig sortColumn)
        {
            // **EXACT SAME NULL/EMPTY CHECKS AS YOUR FILTER**
            if (_fullData == null || !_fullData.Any())
            {
                return;
            }

            // If sort direction is None, reset to the currently filtered or original list
            if (sortColumn.SortDirection == SortDirection.None)
            {
                _fullData.Clear();
                // If there's filtered data, the "unsorted" state is the filtered data. Otherwise, it's the original list.
                if (_filteredData != null && _filteredData.Any())
                {
                    _fullData.AddRange(_filteredData);
                }
                else
                {
                    _fullData.AddRange(originalList);
                }
            }
            else
            {
                // **USE DIRECT LINQ - EXACT SAME PATTERN AS YOUR FILTER**
                try
                {
                    var propertyName = sortColumn.ColumnName;
                    if (string.IsNullOrEmpty(propertyName)) return;

                    IEnumerable<object> sortedData;

                    // The lambda function to extract the sort key, mirroring your filter's property access logic
                    Func<object, object> keySelector = wrapper =>
                    {
                        var dataItem = wrapper as DataRowWrapper;
                        if (dataItem == null || dataItem.OriginalData == null) return null;

                        var item = dataItem.OriginalData;
                        try
                        {
                            var prop = item.GetType().GetProperty(propertyName);
                            if (prop == null) return null;
                            return prop.GetValue(item);
                        }
                        catch (Exception)
                        {
                            // Same graceful failure as your filter's lambda
                            return null;
                        }
                    };

                    // Apply OrderBy or OrderByDescending directly on _fullData
                    if (sortColumn.SortDirection == SortDirection.Ascending)
                    {
                        sortedData = _fullData.OrderBy(keySelector, new SafeComparer()).ToList();
                    }
                    else
                    {
                        sortedData = _fullData.OrderByDescending(keySelector, new SafeComparer()).ToList();
                    }

                    // **EXACT SAME UPDATE PATTERN AS YOUR FILTER**
                    _fullData.Clear();
                    _fullData.AddRange(sortedData);
                }
                catch (Exception ex)
                {
                    // **EXACT SAME ERROR HANDLING AS YOUR FILTER**
                    MessageBox.Show($"Error during sort: {ex.Message}", "Sort Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // On failure, revert to the original list to maintain a stable state
                    _fullData.Clear();
                    _fullData.AddRange(originalList);
                }
            }

            // **EXACT SAME STATE REFRESH AS YOUR FILTER**
            _dataOffset = 0;
            InitializeRows();
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
            OnSortChanged(); // Notify that the sort has changed
        }
        // Add this custom comparer class (similar to your filter's type handling)
        private class SafeComparer : IComparer<object>
        {
            public int Compare(object x, object y)
            {
                // Handle null values (same null handling as your filter)
                if (x == null && y == null) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                // Handle same type comparison
                if (x.GetType() == y.GetType() && x is IComparable comparableX)
                {
                    return comparableX.CompareTo(y);
                }

                // Handle different types or non-comparable types (same fallback as your filter)
                try
                {
                    // Convert to strings for comparison as fallback
                    string strX = x.ToString();
                    string strY = y.ToString();

                    // Try numeric comparison first (same logic as your filter's type detection)
                    if (double.TryParse(strX, out double numX) && double.TryParse(strY, out double numY))
                    {
                        return numX.CompareTo(numY);
                    }

                    // Try date comparison (same logic as your filter)
                    if (DateTime.TryParse(strX, out DateTime dateX) && DateTime.TryParse(strY, out DateTime dateY))
                    {
                        return dateX.CompareTo(dateY);
                    }

                    // Default to string comparison (same fallback as your filter)
                    return string.Compare(strX, strY, StringComparison.OrdinalIgnoreCase);
                }
                catch
                {
                    return 0; // Equal if comparison fails
                }
            }
        }

        private void HandleFilterIconClick(int columnIndex)
        {
            var column = Columns.FirstOrDefault(c => c.Index == columnIndex);
            if (column == null) return;

            // Toggle filter state or show filter dialog
            column.IsFiltered = !column.IsFiltered;

            // You can implement a filter dialog here or use the existing filter functionality
            // For now, let's just toggle the visual state
            Invalidate();

            // Raise an event for custom filter handling
            OnColumnFilterClicked(new ColumnFilterEventArgs(column, columnIndex));
        }
        public event EventHandler<ColumnFilterEventArgs> ColumnFilterClicked;
        public event EventHandler<ColumnSortEventArgs> ColumnSortClicked;
        public event EventHandler SortChanged;
        protected virtual void OnSortChanged()
        {
            SortChanged?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnColumnFilterClicked(ColumnFilterEventArgs e)
        {
            ColumnFilterClicked?.Invoke(this, e);
        }

        protected virtual void OnColumnSortClicked(ColumnSortEventArgs e)
        {
            ColumnSortClicked?.Invoke(this, e);
        }

        public class ColumnFilterEventArgs : EventArgs
        {
            public BeepColumnConfig Column { get; }
            public int ColumnIndex { get; }

            public ColumnFilterEventArgs(BeepColumnConfig column, int columnIndex)
            {
                Column = column;
                ColumnIndex = columnIndex;
            }
        }

        public class ColumnSortEventArgs : EventArgs
        {
            public BeepColumnConfig Column { get; }
            public int ColumnIndex { get; }
            public SortDirection Direction { get; }

            public ColumnSortEventArgs(BeepColumnConfig column, int columnIndex, SortDirection direction)
            {
                Column = column;
                ColumnIndex = columnIndex;
                Direction = direction;
            }
        }
        #endregion "Sorting"
        #region "High DPI Support"
        // Add these fields to cache DPI-scaled values
        private float _cachedDpiScale = 1.0f;
        private int _scaledRowHeight;
        private int _scaledColumnHeaderHeight;
        private int _scaledDefaultColumnWidth;
        private Dictionary<int, int> _scaledColumnWidths = new Dictionary<int, int>();
        private bool _dpiCacheValid = false;

        // Add this method to update cached DPI values
        private void UpdateDpiCache()
        {
            if (_dpiCacheValid && Math.Abs(_cachedDpiScale - DpiScaleFactor) < 0.01f)
                return;

            _cachedDpiScale = DpiScaleFactor;
            _scaledRowHeight = ScaleValue(_rowHeight);
            _scaledColumnHeaderHeight = ScaleValue(_defaultcolumnheaderheight);
            _scaledDefaultColumnWidth = ScaleValue(_defaultcolumnheaderwidth);

            // Cache scaled column widths
            _scaledColumnWidths.Clear();
            foreach (var col in Columns)
            {
                _scaledColumnWidths[col.Index] = ScaleValue(col.Width);
            }

            _dpiCacheValid = true;
        }

        // Override DPI change handler
        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);
            _dpiCacheValid = false;
            UpdateDpiCache();
            _layoutDirty = true;
            Invalidate();
        }
        #endregion "High DPI Support"
        #region Column Color Management

        /// <summary>
        /// Sets custom colors for a specific column
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="backColor">Background color for cells</param>
        /// <param name="foreColor">Text color for cells</param>
        /// <param name="headerBackColor">Background color for header (optional)</param>
        /// <param name="headerForeColor">Text color for header (optional)</param>
        public void SetColumnColors(string columnName, Color backColor, Color foreColor,
            Color? headerBackColor = null, Color? headerForeColor = null)
        {
            var column = GetColumnByName(columnName);
            if (column != null)
            {
                column.UseCustomColors = true;
                column.ColumnBackColor = backColor;
                column.ColumnForeColor = foreColor;

                if (headerBackColor.HasValue)
                    column.ColumnHeaderBackColor = headerBackColor.Value;
                if (headerForeColor.HasValue)
                    column.ColumnHeaderForeColor = headerForeColor.Value;

                Invalidate(); // Redraw grid with new colors
            }
        }

        /// <summary>
        /// Sets custom colors for a column by index
        /// </summary>
        public void SetColumnColors(int columnIndex, Color backColor, Color foreColor,
            Color? headerBackColor = null, Color? headerForeColor = null)
        {
            var column = GetColumnByIndex(columnIndex);
            if (column != null)
            {
                column.UseCustomColors = true;
                column.ColumnBackColor = backColor;
                column.ColumnForeColor = foreColor;

                if (headerBackColor.HasValue)
                    column.ColumnHeaderBackColor = headerBackColor.Value;
                if (headerForeColor.HasValue)
                    column.ColumnHeaderForeColor = headerForeColor.Value;

                Invalidate();
            }
        }

        /// <summary>
        /// Removes custom colors from a column (reverts to theme colors)
        /// </summary>
        public void ClearColumnColors(string columnName)
        {
            var column = GetColumnByName(columnName);
            if (column != null)
            {
                column.UseCustomColors = false;
                column.ColumnBackColor = Color.Empty;
                column.ColumnForeColor = Color.Empty;
                column.ColumnHeaderBackColor = Color.Empty;
                column.ColumnHeaderForeColor = Color.Empty;
                column.ColumnBorderColor = Color.Empty;

                Invalidate();
            }
        }

        /// <summary>
        /// Gets the effective background color for a column
        /// </summary>
        public Color GetColumnBackColor(BeepColumnConfig column)
        {
            return column.HasCustomBackColor ? column.ColumnBackColor : _currentTheme.GridBackColor;
        }

        /// <summary>
        /// Gets the effective foreground color for a column
        /// </summary>
        public Color GetColumnForeColor(BeepColumnConfig column)
        {
            return column.HasCustomForeColor ? column.ColumnForeColor : _currentTheme.GridForeColor;
        }

        #endregion
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _persistentSelectedRows.Clear(); // Clear p
                if (_verticalScrollBar != null)
                {
                    _verticalScrollBar.Dispose();
                    _verticalScrollBar = null;
                }
                if (_horizontalScrollBar != null)
                {
                    _horizontalScrollBar.Dispose();
                    _horizontalScrollBar = null;
                }
                if(_editingControl != null)
                {
                    _editingControl.Dispose();
                    _editingControl = null;
                }
                //if (_editorPopupForm != null)
                //{
                //    _editorPopupForm.Dispose();
                //    _editorPopupForm = null;
                //}
                if (titleLabel != null)
                {
                    titleLabel.Dispose();
                    titleLabel = null;
                }
                if (DataNavigator != null)
                {
                    DataNavigator.Dispose();
                    DataNavigator = null;
                }
                if (Rows != null)
                {
                    Rows.ListChanged -= Rows_ListChanged;
                    Rows.Clear();
                    Rows = null;
                }
                if (Columns != null)
                {
                    Columns.Clear();
                    Columns = null;
                }
                if (disposing && _selectAllCheckBox != null)
                {
                    _selectAllCheckBox.Dispose();
                    _selectAllCheckBox = null;
                }
               if(disposing && filterTextBox != null)
                {
                    filterTextBox.Dispose();
                    filterTextBox = null;
                }
                if (filterButton != null)
                {
                    filterButton.Dispose();
                    filterButton = null;
                }
                if (_fillRowsTimer != null)
                {
                    _fillRowsTimer?.Stop();
                    _fillRowsTimer?.Dispose();
                    _fillRowsTimer = null;
                }
                   
                if (_resizeTimer != null)
                {
                    _resizeTimer.Stop();
                    _resizeTimer.Dispose();
                    _resizeTimer = null;
                }
                if (_fullData != null)
                {
                    _fullData.Clear();
                    _fullData = null;
                }
                if (_bindingSource != null)
                {
                    _bindingSource.ListChanged -= BindingSource_ListChanged;
                    _bindingSource = null;
                }
            }
        }
    }
}