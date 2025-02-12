﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis.Logic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.BindingNavigator;
using TheTechIdea.Beep.Winform.Controls.Grid.DataColumns;

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    /// <summary>
    /// The BeepGrid class is a custom control that extends the functionality of a DataGridView with additional features such as sorting, filtering, and theming.
    /// </summary>
    /// 
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepDataGridView))] // Optional//"Resources.BeepButtonIcon.bmp"
    [Category("Beep Controls")]
    [DisplayName("Beep Data Grid View")]
    public class BeepDataGridView:BeepControl
    {
        #region Properties
        public DMEEditor DMEEditor { get; set; }
        private DataGridView _targetGrid;
        private BeepGridFooter _linkedFooter;
        private bool _isUpdating;

        // Panels:
        // Row #1: top panel (icons + title)
        // Row #2: custom column header row
        // Row #3: optional filter row

        private Panel _topPanel;
        private TableLayoutPanel _topTableLayout;
        private Panel _headerPanel;
        private Panel _filterPanel;

        // Buttons & Title in the top panel
        private BeepButton _csvExportButton, _printButton, _shareButton, _totalShowButton, _filterToggleButton;
        private BeepLabel _titleLabel;

        // Column-sorting & filter references
        private Dictionary<DataGridViewColumn, SortOrder> _columnSortOrders = new();
        private Dictionary<DataGridViewColumn, BeepLabel> _headerLabels = new();
        private Dictionary<DataGridViewColumn, BeepTextBox> _filterBoxes = new();
        /// <summary>
        /// Event handler for cell painting, allowing custom cell rendering.
        /// </summary>
        public EventHandler<DataGridViewCellPaintingEventArgs> CellPainting { get; set; }

        /// <summary>
        /// Stores configuration details for each column, such as filters and totals.
        /// </summary>
        /// 
        private List<BeepGridColumnConfig> _columnconfig;
        [Browsable(true)]
        [Category("Data")]
        [Description("Column Configurations for the grid.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public List<BeepGridColumnConfig> ColumnConfigs
        {
            get { return _columnconfig; }
            set { _columnconfig = value; }
        }
        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TitleFont
        {
            get => _textFont;
            set
            {

                _textFont = value;
                UseThemeFont = false;
                if (_titleLabel != null)
                {
                    if (UseThemeFont)
                    {
                        _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                        _titleLabel.UseThemeFont = true;
                    }
                    else
                    {
                        _titleLabel.TextFont = _textFont;
                    }
                    // Force the label to recalculate its preferred size
                    _titleLabel.PerformLayout();

                    _topTableLayout.RowStyles[0].Height = _titleLabel.PreferredSize.Height + (2 * (padding + 2)); // Adjust height dynamically
                    RecalcHeight();
                    Invalidate();
                }



            }
        }
        private EntityStructure _entity;
        public EntityStructure Entity
        {
            get { return _entity; }
            set { _entity = value; }
        }
        /// <summary>
        /// A dictionary to hold filters for each column.
        /// </summary>
        private Dictionary<string, string> ColumnFilters = new Dictionary<string, string>();
        /// <summary>
        /// A dictionary mapping header labels to their corresponding sort icons.
        /// </summary>
        private Dictionary<BeepLabel, BeepImage> sortIcons = new Dictionary<BeepLabel, BeepImage>();

        /// <summary>
        /// A dictionary mapping header labels to their corresponding DataGridView columns.
        /// </summary>
        private Dictionary<BeepLabel, DataGridViewColumn> headerColumnMapping = new Dictionary<BeepLabel, DataGridViewColumn>();

       

        private BeepScrollBar verticalScrollBar;
        private BeepScrollBar horizontalScrollBar;


        // Column resizing
        private bool _isResizing = false;
        private bool _resizingLeft = false;
        private BeepLabel _resizingLabel = null;
        private Point _initialMousePos;
        private int _initialColWidth;
        private int _initialLabelLeft;

        // Layout heights
        private int _topPanelHeight = 30;  // row #1
        private int _headerPanelHeight = 30;  // row #2
        private int _filterPanelHeight = 30;  // row #3 if visible
        private bool _showFilter = false;     // filter row toggled
        private int padding = 2;
        private bool isinit = true;
        private Size _buttonsize = new Size(16, 16);
        private Size _imagesize = new Size(14, 14);
        // FlowLayout for column-aligned totals
        private Panel _totalsFlow;

        // Binding navigator for record navigation
        private BeepBindingNavigator _bindingNavigator;

        // A dictionary to store the label for each column
        private Dictionary<DataGridViewColumn, BeepLabel> _columnTotals = new();

        // Toggle whether totals panel is shown
        private bool _showTotalsPanel = false;

        // Toggle whether data navigator is shown
        private bool _showDataNavigator = true;
        #region "Data Source"
        // Backing fields to hold design-time values.
        private object _dataSource;
        private string _dataMember;

        private BindingSource _bindingSource = new BindingSource();
        //public BindingSource BindingSource
        //{
        //    get { return _bindingSource; }
        //    set
        //    {
        //        InQuery = true;
        //        _bindingSource = value;
        //        if (_targetGrid != null) _targetGrid.DataSource = _bindingSource;
        //        InQuery = false;
        //    }
        //}
        // Expose DataSource with design-time attributes.
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
                // If _targetGrid is already created, forward the value
                if (_targetGrid != null)
                {
                    _bindingSource.DataSource = value;
                    _targetGrid.DataSource = _bindingSource;
                }
            }
        }

        // Expose DataMember with design-time attributes.
        [Browsable(true)]
        [Category("Data")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public new string DataMember
        {
            get => _dataMember;
            set
            {
                _dataMember = value;
                Console.WriteLine("Data Member: " + value);
                if (_targetGrid != null)
                {
                    Console.WriteLine("Setting Data Member in bindingsource: " + value);
                    _bindingSource.DataMember = value;
                    Console.WriteLine("Setting Data Member in  grid : " + _bindingSource.DataMember);
                 //   _targetGrid.DataMember = value;
                    ResetData();
                }
            }
        }

        #endregion "Data Source"
        #region "Appearance"
        /// <summary>
        /// Show or hide the filter row below the column headers
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        public bool ShowFilter
        {
            get => _showFilter;
            set
            {
                _showFilter = value;
                if (_filterPanel != null)
                {
                    _filterPanel.Visible = value;
                    RecalcHeight();
                    if (value)
                    {
                        RebuildColumnsAndFilters(); // so we build textboxes
                    }
                    
                }

            }
        }

        /// <summary>
        /// Show or hide the totals row.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show or hide the totals row.")]
        public bool ShowTotalsPanel
        {
            get => _showTotalsPanel;
            set
            {
                _showTotalsPanel = value;
                if(_totalsFlow!=null)     _totalsFlow.Visible = value;
            }
        }

        /// <summary>
        /// Show or hide the data navigator.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Show or hide the data navigator.")]
        public bool ShowDataNavigator
        {
            get => _showDataNavigator;
            set
            {
                _showDataNavigator = value;
                if(_bindingNavigator!=null)     _bindingNavigator.Visible = value;
            }
        }

        #endregion "Appearance"
        #region "Sort and Filter"

        /// <summary>
        /// The current column being sorted.
        /// </summary>
        private DataGridViewColumn SortColumn = null;

        public bool IsSorting { get; private set; }

        /// <summary>
        /// The current label being used for sorting.
        /// </summary>
        private BeepLabel SortColumnLabel = null;

        /// <summary>
        /// The current direction of sorting (Ascending, Descending, or None).
        /// </summary>
        SortOrder Currentdirection = SortOrder.None;
        private bool isResizing;
        #endregion "Sort and Filter"
      
        #endregion Properties

        #region Constructor
        public BeepDataGridView()
        {
            Margin = new Padding(2);
            Padding = new Padding(2);
            _targetGrid = new DataGridView();
            _bindingSource = new BindingSource();
            InitializeLayout();
            SetupDataGridView();
        }
        protected override Size DefaultSize => new Size(300, 300);

        public bool InQuery { get; private set; }
        public string Title { get; private set; }
        #endregion Constructor
        #region "Setup"
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (isinit)
            {
               // InitializeLayout();
                if (_targetGrid != null)
                {
                    _bindingSource.DataSource = _dataSource;
                    _targetGrid.DataSource = _bindingSource;
                    _bindingSource.DataMember = _dataMember;
                    _targetGrid.DataMember = _dataMember;
                }
                isinit = false;
            }
        }
        private void RecalcHeight()
        {
            _topPanelHeight = _titleLabel.PreferredSize.Height + (2 * padding);
            _topPanel.Height = _topPanelHeight;

            // If there are no header labels, the header panel might get set to 0.
            int headerHeight = (_headerLabels.Count > 0) ? _headerPanel.Height : 30; // use 30 as a minimum
            _headerPanel.Height = headerHeight;

            int filterHeight = _showFilter ? _filterPanel.Height : 0;

            // Set the overall control height (for example)
          //  this.Height = _topPanel.Height + headerHeight + filterHeight;
        }
        private void InitializeLayout()
        {
           
            BeepGridMiscUI.InitializeComponent(this,_targetGrid);
            BeepGridMiscUI.HideScrollHorizontal = false;
            BeepGridMiscUI.HideScrollVertical = false;
            BeepGridMiscUI.MainSplitContainer.Panel1MinSize = 100;
            //SetupDataGridView();
            _bindingSource = new BindingSource();
            _bindingSource.DataSourceChanged += OnDataSourceChanged;
            //  Console.WriteLine("Initializing BeepGridHeader layout...");
            // Manually stack three panels
          
            _topPanel = new Panel
            {
                Height = _topPanelHeight,
                Dock = DockStyle.Top
            };
            //  Console.WriteLine("Top Panel Height: " + _topPanelHeight);
            // inside topPanel, we place a TableLayout for your 5 icons + label
            _topTableLayout = new TableLayoutPanel
            {
                ColumnCount = 6,
                RowCount = 1,
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            //  Console.WriteLine("Table Layout Column Count: " + _topTableLayout.ColumnCount);
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _topTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 23f));
            //  Console.WriteLine("Table Layout Row Count: " + _topTableLayout.RowCount);
            _topPanel.Controls.Add(_topTableLayout);
            //   Console.WriteLine("Top Panel Controls Count: " + _topPanel.Controls.Count);
            // Create beepbuttons + label
            _csvExportButton = new BeepButton
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.export.svg",
                Size = _buttonsize,
                MaxImageSize = _imagesize,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFramless = true,
                IsChild = true,
                Dock = DockStyle.Fill
            };
            _printButton = new BeepButton
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.print.svg",
                Size = _buttonsize,
                MaxImageSize = _imagesize,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFramless = true,
                IsChild = true,
                Dock = DockStyle.Fill
            };
            _shareButton = new BeepButton
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.share.svg",
                Size = _buttonsize,
                MaxImageSize = _imagesize,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFramless = true,
                IsChild = true,
                Dock = DockStyle.Fill
            };
            _totalShowButton = new BeepButton
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.sum.svg",
                Size = _buttonsize,
                MaxImageSize = _imagesize,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFramless = true,
                IsChild = true,
                Dock = DockStyle.Fill
            };
            _filterToggleButton = new BeepButton
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.search.svg",
                Size = _buttonsize,
                MaxImageSize = _imagesize,
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFramless = true,
                IsChild = true,
                Dock = DockStyle.Fill
            };
            _titleLabel = new BeepLabel
            {
                Text = "Title",
                TextAlign = ContentAlignment.MiddleCenter,
                ImageAlign = ContentAlignment.MiddleRight,
                IsFramless = true,
                IsChild = true,
                Dock = DockStyle.Fill
            };
            //  Console.WriteLine("Buttons Created");
            // Add them to table
            _topTableLayout.Controls.Add(_csvExportButton, 0, 0);
            _topTableLayout.Controls.Add(_totalShowButton, 1, 0);
            _topTableLayout.Controls.Add(_shareButton, 2, 0);
            _topTableLayout.Controls.Add(_printButton, 3, 0);
            _topTableLayout.Controls.Add(_filterToggleButton, 4, 0);
            _topTableLayout.Controls.Add(_titleLabel, 5, 0);
            //   Console.WriteLine("Controls Added to Table");
            // Hook up events
            _csvExportButton.Click += (s, e) => OnCsvExport();
            _printButton.Click += (s, e) => OnPrint();
            _totalShowButton.Click += (s, e) => OnToggleTotals();
            _filterToggleButton.Click += (s, e) => ShowFilter = !ShowFilter;
            //  Console.WriteLine("Events Hooked Up");
            // (2) column header row
            // --- Header Panel (for column labels) ---
            _headerPanel = new Panel
            {
                Height = 30,  // Ensure a fixed height
                Dock = DockStyle.Top,
                BackColor = Color.Gray,
                Visible = true

            };
            //    Console.WriteLine("Header Panel Height: " + _headerPanelHeight);
            ;
            //    Console.WriteLine("Header Panel Controls Count: " + _headerPanel.Controls.Count);
            // (3) filter row (optional)
            // --- Filter Panel (for filter textboxes) ---
            _filterPanel = new Panel
            {
                Height = 25,  // Ensure a fixed height (or your desired height)
                Dock = DockStyle.Top,
                Visible = true,
                BackColor = Color.WhiteSmoke
            };
            //   Console.WriteLine("Filter Panel Height: " + _filterPanelHeight);

            // Create the flow for totals (aligned with columns)
            _totalsFlow = new Panel
            {
                Dock = DockStyle.Top,
                Height = 24,
                AutoScroll = false
            };

            _bindingNavigator = new BeepBindingNavigator
            {
                Dock = DockStyle.Fill
            };



            // Initial visibility
            BeepGridMiscUI.HideTotals= _showTotalsPanel;
           


            // Add them from bottom to top
            BeepGridMiscUI.FilterColumnHeaderPanel.Controls.Add(_filterPanel);
            BeepGridMiscUI.ColumnHeaderPanel.Controls.Add(_headerPanel);
            BeepGridMiscUI.HeaderPanel.Controls.Add(_topPanel);

            BeepGridMiscUI.DataNavigatorPanel.Controls.Add(_bindingNavigator);
            BeepGridMiscUI.TotalsPanel.Controls.Add(_totalsFlow);
            //   Console.WriteLine("Controls Added to BeepGridHeader");
            //  RecalcHeight();
            //   Console.WriteLine("Height Recalculated");
        }
        #endregion "Setup"
       
        #region Column Building
        private void RebuildColumnsAndFilters()
        {
            // Clear existing controls and dictionaries
            _headerPanel.Controls.Clear();
            _filterPanel.Controls.Clear();
            _headerLabels.Clear();
            _filterBoxes.Clear();
            _columnSortOrders.Clear();
            if (ColumnConfigs == null) ColumnConfigs = new List<BeepGridColumnConfig>();
            ColumnConfigs.Clear();
            sortIcons.Clear();

            if (_targetGrid == null)
                return;

            int leftPosition = 0;  // Running left coordinate

            // Loop through each grid column and add corresponding header and filter controls.
            foreach (DataGridViewColumn col in _targetGrid.Columns)
            {
                AddOneColumn(col, ref leftPosition);
            }

            // Adjust the width of the header and filter panels to match the total columns width.
            _headerPanel.Width = leftPosition;
            if (_showFilter)
                _filterPanel.Width = leftPosition;
        }
        private void AddOneColumn(DataGridViewColumn col, ref int leftPosition)
        {
            // Create the header label.
            BeepLabel lbl = new BeepLabel
            {
                Text = col.HeaderText,
                Width = col.Width + BorderThickness,                         // Use the grid column's width.
                Height = _headerPanel.Height,              // Use the header panel's fixed height.
                TextAlign = ContentAlignment.MiddleCenter,
                Tag = col,
                GuidID = Guid.NewGuid().ToString(),
                Location = new Point(leftPosition, 0),
                AutoSize = false,
                BorderStyle = BorderStyle.FixedSingle,
                IsRounded = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsRoundedAffectedByTheme = false,
                Theme = this.Theme

            };
            lbl.Click += (s, e) => OnColumnHeaderClick(col);

            // Attach mouse events for resizing if needed.
            lbl.MouseDown += CustomHeaderLabel_MouseDown;
            lbl.MouseMove += CustomHeaderLabel_MouseMove;
            lbl.MouseUp += CustomHeaderLabel_MouseUp;

            // Add the label to the header panel and record it.
            _headerPanel.Controls.Add(lbl);
            _headerLabels[col] = lbl;

            // If filtering is enabled, add a filter textbox.
            if (_showFilter)
            {
                BeepTextBox txt = new BeepTextBox
                {
                    Width = col.Width,
                    Height = _filterPanel.Height,         // Use the filter panel's fixed height.
                    Tag = col,

                    Location = new Point(leftPosition, 0),
                    AutoSize = false,
                    IsRounded = false,
                    IsBorderAffectedByTheme = false,
                    IsShadowAffectedByTheme = false,
                    IsRoundedAffectedByTheme = false,
                    Theme = this.Theme

                };
                txt.TextChanged += (s, e) => ApplyFilter();
                _filterPanel.Controls.Add(txt);
                _filterBoxes[col] = txt;
            }
            if(_showTotalsPanel)
            {
                BeepLabel flterlbl = new BeepLabel
                {
                    Text = "0",
                    Width = col.Width + BorderThickness,                         // Use the grid column's width.
                    Height = _headerPanel.Height,              // Use the header panel's fixed height.
                    TextAlign = ContentAlignment.MiddleCenter,
                    Tag = col,
                    GuidID = Guid.NewGuid().ToString(),
                    Location = new Point(leftPosition, 0),
                    AutoSize = false,
                    BorderStyle = BorderStyle.FixedSingle,
                    IsRounded = false,
                    IsBorderAffectedByTheme = false,
                    IsShadowAffectedByTheme = false,
                    IsRoundedAffectedByTheme = false,
                    Theme = this.Theme
                };
                _columnTotals[col] = flterlbl;
                _totalsFlow.Controls.Add(flterlbl);
            }
            headerColumnMapping[lbl] = col;

            // PictureBox for Sort Icon
            BeepImage sortIcon = new BeepImage
            {
                Width = 16,
                Height = 16,
                Top = 2,
                Left = lbl.Width - 18, // Adjust as needed
                Visible = false,
                MaximumSize = new Size(14, 14),
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.sort.svg",
            };
            lbl.Controls.Add(sortIcon);
            sortIcons[lbl] = sortIcon;

            // Update the left position for the next control.
            leftPosition += col.Width;

            // Optionally, update your column configurations.
            AddColumnConfigurations(col, col.Index, col.Width, col.DataPropertyName, col.DataPropertyName, lbl);
        }
        private void RemoveOneColumn(DataGridViewColumn col)
        {
            // Remove the header label from the _headerPanel.
            if (_headerLabels.TryGetValue(col, out BeepLabel lbl))
            {
                _headerPanel.Controls.Remove(lbl);
                lbl.Dispose();
                _headerLabels.Remove(col);
            }

            // Remove the filter textbox from the _filterPanel.
            if (_filterBoxes.TryGetValue(col, out BeepTextBox tb))
            {
                _filterPanel.Controls.Remove(tb);
                tb.Dispose();
                _filterBoxes.Remove(col);
            }
            if (_columnTotals.TryGetValue(col, out var flterlbl))
            {
                flterlbl.Parent?.Controls.Remove(lbl);
                flterlbl.Dispose();
                _columnTotals.Remove(col);
                //_totalsFlow.Controls.Remove(flterlbl);
                // remove from header panel


            }
            // remove from column configs
            ColumnConfigs.RemoveAll(c => c.ColumnName == col.Name);
            // Optionally, update the header and filter panels' widths if necessary.
            UpdateHeaderAndPanelPositions();
        }

        #endregion
        #region Column Events
        /// <summary>
        /// Updates the custom header labels for the DataGridView columns and applies sorting icons.
        /// </summary>
        private void UpdateCustomHeaders()
        {
            _headerPanel.Controls.Clear();
            sortIcons.Clear();
            foreach (DataGridViewColumn column in _targetGrid.Columns)
            {
                BeepLabel headerLabel = new BeepLabel
                {
                    Text = column.HeaderText,
                    Width = column.Width,
                    Height = _headerPanel.Height,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BorderStyle = this.BorderStyle,
                    Left = column.DisplayIndex * column.Width,
                    Tag = column,

                };
                headerColumnMapping[headerLabel] = column;

                // PictureBox for Sort Icon
                BeepImage sortIcon = new BeepImage
                {
                    Width = 16,
                    Height = 16,
                    Top = 2,
                    Left = headerLabel.Width - 18, // Adjust as needed
                    Visible = false,
                    MaximumSize = new Size(14, 14),
                };
                headerLabel.Controls.Add(sortIcon);
                sortIcons[headerLabel] = sortIcon;
                headerLabel.Click += CustomHeaderLabel_Click;
                _headerPanel.Controls.Add(headerLabel);
            }
        }

        /// <summary>
        /// Updates the sort icons in the headers based on the current sort direction.
        /// </summary>
        /// <param name="sortedLabel">The label of the sorted column.</param>
        /// <param name="sortDirection">The direction of sorting.</param>
        private void UpdateSortIcons(BeepLabel sortedLabel, SortOrder sortDirection)
        {
            foreach (var headerLabel in sortIcons.Keys)
            {
                if (sortIcons[headerLabel] != null)
                {
                    sortIcons[headerLabel].Visible = headerLabel == sortedLabel;
                    if (sortIcons[headerLabel].Visible)
                    {
                        switch (sortDirection)
                        {
                            case SortOrder.None:
                                sortIcons[headerLabel].Image = null;
                                break;
                            case SortOrder.Ascending:
                                sortIcons[headerLabel].Image = Properties.Resources.SortAscending;
                                break;
                            case SortOrder.Descending:
                                sortIcons[headerLabel].Image = Properties.Resources.SortDescending;
                                break;
                        }
                    }
                }
            }
        }
        private void OnColumnAdded(object? sender, DataGridViewColumnEventArgs e)
        {
            // get the column left position
            int leftPosition = 0;
            foreach (DataGridViewColumn col in _targetGrid.Columns)
            {
                if (col == e.Column)
                {
                    AddOneColumn(col, ref leftPosition);
                    break;
                }
                leftPosition += col.Width;
            }
            
        }
        private void OnColumnRemoved(object? sender, DataGridViewColumnEventArgs e)
        {
            RemoveOneColumn(e.Column);
        }
        private void OnColumnWidthChanged(object? sender, DataGridViewColumnEventArgs e)
        {
            UpdateHeaderAndPanelPositions();
        }
        #endregion

        #region Sorting
        private void OnColumnHeaderClick(DataGridViewColumn col)
        {
            if (_targetGrid == null) return;

            if (!_columnSortOrders.TryGetValue(col, out SortOrder so))
                so = SortOrder.None;

            var next = (so == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;

            // Clear glyph from all
            foreach (DataGridViewColumn c in _targetGrid.Columns)
            {
                c.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
            // Sort
            _targetGrid.Sort(col, next == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
            col.HeaderCell.SortGlyphDirection = next;
            _columnSortOrders[col] = next;
        }
        #endregion

        #region Resizing
        /// <summary>
        /// Handles the click event for sorting when a column header is clicked, toggling the sort direction.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void CustomHeaderLabel_Click(object sender, EventArgs e)
        {
            if (isResizing)
            {
                return;
            }
            var headerLabel = sender as BeepLabel;
            if (headerLabel == null) return;

            if (!headerColumnMapping.TryGetValue(headerLabel, out var column))
                return;

            var newSortDirection = (Currentdirection == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            DataGridViewColumn oldColumn = _targetGrid.Columns[column.Name];
            if (SortColumn != oldColumn)
            {
                if (SortColumn != null)
                {
                    Currentdirection = SortOrder.None;
                    SortColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                    UpdateSortIcons(SortColumnLabel, SortOrder.None);
                }
                SortColumn = oldColumn;
                IsSorting = true;
                SortColumnLabel = headerLabel;
            }

            if (Currentdirection == SortOrder.None || oldColumn.SortMode == DataGridViewColumnSortMode.NotSortable)
            {
                oldColumn.SortMode = DataGridViewColumnSortMode.Automatic;
                newSortDirection = SortOrder.Ascending;
            }
            else if (Currentdirection == SortOrder.Ascending)
            {
                newSortDirection = SortOrder.Descending;
            }
            else if (Currentdirection == SortOrder.Descending)
            {
                oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                Currentdirection = SortOrder.None;
                _bindingSource.RemoveSort();
                UpdateSortIcons(headerLabel, SortOrder.None);
                return;
            }
            Currentdirection = newSortDirection;

            _targetGrid.Sort(SortColumn, newSortDirection == SortOrder.Ascending ? System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending);
            UpdateSortIcons(headerLabel, newSortDirection);
        }
        private void CustomHeaderLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (_targetGrid == null || !(sender is BeepLabel lbl)) return;
            _isResizing = true;
            _resizingLabel = lbl;
            _initialMousePos = e.Location;
            _initialColWidth = lbl.Width;
            _initialLabelLeft = lbl.Left;
            _resizingLeft = e.X < lbl.Width / 2;
        }
        private void CustomHeaderLabel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isResizing || _resizingLabel == null || _targetGrid == null) return;

            DataGridViewColumn col = null;
            foreach (var kvp in _headerLabels)
            {
                if (kvp.Value == _resizingLabel)
                {
                    col = kvp.Key;
                    break;
                }
            }

            if (col == null) return;

            int newWidth, newLeft;
            if (_resizingLeft)
            {
                newLeft = _initialLabelLeft + (e.X - _initialMousePos.X);
                newWidth = _initialColWidth - (e.X - _initialMousePos.X);
            }
            else
            {
                newWidth = _initialColWidth + (e.X - _initialMousePos.X);
                newLeft = _resizingLabel.Left;
            }

            if (newWidth > 0)
            {
                _resizingLabel.Width = newWidth;
                _resizingLabel.Left = newLeft;
                col.Width = newWidth;
                UpdateHeaderAndPanelPositions();
            }
        }
        private void CustomHeaderLabel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _isResizing)
            {
                _isResizing = false;
                _resizingLabel = null;
            }
        }

        #endregion

        #region Filtering
        /// <summary>
        /// Applies the current filters to the BindingSource of the grid.
        /// </summary>
        private void ApplyFilters()
        {
            string completeFilter = string.Join(" AND ", ColumnFilters.Values.Where(filter => !string.IsNullOrEmpty(filter)));
            Console.WriteLine("Applying Filter: " + completeFilter);
            if (string.IsNullOrEmpty(completeFilter))
            {
                _bindingSource.RemoveFilter();
            }
            else
            {
                _bindingSource.Filter = completeFilter;
            }
            _targetGrid.DataSource = _bindingSource;
        }
        private void ApplyFilter()
        {

       //     Console.WriteLine("Applying Filter");
            if (!_showFilter || _targetGrid == null) return;
            if (_targetGrid.DataSource is BindingSource bs)
            {
             //   Console.WriteLine("Applying Filter 1");
                var conditions = new List<string>();
                foreach (var kvp in _filterBoxes)
                {
                    var col = kvp.Key;
                    var txt = kvp.Value;
                    if (!string.IsNullOrWhiteSpace(txt.Text))
                    {
                        string prop = col.DataPropertyName;
                        if (string.IsNullOrEmpty(prop)) prop = col.Name;
                        string search = txt.Text.Replace("'", "''");
                        conditions.Add($"{prop} LIKE '%{search}%'");
                    }
                }
              //  Console.WriteLine("Conditions: " + string.Join(" AND ", conditions));
                _bindingSource.Filter = string.Join(" AND ", conditions);
                _targetGrid.Refresh();

              //  Console.WriteLine("Filter Applied End");
            }
        }
        #endregion

        #region Button Logic
        private void OnCsvExport()
        {
            Console.WriteLine("CSV export clicked.");
        }
        private void OnPrint()
        {
            Console.WriteLine("Print clicked.");
        }
        private void OnToggleTotals()
        {
            Console.WriteLine("Toggle Totals clicked.");
        }
        #endregion

        #region DataGridView Setup and Handling
        public void SetupDataGridView()
        {
            if (_targetGrid == null) return;
            _targetGrid.ColumnHeadersVisible = false;
            _targetGrid.RowHeadersVisible = false;
            _targetGrid.AllowUserToAddRows = false;
            _targetGrid.AllowUserToDeleteRows = false;
            _targetGrid.AllowUserToOrderColumns = false;
            _targetGrid.AllowUserToResizeRows = false;
            _targetGrid.AllowUserToResizeColumns = true;
            _targetGrid.AllowUserToResizeRows = false;
            _targetGrid.AutoGenerateColumns = false;
            _targetGrid.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            _targetGrid.CellBorderStyle = DataGridViewCellBorderStyle.None;
            _targetGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _targetGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _targetGrid.ColumnRemoved += OnColumnRemoved;
            _targetGrid.Scroll += _targetGrid_Scroll;
            _targetGrid.DataSourceChanged += OnDataSourceChanged;
            _targetGrid.DataMemberChanged += OnDataMemberChanged;
            _targetGrid.DataContextChanged += OnDataContextChanged;
            _targetGrid.DataBindingComplete += OnDataBindingComplete;
            _targetGrid.ColumnWidthChanged += DataGridView_ColumnWidthChanged;
            _targetGrid.CellEndEdit += DataGridView1_CellEndEdit;
            _targetGrid.CellBeginEdit += DataGridView1_CellBeginEdit;
            _targetGrid.DataError += DataGridView1_DataError;
            _targetGrid.BindingContextChanged += DataGridView1_BindingContextChanged;
          //  _bindingSource.DataSourceChanged += OnDataSourceChanged;
            
            RebuildColumnsAndFilters();
            // Hide the default scrollbar
            // _targetGrid.ScrollBars = ScrollBars.None;
        }
 
        #region DataGridView Events
        /// Handles the ColumnWidthChanged event for updating header and panel positions when a column's width changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="DataGridViewColumnEventArgs"/> that contains the event data.</param>
        private void DataGridView_ColumnWidthChanged(object? sender, DataGridViewColumnEventArgs e)
        {
            Console.WriteLine("Column Width Changed");
            // ColumnConfig with new width
            BeepGridColumnConfig cfg = ColumnConfigs[ColumnConfigs.FindIndex(p => p.ColumnName == e.Column.Name)];
            if (cfg == null) return;
            Console.WriteLine($"Column Width Changed: From {cfg.Width} to {e.Column.Width} ");
            cfg.Width = e.Column.Width;
            UpdateHeaderAndPanelPositions();
            _filterPanel.Update();
            _headerPanel.Update();
            Invalidate();
        }

        ///
        #endregion DataGridView Events
        #region DataGridView Cell Editing
        /// <summary>
        /// Event handler triggered when a filter TextBox value changes, updating the filters applied to the grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void FilterTextBox_TextChanged(object? sender, EventArgs e)
        {
            TextBox filterBox = sender as TextBox;
            var a = ColumnConfigs.Find(p => p.GuidID == filterBox.Tag.ToString());
            if (a == null)
            {
                return;
            }
            string columnName = a.ColumnName;

            // Update the filter for the column in the dictionary
            if (string.IsNullOrWhiteSpace(filterBox.Text))
            {
                ColumnFilters[columnName] = "";
            }
            else
            {
                ColumnFilters[columnName] = $"{columnName} LIKE '%{filterBox.Text}%'";
            }

            // Rebuild and apply the complete filter string
            ApplyFilters();
        }


        /// <summary>
        /// Handles the validation of the grid cell when it loses focus or the edit is being committed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the validation information.</param>
        private void DataGridView1_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            try
            {
                if (_targetGrid.IsCurrentCellDirty)
                {
                    _targetGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch (Exception ex)
            {
                //DMEEditor.AddLogMessage("Beep", $"Error in Cell Validation in Grid: {ex.Message}", DateTime.Now, 0, "", Errors.Failed);
            }
        }

        /// <summary>
        /// Event handler triggered when cell editing begins in the grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the column and row index.</param>
        private void DataGridView1_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            if (InQuery)
            {
                return;
            }
            decimal oldValue;
            if (_targetGrid.Columns[e.ColumnIndex] is BeepDataGridViewNumericColumn)
            {
                BeepGridColumnConfig cfg = ColumnConfigs[e.ColumnIndex];
                string cellValue = _targetGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                if (!string.IsNullOrEmpty(cellValue))
                {
                    decimal.TryParse(cellValue, out oldValue);
                }
                else
                {
                    oldValue = 0;
                }
                cfg.OldValue = oldValue;
            }
        }

        /// <summary>
        /// Event handler triggered when cell editing ends in the grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the column and row index.</param>
        private void DataGridView1_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (InQuery)
            {
                return;
            }
            if (ColumnConfigs != null && ColumnConfigs.Count > 0)
            {
                BeepGridColumnConfig cfg = ColumnConfigs[e.ColumnIndex];
                if (_targetGrid.Columns[e.ColumnIndex] is BeepDataGridViewNumericColumn)
                {
                    decimal newValue;
                    string cellValue = _targetGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                    if (!string.IsNullOrEmpty(cellValue) && decimal.TryParse(cellValue, out newValue))
                    {
                        cfg.Total = cfg.Total - cfg.OldValue + newValue;
                    }
                    else
                    {
                        cfg.Total = cfg.Total - cfg.OldValue;
                    }
                }
            }
        }
        /// <summary>
        /// Handles DataGridView data error events, logging error messages in the DMEEditor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="DataGridViewDataErrorEventArgs"/> that contains the event data.</param>
        private void DataGridView1_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            // DMEEditor.AddLogMessage("Error", $"Error in Grid {e.Exception.Message}", DateTime.Now, 0, "", Errors.Failed);
        }
        #endregion DataGridView Cell Editing
        #region DataGridView Scrolling
        private void DetachScrollBars()
        {


            // Sync horizontal scrolling

            if (verticalScrollBar != null)
            {
                verticalScrollBar.Scroll -= (s, e) => SyncVerticalScroll();
                _targetGrid.RowsAdded -= (s, e) => UpdateVerticalScrollBar();
                _targetGrid.RowsRemoved -= (s, e) => UpdateVerticalScrollBar();
                verticalScrollBar.Scroll -= (s, e) => SyncVerticalScroll();
                if (Parent != null) Parent.Controls.Remove(verticalScrollBar);
                verticalScrollBar.Dispose();
                verticalScrollBar = null;
            }

            if (horizontalScrollBar != null)
            {
                horizontalScrollBar.Scroll -= (s, e) => SyncHorizontalScroll();
                _targetGrid.ColumnWidthChanged -= (s, e) => UpdateHorizontalScrollBar();
                // Update scrollbars when TargetDataGridView resizes
                _targetGrid.SizeChanged -= (s, e) => RepositionScrollBars();
                _targetGrid.LocationChanged -= (s, e) => RepositionScrollBars();
                horizontalScrollBar.Scroll -= (s, e) => SyncHorizontalScroll();
                if (Parent != null) Parent.Controls.Remove(horizontalScrollBar);
                horizontalScrollBar.Dispose();
                horizontalScrollBar = null;
            }
        }

        private void AttachScrollBars()
        {
            if (_targetGrid == null || Parent == null) return;

            // Create and attach Vertical ScrollBar
            if (verticalScrollBar == null)
            {
                verticalScrollBar = new BeepScrollBar
                {
                    Width = 12,
                    Height = _targetGrid.Height,
                    Visible = true
                };
                Parent.Controls.Add(verticalScrollBar);
            }

            // Create and attach Horizontal ScrollBar
            if (horizontalScrollBar == null)
            {
                horizontalScrollBar = new BeepScrollBar
                {
                    Height = 12,
                    Width = _targetGrid.Width,
                    Visible = true
                };
                Parent.Controls.Add(horizontalScrollBar);
            }

            // Set initial positions
            RepositionScrollBars();
            verticalScrollBar.Scroll += (s, e) => SyncVerticalScroll();
            _targetGrid.RowsAdded += (s, e) => UpdateVerticalScrollBar();
            _targetGrid.RowsRemoved += (s, e) => UpdateVerticalScrollBar();

            // Sync horizontal scrolling
            horizontalScrollBar.Scroll += (s, e) => SyncHorizontalScroll();
            _targetGrid.ColumnWidthChanged += (s, e) => UpdateHorizontalScrollBar();
            // Update scrollbars when TargetDataGridView resizes
            _targetGrid.SizeChanged += (s, e) => RepositionScrollBars();
            _targetGrid.LocationChanged += (s, e) => RepositionScrollBars();
        }
        private void RepositionScrollBars()
        {
            if (_targetGrid == null || verticalScrollBar == null || horizontalScrollBar == null) return;

            // Position Vertical ScrollBar to the right of DataGridView
            verticalScrollBar.Location = new Point(_targetGrid.Right - verticalScrollBar.Width, _targetGrid.Top);
            verticalScrollBar.Height = _targetGrid.Height;

            // Position Horizontal ScrollBar at the bottom of DataGridView
            horizontalScrollBar.Location = new Point(_targetGrid.Left, _targetGrid.Bottom - horizontalScrollBar.Height);
            horizontalScrollBar.Width = _targetGrid.Width;

            // Ensure scrollbars are visible only when needed
            verticalScrollBar.Visible = _targetGrid.RowCount > _targetGrid.DisplayedRowCount(false);
            horizontalScrollBar.Visible = _targetGrid.Columns.Count > 0 && _targetGrid.Width < _targetGrid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
        }

        private void UpdateVerticalScrollBar()
        {
            if (_targetGrid.RowCount > 0)
            {
                verticalScrollBar.Maximum = _targetGrid.RowCount - 1;
                verticalScrollBar.LargeChange = _targetGrid.DisplayedRowCount(false);
            }
        }

        private void SyncVerticalScroll()
        {
            if (verticalScrollBar.Value < _targetGrid.RowCount)
            {
                _targetGrid.FirstDisplayedScrollingRowIndex = verticalScrollBar.Value;
            }
        }

        private void UpdateHorizontalScrollBar()
        {
            int totalWidth = 0;
            foreach (DataGridViewColumn col in _targetGrid.Columns)
                totalWidth += col.Width;

            if (totalWidth > _targetGrid.ClientSize.Width)
            {
                horizontalScrollBar.Maximum = totalWidth - _targetGrid.ClientSize.Width;
                horizontalScrollBar.LargeChange = _targetGrid.ClientSize.Width / 10; // Adjust scroll step
            }
        }

        private void SyncHorizontalScroll()
        {
            _targetGrid.HorizontalScrollingOffset = horizontalScrollBar.Value;
        }
        private void _targetGrid_Scroll(object? sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                UpdateHeaderAndPanelPositions();
            }
        }

        /// <summary>
        /// Updates the positions and sizes of headers, filter controls, and total controls based on the current grid layout.
        /// </summary>
        private void UpdateHeaderAndPanelPositions()
        {
            if (_targetGrid == null) return;

            // horizontal offset from the DataGridView's scrolling
            int offset = _targetGrid.HorizontalScrollingOffset;

            // ~~~~~ 1) Shift HEADER LABELS in _headerPanel ~~~~~
            int xPos = -offset;
            foreach (Control ctrl in _headerPanel.Controls)
            {
                if (ctrl is BeepLabel headerLabel)
                {
                    // find the DataGridViewColumn by label.Text (assuming it's the column's HeaderText)
                    var col = _targetGrid.Columns[headerLabel.Text];
                    if (col != null)
                    {
                        // find the matching ColumnConfig by guid
                        int idx = ColumnConfigs.FindIndex(c => c.ColumnName == col.Name);
                        if (idx >= 0)
                        {
                            // ensure the label's width matches the column's actual width
                            headerLabel.Width = col.Width;
                            // or ensure ColumnConfigs is updated:
                            ColumnConfigs[idx].Width = col.Width;
                        }

                        // position the label
                        headerLabel.Left = xPos;
                        xPos += headerLabel.Width;
                        if (ShowFilter)
                        {
                            //get filter textbox for the column
                            if (_filterBoxes.TryGetValue(col, out BeepTextBox filterBox))
                            {
                                // position the filter textbox
                                filterBox.Left = headerLabel.Left;
                                filterBox.Width = headerLabel.Width;
                            }

                        }
                    }
                    headerLabel.Invalidate();
                }
            }

            // ~~~~~ 2) Shift FILTER TEXTBOXES in _filterPanel ~~~~~
            xPos = -offset;
            foreach (Control ctrl in _filterPanel.Controls)
            {
                if (ctrl is BeepLabel filterBox)
                {
                    var col = _targetGrid.Columns[filterBox.Text];
                    // find the matching ColumnConfig by guid
                    int idx = ColumnConfigs.FindIndex(c => c.ColumnName == col.Name);
                    if (idx >= 0)
                    {
                        int w = ColumnConfigs[idx].Width;
                        filterBox.Left = xPos;
                        filterBox.Width = w;
                        xPos += w;
                    }
                    filterBox.Invalidate();
                }
            }

            // ~~~~~ 3) Notify the FOOTER to update positions too ~~~~~

        }

        #endregion DataGridView Scrolling
        #region DataGridView Column Handling
        /// <summary>
        /// Handles the ColumnRemoved event to remove column configurations, filters, and totals when a column is removed from the grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="DataGridViewColumnEventArgs"/> that contains the event data.</param>
        private void DataGridView1_ColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            // 1) Remove the ColumnConfig entry
            int cfgIndex = ColumnConfigs.FindIndex(cfg => cfg.Index == e.Column.Index);
            if (cfgIndex >= 0)
            {
                ColumnConfigs.RemoveAt(cfgIndex);
            }

            // 2) Remove the header label if present
            if (_headerLabels.TryGetValue(e.Column, out BeepLabel headerLabel))
            {
                // Removes the label from your header panel
                headerLabel.Parent?.Controls.Remove(headerLabel);
                headerLabel.Dispose();
                // Remove from dictionary
                _headerLabels.Remove(e.Column);
            }

            // 3) Remove the filter textbox if present
            if (_filterBoxes.TryGetValue(e.Column, out BeepTextBox filterBox))
            {
                // Removes the filter box from your filter panel
                filterBox.Parent?.Controls.Remove(filterBox);
                filterBox.Dispose();
                // Remove from dictionary
                _filterBoxes.Remove(e.Column);
            }

            // 4) Optionally re-run any alignment logic
            // e.g. UpdateHeaderAndPanelPositions();

            // 5) If you have an event to sync with the footer
          
        }

        /// Adds column configuration settings for a specified column, including filters and totals.
        /// </summary>
        /// <param name="column">The column to configure.</param>
        /// <param name="index">The index of the column.</param>
        /// <param name="width">The width of the column.</param>
        /// <param name="name">The name of the column.</param>
        /// <param name="headertext">The header text of the column.</param>

        #endregion DataGridView Column Handling
        #region DataGridView Column Sorting
        #endregion DataGridView Column Sorting
        #region DataGridView Column Filtering
        #endregion DataGridView Column Filtering

        #endregion DataGridView Setup and Handling

        #region Theme Management
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null)
            {
                return;
            }
            if (_targetGrid == null)
            {
                return;
            }
            // apply theme for grid
            this._targetGrid.BackgroundColor = _currentTheme.GridBackColor;
            this._targetGrid.DefaultCellStyle.BackColor = _currentTheme.GridBackColor;
            this._targetGrid.DefaultCellStyle.ForeColor = _currentTheme.GridForeColor;
            this._targetGrid.DefaultCellStyle.SelectionForeColor = _currentTheme.GridRowSelectedForeColor;
            this._targetGrid.DefaultCellStyle.SelectionBackColor = _currentTheme.GridRowSelectedBackColor;
            this._targetGrid.ColumnHeadersDefaultCellStyle.BackColor = _currentTheme.GridHeaderBackColor;
            this._targetGrid.ColumnHeadersDefaultCellStyle.ForeColor = _currentTheme.GridHeaderForeColor;
            this._targetGrid.GridColor = _currentTheme.GridLineColor;
            this._targetGrid.AlternatingRowsDefaultCellStyle.BackColor = _currentTheme.AltRowBackColor;
            this._targetGrid.AlternatingRowsDefaultCellStyle.ForeColor = _currentTheme.GridForeColor;
            this._targetGrid.DefaultCellStyle.SelectionBackColor = _currentTheme.GridRowSelectedBackColor;
            this._targetGrid.DefaultCellStyle.SelectionForeColor = _currentTheme.GridRowSelectedForeColor;

            // apply theme for header
            _headerPanel.BackColor = _currentTheme.PanelBackColor;
            BackColor = _currentTheme.PanelBackColor;
            _topTableLayout.BackColor = _currentTheme.PanelBackColor;
            this._filterPanel.BackColor = _currentTheme.PanelBackColor;
            foreach (Control ctrl in _headerPanel.Controls)
            {
                if (ctrl is BeepLabel lbl)
                {
                    lbl.Theme = Theme;
                }
            }
            foreach (Control ctrl in _filterPanel.Controls)
            {
                if (ctrl is BeepTextBox txt)
                {
                    txt.Theme = Theme;
                }
            }
            //apply theme for buttons
            this._filterToggleButton.Theme = Theme;
            this._totalShowButton.Theme = Theme;
            this._printButton.Theme = Theme;
            this._csvExportButton.Theme = Theme;
            //this.BindingNavigator.Theme = Theme;
            this._titleLabel.Theme = Theme;
            this._titleLabel.ForeColor = ColorUtils.GetForColor(_currentTheme.PanelBackColor, _currentTheme.LabelForeColor);
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
                _titleLabel.UseThemeFont = true;
            }
            else
            {
                _titleLabel.TextFont = _textFont;
            }
            _topTableLayout.RowStyles[0].Height = _titleLabel.PreferredSize.Height + (2 * (padding + 2)); // Adjust height dynamically

            
        }
        #endregion Theme Management

        #region Entity Management
        private void AddColumnConfigurations(DataGridViewColumn column, int index, int width, string name, string headertext, BeepLabel lbl = null)
        {
            if (ColumnConfigs.Count > 0 && ColumnConfigs.Exists(p => p.Index == index))
            {
                return;
            }
            BeepGridColumnConfig cfg = new BeepGridColumnConfig()
            {
                Index = index,
                ColumnName = name,
                ColumnCaption = headertext,
                Filter = "",
                HasTotal = false,
                IsFiltered = false,
                IsSorted = false,
                IsFilteOn = false,
                IsTotalOn = false,
                Width = width,

            };
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Resizable = DataGridViewTriState.True;

            // FilterBox.TextChanged += FilterTextBox_TextChanged;
            //_filterPanel.Controls.Add(FilterBox);
            ColumnConfigs.Add(cfg);
        }

        /// <summary>
        /// <summary>
        /// Dynamically creates columns in the grid based on the provided entity structure.
        /// </summary>
        public void CreateColumnsForEntity()
        {
            _targetGrid.SuspendLayout();
            _targetGrid.Columns.Clear();
            if (_targetGrid == null || Entity == null || Entity.Fields == null)
            {
                _targetGrid.ResumeLayout();
                return;
            }
            _targetGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _targetGrid.AllowUserToResizeColumns = true;
            try
            {
                foreach (var field in Entity.Fields)
                {
                    DataGridViewColumn column = null;

                    // Create appropriate column type based on the field's data type
                    switch (Type.GetType(field.fieldtype))
                    {
                        case Type type when type == typeof(string):
                            column = new DataGridViewTextBoxColumn();
                            break;
                        case Type type when type == typeof(int) || type == typeof(long) || type == typeof(short):
                            column = new BeepDataGridViewNumericColumn(); // Custom Numeric Column
                            column.ValueType = type;
                            break;
                        case Type type when type == typeof(decimal) || type == typeof(double) || type == typeof(float):
                            column = new BeepDataGridViewNumericColumn(); // Custom Numeric Column
                            column.ValueType = type;
                            break;
                        case Type type when type == typeof(bool):
                            column = new BeepDataGridViewThreeStateCheckBoxColumn(); // Custom Three State Checkbox Column
                            break;
                        case Type type when type == typeof(DateTime):
                            column = new BeepDataGridViewDateTimePickerColumn
                            {
                                ValueType = type,
                                DefaultCellStyle = { Format = "g" } // General date-time format
                            };
                            break;
                        case Type type when type.IsEnum:
                            column = new BeepDataGridViewComboBoxColumn() // Custom ComboBox Column with cascading support
                            {
                                DataSource = Enum.GetValues(type),
                                ValueType = type
                            };
                            break;
                        case Type type when type == typeof(Guid):
                            column = new DataGridViewTextBoxColumn();
                            break;
                        case Type type when type == typeof(object):
                            column = new DataGridViewTextBoxColumn
                            {
                                ValueType = typeof(string) // Display ObjectId as a string
                            };
                            break;
                        case Type type when type == typeof(float) || type == typeof(double) || type == typeof(decimal):
                            column = new BeepDataGridViewProgressBarColumn(); // Custom ProgressBar Column
                            break;
                        case Type type when type == typeof(List<string>): // or any List-based structure
                            column = new BeepDataGridViewMultiColumnColumn(); // Custom MultiColumn ComboBox
                            break;
                    }

                    if (column != null)
                    {
                        column.DataPropertyName = field.fieldname;
                        column.Name = field.fieldname;
                        column.HeaderText = field.fieldname;
                        column.Tag = Guid.NewGuid().ToString();
                        _targetGrid.Columns.Add(column);
                        AddColumnConfigurations(column, column.Index, column.Width, field.fieldname, field.fieldname);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error adding columns in Grid for Entity {Entity.EntityName}: {ex.Message}");
            }
            _targetGrid.ResumeLayout();
        }


        /// <summary>
        /// Resets the grid data and entity structure, and updates the grid columns based on the provided entity structure.
        /// </summary>
        /// <param name="data">The new data to bind to the grid.</param>
        /// <param name="entity">The entity structure defining the columns.</param>
        /// <returns>An <see cref="IErrorsInfo"/> object representing the result of the operation.</returns>
        public IErrorsInfo SetData(object data, EntityStructure entity)
        {
            if (entity != null)
            {
                Entity = entity;
            }
            else
            {
                LogMessage("Entity Structure is Null");
                return DMEEditor.ErrorObject;
            }


            if (_bindingSource != null && Entity != null)
            {
                _targetGrid.Columns.Clear();
                _bindingSource.ResetBindings(false);
                _bindingSource.DataSource = data;
                _targetGrid.DataSource = _bindingSource;
                CreateColumnsForEntity();
                ResetData();
               
                Title = Entity.EntityName;
                _titleLabel.Text = Entity.EntityName;
            }

            return DMEEditor.ErrorObject;
        }

        /// <summary>
        /// Resets the grid columns based on the provided entity structure.
        /// </summary>
        /// <param name="entity">The entity structure defining the columns.</param>
        /// <returns>An <see cref="IErrorsInfo"/> object representing the result of the operation.</returns>
        public IErrorsInfo ResetEntity(EntityStructure entity)
        {
            if (entity != null)
            {
                Entity = entity;
            }
            else
            {
                LogMessage("Entity Structure is Null");
                return DMEEditor.ErrorObject;
            }

            _targetGrid.Columns.Clear();
            if (_bindingSource != null && Entity != null)
            {
                ColumnConfigs.Clear();

                CreateColumnsForEntity();
                RebuildColumnsAndFilters();
                Title = Entity.EntityName;
                _titleLabel.Text = Entity.EntityName;
            }
            _bindingSource.ResetBindings(false);
            return DMEEditor.ErrorObject;
        }

        #endregion Entity Management

        #region Data Management
        private void DataGridView1_BindingContextChanged(object? sender, EventArgs e)
        {
            Console.WriteLine("Binding Context Changed");
            ResetData();
        }
        private void OnDataSourceChanged(object? sender, EventArgs e)
        {
            Console.WriteLine("Data Source Changed");
            ResetData();
        }
        private void ResetData()
        {
            Console.WriteLine("start Reset Data");
            if (_targetGrid == null) return;
            //   _bindingSource = _targetGrid.Data as BindingSource;
            // Clear existing column configurations
            _headerLabels.Clear();
            _filterBoxes.Clear();
            _columnSortOrders.Clear();
            ColumnConfigs.Clear();

            // Rebuild columns and filters
            Console.WriteLine("Rebuild Columns and Filters");
            RebuildColumnsAndFilters();

            // Apply filters if the filter panel is visible
            if (_showFilter)
            {
                ApplyFilter();
            }
          
            // Update the layout to match the new data source
            Console.WriteLine("Update Header and Panel Positions");
            UpdateHeaderAndPanelPositions();
            Console.WriteLine("Refresh");
            _targetGrid.Refresh();
        }
        private void OnDataMemberChanged(object? sender, EventArgs e)
        {
            Console.WriteLine("Data Member Changed");
            if (_targetGrid == null) return;
            _headerLabels.Clear();
            _filterBoxes.Clear();
            _columnSortOrders.Clear();
            ColumnConfigs.Clear();
            // Rebuild columns and filters to reflect the new data member
            RebuildColumnsAndFilters();

            // Update filters if the filter panel is visible
            if (_showFilter)
            {
                ApplyFilter();
            }

            // Refresh the header layout
            UpdateHeaderAndPanelPositions();
        }
        private void OnDataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (_targetGrid == null) return;

            // Ensure sorting icons are updated
            foreach (var kvp in _columnSortOrders)
            {
                var col = kvp.Key;
                var sortOrder = kvp.Value;
                if (_headerLabels.TryGetValue(col, out BeepLabel lbl))
                {
                    UpdateSortIcons(lbl, sortOrder);
                }
            }

            // Apply filters if the filter panel is visible
            if (_showFilter)
            {
                ApplyFilter();
            }

            // Notify the footer (if linked) to update totals
           
        }
        private void OnDataContextChanged(object? sender, EventArgs e)
        {
            Console.WriteLine("Data Context Changed");
            if (_targetGrid == null) return;

            // Rebuild columns and filters to reflect the new data context
            RebuildColumnsAndFilters();

            // Update filters if the filter panel is visible
            if (_showFilter)
            {
                ApplyFilter();
            }

            // Refresh the header layout
            UpdateHeaderAndPanelPositions();
        }
        #endregion Data Management

        #region Logging
        private void LogMessage(string message)
        {
            if (DMEEditor != null)
            {
                DMEEditor.AddLogMessage("Beep", message, DateTime.Now, 0, "", Errors.Ok);
            }
            else Console.WriteLine(message);
        }
        #endregion Logging
        #region "Footer Logic"
        #region Build & Update Totals

        /// <summary>
        /// Build a label for each column, stored in a dictionary for alignment & updates.
        /// </summary>
        private void BuildTotalsRow()
        {
            _totalsFlow.Controls.Clear();
            _columnTotals.Clear();

            if (_targetGrid == null) return;
            foreach (DataGridViewColumn col in _targetGrid.Columns)
            {
                // Create a label or textbox
                var lbl = new BeepLabel
                {
                    Text = col.HeaderText + ": 0",
                    AutoSize = false,
                    Width = col.Width,
                    Height = 24,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Tag = col,
                    GuidID = Guid.NewGuid().ToString()

                };
                _totalsFlow.Controls.Add(lbl);
                _columnTotals[col] = lbl;
            }
        }

        /// <summary>
        /// Recompute totals for numeric columns and place them in the labels.
        /// Skips if ShowTotalsPanel is false or no rows.
        /// </summary>
        public void UpdateTotals()
        {
            if (_targetGrid == null || !_showTotalsPanel) return;
            if (_targetGrid.Rows.Count == 0) return;

            foreach (var kvp in _columnTotals)
            {
                DataGridViewColumn col = kvp.Key;
                BeepLabel lbl = kvp.Value;

                if (IsNumericColumn(col))
                {
                    decimal sum = 0;
                    int rowCount = 0;

                    foreach (DataGridViewRow row in _targetGrid.Rows)
                    {
                        if (!row.IsNewRow && row.Cells[col.Index].Value != null)
                        {
                            if (decimal.TryParse(row.Cells[col.Index].Value.ToString(), out decimal val))
                            {
                                sum += val;
                                rowCount++;
                            }
                        }
                    }
                    lbl.Text = $"{col.HeaderText}: {sum}";
                }
                else
                {
                    lbl.Text = $"{col.HeaderText}";
                }
            }
        }
        #region Helpers

        private bool IsNumericColumn(DataGridViewColumn col)
        {
            var t = col.ValueType;
            if (t == null) return false;
            return (t == typeof(int) || t == typeof(long) ||
                    t == typeof(float) || t == typeof(double) ||
                    t == typeof(decimal));
        }

        #endregion
        #endregion

        #region Binding Navigator

       

       

        #endregion



        #endregion "Footer Logic"
    }
}
