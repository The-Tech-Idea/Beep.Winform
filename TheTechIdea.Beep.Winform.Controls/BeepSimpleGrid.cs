﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Desktop.Common.Helpers;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Shared;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis.Modules;

using Timer = System.Windows.Forms.Timer;



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
        private Dictionary<int, bool> _persistentSelectedRows = new Dictionary<int, bool>(); // Add this field to track selection by RowID
        private object _dataSource;
        object finalData;
        private List<object> _fullData;
        private int _dataOffset = 0;
        public IEntityStructure Entity { get; set; }

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
                _dataSource = value;
                DataSetup();
                InitializeData();
                FillVisibleRows();
                UpdateScrollBars();
                Invalidate();
            }
        }
        public string QueryFunctionName { get; set; }
        public string QueryFunction { get; set; }
        #endregion "Data Source Properties"
        #region "Appearance Properties"
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
        // [Editor(typeof(BeepGridColumnConfigCollectionEditor), typeof(UITypeEditor))] // Uncomment if you have a custom editor
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public List<BeepColumnConfig> Columns
        {
            get
            {
                //   Debug.WriteLine($"Columns Getter: Count = {_columns.Count}, DesignMode = {DesignMode}");
                return _columns;
            }
            set
            {
                //   Debug.WriteLine($"Columns Setter: Count = {value?.Count}, DesignMode = {DesignMode}");
                if (value != null && value != _columns)
                {
                    _columns = value;
                    if (_columns.Any())
                    {
                        columnssetupusingeditordontchange = true;
                        //     Debug.WriteLine("Columns set via property, marking as designer-configured");
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
        
        public BindingList<BeepRowConfig> Rows { get; set; } = new BindingList<BeepRowConfig>();
        public BeepRowConfig aggregationRow { get; set; }
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

        private List<BeepColumnConfig> _columns = new List<BeepColumnConfig>();


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
            DataNavigator = new BeepBindingNavigator
            {
                ShowAllBorders = false,
                ShowShadow = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                Theme = Theme,

            };
            AttachNavigatorEvents();
            _scrollTimer = new Timer { Interval = 16 }; // ~60 FPS for smooth animation
            _scrollTimer.Tick += ScrollTimer_Tick;
            ApplyThemeToChilds = false;
            Controls.Add(DataNavigator);

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
            _fullData = DataNavigator.BindingSource.DataSource as List<object> ?? new List<object>();
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
            //    Debug.WriteLine("CallPrinter triggered - implement printing logic here.");
                // Example: Print the current _fullData
                // You could raise an event or call a printing method
                MessageBox.Show("Printing functionality not implemented yet.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
             //   Debug.WriteLine($"CallPrinter Error: {ex.Message}");
                SendLog($"Error in printing: {ex.Message}");
            }
        }

        private void DataNavigator_SendMessage(object sender, BindingSource bs)
        {
            try
            {
                // Placeholder for sending/sharing functionality
              //  Debug.WriteLine("SendMessage triggered - implement sharing logic here.");
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
             //   Debug.WriteLine($"SendMessage Error: {ex.Message}");
                SendLog($"Error in sending message: {ex.Message}");
            }
        }

        private void DataNavigator_ShowSearch(object sender, BindingSource bs)
        {
            try
            {
                // Placeholder for search/filter functionality
             //   Debug.WriteLine("ShowSearch triggered - implement filter UI here.");
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
            //    Debug.WriteLine($"ShowSearch Error: {ex.Message}");
                SendLog($"Error in search: {ex.Message}");
            }
        }

        private void DataNavigator_NewRecordCreated(object sender, BindingSource bs)
        {
            try
            {
                if (Entity != null)
                {
                    var newItem = Activator.CreateInstance(Type.GetType(Entity.EntityName));
                    _fullData.Add(newItem);
                    Tracking newTracking = new Tracking(Guid.NewGuid(), originalList.Count, _fullData.Count - 1)
                    {
                        EntityState = EntityState.Added
                    };
                    originalList.Add(newItem);
                    Trackings.Add(newTracking);

                    // Notify BindingSource
                    bs.ResetBindings(false);
                    bs.Position = bs.Count - 1;

                    int newOffset = Math.Max(0, _fullData.Count - GetVisibleRowCount());
                    StartSmoothScroll(newOffset);
                    FillVisibleRows();

                    int newRowIndex = _fullData.Count - 1 - _dataOffset;
                    if (newRowIndex >= 0 && newRowIndex < Rows.Count)
                    {
                        SelectCell(newRowIndex, 0);
                        BeginEdit();
                    }
                }
                else
                {
                    Debug.WriteLine("Cannot add new record: Entity structure not defined.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"NewRecordCreated Error: {ex.Message}");
                SendLog($"Error adding new record: {ex.Message}");
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
              //  Debug.WriteLine("Data saved locally in grid.");
                MessageBox.Show("Changes saved locally. Implement external persistence if needed.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveCalled Error: {ex.Message}");
                SendLog($"Error saving data: {ex.Message}");
            }
        }

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
                        Tracking tracking = GetTrackingItem(item);
                        if (tracking != null)
                        {
                            tracking.EntityState = EntityState.Deleted;
                            deletedList.Add(item);
                            _fullData.RemoveAt(dataIndex);

                            // Notify BindingSource
                            bs.ResetBindings(false);

                            if (_fullData.Any())
                            {
                                int newSelectedIndex = Math.Min(dataIndex, _fullData.Count - 1);
                                bs.Position = newSelectedIndex;
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
                                bs.Position = -1;
                                FillVisibleRows();
                            }

                            if (IsLogging)
                            {
                                UpdateLog[DateTime.Now] = new EntityUpdateInsertLog
                                {
                                    TrackingRecord = tracking,
                                    LogAction = LogAction.Delete
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DeleteCalled Error: {ex.Message}");
                SendLog($"Error deleting record: {ex.Message}");
            }
        }

        private void DataNavigator_EditCalled(object sender, BindingSource bs)
        {
            try
            {
                if (_selectedCell != null)
                {
                    BeginEdit();
                 //   Debug.WriteLine($"EditCalled: Editing cell at row {_selectedCell.RowIndex}, column {_selectedCell.ColumnIndex}");
                }
                else
                {
                //    Debug.WriteLine("EditCalled: No cell selected to edit.");
                    MessageBox.Show("Please select a cell to edit.", "BeepSimpleGrid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditCalled Error: {ex.Message}");
                SendLog($"Error starting edit: {ex.Message}");
            }
        }
     
        private void SendLog(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
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
        private void InsertIntoDataSource(object newData)
        {
            // Example: Insert into a database using IDBConnection
            //if (Entity != null && DataSource is BindingSource bs && bs.DataSource is IDBConnection connection)
            //{
            //    // Implement insert logic (e.g., using connection.ExecuteInsert)
            //    Debug.WriteLine($"Inserted new record: {newData}");
            //}
        }

        private void UpdateInDataSource(object originalData, Dictionary<string, object> changes)
        {
            //// Example: Update in a database using IDBConnection
            //if (Entity != null && DataSource is BindingSource bs && bs.DataSource is IDBConnection connection)
            //{
            //    // Implement update logic (e.g., using connection.ExecuteUpdate with originalData and changes)
            //    Debug.WriteLine($"Updated record: Original={originalData}, Changes={string.Join(", ", changes.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
            //}
        }

        private void DeleteFromDataSource(object originalData)
        {
            // Example: Delete from a database using IDBConnection
            //if (Entity != null && DataSource is BindingSource bs && bs.DataSource is IDBConnection connection)
            //{
            //    // Implement delete logic (e.g., using connection.ExecuteDelete)
            //    Debug.WriteLine($"Deleted record: {originalData}");
            //}
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
                Debug.WriteLine($"UpdateDataRecordFromRow: Failed to retrieve RowID for row at index {editingCell.RowIndex}");
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
                Debug.WriteLine($"UpdateDataRecordFromRow: Could not find record with RowID {rowID} in _fullData");
                return;
            }

            object originalData = dataItem.OriginalData;
            var originalWrappedItem = GetItemFromOriginalList(GetOriginalIndex(dataItem)) as DataRowWrapper;
            object originalItem = originalWrappedItem?.OriginalData;

            bool hasChanges = false;
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
                                        var changes = GetChangedFields(originalWrappedItem, dataItem);
                                        foreach (var kvp in changes)
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

            // Notify the data navigator of the change
            if (DataNavigator != null && hasChanges)
            {
                DataNavigator.BindingSource.ResetBindings(false); // Notify navigator of item changes
            }

            // Refresh the grid to reflect the updated data
            if (hasChanges)
            {
                Invalidate();
            }
        }
        private void SaveToDataSource()
        {
            try
            {
                if (_fullData == null || originalList == null || DataSource == null)
                {
                    Debug.WriteLine("SaveToDataSource: _fullData, originalList, or DataSource is null. Cannot sync changes.");
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
                                     //   Debug.WriteLine($"Saving Added Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                                      //  Debug.WriteLine($"Saving Modified Record - RowID: {wrappedItem.RowID}, OriginalIndex: {originalIndex}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                             //   Debug.WriteLine($"Saving Deleted Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                                 //       Debug.WriteLine($"Saving Added Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                                 //      Debug.WriteLine($"Saving Modified Record - RowID: {wrappedItem.RowID}, OriginalIndex: {originalIndex}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                          //      Debug.WriteLine($"Saving Deleted Record - RowID: {wrappedItem.RowID}, TrackingUniqueId: {wrappedItem.TrackingUniqueId}");
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
                    Debug.WriteLine("SaveToDataSource: DataSource is neither BindingSource nor IList. Cannot sync changes.");
                    return;
                }

                // Update the grid state
                UpdateScrollBars();
                Invalidate();

                Debug.WriteLine("SaveToDataSource: All changes processed and synchronized with DataSource.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SaveToDataSource Error: {ex.Message}");
                SendLog($"Error saving to data source: {ex.Message}");
            }
        }
        #endregion DataSource Update
        #region Initialization
        private void DataSetup()
        {
            IEntityStructure entity = null;
            object resolvedData = null; // Unified variable for final data or type resolution
          //  Debug.WriteLine($"DataSetup Started: _dataSource Type = {_dataSource?.GetType()}, DesignMode = {DesignMode}, Columns Count = {_columns.Count}");

            // Step 1: Handle different _dataSource types
            if (_dataSource == null)
            {
               // Debug.WriteLine("DataSource is null, no entity generated");
            }
            else if (_dataSource is BindingSource bindingSrc)
            {
              //  Debug.WriteLine($"BindingSource Detected: DataSource = {bindingSrc.DataSource?.GetType()}, DataMember = {bindingSrc.DataMember}");
                var dataSource = bindingSrc.DataSource;

                if (dataSource == null)
                {
                 //   Debug.WriteLine("BindingSource.DataSource is null, checking BindingSource.List");
                    resolvedData = bindingSrc.List; // Could be IList, DataView, etc.
                }
                else if (dataSource is Type type)
                {
                    // Handle Type (design-time or runtime)
                  //  Debug.WriteLine($"{(DesignMode ? "Design-time" : "Runtime")}: DataSource is Type = {type.FullName}");
                    if (!string.IsNullOrEmpty(bindingSrc.DataMember))
                    {
                        PropertyInfo dataMemberProp = type.GetProperty(bindingSrc.DataMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (dataMemberProp != null)
                        {
                            Type itemType = GetItemTypeFromDataMember(dataMemberProp.PropertyType);
                            if (itemType != null)
                            {
                         //       Debug.WriteLine($"Resolved ItemType from DataMember '{bindingSrc.DataMember}' = {itemType.FullName}");
                                entity = EntityHelper.GetEntityStructureFromType(itemType);
                            }
                            else
                            {
                           //     Debug.WriteLine($"Could not extract item type from DataMember '{bindingSrc.DataMember}' property type: {dataMemberProp.PropertyType}");
                            }
                        }
                        else
                        {
                         //   Debug.WriteLine($"DataMember '{bindingSrc.DataMember}' not found on Type {type.FullName}");
                        }
                    }
                    else
                    {
                     //   Debug.WriteLine("No DataMember specified with Type DataSource, using Type directly");
                        entity = EntityHelper.GetEntityStructureFromType(type);
                    }
                }
                else
                {
                    // Handle instance (could be DataTable, IList, or class with DataMember)
                    if (!string.IsNullOrEmpty(bindingSrc.DataMember))
                    {
                        PropertyInfo prop = dataSource.GetType().GetProperty(bindingSrc.DataMember, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (prop != null)
                        {
                            resolvedData = prop.GetValue(dataSource);
                          //  Debug.WriteLine($"Resolved data from DataMember = {resolvedData?.GetType()}");
                        }
                        else
                        {
                          //  Debug.WriteLine($"DataMember '{bindingSrc.DataMember}' not found on instance, using BindingSource.List or DataSource");
                            resolvedData = bindingSrc.List != null && bindingSrc.List.Count > 0 ? bindingSrc.List : dataSource;
                        }
                    }
                    else
                    {
                      //  Debug.WriteLine("No DataMember, attempting auto-detection or using BindingSource.List");
                        resolvedData = GetCollectionPropertyFromInstance(dataSource) ?? (bindingSrc.List != null && bindingSrc.List.Count > 0 ? bindingSrc.List : dataSource);
                      //  Debug.WriteLine($"Resolved data = {resolvedData?.GetType()}");
                    }
                }
            }
            else if (_dataSource is DataTable dataTable)
            {
              //  Debug.WriteLine("DataSource is DataTable");
                resolvedData = dataTable;
            }
            else if (_dataSource is IList iList)
            {
               // Debug.WriteLine("DataSource is IList");
                resolvedData = iList;
            }
            else
            {
              //  Debug.WriteLine($"DataSource is unrecognized type: {_dataSource.GetType()}, attempting auto-detection");
                resolvedData = GetCollectionPropertyFromInstance(_dataSource) ?? _dataSource;
            }

            // Step 2: Process resolvedData to set finalData and entity
            if (entity == null) // Entity not set yet (i.e., not a Type case)
            {
                if (resolvedData == null)
                {
                  //  Debug.WriteLine("Resolved data is null, no entity generated");
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
                    //    Debug.WriteLine($"Extracted item type from IList: {itemType.FullName}");
                        entity = EntityHelper.GetEntityStructureFromType(itemType);
                    }
                    else
                    {
                        entity = EntityHelper.GetEntityStructureFromListorTable(finalData);
                    }
                }
                else
                {
                   // Debug.WriteLine($"Resolved data is not a recognized collection type: {resolvedData.GetType()}, using as-is");
                    finalData = resolvedData;
                    entity = EntityHelper.GetEntityStructureFromListorTable(finalData);
                }
            }
            else
            {
                // Entity already set from Type handling, finalData remains null (structure only)
               // Debug.WriteLine("Entity set from Type handling, finalData remains null");
            }

            //Step 3: Process entity and columns
            if (entity != null)
            {
              //  Debug.WriteLine($"New Entity: {entity.EntityName}, Existing Entity: {Entity?.EntityName}, Columns Count = {_columns.Count}");
                if (_columns.Any() )
                {
                    if (Entity != null && entity.EntityName.Equals(Entity.EntityName))
                    {
                    //    Debug.WriteLine("Preserving designer Columns, syncing fields");
                      //  SyncFields(entity);
                        SyncColumnsWithEntity(entity);
                    }
                    else
                    {
                     //   Debug.WriteLine("New Entity with designer Columns, updating Entity only");
                        Entity = entity;
                        SyncColumnsWithEntity(entity);
                    }
                }
                else
                {
                   // Debug.WriteLine("No designer Columns or not protected, regenerating");
                    Entity = entity;
                    CreateColumnsForEntity();
                }
            }
            else if (_columns.Any() && columnssetupusingeditordontchange)
            {
              //  Debug.WriteLine("No new Entity, keeping designer Columns");
            }

            //Debug.WriteLine($"DataSetup Completed: finalData = {finalData?.GetType()}, Entity = {Entity?.EntityName}, Columns Count = {_columns.Count}");
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
                     //   Debug.WriteLine($"Found collection property: {prop.Name}, Type = {prop.PropertyType}");
                    }
                }
            }

            if (collectionCount == 1 && collectionProp != null)
            {
                return collectionProp.GetValue(instance);
            }
            else if (collectionCount > 1)
            {
             //   Debug.WriteLine("Multiple collection properties found, please specify DataMember");
            }
            return null; // No single collection found or all are null
        }
        private Type GetItemTypeFromDataMember(Type propertyType)
        {
           // Debug.WriteLine($"GetItemTypeFromDataMember: PropertyType = {propertyType.FullName}");

            // Handle generic IEnumerable<T> (e.g., List<T>)
            if (propertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                Type[] genericArgs = propertyType.GetGenericArguments();
                if (genericArgs.Length > 0)
                {
                 //   Debug.WriteLine($"Extracted item type: {genericArgs[0].FullName}");
                    return genericArgs[0];
                }
            }

            // Handle DataTable
            if (propertyType == typeof(DataTable))
            {
              //  Debug.WriteLine("DataTable detected, returning DataRow");
                return typeof(DataRow);
            }

            // Handle arrays
            if (propertyType.IsArray)
            {
             //   Debug.WriteLine($"Array detected, element type: {propertyType.GetElementType().FullName}");
                return propertyType.GetElementType();
            }

          //  Debug.WriteLine("No item type extracted");
            return null;
        }
        private void SyncColumnsWithEntity(IEntityStructure entity)
        {
            // Debug.WriteLine("Syncing Columns with Entity");

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
                  //  Debug.WriteLine($"Synced Column '{field.fieldname}': Type = {existingColumn.PropertyTypeName}");
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
                  //  Debug.WriteLine($"Added new Column '{field.fieldname}': Type = {newColumn.PropertyTypeName}");
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
                Debug.WriteLine($"Error adding columns for Entity {Entity?.EntityName}: {ex.Message}");
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
        {
            // Wrap _fullData objects with RowID
            _fullData = finalData is IEnumerable<object> enumerable ? enumerable.ToList() : new List<object>();
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
            if (_fullData == null || !_fullData.Any()) return;

            // Store previous DisplayIndex values before updating
            foreach (var row in Rows)
            {
                row.OldDisplayIndex = row.DisplayIndex; // Save the old DisplayIndex
            }

            for (int i = 0; i < Rows.Count; i++)
            {
                int dataIndex = _dataOffset + i;
                var row = Rows[i];
                try
                {
                    if (dataIndex >= 0 && dataIndex < _fullData.Count)
                    {
                        var dataItem = _fullData[dataIndex] as DataRowWrapper;
                        if (dataItem != null)
                        {
                            EnsureTrackingForItem(dataItem); // Pass the wrapped item

                            // Update DisplayIndex to the current absolute position in _fullData
                            row.DisplayIndex = dataIndex;

                            // Fill row with data for all columns
                            for (int j = 0; j < Columns.Count; j++)
                            {
                                var col = Columns[j];
                                var cell = row.Cells[j];
    
                                    if (col.IsSelectionCheckBox)
                                    {
                                        // Use RowID to check persistent selection state
                                        int rowID = dataItem.RowID;
                                        bool isSelected = _persistentSelectedRows.ContainsKey(rowID) && _persistentSelectedRows[rowID];
                                        cell.CellValue = isSelected;
                                        cell.CellData = isSelected;
                                        //  Debug.WriteLine($"FillVisibleRows: Row {i}, DataIndex {dataIndex}, RowID {rowID}, Sel = {isSelected}, _persistentSelectedRows.Count = {_persistentSelectedRows.Count}");
                                    }
                                    else if (col.IsRowNumColumn)
                                    {
                                        // Set row number based on absolute data index
                                        cell.CellValue = dataIndex + 1; // Display 1-based index
                                        cell.CellData = dataIndex + 1;
                                    }
                                    else if (col.IsRowID)
                                    {
                                        // Set RowID (hidden column)
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

                                

                              
                                row.IsDataLoaded = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (var cell in row.Cells)
                        {
                            cell.CellValue = null;
                            cell.CellData = null;
                        }
                        row.IsDataLoaded = false;
                        row.DisplayIndex = -1;

                        // Clear selection if out of bounds
                        if (_currentRowIndex == i)
                        {
                            _currentRowIndex = -1;
                            _currentRow = null;
                            CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(-1, null));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FillVisibleRows Error at Row {i}, DataIndex {dataIndex}: {ex.Message}\nStackTrace: {ex.StackTrace}");
                }
            }
            // Update aggregationRow separately if it exists
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
                       // Debug.WriteLine($"FillVisibleRows (Aggregation): Col[{j}] CellValue={cell.CellValue}");
                    }
                }
            }
            // Update _selectedRows and _selectedgridrows based on persistent selection state using RowID
            var newSelectedRows = new List<int>();
            var newSelectedGridRows = new List<BeepRowConfig>();
            for (int i = 0; i < Rows.Count; i++)
            {
                int dataIndex = _dataOffset + i;
                if (dataIndex >= 0 && dataIndex < _fullData.Count)
                {
                    var dataItem = _fullData[dataIndex] as DataRowWrapper;
                    if (dataItem != null)
                    {
                        int rowID = dataItem.RowID;
                        if (_persistentSelectedRows.ContainsKey(rowID) && _persistentSelectedRows[rowID])
                        {
                            newSelectedRows.Add(i); // Visible index
                            newSelectedGridRows.Add(Rows[i]); // Current row instance
                        }
                    }
                }
            }
            _selectedRows = newSelectedRows;
            _selectedgridrows = newSelectedGridRows;

            // Sync tracking and adjust selection
            UpdateTrackingIndices();
            SyncSelectedRowIndexAndEditor();
            UpdateScrollBars();
            Invalidate();
        }
        private void MoveNextRow()
        {
            if (DataNavigator.BindingSource.Position < _fullData.Count - 1)
            {
                ScrollBy(1);  // +1 means scroll down one row
                DataNavigator.BindingSource.MoveNext();
                SyncWithNavigatorPosition();
            }
        }

        private void MovePreviousRow()
        {
            if (DataNavigator.BindingSource.Position > 0)
            {
                ScrollBy(-1); // -1 means scroll up one row
                DataNavigator.BindingSource.MovePrevious();
                SyncWithNavigatorPosition();
            }
        }
      
        public void MoveNextCell()
        {
            try
            {
                if (Rows == null || Rows.Count == 0 || Columns == null || Columns.Count == 0)
                {
                 //   Debug.WriteLine("MoveNextCell: No rows or columns available.");
                    return;
                }

              //  Debug.WriteLine($"MoveNextCell: Current position - Row: {_currentRowIndex}, Col: {_selectedColumnIndex}");

                int lastVisibleColumn = GetLastVisibleColumn();
                int firstVisibleColumn = GetNextVisibleColumn(-1); // Find the first column
                int nextColumn = GetNextVisibleColumn(_selectedColumnIndex);

              //  Debug.WriteLine($"LastVisibleColumn: {lastVisibleColumn}, FirstVisibleColumn: {firstVisibleColumn}, NextColumn: {nextColumn}");

                // 🔹 If at the last column of the last row, wrap back to the first row/column
                if (_currentRowIndex == Rows.Count - 1 && _selectedColumnIndex == lastVisibleColumn)
                {
                    if (firstVisibleColumn == -1)
                    {
              //          Debug.WriteLine("No visible columns to wrap to.");
                        return;
                    }
               //     Debug.WriteLine($"🔄 Wrapping to first row, first column: {firstVisibleColumn}");
                    SelectCell(0, firstVisibleColumn);
                    EnsureColumnVisible(firstVisibleColumn);
                    return;
                }

                // 🔹 If at the last visible column but more columns exist, scroll and move to the next column
                if (_selectedColumnIndex == lastVisibleColumn && nextColumn != -1)
                {
                //    Debug.WriteLine($"➡ Scrolling to next column: {nextColumn}");
                    EnsureColumnVisible(nextColumn);
                    SelectCell(_currentRowIndex, nextColumn);
                    return;
                }

                // 🔹 If at the last visible column of the row, move to the first column of the next row
                if (_selectedColumnIndex == lastVisibleColumn)
                {
                    if (_currentRowIndex < Rows.Count - 1)
                    {
               //         Debug.WriteLine($"➡ Moving to next row, first column: {firstVisibleColumn}");
                        SelectCell(_currentRowIndex + 1, firstVisibleColumn);
                        EnsureColumnVisible(firstVisibleColumn);
                        ScrollBy(1); // Ensure next row is visible
                    }
                    else
                    {
              //          Debug.WriteLine($"🔄 Wrapping to first column in the same row: {firstVisibleColumn}");
                        SelectCell(_currentRowIndex, firstVisibleColumn);
                        EnsureColumnVisible(firstVisibleColumn);
                    }
                }
                else
                {
                    // Move to next visible column
                    if (nextColumn == -1)
                    {
               //         Debug.WriteLine("❌ No next visible column found.");
                        return;
                    }
               //     Debug.WriteLine($"➡ Moving to next column: {nextColumn}");
                    EnsureColumnVisible(nextColumn);
                    SelectCell(_currentRowIndex, nextColumn);
                    //Focus();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MoveNextCell crashed: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
        private int GetLastVisibleColumn()
        {
            try
            {
                if (Columns == null || Columns.Count == 0)
                {
                 //   Debug.WriteLine("GetLastVisibleColumn: Columns is null or empty.");
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
                Debug.WriteLine($"GetLastVisibleColumn crashed: {ex.Message}");
                return -1;
            }
        }
        private int GetNextVisibleColumn(int currentIndex)
        {
            try
            {
                if (Columns == null || Columns.Count == 0)
                {
                  //  Debug.WriteLine("GetNextVisibleColumn: Columns is null or empty.");
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
                Debug.WriteLine($"GetNextVisibleColumn crashed: {ex.Message}");
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
                 //   Debug.WriteLine($"EnsureColumnVisible: Invalid column index {colIndex}");
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
                Debug.WriteLine($"EnsureColumnVisible crashed: {ex.Message}\nStackTrace: {ex.StackTrace}");
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
                            SelectCell(_currentRowIndex - 1, _selectedColumnIndex);
                        }
                        break;
                    case Keys.Down:
                        if (_currentRowIndex < Rows.Count - 1)
                        {
                            SelectCell(_currentRowIndex + 1, _selectedColumnIndex);
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
                    if (_currentRowIndex > 0)
                    {
                        SelectCell(_currentRowIndex - 1, _selectedColumnIndex);
                    }
                    break;
                case Keys.Down:
                    if (_currentRowIndex < Rows.Count - 1)
                    {
                        SelectCell(_currentRowIndex + 1, _selectedColumnIndex);
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
        private void UpdateCellControl(IBeepUIComponent control, BeepColumnConfig column, object value)
        {
            if (control == null) return;

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
                    if (column?.Items != null)
                    {
                        comboBox.ListItems = new BindingList<SimpleItem>(column.Items);
                        if (value is SimpleItem simpleItem)
                        {
                            var item = column.Items.FirstOrDefault(i => i.Text == simpleItem.Text && i.ImagePath == simpleItem.ImagePath);
                            if (item != null)
                            {
                                comboBox.SelectedItem = item;
                            }
                        }
                        else
                        {
                            string stringValue = value.ToString();
                            var item = column.Items.FirstOrDefault(i => i.Value?.ToString() == stringValue || i.Text == stringValue);
                            if (item != null)
                            {
                                comboBox.SelectedItem = item;
                            }
                        }
                    }
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
                    image.ImagePath = ImageListHelper.GetImagePathFromName(value.ToString());
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
        private void DrawNavigationRow(Graphics g, Rectangle rect)
        {
            DataNavigator.Location = new Point(rect.Left+1, rect.Top + 1);
            DataNavigator.Size = new Size(rect.Width-2, rect.Height-2);
          //  DataNavigator.Invalidate();
            using (var pen = new Pen(_currentTheme.GridLineColor))
            {
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            }
        }
        private void UpdateStickyWidth()
        {
            var stickyColumns = Columns.Where(c => c.Sticked && c.Visible).ToList();
            int baseStickyWidth = stickyColumns.Sum(c => c.Width);

            // Cap _stickyWidth to prevent overflow within gridRect
            _stickyWidth = Math.Min(baseStickyWidth, gridRect.Width);
          //  System.Diagnostics.Debug.WriteLine($"UpdateStickyWidth: _stickyWidth={_stickyWidth}, BaseSticky={baseStickyWidth}, GridRect.Width={gridRect.Width}");
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
                    if ( yOffset + RowHeight > bounds.Bottom )
                        break;
                    // Align rowRect with scrollingRegion, accounting for scroll
                    int scrollingStartX = scrollingRegion.Left - _xOffset;
                    int totalScrollableWidth = Columns.Where(c => !c.Sticked && c.Visible).Sum(c => c.Width) +
                                              (Columns.Count(c => !c.Sticked && c.Visible) - 1) * 1;
                    int scrollingWidth = Math.Max(scrollingRegion.Width + _xOffset, totalScrollableWidth);
                    var rowRect = new Rectangle(scrollingStartX, displayY, scrollingWidth, RowHeight);

                    PaintScrollingRow(g, row, rowRect);
                    if (!row.IsAggregation) yOffset += _rowHeight; // Only increment yOffset for non-aggregation rows

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
                        if (yOffset + RowHeight > bounds.Bottom )
                            break;

                        var cell = row.Cells[Columns.IndexOf(stickyCol)];
                        var cellRect = new Rectangle(stickyX, displayY, stickyCol.Width, RowHeight);
                        Color backcolor = cell.RowIndex == _currentRowIndex ? _currentTheme.SelectedRowBackColor : _currentTheme.GridBackColor;
                        PaintCell(g, cell, cellRect, backcolor);
                        if (!row.IsAggregation) yOffset += _rowHeight; // Only increment yOffset for non-aggregation rows
                    }
                    yOffset = bounds.Top;
                }
            }

            g.ResetClip();
        }

        private void PaintScrollingRow(Graphics g, BeepRowConfig row, Rectangle rowRect)
        {
            int xOffset = rowRect.Left; // Starts at scrollingRegion.Left - _xOffset from PaintRows
            int rightBoundary = rowRect.Right; //
                                               //rowRect’s right edge
            int totalScrollableWidth = Columns.Where(c => !c.Sticked && c.Visible).Sum(c => c.Width) +
                                      (Columns.Count(c => !c.Sticked && c.Visible) - 1) * 1; // Include border width
            int leftBoundary = rowRect.Left + _stickyWidth; // Left edge of the scrolling region

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

                xOffset += Columns[i].Width;
                if (xOffset >= rightBoundary && xOffset < rowRect.Left + totalScrollableWidth)
                    rightBoundary = Math.Min(rowRect.Left + totalScrollableWidth, rowRect.Right); // Extend boundary if within scrollable range
                if (xOffset >= rightBoundary) break; // Exit if past boundary
            }
        }
        private void PaintCell(Graphics g, BeepCellConfig cell, Rectangle cellRect, Color backcolor)
        {
            // If this cell is being edited, skip drawing so that
            // the editor control remains visible.
            //  if (_selectedCell == cell && !_columns[_selectedCell.ColumnIndex].ReadOnly) return;
            Rectangle TargetRect = cellRect;
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
                UpdateCellControl(columnEditor, Columns[cell.ColumnIndex], cell.CellValue);

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
                    yOffset += _rowHeight;
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

            //System.Diagnostics.Debug.WriteLine($"DrawColumnBorders: StickyWidth={stickyWidth}, LastXOffset={xOffset}, Bounds={bounds}, XOffset={_xOffset}");
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
                    Debug.WriteLine($"PaintColumnHeaders Scrolling: Col={col.ColumnName}, X={scrollingXOffset}, Width={col.Width}, _xOffset={_xOffset}");
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
                    Debug.WriteLine($"PaintColumnHeaders Sticky: Col={col.ColumnName}, X={xOffset}, Width={col.Width}");
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

            Debug.WriteLine($"PaintColumnHeaders: StickyWidth={stickyWidth}, HeaderRect={headerRect}, _xOffset={_xOffset}");
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
                DataNavigator.BindingSource.DataSource = _fullData;
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
            DataNavigator.BindingSource.DataSource = filteredData;
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
              //  System.Diagnostics.Debug.WriteLine("MoveEditor: Skipped - null reference or editor not shown");
                return;
            }

            // Get the cell’s rectangle relative to gridRect
            Rectangle cellRect = GetCellRectangleIn(_editingCell);

            // Adjust for scrolling offsets
            int yOffset = _dataOffset * RowHeight;
            int xOffset = _xOffset;
            cellRect.Y -= yOffset; // Shift up by vertical scroll
            cellRect.X -= xOffset; // Shift left by horizontal scroll

            // Define grid bounds
            int gridLeft = 0; // Relative to gridRect’s client area
            int gridRight = gridRect.Width;
            int gridTop = 0;
            int gridBottom = gridRect.Height;

            // Check if the editor is fully out of view
            bool isFullyOutOfView =
                (cellRect.Right < gridLeft) ||  // Completely off to the left
                (cellRect.Left > gridRight) ||  // Completely off to the right
                (cellRect.Bottom < gridTop) ||  // Scrolled out past the top
                (cellRect.Top > gridBottom);    // Completely off below

            if (isFullyOutOfView)
            {
                //System.Diagnostics.Debug.WriteLine($"MoveEditor: Editor out of view - Hiding (CellRect={cellRect})");
                _editingControl.Visible = false;
            }
            else
            {
                // Position editor within gridRect’s client coordinates
                _editingControl.Location = new Point(cellRect.X, cellRect.Y);
                _editingControl.Visible = true;
             //   System.Diagnostics.Debug.WriteLine($"MoveEditor: Editor moved to {cellRect.X},{cellRect.Y}");
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
     //           Debug.WriteLine("MoveEditor: Skipped - null reference");
                return;
            }

            // 🔹 Get the exact rectangle of the cell **after scrolling**
            Rectangle cellRect = GetCellRectangle(_editingCell);
          //  Debug.WriteLine($"MoveEditor: cellRect.Y={cellRect.Y}, _dataOffset={_dataOffset}");

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
          //      Debug.WriteLine("MoveEditor: Editor is out of view - Hiding");
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
            // First, ensure the point is inside the grid's drawing area.
            if (!gridRect.Contains(location))
                return null;

            // Compute the Y coordinate relative to gridRect.
            int yRelative = location.Y - gridRect.Top;
            int rowIndex = yRelative / RowHeight;
            if (rowIndex < 0 || rowIndex >= Rows.Count)
                return null;
            var row = Rows[rowIndex];

            // Compute the X coordinate relative to gridRect.
            // Note: We add XOffset because our content is shifted to the right by the scroll offset.
            int xRelative = location.X - gridRect.Left + XOffset;
            int currentX = 0;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (!Columns[i].Visible)
                    continue;
                if (xRelative >= currentX && xRelative < currentX + Columns[i].Width)
                {
                    // Return the cell at column i (assuming 1-to-1 correspondence).
                    return row.Cells[i];
                }
                currentX += Columns[i].Width;
            }
            return null;
        }
        private Rectangle GetCellRectangleIn(BeepCellConfig cell)
        {
            if (cell == null)
            {
              //  System.Diagnostics.Debug.WriteLine("GetCellRectangle: Cell is null");
                return Rectangle.Empty;
            }

            // Find cell’s row index in Rows to match GetCellAtLocation’s coordinate system
            int rowIndex = -1;
            for (int r = 0; r < Rows.Count; r++)
            {
                if (Rows[r].Cells.Contains(cell))
                {
                    rowIndex = r;
                    break;
                }
            }
            if (rowIndex == -1)
            {
              //  System.Diagnostics.Debug.WriteLine("GetCellRectangle: Cell not found in Rows");
                return Rectangle.Empty;
            }

            int colIndex = Rows[rowIndex].Cells.IndexOf(cell);
            if (colIndex == -1)
            {
               // System.Diagnostics.Debug.WriteLine("GetCellRectangle: Cell not found in row");
                return Rectangle.Empty;
            }

            // Calculate position matching GetCellAtLocation
            int y = gridRect.Top + (rowIndex * RowHeight); // No _dataOffset here, handled in Rows
            int x = gridRect.Left;
            for (int i = 0; i < colIndex; i++)
            {
                if (Columns[i].Visible)
                    x += Columns[i].Width;
            }
            int width = Columns[colIndex].Width;
            int height = RowHeight;

            Rectangle rect = new Rectangle(x, y, width, height);
         //   System.Diagnostics.Debug.WriteLine($"GetCellRectangle: Cell={x},{y}, Value={width}x{height}, _dataOffset={_dataOffset}, _xOffset={_xOffset}");
            return rect;
        }
        private Rectangle GetCellRectangle(BeepCellConfig cell)
        {
            // Find which row and column this cell belongs to.
            int rowIndex = -1, colIndex = -1;
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
                return Rectangle.Empty;

            // Calculate Y coordinate: gridRect.Top is the start of rows.
            int y = gridRect.Top + rowIndex * RowHeight;

            // Calculate X coordinate: start at gridRect.Left and add the widths of the previous visible columns.
            int x = gridRect.Left - XOffset;
            for (int i = 0; i < colIndex; i++)
            {
                if (Columns[i].Visible)
                    x += Columns[i].Width;
            }
            int width = Columns[colIndex].Width;
            int height = RowHeight;
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
             //   Debug.WriteLine($"StartSmoothScroll: TargetV={_scrollTargetVertical}, TargetH={_scrollTargetHorizontal}, CurrentV={_dataOffset}, CurrentH={_xOffset}");
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
             //   Debug.WriteLine($"ScrollTimer_Tick: OffsetV={_dataOffset}, OffsetH={_xOffset}, Updated={updated}");
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

              //  Debug.WriteLine($"UpdateScrollBars Horizontal: totalColumnWidth={totalColumnWidth}, visibleWidth={visibleWidth}, stickyWidth={stickyWidth}, maxXOffset={maxXOffset}, _xOffset={_xOffset}");
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
            return gridRect.Height / RowHeight;
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
            int yOffset = _dataOffset * RowHeight; // Use RowHeight directly
            int xOffset = _xOffset;

            // Only update positions for visible rows
            int startRow = Math.Max(0, _dataOffset);
            int endRow = Math.Min(_dataOffset + GetVisibleRowCount(), Rows.Count);

            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                var row = Rows[rowIndex];
                row.UpperY = gridRect.Top + (rowIndex * RowHeight) - yOffset;

                int x = gridRect.Left - xOffset;
                for (int colIndex = 0; colIndex < Columns.Count; colIndex++)
                {
                    if (Columns[colIndex].Visible)
                    {
                        var cell = row.Cells[colIndex];
                        cell.X = x;
                        cell.Y = row.UpperY;
                        cell.Width = Columns[colIndex].Width;
                        cell.Height = RowHeight;
                        x += Columns[colIndex].Width;

                        // Optimize by checking if cell is within visible area
                        if (cell.X + cell.Width < gridRect.Left || cell.X > gridRect.Right)
                            continue; // Skip off-screen cells
                    }
                }
            }

           // Debug.WriteLine($"UpdateCellPositions: yOffset={yOffset}, xOffset={_xOffset}, VisibleRows={endRow - startRow}, Columns={Columns.Count(c => c.Visible)}");
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
        // New method to change column width
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
                System.Diagnostics.Debug.WriteLine($"SetColumnWidth: Column '{columnName}' not found.");
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
                    Columns[_resizingIndex].Width = Math.Max(20, Columns[_resizingIndex].Width + deltaX); // Prevent negative width
                }
                _lastMousePos = e.Location;
                UpdateScrollBars();
                Invalidate();
            }
            else if (_resizingRow && _resizingIndex >= 0)
            {
                int deltaY = e.Y - _lastMousePos.Y;
                _rowHeight = Math.Max(10, _rowHeight + deltaY); // Prevent negative height
                _lastMousePos = e.Location;
               // InitializeRows(); // Recreate rows with new height
                UpdateScrollBars();
                Invalidate();
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
            // Calculate the Y coordinate relative to the grid's drawing area (rows start at gridRect.Top).
            int yRelative = location.Y - gridRect.Top;
            int row = yRelative / RowHeight;
            if (row < 0 || row >= Rows.Count)
            {
                rowIndex = -1;
                return false;
            }
            // The bottom edge of this row (in relative coordinates) is at (row + 1) * RowHeight.
            int rowBottom = (row + 1) * RowHeight;
            if (Math.Abs(yRelative - rowBottom) <= _resizeMargin)
            {
                rowIndex = row;
                return true;
            }
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
        // New method to update gridRect (example implementation)

        #endregion
        #region Editor
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

            UpdateCellControl(_editingControl, Columns[colIndex], cell.CellValue);

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

          //  System.Diagnostics.Debug.WriteLine($"ShowCellEditor: Cell={cell.X},{cell.Y}, Value={cellSize}");
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
        //    Debug.WriteLine($"Cell size: {cellSize}");
            _editorPopupForm.ClientSize = cellSize; // 👈 Ensures no extra space due to window frame
            _editingControl.Size = cellSize; // 👈 Matches cell exactly
            _editingControl.Dock = DockStyle.Fill; // 👈 Ensures it doesn't resize incorrectly
          //  Debug.WriteLine($"Editor size: {_editingControl.Value}");
         //   Debug.WriteLine($"Popup size: {_editorPopupForm.ClientSize}");
            // **🔹 Position popup exactly at the cell location (relative to BeepSimpleGrid)**
            Point screenLocation = this.PointToScreen(new Point(cell.X, cell.Y));
            _editorPopupForm.Location = screenLocation;
            _editingControl.Theme = Theme;
            // **Set initial text**
            //  _editingControl.Text = cell.CellValue;
          
            UpdateCellControl(_editingControl, Columns[colIndex], cell.CellValue);
            // 🔹 Confirm value in editor **after setting**

         //   Debug.WriteLine($"🔄 Setting BeepTextBox text before popupform is Show: {_editingControl.Text}");
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
            // //   Debug.WriteLine($"✅ After popup is fully visible, setting text: {_editingControl.Text}");
            //}, TaskScheduler.FromCurrentSynchronizationContext());
            _editorPopupForm.SetValue(cell.CellValue);
            _editingControl.Focus();
            IsEditorShown = true;
           // Debug.WriteLine($"✅ after popform Show the Editor BeepTextBox text   Show : {_editingControl.Text}");
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
           //     Debug.WriteLine($"✅ before closing Editor Text After Update: {_editingControl.Text}");

                SaveEditedValue();
             //   Debug.WriteLine($"✅ After Save Text After : {_editingControl.Text}");
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

             //   Debug.WriteLine("Popup editor closed successfully.");
            }
            IsEditorShown = false;
        }
        private void SaveEditedValue()
        {
            if ( _editingCell == null || _editingControl==null)
            {
           //     Debug.WriteLine($"⚠️ Editing control or cell is null!");
                return;
            }

            object newValue = _editingControl.GetValue();
          //  Debug.WriteLine($"🔄 Saving value: {newValue} (Old: {_editingCell.CellData})");

            // 🔹 Check if the new value is empty or null
            if (newValue == null || string.IsNullOrWhiteSpace(newValue.ToString()))
            {
             //   Debug.WriteLine($"⚠️ New value is empty. Skipping update.");
                return;
            }

            // 🔹 Retrieve PropertyType from the corresponding column
            BeepColumnConfig columnConfig = Columns.FirstOrDefault(c => c.Index == _editingCell.ColumnIndex);
            if (columnConfig == null)
            {
            //    Debug.WriteLine($"⚠️ Column config not found. Skipping update.");
                return;
            }
            Type propertyType = Type.GetType(columnConfig.PropertyTypeName, throwOnError: false) ?? typeof(string); // Default to string if null

             //🔹 Convert new value to the correct type before comparing
            object convertedNewValue = MiscFunctions.ConvertValueToPropertyType(propertyType, newValue);
            object convertedOldValue = MiscFunctions.ConvertValueToPropertyType(propertyType, _editingCell.CellData);

            // 🔹 Skip update if the new value is the same as the old value
            if (convertedNewValue != null && convertedOldValue != null && convertedNewValue.Equals(convertedOldValue))
            {
            //    Debug.WriteLine($"⚠️ New value is the same as old. Skipping update.");
                return;
            }

            // 🔹 Update cell's stored value
            _editingCell.OldValue = _editingCell.CellData;
            _editingCell.CellData =  convertedNewValue;
            _editingCell.CellValue = convertedNewValue;
            _editingCell.IsDirty = true;

            // 🔹 Update the corresponding data record
            UpdateDataRecordFromRow(_editingCell);

            // 🔹 Trigger validation if necessary
            _editingCell.ValidateCell();

           // Debug.WriteLine($"✅ Cell updated. New: {_editingCell.CellValue}");

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
            if (clickedCell.IsAggregation) return;
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

                        //Debug.WriteLine($"BeepGrid_MouseClick: Row {rowIndex}, DataIndex {dataIndex}, RowID {rowID}, Sel = {!isSelected}, _persistentSelectedRows.Count = {_persistentSelectedRows.Count}");
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
                CloseCurrentEditorIn();
                if (_selectedCell != null)
                {
                    _editingCell = _selectedCell;
                    SelectCell(_selectedCell);
                    if (DataNavigator != null && _currentRow != null && _currentRow.DisplayIndex >= 0)
                    {
                        DataNavigator.BindingSource.Position = _currentRow.DisplayIndex;
                    }
                    if (!_columns[_selectedCell.ColumnIndex].ReadOnly && _columns[_selectedCell.ColumnIndex].CellEditor!= BeepColumnType.Image  )
                    {
                        ShowCellEditorIn(_selectedCell, e.Location);
                    }
                }
            }
        }
        #endregion Editor
        #region Events
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
        public event EventHandler<BeepCellEventArgs> CellMouseWheel;


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
            foreach (var tracking in Trackings)
            {
                int currentIndex = -1;
                for (int i = 0; i < _fullData.Count; i++)
                {
                    var wrappedItem = _fullData[i] as DataRowWrapper;
                    var originalWrapped = originalList[tracking.OriginalIndex] as DataRowWrapper;
                    if (wrappedItem != null && originalWrapped != null && wrappedItem.TrackingUniqueId == originalWrapped.TrackingUniqueId)
                    {
                        currentIndex = i;
                        break;
                    }
                }

                if (currentIndex >= 0 && tracking.CurrentIndex != currentIndex)
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
                        Debug.WriteLine($"UpdateTrackingIndices: Tracking {tracking.UniqueId}, OriginalIndex {tracking.OriginalIndex}, Updated CurrentIndex to {currentIndex}, RowState {wrappedItem.RowState}");
                    }
                }
            }
        }

        // Sync _currentRowIndex and move the editor
        private void SyncSelectedRowIndexAndEditor()
        {
            if (_currentRow != null && _fullData.Any())
            {
                int selectedDataIndex = -1;
                int oldRowIndex = -1;
                int oldColumnIndex = _selectedCell != null ? _selectedCell.ColumnIndex : -1;
                for (int i = 0; i < Rows.Count; i++)
                {
                    if (Rows[i] == _currentRow)
                    {
                        oldRowIndex = i;
                        selectedDataIndex = Rows[i].OldDisplayIndex; // Old DisplayIndex
                        break;
                    }
                }

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

                        if (oldRowIndex != -1 && oldRowIndex != newRowIndex)
                        {
                            string direction = newRowIndex < oldRowIndex ? "up" : "down";
                           // Debug.WriteLine($"Selected row moved {direction}: OldRowIndex={oldRowIndex}, NewRowIndex={newRowIndex}, DisplayIndex={_currentRow.DisplayIndex}");
                        }

                        CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(selectedDataIndex, _currentRow));
                    }
                    else
                    {
                        _currentRowIndex = -1;
                        _currentRow = null;
                        _selectedCell = null;
                        if (_editingControl != null && _editingControl.Visible)
                        {
                            _editingControl.Visible = false;
                        }
                        CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(-1, null));
                    }
                }
                else
                {
                    _currentRowIndex = -1;
                    _currentRow = null;
                    _selectedCell = null;
                    if (_editingControl != null && _editingControl.Visible)
                    {
                        _editingControl.Visible = false;
                    }
                    CurrentRowChanged?.Invoke(this, new BeepRowSelectedEventArgs(-1, null));
                }
            }
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
            //    Debug.WriteLine("MapPropertyTypeToDbFieldCategory: propertyTypeName is null or empty, defaulting to String");
                return DbFieldCategory.String;
            }

            // Attempt to resolve the Type from the AssemblyQualifiedName
            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
            {
             //   Debug.WriteLine($"MapPropertyTypeToDbFieldCategory: Could not resolve type '{propertyTypeName}', defaulting to String");
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
          //  Debug.WriteLine($"MapPropertyTypeToDbFieldCategory: Unknown type '{type.FullName}', defaulting to String");
            return DbFieldCategory.String;
        }
        private BeepColumnType MapPropertyTypeToCellEditor(string propertyTypeName)
        {
            // Default to Text if typeName is null or empty
            if (string.IsNullOrWhiteSpace(propertyTypeName))
            {
            //    Debug.WriteLine("MapPropertyTypeToCellEditor: propertyTypeName is null or empty, defaulting to Text");
                return BeepColumnType.Text;
            }

            // Attempt to resolve the Type from the AssemblyQualifiedName
            Type type = Type.GetType(propertyTypeName, throwOnError: false);
            if (type == null)
            {
              //  Debug.WriteLine($"MapPropertyTypeToCellEditor: Could not resolve type '{propertyTypeName}', defaulting to Text");
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
          //  Debug.WriteLine($"MapPropertyTypeToCellEditor: Unknown type '{type.FullName}', defaulting to Text");
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