
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.GridX.Helpers;
using TheTechIdea.Beep.Winform.Controls.GridX.Painters;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Converters;
 
using TheTechIdea.Beep.Vis.Modules;
using Math = System.Math;
using TheTechIdea.Beep.Winform.Controls.GridX.Layouts;

namespace TheTechIdea.Beep.Winform.Controls.GridX
{
    

    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Refactored, helper-driven grid control inspired by BeepSimpleGrid.")]
    [DisplayName("Beep Grid Pro")]
    [ComplexBindingProperties("DataSource", "DataMember")] // Enable designer complex data binding support
    public  class BeepGridPro : BaseControl
    {
        // Windows API for preventing flicker
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
        
        private const int WM_SETREDRAW = 0x000B;
        private const int WM_ERASEBKGND = 0x0014;

        // Dedicated host layer for in-place editors (DevExpress-like practice)
       // private readonly Panel _editorHost;
      //  internal Control EditorHost => _editorHost;

        internal Helpers.GridLayoutHelper Layout { get; }
        internal Helpers.GridDataHelper Data { get; }
        internal Helpers.GridRenderHelper Render { get; }
        internal Helpers.GridSelectionHelper Selection { get; }
        internal Helpers.GridInputHelper Input { get; }
        internal Helpers.GridScrollHelper Scroll { get; }
        internal Helpers.GridScrollBarsHelper ScrollBars { get; }
        internal Helpers.GridSortFilterHelper SortFilter { get; }
        internal Helpers.GridEditHelper Edit { get; }
        internal Helpers.GridThemeHelper ThemeHelper { get; }
        internal Helpers.GridNavigatorHelper Navigator { get; }
        internal Helpers.GridNavigationPainterHelper NavigatorPainter { get; }
        internal Helpers.GridSizingHelper Sizing { get; }
        internal Helpers.GridDialogHelper Dialog { get; }

        // Data management fields (similar to BeepSimpleGrid)
        internal Type _entityType;
        internal List<object> _fullData;
        internal int _dataOffset = 0;

        private GridUnitOfWorkBinder _uowBinder;
        private object _uow;

        [Browsable(true)]
        [Category("Data")] 
        [Description("Assign an IUnitofWork<T> instance. When set, its Units will be used as the grid data source and kept in sync.")]
        [TypeConverter(typeof(UnitOfWorksConverter))]
        public object Uow
        {
            get => _uow;
            set
            {
                if (!ReferenceEquals(_uow, value))
                {
                    _uow = value;
                    Navigator.SetUnitOfWork(_uow);
                }
            }
        }

        [Browsable(true)]
        [Category("Data")]
        [AttributeProvider(typeof(IListSource))] // Show BindingSource and list-like components in designer
        [DefaultValue(null)]
        public object DataSource 
        {
            get => Data.DataSource;
            set { 
                if (!ReferenceEquals(Data.DataSource, value))
                {
                    Data.Bind(value); // Bind to original data source
                    Navigator.BindTo(value); 
                    Data.InitializeData(); // Sync data after binding
                    Layout.Recalculate(); 
                    if (!DesignMode) Invalidate();
                }
            }
        }

        // Optional DataMember to support ComplexBindingProperties at design-time
        private string _dataMember = string.Empty;
        [Browsable(true)]
        [Category("Data")]
        [DefaultValue("")]
        [TypeConverter(typeof(BeepDataMemberConverter))]
        public string DataMember 
        { 
            get => _dataMember; 
            set 
            { 
                if (_dataMember != value) 
                { 
                    _dataMember = value ?? string.Empty; 
                    // Rebind to apply the new DataMember when possible
                    if (Data.DataSource != null) 
                    { 
                        Navigator.BindTo(Data.DataSource); // Re-bind navigator with new DataMember
                        Data.Bind(Data.DataSource); // Re-bind data with original data source
                        Data.InitializeData(); // Re-sync data with new DataMember
                        Invalidate(); 
                    }
                } 
            } 
        }

        // Expose columns collection for design-time editing and serialization
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        public BeepGridColumnConfigCollection Columns => Data.Columns;

        [Browsable(false)]
        public BindingList<BeepRowConfig> Rows => Data.Rows;

        /// <summary>
        /// Gets the number of rows that can fit in the current visible area (viewport).
        /// This is dynamically calculated based on RowsRect.Height / RowHeight.
        /// Used for pagination calculations - represents the "page size" for navigation.
        /// </summary>
        [Browsable(false)]
        [Description("The number of rows that can fit in the current visible area (dynamic page size for pagination)")]
        public int VisibleRowCapacity => Render?.GetVisibleRowCapacity() ?? 10;

        [Browsable(false)]
        internal Type EntityType => _entityType;

        internal void SetEntityType(Type entityType)
        {
            _entityType = entityType;
        }

        // Track update suspension for flicker-free bulk operations
        private int _updateCount = 0;

        /// <summary>
        /// Suspends painting to allow multiple operations without flickering
        /// Uses Windows API WM_SETREDRAW for maximum effectiveness
        /// </summary>
        public void BeginUpdate()
        {
            if (_updateCount == 0 && IsHandleCreated)
            {
                SendMessage(Handle, WM_SETREDRAW, false, 0);
            }
            _updateCount++;
        }

        /// <summary>
        /// Resumes painting after BeginUpdate and refreshes the control
        /// </summary>
        public void EndUpdate()
        {
            if (_updateCount > 0)
            {
                _updateCount--;
                if (_updateCount == 0 && IsHandleCreated)
                {
                    SendMessage(Handle, WM_SETREDRAW, true, 0);
                    Invalidate(true); // true = invalidate children too
                    Update(); // Force immediate repaint
                }
            }
        }

        /// <summary>
        /// Override WndProc to handle WM_ERASEBKGND and prevent background flicker
        /// Since we use double buffering, we don't need Windows to erase the background
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_ERASEBKGND)
            {
                // Always prevent erasing background - we handle all painting ourselves with double buffering
                m.Result = (IntPtr)1;
                return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Override OnPaintBackground to prevent any background painting
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Don't call base - we handle all painting in OnPaint/DrawContent
            // This prevents flicker from background erase
        }

        /// <summary>
        /// Override CreateParams to add WS_EX_COMPOSITED Style for better flicker reduction
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED - reduces flicker by compositing all children
                return cp;
            }
        }

        [Browsable(true)]
        [Category("Layout")] 
        public int RowHeight 
        { 
            get => Layout.RowHeight;
            set { 
                if (Layout.RowHeight != Math.Max(18, value))
                {
                    Layout.RowHeight = Math.Max(18, value);
                    if (!DesignMode) Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")] 
        public int ColumnHeaderHeight 
        { 
            get => Layout.ColumnHeaderHeight; 
            set { 
                if (Layout.ColumnHeaderHeight != Math.Max(22, value))
                {
                    Layout.ColumnHeaderHeight = Math.Max(22, value);
                    if (!DesignMode) Invalidate();
                }
            }
        }

        [Browsable(true)]
        [Category("Layout")] 
        public bool ShowColumnHeaders 
        { 
            get => Layout.ShowColumnHeaders; 
            set { 
                if (Layout.ShowColumnHeaders != value)
                {
                    Layout.ShowColumnHeaders = value;
                    if (!DesignMode) Invalidate();
                }
            }
        }

        // Owner-drawn navigator footer like BeepSimpleGrid (not a child control)
        private bool _showNavigator = true;
        [Browsable(true)]
        [Category("Layout")] 
        [DefaultValue(true)]
        public bool ShowNavigator
        {
            get => _showNavigator;
            set { 
                if (_showNavigator != value) 
                { 
                    _showNavigator = value; 
                    Layout.Recalculate(); 
                    if (!DesignMode) Invalidate(); 
                } 
            }
        }

        private BeepGridStyle _gridStyle = BeepGridStyle.Bootstrap;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The visual Style preset for the grid, inspired by popular JavaScript frameworks.")]
        [DefaultValue(BeepGridStyle.Default)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public BeepGridStyle GridStyle
        {
            get => _gridStyle;
            set
            {
                if (_gridStyle != value)
                {
                    _gridStyle = value;
                    ApplyGridStyle();
                    // Always refresh, both at design time and runtime
                    Invalidate();
                    Refresh();

                    // Notify designer of the change
                    if (DesignMode && Site != null)
                    {
                        IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                        changeService?.OnComponentChanged(this, null, null, null);
                    }
                }
            }
        }

        private navigationStyle _navigationStyle = navigationStyle.Standard;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Navigation bar Style - choose from 12 framework-inspired designs")]
        [DefaultValue(navigationStyle.Standard)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public navigationStyle NavigationStyle
        {
            get => _navigationStyle;
            set
            {
                if (_navigationStyle != value)
                {
                    _navigationStyle = value;
                    NavigatorPainter.NavigationStyle = value;
                    
                    // Update navigator height based on the new Style
                    if (_usePainterNavigation)
                    {
                        if (value == navigationStyle.None)
                        {
                            Layout.NavigatorHeight = 0; // No height for None
                        }
                        else
                        {
                            Layout.NavigatorHeight = NavigatorPainter.GetRecommendedNavigatorHeight();
                        }
                        Layout.Recalculate();
                    }
                    
                    Invalidate();
                    Refresh();

                    // Notify designer of the change
                    if (DesignMode && Site != null)
                    {
                        IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                        changeService?.OnComponentChanged(this, null, null, null);
                    }
                }
            }
        }

        private bool _usePainterNavigation = true;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Use modern painter-based navigation (true) or legacy button-based navigation (false)")]
        [DefaultValue(true)]
        public bool UsePainterNavigation
        {
            get => _usePainterNavigation;
            set
            {
                if (_usePainterNavigation != value)
                {
                    _usePainterNavigation = value;
                    NavigatorPainter.UsePainterNavigation = value;
                    Invalidate();
                }
            }
        }

        private GridLayoutPreset _layoutPreset = GridLayoutPreset.Default;
        [Browsable(true)]
        [Category("Layout")]
        [Description("Structural layout preset (spacing, stripes, header effects). Not colors.")]
        [DefaultValue(GridLayoutPreset.Default)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public GridLayoutPreset LayoutPreset
        {
            get => _layoutPreset;
            set
            {
                if (_layoutPreset == value) return;
                _layoutPreset = value;
                ApplyLayoutPreset(value);
                Invalidate();
                Refresh();
                if (DesignMode && Site != null)
                {
                    var changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                    changeService?.OnComponentChanged(this, null, null, null);
                }
            }
        }

        public void ApplyLayoutPreset(GridLayoutPreset preset)
        {
            IGridLayoutPreset impl = preset switch
            {
                GridLayoutPreset.Clean => new CleanTableLayoutHelper(),
                GridLayoutPreset.Dense => new DenseTableLayoutHelper(),
                GridLayoutPreset.Striped => new StripedTableLayoutHelper(),
                GridLayoutPreset.Borderless => new BorderlessTableLayoutHelper(),
                GridLayoutPreset.HeaderBold => new HeaderBoldTableLayoutHelper(),
                GridLayoutPreset.MaterialHeader => new MaterialHeaderTableLayoutHelper(),
                GridLayoutPreset.Card => new CardTableLayoutHelper(),
                GridLayoutPreset.ComparisonTable => new ComparisonTableLayoutHelper(),
                GridLayoutPreset.MatrixSimple => new MatrixSimpleTableLayoutHelper(),
                GridLayoutPreset.MatrixStriped => new MatrixStripedTableLayoutHelper(),
                GridLayoutPreset.PricingTable => new PricingTableLayoutHelper(),
                _ => new DefaultTableLayoutHelper()
            };
            this.ApplyLayoutPreset(impl);
        }

        [Browsable(true)]
        [Category("Behavior")]
        public bool AllowUserToResizeColumns { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        public bool AllowUserToResizeRows { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ReadOnly { get; set; } = false;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Allow selecting multiple rows via checkbox column. If false, only one row can be selected at a time.")]
        public bool MultiSelect { get; set; } = true;

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ShowCheckBox 
        { 
            get => Data.Columns.Any(c => c.IsSelectionCheckBox && c.Visible);
            set 
            { 
                if (ShowCheckBox != value)
                {
                    // Ensure system columns exist before setting visibility
                    Data.EnsureSystemColumns();
                    
                    var selColumn = Data.Columns.FirstOrDefault(c => c.IsSelectionCheckBox);
                    if (selColumn != null)
                    {
                        selColumn.Visible = value;
                        Layout.Recalculate();
                        if (!DesignMode) Invalidate();
                    }
                }
            }
        }
        private BeepColumnConfig _selectioncheckboxColumn;
        public BeepColumnConfig SelectionCheckBoxColumn
        {
            get
            {
                if (_selectioncheckboxColumn == null)
                {
                    _selectioncheckboxColumn = Data.Columns.FirstOrDefault(r => r.IsSelectionCheckBox);
                    if (_selectioncheckboxColumn == null)
                    {
                        Data.EnsureSystemColumns();
                        _selectioncheckboxColumn = Data.Columns.FirstOrDefault(r => r.IsSelectionCheckBox);
                    }
                }
                return _selectioncheckboxColumn;
            }
        }
        private BeepColumnConfig _rowIDColumn;
        public BeepColumnConfig RowIDColumn
        {
            get
            {
                if (_rowIDColumn == null)
                {
                    _rowIDColumn = Data.Columns.FirstOrDefault(c => c.IsRowID);
                    if (_rowIDColumn == null)
                    {
                        Data.EnsureSystemColumns();
                        _rowIDColumn = Data.Columns.FirstOrDefault(c => c.IsRowID);
                    }
                }
                return _rowIDColumn;
            }
        }
        private BeepColumnConfig _rowNumberColumn;
        public BeepColumnConfig RowNumberColumn
        {
            get
            {
                if (_rowNumberColumn == null)
                {
                    _rowNumberColumn = Data.Columns.FirstOrDefault(c => c.IsRowNumColumn);
                    if (_rowNumberColumn == null)
                    {
                        Data.EnsureSystemColumns();
                        _rowNumberColumn = Data.Columns.FirstOrDefault(c => c.IsRowNumColumn);
                    }
                }
                return _rowNumberColumn;
            }
        }
        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(DataGridViewAutoSizeColumnsMode.None)]
        public DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode { get; set; } = DataGridViewAutoSizeColumnsMode.None;

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(false)]
        public bool AutoSizeRowsToContent { get; set; } = false;

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(2)]
        [Description("Padding added to auto-sized row heights (in pixels)")]
        public int RowAutoSizePadding { get; set; } = 2;

        [Browsable(true)]
        [Category("Layout")]
        [DefaultValue(true)]
        [Description("Whether to use DPI-aware scaling for row height calculations")]
        public bool UseDpiAwareRowHeights { get; set; } = true;
     

        public BeepGridPro():base   ()
        {
            // Enhance control styles for better performance and reduced flickering
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                  
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw , true);
            SetStyle(ControlStyles.Selectable, true);
            UpdateStyles();
           // Console.WriteLine("BeepGridPro constructor called");
            // Disable base-frame right border and borders so DrawingRect uses full client area
            //ShowRightBorder = false;
            //ShowAllBorders = false;
            //IsFrameless = true;
           // Console.WriteLine("BeepGridPro base constructor completed");
            // Create a dedicated host layer for editors (kept only over active cell)
          
           // Console.WriteLine("BeepGridPro editor host created");
            Layout = new Helpers.GridLayoutHelper(this);
           // Console.WriteLine("BeepGridPro layout helper created");
            Data = new Helpers.GridDataHelper(this);
           // Console.WriteLine("BeepGridPro data helper created");
            Render = new Helpers.GridRenderHelper(this);
           // Console.WriteLine("BeepGridPro render helper created");
            Selection = new Helpers.GridSelectionHelper(this);
           // Console.WriteLine("BeepGridPro selection helper created");
            Input = new Helpers.GridInputHelper(this);
           // Console.WriteLine("BeepGridPro input helper created");
            Scroll = new Helpers.GridScrollHelper(this);
           // Console.WriteLine("BeepGridPro scroll helper created");
            ScrollBars = new Helpers.GridScrollBarsHelper(this);
           // Console.WriteLine("BeepGridPro scrollbars helper created"); 
            SortFilter = new Helpers.GridSortFilterHelper(this);
           // Console.WriteLine("BeepGridPro sort/filter helper created");
            Edit = new Helpers.GridEditHelper(this);
           // Console.WriteLine("BeepGridPro edit helper created");
            ThemeHelper = new Helpers.GridThemeHelper(this);
           // Console.WriteLine("BeepGridPro theme helper created");
            Navigator = new Helpers.GridNavigatorHelper(this);
           // Console.WriteLine("BeepGridPro navigator helper created");
            NavigatorPainter = new Helpers.GridNavigationPainterHelper(this);
           // Console.WriteLine("BeepGridPro navigator painter helper created");
            
            // Sync navigation painter properties
            NavigatorPainter.UsePainterNavigation = _usePainterNavigation;
            NavigatorPainter.NavigationStyle = _navigationStyle;
            
            // Set initial navigator height based on Style
            if (_usePainterNavigation)
            {
                if (_navigationStyle == navigationStyle.None)
                {
                    Layout.NavigatorHeight = 0;
                }
                else
                {
                    Layout.NavigatorHeight = NavigatorPainter.GetRecommendedNavigatorHeight();
                }
            }
            
            Sizing = new Helpers.GridSizingHelper(this);
           // Console.WriteLine("BeepGridPro sizing helper created");
            Dialog = new Helpers.GridDialogHelper(this);
           // Console.WriteLine("BeepGridPro dialog helper created");
            // Only subscribe to events and setup complex initialization if not in design mode
            if (!DesignMode)
            {
                // Subscribe to column model changes so designer edits are reflected immediately
                HookColumnsCollection(Data.Columns);

                this.MouseDown += (s, e) => 
                {
                    // Check if scrollbar handled the click first
                    bool handledByScrollbar = ScrollBars?.HandleMouseDown(e.Location, e.Button) ?? false;
                    if (!handledByScrollbar)
                    {
                        Input.HandleMouseDown(e);
                    }
                };
                this.MouseMove += (s, e) => 
                {
                    // Don't process input mouse move if scrollbar is dragging
                    if (ScrollBars?.IsDragging ?? false)
                    {
                        ScrollBars?.HandleMouseMove(e.Location);
                    }
                    else
                    {
                        ScrollBars?.HandleMouseMove(e.Location);
                        Input.HandleMouseMove(e);
                    }
                };
                this.MouseUp += (s, e) => 
                {
                    // Let scrollbar handle mouse up and check if it was handled
                    bool handledByScrollbar = ScrollBars?.HandleMouseUp(e.Location, e.Button) ?? false;
                    // Only process input mouse up if scrollbar didn't handle it
                    if (!handledByScrollbar)
                    {
                        Input.HandleMouseUp(e);
                    }
                };
                this.MouseWheel += (s, e) => { 
                    // Handle mouse wheel exactly like BeepSimpleGrid
                    ScrollBars?.HandleMouseWheel(e); 
                    // Custom scrollbars updated automatically through drawing
                };
                this.KeyDown += (s, e) => Input.HandleKeyDown(e);

                // Enable Excel-like filter popup automatically
                this.EnableExcelFilter();
            }
            //PainterKind= Base.BaseControl.BaseControlPainterKind.Classic;
           
            RowHeight = 25;
            ColumnHeaderHeight = 28;
            ShowColumnHeaders = true;
        }

        //// Ensure window Style flags are set during handle creation
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var cp = base.CreateParams;
        //        // Temporarily remove these flags to fix editor host visibility issues
        //        // cp.ProgressBarStyle |= WS_CLIPCHILDREN | WS_CLIPSIBLINGS;
        //        return cp;
        //    }
        //}

        private void HookColumnsCollection(BeepGridColumnConfigCollection cols)
        {
            if (cols == null) return;
            cols.ListChanged -= Columns_ListChanged; // avoid duplicates
            cols.ListChanged += Columns_ListChanged;
            // Subscribe to existing items
            foreach (var col in cols)
            {
                if (col is INotifyPropertyChanged inpc)
                {
                    inpc.PropertyChanged -= Column_PropertyChanged;
                    inpc.PropertyChanged += Column_PropertyChanged;
                }
            }
        }

        private void Columns_ListChanged(object sender, ListChangedEventArgs e)
        {
            // Skip excessive processing in design mode
            if (DesignMode) return;
            
            // Subscribe to added items
            if (e.ListChangedType == ListChangedType.ItemAdded && e.NewIndex >= 0 && e.NewIndex < Data.Columns.Count)
            {
                var col = Data.Columns[e.NewIndex];
                if (col is INotifyPropertyChanged inpc)
                {
                    inpc.PropertyChanged -= Column_PropertyChanged;
                    inpc.PropertyChanged += Column_PropertyChanged;
                }
            }
            else if (e.ListChangedType == ListChangedType.Reset)
            {
                // Re-hook all
                HookColumnsCollection(Data.Columns);
            }

            SafeRecalculate();
            Invalidate();
        }

        private void Column_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Skip excessive processing in design mode
            if (DesignMode) return;
            
            SafeRecalculate();
            Invalidate();
        }

        private void SafeRecalculate()
        {
            if (Layout != null && !Layout.IsCalculating)
                Layout.Recalculate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            SafeRecalculate();

            // Do not stretch the editor host; it will be sized to the active cell only
            
            // Only update scrollbars if not in design mode
            if (!DesignMode)
            {
                ScrollBars?.UpdateBars();
                // Don't call Invalidate() here - ResizeRedraw Style already triggers repaint
                // Calling Invalidate() here causes flicker because it forces an extra paint cycle
            }
        }

        protected override void DrawContent(Graphics g)
        {
            // Console.WriteLine("BeepGridPro DrawContent START");

            // Call base for graphics setup and UpdateDrawingRect (now handled by BaseControl)
            //  base.DrawContent(g);
            UpdateDrawingRect();
            // Clip to the DrawingRect to ensure grid draws within the intended area
            var drawingRect = DrawingRect;
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
            {
               // Console.WriteLine("BeepGridPro DrawContent - Invalid DrawingRect, skipping draw");
                return;
            }

           // Console.WriteLine("BeepGridPro DrawContent - DrawingRect: " + drawingRect.ToString());

            // Save the current graphics state
            var graphicsState = g.Save();

            try
            {
                // Set clipping region to DrawingRect
               g.SetClip(drawingRect);

                // Now do our custom grid drawing within the clipped area
               // Console.WriteLine("BeepGridPro DrawContent called");
               // Console.WriteLine("BeepGridPro DrawContent - Starting layout calculation");
                Layout?.EnsureCalculated();
               // Console.WriteLine("BeepGridPro Layout calculated");
               Render?.Draw(g);
                // Console.WriteLine("BeepGridPro Render completed");

                // Draw custom scrollbars after grid content
                ScrollBars?.DrawScrollBars(g);
                // Console.WriteLine("BeepGridPro ScrollBars drawn");
            }
            catch (Exception ex)
            {
               // Console.WriteLine($"BeepGridPro DrawContent - Grid drawing failed: {ex.Message}");
            }
            finally
            {
                // Always restore the graphics state
                g.Restore(graphicsState);
            }

            // Keep scrollbars in sync after rendering (outside clipping)
            if (!DesignMode)
            {
               ScrollBars?.UpdateBars();
            }

            // Console.WriteLine("BeepGridPro DrawContent END");
        }



        public override void ApplyTheme()
        {
            base.ApplyTheme();
            ThemeHelper.ApplyTheme();
            ApplyGridStyle(); // Apply grid Style after theme
            Invalidate();
        }

        /// <summary>
        /// Applies the selected grid Style preset to customize the grid's appearance
        /// </summary>
        public void ApplyGridStyle()
        {
            // Reset to base theme colors first
            var theme = BeepThemesManager.GetTheme(Theme);
            if (theme == null) return;

            // Apply Style-specific customizations (layout and visual effects only, no color changes)
            switch (_gridStyle)
            {
                case BeepGridStyle.Default:
                    // Use theme defaults - no special styling
                    RowHeight = 24;
                    ColumnHeaderHeight = 26;
                    Render.ShowGridLines = true;
                    Render.ShowRowStripes = false;
                    Render.UseHeaderGradient = false;
                    Render.ShowSortIndicators = true;
                    Render.UseHeaderHoverEffects = true;
                    Render.UseBoldHeaderText = false;
                    Render.HeaderCellPadding = 2;
                    break;

                case BeepGridStyle.Clean:
                    // Clean Style: subtle borders, minimal styling
                    RowHeight = 26;
                    ColumnHeaderHeight = 28;
                    Render.ShowGridLines = true;
                    Render.ShowRowStripes = false;
                    Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    Render.UseHeaderGradient = false;
                    Render.ShowSortIndicators = true;
                    Render.UseHeaderHoverEffects = true;
                    Render.UseBoldHeaderText = false;
                    Render.HeaderCellPadding = 3;
                    break;

                case BeepGridStyle.Bootstrap:
                    // Bootstrap-inspired: striped rows, clean borders
                    RowHeight = 24;
                    ColumnHeaderHeight = 26;
                    Render.ShowGridLines = true;
                    Render.ShowRowStripes = true;
                    Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    Render.UseHeaderGradient = false;
                    Render.ShowSortIndicators = true;
                    Render.UseHeaderHoverEffects = true;
                    Render.UseBoldHeaderText = false;
                    Render.HeaderCellPadding = 2;
                    break;

                case BeepGridStyle.Material:
                    // Material Design: elevation effects, clean lines
                    RowHeight = 28;
                    ColumnHeaderHeight = 32;
                    Render.ShowGridLines = true;
                    Render.ShowRowStripes = false;
                    Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    Render.UseElevation = true;
                    Render.UseHeaderGradient = true;
                    Render.ShowSortIndicators = true;
                    Render.UseHeaderHoverEffects = true;
                    Render.UseBoldHeaderText = false;
                    Render.HeaderCellPadding = 4;
                    break;

                case BeepGridStyle.Flat:
                    // Flat design: minimal borders, flat appearance
                    RowHeight = 24;
                    ColumnHeaderHeight = 26;
                    Render.ShowGridLines = false;
                    Render.ShowRowStripes = false;
                    Render.UseElevation = false;
                    Render.UseHeaderGradient = false;
                    Render.ShowSortIndicators = true;
                    Render.UseHeaderHoverEffects = false;
                    Render.UseBoldHeaderText = false;
                    Render.HeaderCellPadding = 2;
                    break;


                case BeepGridStyle.Compact:
                    // Compact: smaller padding, tighter spacing
                    RowHeight = 20;
                    ColumnHeaderHeight = 24;
                    Render.ShowGridLines = true;
                    Render.ShowRowStripes = false;
                    Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    Render.UseHeaderGradient = false;
                    Render.ShowSortIndicators = true;
                    Render.UseHeaderHoverEffects = false;
                    Render.UseBoldHeaderText = false;
                    Render.HeaderCellPadding = 1;
                    break;

                case BeepGridStyle.Corporate:
                    // Corporate: professional, conservative styling
                    RowHeight = 26;
                    ColumnHeaderHeight = 30;
                    Render.ShowGridLines = true;
                    Render.ShowRowStripes = false;
                    Render.GridLineStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                    Render.UseElevation = false;
                    Render.UseHeaderGradient = true;
                    Render.ShowSortIndicators = true;
                    Render.UseHeaderHoverEffects = true;
                    Render.UseBoldHeaderText = true;
                    Render.HeaderCellPadding = 3;
                    break;

                case BeepGridStyle.Minimal:
                    // Minimal: focus on content, very subtle styling
                    RowHeight = 24;
                    ColumnHeaderHeight = 26;
                    Render.ShowGridLines = false;
                    Render.ShowRowStripes = false;
                    Render.UseElevation = false;
                    Render.UseHeaderGradient = false;
                    Render.ShowSortIndicators = false;
                    Render.UseHeaderHoverEffects = false;
                    Render.UseBoldHeaderText = false;
                    Render.HeaderCellPadding = 2;
                    break;

                case BeepGridStyle.Card:
                    // Card-based: modern card-like appearance
                    RowHeight = 28;
                    ColumnHeaderHeight = 32;
                    Render.ShowGridLines = false;
                    Render.ShowRowStripes = false;
                    Render.UseElevation = true;
                    Render.CardStyle = true;
                    Render.UseHeaderGradient = true;
                    Render.ShowSortIndicators = true;
                    Render.UseHeaderHoverEffects = true;
                    Render.UseBoldHeaderText = false;
                    Render.HeaderCellPadding = 4;
                    break;

                case BeepGridStyle.Borderless:
                    // Borderless: no visible grid lines
                    RowHeight = 24;
                    ColumnHeaderHeight = 26;
                    Render.ShowGridLines = false;
                    Render.ShowRowStripes = false;
                    Render.UseElevation = false;
                    Render.UseHeaderGradient = false;
                    Render.ShowSortIndicators = true;
                    Render.UseHeaderHoverEffects = false;
                    Render.UseBoldHeaderText = false;
                    Render.HeaderCellPadding = 2;
                    break;
            }

            // Recalculate layout after Style changes
            Layout.Recalculate();
        }

        public void AutoGenerateColumns()
        {
            Data.AutoGenerateColumns();
            SafeRecalculate();
            ScrollBars?.UpdateBars();
            Invalidate();
        }

        /// <summary>
        /// Ensures that system columns (checkbox, row number, row ID) are present in the grid.
        /// This method is called automatically but can be called manually if needed.
        /// </summary>
        public void EnsureSystemColumns()
        {
            Data.EnsureSystemColumns();
            SafeRecalculate();
            ScrollBars?.UpdateBars();
            Invalidate();
        }

        public void RefreshGrid()
        {
            Data.RefreshRows();
            SafeRecalculate();
            ScrollBars?.UpdateBars();
            Invalidate();
        }

        /// <summary>
        /// Recalculates all heights (ColumnHeaderHeight, RowHeight, NavigatorHeight) using painter-based calculations
        /// that account for font sizes and style-specific spacing requirements
        /// </summary>
        public void RecalculateHeightsFromPainters()
        {
            // Calculate header height from column header painter (font-aware)
            var headerPainter = new Helpers.GridColumnHeadersPainterHelper(this);
            ColumnHeaderHeight = headerPainter.CalculateHeaderHeight();

            // Calculate row height from data font (font-aware)
            if (Font != null)
            {
                int baseFontHeight = Font.Height;
                int cellPadding = Render?.HeaderCellPadding ?? 2;
                RowHeight = baseFontHeight + (cellPadding * 2) + 4; // 4px for comfortable spacing
            }

            // Calculate navigator height from navigator painter if enabled (already has font-aware calculation)
            if (ShowNavigator && NavigatorPainter != null)
            {
                Layout.NavigatorHeight = NavigatorPainter.GetRecommendedNavigatorHeight();
            }
            else if (!ShowNavigator)
            {
                Layout.NavigatorHeight = 0;
            }

            // Recalculate layout with new heights
            SafeRecalculate();
            Invalidate();
        }

        /// <summary>
        /// Auto-resize columns to fit their content, similar to BeepSimpleGrid
        /// Also handles row height auto-sizing when AutoSizeColumnsMode is AllCells or AllCellsExceptHeader, or when AutoSizeRowsToContent is true
        /// </summary>
        public void AutoResizeColumnsToFitContent()
        {
            Sizing.AutoResizeColumnsToFitContent();
            SafeRecalculate();
            ScrollBars?.UpdateBars();
            Invalidate();
        }

        /// <summary>
        /// Auto-resize row heights to fit their content based on the tallest cell in each row
        /// </summary>
        public void AutoSizeRowsToFitContent()
        {
            Sizing.AutoSizeRowsToFitContent();
            SafeRecalculate();
            ScrollBars?.UpdateBars();
            Invalidate();
        }

        /// <summary>
        /// Calculate optimal width for a column based on its content
        /// </summary>
        /// <param name="column">The column to measure</param>
        /// <param name="includeHeader">Whether to include header text in measurement</param>
        /// <param name="allRows">Whether to measure all rows or just visible rows</param>
        /// <returns>Optimal width in pixels</returns>
        private int GetColumnWidth(BeepColumnConfig column, bool includeHeader, bool allRows)
        {
            return Sizing.GetColumnWidth(column, includeHeader, allRows);
        }

        /// <summary>
        /// Set a specific column width by name
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="width">New width in pixels</param>
        public void SetColumnWidth(string columnName, int width)
        {
            Sizing.SetColumnWidth(columnName, width);
            SafeRecalculate();
            ScrollBars?.UpdateBars();
            Invalidate();
        }

        public void SelectCell(int rowIndex, int columnIndex)
        {
            Selection.SelectCell(rowIndex, columnIndex);
            Invalidate();
        }

        public void AttachNavigator(BeepBindingNavigator navigator, object dataSource)
        {
            Navigator.Attach(navigator, dataSource);
        }

        // Expose action methods for owner-drawn navigator - delegate to Navigator helper
        public void MoveFirst() => Navigator.MoveFirst();
        public void MovePrevious() => Navigator.MovePrevious();
        public void MoveNext() => Navigator.MoveNext();
        public void MoveLast() => Navigator.MoveLast();
        public void InsertNew() => Navigator.InsertNew();
        public void DeleteCurrent() => Navigator.DeleteCurrent();
        public void Save() => Navigator.Save();
        public void Cancel() => Navigator.Cancel();
        
        [Browsable(false)]
        public IReadOnlyList<BeepRowConfig> SelectedRows => Data.Rows.Where(r => r.IsSelected).ToList();

        [Browsable(false)]
        public IReadOnlyList<int> SelectedRowIndices => Data.Rows
            .Select((r, i) => new { r, i })
            .Where(x => x.r.IsSelected)
            .Select(x => x.i)
            .ToList();

        public bool ShowGridLines { get; internal set; }
        public bool AlternateRowColor { get; internal set; }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when the row selection changes (checkbox or active cell). RowIndex = -1 means bulk change.")]
        public event EventHandler<BeepRowSelectedEventArgs> RowSelectionChanged;

        internal void OnRowSelectionChanged(int rowIndex)
        {
            BeepRowConfig row = (rowIndex >= 0 && rowIndex < Data.Rows.Count) ? Data.Rows[rowIndex] : null;
            RowSelectionChanged?.Invoke(this, new BeepRowSelectedEventArgs(rowIndex, row));
        }

        internal void OnRowSelectionChanged(BeepRowConfig row)
        {
            int idx = row != null ? Data.Rows.IndexOf(row) : -1;
            OnRowSelectionChanged(idx);
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when a save operation is requested/completed.")]
        public event EventHandler SaveCalled;

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Raised when a cell value is changed by the editor.")]
        public event EventHandler<BeepCellEventArgs> CellValueChanged;

        internal void OnSaveCalled()
        {
            try { SaveCalled?.Invoke(this, EventArgs.Empty); } catch { }
        }

        /// <summary>
        /// Raise CellValueChanged event for helpers to call
        /// </summary>
        /// <param name="cell">The cell that changed</param>
        internal void OnCellValueChanged(BeepCellConfig cell)
        {
            CellValueChanged?.Invoke(this, new BeepCellEventArgs(cell));
        }

        // BeepSimpleGrid compatibility helpers
        public BeepColumnConfig GetColumnByName(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName)) return null;
            return Data.Columns.FirstOrDefault(c => string.Equals(c.ColumnName, columnName, StringComparison.OrdinalIgnoreCase));
        }

        public BeepColumnConfig GetColumnByCaption(string caption)
        {
            if (string.IsNullOrWhiteSpace(caption)) return null;
            return Data.Columns.FirstOrDefault(c => string.Equals(caption, c.ColumnCaption, StringComparison.OrdinalIgnoreCase));
        }

        public BeepColumnConfig GetColumnByIndex(int index)
        {
            if (index < 0 || index >= Data.Columns.Count) return null;
            return Data.Columns[index];
        }

        public Dictionary<string, BeepColumnConfig> GetDictionaryColumns()
        {
            return Data.Columns.ToDictionary(c => c.ColumnName, c => c, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Shows an editor dialog for the currently selected cell
        /// </summary>
        public void ShowCellEditor()
        {
            if (Selection.HasSelection)
            {
                var cell = Data.Rows[Selection.RowIndex].Cells[Selection.ColumnIndex];
                Dialog.ShowEditorDialog(cell);
            }
        }

        /// <summary>
        /// Shows the filter dialog
        /// </summary>
        public void ShowFilterDialog()
        {
            Dialog.ShowFilterDialog();
        }

        /// <summary>
        /// Shows the search dialog
        /// </summary>
        public void ShowSearchDialog()
        {
            Dialog.ShowSearchDialog();
        }

        /// <summary>
        /// Shows the column configuration dialog
        /// </summary>
        public void ShowColumnConfigDialog()
        {
            Dialog.ShowColumnConfigDialog();
        }

        /// <summary>
        /// Enables Excel-like filter functionality (can be called to initialize filters)
        /// </summary>
        public void EnableExcelFilter()
        {
            // This method can be expanded to add filter dropdowns to column headers
            // For now, it's a placeholder that ensures the grid is ready for filtering
            foreach (var col in Data.Columns.Where(c => !c.IsSelectionCheckBox && !c.IsRowNumColumn && !c.IsRowID))
            {
                col.ShowFilterIcon = true;
            }
            Invalidate();
        }

        /// <summary>
        /// Toggles the sort direction for the specified column index
        /// </summary>
        /// <param name="columnIndex">The index of the column to sort</param>
        public void ToggleColumnSort(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= Data.Columns.Count)
                return;

            var column = Data.Columns[columnIndex];
            if (column == null)
                return;

            // Toggle sort direction
            var newDirection = column.SortDirection == SortDirection.Ascending 
                ? SortDirection.Descending 
                : SortDirection.Ascending;

            // Clear previous sort indicators
            foreach (var col in Data.Columns)
            {
                col.IsSorted = false;
                col.ShowSortIcon = false;
            }

            // Set new sort
            column.IsSorted = true;
            column.ShowSortIcon = true;
            column.SortDirection = newDirection;

            // Apply the sort
            SortFilter.Sort(column.ColumnName, newDirection);

            // Refresh the grid
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // Scrollbar handling moved to MouseMove event handler to prevent double-processing
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            // Scrollbar handling moved to MouseDown event handler to prevent double-processing
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            // Scrollbar handling moved to MouseUp event handler to prevent double-processing
        }

        #region Targeted Invalidation Methods

        /// <summary>
        /// Invalidates only the specified row, avoiding full grid repaint
        /// </summary>
        public void InvalidateRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Data.Rows.Count)
                return;

            var row = Data.Rows[rowIndex];
            if (row.Cells.Count > 0)
            {
                // Get the bounding rectangle for the entire row
                var firstCell = row.Cells[0];
                var lastCell = row.Cells[row.Cells.Count - 1];
                
                if (!firstCell.Rect.IsEmpty && !lastCell.Rect.IsEmpty)
                {
                    var rowRect = new Rectangle(
                        Layout.RowsRect.Left,
                        firstCell.Rect.Top,
                        Layout.RowsRect.Width,
                        firstCell.Rect.Height
                    );
                    
                    // Only invalidate the specific row region
                    Invalidate(rowRect);
                }
            }
        }

        /// <summary>
        /// Invalidates only the specified cell, avoiding full grid repaint
        /// </summary>
        public void InvalidateCell(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= Data.Rows.Count)
                return;
            if (columnIndex < 0 || columnIndex >= Data.Columns.Count)
                return;

            var cell = Data.Rows[rowIndex].Cells[columnIndex];
            if (!cell.Rect.IsEmpty)
            {
                // Only invalidate the specific cell region
                Invalidate(cell.Rect);
            }
        }

        /// <summary>
        /// Invalidates a range of rows for batch updates
        /// </summary>
        public void InvalidateRows(int startRowIndex, int endRowIndex)
        {
            if (startRowIndex < 0 || startRowIndex >= Data.Rows.Count)
                return;
            if (endRowIndex < 0 || endRowIndex >= Data.Rows.Count)
                return;
            
            int start = Math.Min(startRowIndex, endRowIndex);
            int end = Math.Max(startRowIndex, endRowIndex);
            
            if (Data.Rows[start].Cells.Count > 0 && Data.Rows[end].Cells.Count > 0)
            {
                var firstCell = Data.Rows[start].Cells[0];
                var lastRow = Data.Rows[end];
                var lastCell = lastRow.Cells[0];
                
                if (!firstCell.Rect.IsEmpty && !lastCell.Rect.IsEmpty)
                {
                    int rowHeight = lastCell.Rect.Height > 0 ? lastCell.Rect.Height : RowHeight;
                    var rangeRect = new Rectangle(
                        Layout.RowsRect.Left,
                        firstCell.Rect.Top,
                        Layout.RowsRect.Width,
                        (lastCell.Rect.Top + rowHeight) - firstCell.Rect.Top
                    );
                    
                    Invalidate(rangeRect);
                }
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dialog?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
