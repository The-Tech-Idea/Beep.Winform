
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
    public partial class BeepGridPro : BaseControl
    {
        #region Helper Instances
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
        internal Helpers.GridClipboardHelper Clipboard { get; }
        internal Helpers.GridColumnReorderHelper ColumnReorder { get; }
        #endregion

        #region Private Fields
        // Data management fields (moved to partial in BeepGridPro.Properties.cs except _uowBinder)
        private GridUnitOfWorkBinder _uowBinder;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="BeepGridPro"/> control.
        /// </summary>
        public BeepGridPro():base   ()
        {
            // Enhance control styles for better performance and reduced flickering
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                  
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw , true);
            SetStyle(ControlStyles.Selectable, true);
            UpdateStyles();
            
            // Disable base-frame right border and borders so DrawingRect uses full client area
            //ShowRightBorder = false;
            //ShowAllBorders = false;
            //IsFrameless = true;
            
            // Create a dedicated host layer for editors (kept only over active cell)
            Layout = new Helpers.GridLayoutHelper(this);
            Data = new Helpers.GridDataHelper(this);
            Render = new Helpers.GridRenderHelper(this);
            Selection = new Helpers.GridSelectionHelper(this);
            Input = new Helpers.GridInputHelper(this);
            Scroll = new Helpers.GridScrollHelper(this);
            ScrollBars = new Helpers.GridScrollBarsHelper(this);
            SortFilter = new Helpers.GridSortFilterHelper(this);
            Edit = new Helpers.GridEditHelper(this);
            ThemeHelper = new Helpers.GridThemeHelper(this);
            Navigator = new Helpers.GridNavigatorHelper(this);
            NavigatorPainter = new Helpers.GridNavigationPainterHelper(this);
            
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
            Dialog = new Helpers.GridDialogHelper(this);
            Clipboard = new Helpers.GridClipboardHelper(this);
            ColumnReorder = new Helpers.GridColumnReorderHelper(this);
            // Only setup complex initialization if not in design mode
            if (!DesignMode)
            {
                // Subscribe to column model changes so designer edits are reflected immediately
                HookColumnsCollection(Data.Columns);
                // Enable Excel-like filter popup automatically
                this.EnableExcelFilter();
            }
            //PainterKind= Base.BaseControl.BaseControlPainterKind.Classic;
           
            RowHeight = 25;
            ColumnHeaderHeight = 28;
            ShowColumnHeaders = true;

            // Set accessibility properties
            AccessibleRole = AccessibleRole.Table;
            AccessibleName = "Data Grid";
            AccessibleDescription = "Data grid with rows and columns";
        }
        #endregion

        #region Column Collection Management
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
        #endregion

        #region Event Handlers - Column Collection
        private void Columns_ListChanged(object? sender, ListChangedEventArgs e)
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

        private void Column_PropertyChanged(object? sender, PropertyChangedEventArgs e)
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
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Handles control resize events and recalculates layout.
        /// </summary>
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

        /// <summary>
        /// Draws the grid content including headers, rows, cells, and scrollbars.
        /// </summary>
        protected override void DrawContent(Graphics g)
        {
            //// Don't repaint while context menu is active to prevent flicker and focus issues
            //if (_contextMenuActive)
            //{
            //    return;
            //}

            // Call base for graphics setup and UpdateDrawingRect (now handled by BaseControl)
            //  base.DrawContent(g);
            UpdateDrawingRect();
            // Clip to the DrawingRect to ensure grid draws within the intended area
            var drawingRect = DrawingRect;
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
            {
                return;
            }

            // Save the current graphics state
            var graphicsState = g.Save();

            try
            {
                // Set clipping region to DrawingRect
               g.SetClip(drawingRect);

                // Now do our custom grid drawing within the clipped area
                Layout?.EnsureCalculated();
               Render?.Draw(g);

                // Draw custom scrollbars after grid content
                ScrollBars?.DrawScrollBars(g);
            }
            catch
            {
                // Silently handle drawing exceptions to prevent crashes
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
        }
        #endregion

        #region Theme and Style Management
        /// <summary>
        /// Applies the current theme to the grid and its components.
        /// </summary>
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

        /// <summary>
        /// Automatically generates columns based on the data source structure.
        /// </summary>
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

        /// <summary>
        /// Refreshes the grid by reloading data and recalculating layout.
        /// </summary>
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
        #endregion

        #region Public Properties - Data Access
        /// <summary>
        /// Gets the currently selected rows in the grid.
        /// </summary>
        [Browsable(false)]
        public IReadOnlyList<BeepRowConfig> SelectedRows => Data.Rows.Where(r => r.IsSelected).ToList();

        /// <summary>
        /// Gets the indices of the currently selected rows.
        /// </summary>
        [Browsable(false)]
        public IReadOnlyList<int> SelectedRowIndices => Data.Rows
            .Select((r, i) => new { r, i })
            .Where(x => x.r.IsSelected)
            .Select(x => x.i)
            .ToList();

        /// <summary>
        /// Gets or sets whether grid lines are visible between cells.
        /// </summary>
        public bool ShowGridLines { get; internal set; }

        /// <summary>
        /// Gets or sets whether alternate row colors are used for striping effect.
        /// </summary>
        public bool AlternateRowColor { get; internal set; }
        #endregion

        // Context menu related methods in BeepGridPro.ContextMenu.cs partial
        // Clipboard operations in BeepGridPro.ClipboardOps.cs partial
        // Navigation methods in BeepGridPro.Navigation.cs partial
        // Data access methods in BeepGridPro.DataAccess.cs partial
        // Invalidation methods in BeepGridPro.Invalidation.cs partial
        // Mouse/Keyboard handlers in BeepGridPro.Input.cs partial
        // Events in BeepGridPro.Events.cs partial class
        // Dialog methods in BeepGridPro.Dialogs.cs partial class

        #region Dispose
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dialog?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}