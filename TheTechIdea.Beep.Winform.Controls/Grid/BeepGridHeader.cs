using System;
using System.Collections.Generic;
using System.ComponentModel;

using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Logic;
using TheTechIdea.Beep.Vis.Modules;

using TheTechIdea.Beep.Winform.Controls.Grid.DataColumns; // If your code references it

namespace TheTechIdea.Beep.Winform.Controls.Grid
{
    [ToolboxItem(true)]
    [DisplayName("Beep Grid Header")]
    [Category("Beep Controls")]
    public class BeepGridHeader : BeepControl
    {
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
        private List<BeepGridColumnConfig> ColumnConfigs = new List<BeepGridColumnConfig>();
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

        private EventHandler RefershFooter { get; set; }
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
        /// <summary>
        public BeepGridHeader()
        {
            // By default, no filter => total height = top + header => 60
            InitializeLayout();
            _bindingSource = new BindingSource();
            this.LocationChanged += Header_LocationChanged;
           this.SizeChanged += Header_SizeChanged;
        }

      

        #region Public Properties

        public IDMEEditor DMEEditor { get; set; }
        private EntityStructure _entity;
        /// Controls the visibility of the navigator panel.
        /// </summary>
        ///  [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowNavigatorPanel
        {
            get
            {
                if(TargetFooter== null)
                {
                    return false;
                }
                return TargetFooter.ShowDataNavigator;
            }
            set
            {
                if(TargetFooter != null)   TargetFooter.ShowDataNavigator = value;

            }
        }
        private BindingSource _bindingSource=new BindingSource();
        /// <summary>
        /// Gets or sets the BindingSource used by the grid's navigator.
        /// </summary>
        public BindingSource DataBindingSource
        {
            get { return _bindingSource; }
            set
            {
                InQuery = true;
               
                _bindingSource = value;
              //  if (TargetDataGridView != null) TargetDataGridView.DataSource = _bindingSource;
                
                InQuery = false;
            }
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
                if(_titleLabel != null)
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

        public EntityStructure Entity
        {
            get { return _entity; }
            set { _entity = value; }
        }

        /// <summary>
        /// The DataGridView we attach to. We'll follow its location/size changes, and build column headers.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [Description("The DataGridView that this BeepGridHeader syncs with and positions above.")]
        public DataGridView TargetDataGridView
        {
            get => _targetGrid;
            set
            {
                if (_targetGrid != null)
                {
                    _targetGrid.LocationChanged -= TargetMoved;
                    _targetGrid.SizeChanged -= TargetMoved;
                    _targetGrid.ColumnAdded -= OnColumnAdded;
                    _targetGrid.ColumnRemoved -= OnColumnRemoved;
                    _targetGrid.Scroll -= _targetGrid_Scroll;
                    _targetGrid.ColumnWidthChanged -= OnColumnWidthChanged;
                    _targetGrid.DataSourceChanged -= OnDataSourceChanged;
                    _targetGrid.DataMemberChanged -= OnDataMemberChanged;
                    _targetGrid.DataContextChanged -= OnDataContextChanged;
                    _targetGrid.DataBindingComplete -= OnDataBindingComplete;
                    _targetGrid.ColumnWidthChanged -= DataGridView_ColumnWidthChanged;
                    _targetGrid.CellEndEdit -= DataGridView1_CellEndEdit;
                    _targetGrid.CellBeginEdit-= DataGridView1_CellBeginEdit;
                    _targetGrid.DataError -= DataGridView1_DataError;
                }
                _targetGrid = value;
                if (_targetGrid != null)
                {
                    SetupDataGridView();
                    _targetGrid.LocationChanged += TargetMoved;
                    _targetGrid.SizeChanged += TargetMoved;
                    _targetGrid.ColumnAdded += OnColumnAdded;
                    _targetGrid.ColumnRemoved += OnColumnRemoved;
                    _targetGrid.Scroll += _targetGrid_Scroll;
                    _targetGrid.ColumnWidthChanged += OnColumnWidthChanged;
                    _targetGrid.DataSourceChanged += OnDataSourceChanged;
                    _targetGrid.DataMemberChanged += OnDataMemberChanged;
                    _targetGrid.DataContextChanged += OnDataContextChanged;
                    _targetGrid.DataBindingComplete += OnDataBindingComplete;
                    _targetGrid.ColumnWidthChanged += DataGridView_ColumnWidthChanged;
                    _targetGrid.CellEndEdit += DataGridView1_CellEndEdit;
                    _targetGrid.CellBeginEdit += DataGridView1_CellBeginEdit;
                    _targetGrid.DataError += DataGridView1_DataError;
                    Reposition();
                    RebuildColumnsAndFilters();
                    
                }
            }
        }

      
        /// <summary>
        /// Optionally link to a BeepGridFooter so that if the footer moves, we check if the grid moves, etc.
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        public BeepGridFooter TargetFooter
        {
            get => _linkedFooter;
            set
            {
                if (_linkedFooter != null)
                {
                    _linkedFooter.LocationChanged -= TargetMoved;
                    _linkedFooter.SizeChanged -= TargetMoved;
                }
                _linkedFooter = value;
                if (_linkedFooter != null)
                {
                    _linkedFooter.LocationChanged += TargetMoved;
                    _linkedFooter.SizeChanged += TargetMoved;
                    Reposition();
                }
            }
        }

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
                if(_filterPanel != null)
                {
                    _filterPanel.Visible = value;
                    RecalcHeight();
                    if (value)
                    {
                        RebuildColumnsAndFilters(); // so we build textboxes
                    }
                    Reposition();
                }
               
            }
        }

        /// <summary>
        /// Title in the top panel label
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        public string Title
        {
            get => _titleLabel.Text;
            set
            {
                if (_titleLabel != null)
                {
                    _titleLabel.Text = value;
                    RecalcHeight();
                    Reposition();
                }
            }
        }
        public bool InQuery { get; private set; }

        /// <summary>
        /// Tracks whether a column is currently being resized.
        /// </summary>
        private bool isResizing = false;

        /// <summary>
        /// Tracks if the left side of a column is being resized.
        /// </summary>
        private bool isResizingLeft = false;

        /// <summary>
        /// Stores the initial mouse position when resizing begins.
        /// </summary>
        private Point initialMousePosition;

        /// <summary>
        /// Stores the initial width of the column being resized.
        /// </summary>
        private int initialColumnWidth;

        /// <summary>
        /// Stores the initial left position of the label when resizing.
        /// </summary>
        private int initialLabelLeft;

        /// <summary>
        /// Reference to the label being resized.
        /// </summary>
        private Label resizingLabel;

        /// <summary>
        /// The current column being sorted.
        /// </summary>
        private DataGridViewColumn SortColumn = null;

        /// <summary>
        /// The current label being used for sorting.
        /// </summary>
        private BeepLabel SortColumnLabel = null;

        /// <summary>
        /// The current direction of sorting (Ascending, Descending, or None).
        /// </summary>
        SortOrder Currentdirection = SortOrder.None;

        /// <summary>
        /// Indicates if sorting is currently applied on the grid.
        /// </summary>
        private bool IsSorting = false;
        #endregion
        #region Layout Setup
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            isinit = false;
        }
        private void InitializeLayout()
        {
            _bindingSource = new BindingSource();
            _bindingSource.DataSourceChanged += OnDataSourceChanged;
            Console.WriteLine("Initializing BeepGridHeader layout...");
            // Manually stack three panels
            Height = 60; // 60 if no filter
            Width = 600;  // arbitrary placeholder
            Console.WriteLine("Height: " + Height);
            // (1) top panel
            _topPanel = new Panel
            {
                Height = _topPanelHeight,
                Dock = DockStyle.Top
            };
            Console.WriteLine("Top Panel Height: " + _topPanelHeight);
            // inside topPanel, we place a TableLayout for your 5 icons + label
            _topTableLayout = new TableLayoutPanel
            {
                ColumnCount = 6,
                RowCount = 1,
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            Console.WriteLine("Table Layout Column Count: " + _topTableLayout.ColumnCount);
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            _topTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            _topTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 23f));
            Console.WriteLine("Table Layout Row Count: " + _topTableLayout.RowCount);
            _topPanel.Controls.Add(_topTableLayout);
            Console.WriteLine("Top Panel Controls Count: " + _topPanel.Controls.Count);
            // Create beepbuttons + label
            _csvExportButton = new BeepButton
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.export.svg",
                Size = new Size(15, 15),
                MaxImageSize = new Size(14, 14),
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFramless = true,
                IsChild = true,
                Dock = DockStyle.Fill
            };
            _printButton = new BeepButton
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.print.svg",
                Size = new Size(15, 15),
                MaxImageSize = new Size(14, 14),
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFramless = true,
                IsChild = true,
                Dock = DockStyle.Fill
            };
            _shareButton = new BeepButton
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.share.svg",
                Size = new Size(15, 15),
                MaxImageSize = new Size(14, 14),
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFramless = true,
                IsChild = true,
                Dock = DockStyle.Fill
            };
            _totalShowButton = new BeepButton
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.sum.svg",
                Size = new Size(15, 15),
                MaxImageSize = new Size(14, 14),
                ImageAlign = ContentAlignment.MiddleCenter,
                HideText = true,
                IsFramless = true,
                IsChild = true,
                Dock = DockStyle.Fill
            };
            _filterToggleButton = new BeepButton
            {
                ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.search.svg",
                Size = new Size(15, 15),
                MaxImageSize = new Size(14, 14),
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
            Console.WriteLine("Buttons Created");
            // Add them to table
            _topTableLayout.Controls.Add(_csvExportButton, 0, 0);
            _topTableLayout.Controls.Add(_totalShowButton, 1, 0);
            _topTableLayout.Controls.Add(_shareButton, 2, 0);
            _topTableLayout.Controls.Add(_printButton, 3, 0);
            _topTableLayout.Controls.Add(_filterToggleButton, 4, 0);
            _topTableLayout.Controls.Add(_titleLabel, 5, 0);
            Console.WriteLine("Controls Added to Table");
            // Hook up events
            _csvExportButton.Click += (s, e) => OnCsvExport();
            _printButton.Click += (s, e) => OnPrint();
            _totalShowButton.Click += (s, e) => OnToggleTotals();
            _filterToggleButton.Click += (s, e) => ShowFilter = !ShowFilter;
            Console.WriteLine("Events Hooked Up");
            // (2) column header row
            // --- Header Panel (for column labels) ---
            _headerPanel = new Panel
            {
                Height = 30,  // Ensure a fixed height
                Dock = DockStyle.Top,
                BackColor = Color.White
            };
            Console.WriteLine("Header Panel Height: " + _headerPanelHeight);
           ;
            Console.WriteLine("Header Panel Controls Count: " + _headerPanel.Controls.Count);
            // (3) filter row (optional)
            // --- Filter Panel (for filter textboxes) ---
            _filterPanel = new Panel
            {
                Height = 25,  // Ensure a fixed height (or your desired height)
                Dock = DockStyle.Top,
                Visible = _showFilter,
                BackColor = Color.WhiteSmoke
            };
            Console.WriteLine("Filter Panel Height: " + _filterPanelHeight);
           
          

            // Add them from bottom to top
            Controls.Add(_filterPanel);
            Controls.Add(_headerPanel);
            Controls.Add(_topPanel);
            Console.WriteLine("Controls Added to BeepGridHeader");
            RecalcHeight();
            Console.WriteLine("Height Recalculated");
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
            this.Height = _topPanel.Height + headerHeight + filterHeight;
        }

        #endregion
        #region Movement Sync
        private void Header_LocationChanged(object sender, EventArgs e)
        {
            if (isinit )
            {
                return;
            }
            if (_isUpdating) return;
            _isUpdating = true;

            try
            {
                if (_targetGrid != null)
                {
                    _targetGrid.Top = this.Bottom;
                    RepositionFooter();
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void Header_SizeChanged(object sender, EventArgs e)
        {
            if (isinit)
            {
                return;
            }
            if (_isUpdating) return;
            _isUpdating = true;

            try
            {
                if (_targetGrid != null)
                {
                    // Ensure Grid and Footer widths match Header width
                    _targetGrid.Width = this.Width;
                    _targetGrid.Left = this.Left;

                    if (_linkedFooter != null)
                    {
                        _linkedFooter.Width = this.Width;
                        _linkedFooter.Left = this.Left;
                    }

                    // Adjust Grid Height based on Footer position
                    int newGridHeight = (_linkedFooter != null)
                        ? (_linkedFooter.Top - this.Bottom)
                        : (_targetGrid.Parent.Height - this.Bottom);

                    _targetGrid.Top = this.Bottom;
                    _targetGrid.Height = Math.Max(50, newGridHeight);

                    // Ensure Footer is repositioned properly
                    RepositionFooter();
                }
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void RepositionFooter()
        {
            if (_linkedFooter != null && _targetGrid != null)
            {
                _linkedFooter.Top = _targetGrid.Bottom;
            }
        }
        private void TargetMoved(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            try
            {
                Reposition();
            }
            finally
            {
                _isUpdating = false;
            }
        }

        private void Reposition()
        {
            // place ourselves above the grid
            if (_targetGrid == null || _targetGrid.Parent == null) return;

            if (Parent != _targetGrid.Parent)
            {
                Parent?.Controls.Remove(this);
                _targetGrid.Parent.Controls.Add(this);
            }

            Width = _targetGrid.Width;
            Left = _targetGrid.Left;
            Top = _targetGrid.Top - this.Height;
        }
        #endregion

        #region Column Building
        private void RebuildColumnsAndFilters()
        {
            // Clear existing controls and dictionaries
            _headerPanel.Controls.Clear();
            _filterPanel.Controls.Clear();
            _headerLabels.Clear();
            _filterBoxes.Clear();
            _columnSortOrders.Clear();
            ColumnConfigs.Clear();

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
                Width = col.Width,                         // Use the grid column's width.
                Height = _headerPanel.Height,              // Use the header panel's fixed height.
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = col,
                GuidID = Guid.NewGuid().ToString(),
                Location = new Point(leftPosition, 0),
                AutoSize = false,
                 
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
                    GuidID =lbl.GuidID,
                    Location = new Point(leftPosition, 0),
                    AutoSize = false
                };
                txt.TextChanged += (s, e) => ApplyFilter();
                _filterPanel.Controls.Add(txt);
                _filterBoxes[col] = txt;
            }

            // Update the left position for the next control.
            leftPosition += col.Width;

            // Optionally, update your column configurations.
            AddColumnConfigurations(col, col.Index, col.Width, col.DataPropertyName, col.DataPropertyName,lbl);
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
            foreach (DataGridViewColumn column in TargetDataGridView.Columns)
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
                    GuidID = Guid.NewGuid().ToString(),
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
        private void OnColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            //AddOneColumn(e.Column,);
        }
        private void OnColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            RemoveOneColumn(e.Column);
        }
        private void OnColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (_headerLabels.TryGetValue(e.Column, out BeepLabel lbl))
            {
                lbl.Width = e.Column.Width;
            }
            if (_filterBoxes.TryGetValue(e.Column, out BeepTextBox txt))
            {
                txt.Width = e.Column.Width;
            }
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
        /// Handles the MouseDown event for custom column header resizing, initializing the resize operation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        //private void CustomHeaderLabel_MouseDown(object sender, MouseEventArgs e)
        //{
        //    TargetDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        //    TargetDataGridView.AllowUserToResizeColumns = true;
        //    var headerLabel = sender as Label;
        //    if (headerLabel == null) return;

        //    if (e.Button == MouseButtons.Left)
        //    {
        //        initialMousePosition = e.Location;
        //        initialColumnWidth = headerLabel.Width;
        //        initialLabelLeft = headerLabel.Left;
        //        isResizing = true;
        //        resizingLabel = headerLabel;

        //        // Determine if resizing from left or right edge
        //        isResizingLeft = e.X < headerLabel.Width / 2;
        //    }
        //}

        /// Handles the MouseMove event for custom column header resizing, updating the column width during resize.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        //private void CustomHeaderLabel_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (isResizing && resizingLabel != null)
        //    {
        //        if (isResizingLeft)
        //        {
        //            int newLeft = initialLabelLeft + (e.X - initialMousePosition.X);
        //            int newWidth = initialColumnWidth - (e.X - initialMousePosition.X);

        //            if (newWidth > 0)
        //            {
        //                resizingLabel.Width = newWidth;
        //                resizingLabel.Left = newLeft;
        //                var column = headerColumnMapping[resizingLabel];
        //                column.Width = newWidth;
        //                UpdateHeaderAndPanelPositions();
        //            }
        //        }
        //        else
        //        {
        //            int newWidth = initialColumnWidth + (e.X - initialMousePosition.X);

        //            if (newWidth > 0)
        //            {
        //                resizingLabel.Width = newWidth;
        //                var column = headerColumnMapping[resizingLabel];
        //                column.Width = newWidth;
        //                UpdateHeaderAndPanelPositions();
        //            }
        //        }
        //    }
        //}

       
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
            DataGridViewColumn oldColumn = TargetDataGridView.Columns[column.Name];
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
                DataBindingSource.RemoveSort();
                UpdateSortIcons(headerLabel, SortOrder.None);
                return;
            }
            Currentdirection = newSortDirection;

            TargetDataGridView.Sort(SortColumn, newSortDirection == SortOrder.Ascending ? System.ComponentModel.ListSortDirection.Ascending : System.ComponentModel.ListSortDirection.Descending);
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
        /// <summary>

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
                DataBindingSource.RemoveFilter();
            }
            else
            {
                DataBindingSource.Filter = completeFilter;
            }
            TargetDataGridView.DataSource = DataBindingSource;
        }
        private void ApplyFilter()
        {
            if (!_showFilter || _targetGrid == null) return;
            if (_targetGrid.DataSource is BindingSource bs)
            {
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
                        conditions.Add($"[{prop}] LIKE '%{search}%'" );
                    }
                }
                bs.Filter = string.Join(" AND ", conditions);
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
            // Set up the DataGridView to use this header
            _targetGrid.Controls.Add(this);
       //     _targetGrid.ColumnHeadersHeight = _headerPanelHeight;
            _targetGrid.ColumnHeadersVisible = false;
            _targetGrid.RowHeadersVisible = false;
            _targetGrid.AllowUserToAddRows = false;
            _targetGrid.AllowUserToDeleteRows = false;
            _targetGrid.AllowUserToOrderColumns = false;
            _targetGrid.AllowUserToResizeRows = false;
            _targetGrid.AllowUserToResizeColumns = true;
            _targetGrid.AllowUserToResizeRows = false;
          
        }
        #region DataGridView Events
        /// Handles the ColumnWidthChanged event for updating header and panel positions when a column's width changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="DataGridViewColumnEventArgs"/> that contains the event data.</param>
        private void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            UpdateHeaderAndPanelPositions();
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
                if (TargetDataGridView.IsCurrentCellDirty)
                {
                    TargetDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
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
        private void DataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (InQuery)
            {
                return;
            }
            decimal oldValue;
            if (TargetDataGridView.Columns[e.ColumnIndex] is BeepDataGridViewNumericColumn)
            {
                BeepGridColumnConfig cfg = ColumnConfigs[e.ColumnIndex];
                string cellValue = TargetDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

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
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (InQuery)
            {
                return;
            }
            if (ColumnConfigs != null && ColumnConfigs.Count > 0)
            {
                BeepGridColumnConfig cfg = ColumnConfigs[e.ColumnIndex];
                if (TargetDataGridView.Columns[e.ColumnIndex] is BeepDataGridViewNumericColumn)
                {
                    decimal newValue;
                    string cellValue = TargetDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

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
        private void _targetGrid_Scroll(object? sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                UpdateHeaderAndPanelPositions();
            }
        }

        private void DataGridView_Scroll(object sender, ScrollEventArgs e)
        {
            
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
                    var col = TargetDataGridView.Columns[headerLabel.Text];
                    if (col != null && headerLabel.GuidID is string guid)
                    {
                        // find the matching ColumnConfig by guid
                        int idx = ColumnConfigs.FindIndex(c => c.GuidID == guid);
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
                    }
                }
            }

            // ~~~~~ 2) Shift FILTER TEXTBOXES in _filterPanel ~~~~~
            xPos = -offset;
            foreach (Control ctrl in _filterPanel.Controls)
            {
                if (ctrl is BeepTextBox filterBox && filterBox.GuidID is string filterGuid)
                {
                    // find the matching ColumnConfig
                    int idx = ColumnConfigs.FindIndex(c => c.GuidID == filterGuid);
                    if (idx >= 0)
                    {
                        int w = ColumnConfigs[idx].Width;
                        filterBox.Left = xPos;
                        filterBox.Width = w;
                        xPos += w;
                    }
                }
            }

            // ~~~~~ 3) Notify the FOOTER to update positions too ~~~~~
            RefershFooter?.Invoke(this, EventArgs.Empty);
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
            RefershFooter?.Invoke(this, EventArgs.Empty);
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
            if(_currentTheme == null)
            {
                return;
            }
            if(TargetDataGridView== null)
            {
                return;
            }
            // apply theme for grid
            this.TargetDataGridView.BackgroundColor = _currentTheme.GridBackColor;
            this.TargetDataGridView.DefaultCellStyle.BackColor = _currentTheme.GridBackColor;
            this.TargetDataGridView.DefaultCellStyle.ForeColor = _currentTheme.GridForeColor;
            this.TargetDataGridView.DefaultCellStyle.SelectionForeColor = _currentTheme.GridRowSelectedForeColor;
            this.TargetDataGridView.DefaultCellStyle.SelectionBackColor = _currentTheme.GridRowSelectedBackColor;
            this.TargetDataGridView.ColumnHeadersDefaultCellStyle.BackColor = _currentTheme.GridHeaderBackColor;
            this.TargetDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = _currentTheme.GridHeaderForeColor;
            this.TargetDataGridView.GridColor = _currentTheme.GridLineColor;
            this.TargetDataGridView.AlternatingRowsDefaultCellStyle.BackColor = _currentTheme.AltRowBackColor;
            this.TargetDataGridView.AlternatingRowsDefaultCellStyle.ForeColor = _currentTheme.GridForeColor;
            this.TargetDataGridView.DefaultCellStyle.SelectionBackColor = _currentTheme.GridRowSelectedBackColor;
            this.TargetDataGridView.DefaultCellStyle.SelectionForeColor = _currentTheme.GridRowSelectedForeColor;

            // apply theme for header
            _headerPanel.BackColor = _currentTheme.PanelBackColor;
            BackColor = _currentTheme.PanelBackColor;
            _topTableLayout.BackColor = _currentTheme.PanelBackColor;
            this._filterPanel.BackColor = _currentTheme.PanelBackColor;
           
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

            if (TargetFooter != null)
            {
                TargetFooter.Theme = Theme;
            }
        }
        #endregion Theme Management

        #region Entity Management
        private void AddColumnConfigurations(DataGridViewColumn column, int index, int width, string name, string headertext,BeepLabel lbl=null)
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
                GuidID = lbl== null ? Guid.NewGuid().ToString() : lbl.GuidID,
            };

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
            TargetDataGridView.SuspendLayout();
            TargetDataGridView.Columns.Clear();
            if (TargetDataGridView == null || Entity == null || Entity.Fields == null)
            {
                TargetDataGridView.ResumeLayout();
                return;
            }
            TargetDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            TargetDataGridView.AllowUserToResizeColumns = true;
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
                        TargetDataGridView.Columns.Add(column);
                        AddColumnConfigurations(column, column.Index, column.Width, field.fieldname, field.fieldname);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error adding columns in Grid for Entity {Entity.EntityName}: {ex.Message}");
            }
            TargetDataGridView.ResumeLayout();
        }


        /// <summary>
        /// Resets the grid data and entity structure, and updates the grid columns based on the provided entity structure.
        /// </summary>
        /// <param name="data">The new data to bind to the grid.</param>
        /// <param name="entity">The entity structure defining the columns.</param>
        /// <returns>An <see cref="IErrorsInfo"/> object representing the result of the operation.</returns>
        public IErrorsInfo ResetData(object data, EntityStructure entity)
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

            TargetDataGridView.Columns.Clear();
            if (DataBindingSource != null && Entity != null)
            {
                ColumnConfigs.Clear();
             
                RefershFooter?.Invoke(this, EventArgs.Empty);
                CreateColumnsForEntity();
                RebuildColumnsAndFilters();
                Title = Entity.EntityName;
                _titleLabel.Text = Entity.EntityName;
            }
            DataBindingSource.ResetBindings(false);
            DataBindingSource.DataSource = data;
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
                LogMessage( "Entity Structure is Null");
                return DMEEditor.ErrorObject;
            }

            TargetDataGridView.Columns.Clear();
            if (DataBindingSource != null && Entity != null)
            {
                ColumnConfigs.Clear();
              
                RefershFooter?.Invoke(this, EventArgs.Empty);

                CreateColumnsForEntity();
                RebuildColumnsAndFilters();
                Title = Entity.EntityName;
                _titleLabel.Text = Entity.EntityName;
            }
            DataBindingSource.ResetBindings(false);
            return DMEEditor.ErrorObject;
        }

        #endregion Entity Management

        #region Data Management
        private void OnDataSourceChanged(object? sender, EventArgs e)
        {
            if (_targetGrid == null) return;
            DataBindingSource = _targetGrid.DataSource as BindingSource;
            // Clear existing column configurations
            _headerLabels.Clear();
            _filterBoxes.Clear();
            _columnSortOrders.Clear();

            // Rebuild columns and filters
            RebuildColumnsAndFilters();

            // Apply filters if the filter panel is visible
            if (_showFilter)
            {
                ApplyFilter();
            }

            // Update the layout to match the new data source
            UpdateHeaderAndPanelPositions();
        }
        private void OnDataMemberChanged(object? sender, EventArgs e)
        {
            if (_targetGrid == null) return;

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
            RefershFooter?.Invoke(this, EventArgs.Empty);
        }

        private void OnDataContextChanged(object? sender, EventArgs e)
        {
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
            if(DMEEditor != null)
            {
                DMEEditor.AddLogMessage("Beep", message, DateTime.Now, 0, "", Errors.Ok);
            }else           Console.WriteLine(message);
        }
        #endregion Logging
    }
}
