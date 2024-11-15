﻿using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;
using Newtonsoft.Json;
using TheTechIdea.Beep.Winform.Controls.Grid;
using TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns.CustomDataGridViewColumns;
using TheTechIdea.Beep.Winform.Controls.Grid.Datacolumns;
using TheTechIdea.Beep.Winform.Controls.Template;
using System.Security.AccessControl;
using System.Windows.Forms.VisualStyles;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepSimpleGrid : BeepControl, IDisposable
    {
        #region "Properties"  
        int bottomPanelY;
        int botomspacetaken = 0;
        int topPanelY ;
        protected BeepTheme _theme = BeepThemesManager.DefaultTheme;
        protected string _themeName = "DefaultTheme";
        protected EnumBeepThemes _themeEnum = EnumBeepThemes.DefaultTheme;

        public int _rowHeight = 30; // Default row height
        private List<int> _columnWidths = new List<int> { 100, 100, 100 }; // Default column widths

        private bool _resizingColumn = false;
        public bool _resizingRow = false;
        private int _resizingIndex = -1;
        private int _resizeMargin = 5; // Distance from the edge to start resizing
        private Point _lastMousePos; // Store the last mouse position for resizing

        private BeepGridRow _hoveredRow = null; // For row hover effects
        private BeepGridCell _hoveredCell = null; // For cell hover effects

        private List<Rectangle> sortIconBounds = new List<Rectangle>();
        private List<Rectangle> filterIconBounds = new List<Rectangle>();
        private int _defaultcolumnheaderheight = 20;
        private int _defaultcolumnheaderwidth = 50;
        public object DataSource { get; set; }
        public string QueryFunctionName { get; set; }
        public string QueryFunction { get; set; }
        private int _xOffset = 0; // New offset for drawing

        private bool _showFilterButton = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowFilterButton
        {
            get => _showFilterButton;
            set
            {
                _showFilterButton = value;
                Invalidate(); // Redraw grid with new filter button visibility
            }
        }
        private bool _showSortIcons = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowSortIcons
        {
            get => _showSortIcons;
            set
            {
                _showSortIcons = value;
                Invalidate(); // Redraw grid with new sort icon visibility
            }
        }
        private bool _showRowNumbers = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowRowNumbers
        {
            get => _showRowNumbers;
            set
            {
                _showRowNumbers = value;
                Invalidate(); // Redraw grid with new row number visibility
            }
        }
        private bool _showColumnHeaders = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowColumnHeaders
        {
            get => _showColumnHeaders;
            set
            {
                _showColumnHeaders = value;
                Invalidate(); // Redraw grid with new column header visibility
            }
        }
        private bool _showRowHeaders = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowRowHeaders
        {
            get => _showRowHeaders;
            set
            {
                _showRowHeaders = value;
                Invalidate(); // Redraw grid with new row header visibility
            }
        }
        private bool _showNavigator = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowNavigator
        {
            get => _showNavigator;
            set
            {
                _showNavigator = value;
                Invalidate(); // Redraw grid with new navigator visibility
            }
        }
        private bool _showCRUDPanel = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowCRUDPanel
        {
            get => _showCRUDPanel;
            set
            {
                _showCRUDPanel = value;
                Invalidate(); // Redraw grid with new CRUD panel visibility
            }
        }
        private bool _showTitle = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowTitle
        {
            get => _showTitle;
            set
            {
                _showTitle = value;
                Invalidate(); // Redraw grid with new title visibility
            }
        }
        private bool _showBottomRow = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowBottomRow
        {
            get => _showBottomRow;
            set
            {
                _showBottomRow = value;
                Invalidate(); // Redraw grid with new bottom row visibility
            }
        }
        private bool _showFooter = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowFooter
        {
            get => _showFooter;
            set
            {
                _showFooter = value;
                Invalidate(); // Redraw grid with new footer visibility
            }
        }
        private bool _showHeaderPanel = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowHeaderPanel
        {
            get => _showHeaderPanel;
            set
            {
                _showHeaderPanel = value;
                Invalidate(); // Redraw grid with new header panel visibility
            }
        }
        private bool _showHeaderPanelBorder = true;
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowHeaderPanelBorder
        {
            get => _showHeaderPanelBorder;
            set
            {
                _showHeaderPanelBorder = value;
                Invalidate(); // Redraw grid with new header panel border visibility
            }
        }
        private bool _showHeaderPanelTitle = true;
        [DefaultValue(true)]
        [Browsable(true)]
        [Category("Layout")]
        public bool ShowHeaderPanelTitle
        {
            get => _showHeaderPanelTitle;
            set
            {
                _showHeaderPanelTitle = value;
                Invalidate(); // Redraw grid with new header panel title visibility
            }
        }

        [Browsable(true)]
        [Category("Layout")]
        public int ColumnHeight
        {
            get { return _defaultcolumnheaderheight; }
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
                Invalidate(); // Redraw grid with new offset
            }
        }
        public GridDataSourceType DataSourceType { get; set; } = GridDataSourceType.Fixed;

        [Browsable(true)]
        [TypeConverter(typeof(ThemeConverter))]
        public EnumBeepThemes Theme
        {
            get => _themeEnum;
            set
            {
                _themeEnum = value;
                _theme = BeepThemesManager.GetTheme(value);
                _themeName = BeepThemesManager.GetThemeName(value);
                ApplyTheme();
            }
        }
        private string _gridId;

        /// <summary>
        /// Unique identifier for each BeepGrid instance, persisted across design and runtime.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Design"), Description("Unique identifier for the grid.")]
        public string GridId
        {
            get => _gridId;
            set => _gridId = value;
        }
        private string _title = "BeepSimpleGrid Title"; // Title text
        public string Title
        {
            get => _title;
            set => _title = value;

        }
        public BindingList<BeepGridColumnConfig> beepGridColumns { get; set; } = new BindingList<BeepGridColumnConfig>();
        public BindingList<BeepGridRow> Rows { get; set; } = new BindingList<BeepGridRow>();
        public BeepGridRow BottomRow { get; set; } // For aggregations and totals

        public  BeepDataNavigator DataNavigator { get; set; } = new BeepDataNavigator();





        #endregion "Properties"

        public BeepSimpleGrid()
        {
            //  Rows.ListChanged += Rows_ListChanged;
            // Apply default dimensions to accommodate all layout elements by default
            this.MinimumSize = new Size(300, 200); // Set based on layout needs
            this.Size = new Size(400, 300); // Default start size

            this.MouseDown += BeepGrid_MouseDown;
            this.MouseMove += BeepGrid_MouseMove;
            this.MouseUp += BeepGrid_MouseUp;
            
           // ApplyTheme();
        }
     

        private void Rows_ListChanged(object sender, ListChangedEventArgs e)
        {
            Invalidate(); // Redraw the grid when rows change
        }
        #region "Header Layout"
        protected int headerPanelHeight = 30; // Height of the header panel
        protected int bottomagregationPanelHeight = 20; // Height of the bottom panel for agregation
        protected int footerPanelHeight = 20; // Height of the Footer panel
        protected int navigatorPanelHeight = 25; // Height of the navigator panel

        private Rectangle footerPanelRect; // Rectangle for the header panel
        private Rectangle headerPanelRect; // Rectangle for the header panel
        private Rectangle bottomagregationPanelRect; // Rectangle for the header panel
        private Rectangle navigatorPanelRect; // Rectangle for the header panel

        private Rectangle filterButtonRect; // Rectangle for the filter button area
        private BeepLabel titleLabel;
        private BeepButton filterButton;
        private int _buttonssize = 25;
        private void FilterButton_Click(object sender, EventArgs e)
        {
            // Show filter form when filter button is clicked
            ShowFilterForm();
        }
        #endregion "Header Layout"
        #region "Theme"
        public override void ApplyTheme()
        {
            this.BackColor = _theme.BackgroundColor;
            this.ForeColor = _theme.PrimaryTextColor;
            if (titleLabel != null) 
            {
                titleLabel.Theme = Theme;
            }
            
            if (DataNavigator != null)
            {
                DataNavigator.Theme = Theme;
            }
            
            //foreach (var row in Rows)
            //{
            //    row.ApplyTheme(_theme);
            //}

            Invalidate(); // Repaint the grid after applying the theme
        }

       
        #endregion "Theme"
        #region "Paint"
        #region "Drawin on DrawingRectangle"
        protected override void OnPaint(PaintEventArgs e)
        {
            Controls.Clear();
            UpdateDrawingRect();
            base.OnPaint(e);
            UpdateDrawingRect();
            // Adjust all drawing within DrawingRect
            var g = e.Graphics;

            var drawingBounds =DrawingRect;
            drawingBounds.Inflate(-1, -1); // Adjust for border
            headerPanelRect = new Rectangle(drawingBounds.Left, drawingBounds.Top, drawingBounds.Width, headerPanelHeight);
             bottomPanelY = drawingBounds.Bottom;
             botomspacetaken = 0;
             topPanelY = drawingBounds.Top;
            if (_showNavigator)
            {
                bottomPanelY -= navigatorPanelHeight;
                botomspacetaken = navigatorPanelHeight;
                navigatorPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width, navigatorPanelHeight);
                DrawNavigationRow(g, navigatorPanelRect);
            }
            if (_showFooter)
            {
                bottomPanelY -= footerPanelHeight;
                botomspacetaken += footerPanelHeight;
                footerPanelRect = new Rectangle(drawingBounds.Left, bottomPanelY, drawingBounds.Width, footerPanelHeight);
                DrawFooterRow(g, footerPanelRect);
            }
            if (_showBottomRow)
            {
                bottomPanelY -= bottomagregationPanelHeight;
                botomspacetaken += bottomagregationPanelHeight;
                bottomagregationPanelRect = new Rectangle(drawingBounds.Left , bottomPanelY, drawingBounds.Width, bottomagregationPanelHeight);
                DrawBottomAggregationRow(g, bottomagregationPanelRect);
            }
            if(_showHeaderPanel)
            {
                topPanelY += headerPanelHeight;
                botomspacetaken += headerPanelHeight;
                // Draw Header Panel and Title
                DrawHeaderPanel(g, headerPanelRect);
            }
           
            var gridRect = new Rectangle(drawingBounds.Left, topPanelY, drawingBounds.Width  , drawingBounds.Height- botomspacetaken);
            // Draw Column Headers
            PaintColumnHeaders(g, gridRect);

            // Draw Rows
            PaintRows(g, gridRect);

            // Draw Column Borders
           // DrawColumnBorders(g, gridRect);

        }

        private void DrawBottomAggregationRow(Graphics g, Rectangle bottomagregationPanelRect)
        {
            using (var pen = new Pen(_theme.BorderColor))
            {


                g.DrawLine(pen, bottomagregationPanelRect.Left, bottomagregationPanelRect.Top, bottomagregationPanelRect.Right, bottomagregationPanelRect.Top);

            }
        }
        private void DrawFooterRow(Graphics g, Rectangle footerPanelRect)
        {
            using (var pen = new Pen(_theme.BorderColor))
            {


                g.DrawLine(pen, footerPanelRect.Left, footerPanelRect.Top, footerPanelRect.Right, footerPanelRect.Top);

            }
        }
        private void DrawNavigationRow(Graphics g, Rectangle navigatorPanelRect)
        {
            if (DataNavigator == null)
            {
                DataNavigator = new BeepDataNavigator();
            }
            DataNavigator.Location = new Point(navigatorPanelRect.Left+2, navigatorPanelRect.Top+2);
            DataNavigator.Size = new Size(navigatorPanelRect.Width-2, navigatorPanelHeight-2);
          //  DataNavigator.Theme = Theme;
            
            DataNavigator.ShowAllBorders = false;
            DataNavigator.ShowShadow = false;
            DataNavigator.Theme=Theme;
            // draw line between header and grid
            using (var pen = new Pen(_theme.BorderColor))
            {
                g.DrawLine(pen, navigatorPanelRect.Left, navigatorPanelRect.Top, navigatorPanelRect.Right, navigatorPanelRect.Top);
            }
            Controls.Add(DataNavigator);
        }
        private void PaintRows(Graphics graphics, Rectangle drawingBounds)
        {
            int rowStartY = drawingBounds.Top;

            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                var row = Rows[rowIndex];
                var rowRect = new Rectangle(drawingBounds.Left, rowStartY, drawingBounds.Width, _rowHeight);

                // Use PaintRow to draw each individual row
                PaintRow(graphics, row, rowRect);

                // Update rowStartY to position the next row below the current one
                rowStartY += rowRect.Height;
            }
        }
        private void DrawHeaderPanel(Graphics g, Rectangle drawingBounds)
        {
            var headerPanelBorderRect = new Rectangle(drawingBounds.Left, drawingBounds.Top, drawingBounds.Width, headerPanelHeight);
            // Set up header panel rectangle based on drawingBounds
            // Rectangle headerPanelRect = new Rectangle(drawingBounds.Left, drawingBounds.Top, drawingBounds.Width, headerPanelHeight);


            // Draw the header panel top border
            //using (var borderPen = new Pen(_theme.BorderColor))
            //{
            //    g.DrawLine(borderPen, headerPanelBorderRect.Left, headerPanelBorderRect.Top, headerPanelBorderRect.Right, headerPanelBorderRect.Top);
            //}
            // Draw the header panel bottom border
            using (var borderPen = new Pen(_theme.BorderColor))
            {
                g.DrawLine(borderPen, headerPanelBorderRect.Left, headerPanelBorderRect.Bottom, headerPanelBorderRect.Right, headerPanelBorderRect.Bottom);
            }


            // Draw the Title Label (BeepLabel) within drawingBounds

            titleLabel = new BeepLabel
                {
                    Text = string.IsNullOrEmpty(Title)?"Beep Grid": Title,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Location = new Point(headerPanelBorderRect.Left+1, headerPanelBorderRect.Top+1), // Adjust Y as needed
                    
                    Theme = Theme,
                    ImageAlign = System.Drawing.ContentAlignment.MiddleLeft,
                    TextImageRelation = TextImageRelation.ImageBeforeText,
                    ShowAllBorders = false,
                    ShowShadow = false,
                    //ShowBottomBorder =true,
                    //ShowTopBorder = true,
                    IsChild = true
                };
            
            titleLabel.Size = new Size(headerPanelBorderRect.Width-2 , headerPanelBorderRect.Height-2 );
            titleLabel.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.home.svg";
            titleLabel.BackColor = _theme.BackColor;
            // Set title label bounds within headerPanelRect
            // Rectangle titleLabelRect = new Rectangle(headerPanelBorderRect.Left + 10, headerPanelBorderRect.Top+2, headerPanelBorderRect.Width - (_buttonssize + 15), headerPanelBorderRect.Height-2);
            // titleLabel.DrawToGraphics(g, titleLabelRect);

            // Draw the Filter Button within headerPanelRect, aligned to the right side

            //    filterButton = new BeepButton
            //    {
            //        Text = "",
            //        Size = new Size(_buttonssize, _buttonssize),
            //        Location = new Point(headerPanelBorderRect.Right - _buttonssize - 5, headerPanelBorderRect.Top + 2), // Adjust X and Y as needed
            //        Theme = Theme,
            //        ImageAlign= System.Drawing.ContentAlignment.MiddleCenter,
            //        TextImageRelation= TextImageRelation.ImageAboveText,
            //        MaxImageSize = new Size(20, 20),
            //        ShowShadow = false,
            //        ShowAllBorders = false,
            //        IsChild = true,
            //    };
            //    filterButton.ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.search.svg";
            //filterButton.ApplyTheme(_theme);
            //Rectangle filterButtonRect = new Rectangle(headerPanelBorderRect.Right - _buttonssize - 5, headerPanelBorderRect.Top+2, _buttonssize, _buttonssize);
            //// filterButton.DrawToGraphics(g, filterButtonRect);
            //// Add click event to the filter button
            //filterButton.Click += FilterButton_Click;
            //Controls.Add(filterButton);

            // Add controls to BeepSimpleGrid
            Controls.Add(titleLabel);
            titleLabel.Theme = Theme;
        }
        private void PaintColumnHeaders(Graphics g, Rectangle drawingBounds)
        {
            // Define the area for column headers within DrawingRect
            int yOffset = drawingBounds.Top;
            int xOffset = drawingBounds.Left + XOffset;

            // Default or user-defined headers
            List<string> defaultHeaders = new List<string> { "Column 1", "Column 2", "Column 3" };
            int columnCount = _columnWidths.Count > 0 ? _columnWidths.Count : defaultHeaders.Count;

            for (int i = 0; i < columnCount; i++)
            {
                string headerText = (beepGridColumns.Count > i && !string.IsNullOrEmpty(beepGridColumns[i].HeaderText))
                    ? beepGridColumns[i].HeaderText
                    : defaultHeaders[i];

                var columnRect = new Rectangle(xOffset, yOffset, _columnWidths[i], _defaultcolumnheaderheight);

                using (var textBrush = new SolidBrush(_theme.PrimaryColor))
                {
                    g.DrawString(headerText, Font, textBrush, columnRect, new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    });
                }

                xOffset += _columnWidths[i];
            }
            // draw line between header and grid
            using (var pen = new Pen(_theme.BorderColor))
            {
                g.DrawLine(pen, drawingBounds.Left, drawingBounds.Top + _defaultcolumnheaderheight, drawingBounds.Right, drawingBounds.Top + _defaultcolumnheaderheight);
            }
        }
        private void PaintCell(Graphics g, BeepGridCell cell, Rectangle cellRect)
        {
            // Draw cell background
            using (var cellBrush = new SolidBrush(_theme.BackColor))
            {
                g.FillRectangle(cellBrush, cellRect);
            }

            // Draw cell text if available
            if (cell.UIComponent != null && cell.UIComponent is BeepLabel cellLabel)
            {
                cellLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                cellLabel.Text = cell.UIComponent.ToString(); // Display the content as text
                cellLabel.DrawToGraphics(g, cellRect);
            }
            else if (cell.UIComponent != null)
            {
                // Draw any other UIComponent content if it's not a BeepLabel
                g.DrawString(cell.UIComponent.ToString(), Font, new SolidBrush(_theme.PrimaryTextColor),
                    cellRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }

            //// Draw cell border
            //using (var borderPen = new Pen(_theme.BorderColor))
            //{
            //    g.DrawRectangle(borderPen, cellRect);
            //}
        }
        private void DrawColumnBorders(Graphics g, Rectangle drawingBounds)
        {

            int xOffset = drawingBounds.Left+XOffset;
            int yOffset = drawingBounds.Top +ColumnHeight;
            using (var pen = new Pen(_theme.BorderColor))
            {
                for (int i = 0; i < _columnWidths.Count-1; i++)
                {
                    xOffset += _columnWidths[i];
                    g.DrawLine(pen, xOffset, yOffset, xOffset, topPanelY );
                }
            }
        }
        private void DrawRowsBorders(Graphics g, Rectangle drawingBounds)
        {
            int yOffset = drawingBounds.Top + ColumnHeight;
            using (var pen = new Pen(_theme.BorderColor))
            {
                for (int i = 0; i < Rows.Count; i++)
                {
                    yOffset += _rowHeight;
                    g.DrawLine(pen, drawingBounds.Left, yOffset, drawingBounds.Right, yOffset);
                }
            }
        }
        #endregion "Drawing on DrawingRectangle"
        private void PaintEmptyRow(Graphics graphics, Rectangle rowRect)
        {
            // Fill row with the default background color (could use a lighter color to indicate it's empty)
            rowRect.X += XOffset; // Apply XOffset to adjust drawing position
            using (var brush = new SolidBrush(BackColor))
            {
                graphics.FillRectangle(brush, rowRect);
            }

            // Draw the empty row borders (no content, just visual rows)
            using (var pen = new Pen(_theme.BorderColor))
            {
                graphics.DrawRectangle(pen, rowRect);
            }
        }
        private void PaintColumnHeaders(Graphics graphics)
        {

            // Default header titles if no columns are defined
            List<string> defaultHeaders = new List<string> { "Column 1", "Column 2", "Column 3" };

            // Use defined column widths or placeholder widths
            int columnCount = _columnWidths.Count > 0 ? _columnWidths.Count : defaultHeaders.Count;
            int xOffset = XOffset;

            for (int i = 0; i < columnCount; i++)
            {
                // If a header is defined, use it; otherwise, use the default header
                string headerText = (beepGridColumns.Count > i && !string.IsNullOrEmpty(beepGridColumns[i].HeaderText))
                                    ? beepGridColumns[i].HeaderText
                                    : defaultHeaders[i];

                var columnRect = new Rectangle(xOffset, headerPanelHeight, _columnWidths[i], _defaultcolumnheaderheight);

                // Draw the header text if it's not empty
                if (!string.IsNullOrEmpty(headerText))
                {
                    using (var brush = new SolidBrush(_theme.PrimaryTextColor))
                    {
                        graphics.DrawString(headerText, Font, brush, columnRect, new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });
                    }

                    // Draw sort icons for the header
                    DrawColumnIcons(graphics, columnRect, i);
                }

                xOffset += _columnWidths[i];
            }
        }
        private void DrawColumnIcons(Graphics graphics, Rectangle columnRect, int columnIndex)
        {
            // Only draw icons if the column has a valid title (i.e., non-empty)
            //if (!string.IsNullOrEmpty(beepGridColumns[columnIndex].HeaderText))
            //{
                // Load SVG icons using BeepImage
                var sortIcon = new BeepImage
                {
                    ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.sort.svg",
                    Size = new Size(10, 10),
                    Location = new Point(columnRect.Left + 2, columnRect.Top + 2), // Top-left for sort icon
                    Theme = _themeEnum // Apply the current theme
                };

                //var filterIcon = new BeepImage
                //{
                //    ImagePath = "TheTechIdea.Beep.Winform.Controls.GFX.SVG.search.svg",
                //    Size = new Size(10, 10),
                //    Location = new Point(columnRect.Right - 12, columnRect.Top + 2), // Top-right for filter icon
                //    Theme = _themeEnum // Apply the current theme
                //};

                // Render the SVG icons using BeepImage's internal draw methods
                sortIcon.DrawImage(graphics, new Rectangle(sortIcon.Location, sortIcon.Size));
               // filterIcon.DrawImage(graphics, new Rectangle(filterIcon.Location, filterIcon.Size));

                // Add clickable regions or events for the icons
                AddIconClickEvents(columnIndex, new Rectangle(sortIcon.Location, sortIcon.Size));
           //}
        }
        private void AddIconClickEvents(int columnIndex, Rectangle sortIconRect)
        {
            // Here you can add logic for detecting clicks on the sort and filter icons.
            // This can be done by adding mouse event handlers that check whether the mouse
            // click occurred inside the icon rectangles.
            this.MouseClick += (sender, e) =>
            {
                if (sortIconRect.Contains(e.Location))
                {
                    OnSortColumn(columnIndex);
                }
                
            };
        }
        private void PaintRow(Graphics graphics, BeepGridRow row, Rectangle rowRect)
        {
            // Ensure we only draw within the grid's DrawingRect area
            int yOffset = DrawingRect.Top+headerPanelHeight ;
            int xOffset = DrawingRect.Left + XOffset;

            // Set the row height based on the default row height or the height of the tallest control in the row
            int maxCellHeight = _rowHeight;

            for (int i = 0; i < row.Cells.Count; i++)
            {
                var cell = row.Cells[i];
                int cellWidth = _columnWidths[i];

                // Define the cell rectangle within the grid's drawing boundaries
                var cellRect = new Rectangle(xOffset, yOffset, cellWidth, _rowHeight);
                PaintCell(graphics, cell, cellRect);

                // Adjust row height based on the cell’s preferred height if it contains a control
                if (cell.UIComponent is Control control)
                {
                    int controlHeight = control.PreferredSize.Height;
                    if (controlHeight > maxCellHeight)
                    {
                        maxCellHeight = controlHeight;
                    }
                }

                // Move to the next cell horizontally
                xOffset += cellWidth;
            }

            // Update row rectangle with the calculated height and draw the row background
            rowRect.Height = maxCellHeight;
            using (var rowBrush = new SolidBrush(BackColor))
            {
                graphics.FillRectangle(rowBrush, rowRect);
            }

            //// Draw row borders if needed (optional)
            //if (ShowAllBorders)
            //{
            //    using (var pen = new Pen(_theme.BorderColor))
            //    {
            //        graphics.DrawRectangle(pen, rowRect);
            //    }
            //}if(ShowBottomBorder && ShowAllBorders==false)
            //{
            //    using (var pen = new Pen(_theme.BorderColor))
            //    {
            //        graphics.DrawLine(pen, rowRect.Left, rowRect.Bottom, rowRect.Right, rowRect.Bottom);
            //    }
            //}
            //if (ShowLeftBorder && ShowAllBorders == false) 
            //{
            //    using (var pen = new Pen(_theme.BorderColor))
            //    {
            //        graphics.DrawLine(pen, rowRect.Left, rowRect.Top, rowRect.Left, rowRect.Bottom);
            //    }

            //}
            //if (ShowRightBorder && ShowAllBorders == false)
            //{
            //    using (var pen = new Pen(_theme.BorderColor))
            //    {
            //        graphics.DrawLine(pen, rowRect.Right, rowRect.Top, rowRect.Right, rowRect.Bottom);
            //    }

            //}
            //if (ShowTopBorder && ShowAllBorders == false)
            //{
            //    using (var pen = new Pen(_theme.BorderColor))
            //    {
            //        graphics.DrawLine(pen, rowRect.Left, rowRect.Top, rowRect.Right, rowRect.Top);
            //    }

            //}
        }
        #endregion "Paint"
        #region Resizing Logic
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //// Recalculate the layout here if necessary
            //AdjustColumnWidths();
            //AdjustRowHeights();

            // Redraw the grid after resize adjustments
            Invalidate();
        }
        private void AdjustColumnWidths()
        {
            int availableWidth = this.Width - XOffset; // Account for any offsets
            int columnCount = _columnWidths.Count;

            if (columnCount > 0)
            {
                int newWidth = availableWidth / columnCount;

                for (int i = 0; i < _columnWidths.Count; i++)
                {
                    _columnWidths[i] = newWidth;
                }
            }
        }
        private void AdjustRowHeights()
        {
            // Default row height for the grid, or a minimum height you want to ensure
            int defaultRowHeight = 30;

            // Adjust each row's height
            foreach (var row in Rows)
            {
                int maxCellHeight = defaultRowHeight;

                // Check each cell in the row to find the tallest content
                foreach (var cell in row.Cells)
                {
                    // Determine the height required for the cell content
                    if (cell.UIComponent is Control control)
                    {
                        // Calculate height based on the control's preferred size
                        int requiredHeight = control.PreferredSize.Height;

                        // Update maxCellHeight if this cell requires more height
                        if (requiredHeight > maxCellHeight)
                        {
                            maxCellHeight = requiredHeight;
                        }
                    }
                    else
                    {
                        // If not a control, measure text size based on Font and add padding
                        using (Graphics g = this.CreateGraphics())
                        {
                            SizeF textSize = g.MeasureString(cell.UIComponent?.ToString() ?? string.Empty, this.Font);
                            int textHeight = (int)textSize.Height + 8; // Adding padding

                            if (textHeight > maxCellHeight)
                            {
                                maxCellHeight = textHeight;
                            }
                        }
                    }
                }

                // Set the row height based on the tallest cell content in this row
                row.Height = maxCellHeight;
            }

            // Redraw the grid to apply the new row heights
            Invalidate();
        }
        private Rectangle CalculateDrawingRectangleForGrid()
        {
            int height = 0;
            // calculate the Rectangle  Area for the grid based on the layout
            // take in accoutn if HeaderPanel is visible and BottomPanel is visible
            // take in account if Navigator is visible
            // remove all from original DrawingRectangle
            // return the new DrawingRectangle
            int heightAdjustment = 0;

            if (ShowHeaderPanel)
                heightAdjustment += headerPanelHeight;

            if (ShowNavigator)
                heightAdjustment += 30; // Assuming 30px height for navigator
            if(ShowFooter)
                heightAdjustment += 30; // Assuming 30px height for footer
            if(ShowFooter)
                heightAdjustment += 30; // Assuming 30px height for footer
            return new Rectangle(0, 0, DrawingRect.Width, DrawingRect.Height - heightAdjustment);


            
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
            if (_resizingColumn)
            {
                int deltaX = e.X - _lastMousePos.X;
                _columnWidths[_resizingIndex] += deltaX;
                _lastMousePos = e.Location;
                Invalidate();
            }
            else if (_resizingRow)
            {
                int deltaY = e.Y - _lastMousePos.Y;
                _rowHeight += deltaY;
                _lastMousePos = e.Location;
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
        }

        private bool IsNearColumnBorder(Point location, out int columnIndex)
        {
            int xOffset = 0;
            for (int i = 0; i < _columnWidths.Count; i++)
            {
                xOffset += _columnWidths[i];
                if (Math.Abs(location.X - xOffset) <= _resizeMargin)
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
            int yOffset = _rowHeight;
            for (int i = 0; i < Rows.Count; i++)
            {
                if (Math.Abs(location.Y - yOffset) <= _resizeMargin)
                {
                    rowIndex = i;
                    return true;
                }
                yOffset += _rowHeight;
            }
            rowIndex = -1;
            return false;
        }
        #endregion Resizing Logic
        #region "Events"
        public event EventHandler<BeepGridRowEventArgs> RowClick;
        public event EventHandler<BeepGridCellEventArgs> CellClick;

        protected virtual void OnRowClick(BeepGridRow row)
        {
            RowClick?.Invoke(this, new BeepGridRowEventArgs(row));
        }

        protected virtual void OnCellClick(BeepGridCell cell)
        {
            CellClick?.Invoke(this, new BeepGridCellEventArgs(cell));
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Detect column icon clicks
            for (int i = 0; i < sortIconBounds.Count; i++)
            {
                if (sortIconBounds[i].Contains(e.Location))
                {
                    OnSortColumn(i);
                    return;
                }
            }

            // Check if filter button was clicked
            if (filterButtonRect.Contains(e.Location))
            {
                ShowFilterForm();
                return;
            }

            // Detect row and cell clicks
            int yOffset = _defaultcolumnheaderheight + headerPanelHeight + 2;
            for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
            {
                var rowRect = new Rectangle(0, yOffset, Width, _rowHeight);
                if (rowRect.Contains(e.Location))
                {
                    var row = Rows[rowIndex];
                    OnRowClick(row);

                    int xOffset = 0;
                    for (int colIndex = 0; colIndex < row.Cells.Count; colIndex++)
                    {
                        var cellRect = new Rectangle(xOffset, yOffset, _columnWidths[colIndex], _rowHeight);
                        if (cellRect.Contains(e.Location))
                        {
                            OnCellClick(row.Cells[colIndex]);
                            return;
                        }
                        xOffset += _columnWidths[colIndex];
                    }
                }
                yOffset += _rowHeight;
            }
        }


        #endregion "Events"
        #region "Sort and Filter"

        private void ShowFilterForm()
        {
            using (Form filterForm = new Form())
            {
                filterForm.Text = "Filter Conditions";
                filterForm.Size = new Size(400, 300);
                filterForm.StartPosition = FormStartPosition.CenterParent;

                Label conditionLabel = new Label { Text = "Enter Condition:", Left = 20, Top = 20, Width = 350 };
                TextBox conditionBox = new TextBox { Left = 20, Top = 50, Width = 350 };

                Button applyButton = new Button { Text = "Apply Filter", Left = 20, Top = 100, Width = 100 };
                applyButton.Click += (s, args) => { ApplyFilter(conditionBox.Text); filterForm.Close(); };

                filterForm.Controls.Add(conditionLabel);
                filterForm.Controls.Add(conditionBox);
                filterForm.Controls.Add(applyButton);

                filterForm.ShowDialog();
            }
        }

        private void ApplyFilter(string condition)
        {
            // Apply the filter condition to BeepSimpleGrid data
            var filteredRows = Rows.Where(row => row.Cells.Any(cell => cell.UIComponent?.ToString().Contains(condition, StringComparison.OrdinalIgnoreCase) == true)).ToList();

            Rows.Clear();
            foreach (var row in filteredRows)
            {
                Rows.Add(row);
            }

            Invalidate(); // Redraw grid to reflect filtered data
        }
        protected virtual void OnFilterColumn(int columnIndex)
        {
            // Prompt the user to input a filter value (you can customize this dialog as needed)
            string filterValue = ShowFilterDialog(columnIndex);

            if (!string.IsNullOrEmpty(filterValue))
            {
                // Apply the filter to the rows based on the entered filter value
                var filteredRows = Rows.Where(row => row.Cells[columnIndex].UIComponent?.ToString().Contains(filterValue, StringComparison.OrdinalIgnoreCase) == true).ToList();

                // Clear the existing rows and add the filtered rows
                Rows.Clear();
                foreach (var row in filteredRows)
                {
                    Rows.Add(row);
                }

                // Redraw the grid to reflect the filtered results
                Invalidate();
            }
        }

        private string ShowFilterDialog(int columnIndex)
        {
            // Create a simple input dialog to get the filter value from the user
            using (Form filterDialog = new Form())
            {
                filterDialog.Text = $"Filter Column {columnIndex + 1}";
                filterDialog.Size = new Size(300, 150);
                filterDialog.StartPosition = FormStartPosition.CenterParent;

                Label promptLabel = new Label() { Left = 20, Top = 20, Text = $"Enter filter for column {columnIndex + 1}:", Width = 250 };
                TextBox inputBox = new TextBox() { Left = 20, Top = 50, Width = 250 };
                Button okButton = new Button() { Text = "OK", Left = 20, Width = 100, Top = 80, DialogResult = System.Windows.Forms.DialogResult.OK };
                Button cancelButton = new Button() { Text = "Cancel", Left = 130, Width = 100, Top = 80, DialogResult = System.Windows.Forms.DialogResult.Cancel };

                filterDialog.Controls.Add(promptLabel);
                filterDialog.Controls.Add(inputBox);
                filterDialog.Controls.Add(okButton);
                filterDialog.Controls.Add(cancelButton);

                filterDialog.AcceptButton = okButton;
                filterDialog.CancelButton = cancelButton;

                if (filterDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return inputBox.Text;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        protected virtual void OnSortColumn(int columnIndex)
        {
            // Sort Rows based on the value in the specified columnIndex
            var sortedRows = Rows.OrderBy(row => row.Cells[columnIndex].UIComponent?.ToString()).ToList();

            // Update the Rows with sorted data
            Rows.Clear();
            foreach (var row in sortedRows)
            {
                Rows.Add(row);
            }

            Invalidate(); // Repaint the grid after sorting
        }

        #endregion "Sort and Filter"
        #region "Dispose"
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var control in Controls)
                {
                    if (control is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
            base.Dispose(disposing);
        }

        #endregion "Dispose"
        #region "Layout Load and Save"
        /// <summary>
        /// Generates a unique GridId based on the current timestamp or GUID.
        /// This is used to uniquely identify each grid dropped in the form.
        /// </summary>
        private void GenerateUniqueGridId()
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                // Ensure unique ID is generated at design time
                _gridId = "BeepGrid_" + Guid.NewGuid().ToString("N");
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyTheme();
            // Ensure consistent GridId at runtime and design time
            if (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                LoadGridLayout();  // Load layout specific to this grid based on GridId
            }
        }
        public void LoadGridLayout()
        {
            string layoutFilePath = $"{GridId}_layout.json";
            if (File.Exists(layoutFilePath))
            {
                LoadColumnLayoutFromFile(layoutFilePath);  // Load layout if file exists
            }
        }

        public void SaveGridLayout()
        {
            string layoutFilePath = $"{GridId}_layout.json";
            SaveColumnLayoutToFile(layoutFilePath);  // Save layout to file
        }
        public void ApplyColumnConfigurations()
        {
            this.beepGridColumns.Clear(); // Clear existing columns before applying the configuration

            foreach (var config in beepGridColumns)
            {
                DataGridViewColumn column = null;

                // Handle different types of columns
                switch (config.ColumnType)
                {
                    case "BeepDataGridViewComboBoxColumn":
                        column = new BeepDataGridViewComboBoxColumn
                        {
                            Name = config.Name,
                            HeaderText = config.HeaderText,
                            Width = config.Width,
                            DataSourceName = config.DataSourceName,
                            DisplayMember = config.DisplayMember,
                            ValueMember = config.ValueMember
                        };

                        // Check for custom cascading map
                        if (column is BeepDataGridViewComboBoxColumn comboBoxColumn && config.CascadingMap != null)
                        {
                            comboBoxColumn.CascadingMap = config.CascadingMap;
                        }
                        break;

                    case "BeepDataGridViewNumericColumn":
                        column = new BeepDataGridViewNumericColumn
                        {
                            Name = config.Name,
                            HeaderText = config.HeaderText,
                            Width = config.Width,
                            // You can add more numeric-specific properties here if needed
                        };
                        break;

                    case "BeepDataGridViewProgressBarColumn":
                        column = new BeepDataGridViewProgressBarColumn
                        {
                            Name = config.Name,
                            HeaderText = config.HeaderText,
                            Width = config.Width,
                            ProgressBarColor = config.ProgressBarColor
                        };
                        break;

                    case "BeepDataGridViewSvgColumn":
                        column = new BeepDataGridViewSvgColumn
                        {
                            Name = config.Name,
                            HeaderText = config.HeaderText,
                            Width = config.Width,
                        };
                        break;

                    case "BeepDataGridViewThreeStateCheckBoxColumn":
                        column = new BeepDataGridViewThreeStateCheckBoxColumn
                        {
                            Name = config.Name,
                            HeaderText = config.HeaderText,
                            Width = config.Width
                        };
                        break;

                    case "BeepDataGridViewSliderColumn":
                        column = new BeepDataGridViewSliderColumn
                        {
                            Name = config.Name,
                            HeaderText = config.HeaderText,
                            Width = config.Width,
                        };
                        break;

                    case "BeepDataGridViewRatingColumn":
                        column = new BeepDataGridViewRatingColumn
                        {
                            Name = config.Name,
                            HeaderText = config.HeaderText,
                            Width = config.Width,
                            FilledStarColor = config.FilledStarColor,
                            EmptyStarColor = config.EmptyStarColor,
                            MaxStars = config.MaxStars
                        };
                        break;

                    case "BeepDataGridViewSwitchColumn":
                        column = new BeepDataGridViewSwitchColumn
                        {
                            Name = config.Name,
                            HeaderText = config.HeaderText,
                            Width = config.Width,
                        };
                        break;

                    case "BeepDataGridViewMultiColumnColumn":
                        column = new BeepDataGridViewMultiColumnColumn
                        {
                            Name = config.Name,
                            HeaderText = config.HeaderText,
                            Width = config.Width,
                        };
                        break;

                    case "BeepDataGridViewImageComboBoxColumn":
                        column = new BeepDataGridViewImageComboBoxColumn
                        {
                            Name = config.Name,
                            HeaderText = config.HeaderText,
                            Width = config.Width,
                        };
                        break;

                    default:
                        // Fallback to DataGridViewTextBoxColumn if type is not found
                        column = new DataGridViewTextBoxColumn
                        {
                            Name = config.Name,
                            HeaderText = config.HeaderText,
                            Width = config.Width
                        };
                        break;
                }

                if (column != null)
                {
                   // this.beepGridColumns.Add(column); // Add the column to the grid
                }
            }
        }

        public void SaveColumnLayoutToFile(string filePath)
        {
            var json = JsonConvert.SerializeObject(beepGridColumns, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        public void LoadColumnLayoutFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                beepGridColumns = JsonConvert.DeserializeObject<BindingList<BeepGridColumnConfig>>(json);
                ApplyColumnConfigurations(); // Rebuild the grid with loaded columns
            }
        }
        #endregion "Layout Load and Save"
    }

    // Represents a single row in the grid
    public class BeepGridRow
    {
        public BeepGridRow()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public int Index { get; set; }
        public int DisplayIndex { get; set; }
        public BindingList<BeepGridCell> Cells { get; set; } = new BindingList<BeepGridCell>();
        private string _columnname; // used for to store the display header of column
      
        private string _rowname; // used for to store the display header  of row
        public string RowName
        {
            get { return _rowname; }
            set { _rowname = value; }
        }
        private string _rowNameField; // used for store the name of field that has value to display as row head
        public string RowNameField
        {
            get { return string.IsNullOrEmpty(_rowNameField) ? RowName : _rowNameField; ; }
            set { _rowNameField = value; }
        }
        public int Height { get; set; } = 30; // Default row height

        // Row Events
        public event EventHandler<BeepGridRowEventArgs> RowClick;
        public event EventHandler<BeepGridRowEventArgs> RowValidate;
        public event EventHandler<BeepGridRowEventArgs> RowDelete;
        public event EventHandler<BeepGridRowEventArgs> RowAdd;
        public event EventHandler<BeepGridRowEventArgs> RowUpdate;

        public void ApplyTheme(BeepTheme theme)
        {
            foreach (var cell in Cells)
            {
                cell.ApplyTheme(theme);
            }
        }
    }

    // Represents a single cell in the grid
    public class BeepGridCell
    {
        public BeepGridCell()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public int Index { get; set; }
        public int DisplayIndex { get; set; }
        public IBeepUIComponent UIComponent { get; set; }

        public void ApplyTheme(BeepTheme theme)
        {
            if (UIComponent != null)
            {
                UIComponent.ApplyTheme(theme);
            }
        }
    }

    // Custom event args for row events
    public class BeepGridRowEventArgs : EventArgs
    {
        public BeepGridRow Row { get; }
        public BeepGridRowEventArgs(BeepGridRow row) => Row = row;
    }

    // Custom event args for cell events
    public class BeepGridCellEventArgs : EventArgs
    {
        public BeepGridCell Cell { get; }
        public BeepGridCellEventArgs(BeepGridCell cell) => Cell = cell;
    }

    public enum GridDataSourceType
    {
        Fixed,
        BindingSource,
        IDataSource
    }
}