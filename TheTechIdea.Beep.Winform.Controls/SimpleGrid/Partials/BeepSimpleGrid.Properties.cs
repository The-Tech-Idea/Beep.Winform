using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.TextFields;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Partial class containing all properties and fields for BeepSimpleGrid
    /// </summary>
    public partial class BeepSimpleGrid
    {
        #region Private Fields

        // Fill and update control fields
        private bool _fillVisibleRowsPending;
        private bool _isUpdatingRows;
        private DateTime _lastFillTime = DateTime.MinValue;
        private const int FILL_DEBOUNCE_MS = 50;
        private Timer _fillRowsTimer;

        // Layout and UI state fields
        private int filtercontrolsheight = 3;
        private int filterPanelHeight = 60;
        private bool _layoutDirty;
        private string _titleimagestring = "simpleinfoapps.svg";
        private BeepCheckBoxBool _selectAllCheckBox;
        private bool _deferRedraw;
        private Dictionary<int, int> _rowHeights = new Dictionary<int, int>();
        private bool _navigatorDrawn;
        private bool _showFilterpanel;
        private Rectangle filterPanelRect;
        private bool _filterpaneldrawn;

        // Filter controls
        private BeepTextBox filterTextBox;
        private BeepComboBox filterColumnComboBox;

        // Panel dimensions
        protected int headerPanelHeight = 30;
        protected int bottomagregationPanelHeight = 20;
        protected int footerPanelHeight = 12;
        protected int navigatorPanelHeight = 30;

        // UI component dimensions
        private int _stickyWidth;
        private Rectangle footerPanelRect;
        private Rectangle headerPanelRect;
        private Rectangle columnsheaderPanelRect;
        private Rectangle bottomagregationPanelRect;
        private Rectangle navigatorPanelRect;
        private Rectangle filterButtonRect;

        // Labels and buttons
        private BeepLabel titleLabel;
        private BeepButton filterButton;
        private BeepLabel percentageLabel;

        // Display state
        private int visibleHeight;
        private int visibleRowCount;
        private int bottomPanelY;
        private int botomspacetaken;
        private int topPanelY;
        private Rectangle gridRect;
        private object rowsrect;

        // Row and column sizing
        private int _rowHeight = 25;
        private int _xOffset;
        private bool _showFilterButton = true;

        // Resize state
        private bool _resizingColumn;
        private bool _resizingRow;
        private int _resizingIndex = -1;
        private int _resizeMargin = 2;
        private Point _lastMousePos;

        // Cell and row state
        private int _editingRowIndex = -1;
        private BeepRowConfig _hoveredRow;
        private BeepRowConfig _currentRow;
        private BeepCellConfig _hoveredCell;
        private BeepCellConfig _selectedCell;
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

        // Column configuration
        private int _defaultcolumnheaderheight = 40;
        private int _defaultcolumnheaderwidth = 50;
        private TextImageRelation textImageRelation = TextImageRelation.ImageAboveText;

        // Control pooling and caching
        private Dictionary<BeepColumnType, List<IBeepUIComponent>> _controlPool = new Dictionary<BeepColumnType, List<IBeepUIComponent>>();
        private Dictionary<int, Rectangle> _cellBounds = new Dictionary<int, Rectangle>();
        private int _startviewrow;
        private int _endviewrow;

        // Fonts
        private Font _textFont;
        private Font _columnHeadertextFont = new Font("Arial", 8);

        // Selection tracking
        private Dictionary<int, bool> _persistentSelectedRows = new Dictionary<int, bool>();

        // Data source fields
        private object _dataSource;
        private object finalData;
        private bool _isInitializing = true;
        private object _pendingDataSource;
        private List<object> _fullData;
        private Type _entityType;
        private int _dataOffset;
        private string entityname;
        private EntityStructure _entity;

        // Sort and filter state
        private int _selectedSortIconIndex = -1;
        private int _selectedFilterIconIndex = -1;
        private List<Rectangle> columnHeaderBounds = new List<Rectangle>();
        private List<Rectangle> rowHeaderBounds = new List<Rectangle>();
        private List<Rectangle> sortIconBounds = new List<Rectangle>();
        private List<Rectangle> filterIconBounds = new List<Rectangle>();
        private Dictionary<string, Size> _columnTextSizes = new Dictionary<string, Size>();
        private bool _fitColumntoContent;

        // Columns and rows
        private List<BeepColumnConfig> _columns = new List<BeepColumnConfig>();
        private bool columnssetupusingeditordontchange;

        // Display flags
        private bool _showverticalgridlines = true;
        private bool _showhorizontalgridlines = true;
        private bool _showSortIcons = true;
        private bool _showRowNumbers = true;
        private bool _showColumnHeaders = true;
        private bool _showRowHeaders = true;
        private bool _showNavigator = true;
        private bool _showaggregationRow;
        private bool _showFooter;
        private bool _showHeaderPanel = true;
        private bool _showHeaderPanelBorder = true;

        // Scrollbar fields
        private int _scrollTargetVertical;
        private int _scrollTargetHorizontal;
        private BeepScrollBar _verticalScrollBar;
        private BeepScrollBar _horizontalScrollBar;
        private bool _showVerticalScrollBar = true;
        private Timer _scrollTimer;
        private int _scrollTarget;
        private int _scrollStep = 10;
        private bool _showHorizontalScrollBar = true;

        // Data navigator
        private BeepBindingNavigator _dataNavigator;

        // Selection and editing
        private bool _showCheckboxes;
        private List<int> _selectedRows = new List<int>();
        private List<BeepRowConfig> _selectedgridrows = new List<BeepRowConfig>();
        private int _selectionColumnWidth = 30;
        private bool _allowUserToAddRows;
        private bool _allowUserToDeleteRows;
        private DataGridViewSelectionMode _selectionMode = DataGridViewSelectionMode.FullRowSelect;
        private DataGridViewAutoSizeColumnsMode _autoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        private bool _readOnly;
        private string _percentageText = string.Empty;

        // Rendering resources
        private SolidBrush _cellBrush;
        private SolidBrush _selectedCellBrush;
        private Pen _borderPen;
        private Pen _selectedBorderPen;

        // Filter data
        private List<object> _filteredData;

        // Caching
        private int _cachedTotalColumnWidth;
        private int _cachedMaxXOffset;
        private bool _scrollBarsUpdatePending;

        // Resize timer
        private System.Windows.Forms.Timer _resizeTimer;
        private bool _isResizing;
        private Size _pendingSize;
        private Size _lastCalculatedSize;

        // Column drawers and editors
        private Dictionary<string, IBeepUIComponent> _columnDrawers = new Dictionary<string, IBeepUIComponent>();
        private Dictionary<string, IBeepUIComponent> _columnEditors = new Dictionary<string, IBeepUIComponent>();

        // Editor state
        private BeepControl _editingControl;
        private BeepCellConfig _editingCell;
        private bool _isEndingEdit;
        private BeepCellConfig _tempcell;

        // Change tracking
        private List<object> originalList = new List<object>();
        private List<object> deletedList = new List<object>();
        private Dictionary<object, Dictionary<string, object>> ChangedValues = new Dictionary<object, Dictionary<string, object>>();
        private bool _isAddingNewRecord;
        private BindingSource _bindingSource;

        // Navigation buttons
        private BeepButton Recordnumberinglabel1;
        private BeepButton FindButton, NewButton, EditButton, PreviousButton, NextButton, RemoveButton, RollbackButton, SaveButton, PrinterButton, MessageButton;
        private BeepButton FirstPageButton, PrevPageButton, NextPageButton, LastPageButton;
        private BeepButton PageLabel;
        private int spacing = 5;
        private int labelWidth = 100;
        private int pageLabelWidth = 60;
        private Size buttonSize = new Size(20, 20);
        private List<Control> buttons = new List<Control>();
        private List<Control> pagingButtons = new List<Control>();
        private int _currentPage = 1;
        private int _totalPages;
        private Dictionary<string, BeepButton> _navigationButtonCache = new Dictionary<string, BeepButton>();
        private bool tooltipShown;

        // DPI scaling
        private float _cachedDpiScale = 1.0f;
        private int _scaledRowHeight;
        private int _scaledColumnHeaderHeight;
        private int _scaledDefaultColumnWidth;
        private Dictionary<int, int> _scaledColumnWidths = new Dictionary<int, int>();
        private bool _dpiCacheValid;

        // Additional fields
        private string _titletext = "";

        #endregion

        #region Public Properties

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
                }
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
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

        public string QueryFunctionName { get; set; }
        public string QueryFunction { get; set; }

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
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EntityStructure Entity
        {
            get => _entity;
            set
            {
                _entity = value;
                if (_entity != null)
                {
                    if (!string.IsNullOrEmpty(_entity.EntityName))
                    {
                        try
                        {
                            entityname = _entity.EntityName;
                            if (_entityType == null) _entityType = Type.GetType(_entity.EntityName);
                        }
                        catch (Exception)
                        {
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
                if (_isInitializing)
                {
                    _pendingDataSource = value;
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

        public bool ShowSortIcon { get; set; }
        public bool ShowFilterIcon { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.None;
        public bool IsFiltered { get; set; }

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
                    foreach (var column in Columns)
                    {
                        column.Width = GetColumnWidth(column);
                    }
                }
                Invalidate();
            }
        }

        public BindingList<BeepRowConfig> Rows { get; set; } = new BindingList<BeepRowConfig>();
        public BeepRowConfig aggregationRow { get; set; }

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
        [Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(UITypeEditor))]
        public List<BeepColumnConfig> Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                if (_columns != null)
                {
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
            set
            {
                _defaultcolumnheaderwidth = value;
                Invalidate();
            }
        }

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
        public bool ShowFilter
        {
            get => _showFilterpanel;
            set
            {
                _showFilterpanel = value;
                _filterpaneldrawn = false;
                _layoutDirty = true;
                filterTextBox.Visible = value;
                filterColumnComboBox.Visible = value;
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

        public bool IsEditorShown { get; private set; }

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

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<int> SelectedRows
        {
            get => _selectedRows;
            set
            {
                _selectedRows = value ?? new List<int>();
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<BeepRowConfig> SelectedGridRows
        {
            get => _selectedgridrows;
            set
            {
                _selectedgridrows = value ?? new List<BeepRowConfig>();
                Invalidate();
            }
        }

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
        public bool DrawInBlackAndWhite { get; set; }

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

        public List<Tracking> Trackings { get; set; } = new List<Tracking>();
        public bool IsLogging { get; set; }
        public Dictionary<DateTime, EntityUpdateInsertLog> UpdateLog { get; set; } = new Dictionary<DateTime, EntityUpdateInsertLog>();
        public bool VerifyDelete { get; set; }

        #endregion
    }
}
