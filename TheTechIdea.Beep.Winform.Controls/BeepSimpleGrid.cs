﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;
using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Winform.Controls.Helpers;



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
        private Dictionary<int, int> _rowHeights = new Dictionary<int, int>(); // Key: DisplayIndex, Value: Height
        private bool _navigatorDrawn = false;
        private bool _showFilterpanel=false;
        private Rectangle filterPanelRect;
        private bool _filterpaneldrawn=false;

        private BeepTextBox filterTextBox;
        private BeepComboBox filterColumnComboBox;
        protected int headerPanelHeight = 20;
        protected int bottomagregationPanelHeight = 20;
        protected int footerPanelHeight = 12;
        protected int navigatorPanelHeight = 20;
        private int _stickyWidth = 0; // Cache sticky column width
        private Rectangle footerPanelRect;
        private Rectangle headerPanelRect;
        private Rectangle columnsheaderPanelRect;
        private Rectangle bottomagregationPanelRect;
        private Rectangle navigatorPanelRect;
        private Rectangle filterButtonRect;
        private BeepLabel titleLabel;
        private BeepButton filterButton;
        private int visibleHeight;
        private int visibleRowCount;
        private int bottomPanelY;
        private int botomspacetaken = 0;
        private int topPanelY;
        private Rectangle gridRect;
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
        private int _selectedSortIconIndex = -1;
        private int _selectedFilterIconIndex = -1;
        private List<Rectangle> columnHeaderBounds = new List<Rectangle>();
        private List<Rectangle> rowHeaderBounds = new List<Rectangle>();
        private List<Rectangle> sortIconBounds = new List<Rectangle>();
        private List<Rectangle> filterIconBounds = new List<Rectangle>();
        private int _defaultcolumnheaderheight = 40;
        private int _defaultcolumnheaderwidth = 50;
        private TextImageRelation textImageRelation = TextImageRelation.ImageAboveText;

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

        private string _titletext = "Simple BeepGrid";
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
                MiscFunctions.SendLog("1 Entity Setter: " + value?.EntityName);
                _entity = value;
                if (_entity != null)
                {
                   
                    MiscFunctions.SendLog("2 Entity Setter: " + _entity.EntityName);
                    if (!string.IsNullOrEmpty(_entity.EntityName))
                    {
                        try
                        {
                            entityname = _entity.EntityName;
                            MiscFunctions.SendLog("3 Entity Setter: " + _entity.EntityName);
                            _entityType = Type.GetType(_entity.EntityName);
                        }
                        catch (Exception)
                        {
                            MiscFunctions.SendLog("4 Entity Setter: " + _entity.EntityName);

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
                MiscFunctions.SendLog($"DataSource Setter: Type = {value?.GetType()?.Name}, DesignMode = {DesignMode}, IsInitializing = {_isInitializing}");
                if (_isInitializing)
                {
                    _pendingDataSource = value; // Defer setting until initialization is complete
                    MiscFunctions.SendLog("Deferring DataSource setup until initialization completes");
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public List<BeepColumnConfig> Columns
        {
            get
            {
                //   MiscFunctions.SendLog($"Columns Getter: Count = {_columns.Count}, DesignMode = {DesignMode}");
                return _columns;
            }
            set
            {
                  MiscFunctions.SendLog($"Columns Setter: Count = {value?.Count}, DesignMode = {DesignMode}");

                    MiscFunctions.SendLog("Columns set via property, marking as designer-configured");
                    _columns = value;
                if (_columns != null)
                {
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
        private string _title = "BeepSimpleGrid Title";
        public string Title
        {
            get => _title;
            set => _title = value;
        }

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
                Invalidate();
            }
        }

        private bool _showSelection = true;
        [Browsable(true)]
        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowSelection
        {
            get => _showSelection;
            set
            {
                _showSelection = value;
                Invalidate();
            }
        }

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
        #endregion "Properties"
        #region Constructor
        public BeepSimpleGrid():base()
        {
            // This ensures child controls are painted properly
            this.SetStyle(ControlStyles.ContainerControl, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            // Ensure _columns is only initialized once
            SetStyle(ControlStyles.Selectable | ControlStyles.UserMouse , true);
            TabStop = true;
            this.Focus();
            _isInitializing = true;
            this.PreviewKeyDown += BeepGrid_PreviewKeyDown;

            this.KeyDown += BeepSimpleGrid_KeyDown; 
         

            Width = 200;
            Height = 200;
            // Attach event handlers for mouse actions
            this.MouseClick += BeepGrid_MouseClick;
            
            this.MouseDown += BeepGrid_MouseDown;
            this.MouseMove += BeepGrid_MouseMove;
            this.MouseUp += BeepGrid_MouseUp;
            TabKeyPressed += Tabhandler;
            
            Rows.ListChanged += Rows_ListChanged;
            ApplyThemeToChilds = false;
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
            //    Theme = Theme,

            //};
            //AttachNavigatorEvents();
            _scrollTimer = new Timer { Interval = 16 }; // ~60 FPS for smooth animation
            _scrollTimer.Tick += ScrollTimer_Tick;
            ApplyThemeToChilds = false;
            //Controls.Add(DataNavigator);

            titleLabel = new BeepLabel
            {
                Text = string.IsNullOrEmpty(Title) ? "Beep Grid" : Title,
                TextAlign = ContentAlignment.MiddleCenter,
                Theme = Theme,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                MaxImageSize = new Size(20, 20),
                ShowAllBorders = false,
                ShowShadow = false,
                IsChild = true,
                IsShadowAffectedByTheme = false,
                IsBorderAffectedByTheme = false
            };
            titleLabel.BackColor = _currentTheme?.BackColor ?? Color.White;

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
            // Initialize filter controls
            filterTextBox = new BeepTextBox
            {
                Theme = Theme,
                IsChild = true,
                Width = 150,
                Height = 20,
                PlaceholderText = "Filter grid..."
            };
            filterTextBox.TextChanged += FilterTextBox_TextChanged;
            Controls.Add(filterTextBox);

            filterColumnComboBox = new BeepComboBox
            {
                Theme = Theme,
                IsChild = true,
                Width = 120,
                Height = 20,
               
            };
            filterColumnComboBox.SelectedItemChanged += FilterColumnComboBox_SelectedIndexChanged;
            Controls.Add(filterColumnComboBox);
            CreateNavigationButtons();
            
            InitializeRows();
        }

        #endregion
        #region Data Navigator
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
            //    MiscFunctions.SendLog("CallPrinter triggered - implement printing logic here.");
                // Example: Print the current _fullData
                // You could raise an event or call a printing method
                MessageBox.Show("Printing functionality not implemented yet.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
             //   MiscFunctions.SendLog($"CallPrinter Error: {ex.Message}");
                MiscFunctions.SendLog($"Error in printing: {ex.Message}");
            }
        }
        private void DataNavigator_SendMessage(object sender, BindingSource bs)
        {
            try
            {
                // Placeholder for sending/sharing functionality
              //  MiscFunctions.SendLog("SendMessage triggered - implement sharing logic here.");
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
             //   MiscFunctions.SendLog($"SendMessage Error: {ex.Message}");
                MiscFunctions.SendLog($"Error in sending message: {ex.Message}");
            }
        }
        private void DataNavigator_ShowSearch(object sender, BindingSource bs)
        {
            try
            {
                // Placeholder for search/filter functionality
             //   MiscFunctions.SendLog("ShowSearch triggered - implement filter UI here.");
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
            //    MiscFunctions.SendLog($"ShowSearch Error: {ex.Message}");
                MiscFunctions.SendLog($"Error in search: {ex.Message}");
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
                            MiscFunctions.SendLog($"DataNavigator_DeleteCalled: Item at index {dataIndex} is not a DataRowWrapper");
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
                MiscFunctions.SendLog($"DeleteCalled Error: {ex.Message}");
                MiscFunctions.SendLog($"Error deleting record: {ex.Message}");
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
                    MiscFunctions.SendLog("DataNavigator_NewRecordCreated: Unsupported data source type");
                }
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"NewRecordCreated Error: {ex.Message}");
                MiscFunctions.SendLog($"Error adding new record: {ex.Message}");
            }
        }
        private void DataNavigator_EditCalled(object sender, BindingSource bs)
        {
            try
            {
                if (_selectedCell != null)
                {
                    BeginEdit();
                 //   MiscFunctions.SendLog($"EditCalled: Editing cell at row {_selectedCell.RowIndex}, column {_selectedCell.ColumnIndex}");
                }
                else
                {
                //    MiscFunctions.SendLog("EditCalled: No cell selected to edit.");
                    MessageBox.Show("Please select a cell to edit.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"EditCalled Error: {ex.Message}");
                MiscFunctions.SendLog($"Error starting edit: {ex.Message}");
            }
        }
        private void DataNavigator_SaveCalled(object sender, BindingSource bs)
        {
            try
            {
                // Save changes to _fullData (external persistence delegated to caller)
                CloseCurrentEditorIn(); // Ensure any open editor is saved
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
                //  MiscFunctions.SendLog("Data saved locally in grid.");
                MessageBox.Show("Changes saved locally. Implement external persistence if needed.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"SaveCalled Error: {ex.Message}");
                MiscFunctions.SendLog($"Error saving data: {ex.Message}");
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
        #endregion Data Navigator
        #region DataSource Update
        private void CreateNewRecordForBindingSource()
        {
            try
            {
                if (Entity == null)
                {
                    MiscFunctions.SendLog("CreateNewRecordForBindingSource: Cannot add new record: Entity structure not defined.");
                    return;
                }

                // Create a new record of the appropriate type
                var newItem = Activator.CreateInstance(Type.GetType(Entity.EntityName));

                // Add to the BindingSource
                if (_bindingSource != null)
                {
                    _bindingSource.Add(newItem); // Add to BindingSource, which triggers BindingSource_ListChanged
                    MiscFunctions.SendLog($"CreateNewRecordForBindingSource: Added new record to BindingSource: {newItem}");

                    // The BindingSource_ListChanged event (ListChangedType.ItemAdded) will handle adding to _fullData, Trackings, and originalList
                    // We just need to update the grid's selection and scroll position
                    int newOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
                    StartSmoothScroll(newOffset);
                    int newRowIndex = _fullData.Count - 1 - _dataOffset;
                    if (newRowIndex >= 0 && newRowIndex < Rows.Count)
                    {
                        SelectCell(newRowIndex, 0);
                        BeginEdit();
                    }

                    // Log the operation if IsLogging is enabled
                    if (IsLogging)
                    {
                        var tracking = Trackings.LastOrDefault(); // Get the tracking entry added by BindingSource_ListChanged
                        if (tracking != null)
                        {
                            UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                            {
                                TrackingRecord = tracking,
                                LogAction = LogAction.Insert,
                                UpdatedFields = new Dictionary<string, object>()
                            };
                        }
                    }
                }
                else
                {
                    MiscFunctions.SendLog($"CreateNewRecordForBindingSource: _bindingSource is null, cannot add new record");
                }
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"CreateNewRecordForBindingSource Error: {ex.Message}");
                MiscFunctions.SendLog($"Error adding new record: {ex.Message}");
            }
        }

        // Existing CreateNewRecordForIList (already includes logging and error handling)
        private void CreateNewRecordForIList()
        {
            try
            {
                if (Entity == null)
                {
                    MiscFunctions.SendLog("CreateNewRecordForIList: Cannot add new record: Entity structure not defined.");
                    return;
                }

                // Create a new record of the appropriate type
                var newItem = Activator.CreateInstance(_entityType);

                // Add to the IList data source
                if (_dataSource is IList list)
                {
                    list.Add(newItem);
                    MiscFunctions.SendLog($"CreateNewRecordForIList: Added new record to IList: {newItem}");
                }
                else
                {
                    MiscFunctions.SendLog($"CreateNewRecordForIList: _dataSource is not an IList");
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

                MiscFunctions.SendLog($"CreateNewRecordForIList: Added new record to _fullData at index {_fullData.Count - 1}");
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"CreateNewRecordForIList Error: {ex.Message}");
                MiscFunctions.SendLog($"Error adding new record: {ex.Message}");
            }
        }
        private void InsertIntoDataSource(object newData)
        {
            if (_dataSource is BindingSource)
            {
                if (_bindingSource != null)
                {
                    _bindingSource.Add(newData); // Add to BindingSource, which notifies listeners
                    MiscFunctions.SendLog($"InsertIntoDataSource: Added new record to BindingSource: {newData}");
                }
                else
                {
                    MiscFunctions.SendLog($"InsertIntoDataSource: _bindingSource is null, cannot add new record");
                }
            }
            else if (_dataSource is IList list)
            {
                list.Add(newData); // Add to IList
                MiscFunctions.SendLog($"InsertIntoDataSource: Added new record to IList: {newData}");
            }
            else
            {
                MiscFunctions.SendLog($"InsertIntoDataSource: Unsupported data source type: {_dataSource?.GetType()}");
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
                        MiscFunctions.SendLog($"UpdateInDataSource: Updated record in BindingSource at index {dataIndex}: Original={originalData}, Changes={string.Join(", ", changes.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
                    }
                    else
                    {
                        // If the item is no longer in the BindingSource's List (e.g., filtered out), refresh _fullData
                        DataSetup();
                        InitializeData();
                        MiscFunctions.SendLog($"UpdateInDataSource: Item not found in BindingSource, refreshed grid");
                    }
                }
                else
                {
                    MiscFunctions.SendLog($"UpdateInDataSource: _bindingSource is null, cannot update record");
                }
            }
            else if (_dataSource is IList list)
            {
                int dataIndex = list.IndexOf(originalData);
                if (dataIndex >= 0 && dataIndex < list.Count)
                {
                    list[dataIndex] = originalData; // Update the item in the IList
                    MiscFunctions.SendLog($"UpdateInDataSource: Updated record in IList at index {dataIndex}: Original={originalData}, Changes={string.Join(", ", changes.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
                }
                else
                {
                    MiscFunctions.SendLog($"UpdateInDataSource: Item not found in IList at index {dataIndex}");
                }
            }
            else
            {
                MiscFunctions.SendLog($"UpdateInDataSource: Unsupported data source type: {_dataSource?.GetType()}");
            }
        }

        private void DeleteFromDataSource(object originalData)
        {
            try
            {
                if (originalData == null)
                {
                    MiscFunctions.SendLog("DeleteFromDataSource: Original data is null, cannot delete record");
                    return;
                }

                // Find the DataRowWrapper in _fullData to get the dataIndex
                DataRowWrapper wrapper = null;
                int dataIndex = -1;
                for (int i = 0; i < _fullData.Count; i++)
                {
                    var item = _fullData[i] as DataRowWrapper;
                    if (item != null && item.OriginalData == originalData)
                    {
                        wrapper = item;
                        dataIndex = i;
                        break;
                    }
                }

                if (wrapper == null || dataIndex < 0)
                {
                    MiscFunctions.SendLog($"DeleteFromDataSource: Could not find DataRowWrapper for originalData in _fullData");
                    return;
                }

                // Get the tracking entry before removing
                Tracking tracking = GetTrackingItem(wrapper);
                if (tracking != null)
                {
                    tracking.EntityState = EntityState.Deleted;
                }

                if (_dataSource is BindingSource)
                {
                    if (_bindingSource != null)
                    {
                        int bsIndex = _bindingSource.IndexOf(originalData);
                        if (bsIndex >= 0 && bsIndex < _bindingSource.Count)
                        {
                            // Remove from _fullData, Trackings, and originalList before removing from BindingSource
                            _fullData.RemoveAt(dataIndex);
                            originalList.RemoveAt(dataIndex);
                            Trackings.Remove(tracking);
                            deletedList.Add(wrapper);

                            // Update indices in _fullData
                            for (int i = 0; i < _fullData.Count; i++)
                            {
                                var dataRow = _fullData[i] as DataRowWrapper;
                                if (dataRow != null)
                                {
                                    dataRow.RowID = i;
                                }
                            }

                            // Log the operation if IsLogging is enabled
                            if (IsLogging && tracking != null)
                            {
                                UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                                {
                                    TrackingRecord = tracking,
                                    LogAction = LogAction.Delete,
                                    UpdatedFields = new Dictionary<string, object>()
                                };
                            }

                            // Remove from BindingSource
                            _bindingSource.RemoveAt(bsIndex); // This will trigger BindingSource_ListChanged, but we've already handled the updates
                            MiscFunctions.SendLog($"DeleteFromDataSource: Deleted record from BindingSource at index {bsIndex}: {originalData}");
                        }
                        else
                        {
                            MiscFunctions.SendLog($"DeleteFromDataSource: Item not found in BindingSource at index {bsIndex}");
                        }
                    }
                    else
                    {
                        MiscFunctions.SendLog($"DeleteFromDataSource: _bindingSource is null, cannot delete record");
                    }
                }
                else if (_dataSource is IList list)
                {
                    int listIndex = list.IndexOf(originalData);
                    if (listIndex >= 0 && listIndex < list.Count)
                    {
                        // Remove from IList
                        list.RemoveAt(listIndex);
                        MiscFunctions.SendLog($"DeleteFromDataSource: Deleted record from IList at index {listIndex}: {originalData}");

                        // Remove from _fullData
                        _fullData.RemoveAt(dataIndex);

                        // Update indices in _fullData
                        for (int i = 0; i < _fullData.Count; i++)
                        {
                            var dataRow = _fullData[i] as DataRowWrapper;
                            if (dataRow != null)
                            {
                                dataRow.RowID = i;
                            }
                        }

                        // Update Trackings and originalList
                        originalList.RemoveAt(dataIndex);
                        Trackings.Remove(tracking);
                        deletedList.Add(wrapper);

                        // Log the operation if IsLogging is enabled
                        if (IsLogging && tracking != null)
                        {
                            UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                            {
                                TrackingRecord = tracking,
                                LogAction = LogAction.Delete,
                                UpdatedFields = new Dictionary<string, object>()
                            };
                        }

                        MiscFunctions.SendLog($"DeleteFromDataSource: Updated _fullData, Trackings, and originalList after deleting record at index {dataIndex}");
                    }
                    else
                    {
                        MiscFunctions.SendLog($"DeleteFromDataSource: Item not found in IList at index {listIndex}");
                    }
                }
                else
                {
                    MiscFunctions.SendLog($"DeleteFromDataSource: Unsupported data source type: {_dataSource?.GetType()}");
                }
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"DeleteFromDataSource Error: {ex.Message}");
                MiscFunctions.SendLog($"Error deleting record: {ex.Message}");
            }
        }
        // Function to update a BindingSource data source
        private void UpdateBindingSourceDataSource(int dataIndex, object updatedItem)
        {
            if (_bindingSource != null && dataIndex >= 0 && dataIndex < _bindingSource.Count)
            {
                _bindingSource[dataIndex] = updatedItem; // Update the item in the BindingSource
                _bindingSource.ResetItem(dataIndex); // Notify listeners of the change
                MiscFunctions.SendLog($"UpdateBindingSourceDataSource: Updated item at index {dataIndex} in BindingSource and notified listeners");
            }
            else
            {
                // If the item is no longer in the BindingSource's List (e.g., filtered out), refresh _fullData
                DataSetup();
                InitializeData();
                MiscFunctions.SendLog($"UpdateBindingSourceDataSource: Item at index {dataIndex} not found in BindingSource, refreshed grid");
            }
        }
        // Function to update an IList data source
        private void UpdateIListDataSource(int dataIndex, object updatedItem)
        {
            if (_dataSource is IList list && dataIndex >= 0 && dataIndex < list.Count)
            {
                list[dataIndex] = updatedItem; // Update the item in the IList
                MiscFunctions.SendLog($"UpdateIListDataSource: Updated item at index {dataIndex} in IList");
            }
            else
            {
                MiscFunctions.SendLog($"UpdateIListDataSource: Invalid index {dataIndex} or _dataSource is not an IList");
            }
        }
        private void UpdateDataRecordFromRow(BeepCellConfig editingCell)
        {
            if (Rows == null || editingCell == null || editingCell.RowIndex < 0 || editingCell.RowIndex >= Rows.Count || _fullData == null || !_fullData.Any())
                return;

            BeepRowConfig row = Rows[editingCell.RowIndex];

            // Find the "RowID" cell in the row to get the stable identifier
            BeepColumnConfig rowidcol = Columns.Find(p => p.IsRowID);
            var rowIDCell = row.Cells.FirstOrDefault(c => Columns[c.ColumnIndex].IsRowID);
            if (rowIDCell == null || rowIDCell.CellValue == null || !int.TryParse(rowIDCell.CellValue.ToString(), out int rowID))
            {
                MiscFunctions.SendLog($"UpdateDataRecordFromRow: Failed to retrieve RowID for row at index {editingCell.RowIndex}");
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
                MiscFunctions.SendLog($"UpdateDataRecordFromRow: Could not find record with RowID {rowID} in _fullData");
                return;
            }

            object originalData = dataItem.OriginalData;
            var originalWrappedItem = GetItemFromOriginalList(GetOriginalIndex(dataItem)) as DataRowWrapper;
            object originalItem = originalWrappedItem?.OriginalData;

            bool hasChanges = false;
            Dictionary<string, object> changes = new Dictionary<string, object>();
            foreach (var cell in row.Cells)
            {
                if (cell.IsDirty)
                {
                    var col = Columns[cell.ColumnIndex];
                    // Skip special columns like "Sel", "RowNum", and "RowID"
                    if (col.IsSelectionCheckBox || col.IsRowNumColumn || col.IsRowID)
                        continue;

                    var prop = originalData.GetType().GetProperty(col.ColumnName ?? col.ColumnCaption);
                    if (prop != null)
                    {
                        object convertedValue = MiscFunctions.ConvertValueToPropertyType(prop.PropertyType, cell.CellValue);
                        object originalValue = prop.GetValue(originalData);
                        if (!Equals(convertedValue, originalValue)) // Check for actual change
                        {
                            prop.SetValue(originalData, convertedValue);
                            changes[col.ColumnName ?? col.ColumnCaption] = convertedValue;
                            hasChanges = true;
                        }

                        // Update RowState and DateTimeChange in DataRowWrapper
                        if (hasChanges && dataItem.RowState != DataRowState.Added && dataItem.RowState != DataRowState.Deleted)
                        {
                            dataItem.RowState = DataRowState.Modified;
                            dataItem.DateTimeChange = DateTime.Now;
                        }

                        if (IsLogging)
                        {
                            Tracking tracking = GetTrackingItem(dataItem);
                            if (tracking != null)
                            {
                                tracking.EntityState = MapRowStateToEntityState(dataItem.RowState); // Sync EntityState with RowState
                                if (hasChanges && tracking.EntityState != EntityState.Added && tracking.EntityState != EntityState.Deleted)
                                {
                                    if (!ChangedValues.ContainsKey(dataItem))
                                    {
                                        ChangedValues[dataItem] = GetChangedFields(originalWrappedItem, dataItem);
                                    }
                                    else
                                    {
                                        var updatedChanges = GetChangedFields(originalWrappedItem, dataItem);
                                        foreach (var kvp in updatedChanges)
                                        {
                                            ChangedValues[dataItem][kvp.Key] = kvp.Value;
                                        }
                                    }
                                    UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                                    {
                                        TrackingRecord = tracking,
                                        UpdatedFields = ChangedValues[dataItem]
                                    };
                                }
                            }
                        }
                    }
                    row.IsDirty = true;
                }
            }

            // Update the underlying data source if there are changes
            if (hasChanges)
            {
                if (_dataSource is BindingSource)
                {
                    UpdateBindingSourceDataSource(dataIndex, originalData);
                }
                else if (_dataSource is IList)
                {
                    UpdateIListDataSource(dataIndex, originalData);
                }

                // Notify the data navigator of the change
                if (DataNavigator != null)
                {
                    if (_dataSource is BindingSource && DataNavigator.BindingSource == _bindingSource)
                    {
                        // Already notified via UpdateBindingSourceDataSource
                    }
                    else
                    {
                        DataNavigator.BindingSource?.ResetBindings(false); // Notify navigator of item changes
                    }
                }

                // Redraw the grid to reflect any visual changes
                Invalidate();
            }
        }
        private void SaveToDataSource()
        {
            try
            {
                if (_fullData == null || originalList == null || DataSource == null)
                {
                    MiscFunctions.SendLog("SaveToDataSource: _fullData, originalList, or DataSource is null. Cannot sync changes.");
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
                                     //   MiscFunctions.SendLog($"Saving Added Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                                      //  MiscFunctions.SendLog($"Saving Modified Record - RowID: {wrappedItem.RowID}, OriginalIndex: {originalIndex}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                             //   MiscFunctions.SendLog($"Saving Deleted Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                                 //       MiscFunctions.SendLog($"Saving Added Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                                 //      MiscFunctions.SendLog($"Saving Modified Record - RowID: {wrappedItem.RowID}, OriginalIndex: {originalIndex}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                          //      MiscFunctions.SendLog($"Saving Deleted Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                    MiscFunctions.SendLog("SaveToDataSource: DataSource is neither BindingSource nor IList. Cannot sync changes.");
                    return;
                }

                // Update the grid state
                UpdateScrollBars();
                Invalidate();

                MiscFunctions.SendLog("SaveToDataSource: All changes processed and synchronized with DataSource.");
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"SaveToDataSource Error: {ex.Message}");
                MiscFunctions.SendLog($"Error saving to data source: {ex.Message}");
            }
        }
        #endregion DataSource Update
        #region Initialization
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (_isInitializing)
            {
                MiscFunctions.SendLog("OnHandleCreated: Completing deferred initialization");
                if (_pendingDataSource != null)
                {
                    _dataSource = _pendingDataSource;
                    DataSetup();
                    InitializeData();
                    FillVisibleRows();
                    UpdateScrollBars();
                    Invalidate();
                }
                _isInitializing = false; // Initialization complete
            }
        }
        private Tuple<object,EntityStructure> SetupBindingSource()
        {
            object resolvedData = null; // Unified variable for final data or type resolution
                                        //  MiscFunctions.SendLog($"BindingSource Detected: DataSource = {bindingSrc.DataSource?.GetType()}, DataMember = {bindingSrc.DataMember}");
            EntityStructure entity = null;
            if (DataSource == null)
            {
                //   MiscFunctions.SendLog("BindingSource.DataSource is null, checking BindingSource.List");
                resolvedData = _bindingSource.List; // Could be IList, DataView, etc.
            }
            else if (_bindingSource.DataSource is Type type)
            {
                // Handle Type (design-time or runtime)
                 MiscFunctions.SendLog($"{(DesignMode ? "Design-time" : "Runtime")}: DataSource is Type = {type.FullName}");
                if (!string.IsNullOrEmpty(_bindingSource.DataMember))
                {
                    PropertyInfo dataMemberProp = type.GetProperty(_bindingSource.DataMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (dataMemberProp != null)
                    {
                        Type itemType = GetItemTypeFromDataMember(dataMemberProp.PropertyType);
                        if (itemType != null)
                        {
                                  MiscFunctions.SendLog($"Resolved ItemType from DataMember '{_bindingSource.DataMember}' = {itemType.FullName}");
                            entity = EntityHelper.GetEntityStructureFromType(itemType);
                        }
                        else
                        {
                              MiscFunctions.SendLog($"Could not extract item type from DataMember '{_bindingSource.DataMember}' property type: {dataMemberProp.PropertyType}");
                        }
                    }
                    else
                    {
                          MiscFunctions.SendLog($"DataMember '{_bindingSource.DataMember}' not found on Type {type.FullName}");
                    }
                }
                else
                {
                     MiscFunctions.SendLog("No DataMember specified with Type DataSource, using Type directly");
                    entity = EntityHelper.GetEntityStructureFromType(type);
                }
            }
            else
            {
                // Handle instance (could be DataTable, IList, or class with DataMember)
                if (!string.IsNullOrEmpty(_bindingSource.DataMember))
                {
                    PropertyInfo prop = _bindingSource.DataSource.GetType().GetProperty(_bindingSource.DataMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (prop != null)
                    {
                        resolvedData = prop.GetValue(_bindingSource.DataSource);
                          MiscFunctions.SendLog($"Resolved data from DataMember = {resolvedData?.GetType()}");
                    }
                    else
                    {
                          MiscFunctions.SendLog($"DataMember '{_bindingSource.DataMember}' not found on instance, using BindingSource.List or DataSource");
                        resolvedData = _bindingSource.List != null && _bindingSource.List.Count > 0 ? _bindingSource.List : _bindingSource.DataSource;
                    }
                }
                else
                {
                      MiscFunctions.SendLog("No DataMember, attempting auto-detection or using BindingSource.List");
                    resolvedData = GetCollectionPropertyFromInstance(_bindingSource.DataSource) ?? (_bindingSource.List != null && _bindingSource.List.Count > 0 ? _bindingSource.List : _bindingSource.DataSource);
                    MiscFunctions.SendLog($"Resolved data = {resolvedData?.GetType()}");
                }
            }
            return Tuple.Create(resolvedData, entity);
        }
        private void DataSetup()
        {
            EntityStructure entity = null;
            object resolvedData = null; // Unified variable for final data or type resolution
           MiscFunctions.SendLog($"DataSetup Started: _dataSource Type = {_dataSource?.GetType()}, DesignMode = {DesignMode},");

            // Step 1: Handle different _dataSource types
            if (_dataSource == null)
            {
                MiscFunctions.SendLog("DataSource is null, no entity generated");
            }
            else if (_dataSource is BindingSource bindingSrc)
            {
             //   MiscFunctions.SendLog($"BindingSource Detected: DataSource = {bindingSrc.DataSource?.GetType()}, DataMember = {bindingSrc.DataMember}");
                var dataSource = bindingSrc.DataSource;
                AssignBindingSource(bindingSrc);

                var ret = SetupBindingSource();
                resolvedData = ret.Item1;
                entity = ret.Item2;
            }
            else if (_dataSource is DataTable dataTable)
            {
               MiscFunctions.SendLog("DataSource is DataTable");
                resolvedData = dataTable;
            }
            else if (_dataSource is IList iList)
            {
                MiscFunctions.SendLog("DataSource is IList");
                resolvedData = iList;
            }
            else
            {
                MiscFunctions.SendLog($"DataSource is unrecognized type: {_dataSource.GetType()}, attempting auto-detection");
                resolvedData = GetCollectionPropertyFromInstance(_dataSource) ?? _dataSource;
            }
            if (resolvedData == null)
            {
                MiscFunctions.SendLog("Resolved data is null, no entity generated");
            }
            else if (resolvedData is DataTable dt)
            {
                finalData = dt;

                entity = EntityHelper.GetEntityStructureFromListorTable(finalData);
            }
            else if (resolvedData is IList list && list.Count > 0)
            {
                finalData = list;
                Type itemType = list[0]?.GetType();

                    if (itemType != null)
                    {
                         MiscFunctions.SendLog($"Extracted item type from IList: {itemType.FullName}");
                        entity = EntityHelper.GetEntityStructureFromType(itemType);
                    }
                    else
                    {
                        entity = EntityHelper.GetEntityStructureFromListorTable(finalData);
                    }

            }
            else
            {
                MiscFunctions.SendLog($"Resolved data is not a recognized collection type: {resolvedData.GetType()}, using as-is");
                finalData = resolvedData;
                entity = EntityHelper.GetEntityStructureFromListorTable(finalData);
            }

            if (_columns == null) _columns = new List<BeepColumnConfig>();
            //Step 3: Process entity and columns
            if (entity != null)
            {
              MiscFunctions.SendLog($"New Entity: {entity.EntityName}, Existing Entity: {EntityName}");
                if (_columns.Any() )
                {
                    if (!string.IsNullOrEmpty(EntityName) && entity.EntityName.Equals(EntityName))
                    {
                        MiscFunctions.SendLog("Preserving designer Columns, syncing fields");
                      //  SyncFields(entity);
                        SyncColumnsWithEntity(entity);
                    }
                    else
                    {
                        MiscFunctions.SendLog("New Entity with designer Columns, updating Entity only");
                        Entity = entity;
                       CreateColumnsForEntity();
                    }
                }
                else
                {
                   MiscFunctions.SendLog("No designer Columns or not protected, regenerating");
                    Entity = entity;
                    CreateColumnsForEntity();
                }
            }
            else 
            {
                _columns=new List<BeepColumnConfig>();
                Entity = null;
                MiscFunctions.SendLog("No new Entity, keeping designer Columns");
            }

            MiscFunctions.SendLog($"DataSetup Completed: finalData = {finalData?.GetType()}, Entity = {Entity?.EntityName}, Columns Count = {_columns.Count}");
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
                     //   MiscFunctions.SendLog($"Found collection property: {prop.Name}, Type = {prop.PropertyType}");
                    }
                }
            }

            if (collectionCount == 1 && collectionProp != null)
            {
                return collectionProp.GetValue(instance);
            }
            else if (collectionCount > 1)
            {
             //   MiscFunctions.SendLog("Multiple collection properties found, please specify DataMember");
            }
            return null; // No single collection found or all are null
        }
        private Type GetItemTypeFromDataMember(Type propertyType)
        {
           // MiscFunctions.SendLog($"GetItemTypeFromDataMember: PropertyType = {propertyType.FullName}");

            // Handle generic IEnumerable<T> (e.g., List<T>)
            if (propertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                Type[] genericArgs = propertyType.GetGenericArguments();
                if (genericArgs.Length > 0)
                {
                 //   MiscFunctions.SendLog($"Extracted item type: {genericArgs[0].FullName}");
                    return genericArgs[0];
                }
            }

            // Handle DataTable
            if (propertyType == typeof(DataTable))
            {
              //  MiscFunctions.SendLog("DataTable detected, returning DataRow");
                return typeof(DataRow);
            }

            // Handle arrays
            if (propertyType.IsArray)
            {
             //   MiscFunctions.SendLog($"Array detected, element type: {propertyType.GetElementType().FullName}");
                return propertyType.GetElementType();
            }

          //  MiscFunctions.SendLog("No item type extracted");
            return null;
        }
        private void SyncColumnsWithEntity(IEntityStructure entity)
        {
            // MiscFunctions.SendLog("Syncing Columns with Entity");

            // Preserve special columns ("Sel" and "RowNum") by removing them temporarily
            var selColumn = _columns.FirstOrDefault(c => c.IsSelectionCheckBox);
            var rowNumColumn = _columns.FirstOrDefault(c => c.IsRowNumColumn);
            var rowIDColumn = _columns.FirstOrDefault(c => c.IsRowID);
            _columns.RemoveAll(c => c.IsSelectionCheckBox || c.IsRowNumColumn || c.IsRowID);

            // Map existing columns by name for quick lookup
            var columnMap = _columns.ToDictionary(c => c.ColumnName, c => c, StringComparer.OrdinalIgnoreCase);

            // Sync fields into columns, preserving designer settings
            foreach (var field in entity.Fields)
            {
                if (columnMap.TryGetValue(field.fieldname, out var existingColumn))
                {
                    // Update type-related properties, preserve designer settings
                    existingColumn.PropertyTypeName = field.fieldtype ?? existingColumn.PropertyTypeName ?? typeof(object).AssemblyQualifiedName;
                    existingColumn.ColumnType = MapPropertyTypeToDbFieldCategory(existingColumn.PropertyTypeName);
                 //   existingColumn.CellEditor = MapPropertyTypeToCellEditor(existingColumn.PropertyTypeName);
                    // Designer settings like Width, Visible, ColumnCaption remain unchanged
                  //  MiscFunctions.SendLog($"Synced Column '{field.fieldname}': Type = {existingColumn.PropertyTypeName}");
                }
                else
                {
                    // Add new column if it doesn’t exist, with defaults
                    var newColumn = new BeepColumnConfig
                    {
                        ColumnName = field.fieldname,
                        ColumnCaption = MiscFunctions.CreateCaptionFromFieldName(field.fieldname),
                        PropertyTypeName = field.fieldtype ?? typeof(object).AssemblyQualifiedName,
                        Width = 100,
                        Visible = true,
                        SortMode = DataGridViewColumnSortMode.Automatic,
                        Resizable = DataGridViewTriState.True,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    };
                    newColumn.ColumnType = MapPropertyTypeToDbFieldCategory(newColumn.PropertyTypeName);
                    newColumn.CellEditor = MapPropertyTypeToCellEditor(newColumn.PropertyTypeName);
                    _columns.Add(newColumn);
                  //  MiscFunctions.SendLog($"Added new Column '{field.fieldname}': Type = {newColumn.PropertyTypeName}");
                }
            }

            // Re-add special columns at the beginning
            List<BeepColumnConfig> updatedColumns = new List<BeepColumnConfig>();

            // Add checkbox/selection column if enabled

                if (selColumn == null)
                {
                    selColumn = new BeepColumnConfig
                    {
                        ColumnCaption =  "☑" ,
                        ColumnName = "Sel",
                        Width = _selectionColumnWidth,
                        Visible = true,
                        Sticked = true,
                        IsUnbound = true,
                        IsSelectionCheckBox = true,
                        PropertyTypeName = typeof(bool).AssemblyQualifiedName,
                        CellEditor = BeepColumnType.CheckBoxBool
                    };
                    selColumn.ColumnType = MapPropertyTypeToDbFieldCategory(selColumn.PropertyTypeName);
                }
                updatedColumns.Add(selColumn);
   

            // Add row number column if enabled

                if (rowNumColumn == null)
                {
                    rowNumColumn = new BeepColumnConfig
                    {
                        ColumnCaption = "#",
                        ColumnName = "RowNum",
                        Width = 30,
                        Visible = true,
                        Sticked = true,
                        ReadOnly = true,
                        IsUnbound = true,
                        IsRowNumColumn = true,
                        AggregationType = AggregationType.Count,
                        PropertyTypeName = typeof(int).AssemblyQualifiedName,
                        CellEditor = BeepColumnType.Text
                    };
                    rowNumColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowNumColumn.PropertyTypeName);
                }
            updatedColumns.Add(rowNumColumn);
            if (rowIDColumn == null)
            {
                rowIDColumn = new BeepColumnConfig
                {
                    ColumnCaption = "RowID",
                    ColumnName = "RowID",
                    Width = 30,
                    Index = _showCheckboxes || _showSelection ? 1 : 0,
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
                rowIDColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowNumColumn.PropertyTypeName);
                
            }
            updatedColumns.Add(rowIDColumn);
            // Add the synced entity columns
            updatedColumns.AddRange(_columns);

            // Update _columns with the new list
            _columns = updatedColumns;

            // Re-index columns
            for (int i = 0; i < _columns.Count; i++)
            {
                _columns[i].Index = i;
            }

            Invalidate();
        }
        public void CreateColumnsForEntity()
        {
            try
            {
                // Clear any existing columns
                Columns.Clear();

                // Add checkbox/selection column if enabled

                    var selColumn = new BeepColumnConfig
                    {
                        ColumnCaption =  "☑",
                        ColumnName = "Sel",
                        Width = _selectionColumnWidth,
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
                    Columns.Add(selColumn);
  

                // Add row number column if enabled

                    var rowNumColumn = new BeepColumnConfig
                    {
                        ColumnCaption = "#",
                        ColumnName = "RowNum",
                        Width = 30,
                        Index = _showCheckboxes || _showSelection ? 1 : 0,
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
                        AggregationType= AggregationType.Count
                    };
                    rowNumColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowNumColumn.PropertyTypeName);
                    Columns.Add(rowNumColumn);


                var RowIDColumn = new BeepColumnConfig
                {
                    ColumnCaption = "RowID",
                    ColumnName = "RowID",
                    Width = 30,
                    Index = _showCheckboxes || _showSelection ? 1 : 0,
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
                RowIDColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowNumColumn.PropertyTypeName);
                Columns.Add(RowIDColumn);
                // Add entity-derived columns
                int startIndex = Columns.Count; // Start indexing after special columns
                foreach (var field in Entity.Fields)
                {
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

                    Type fieldType = Type.GetType(colConfig.PropertyTypeName, throwOnError: false) ?? typeof(object);
                    if (fieldType == typeof(DateTime))
                    {
                        colConfig.Format = "g";
                    }
                    else if (fieldType.IsEnum)
                    {
                        var values = Enum.GetValues(fieldType);
                        colConfig.Items = new List<SimpleItem>();
                        foreach (var val in values)
                        {
                            colConfig.Items.Add(new SimpleItem { Value = val, Text = val.ToString() });
                        }
                    }

                    Columns.Add(colConfig);
                }
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Error adding columns for Entity {Entity?.EntityName}: {ex.Message}");
            }
        }
        private void InitializeRows()
        {
            if (Rows == null) Rows = new BindingList<BeepRowConfig>();
            Rows.Clear();
            if (_fullData == null || !_fullData.Any()) return; // No rows if no data
            int visibleHeight = DrawingRect.Height - (ShowColumnHeaders ? ColumnHeaderHeight : 0) - (ShowHeaderPanel ? headerPanelHeight : 0);
            int visibleRowCount = visibleHeight / _rowHeight;
            int dataRowCount = _fullData.Count;

            // Limit rows to the lesser of visible rows or data rows
            int rowCount = Math.Min(visibleRowCount, dataRowCount);
            for (int i = 0; i < rowCount; i++)
            {
                var row = new BeepRowConfig();
                foreach (var col in Columns)
                {
                    // Create a new cell that will later be filled with data.
                    // Note: We no longer assign UIComponent here.
                    var cell = new BeepCellConfig
                    {
                        // Do not default CellValue to string.Empty if FillVisibleRows will update it.
                        // You can leave it null or use a default if desired.
                        CellValue = null,
                        CellData = null,
                        IsEditable =true,
                        ColumnIndex = col.Index,
                        IsVisible = col.Visible,
                        RowIndex = i

                    };
                    row.Cells.Add(cell);
                }
                row.Index = i;
                row.DisplayIndex = i;
                row.Height = _rowHeight;
                Rows.Add(row);
            }
            // Initialize aggregationRow if ShowAggregationRow is true
             aggregationRow = new BeepRowConfig
            {
                Index = Rows.Count, // Last index
                DisplayIndex = -1,  // Indicate it's a sticky row
                IsAggregation = true
            };
            foreach (var col in Columns)
            {
                var cell = new BeepCellConfig
                {
                    CellValue = null,
                    CellData = null,
                    IsEditable = false, // Aggregation cells are read-only
                    ColumnIndex = col.Index,
                    IsVisible = col.Visible,
                    RowIndex = Rows.Count,
                    IsAggregation = true // Mark as aggregation cell
                };
                aggregationRow.Cells.Add(cell);
            }
          //  Rows.Add(aggregationRow);
            UpdateScrollBars();
        }
        private void InitializeData()
        {// Determine _entityType from finalData before wrapping
           
           
            // Wrap _fullData objects with RowID
            _fullData = finalData is IEnumerable<object> enumerable ? enumerable.ToList() : new List<object>();
            if (_fullData != null)
            {
                if (_fullData.Count > 0)
                {
                    var firstItem1 = _fullData.First();
                    if (firstItem1 != null && _entityType==null)
                    {
                        _entityType = firstItem1.GetType();
                        MiscFunctions.SendLog($"InitializeData: Determined _entityType from first item in finalData: {_entityType.FullName}");
                    }
                }

            }
            var wrappedData = new List<object>();
            for (int i = 0; i < _fullData.Count; i++)
            {
                var wrapper = new DataRowWrapper(_fullData[i], i)
                {
                    TrackingUniqueId = Guid.NewGuid(), // Set a unique identifier for tracking
                    RowState = DataRowState.Unchanged, // Initial state
                    DateTimeChange = DateTime.Now
                };
                wrappedData.Add(wrapper);
            }
            _fullData = wrappedData;
            _dataOffset = 0;
          
        
            if (_columns.Count == 0 && _fullData.Any())
            {
                var firstItem = _fullData.First() as DataRowWrapper;
                if (firstItem != null)
                {
                  
                    var originalData = firstItem.OriginalData;
                    _entityType = originalData.GetType();
                    var properties = originalData.GetType().GetProperties(); // Get properties of the original data object
                    int index = 0;

                    // Add checkbox/selection column if enabled

                        var selColumn = new BeepColumnConfig
                        {
                            ColumnCaption = "☑",
                            ColumnName = "Sel",
                            Width = _selectionColumnWidth,
                            Index = index++,
                            Visible = true,
                            Sticked = true,
                            ReadOnly = false,
                            IsSelectionCheckBox = true,
                            PropertyTypeName = typeof(bool).AssemblyQualifiedName,
                            CellEditor = BeepColumnType.CheckBoxBool
                        };
                        selColumn.ColumnType = MapPropertyTypeToDbFieldCategory(selColumn.PropertyTypeName);
                        _columns.Add(selColumn);
               

                    // Add row number column if enabled

                        var rowNumColumn = new BeepColumnConfig
                        {
                            ColumnCaption = "#",
                            ColumnName = "RowNum",
                            Width = 30,
                            Index = index++,
                            Visible = true,
                            Sticked = true,
                            ReadOnly = true,
                            IsRowNumColumn = true,
                            PropertyTypeName = typeof(int).AssemblyQualifiedName,
                            CellEditor = BeepColumnType.Text
                        };
                        rowNumColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowNumColumn.PropertyTypeName);
                        _columns.Add(rowNumColumn);
            

                    // Add RowID column (hidden, for internal use)
                    var rowIDColumn = new BeepColumnConfig
                    {
                        ColumnCaption = "RowID",
                        ColumnName = "RowID",
                        Width = 0, // Hidden
                        Index = index++,
                        Visible = false,
                        Sticked = false,
                        ReadOnly = true,
                        IsRowID=true,
                        PropertyTypeName = typeof(int).AssemblyQualifiedName,
                        CellEditor = BeepColumnType.Text
                    };
                    rowIDColumn.ColumnType = MapPropertyTypeToDbFieldCategory(rowIDColumn.PropertyTypeName);
                    _columns.Add(rowIDColumn);

                    // Add data columns from properties of the original data object
                    foreach (var prop in properties)
                    {
                        string propertyTypeName = prop.PropertyType.AssemblyQualifiedName;
                        var columnConfig = new BeepColumnConfig
                        {
                            ColumnCaption = prop.Name,
                            ColumnName = prop.Name,
                            Width = 100,
                            Index = index++,
                            Visible = true,
                            PropertyTypeName = propertyTypeName
                        };

                        columnConfig.ColumnType = MapPropertyTypeToDbFieldCategory(propertyTypeName);
                        columnConfig.CellEditor = MapPropertyTypeToCellEditor(propertyTypeName);

                        _columns.Add(columnConfig);
                    }
                }
            }

            originalList.Clear();
            originalList.AddRange(_fullData); // Snapshot initial data
            Trackings.Clear();
            for (int i = 0; i < _fullData.Count; i++)
            {
                Trackings.Add(new Tracking(Guid.NewGuid(), i, i) { EntityState = EntityState.Unchanged });
            }
            InitializeRows();
            UpdateScrollBars();

            // Populate columns in ComboBox (including "All Columns" option)
            filterColumnComboBox.Items.Add(new SimpleItem { Text = "All Columns", Value = null });
            foreach (var col in Columns)
            {
                if (col.Visible)
                    filterColumnComboBox.Items.Add(new SimpleItem { Text = col.ColumnCaption ?? col.ColumnName, Value = col.ColumnName });
            }
            filterColumnComboBox.SelectedIndex = 0; // Default to "All Columns"

            // Attach _fullData to DataNavigator as an IList
            if (DataNavigator != null) DataNavigator.DataSource = _fullData;
        }
        #endregion
        #region Data Filling and Navigation
        private void FillVisibleRows()
        {
            if (_fullData == null || !_fullData.Any())
            {
                MiscFunctions.SendLog("FillVisibleRows: No data available");
                Rows.Clear();
                return;
            }

            int visibleRowCount = GetVisibleRowCount();
            int startRow = Math.Max(0, _dataOffset);
            int endRow = Math.Min(_dataOffset + visibleRowCount, _fullData.Count);

            // Clear and recreate only visible rows
            Rows.Clear();
            for (int i = startRow; i < endRow; i++)
            {
                int dataIndex = i;
                var row = new BeepRowConfig
                {
                    Index = i - _dataOffset,
                    DisplayIndex = dataIndex,
                    IsAggregation = false,
                    Height = _rowHeights.ContainsKey(dataIndex) ? _rowHeights[dataIndex] : _rowHeight,
                    OldDisplayIndex = dataIndex // Initially same as DisplayIndex
                };
                var dataItem = _fullData[dataIndex] as DataRowWrapper;
                if (dataItem != null)
                {
                    EnsureTrackingForItem(dataItem);
                    row.IsDataLoaded = true;

                    for (int j = 0; j < Columns.Count; j++)
                    {
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
                    }
                }
                Rows.Add(row);
            }

            // Update aggregationRow separately
            if (_showaggregationRow && aggregationRow != null)
            {
                for (int j = 0; j < aggregationRow.Cells.Count && j < Columns.Count; j++)
                {
                    var col = Columns[j];
                    var cell = aggregationRow.Cells[j];
                    if (cell.IsAggregation)
                    {
                        object aggregatedValue = ComputeAggregation(col, _fullData);
                        cell.CellValue = aggregatedValue?.ToString() ?? "";
                        cell.CellData = aggregatedValue;
                    }
                }
            }

            // Update _selectedRows and _selectedgridrows
            var newSelectedRows = new List<int>();
            var newSelectedGridRows = new List<BeepRowConfig>();
            for (int i = 0; i < Rows.Count; i++)
            {
                int dataIndex = _dataOffset + i;
                var dataItem = _fullData[dataIndex] as DataRowWrapper;
                if (dataItem != null)
                {
                    int rowID = dataItem.RowID;
                    if (_persistentSelectedRows.ContainsKey(rowID) && _persistentSelectedRows[rowID])
                    {
                        newSelectedRows.Add(i);
                        newSelectedGridRows.Add(Rows[i]);
                    }
                }
            }
            _selectedRows = newSelectedRows;
            _selectedgridrows = newSelectedGridRows;
            UpdateTrackingIndices();
            SyncSelectedRowIndexAndEditor();
            UpdateCellPositions();
            UpdateScrollBars();
            UpdateRecordNumber();
            Invalidate();
            MiscFunctions.SendLog($"FillVisibleRows: Updated {Rows.Count} visible rows, _fullData.Count={_fullData.Count}");
        }
        private void MoveNextRow()
        {
            if (_currentRowIndex < Rows.Count - 1)
            {
                SelectCell(_currentRowIndex + 1, _selectedColumnIndex);
                ScrollBy(1);
            }
        }
        private void MovePreviousRow()
        {
            if (_currentRowIndex > 0)
            {
                SelectCell(_currentRowIndex - 1, _selectedColumnIndex);
                ScrollBy(-1);
            }
        }
        public void MoveNextCell()
        {
            try
            {
                if (Rows == null || Rows.Count == 0 || Columns == null || Columns.Count == 0)
                {
                 //   MiscFunctions.SendLog("MoveNextCell: No rows or columns available.");
                    return;
                }

              //  MiscFunctions.SendLog($"MoveNextCell: Current position - Row: {_currentRowIndex}, Col: {_selectedColumnIndex}");

                int lastVisibleColumn = GetLastVisibleColumn();
                int firstVisibleColumn = GetNextVisibleColumn(-1); // Find the first column
                int nextColumn = GetNextVisibleColumn(_selectedColumnIndex);

              //  MiscFunctions.SendLog($"LastVisibleColumn: {lastVisibleColumn}, FirstVisibleColumn: {firstVisibleColumn}, NextColumn: {nextColumn}");

                // 🔹 If at the last column of the last row, wrap back to the first row/column
                if (_currentRowIndex == Rows.Count - 1 && _selectedColumnIndex == lastVisibleColumn)
                {
                    if (firstVisibleColumn == -1)
                    {
              //          MiscFunctions.SendLog("No visible columns to wrap to.");
                        return;
                    }
               //     MiscFunctions.SendLog($"🔄 Wrapping to first row, first column: {firstVisibleColumn}");
                    SelectCell(0, firstVisibleColumn);
                    EnsureColumnVisible(firstVisibleColumn);
                    return;
                }

                // 🔹 If at the last visible column but more columns exist, scroll and move to the next column
                if (_selectedColumnIndex == lastVisibleColumn && nextColumn != -1)
                {
                //    MiscFunctions.SendLog($"➡ Scrolling to next column: {nextColumn}");
                    EnsureColumnVisible(nextColumn);
                    SelectCell(_currentRowIndex, nextColumn);
                    return;
                }

                // 🔹 If at the last visible column of the row, move to the first column of the next row
                if (_selectedColumnIndex == lastVisibleColumn)
                {
                    if (_currentRowIndex < Rows.Count - 1)
                    {
               //         MiscFunctions.SendLog($"➡ Moving to next row, first column: {firstVisibleColumn}");
                        SelectCell(_currentRowIndex + 1, firstVisibleColumn);
                        EnsureColumnVisible(firstVisibleColumn);
                        ScrollBy(1); // Ensure next row is visible
                    }
                    else
                    {
              //          MiscFunctions.SendLog($"🔄 Wrapping to first column in the same row: {firstVisibleColumn}");
                        SelectCell(_currentRowIndex, firstVisibleColumn);
                        EnsureColumnVisible(firstVisibleColumn);
                    }
                }
                else
                {
                    // Move to next visible column
                    if (nextColumn == -1)
                    {
               //         MiscFunctions.SendLog("❌ No next visible column found.");
                        return;
                    }
               //     MiscFunctions.SendLog($"➡ Moving to next column: {nextColumn}");
                    EnsureColumnVisible(nextColumn);
                    SelectCell(_currentRowIndex, nextColumn);
                    //Focus();
                }
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"MoveNextCell crashed: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
        private int GetLastVisibleColumn()
        {
            try
            {
                if (Columns == null || Columns.Count == 0)
                {
                 //   MiscFunctions.SendLog("GetLastVisibleColumn: Columns is null or empty.");
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
                MiscFunctions.SendLog($"GetLastVisibleColumn crashed: {ex.Message}");
                return -1;
            }
        }
        private int GetNextVisibleColumn(int currentIndex)
        {
            try
            {
                if (Columns == null || Columns.Count == 0)
                {
                  //  MiscFunctions.SendLog("GetNextVisibleColumn: Columns is null or empty.");
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
                MiscFunctions.SendLog($"GetNextVisibleColumn crashed: {ex.Message}");
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
                 //   MiscFunctions.SendLog($"EnsureColumnVisible: Invalid column index {colIndex}");
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
                MiscFunctions.SendLog($"EnsureColumnVisible crashed: {ex.Message}\nStackTrace: {ex.StackTrace}");
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
        private BeepControl CreateCellControlForEditing(BeepCellConfig cell)
        {
            // Get the column definition based on cell.ColumnIndex.
            var column = Columns[cell.ColumnIndex];

            switch (column.CellEditor)
            {
                case BeepColumnType.Text:
                    return new BeepTextBox { Theme = Theme, IsChild = true };
                case BeepColumnType.CheckBoxBool:
                    return new BeepCheckBoxBool { Theme = Theme,HideText=true,Text=string.Empty, IsChild = true };
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
                    return new BeepRadioButton { Theme = Theme, IsChild = true };
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
            BeepRowConfig row = Rows[cell.RowIndex];
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
                    case BeepRadioButton radioButton:
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

                case BeepRadioButton radioButton:
                    radioButton.Reset();
                    if (column?.Items != null)
                    {
                        radioButton.Options = new List<SimpleItem>(column.Items);
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
                    image.ImagePath = ImageListHelper.GetImagePathFromName(cell.CellValue.ToString());
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
            //        var childCell = row.Cells.FirstOrDefault(c => c.ColumnIndex == childColumn.Index);
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
        protected override void OnPaint(PaintEventArgs e)
        {
            // base.OnPaint(e);
            UpdateDrawingRect();
            var g = e.Graphics;
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
            if (!_navigatorDrawn)
            {
                _navigatorDrawn = true;
                if (_showNavigator)
                {
                    navigatorPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), navigatorPanelHeight);
                    DrawNavigationRow(g, navigatorPanelRect);
                }
                else
                {
                    navigatorPanelRect = new Rectangle(-100, -100, drawingBounds.Width, navigatorPanelHeight);
                    DrawNavigationRow(g, navigatorPanelRect);
                }
            }

            if (_showFooter)
            {
                bottomPanelY -= footerPanelHeight;
                botomspacetaken += footerPanelHeight;
                footerPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), footerPanelHeight);
                DrawFooterRow(g, footerPanelRect);
            }

            if (_showaggregationRow)
            {
                bottomPanelY -= bottomagregationPanelHeight;
                botomspacetaken += bottomagregationPanelHeight;
                bottomagregationPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), bottomagregationPanelHeight);
                DrawBottomAggregationRow(g, bottomagregationPanelRect);
            }

            // Draw Top Items before Drawing the Grid
            int filterPanelHeight = 40;
            if (!_filterpaneldrawn)
            {
                _filterpaneldrawn = true;
                if (_showFilterpanel)
                {
                    filterPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), filterPanelHeight);
                    DrawFilterPanel(g, filterPanelRect);
                }
                else
                {
                    filterPanelRect = new Rectangle(-100, -100, drawingBounds.Width, 30);
                    DrawFilterPanel(g, filterPanelRect);
                }
            }
            if (_showFilterpanel)
            {
                topPanelY += filterPanelHeight + 10;
            }
            if (_showHeaderPanel)
            {
                headerPanelHeight = titleLabel?.GetPreferredSize(Size.Empty).Height ?? headerPanelHeight;
                headerPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), headerPanelHeight);
                DrawHeaderPanel(g, headerPanelRect);
                topPanelY += headerPanelHeight;
            }
            if (_showColumnHeaders)
            {
                columnsheaderPanelRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0), ColumnHeaderHeight);
                PaintColumnHeaders(g, columnsheaderPanelRect);
                topPanelY += ColumnHeaderHeight;
            }

            // Grid would Draw on the remaining space
            int availableHeight = drawingBounds.Height - topPanelY - botomspacetaken;
            int availableWidth = drawingBounds.Width - (_verticalScrollBar.Visible ? _verticalScrollBar.Width : 0);
            gridRect = new Rectangle(drawingBounds.Left, topPanelY, availableWidth, availableHeight);

            // Draw grid content
            UpdateStickyWidth();
            PaintRows(g, gridRect);

            if (_showverticalgridlines)
                DrawColumnBorders(g, gridRect);
            if (_showhorizontalgridlines)
                DrawRowsBorders(g, gridRect);

            // Position scrollbars after rendering
            PositionScrollBars();

            // Ensure editor control is visible if present
            if (IsEditorShown && _editingControl != null && _editingControl.Parent == this)
            {
                _editingControl.Invalidate(); // Force editor redraw if needed
            }
            if (_horizontalScrollBar.Visible)
            {
                _horizontalScrollBar.Invalidate(); // Force horizontal scrollbar redraw
            }
            if (_verticalScrollBar.Visible)
            {
                _verticalScrollBar.Invalidate(); // Force vertical scrollbar redraw
            }
            if(buttons.Count > 0)
            {
                foreach (var button in buttons)
                {
                    button.Invalidate();
                }
            }
        }
        private void DrawFilterPanel(Graphics g, Rectangle filterPanelRect)
        {
            int filterX = 10; // Left padding
            // Position Filter controls on filterpanelrect
            if (filterColumnComboBox != null)
            {
                filterColumnComboBox.Location = new Point(filterPanelRect.X + filterX, filterPanelRect.Y+5);
                filterColumnComboBox.Size = new Size(200, 25);
                filterX += filterColumnComboBox.Width + 10; // Add padding
                filterTextBox.Location = new Point(filterPanelRect.X + filterX, filterPanelRect.Y + 5);
                filterTextBox.Size = new Size(200, 24);
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
            var stickyColumns = Columns.Where(c => c.Sticked && c.Visible).ToList();
            int baseStickyWidth = stickyColumns.Sum(c => c.Width);

            // Cap _stickyWidth to prevent overflow within gridRect
            _stickyWidth = Math.Min(baseStickyWidth, gridRect.Width);
          //   MiscFunctions.SendLog($"UpdateStickyWidth: _stickyWidth={_stickyWidth}, BaseSticky={baseStickyWidth}, GridRect.Width={gridRect.Width}");
        }
        private void PaintColumnHeaders(Graphics g, Rectangle headerRect)
        {
            int xOffset = headerRect.Left;

            // Ensure _stickyWidth is calculated and capped
            UpdateStickyWidth();
            int stickyWidth = _stickyWidth;
            stickyWidth = Math.Min(stickyWidth, headerRect.Width); // Prevent overflow

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
                 //   MiscFunctions.SendLog($"PaintColumnHeaders Scrolling: Col={col.ColumnName}, X={scrollingXOffset}, Width={col.Width}, _xOffset={_xOffset}");
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
                   // MiscFunctions.SendLog($"PaintColumnHeaders Sticky: Col={col.ColumnName}, X={xOffset}, Width={col.Width}");
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

          //  MiscFunctions.SendLog($"PaintColumnHeaders: StickyWidth={stickyWidth}, HeaderRect={headerRect}, _xOffset={_xOffset}");
        }
        private void PaintHeaderCell(Graphics g, BeepColumnConfig col, Rectangle cellRect, StringFormat format)
        {
            using (Brush bgBrush = new SolidBrush(_currentTheme.HeaderBackColor))
            using (Brush textBrush = new SolidBrush(_currentTheme.ButtonForeColor)) // Your preferred color
            {
                g.FillRectangle(bgBrush, cellRect);
               // g.DrawRectangle(Pens.Black, cellRect);
                g.DrawString(col.ColumnCaption, _columnHeadertextFont ?? Font, textBrush, cellRect, format);
            }
        }
        private void PaintRows(Graphics g, Rectangle bounds)
        {
            int yOffset = bounds.Top;
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
                    int scrollingStartX = scrollingRegion.Left - _xOffset;
                    int totalScrollableWidth = Columns.Where(c => !c.Sticked && c.Visible).Sum(c => c.Width) +
                                              (Columns.Count(c => !c.Sticked && c.Visible) - 1) * 1;
                    int scrollingWidth = Math.Max(scrollingRegion.Width + _xOffset, totalScrollableWidth);
                    var rowRect = new Rectangle(scrollingStartX, displayY, scrollingWidth, row.Height);

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
            int xOffset = rowRect.Left; // Starts at scrollingRegion.Left - _xOffset from PaintRows
            int rightBoundary = rowRect.Right; // rowRect’s right edge
            int totalScrollableWidth = Columns.Where(c => !c.Sticked && c.Visible).Sum(c => c.Width) +
                                      (Columns.Count(c => !c.Sticked && c.Visible) - 1) * 1; // Include border width
            int leftBoundary = rowRect.Left; // Left edge of the scrolling region (already includes _stickyWidth)

            // Adjust initial xOffset to prevent drawing beyond leftBoundary
            if (xOffset < leftBoundary)
            {
                int overflow = leftBoundary - xOffset;
                xOffset = leftBoundary;
            }

            for (int i = 0; i < row.Cells.Count && i < Columns.Count; i++)
            {
                if (!Columns[i].Visible || Columns[i].Sticked) continue;

                var cell = row.Cells[i];
                cell.X = xOffset;
                cell.Y = rowRect.Top;
                cell.Width = Columns[i].Width;
                cell.Height = rowRect.Height;

                // Skip or adjust if cell starts outside leftBoundary
                if (xOffset < leftBoundary)
                {
                    int overflow = leftBoundary - xOffset;
                    cell.Width = Math.Max(0, cell.Width - overflow);
                    cell.X = leftBoundary;
                    if (cell.Width <= 0) continue; // Skip if fully outside
                }

                // Stop if cell would exceed rowRect.Right
                if (xOffset + cell.Width > rightBoundary)
                {
                    cell.Width = Math.Max(0, rightBoundary - xOffset); // Truncate last cell if needed
                    if (cell.Width <= 0) break; // No room left
                }

                var cellRect = new Rectangle(cell.X, cell.Y, cell.Width, cell.Height);
                Color backcolor = cell.RowIndex == _currentRowIndex ? _currentTheme.SelectedRowBackColor : _currentTheme.GridBackColor;
                PaintCell(g, cell, cellRect, backcolor);
                // set cell coordinates and size in cell
                cell.X = cellRect.X;
                cell.Y = cellRect.Y;
                cell.Width = cellRect.Width;
                cell.Height = cellRect.Height;
                xOffset += Columns[i].Width;
                if (xOffset >= rightBoundary && xOffset < rowRect.Left + totalScrollableWidth)
                    rightBoundary = Math.Min(rowRect.Left + totalScrollableWidth, rowRect.Right); // Extend boundary if within scrollable range
                if (xOffset >= rightBoundary) break; // Exit if past boundary

                // Debug output for scrolling cells
             //   MiscFunctions.SendLog($"PaintScrollingRow: Row={cell.RowIndex}, Col={Columns[i].ColumnName}, X={cell.X}, Width={cell.Width}, LeftBoundary={leftBoundary}, RightBoundary={rightBoundary}");
            }
        }
        private void PaintCell(Graphics g, BeepCellConfig cell, Rectangle cellRect, Color backcolor)
        {
            // If this cell is being edited, skip drawing so that
            // the editor control remains visible.
            //  if (_selectedCell == cell && !_columns[_selectedCell.ColumnIndex].ReadOnly) return;
            Rectangle TargetRect = cellRect;
            cell.Rect = TargetRect;

            BeepColumnConfig column = Columns[cell.ColumnIndex];
            BeepRowConfig row = Rows[cell.RowIndex];
            using (var cellBrush = new SolidBrush(backcolor))
            {
                g.FillRectangle(cellBrush, TargetRect);
            }
            if (_selectedCell == cell)
            {
                using (var cellBrush = new SolidBrush(_currentTheme.GridRowHoverBackColor)) // ✅ Use a highlight color
                {
                    g.FillRectangle(cellBrush, TargetRect);
                }
                using (var borderPen = new Pen(_currentTheme.PrimaryTextColor, 2)) // ✅ Add a border to show selection
                {
                    g.DrawRectangle(borderPen, TargetRect);
                }
            }

            // Get the column editor if available
            if (!_columnEditors.TryGetValue(Columns[cell.ColumnIndex].ColumnName, out IBeepUIComponent columnEditor))

            {

                // Create a new control if it doesn't exist (failsafe)
                columnEditor = CreateCellControlForEditing(cell);
                _columnEditors[Columns[cell.ColumnIndex].ColumnName] = columnEditor;
            }

            if (columnEditor != null)
            {
                var editor = (Control)columnEditor;
                editor.Bounds = new Rectangle(TargetRect.X, TargetRect.Y, TargetRect.Width, TargetRect.Height);
                 var checkValueupdate=new BeepCellEventArgs(cell);
                CellPreUpdateCellValue?.Invoke(this, checkValueupdate);
                if(!checkValueupdate.Cancel)
                {
                    UpdateCellControl(columnEditor, Columns[cell.ColumnIndex], cell, cell.CellValue);

                }

                // Force BeepTextBox for aggregation cells
                if (cell.IsAggregation)
                {
                    BeepTextBox textBox = columnEditor as BeepTextBox ?? new BeepTextBox
                    {
                        Theme = Theme,
                        IsReadOnly = true, // Aggregation cells are read-only
                        Text = cell.CellValue?.ToString() ?? ""
                    };
                    textBox.ForeColor = _currentTheme.GridForeColor;
                    textBox.BackColor = _currentTheme.GridBackColor;
                    textBox.Draw(g, TargetRect);
                }
                else
                {
                   
                    var checkCustomDraw = new BeepCellEventArgs(cell);
                    checkCustomDraw.Graphics= g;
                    CellCustomCellDraw?.Invoke(this, checkCustomDraw);
                    if(checkCustomDraw.Cancel)
                    {
                        return;
                    }
                    // Draw the editor based on column type for non-aggregation cells
                    switch (columnEditor)
                    {
                        case BeepTextBox textBox:
                            textBox.ForeColor = _currentTheme.GridForeColor;
                            textBox.BackColor = _currentTheme.GridBackColor;
                            textBox.Draw(g, TargetRect);
                            break;
                        case BeepCheckBoxBool checkBox1:
                            checkBox1.ForeColor = _currentTheme.GridForeColor;
                            checkBox1.BackColor = _currentTheme.GridBackColor;
                            checkBox1.Draw(g, TargetRect);
                            break;
                        case BeepCheckBoxChar checkBox2:
                            checkBox2.ForeColor = _currentTheme.GridForeColor;
                            checkBox2.BackColor = _currentTheme.GridBackColor;
                            checkBox2.Draw(g, TargetRect);
                            break;
                        case BeepCheckBoxString checkBox3:
                            checkBox3.ForeColor = _currentTheme.GridForeColor;
                            checkBox3.BackColor = _currentTheme.GridBackColor;
                            checkBox3.Draw(g, TargetRect);
                            break;
                        case BeepComboBox comboBox:
                            comboBox.ForeColor = _currentTheme.GridForeColor;
                            comboBox.BackColor = _currentTheme.GridBackColor;
                           
                            comboBox.Draw(g, TargetRect);
                            break;
                        case BeepDatePicker datePicker:
                            datePicker.ForeColor = _currentTheme.GridForeColor;
                            datePicker.BackColor = _currentTheme.GridBackColor;
                            datePicker.Draw(g, TargetRect);
                            break;
                        case BeepImage image:
                            image.Size=new Size(column.MaxImageWidth, column.MaxImageHeight);
                            image.DrawImage(g, TargetRect);
                            break;
                        case BeepButton button:
                            button.ForeColor = _currentTheme.GridForeColor;
                            button.BackColor = _currentTheme.GridBackColor;
                            button.Draw(g, TargetRect);
                            break;
                        case BeepProgressBar progressBar:
                            progressBar.ForeColor = _currentTheme.GridForeColor;
                            progressBar.BackColor = _currentTheme.GridBackColor;
                            progressBar.Draw(g, TargetRect);
                            break;
                        case BeepStarRating starRating:
                            starRating.ForeColor = _currentTheme.GridForeColor;
                            starRating.BackColor = _currentTheme.GridBackColor;
                            starRating.Draw(g, TargetRect);
                            break;
                        case BeepNumericUpDown numericUpDown:
                            numericUpDown.ForeColor = _currentTheme.GridForeColor;
                            numericUpDown.BackColor = _currentTheme.GridBackColor;
                            numericUpDown.Draw(g, TargetRect);
                            break;
                        case BeepSwitch switchControl:
                            switchControl.ForeColor = _currentTheme.GridForeColor;
                            switchControl.BackColor = _currentTheme.GridBackColor;
                            switchControl.Draw(g, TargetRect);
                            break;
                        case BeepListofValuesBox listBox:
                            listBox.ForeColor = _currentTheme.GridForeColor;
                            listBox.BackColor = _currentTheme.GridBackColor;
                            listBox.Draw(g, TargetRect);
                            break;
                        case BeepLabel label:
                            label.ForeColor = _currentTheme.GridForeColor;
                            label.BackColor = _currentTheme.GridBackColor;
                            label.Draw(g, TargetRect);
                            break;
                        default:
                            using (var textBrush = new SolidBrush(_currentTheme.GridForeColor))
                            {
                                g.DrawString(cell.CellValue?.ToString() ?? "", Font, textBrush, TargetRect,
                                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                            }
                            break;
                    }
                }
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

            // MiscFunctions.SendLog($"DrawColumnBorders: StickyWidth={stickyWidth}, LastXOffset={xOffset}, Bounds={bounds}, XOffset={_xOffset}");
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
            titleLabel.TextFont = _textFont;
            titleLabel.ImageAlign = ContentAlignment.MiddleLeft;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.TextImageRelation = TextImageRelation.ImageBeforeText;
            titleLabel.Size = new Size(rect.Width - 2, rect.Height - 2);
            titleLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            titleLabel.BackColor = _currentTheme.BackColor;
            titleLabel.Text = TitleText;
            titleLabel.Theme = Theme;
            titleLabel.DrawToGraphics(g, rect);
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
        private void ApplyFilter(string filterText, string columnName)
        {
            if (string.IsNullOrWhiteSpace(filterText) || _fullData == null)
            {
                FillVisibleRows(); // Reset to full data
               _bindingSource.DataSource = _fullData;
                return;
            }

            filterText = filterText.ToLowerInvariant();
            List<object> filteredData;

            if (string.IsNullOrEmpty(columnName)) // Global search (All Columns)
            {
                filteredData = _fullData.Where(item =>
                    Columns.Where(c => c.Visible).Any(col =>
                    {
                        var prop = item.GetType().GetProperty(col.ColumnName);
                        var value = prop?.GetValue(item)?.ToString()?.ToLowerInvariant();
                        return value != null && value.Contains(filterText);
                    })).ToList();
            }
            else // Column-specific search
            {
                filteredData = _fullData.Where(item =>
                {
                    var prop = item.GetType().GetProperty(columnName);
                    var value = prop?.GetValue(item)?.ToString()?.ToLowerInvariant();
                    return value != null && value.Contains(filterText);
                }).ToList();
            }

            // Update grid with filtered data
            _dataOffset = 0; // Reset scroll position
            _bindingSource.DataSource = filteredData;
            FillVisibleRows();
            UpdateScrollBars();
            Invalidate();
        }
        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            string selectedColumn = filterColumnComboBox.SelectedItem is SimpleItem item && item.Value != null
                ? item.Value.ToString()
                : null;
            ApplyFilter(filterTextBox.Text, selectedColumn);
        }

        private void FilterColumnComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedColumn = filterColumnComboBox.SelectedItem is SimpleItem item && item.Value != null
                ? item.Value.ToString()
                : null;
            ApplyFilter(filterTextBox.Text, selectedColumn);
        }
        #endregion Filter Panel
        #region Cell Editing
        public void SelectCell(int rowIndex, int columnIndex)
        {
            if (IsEditorShown)
            {
                CloseCurrentEditorIn();
            }
            if (rowIndex < 0 || rowIndex >= Rows.Count) return;
            if (columnIndex < 0 || columnIndex >= Columns.Count) return;

            _currentRowIndex = rowIndex;
            _selectedColumnIndex = columnIndex;
            _selectedCell = Rows[rowIndex].Cells[columnIndex];
            // Set OilDisplayIndex when selecting a row
      
            // 🔹 Use updated X and Y positions
            int cellX = _selectedCell.X;
            int cellY = _selectedCell.Y;
            Point cellLocation = new Point(cellX, cellY);
            CurrentRow = Rows[rowIndex];
         
            // put row data in the selected row
            CurrentRow.RowData = _fullData[rowIndex];
            CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(rowIndex, CurrentRow));
            CurrentCellChanged?.Invoke(this, new BeepCellSelectedEventArgs(rowIndex, columnIndex, _selectedCell));
            UpdateRecordNumber();
            Invalidate();
        }
        public void SelectCell(BeepCellConfig cell)
        {
            if (cell == null) return;
            _editingRowIndex = cell.RowIndex; // Absolute index in _fullData
            SelectCell(cell.RowIndex, cell.ColumnIndex);
        }
        private void MoveEditorIn()
        {
            if (_editingCell == null || _editingControl == null || !IsEditorShown)
            {
              //   MiscFunctions.SendLog("MoveEditor: Skipped - null reference or editor not shown");
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
                // MiscFunctions.SendLog($"MoveEditor: Editor out of view - Hiding (CellRect={cellRect})");
                _editingControl.Visible = false;
            }
            else
            {
                // Position editor within gridRect’s client coordinates
                _editingControl.Location = new Point(cellRect.X, cellRect.Y);
                _editingControl.Visible = true;
             //    MiscFunctions.SendLog($"MoveEditor: Editor moved to {cellRect.X},{cellRect.Y}");
            }
        }
        private BeepRowConfig GetRowAtLocation(Point location)
        {
            // First, ensure the point is inside the grid's drawing area.
            if (!gridRect.Contains(location))
                return null;
            // Compute the Y coordinate relative to gridRect.
            int yRelative = location.Y - gridRect.Top;
            int rowIndex = yRelative / RowHeight;
            if (rowIndex < 0 || rowIndex >= Rows.Count)
                return null;
            return Rows[rowIndex];
        }
        private void MoveEditor()
        {
            if (_editingCell == null || _editingControl == null || _editorPopupForm == null)
            {
     //           MiscFunctions.SendLog("MoveEditor: Skipped - null reference");
                return;
            }

            // 🔹 Get the exact rectangle of the cell **after scrolling**
            Rectangle cellRect = GetCellRectangle(_editingCell);
          //  MiscFunctions.SendLog($"MoveEditor: cellRect.Y={cellRect.Y}, _dataOffset={_dataOffset}");

            int gridLeft = gridRect.Left;
            int gridRight = gridRect.Right;
            int gridTop = gridRect.Top;
            int gridBottom = gridRect.Bottom;

            // 🔹 Adjust for vertical scrolling (subtract scroll offset)
            cellRect.Y -= (_dataOffset * RowHeight);

            // 🔹 Check if the editor should be hidden (out of grid bounds)
            bool isFullyOutOfView =
                (cellRect.Right < gridLeft) ||  // Completely off to the left
                (cellRect.Left > gridRight) ||  // Completely off to the right
                (cellRect.Bottom < gridTop) ||  // 🔹 Scrolled out past the **top** of the grid
                (cellRect.Top > gridBottom);    // Completely off below

            if (isFullyOutOfView)
            {
          //      MiscFunctions.SendLog("MoveEditor: Editor is out of view - Hiding");
                _editorPopupForm.Visible = false;
                return;
            }

            // 🔹 Move the editor **with scrolling**
            Point screenLocation = this.PointToScreen(new Point(cellRect.X, cellRect.Y));
            _editorPopupForm.Location = screenLocation;
            _editorPopupForm.Visible = true;
        }
        private BeepCellConfig GetCellAtLocation(Point location)
        {
            // Ensure the point is inside the grid's drawing area
            if (!gridRect.Contains(location))
            {
                MiscFunctions.SendLog($"GetCellAtLocation: Location {location} outside gridRect {gridRect}");
                return null;
            }

            // Compute the Y coordinate relative to gridRect
            int yRelative = location.Y - gridRect.Top;
            if (yRelative < 0)
            {
                MiscFunctions.SendLog($"GetCellAtLocation: yRelative {yRelative} above grid");
                return null;
            }

            // Find the row by summing row heights
            int currentY = 0;
            int rowIndex = -1;
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
            if (rowIndex == -1 || rowIndex >= Rows.Count)
            {
                MiscFunctions.SendLog($"GetCellAtLocation: No row found for yRelative {yRelative}");
                return null;
            }
            var row = Rows[rowIndex];

            // Compute the X coordinate relative to gridRect
            int xRelative = location.X - gridRect.Left;
            int stickyWidthTotal = _stickyWidth; // Set in PaintRows
            int xAdjusted = xRelative;

            // Handle sticky columns
            if (xRelative < stickyWidthTotal)
            {
                int currentX = 0;
                for (int i = 0; i < Columns.Count; i++)
                {
                    var column = Columns[i];
                    if (!column.Visible || !column.Sticked) continue;
                    if (xRelative >= currentX && xRelative < currentX + column.Width)
                    {
                        return row.Cells[i];
                    }
                    currentX += column.Width;
                }
                return null;
            }
            else
            {
                // Adjust for scrolling non-sticky columns
                xAdjusted += _xOffset;
                int currentX = stickyWidthTotal; // Start after sticky region
                for (int i = 0; i < Columns.Count; i++)
                {
                    var column = Columns[i];
                    if (!column.Visible || column.Sticked) continue;
                    if (xAdjusted >= currentX && xAdjusted < currentX + column.Width)
                    {
                        return row.Cells[i];
                    }
                    currentX += column.Width;
                }
            }

            MiscFunctions.SendLog($"GetCellAtLocation: No cell found at {location}");
            return null;
        }
        private Rectangle GetCellRectangleIn(BeepCellConfig cell)
        {
            if (cell == null)
            {
                MiscFunctions.SendLog("GetCellRectangleIn: Cell is null");
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
                MiscFunctions.SendLog("GetCellRectangleIn: Cell not found in Rows");
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
            MiscFunctions.SendLog($"GetCellRectangleIn: Cell={x},{y}, Size={width}x{height}, RowIndex={rowIndex}, ColIndex={colIndex}");
            return rect;
        }
        private Rectangle GetCellRectangle(BeepCellConfig cell)
        {
            if (cell == null)
            {
                MiscFunctions.SendLog("GetCellRectangle: Cell is null");
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
                MiscFunctions.SendLog("GetCellRectangle: Cell not found in Rows");
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

            return new Rectangle(x, y, width, height);
        }
        #endregion Cell Editing
        #region Scrollbar Management
        private int _cachedTotalColumnWidth = 0; // Cache total column width
        private int _cachedMaxXOffset = 0; // Cache maximum horizontal offset

        private void StartSmoothScroll(int targetVertical, int targetHorizontal = -1)
        {
            int maxVerticalOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
            _scrollTargetVertical = Math.Max(0, Math.Min(targetVertical, maxVerticalOffset));
            bool verticalChanged = _scrollTargetVertical != _dataOffset;

            if (targetHorizontal >= 0)
            {
                UpdateCachedHorizontalMetrics(); // Update cached values
                _scrollTargetHorizontal = Math.Max(0, Math.Min(targetHorizontal, _cachedMaxXOffset));
            }
            else
            {
                _scrollTargetHorizontal = _xOffset;
            }
            bool horizontalChanged = _scrollTargetHorizontal != _xOffset;

            if (verticalChanged || horizontalChanged)
            {
                if (_scrollTimer == null)
                {
                    _scrollTimer = new Timer { Interval = 16 }; // Approx 60 FPS
                    _scrollTimer.Tick += ScrollTimer_Tick;
                }
                _scrollTimer.Start();
             //   MiscFunctions.SendLog($"StartSmoothScroll: TargetV={_scrollTargetVertical}, TargetH={_scrollTargetHorizontal}, CurrentV={_dataOffset}, CurrentH={_xOffset}");
            }
        }
        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            bool updated = false;
            double easingFactor = 0.2; // Smooth easing factor

            // Vertical scrolling
            if (_dataOffset < _scrollTargetVertical)
            {
                _dataOffset += (int)Math.Ceiling((_scrollTargetVertical - _dataOffset) * easingFactor);
                if (_dataOffset >= _scrollTargetVertical) _dataOffset = _scrollTargetVertical;
                updated = true;
            }
            else if (_dataOffset > _scrollTargetVertical)
            {
                _dataOffset -= (int)Math.Ceiling((_dataOffset - _scrollTargetVertical) * easingFactor);
                if (_dataOffset <= _scrollTargetVertical) _dataOffset = _scrollTargetVertical;
                updated = true;
            }

            // Horizontal scrolling
            if (_xOffset < _scrollTargetHorizontal)
            {
                _xOffset += (int)Math.Ceiling((_scrollTargetHorizontal - _xOffset) * easingFactor);
                if (_xOffset >= _scrollTargetHorizontal) _xOffset = _scrollTargetHorizontal;
                updated = true;
            }
            else if (_xOffset > _scrollTargetHorizontal)
            {
                _xOffset -= (int)Math.Ceiling((_xOffset - _scrollTargetHorizontal) * easingFactor);
                if (_xOffset <= _scrollTargetHorizontal) _xOffset = _scrollTargetHorizontal;
                updated = true;
            }

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
                FillVisibleRows();
                UpdateScrollBars(); // Sync scrollbar values
                                    // Invalidate only the scrolled region
                Rectangle scrollRegion = new Rectangle(gridRect.Left - _xOffset, gridRect.Top, gridRect.Width, gridRect.Height);
                Invalidate(scrollRegion); // Invalidate only the visible area
             //   MiscFunctions.SendLog($"ScrollTimer_Tick: OffsetV={_dataOffset}, OffsetH={_xOffset}, Updated={updated}");
            }
        }
        // Mouse wheel support for smooth scrolling
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
        private void UpdateScrollBars()
        {
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

            int maxOffset = Math.Max(0, _fullData.Count - visibleRowCount );

            if (_showVerticalScrollBar && _fullData.Count >= visibleRowCount )
            {
                _verticalScrollBar.Minimum = 0;
                _verticalScrollBar.Maximum = maxOffset + visibleRowCount;
                _verticalScrollBar.LargeChange = visibleRowCount ;
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

              //  MiscFunctions.SendLog($"UpdateScrollBars Horizontal: totalColumnWidth={totalColumnWidth}, visibleWidth={visibleWidth}, stickyWidth={stickyWidth}, maxXOffset={maxXOffset}, _xOffset={_xOffset}");
            }
            else
            {
                if (_horizontalScrollBar.Visible)
                {
                    _horizontalScrollBar.Visible = false;
                    _xOffset = 0;
                }
            }

            //  PositionScrollBars();
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
                MiscFunctions.SendLog("GetVisibleRowCount: No data available");
                return 0;
            }

            // Use _fullData.Count as the upper limit, not Rows.Count
            int totalHeight = 0;
            int visibleCount = 0;
            for (int i = _dataOffset; i < _fullData.Count; i++)
            {
                // Use Rows[i] if available, fallback to _rowHeight
                int rowHeight = (i < Rows.Count) ? Rows[i].Height : _rowHeight;
                if (rowHeight <= 0)
                {
                    MiscFunctions.SendLog($"GetVisibleRowCount: Invalid height for row {i}, using default {_rowHeight}");
                    rowHeight = _rowHeight;
                }
                totalHeight += rowHeight;
                if (totalHeight > gridRect.Height)
                    break;
                visibleCount++;
            }

            int result = Math.Max(1, visibleCount);
            MiscFunctions.SendLog($"GetVisibleRowCount: gridRect.Height={gridRect.Height}, totalHeight={totalHeight}, visibleCount={result}, _dataOffset={_dataOffset}");
            return result;
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
                _horizontalScrollBar.Width = visibleWidth - (_verticalScrollBar.Visible ? verticalScrollWidth : 0);
            }
        }
        private void VerticalScrollBar_Scroll(object sender, EventArgs e)
        {
            StartSmoothScroll(_verticalScrollBar.Value);
            MoveEditorIn(); // Move editor if active
        }
        private void VerticalScrollBar_ValueChanged(object sender, EventArgs e)
        {
            StartSmoothScroll(_verticalScrollBar.Value);
        }
        private void HorizontalScrollBar_Scroll(object sender, EventArgs e)
        {
            StartSmoothScroll(_dataOffset, _horizontalScrollBar.Value);
            MoveEditorIn(); // Move editor if active
        }
        private void HorizontalScrollBar_ValueChanged(object sender, EventArgs e)
        {
            StartSmoothScroll(_dataOffset, _horizontalScrollBar.Value);


        }
        private void UpdateCellPositions()
        {
            if (Rows == null || Rows.Count == 0)
            {
                MiscFunctions.SendLog("UpdateCellPositions: No rows to update");
                return;
            }

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

            MiscFunctions.SendLog($"UpdateCellPositions: yOffset=0, xOffset={_xOffset}, VisibleRows={Rows.Count}, TotalRows={Rows.Count}");
        }
        private void UpdateRowCount()
        {
            if (_fullData == null) return;
            if (_fullData.Count == 0) return;

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
                        Index = index + i,
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
        #endregion
        #region Resizing Logic
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
              //   MiscFunctions.SendLog($"SetColumnWidth: Column '{columnName}' not found.");
            }
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
        private void BeepGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_resizingColumn && _resizingIndex >= 0)
            {
                int deltaX = e.X - _lastMousePos.X;
                if (_resizingIndex < Columns.Count)
                {
                    Columns[_resizingIndex].Width = Math.Max(20, Columns[_resizingIndex].Width + deltaX);
                }
                _lastMousePos = e.Location;
                UpdateScrollBars();
                Invalidate();
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
                    Invalidate();
                }
            }
            else
            {
                if (IsNearColumnBorder(e.Location, out _resizingIndex))
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else if (IsNearRowBorder(e.Location, out _resizingIndex))
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
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
        private bool IsNearColumnBorder(Point location, out int columnIndex)
        {
            // Calculate the X coordinate relative to the grid's content.
            // gridRect.Left is where the cells start; add XOffset to account for horizontal scrolling.
            int xRelative = location.X - gridRect.Left + XOffset;
            int currentX = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (!Columns[i].Visible)
                    continue;
                currentX += Columns[i].Width;
                // Check if the point is within _resizeMargin pixels of this column’s right edge.
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
            base.OnResize(e);
            _navigatorDrawn=false;
            _filterpaneldrawn = false;
            UpdateDrawingRect();
            UpdateRowCount();
            FillVisibleRows();
            UpdateScrollBars();
           
            Invalidate();
        }
        #endregion
        #region Editor
        private object GetCellValue(object dataItem, BeepColumnConfig column)
        {
            if (dataItem == null || column == null)
            {
                MiscFunctions.SendLog($"GetCellValue: Null dataItem or column");
                return null;
            }

            try
            {
                var wrapper = dataItem as DataRowWrapper;
                object item = wrapper != null ? wrapper.OriginalData : dataItem;

                // Use ColumnName or ColumnCaption to get the property
                string propertyName = !string.IsNullOrEmpty(column.ColumnName) ? column.ColumnName : column.ColumnCaption;
                if (string.IsNullOrEmpty(propertyName))
                {
                    MiscFunctions.SendLog($"GetCellValue: Column {column.Index} has no ColumnName or ColumnCaption");
                    return null;
                }

                PropertyInfo prop = item.GetType().GetProperty(propertyName);
                if (prop != null)
                {
                    return prop.GetValue(item);
                }
                else
                {
                    MiscFunctions.SendLog($"GetCellValue: Property '{propertyName}' not found in data item type {item.GetType().Name}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"GetCellValue: Error retrieving value for column {column.ColumnName ?? column.ColumnCaption}: {ex.Message}");
                return null;
            }
        }
        private void ShowCellEditorIn(BeepCellConfig cell, Point location)
        {
            if (!cell.IsEditable)
                return;

            int colIndex = cell.ColumnIndex;
            string columnName = Columns[colIndex].ColumnName;

            // Close any existing editor
            CloseCurrentEditor();

            _editingCell = cell;
            Size cellSize = new Size(cell.Width, cell.Height);

            // Create or reuse editor control
            _editingControl = CreateCellControlForEditing(cell);
            _editingControl.Size = cellSize;
            _editingControl.Location = new Point(cell.X, cell.Y);
            _editingControl.Theme = Theme;

            UpdateCellControl(_editingControl, Columns[colIndex],cell, cell.CellValue);

            // Attach event handlers
            _editingControl.TabKeyPressed -= Tabhandler;
            _editingControl.TabKeyPressed += Tabhandler;
            _editingControl.EscapeKeyPressed -= Canclehandler;
            _editingControl.EscapeKeyPressed += Canclehandler;

            // Add to grid’s Controls collection
            if (_editingControl.Parent != this)
            {
                this.Controls.Add(_editingControl);
            }
            _editingControl.BringToFront();
            _editingControl.Focus();
            IsEditorShown = true;

          //   MiscFunctions.SendLog($"ShowCellEditor: Cell={cell.X},{cell.Y}, Value={cellSize}");
        }
        private void CloseCurrentEditorIn()
        {
            if (_editingControl != null && IsEditorShown)
            {
                SaveEditedValue();
                if (_editingControl.Parent == this)
                {
                    this.Controls.Remove(_editingControl);
                }
                _editingControl.Dispose();
                _editingControl = null;
            }
            _editingCell = null;
            IsEditorShown = false;
            MiscFunctions.SendLog("Popup editor closed successfully.");
            EditorClosed?.Invoke(this, EventArgs.Empty); // Raise the event
            Invalidate(); // Redraw grid after editor closes
        }
        // One editor control per column (keyed by column index)
        private Dictionary<string, IBeepUIComponent> _columnEditors = new Dictionary<string, IBeepUIComponent>();
        // The currently active editor and cell being edited
        private BeepControl _editingControl = null;
        private BeepCellConfig _editingCell = null;
        private IBeepComponentForm _editorPopupForm;
        private void ShowCellEditor(BeepCellConfig cell, Point location)
        {
            if (!cell.IsEditable)
                return;

            int colIndex = cell.ColumnIndex;
            string columnName = Columns[colIndex].ColumnName;
            if(_editingControl!=null) 

            // Close any existing editor before opening a new one
            CloseCurrentEditor();

            _editingCell = cell;
            Size cellSize = new Size(cell.Width, cell.Height);
            // Create the popup form if it's null
            if (_editorPopupForm == null)
            {
                _editorPopupForm = new IBeepComponentForm
                {
                    FormBorderStyle = FormBorderStyle.None,
                    StartPosition = FormStartPosition.Manual,
                    ShowInTaskbar = false,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    TopMost = false,
                    MinimumSize = cellSize, // Force exact size
                    MaximumSize = cellSize
                };

            }
            // Apply size
            _editorPopupForm.Size = cellSize;
            _editorPopupForm.ClientSize = cellSize;
            // Retrieve or create editor control
            _editingControl = CreateCellControlForEditing(cell);
                
           

            // Ensure the control is only added once
            if (_editingControl.Parent != _editorPopupForm)
            {
                _editorPopupForm.AddComponent(_editingControl);
            }
            _editingControl.Theme = Theme;
          
            // **🔹 Force the popup form and editor control to match the exact cell size**
            // _editingControl.IsFrameless = true;
            //   _editingControl.ShowAllBorders = false;
        //    MiscFunctions.SendLog($"Cell size: {cellSize}");
            _editorPopupForm.ClientSize = cellSize; // 👈 Ensures no extra space due to window frame
            _editingControl.Size = cellSize; // 👈 Matches cell exactly
            _editingControl.Dock = DockStyle.Fill; // 👈 Ensures it doesn't resize incorrectly
          //  MiscFunctions.SendLog($"Editor size: {_editingControl.Value}");
         //   MiscFunctions.SendLog($"Popup size: {_editorPopupForm.ClientSize}");
            // **🔹 Position popup exactly at the cell location (relative to BeepSimpleGrid)**
            Point screenLocation = this.PointToScreen(new Point(cell.X, cell.Y));
            _editorPopupForm.Location = screenLocation;
            _editingControl.Theme = Theme;
            // **Set initial text**
            //  _editingControl.Text = cell.CellValue;
          
            UpdateCellControl(_editingControl, Columns[colIndex], cell, cell.CellValue);
            // 🔹 Confirm value in editor **after setting**

         //   MiscFunctions.SendLog($"🔄 Setting BeepTextBox text before popupform is Show: {_editingControl.Text}");
          //  _editorPopupForm.SetValue(cell.CellValue);
          
            _editingControl.TabKeyPressed -= Tabhandler;
             _editingControl.TabKeyPressed += Tabhandler;
            _editingControl.EscapeKeyPressed -= Canclehandler;
            _editingControl.EscapeKeyPressed += Canclehandler;
           //    _editingControl.LostFocus -= LostFocusHandler;
           //   _editingControl.LostFocus += LostFocusHandler;
            _editorPopupForm.Show(this);
            //Task.Delay(50).ContinueWith(t =>
            //{
            //    _editorPopupForm.SetValue(cell.CellValue);
            // //   MiscFunctions.SendLog($"✅ After popup is fully visible, setting text: {_editingControl.Text}");
            //}, TaskScheduler.FromCurrentSynchronizationContext());
            _editorPopupForm.SetValue(cell.CellValue);
            _editingControl.Focus();
            IsEditorShown = true;
           // MiscFunctions.SendLog($"✅ after popform Show the Editor BeepTextBox text   Show : {_editingControl.Text}");
        }
        private void LostFocusHandler(object? sender, EventArgs e)
        {
            CloseCurrentEditorIn();
        }
        private void Canclehandler(object? sender, EventArgs e)
        {
            CancelEditing();
        }
        private void Tabhandler(object? sender, EventArgs e)
        {
           MoveNextCell();
        }
        private void CloseCurrentEditor()
        {
            if (_editingControl != null)
            {
                // Save the current value before closing
              //  _editingCell.IsEditable = false;
           //     MiscFunctions.SendLog($"✅ before closing Editor Text After Update: {_editingControl.Text}");

                SaveEditedValue();
             //   MiscFunctions.SendLog($"✅ After Save Text After : {_editingControl.Text}");
                // Remove editor control from popup (prevents premature disposal)
                if (_editorPopupForm != null && _editingControl.Parent == _editorPopupForm)
                {
                    _editorPopupForm.Controls.Remove(_editingControl);
                }

                // Close and dispose of the popup form safely
                if (_editorPopupForm != null)
                {
                    _editorPopupForm.Close();
                    _editorPopupForm.Dispose();
                    _editorPopupForm = null;
                }

                // Reset references (but don't dispose `_editingControl` yet)
                _editingCell = null;
                 _editingControl = null;

             //   MiscFunctions.SendLog("Popup editor closed successfully.");
            }
            MiscFunctions.SendLog("Popup editor closed successfully.");
            EditorClosed?.Invoke(this, EventArgs.Empty); // Raise the event
            IsEditorShown = false;
        }
        private void SaveEditedValue()
        {
            if ( _editingCell == null || _editingControl==null)
            {
           //     MiscFunctions.SendLog($"⚠️ Editing control or cell is null!");
                return;
            }

            object newValue = _editingControl.GetValue();
          //  MiscFunctions.SendLog($"🔄 Saving value: {newValue} (Old: {_editingCell.CellData})");

            // 🔹 Check if the new value is empty or null
            if (newValue == null || string.IsNullOrWhiteSpace(newValue.ToString()))
            {
             //   MiscFunctions.SendLog($"⚠️ New value is empty. Skipping update.");
                return;
            }

            // 🔹 Retrieve PropertyType from the corresponding column
            BeepColumnConfig columnConfig = Columns.FirstOrDefault(c => c.Index == _editingCell.ColumnIndex);
            if (columnConfig == null)
            {
            //    MiscFunctions.SendLog($"⚠️ Column config not found. Skipping update.");
                return;
            }
            Type propertyType = Type.GetType(columnConfig.PropertyTypeName, throwOnError: false) ?? typeof(string); // Default to string if null

             //🔹 Convert new value to the correct type before comparing
            object convertedNewValue = MiscFunctions.ConvertValueToPropertyType(propertyType, newValue);
            object convertedOldValue = MiscFunctions.ConvertValueToPropertyType(propertyType, _editingCell.CellData);

            // 🔹 Skip update if the new value is the same as the old value
            if (convertedNewValue != null && convertedOldValue != null && convertedNewValue.Equals(convertedOldValue))
            {
            //    MiscFunctions.SendLog($"⚠️ New value is the same as old. Skipping update.");
                return;
            }

            // 🔹 Update cell's stored value
            _editingCell.OldValue = _editingCell.CellData;
            _editingCell.CellData =  convertedNewValue;
            _editingCell.CellValue = convertedNewValue;
            _editingCell.IsDirty = true;

            // 🔹 Update the corresponding data record
            UpdateDataRecordFromRow(_editingCell);
            CellValueChanged?.Invoke(this, new BeepCellEventArgs(_editingCell));
            // 🔹 Trigger validation if necessary
            _editingCell.ValidateCell();

           // MiscFunctions.SendLog($"✅ Cell updated. New: {_editingCell.CellValue}");

            Invalidate(); // 🔹 Redraw grid if necessary
        }
        private void CancelEditing()
        {
            // Optionally, revert the editor's value if needed.
            CloseCurrentEditorIn();
        }
        private void BeepGrid_MouseClick(object sender, MouseEventArgs e)
        {
            var clickedCell = GetCellAtLocation(e.Location);
            if (clickedCell == null) return;
           // if (clickedCell.IsAggregation) return;
            if (clickedCell != null)
            {
                int colIndex = clickedCell.ColumnIndex;
                if (Columns[colIndex].IsSelectionCheckBox && (_showCheckboxes || _showSelection))
                {
                    int rowIndex = clickedCell.RowIndex;
                    if (rowIndex >= 0 && rowIndex < Rows.Count)
                    {
                        int dataIndex = _dataOffset + rowIndex;
                        var dataItem = _fullData[dataIndex] as DataRowWrapper;
                        int rowID = dataItem.RowID;

                        bool isSelected = (bool)(clickedCell.CellValue ?? false);
                        _persistentSelectedRows[rowID] = !isSelected;
                        clickedCell.CellValue = !isSelected;
                        clickedCell.CellData = !isSelected;

                        //MiscFunctions.SendLog($"BeepGrid_MouseClick: Row {rowIndex}, DataIndex {dataIndex}, RowID {rowID}, Sel = {!isSelected}, _persistentSelectedRows.Count = {_persistentSelectedRows.Count}");
                        Invalidate();
                        RaiseSelectedRowsChanged();
                    }
                    return;
                }
            }

            _selectedCell = clickedCell;
            if (_editingCell != null && clickedCell != null && _editingCell.Id == clickedCell.Id)
            {
                SaveEditedValue();
                return;
            }
            else
            {
                if (_columns == null) return;
                CloseCurrentEditorIn();
                if (_selectedCell != null)
                {
                    _editingCell = _selectedCell;
                    SelectCell(_selectedCell);
                    if (DataNavigator != null && _currentRow != null && _currentRow.DisplayIndex >= 0)
                    {
                        DataNavigator.BindingSource.Position = _currentRow.DisplayIndex;
                    }
                    if (_columns == null) return;
                    if (!_columns[_selectedCell.ColumnIndex].ReadOnly && _columns[_selectedCell.ColumnIndex].CellEditor!= BeepColumnType.Image  )
                    {
                        ShowCellEditorIn(_selectedCell, e.Location);
                    }
                }
            }
        }
        #endregion Editor
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
                  //  MiscFunctions.SendLog($"UpdateTrackingIndices: Invalid OriginalIndex {tracking.OriginalIndex} for Tracking {tracking.UniqueId}, marking for removal");
                    staleTrackings.Add(tracking);
                    continue;
                }

                // Get the original DataRowWrapper from originalList
                var originalWrapped = originalList[tracking.OriginalIndex] as DataRowWrapper;
                if (originalWrapped == null)
                {
                 //   MiscFunctions.SendLog($"UpdateTrackingIndices: No DataRowWrapper found at OriginalIndex {tracking.OriginalIndex} for Tracking {tracking.UniqueId}, marking for removal");
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
                          //  MiscFunctions.SendLog($"UpdateTrackingIndices: Tracking {tracking.UniqueId}, OriginalIndex {tracking.OriginalIndex}, Updated CurrentIndex to {currentIndex}, RowState {wrappedItem.RowState}");
                        }
                    }
                }
                else
                {
                    // Record not found in _fullData, mark Tracking for removal
                 //   MiscFunctions.SendLog($"UpdateTrackingIndices: Record for Tracking {tracking.UniqueId} not found in _fullData, marking for removal");
                    staleTrackings.Add(tracking);
                }
            }

            // Remove stale Tracking entries
            foreach (var staleTracking in staleTrackings)
            {
                Trackings.Remove(staleTracking);
              //  MiscFunctions.SendLog($"UpdateTrackingIndices: Removed stale Tracking {staleTracking.UniqueId}");
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
                        MiscFunctions.SendLog($"Selected row moved {direction}: OldRowIndex={oldRowIndex}, NewRowIndex={newRowIndex}, DisplayIndex={_currentRow.DisplayIndex}");
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

          //  MiscFunctions.SendLog($"SyncSelectedRowIndexAndEditor: selectedDataIndex={selectedDataIndex}, newRowIndex={newRowIndex}, _currentRowIndex={_currentRowIndex}");
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
            base.ApplyTheme();
            this.BackColor = _currentTheme.GridBackColor;
            this.ForeColor = _currentTheme.GridForeColor;
            if (titleLabel != null)
            {
                titleLabel.Theme = Theme;
            }
            if (DataNavigator != null)
            {
                _currentTheme.ButtonBackColor = _currentTheme.GridBackColor;
                _currentTheme.ButtonForeColor = _currentTheme.GridForeColor;
                DataNavigator.Theme = Theme;
            }
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
            Invalidate();
        }
        #endregion
        #region BindingSource Management
      
        private BindingSource _bindingSource;
        private void AssignBindingSource(BindingSource bindingSrc)
        {
            // MiscFunctions.SendLog($"AssignBindingSource: BindingSource Detected, DataSource = {bindingSrc?.DataSource?.GetType()}, DataMember = {bindingSrc?.DataMember}");
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
          //  MiscFunctions.SendLog($"BindingSource_ListChanged: Type={e.ListChangedType}, NewIndex={e.NewIndex}, OldIndex={e.OldIndex}, _fullData.Count={_fullData?.Count}");

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
            if (newIndex < 0 || _bindingSource == null || newIndex >= _bindingSource.Count) return;

            var newItem = _bindingSource[newIndex];
            var wrapper = new DataRowWrapper(newItem, newIndex)
            {
                TrackingUniqueId = Guid.NewGuid(),
                RowState = DataRowState.Added,
                DateTimeChange = DateTime.Now
            };

            // Insert into _fullData at the correct position
            if (newIndex <= _fullData.Count)
            {
                _fullData.Insert(newIndex, wrapper);
            }
            else
            {
                _fullData.Add(wrapper);
            }

            // Update indices in _fullData
            for (int i = 0; i < _fullData.Count; i++)
            {
                var dataRow = _fullData[i] as DataRowWrapper;
                if (dataRow != null)
                {
                    dataRow.RowID = i;
                }
            }

            // Ensure tracking for the new item
            EnsureTrackingForItem(wrapper);

            // Adjust _dataOffset if needed
            if (_dataOffset > newIndex)
            {
                _dataOffset++;
            }

            UpdateRowCount();
        }
        private void HandleItemChanged(int index)
        {
            if (index < 0 || index >= _fullData.Count || _bindingSource == null || index >= _bindingSource.Count) return;

            var updatedItem = _bindingSource[index];
            var wrapper = _fullData[index] as DataRowWrapper;
            if (wrapper != null)
            {
                wrapper.OriginalData = updatedItem;
                wrapper.RowState = DataRowState.Modified;
                wrapper.DateTimeChange = DateTime.Now;

                // Ensure tracking for the updated item
                EnsureTrackingForItem(wrapper);
            }
        }
        private void HandleItemDeleted(int index)
        {
            if (index < 0 || index >= _fullData.Count) return;

            var wrapper = _fullData[index] as DataRowWrapper;
            if (wrapper != null)
            {
                wrapper.RowState = DataRowState.Deleted;
                wrapper.DateTimeChange = DateTime.Now;

                // Ensure tracking for the deleted item
                EnsureTrackingForItem(wrapper);

                // Update Trackings and originalList
                var tracking = Trackings.FirstOrDefault(t => t.OriginalIndex == index);
                if (tracking != null)
                {
                    tracking.EntityState = EntityState.Deleted;
                    Trackings.Remove(tracking);
                }
                originalList.RemoveAt(index);
            }

            // Remove from _fullData
            _fullData.RemoveAt(index);

            // Update indices in _fullData
            for (int i = 0; i < _fullData.Count; i++)
            {
                var dataRow = _fullData[i] as DataRowWrapper;
                if (dataRow != null)
                {
                    dataRow.RowID = i;
                }
            }

            // Adjust _dataOffset if needed
            if (_dataOffset > index)
            {
                _dataOffset--;
            }

            UpdateRowCount();
        }
        private void HandleItemMoved(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= _fullData.Count || newIndex < 0 || newIndex >= _fullData.Count) return;

            var item = _fullData[oldIndex];
            _fullData.RemoveAt(oldIndex);
            _fullData.Insert(newIndex, item);

            // Update indices in _fullData
            for (int i = 0; i < _fullData.Count; i++)
            {
                var dataRow = _fullData[i] as DataRowWrapper;
                if (dataRow != null)
                {
                    dataRow.RowID = i;
                    // Ensure tracking for the moved item
                    EnsureTrackingForItem(dataRow);
                }
            }

            // Update Trackings to match the new order
            var tracking = Trackings.FirstOrDefault(t => t.OriginalIndex == oldIndex);
            if (tracking != null)
            {
                tracking.OriginalIndex = newIndex;
                tracking.CurrentIndex = newIndex;
            }

            // Adjust _dataOffset if needed
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
        #region Drawing Navigator
        public event EventHandler CallPrinter;
        public event EventHandler SendMessage;
        public event EventHandler ShowSearch;
        public event EventHandler NewRecordCreated;
        public event EventHandler SaveCalled;
        public event EventHandler DeleteCalled;
        public event EventHandler EditCalled;
        public bool VerifyDelete = false;
        private BeepLabel Recordnumberinglabel1;
        private BeepButton FindButton, NewButton, EditButton, PreviousButton, NextButton, RemoveButton, RollbackButton, SaveButton, PrinterButton, MessageButton;
        private int spacing = 5; // Spacing between buttons
        private int labelWidth = 100; // Width of the label
        private Size buttonSize = new Size(16, 16);
        private List<Control> buttons=new List<Control>();
        Panel MainPanel;
        private void DrawNavigationRow(Graphics g, Rectangle rect)
        {
           // DataNavigator.Location = new Point(rect.Left + 1, rect.Top + 1);
           // DataNavigator.Size = new Size(rect.Width - 2, rect.Height - 2);
            //  DataNavigator.Invalidate();
            MainPanel.Location = new Point(rect.Left + 1, rect.Top + 1);
            MainPanel.Size = new Size(rect.Width - 2, rect.Height - 2);
            PositionControls( spacing);
          //  MainPanel.Invalidate();
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            }
        }
        private void CreateNavigationButtons()
        {
          

            MainPanel = new Panel
            {
                //Dock = DockStyle.Fill,
                
            };
         
           // MainPanel.Bounds = DrawingRect;

            FindButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.search_1.svg", buttonSize, FindpictureBox_Click);
            EditButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.pencil.svg", buttonSize, EditpictureBox_Click);
            PrinterButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.print1.svg", buttonSize, PrinterpictureBox_Click);
            MessageButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.mail.svg", buttonSize, MessagepictureBox_Click);
            SaveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.check.svg", buttonSize, SavepictureBox_Click);
            PreviousButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.backwards.svg", buttonSize, PreviouspictureBox_Click);
            Recordnumberinglabel1 = new BeepLabel
            {
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(labelWidth, buttonSize.Height),
                Text = "0",
                ShowAllBorders = true,
                IsRounded = false,
                IsChild = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsRoundedAffectedByTheme = false
            };
            NextButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.forward.svg", buttonSize, NextpictureBox_Click);
            NewButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.plus.svg", buttonSize, NewButton_Click);
            RemoveButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.minus.svg", buttonSize, RemovepictureBox_Click);
            RollbackButton = CreateButton("TheTechIdea.Beep.Winform.Controls.GFX.SVG.go-back.svg", buttonSize, RollbackpictureBox_Click);

             buttons = new List<Control>
            {
                FindButton, EditButton, PrinterButton, MessageButton, SaveButton,
                PreviousButton, Recordnumberinglabel1, NextButton, NewButton, RemoveButton, RollbackButton
            };
           Controls.Add(MainPanel);
           

        }
        private BeepButton CreateButton(string imagePath, Size size, EventHandler clickHandler)
        {
            var button = new BeepButton
            {
                ImagePath = imagePath,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFrameless = true,
                Size = size,
                IsChild = true,
                Anchor = AnchorStyles.None,
                Margin = new Padding(0),
                Padding = new Padding(0),
                MaxImageSize = new Size(size.Width - 1, size.Height - 1)
            };
            button.Click += clickHandler;
            return button;
        }
        private void PositionControls( int spacing)
        {
            if (MainPanel == null || buttons == null || buttons.Count == 0) return;

            int totalWidth = buttons.Sum(c => c.Width) + spacing * (buttons.Count - 1);
            int startX = (MainPanel.Width - totalWidth) / 2;
            int currentX = startX;
            int centerY = (MainPanel.Height - buttons[0].Height) / 2;

            foreach (var control in buttons)
            {
                if (!MainPanel.Controls.Contains(control))
                {
                    MainPanel.Controls.Add(control);
                }
                control.Left = currentX;
                control.Top = centerY;
                currentX += control.Width + spacing;
            }
        }

        private void UpdateRecordNumber()
        {
            if (Recordnumberinglabel1 == null) return;

            if (_fullData != null && _fullData.Any())
            {
                int position = _currentRowIndex + _dataOffset + 1; // Position is based on _currentRowIndex and _dataOffset
                Recordnumberinglabel1.Text = $"{position} From {_fullData.Count}";
            }
            else
            {
                Recordnumberinglabel1.Text = "-";
            }
        }

        private void UpdateNavigationButtonState()
        {
            if (_fullData == null)
            {
                PreviousButton.Enabled = false;
                NextButton.Enabled = false;
                RemoveButton.Enabled = false;
                SaveButton.Enabled = false;
                return;
            }

            int position = _currentRowIndex + _dataOffset; // Current position in _fullData
            PreviousButton.Enabled = position > 0;
            NextButton.Enabled = position < _fullData.Count - 1;
            RemoveButton.Enabled = _fullData.Count > 0;
            SaveButton.Enabled = _fullData.Count > 0;
        }

        private void SavepictureBox_Click(object sender, EventArgs e)
        {
            try
            {
           //     MiscFunctions.SendLog("Save Record");
                SaveCalled?.Invoke(sender, null); // Remove _bindingSource from the event
                MessageBox.Show(Parent, "Record Saved", "Beep", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Binding Navigator {ex.Message}");
                MessageBox.Show(Parent, ex.Message, "Beep", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            try
            {
              //  MiscFunctions.SendLog("Add New Record");

                if (_dataSource is BindingSource)
                {
                    CreateNewRecordForBindingSource();
                }
                else if (_dataSource is IList)
                {
                    CreateNewRecordForIList();
                }
                else
                {
               //     MiscFunctions.SendLog("NewButton_Click: Unsupported data source type");
                    return;
                }

                // Update UI
                UpdateRecordNumber();
                UpdateNavigationButtonState();

                NewRecordCreated?.Invoke(this, EventArgs.Empty); // Remove _bindingSource from the event
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void RemovepictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                MiscFunctions.SendLog("Remove Record");
                DeleteCalled?.Invoke(sender, EventArgs.Empty); // Remove _bindingSource from the event

                if (_fullData != null && _fullData.Any() && _currentRow != null)
                {
                    if (VerifyDelete && MessageBox.Show(Parent, "Are you sure you want to Delete Record?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                        return;

                    int dataIndex = _currentRow.DisplayIndex;
                    if (dataIndex >= 0 && dataIndex < _fullData.Count)
                    {
                        var item = _fullData[dataIndex];
                        var wrapper = item as DataRowWrapper;
                        if (wrapper != null)
                        {
                            if (_dataSource is BindingSource && _bindingSource != null)
                            {
                                // For BindingSource, use _bindingSource.RemoveCurrent()
                                int bsIndex = _bindingSource.IndexOf(wrapper.OriginalData);
                                if (bsIndex >= 0 && bsIndex < _bindingSource.Count)
                                {
                                    _bindingSource.RemoveCurrent(); // This will trigger BindingSource_ListChanged
                                    MiscFunctions.SendLog($"Remove: Removed record from BindingSource at index {bsIndex}");
                                }
                                else
                                {
                                    MiscFunctions.SendLog($"Remove: Item not found in BindingSource at index {bsIndex}");
                                    return;
                                }
                            }
                            else if (_dataSource is IList)
                            {
                                // For IList, use DeleteFromDataSource
                                DeleteFromDataSource(wrapper.OriginalData);
                            }
                            else
                            {
                                MiscFunctions.SendLog($"Remove: Unsupported data source type: {_dataSource?.GetType()}");
                                return;
                            }

                            // Update UI
                            UpdateRecordNumber();
                            UpdateNavigationButtonState();

                            // Update selection and scroll position
                            if (_fullData.Any())
                            {
                                int newSelectedIndex = Math.Min(dataIndex, _fullData.Count - 1);
                                StartSmoothScroll(Math.Max(0, newSelectedIndex - GetVisibleRowCount() + 1));
                                if (newSelectedIndex >= _dataOffset && newSelectedIndex < _dataOffset + Rows.Count)
                                {
                                    _currentRowIndex = newSelectedIndex - _dataOffset;
                                    _currentRow = Rows[_currentRowIndex];
                                    SelectCell(_currentRowIndex, _selectedColumnIndex >= 0 ? _selectedColumnIndex : 0);
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void NextpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                MiscFunctions.SendLog("Next Record");
                MoveNextRow();

                // Update UI
                UpdateRecordNumber();
                UpdateNavigationButtonState();

                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void PreviouspictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                MiscFunctions.SendLog("Previous Record");
                MovePreviousRow();

                // Update UI
                UpdateRecordNumber();
                UpdateNavigationButtonState();

                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Binding Navigator {ex.Message}");
            }
        }

        private void RollbackpictureBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(Parent, "Are you sure you want to cancel Changes?", "Beep", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    // Reset the current row to its original state
                    if (_currentRow != null)
                    {
                        int dataIndex = _currentRow.DisplayIndex;
                        if (dataIndex >= 0 && dataIndex < _fullData.Count)
                        {
                            var wrapper = _fullData[dataIndex] as DataRowWrapper;
                            if (wrapper != null)
                            {
                                // Reset the row’s cells to their original values
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

                                // Ensure tracking for the reset item
                                EnsureTrackingForItem(wrapper);
                            }
                        }
                    }

                    // Update UI
                    UpdateRecordNumber();
                    UpdateNavigationButtonState();

                    FillVisibleRows();
                    UpdateScrollBars();
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"Binding Navigator {ex.Message}");
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
        }
        private void MessagepictureBox_Click(object sender, EventArgs e)
        {
            SendMessage?.Invoke(this, EventArgs.Empty);
        }
        #endregion
        #region Helper Methods
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
            //    MiscFunctions.SendLog("MapPropertyTypeToDbFieldCategory: propertyTypeName is null or empty, defaulting to String");
                return DbFieldCategory.String;
            }

            // Attempt to resolve the Type from the AssemblyQualifiedName
            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
            {
             //   MiscFunctions.SendLog($"MapPropertyTypeToDbFieldCategory: Could not resolve type '{propertyTypeName}', defaulting to String");
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
          //  MiscFunctions.SendLog($"MapPropertyTypeToDbFieldCategory: Unknown type '{type.FullName}', defaulting to String");
            return DbFieldCategory.String;
        }
        private BeepColumnType MapPropertyTypeToCellEditor(string propertyTypeName)
        {
            // Default to Text if typeName is null or empty
            if (string.IsNullOrWhiteSpace(propertyTypeName))
            {
            //    MiscFunctions.SendLog("MapPropertyTypeToCellEditor: propertyTypeName is null or empty, defaulting to Text");
                return BeepColumnType.Text;
            }

            // Attempt to resolve the Type from the AssemblyQualifiedName
            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
            {
              //  MiscFunctions.SendLog($"MapPropertyTypeToCellEditor: Could not resolve type '{propertyTypeName}', defaulting to Text");
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
          //  MiscFunctions.SendLog($"MapPropertyTypeToCellEditor: Unknown type '{type.FullName}', defaulting to Text");
            return BeepColumnType.Text;
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
                if (_editorPopupForm != null)
                {
                    _editorPopupForm.Dispose();
                    _editorPopupForm = null;
                }
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
            }
        }
    }
}